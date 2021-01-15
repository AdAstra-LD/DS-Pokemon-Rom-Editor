using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using NarcAPI;
using Tao.OpenGl;
using LibNDSFormats.NSBMD;
using LibNDSFormats.NSBTX;

namespace DSPRE {
    public partial class MainProgram : Form {
        public MainProgram() {
            InitializeComponent();
        }

        #region RM
        ResourceManager rm = new ResourceManager("DSPRE.WinFormStrings", Assembly.GetExecutingAssembly());
        #endregion

        #region Program Window

        #region Variables
        public bool disableHandlers = false;
        public bool iconON = false;
        public static string workDir;
        public bool expandedARM9;
        public bool standardizedItems;

        /* Editors Setup */
        private bool matrixEditorIsReady = false;
        private bool mapEditorIsReady = false;
        private bool eventEditorIsReady = false;
        private bool scriptEditorIsReady = false;
        private bool textEditorIsReady = false;
        private bool tilesetEditorIsReady = false;


        /* ROM Information */
        public static string gameCode;
        RomInfo romInfo;

        #endregion

        #region Subroutines
        private void CompressOverlay(int overlayNumber) {
            String overlayFilePath = '"' + workDir + "overlay" + "\\" + "overlay_" + overlayNumber.ToString("D4") + ".bin" + '"';
            Process unpack = new Process();
            unpack.StartInfo.FileName = @"Tools\blz.exe";
            unpack.StartInfo.Arguments = "-en " + overlayFilePath;
            Application.DoEvents();
            unpack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            unpack.StartInfo.CreateNoWindow = true;
            unpack.Start();
            unpack.WaitForExit();
        }

        private bool DecompressArm9() {
            int attempts = 0;
            long arm9Length = new FileInfo(workDir + @"arm9.bin").Length;

            while (attempts < 3 && arm9Length < 0xBC000) {
                attempts++;
                if (attempts > 1) {
                    BinaryWriter arm9Truncate = new BinaryWriter(File.OpenWrite(workDir + @"arm9.bin"));
                    
                    arm9Truncate.BaseStream.SetLength(arm9Length - 0xc);
                    arm9Truncate.Close();
                }
                Process decompress = new Process();
                decompress.StartInfo.FileName = @"Tools\blz.exe";
                decompress.StartInfo.Arguments = @" -d " + '"' + workDir + "arm9.bin" + '"';
                decompress.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                decompress.StartInfo.CreateNoWindow = true;
                decompress.Start();
                decompress.WaitForExit();

                arm9Length = new FileInfo(workDir + @"arm9.bin").Length;
            } 

            return (arm9Length > 0xBC000);
        }

        private int decompressOverlay(int overlayNumber, bool makeBackup) {
            String overlayFilePath = workDir + "overlay" + "\\" + "overlay_" + overlayNumber.ToString("D4") + ".bin";

            if (!File.Exists(overlayFilePath)) {
                MessageBox.Show("Overlay to decompress #" + overlayNumber + " doesn't exist",
                    "Overlay not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

            if (makeBackup) {
                if (File.Exists(overlayFilePath + ".bak")) {
                    if (new FileInfo(overlayFilePath).Length > new FileInfo(overlayFilePath + ".bak").Length) { //if overlay is bigger than its backup
                        Console.WriteLine("Overlay " + overlayNumber + " is already uncompressed and its compressed backup exists.");
                        return 1;
                    }
                    File.Delete(overlayFilePath + ".bak");
                }
                File.Copy(overlayFilePath, overlayFilePath + ".bak");
            }

            Process unpack = new Process();
            unpack.StartInfo.FileName = @"Tools\blz.exe";
            String arguments = "-d " + '"' + overlayFilePath + '"';
            unpack.StartInfo.Arguments = arguments;
            Application.DoEvents();
            unpack.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            unpack.StartInfo.CreateNoWindow = false;
            unpack.Start();
            unpack.WaitForExit();
            return unpack.ExitCode;
        }

        private void restoreOverlayFromCompressedBackup(int overlayNumber) {
            String overlayFilePath = workDir + "overlay" + "\\" + "overlay_" + overlayNumber.ToString("D4") + ".bin";

            if (new FileInfo(overlayFilePath).Length <= new FileInfo(overlayFilePath + ".bak").Length) { //if overlay is bigger than its backup
                Console.WriteLine("Overlay " + overlayNumber + " is already compressed.");
                return;
            }

            if (File.Exists(overlayFilePath + ".bak")) {
                File.Delete(overlayFilePath);
                File.Move(overlayFilePath + ".bak", overlayFilePath);
            } else {
                MessageBox.Show("File " + '"' + overlayFilePath + ".bak" + '"' + " couldn't be found and restored.",
                    "Can't restore overlay from backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string[] GetBuildingsList(bool interior) {
            List<string> names = new List<string>();
            string path = romInfo.GetBuildingModelsDirPath(interior);
            int buildModelsCount = Directory.GetFiles(path).Length;

            for (int i = 0; i < buildModelsCount; i++) {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(path + "\\" + i.ToString("D4")))) {
                    reader.BaseStream.Position = 0x38;
                    string nsbmdName = Encoding.UTF8.GetString(reader.ReadBytes(16)).TrimEnd();
                    names.Add(i + ": " + nsbmdName);
                }
            }
            return names.ToArray();
        }

        private string[] GetTrainerNames() {
            List<string> trainerList = new List<string>();

            /* Store all trainer names and classes */
            TextArchive trainerClasses = LoadMessageArchive(romInfo.GetTrainerClassMessageNumber());
            TextArchive trainerNames = LoadMessageArchive(romInfo.GetTrainerNamesMessageNumber());
            BinaryReader trainerReader;
            int trainerCount = Directory.GetFiles(romInfo.GetTrainerDataDirPath()).Length;

            for (int i = 0; i < trainerCount; i++) {
                trainerReader = new BinaryReader(new FileStream(romInfo.GetTrainerDataDirPath() + "\\" + i.ToString("D4"), FileMode.Open));
                trainerReader.BaseStream.Position += 0x1;
                trainerList.Add("[" + i.ToString("D2") + "] " + trainerClasses.messages[trainerReader.ReadUInt16()] + " " + trainerNames.messages[i]);
            }
            return trainerList.ToArray();
        }

        private string[] GetItemNames() {
            return LoadMessageArchive(romInfo.GetItemNamesMessageNumber()).messages.ToArray();
        }

        private string[] GetItemNames(int startIndex, int count) {
            return LoadMessageArchive(romInfo.GetItemNamesMessageNumber()).messages.GetRange(startIndex, count).ToArray();
        }

        private string[] GetPokémonNames() {
            return LoadMessageArchive(romInfo.GetPokémonNamesMessageNumber()).messages.ToArray();
        }

        private string[] GetAttackNames() {
            return LoadMessageArchive(romInfo.GetAttackNamesMessageNumber()).messages.ToArray();
        }

        private AreaData LoadAreaData(uint areaDataID) {
            return new AreaData(new FileStream(romInfo.GetAreaDataDirPath() + "//" + areaDataID.ToString("D4"), FileMode.Open), romInfo.GetGameVersion());
        }

        private MapFile LoadMapFile(int mapNumber) {
            try {
                return new MapFile(new FileStream(romInfo.GetMapDirPath() + "\\" + mapNumber.ToString("D4"), FileMode.Open), romInfo.GetGameVersion());
            } catch (FileNotFoundException) {
                MessageBox.Show("File " + '"' + romInfo.GetMapDirPath() + "\\" + mapNumber.ToString("D4") + '"' + " is missing.", "File not found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }

        private Matrix LoadMatrix(int matrixNumber) {
            return new Matrix(new FileStream(romInfo.GetMatrixDirPath() + "\\" + matrixNumber.ToString("D4"), FileMode.Open));
        }

        public TextArchive LoadMessageArchive(int fileID) {
            TextArchive ta = null;
            try {
                ta = new TextArchive(new FileStream(romInfo.GetTextArchivesPath() + "\\" + fileID.ToString("D4"), FileMode.Open));
            } catch (FileNotFoundException) {
                MessageBox.Show("Text archive not found.\n", "Can't load text", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ta;
        }

        private EventFile LoadEventFile(int fileID) {
            EventFile ev = null;
            try {
                ev = new EventFile(new FileStream(romInfo.GetEventsDirPath() + "\\" + fileID.ToString("D4"), FileMode.Open));
            } catch (FileNotFoundException) {
                MessageBox.Show("Event file not found.\n", "Can't load event", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ev;
        }

        private void PaintGameIcon(object sender, PaintEventArgs e) {
            if (iconON == true) {
                BinaryReader readIcon;
                try {
                    readIcon = new BinaryReader(File.OpenRead(workDir + @"banner.bin"));
                } catch (FileNotFoundException) {
                    MessageBox.Show("Couldn't load " + '"' + "banner.bin" + '"' + '.', "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                #region Read Icon Palette
                readIcon.BaseStream.Position = 0x220;
                byte firstByte, secondByte;
                int palR, palG, palB;
                int palCounter = 0;
                int[] paletteArray = new int[48];
                for (int i = 0; i < 16; i++) {
                    palR = 0;
                    palG = 0;
                    palB = 0;
                    secondByte = readIcon.ReadByte();
                    firstByte = readIcon.ReadByte();

                    if ((firstByte & (1 << 6)) != 0)
                        palB = palB | (1 << 4);
                    if ((firstByte & (1 << 5)) != 0)
                        palB = palB | (1 << 3);
                    if ((firstByte & (1 << 4)) != 0)
                        palB = palB | (1 << 2);
                    if ((firstByte & (1 << 3)) != 0)
                        palB = palB | (1 << 1);
                    if ((firstByte & (1 << 2)) != 0)
                        palB = palB | (1 << 0);
                    if ((firstByte & (1 << 1)) != 0)
                        palG = palG | (1 << 4);
                    if ((firstByte & (1 << 0)) != 0)
                        palG = palG | (1 << 3);
                    if ((secondByte & (1 << 7)) != 0)
                        palG = palG | (1 << 2);
                    if ((secondByte & (1 << 6)) != 0)
                        palG = palG | (1 << 1);
                    if ((secondByte & (1 << 5)) != 0)
                        palG = palG | (1 << 0);
                    if ((secondByte & (1 << 4)) != 0)
                        palR = palR | (1 << 4);
                    if ((secondByte & (1 << 3)) != 0)
                        palR = palR | (1 << 3);
                    if ((secondByte & (1 << 2)) != 0)
                        palR = palR | (1 << 2);
                    if ((secondByte & (1 << 1)) != 0)
                        palR = palR | (1 << 1);
                    if ((secondByte & (1 << 0)) != 0)
                        palR = palR | (1 << 0);

                    paletteArray[palCounter] = palR * 8;
                    palCounter++;
                    paletteArray[palCounter] = palG * 8;
                    palCounter++;
                    paletteArray[palCounter] = palB * 8;
                    palCounter++;
                }
                #endregion
                #region Read Icon Image
                readIcon.BaseStream.Position = 0x20;
                byte pixelByte;
                int pixelPalId;
                int iconX;
                int iconY = 0;
                int xTile = 0;
                int yTile = 0;
                for (int o = 0; o < 4; o++) {
                    for (int a = 0; a < 4; a++) {
                        for (int i = 0; i < 8; i++) {
                            iconX = xTile;

                            for (int counter = 0; counter < 4; counter++) {
                                pixelByte = readIcon.ReadByte();
                                pixelPalId = pixelByte & 0x0F;
                                Brush icon = new SolidBrush(Color.FromArgb(255, paletteArray[pixelPalId * 3], paletteArray[pixelPalId * 3 + 1], paletteArray[pixelPalId * 3 + 2]));
                                e.Graphics.FillRectangle(icon, iconX, i + yTile, 1, 1);
                                iconX++;
                                pixelPalId = (pixelByte & 0xF0) >> 4;
                                icon = new SolidBrush(Color.FromArgb(255, paletteArray[pixelPalId * 3], paletteArray[pixelPalId * 3 + 1], paletteArray[pixelPalId * 3 + 2]));
                                e.Graphics.FillRectangle(icon, iconX, i + yTile, 1, 1);
                                iconX++;
                            }
                            iconY++;
                        }
                        iconY = 0;
                        xTile += 8;
                    }
                    xTile = 0;
                    yTile += 8;
                }
                #endregion
                readIcon.Close();
            } else return;
        }
        public static byte[] ReadFromArm9(long startOffset, long numberOfBytes) {
            BinaryReader readArm9 = new BinaryReader(File.OpenRead(workDir + @"arm9.bin"));
            readArm9.BaseStream.Position = startOffset;
            byte[] tarGetBytes = null;

            if (numberOfBytes < 0) {
                numberOfBytes = 2097152; //ARM9 is definitely smaller than 2MB
            }
            try {
                tarGetBytes = readArm9.ReadBytes((int)numberOfBytes);
            } catch (EndOfStreamException) {
                Console.WriteLine("ARM9 Stream ended");
            }
            readArm9.Dispose();
            return tarGetBytes;
        }
        private void DeleteTempFolders() {
            foreach (var tuple in romInfo.GetNarcPaths().Zip(romInfo.GetExtractedNarcDirs(), Tuple.Create)) {
                Directory.Delete(tuple.Item2, true); // Delete folder
            }
        }

        private void RepackRom(string ndsFileName) {
            Process repack = new Process();
            repack.StartInfo.FileName = @"Tools\ndstool.exe";
            repack.StartInfo.Arguments = "-c " + '"' + ndsFileName + '"'
                + " -9 " + '"' + workDir + "arm9.bin" + '"'
                + " -7 " + '"' + workDir + "arm7.bin" + '"'
                + " -y9 " + '"' + workDir + "y9.bin" + '"'
                + " -y7 " + '"' + workDir + "y7.bin" + '"'
                + " -d " + '"' + workDir + "data" + '"'
                + " -y " + '"' + workDir + "overlay" + '"'
                + " -t " + '"' + workDir + "banner.bin" + '"'
                + " -h " + '"' + workDir + "header.bin" + '"';

            Application.DoEvents();
            repack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            repack.StartInfo.CreateNoWindow = true;
            repack.Start();
            repack.WaitForExit();
        }
        private void SetupEventEditor() {
            /* Extract essential NARCs sub-archives*/
            string[] narcPaths = romInfo.GetNarcPaths();
            string[] extractedNarcDirs = romInfo.GetExtractedNarcDirs();

            statusLabel.Text = "Attempting to unpack Event Editor NARCs... Please wait. This might take a while";
            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 12;
            toolStripProgressBar.Value = 0;
            Update();

            for (int i = 2; i < 13; i++) {
                var tuple = Tuple.Create(narcPaths[i], extractedNarcDirs[i]);
                DirectoryInfo di = new DirectoryInfo(tuple.Item2);
                if (!di.Exists || di.GetFiles().Length == 0) {
                    Narc.Open(workDir + tuple.Item1).ExtractToFolder(tuple.Item2);
                }
                toolStripProgressBar.Value++;
            }
            if (romInfo.GetGameVersion() == "HeartGold" || romInfo.GetGameVersion() == "SoulSilver") {
                var tuple = Tuple.Create(narcPaths[narcPaths.Length-1], extractedNarcDirs[extractedNarcDirs.Length-1]);
                DirectoryInfo di = new DirectoryInfo(tuple.Item2);
                if (!di.Exists || di.GetFiles().Length == 0) {
                    Narc.Open(workDir + tuple.Item1).ExtractToFolder(tuple.Item2);
                }
                toolStripProgressBar.Value++;
            }
            

            disableHandlers = true;
            if (File.Exists(romInfo.GetOWtablePath())) {
                switch (romInfo.GetGameVersion()) {
                    case "Diamond":
                    case "Pearl":
                    case "Platinum":
                        break;
                    default:
                        int ret = decompressOverlay(1, true); // HGSS Overlay 1 must be uncompressed in order to read the overworld table
                        if (ret == -1) {
                            MessageBox.Show("Overlay 1 couldn't be decompressed.\nOverworld sprites in the Event Editor will be " +
                                "displayed incorrectly or not displayed at all.", "Error EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                }
            }

            /* Add event file numbers to box */
            int eventCount = Directory.GetFiles(romInfo.GetEventsDirPath()).Length;
            int owSpriteCount = Directory.GetFiles(romInfo.GetOWSpriteDirPath()).Length;
            string[] trainerNames = GetTrainerNames();

            statusLabel.Text = "Loading Events... Please wait";
            toolStripProgressBar.Maximum = eventCount+owSpriteCount+trainerNames.Length;
            toolStripProgressBar.Value = 0;
            Update();

            /* Add event list to event combobox */
            for (int i = 0; i < eventCount; i++) {
                selectEventComboBox.Items.Add("Event File " + i);
                toolStripProgressBar.Value++;
            }

            /* Add sprite list to ow sprite box */
            for (int i = 0; i < owSpriteCount; i++) {
                owSpriteComboBox.Items.Add("Sprite " + i);
                toolStripProgressBar.Value++;
            }

            /* Add trainer list to ow trainer box */
            owTrainerComboBox.Items.AddRange(trainerNames);

            /* Add item list to ow item box */
            int count = LoadScriptFile(romInfo.GetItemScriptFileNumber()).scripts.Count - 1;
            owItemComboBox.Items.AddRange(GetItemNames(0, count));

            /* Add ow movement list to box */
            owMovementComboBox.Items.AddRange(new string[]
                {
                    "[00]  None",
                    "[01]  None",
                    "[02]  Looking in all directions",
                    "[03]  Walking around in all directions",
                    "[04]  Walking Up, Down",
                    "[05]  Walking Left, Right",
                    "[06]  Looking Up, Left",
                    "[07]  Looking Up, Right",
                    "[08]  Looking Down, Left",
                    "[09]  Looking Down, Right",
                    "[10]  Looking Up, Down, Left",
                    "[11]  Looking Up, Right, Down",
                    "[12]  Looking Right, Left, Up",
                    "[13]  Looking Right, Left, Down",
                    "[14]  Facing Up",
                    "[15]  Facing Down",
                    "[16]  Facing Left",
                    "[17]  Facing Right",
                    "[18]  Counterclockwise spinning",
                    "[19]  Clockwise spinning",
                    "[20]  Running Up, Down",
                    "[21]  L Run (Up, Right)",
                    "[22]  Patrols Area, then stops",
                    "[23]  Patrols Area, then stops",
                    "[24]  L Run (Up, Right)",
                    "[25]  Patrols Area, then stops",
                    "[26]  Patrols Area, then stops",
                    "[27]  Patrols Area, then stops",
                    "[28]  L run (Right, Down)",
                    "[29]  L run (Left, Up)",
                    "[30]  Continuous patrolling",
                    "[31]  Continuous patrolling",
                    "[32]  L Run (Down, Right)",
                    "[33]  L Run (Right, Up)",
                    "[34]  Patrols Area, then stops",
                    "[35]  Patrols Area, then stops",
                    "[36]  L Run (Down, Left)",
                    "[37]  Running Up, Left, Down, Right",
                    "[38]  Running Down, Right, Up, Left",
                    "[39]  Running Left, Down, Right, Up",
                    "[40]  Running Right, Up, Left, Down",
                    "[41]  Running Up, Right, Down, Left",
                    "[42]  Running Down, Left, Up, Right",
                    "[43]  Running Left, Up, Right, Down",
                    "[44]  Running Right, Down, Left, Up",
                    "[45]  Looking Up, Down",
                    "[46]  Looking Right, Left",
                    "[47]  ?",
                    "[48]  Follow Hero",
                    "[49]  Semi-circle spin (Down, Right, Up)",
                    "[50]  ?",
                    "[51]  Hidden Under Snow",
                    "[52]  Hidden Under Snow",
                    "[53]  Hidden Underground",
                    "[54]  Hidden Under Grass",
                    "[55]  Mimicks Player (moves within range)",
                    "[56]  Mimicks Player (moves within range)",
                    "[57]  Mimicks Player (moves within range)",
                    "[58]  Mimicks Player (moves within range)",
                    "[59]  Mimick's Player facing direction",
                    "[60]  Mimick's Player facing direction",
                    "[61]  Mimick's Player facing direction",
                    "[62]  Mimick's Player facing direction",
                    "[63]  Jogging on the spot",
                    "[64]  Jogging on the spot",
                    "[65]  Jogging on the spot",
                    "[66]  Jogging on the spot",
                    "[67]  Walking Right, Left",
                    "[68]  Looking Right",
                    "[69]  ?",
                    "[70]  ?",
                    "[71]  Looking Left"
                });

            /* Create dictionary for 3D overworlds */
            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                    break;
                case "Platinum":
                    ow3DSpriteDict = new Dictionary<uint, string>() {
                        [91] = "brown_sign",
                        [92] = "red_sign",
                        [93] = "gray_sign",
                        [94] = "route_sign",
                        [96] = "blue_sign",
                        [101] = "dawn_platinum",
                        [174] = "dppt_suitcase",
                    };
                    break;
                default:
                    break;
            }

            disableHandlers = false;

            /* Draw matrix 0 in matrix navigator */
            eventMatrix = LoadMatrix(0);
            selectEventComboBox.SelectedIndex = 0;

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;
        }
        private void SetupFlagNames() {
            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    flag1CheckBox.Text = "Fly";
                    flag2CheckBox.Text = "Escape Rope";
                    flag3CheckBox.Text = "Run";
                    flag4CheckBox.Text = "Bike";
                    flag5CheckBox.Text = "Battle BG b4";
                    flag6CheckBox.Text = "Battle BG b3";
                    flag7CheckBox.Text = "Battle BG b2";
                    flag8CheckBox.Text = "Battle BG b1";
                    break;
                default:
                    flag1CheckBox.Text = "Flag 1";
                    flag2CheckBox.Text = "Flag 2";
                    flag3CheckBox.Text = "Flag 3";
                    flag4CheckBox.Text = "Fly";
                    flag5CheckBox.Text = "Escape Rope";
                    flag6CheckBox.Text = "Flag 6";
                    flag7CheckBox.Text = "Bicycle";
                    flag8CheckBox.Text = "Flag 8";
                    break;
            }
            toolStripProgressBar.Visible = false;
        }
        private void SetupHeaderEditor() {
            /* Extract essential NARCs sub-archives*/
            string[] narcPaths = romInfo.GetNarcPaths();
            string[] extractedNarcDirs = romInfo.GetExtractedNarcDirs();

            statusLabel.Text = "Attempting to unpack Header Editor NARCs... Please wait.";
            Update();

            for (int i = 0; i < 2; i++) {
                var tuple = Tuple.Create(narcPaths[i], extractedNarcDirs[i]);
                DirectoryInfo di = new DirectoryInfo(tuple.Item2);
                if (!di.Exists || di.GetFiles().Length == 0) {
                    Narc.Open(workDir + tuple.Item1).ExtractToFolder(tuple.Item2);
                }
            }
            /* Read Header internal names */
            internalNames = new List<string>();
            using (BinaryReader reader = new BinaryReader(File.OpenRead(workDir + @"data\fielddata\maptable\mapname.bin"))) {
                int internalNameLen = 0x10;
                int headerCount = romInfo.GetHeaderCount();

                for (int i = 0; i < headerCount; i++) {
                    reader.BaseStream.Position = i * internalNameLen;
                    byte[] tarGetBytes = reader.ReadBytes(internalNameLen + 1);
                    int bytesLen = tarGetBytes.Length;

                    if (bytesLen <= internalNameLen)
                        tarGetBytes[bytesLen - 1] = 0x00;
                    else
                        tarGetBytes[internalNameLen] = 0x00;

                    string internalName = Encoding.ASCII.GetString(tarGetBytes);//.TrimEnd();
                    headerListBox.Items.Add(i + ": " + internalName);
                    internalNames.Add(internalName);
                }
            }

            /*Add list of options to each control */
            mapNameComboBox.Items.AddRange(LoadMessageArchive(romInfo.GetMapNamesMessageNumber()).messages.ToArray());
            HeaderDatabase headerInfo = new HeaderDatabase();
            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                    areaIconComboBox.Enabled = false;
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject("dpareaicon");
                    cameraComboBox.Items.AddRange(headerInfo.DPPtCameraValues);
                    musicDayComboBox.Items.AddRange(headerInfo.DPMusicValues);
                    musicNightComboBox.Items.AddRange(headerInfo.DPMusicValues);
                    showNameComboBox.Items.AddRange(headerInfo.DPShowNameValues);
                    weatherComboBox.Items.AddRange(headerInfo.DPWeatherValues);
                    break;
                case "Platinum":
                    areaIconComboBox.Items.AddRange(headerInfo.PtAreaIconValues);
                    cameraComboBox.Items.AddRange(headerInfo.DPPtCameraValues);
                    musicDayComboBox.Items.AddRange(headerInfo.PtMusicValues);
                    musicNightComboBox.Items.AddRange(headerInfo.PtMusicValues);
                    showNameComboBox.Items.AddRange(headerInfo.PtShowNameValues);
                    weatherComboBox.Items.AddRange(headerInfo.PtWeatherValues);
                    break;
                default:
                    showNameComboBox.Enabled = false;
                    areaIconComboBox.Items.AddRange(headerInfo.HGSSAreaIconValues);
                    cameraComboBox.Items.AddRange(headerInfo.HGSSCameraValues);
                    musicDayComboBox.Items.AddRange(headerInfo.HGSSMusicValues);
                    musicNightComboBox.Items.AddRange(headerInfo.HGSSMusicValues);
                    weatherComboBox.Items.AddRange(headerInfo.HGSSWeatherValues);
                    break;
            }
            if (headerListBox.Items.Count > 0)
                headerListBox.SelectedIndex = 0;
        }
        private void SetupMapEditor() { 
            /* Extract essential NARCs sub-archives*/
            string[] narcPaths = romInfo.GetNarcPaths();
            string[] extractedNarcDirs = romInfo.GetExtractedNarcDirs();

            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 15;
            toolStripProgressBar.Value = 0;
            statusLabel.Text = "Attempting to unpack Map Editor NARCs... Please wait.";
            Update();

            for (int i = 3; i < 9; i++) {
                var tuple = Tuple.Create(narcPaths[i], extractedNarcDirs[i]);
                DirectoryInfo di = new DirectoryInfo(tuple.Item2);
                if (!di.Exists || di.GetFiles().Length == 0) {
                    Narc.Open(workDir + tuple.Item1).ExtractToFolder(tuple.Item2);
                }
                toolStripProgressBar.Value++;
            }
            if (romInfo.GetGameVersion() == "HeartGold" || romInfo.GetGameVersion() == "SoulSilver") {
                var tuple = Tuple.Create(narcPaths[narcPaths.Length - 1], extractedNarcDirs[extractedNarcDirs.Length - 1]);
                DirectoryInfo di = new DirectoryInfo(tuple.Item2);
                if (!di.Exists || di.GetFiles().Length == 0) {
                    Narc.Open(workDir + tuple.Item1).ExtractToFolder(tuple.Item2);
                }
                toolStripProgressBar.Value++;
            }

            disableHandlers = true;

            mapOpenGlControl.MakeCurrent();
            mapOpenGlControl.MouseWheel += new MouseEventHandler(mapOpenGlControl_MouseWheel);
            collisionPainterPictureBox.Image = new Bitmap(100, 100);
            typePainterPictureBox.Image = new Bitmap(100, 100);
            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    break;
                default:
                    interiorbldRadioButton.Enabled = true;
                    exteriorbldRadioButton.Enabled = true;
                    break;
            };

            /* Add map names to box */
            for (int i = 0; i < romInfo.GetMapCount(); i++) {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(romInfo.GetMapDirPath() + "\\" + i.ToString("D4")))) {
                    switch (romInfo.GetGameVersion()) {
                        case "Diamond":
                        case "Pearl":
                        case "Platinum":
                            reader.BaseStream.Position = 0x10 + reader.ReadUInt32() + reader.ReadUInt32() + 0x34;
                            break;
                        default:
                            reader.BaseStream.Position = 0x12;
                            short bgsSize = reader.ReadInt16();
                            reader.BaseStream.Position = 0x0;
                            reader.BaseStream.Position = 0x14 + bgsSize + reader.ReadUInt32() + reader.ReadUInt32() + 0x34;
                            break;
                    };
                    string nsbmdName = Encoding.UTF8.GetString(reader.ReadBytes(16));
                    selectMapComboBox.Items.Add(i + ": " + nsbmdName);
                }

            }
            toolStripProgressBar.Value++;

            /* Fill building models list */
            buildIndexComboBox.Items.AddRange(GetBuildingsList(false));
            toolStripProgressBar.Value++;

            /*  Fill map textures list */
            mapTextureComboBox.Items.Add("Untextured");
            for (int i = 0; i < romInfo.GetMapTexturesCount(); i++)
                mapTextureComboBox.Items.Add("Texture " + i);
            toolStripProgressBar.Value++;

            /*  Fill building textures list */
            buildTextureComboBox.Items.Add("Untextured");
            for (int i = 0; i < romInfo.GetBuildingTexturesCount(); i++)
                buildTextureComboBox.Items.Add("Texture " + i);
            toolStripProgressBar.Value++;

            /*  Fill collision painters list */
            List<string> collisionPainters = new List<string>()
            {
                "Walkable",
                "Blocked",
                "Grass Sound",
            };
            for (int i = 0; i < 3; i++) collisionPainterComboBox.Items.Add(collisionPainters[i]);

            /*  Fill type painters list */
            List<string> typePainters = new List<string>()
            {
                "[00] None",
                "[02] Tall Grass (Wild)",
                "[03] Very Tall Grass (Wild)",
                "[06] Tree Headbutt (HGSS)",
                "[08] Cave Floor",
                "[0B] Old Château floor",
                "[0C] Ground Mountain",
                "[10] River Water (Wild)",
                "[11] Whirlpool (HGSS)",
                "[13] Waterfall",
                "[15] Sea Water (Wild)",
                "[16] Puddle",
                "[17] Shallow Walkable water",
                "[20] Ice",
                "[21] Sand",
                "[22] Cave Underwater",
                "[24] Safari Zone Border",
                "[2C] Magma",
                "[2D] Reflection",
                "[30] Block Right",
                "[31] Block Left",
                "[32] Block Up",
                "[33] Block Down",
                "[38] Jump Right",
                "[39] Jump Left",
                "[3A] Jump Up (Broken in HGSS)",
                "[3B] Jump Down",
                "[3C] Ladder front",
                "[3D] Ladder back",
                "[3E] Ladder down",
                "[3F] Jump Corner DownLeft",
                "[40] Slide Right",
                "[41] Slide Left",
                "[42] Slide Up",
                "[43] Slide Down",
                "[4B] Horiz Rock Climb",
                "[4C] Vert Rock Climb",
                "[4D] Stop Sliding",
                "[5E] Stairs Warp (Right)",
                "[5F] Stairs Warp (Left)",
                "[62] Warp Entrance (Right)",
                "[63] Warp Entrance (Left)",
                "[64] Warp Entrance (Up)",
                "[65] Warp Entrance (Down)",
                "[67] Warp Panel",
                "[69] Door",
                "[6A] Automatic stairs Down right",
                "[6B] Automatic stairs Up right",
                "[6C] Warp Right",
                "[6D] Warp Left",
                "[6E] Warp Up",
                "[6F] Warp Down",
                "[70] Bridge Start",
                "[71] Bridge Middle",
                "[72] Bridge Over Cave",
                "[73] Bridge Over Water",
                "[75] Bridge Over Snow",
                "[76] Vertical bike bridge",
                "[79] Vertical bike bridge over sand",
                "[7A] Horizontal bike bridge",
                "[7C] Horizontal bike bridge over water",
                "[7D] Horizontal bike bridge over sand",
                "[80] Table",
                "[83] Storage PC",
                "[85] Open TownMap",
                "[86] TV",
                "[A0] Farm Land",
                "[A1] Deep Snow",
                "[A2] Very Deep Snow",
                "[A3] Ultra Deep Snow",
                "[A4] Mud",
                "[A6] Mud Grass",
                "[A8] Snow",
                "[D8] Bike Jump Left",
                "[D9] Bike Slope Top",
                "[DA] Bike Slope Bottom",
                "[DB] Bike parking",
                "[E4] Trash Can",
                "[E5] Shop items"
            };

            for (int i = 0; i < typePainters.Count; i++)
                typePainterComboBox.Items.Add(typePainters[i]);
            toolStripProgressBar.Value++;

            /* Set controls' initial values */
            selectCollisionPanel.BackColor = Color.MidnightBlue;
            typePainterComboBox.SelectedIndex = 0;
            collisionPainterComboBox.SelectedIndex = 1;


            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;
            disableHandlers = false;


            //Default selections
            selectMapComboBox.SelectedIndex = 0;
            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    mapTextureComboBox.SelectedIndex = 7;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
                case "HeartGold":
                case "SoulSilver":
                    mapTextureComboBox.SelectedIndex = 3;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
                default:
                    mapTextureComboBox.SelectedIndex = 2;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
            };
        }
        private void SetupMatrixEditor() {
            string[] narcPaths = romInfo.GetNarcPaths();
            string[] extractedNarcDirs = romInfo.GetExtractedNarcDirs();

            statusLabel.Text = "Setting up Matrix Editor...";
            Update();

            var tuple = Tuple.Create(narcPaths[2], extractedNarcDirs[2]); // 2 = matrixDir
            DirectoryInfo di = new DirectoryInfo(tuple.Item2);
            if (!di.Exists || di.GetFiles().Length == 0) {
                Narc.Open(workDir + tuple.Item1).ExtractToFolder(tuple.Item2);
            }


            disableHandlers = true;

            /* Add matrix entries to ComboBox */
            selectMatrixComboBox.Items.Add("Matrix 0 - Main");
            for (int i = 1; i < romInfo.GetMatrixCount(); i++)
                selectMatrixComboBox.Items.Add("Matrix " + i);

            /* Initialize dictionary of colors corresponding to border maps in the matrix editor */
            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    mapColorsDict = new Dictionary<List<uint>, Tuple<Color, Color>>() {
                        [new List<uint> { 173, 176, 177, 179 }] = Tuple.Create(Color.ForestGreen, Color.White),
                        [new List<uint> { 174 }] = Tuple.Create(Color.SteelBlue, Color.White),
                        [new List<uint> { 175 }] = Tuple.Create(Color.Sienna, Color.White),
                        [new List<uint> { 178 }] = Tuple.Create(Color.PowderBlue, Color.Black),
                        [new List<uint> { Matrix.EMPTY }] = Tuple.Create(Color.Black, Color.White)
                    };
                    break;
                case "HeartGold":
                case "SoulSilver":
                    mapColorsDict = new Dictionary<List<uint>, Tuple<Color, Color>>() {
                        [new List<uint> { 208 }] = Tuple.Create(Color.ForestGreen, Color.White),
                        [new List<uint> { 209 }] = Tuple.Create(Color.SteelBlue, Color.White),
                        [new List<uint> { 210 }] = Tuple.Create(Color.Sienna, Color.White),
                        [new List<uint> { Matrix.EMPTY }] = Tuple.Create(Color.Black, Color.White)
                    };
                    break;
                default:
                    mapColorsDict = new Dictionary<List<uint>, Tuple<Color, Color>>() {
                        [new List<uint> { 203 }] = Tuple.Create(Color.FromArgb(80, 200, 16), Color.White),
                        [new List<uint> { 204, 209 }] = Tuple.Create(Color.SteelBlue, Color.White),
                        [new List<uint> { 205, 206 }] = Tuple.Create(Color.DarkGreen, Color.White),
                        [new List<uint> { 207, 208 }] = Tuple.Create(Color.ForestGreen, Color.White),
                        [new List<uint> { 210 }] = Tuple.Create(Color.Sienna, Color.White),
                        [new List<uint> { Matrix.EMPTY }] = Tuple.Create(Color.Black, Color.White)
                    };
                    break;
            }

            disableHandlers = false;
            selectMatrixComboBox.SelectedIndex = 0;
        }
        private void SetupScriptEditor() {
            /* Extract essential NARCs sub-archives*/
            string[] narcPaths = romInfo.GetNarcPaths();
            string[] extractedNarcDirs = romInfo.GetExtractedNarcDirs();

            statusLabel.Text = "Setting up Script Editor...";
            Update();

            var tuple = Tuple.Create(narcPaths[12], extractedNarcDirs[12]); //12 = scripts Narc Dir
            DirectoryInfo di = new DirectoryInfo(tuple.Item2);
            if (!di.Exists || di.GetFiles().Length == 0) {
                Narc.Open(workDir + tuple.Item1).ExtractToFolder(tuple.Item2);
            }


            int scriptCount = Directory.GetFiles(romInfo.GetScriptDirPath()).Length;
            for (int i = 0; i < scriptCount; i++)
                selectScriptFileComboBox.Items.Add("Script File " + i);

            String exclMSG = "Currently, the script editor is VERY unreliable.\n" +
                "Remember to use it carefully.\n";
            MessageBox.Show(exclMSG, "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            selectScriptFileComboBox.SelectedIndex = 0;
        }
        private void SetupTextEditor() {
            string[] narcPaths = romInfo.GetNarcPaths();
            string[] extractedNarcDirs = romInfo.GetExtractedNarcDirs();

            var tuple = Tuple.Create(narcPaths[2], extractedNarcDirs[2]);
            DirectoryInfo di = new DirectoryInfo(tuple.Item2);
            if (!di.Exists || di.GetFiles().Length == 0) {
                Narc.Open(workDir + tuple.Item1).ExtractToFolder(tuple.Item2);
            }

            statusLabel.Text = "Setting up Text Editor...";
            Update();

            for (int i = 0; i < romInfo.GetTextArchivesCount(); i++)
                selectTextFileComboBox.Items.Add("Text Archive " + i);

            selectTextFileComboBox.SelectedIndex = 0;
        }
        private void SetupTilesetEditor() {
            string[] narcPaths = romInfo.GetNarcPaths();
            string[] extractedNarcDirs = romInfo.GetExtractedNarcDirs();

            statusLabel.Text = "Attempting to unpack Tileset Editor NARCs... Please wait.";
            Update();

            for (int i = 6; i < 9; i++) {
                var tuple = Tuple.Create(narcPaths[i], extractedNarcDirs[i]);
                DirectoryInfo di = new DirectoryInfo(tuple.Item2);
                if (!di.Exists || di.GetFiles().Length == 0) {
                    Narc.Open(workDir + tuple.Item1).ExtractToFolder(tuple.Item2);
                }
            }

            /* Fill Tileset ListBox */
            FillTilesetBox();

            /* Fill AreaData ComboBox */
            int areaDataCount = romInfo.GetAreaDataCount();
            for (int i = 0; i < areaDataCount; i++)
                selectAreaDataComboBox.Items.Add("Area Data " + i);

            /* Enable gameVersion-specific controls */

            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    break;
                default:
                    areaDataDynamicTexturesComboBox.Enabled = true;
                    areaDataAreaTypeComboBox.Enabled = true;
                    break;
            };

            if (selectAreaDataComboBox.Items.Count > 0)
                selectAreaDataComboBox.SelectedIndex = 0;
            if (texturePacksListBox.Items.Count > 0)
                texturePacksListBox.SelectedIndex = 0;
            if (texturesListBox.Items.Count > 0)
                texturesListBox.SelectedIndex = 0;
            if (palettesListBox.Items.Count > 0)
                palettesListBox.SelectedIndex = 0;
        }
        private int UnpackRomCheckUserChoice() {
            // Check if extracted data for the ROM exists, and ask user if they want to load it.
            // Returns true if user aborted the process
            if (Directory.Exists(workDir)) {
                DialogResult d = MessageBox.Show("Extracted data of this ROM has been found.\n" +
                    "Do you want to load it and unpack it?", "Data detected", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (d == DialogResult.Cancel) {
                    return -1; //user wants to abort loading
                } else if (d == DialogResult.Yes) {
                    return 0; //user wants to load data
                } else {
                    DialogResult nd = MessageBox.Show("All data of this ROM will be re-extracted. Proceed?\n",
                        "Existing data will be deleted", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (nd == DialogResult.No) {
                        return -1; //user wants to abort loading
                    } else {
                        return 1; //user wants to re-extract data
                    }
                }
            } else {
                return 2; //No data found
            }
        }
        private void UnpackRom(string ndsFileName) {
            statusLabel.Text = "Unpacking ROM contents to " + workDir + " ...";
            Update();

            Directory.CreateDirectory(workDir);
            Process unpack = new Process();
            unpack.StartInfo.FileName = @"Tools\ndstool.exe";
            unpack.StartInfo.Arguments = "-x " + '"' + ndsFileName + '"'
                + " -9 " + '"' + workDir + "arm9.bin" + '"'
                + " -7 " + '"' + workDir + "arm7.bin" + '"'
                + " -y9 " + '"' + workDir + "y9.bin" + '"'
                + " -y7 " + '"' + workDir + "y7.bin" + '"'
                + " -d " + '"' + workDir + "data" + '"'
                + " -y " + '"' + workDir + "overlay" + '"'
                + " -t " + '"' + workDir + "banner.bin" + '"'
                + " -h " + '"' + workDir + "header.bin" + '"';
            Application.DoEvents();
            unpack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            unpack.StartInfo.CreateNoWindow = true;
            unpack.Start();
            unpack.WaitForExit();
        }
        public static void WriteToArm9(long startOffset, byte[] bytesToWrite) {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(workDir + @"arm9.bin"))) {
                writer.BaseStream.Position = startOffset;
                writer.Write(bytesToWrite, 0, bytesToWrite.Length);
            }
        }
        #endregion

        private void asmHacksToolStripMenuItem_Click(object sender, EventArgs e) {
            using (ROMToolboxDialog window = new ROMToolboxDialog(romInfo)) {
                window.ShowDialog();
                if (window.standardizedItems)
                    isItemRadioButton.Enabled = true;
            }
        }
        private void buildingEditorButton_Click(object sender, EventArgs e) {
            using (BuildingEditor editor = new BuildingEditor(romInfo))
                editor.ShowDialog();
        }
        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e) {
            string message = "DS Pokémon Rom Editor by Nømura (Unofficial Branch)" + Environment.NewLine + "version 1.0.6a" + Environment.NewLine
                + Environment.NewLine + "This tool was largely inspired by Markitus95's Spiky's DS Map Editor, from which certain assets were also recycled. Credits go to Markitus, Ark, Zark, Florian, and everyone else who owes credit for SDSME." + Environment.NewLine +
                "Special thanks go to Trifindo, Mikelan98, BagBoy, and JackHack96, whose help, research and expertise in the field of NDS Rom Hacking made the development of this tool possible.";

            MessageBox.Show(message, "about", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void loadRom_Click(object sender, EventArgs e) {
            OpenFileDialog openRom = new OpenFileDialog(); // Select ROM
            openRom.Filter = "NDS File (*.nds)|*.nds";
            if (openRom.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryReader br = new BinaryReader(File.OpenRead(openRom.FileName))) {
                br.BaseStream.Seek(0xC, SeekOrigin.Begin); // get ROM ID
                gameCode = Encoding.UTF8.GetString(br.ReadBytes(4));
            }
            workDir = Path.GetDirectoryName(openRom.FileName) + "\\" + Path.GetFileNameWithoutExtension(openRom.FileName) + "_DSPRE_contents" + "\\";

            /* Set ROM gameVersion and language */
            romInfo = new RomInfo(gameCode, workDir);
            if (romInfo.GetGameVersion() == null) {
                statusLabel.Text = "Unsupported ROM";
                Update();
                return;
            }

            versionLabel.Text = "ROM: Pokémon " + romInfo.GetGameVersion();
            languageLabel.Text = "Language: " + romInfo.GetGameLanguage();


            int userchoice = UnpackRomCheckUserChoice();
            switch (userchoice) {
                case -1:
                    statusLabel.Text = "Loading aborted";
                    Update();
                    return;
                case 0:
                    break;
                case 1:
                case 2:
                    Application.DoEvents();
                    if (userchoice == 1) {
                        statusLabel.Text = "Deleting old data...";
                        try {
                            Directory.Delete(workDir, true);
                        } catch (DirectoryNotFoundException) {
                            MessageBox.Show("Concurrent access detected: \n" + workDir +
                                "\nIn this case, it's not a problem.\nHowever, always make sure no other process is " +
                                "using the same ROM folder while DSPRE is running.", "Folder has already been deleted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        Update();
                    }

                    try {
                        UnpackRom(openRom.FileName);
                    } catch (IOException) {
                        MessageBox.Show("Can't access temp directory: \n" + workDir + "\nThis might be a temporary issue.\nMake sure no other process is using it and try again.", "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        statusLabel.Text = "Error: concurrent access to " + workDir;
                        Update();
                        return;
                    }
                    break;
            }

            iconON = true;
            gameIcon.Refresh();  // Paint game icon
            statusLabel.Text = "Attempting to unpack NARCs from folder...";
            Update();

            /*foreach (Tuple<string, string> tuple in romInfo.GetNarcPaths().Zip(romInfo.GetExtractedNarcDirs(), Tuple.Create))
                Narc.Open(workDir + tuple.Item1).ExtractToFolder(tuple.Item2);*/

            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    break;
                default:
                    if (!DecompressArm9()) {
                        MessageBox.Show("ARM9 decompression failed. The program can't proceed.\nAborting.",
                                    "Errror with ARM9 decompression", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
            }

            /* Setup essential editors */
            SetupFlagNames();
            SetupHeaderEditor();
            eventOpenGlControl.InitializeContexts();
            mapOpenGlControl.InitializeContexts();

            mainTabControl.Show();
            saveRomButton.Enabled = true;
            unpackAllButton.Enabled = true;
            romToolboxButton.Enabled = true;
            buildingEditorButton.Enabled = true;
            wildEditorButton.Enabled = true;

            loadRomButton.Enabled = false;
            openROMToolStripMenuItem.Enabled = false;

            statusLabel.Text = "Ready";
        }

        private void saveRom_Click(object sender, EventArgs e) {
            SaveFileDialog saveRom = new SaveFileDialog();
            saveRom.Filter = "NDS File (*.nds)|*.nds";
            if (saveRom.ShowDialog(this) != DialogResult.OK)
                return;

            statusLabel.Text = "Repacking NARCS...";
            Update();

            // Repack NARCs
            foreach (var tuple in romInfo.GetNarcPaths().Zip(romInfo.GetExtractedNarcDirs(), Tuple.Create)) {
                DirectoryInfo di = new DirectoryInfo(tuple.Item2);
                if (di.Exists) {
                    Narc.FromFolder(tuple.Item2).Save(workDir + tuple.Item1); // Make new NARC from folder
                }
            }

            if (eventEditorIsReady) {
                switch (romInfo.GetGameVersion()) {
                    case "Diamond":
                    case "Pearl":
                    case "Platinum":
                        break;
                    default:
                        restoreOverlayFromCompressedBackup(1); // Must restore compressed overlay 1 in HGSS, which contains overworld table
                        break;
                }
            }

            statusLabel.Text = "Repacking ROM...";
            Update();
            //DeleteTempFolders();
            RepackRom(saveRom.FileName);

            if (eventEditorIsReady)
                if (romInfo.GetGameVersion() != "Diamond" && romInfo.GetGameVersion() != "Pearl" && romInfo.GetGameVersion() != "Platinum")
                    decompressOverlay(1, true);

            statusLabel.Text = "Ready";
        }

        private void unpackAllButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("Do you wish to unpack all extracted NARCS?\n" +
                "This operation might be long and can't be interrupted.\n" +
                "Any unsaved changes made to the ROM in this session will be lost." +
                "\nProceed?", "About to unpack all NARCS",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                toolStripProgressBar.Maximum = romInfo.GetNarcPaths().Length;
                toolStripProgressBar.Visible = true;
                toolStripProgressBar.Value = 0;
                statusLabel.Text = "Attempting to unpack all NARCs... Be patient. This might take a while...";
                Update();
                foreach (var tuple in romInfo.GetNarcPaths().Zip(romInfo.GetExtractedNarcDirs(), Tuple.Create)) {
                    Narc.Open(workDir + tuple.Item1).ExtractToFolder(tuple.Item2);
                    toolStripProgressBar.Value++;
                }

                MessageBox.Show("Operation completed.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusLabel.Text = "Ready";
                toolStripProgressBar.Value = 0;
                toolStripProgressBar.Visible = false;
                Update();
            }
        }

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (mainTabControl.SelectedTab == headerEditorTabPage) {
                //
            } else if (mainTabControl.SelectedTab == matrixEditorTabPage) {
                if (!matrixEditorIsReady) {
                    SetupMatrixEditor();
                    matrixEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == mapEditorTabPage) {
                if (!mapEditorIsReady) {
                    SetupMapEditor();
                    mapEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == eventEditorTabPage) {
                if (!eventEditorIsReady) {
                    SetupEventEditor();
                    eventEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == scriptEditorTabPage) {
                if (!scriptEditorIsReady) {
                    SetupScriptEditor();
                    scriptEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == textEditorTabPage) {
                if (!textEditorIsReady) {
                    SetupTextEditor();
                    textEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == tilesetEditorTabPage){
                if (!tilesetEditorIsReady) {
                    SetupTilesetEditor();
                    tilesetEditorIsReady = true;
                }
            }
            statusLabel.Text = "Ready";
        }



        private void wildEditorButton_Click(object sender, EventArgs e) {
            openWildEditor();
        }

        private void openWildEditor() {
            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    using (WildEditorDPPt editor = new WildEditorDPPt(romInfo.GetEncounterDirPath(), GetPokémonNames(), (int)wildPokeUpDown.Value))
                        editor.ShowDialog();
                    break;
                default:
                    using (WildEditorHGSS editor = new WildEditorHGSS(romInfo.GetEncounterDirPath(), GetPokémonNames(), (int)wildPokeUpDown.Value))
                        editor.ShowDialog();
                    break;
            }
        }

        private void openWildEditorWithIdButtonClick(object sender, EventArgs e) {
            openWildEditor();
        }
        #endregion

        #region Header Editor

        #region Variables
        public Header currentHeader;
        public List<string> internalNames;

        public Dictionary<int, string> hgssAreaIconImageDict = new Dictionary<int, string>() {
            [0] = "hgsswood",
            [1] = "hgssgray",
            [2] = "hgsswall",
            [3] = "empty",
            [4] = "hgsstown",
            [5] = "hgsscave",
            [6] = "hgssfield",
            [7] = "hgsslake",
            [8] = "hgssforest",
            [9] = "hgsswater",
        };
        public Dictionary<int, string> dpweatherImageDict = new Dictionary<int, string>() {
            [0] = "dpnormal",
            [1] = "dpcloudy",
            [2] = "dprain",
            [3] = "dpheavyrain",
            [4] = "dpthunderstorm",
            [5] = "dpsnowslow",
            [6] = "dpdiamondsnow",
            [7] = "dpblizzard",
            [8] = "dpsandfall",
            [9] = "dpsandstorm",
            [10] = "dphail",
            [11] = "dprocksascending",
            [12] = "dpfog",
            [13] = "dpfog",
            [14] = "dpdark",
            [15] = "dplightning",
            [16] = "dplightsandstorm"
        };
        public Dictionary<int, string> ptweatherImageDict = new Dictionary<int, string>() {
            [0] = "ptnormal",
            [1] = "ptcloudy",
            [2] = "ptrain",
            [3] = "ptheavyrain",
            [4] = "ptthunderstorm",
            [5] = "ptsnowslow",
            [6] = "ptdiamondsnow",
            [7] = "ptblizzard",
            [8] = "ptsandfall",
            [9] = "ptsandstorm",
            [10] = "pthail",
            [11] = "ptrocksascending",
            [12] = "ptfog",
            [13] = "ptfog",
            [14] = "ptdark",
            [15] = "ptlightning",
            [16] = "ptlightsandstorm",
            [17] = "ptforestweather",
            [18] = "ptspotlight",
            [19] = "ptspotlight"
        };
        public Dictionary<int, string> hgssweatherImageDict = new Dictionary<int, string>() {
            [0] = "hgssnormal",
            [1] = "hgssrain",
            [2] = "hgsssnow",
            [3] = "hgsshail",
            [4] = "hgssfog",
            [5] = "hgssdark",
            [6] = "hgssdark2"
        };
        #endregion

        #region Subroutines
        public Header LoadHeader(int headerNumber) {
            /* Calculate header offset and load data */
            long headerOffset = romInfo.GetHeaderTableOffset() + 0x18 * headerNumber;
            byte[] headerData = ReadFromArm9(headerOffset, 24);

            /* Encapsulate header data into the class appropriate for the game gameVersion */

            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                    return new HeaderDP(new MemoryStream(headerData));
                case "Platinum":
                    return new HeaderPt(new MemoryStream(headerData));
                default:
                    return new HeaderHGSS(new MemoryStream(headerData));
            }
        }
        #endregion

        private void areaDataUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;
            currentHeader.areaDataID = (byte)areaDataUpDown.Value;
        }
        private void areaIconComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            string imageName;
            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                    break;
                case "Platinum":
                    ((HeaderPt)currentHeader).areaIcon = (byte)areaIconComboBox.SelectedIndex;
                    imageName = "areaicon0" + areaIconComboBox.SelectedIndex.ToString();
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                    break;
                default:
                    ((HeaderHGSS)currentHeader).areaIcon = Byte.Parse(areaIconComboBox.SelectedItem.ToString().Substring(1, 3));
                    imageName = hgssAreaIconImageDict[areaIconComboBox.SelectedIndex];
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                    break;
            }
        }
        private void cameraComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            string imageName;
            try {
                switch (romInfo.GetGameVersion()) {
                    case "Diamond":
                    case "Pearl":
                        currentHeader.camera = (byte)cameraComboBox.SelectedIndex;
                        imageName = "dpcamera" + cameraComboBox.SelectedIndex.ToString();
                        break;
                    case "Platinum":
                        currentHeader.camera = (byte)cameraComboBox.SelectedIndex;
                        imageName = "ptcamera" + cameraComboBox.SelectedIndex.ToString();
                        break;
                    default:
                        currentHeader.camera = Byte.Parse(cameraComboBox.SelectedItem.ToString().Substring(1, 3));
                        imageName = "hgsscamera" + currentHeader.camera.ToString("D3");
                        break;
                }
                cameraPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
            } catch (NullReferenceException) {
                MessageBox.Show("The current header uses an unrecognized camera.\nThis is not a problem. Settings will be saved normally.", "Unknown camera settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void eventFileUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            currentHeader.eventID = (ushort)eventFileUpDown.Value;
        }
        private void headerFlagsCheckBoxes_CheckedChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            int n;

            if (sender == flag7CheckBox)
                n = 7;
            else if (sender == flag6CheckBox)
                n = 6;
            else if (sender == flag5CheckBox)
                n = 5;
            else if (sender == flag4CheckBox)
                n = 4;
            else if (sender == flag3CheckBox)
                n = 3;
            else if (sender == flag2CheckBox)
                n = 2;
            else if (sender == flag1CheckBox)
                n = 1;
            else
                n = 0;

            /* Toggle flag-specific bit */
            int flags = currentHeader.flags;
            flags ^= 1 << n;
            currentHeader.flags = (byte)flags;
        }
        private void headerListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            String currentInternalName = headerListBox.SelectedItem.ToString();
            const String separator = ": ";
            int separatorPosition = currentInternalName.IndexOf(separator);
            currentHeader = LoadHeader(Int32.Parse(currentInternalName.Substring(0, separatorPosition)));

            /* Setup controls for common fields across headers */
            internalNameBox.Text = currentInternalName.Substring(separatorPosition + separator.Length);
            matrixUpDown.Value = currentHeader.matrix;
            areaDataUpDown.Value = currentHeader.areaDataID;
            scriptFileUpDown.Value = currentHeader.script;
            levelScriptUpDown.Value = currentHeader.levelScript;
            eventFileUpDown.Value = currentHeader.eventID;
            textFileUpDown.Value = currentHeader.text;
            wildPokeUpDown.Value = currentHeader.wildPokémon;
            weatherComboBox.SelectedIndex = weatherComboBox.FindString("[" + currentHeader.weather.ToString("D2"));
            cameraComboBox.SelectedIndex = cameraComboBox.FindString("[" + currentHeader.camera.ToString("D3"));

            /* Flags */
            int i = 7;
            disableHandlers = true;
            foreach (Control cBox in flagsGroupBox.Controls) {
                ((CheckBox)cBox).Checked = false;

                if ((currentHeader.flags & (1 << i)) != 0)
                    ((CheckBox)cBox).Checked = true;
                i--;
            }
            disableHandlers = false;

            /* Setup controls for fields with gameVersion-specific differences */
            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                    mapNameComboBox.SelectedIndex = ((HeaderDP)currentHeader).mapName;
                    musicDayComboBox.SelectedIndex = musicDayComboBox.FindString("[" + ((HeaderDP)currentHeader).musicDay.ToString());
                    musicNightComboBox.SelectedIndex = musicNightComboBox.FindString("[" + ((HeaderDP)currentHeader).musicNight.ToString());
                    showNameComboBox.SelectedIndex = showNameComboBox.FindString("[" + $"{currentHeader.showName:D3}");
                    break;
                case "Platinum":
                    areaIconComboBox.SelectedIndex = ((HeaderPt)currentHeader).areaIcon;
                    mapNameComboBox.SelectedIndex = ((HeaderPt)currentHeader).mapName;
                    musicDayComboBox.SelectedIndex = musicDayComboBox.FindString("[" + ((HeaderPt)currentHeader).musicDay.ToString());
                    musicNightComboBox.SelectedIndex = musicNightComboBox.FindString("[" + ((HeaderPt)currentHeader).musicNight.ToString());
                    showNameComboBox.SelectedIndex = showNameComboBox.FindString("[" + $"{currentHeader.showName:D3}");
                    break;
                default:
                    areaIconComboBox.SelectedIndex = areaIconComboBox.FindString("[" + $"{((HeaderHGSS)currentHeader).areaIcon:D3}");
                    mapNameComboBox.SelectedIndex = ((HeaderHGSS)currentHeader).mapName;
                    musicDayComboBox.SelectedIndex = musicDayComboBox.FindString("[" + ((HeaderHGSS)currentHeader).musicDay.ToString());
                    musicNightComboBox.SelectedIndex = musicNightComboBox.FindString("[" + ((HeaderHGSS)currentHeader).musicNight.ToString());
                    break;
            }
        }
        private void headerListBox_Leave(object sender, EventArgs e) {
            if (disableHandlers) return;
            headerListBox.Refresh();
        }
        private void levelScriptUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            currentHeader.levelScript = (ushort)levelScriptUpDown.Value;
        }
        private void mapNameComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                    ((HeaderDP)currentHeader).mapName = (ushort)mapNameComboBox.SelectedIndex;
                    break;
                case "Platinum":
                    ((HeaderPt)currentHeader).mapName = (byte)mapNameComboBox.SelectedIndex;
                    break;
                default:
                    ((HeaderHGSS)currentHeader).mapName = (byte)mapNameComboBox.SelectedIndex;
                    break;
            }
        }
        private void matrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            currentHeader.matrix = (ushort)matrixUpDown.Value;
        }
        private void musicDayComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                    ((HeaderDP)currentHeader).musicDay = UInt16.Parse(musicDayComboBox.SelectedItem.ToString().Substring(1, 4));
                    break;
                case "Platinum":
                    ((HeaderPt)currentHeader).musicDay = UInt16.Parse(musicDayComboBox.SelectedItem.ToString().Substring(1, 4));
                    break;
                default:
                    ((HeaderHGSS)currentHeader).musicDay = UInt16.Parse(musicDayComboBox.SelectedItem.ToString().Substring(1, 4));
                    break;
            }
        }
        private void musicNightComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                    ((HeaderDP)currentHeader).musicNight = UInt16.Parse(musicNightComboBox.SelectedItem.ToString().Substring(1, 4));
                    break;
                case "Platinum":
                    ((HeaderPt)currentHeader).musicNight = UInt16.Parse(musicNightComboBox.SelectedItem.ToString().Substring(1, 4));
                    break;
                default:
                    ((HeaderHGSS)currentHeader).musicNight = UInt16.Parse(musicNightComboBox.SelectedItem.ToString().Substring(1, 4));
                    break;
            }
        }
        private void openAreaDataButton_Click(object sender, EventArgs e) {
            if (!tilesetEditorIsReady) {
                SetupTilesetEditor();
                tilesetEditorIsReady = true;
            }

            selectAreaDataComboBox.SelectedIndex = (int)areaDataUpDown.Value;
            texturePacksListBox.SelectedIndex = (mapTilesetRadioButton.Checked ? (int)areaDataMapTilesetUpDown.Value : (int)areaDataBuildingTilesetUpDown.Value);
            mainTabControl.SelectedTab = tilesetEditorTabPage;

            if (texturesListBox.Items.Count > 0)
                texturesListBox.SelectedIndex = 0;
            if (palettesListBox.Items.Count > 0)
                palettesListBox.SelectedIndex = 0;
        }
        private void openEventsButton_Click(object sender, EventArgs e) {
            if (!eventEditorIsReady) {
                SetupEventEditor();
                eventEditorIsReady = true;
            }

            if (matrixUpDown.Value != 0)
                eventAreaDataUpDown.Value = areaDataUpDown.Value; // Use Area Data for textures if matrix is not 0

            eventMatrixUpDown.Value = matrixUpDown.Value; // Open the right matrix in event editor
            selectEventComboBox.SelectedIndex = (int)eventFileUpDown.Value; // Select event file
            mainTabControl.SelectedTab = eventEditorTabPage;

            centerEventviewOnEntities();
            eventMatrixXUpDown_ValueChanged(null, null);
        }

        private void centerEventviewOnEntities() {
            disableHandlers = true;
            if (currentEventFile.overworlds.Count > 0) {
                eventMatrixXUpDown.Value = currentEventFile.overworlds[0].xMatrixPosition;
                eventMatrixYUpDown.Value = currentEventFile.overworlds[0].yMatrixPosition;
            } else if (currentEventFile.warps.Count > 0) {
                eventMatrixXUpDown.Value = currentEventFile.warps[0].xMatrixPosition;
                eventMatrixYUpDown.Value = currentEventFile.warps[0].yMatrixPosition;
            } else if (currentEventFile.spawnables.Count > 0) {
                eventMatrixXUpDown.Value = currentEventFile.spawnables[0].xMatrixPosition;
                eventMatrixYUpDown.Value = currentEventFile.spawnables[0].yMatrixPosition;
            } else if (currentEventFile.triggers.Count > 0) {
                eventMatrixXUpDown.Value = currentEventFile.triggers[0].xMatrixPosition;
                eventMatrixYUpDown.Value = currentEventFile.triggers[0].yMatrixPosition;
            } else {
                eventMatrixXUpDown.Value = 0;
                eventMatrixYUpDown.Value = 0;
            }
            disableHandlers = false;
        }

        private void goToEventButtons_Click(object sender, EventArgs e) {
            centerEventviewOnSelectedEvent();
        }

        private void centerEventviewOnSelectedEvent() {
            if (selectedEvent == null) {
                MessageBox.Show("You haven't selected any event.", "Nothing to do here",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                eventMatrixXUpDown.Value = selectedEvent.xMatrixPosition;
                eventMatrixYUpDown.Value = selectedEvent.yMatrixPosition;
                Update();
            }
        }
        private bool isEventOnCurrentMatrix(Event ev) {
            if (ev.xMatrixPosition == eventMatrixXUpDown.Value)
                if (ev.yMatrixPosition == eventMatrixYUpDown.Value)
                    return true;
            return false;
        }

        private void destinationWarpGoToButton_Click(object sender, EventArgs e) {
            goToWarpDestination();
        }

        private void goToWarpDestination() {
            int destAnchor = (int)warpAnchorUpDown.Value;
            int destHeader = (int)warpHeaderUpDown.Value;
            ushort destEventID = LoadHeader(destHeader).eventID;
            EventFile destEvent = LoadEventFile(destEventID);

            if (destEvent.warps.Count < destAnchor + 1) {
                DialogResult d = MessageBox.Show("The selected warp's destination anchor doesn't exist.\n" +
                    "Do you want to open the destination map anyway?", "Warp is not connected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (d == DialogResult.No)
                    return;
                else {
                    eventMatrixUpDown.Value = LoadHeader((int)warpHeaderUpDown.Value).matrix;
                    eventAreaDataUpDown.Value = LoadHeader((int)warpHeaderUpDown.Value).areaDataID;
                    selectEventComboBox.SelectedIndex = destEventID;
                    centerEventviewOnEntities();
                    return;
                }
            }
            eventMatrixUpDown.Value = LoadHeader((int)warpHeaderUpDown.Value).matrix;
            eventAreaDataUpDown.Value = LoadHeader((int)warpHeaderUpDown.Value).areaDataID;
            selectEventComboBox.SelectedIndex = destEventID;
            warpsListBox.SelectedIndex = destAnchor;
            centerEventviewOnSelectedEvent();
        }

        private void openMatrixButton_Click(object sender, EventArgs e) {
            if (!matrixEditorIsReady) {
                SetupMatrixEditor();
                matrixEditorIsReady = true;
            }
            mainTabControl.SelectedTab = matrixEditorTabPage;
            int matrixNumber = (int)matrixUpDown.Value;
            selectMatrixComboBox.SelectedIndex = matrixNumber;
        }
        private void openTextArchiveButton_Click(object sender, EventArgs e) {
            if (!textEditorIsReady) {
                SetupTextEditor();
                textEditorIsReady = true;
            }
            selectTextFileComboBox.SelectedIndex = (int)textFileUpDown.Value;
            mainTabControl.SelectedTab = textEditorTabPage;
        }
        private void saveHeaderButton_Click(object sender, EventArgs e) {
            long headerOffset = romInfo.GetHeaderTableOffset() + 0x18 * headerListBox.SelectedIndex;
            WriteToArm9(headerOffset, currentHeader.SaveHeader());
        }
        private void resetButton_Click(object sender, EventArgs e) {
            if (headerListBox.Items.Count < internalNames.Count)
                resetHeaderSearchResults();
        }

        private void resetHeaderSearchResults() {
            headerListBox.Enabled = true;
            searchLocationTextBox.Clear();
            headerListBox.Items.Clear();
            for (int i = 0; i < internalNames.Count; i++) {
                String name = internalNames[i];
                headerListBox.Items.Add(i + ": " + name);
            }
        }
        private void searchHeaderTextBox_KeyPress(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                startSearchGameLocation();
            }

        }
        private void searchHeaderButton_Click(object sender, EventArgs e) {
            startSearchGameLocation();
        }

        private void startSearchGameLocation() {
            if (searchLocationTextBox.Text.Length != 0) {
                headerListBox.Items.Clear();
                bool empty = true;


                switch (romInfo.GetGameVersion()) {
                    case "Diamond":
                    case "Pearl":
                        for (int i = 0; i < internalNames.Count; i++) {
                            String locationName = mapNameComboBox.Items[((HeaderDP)LoadHeader(i)).mapName].ToString();
                            if (locationName.IndexOf(searchLocationTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                                headerListBox.Items.Add(i + ": " + internalNames[i]);
                                empty = false;
                            }
                        }
                        break;
                    case "Platinum":
                        for (int i = 0; i < internalNames.Count; i++) {
                            String locationName = mapNameComboBox.Items[((HeaderPt)LoadHeader(i)).mapName].ToString();
                            if (locationName.IndexOf(searchLocationTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                                headerListBox.Items.Add(i + ": " + internalNames[i]);
                                empty = false;
                            }
                        }
                        break;
                    case "HeartGold":
                    case "SoulSilver":
                        for (int i = 0; i < internalNames.Count; i++) {
                            String locationName = mapNameComboBox.Items[((HeaderHGSS)LoadHeader(i)).mapName].ToString();
                            if (locationName.IndexOf(searchLocationTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                                headerListBox.Items.Add(i + ": " + internalNames[i]);
                                empty = false;
                            }
                        }
                        break;
                }
                if (empty) {
                    headerListBox.Items.Add("No Result for " + '"' + searchLocationTextBox.Text + '"');
                    headerListBox.Enabled = false;
                } else {
                    headerListBox.SelectedIndex = 0;
                    headerListBox.Enabled = true;
                }
            } else if (headerListBox.Items.Count < internalNames.Count) {
                resetHeaderSearchResults();
            }
        }

        private void scriptFileUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            currentHeader.script = (ushort)scriptFileUpDown.Value;
        }
        private void showNameComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;
            currentHeader.showName = Byte.Parse(showNameComboBox.SelectedItem.ToString().Substring(1, 3));
        }
        private void textFileUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            currentHeader.text = (ushort)textFileUpDown.Value;
        }
        private void weatherComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            currentHeader.weather = Byte.Parse(weatherComboBox.SelectedItem.ToString().Substring(1, 2));

            string imageName;
            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                    imageName = dpweatherImageDict[weatherComboBox.SelectedIndex];
                    break;
                case "Platinum":
                    imageName = ptweatherImageDict[weatherComboBox.SelectedIndex];
                    break;
                default:
                    imageName = hgssweatherImageDict[weatherComboBox.SelectedIndex];
                    break;
            }

            weatherPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
        }
        private void wildPokeUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            currentHeader.wildPokémon = (ushort)wildPokeUpDown.Value;
        }
        #endregion

        #region Matrix Editor

        #region Variables
        Matrix currentMatrix;
        public Dictionary<List<uint>, Tuple<Color, Color>> mapColorsDict;
        #endregion

        #region Subroutines
        private void Clear_Matrix_Tables() {
            headersGridView.Rows.Clear();
            headersGridView.Columns.Clear();
            heightsGridView.Rows.Clear();
            heightsGridView.Columns.Clear();
            mapFilesGridView.Rows.Clear();
            mapFilesGridView.Columns.Clear();
            matrixTabControl.TabPages.Remove(headersTabPage);
            matrixTabControl.TabPages.Remove(heightsTabPage);
        }
        private Tuple<Color, Color> Format_Map_Cell(uint cellValue) {
            foreach (KeyValuePair<List<uint>, Tuple<Color, Color>> entry in mapColorsDict) {
                if (entry.Key.Contains(cellValue)) return entry.Value;
            }
            return Tuple.Create(Color.White, Color.Black);
        }
        private void Generate_Matrix_Tables() {
            /* Generate table columns */
            for (int i = 0; i < currentMatrix.width; i++) {
                headersGridView.Columns.Add("Column" + i, i.ToString("D"));
                headersGridView.Columns[i].Width = 34; // Set column size
                headersGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

                heightsGridView.Columns.Add("Column" + i, i.ToString("D"));
                heightsGridView.Columns[i].Width = 22; // Set column size
                heightsGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

                mapFilesGridView.Columns.Add("Column" + i, i.ToString("D"));
                mapFilesGridView.Columns[i].Width = 34; // Set column size
                mapFilesGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

            }

            /* Generate table rows */
            for (int i = 0; i < currentMatrix.height; i++) {
                mapFilesGridView.Rows.Add();
                mapFilesGridView.Rows[i].HeaderCell.Value = i.ToString();

                headersGridView.Rows.Add();
                headersGridView.Rows[i].HeaderCell.Value = i.ToString();

                heightsGridView.Rows.Add();
                heightsGridView.Rows[i].HeaderCell.Value = i.ToString();
            }

            /* Fill tables */
            for (int i = 0; i < currentMatrix.height; i++) {
                for (int j = 0; j < currentMatrix.width; j++) {
                    headersGridView.Rows[i].Cells[j].Value = currentMatrix.headers[i, j];
                    heightsGridView.Rows[i].Cells[j].Value = currentMatrix.altitudes[i, j];
                    mapFilesGridView.Rows[i].Cells[j].Value = currentMatrix.maps[i, j];
                }
            }

            if (currentMatrix.hasHeadersSection) matrixTabControl.TabPages.Add(headersTabPage);
            if (currentMatrix.hasAltitudesSection) matrixTabControl.TabPages.Add(heightsTabPage);
        }
        #endregion

        private void addHeadersButton_Click(object sender, EventArgs e) {
            if (currentMatrix.hasHeadersSection) return;
            else {
                currentMatrix.AddHeadersSection();
                matrixTabControl.TabPages.Add(headersTabPage);
            }
        }
        private void addHeightsButton_Click(object sender, EventArgs e) {
            if (currentMatrix.hasAltitudesSection) return;
            else {
                currentMatrix.AddHeightsSection();
                matrixTabControl.TabPages.Add(heightsTabPage);
            }
        }
        private void addMatrixButton_Click(object sender, EventArgs e) {
            /* Load new matrix, a copy of Matrix 0 */
            Matrix newMatrix = LoadMatrix(0);

            /* Add new matrix file to matrix folder */
            string matrixPath = romInfo.GetMatrixDirPath() + "\\" + romInfo.GetMatrixCount().ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(matrixPath, FileMode.Create))) writer.Write(newMatrix.Save());

            /* Update ComboBox*/
            selectMatrixComboBox.Items.Add("Matrix " + romInfo.GetMatrixCount());
        }
        private void exportMatrixButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Matrix File (*.mtx)|*.mtx";
            if (sf.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(new FileStream(sf.FileName, FileMode.Create))) writer.Write(currentMatrix.Save());
        }
        private void headersGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (headerListBox.Items.Count < internalNames.Count)
                resetHeaderSearchResults();
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) {
                int headerNumber = Convert.ToInt32(headersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                headerListBox.SelectedIndex = headerNumber;
                mainTabControl.SelectedTab = headerEditorTabPage;
            }
        }
        private void headersGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (disableHandlers)
                return;
            if (e.RowIndex > -1 && e.ColumnIndex > -1) {
                /* If input is junk, use 0000 as placeholder value */
                ushort cellValue;
                if (!UInt16.TryParse(headersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out cellValue)) cellValue = 0;

                /* Change value in matrix object */
                currentMatrix.headers[e.RowIndex, e.ColumnIndex] = cellValue;
            }
        }
        private void headersGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.Value == null) return;
            disableHandlers = true;

            /* Format table cells corresponding to border maps or void */
            ushort colorValue;
            if (!UInt16.TryParse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out colorValue)) colorValue = Matrix.EMPTY;

            Tuple<Color, Color> cellColors = Format_Map_Cell(colorValue);
            e.CellStyle.BackColor = cellColors.Item1;
            e.CellStyle.ForeColor = cellColors.Item2;

            /* If invalid input is entered, show 00 */
            ushort cellValue;
            if (!UInt16.TryParse(headersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out cellValue)) e.Value = 0;

            disableHandlers = false;

        }
        private void heightsGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (disableHandlers) return;
            if (e.RowIndex > -1 && e.ColumnIndex > -1) {
                /* If input is junk, use 00 as placeholder value */
                byte cellValue;
                if (!Byte.TryParse(heightsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out cellValue)) cellValue = 0;

                /* Change value in matrix object */
                currentMatrix.altitudes[e.RowIndex, e.ColumnIndex] = cellValue;
            }
        }
        private void heightsGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.Value == null)
                return;
            disableHandlers = true;

            /* Format table cells corresponding to border maps or void */
            ushort colorValue;
            if (!UInt16.TryParse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out colorValue)) colorValue = Matrix.EMPTY;

            Tuple<Color, Color> cellColors = Format_Map_Cell(colorValue);
            e.CellStyle.BackColor = cellColors.Item1;
            e.CellStyle.ForeColor = cellColors.Item2;

            /* If invalid input is entered, show 00 */
            byte cellValue;
            if (!Byte.TryParse(heightsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out cellValue)) e.Value = 0;

            disableHandlers = false;

        }
        private void importMatrixButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .mtx file */
            DialogResult d;
            if (selectMatrixComboBox.SelectedIndex == 0) {
                d = MessageBox.Show("Replacing a matrix - especially Matrix 0 - with a new file is risky.\nDo not do it unless you are absolutely sure.\nProceed?", "Risky operation",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (d == DialogResult.No)
                    return;
            }

            OpenFileDialog importMatrix = new OpenFileDialog();
            importMatrix.Filter = "Matrix File (*.mtx)|*.mtx";
            if (importMatrix.ShowDialog(this) != DialogResult.OK)
                return;

            /* Update matrix object in memory */
            currentMatrix = new Matrix(new FileStream(importMatrix.FileName, FileMode.Open));

            /* Refresh DataGridView tables */
            Clear_Matrix_Tables();
            Generate_Matrix_Tables();

            /* Setup matrix editor controls */
            disableHandlers = true;
            matrixNameTextBox.Text = currentMatrix.name;
            widthUpDown.Value = currentMatrix.width;
            heightUpDown.Value = currentMatrix.height;
            disableHandlers = false;

        }
        private void heightUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            disableHandlers = true;

            /* Add or remove rows in DataGridView control */
            int delta = (int)heightUpDown.Value - currentMatrix.height;
            for (int i = 0; i < Math.Abs(delta); i++) {
                if (delta < 0) // Remove rows
                {
                    headersGridView.Rows.RemoveAt(currentMatrix.height - 1 - i);
                    heightsGridView.Rows.RemoveAt(currentMatrix.height - 1 - i);
                    mapFilesGridView.Rows.RemoveAt(currentMatrix.height - 1 - i);
                } else // Add rows
                  {
                    /* Add row in DataGridView */
                    headersGridView.Rows.Add();
                    heightsGridView.Rows.Add();
                    mapFilesGridView.Rows.Add();

                    /* Add row header */
                    headersGridView.Rows[currentMatrix.height + i].HeaderCell.Value = (currentMatrix.height + i + 1).ToString();
                    heightsGridView.Rows[currentMatrix.height + i].HeaderCell.Value = (currentMatrix.height + i + 1).ToString();
                    mapFilesGridView.Rows[currentMatrix.height + i].HeaderCell.Value = (currentMatrix.height + i + 1).ToString();

                    /* Fill new rows */
                    for (int j = 0; j < currentMatrix.width; j++) {
                        headersGridView.Rows[currentMatrix.height + i].Cells[j].Value = 0;
                        heightsGridView.Rows[currentMatrix.height + i].Cells[j].Value = 0;
                        mapFilesGridView.Rows[currentMatrix.height + i].Cells[j].Value = Matrix.EMPTY;
                    }
                }
            }

            /* Modify matrix object */
            currentMatrix.ResizeMatrix((int)heightUpDown.Value, (int)widthUpDown.Value);

            disableHandlers = false;
        }
        private void mapFilesGridView_CellMouseDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (!mapEditorIsReady) {
                SetupMapEditor();
                mapEditorIsReady = true;
            }

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) {
                if (currentMatrix.maps[e.RowIndex, e.ColumnIndex] == Matrix.EMPTY) {
                    MessageBox.Show("You can't load an empty map.\nSelect a valid map and try again.\n" +
                        "If you only meant to change the value of this cell, wait some time between one mouse click and the other.\n" +
                        "Alternatively, highlight the cell and press F2 on your keyboard.",
                        "User attempted to load VOID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                /* Determine area data */
                int header;
                if (currentMatrix.hasHeadersSection) {
                    header = currentMatrix.headers[e.RowIndex, e.ColumnIndex];
                } else {
                    header = headerListBox.SelectedIndex;
                }

                AreaData areaData;
                if (header > internalNames.Count) {
                    MessageBox.Show("This map is associated to a non-existent header.\nThis will lead to unpredictable behaviour and, possibily, problems, if you attempt to load it in game.",
                        "Invalid header", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    header = 0;
                }

                /* get texture file numbers from area data */
                areaData = LoadAreaData(LoadHeader(header).areaDataID);
                /* Load Map File and switch to Map Editor tab */
                disableHandlers = true;

                selectMapComboBox.SelectedIndex = currentMatrix.maps[e.RowIndex, e.ColumnIndex];
                mapTextureComboBox.SelectedIndex = areaData.mapTileset + 1;
                buildTextureComboBox.SelectedIndex = areaData.buildingsTileset + 1;
                mainTabControl.SelectedTab = mapEditorTabPage;

                if (mapPartsTabControl.SelectedTab == permissionsTabPage) //what's this IF for??


                    if (areaData.areaType == 0)
                        interiorbldRadioButton.Checked = true;

                disableHandlers = false;
                selectMapComboBox_SelectedIndexChanged(null, null);
            }
        }
        private void mapFilesGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (disableHandlers)
                return;
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) {
                /* If input is junk, use '\' (FF FF) as placeholder value */
                ushort cellValue = Matrix.EMPTY;
                try {
                    cellValue = UInt16.Parse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                } catch { }

                /* Change value in matrix object */
                currentMatrix.maps[e.RowIndex, e.ColumnIndex] = cellValue;
            }
        }
        private void mapFilesGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            disableHandlers = true;

            /* Format table cells corresponding to border maps or void */
            ushort colorValue = Matrix.EMPTY;
            try {
                colorValue = UInt16.Parse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            } catch { }

            Tuple<Color, Color> cellColors = Format_Map_Cell(colorValue);
            e.CellStyle.BackColor = cellColors.Item1;
            e.CellStyle.ForeColor = cellColors.Item2;

            if (colorValue == Matrix.EMPTY)
                e.Value = '-';

            disableHandlers = false;

        }
        private void matrixNameTextBox_TextChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            currentMatrix.name = matrixNameTextBox.Text;
        }
        private void removeHeadersButton_Click(object sender, EventArgs e) {
            matrixTabControl.TabPages.Remove(headersTabPage);
            currentMatrix.RemoveHeadersSection();
        }
        private void removeHeightsButton_Click(object sender, EventArgs e) {
            matrixTabControl.TabPages.Remove(heightsTabPage);
            currentMatrix.RemoveHeightsSection();
        }
        private void removeMatrixButton_Click(object sender, EventArgs e) {
            if (selectMatrixComboBox.Items.Count > 0) {
                /* Delete matrix file */
                string matrixPath = romInfo.GetMatrixDirPath() + "\\" + (romInfo.GetMatrixCount() - 1).ToString("D4");
                File.Delete(matrixPath);

                /* Change selected index if the matrix to be deleted is currently selected */
                if (selectMatrixComboBox.SelectedIndex == romInfo.GetMatrixCount() - 1) selectMatrixComboBox.SelectedIndex--;

                /* Remove entry from ComboBox, and decrease matrix count */
                selectMatrixComboBox.Items.RemoveAt(romInfo.GetMatrixCount() - 1);
            }
        }
        private void saveMatrixButton_Click(object sender, EventArgs e) {
            string matrixPath = romInfo.GetMatrixDirPath() + "\\" + selectMatrixComboBox.SelectedIndex.ToString("D4");
            using (BinaryWriter matrixWriter = new BinaryWriter(new FileStream(matrixPath, FileMode.Create)))
                matrixWriter.Write(currentMatrix.Save());

            eventMatrix = LoadMatrix(selectMatrixComboBox.SelectedIndex);
        }
        private void selectMatrixComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            Clear_Matrix_Tables();
            currentMatrix = LoadMatrix(selectMatrixComboBox.SelectedIndex);
            Generate_Matrix_Tables();

            /* Setup matrix editor controls */
            disableHandlers = true;
            matrixNameTextBox.Text = currentMatrix.name;
            widthUpDown.Value = currentMatrix.width;
            heightUpDown.Value = currentMatrix.height;
            disableHandlers = false;
        }
        private void widthUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;
            disableHandlers = true;

            /* Add or remove rows in DataGridView control */
            int delta = (int)widthUpDown.Value - currentMatrix.width;
            for (int i = 0; i < Math.Abs(delta); i++) {
                if (delta < 0) {
                    headersGridView.Columns.RemoveAt(currentMatrix.width - 1 - i);
                    heightsGridView.Columns.RemoveAt(currentMatrix.width - 1 - i);
                    mapFilesGridView.Columns.RemoveAt(currentMatrix.width - 1 - i);
                } else {
                    /* Add columns */
                    headersGridView.Columns.Add(" ", (currentMatrix.width + i).ToString());
                    heightsGridView.Columns.Add(" ", (currentMatrix.width + i).ToString());
                    mapFilesGridView.Columns.Add(" ", (currentMatrix.width + i).ToString());

                    /* Adjust column width */
                    headersGridView.Columns[currentMatrix.width + i].Width = 34;
                    heightsGridView.Columns[currentMatrix.width + i].Width = 22;
                    mapFilesGridView.Columns[currentMatrix.width + i].Width = 34;

                    /* Fill new rows */
                    for (int j = 0; j < currentMatrix.height; j++) {
                        headersGridView.Rows[j].Cells[currentMatrix.width + i].Value = 0;
                        heightsGridView.Rows[j].Cells[currentMatrix.width + i].Value = 0;
                        mapFilesGridView.Rows[j].Cells[currentMatrix.width + i].Value = Matrix.EMPTY;
                    }
                }
            }

            /* Modify matrix object */
            currentMatrix.ResizeMatrix((int)heightUpDown.Value, (int)widthUpDown.Value);

            disableHandlers = false;
        }
        #endregion

        #region Map Editor

        #region Variables
        /*  Camera settings */
        public bool hideBuildings = new bool();
        public bool mapTexturesOn = true;
        public bool buildingTexturesOn = true;
        public static float ang = 0.0f;
        public static float dist = 12.8f;
        public static float elev = 50.0f;
        public float perspective = 45f;

        /* Renderers */
        public static NSBMDGlRenderer mapRenderer = new NSBMDGlRenderer();
        public static NSBMDGlRenderer buildingsRenderer = new NSBMDGlRenderer();

        /* Map file */
        MapFile currentMapFile;

        /* Permission painters */
        public Pen paintPen;
        public SolidBrush paintBrush;
        public SolidBrush textBrush;
        public byte paintByte;
        StringFormat sf;
        public Rectangle mainCell;
        public Rectangle smallCell;
        public Rectangle painterBox = new Rectangle(0, 0, 100, 100);
        public Font textFont;
        #endregion

        #region Subroutines
        private void FillBuildingsBox() {
            buildingsListBox.Items.Clear();
            for (int i = 0; i < currentMapFile.buildings.Count; i++) buildingsListBox.Items.Add("Building " + (i + 1).ToString()); // Add entry into buildings ListBox
        }
        private Building LoadBuildingModel(Building building, bool interior) {
            string modelPath = romInfo.GetBuildingModelsDirPath(interior) + "\\" + building.modelID.ToString("D4");

            using (Stream fs = new FileStream(modelPath, FileMode.Open))
                building.NSBMDFile = NSBMDLoader.LoadNSBMD(fs);
            return building;
        }
        private NSBMD LoadModelTextures(NSBMD model, string textureFolder, int fileID) {
            string texturePath = textureFolder + "\\" + fileID.ToString("D4");
            model.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(File.ReadAllBytes(texturePath)), out model.Textures, out model.Palettes);
            try {
                model.MatchTextures();
            } catch { }
            return model;
        }
        private void RenderMap(ref NSBMDGlRenderer mapRenderer, ref NSBMDGlRenderer buildingsRenderer, ref MapFile mapFile, float ang, float dist, float elev, float perspective, int width, int height, bool mapTexturesON, bool buildingTexturesON) {
            #region Useless variables that the rendering API still needs
            MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File ani = new MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File();
            MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File tp = new MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File();
            MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File ca = new MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File();
            int[] aniframeS = new int[0];
            #endregion

            /* Invalidate drawing surfaces */
            mapOpenGlControl.Invalidate();
            eventOpenGlControl.Invalidate();

            /* Adjust rendering settings */
            SetupRenderer(ang, dist, elev, perspective, width, height);

            /* Render the map model */
            mapRenderer.Model = mapFile.mapModel.models[0];
            Gl.glScalef(mapFile.mapModel.models[0].modelScale / 64, mapFile.mapModel.models[0].modelScale / 64, mapFile.mapModel.models[0].modelScale / 64);

            /* Determine if map textures must be rendered */
            if (!mapTexturesON) Gl.glDisable(Gl.GL_TEXTURE_2D);
            else Gl.glEnable(Gl.GL_TEXTURE_2D);

            mapRenderer.RenderModel("", ani, aniframeS, aniframeS, aniframeS, aniframeS, aniframeS, ca, false, -1, 0.0f, 0.0f, dist, elev, ang, true, tp, mapFile.mapModel); // Render map model

            if (!hideBuildings) {
                if (!buildingTexturesON) Gl.glDisable(Gl.GL_TEXTURE_2D);
                else Gl.glEnable(Gl.GL_TEXTURE_2D);

                for (int i = 0; i < mapFile.buildings.Count; i++) {
                    buildingsRenderer.Model = mapFile.buildings[i].NSBMDFile.models[0];
                    ScaleTranslateBuilding(mapFile.buildings[i], mapFile.mapModel);
                    buildingsRenderer.RenderModel("", ani, aniframeS, aniframeS, aniframeS, aniframeS, aniframeS, ca, false, -1, 0.0f, 0.0f, dist, elev, ang, true, tp, mapFile.buildings[i].NSBMDFile);
                }
            }
        }
        private void ScaleTranslateBuilding(Building building, NSBMD parentMap) {
            float xFraction = building.xFraction;
            float xPosition = building.xPosition;
            float zFraction = building.zFraction;
            float zPosition = building.zPosition;
            float yFraction = building.yFraction;
            float yPosition = building.yPosition;

            float scaleFactor = (building.NSBMDFile.models[0].modelScale / 64);
            float translateFactor = 16 / building.NSBMDFile.models[0].modelScale;

            Gl.glScalef(scaleFactor * building.width / 16, scaleFactor * building.length / 16, scaleFactor * building.height / 16);
            Gl.glTranslatef((xPosition + (xFraction / 65536f)) * translateFactor, (zPosition + (zFraction / 65536f)) * translateFactor, (yPosition + (yFraction / 65536f)) * translateFactor);
        }
        private void SetupRenderer(float ang, float dist, float elev, float perspective, int width, int height) {
            Gl.glEnable(Gl.GL_RESCALE_NORMAL);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glFrontFace(Gl.GL_CCW);
            Gl.glClearDepth(1);
            Gl.glEnable(Gl.GL_ALPHA_TEST);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glAlphaFunc(Gl.GL_GREATER, 0f);
            Gl.glClearColor(51f / 255f, 51f / 255f, 51f / 255f, 1f);
            float aspect;
            Gl.glViewport(0, 0, width, height);
            aspect = mapOpenGlControl.Width / mapOpenGlControl.Height;//(vp[2] - vp[0]) / (vp[3] - vp[1]);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(perspective, aspect, 0.02f, 1000000.0f);//0.02f, 32.0f);
            Gl.glTranslatef(0, 0, -dist);
            Gl.glRotatef(elev, 1, 0, 0);
            Gl.glRotatef(ang, 0, 1, 0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glTranslatef(0, 0, -dist);
            Gl.glRotatef(elev, 1, 0, 0);
            Gl.glRotatef(-ang, 0, 1, 0);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLoadIdentity();
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
            Gl.glColor3f(1.0f, 1.0f, 1.0f);
            Gl.glDepthMask(Gl.GL_TRUE);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
        }
        #endregion

        private void addMapFileButton_Click(object sender, EventArgs e) {
            /* Add new map file to map folder */
            string mapFilePath = romInfo.GetMapDirPath() + "\\" + selectMapComboBox.Items.Count.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(mapFilePath, FileMode.Create))) writer.Write(LoadMapFile(0).Save());

            /* Update ComboBox and select new file */
            selectMapComboBox.Items.Add(selectMapComboBox.Items.Count + ": " + "newmap");
            selectMapComboBox.SelectedIndex = selectMapComboBox.Items.Count - 1;
        }
        private void buildTextureComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            if (buildTextureComboBox.SelectedIndex == 0)
                buildingTexturesOn = false;
            else {
                buildingTexturesOn = true;

                foreach (Building building in currentMapFile.buildings) {
                    string texturePath = romInfo.GetBuildingTexturesDirPath() + "\\" + (buildTextureComboBox.SelectedIndex - 1).ToString("D4");
                    building.NSBMDFile.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(File.ReadAllBytes(texturePath)), out building.NSBMDFile.Textures, out building.NSBMDFile.Palettes);
                    try {
                        building.NSBMDFile.MatchTextures();
                    } catch { }
                }
            }
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void mapTextureComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            if (mapTextureComboBox.SelectedIndex == 0)
                mapTexturesOn = false;
            else {
                mapTexturesOn = true;

                string texturePath = romInfo.GetMapTexturesDirPath() + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4");
                currentMapFile.mapModel.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(File.ReadAllBytes(texturePath)), out currentMapFile.mapModel.Textures, out currentMapFile.mapModel.Palettes);
                try {
                    currentMapFile.mapModel.MatchTextures();
                } catch { }
            }
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void mapEditorTabPage_Enter(object sender, EventArgs e) {
            mapOpenGlControl.MakeCurrent();
            if (selectMapComboBox.SelectedIndex > -1) RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void mapOpenGlControl_MouseWheel(object sender, MouseEventArgs e) // Zoom In/Out
        {
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) dist += (float)e.Delta / 200;
            else dist -= (float)e.Delta / 200;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void mapOpenGlControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Right:
                    ang += 1;
                    break;
                case Keys.Left:
                    ang -= 1;
                    break;
                case Keys.Down:
                    elev += 1;
                    break;
                case Keys.Up:
                    elev -= 1;
                    break;
            }
            mapOpenGlControl.Invalidate();
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void mapPartsTabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (mapPartsTabControl.SelectedTab == buildingsTabPage) {
                radio2D.Checked = false;

                hideBuildings = false;
                radio3D.Enabled = true;
                radio2D.Enabled = true;
                wireframeCheckBox.Enabled = true;

                mapOpenGlControl.BringToFront();

                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
            } else if (mapPartsTabControl.SelectedTab == permissionsTabPage) {
                radio2D.Checked = true;

                hideBuildings = false;
                radio3D.Enabled = false;
                radio2D.Enabled = false;
                wireframeCheckBox.Enabled = false;

                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);

                movPictureBox.BackgroundImage = GrabMapScreenshot(movPictureBox.Width, movPictureBox.Height);
                movPictureBox.BringToFront();
            } else if (mapPartsTabControl.SelectedTab == modelTabPage) {
                radio2D.Checked = false;

                hideBuildings = true;
                radio3D.Enabled = true;
                radio2D.Enabled = true;
                wireframeCheckBox.Enabled = true;

                mapOpenGlControl.BringToFront();

                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
            } else { // Model tab 
                radio2D.Checked = true;

                hideBuildings = false;
                radio3D.Enabled = false;
                radio2D.Enabled = false;
                wireframeCheckBox.Enabled = false;

                mapOpenGlControl.BringToFront();

                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);

            }
        }
        private void radio2D_CheckedChanged(object sender, EventArgs e) {
            if (radio2D.Checked) {
                cam2Dmode();
            } else {
                cam3Dmode();
                radio3D.Checked = true;
            }

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }

        private void cam2Dmode() {
            perspective = 4f;
            ang = 0f;
            dist = 115.0f;
            elev = 90f;
        }

        private void cam3Dmode() {
            perspective = 45f;
            ang = 0f;
            dist = 12.8f;
            elev = 50.0f;
        }

        private void removeMapFileButton_Click(object sender, EventArgs e) {
            /* Delete last map file */
            File.Delete(romInfo.GetMapDirPath() + "\\" + (selectMapComboBox.Items.Count - 1).ToString("D4"));

            /* Check if currently selected file is the last one, and in that case select the one before it */
            int lastIndex = selectMapComboBox.Items.Count - 1;
            if (selectMapComboBox.SelectedIndex == lastIndex) selectMapComboBox.SelectedIndex--;

            /* Remove item from ComboBox */
            selectMapComboBox.Items.RemoveAt(lastIndex);
        }
        private void saveMapButton_Click(object sender, EventArgs e) {
            string mapIndex = selectMapComboBox.SelectedIndex.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(romInfo.GetMapDirPath() + "\\" + mapIndex, FileMode.Create))) 
                writer.Write(currentMapFile.Save());
        }
        private void selectMapComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            /* Load map data into MapFile class instance */
            currentMapFile = LoadMapFile(selectMapComboBox.SelectedIndex);

            /* Load map textures for renderer */
            if (mapTextureComboBox.SelectedIndex > 0) currentMapFile.mapModel = LoadModelTextures(currentMapFile.mapModel, romInfo.GetMapTexturesDirPath(), mapTextureComboBox.SelectedIndex - 1);


            /* Load buildings nsbmd and textures for renderer into MapFile's building objects */
            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                currentMapFile.buildings[i] = LoadBuildingModel(currentMapFile.buildings[i], interiorbldRadioButton.Checked); // Load building nsbmd
                if (buildTextureComboBox.SelectedIndex > 0) currentMapFile.buildings[i].NSBMDFile = LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, romInfo.GetBuildingTexturesDirPath(), buildTextureComboBox.SelectedIndex - 1); // Load building textures                
            }

            /* Render the map */
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);

            /* Draw permissions in the small selection boxes */
            Draw_Small_Collision();
            Draw_Small_Type();

            /* Draw selected permissions category */
            if (selectCollisionPanel.BackColor == Color.MidnightBlue) Draw_Collision_Grid();
            else Draw_Type_Grid();

            /* Set map screenshot as background picture in permissions editor PictureBox */
            movPictureBox.BackgroundImage = GrabMapScreenshot(movPictureBox.Width, movPictureBox.Height);

            /* Fill buildings ListBox, and if not empty select first item */
            FillBuildingsBox();
            if (buildingsListBox.Items.Count > 0) buildingsListBox.SelectedIndex = 0;

        }
        private void textureComboBoxes_SelectedIndexChanged(object sender, EventArgs e) {

        }
        private void wireframeCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (wireframeCheckBox.Checked)
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
            else
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }

        #region Building Editor
        private void addBuildingButton_Click(object sender, EventArgs e) {
            /* Add new building object to MapFile */
            currentMapFile.AddBuilding();

            /* Load new building's model and textures for the renderer */
            currentMapFile.buildings[currentMapFile.buildings.Count - 1] = LoadBuildingModel(currentMapFile.buildings[currentMapFile.buildings.Count - 1], interiorbldRadioButton.Checked);
            currentMapFile.buildings[currentMapFile.buildings.Count - 1].NSBMDFile = LoadModelTextures(currentMapFile.buildings[currentMapFile.buildings.Count - 1].NSBMDFile, romInfo.GetBuildingTexturesDirPath(), buildTextureComboBox.SelectedIndex - 1);

            /* Add new entry to buildings ListBox */
            buildingsListBox.Items.Add("Building " + (buildingsListBox.Items.Count + 1));
            buildingsListBox.SelectedIndex = buildingsListBox.Items.Count - 1;

            /* Redraw scene with new building */
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);

        }
        private void buildIndexComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].modelID = (uint)buildIndexComboBox.SelectedIndex;
            currentMapFile.buildings[buildingsListBox.SelectedIndex] = LoadBuildingModel(currentMapFile.buildings[buildingsListBox.SelectedIndex], interiorbldRadioButton.Checked);
            currentMapFile.buildings[buildingsListBox.SelectedIndex].NSBMDFile = LoadModelTextures(currentMapFile.buildings[buildingsListBox.SelectedIndex].NSBMDFile, romInfo.GetBuildingTexturesDirPath(), buildTextureComboBox.SelectedIndex - 1);

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);

        }
        private void buildingsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            #region Temporarily disable events to allow for faster execution
            if (disableHandlers)
                return;
            disableHandlers = true;
            #endregion

            int buildingNumber = buildingsListBox.SelectedIndex;

            buildIndexComboBox.SelectedIndex = (int)currentMapFile.buildings[buildingNumber].modelID;
            xFractionUpDown.Value = currentMapFile.buildings[buildingNumber].xFraction;
            xBuildUpDown.Value = currentMapFile.buildings[buildingNumber].xPosition;
            zFractionUpDown.Value = currentMapFile.buildings[buildingNumber].zFraction;
            zBuildUpDown.Value = currentMapFile.buildings[buildingNumber].zPosition;
            yFractionUpDown.Value = currentMapFile.buildings[buildingNumber].yFraction;
            yBuildUpDown.Value = currentMapFile.buildings[buildingNumber].yPosition;
            buildingWidthUpDown.Value = currentMapFile.buildings[buildingNumber].width;
            buildingHeightUpDown.Value = currentMapFile.buildings[buildingNumber].height;
            buildingLengthUpDown.Value = currentMapFile.buildings[buildingNumber].length;

            #region Re-enable disabled events
            disableHandlers = false;
            #endregion
        }
        private void buildingHeightUpDown_ValueChanged(object sender, EventArgs e) {
            currentMapFile.buildings[buildingsListBox.SelectedIndex].height = (uint)buildingHeightUpDown.Value;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void buildingLengthUpDown_ValueChanged(object sender, EventArgs e) {
            currentMapFile.buildings[buildingsListBox.SelectedIndex].length = (uint)buildingLengthUpDown.Value;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void buildingWidthUpDown_ValueChanged(object sender, EventArgs e) {
            currentMapFile.buildings[buildingsListBox.SelectedIndex].width = (uint)buildingWidthUpDown.Value;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void exportBuildingsButton_Click(object sender, EventArgs e) {
            SaveFileDialog eb = new SaveFileDialog();
            eb.Filter = "Buildings File (*.bld)|*.bld";
            if (eb.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(eb.FileName))) {
                writer.Write(currentMapFile.ExportBuildings());
            }

            MessageBox.Show("Buildings exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importBuildingsButton_Click(object sender, EventArgs e) {
            OpenFileDialog ib = new OpenFileDialog();
            ib.Filter = "Buildings File (*.bld)|*.bld";
            if (ib.ShowDialog(this) != DialogResult.OK)
                return;

            currentMapFile.ImportBuildings(new FileStream(ib.FileName, FileMode.Open));
            FillBuildingsBox();
            if (buildingsListBox.Items.Count > 0) buildingsListBox.SelectedIndex = 0;

            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                currentMapFile.buildings[i] = LoadBuildingModel(currentMapFile.buildings[i], interiorbldRadioButton.Checked); // Load building nsbmd
                currentMapFile.buildings[i].NSBMDFile = LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, romInfo.GetBuildingTexturesDirPath(), buildTextureComboBox.SelectedIndex - 1); // Load building textures                
            }

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);

            MessageBox.Show("Buildings imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        private void interiorRadioButton_CheckedChanged(object sender, EventArgs e) {
            disableHandlers = true;
            int index = buildIndexComboBox.SelectedIndex;

            buildIndexComboBox.Items.Clear();
            buildIndexComboBox.Items.AddRange(GetBuildingsList(interiorbldRadioButton.Checked));
            buildIndexComboBox.SelectedIndex = index;

            /* Load buildings nsbmd and textures for renderer into MapFile's building objects */
            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                currentMapFile.buildings[i] = LoadBuildingModel(currentMapFile.buildings[i], interiorbldRadioButton.Checked); // Load building nsbmd
                currentMapFile.buildings[i].NSBMDFile = LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, romInfo.GetBuildingTexturesDirPath(), buildTextureComboBox.SelectedIndex - 1); // Load building textures                
            }

            /* Render the map */
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);

            disableHandlers = false;
        }
        private void removeBuildingButton_Click(object sender, EventArgs e) {
            if (buildingsListBox.Items.Count > 0) {
                disableHandlers = true;

                /* Remove building object from list and the corresponding entry in the ListBox */
                int buildingNumber = buildingsListBox.SelectedIndex;
                currentMapFile.buildings.RemoveAt(buildingNumber);
                buildingsListBox.Items.RemoveAt(buildingNumber);

                FillBuildingsBox(); // Update ListBox
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);

                disableHandlers = false;

                if (buildingNumber > 0) buildingsListBox.SelectedIndex = buildingNumber - 1;
            }
        }
        private void xBuildUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].xPosition = (short)xBuildUpDown.Value;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void yBuildUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].yPosition = (short)yBuildUpDown.Value;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void zBuildUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].zPosition = (short)zBuildUpDown.Value;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void xFractionUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].xFraction = (ushort)xFractionUpDown.Value;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void yFractionUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].yFraction = (ushort)yFractionUpDown.Value;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        private void zFractionUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].zFraction = (ushort)zFractionUpDown.Value;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);
        }
        #endregion

        #region Movement Permissions Editor

        #region Subroutines
        private Bitmap GrabMapScreenshot(int width, int height) {
            Bitmap bmp = new Bitmap(width, height);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Gl.glReadPixels(0, 0, width, height, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }
        private void Draw_Collision_Grid() {
            Bitmap mainBm = new Bitmap(608, 608);
            using (Graphics gMain = Graphics.FromImage(mainBm)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        Set_Collision_Painter(currentMapFile.collisions[i, j]);

                        /* Draw collision on the main grid */
                        mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                        gMain.DrawRectangle(paintPen, mainCell);
                        gMain.FillRectangle(paintBrush, mainCell);
                    }
                }
            }
            movPictureBox.Image = mainBm;
            movPictureBox.Invalidate();
        }
        private void Draw_Small_Collision() {
            Bitmap smallBm = new Bitmap(100, 100);
            using (Graphics gSmall = Graphics.FromImage(smallBm)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        Set_Collision_Painter(currentMapFile.collisions[i, j]);

                        /* Draw collision on the small image */
                        smallCell = new Rectangle(3 * j, 3 * i, 3, 3);
                        gSmall.DrawRectangle(paintPen, smallCell);
                        gSmall.FillRectangle(paintBrush, smallCell);
                    }
                }
            }
            collisionPictureBox.Image = smallBm;
            collisionPictureBox.Invalidate();
        }
        private void Draw_Type_Grid() {
            Bitmap mainBm = new Bitmap(608, 608);
            using (Graphics gMain = Graphics.FromImage(mainBm)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        Set_Type_Painter(Convert.ToInt32(currentMapFile.types[i, j]));

                        /* Draw cell with color */
                        mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                        gMain.DrawRectangle(paintPen, mainCell);
                        gMain.FillRectangle(paintBrush, mainCell);

                        /* Draw byte on cell */
                        StringFormat sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        gMain.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        gMain.DrawString(currentMapFile.types[i, j].ToString("X2"), textFont, textBrush, mainCell, sf);
                    }
                }
            }
            movPictureBox.Image = mainBm;
            movPictureBox.Invalidate();
        }
        private void Draw_Small_Type() {
            Bitmap smallBm = new Bitmap(100, 100);
            using (Graphics gSmall = Graphics.FromImage(smallBm)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        Set_Type_Painter(currentMapFile.types[i, j]);

                        /* Draw collision on the small image */
                        smallCell = new Rectangle(3 * j, 3 * i, 3, 3);
                        gSmall.DrawRectangle(paintPen, smallCell);
                        gSmall.FillRectangle(paintBrush, smallCell);
                    }
                }
            }
            typePictureBox.Image = smallBm;
            typePictureBox.Invalidate();
        }
        private void Edit_Cell(int xPosition, int yPosition) {
            try {
                mainCell = new Rectangle(xPosition * 19, yPosition * 19, 19, 19);
                smallCell = new Rectangle(xPosition * 3, yPosition * 3, 3, 3);

                using (Graphics mainG = Graphics.FromImage(movPictureBox.Image)) {
                    /*  Draw new cell on main grid */
                    mainG.SetClip(mainCell);
                    mainG.Clear(Color.Transparent);
                    mainG.DrawRectangle(paintPen, mainCell);
                    mainG.FillRectangle(paintBrush, mainCell);
                    if (selectTypePanel.BackColor == Color.MidnightBlue) {
                        sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        mainG.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        mainG.DrawString(paintByte.ToString("X2"), textFont, textBrush, mainCell, sf);
                    }
                }

                if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                    using (Graphics smallG = Graphics.FromImage(collisionPictureBox.Image)) {
                        /* Draw new cell on small grid */
                        smallG.SetClip(smallCell);
                        smallG.Clear(Color.Transparent);
                        smallG.DrawRectangle(paintPen, smallCell);
                        smallG.FillRectangle(paintBrush, smallCell);
                    }
                    currentMapFile.collisions[yPosition, xPosition] = paintByte;
                    collisionPictureBox.Invalidate();
                } else {
                    using (Graphics smallG = Graphics.FromImage(typePictureBox.Image)) {
                        /* Draw new cell on small grid */
                        smallG.SetClip(smallCell);
                        smallG.Clear(Color.Transparent);
                        smallG.DrawRectangle(paintPen, smallCell);
                        smallG.FillRectangle(paintBrush, smallCell);
                    }
                    currentMapFile.types[yPosition, xPosition] = paintByte;
                    typePictureBox.Invalidate();
                }
                movPictureBox.Invalidate();
            } catch { return; }
        }
        private void Restore_Painter() {
            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
                collisionPainterComboBox_SelectedIndexChanged(null, null); // Restore painters to original state
            else if (typePainterComboBox.Enabled)
                typePainterComboBox_SelectedIndexChanged(null, null); // Restore painters to original state
            else typePainterUpDown_ValueChanged(null, null);
        }
        private void Set_Collision_Painter(int collisionValue) {
            switch (collisionValue) {
                case 0x0:
                    paintPen = new Pen(Color.FromArgb(128, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.White));
                    break;
                case 0x80:
                    paintPen = new Pen(Color.FromArgb(128, Color.Red));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Red));
                    break;
                default:
                    paintPen = new Pen(Color.FromArgb(128, Color.LimeGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.LimeGreen));
                    break;
            }
            paintByte = (byte)collisionValue;
        }
        private void Set_Type_Painter(int typeValue) {
            switch (typeValue) {
                case 0x0:
                    paintPen = new Pen(Color.FromArgb(128, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.White));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x2:
                    paintPen = new Pen(Color.FromArgb(128, Color.LimeGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.LimeGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x3:
                    paintPen = new Pen(Color.FromArgb(128, Color.Green));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Green));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x8:
                case 0xC:
                    paintPen = new Pen(Color.FromArgb(128, Color.BurlyWood));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.BurlyWood));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.5f);
                    break;
                case 0x10:
                    paintPen = new Pen(Color.FromArgb(128, Color.SkyBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.SkyBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x13:
                    paintPen = new Pen(Color.FromArgb(128, Color.SteelBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.SteelBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x15:
                    paintPen = new Pen(Color.FromArgb(128, Color.RoyalBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.RoyalBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x16:
                    paintPen = new Pen(Color.FromArgb(128, Color.LightSlateGray));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.LightSlateGray));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x20:
                    paintPen = new Pen(Color.FromArgb(128, Color.Cyan));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Cyan));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x21:
                    paintPen = new Pen(Color.FromArgb(128, Color.PeachPuff));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.PeachPuff));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x38:
                case 0x39:
                case 0x3A:
                case 0x3B:
                    paintPen = new Pen(Color.FromArgb(128, Color.Maroon));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Maroon));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.7f);
                    break;
                case 0x40:
                case 0x41:
                case 0x42:
                case 0x43:
                    paintPen = new Pen(Color.FromArgb(128, Color.Gold));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Gold));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x5E:
                    paintPen = new Pen(Color.FromArgb(128, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.7f);
                    break;
                case 0x5F:
                case 0x62:
                case 0x63:
                case 0x64:
                case 0x65:
                case 0x69:
                    paintPen = new Pen(Color.FromArgb(128, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x6C:
                case 0x6D:
                case 0x6E:
                case 0x6F:
                    paintPen = new Pen(Color.FromArgb(128, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.6f);
                    break;
                case 0xA1:
                case 0xA2:
                case 0xA3:
                    paintPen = new Pen(Color.FromArgb(128, Color.Honeydew));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Honeydew));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0xA4:
                    paintPen = new Pen(Color.FromArgb(128, Color.Peru));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Peru));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0xA6:
                    paintPen = new Pen(Color.FromArgb(128, Color.SeaGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.SeaGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.7f);
                    break;
                default:
                    paintPen = new Pen(Color.FromArgb(128, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.White));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 8.7f);
                    break;
            }
            paintByte = (byte)typeValue;
        }
        #endregion

        private void clearCurrentButton_Click(object sender, EventArgs e) {
            PictureBox smallBox;

            if (selectCollisionPanel.BackColor == Color.MidnightBlue) smallBox = collisionPictureBox;
            else smallBox = typePictureBox;

            using (Graphics smallG = Graphics.FromImage(smallBox.Image))
            using (Graphics mainG = Graphics.FromImage(movPictureBox.Image)) {
                smallG.Clear(Color.Transparent);
                mainG.Clear(Color.Transparent);
                Set_Collision_Painter(0x0);

                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                        mainG.DrawRectangle(paintPen, mainCell);
                        mainG.FillRectangle(paintBrush, mainCell);
                    }
                }
            }

            if (selectCollisionPanel.BackColor == Color.MidnightBlue) currentMapFile.collisions = new byte[32, 32]; // Set all collision bytes to clear (0x0)               
            else currentMapFile.types = new byte[32, 32]; // Set all type bytes to clear (0x0)

            movPictureBox.Invalidate(); // Refresh main image
            smallBox.Invalidate();
            Restore_Painter();
        }
        private void collisionPainterComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            int collisionValue;
            if (collisionPainterComboBox.SelectedIndex == 0) collisionValue = 0;
            else if (collisionPainterComboBox.SelectedIndex == 1) collisionValue = 0x80;
            else collisionValue = 1;

            Set_Collision_Painter(collisionValue);

            using (Graphics g = Graphics.FromImage(collisionPainterPictureBox.Image)) g.Clear(Color.FromArgb(255, paintBrush.Color));
            collisionPainterPictureBox.Invalidate();
        }
        private void collisionPictureBox_Click(object sender, EventArgs e) {
            selectTypePanel.BackColor = Color.Transparent;
            typeGroupBox.Enabled = false;
            selectCollisionPanel.BackColor = Color.MidnightBlue;
            collisionGroupBox.Enabled = true;

            Draw_Collision_Grid();
            Restore_Painter();
        }
        private void exportMovButton_Click(object sender, EventArgs e) {
            SaveFileDialog em = new SaveFileDialog();
            em.Filter = "Permissions File (*.per)|*.per";
            if (em.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(em.FileName))) writer.Write(currentMapFile.ExportPermissions());
            MessageBox.Show("Permissions exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importMovButton_Click(object sender, EventArgs e) {
            OpenFileDialog ip = new OpenFileDialog();
            ip.Filter = "Permissions File (*.per)|*.per";
            if (ip.ShowDialog(this) != DialogResult.OK)
                return;

            currentMapFile.ImportPermissions(new FileStream(ip.FileName, FileMode.Open));

            Draw_Small_Collision();
            Draw_Small_Type();
            if (selectCollisionPanel.BackColor == Color.MidnightBlue) Draw_Collision_Grid();
            else Draw_Type_Grid();
            Restore_Painter();

            MessageBox.Show("Permissions imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void movPictureBox_Click(object sender, EventArgs e) {
            Edit_Cell(movPictureBox.PointToClient(MousePosition).X / 19, movPictureBox.PointToClient(MousePosition).Y / 19);
        }
        private void movPictureBox_MouseMove(object sender, MouseEventArgs e) {
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left) {
                Edit_Cell(e.Location.X / 19, e.Location.Y / 19);
            }
        }
        private void typePainterComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            string selectedType = typePainterComboBox.SelectedItem.ToString();
            int typeValue = Convert.ToInt32(selectedType.Substring(1, 2), 16);

            Set_Type_Painter(typeValue);

            sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            using (Graphics g = Graphics.FromImage(typePainterPictureBox.Image)) {
                g.Clear(Color.FromArgb(255, paintBrush.Color));
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(typeValue.ToString("X2"), new Font("Microsoft Sans Serif", 24), textBrush, painterBox, sf);
            }
            typePainterPictureBox.Invalidate();
        }
        private void typePainterUpDown_ValueChanged(object sender, EventArgs e) {
            int typeValue = (int)typePainterUpDown.Value;
            Set_Type_Painter(typeValue);

            sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            painterBox = new Rectangle(0, 0, 100, 100);
            using (Graphics g = Graphics.FromImage(typePainterPictureBox.Image)) {
                g.Clear(Color.FromArgb(255, paintBrush.Color));
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(typeValue.ToString("X2"), new Font("Microsoft Sans Serif", 24), textBrush, painterBox, sf);
            }
            typePainterPictureBox.Invalidate();
        }
        private void typePictureBox_Click(object sender, EventArgs e) {
            selectCollisionPanel.BackColor = Color.Transparent;
            collisionGroupBox.Enabled = false;
            selectTypePanel.BackColor = Color.MidnightBlue;
            typeGroupBox.Enabled = true;

            Draw_Type_Grid();
            Restore_Painter();
        }
        private void typesRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (knownTypesRadioButton.Checked) {
                typePainterUpDown.Enabled = false;
                typePainterComboBox.Enabled = true;
                typePainterComboBox_SelectedIndexChanged(null, null);
            }
        }
        private void valueTypeRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (valueTypeRadioButton.Checked) {
                typePainterComboBox.Enabled = false;
                typePainterUpDown.Enabled = true;
                typePainterUpDown_ValueChanged(null, null);
            }
        }
        #endregion

        #region 3D Model Editor
        private void importMapButton_Click(object sender, EventArgs e) {

            OpenFileDialog im = new OpenFileDialog();
            im.Filter = "NSBMD model (*.nsbmd)|*.nsbmd";
            if (im.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryReader modelReader = new BinaryReader(new FileStream(im.FileName, FileMode.Open))) {
                if (modelReader.ReadUInt32() != 0x30444D42) {
                    MessageBox.Show("Please select an NSBMD file.", "Invalid File");
                    return;
                } else currentMapFile.ImportMapModel(modelReader.BaseStream);
            }

            if (mapTextureComboBox.SelectedIndex > 0) currentMapFile.mapModel = LoadModelTextures(currentMapFile.mapModel, romInfo.GetMapTexturesDirPath(), mapTextureComboBox.SelectedIndex - 1);
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, buildingTexturesOn);

            MessageBox.Show("Map model imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void exportMapButton_Click(object sender, EventArgs e) {
            SaveFileDialog em = new SaveFileDialog();
            em.Filter = "NSBMD model (*.nsbmd)|*.nsbmd";
            if (em.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(em.FileName))) {
                writer.Write(currentMapFile.ExportMapModel());
            }

            MessageBox.Show("Map model exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region BDHC Editor
        private void bdhcImportButton_Click(object sender, EventArgs e) {
            OpenFileDialog it = new OpenFileDialog();
            if (romInfo.GetGameVersion() == "Diamond" || romInfo.GetGameVersion() == "Pearl")
                it.Filter = "Terrain File (*.bdhc)|*.bdhc";
            else
                it.Filter = "Terrain File (*.bdhc, *.bdhcam)|*.bdhc;*.bdhcam";

            if (it.ShowDialog(this) != DialogResult.OK)
                return;

            currentMapFile.ImportTerrain(new FileStream(it.FileName, FileMode.Open));
            MessageBox.Show("Terrain settings imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void bdhcExportButton_Click(object sender, EventArgs e) {
            SaveFileDialog eb = new SaveFileDialog();
            eb.Filter = "Terrain File (*.bdhc)|*.bdhc";
            if (eb.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(eb.FileName))) writer.Write(currentMapFile.ExportTerrain());
            MessageBox.Show("Terrain settings exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #endregion

        #region Event Editor

        #region Variables      
        public static NSBMDGlRenderer eventMapRenderer = new NSBMDGlRenderer();
        public static NSBMDGlRenderer eventBuildingsRenderer = new NSBMDGlRenderer();
        public static MapFile eventMapFile;
        public NSMBe4.NSBMD.NSBTX_File overworldFrames;
        public Matrix eventMatrix;
        public string spritesTablePath;

        public EventFile currentEventFile;
        public Event selectedEvent;
        public Dictionary<uint, string> ow3DSpriteDict = new Dictionary<uint, string>();

        /* Painters to draw the matrix grid */
        public Pen eventPen;
        public Brush eventBrush;
        public Rectangle eventMatrixRectangle;
        #endregion

        #region Subroutines
        private void DisplayActiveEvents() {
            eventPictureBox.Image = new Bitmap(eventPictureBox.Width, eventPictureBox.Height);

            /* Draw spawnables */
            if (showSignsCheckBox.Checked) 
                for (int i = 0; i < currentEventFile.spawnables.Count; i++) {
                    Spawnable spawnable = currentEventFile.spawnables[i];
                    if (spawnable.xMatrixPosition == eventMatrixXUpDown.Value && spawnable.yMatrixPosition == eventMatrixYUpDown.Value) {
                        using (Graphics g = Graphics.FromImage(eventPictureBox.Image)) {
                            g.CompositingMode = CompositingMode.SourceOver;
                            g.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject("sign"), (spawnable.xMapPosition) * 17, (spawnable.yMapPosition) * 17);
                            if (selectedEvent == spawnable) { // Draw selection rectangle if event is the selected one
                                drawSelectionRectangle(g, spawnable);
                            }
                        }
                    }
                }

            /* Draw overworlds */
            if (showOwsCheckBox.Checked) {
                for (int i = 0; i < currentEventFile.overworlds.Count; i++) {
                    Overworld overworld = currentEventFile.overworlds[i];
                    if (isEventOnCurrentMatrix(overworld)) { // Draw image only if event is in current map
                        using (Graphics g = Graphics.FromImage(eventPictureBox.Image)) {
                            g.CompositingMode = CompositingMode.SourceOver;
                            Bitmap sprite = GetOverworldImage(overworld.spriteID, overworld.orientation);
                            sprite.MakeTransparent();
                            g.DrawImage(sprite, (overworld.xMapPosition) * 17 - 7 + (32 - sprite.Width) / 2, (overworld.yMapPosition - 1) * 17 + (32 - sprite.Height));

                            if (selectedEvent == overworld) {
                                drawSelectionRectangleOverworld(g, overworld);
                            }
                        }
                    }
                }
            }

            /* Draw warps */
            if (showWarpsCheckBox.Checked) for (int i = 0; i < currentEventFile.warps.Count; i++) {
                    Warp warp = currentEventFile.warps[i];
                    if (isEventOnCurrentMatrix(warp)) {
                        using (Graphics g = Graphics.FromImage(eventPictureBox.Image)) {
                            g.CompositingMode = CompositingMode.SourceOver;
                            g.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject("warp"), (warp.xMapPosition) * 17, (warp.yMapPosition) * 17);
                            if (selectedEvent == warp) { // Draw selection rectangle if event is the selected one

                                drawSelectionRectangle(g, warp);
                            }
                        }

                    }
                }

            /* Draw triggers */
            if (showTriggersCheckBox.Checked) for (int i = 0; i < currentEventFile.triggers.Count; i++) {
                    Trigger trigger = currentEventFile.triggers[i];
                    if (isEventOnCurrentMatrix(trigger)) {
                        using (Graphics g = Graphics.FromImage(eventPictureBox.Image)) {
                            g.CompositingMode = CompositingMode.SourceOver;
                            for (int y = 0; y < currentEventFile.triggers[i].length; y++) {
                                for (int x = 0; x < currentEventFile.triggers[i].width; x++) {
                                    g.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject("trigger"), (trigger.xMapPosition + x) * 17, (trigger.yMapPosition + y) * 17);
                                }
                            }
                            if (selectedEvent == trigger) {// Draw selection rectangle if event is the selected one
                                drawSelectionRectangleTrigger(g, trigger);
                            }
                        }
                    }
                }

            eventPictureBox.Invalidate();
        }

        private void drawSelectionRectangle(Graphics g, Event ev) {
            eventPen = Pens.Red;
            g.DrawRectangle(eventPen, (ev.xMapPosition) * 17 - 1, (ev.yMapPosition) * 17 - 1, 18, 18);
            g.DrawRectangle(eventPen, (ev.xMapPosition) * 17 - 2, (ev.yMapPosition) * 17 - 2, 20, 20);
        }
        private void drawSelectionRectangleTrigger(Graphics g, Trigger t) {
            eventPen = Pens.Red;
            g.DrawRectangle(eventPen, (t.xMapPosition) * 17 - 1, (t.yMapPosition) * 17 - 1, 17 * t.width + 1, 17 * t.length + 1);
            g.DrawRectangle(eventPen, (t.xMapPosition) * 17 - 2, (t.yMapPosition) * 17 - 2, 17 * t.width + 3, 17 * t.length + 3);

        }
        private void drawSelectionRectangleOverworld(Graphics g, Overworld ow) {
            eventPen = Pens.Red;
            g.DrawRectangle(eventPen, (ow.xMapPosition) * 17 - 8, (ow.yMapPosition - 1) * 17, 34, 34);
            g.DrawRectangle(eventPen, (ow.xMapPosition) * 17 - 9, (ow.yMapPosition - 1) * 17 - 1, 36, 36);
        }



        private void DisplayEventMap() {
            /* Determine map file to open and open it in BinaryReader, unless map is VOID */
            uint mapIndex = Matrix.EMPTY;
            if (eventMatrixXUpDown.Value > eventMatrix.width || eventMatrixYUpDown.Value > eventMatrix.height) {
                String errorMsg = "This event file contains elements located on an unreachable map, beyond the current matrix.\n" +
                    "It is strongly advised that you bring every Overworld, Spawnable, Warp and Trigger of this event to a map that belongs to the matrix's range.";
                MessageBox.Show(errorMsg, "Can't load proper map", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else {
                mapIndex = eventMatrix.maps[(int)(eventMatrixYUpDown.Value), (int)(eventMatrixXUpDown.Value)];
            }

            if (mapIndex == Matrix.EMPTY) {
                eventPictureBox.BackgroundImage = new Bitmap(eventPictureBox.Width, eventPictureBox.Height);
                using (Graphics g = Graphics.FromImage(eventPictureBox.BackgroundImage)) g.Clear(Color.Black);
            } else {
                /* Determine area data */
                uint areaDataID;
                if (eventMatrix.hasHeadersSection) {
                    int header = eventMatrix.headers[(int)(eventMatrixYUpDown.Value), (int)(eventMatrixXUpDown.Value)];
                    areaDataID = LoadHeader(header).areaDataID;
                } else areaDataID = (uint)eventAreaDataUpDown.Value;

                /* get texture file numbers from area data */
                AreaData areaData = LoadAreaData(areaDataID);

                /* Read map and building models, match them with textures and render them*/
                eventMapFile = LoadMapFile((int)mapIndex);
                eventMapFile.mapModel = LoadModelTextures(eventMapFile.mapModel, romInfo.GetMapTexturesDirPath(), areaData.mapTileset);

                bool isInteriorMap = new bool();
                if ((romInfo.GetGameVersion() == "HeartGold" || romInfo.GetGameVersion() == "SoulSilver")
                && areaData.areaType == 0x0)
                    isInteriorMap = true;

                for (int i = 0; i < eventMapFile.buildings.Count; i++) {
                    eventMapFile.buildings[i] = LoadBuildingModel(eventMapFile.buildings[i], isInteriorMap); // Load building nsbmd
                    eventMapFile.buildings[i].NSBMDFile = LoadModelTextures(eventMapFile.buildings[i].NSBMDFile, romInfo.GetBuildingTexturesDirPath(), areaData.buildingsTileset); // Load building textures                
                }

                RenderMap(ref eventMapRenderer, ref eventBuildingsRenderer, ref eventMapFile, 0f, 115.0f, 90f, 4f, eventOpenGlControl.Width, eventOpenGlControl.Height, true, true);
                eventPictureBox.BackgroundImage = GrabMapScreenshot(eventOpenGlControl.Width, eventOpenGlControl.Height);
            }
            eventPictureBox.Invalidate();
        }
        private void DrawEventMatrix() {
            eventMatrixPictureBox.Image = new Bitmap(1 + 16 * eventMatrix.width, 1 + 16 * eventMatrix.height);

            using (Graphics g = Graphics.FromImage(eventMatrixPictureBox.Image)) {
                /* First, fill the rectangle with black */
                g.Clear(Color.Black);

                /* Now, draw the white cell borders on the black rectangle */
                eventPen = Pens.White;
                for (int y = 0; y < eventMatrix.height; y++) {
                    for (int x = 0; x < eventMatrix.width; x++) {
                        eventMatrixRectangle = new Rectangle(1 + 16 * x, 1 + 16 * y, 14, 14);
                        g.DrawRectangle(eventPen, eventMatrixRectangle);
                    }
                }
            }
        }
        private void FillSpawnablesBox() {
            spawnablesListBox.Items.Clear();
            for (int i = 0; i < currentEventFile.spawnables.Count; i++) 
                spawnablesListBox.Items.Add("Spawnable " + i);
        }
        private void FillOverworldsBox() {
            overworldsListBox.Items.Clear();
            for (int i = 0; i < currentEventFile.overworlds.Count; i++)
                overworldsListBox.Items.Add("Overworld " + i);
        }
        private void FillWarpsBox() {
            warpsListBox.Items.Clear();
            for (int i = 0; i < currentEventFile.warps.Count; i++) 
                warpsListBox.Items.Add("Warp " + i);
        }
        private void FillTriggersBox() {
            triggersListBox.Items.Clear();
            for (int i = 0; i < currentEventFile.triggers.Count; i++)
                triggersListBox.Items.Add("Trigger " + i);
        }
        private Bitmap GetOverworldImage(ushort spriteID, ushort orientation) {
            /* Find sprite corresponding to ID and load it*/
            string imageName;

            if (ow3DSpriteDict.TryGetValue(spriteID, out imageName)) { // If overworld is 3D, load image from dictionary
                return (Bitmap)Properties.Resources.ResourceManager.GetObject(imageName);
            } else {
                int archiveID = MatchOverworldIDToSpriteArchive(spriteID, spritesTablePath);
                if (archiveID == -1)
                    return (Bitmap)Properties.Resources.ResourceManager.GetObject("overworld"); // If id is -1, load bounding box
                else {
                    try {
                        FileStream stream = new FileStream(romInfo.GetOWSpriteDirPath() + "\\" + archiveID.ToString("D4"), FileMode.Open);
                        NSMBe4.NSBMD.NSBTX_File nsbtx = new NSMBe4.NSBMD.NSBTX_File(stream);


                        if (nsbtx.TexInfo.num_objs > 2) { // Read nsbtx slot corresponding to overworld's movement

                            switch (orientation) {
                                case 0:
                                    return LoadTextureFromNSBTX(nsbtx, 0, 0);
                                case 1:
                                    return LoadTextureFromNSBTX(nsbtx, 11, 0);
                                case 2:
                                    return LoadTextureFromNSBTX(nsbtx, 2, 0);
                                default:
                                    return LoadTextureFromNSBTX(nsbtx, 4, 0);
                            }
                        } else return LoadTextureFromNSBTX(nsbtx, 0, 0); // Read nsbtx slot 0 if ow has only 2 frames
                    } catch { // Load bounding box if sprite cannot be found

                        return (Bitmap)Properties.Resources.ResourceManager.GetObject("overworld");
                    }
                }
            }
        }
        private void MarkUsedCells() {
            using (Graphics g = Graphics.FromImage(eventMatrixPictureBox.Image)) {
                eventBrush = Brushes.Orange;

                for (int i = 0; i < currentEventFile.spawnables.Count; i++) {
                    eventMatrixRectangle = new Rectangle(2 + 16 * currentEventFile.spawnables[i].xMatrixPosition, 2 + 16 * currentEventFile.spawnables[i].yMatrixPosition, 13, 13);
                    g.FillRectangle(eventBrush, eventMatrixRectangle);
                }
                for (int i = 0; i < currentEventFile.overworlds.Count; i++) {
                    eventMatrixRectangle = new Rectangle(2 + 16 * currentEventFile.overworlds[i].xMatrixPosition, 2 + 16 * currentEventFile.overworlds[i].yMatrixPosition, 13, 13);
                    g.FillRectangle(eventBrush, eventMatrixRectangle);
                }
                for (int i = 0; i < currentEventFile.warps.Count; i++) {
                    eventMatrixRectangle = new Rectangle(2 + 16 * currentEventFile.warps[i].xMatrixPosition, 2 + 16 * currentEventFile.warps[i].yMatrixPosition, 13, 13);
                    g.FillRectangle(eventBrush, eventMatrixRectangle);
                }
                for (int i = 0; i < currentEventFile.triggers.Count; i++) {
                    eventMatrixRectangle = new Rectangle(2 + 16 * currentEventFile.triggers[i].xMatrixPosition, 2 + 16 * currentEventFile.triggers[i].yMatrixPosition, 13, 13);
                    g.FillRectangle(eventBrush, eventMatrixRectangle);
                }
            }
            eventMatrixPictureBox.Invalidate();
        }
        private void MarkActiveCell(int xPosition, int yPosition) {
            /*  Redraw the matrix to avoid multiple green cells  */
            DrawEventMatrix();
            MarkUsedCells();

            /* Set rectangle to paint and brush color */
            eventMatrixRectangle = new Rectangle(2 + 16 * xPosition, 2 + 16 * yPosition, 13, 13);
            eventBrush = Brushes.Lime;

            /* Paint cell */
            using (Graphics g = Graphics.FromImage(eventMatrixPictureBox.Image)) {
                g.FillRectangle(eventBrush, eventMatrixRectangle);
            }

            /* Update PictureBox and current coordinates labels */
            eventMatrixPictureBox.Invalidate();
            eventMatrixXUpDown.Value = xPosition;
            eventMatrixYUpDown.Value = yPosition;
        }
        private int MatchOverworldIDToSpriteArchive(uint ID, String overworldTablePath) {

            Console.WriteLine("Searching for ID : " + ID.ToString("X4"));
            using (BinaryReader idReader = new BinaryReader(new FileStream(romInfo.GetOWtablePath(), FileMode.Open))) {
                int archiveID;
                switch (romInfo.GetGameVersion()) {
                    case "Diamond":
                    case "Pearl":
                        idReader.BaseStream.Position = 0x22BCC;
                        archiveID = matchOverworldInTableDPPt(idReader, ID);
                        break;
                    case "Platinum":
                        switch (romInfo.GetGameLanguage()) { // Go to the beginning of the overworld table
                            case "ITA":
                                idReader.BaseStream.Position = 0x2BC44;
                                break;
                            case "GER":
                                idReader.BaseStream.Position = 0x2BC50;
                                break;
                            default:
                                idReader.BaseStream.Position = 0x2BC34;
                                break;
                        }
                        archiveID = matchOverworldInTableDPPt(idReader, ID);
                        break;
                    default:
                        idReader.BaseStream.Position = 0x21BA8;
                        archiveID = matchOverworldInTableHGSS(idReader, ID);
                        break;
                }

                Console.WriteLine("Result is " + archiveID);
                return archiveID;
            }
        }

        private int matchOverworldInTableHGSS(BinaryReader idReader, uint ID) {
            bool match = new bool();
            try {
                while (!match) { // Search for the overworld id in the table  
                    ushort idFound = idReader.ReadUInt16();
                    Console.WriteLine("Matching against : " + idFound.ToString("X4"));
                    if (idFound == ID)
                        return (int)idReader.ReadUInt16(); // If the entry is a match, stop and go to reading part
                    else
                        idReader.BaseStream.Position += 0x4; // If the entry is not a match, move forward
                }
            } catch (EndOfStreamException) {
                Console.WriteLine("Could not find ID : " + ID.ToString("X4"));
            }
            return -1; // If no match has been found, return -1, which loads bounding box
        }

        private int matchOverworldInTableDPPt(BinaryReader idReader, uint ID) {
            bool match = new bool();
            try {
                while (!match) { // Search for the overworld id in the table
                    uint idFound = idReader.ReadUInt32();
                    //if (idFound == 0xFFFF) 
                    //    break;
                    //else {

                    Console.WriteLine("Matching against : " + idFound.ToString("X4"));
                    if (idFound == ID)
                        return (int)idReader.ReadUInt32(); // Read ID from file if there was a match
                    else
                        idReader.BaseStream.Position += 0x4; // If the entry is not a match, move forward
                    //}
                }
            } catch (EndOfStreamException) {
                Console.WriteLine("Could not find ID : " + ID.ToString("X4"));
            }
            return -1; // If no match has been found, return -1, which loads bounding box
        }

        private Bitmap LoadTextureFromNSBTX(NSMBe4.NSBMD.NSBTX_File nsbtx, int imageIndex, int palIndex) {
            Bitmap b_ = new Bitmap(nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].width, nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].height);
            NSMBe4.NSBMD.ImageTexeler.LockBitmap b = new NSMBe4.NSBMD.ImageTexeler.LockBitmap(b_);
            b.LockBits();
            int pixelnum = b.Height * b.Width;
            try {
                switch (nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].format) {
                    case 1:
                        for (int j = 0; j < pixelnum; j++) {
                            int index = nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].Image[j] & 0x1f;
                            int alpha = (nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].Image[j] >> 5);
                            alpha = ((alpha * 4) + (alpha / 2)) * 8;
                            Color c = Color.FromArgb(alpha, nsbtx.PalInfo.infoBlock.PalInfo[palIndex].pal[index]);
                            b.SetPixel(j - ((j / b.Width) * b.Width), j / b.Width, c);
                        }
                        break;
                    case 2:
                        for (int j = 0; j < pixelnum; j++) {
                            uint index = nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].Image[j / 4];
                            index = (index >> ((j % 4) << 1)) & 3;
                            if (index == 0 && nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].color0 == 1) b.SetPixel(j - ((j / b.Width) * b.Width), (j / b.Width), Color.Transparent);
                            else b.SetPixel(j - (j / b.Width) * b.Width, (j / b.Width), nsbtx.PalInfo.infoBlock.PalInfo[palIndex].pal[index]);
                        }
                        break;
                    case 3:
                        for (int j = 0; j < pixelnum; j++) {
                            uint index = nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].Image[j / 2];
                            index = (index >> ((j % 2) << 2)) & 0x0f;
                            if (index == 0 && nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].color0 == 1) b.SetPixel(j - (j / b.Width) * b.Width, (j / b.Width), Color.Transparent);
                            else b.SetPixel(j - (j / b.Width) * b.Width, (j / b.Width), nsbtx.PalInfo.infoBlock.PalInfo[palIndex].pal[index]);
                        }
                        break;
                    case 4:
                        for (int j = 0; j < pixelnum; j++) {
                            byte index = nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].Image[j];
                            if (index == 0 && nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].color0 == 1) b.SetPixel(j - (j / b.Width) * b.Width, j / b.Width, Color.Transparent);
                            else b.SetPixel(j - (j / b.Width) * b.Width, j / b.Width, nsbtx.PalInfo.infoBlock.PalInfo[palIndex].pal[index]);
                        }
                        break;
                    case 5:
                        overworldFrames.convert_4x4texel_b(nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].Image, b.Width, b.Height, nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].spData, nsbtx.PalInfo.infoBlock.PalInfo[palIndex].pal, b);
                        b.UnlockBits();
                        break;
                    case 6:
                        for (int j = 0; j < pixelnum; j++) {
                            int index = nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].Image[j] & 0x7;
                            int alpha = (nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].Image[j] >> 3);// & 0x1f;
                            alpha *= 8;
                            Color c = Color.FromArgb(alpha, nsbtx.PalInfo.infoBlock.PalInfo[palIndex].pal[index]);
                            b.SetPixel(j - (j / b.Width) * b.Width, j / b.Width, c);
                        }
                        break;
                    case 7:
                        for (int j = 0; j < pixelnum; j++) {
                            UInt16 p = (ushort)(nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].Image[j * 2] + (nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].Image[j * 2 + 1] << 8));
                            Color c = Color.FromArgb((((p & 0x8000) != 0) ? 0xff : 0), (((p >> 0) & 0x1f) << 3), (((p >> 5) & 0x1f) << 3), (((p >> 10) & 0x1f) << 3));
                            b.SetPixel(j - (j / b.Width) * b.Width, j / b.Width, c);
                        }
                        break;
                }
            } catch {
                b.UnlockBits();
            }

            b.UnlockBits();
            return b_;
        }
        #endregion

        private void addEventFileButton_Click(object sender, EventArgs e) {
            /* Add new event file to event folder */
            string eventFilePath = romInfo.GetEventsDirPath() + "\\" + selectEventComboBox.Items.Count.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(eventFilePath, FileMode.Create))) writer.Write(LoadEventFile(0).Save());

            /* Update ComboBox and select new file */
            selectEventComboBox.Items.Add("Event File " + selectEventComboBox.Items.Count);
            selectEventComboBox.SelectedIndex = selectEventComboBox.Items.Count - 1;
        }
        private void eventEditorTabPage_Enter(object sender, EventArgs e) {
            eventOpenGlControl.MakeCurrent();
        }
        private void eventMatrixPictureBox_Click(object sender, EventArgs e) {
            MarkActiveCell(eventMatrixPictureBox.PointToClient(MousePosition).X / 16, eventMatrixPictureBox.PointToClient(MousePosition).Y / 16);
            eventMatrixXUpDown.Value = eventMatrixPictureBox.PointToClient(MousePosition).X / 16;
            eventMatrixYUpDown.Value = eventMatrixPictureBox.PointToClient(MousePosition).Y / 16;
        }
        private void eventMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;
            disableHandlers = true;

            eventMatrix = LoadMatrix((int)eventMatrixUpDown.Value);
            eventMatrixXUpDown.Value = 0;
            eventMatrixYUpDown.Value = 0;
            eventMatrixXUpDown.Maximum = eventMatrix.width - 1;
            eventMatrixYUpDown.Maximum = eventMatrix.height - 1;
            DrawEventMatrix();
            MarkUsedCells();

            disableHandlers = false;
        }
        private void eventShiftLeftButton_Click(object sender, EventArgs e) {
            if (eventMatrixXUpDown.Value > 0) eventMatrixXUpDown.Value -= 1;
        }
        private void eventShiftUpButton_Click(object sender, EventArgs e) {
            if (eventMatrixYUpDown.Value > 0) eventMatrixYUpDown.Value -= 1;
        }
        private void eventShiftRightButton_Click(object sender, EventArgs e) {
            if (eventMatrixXUpDown.Value < eventMatrix.width - 1) eventMatrixXUpDown.Value += 1;
        }
        private void eventShiftDownButton_Click(object sender, EventArgs e) {
            if (eventMatrixYUpDown.Value < eventMatrix.height - 1) eventMatrixYUpDown.Value += 1;
        }
        private void eventMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            DisplayEventMap();
            DisplayActiveEvents();
        }
        private void eventMatrixYUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            DisplayEventMap();
            DisplayActiveEvents();
        }
        private void exportEventFileButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Event File (*.evt)|*.evt";
            if (sf.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(new FileStream(sf.FileName, FileMode.Create))) writer.Write(currentEventFile.Save());
        }
        private void importEventFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .evt file */
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Event File (*.evt)|*.evt";
            if (of.ShowDialog(this) != DialogResult.OK)
                return;

            /* Update matrix object in memory */
            string path = romInfo.GetEventsDirPath() + "\\" + selectEventComboBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Display success message */
            MessageBox.Show("Events imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            /* Refresh controls */
            selectEventComboBox_SelectedIndexChanged(null, null);
        }
        private void removeEventFileButton_Click(object sender, EventArgs e) {
            /* Delete event file */
            File.Delete(romInfo.GetEventsDirPath() + "\\" + (selectEventComboBox.Items.Count - 1).ToString("D4"));

            /* Check if currently selected file is the last one, and in that case select the one before it */
            int lastIndex = selectEventComboBox.Items.Count - 1;
            if (selectEventComboBox.SelectedIndex == lastIndex) selectEventComboBox.SelectedIndex--;

            /* Remove item from ComboBox */
            selectEventComboBox.Items.RemoveAt(lastIndex);
        }
        private void saveEventsButton_Click(object sender, EventArgs e) {
            string eventFile = selectEventComboBox.SelectedIndex.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(romInfo.GetEventsDirPath() + "\\" + eventFile, FileMode.Create))) writer.Write(currentEventFile.Save());
        }
        private void selectEventComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            /* Load events data into EventFile class instance */
            currentEventFile = LoadEventFile(selectEventComboBox.SelectedIndex);

            /* Update ListBoxes */
            FillSpawnablesBox();
            FillOverworldsBox();
            FillTriggersBox();
            FillWarpsBox();

            /* Draw matrix image in the navigator */
            DrawEventMatrix();
            MarkUsedCells();
            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            eventMatrixPictureBox.Invalidate();

            /* Render events on map */
            DisplayEventMap();
            DisplayActiveEvents();
        }
        private void showEventsCheckBoxes_CheckedChanged(object sender, EventArgs e) {
            DisplayActiveEvents();
        }

        private void eventPictureBox_Click(object sender, EventArgs e) {
            Point coordinates = eventPictureBox.PointToClient(Cursor.Position);
            Point mouseTilePos = new Point(coordinates.X / 17, coordinates.Y / 17);
            MouseEventArgs mea = (MouseEventArgs)e;

            if (mea.Button == MouseButtons.Left) {
                if (selectedEvent != null) {

                    selectedEvent.xMapPosition = (short)mouseTilePos.X;
                    selectedEvent.yMapPosition = (short)mouseTilePos.Y;
                    selectedEvent.xMatrixPosition = (ushort)eventMatrixXUpDown.Value;
                    selectedEvent.yMatrixPosition = (ushort)eventMatrixYUpDown.Value;

                    DisplayActiveEvents();
                }
            } else if (mea.Button == MouseButtons.Right) {
                for (int i = 0; i < currentEventFile.warps.Count; i++) {
                    Warp ev = currentEventFile.warps[i];
                    if (isEventUnderMouse(ev, mouseTilePos)) {
                        if (ev == selectedEvent) {
                            goToWarpDestination();
                            return;
                        }
                        selectedEvent = ev;
                        eventsTabControl.SelectedTab = warpsTabPage;
                        warpsListBox.SelectedIndex = i;
                        DisplayActiveEvents();
                        return;
                    }
                }
                for (int i = 0; i < currentEventFile.spawnables.Count; i++) {
                    Spawnable ev = currentEventFile.spawnables[i];
                    if (isEventUnderMouse(ev, mouseTilePos)) {
                        selectedEvent = ev;
                        eventsTabControl.SelectedTab = signsTabPage;
                        spawnablesListBox.SelectedIndex = i;
                        DisplayActiveEvents();
                        return;
                    }
                }
                for (int i = 0; i < currentEventFile.overworlds.Count; i++) {
                    Overworld ev = currentEventFile.overworlds[i];
                    if (isEventUnderMouse(ev, mouseTilePos)) {
                        selectedEvent = ev;
                        eventsTabControl.SelectedTab = overworldsTabPage;
                        overworldsListBox.SelectedIndex = i;
                        DisplayActiveEvents();
                        return;
                    }
                }
                for (int i = 0; i < currentEventFile.triggers.Count; i++) {
                    Trigger ev = currentEventFile.triggers[i];
                    if (isEventUnderMouse(ev, mouseTilePos)) {
                        selectedEvent = ev;
                        eventsTabControl.SelectedTab = triggersTabPage;
                        triggersListBox.SelectedIndex = i;
                        DisplayActiveEvents();
                        return;
                    }
                }
            } else if (mea.Button == MouseButtons.Middle) {
                for (int i = 0; i < currentEventFile.warps.Count; i++) {
                    Warp ev = currentEventFile.warps[i];
                    if (isEventUnderMouse(ev, mouseTilePos)) {
                        if (ev == selectedEvent) {
                            goToWarpDestination();
                            return;
                        }
                    }
                }
            }
        }

        private bool isEventUnderMouse(Event ev, Point mouseTilePos) {
            if (isEventOnCurrentMatrix(ev)) {
                Point evLocalCoords = new Point(ev.xMapPosition, ev.yMapPosition);
                if (evLocalCoords.Equals(mouseTilePos))
                    return true;
            }
            return false;
        }

        #region Spawnables Editor
        private void addSpawnableButton_Click(object sender, EventArgs e) {
            currentEventFile.spawnables.Add(new Spawnable((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));
            spawnablesListBox.Items.Add("Spawnable " + (currentEventFile.spawnables.Count - 1).ToString());
            spawnablesListBox.SelectedIndex = currentEventFile.spawnables.Count - 1;
        }
        private void removeSpawnableButton_Click(object sender, EventArgs e) {
            if (spawnablesListBox.Items.Count > 0) {
                disableHandlers = true;

                /* Remove trigger object from list and the corresponding entry in the ListBox */
                int spawnableNumber = spawnablesListBox.SelectedIndex;
                currentEventFile.spawnables.RemoveAt(spawnableNumber);
                spawnablesListBox.Items.RemoveAt(spawnableNumber);

                FillSpawnablesBox(); // Update ListBox

                disableHandlers = false;

                if (spawnableNumber > 0)
                    spawnablesListBox.SelectedIndex = spawnableNumber - 1;
            }
        }
        private void spawnablesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            #region Disable events for fast execution
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;
            disableHandlers = true;
            #endregion

            selectedEvent = currentEventFile.spawnables[spawnablesListBox.SelectedIndex];

            signScriptUpDown.Value = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].scriptNumber;
            signMapXUpDown.Value = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].xMapPosition;
            signMapYUpDown.Value = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].yMapPosition;
            signZUpDown.Value = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].zPosition;
            signMatrixXUpDown.Value = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].xMatrixPosition;
            signMatrixYUpDown.Value = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].yMatrixPosition;

            DisplayActiveEvents();

            #region Re-enable events
            disableHandlers = false;
            #endregion
        }
        private void signMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].xMatrixPosition = (ushort)signMatrixXUpDown.Value;
            DisplayActiveEvents();
        }
        private void signMatrixYUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].yMatrixPosition = (ushort)signMatrixYUpDown.Value;
            DisplayActiveEvents();
        }
        private void signScriptUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;
            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].scriptNumber = (ushort)signScriptUpDown.Value;
        }
        private void signMapXUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].xMapPosition = (short)signMapXUpDown.Value;
            DisplayActiveEvents();
        }
        private void signMapYUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].yMapPosition = (short)signMapYUpDown.Value;
            DisplayActiveEvents();
        }
        private void signZUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].zPosition = (short)signZUpDown.Value;
            DisplayActiveEvents();
        }
        #endregion

        #region Overworlds Editor
        private void addOverworldButton_Click(object sender, EventArgs e) {
            currentEventFile.overworlds.Add(new Overworld(currentEventFile.overworlds.Count + 1, (int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));
            overworldsListBox.Items.Add("Overworld " + (currentEventFile.overworlds.Count - 1).ToString());
            overworldsListBox.SelectedIndex = currentEventFile.overworlds.Count - 1;
        }
        private void isTrainerRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            /* Disable script number control */
            owScriptNumericUpDown.Enabled = false;

            /* Set special settings controls */
            owSpecialGroupBox.Enabled = true;

            owTrainerComboBox.Enabled = true;
            owTrainerLabel.Enabled = true;

            owSightRangeUpDown.Enabled = true;
            owSightRangeLabel.Enabled = true;
            owPartnerTrainerCheckBox.Enabled = true;

            owItemComboBox.Enabled = false;
            owItemLabel.Enabled = false;

            /* Set trainer flag to true */
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].type = 0x1;

            /* Adjust script number */
            if (owTrainerComboBox.SelectedIndex >= 0)
                owTrainerComboBox_SelectedIndexChanged(null, null);
        }
        private void isItemRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            /* Disable script number control */
            owScriptNumericUpDown.Enabled = false;

            /* Set special settings controls */
            owSpecialGroupBox.Enabled = true;

            owTrainerComboBox.Enabled = false;
            owTrainerLabel.Enabled = false;

            owSightRangeUpDown.Enabled = false;
            owSightRangeLabel.Enabled = false;
            owPartnerTrainerCheckBox.Enabled = false;

            if (isItemRadioButton.Enabled) {
                owItemComboBox.Enabled = true;
                owItemComboBox.SelectedIndex = Math.Max(0, currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber - 7000);
                owItemLabel.Enabled = true;
            }

            /* Set overworld type to item */
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].type = 0x3;

        }
        private void normalRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            /* Enable script number control buttons */
            owScriptNumericUpDown.Enabled = true;

            /* Set special settings controls */
            owSpecialGroupBox.Enabled = false;

            /* Set trainer flag to false and correct script number */
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].type = 0x0;
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)owScriptNumericUpDown.Value;
        }
        private void owItemComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(7000 + owItemComboBox.SelectedIndex);
        }
        private void overworldsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            #region Disable Events for fast execution
            if (disableHandlers) return;
            disableHandlers = true;
            #endregion

            int index = overworldsListBox.SelectedIndex;
            try {
                selectedEvent = currentEventFile.overworlds[index];

                /* Sprite index and image controls */
                owSpriteComboBox.SelectedIndex = MatchOverworldIDToSpriteArchive(currentEventFile.overworlds[index].spriteID, spritesTablePath);
                owSpritePictureBox.BackgroundImage = GetOverworldImage(currentEventFile.overworlds[index].spriteID, currentEventFile.overworlds[index].orientation);
            } catch (ArgumentOutOfRangeException) {
                String errorMsg = "This Overworld's sprite ID couldn't be read correctly.";
                MessageBox.Show(errorMsg, "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try {
                /* Set coordinates controls */
                owXMapUpDown.Value = currentEventFile.overworlds[index].xMapPosition;
                owYMapUpDown.Value = currentEventFile.overworlds[index].yMapPosition;
                owXMatrixUpDown.Value = currentEventFile.overworlds[index].xMatrixPosition;
                owYMatrixUpDown.Value = currentEventFile.overworlds[index].yMatrixPosition;
                owZPositionUpDown.Value = currentEventFile.overworlds[index].zPosition;

                /*ID, Flag and Script number controls */
                owIDNumericUpDown.Value = currentEventFile.overworlds[index].owID;
                owFlagNumericUpDown.Value = currentEventFile.overworlds[index].flag;
                owScriptNumericUpDown.Value = currentEventFile.overworlds[index].scriptNumber;

                /* Special settings controls */
                if (currentEventFile.overworlds[index].type == 0x1) {
                    disableHandlers = false;
                    isTrainerRadioButton.Checked = true;
                    disableHandlers = true;
                    if (currentEventFile.overworlds[index].scriptNumber >= 4999) {
                        owTrainerComboBox.SelectedIndex = Math.Max(currentEventFile.overworlds[index].scriptNumber - 4999, 0); // Partner of double battle trainer
                        owPartnerTrainerCheckBox.Checked = true;
                    } else {
                        owTrainerComboBox.SelectedIndex = Math.Max(currentEventFile.overworlds[index].scriptNumber - 2999, 0); // Normal trainer
                        owPartnerTrainerCheckBox.Checked = false;
                    }
                } else if (currentEventFile.overworlds[index].type == 0x3 || currentEventFile.overworlds[index].scriptNumber >= 7000 && currentEventFile.overworlds[index].scriptNumber <= 8000) {
                    disableHandlers = false;
                    isItemRadioButton.Checked = true;
                    owItemComboBox.SelectedIndex = Math.Max(currentEventFile.overworlds[index].scriptNumber - 7000, 0);
                    disableHandlers = true;
                } else {
                    disableHandlers = false;
                    normalRadioButton.Checked = true;
                    disableHandlers = true;
                }


                /* Movement settings */
                owMovementComboBox.SelectedIndex = currentEventFile.overworlds[index].movement;
                owOrientationComboBox.SelectedIndex = currentEventFile.overworlds[index].orientation;
                owSightRangeUpDown.Value = currentEventFile.overworlds[index].sightRange;
                owXRangeUpDown.Value = currentEventFile.overworlds[index].xRange;
                owYRangeUpDown.Value = currentEventFile.overworlds[index].yRange;

                DisplayActiveEvents();
            } catch (ArgumentOutOfRangeException) {
                String errorMsg = "There was a problem loading the overworld events of this Event file.";
                MessageBox.Show(errorMsg, "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            #region Re-enable events
            disableHandlers = false;
            #endregion
        }
        private void owFlagNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].flag = (ushort)owFlagNumericUpDown.Value;
        }
        private void owIDNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;
            if (currentEventFile.overworlds.Any() && overworldsListBox.SelectedIndex > 0)
                currentEventFile.overworlds[overworldsListBox.SelectedIndex].owID = (ushort)owIDNumericUpDown.Value;
        }
        private void owMovementComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].movement = (ushort)owMovementComboBox.SelectedIndex;
        }
        private void owOrientationComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;

            currentEventFile.overworlds[overworldsListBox.SelectedIndex].orientation = (ushort)owOrientationComboBox.SelectedIndex;
            owSpritePictureBox.BackgroundImage = GetOverworldImage(currentEventFile.overworlds[overworldsListBox.SelectedIndex].spriteID, currentEventFile.overworlds[overworldsListBox.SelectedIndex].orientation);
            DisplayActiveEvents();
            owSpritePictureBox.Invalidate();
        }
        private void owScriptNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)owScriptNumericUpDown.Value;
        }
        private void owSightRangeUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].sightRange = (ushort)owSightRangeUpDown.Value;
        }
        private void owSpriteComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;
            if (overworldsListBox.SelectedIndex != 0) {
                currentEventFile.overworlds[overworldsListBox.SelectedIndex].spriteID = (ushort)owSpriteComboBox.SelectedIndex;
                owSpritePictureBox.BackgroundImage = GetOverworldImage(currentEventFile.overworlds[overworldsListBox.SelectedIndex].spriteID, currentEventFile.overworlds[overworldsListBox.SelectedIndex].orientation);
                DisplayActiveEvents();
                owSpritePictureBox.Invalidate();
            }
        }
        private void owPartnerTrainerCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;

            if (owPartnerTrainerCheckBox.Checked)
                currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber += 2000;
            else
                currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber -= 2000;
        }
        private void owTrainerComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;

            if (owPartnerTrainerCheckBox.Checked) currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(4999 + owTrainerComboBox.SelectedIndex);
            else currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(2999 + owTrainerComboBox.SelectedIndex);
        }
        private void owXMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].xMapPosition = (short)owXMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void owXRangeUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].xRange = (ushort)owXRangeUpDown.Value;
        }
        private void owYRangeUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].yRange = (ushort)owYRangeUpDown.Value;
        }
        private void owYMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].yMapPosition = (short)owYMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void owZPositionUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].zPosition = (short)owZPositionUpDown.Value;
        }
        private void owXMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            currentEventFile.overworlds[overworldsListBox.SelectedIndex].xMatrixPosition = (ushort)owXMatrixUpDown.Value;
            eventMatrixPictureBox.Image = new Bitmap(eventMatrixPictureBox.Width, eventMatrixPictureBox.Height);
            DrawEventMatrix(); // Redraw matrix to eliminate old used cells
            MarkUsedCells(); // Mark new used cells
            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            DisplayActiveEvents();
        }
        private void owYMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            currentEventFile.overworlds[overworldsListBox.SelectedIndex].yMatrixPosition = (ushort)owYMatrixUpDown.Value;
            eventMatrixPictureBox.Image = new Bitmap(eventMatrixPictureBox.Width, eventMatrixPictureBox.Height);
            DrawEventMatrix();
            MarkUsedCells();
            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            DisplayActiveEvents();
        }
        private void removeOverworldButton_Click(object sender, EventArgs e) {
            if (overworldsListBox.Items.Count > 0) {
                disableHandlers = true;

                /* Remove overworld object from list and the corresponding entry in the ListBox */
                int owNumber = overworldsListBox.SelectedIndex;
                currentEventFile.overworlds.RemoveAt(owNumber);
                overworldsListBox.Items.RemoveAt(owNumber);

                FillOverworldsBox(); // Update ListBox

                disableHandlers = false;

                if (owNumber > 0) overworldsListBox.SelectedIndex = owNumber - 1;
            }
        }
        #endregion

        #region Warps Editor
        private void addWarpButton_Click(object sender, EventArgs e) {
            currentEventFile.warps.Add(new Warp((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));
            warpsListBox.Items.Add("Warp " + (currentEventFile.warps.Count - 1).ToString());
            warpsListBox.SelectedIndex = currentEventFile.warps.Count - 1;
        }
        private void removeWarpButton_Click(object sender, EventArgs e) {
            if (warpsListBox.Items.Count > 0) {
                disableHandlers = true;

                /* Remove warp object from list and the corresponding entry in the ListBox */
                int warpNumber = warpsListBox.SelectedIndex;
                currentEventFile.warps.RemoveAt(warpNumber);
                warpsListBox.Items.RemoveAt(warpNumber);

                FillWarpsBox(); // Update ListBox

                disableHandlers = false;

                if (warpNumber > 0) warpsListBox.SelectedIndex = warpNumber - 1;
            }
        }
        private void warpAnchorUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            currentEventFile.warps[warpsListBox.SelectedIndex].anchor = (ushort)warpAnchorUpDown.Value;
        }
        private void warpHeaderUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            currentEventFile.warps[warpsListBox.SelectedIndex].header = (ushort)warpHeaderUpDown.Value;
        }
        private void warpsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            #region Disable events for fast execution
            if (disableHandlers || warpsListBox.SelectedIndex < 0)
                return;
            disableHandlers = true;
            #endregion

            selectedEvent = currentEventFile.warps[warpsListBox.SelectedIndex];

            warpHeaderUpDown.Value = currentEventFile.warps[warpsListBox.SelectedIndex].header;
            warpAnchorUpDown.Value = currentEventFile.warps[warpsListBox.SelectedIndex].anchor;
            warpXMapUpDown.Value = currentEventFile.warps[warpsListBox.SelectedIndex].xMapPosition;
            warpYMapUpDown.Value = currentEventFile.warps[warpsListBox.SelectedIndex].yMapPosition;
            warpZUpDown.Value = currentEventFile.warps[warpsListBox.SelectedIndex].zPosition;
            warpXMatrixUpDown.Value = currentEventFile.warps[warpsListBox.SelectedIndex].xMatrixPosition;
            warpYMatrixUpDown.Value = currentEventFile.warps[warpsListBox.SelectedIndex].yMatrixPosition;

            DisplayActiveEvents(); // Redraw events to show selection box

            #region Re-enable events
            disableHandlers = false;
            #endregion
        }
        private void warpMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0)
                return;

            currentEventFile.warps[warpsListBox.SelectedIndex].xMatrixPosition = (ushort)warpXMatrixUpDown.Value;
            DisplayActiveEvents();
        }
        private void warpMatrixYUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0)
                return;

            currentEventFile.warps[warpsListBox.SelectedIndex].yMatrixPosition = (ushort)warpYMatrixUpDown.Value;
            DisplayActiveEvents();
        }
        private void warpXMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0)
                return;

            currentEventFile.warps[warpsListBox.SelectedIndex].xMapPosition = (short)warpXMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void warpYMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            currentEventFile.warps[warpsListBox.SelectedIndex].yMapPosition = (short)warpYMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void warpZUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            currentEventFile.warps[warpsListBox.SelectedIndex].zPosition = (short)warpZUpDown.Value;
            DisplayActiveEvents();
        }
        #endregion

        #region Triggers Editor
        private void addTriggerButton_Click(object sender, EventArgs e) {
            currentEventFile.triggers.Add(new Trigger((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));
            triggersListBox.Items.Add("Trigger " + (currentEventFile.triggers.Count - 1).ToString());
            triggersListBox.SelectedIndex = currentEventFile.triggers.Count - 1;
        }
        private void removeTriggerButton_Click(object sender, EventArgs e) {
            if (triggersListBox.Items.Count > 0) {
                disableHandlers = true;

                /* Remove trigger object from list and the corresponding entry in the ListBox */
                int triggerNumber = triggersListBox.SelectedIndex;
                currentEventFile.triggers.RemoveAt(triggerNumber);
                triggersListBox.Items.RemoveAt(triggerNumber);

                FillTriggersBox(); // Update ListBox

                disableHandlers = false;

                if (triggerNumber > 0) triggersListBox.SelectedIndex = triggerNumber - 1;
            }
        }
        private void triggerFlagUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            currentEventFile.triggers[triggersListBox.SelectedIndex].flag = (ushort)triggerFlagUpDown.Value;
        }
        private void triggersListBox_SelectedIndexChanged(object sender, EventArgs e) {
            #region Disable events for fast execution
            if (disableHandlers) return;
            disableHandlers = true;
            #endregion

            selectedEvent = currentEventFile.triggers[triggersListBox.SelectedIndex];

            triggerScriptUpDown.Value = currentEventFile.triggers[triggersListBox.SelectedIndex].scriptNumber;
            triggerFlagUpDown.Value = currentEventFile.triggers[triggersListBox.SelectedIndex].flag;
            triggerWidthUpDown.Value = currentEventFile.triggers[triggersListBox.SelectedIndex].width;
            triggerLengthUpDown.Value = currentEventFile.triggers[triggersListBox.SelectedIndex].length;

            triggerXMapUpDown.Value = currentEventFile.triggers[triggersListBox.SelectedIndex].xMapPosition;
            triggerYMapUpDown.Value = currentEventFile.triggers[triggersListBox.SelectedIndex].yMapPosition;
            triggerZUpDown.Value = currentEventFile.triggers[triggersListBox.SelectedIndex].zPosition;
            triggerXMatrixUpDown.Value = currentEventFile.triggers[triggersListBox.SelectedIndex].xMatrixPosition;
            triggerYMatrixUpDown.Value = currentEventFile.triggers[triggersListBox.SelectedIndex].yMatrixPosition;

            DisplayActiveEvents();

            #region Re-enable events
            disableHandlers = false;
            #endregion
        }
        private void triggerLengthUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].length = (ushort)triggerLengthUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerScriptUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            currentEventFile.triggers[triggersListBox.SelectedIndex].scriptNumber = (ushort)triggerScriptUpDown.Value;
        }
        private void triggerXMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].xMapPosition = (short)triggerXMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerYMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].yMapPosition = (short)triggerYMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerZUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].zPosition = (short)triggerZUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerXMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].xMatrixPosition = (ushort)triggerXMatrixUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerYMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].yMatrixPosition = (ushort)triggerYMatrixUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerWidthUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].width = (ushort)triggerWidthUpDown.Value;
            DisplayActiveEvents();
        }
        #endregion

        #endregion

        #region Script Editor

        #region Variables
        public ScriptFile currentScriptFile;
        #endregion

        #region Subroutines
        public ScriptFile LoadScriptFile(int fileID) {
            return new ScriptFile((new FileStream(romInfo.GetScriptDirPath() +
                "\\" + fileID.ToString("D4"), FileMode.Open)), romInfo.GetGameVersion());
        }
        public void SaveScriptFile(int fileID) {
            using (BinaryWriter writer = new BinaryWriter((new FileStream(romInfo.GetScriptDirPath() +
                "\\" + fileID.ToString("D4"), FileMode.Create)))) writer.Write(currentScriptFile.Save());
        }
        #endregion

        #region LineNumbers Scripts
        public int GetWidthScript() {
            int w = 29;
            // get total lines of scriptTextBox    
            int line = scriptTextBox.Lines.Length;

            if (line <= 99) {
                w = 24 + (int)scriptTextBox.Font.Size;
            } else if (line <= 999) {
                w = 34 + (int)scriptTextBox.Font.Size;
            } else {
                w = 54 + (int)scriptTextBox.Font.Size;
            }

            return w;
        }

        public void AddLineNumbers(RichTextBox mainbox, RichTextBox linebox) {
            // create & set Point pt to (0,0)    
            Point pt = new Point(0, 0);

            // get First Index & First Line from scriptTextBox    
            int First_Index = mainbox.GetCharIndexFromPosition(pt);
            int First_Line = mainbox.GetLineFromCharIndex(First_Index);

            // set X & Y coordinates of Point pt to ClientRectangle Width & Height respectively    
            pt.X = ClientRectangle.Width;
            pt.Y = ClientRectangle.Height;

            // get Last Index & Last Line from scriptTextBox    
            int Last_Index = mainbox.GetCharIndexFromPosition(pt);
            int Last_Line = mainbox.GetLineFromCharIndex(Last_Index);

            // set Center alignment to LineNumberTextBox    
            linebox.SelectionAlignment = HorizontalAlignment.Center;

            // set LineNumberTextBox text to null & width to GetWidth() function value    
            linebox.Text = "";
            linebox.Width = GetWidthScript();

            // now add each line number to LineNumberTextBox upto last line    
            for (int i = First_Line + 1; i <= Last_Line; i++) {
                linebox.Text += i + "\n";
            }
        }

        private void scriptTextBox_SelectionChanged(object sender, EventArgs e) {
            Point pt = scriptTextBox.GetPositionFromCharIndex(scriptTextBox.SelectionStart);
            if (pt.X == 1) {
                AddLineNumbers(scriptTextBox, LineNumberTextBoxScript);
            }
        }

        private void scriptTextBox_VScroll(object sender, EventArgs e) {
            LineNumberTextBoxScript.Text = "";
            AddLineNumbers(scriptTextBox, LineNumberTextBoxScript);
            LineNumberTextBoxScript.Invalidate();
        }

        private void scriptTextBox_TextChanged(object sender, EventArgs e) {
            if (scriptTextBox.Text == "") {
                AddLineNumbers(scriptTextBox, LineNumberTextBoxScript);
            }
        }

        private void LineNumberTextBoxScript_MouseDown(object sender, MouseEventArgs e) {
            scriptTextBox.Select();
            LineNumberTextBoxScript.DeselectAll();
        }
        #endregion
        #region LineNumbers Functions
        public int GetWidthFunc() {
            int w = 25;
            // get total lines of functionTextBox    
            int line = functionTextBox.Lines.Length;

            if (line <= 99) {
                w = 20 + (int)functionTextBox.Font.Size;
            } else if (line <= 999) {
                w = 30 + (int)functionTextBox.Font.Size;
            } else {
                w = 50 + (int)functionTextBox.Font.Size;
            }

            return w;
        }

        private void functionTextBox_SelectionChanged(object sender, EventArgs e) {
            Point pt = functionTextBox.GetPositionFromCharIndex(functionTextBox.SelectionStart);
            if (pt.X == 1) {
                AddLineNumbers(functionTextBox, LineNumberTextBoxFunc);
            }
        }

        private void functionTextBox_VScroll(object sender, EventArgs e) {
            LineNumberTextBoxFunc.Text = "";
            AddLineNumbers(functionTextBox, LineNumberTextBoxFunc);
            LineNumberTextBoxFunc.Invalidate();
        }

        private void functionTextBox_TextChanged(object sender, EventArgs e) {
            if (functionTextBox.Text == "") {
                AddLineNumbers(functionTextBox, LineNumberTextBoxFunc);
            }
        }

        private void LineNumberTextBoxFunc_MouseDown(object sender, MouseEventArgs e) {
            functionTextBox.Select();
            LineNumberTextBoxFunc.DeselectAll();
        }
        #endregion
        #region LineNumbers Movements
        public int GetWidthMov() {
            int w = 25;
            // get total lines of movementTextBox    
            int line = movementTextBox.Lines.Length;

            if (line <= 99) {
                w = 20 + (int)movementTextBox.Font.Size;
            } else if (line <= 999) {
                w = 30 + (int)movementTextBox.Font.Size;
            } else {
                w = 50 + (int)movementTextBox.Font.Size;
            }

            return w;
        }

        private void movementTextBox_SelectionChanged(object sender, EventArgs e) {
            Point pt = movementTextBox.GetPositionFromCharIndex(movementTextBox.SelectionStart);
            if (pt.X == 1) {
                AddLineNumbers(movementTextBox, LineNumberTextBoxMov);
            }
        }

        private void movementTextBox_VScroll(object sender, EventArgs e) {
            LineNumberTextBoxMov.Text = "";
            AddLineNumbers(movementTextBox, LineNumberTextBoxMov);
            LineNumberTextBoxMov.Invalidate();
        }

        private void movementTextBox_TextChanged(object sender, EventArgs e) {
            if (movementTextBox.Text == "") {
                AddLineNumbers(movementTextBox, LineNumberTextBoxMov);
            }
        }

        private void LineNumberTextBoxMov_MouseDown(object sender, MouseEventArgs e) {
            movementTextBox.Select();
            LineNumberTextBoxMov.DeselectAll();
        }
        #endregion
        private void addScriptFileButton_Click(object sender, EventArgs e) {
            /* Add new event file to event folder */
            string scriptFilePath = romInfo.GetScriptDirPath() + "\\" + selectScriptFileComboBox.Items.Count.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(scriptFilePath, FileMode.Create))) writer.Write(LoadScriptFile(0).Save());

            /* Update ComboBox and select new file */
            selectScriptFileComboBox.Items.Add("Script File " + selectScriptFileComboBox.Items.Count);
            selectScriptFileComboBox.SelectedIndex = selectScriptFileComboBox.Items.Count - 1;
        }
        private void exportScriptFileButton_Click(object sender, EventArgs e) {
            exportScriptFile();
        }
        private void exportScriptFile() {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Script File (*.scr)|*.scr";
            if (sf.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(new FileStream(sf.FileName, FileMode.Create))) writer.Write(currentScriptFile.Save());
        }

        private void importScriptFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .scr file */
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Script File (*.scr)|*.scr";
            if (of.ShowDialog(this) != DialogResult.OK) return;

            /* Update scriptFile object in memory */
            string path = romInfo.GetScriptDirPath() + "\\" + selectScriptFileComboBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Display success message */
            MessageBox.Show("Scripts imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            /* Refresh controls */
            selectScriptFileComboBox_SelectedIndexChanged(null, null);
        }
        private void openScriptButton_Click(object sender, EventArgs e) {
            if (!scriptEditorIsReady) {
                SetupScriptEditor();
                scriptEditorIsReady = true;
            }

            selectScriptFileComboBox.SelectedIndex = (int)scriptFileUpDown.Value;
            mainTabControl.SelectedTab = scriptEditorTabPage;
        }

        private void openLevelScriptButton_Click(object sender, EventArgs e) {
            String errorMsg = "Level scripts are currently not supported.\n" +
                    "For that, you can use AdAstra's Level Script Editor.";
            MessageBox.Show(errorMsg, "Unimplemented feature", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void removeScriptFileButton_Click(object sender, EventArgs e) {
            /* Delete script file */
            File.Delete(romInfo.GetScriptDirPath() + "\\" + (selectScriptFileComboBox.Items.Count - 1).ToString("D4"));

            /* Check if currently selected file is the last one, and in that case select the one before it */
            int lastIndex = selectScriptFileComboBox.Items.Count - 1;
            if (selectScriptFileComboBox.SelectedIndex == lastIndex)
                selectScriptFileComboBox.SelectedIndex--;

            /* Remove item from ComboBox */
            selectScriptFileComboBox.Items.RemoveAt(lastIndex);
        }
        private void saveScriptFileButton_Click(object sender, EventArgs e) {
            currentScriptFile.scripts.Clear();
            currentScriptFile.functions.Clear();
            currentScriptFile.movements.Clear();

            /* Create new script objects */
            List<Command> scriptCommands = new List<Command>();
            populateScriptCommands(scriptCommands);

            List<Command> functionCommands = new List<Command>();
            populateFunctionCommands(scriptCommands);

            List<Command> movementCommands = new List<Command>();
            populateMovementCommands(movementCommands);

            /* Write new scripts to file */
            SaveScriptFile(selectScriptFileComboBox.SelectedIndex);
        }

        private void populateScriptCommands(List<Command> commands) {
            for (int i = 0; i < scriptTextBox.Lines.Length; i++) {
                if (!scriptTextBox.Lines[i].Contains('@'))
                    continue; // Move on until script header is found
                else {
                    i++; // Skip line
                    while (scriptTextBox.Lines[i].Length == 0)
                        i++; //Skip all empty lines 

                    if (scriptTextBox.Lines[i].Contains("UseScript")) {
                        int scriptNumber = Int16.Parse(scriptTextBox.Lines[i].Substring(1 + scriptTextBox.Lines[i].IndexOf('#')));
                        currentScriptFile.scripts.Add(new Script(scriptNumber));
                    } else {
                        /* Read script commands */
                        while (scriptTextBox.Lines[i] != "End" && !scriptTextBox.Lines[i].Contains("Jump Function") && i < scriptTextBox.Lines.Length - 1) {
                            Console.WriteLine("Script line " + i.ToString());
                            commands.Add(new Command(scriptTextBox.Lines[i], romInfo.GetGameVersion(), false));
                            i++;
                        }
                        commands.Add(new Command(scriptTextBox.Lines[i], romInfo.GetGameVersion(), false)); // Add end or jump/call command
                        currentScriptFile.scripts.Add(new Script(commands));
                    }
                }
            }
        }

        private void populateFunctionCommands(List<Command> commands) {
            for (int i = 0; i < functionTextBox.Lines.Length; i++) {
                if (!functionTextBox.Lines[i].Contains('@'))
                    continue; // Move on until function header is found
                else {
                    i += 0x2; // Skip blank line

                    /* Read function commands */
                    while (functionTextBox.Lines[i] != "End" && !functionTextBox.Lines[i].Contains("Return") && !functionTextBox.Lines[i].Contains("Jump F")) {
                        commands.Add(new Command(functionTextBox.Lines[i], romInfo.GetGameVersion(), false));
                        i++;
                    }
                    commands.Add(new Command(functionTextBox.Lines[i], romInfo.GetGameVersion(), false)); // Add end command
                    currentScriptFile.functions.Add(new Script(commands));
                }
            }
        }


        private void populateMovementCommands(List<Command> commands) {
            for (int i = 0; i < movementTextBox.Lines.Length; i++) {
                if (!movementTextBox.Lines[i].Contains('@'))
                    continue; // Move on until script header is found
                else {
                    i += 0x2; // Skip blank line

                    /* Read script commands */
                    while (movementTextBox.Lines[i] != "End") {
                        commands.Add(new Command(movementTextBox.Lines[i], romInfo.GetGameVersion(), true));
                        i++;
                    }
                    commands.Add(new Command(movementTextBox.Lines[i], romInfo.GetGameVersion(), true)); // Add end command

                    currentScriptFile.movements.Add(new Script(commands));
                }
            }
        }


        private void searchInScriptsButton_Click(object sender, EventArgs e) {
            searchInScriptsResultTextBox.Clear();
            string searchString = searchInScriptsUpDown.Text;
            searchProgressBar.Maximum = selectScriptFileComboBox.Items.Count;

            for (int i = 0; i < selectScriptFileComboBox.Items.Count; i++) {
                try {
                    ScriptFile file = LoadScriptFile(i);

                    for (int j = 0; j < file.scripts.Count; j++) {
                        for (int k = 0; k < file.scripts[j].commands.Count; k++) {
                            if (file.scripts[j].commands[k].cmdName.Contains(searchString))
                                searchInScriptsResultTextBox.AppendText(i + " - " + "Script " + (j + 1) + ": " + file.scripts[j].commands[k].cmdName + Environment.NewLine);
                        }
                    }
                    for (int j = 0; j < file.functions.Count; j++) {
                        for (int k = 0; k < file.functions[j].commands.Count; k++) {
                            if (file.functions[j].commands[k].cmdName.Contains(searchString))
                                searchInScriptsResultTextBox.AppendText(i + " - " + "Function " + (j + 1) + ": " + file.functions[j].commands[k].cmdName + Environment.NewLine);
                        }
                    }
                } catch { }
                searchProgressBar.Value = i;
            }
            searchProgressBar.Value = 0;
        }
        private void selectScriptFileComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            /* clear controls */
            scriptTextBox.Clear();
            functionTextBox.Clear();
            movementTextBox.Clear();

            currentScriptFile = LoadScriptFile(selectScriptFileComboBox.SelectedIndex); // Load script file

            if (currentScriptFile.isLevelScript) {
                scriptTextBox.Focus();
                scriptTextBox.Text += "Level script files currently not supported";
                functionTextBox.Enabled = false;
                movementTextBox.Enabled = false;
            } else {
                functionTextBox.Enabled = true;
                movementTextBox.Enabled = true;

                /* Add scripts */
                statusLabel.Text = "Parsing Script commands...";
                for (int i = 0; i < currentScriptFile.scripts.Count; i++) {
                    Script currentScript = currentScriptFile.scripts[i];

                    /* Write header */
                    string scrHeader = "----- " + "@Script_#" + (i + 1) + " -----" + Environment.NewLine;
                    scriptTextBox.AppendText(scrHeader, Color.Green);
                    scriptTextBox.Text += Environment.NewLine;

                    /* If current script is identical to another, print UseScript instead of commands */
                    if (currentScript.useScript != -1)
                        scriptTextBox.Text += "UseScript_#" + currentScript.useScript;
                    else {
                        for (int j = 0; j < currentScript.commands.Count; j++)
                            scriptTextBox.AppendText(currentScript.commands[j].cmdName + Environment.NewLine, Color.Black);
                    }
                    scriptTextBox.Text += Environment.NewLine; // Write blank line to separate next script
                }

                /* Add functions */
                statusLabel.Text = "Parsing Functions...";
                for (int i = 0; i < currentScriptFile.functions.Count; i++) {
                    Script currentFunction = currentScriptFile.functions[i];

                    string funcHeader = "----- " + "@Function_#" + (i + 1) + " -----" + Environment.NewLine;
                    functionTextBox.AppendText(funcHeader, Color.Blue);
                    functionTextBox.Text += Environment.NewLine;
                    for (int j = 0; j < currentFunction.commands.Count; j++)
                        functionTextBox.Text += currentFunction.commands[j].cmdName + Environment.NewLine;

                    functionTextBox.Text += Environment.NewLine;
                }

                /* Add movements */
                statusLabel.Text = "Parsing Movements...";
                for (int i = 0; i < currentScriptFile.movements.Count; i++) {
                    Script currentMovement = currentScriptFile.movements[i];

                    string movHeader = "----- " + "@Movement_#" + (i + 1) + " -----" + Environment.NewLine;
                    movementTextBox.AppendText(movHeader, Color.Brown);
                    movementTextBox.Text += Environment.NewLine;
                    for (int j = 0; j < currentMovement.commands.Count; j++)
                        movementTextBox.Text += currentMovement.commands[j].cmdName + Environment.NewLine;

                    movementTextBox.Text += Environment.NewLine;
                }
            }
            statusLabel.Text = "Ready";
            AddLineNumbers(scriptTextBox, LineNumberTextBoxScript);
            AddLineNumbers(functionTextBox, LineNumberTextBoxFunc);
            AddLineNumbers(movementTextBox, LineNumberTextBoxMov);
        }

        #region Miscellaneous
        private void clearflagButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert flag number (hex):", "hex")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "ClearFlag 0x" + ((int)f.numericUpDown1.Value).ToString("X4"));
            }
        }
        private void messageButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert message number:", "dec")) {
                f.ShowDialog();
                if (f.okSelected) {
                    String msg = "\nSetVariableHero 0x0" +
                        "\nPlayFanfare 0x5DC" + "\nLockAll" + "\nFacePlayer" +
                        "\nMessage 0x" + ((int)f.numericUpDown1.Value).ToString("X") +
                        "\nWaitButton" + "\nCloseMessage" + "\nReleaseAll";
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, msg);
                }
            }
        }
        private void setflagButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert flag number (hex):", "hex")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "\nSetFlag 0x" + ((int)f.numericUpDown1.Value).ToString("X4"));
                else
                    f.Hide();
            }
        }
        private void setvarButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert variable number (hex):", "hex")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "SetVar 0x" + ((int)f.numericUpDown1.Value).ToString("X4"));
                else
                    f.Hide();
            }
        }
        private void jumpToFuncButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Function number to jump to:", "Decimal")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "Jump Function_#" + ((int)f.numericUpDown1.Value).ToString());
                else
                    f.Hide();
            }
        }

        private void playCryButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert cry number (hex):", "hex")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "PlayCry 0x" + ((int)f.numericUpDown1.Value).ToString("X4"));
                else
                    f.Hide();
            }
        }
        private void routeSignButton_Click(object sender, EventArgs e) {

        }
        private void trainerBattleButton_Click(object sender, EventArgs e) {

        }
        #endregion

        #region Overworlds
        private void giveItemButton_Click(object sender, EventArgs e) {
            using (GiveItemDialog f = new GiveItemDialog(GetItemNames())) {
                f.ShowDialog();
                if (f.okSelected) {
                    string firstLine = "SetVar 0x8004 0x" + f.itemComboBox.SelectedIndex.ToString("X");
                    string secondLine = "SetVar 0x8005 0x" + ((int)f.quantityNumericUpDown.Value).ToString("X");
                    string thirdLine = "CallStandard 0xFC 0x7";

                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, firstLine + "\r" + secondLine + "\r" + thirdLine);

                } else
                    f.Hide();

            }
        }
        private void lockallButton_Click(object sender, EventArgs e) {
            scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "LockAll");
        }
        private void releaseallButton_Click(object sender, EventArgs e) {
            scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "ReleaseAll");
        }
        private void lockButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("ID of the overworld to lock:", "dec")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "Lock" + " " + "Overworld_#" + ((int)f.numericUpDown1.Value).ToString("D"));
                else
                    f.Hide();
            }
        }
        private void releaseButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("ID of the overworld to release:", "dec")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "Release" + " " + "Overworld_#" + ((int)f.numericUpDown1.Value).ToString("D"));
                else
                    f.Hide();
            }
        }
        private void waitmovementButton_Click(object sender, EventArgs e) {
            scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "WaitMovement");
        }
        private void addpeopleButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("ID of the Overworld to add:", "dec")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "AddOW" + " " + "Overworld_#" + ((int)f.numericUpDown1.Value).ToString("D"));
                else
                    f.Hide();
            }
        }
        private void removepeopleButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("ID of the Overworld to remove:", "dec")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "RemoveOW" + " " + "Overworld_#" + ((int)f.numericUpDown1.Value).ToString("D"));
                else
                    f.Hide();
            }
        }
        #endregion

        #region Give/Take
        private void givePokémonButton_Click(object sender, EventArgs e) {
            using (GivePokémonDialog f = new GivePokémonDialog(GetPokémonNames(), GetItemNames(), GetAttackNames())) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, f.command);
                else
                    f.Hide();
            }
        }
        private void giveMoneyButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert money amount:", "dec")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "GiveMoney" + " " + "0x" + ((int)f.numericUpDown1.Value).ToString("X"));
                else
                    f.Hide();
            }
        }
        private void takeMoneyButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert money amount:", "dec")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "TakeMoney" + " " + "0x" + ((int)f.numericUpDown1.Value).ToString("X"));
                else
                    f.Hide();
            }
        }
        private void takeItemButton_Click(object sender, EventArgs e) {
            using (GiveItemDialog f = new GiveItemDialog(GetItemNames())) {
                f.ShowDialog();
                if (f.okSelected) {
                    string item = f.itemComboBox.SelectedIndex.ToString("X");
                    string quantity = ((int)f.quantityNumericUpDown.Value).ToString("X");

                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "TakeItem" + " " + "0x" + item + " " + "0x" + quantity + " " + "0x800C");

                } else
                    f.Hide();

            }
        }
        private void giveBadgeButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert badge number:", "dec")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "EnableBadge 0x" + ((int)f.numericUpDown1.Value).ToString("X"));
                else
                    f.Hide();
            }
        }
        private void takeBadgeButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert badge number:", "dec")) {
                f.ShowDialog();
                if (f.okSelected)
                    scriptTextBox.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "DisableBadge 0x" + ((int)f.numericUpDown1.Value).ToString("X"));
                else
                    f.Hide();
            }
        }
        #endregion

        #endregion

        #region Text Editor

        #region Variables
        TextArchive currentMessageFile;
        #endregion

        #region Subroutines

        #endregion

        private void addMessageFileButton_Click(object sender, EventArgs e) {
            /* Add new event file to event folder */
            string messageFilePath = romInfo.GetTextArchivesPath() + "\\" + selectTextFileComboBox.Items.Count.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(messageFilePath, FileMode.Create))) writer.Write(LoadMessageArchive(0).Save());

            /* Update ComboBox and select new file */
            selectTextFileComboBox.Items.Add("Text Archive " + selectTextFileComboBox.Items.Count);
            selectTextFileComboBox.SelectedIndex = selectTextFileComboBox.Items.Count - 1;
        }
        private void addStringButton_Click(object sender, EventArgs e) {
            currentMessageFile.messages.Add("");
            textEditorDataGridView.Rows.Add("");

            int rowInd = textEditorDataGridView.RowCount - 1;

            disableHandlers = true;
            textEditorDataGridView.Rows[rowInd].HeaderCell.Value = "0x" + rowInd.ToString("X");
            disableHandlers = false;

        }
        private void exportTextFileButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Text Archive (*.msg)|*.msg";
            if (sf.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(new FileStream(sf.FileName, FileMode.Create))) writer.Write(currentMessageFile.Save());
        }
        private void importTextFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .msg file */
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Text Archive (*.msg)|*.msg";
            if (of.ShowDialog(this) != DialogResult.OK) return;

            /* Update Text Archive object in memory */
            string path = romInfo.GetTextArchivesPath() + "\\" + selectTextFileComboBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Display success message */
            MessageBox.Show("Text Archive imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            /* Refresh controls */
            selectTextFileComboBox_SelectedIndexChanged(null, null);
        }
        private void removeMessageFileButton_Click(object sender, EventArgs e) {
            /* Delete Text Archive */
            File.Delete(romInfo.GetTextArchivesPath() + "\\" + (selectTextFileComboBox.Items.Count - 1).ToString("D4"));

            /* Check if currently selected file is the last one, and in that case select the one before it */
            int lastIndex = selectTextFileComboBox.Items.Count - 1;
            if (selectTextFileComboBox.SelectedIndex == lastIndex) selectTextFileComboBox.SelectedIndex--;

            /* Remove item from ComboBox */
            selectTextFileComboBox.Items.RemoveAt(lastIndex);
        }
        private void removeStringButton_Click(object sender, EventArgs e) {
            if (currentMessageFile.messages.Count > 0) {
                currentMessageFile.messages.RemoveAt(currentMessageFile.messages.Count - 1);
                textEditorDataGridView.Rows.RemoveAt(textEditorDataGridView.Rows.Count - 1);
            }
        }
        private void saveTextArchiveButton_Click(object sender, EventArgs e) {
            saveTextArchive();
        }

        private void saveTextArchive() {
            BinaryWriter textWriter = new BinaryWriter(new FileStream(romInfo.GetTextArchivesPath() + "\\" + selectTextFileComboBox.SelectedIndex.ToString("D4"), FileMode.Create));
            textWriter.Write((UInt16)currentMessageFile.messages.Count);
            textWriter.Write((UInt16)currentMessageFile.initialKey);
            int key = (currentMessageFile.initialKey * 0x2FD) & 0xFFFF;
            int key2 = 0;
            int realKey = 0;
            int offset = 0x4 + (currentMessageFile.messages.Count * 8);
            int[] stringSize = new int[currentMessageFile.messages.Count];

            for (int i = 0; i < currentMessageFile.messages.Count; i++) // Reads and stores string offsets and sizes
            {
                key2 = (key * (i + 1) & 0xFFFF);
                realKey = key2 | (key2 << 16);
                textWriter.Write(offset ^ realKey);
                int length = currentMessageFile.GetStringLength(textEditorDataGridView[0, i].Value.ToString());
                stringSize[i] = length;
                textWriter.Write(length ^ realKey);
                offset += length * 2;
            }
            for (int i = 0; i < currentMessageFile.messages.Count; i++) // Encodes strings and writes them to file
            {
                key = (0x91BD3 * (i + 1)) & 0xFFFF;
                int[] currentString = currentMessageFile.EncodeString(textEditorDataGridView[0, i].Value.ToString(), i, stringSize[i]);
                for (int j = 0; j < stringSize[i] - 1; j++) {
                    textWriter.Write((UInt16)(currentString[j] ^ key));
                    key += 0x493D;
                    key &= 0xFFFF;
                }
                textWriter.Write((UInt16)(0xFFFF ^ key));
            }
            textWriter.Close();
        }
        private void searchMessageButton_Click(object sender, EventArgs e) {
            searchMessageResultTextBox.Clear();
            string searchString = searchMessageTextBox.Text;
            textSearchProgressBar.Maximum = romInfo.GetTextArchivesCount();
            int textArchivesCount = romInfo.GetTextArchivesCount();
            if (textArchivesCount > 828)
                textArchivesCount = 828;

            caseSensitiveCheckbox.Enabled = false;
            if (caseSensitiveCheckbox.Checked) {
                for (int i = 0; i < textArchivesCount; i++) {

                    TextArchive file = LoadMessageArchive(i);

                    for (int j = 0; j < file.messages.Count; j++)
                        if (file.messages[j].IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                            searchMessageResultTextBox.AppendText("(" + i + ")" + " - Line #" + j.ToString("D") + ", " + Environment.NewLine);
                        }
                    textSearchProgressBar.Value = i;
                }
            } else {
                for (int i = 0; i < textArchivesCount; i++) {

                    TextArchive file = LoadMessageArchive(i);

                    for (int j = 0; j < file.messages.Count; j++)
                        if (file.messages[j].IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                            searchMessageResultTextBox.AppendText("(" + i + ")" + " - Line #" + j.ToString("D") + ", " + Environment.NewLine);
                        }
                    textSearchProgressBar.Value = i;
                }
            }

            textSearchProgressBar.Value = 0;
            caseSensitiveCheckbox.Enabled = true;
        }
        private void replaceMessageButton_Click(object sender, EventArgs e) {
            // Usage: search box -> WORD_TO_REPLACE, NEW_WORD
            searchMessageResultTextBox.Clear();
            string searchString = searchMessageTextBox.Text;
            string replaceString = replaceMessageTextBox.Text;
            textSearchProgressBar.Maximum = romInfo.GetTextArchivesCount();
            int msgCount = romInfo.GetTextArchivesCount();
            if (msgCount > 828) msgCount = 828;
            for (int k = 0; k < msgCount; k++) {
                TextArchive file = LoadMessageArchive(k);
                currentMessageFile = file;
                bool found = false;

                for (int j = 0; j < file.messages.Count; j++) {
                    if (file.messages[j].Contains(searchString)) {
                        file.messages[j] = file.messages[j].Replace(searchString, replaceString);
                        found = true;
                    }

                }
                textSearchProgressBar.Value = k;
                if (found) {
                    disableHandlers = true;
                    textEditorDataGridView.Rows.Clear();
                    searchMessageResultTextBox.AppendText(searchString + " found and replaced by " + replaceString + Environment.NewLine);
                    for (int i = 0; i < currentMessageFile.messages.Count; i++) {
                        textEditorDataGridView.Rows.Add(currentMessageFile.messages[i]);
                        textEditorDataGridView.Rows[i].HeaderCell.Value = "0x" + i.ToString("X");
                    }
                    disableHandlers = false;
                    BinaryWriter textWriter = new BinaryWriter(new FileStream(romInfo.GetTextArchivesPath() + "\\" + k.ToString("D4"), FileMode.Create));
                    textWriter.Write((UInt16)currentMessageFile.messages.Count);
                    textWriter.Write((UInt16)currentMessageFile.initialKey);
                    int key = (currentMessageFile.initialKey * 0x2FD) & 0xFFFF;
                    int key2 = 0;
                    int realKey = 0;
                    int offset = 0x4 + (currentMessageFile.messages.Count * 8);
                    int[] stringSize = new int[currentMessageFile.messages.Count];

                    for (int i = 0; i < currentMessageFile.messages.Count; i++) // Reads and stores string offsets and sizes
                    {
                        key2 = (key * (i + 1) & 0xFFFF);
                        realKey = key2 | (key2 << 16);
                        textWriter.Write(offset ^ realKey);
                        int length = currentMessageFile.GetStringLength(textEditorDataGridView[0, i].Value.ToString());
                        stringSize[i] = length;
                        textWriter.Write(length ^ realKey);
                        offset += length * 2;
                    }
                    for (int i = 0; i < currentMessageFile.messages.Count; i++) // Encodes strings and writes them to file
                    {
                        key = (0x91BD3 * (i + 1)) & 0xFFFF;
                        int[] currentString = currentMessageFile.EncodeString(textEditorDataGridView[0, i].Value.ToString(), i, stringSize[i]);
                        for (int j = 0; j < stringSize[i] - 1; j++) {
                            textWriter.Write((UInt16)(currentString[j] ^ key));
                            key += 0x493D;
                            key &= 0xFFFF;
                        }
                        textWriter.Write((UInt16)(0xFFFF ^ key));
                    }
                    textWriter.Close();
                }
                //else searchMessageResultTextBox.AppendText(searchString + " not found in this file");
                //this.saveMessageFileButton_Click(sender, e);
            }
            textSearchProgressBar.Value = 0;
        }
        private void selectTextFileComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            disableHandlers = true;

            textEditorDataGridView.Rows.Clear();
            currentMessageFile = LoadMessageArchive(selectTextFileComboBox.SelectedIndex);

            for (int i = 0; i < currentMessageFile.messages.Count; i++) {
                textEditorDataGridView.Rows.Add(currentMessageFile.messages[i]);
            }


            if (hexRadiobutton.Checked) {
                printTextEditorLinesHex();
            } else {
                printTextEditorLinesDecimal();
            }

            disableHandlers = false;

        }
        private void printTextEditorLinesHex() {
            disableHandlers = true;
            for (int i = 0; i < currentMessageFile.messages.Count; i++) {
                textEditorDataGridView.Rows[i].HeaderCell.Value = "0x" + i.ToString("X");
            }
        }

        private void printTextEditorLinesDecimal() {
            for (int i = 0; i < currentMessageFile.messages.Count; i++) {
                textEditorDataGridView.Rows[i].HeaderCell.Value = i.ToString();
            }
        }

        private void textEditorDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (disableHandlers)
                return;
            if (e.RowIndex > -1)
                currentMessageFile.messages[e.RowIndex] = textEditorDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }

        #endregion

        #region Tileset Editor

        #region Variables
        public NSMBe4.NSBMD.NSBTX_File currentTileset;
        public AreaData currentAreaData;
        #endregion

        #region Subroutines
        public void FillTilesetBox() {
            texturePacksListBox.Items.Clear();

            int tilesetFileCount;
            if (mapTilesetRadioButton.Checked)
                tilesetFileCount = romInfo.GetMapTexturesCount();
            else
                tilesetFileCount = romInfo.GetBuildingTexturesCount();

            for (int i = 0; i < tilesetFileCount; i++)
                texturePacksListBox.Items.Add("Texture Pack " + i);
        }
        #endregion

        private void buildingsTilesetRadioButton_CheckedChanged(object sender, EventArgs e) {
            FillTilesetBox();
            texturePacksListBox.SelectedIndex = (int)areaDataBuildingTilesetUpDown.Value;
            if (texturesListBox.Items.Count > 0)
                texturesListBox.SelectedIndex = 0;
            if (palettesListBox.Items.Count > 0)
                palettesListBox.SelectedIndex = 0;
        }
        private void exportNSBTXButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "NSBTX File (*.nsbtx)|*.nsbtx";
            if (sf.ShowDialog(this) != DialogResult.OK)
                return;

            string tilesetPath;
            if (mapTilesetRadioButton.Checked) tilesetPath = romInfo.GetMapTexturesDirPath() + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            else tilesetPath = romInfo.GetBuildingTexturesDirPath() + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            File.Copy(tilesetPath, sf.FileName);
        }
        private void importNSBTXButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .nsbtx file */
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "NSBTX File (*.nsbtx)|*.nsbtx";
            if (of.ShowDialog(this) != DialogResult.OK) return;

            /* Update nsbtx file */
            string tilesetPath;
            if (mapTilesetRadioButton.Checked) tilesetPath = romInfo.GetMapTexturesDirPath() + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            else tilesetPath = romInfo.GetBuildingTexturesDirPath() + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, tilesetPath, true);

            /* Update nsbtx object in memory and controls */
            currentTileset = new NSMBe4.NSBMD.NSBTX_File(new FileStream(of.FileName, FileMode.Open));

        }
        private void mapTilesetRadioButton_CheckedChanged(object sender, EventArgs e) {
            FillTilesetBox();
            texturePacksListBox.SelectedIndex = (int)areaDataMapTilesetUpDown.Value;
            if (texturesListBox.Items.Count > 0)
                texturesListBox.SelectedIndex = 0;
            if (palettesListBox.Items.Count > 0)
                palettesListBox.SelectedIndex = 0;
        }
        private void palettesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;
            try {
                texturePictureBox.Image = LoadTextureFromNSBTX(currentTileset, texturesListBox.SelectedIndex, palettesListBox.SelectedIndex);
            } catch { }
        }
        private void texturePacksListBox_SelectedIndexChanged(object sender, EventArgs e) {
            /* Clear ListBoxes */
            texturesListBox.Items.Clear();
            palettesListBox.Items.Clear();

            /* Load tileset file */
            string tilesetPath;
            if (mapTilesetRadioButton.Checked)
                tilesetPath = romInfo.GetMapTexturesDirPath() + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            else
                tilesetPath = romInfo.GetBuildingTexturesDirPath() + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            currentTileset = new NSMBe4.NSBMD.NSBTX_File(new FileStream(tilesetPath, FileMode.Open));

            /* Add textures and palette slot names to ListBoxes */
            texturesListBox.Items.AddRange(currentTileset.TexInfo.names.ToArray());
            palettesListBox.Items.AddRange(currentTileset.PalInfo.names.ToArray());

            if (texturesListBox.Items.Count > 0)
                texturesListBox.SelectedIndex = 0;
        }
        private void texturesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            String texSelected = texturesListBox.SelectedItem.ToString();
            String result = findAndSelectMatchingPalette(texSelected);
            if (result != null) {
                palettesListBox.SelectedItem = result;
                statusLabel.Text = "Ready";
            }

            try {
                texturePictureBox.Image = LoadTextureFromNSBTX(currentTileset, texturesListBox.SelectedIndex, palettesListBox.SelectedIndex);
            } catch { }
        }

        private String findAndSelectMatchingPalette(String findThis) {
            statusLabel.Text = "Searching palette...";

            String copy = findThis;
            while (copy.Length > 0) {
                if (palettesListBox.Items.Contains(copy + "_pl")) {
                    return copy + "_pl";
                }
                if (palettesListBox.Items.Contains(copy)) {
                    return copy;
                }
                copy = copy.Substring(0, copy.Length - 1);
            }

            foreach (String palette in palettesListBox.Items) {
                if (palette.StartsWith(findThis)) {
                    return palette;
                }
            }

            statusLabel.Text = "Couldn't find a palette to match " + '"' + findThis + '"';
            return null;
        }

        private void areaDataBuildingTilesetUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) return;
            currentAreaData.buildingsTileset = (ushort)areaDataBuildingTilesetUpDown.Value;
        }
        private void areaDataDynamicTexturesComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;
            if (areaDataDynamicTexturesComboBox.SelectedIndex == 0x2)
                currentAreaData.dynamicTextureType = 0xFFFF;
            else
                currentAreaData.dynamicTextureType = (ushort)areaDataDynamicTexturesComboBox.SelectedIndex;

        }
        private void areaDataLightTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;
            currentAreaData.lightType = (byte)areaDataLightTypeComboBox.SelectedIndex;
        }
        private void areaDataMapTilesetUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;
            currentAreaData.mapTileset = (ushort)areaDataMapTilesetUpDown.Value;
        }
        private void areaDataAreaTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;
            currentAreaData.areaType = (byte)areaDataAreaTypeComboBox.SelectedIndex;
        }
        private void saveAreaDataButton_Click(object sender, EventArgs e) {
            string areaDataPath = romInfo.GetAreaDataDirPath() + "\\" + selectAreaDataComboBox.SelectedIndex.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(areaDataPath, FileMode.Create)))
                writer.Write(currentAreaData.SaveAreaData(romInfo.GetGameVersion()));
        }
        private void selectAreaDataComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentAreaData = LoadAreaData((uint)selectAreaDataComboBox.SelectedIndex);

            areaDataBuildingTilesetUpDown.Value = currentAreaData.buildingsTileset;
            areaDataMapTilesetUpDown.Value = currentAreaData.mapTileset;
            areaDataLightTypeComboBox.SelectedIndex = currentAreaData.lightType;
            switch (romInfo.GetGameVersion()) {
                case "Diamond":
                case "Pearl":
                case "Platinum":
                    break;
                default:
                    if (currentAreaData.dynamicTextureType == 0xFFFF) {
                        areaDataDynamicTexturesComboBox.SelectedIndex = 0x2;
                    } else {
                        areaDataDynamicTexturesComboBox.SelectedIndex = currentAreaData.dynamicTextureType;
                    }
                    areaDataAreaTypeComboBox.SelectedIndex = currentAreaData.areaType;
                    break;
            }
        }

        private void hexRadiobutton_CheckedChanged(object sender, EventArgs e) {
            disableHandlers = true;
            if (hexRadiobutton.Checked) {
                printTextEditorLinesHex();
            } else {
                printTextEditorLinesDecimal();
            }
            disableHandlers = false;
        }


        #endregion
    }
}