using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Linq;

namespace DSPRE {
    public partial class ROMToolboxDialog : Form {
        internal class ARM9PatchData {
            internal string initString;
            internal string branchString;

            internal uint branchOffset;
            internal uint initOffset;

            public ARM9PatchData() {
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

            internal int subroutineOffset;
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

        RomInfo romInfo;
        public int expandedARMfileID = byte.Parse(new ResourceManager("DSPRE.Resources.ROMToolboxDB.SyntheticOverlayFileNumber", Assembly.GetExecutingAssembly()).GetString("fileID" + "_" + RomInfo.gameVersion));
        public static bool flag_standardizedItems { get; private set; } = false;
        public static bool flag_arm9Expanded { get; private set; } = false;
        public static bool flag_BDHCAMpatchApplied { get; private set; } = false;
        public static bool overlayMustBeRestoredFromBackup { get; private set; } = true;

        #region Constructor
        public ROMToolboxDialog(RomInfo romInfo) {
            InitializeComponent();

            this.romInfo = romInfo;

            DSUtils.UnpackNarc(12);
            CheckStandardizedItems();
            CheckARM9Expansion();
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    bdhcamARM9requiredLBL.Enabled = false;
                    BDHCAMpatchButton.Text = "Unsupported";

                    overlay1uncomprButton.Enabled = false;
                    overlay1uncompressedLBL.Enabled = false;
                    overlay1patchtextLBL.Enabled = false;
                    overlay1uncomprButton.Text = "Unsupported";
                    break;
                case "Plat":
                    overlay1uncomprButton.Enabled = false;
                    overlay1uncompressedLBL.Enabled = false;
                    overlay1patchtextLBL.Enabled = false;
                    overlay1uncomprButton.Text = "Unsupported";

                    if (CheckFilesBDHCAMPatchApplied() == 1)
                        BDHCAMpatchButton.Text = "Already applied";
                    break;
                case "HG":
                case "SS":
                    if (!DSUtils.CheckOverlayHasCompressionFlag(1)) {
                        overlay1uncomprButton.Enabled = false;
                        overlay1uncompressedLBL.Enabled = false;
                        overlay1patchtextLBL.Enabled = false;
                        overlay1uncomprButton.Text = "Already applied";
                    }
                    if (CheckFilesBDHCAMPatchApplied() == 1)
                        BDHCAMpatchButton.Text = "Already applied";
                    break;
            }
        }
        #endregion

        #region Patch Checkers
        private int CheckARM9Expansion() {
            if (!flag_arm9Expanded) {
                ARM9PatchData data = new ARM9PatchData();

                try {
                    byte[] branchCodeRead = DSUtils.ReadFromArm9(data.branchOffset, data.branchString.Length / 3 + 1); //Read branchCode
                    byte[] initCodeRead = DSUtils.ReadFromArm9(data.initOffset, data.initString.Length / 3 + 1); //Read initCode

                    byte[] branchCode = HexStringtoByteArray(data.branchString);
                    byte[] initCode = HexStringtoByteArray(data.branchString);

                    if (branchCodeRead.Length != branchCode.Length)
                        return 0; //0 means ARM9 Expansion has never been applied
                    if (!branchCodeRead.SequenceEqual(branchCode))
                        return 0;
                    if (initCodeRead.Length != initCode.Length)
                        return 0;
                    if (!initCodeRead.SequenceEqual(initCode))
                        return 0;
                } catch {
                    return -1; //1 means Check failure
                }
            }

            applyARM9ExpansionButton.Enabled = false;
            arm9expansionTextLBL.Enabled = false;
            arm9expansionLBL.Enabled = false;
            applyARM9ExpansionButton.Text = "Already applied";

            flag_arm9Expanded = true;

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
        private int CheckFilesBDHCAMPatchApplied() {
            if (!flag_arm9Expanded) { 
                bdhcamARM9requiredLBL.Visible = true;
                BDHCAMpatchButton.Enabled = false;
                BDHCAMpatchLBL.Enabled = false;
                BDHCAMpatchTextLBL.Enabled = false;
                return 0;
            }

            if (flag_BDHCAMpatchApplied)
                return 1;

            bdhcamARM9requiredLBL.Visible = false;
            BDHCAMpatchButton.Enabled = true;
            BDHCAMpatchLBL.Enabled = true;
            BDHCAMpatchTextLBL.Enabled = true;

            return 0;
        }
        public bool CheckStandardizedItems() {
            if ( flag_standardizedItems || MainProgram.ScanScriptsCheckStandardizedItemNumbers() ) {
                applyItemStandardizeButton.Enabled = false;
                StandardizePatchLBL.Enabled = false;
                StandardizePatchTextLBL.Enabled = false;
                applyItemStandardizeButton.Text = "Already applied";

                flag_standardizedItems = true;
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
                //do things with   RomInfo.pokémonNamesTextNumbers;
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
                    d1 = MessageBox.Show("It is advised to configure Overlay1 as uncompressed before proceeding.\n\n" +
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
                "- Modify file #" + expandedARMfileID + " inside " + '\n' + romInfo.syntheticOverlayPath + '\n' + "to insert the BDHCAM routine (any data between 0x" + data.subroutineOffset.ToString("X") + " and 0x" + data.subroutineOffset + data.subroutine.Length.ToString("X") + " will be overwritten)." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d2 == DialogResult.Yes) {
                File.Copy(RomInfo.arm9Path, RomInfo.arm9Path + ".backup", true);

                try {
                    DSUtils.WriteToArm9(data.branchOffset, HexStringtoByteArray(data.branchString)); //Write new branchOffset

                    /* Write to overlayfile */
                    string overlayFilePath = romInfo.workDir + "overlay" + "\\" + "overlay_" + data.overlayNumber.ToString("D4") + ".bin";
                    DSUtils.DecompressOverlay(data.overlayNumber, true);

                    DSUtils.WriteToFile(overlayFilePath, data.overlayOffset1, HexStringtoByteArray(data.overlayString1)); //Write new overlayCode1
                    DSUtils.WriteToFile(overlayFilePath, data.overlayOffset2, HexStringtoByteArray(data.overlayString2)); //Write new overlayCode2
                    overlayMustBeRestoredFromBackup = false;
                } catch {
                    MessageBox.Show("Operation failed. It is strongly advised that you restore the arm9 and overlay from their respective backups.", "Something went wrong",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                String fullFilePath = romInfo.syntheticOverlayPath + '\\' + expandedARMfileID.ToString("D4");

                /*Write Expanded ARM9 File*/
                using (BinaryWriter f = new BinaryWriter(File.OpenWrite(fullFilePath))) {
                    f.BaseStream.Position = data.subroutineOffset;
                    f.Write(data.subroutine);
                }

                flag_BDHCAMpatchApplied = true;

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
                DSUtils.DecompressOverlay(1, true);

                overlay1uncomprButton.Enabled = false;
                overlay1uncompressedLBL.Enabled = false;
                overlay1patchtextLBL.Enabled = false;
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
                for (int i = 0; i < itemScript.scripts.Count - 1; i++) {
                    itemScript.scripts[i].commands[0].parameterList[1] = BitConverter.GetBytes((ushort)i); // Fix item index
                    itemScript.scripts[i].commands[1].parameterList[1] = BitConverter.GetBytes((ushort)1); // Fix item quantity
                }
                itemScript.SaveToFile(RomInfo.itemScriptFileNumber);
                MessageBox.Show("Operation successful.", "Process completed.", MessageBoxButtons.OK, MessageBoxIcon.Information);

                applyItemStandardizeButton.Enabled = false;
                StandardizePatchLBL.Enabled = false;
                StandardizePatchTextLBL.Enabled = false;
                applyItemStandardizeButton.Text = "Already applied";
                flag_standardizedItems = true;
            }
        }
        private void ApplyARM9ExpansionButton_Click(object sender, EventArgs e) {
            ARM9PatchData data = new ARM9PatchData();

            DialogResult d;
            d = MessageBox.Show("Confirming this process will apply the following changes:\n\n" +
                "- Backup ARM9 file (arm9.bin.backup will be created)." + "\n\n" +
                "- Replace " + (data.branchString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + data.branchOffset.ToString("X") + " with " + '\n' + data.branchString + "\n\n" +
                "- Replace " + (data.initString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + data.initOffset.ToString("X") + " with " + '\n' + data.initString + "\n\n" +
                "- Modify file #" + expandedARMfileID + " inside " + '\n' + romInfo.syntheticOverlayPath + '\n' + " to accommodate for 88KB of data (no backup)." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes) {
                File.Copy(RomInfo.arm9Path, RomInfo.arm9Path + ".backup", true);

                try {
                    DSUtils.WriteToArm9(data.branchOffset, HexStringtoByteArray(data.branchString)); //Write new branchOffset
                    DSUtils.WriteToArm9(data.initOffset, HexStringtoByteArray(data.initString)); //Write new initOffset

                    string fullFilePath = romInfo.syntheticOverlayPath + '\\' + expandedARMfileID.ToString("D4");
                    File.Delete(fullFilePath);
                    using (BinaryWriter f = new BinaryWriter(File.Create(fullFilePath))) {
                        for (int i = 0; i < 0x16000; i++)
                            f.Write((byte)0x00); // Write Expanded ARM9 File 
                    }

                    applyARM9ExpansionButton.Enabled = false;
                    arm9expansionTextLBL.Enabled = false;
                    arm9expansionLBL.Enabled = false;
                    applyARM9ExpansionButton.Text = "Already applied";

                    flag_arm9Expanded = true;

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
        #endregion
        #region Utilities
        private byte[] HexStringtoByteArray(string hexString) {
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
            MessageBox.Show("This operation is currently impossible to carry out on any Pokémon " + romInfo.gameName + " rom.",
                "Unsupported ROM", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UnsupportedROMRomLanguage() {
            MessageBox.Show("This operation is currently impossible to carry out on the " + RomInfo.gameLanguage +
                " RomInfo.gameVersion of this rom.", "Unsupported RomInfo.gameLanguageuage", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void AlreadyApplied() {
            MessageBox.Show("This patch has already been applied.", "Can't reapply patch", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
    }

    
}
