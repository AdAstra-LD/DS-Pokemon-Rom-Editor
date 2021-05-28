using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Linq;
using DSPRE.ROMFiles;
using System.Collections.Generic;
using DSPRE.Resources.ROMToolboxDB;
using DSPRE.Resources;
using static DSPRE.RomInfo;

namespace DSPRE {
    public partial class ROMToolboxDialog : Form {
        internal class ARM9PatchData {
            internal string initString;
            internal string branchString;

            internal uint branchOffset;
            internal uint initOffset;

            internal ARM9PatchData() {
                branchOffset = ToolboxDB.arm9ExpansionOffsetsDB[nameof(branchOffset) + "_" + RomInfo.gameFamily] - 0x02000000;
                initOffset = ToolboxDB.arm9ExpansionOffsetsDB[nameof(initOffset) + "_" + RomInfo.gameFamily + "_" + RomInfo.gameLanguage] - 0x02000000;
                branchString = ToolboxDB.arm9ExpansionCodeDB[nameof(branchString) + "_" + RomInfo.gameFamily + "_" + RomInfo.gameLanguage];

                if (RomInfo.gameFamily == "Plat" ) {
                    initString = ToolboxDB.arm9ExpansionCodeDB[nameof(initString) + "_" + RomInfo.gameFamily + "_" + RomInfo.gameLanguage];
                } else {
                    initString = ToolboxDB.arm9ExpansionCodeDB[nameof(initString) + "_" + RomInfo.gameFamily];
                }
            }
        }
        internal class BDHCAMPatchData {
            internal byte overlayNumber;

            internal uint branchOffset;
            internal string branchString;

            internal uint overlayOffset1;
            internal uint overlayOffset2;

            internal string overlayString1;
            internal string overlayString2;

            internal byte[] subroutine;

            internal BDHCAMPatchData () {
                switch (RomInfo.gameFamily) {
                    case "Plat":
                        overlayNumber = 5;
                        branchString = ToolboxDB.BDHCamCodeDB[nameof(branchString) + "_" + RomInfo.gameFamily + "_" + RomInfo.gameLanguage];
                        
                        branchOffset = ToolboxDB.BDHCamOffsetsDB[nameof(branchOffset) + "_" + RomInfo.gameFamily + "_" + RomInfo.gameLanguage];
                        overlayOffset1 = ToolboxDB.BDHCamOffsetsDB[nameof(overlayOffset1) + "_" + RomInfo.gameFamily + "_" + RomInfo.gameLanguage];
                        overlayOffset2 = ToolboxDB.BDHCamOffsetsDB[nameof(overlayOffset2) + "_" + RomInfo.gameFamily + "_" + RomInfo.gameLanguage];
                        break;
                    case "HGSS":
                        overlayNumber = 1;
                        branchString = ToolboxDB.BDHCamCodeDB[nameof(branchString) + "_" + RomInfo.gameFamily];

                        branchOffset = ToolboxDB.BDHCamOffsetsDB[nameof(branchOffset) + "_" + RomInfo.gameFamily];
                        overlayOffset1 = ToolboxDB.BDHCamOffsetsDB[nameof(overlayOffset1) + "_" + RomInfo.gameFamily];
                        overlayOffset2 = ToolboxDB.BDHCamOffsetsDB[nameof(overlayOffset2) + "_" + RomInfo.gameFamily];
                        break;
                }
                branchOffset -= 0x02000000;
                overlayString1 = ToolboxDB.BDHCamCodeDB[nameof(overlayString1)];
                overlayString2 = ToolboxDB.BDHCamCodeDB[nameof(overlayString2)];
                subroutine = (byte[])new ResourceManager("DSPRE.Resources.ROMToolboxDB.BDHCAMPatchDB", Assembly.GetExecutingAssembly()).GetObject(RomInfo.romID + "_cam");
            }
        }
        internal class DynamicHeadersPatchData
        {
            internal uint initOffset;
            internal string initString;
            internal string REFERENCE_STRING = "19 00 C0 46";
            internal int pointerDiff;

            internal DynamicHeadersPatchData()
            {
                initOffset = ToolboxDB.getDynamicHeadersInitOffset(RomInfo.romID);
                initString = ToolboxDB.getDynamicHeadersInitString(RomInfo.romID);

                if (RomInfo.gameFamily == "HGSS")
                {
                    pointerDiff = (int)(initOffset - ToolboxDB.getDynamicHeadersInitOffset("IPKE"));
                }
                else
                {
                    pointerDiff = (int)(initOffset - ToolboxDB.getDynamicHeadersInitOffset("CPUE"));
                }
            }
        }
        public uint expandedARMfileID = ToolboxDB.syntheticOverlayFileNumbersDB[RomInfo.gameFamily];
        public static bool flag_standardizedItems { get; private set; } = false;
        public static bool flag_arm9Expanded { get; private set; } = false;
        public static bool flag_BDHCamPatchApplied { get; private set; } = false;
        public static bool flag_DynamicHeadersPatchApplied { get; private set; } = false;
        public static bool flag_MatrixExpansionApplied { get; private set; } = false;
        public static bool overlay1MustBeRestoredFromBackup { get; private set; } = true;

        #region Constructor
        public ROMToolboxDialog() {
            InitializeComponent();
            
            CheckStandardizedItems();
            
            if (RomInfo.gameLanguage == "ENG" || RomInfo.gameLanguage == "ESP") {
                CheckARM9Expansion();
            } else {
                DisableARM9patch("Unsupported\nlanguage");
                DisableBDHCamPatch("Unsupported\nlanguage");
                DisableScrcmdRepointPatch("Unsupported\nlanguage");
            }

            switch (RomInfo.gameFamily) {
                case "DP":
                    DisableOverlay1patch("Unsupported");
                    DisableDynamicHeadersPatch("Unsupported");
                    DisableMatrixExpansionPatch("Unsupported");
                    DisableScrcmdRepointPatch("Unsupported");
                    break;
                case "Plat":
                    DisableOverlay1patch("Unsupported");
                    DisableMatrixExpansionPatch("Unsupported");
                    DisableScrcmdRepointPatch("Unsupported");
                    CheckFilesBDHCamPatchApplied();
                    CheckDynamicHeadersPatchApplied();
                    break;
                case "HGSS":
                    if (!DSUtils.CheckOverlayHasCompressionFlag(1)) {
                        DisableOverlay1patch("Already applied");
                        overlay1CB.Visible = true;
                    }

                    CheckFilesBDHCamPatchApplied();

                    if (RomInfo.gameLanguage == "ENG" || RomInfo.gameLanguage == "ESP") {
                        CheckMatrixPatchApplied();
                        CheckScrcmdRepointPatchApplied();
                    } else {
                        DisableMatrixExpansionPatch("Unsupported\nlanguage");
                        DisableScrcmdRepointPatch("Unsupported\nlanguage");
                    }

                    CheckDynamicHeadersPatchApplied();
                    break;
            }
        }

        #region Patch Disable
        private void DisableOverlay1patch(string reason) {
            overlay1uncomprButton.Enabled = false;
            overlay1uncompressedLBL.Enabled = false;
            overlay1patchtextLBL.Enabled = false;
            overlay1uncomprButton.Text = reason;
        }
        private void DisableBDHCamPatch(string reason) {
            BDHCamPatchButton.Enabled = false;
            BDHCamPatchLBL.Enabled = false;
            BDHCamPatchTextLBL.Enabled = false;
            BDHCamARM9requiredLBL.Enabled = false;
            BDHCamPatchButton.Text = reason;
        }
        private void DisableARM9patch(string reason) {
            applyARM9ExpansionButton.Enabled = false;
            arm9expansionTextLBL.Enabled = false;
            arm9expansionLBL.Enabled = false;
            applyARM9ExpansionButton.Text = reason;
        }
        private void DisableDynamicHeadersPatch(string reason)
        {
            applyDynamicHeadersButton.Enabled = false;
            dynamicHeadersTextLBL.Enabled = false;
            dynamicHeadersLBL.Enabled = false;
            applyDynamicHeadersButton.Text = reason;
        }
        private void DisableMatrixExpansionPatch(string reason) {
            expandMatrixButton.Enabled = false;
            matrixExpansionLBL.Enabled = false;
            matrixExpansionTextLBL.Enabled = false;
            expandMatrixButton.Text = reason;
        }
        private void DisableStandardizeItemsPatch(string reason) {
            applyItemStandardizeButton.Enabled = false;
            standardizePatchLBL.Enabled = false;
            standardizePatchTextLBL.Enabled = false;
            applyItemStandardizeButton.Text = reason;
        }
        private void DisableScrcmdRepointPatch(string reason) {
            repointScrcmdButton.Enabled = false;
            repointScrcmdLBL.Enabled = false;
            repointScrcmdTextLBL.Enabled = false;
            scrcmdARM9requiredLBL.Enabled = false;
            repointScrcmdButton.Text = reason;
        }
        #endregion
        #endregion

        #region Patch Checkers
        private int CheckARM9Expansion() {
            if (!flag_arm9Expanded) {
                ARM9PatchData data = new ARM9PatchData();

                try { 
                    byte[] branchCode = HexStringToByteArray(data.branchString);
                    byte[] branchCodeRead = DSUtils.ReadFromArm9(data.branchOffset, data.branchString.Length / 3 + 1); //Read branchCode
                    if (branchCodeRead.Length != branchCode.Length)
                        return 0; //0 means ARM9 Expansion has never been applied
                    if (!branchCodeRead.SequenceEqual(branchCode))
                        return 0;

                    byte[] initCode = HexStringToByteArray(data.initString);
                    byte[] initCodeRead = DSUtils.ReadFromArm9(data.initOffset, data.initString.Length / 3 + 1); //Read initCode
                    if (initCodeRead.Length != initCode.Length)
                        return 0;
                    if (!initCodeRead.SequenceEqual(initCode))
                        return 0;
                } catch {
                    return -1; //-1 means Check failure
                }
            }

            flag_arm9Expanded = true;
            arm9patchCB.Visible = true;
            DisableARM9patch("Already applied");

            switch (RomInfo.gameFamily) {
                case "Plat":
                case "HGSS":
                    BDHCamARM9requiredLBL.Visible = false;
                    BDHCamPatchButton.Enabled = true;
                    BDHCamPatchLBL.Enabled = true;
                    BDHCamPatchTextLBL.Enabled = true;
                    break;
            }
            return 1; //arm9 Expansion has already been applied
        }
        private int CheckDynamicHeadersPatchApplied()
        {
            if (!flag_DynamicHeadersPatchApplied)
            {
                if (!CheckDynamicHeaders()) return 0;
            }

            flag_DynamicHeadersPatchApplied = true;
            dynamicHeadersPatchCB.Visible = true;

            DisableDynamicHeadersPatch("Already applied");
            return 1;
        }
        public bool CheckDynamicHeaders()
        {
            DynamicHeadersPatchData data = new DynamicHeadersPatchData();
            ushort initValue = BitConverter.ToUInt16(DSUtils.ReadFromArm9(data.initOffset, 0x2), 0);

            if (initValue == 0xB500) return true;
            else return false;
        }
        private int CheckFilesBDHCamPatchApplied() {
            if (!flag_arm9Expanded) { 
                BDHCamARM9requiredLBL.Visible = true;
                DisableBDHCamPatch("!");
                return 0;
            }

            if (!flag_BDHCamPatchApplied) {
                BDHCAMPatchData data = new BDHCAMPatchData();
                try {
                    byte[] branchCode = HexStringToByteArray(data.branchString);
                    byte[] branchCodeRead = DSUtils.ReadFromArm9(data.branchOffset, branchCode.Length);

                    if (branchCode.Length != branchCodeRead.Length)
                        return 0; //0 means BDHCAM patch has not been applied
                    if (!branchCode.SequenceEqual(branchCodeRead))
                        return 0;


                    string overlayFilePath = RomInfo.workDir + "overlay" + "\\" + "overlay_" + data.overlayNumber.ToString("D4") + ".bin";
                    DSUtils.DecompressOverlay(data.overlayNumber);

                    byte[] overlayCode1 = HexStringToByteArray(data.overlayString1);
                    byte[] overlayCode1Read = DSUtils.ReadFromFile(overlayFilePath, data.overlayOffset1, overlayCode1.Length);
                    if (overlayCode1.Length != overlayCode1Read.Length)
                        return 0; //0 means BDHCAM patch has not been applied
                    if (!overlayCode1.SequenceEqual(overlayCode1Read))
                        return 0;


                    byte[] overlayCode2 = HexStringToByteArray(data.overlayString2);
                    byte[] overlayCode2Read = DSUtils.ReadFromFile(overlayFilePath, data.overlayOffset2, overlayCode2.Length); //Write new overlayCode1
                    if (overlayCode2.Length != overlayCode2Read.Length)
                        return 0; //0 means BDHCAM patch has not been applied
                    if (!overlayCode2.SequenceEqual(overlayCode2Read))
                        return 0;

                    String fullFilePath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + expandedARMfileID.ToString("D4");
                    byte[] subroutineRead = DSUtils.ReadFromFile(fullFilePath, ToolboxDB.BDHCamSubroutineOffset, data.subroutine.Length); //Write new overlayCode1
                    if (data.subroutine.Length != subroutineRead.Length)
                        return 0; //0 means BDHCAM patch has not been applied
                    if (!data.subroutine.SequenceEqual(subroutineRead))
                        return 0;
                } catch {
                    return -1;
                }
            }
            flag_BDHCamPatchApplied = true;
            BDHCamCB.Visible = true;

            DisableBDHCamPatch("Already applied");
            return 0;
        }
        public bool CheckStandardizedItems() {
            DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames> { RomInfo.DirNames.scripts });

            if ( flag_standardizedItems || MainProgram.ScanScriptsCheckStandardizedItemNumbers() ) {
                itemNumbersCB.Visible = true;
                flag_standardizedItems = true;

                DisableStandardizeItemsPatch("Already applied");
                return true;
            }
            return false;
        }
        private int CheckMatrixPatchApplied() {
            if (!flag_MatrixExpansionApplied) {
                try {
                    foreach (KeyValuePair<uint[], string> kv in ToolboxDB.matrixExpansionDB) {
                        foreach (uint offset in kv.Key) {
                            int languageOffset = 0;
                            if (RomInfo.romID == "IPKE" || RomInfo.romID == "IPGE" || RomInfo.romID == "IPGS")
                                languageOffset = +8;

                            byte[] read = DSUtils.ReadFromArm9((uint)(offset - 0x02000000 + languageOffset), kv.Value.Length / 3 + 1);
                            byte[] code = HexStringToByteArray(kv.Value); 
                            if (read.Length != code.Length)
                                return 0;
                            if (!read.SequenceEqual(code))
                                return 0;
                        }
                    }
                } catch {
                    return -1; //-1 means Check failure
                }
            }

            DisableMatrixExpansionPatch("Already applied");
            flag_MatrixExpansionApplied = true;
            expandedMatrixCB.Visible = true;
            return 1; //arm9 Expansion has already been applied
        }

        private void CheckScrcmdRepointPatchApplied() {
            //throw new NotImplementedException();
        }
        #endregion

        #region Button Actions
        private void SentenceCasePatchButton_Click(object sender, EventArgs e) {
            DialogResult d;
            d = MessageBox.Show("Confirming this process will apply the following changes:\n\n" +
                "- Every Pokémon name will be converted to Sentence Case." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes) {
                foreach (int ID in RomInfo.pokemonNamesTextNumbers) {
                    TextArchive pokeName = new TextArchive(ID);
                    for(ushort i = 1; i < pokeName.messages.Count; i++) {
                        if (pokeName.messages[i].Length <= 1)
                            i++;

                        pokeName.messages[i] = pokeName.messages[i].Replace(PokeDatabase.System.pokeNames[i].ToUpper(), PokeDatabase.System.pokeNames[i]);
                    }
                    pokeName.SaveToFileDefaultDir(ID, showSuccessMessage: false);
                }
                //sentenceCaseCB.Visible = true;
                MessageBox.Show("Pokémon names have been converted to Sentence Case.", "Operation successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void BDHCAMPatchButton_Click(object sender, EventArgs e) {
            BDHCAMPatchData data = new BDHCAMPatchData();

            if (RomInfo.gameFamily == "HGSS") {
                if (DSUtils.CheckOverlayHasCompressionFlag(data.overlayNumber)) { 
                    DialogResult d1;
                    d1 = MessageBox.Show("It is STRONGLY recommended to configure Overlay1 as uncompressed before proceeding.\n\n" +
                        "More details in the following dialog.\n\n" + "Do you want to know more?",
                        "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (d1 == DialogResult.Yes) {
                        overlay1uncomprButton_Click(null, null);
                    }
                }
            }

            DialogResult d2;
            d2 = MessageBox.Show("This process will apply the following changes:\n\n" +
                "- Backup ARM9 file (arm9.bin.backup will be created)." + "\n\n" +
                "- Backup Overlay" + data.overlayNumber + " file (overlay" + data.overlayNumber + ".bin.backup will be created)." + "\n\n" +
                "- Replace " + (data.branchString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + data.branchOffset.ToString("X") + " with " + '\n' + data.branchString + "\n\n" +
                "- Replace " + (data.overlayString1.Length / 3 + 1) + " bytes of data at overlay" + data.overlayNumber + " offset 0x" + data.overlayOffset1.ToString("X") + " with " + '\n' + data.overlayString1 + "\n\n" +
                "- Replace " + (data.overlayString2.Length / 3 + 1) + " bytes of data at overlay" + data.overlayNumber + " offset 0x" + data.overlayOffset2.ToString("X") + " with " + '\n' + data.overlayString2 + "\n\n" +
                "- Modify file #" + expandedARMfileID + " inside " + '\n' + RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\n' + "to insert the BDHCAM routine (any data between 0x" + ToolboxDB.BDHCamSubroutineOffset.ToString("X") + " and 0x" + (ToolboxDB.BDHCamSubroutineOffset + data.subroutine.Length).ToString("X") + " will be overwritten)." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d2 == DialogResult.Yes) {
                File.Copy(RomInfo.arm9Path, RomInfo.arm9Path + ".backup", overwrite: true);

                try {
                    DSUtils.WriteToArm9(HexStringToByteArray(data.branchString), data.branchOffset); //Write new branchOffset

                    /* Write to overlayfile */
                    string overlayFilePath = RomInfo.workDir + "overlay" + "\\" + "overlay_" + data.overlayNumber.ToString("D4") + ".bin";
                    if (DSUtils.OverlayIsCompressed(data.overlayNumber)) {
                        DSUtils.DecompressOverlay(data.overlayNumber);
                    }

                    DSUtils.WriteToFile(overlayFilePath, HexStringToByteArray(data.overlayString1), data.overlayOffset1); //Write new overlayCode1
                    DSUtils.WriteToFile(overlayFilePath, HexStringToByteArray(data.overlayString2), data.overlayOffset2); //Write new overlayCode2
                    overlay1MustBeRestoredFromBackup = false;

                    String fullFilePath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + expandedARMfileID.ToString("D4");

                    /*Write Expanded ARM9 File*/
                    DSUtils.WriteToFile(fullFilePath, data.subroutine, ToolboxDB.BDHCamSubroutineOffset);
                } catch {
                    MessageBox.Show("Operation failed. It is strongly advised that you restore the arm9 and overlay from their respective backups.", "Something went wrong",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                overlay1MustBeRestoredFromBackup = false;
                DisableBDHCamPatch("Already applied");
                flag_BDHCamPatchApplied = true;
                BDHCamCB.Visible = true;

                MessageBox.Show("The BDHCAM patch has been applied.", "Operation successful.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void overlay1uncomprButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("This process will apply the following changes:\n\n" +
                "- Overlay 1 will be decompressed.\n\n" +
                "- Overlay 1 will be configured as \"uncompressed\" in the overlay table.\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes) {
                DSUtils.SetOverlayCompressionInTable(1, 0);
                if (DSUtils.OverlayIsCompressed(1)) {
                    DSUtils.DecompressOverlay(1);
                }

                DisableOverlay1patch("Already applied");
                overlay1CB.Visible = true;
                MessageBox.Show("Overlay1 is now configured as uncompressed.", "Operation successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ApplyItemStandardizeButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("This process will apply the following changes:\n\n" +
                "- Item scripts will be rearranged to follow the natural, ascending index order.\n\n" +
                "- Consequently, every Item event already on the ground will be changed.",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes) {

                DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames> { RomInfo.DirNames.scripts });

                if (flag_standardizedItems) {
                    AlreadyApplied();
                } else {
                    ScriptFile itemScript = new ScriptFile(RomInfo.itemScriptFileNumber);

                    int itemCount = new TextArchive(RomInfo.itemNamesTextNumber).messages.Count;
                    CommandContainer executeGive = new CommandContainer((uint)itemCount, itemScript.allScripts[itemScript.allScripts.Count - 1]);
                    itemScript.allScripts.Clear();

                    for (ushort i = 0; i < itemCount; i++) {
                        List<ScriptCommand> cmdList = new List<ScriptCommand>();
                        cmdList.Add(new ScriptCommand("SetVar 0x8008 " + i));
                        cmdList.Add(new ScriptCommand("SetVar 0x8009 0x1"));
                        cmdList.Add(new ScriptCommand("Jump Function_#1"));

                        itemScript.allScripts.Add(new CommandContainer((ushort)(i + 1), ScriptFile.containerTypes.SCRIPT, commandList: cmdList));
                    }
                    itemScript.allScripts.Add(executeGive);
                    //itemScript.allFunctions[1].commands[0].cmdParams[]

                    itemScript.SaveToFileDefaultDir(RomInfo.itemScriptFileNumber, showSuccessMessage: false);
                    MessageBox.Show("Operation successful.", "Process completed.", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DisableStandardizeItemsPatch("Already applied");
                    itemNumbersCB.Visible = true;
                    flag_standardizedItems = true;
                }
            } else {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ApplyARM9ExpansionButton_Click(object sender, EventArgs e) {
            ARM9PatchData data = new ARM9PatchData();

            DialogResult d;
            d = MessageBox.Show("Confirming this process will apply the following changes:\n\n" +
                "- Backup ARM9 file (arm9.bin.backup will be created)." + "\n\n" +
                "- Replace " + (data.branchString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + data.branchOffset.ToString("X") + " with " + '\n' + data.branchString + "\n\n" +
                "- Replace " + (data.initString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + data.initOffset.ToString("X") + " with " + '\n' + data.initString + "\n\n" +
                "- Modify file #" + expandedARMfileID + " inside " + '\n' + RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\n' + " to accommodate for 88KB of data (no backup)." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes) {
                File.Copy(RomInfo.arm9Path, RomInfo.arm9Path + ".backup", overwrite: true);

                try {
                    DSUtils.WriteToArm9(HexStringToByteArray(data.branchString), data.branchOffset); //Write new branchOffset
                    DSUtils.WriteToArm9(HexStringToByteArray(data.initString), data.initOffset); //Write new initOffset

                    string fullFilePath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + expandedARMfileID.ToString("D4");
                    File.Delete(fullFilePath);
                    using (BinaryWriter f = new BinaryWriter(File.Create(fullFilePath))) {
                        for (int i = 0; i < 0x16000; i++)
                            f.Write((byte)0x00); // Write Expanded ARM9 File 
                    }

                    DisableARM9patch("Already applied");
                    arm9patchCB.Visible = true;
                    flag_arm9Expanded = true;

                    switch (RomInfo.gameFamily) {
                        case "Plat":
                        case "HGSS":
                            BDHCamPatchButton.Text = "Apply Patch";
                            BDHCamPatchButton.Enabled = true;
                            BDHCamPatchLBL.Enabled = true;
                            BDHCamPatchTextLBL.Enabled = true;
                            BDHCamARM9requiredLBL.Visible = false;
                            break;
                    }

                    MessageBox.Show("The ARM9's usable memory has been expanded.", "Operation successful.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } catch {
                    MessageBox.Show("Operation failed. It is strongly advised that you restore the arm9 backup (arm9.bin.backup).", "Something went wrong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void expandMatrixButton_Click(object sender, EventArgs e) {
            string listOfChanges = "";
            int languageOffset = 0;

            if (RomInfo.romID == "IPKE" || RomInfo.romID == "IPGE" || RomInfo.romID == "IPGS")
                languageOffset = +8;

            foreach (KeyValuePair<uint[], string> kv in ToolboxDB.matrixExpansionDB) {
                listOfChanges += " - Replace " + (kv.Value.Length / 3 + 1) + " bytes of data at arm9 offset";
                if (kv.Key.Length > 1)
                    listOfChanges += "s";

                for (int i = 0; i < kv.Key.Length; i++) {
                    listOfChanges += " 0x" + (kv.Key[i] - 0x02000000 + languageOffset).ToString("X");

                    if (i < kv.Key.Length - 1)
                        listOfChanges += ",";
                }
                listOfChanges += " with " + '\n' + kv.Value + "\n\n";
            }

            DialogResult d;
            d = MessageBox.Show("Confirming this process will apply the following changes:\n\n" +
                listOfChanges +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes) {
                try {
                    foreach (KeyValuePair<uint[], string> kv in ToolboxDB.matrixExpansionDB) {
                        foreach(uint offset in kv.Key) {
                            DSUtils.WriteToArm9(HexStringToByteArray(kv.Value), (uint)(offset - 0x02000000 + languageOffset));
                        }
                    }
                } catch {
                    MessageBox.Show("Operation failed. It is strongly advised that you restore the arm9 backup (arm9.bin.backup).", "Something went wrong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                DisableMatrixExpansionPatch("Already applied");
                expandedMatrixCB.Visible = true;
                flag_MatrixExpansionApplied = true;
                MessageBox.Show("Matrix 0 can now be freely expanded up to twice its size.", "Operation successful.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void dynamicHeadersButton_Click(object sender, EventArgs e)
        {
            DynamicHeadersPatchData data = new DynamicHeadersPatchData();

            DialogResult d;
            d = MessageBox.Show("Confirming this process will apply the following changes:\n\n" +
                "- Backup ARM9 file (arm9.bin.backup will be created)." + "\n\n" +
                "- Non ho sbatti di listare i cambiamenti" + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes)
            {
                File.Copy(RomInfo.arm9Path, RomInfo.arm9Path + ".backup", overwrite: true);

                try
                {
                    /* Write main routine (HG USA):

                     00 B5		        push (lr)
                     01 1C		        mov r1, r0
                     32 20*		        mov r0, #0x32
                     00 22		        mov r2, #0x0
                     CC F7 58 F9**	    bl 0x02007524	@Load_Memory
                     03 1C		        mov r3, r0
                     DF F7 49 FC**	    bl 0x0201AB0C	@Free_Memory
                     00 BD		        pop, pc

                    *FOR PLATINUM (all languages):
                     94 20		        mov r0, #0x94

                    **BRANCHES FOR OTHER VERSIONS/LANGUAGES:

                     HG ESP (IPKS): 
                     CC F7 5C F9	    bl 0x02007524	@Load_Memory
                     DF F7 4D FC	    bl 0x0201AB0C	@Free_Memory

                     HG JAP (IPKJ) and SS JAP (IPGJ):
                     CC F7 08 FB	    bl 0x0200743C	@Load_Memory
                     DF F7 C7 FC	    bl 0x0201A7C0	@Free_Memory

                     Plat USA (CPUE):
                     CC F7 48 FD	    bl 0x02006AC0	@Load_Memory
                     DE F7 C7 F8	    bl 0x020181C4	@Free_Memory
                    
                     Plat ESP (CPUS), ITA (CPUI), FRA (CPUF), GER (CPUD):
                     CC F7 00 FD	    bl 0x02006AD4	@Load_Memory
                     CC F7 74 FC	    bl 0x02018234	@Free_Memory

                     Plat JAP (CPUJ):
                     CC F7 0A FF	    bl 0x02006A00	@Load_Memory
                     DE F7 3D F9	    bl 0x02017E6C	@Free_Memory
                     */

                    DSUtils.WriteToArm9(HexStringToByteArray(data.initString), data.initOffset);

                    /* - Neutralize instances of HeaderID * 0x18 so the base offset the data is read from is always 0x0:
                           
                            Replace this:
                            18 21       mov r1, #0x18
                            41 43       mul r1, r0

                            with this:
                            19 00       lsl r1, r3, 0
                            C0 46       nop
        
                      - Change pointers to header fields from ARM9_HEADER_TABLE_OFFSET + n to simply 0 + n
                     
                       * for ESP HG (IPKS): subtract 0x8 from every reference offset
                       * for JAP HG (IPKJ) and SS (IPGJ): subtract 0x448 from every reference offset
                       * for Plat ESP, ITA, FRA, GER, JAP: add 0xA4 to every reference offset
                       * for Plat JAP: subtract 0x444 from every reference offset

                     */

                    foreach (Tuple<uint, uint> reference in ToolboxDB.dynamicHeadersPointersDB[RomInfo.gameFamily])
                    {
                        DSUtils.WriteToArm9(HexStringToByteArray(data.REFERENCE_STRING), (uint)(reference.Item1 + data.pointerDiff));

                        uint pointerValue = BitConverter.ToUInt32(DSUtils.ReadFromArm9((uint)(reference.Item2 + data.pointerDiff), 4), 0) - PokeDatabase.System.headerOffsetsDict[RomInfo.romID] - 0x02000000;
                        DSUtils.WriteToArm9(BitConverter.GetBytes(pointerValue), (uint)(reference.Item2 + data.pointerDiff));
                    }

                    if (RomInfo.gameFamily == "HGSS")
                    {
                        /*  Special case: at 0x3B522 (non-JAP and non-Spanish HG offset) there is an instruction 
                            between the mov r1, #0x18 and mul r1, r0 commands, so we must handle this separately */

                        DSUtils.WriteToArm9(HexStringToByteArray("19 00"), (uint)(0x3B522 + data.pointerDiff));
                        DSUtils.WriteToArm9(HexStringToByteArray("C0 46"), (uint)(0x3B526 + data.pointerDiff));
                        DSUtils.WriteToArm9(HexStringToByteArray("00 00 00 00"), (uint)(0x3B53C + data.pointerDiff));
                    }

                    // Clear the dynamic headers directory in 'unpacked'
                    string headersDir = RomInfo.GetDynamicHeadersDirPath();
                    Directory.Delete(headersDir, true);
                    Directory.CreateDirectory(headersDir);

                    /* Now move the headers data from arm9 to the new directory. Upon saving the ROM,
                       the data will be packed into a NARC and replace a/0/5/0 in HGSS or 
                       debug/cb_edit/d_test.narc in Platinum */

                    for (int i = 0; i < RomInfo.GetHeaderCount(); i++)
                    {
                        byte[] headerData = MapHeader.LoadFromARM9((ushort)i).ToByteArray();
                        DSUtils.WriteToFile(headersDir + "\\" + i.ToString("D4"), headerData);
                    }

                    DisableDynamicHeadersPatch("Already applied");
                    dynamicHeadersPatchCB.Visible = true;
                    flag_DynamicHeadersPatchApplied = true;

                    MessageBox.Show("The headers are now dynamically allocated in memory.", "Operation successful.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Operation failed. It is strongly advised that you restore the arm9 backup (arm9.bin.backup).", "Something went wrong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #region Mikelan's custom commands
        private void applyCustomCommands(object sender, EventArgs e) {
            int expTableOffset = GetCommandTableOffset();

            if (expTableOffset < 0) {
                DialogResult d;
                d = MessageBox.Show("Script command table has not been repointed.\n\n" +
                    "Do you wish to repoint it to the expanded ARM9 file?\n\n" +
                    "By default it will be written from 0x200 to 0x1700.\n" +
                    "If you already have something there, you must cancel this window and move these things to a new location, or you can manually repoint the script command table to a different free location in the expanded ARM9 file",
                    "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (d == DialogResult.Yes) {
                    RepointCommandTable();
                } else {
                    return;
                }
            }

            if (ImportCustomCommand()) {
                MessageBox.Show("Script commands succesfully installed in the ROM", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private int GetCommandTableOffset() { // Checks if command table is repointed IN THE EXPANDED ARM9 FILE, returns pointer inside this file
            ResourceManager customcmdDB = new ResourceManager("DSPRE.Resources.ROMToolboxDB.CustomScrCmdDB", Assembly.GetExecutingAssembly());
            int pointerOffset = int.Parse(customcmdDB.GetString("pointerOffset" + "_" + RomInfo.gameVersion + "_" + RomInfo.gameLanguage));
            using (BinaryReader arm9Reader = new BinaryReader(new FileStream(RomInfo.arm9Path, FileMode.Open))) {
                arm9Reader.BaseStream.Position = pointerOffset;
                int cmdTable = arm9Reader.ReadInt32();
                if (((cmdTable - 0x023C8000) >= 0) && ((cmdTable - 0x023C8000) <= 0x12B00)) {
                    return (cmdTable - 0x023C8000); // Table position inside the expanded arm9 file
                }
            }
            return -1; // No table in expanded arm9 file
        }
        private void RepointCommandTable() {
            string expandedPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + "\\0000";
            ResourceManager customcmdDB = new ResourceManager("DSPRE.Resources.ROMToolboxDB.CustomScrCmdDB", Assembly.GetExecutingAssembly());

            FileStream arm9FileStream = new FileStream(RomInfo.arm9Path, FileMode.Open); // I make a copy of the stream so the file is free for writing
            MemoryStream arm9Stream = new MemoryStream();
            arm9FileStream.CopyTo(arm9Stream);
            byte[] cmdTbl = arm9Stream.ToArray();

            using (BinaryWriter expArmWriter = new BinaryWriter(new FileStream(expandedPath, FileMode.Open))) {
                expArmWriter.BaseStream.Position = 0x200; // Command table default offset
                expArmWriter.Write(cmdTbl, int.Parse(customcmdDB.GetString("originalTableOffset" + "_" + RomInfo.gameVersion + "_" + RomInfo.gameLanguage)), 4 * 0x355);
            }

            arm9FileStream.Close();

            using (BinaryWriter arm9Writer = new BinaryWriter(new FileStream(RomInfo.arm9Path, FileMode.Open))) // Change both the pointer and the limit
            {
                arm9Writer.BaseStream.Position = int.Parse(customcmdDB.GetString("pointerOffset" + "_" + RomInfo.gameVersion + "_" + RomInfo.gameLanguage));
                arm9Writer.Write((uint)0x023C8200);

                arm9Writer.BaseStream.Position = int.Parse(customcmdDB.GetString("limitOffset" + "_" + RomInfo.gameVersion + "_" + RomInfo.gameLanguage));
                arm9Writer.Write((uint)0x053C);
            }
        }
        private bool ImportCustomCommand() {
            string expandedPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + "\\0000";
            int appliedPatches = 0;

            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Custom Script Command File (*.scrcmd)|*.scrcmd";
            if (of.ShowDialog(this) != DialogResult.OK)
                return false;

            FileStream expandedFileStream = new FileStream(expandedPath, FileMode.Open);
            MemoryStream expandedStream = new MemoryStream();
            expandedFileStream.CopyTo(expandedStream);
            expandedFileStream.Close();

            using (BinaryWriter expandedWriter = new BinaryWriter(new FileStream(expandedPath, FileMode.Open))) {
                using (BinaryReader expandedReader = new BinaryReader(expandedStream)) {
                    try {
                        System.Xml.Linq.XDocument xmldoc = System.Xml.Linq.XDocument.Load(new FileStream(of.FileName, FileMode.Open));

                        foreach (var node in xmldoc.Root.Elements("scriptcommand")) {
                            ushort commandID = UInt16.Parse(node.Attribute("ID").Value, System.Globalization.NumberStyles.HexNumber);
                            string targetROM = node.Element("ROM").Value;
                            string targetLang = node.Element("lang").Value;
                            string commandName = node.Element("name").Value;
                            string paramCount = node.Element("paramcount").Value;
                            string paramCode = node.Element("paramcode").Value;
                            int asmOffset = Int32.Parse(node.Element("asmoffset").Value, System.Globalization.NumberStyles.HexNumber);
                            string asmCode = node.Element("asmcode").Value.Replace("\n", "").Replace("\t", "").Replace(" ", "");

                            if ((RomInfo.gameVersion != targetROM) || (RomInfo.gameLanguage != targetLang)) {
                                continue;
                            }

                            expandedReader.BaseStream.Position = 0x200 + commandID * 4;
                            if (expandedReader.ReadUInt32() != 0) {
                                DialogResult d;
                                d = MessageBox.Show("Script command " + commandID.ToString("X4") + " is already used.\n\n" +
                                    "Do you really want to overwrite it?",
                                    "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (d == DialogResult.No) {
                                    continue;
                                }
                            }

                            expandedWriter.BaseStream.Position = 0x200 + commandID * 4;
                            expandedWriter.Write((UInt32)(0x023C8000 + asmOffset + 1));

                            byte[] asmCodeBytes = StringToByteArray(asmCode);
                            expandedWriter.BaseStream.Position = asmOffset;
                            expandedWriter.Write(asmCodeBytes);

                            appliedPatches++;
                        }

                    } catch {
                        MessageBox.Show("Selected command installation file is corrupted.\n\n" +
                        "Please, download it again or contact its creator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return false;
                    }
                }
            }

            if (appliedPatches == 0) {
                MessageBox.Show("No command could be installed from this file.\n\n" +
                "Make sure the command installation file supports your current ROM.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        #endregion

        #endregion
        #region Utilities
        //Ummm what?
        private byte[] StringToByteArray(String hex) {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static byte[] HexStringToByteArray(string hexString) {
            //FC B5 05 48 C0 46 41 21 
            //09 22 02 4D A8 47 00 20 
            //03 21 FC BD F1 64 00 02 
            //00 80 3C 02
            if (hexString is null)
                return null;

            hexString = hexString.Trim();

            byte[] b = new byte[hexString.Length / 3 + 1];
            for (int i = 0; i < hexString.Length; i += 2) {
                if (hexString[i] == ' ') {
                    hexString = hexString.Substring(1, hexString.Length - 1);
                }

                b[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return b;
        }
        #endregion

        #region Error Messsages
        private void UnsupportedROM() {
            MessageBox.Show("This operation is currently impossible to carry out on any Pokémon " + RomInfo.gameName + " rom.",
                "Unsupported ROM", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void UnsupportedROMLanguage() {
            MessageBox.Show("This operation is currently impossible to carry out on the " + RomInfo.gameLanguage +
                " version of this rom.", "Unsupported Language", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void AlreadyApplied() {
            MessageBox.Show("This patch has already been applied.", "Can't reapply patch", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        private void repointScrcmdButton_Click(object sender, EventArgs e) {

        }


    }
}