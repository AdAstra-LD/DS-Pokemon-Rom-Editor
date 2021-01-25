using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Linq;

namespace DSPRE {
    public partial class ROMToolboxDialog : Form
    {
        RomInfo romInfo;
        public static bool standardizedItems { get; private set; } = false;
        public static bool arm9Expanded { get; private set; } = false;
        public static bool BDHCAMpatchApplied { get; private set; } = false;
        public static bool overlayMustBeRestoredFromBackup { get; private set; } = true;

        public ROMToolboxDialog(RomInfo romInfo) {
            InitializeComponent();
            this.romInfo = romInfo;

            DSUtils.UnpackNarc(12);
            CheckStandardizedItems();
            CheckARM9Expansion();
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    bdhcamARM9LBL.Enabled = false;
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

                    if (CheckBDHCAMPatchApplied() == 1)
                        BDHCAMpatchButton.Text = "Already applied";
                    break;
                case "HG":
                case "SS":
                    if (!DSUtils.CheckTableOverlayMustBeCompressed(1)) {
                        overlay1uncomprButton.Enabled = false;
                        overlay1uncompressedLBL.Enabled = false;
                        overlay1patchtextLBL.Enabled = false;
                        overlay1uncomprButton.Text = "Already applied";
                    }
                    if (CheckBDHCAMPatchApplied() == 1)
                        BDHCAMpatchButton.Text = "Already applied";
                    break;
            }
        }

        private int CheckBDHCAMPatchApplied() {
            if (BDHCAMpatchApplied)
                return 1;

            if (!arm9Expanded) { 
                bdhcamARM9LBL.Visible = true;
                BDHCAMpatchButton.Enabled = false;
                BDHCAMpatchLBL.Enabled = false;
                BDHCAMpatchTextLBL.Enabled = false;
                return 0;
            }

            bdhcamARM9LBL.Visible = false;
            BDHCAMpatchButton.Enabled = true;
            BDHCAMpatchLBL.Enabled = true;
            BDHCAMpatchTextLBL.Enabled = true;

            return 0;
        }

        private void ApplyItemStandardizeButton_Click(object sender, EventArgs e) {
            DSUtils.UnpackNarc(12);

            if (standardizedItems) {
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
                standardizedItems = true;
            }
        }
        public bool CheckStandardizedItems() {
            if (standardizedItems || MainProgram.CheckStandardizedItemNumbers()) {
                applyItemStandardizeButton.Enabled = false;
                StandardizePatchLBL.Enabled = false;
                StandardizePatchTextLBL.Enabled = false;
                applyItemStandardizeButton.Text = "Already applied";

                standardizedItems = true;
                return true;
            }
            return false;
        }

        private int CheckARM9Expansion () {
            if (!arm9Expanded) {

                uint branchOffset = 0;
                string branchString = "";

                uint initOffset = 0;
                string initString = "";
                byte fileID = Byte.Parse(new ResourceManager("DSPRE.Resources.ROMToolboxDB.SyntheticOverlayFileNumber", Assembly.GetExecutingAssembly()).GetString("fileID" + "_" + RomInfo.gameVersion));

                readArm9PatchOffsets(ref initOffset, ref initString, ref branchOffset, ref branchString);

                byte[] branchCode = HexStringtoByteArray(branchString);
                byte[] initCode = HexStringtoByteArray(initString);

                try {
                    byte[] branchCodeRead = DSUtils.ReadFromArm9(branchOffset, branchString.Length / 3 + 1); //Read branchCode
                    byte[] initCodeRead = DSUtils.ReadFromArm9(initOffset, initString.Length / 3 + 1); //Read initCode

                    if (branchCodeRead.Length != branchCode.Length)
                        return 0; //arm9 Expansion has never been applied
                    if (!branchCodeRead.SequenceEqual(branchCode))
                        return 0;
                    if (initCodeRead.Length != initCode.Length)
                        return 0;
                    if (!initCodeRead.SequenceEqual(initCode))
                        return 0;
                } catch {
                    return -1; //check failed
                }
            }

            applyARM9ExpansionButton.Enabled = false;
            arm9expansionTextLBL.Enabled = false;
            arm9expansionLBL.Enabled = false;
            applyARM9ExpansionButton.Text = "Already applied";

            arm9Expanded = true;

            switch (RomInfo.gameVersion) {
                case "Plat":
                case "HG":
                case "SS":
                    bdhcamARM9LBL.Visible = false;
                    BDHCAMpatchButton.Enabled = true;
                    BDHCAMpatchLBL.Enabled = true;
                    BDHCAMpatchTextLBL.Enabled = true;
                    break;
            }
            return 1; //arm9 Expansion has already been applied
        }
        private void ApplyARM9ExpansionButton_Click(object sender, EventArgs e) {
            uint initOffset = 0;
            string initString = "";

            uint branchOffset = 0;
            string branchString = "";
            byte fileID = Byte.Parse(new ResourceManager("DSPRE.Resources.ROMToolboxDB.SyntheticOverlayFileNumber", Assembly.GetExecutingAssembly()).GetString("fileID" + "_" + RomInfo.gameVersion));

            readArm9PatchOffsets(ref initOffset, ref initString, ref branchOffset, ref branchString);

            DialogResult d;
            d = MessageBox.Show("Confirming this process will apply the following changes:\n\n" +
                "- Backup ARM9 file (arm9.bin.backup will be created)." + "\n\n" +
                "- Replace " + (branchString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + branchOffset.ToString("X") + " with " + '\n' + branchString + "\n\n" +
                "- Replace " + (initString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + initOffset.ToString("X") + " with " + '\n' + initString + "\n\n" +
                "- Modify file #" + fileID + " inside " + '\n' + romInfo.syntheticOverlayPath + '\n' + " to accommodate for 88KB of data (no backup)." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes) {
                if (Arm9ApplyPatch(RomInfo.arm9Path, fileID, initOffset, initString, branchOffset, branchString))
                    MessageBox.Show("The ARM9's usable memory has been expanded.", "Operation successful.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else {
                    MessageBox.Show("Operation failed. It is strongly advised that you restore the arm9 backup (arm9.bin.backup).", "Something went wrong",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void readArm9PatchOffsets(ref uint initOffset, ref string initString, ref uint branchOffset, ref string branchString) {
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
                        default:
                            UnsupportedROMRomLanguage();
                            return;
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
                        default:
                            UnsupportedROMRomLanguage();
                            return;
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
                        default:
                            UnsupportedROMRomLanguage();
                            return;
                    }
                    break;
                default:
                    UnsupportedROM();
                    return;
            }
            initOffset -= 0x02000000;
            branchOffset -= 0x02000000;
        }

        private bool Arm9ApplyPatch(String arm9path, byte fileID, uint initOffset, string initString, uint branchOffset, string branchString) {
            File.Copy(arm9path, arm9path + ".backup", true);

            try {
                uint current = 0;
                DSUtils.WriteToArm9(current, DSUtils.ReadFromArm9(current, branchOffset - current)); //Copy all until branchOffset
                DSUtils.WriteToArm9(branchOffset, HexStringtoByteArray(branchString)); //Write new branchOffset
                current = (uint)(branchOffset + branchString.Length);

                DSUtils.WriteToArm9(current, DSUtils.ReadFromArm9(current, initOffset - current));  //Copy all from branchOffset to initOffset
                DSUtils.WriteToArm9(initOffset, HexStringtoByteArray(initString)); //Write new initOffset
                current = (uint)(initOffset + initString.Length);

                DSUtils.WriteToArm9(current, DSUtils.ReadFromArm9(current, -1));
            } catch {
                return false;
            }

            String fullFilePath = romInfo.syntheticOverlayPath + '\\' + fileID.ToString("D4");
            File.Delete(fullFilePath);

            /*Write Expanded ARM9 File*/
            BinaryWriter f = new BinaryWriter(File.Create(fullFilePath));
            for (int i = 0; i < 0x16000; i++)
                f.Write((byte)0x00);

            f.Close();

            applyARM9ExpansionButton.Enabled = false;
            arm9expansionTextLBL.Enabled = false;
            arm9expansionLBL.Enabled = false;
            applyARM9ExpansionButton.Text = "Already applied";

            arm9Expanded = true;

            switch (RomInfo.gameVersion) {
                case "Plat":
                case "HG":
                case "SS":
                    bdhcamARM9LBL.Visible = false;
                    BDHCAMpatchButton.Enabled = true;
                    BDHCAMpatchLBL.Enabled = true;
                    BDHCAMpatchTextLBL.Enabled = true;
                break;
            }
            return true;
        }
        private void namesToSentenceCase(int[] fileArchives) {
            //TODO implement this
        }

        private void ApplyPokemonNamesToSentenceCase_Click(object sender, EventArgs e) {
            int[] fileArchives = null;

            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    fileArchives = new int[2] { 443, 444 };
                    break;
                case "Plat":
                    fileArchives = new int[7] { 493, 494, 793, 794, 795, 796, 797 };
                    break;
                case "HG":
                case "SS":
                    fileArchives = new int[7] { 237, 238, 817, 818, 819, 820, 821 };
                    break;
                default:
                    UnsupportedROM();
                    return;
            }
            DialogResult d;
            d = MessageBox.Show("Confirming this process will apply the following changes:\n\n" +
                "- Every Pokémon name will be converted to Sentence Case." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d == DialogResult.Yes) {
                namesToSentenceCase(fileArchives);
                MessageBox.Show("Pokémon names have been converted to Sentence Case.", "Operation successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("No changes have been made.", "Operation canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BDHCAMPatchButton_Click(object sender, EventArgs e) {
            int subroutineOffset = 0x115b0;

            uint branchOffset = 0;
            uint overlayOffset1 = 0;
            uint overlayOffset2 = 0;

            ResourceManager bdhcamDB = new ResourceManager("DSPRE.Resources.ROMToolboxDB.BDHCAMPatchDB", Assembly.GetExecutingAssembly());
            string branchString;

            byte expandedARMfileID = Byte.Parse(new ResourceManager("DSPRE.Resources.ROMToolboxDB.SyntheticOverlayFileNumber", Assembly.GetExecutingAssembly()).GetString("fileID" + "_" + RomInfo.gameVersion));
            byte overlayNumber;

            bool showOverlay1patchrequest = false;
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

                    if (DSUtils.CheckTableOverlayMustBeCompressed(overlayNumber))
                        showOverlay1patchrequest = true;
                    break;
                default:
                    UnsupportedROM();
                    return;
            }
            branchOffset -= 0x02000000;

            string overlayString1 = bdhcamDB.GetString(nameof(overlayString1));
            string overlayString2 = bdhcamDB.GetString(nameof(overlayString2));

            byte[] subroutine = (byte[])new ResourceManager("DSPRE.Resources.ROMToolboxDB.BDHCAMPatchDB", Assembly.GetExecutingAssembly()).GetObject(romInfo.romID.ToLower() + "_cam");

            
            if (showOverlay1patchrequest) {
                DialogResult d1;
                d1 = MessageBox.Show("It is advised to configure Overlay1 as uncompressed before proceeding.\n\n" +
                    "More details in the following dialog.\n\n" + "Do you want to know more?",
                    "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (d1 == DialogResult.Yes) {
                    overlay1uncomprButton_Click(null, null);
                }
            }

            DialogResult d2;
            d2 = MessageBox.Show("This process will apply the following changes:\n\n" +
                "- Backup ARM9 file (arm9.bin.backup will be created)." + "\n\n" +
                "- Backup Overlay" + overlayNumber + " file (overlay" + overlayNumber + ".bin.backup will be created)." + "\n\n" +
                "- Replace " + (branchString.Length / 3 + 1) + " bytes of data at arm9 offset 0x" + branchOffset.ToString("X") + " with " + '\n' + branchString + "\n\n" +
                "- Replace " + (overlayString1.Length / 3 + 1) + " bytes of data at overlay" + overlayNumber + " offset 0x" + overlayOffset1.ToString("X") + " with " + '\n' + overlayString1 + "\n\n" +
                "- Replace " + (overlayString2.Length / 3 + 1) + " bytes of data at overlay" + overlayNumber + " offset 0x" + overlayOffset2.ToString("X") + " with " + '\n' + overlayString2 + "\n\n" +
                "- Modify file #" + expandedARMfileID + " inside " + '\n' + romInfo.syntheticOverlayPath + '\n' + "to insert the BDHCAM routine (any data between 0x" + subroutineOffset.ToString("X") + " and 0x" + subroutineOffset+subroutine.Length.ToString("X") + " will be overwritten)." + "\n\n" +
                "Do you wish to continue?",
                "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d2 == DialogResult.Yes) {
                File.Copy(RomInfo.arm9Path, RomInfo.arm9Path + ".backup", true);

                try {
                    uint current = 0;
                    DSUtils.WriteToArm9(current, DSUtils.ReadFromArm9(current, branchOffset - current)); //Copy all until branchOffset
                    DSUtils.WriteToArm9(branchOffset, HexStringtoByteArray(branchString)); //Write new branchOffset

                    /* Write to overlayfile */
                    current = 0;
                    string overlayFilePath = romInfo.workDir + "overlay" + "\\" + "overlay_" + overlayNumber.ToString("D4") + ".bin";
                    DSUtils.DecompressOverlay(overlayNumber, true);

                    DSUtils.WriteToFile(overlayFilePath, current, DSUtils.ReadFromFile(overlayFilePath, current, overlayOffset1 - current));  //Copy all from beginning to overlayOffset1
                    DSUtils.WriteToFile(overlayFilePath, overlayOffset1, HexStringtoByteArray(overlayString1)); //Write new overlayCode1
                    current = (uint)(overlayOffset1 + overlayString2.Length);

                    DSUtils.WriteToFile(overlayFilePath, current, DSUtils.ReadFromFile(overlayFilePath, current, overlayOffset2 - current));  //Copy all from overlayCode1 to overlayOffset2
                    DSUtils.WriteToFile(overlayFilePath, overlayOffset2, HexStringtoByteArray(overlayString2)); //Write new overlayCode2
                    current = (uint)(overlayOffset2 + overlayString2.Length);

                    DSUtils.WriteToFile(overlayFilePath, current, DSUtils.ReadFromFile(overlayFilePath, current, -1));
                    overlayMustBeRestoredFromBackup = false;
                } catch {
                    MessageBox.Show("Operation failed. It is strongly advised that you restore the arm9 and overlay from their respective backups.", "Something went wrong",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                String fullFilePath = romInfo.syntheticOverlayPath + '\\' + expandedARMfileID.ToString("D4");

                /*Write Expanded ARM9 File*/
                BinaryWriter f = new BinaryWriter(File.OpenWrite(fullFilePath));
                f.BaseStream.Position = subroutineOffset;
                f.Write(subroutine);

                f.Close();

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
