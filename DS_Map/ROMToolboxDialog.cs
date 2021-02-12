using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Linq;
using DSPRE.ROMFiles;

namespace DSPRE {
    public partial class ROMToolboxDialog : Form {
        internal class ARM9PatchData {
            internal string initString;
            internal string branchString;

            internal uint branchOffset;
            internal uint initOffset;

            internal ARM9PatchData() {
                ResourceManager arm9DB = new ResourceManager("DSPRE.Resources.ROMToolboxDB.ARM9ExpansionDB", Assembly.GetExecutingAssembly());

                switch (RomInfo.gameVersion) {
                    case "D":
                    case "P":
                        initString = arm9DB.GetString(nameof(initString) + "_" + "D");
                        branchString = arm9DB.GetString(nameof(branchString) + "_" + "D" + "_" + RomInfo.gameLanguage);
                        branchOffset = 0x02000C80;
                        switch (RomInfo.gameLanguage) {
                            case "ENG":
                                initOffset = 0x021064EC;
                                break;
                            case "ESP":
                                initOffset = 0x0210668C;
                                break;
                        }
                        break;
                    case "Plat":
                        initString = arm9DB.GetString(nameof(initString) + "_" + RomInfo.gameVersion + "_" + RomInfo.gameLanguage);
                        branchString = arm9DB.GetString(nameof(branchString) + "_" + RomInfo.gameVersion + "_" + RomInfo.gameLanguage);
                        branchOffset = 0x02000CB4;
                        switch (RomInfo.gameLanguage) {
                            case "ENG":
                                initOffset = 0x02100E20;
                                break;
                            case "ESP":
                                initOffset = 0x0210101C;
                                break;
                        }
                        break;
                    case "HG":
                    case "SS":
                        initString = arm9DB.GetString(nameof(initString) + "_" + "HG");
                        branchString = arm9DB.GetString(nameof(branchString) + "_" + "HG" + "_" + RomInfo.gameLanguage);
                        branchOffset = 0x02000CD0;
                        switch (RomInfo.gameLanguage) {
                            case "ENG":
                                initOffset = 0x02110334;
                                break;
                            case "ESP":
                                initOffset = 0x02110354;
                                break;
                        }
                        break;
                }
                initOffset -= 0x02000000;
                branchOffset -= 0x02000000;
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

            internal uint subroutineOffset;
            internal byte[] subroutine;

            internal BDHCAMPatchData () {
                ResourceManager bdhcamDB = new ResourceManager("DSPRE.Resources.ROMToolboxDB.BDHCAMPatchDB", Assembly.GetExecutingAssembly());

                switch (RomInfo.gameVersion) {
                    case "Plat":
                        overlayNumber = 5;
                        branchString = bdhcamDB.GetString(nameof(branchString) + "_" + RomInfo.gameVersion + "_" + RomInfo.gameLanguage);
                        switch (RomInfo.gameLanguage) {
                            case "ENG":
                                branchOffset = 0x0202040C;
                                overlayOffset1 = 0x0001E1B4;
                                overlayOffset2 = 0x0001E2CC;
                                break;
                            case "ESP":
                                branchOffset = 0x0202047C;
                                overlayOffset1 = 0x0001E1BC;
                                overlayOffset2 = 0x0001E2D4;
                                break;
                        }
                        break;
                    case "HG":
                    case "SS":
                        branchString = bdhcamDB.GetString(nameof(branchString) + "_" + "HG");
                        overlayNumber = 1;

                        branchOffset = 0x02023174;
                        overlayOffset1 = 0x0001574C;
                        overlayOffset2 = 0x00015864;
                        break;
                }
                branchOffset -= 0x02000000;
                overlayString1 = bdhcamDB.GetString(nameof(overlayString1));
                overlayString2 = bdhcamDB.GetString(nameof(overlayString2));

                subroutineOffset = 0x115b0;
                subroutine = (byte[])new ResourceManager("DSPRE.Resources.ROMToolboxDB.BDHCAMPatchDB", Assembly.GetExecutingAssembly()).GetObject(RomInfo.romID.ToLower() + "_cam");
            }
        }

        public int expandedARMfileID = byte.Parse(new ResourceManager("DSPRE.Resources.ROMToolboxDB.SyntheticOverlayFileNumber", Assembly.GetExecutingAssembly()).GetString("fileID" + "_" + RomInfo.gameVersion));
        public static bool flag_standardizedItems { get; private set; } = false;
        public static bool flag_arm9Expanded { get; private set; } = false;
        public static bool flag_BDHCAMpatchApplied { get; private set; } = false;
        public static bool overlay1MustBeRestoredFromBackup { get; private set; } = true;

        #region Constructor
        public ROMToolboxDialog(RomInfo romInfo) {
            InitializeComponent();
            
            CheckStandardizedItems();
            
            if (RomInfo.gameLanguage == "ENG" || RomInfo.gameLanguage == "ESP") {
                CheckARM9Expansion();
            } else {
                disableARM9patch();
                applyARM9ExpansionButton.Text = "Unsupported\nlanguage";

                disableBDHCAMpatch();
                BDHCAMpatchButton.Text = "Unsupported\nlanguage";
            }
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    disableOverlay1patch();
                    overlay1uncomprButton.Text = "Unsupported";
                    break;
                case "Plat":
                    disableOverlay1patch();
                    overlay1uncomprButton.Text = "Unsupported";

                    CheckFilesBDHCAMpatchApplied();
                    break;
                case "HG":
                case "SS":
                    if (!DSUtils.CheckOverlayHasCompressionFlag(1)) {
                        disableOverlay1patch();
                        overlay1CB.Visible = true;
                        overlay1uncomprButton.Text = "Already applied";
                    }
                    CheckFilesBDHCAMpatchApplied();
                    break;
            }
        }
        #region Patch Disable
        private void disableOverlay1patch() {
            overlay1uncomprButton.Enabled = false;
            overlay1uncompressedLBL.Enabled = false;
            overlay1patchtextLBL.Enabled = false;
        }

        private void disableBDHCAMpatch() {
            BDHCAMpatchButton.Enabled = false;
            BDHCAMpatchLBL.Enabled = false;
            BDHCAMpatchTextLBL.Enabled = false;
            bdhcamARM9requiredLBL.Enabled = false;
        }

        private void disableARM9patch() {
            applyARM9ExpansionButton.Enabled = false;
            arm9expansionTextLBL.Enabled = false;
            arm9expansionLBL.Enabled = false;
        }
        private void disableStandardizeItemsPatch() {
            applyItemStandardizeButton.Enabled = false;
            standardizePatchLBL.Enabled = false;
            standardizePatchTextLBL.Enabled = false;
        }
        #endregion
        #endregion

        #region Patch Checkers
        private int CheckARM9Expansion() {
            if (!flag_arm9Expanded) {
                ARM9PatchData data = new ARM9PatchData();

                try { 
                    byte[] branchCode = HexStringtoByteArray(data.branchString);
                    byte[] branchCodeRead = DSUtils.ReadFromArm9(data.branchOffset, data.branchString.Length / 3 + 1); //Read branchCode
                    if (branchCodeRead.Length != branchCode.Length)
                        return 0; //0 means ARM9 Expansion has never been applied
                    if (!branchCodeRead.SequenceEqual(branchCode))
                        return 0;

                    byte[] initCode = HexStringtoByteArray(data.initString);
                    byte[] initCodeRead = DSUtils.ReadFromArm9(data.initOffset, data.initString.Length / 3 + 1); //Read initCode
                    if (initCodeRead.Length != initCode.Length)
                        return 0;
                    if (!initCodeRead.SequenceEqual(initCode))
                        return 0;
                } catch {
                    return -1; //1 means Check failure
                }
            }

            flag_arm9Expanded = true;
            arm9patchCB.Visible = true;
            disableARM9patch();
            applyARM9ExpansionButton.Text = "Already applied";

            switch (RomInfo.gameVersion) {
                case "Plat":
                case "HG":
                case "SS":
                    bdhcamARM9requiredLBL.Visible = false;
                    BDHCAMpatchButton.Enabled = true;
                    BDHCAMpatchLBL.Enabled = true;
                    BDHCAMpatchTextLBL.Enabled = true;
                    break;
            }
            return 1; //arm9 Expansion has already been applied
        }
        private int CheckFilesBDHCAMpatchApplied() {
            if (!flag_arm9Expanded) { 
                bdhcamARM9requiredLBL.Visible = true;
                disableBDHCAMpatch();
                return 0;
            }

            if (!flag_BDHCAMpatchApplied) {
                BDHCAMPatchData data = new BDHCAMPatchData();
                try {
                    byte[] branchCode = HexStringtoByteArray(data.branchString);
                    byte[] branchCodeRead = DSUtils.ReadFromArm9(data.branchOffset, branchCode.Length);

                    if (branchCode.Length != branchCodeRead.Length)
                        return 0; //0 means BDHCAM patch has not been applied
                    if (!branchCode.SequenceEqual(branchCodeRead))
                        return 0;


                    string overlayFilePath = RomInfo.workDir + "overlay" + "\\" + "overlay_" + data.overlayNumber.ToString("D4") + ".bin";
                    DSUtils.DecompressOverlay(data.overlayNumber, true);

                    byte[] overlayCode1 = HexStringtoByteArray(data.overlayString1);
                    byte[] overlayCode1Read = DSUtils.ReadFromFile(overlayFilePath, data.overlayOffset1, overlayCode1.Length);
                    if (overlayCode1.Length != overlayCode1Read.Length)
                        return 0; //0 means BDHCAM patch has not been applied
                    if (!overlayCode1.SequenceEqual(overlayCode1Read))
                        return 0;


                    byte[] overlayCode2 = HexStringtoByteArray(data.overlayString2);
                    byte[] overlayCode2Read = DSUtils.ReadFromFile(overlayFilePath, data.overlayOffset2, overlayCode2.Length); //Write new overlayCode1
                    if (overlayCode2.Length != overlayCode2Read.Length)
                        return 0; //0 means BDHCAM patch has not been applied
                    if (!overlayCode2.SequenceEqual(overlayCode2Read))
                        return 0;

                    String fullFilePath = RomInfo.syntheticOverlayPath + '\\' + expandedARMfileID.ToString("D4");
                    byte[] subroutineRead = DSUtils.ReadFromFile(fullFilePath, data.subroutineOffset, data.subroutine.Length); //Write new overlayCode1
                    if (data.subroutine.Length != subroutineRead.Length)
                        return 0; //0 means BDHCAM patch has not been applied
                    if (!data.subroutine.SequenceEqual(subroutineRead))
                        return 0;
                } catch {
                    return -1;
                }
            }
            flag_BDHCAMpatchApplied = true;
            bdhcamCB.Visible = true;

            disableBDHCAMpatch();
            BDHCAMpatchButton.Text = "Already applied";
            return 0;
        }
        public bool CheckStandardizedItems() {
            DSUtils.UnpackNarc(12);
            if ( flag_standardizedItems || MainProgram.ScanScriptsCheckStandardizedItemNumbers() ) {
                itemNumbersCB.Visible = true;
                flag_standardizedItems = true;

                disableStandardizeItemsPatch();
                applyItemStandardizeButton.Text = "Already applied";
                return true;
            }
            return false;
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
                foreach (int ID in RomInfo.pokémonNamesTextNumbers) {
                    TextArchive pokeName = new TextArchive(ID);
                    for(int i = 0; i < pokeName.messages.Count; i++) {
                        if (pokeName.messages[i] == "")
                            i++;
                        pokeName.messages[i] = char.ToUpper(pokeName.messages[i][0]) + pokeName.messages[i].Substring(1).ToLower();
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

            if (RomInfo.gameVersion == "HG" || RomInfo.gameVersion == "SS") {
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
                "- Modify file #" + expandedARMfileID + " inside " + '\n' + RomInfo.syntheticOverlayPath + '\n' + "to insert the BDHCAM routine (any data between 0x" + data.subroutineOffset.ToString("X") + " and 0x" + data.subroutineOffset + data.subroutine.Length.ToString("X") + " will be overwritten)." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d2 == DialogResult.Yes) {
                File.Copy(RomInfo.arm9Path, RomInfo.arm9Path + ".backup", overwrite: true);

                try {
                    DSUtils.WriteToArm9(data.branchOffset, HexStringtoByteArray(data.branchString)); //Write new branchOffset

                    /* Write to overlayfile */
                    string overlayFilePath = RomInfo.workDir + "overlay" + "\\" + "overlay_" + data.overlayNumber.ToString("D4") + ".bin";
                    if (DSUtils.OverlayIsCompressed(data.overlayNumber))
                        DSUtils.DecompressOverlay(data.overlayNumber, true);

                    DSUtils.WriteToFile(overlayFilePath, data.overlayOffset1, HexStringtoByteArray(data.overlayString1)); //Write new overlayCode1
                    DSUtils.WriteToFile(overlayFilePath, data.overlayOffset2, HexStringtoByteArray(data.overlayString2)); //Write new overlayCode2
                    overlay1MustBeRestoredFromBackup = false;

                    String fullFilePath = RomInfo.syntheticOverlayPath + '\\' + expandedARMfileID.ToString("D4");

                    /*Write Expanded ARM9 File*/
                    DSUtils.WriteToFile(fullFilePath, data.subroutineOffset, data.subroutine);
                } catch {
                    MessageBox.Show("Operation failed. It is strongly advised that you restore the arm9 and overlay from their respective backups.", "Something went wrong",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                flag_BDHCAMpatchApplied = true;
                overlay1MustBeRestoredFromBackup = false;

                disableBDHCAMpatch();
                bdhcamCB.Visible = true;
                BDHCAMpatchButton.Text = "Already applied";

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
                if (DSUtils.OverlayIsCompressed(1))
                    DSUtils.DecompressOverlay(1, true);

                disableOverlay1patch();
                overlay1CB.Visible = true;
                overlay1uncomprButton.Text = "Already applied";
                MessageBox.Show("Overlay1 is now configured as uncompressed.", "Operation successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ApplyItemStandardizeButton_Click(object sender, EventArgs e) {
            DSUtils.UnpackNarc(12);

            if (flag_standardizedItems) {
                AlreadyApplied();
            } else {
                ScriptFile itemScript = new ScriptFile(RomInfo.itemScriptFileNumber);

                for (ushort i = 0; i < itemScript.allScripts.Count - 1; i++) {
                    itemScript.allScripts[i].commands[0].cmdParams[1] = BitConverter.GetBytes(i); // Fix item index
                    itemScript.allScripts[i].commands[1].cmdParams[1] = BitConverter.GetBytes((ushort)1); // Fix item quantity
                }
                itemScript.SaveToFileDefaultDir(RomInfo.itemScriptFileNumber, showSuccessMessage: false);
                MessageBox.Show("Operation successful.", "Process completed.", MessageBoxButtons.OK, MessageBoxIcon.Information);

                itemNumbersCB.Visible = true;
                flag_standardizedItems = true;
                disableStandardizeItemsPatch();
                applyItemStandardizeButton.Text = "Already applied";
            }
        }
        private void ApplyARM9ExpansionButton_Click(object sender, EventArgs e) {
            ARM9PatchData data = new ARM9PatchData();

            DialogResult d;
            d = MessageBox.Show("Confirming this process will apply the following changes:\n\n" +
                "- Backup ARM9 file (arm9.bin.backup will be created)." + "\n\n" +
                "- Replace " + (data.branchString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + data.branchOffset.ToString("X") + " with " + '\n' + data.branchString + "\n\n" +
                "- Replace " + (data.initString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + data.initOffset.ToString("X") + " with " + '\n' + data.initString + "\n\n" +
                "- Modify file #" + expandedARMfileID + " inside " + '\n' + RomInfo.syntheticOverlayPath + '\n' + " to accommodate for 88KB of data (no backup)." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes) {
                File.Copy(RomInfo.arm9Path, RomInfo.arm9Path + ".backup", overwrite: true);

                try {
                    DSUtils.WriteToArm9(data.branchOffset, HexStringtoByteArray(data.branchString)); //Write new branchOffset
                    DSUtils.WriteToArm9(data.initOffset, HexStringtoByteArray(data.initString)); //Write new initOffset

                    string fullFilePath = RomInfo.syntheticOverlayPath + '\\' + expandedARMfileID.ToString("D4");
                    File.Delete(fullFilePath);
                    using (BinaryWriter f = new BinaryWriter(File.Create(fullFilePath))) {
                        for (int i = 0; i < 0x16000; i++)
                            f.Write((byte)0x00); // Write Expanded ARM9 File 
                    }

                    arm9patchCB.Visible = true;
                    flag_arm9Expanded = true;
                    disableARM9patch();
                    applyARM9ExpansionButton.Text = "Already applied";

                    switch (RomInfo.gameVersion) {
                        case "Plat":
                        case "HG":
                        case "SS":
                            bdhcamARM9requiredLBL.Visible = false;
                            BDHCAMpatchButton.Enabled = true;
                            BDHCAMpatchLBL.Enabled = true;
                            BDHCAMpatchTextLBL.Enabled = true;
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
        #region Mikelan's custom commands
        private void applyCustomCommands(object sender, EventArgs e) {
            if (new FileInfo(RomInfo.syntheticOverlayPath + "\\0000").Length < 0x16000) {// ARM9 expansion hasn't been done in this ROM
                MessageBox.Show("The ARM9 Expansion patch must be applied before using this feature", "ARM9 expansion needed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (RomInfo.gameVersion == "D" || RomInfo.gameVersion == "P" || RomInfo.gameVersion == "Plat") {
                UnsupportedROM();
                return;
            }

            if (RomInfo.gameLanguage != "ENG" && RomInfo.gameLanguage != "ESP") {
                UnsupportedROMLanguage();
                return;
            }

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
            string expandedPath = RomInfo.syntheticOverlayPath + "\\0000";
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
            string expandedPath = RomInfo.syntheticOverlayPath + "\\0000";
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

        public static byte[] HexStringtoByteArray(string hexString) {
            //FC B5 05 48 C0 46 41 21 
            //09 22 02 4D A8 47 00 20 
            //03 21 FC BD F1 64 00 02 
            //00 80 3C 02
            if (hexString == null)
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
    }
}