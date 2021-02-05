using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;

using NarcAPI;
using Tao.OpenGl;
using LibNDSFormats.NSBMD;
using LibNDSFormats.NSBTX;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using Matrix = DSPRE.ROMFiles.Matrix;

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

        /* Editors Setup */
        public bool matrixEditorIsReady { get; private set; } = false;
        public bool mapEditorIsReady { get; private set; } = false;
        public bool eventEditorIsReady { get; private set; } = false;
        public bool scriptEditorIsReady { get; private set; } = false;
        public bool textEditorIsReady { get; private set; } = false;
        public bool tilesetEditorIsReady { get; private set; } = false;

        /* ROM Information */
        public static string gameCode;
        public static byte europeByte;
        RomInfo romInfo;

        #endregion

        #region Subroutines
        private string[] GetBuildingsList(bool interior) {
            List<string> names = new List<string>();
            string path = romInfo.GetBuildingModelsDirPath(interior);
            int buildModelsCount = Directory.GetFiles(path).Length;

            for (int i = 0; i < buildModelsCount; i++) {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(path + "\\" + i.ToString("D4")))) {
                    reader.BaseStream.Position = 0x38;
                    string nsbmdName = Encoding.UTF8.GetString(reader.ReadBytes(16)).TrimEnd();
                    names.Add(nsbmdName);
                }
            }
            return names.ToArray();
        }

        private string[] GetTrainerNames() {
            List<string> trainerList = new List<string>();

            /* Store all trainer names and classes */
            TextArchive trainerClasses = new TextArchive(romInfo.GetTrainerClassMessageNumber());
            TextArchive trainerNames = new TextArchive(romInfo.GetTrainerNamesMessageNumber());
            BinaryReader trainerReader;
            int trainerCount = Directory.GetFiles(romInfo.trainerDataDirPath).Length;

            for (int i = 0; i < trainerCount; i++) {
                trainerReader = new BinaryReader(new FileStream(romInfo.trainerDataDirPath + "\\" + i.ToString("D4"), FileMode.Open));
                trainerReader.BaseStream.Position += 0x1;
                trainerList.Add("[" + i.ToString("D2") + "] " + trainerClasses.messages[trainerReader.ReadUInt16()] + " " + trainerNames.messages[i]);
            }
            return trainerList.ToArray();
        }

        private string[] GetItemNames() {
            return new TextArchive((RomInfo.itemNamesTextNumber)).messages.ToArray();
        }

        private string[] GetItemNames(int startIndex, int count) {
            return new TextArchive(RomInfo.itemNamesTextNumber).messages.GetRange(startIndex, count).ToArray();
        }

        private string[] GetPokémonNames() {
            return new TextArchive(RomInfo.pokémonNamesTextNumbers[0]).messages.ToArray();
        }

        private string[] GetAttackNames() {
            return new TextArchive(RomInfo.attackNamesTextNumber).messages.ToArray();
        }

        private AreaData LoadAreaData(uint areaDataID) {
            return new AreaData(new FileStream(romInfo.areaDataDirPath + "//" + areaDataID.ToString("D4"), FileMode.Open), RomInfo.gameVersion);
        }

        private MapFile LoadMapFile(int mapNumber) {
            if (mapNumber < 0) {
                MessageBox.Show("Negative map number received " + '(' + mapNumber + ')', "Received negative integer!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            String mapFilePath = RomInfo.mapDirPath + "\\" + mapNumber.ToString("D4");
            try {
                return new MapFile(new FileStream(mapFilePath, FileMode.Open), RomInfo.gameVersion);
            } catch (FileNotFoundException) {
                MessageBox.Show("File " + '"' + mapFilePath + " is missing.", "File not found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }
        private void PaintGameIcon(object sender, PaintEventArgs e) {
            if (iconON) {
                BinaryReader readIcon;
                try {
                    readIcon = new BinaryReader(File.OpenRead(romInfo.workDir + @"banner.bin"));
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
        private void DeleteTempFolders() {
            foreach (var tuple in RomInfo.narcPaths.Zip(RomInfo.extractedNarcDirs, Tuple.Create)) {
                Directory.Delete(tuple.Item2, true); // Delete folder
            }
        }
        private void RepackRom(string ndsFileName) {
            Process repack = new Process();
            repack.StartInfo.FileName = @"Tools\ndstool.exe";
            repack.StartInfo.Arguments = "-c " + '"' + ndsFileName + '"'
                + " -9 " + '"' + RomInfo.arm9Path + '"'
                + " -7 " + '"' + romInfo.workDir + "arm7.bin" + '"'
                + " -y9 " + '"' + romInfo.workDir + "y9.bin" + '"'
                + " -y7 " + '"' + romInfo.workDir + "y7.bin" + '"'
                + " -d " + '"' + romInfo.workDir + "data" + '"'
                + " -y " + '"' + romInfo.workDir + "overlay" + '"'
                + " -t " + '"' + romInfo.workDir + "banner.bin" + '"'
                + " -h " + '"' + romInfo.workDir + "header.bin" + '"';

            Application.DoEvents();
            repack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            repack.StartInfo.CreateNoWindow = true;
            repack.Start();
            repack.WaitForExit();
        }
        private void SetupEventEditor() {
            /* Extract essential NARCs sub-archives*/

            statusLabel.Text = "Attempting to unpack Event Editor NARCs... Please wait. This might take a while";
            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 12;
            toolStripProgressBar.Value = 0;
            Update();

            DSUtils.UnpackNarcs(new List<int> { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, toolStripProgressBar);
            if (RomInfo.gameVersion == "HG" || RomInfo.gameVersion == "SS") {
                DSUtils.UnpackNarc(RomInfo.narcPaths.Length - 1);
            }


            disableHandlers = true;
            if (File.Exists(romInfo.OWtablePath)) {
                switch (RomInfo.gameVersion) {
                    case "D":
                    case "P":
                    case "Plat":
                        break;
                    default:
                        // HGSS Overlay 1 must be decompressed in order to read the overworld table
                        if (DSUtils.DecompressOverlay(1, true) == -1) {
                            MessageBox.Show("Overlay 1 couldn't be decompressed.\nOverworld sprites in the Event Editor will be " +
                                "displayed incorrectly or not displayed at all.", "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                }
            }

            /* Add event file numbers to box */
            int eventCount = Directory.GetFiles(RomInfo.eventsDirPath).Length;
            int owSpriteCount = Directory.GetFiles(romInfo.OWSpriteDirPath).Length;
            string[] trainerNames = GetTrainerNames();

            statusLabel.Text = "Loading Events... Please wait";
            toolStripProgressBar.Maximum = eventCount + owSpriteCount + trainerNames.Length;
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
            int itemScriptId = RomInfo.itemScriptFileNumber;
            try {
                int count = new ScriptFile(itemScriptId).scripts.Count - 1;
                owItemComboBox.Items.AddRange(GetItemNames(0, count));
            } catch {
                MessageBox.Show("There was a problem reading Script File #" + itemScriptId + ".\n" +
                    "It is strongly adviced that you restore this file from a clean backup.", "Item script couldn't be loaded",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            /* Add ow movement list to box */
            owMovementComboBox.Items.AddRange(PokeDatabase.EventEditor.Overworlds.movementsArray);
            spawnableDirComboBox.Items.AddRange(PokeDatabase.EventEditor.Spawnables.orientationsArray);
            spawnableTypeComboBox.Items.AddRange(PokeDatabase.EventEditor.Spawnables.typesArray);

            /* Create dictionary for 3D overworlds */
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    break;
                case "Plat":
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

            if (ScanScriptsCheckStandardizedItemNumbers())
                isItemRadioButton.Enabled = true;

            disableHandlers = false;

            /* Draw matrix 0 in matrix navigator */
            eventMatrix = new Matrix(0);
            selectEventComboBox.SelectedIndex = 0;
            owItemComboBox.SelectedIndex = 0;
            owTrainerComboBox.SelectedIndex = 0;

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;
        }
        private void SetupHeaderEditor() {
            /* Extract essential NARCs sub-archives*/

            statusLabel.Text = "Attempting to unpack Header Editor NARCs... Please wait.";
            Update();

            DSUtils.UnpackNarcs(new List<int> { 0, 1 }, toolStripProgressBar);

            statusLabel.Text = "Reading internal names... Please wait.";
            Update();

            /* Read Header internal names */
            internalNames = new List<string>();
            try {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(romInfo.internalNamesLocation))) {
                    int headerCount = romInfo.GetHeaderCount();

                    for (int i = 0; i < headerCount; i++) {
                        byte[] row = reader.ReadBytes(RomInfo.internalNameLength);

                        string internalName = Encoding.ASCII.GetString(row);//.TrimEnd();
                        headerListBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + internalName);
                        internalNames.Add(internalName.TrimEnd('\0'));
                    }
                }
            } catch (FileNotFoundException) {
                MessageBox.Show(romInfo.internalNamesLocation + " doesn't exist.", "Couldn't read internal names", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            /*Add list of options to each control */
            locationNameComboBox.Items.AddRange(new TextArchive(romInfo.GetLocationNamesTextNumber()).messages.ToArray());
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    areaIconComboBox.Enabled = false;
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject("dpareaicon");
                    areaSettingsLabel.Text = "Show nametag:";
                    cameraComboBox.Items.AddRange(PokeDatabase.CameraAngles.DPPtCameraValues);
                    musicDayComboBox.Items.AddRange(PokeDatabase.MusicDB.DPMusicDict.Values.ToArray());
                    musicNightComboBox.Items.AddRange(PokeDatabase.MusicDB.DPMusicDict.Values.ToArray());
                    areaSettingsComboBox.Items.AddRange(PokeDatabase.ShowName.DPShowNameValues);
                    weatherComboBox.Items.AddRange(PokeDatabase.Weather.DPWeatherDict.Values.ToArray());
                    wildPokeUpDown.Maximum = 65535;
                    break;
                case "Plat":
                    areaIconComboBox.Items.AddRange(PokeDatabase.Area.PtAreaIconValues);
                    areaSettingsLabel.Text = "Show nametag:";
                    cameraComboBox.Items.AddRange(PokeDatabase.CameraAngles.DPPtCameraValues);
                    musicDayComboBox.Items.AddRange(PokeDatabase.MusicDB.PtMusicDict.Values.ToArray());
                    musicNightComboBox.Items.AddRange(PokeDatabase.MusicDB.PtMusicDict.Values.ToArray());
                    areaSettingsComboBox.Items.AddRange(PokeDatabase.ShowName.PtShowNameValues);
                    weatherComboBox.Items.AddRange(PokeDatabase.Weather.PtWeatherDict.Values.ToArray());
                    wildPokeUpDown.Maximum = 65535;
                    break;
                default:
                    areaIconComboBox.Items.AddRange(PokeDatabase.Area.HGSSAreaIconValues);
                    cameraComboBox.Items.AddRange(PokeDatabase.CameraAngles.HGSSCameraValues);
                    areaSettingsComboBox.Items.AddRange(PokeDatabase.Area.HGSSAreaProperties);
                    areaSettingsLabel.Text = "Area Settings:";
                    musicDayComboBox.Items.AddRange(PokeDatabase.MusicDB.HGSSMusicDict.Values.ToArray());
                    musicNightComboBox.Items.AddRange(PokeDatabase.MusicDB.HGSSMusicDict.Values.ToArray());
                    weatherComboBox.Items.AddRange(PokeDatabase.Weather.HGSSWeatherDict.Values.ToArray());
                    wildPokeUpDown.Maximum = 255;

                    flag7CheckBox.Visible = true;
                    flag6CheckBox.Visible = true;
                    flag5CheckBox.Visible = true;
                    flag4CheckBox.Visible = true;
                    flag7CheckBox.Text = "Flag 7";
                    flag6CheckBox.Text = "Flag 6";
                    flag5CheckBox.Text = "Flag 5";
                    flag4CheckBox.Text = "Fly";

                    flag3CheckBox.Text = "Esc. Rope";
                    flag2CheckBox.Text = "Flag 2";
                    flag1CheckBox.Text = "Bicycle";
                    flag0CheckBox.Text = "Flag 0";

                    worldmapCoordsGroupBox.Enabled = true;
                    battleBackgroundUpDown.Visible = false;
                    battleBackgroundLabel.Visible = false;
                    break;
            }
            if (headerListBox.Items.Count > 0)
                headerListBox.SelectedIndex = 0;
        }
        private void battleBackgroundUpDown_ValueChanged(object sender, EventArgs e) {
            currentHeader.battleBackground = (byte)battleBackgroundUpDown.Value;
        }
        private void SetupMapEditor() {
            /* Extract essential NARCs sub-archives*/
            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 14;
            toolStripProgressBar.Value = 0;
            statusLabel.Text = "Attempting to unpack Map Editor NARCs... Please wait.";
            Update();

            DSUtils.UnpackNarcs(new List<int> { 3, 4, 5, 6, 7, 8 }, toolStripProgressBar);
            if (RomInfo.gameVersion == "HG" || RomInfo.gameVersion == "SS") {
                DSUtils.UnpackNarc(RomInfo.narcPaths.Length - 1 );
            }

            disableHandlers = true;

            mapOpenGlControl.MakeCurrent();
            mapOpenGlControl.MouseWheel += new MouseEventHandler(mapOpenGlControl_MouseWheel);
            collisionPainterPictureBox.Image = new Bitmap(100, 100);
            typePainterPictureBox.Image = new Bitmap(100, 100);
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    mapPartsTabControl.TabPages.Remove(bgsTabPage);
                    break;
                default:
                    interiorbldRadioButton.Enabled = true;
                    exteriorbldRadioButton.Enabled = true;
                    break;
            };

            /* Add map names to box */
            for (int i = 0; i < romInfo.GetMapCount(); i++) {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(RomInfo.mapDirPath + "\\" + i.ToString("D4")))) {
                    switch (RomInfo.gameVersion) {
                        case "D":
                        case "P":
                        case "Plat":
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
                    selectMapComboBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + nsbmdName);
                }

            }
            toolStripProgressBar.Value++;

            /* Fill building models list */
            updateBuildingListComboBox(false);

            /*  Fill map textures list */
            mapTextureComboBox.Items.Add("Untextured");
            for (int i = 0; i < romInfo.GetMapTexturesCount(); i++)
                mapTextureComboBox.Items.Add("Map Texture Pack [" + i.ToString("D2") + "]");
            toolStripProgressBar.Value++;

            /*  Fill building textures list */
            buildTextureComboBox.Items.Add("Untextured");
            for (int i = 0; i < romInfo.GetBuildingTexturesCount(); i++)
                buildTextureComboBox.Items.Add("Building Texture Pack [" + i.ToString("D2") + "]");
            toolStripProgressBar.Value++;

            foreach (string s in PokeDatabase.System.MapCollisionPainters) {
                collisionPainterComboBox.Items.Add(s);
            }

            foreach(string s in PokeDatabase.System.MapCollisionTypePainters) {
                collisionTypePainterComboBox.Items.Add(s);
            }
            
            toolStripProgressBar.Value++;

            /* Set controls' initial values */
            selectCollisionPanel.BackColor = Color.MidnightBlue;
            collisionTypePainterComboBox.SelectedIndex = 0;
            collisionPainterComboBox.SelectedIndex = 1;

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;
            disableHandlers = false;

            //Default selections
            selectMapComboBox.SelectedIndex = 0;
            exteriorbldRadioButton.Checked = true;
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    mapTextureComboBox.SelectedIndex = 7;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
                case "HG":
                case "SS":
                    mapTextureComboBox.SelectedIndex = 3;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
                default:
                    mapTextureComboBox.SelectedIndex = 2;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
            };
        }
        private void updateBuildingListComboBox(bool interior) {
            string[] bldList = GetBuildingsList(interior);
            for (int i = 0; i < bldList.Length; i++) {
                buildIndexComboBox.Items.Add("[" + i + "] " + bldList[i]);
            }
            toolStripProgressBar.Value++;
        }
        private void SetupMatrixEditor() {
            statusLabel.Text = "Setting up Matrix Editor...";
            Update();

            DSUtils.UnpackNarc( 2 ); // 2 = matrixDir

            disableHandlers = true;

            /* Add matrix entries to ComboBox */
            selectMatrixComboBox.Items.Add("Matrix 0 - Main");
            for (int i = 1; i < romInfo.GetMatrixCount(); i++)
                selectMatrixComboBox.Items.Add("Matrix " + i);

            disableHandlers = false;
            selectMatrixComboBox.SelectedIndex = 0;
        }
        public void SetupScriptEditor() {
            /* Extract essential NARCs sub-archives*/
            statusLabel.Text = "Setting up Script Editor...";
            Update();

            DSUtils.UnpackNarc(12); //12 = scripts Narc Dir

            int scriptCount = Directory.GetFiles(RomInfo.scriptDirPath).Length;
            for (int i = 0; i < scriptCount; i++)
                selectScriptFileComboBox.Items.Add("Script File " + i);

            String exclMSG = "The script editor has been recently \"fixed\".\n" +
                "Always keep an eye out for unexpected behavior.\n";
            MessageBox.Show(exclMSG, "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            selectScriptFileComboBox.SelectedIndex = 0;
            currentScriptBox = scriptTextBox;
            currentLineNumbersBox = LineNumberTextBoxScript;
        }
        private void SetupTextEditor() {
            string[] narcPaths = RomInfo.narcPaths;
            string[] extractedNarcDirs = RomInfo.extractedNarcDirs;

            DSUtils.UnpackNarc( 1 );

            statusLabel.Text = "Setting up Text Editor...";
            Update();

            for (int i = 0; i < romInfo.GetTextArchivesCount(); i++)
                selectTextFileComboBox.Items.Add("Text Archive " + i);

            selectTextFileComboBox.SelectedIndex = 0;
        }
        private void SetupTilesetEditor() {
            statusLabel.Text = "Attempting to unpack Tileset Editor NARCs... Please wait.";
            Update();

            DSUtils.UnpackNarcs(new List<int> { 6, 7, 8 }, toolStripProgressBar);

            /* Fill Tileset ListBox */
            FillTilesetBox();

            /* Fill AreaData ComboBox */
            int areaDataCount = romInfo.GetAreaDataCount();
            for (int i = 0; i < areaDataCount; i++)
                selectAreaDataListBox.Items.Add("AreaData File " + i);

            /* Enable gameVersion-specific controls */

            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    break;
                default:
                    areaDataDynamicTexturesNumericUpDown.Enabled = true;
                    areaTypeGroupbox.Enabled = true;
                    break;
            };

            if (selectAreaDataListBox.Items.Count > 0)
                selectAreaDataListBox.SelectedIndex = 0;
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
            if (Directory.Exists(romInfo.workDir)) {
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
            statusLabel.Text = "Unpacking ROM contents to " + romInfo.workDir + " ...";
            Update();

            Directory.CreateDirectory(romInfo.workDir);
            Process unpack = new Process();
            unpack.StartInfo.FileName = @"Tools\ndstool.exe";
            unpack.StartInfo.Arguments = "-x " + '"' + ndsFileName + '"'
                + " -9 " + '"' + RomInfo.arm9Path + '"'
                + " -7 " + '"' + romInfo.workDir + "arm7.bin" + '"'
                + " -y9 " + '"' + romInfo.workDir + "y9.bin" + '"'
                + " -y7 " + '"' + romInfo.workDir + "y7.bin" + '"'
                + " -d " + '"' + romInfo.workDir + "data" + '"'
                + " -y " + '"' + romInfo.workDir + "overlay" + '"'
                + " -t " + '"' + romInfo.workDir + "banner.bin" + '"'
                + " -h " + '"' + romInfo.workDir + "header.bin" + '"';
            Application.DoEvents();
            unpack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            unpack.StartInfo.CreateNoWindow = true;
            unpack.Start();
            unpack.WaitForExit();
        }
        #endregion
        private void romToolBoxToolStripMenuItem_Click(object sender, EventArgs e) {
            using (ROMToolboxDialog window = new ROMToolboxDialog(romInfo)) {
                window.ShowDialog();
                if (ROMToolboxDialog.flag_standardizedItems)
                    isItemRadioButton.Enabled = true;
            }
        }
        private void scriptCommandsDatabaseToolStripButton_Click(object sender, EventArgs e) {
            ScriptCommands form = new ScriptCommands();
            form.Show();
        }
        private void headerSearchToolStripButton_Click(object sender, EventArgs e) {
            mainTabControl.SelectedIndex = 0; //Select Header Editor
            using (HeaderSearch h = new HeaderSearch(ref internalNames, headerListBox, statusLabel)) {
                h.ShowDialog();
            }
        }
        private void advancedHeaderSearchToolStripMenuItem_Click(object sender, EventArgs e) {
            headerSearchToolStripButton_Click(null, null);
        }
        private void buildingEditorButton_Click(object sender, EventArgs e) {
            unpackBuildingEditorNARCs();

            using (BuildingEditor editor = new BuildingEditor(romInfo))
                editor.ShowDialog();
        }
        private void unpackBuildingEditorNARCs() {
            toolStripProgressBar.Visible = true;

            statusLabel.Text = "Attempting to unpack Building Editor NARCs... Please wait. This might take a while";
            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 4;
            toolStripProgressBar.Value = 0;
            Update();

            DSUtils.UnpackNarcs(new List<int> { 4, 5, 6 }, toolStripProgressBar);
            if (RomInfo.gameVersion == "HG" || RomInfo.gameVersion == "SS")
                DSUtils.UnpackNarc(RomInfo.narcPaths.Length - 1 );// Last = interior buildings dir

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;
            statusLabel.Text = "Ready";
            Update();
        }
        private void ForceUnpackBuildingEditorNARCs() {
            toolStripProgressBar.Visible = true;

            statusLabel.Text = "Attempting to unpack Building Editor NARCs... Please wait. This might take a while";
            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 4;
            toolStripProgressBar.Value = 0;
            Update();

            DSUtils.ForceUnpackNarcs(new List<int> { 4, 5, 6 }, toolStripProgressBar);
            if (RomInfo.gameVersion == "HG" || RomInfo.gameVersion == "SS")
                DSUtils.ForceUnpackNarcs(new List<int> { RomInfo.narcPaths.Length - 1 }, toolStripProgressBar);// Last = interior buildings dir

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;
            statusLabel.Text = "Ready";
            Update();
        }
        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e) {
            string message = "DS Pokémon Rom Editor by Nømura and AdAstra/LD3005" + Environment.NewLine + "version 1.1.2" + Environment.NewLine
                + Environment.NewLine + "This tool was largely inspired by Markitus95's Spiky's DS Map Editor, from which certain assets were also recycled. Credits go to Markitus, Ark, Zark, Florian, and everyone else who deserves credit for SDSME." + Environment.NewLine
                + Environment.NewLine + "Special thanks go to Trifindo, Mikelan98, JackHack96, Mixone and BagBoy."
                + Environment.NewLine + "Their help, research and expertise in many fields of NDS Rom Hacking made the development of this tool possible.";

            MessageBox.Show(message, "About...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void loadRom_Click(object sender, EventArgs e) {
            OpenFileDialog openRom = new OpenFileDialog(); // Select ROM
            openRom.Filter = "NDS File (*.nds)|*.nds";
            if (openRom.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryReader br = new BinaryReader(File.OpenRead(openRom.FileName))) {
                br.BaseStream.Position = 0xC; // get ROM ID
                gameCode = Encoding.UTF8.GetString(br.ReadBytes(4));
                br.BaseStream.Position = 0x1E;
                europeByte = br.ReadByte();
            }
            string workDir = Path.GetDirectoryName(openRom.FileName) + "\\" + Path.GetFileNameWithoutExtension(openRom.FileName) + "_DSPRE_contents" + "\\";

            /* Set ROM gameVersion and language */
            romInfo = new RomInfo(gameCode, workDir);
            DSUtils.SetWorkDir(workDir);

            if (RomInfo.gameVersion == null) {
                statusLabel.Text = "Unsupported ROM";
                Update();
                return;
            }

            versionLabel.Text = "Pokémon " + romInfo.gameName + " [" + RomInfo.romID + "]";
            languageLabel.Text = "Language: " + RomInfo.gameLanguage;

            if (RomInfo.gameLanguage == "ENG")
                if (europeByte == 0x0A)
                    languageLabel.Text += " [Europe]";
                else
                    languageLabel.Text += " [America]";

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
                            Directory.Delete(romInfo.workDir, true);
                        } catch (IOException) {
                            MessageBox.Show("Concurrent access detected: \n" + romInfo.workDir +
                                "\nMake sure no other process is using the extracted ROM folder while DSPRE is running.", "Concurrent Access", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        Update();
                    }

                    try {
                        UnpackRom(openRom.FileName);
                        DSUtils.editARM9size(-12);
                    } catch (IOException) {
                        MessageBox.Show("Can't access temp directory: \n" + romInfo.workDir + "\nThis might be a temporary issue.\nMake sure no other process is using it and try again.", "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        statusLabel.Text = "Error: concurrent access to " + romInfo.workDir;
                        Update();
                        return;
                    }
                    break;
            }

            iconON = true;
            gameIcon.Refresh();  // Paint game icon
            statusLabel.Text = "Attempting to unpack NARCs from folder...";
            Update();

            /*foreach (Tuple<string, string> tuple in RomInfo.narcPaths.Zip(RomInfo.extractedNarcDirs, Tuple.Create))
                Narc.Open(romInfo.workDir + tuple.Item1).ExtractToFolder(tuple.Item2);*/

            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    break;
                default:
                    if (!DSUtils.DecompressArm9()) {
                        MessageBox.Show("ARM9 decompression failed. The program can't proceed.\nAborting.",
                                    "Errror with ARM9 decompression", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
            }

            /* Setup essential editors */
            SetupHeaderEditor();
            eventOpenGlControl.InitializeContexts();
            mapOpenGlControl.InitializeContexts();

            mainTabControl.Show();
            saveRomButton.Enabled = true;

            unpackAllButton.Enabled = true;
            updateMapNarcsButton.Enabled = true;
            
            buildingEditorButton.Enabled = true;
            wildEditorButton.Enabled = true;
            
            romToolboxButton.Enabled = true;
            headerSearchToolStripButton.Enabled = true;
            scriptCommandsButton.Enabled = true;

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
            foreach (var tuple in RomInfo.narcPaths.Zip(RomInfo.extractedNarcDirs, Tuple.Create)) {
                DirectoryInfo di = new DirectoryInfo(tuple.Item2);
                if (di.Exists) {
                    Narc.FromFolder(tuple.Item2).Save(romInfo.workDir + tuple.Item1); // Make new NARC from folder
                }
            }

            if (RomInfo.gameVersion == "HG" || RomInfo.gameVersion == "SS") {
                if (DSUtils.CheckOverlayHasCompressionFlag(1)) {
                    if (ROMToolboxDialog.overlay1MustBeRestoredFromBackup) {
                        DSUtils.RestoreOverlayFromCompressedBackup(1, eventEditorIsReady);
                    } else {
                        DSUtils.CompressOverlay(1);
                    }
                }
            }

            statusLabel.Text = "Repacking ROM...";
            Update();
            //DeleteTempFolders();
            RepackRom(saveRom.FileName);

            if (RomInfo.gameVersion != "D" && RomInfo.gameVersion != "P" && RomInfo.gameVersion != "Plat")
                if (eventEditorIsReady)
                    DSUtils.DecompressOverlay(1, true);

            statusLabel.Text = "Ready";
        }
        private void unpackAllButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("Do you wish to unpack all extracted NARCS?\n" +
                "This operation might be long and can't be interrupted.\n\n" +
                "Any unsaved changes made to the ROM in this session will be lost." +
                "\nProceed?", "About to unpack all NARCS",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                toolStripProgressBar.Maximum = RomInfo.narcPaths.Length;
                toolStripProgressBar.Visible = true;
                toolStripProgressBar.Value = 0;
                statusLabel.Text = "Attempting to unpack all NARCs... Be patient. This might take a while...";
                Update();
                foreach (var tuple in RomInfo.narcPaths.Zip(RomInfo.extractedNarcDirs, Tuple.Create)) {
                    Narc.Open(romInfo.workDir + tuple.Item1).ExtractToFolder(tuple.Item2);
                    toolStripProgressBar.Value++;
                }

                MessageBox.Show("Operation completed.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusLabel.Text = "Ready";
                toolStripProgressBar.Value = 0;
                toolStripProgressBar.Visible = false;

                matrixEditorIsReady = false;
                mapEditorIsReady = false;
                eventEditorIsReady = false;
                scriptEditorIsReady = false;
                textEditorIsReady = false;
                tilesetEditorIsReady = false;

                Update();
            }
        }
        private void updateMapNarcsButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("Do you wish to unpack all NARC files necessary for the Building Editor ?\n" +
               "This operation might be long and can't be interrupted.\n\n" +
               "Any unsaved changes made to building models and textures in this session will be lost." +
               "\nProceed?", "About to unpack Building NARCs",
               MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                ForceUnpackBuildingEditorNARCs();

                MessageBox.Show("Operation completed.", "Success",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusLabel.Text = "Ready";
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
            } else if (mainTabControl.SelectedTab == nsbtxEditorTabPage) {
                if (!tilesetEditorIsReady) {
                    SetupTilesetEditor();
                    tilesetEditorIsReady = true;
                }
            }
            statusLabel.Text = "Ready";
        }
        private void wildEditorButton_Click(object sender, EventArgs e) {
            openWildEditor(false);
        }
        private void openWildEditor(bool loadCurrent) {
            statusLabel.Text = "Attempting to extract Wild Encounters NARC...";
            Update();

            string[] narcPaths = RomInfo.narcPaths;
            string[] extractedNarcDirs = RomInfo.extractedNarcDirs;
            Tuple<string, string> t;

            if (RomInfo.gameVersion == "HG" || RomInfo.gameVersion == "SS") {
                t = Tuple.Create(narcPaths[narcPaths.Length - 2], extractedNarcDirs[extractedNarcDirs.Length - 2]);
            } else {
                t = Tuple.Create(narcPaths[narcPaths.Length - 1], extractedNarcDirs[extractedNarcDirs.Length - 2]);
            }

            DirectoryInfo di = new DirectoryInfo(t.Item2);
            if (!di.Exists || di.GetFiles().Length == 0) {
                Narc.Open(romInfo.workDir + t.Item1).ExtractToFolder(t.Item2);
            }
            statusLabel.Text = "Passing control to Wild Pokémon Editor...";
            Update();

            int encToOpen;
            if (loadCurrent)
                encToOpen = (int)wildPokeUpDown.Value;
            else
                encToOpen = 0;
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    using (WildEditorDPPt editor = new WildEditorDPPt(romInfo.encounterDirPath, GetPokémonNames(), encToOpen))
                        editor.ShowDialog();
                    break;
                default:
                    using (WildEditorHGSS editor = new WildEditorHGSS(romInfo.encounterDirPath, GetPokémonNames(), encToOpen))
                        editor.ShowDialog();
                    break;
            }
            statusLabel.Text = "Ready";
        }
        private void openWildEditorWithIdButtonClick(object sender, EventArgs e) {
            openWildEditor(true);
        }
        #endregion

        #region Header Editor

        #region Variables
        public MapHeader currentHeader;
        public List<string> internalNames;
        #endregion
        private void areaDataUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;
            currentHeader.areaDataID = (byte)areaDataUpDown.Value;
        }
        private void internalNameBox_TextChanged(object sender, EventArgs e) {
            if (internalNameBox.Text.Length > 13) {
                internalNameLenLabel.ForeColor = Color.FromArgb(255, 0, 0);
            } else if (internalNameBox.Text.Length > 7) {
                internalNameLenLabel.ForeColor = Color.FromArgb(190, 190, 0);
            } else {
                internalNameLenLabel.ForeColor = Color.FromArgb(0, 180, 0);
            }

            internalNameLenLabel.Text = "[ " + (internalNameBox.Text.Length) + " ]";
        }
        private void areaIconComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            string imageName;
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    break;
                case "Plat":
                    ((HeaderPt)currentHeader).areaIcon = (byte)areaIconComboBox.SelectedIndex;
                    imageName = "areaicon0" + areaIconComboBox.SelectedIndex.ToString();
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                    break;
                default:
                    ((HeaderHGSS)currentHeader).areaIcon = Byte.Parse(areaIconComboBox.SelectedItem.ToString().Substring(1, 3));
                    imageName = PokeDatabase.System.AreaPics.hgssAreaPicDict[areaIconComboBox.SelectedIndex];
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                    break;
            }
        }
        private void cameraComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;

            string imageName;
            try {
                switch (RomInfo.gameVersion) {
                    case "D":
                    case "P":
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "dpcamera" + cameraComboBox.SelectedIndex.ToString();
                        break;
                    case "Plat":
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "ptcamera" + cameraComboBox.SelectedIndex.ToString();
                        break;
                    default:
                        currentHeader.cameraAngleID = Byte.Parse(cameraComboBox.SelectedItem.ToString().Substring(1, 2));
                        imageName = "hgsscamera" + currentHeader.cameraAngleID.ToString("D2");
                        break;
                }
                cameraPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
            } catch (NullReferenceException) {
                MessageBox.Show("The current header uses an unrecognized camera.\n", "Unknown camera settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void eventFileUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;
            currentHeader.eventFileID = (ushort)eventFileUpDown.Value;
        }
        private void headerFlagsCheckBoxes_CheckedChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            byte flagVal = 0;
            if (flag0CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 0);

            if (flag1CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 1);

            if (flag2CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 2);

            if (flag3CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 3);

            switch (RomInfo.gameVersion) {
                case "HG":
                case "SS":
                    if (flag4CheckBox.Checked)
                        flagVal += (byte)Math.Pow(2, 4);
                    if (flag5CheckBox.Checked)
                        flagVal += (byte)Math.Pow(2, 5);
                    if (flag6CheckBox.Checked)
                        flagVal += (byte)Math.Pow(2, 6);
                    if (flag7CheckBox.Checked)
                        flagVal += (byte)Math.Pow(2, 7);
                    break;
            }
            currentHeader.flags = flagVal;
        }
        private void headerListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || headerListBox.SelectedIndex < 0)
                return; 

            /* Obtain current header ID from listbox*/
            short headerNumber = Int16.Parse(headerListBox.SelectedItem.ToString().Substring(0, internalNames.Count.ToString().Length));
            currentHeader = MapHeader.LoadFromARM9(headerNumber);
            refreshHeaderEditorFields();
        }

        private void refreshHeaderEditorFields() {
            /* Setup controls for common fields across headers */
            internalNameBox.Text = internalNames[currentHeader.ID];
            matrixUpDown.Value = currentHeader.matrixID;
            areaDataUpDown.Value = currentHeader.areaDataID;
            scriptFileUpDown.Value = currentHeader.scriptFileID;
            levelScriptUpDown.Value = currentHeader.levelScriptID;
            eventFileUpDown.Value = currentHeader.eventFileID;
            textFileUpDown.Value = currentHeader.textArchiveID;
            wildPokeUpDown.Value = currentHeader.wildPokémon;
            weatherUpDown.Value = currentHeader.weatherID;

            cameraComboBox.SelectedIndex = cameraComboBox.FindString("[" + currentHeader.cameraAngleID.ToString("D2"));

            if (RomInfo.gameVersion == "HG" || RomInfo.gameVersion == "SS")
                areaSettingsComboBox.SelectedIndex = cameraComboBox.FindString("[" + ((HeaderHGSS)currentHeader).areaSettings.ToString("D2"));

            if (currentHeader.wildPokémon == RomInfo.nullEncounterID)
                openWildEditorWithIdButton.Enabled = false;
            else
                openWildEditorWithIdButton.Enabled = true;

            /* Setup controls for fields with version-specific differences */
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    locationNameComboBox.SelectedIndex = ((HeaderDP)currentHeader).locationName;
                    musicDayUpDown.Value = ((HeaderDP)currentHeader).musicDayID;
                    musicNightUpDown.Value = ((HeaderDP)currentHeader).musicNightID;
                    areaSettingsComboBox.SelectedIndex = areaSettingsComboBox.FindString("[" + $"{currentHeader.showName:D3}");
                    battleBackgroundUpDown.Value = currentHeader.battleBackground;
                    break;
                case "Plat":
                    areaIconComboBox.SelectedIndex = ((HeaderPt)currentHeader).areaIcon;
                    locationNameComboBox.SelectedIndex = ((HeaderPt)currentHeader).locationName;
                    musicDayUpDown.Value = ((HeaderPt)currentHeader).musicDayID;
                    musicNightUpDown.Value = ((HeaderPt)currentHeader).musicNightID;
                    areaSettingsComboBox.SelectedIndex = areaSettingsComboBox.FindString("[" + $"{currentHeader.showName:D3}");
                    battleBackgroundUpDown.Value = currentHeader.battleBackground;
                    break;
                default:
                    areaIconComboBox.SelectedIndex = areaIconComboBox.FindString("[" + $"{((HeaderHGSS)currentHeader).areaIcon:D3}");
                    locationNameComboBox.SelectedIndex = ((HeaderHGSS)currentHeader).locationName;
                    musicDayUpDown.Value = ((HeaderHGSS)currentHeader).musicDayID;
                    musicNightUpDown.Value = ((HeaderHGSS)currentHeader).musicNightID;
                    worldmapXCoordUpDown.Value = ((HeaderHGSS)currentHeader).worldmapX;
                    worldmapYCoordUpDown.Value = ((HeaderHGSS)currentHeader).worldmapY;
                    break;
            }
            refreshFlags();
            updateWeatherPicAndComboBox();
        }
        private void refreshFlags() {
            BitArray ba = new BitArray(new byte[] { currentHeader.flags });

            flag0CheckBox.Checked = ba[0];
            flag1CheckBox.Checked = ba[1];
            flag2CheckBox.Checked = ba[2];
            flag3CheckBox.Checked = ba[3];

            if (RomInfo.gameVersion == "HG" || RomInfo.gameVersion == "SS") {
                flag4CheckBox.Checked = ba[4];
                flag5CheckBox.Checked = ba[5];
                flag6CheckBox.Checked = ba[6];
                flag7CheckBox.Checked = ba[7];
            }
        }
        private void eventsTabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (eventsTabControl.SelectedTab == signsTabPage) {
                if (spawnablesListBox.Items.Count > 0)
                    spawnablesListBox.SelectedIndex = 0;
            } else if (eventsTabControl.SelectedTab == overworldsTabPage) {
                if (overworldsListBox.Items.Count > 0)
                    overworldsListBox.SelectedIndex = 0;
            } else if (eventsTabControl.SelectedTab == warpsTabPage) {
                if (warpsListBox.Items.Count > 0)
                    warpsListBox.SelectedIndex = 0;
            } else if (eventsTabControl.SelectedTab == triggersTabPage) {
                if (triggersListBox.Items.Count > 0)
                    triggersListBox.SelectedIndex = 0;
            }
        }
        private void headerListBox_Leave(object sender, EventArgs e) {
            if (disableHandlers) 
                return;
            headerListBox.Refresh();
        }
        private void levelScriptUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;
            currentHeader.levelScriptID = (ushort)levelScriptUpDown.Value;
        }
        private void mapNameComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    ((HeaderDP)currentHeader).locationName = (ushort)locationNameComboBox.SelectedIndex;
                    break;
                case "Plat":
                    ((HeaderPt)currentHeader).locationName = (byte)locationNameComboBox.SelectedIndex;
                    break;
                default:
                    ((HeaderHGSS)currentHeader).locationName = (byte)locationNameComboBox.SelectedIndex;
                    break;
            }
        }
        private void matrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;
            currentHeader.matrixID = (ushort)matrixUpDown.Value;
        }
        private void musicDayComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.DPMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
                case "Plat":
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.PtMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
                default:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.HGSSMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
            }
        }
        private void musicNightComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;

            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    currentHeader.musicNightID= (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.DPMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
                case "Plat":
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.PtMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
                default:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.HGSSMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
            }
        }
        private void musicDayUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            disableHandlers = true;
            try {
                ushort updValue = (ushort)((NumericUpDown)sender).Value;
                currentHeader.musicDayID = updValue;
                switch (RomInfo.gameVersion) {
                    case "D":
                    case "P":
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.DPMusicDict[updValue];
                        break;
                    case "Plat":
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.PtMusicDict[updValue];
                        break;
                    default:
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.HGSSMusicDict[updValue];
                        break;
                }
            } catch (KeyNotFoundException) {
                musicDayComboBox.SelectedItem = null;
            }
            disableHandlers = false;
        }
        private void musicNightUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            disableHandlers = true;
            try {
                ushort updValue = (ushort)((NumericUpDown)sender).Value;
                currentHeader.musicNightID = updValue;
                switch (RomInfo.gameVersion) {
                    case "D":
                    case "P":
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.DPMusicDict[updValue];
                        break;
                    case "Plat":
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.PtMusicDict[updValue];
                        break;
                    default:
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.HGSSMusicDict[updValue];
                        break;
                }
            } catch (KeyNotFoundException) {
                musicNightComboBox.SelectedItem = null;
            }
            disableHandlers = false;
        }

        private void weatherUpDown_ValueChanged(object sender, EventArgs e) {
            currentHeader.weatherID = (byte)weatherUpDown.Value;
            updateWeatherPicAndComboBox();
        }
        private void worldmapXCoordUpDown_ValueChanged(object sender, EventArgs e) {
            ((HeaderHGSS)currentHeader).worldmapX = (byte)worldmapXCoordUpDown.Value;
        }
        private void worldmapYCoordUpDown_ValueChanged(object sender, EventArgs e) {
            ((HeaderHGSS)currentHeader).worldmapY = (byte)worldmapYCoordUpDown.Value;
        }
        private void updateWeatherPicAndComboBox() {
            if (disableHandlers)
                return;

            /* Update Weather Combobox*/
            disableHandlers = true;
            try {
                switch (RomInfo.gameVersion) {
                    case "D":
                    case "P":
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.DPWeatherDict[currentHeader.weatherID];
                        break;
                    case "Plat":
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.PtWeatherDict[currentHeader.weatherID];
                        break;
                    default:
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.HGSSWeatherDict[currentHeader.weatherID];
                        break;
                }
            } catch (KeyNotFoundException) {
                weatherComboBox.SelectedItem = null;
            }
            disableHandlers = false;

            /* Update Weather Picture */
            try {
                string imageName = null;
                switch (RomInfo.gameVersion) {
                    case "D":
                    case "P":
                        imageName = PokeDatabase.System.WeatherPics.dpWeatherImageDict[weatherComboBox.SelectedIndex];
                        break;
                    case "Plat":
                        imageName = PokeDatabase.System.WeatherPics.ptWeatherImageDict[weatherComboBox.SelectedIndex];
                        break;
                    default:
                        foreach (KeyValuePair<List<int>, string> entry in PokeDatabase.System.WeatherPics.hgssweatherImageDict) {
                            if (entry.Key.Contains(weatherComboBox.SelectedIndex)) {
                                imageName = entry.Value;
                                break;
                            }
                        }
                        if (imageName == null)
                            throw new KeyNotFoundException();
                        break;
                }

                weatherPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName); 
            } catch (KeyNotFoundException) {
                weatherPictureBox.Image = null;
            }
        }

        private void weatherComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || weatherComboBox.SelectedIndex < 0)
                return;

            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                    weatherUpDown.Value = PokeDatabase.Weather.DPWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
                case "Plat":
                    weatherUpDown.Value = PokeDatabase.Weather.PtWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
                default:
                    weatherUpDown.Value = PokeDatabase.Weather.HGSSWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
            }
            currentHeader.weatherID = (byte)weatherUpDown.Value;
            
        }
        private void openAreaDataButton_Click(object sender, EventArgs e) {
            if (!tilesetEditorIsReady) {
                SetupTilesetEditor();
                tilesetEditorIsReady = true;
            }

            selectAreaDataListBox.SelectedIndex = (int)areaDataUpDown.Value;
            texturePacksListBox.SelectedIndex = (mapTilesetRadioButton.Checked ? (int)areaDataMapTilesetUpDown.Value : (int)areaDataBuildingTilesetUpDown.Value);
            mainTabControl.SelectedTab = nsbtxEditorTabPage;

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
            uint headerOffset = (uint)(PokeDatabase.System.headerOffsetsDict[RomInfo.romID] + MapHeader.length * currentHeader.ID);
            DSUtils.WriteToArm9(headerOffset, currentHeader.toByteArray());

            disableHandlers = true;

            updateCurrentInternalName();
            updateHeaderNameShown(headerListBox.SelectedIndex, currentHeader.ID, internalNames[currentHeader.ID]);
            headerListBox.Focus();
            disableHandlers = false;
        }

        private void updateCurrentInternalName() {
            /* Update internal name according to internalNameBox text*/
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(romInfo.internalNamesLocation))) {
                writer.BaseStream.Position = currentHeader.ID * RomInfo.internalNameLength;

                writer.Write(Encoding.ASCII.GetBytes(internalNameBox.Text.PadRight(16, '\0')));
                internalNames[currentHeader.ID] = internalNameBox.Text;
            }
        }

        private void updateHeaderNameShown(int thisIndex, int headerNumber, string text) {
            disableHandlers = true;

            headerListBox.Items[thisIndex] = headerNumber.ToString("D3") + MapHeader.nameSeparator + text;

            disableHandlers = false;
        }

        private void resetButton_Click(object sender, EventArgs e) {
            searchLocationTextBox.Clear();
            HeaderSearch.HeaderSearchReset(headerListBox, internalNames);
            statusLabel.Text = "Ready";
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
                bool noResult = true;

                switch (RomInfo.gameVersion) {
                    case "D":
                    case "P":
                        for (short i = 0; i < internalNames.Count; i++) {
                            String locationName = locationNameComboBox.Items[((HeaderDP)MapHeader.LoadFromARM9(i)).locationName].ToString();
                            if (locationName.IndexOf(searchLocationTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                                headerListBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + internalNames[i]);
                                noResult = false;
                            }
                        }
                        break;
                    case "Plat":
                        for (short i = 0; i < internalNames.Count; i++) {
                            String locationName = locationNameComboBox.Items[((HeaderPt)MapHeader.LoadFromARM9(i)).locationName].ToString();
                            if (locationName.IndexOf(searchLocationTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                                headerListBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + internalNames[i]);
                                noResult = false;
                            }
                        }
                        break;
                    case "HG":
                    case "SS":
                        for (short i = 0; i < internalNames.Count; i++) {
                            String locationName = locationNameComboBox.Items[((HeaderHGSS)MapHeader.LoadFromARM9(i)).locationName].ToString();
                            if (locationName.IndexOf(searchLocationTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                                headerListBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + internalNames[i]);
                                noResult = false;
                            }
                        }
                        break;
                }
                if (noResult) {
                    headerListBox.Items.Add("No result for " + '"' + searchLocationTextBox.Text + '"');
                    headerListBox.Enabled = false;
                } else {
                    headerListBox.SelectedIndex = 0;
                    headerListBox.Enabled = true;
                }
            } else if (headerListBox.Items.Count < internalNames.Count) {
                HeaderSearch.HeaderSearchReset(headerListBox, internalNames);
            }
        }
        private void scriptFileUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;
            currentHeader.scriptFileID = (ushort)scriptFileUpDown.Value;
        }
        private void areaSettingsComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || areaSettingsComboBox.SelectedItem == null)
                return;

            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    currentHeader.showName = Byte.Parse(areaSettingsComboBox.SelectedItem.ToString().Substring(1, 3));
                    break;
                case "HG":
                case "SS":
                    HeaderHGSS ch = ((HeaderHGSS)currentHeader);

                    ch.areaSettings = (byte)areaSettingsComboBox.SelectedIndex;
                    if (ch.areaSettings == 4) {
                        areaImageLabel.Text = "[Location Tag hidden]";
                        areaIconComboBox.Enabled = false;
                        areaIconPictureBox.Visible = false;
                    } else {
                        areaImageLabel.Text = "Area icon";
                        areaIconComboBox.Enabled = true;
                        areaIconPictureBox.Visible = true;
                    }
                    break;
            }
        }
        private void textFileUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;
            currentHeader.textArchiveID = (ushort)textFileUpDown.Value;
        }
        
        private void wildPokeUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;

            currentHeader.wildPokémon = (ushort)wildPokeUpDown.Value;
            if (wildPokeUpDown.Value == RomInfo.nullEncounterID) {
                wildPokeUpDown.ForeColor = Color.Red;
            } else {
                wildPokeUpDown.ForeColor = Color.Black;
            }

            if (currentHeader.wildPokémon == RomInfo.nullEncounterID)
                openWildEditorWithIdButton.Enabled = false;
            else
                openWildEditorWithIdButton.Enabled = true;
        }

        private void importHeaderFromFileButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Header File (*.dsh; *.bin)|*.dsh;*.bin";
            if (of.ShowDialog(this) != DialogResult.OK)
                return;

            MapHeader h = null;
            try {
                if (new FileInfo(of.FileName).Length > 48)
                    throw new FileFormatException();

                h = MapHeader.BuildFromFile(of.FileName, currentHeader.ID, 0);
                if (h.ID == -1)
                    throw new FileFormatException();

            } catch (FileFormatException) {
                MessageBox.Show("The file you tried to import is either malformed or not a Header file.\nNo changes have been made.",
                        "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentHeader = h;
            uint headerOffset = (uint)(PokeDatabase.System.headerOffsetsDict[RomInfo.romID] + MapHeader.length * currentHeader.ID);
            DSUtils.WriteToArm9(headerOffset, currentHeader.toByteArray());
            try {
                using (BinaryReader reader = new BinaryReader(new FileStream(of.FileName, FileMode.Open))) {
                    reader.BaseStream.Position = MapHeader.length + 8;
                    internalNameBox.Text = Encoding.UTF8.GetString(reader.ReadBytes(RomInfo.internalNameLength));
                    updateCurrentInternalName();
                }
                updateHeaderNameShown(headerListBox.SelectedIndex, currentHeader.ID, internalNames[currentHeader.ID]);
            } catch (EndOfStreamException) { }

            refreshHeaderEditorFields();
        }

        private void exportHeaderToFileButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "DSPRE Header File (*.dsh)|*.dsh";
            sf.FileName = "Header " + currentHeader.ID + " - " + internalNames[currentHeader.ID] + " (" + locationNameComboBox.SelectedItem.ToString() + ")";
            if (sf.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(new FileStream(sf.FileName, FileMode.Create))) {
                writer.Write(currentHeader.toByteArray()); //Write full header
                writer.Write((byte)0x00); //Padding
                writer.Write(Encoding.UTF8.GetBytes("INTNAME")); //Signature
                writer.Write(Encoding.UTF8.GetBytes(internalNames[currentHeader.ID])); //Save Internal name
            }
        }

        /*Copy Paste Functions*/
        #region Variables
        int locationNameCopy;
        string internalNameCopy;
        decimal encountersIDCopy;
        int shownameCopy;
        int areaIconCopy;

        int musicdayCopy;
        int musicnightCopy;
        int weatherCopy;
        int camAngleCopy;
        int areaSettingsCopy;

        decimal scriptsCopy;
        decimal levelScriptsCopy;
        decimal eventsCopy;
        decimal textsCopy;

        decimal matrixCopy;
        decimal areadataCopy;
        decimal worldmapXCoordCopy;
        decimal worldmapYCoordCopy;
        decimal battleBGCopy;

        byte flagsCopy;

        #endregion
        private void copyHeaderButton_Click(object sender, EventArgs e) {
            locationNameCopy = locationNameComboBox.SelectedIndex;
            internalNameCopy = internalNameBox.Text;
            shownameCopy = areaSettingsComboBox.SelectedIndex;
            areaIconCopy = areaIconComboBox.SelectedIndex;
            areaSettingsCopy = areaSettingsComboBox.SelectedIndex;
            encountersIDCopy = wildPokeUpDown.Value;

            musicdayCopy = musicDayComboBox.SelectedIndex;
            musicnightCopy = musicNightComboBox.SelectedIndex;
            weatherCopy = weatherComboBox.SelectedIndex;
            camAngleCopy = cameraComboBox.SelectedIndex;

            scriptsCopy = scriptFileUpDown.Value;
            levelScriptsCopy = levelScriptUpDown.Value;
            eventsCopy = eventFileUpDown.Value;
            textsCopy = textFileUpDown.Value;

            matrixCopy = matrixUpDown.Value;
            areadataCopy = areaDataUpDown.Value;
            worldmapXCoordCopy = worldmapXCoordUpDown.Value;
            worldmapYCoordCopy = worldmapYCoordUpDown.Value;
            battleBGCopy = battleBackgroundUpDown.Value;

            /*Enable paste buttons*/
            pasteHeaderButton.Enabled = true;


            pasteLocationNameButton.Enabled = true;
            pasteInternalNameButton.Enabled = true;
            pasteAreaSettingsButton.Enabled = true;
            pasteAreaIconButton.Enabled = true;
            pasteWildEncountersButton.Enabled = true;

            pasteMusicDayButton.Enabled = true;
            pasteMusicNightButton.Enabled = true;
            pasteWeatherButton.Enabled = true;
            pasteCameraAngleButton.Enabled = true;

            pasteScriptsButton.Enabled = true;
            pasteLevelScriptsButton.Enabled = true;
            pasteEventsButton.Enabled = true;
            pasteTextsButton.Enabled = true;

            pasteMatrixButton.Enabled = true;
            pasteAreaDataButton.Enabled = true;

            worldmapCoordsCopyButton.Enabled = true;

            pasteFlagsButton.Enabled = true;

            headerListBox.Focus();
        }
        private void copyInternalNameButton_Click(object sender, EventArgs e) {
            internalNameCopy = internalNameBox.Text;
            Clipboard.SetData(DataFormats.Text, internalNameCopy);
            pasteInternalNameButton.Enabled = true;
        }
        private void copyLocationNameButton_Click(object sender, EventArgs e) {
            locationNameCopy = locationNameComboBox.SelectedIndex;
            pasteLocationNameButton.Enabled = true;
        }
        private void copyAreaSettingsButton_Click(object sender, EventArgs e) { 
            areaSettingsCopy = areaSettingsComboBox.SelectedIndex;
            pasteAreaSettingsButton.Enabled = true;
        }
        private void copyAreaIconButton_Click(object sender, EventArgs e) {
            areaIconCopy = areaIconComboBox.SelectedIndex;
            pasteAreaIconButton.Enabled = true;
        }
        private void copyWildEncountersButton_Click(object sender, EventArgs e) {
            encountersIDCopy = wildPokeUpDown.Value;
            Clipboard.SetData(DataFormats.Text, encountersIDCopy);
            pasteWildEncountersButton.Enabled = true;
        }
        private void copyMusicDayButton_Click(object sender, EventArgs e) {
            musicdayCopy = musicDayComboBox.SelectedIndex;
            pasteMusicDayButton.Enabled = true;
        }
        private void copyWeatherButton_Click(object sender, EventArgs e) {
            weatherCopy = weatherComboBox.SelectedIndex;
            pasteWeatherButton.Enabled = true;
        }
        private void copyMusicNightButton_Click(object sender, EventArgs e) {
            musicnightCopy = musicNightComboBox.SelectedIndex;
            pasteMusicNightButton.Enabled = true;
        }
        private void copyCameraAngleButton_Click(object sender, EventArgs e) {
            camAngleCopy = cameraComboBox.SelectedIndex;
            pasteCameraAngleButton.Enabled = true;
        }
        private void copyScriptsButton_Click(object sender, EventArgs e) {
            scriptsCopy = scriptFileUpDown.Value;
            Clipboard.SetData(DataFormats.Text, scriptsCopy);
            pasteScriptsButton.Enabled = true;
        }
        private void copyLevelScriptsButton_Click(object sender, EventArgs e) {
            levelScriptsCopy = levelScriptUpDown.Value;
            Clipboard.SetData(DataFormats.Text, levelScriptsCopy);
            pasteLevelScriptsButton.Enabled = true;
        }
        private void copyEventsButton_Click(object sender, EventArgs e) {
            eventsCopy = eventFileUpDown.Value;
            Clipboard.SetData(DataFormats.Text, eventsCopy);
            pasteEventsButton.Enabled = true;
        }
        private void copyTextsButton_Click(object sender, EventArgs e) {
            textsCopy = textFileUpDown.Value;
            Clipboard.SetData(DataFormats.Text, textsCopy);
            pasteTextsButton.Enabled = true;
        }
        private void copyMatrixButton_Click(object sender, EventArgs e) {
            matrixCopy = matrixUpDown.Value;
            Clipboard.SetData(DataFormats.Text, matrixCopy);
            pasteMatrixButton.Enabled = true;
        }
        private void copyAreaDataButton_Click(object sender, EventArgs e) {
            areadataCopy = areaDataUpDown.Value;
            Clipboard.SetData(DataFormats.Text, areadataCopy);
            pasteAreaDataButton.Enabled = true;
        }
        private void worldmapCoordsCopyButton_Click(object sender, EventArgs e) {
            worldmapXCoordCopy = worldmapXCoordUpDown.Value;
            worldmapYCoordCopy = worldmapYCoordUpDown.Value;
            worldmapCoordsPasteButton.Enabled = true;
        }
        private void copyMapSettingsButton_Click(object sender, EventArgs e) {
            flagsCopy = currentHeader.flags;
            battleBGCopy = currentHeader.battleBackground;
            pasteFlagsButton.Enabled = true;
        }

        /* Paste Buttons */
        private void pasteHeaderButton_Click(object sender, EventArgs e) {
            locationNameComboBox.SelectedIndex = locationNameCopy;
            internalNameBox.Text = internalNameCopy;
            wildPokeUpDown.Value = encountersIDCopy;

            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    areaSettingsComboBox.SelectedIndex = shownameCopy;
                    break;
                case "HG":
                case "SS":
                    areaSettingsComboBox.SelectedIndex = areaSettingsCopy;
                    break;
            }
            areaIconComboBox.SelectedIndex = areaIconCopy;

            musicDayComboBox.SelectedIndex = musicdayCopy;
            musicNightComboBox.SelectedIndex = musicnightCopy;
            weatherComboBox.SelectedIndex = weatherCopy;
            cameraComboBox.SelectedIndex = camAngleCopy;

            scriptFileUpDown.Value = scriptsCopy;
            levelScriptUpDown.Value = levelScriptsCopy;
            eventFileUpDown.Value = eventsCopy;
            textFileUpDown.Value = textsCopy;

            matrixUpDown.Value = matrixCopy;
            areaDataUpDown.Value = areadataCopy;

            currentHeader.flags = flagsCopy;
            battleBackgroundUpDown.Value = battleBGCopy;
            refreshFlags();
        }
        private void pasteInternalNameButton_Click(object sender, EventArgs e) {
            internalNameBox.Text = internalNameCopy;
        }
        private void pasteLocationNameButton_Click(object sender, EventArgs e) {
            locationNameComboBox.SelectedIndex = locationNameCopy;
        }
        private void pasteAreaSettingsButton_Click(object sender, EventArgs e) {
            areaSettingsComboBox.SelectedIndex = shownameCopy;
        }
        private void pasteAreaIconButton_Click(object sender, EventArgs e) {
            if (areaIconComboBox.Enabled)
                areaIconComboBox.SelectedIndex = areaIconCopy;
        }
        private void pasteWildEncountersButton_Click(object sender, EventArgs e) {
            wildPokeUpDown.Value = encountersIDCopy;
        }
        private void pasteMusicDayButton_Click(object sender, EventArgs e) {
            musicDayComboBox.SelectedIndex = musicdayCopy;
        }
        private void pasteScriptsButton_Click(object sender, EventArgs e) {
            scriptFileUpDown.Value = scriptsCopy;
        }
        private void pasteLevelScriptsButton_Click(object sender, EventArgs e) {
            levelScriptUpDown.Value = levelScriptsCopy;
        }
        private void pasteEventsButton_Click(object sender, EventArgs e) {
            eventFileUpDown.Value = eventsCopy;
        }
        private void pasteTextsButton_Click(object sender, EventArgs e) {
            textFileUpDown.Value = textsCopy;
        }
        private void pasteMatrixButton_Click(object sender, EventArgs e) {
            matrixUpDown.Value = matrixCopy;
        }
        private void pasteAreaDataButton_Click(object sender, EventArgs e) {
            areaDataUpDown.Value = areadataCopy;
        }
        private void pasteWeatherButton_Click(object sender, EventArgs e) {
            weatherComboBox.SelectedIndex = weatherCopy;
        }
        private void pasteMusicNightButton_Click(object sender, EventArgs e) {
            musicNightComboBox.SelectedIndex = musicnightCopy;
        }
        private void pasteCameraAngleButton_Click(object sender, EventArgs e) {
            cameraComboBox.SelectedIndex = camAngleCopy;
        }
        private void worldmapCoordsPasteButton_Click(object sender, EventArgs e) {
            worldmapXCoordUpDown.Value = worldmapXCoordCopy;
            worldmapYCoordUpDown.Value = worldmapYCoordCopy;
        }
        private void pasteMapSettingsButton_Click(object sender, EventArgs e) {
            currentHeader.flags = flagsCopy;
            battleBackgroundUpDown.Value = battleBGCopy;
            refreshFlags();
        }
        #endregion

        #region Matrix Editor

        Matrix currentMatrix;

        #region Subroutines
        private void ClearMatrixTables() {
            headersGridView.Rows.Clear();
            headersGridView.Columns.Clear();
            heightsGridView.Rows.Clear();
            heightsGridView.Columns.Clear();
            mapFilesGridView.Rows.Clear();
            mapFilesGridView.Columns.Clear();
            matrixTabControl.TabPages.Remove(headersTabPage);
            matrixTabControl.TabPages.Remove(heightsTabPage);
        }
        private Tuple<Color, Color> FormatMapCell(uint cellValue) {
            foreach (KeyValuePair<List<uint>, Tuple<Color, Color>> entry in romInfo.mapCellsColorDictionary) {
                if (entry.Key.Contains(cellValue)) 
                    return entry.Value;
            }
            return Tuple.Create(Color.White, Color.Black);
        }
        private void GenerateMatrixTables() {
            /* Generate table columns */
            if (currentMatrix == null)
                return;

            for (int i = 0; i < currentMatrix.width; i++) {
                headersGridView.Columns.Add("Column" + i, i.ToString("D"));
                headersGridView.Columns[i].Width = 32; // Set column size
                headersGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

                heightsGridView.Columns.Add("Column" + i, i.ToString("D"));
                heightsGridView.Columns[i].Width = 21; // Set column size
                heightsGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

                mapFilesGridView.Columns.Add("Column" + i, i.ToString("D"));
                mapFilesGridView.Columns[i].Width = 32; // Set column size
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

            if (currentMatrix.hasHeadersSection) 
                matrixTabControl.TabPages.Add(headersTabPage);
            if (currentMatrix.hasHeightsSection) 
                matrixTabControl.TabPages.Add(heightsTabPage);
        }
        #endregion

        private void addHeadersButton_Click(object sender, EventArgs e) {
            if (!currentMatrix.hasHeadersSection) {
                currentMatrix.hasHeadersSection = true;
                matrixTabControl.TabPages.Add(headersTabPage);
            }
        }
        private void addHeightsButton_Click(object sender, EventArgs e) {
            if (!currentMatrix.hasHeightsSection) {
                currentMatrix.hasHeightsSection = true;
                matrixTabControl.TabPages.Add(heightsTabPage);
            }
        }
        private void addMatrixButton_Click(object sender, EventArgs e) {
            /* Load new matrix, a copy of Matrix 0 */
            Matrix newMatrix = new Matrix(0);

            /* Add new matrix file to matrix folder */
            string matrixPath = RomInfo.matrixDirPath + "\\" + romInfo.GetMatrixCount().ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(matrixPath, FileMode.Create))) writer.Write(newMatrix.ToByteArray());

            /* Update ComboBox*/
            selectMatrixComboBox.Items.Add("Matrix " + (romInfo.GetMatrixCount()-1).ToString());
        }
        private void exportMatrixButton_Click(object sender, EventArgs e) {
            currentMatrix.SaveToFileExplorePath("Matrix " + selectMatrixComboBox.SelectedIndex);
        }
        private void saveMatrixButton_Click(object sender, EventArgs e) {
            currentMatrix.SaveToFileDefaultDir(selectMatrixComboBox.SelectedIndex);
            eventMatrix = new Matrix(selectMatrixComboBox.SelectedIndex);
        }
        private void headersGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (headerListBox.Items.Count < internalNames.Count)
                HeaderSearch.HeaderSearchReset(headerListBox, internalNames);
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
                if (!UInt16.TryParse(headersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out cellValue)) 
                    cellValue = 0;

                /* Change value in matrix object */
                currentMatrix.headers[e.RowIndex, e.ColumnIndex] = cellValue;
            }
        }
        private void headersGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.Value == null) 
                return;
            disableHandlers = true;

            /* Format table cells corresponding to border maps or void */
            ushort colorValue;
            if (!UInt16.TryParse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out colorValue)) colorValue = Matrix.EMPTY;

            Tuple<Color, Color> cellColors = FormatMapCell(colorValue);
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
                if (!Byte.TryParse(heightsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out cellValue)) 
                    cellValue = 0;

                /* Change value in matrix object */
                currentMatrix.altitudes[e.RowIndex, e.ColumnIndex] = cellValue;
            }
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
                    int index = currentMatrix.width + i;
                    headersGridView.Columns.Add(" ", (index).ToString());
                    heightsGridView.Columns.Add(" ", (index).ToString());
                    mapFilesGridView.Columns.Add(" ", (index).ToString());

                    /* Adjust column width */
                    headersGridView.Columns[index].Width = 34;
                    heightsGridView.Columns[index].Width = 22;
                    mapFilesGridView.Columns[index].Width = 34;

                    /* Fill new rows */
                    for (int j = 0; j < currentMatrix.height; j++) {
                        headersGridView.Rows[j].Cells[index].Value = 0;
                        heightsGridView.Rows[j].Cells[index].Value = 0;
                        mapFilesGridView.Rows[j].Cells[index].Value = Matrix.EMPTY;
                    }
                }
            }

            /* Modify matrix object */
            currentMatrix.ResizeMatrix((int)heightUpDown.Value, (int)widthUpDown.Value);
            disableHandlers = false;
        }
        private void heightUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            disableHandlers = true;

            /* Add or remove rows in DataGridView control */
            int delta = (int)heightUpDown.Value - currentMatrix.height;
            for (int i = 0; i < Math.Abs(delta); i++) {
                if (delta < 0) // Remove rows
                {
                    headersGridView.Rows.RemoveAt(currentMatrix.height - 1 - i);
                    heightsGridView.Rows.RemoveAt(currentMatrix.height - 1 - i);
                    mapFilesGridView.Rows.RemoveAt(currentMatrix.height - 1 - i);
                } else {
                    /* Add row in DataGridView */
                    headersGridView.Rows.Add();
                    heightsGridView.Rows.Add();
                    mapFilesGridView.Rows.Add();

                    int index = currentMatrix.height + i;
                    headersGridView.Rows[index].HeaderCell.Value = (index).ToString();
                    heightsGridView.Rows[index].HeaderCell.Value = (index).ToString();
                    mapFilesGridView.Rows[index].HeaderCell.Value = (index).ToString();
                    
                    /* Fill new rows */
                    for (int j = 0; j < currentMatrix.width; j++) {
                        headersGridView.Rows[index].Cells[j].Value = 0;
                        heightsGridView.Rows[index].Cells[j].Value = 0;
                        mapFilesGridView.Rows[index].Cells[j].Value = Matrix.EMPTY;
                    }
                }
            }

            /* Modify matrix object */
            currentMatrix.ResizeMatrix((int)heightUpDown.Value, (int)widthUpDown.Value);
            disableHandlers = false;
        }
        private void heightsGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.Value == null)
                return;
            disableHandlers = true;

            /* Format table cells corresponding to border maps or void */
            ushort colorValue;
            if (!UInt16.TryParse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out colorValue)) 
                colorValue = Matrix.EMPTY;

            Tuple<Color, Color> cellColors = FormatMapCell(colorValue);
            e.CellStyle.BackColor = cellColors.Item1;
            e.CellStyle.ForeColor = cellColors.Item2;

            /* If invalid input is entered, show 00 */
            byte cellValue;
            if (!Byte.TryParse(heightsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out cellValue)) 
                e.Value = 0;

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
            ClearMatrixTables();
            GenerateMatrixTables();

            /* Setup matrix editor controls */
            disableHandlers = true;
            matrixNameTextBox.Text = currentMatrix.name;
            widthUpDown.Value = currentMatrix.width;
            heightUpDown.Value = currentMatrix.height;
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
                short header;
                if (currentMatrix.hasHeadersSection) {
                    header = (short)currentMatrix.headers[e.RowIndex, e.ColumnIndex];
                } else {
                    header = (short)headerListBox.SelectedIndex;
                }

                AreaData areaData;
                if (header > internalNames.Count) {
                    MessageBox.Show("This map is associated to a non-existent header.\nThis will lead to unpredictable behaviour and, possibily, problems, if you attempt to load it in game.",
                        "Invalid header", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    header = 0;
                }

                /* get texture file numbers from area data */
                areaData = LoadAreaData(MapHeader.LoadFromARM9(header).areaDataID);
                /* Load Map File and switch to Map Editor tab */
                disableHandlers = true;

                selectMapComboBox.SelectedIndex = currentMatrix.maps[e.RowIndex, e.ColumnIndex];
                mapTextureComboBox.SelectedIndex = areaData.mapTileset + 1;
                buildTextureComboBox.SelectedIndex = areaData.buildingsTileset + 1;
                mainTabControl.SelectedTab = mapEditorTabPage;

                //what's this IF for??
                //if (mapPartsTabControl.SelectedTab == permissionsTabPage) 

                if (areaData.areaType == AreaData.TYPE_INDOOR)
                    interiorbldRadioButton.Checked = true;
                else 
                    exteriorbldRadioButton.Checked = true;

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

            Tuple<Color, Color> cellColors = FormatMapCell(colorValue);
            e.CellStyle.BackColor = cellColors.Item1;
            e.CellStyle.ForeColor = cellColors.Item2;

            if (colorValue == Matrix.EMPTY)
                e.Value = '-';

            disableHandlers = false;

        }
        private void matrixNameTextBox_TextChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;
            currentMatrix.name = matrixNameTextBox.Text;
        }
        private void removeHeadersButton_Click(object sender, EventArgs e) {
            matrixTabControl.TabPages.Remove(headersTabPage);
            currentMatrix.hasHeadersSection = false;
        }
        private void removeHeightsButton_Click(object sender, EventArgs e) {
            matrixTabControl.TabPages.Remove(heightsTabPage);
            currentMatrix.hasHeightsSection = false;
        }
        private void removeMatrixButton_Click(object sender, EventArgs e) {
            if (selectMatrixComboBox.Items.Count > 1) {
                /* Delete matrix file */
                int matrixToDelete = romInfo.GetMatrixCount() - 1;

                string matrixPath = RomInfo.matrixDirPath + "\\" + matrixToDelete.ToString("D4");
                File.Delete(matrixPath);

                /* Change selected index if the matrix to be deleted is currently selected */
                if (selectMatrixComboBox.SelectedIndex == matrixToDelete) 
                    selectMatrixComboBox.SelectedIndex--;

                /* Remove entry from ComboBox, and decrease matrix count */
                selectMatrixComboBox.Items.RemoveAt(matrixToDelete);
            } else {
                MessageBox.Show("At least one matrix must be kept.", "Can't delete matrix", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void selectMatrixComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            ClearMatrixTables();
            currentMatrix = new Matrix(selectMatrixComboBox.SelectedIndex);
            GenerateMatrixTables();

            /* Setup matrix editor controls */
            disableHandlers = true;
            matrixNameTextBox.Text = currentMatrix.name;
            widthUpDown.Value = currentMatrix.width;
            heightUpDown.Value = currentMatrix.height;
            disableHandlers = false;
        }
        

        private void importColorTableButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "DSPRE Color Table File (*.ctb)|*.ctb";
            if (of.ShowDialog(this) != DialogResult.OK)
                return;

            string[] fileTableContent = File.ReadAllLines(of.FileName);

            string mapKeyword = "[Maplist]";
            string colorKeyword = "[Color]";
            string textColorKeyword = "[TextColor]";
            string dashSeparator = "-";
            string problematicSegment = "incomplete line";
             
            Dictionary<List<uint>, Tuple<Color, Color>> colorsDict = new Dictionary<List<uint>, Tuple<Color, Color>>();
            List<string> linesWithErrors = new List<string>();

            for (int i = 0; i < fileTableContent.Length; i++)   {
                if (fileTableContent[i].Length > 0) {
                    string[] lineParts = fileTableContent[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    try {
                        int j = 0;
                        if (!lineParts[j].Equals(mapKeyword)) {
                            problematicSegment = nameof(mapKeyword);
                            throw new FormatException();
                        }
                        j++;

                        List<uint> mapList = new List<uint>();
                        while (!lineParts[j].Equals(dashSeparator)) {

                            if (lineParts[j].Equals("and")) {
                                j++;
                            }
                            uint firstValue = uint.Parse(lineParts[j++]);
                            mapList.Add(firstValue);

                            if (lineParts[j].Equals("to")) {
                                j++;
                                uint finalValue = uint.Parse(lineParts[j++]);
                                //Add all numbers ranging from maplist[0] to finalValue
                                if (firstValue > finalValue)
                                    Swap(ref firstValue, ref finalValue);

                                for (uint k = firstValue + 1; k <= finalValue; k++) {
                                    mapList.Add(k);
                                }
                            }
                        }

                        if (!lineParts[j].Equals(dashSeparator)) {
                            problematicSegment = nameof(dashSeparator);
                            throw new FormatException();
                        }
                        j++;

                        if (!lineParts[j].Equals(colorKeyword)) {
                            problematicSegment = nameof(colorKeyword);
                            throw new FormatException();
                        }
                        j++;

                        int r = Int32.Parse(lineParts[j++]);
                        int g = Int32.Parse(lineParts[j++]);
                        int b = Int32.Parse(lineParts[j++]);

                        if (!lineParts[j].Equals(dashSeparator)) {
                            problematicSegment = nameof(dashSeparator);
                            throw new FormatException();
                        }
                        j++;

                        if (!lineParts[j].Equals(textColorKeyword)) {
                            problematicSegment = nameof(textColorKeyword);
                            throw new FormatException();
                        }
                        j++;

                        colorsDict.Add(   mapList, Tuple.Create( Color.FromArgb(r, g, b), Color.FromName(lineParts[j++]) )    );
                    } catch {
                        linesWithErrors.Add(i + 1 + " (err. " + problematicSegment + ")\n");
                        continue;
                    }
                }
            }
            colorsDict.Add(new List<uint> { Matrix.EMPTY }, Tuple.Create(Color.Black, Color.White));

            string errorMsg = "";
            MessageBoxIcon iconType = MessageBoxIcon.Information;
            if (linesWithErrors.Count > 0) {
                errorMsg = "\nHowever, the following lines couldn't be parsed correctly:\n";

                foreach(string s in linesWithErrors)
                    errorMsg += "- Line " + s;

                iconType = MessageBoxIcon.Warning;
            }
            romInfo.SetMapCellsColorDictionary(colorsDict);
            ClearMatrixTables();
            GenerateMatrixTables();
            MessageBox.Show("Color file has been read." + errorMsg, "Operation completed", MessageBoxButtons.OK, iconType);
        }

        public void Swap(ref uint a, ref uint b) {
            uint temp = a;
            a = b;
            b = temp;
        }
        private void resetColorTableButton_Click(object sender, EventArgs e) {
            romInfo.LoadMapCellsColorDictionary();
            ClearMatrixTables();
            GenerateMatrixTables();
        }
        #endregion

        #region Map Editor

        #region Variables
        /*  Camera settings */
        public bool hideBuildings = new bool();
        public bool mapTexturesOn = true;
        public bool showBuildingTextures = true;
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

            string[] bldList = GetBuildingsList(interiorbldRadioButton.Checked);
            for (int i = 0; i < currentMapFile.buildings.Count; i++)
                // Add entry into buildings ListBox
                buildingsListBox.Items.Add((i+1).ToString("D2") + MapHeader.nameSeparator + buildIndexComboBox.Items[(int)currentMapFile.buildings[i].modelID]);
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
            string mapFilePath = RomInfo.mapDirPath + "\\" + selectMapComboBox.Items.Count.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(mapFilePath, FileMode.Create))) writer.Write(LoadMapFile(0).ToByteArray());

            /* Update ComboBox and select new file */
            selectMapComboBox.Items.Add(selectMapComboBox.Items.Count.ToString("D3") + MapHeader.nameSeparator + "newmap");
            selectMapComboBox.SelectedIndex = selectMapComboBox.Items.Count - 1;
        }
        private void replaceMapBinButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .bin file */
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Map BIN File (*.bin)|*.bin";
            if (of.ShowDialog(this) != DialogResult.OK)
                return;

            /* Update map object in memory */
            string path = RomInfo.mapDirPath + "\\" + selectMapComboBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Refresh controls */
            selectMapComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show("Map BIN imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        private void buildTextureComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || buildTextureComboBox.SelectedIndex < 0) 
                return;

            if (buildTextureComboBox.SelectedIndex == 0) {
                showBuildingTextures = false;
            } else {
                string texturePath = romInfo.buildingTexturesDirPath + "\\" + (buildTextureComboBox.SelectedIndex - 1).ToString("D4");
                byte[] textureFile = File.ReadAllBytes(texturePath);

                Stream str = new MemoryStream(textureFile);
                foreach (Building building in currentMapFile.buildings) {
                    str.Position = 0;
                    building.NSBMDFile.materials = NSBTXLoader.LoadNsbtx(str, out building.NSBMDFile.Textures, out building.NSBMDFile.Palettes);

                    try {
                        building.NSBMDFile.MatchTextures();
                        showBuildingTextures = true;
                    } catch {
                        if (!buildTextureComboBox.Items[buildTextureComboBox.SelectedIndex].ToString().StartsWith("Error!")) {
                            disableHandlers = true;
                            buildTextureComboBox.Items[buildTextureComboBox.SelectedIndex] = buildTextureComboBox.Items[buildTextureComboBox.SelectedIndex].ToString().Insert(0, "Error! - ");
                            disableHandlers = false;
                        }
                        showBuildingTextures = false;
                    }
                }
                //buildTextureComboBox.Items[buildTextureComboBox.SelectedIndex] = "Error - Building Texture Pack too small [" + (buildTextureComboBox.SelectedIndex - 1).ToString("D2") + "]";


            }
            
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void mapTextureComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) return;

            if (mapTextureComboBox.SelectedIndex == 0)
                mapTexturesOn = false;
            else {
                mapTexturesOn = true;

                string texturePath = romInfo.mapTexturesDirPath + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4");
                currentMapFile.mapModel.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(File.ReadAllBytes(texturePath)), out currentMapFile.mapModel.Textures, out currentMapFile.mapModel.Palettes);
                try {
                    currentMapFile.mapModel.MatchTextures();
                } catch { }
            }
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void mapEditorTabPage_Enter(object sender, EventArgs e) {
            mapOpenGlControl.MakeCurrent();
            if (selectMapComboBox.SelectedIndex > -1) 
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void mapOpenGlControl_MouseWheel(object sender, MouseEventArgs e) // Zoom In/Out
        {
            dist -= (float)e.Delta / 200;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
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
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
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
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
            } else if (mapPartsTabControl.SelectedTab == permissionsTabPage) {
                radio2D.Checked = true;

                hideBuildings = false;
                radio3D.Enabled = false;
                radio2D.Enabled = false;
                wireframeCheckBox.Enabled = false;

                cam2Dmode();
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);

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
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
            } else { // Terrain and BGS
                radio2D.Checked = true;

                hideBuildings = false;
                radio3D.Enabled = false;
                radio2D.Enabled = false;
                wireframeCheckBox.Enabled = false;

                mapOpenGlControl.BringToFront();

                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
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
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void cam2Dmode() {
            perspective = 4f;
            ang = 0f;
            dist = 115.2f;
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
            File.Delete(RomInfo.mapDirPath + "\\" + (selectMapComboBox.Items.Count - 1).ToString("D4"));

            /* Check if currently selected file is the last one, and in that case select the one before it */
            int lastIndex = selectMapComboBox.Items.Count - 1;
            if (selectMapComboBox.SelectedIndex == lastIndex) 
                selectMapComboBox.SelectedIndex--;

            /* Remove item from ComboBox */
            selectMapComboBox.Items.RemoveAt(lastIndex);
        }
        private void saveMapButton_Click(object sender, EventArgs e) {
            string mapIndex = selectMapComboBox.SelectedIndex.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(RomInfo.mapDirPath + "\\" + mapIndex, FileMode.Create)))
                writer.Write(currentMapFile.ToByteArray());
        }
        private void exportCurrentMapBinButton_Click(object sender, EventArgs e) {
            SaveFileDialog eb = new SaveFileDialog();
            eb.Filter = "Gen IV Map BIN File (*.bin)|*.bin";
            eb.FileName = selectMapComboBox.SelectedItem.ToString();
            if (eb.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(eb.FileName))) {
                writer.Write(currentMapFile.ToByteArray());
            }

            MessageBox.Show("Map BIN exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void selectMapComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;

            /* Load map data into MapFile class instance */
            currentMapFile = LoadMapFile(selectMapComboBox.SelectedIndex);

            /* Load map textures for renderer */
            if (mapTextureComboBox.SelectedIndex > 0) 
                currentMapFile.mapModel = LoadModelTextures(currentMapFile.mapModel, romInfo.mapTexturesDirPath, mapTextureComboBox.SelectedIndex - 1);

            /* Load buildings nsbmd and textures for renderer into MapFile's building objects */
            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                currentMapFile.buildings[i] = LoadBuildingModel(currentMapFile.buildings[i], interiorbldRadioButton.Checked); // Load building nsbmd
                if (buildTextureComboBox.SelectedIndex > 0) 
                    currentMapFile.buildings[i].NSBMDFile = LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, romInfo.buildingTexturesDirPath, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
            }

            /* Render the map */
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);

            /* Draw permissions in the small selection boxes */
            DrawSmallCollision();
            DrawSmallTypeCollision();

            /* Draw selected permissions category */
            if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                DrawCollisionGrid();
            } else {
                DrawTypeGrid();
            }
            /* Set map screenshot as background picture in permissions editor PictureBox */
            movPictureBox.BackgroundImage = GrabMapScreenshot(movPictureBox.Width, movPictureBox.Height);

            RestorePainter();

            /* Fill buildings ListBox, and if not empty select first item */
            FillBuildingsBox();
            if (buildingsListBox.Items.Count > 0) 
                buildingsListBox.SelectedIndex = 0;

            ModelSizeTXT.Text = currentMapFile.mapModelData.Length.ToString() + " B";
            TerrainSizeTXT.Text = currentMapFile.bdhc.Length.ToString() + " B";

            if (RomInfo.gameVersion == "HG" || RomInfo.gameVersion == "SS") {
                BGSSizeTXT.Text = currentMapFile.bgs.Length.ToString() + " B";
            }
        }
        private void wireframeCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (wireframeCheckBox.Checked) {
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
            } else {
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            }

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        #region Building Editor
        private void addBuildingButton_Click(object sender, EventArgs e) {
            addBuildingToMap(new Building());
        }
        private void duplicateBuildingButton_Click(object sender, EventArgs e) {
            if (buildingsListBox.SelectedIndex > -1)
                addBuildingToMap(new Building(currentMapFile.buildings[buildingsListBox.SelectedIndex]));
        }
        private void addBuildingToMap(Building b) {
            currentMapFile.AddBuilding(b);

            /* Load new building's model and textures for the renderer */
            currentMapFile.buildings[currentMapFile.buildings.Count - 1] = LoadBuildingModel(b, interiorbldRadioButton.Checked);
            currentMapFile.buildings[currentMapFile.buildings.Count - 1].NSBMDFile = LoadModelTextures(b.NSBMDFile, romInfo.buildingTexturesDirPath, buildTextureComboBox.SelectedIndex - 1);

            /* Add new entry to buildings ListBox */
            buildingsListBox.Items.Add((buildingsListBox.Items.Count + 1).ToString("D2") + MapHeader.nameSeparator +
                buildIndexComboBox.Items[(int)b.modelID]);
            buildingsListBox.SelectedIndex = buildingsListBox.Items.Count - 1;

            /* Redraw scene with new building */
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void buildIndexComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || buildingsListBox.SelectedIndex < 0) 
                return;

            disableHandlers = true;
            buildingsListBox.Items[buildingsListBox.SelectedIndex] = (buildingsListBox.SelectedIndex + 1).ToString("D2") + MapHeader.nameSeparator + buildIndexComboBox.SelectedItem;
            disableHandlers = false;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].modelID = (uint)buildIndexComboBox.SelectedIndex;
            currentMapFile.buildings[buildingsListBox.SelectedIndex] = LoadBuildingModel(currentMapFile.buildings[buildingsListBox.SelectedIndex], interiorbldRadioButton.Checked);
            currentMapFile.buildings[buildingsListBox.SelectedIndex].NSBMDFile = LoadModelTextures(currentMapFile.buildings[buildingsListBox.SelectedIndex].NSBMDFile, romInfo.buildingTexturesDirPath, buildTextureComboBox.SelectedIndex - 1);

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);

        }
        private void buildingsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;
            disableHandlers = true;

            int buildingNumber = buildingsListBox.SelectedIndex;

            buildIndexComboBox.SelectedIndex = (int)currentMapFile.buildings[buildingNumber].modelID;
            xBuildUpDown.Value = currentMapFile.buildings[buildingNumber].xPosition + (decimal)currentMapFile.buildings[buildingNumber].xFraction/65535;
            zBuildUpDown.Value = currentMapFile.buildings[buildingNumber].zPosition + (decimal)currentMapFile.buildings[buildingNumber].yFraction/65535;
            yBuildUpDown.Value = currentMapFile.buildings[buildingNumber].yPosition + (decimal)currentMapFile.buildings[buildingNumber].zFraction/65535;

            buildingWidthUpDown.Value = currentMapFile.buildings[buildingNumber].width;
            buildingHeightUpDown.Value = currentMapFile.buildings[buildingNumber].height;
            buildingLengthUpDown.Value = currentMapFile.buildings[buildingNumber].length;

            disableHandlers = false;
        }
        private void buildingHeightUpDown_ValueChanged(object sender, EventArgs e) {
            if (buildingsListBox.SelectedIndex > -1) {
                currentMapFile.buildings[buildingsListBox.SelectedIndex].height = (uint)buildingHeightUpDown.Value;
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
            }
        }
        private void buildingLengthUpDown_ValueChanged(object sender, EventArgs e) {
            if (buildingsListBox.SelectedIndex > -1) {
                currentMapFile.buildings[buildingsListBox.SelectedIndex].length = (uint)buildingLengthUpDown.Value;
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
            }
        }
        private void buildingWidthUpDown_ValueChanged(object sender, EventArgs e) {
            if (buildingsListBox.SelectedIndex > -1) {
                currentMapFile.buildings[buildingsListBox.SelectedIndex].width = (uint)buildingWidthUpDown.Value;
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
            }
        }
        private void exportBuildingsButton_Click(object sender, EventArgs e) {
            SaveFileDialog eb = new SaveFileDialog();
            eb.Filter = "Buildings File (*.bld)|*.bld";
            eb.FileName = selectMapComboBox.SelectedItem.ToString();
            if (eb.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(eb.FileName))) {
                writer.Write(currentMapFile.BuildingsToByteArray());
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
                currentMapFile.buildings[i].NSBMDFile = LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, romInfo.buildingTexturesDirPath, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
            }

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
            MessageBox.Show("Buildings imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void interiorRadioButton_CheckedChanged(object sender, EventArgs e) {
            disableHandlers = true;
            int index = buildIndexComboBox.SelectedIndex;
            buildIndexComboBox.Items.Clear();

            /* Fill building models list */
            updateBuildingListComboBox(interiorbldRadioButton.Checked);
            FillBuildingsBox();

            try {
                buildIndexComboBox.SelectedIndex = index;
            } catch (ArgumentOutOfRangeException) {
                string bldType;
                if (interiorbldRadioButton.Checked)
                    bldType = "interior";
                else
                    bldType = "exterior";

                MessageBox.Show("Couldn't find " + bldType + " building #" + index + '.' +
                    "\nBuilding 0 will be loaded in its place.", "Building not found", MessageBoxButtons.OK, MessageBoxIcon.Error);

                buildIndexComboBox.SelectedIndex = 0;
                currentMapFile.buildings[buildIndexComboBox.SelectedIndex].modelID = 0;
            }

            /* Load buildings nsbmd and textures for renderer into MapFile's building objects */
            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                currentMapFile.buildings[i] = LoadBuildingModel(currentMapFile.buildings[i], interiorbldRadioButton.Checked); // Load building nsbmd
                currentMapFile.buildings[i].NSBMDFile = LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, romInfo.buildingTexturesDirPath, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
            }

            /* Render the map */
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
            disableHandlers = false;
        }
        private void removeBuildingButton_Click(object sender, EventArgs e) {
            int toRemoveListBoxID = buildingsListBox.SelectedIndex;
            if (toRemoveListBoxID > -1) {
                disableHandlers = true;

                /* Remove building object from list and the corresponding entry in the ListBox */
                
                currentMapFile.buildings.RemoveAt(toRemoveListBoxID);
                buildingsListBox.Items.RemoveAt(toRemoveListBoxID);

                FillBuildingsBox(); // Update ListBox
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);

                disableHandlers = false;

                if (buildingsListBox.Items.Count > 0) {
                    if (toRemoveListBoxID > 0) {
                        buildingsListBox.SelectedIndex = toRemoveListBoxID - 1;
                    } else {
                        buildingsListBox.SelectedIndex = 0;
                    }
                }
            }
        }
        private void xBuildUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || buildingsListBox.SelectedIndex < 0)
                return;

            var wholePart = Math.Truncate(xBuildUpDown.Value);
            var decPart = xBuildUpDown.Value - wholePart;

            if (decPart < 0)
                decPart += 1;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].xPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].xFraction = (ushort)(decPart*65535);
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void yBuildUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || buildingsListBox.SelectedIndex < 0)
                return;

            var wholePart = Math.Truncate(yBuildUpDown.Value);
            var decPart = yBuildUpDown.Value - wholePart;

            if (decPart < 0)
                decPart += 1;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].yPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].yFraction = (ushort)(decPart * 65535);
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void zBuildUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || buildingsListBox.SelectedIndex < 0)
                return;

            var wholePart = Math.Truncate(zBuildUpDown.Value);
            var decPart = zBuildUpDown.Value - wholePart;

            if (decPart < 0)
                decPart += 1;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].zPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].zFraction = (ushort)(decPart*65535);
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
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
        private void DrawCollisionGrid() {
            Bitmap mainBm = new Bitmap(608, 608);
            using (Graphics gMain = Graphics.FromImage(mainBm)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        SetCollisionPainter(currentMapFile.collisions[i, j]);

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
        private void DrawSmallCollision() {
            Bitmap smallBm = new Bitmap(100, 100);
            using (Graphics gSmall = Graphics.FromImage(smallBm)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        SetCollisionPainter(currentMapFile.collisions[i, j]);

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
        private void DrawTypeGrid() {
            Bitmap mainBm = new Bitmap(608, 608);
            using (Graphics gMain = Graphics.FromImage(mainBm)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        SetTypePainter(Convert.ToInt32(currentMapFile.types[i, j]));

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
        private void DrawSmallTypeCollision() {
            Bitmap smallBm = new Bitmap(100, 100);
            using (Graphics gSmall = Graphics.FromImage(smallBm)) {
                for (int i = 0; i < 32; i++) {
                    for (int j = 0; j < 32; j++) {
                        SetTypePainter(currentMapFile.types[i, j]);

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
        private void EditCell(int xPosition, int yPosition) {
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
        private void RestorePainter() {
            if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                collisionPainterComboBox_ResetSelection(null, null); // Restore painters to original state
            } else if (collisionTypePainterComboBox.Enabled) {
                typePainterComboBox_SelectedIndexChanged(null, null); // Restore painters to original state
            } else {
                typePainterUpDown_ValueChanged(null, null);
            }
        }
        private void SetCollisionPainter(int collisionValue) {
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
        private void SetTypePainter(int typeValue) {
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
                    textFont = new Font("Arial", 8.65f);
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
                case 0x30:
                case 0x31:
                case 0x32:
                case 0x33:
                    paintPen = new Pen(Color.FromArgb(128, Color.Red));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Red));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x3C:
                case 0x3D:
                case 0x3E:
                    paintPen = new Pen(Color.FromArgb(0x7F654321));
                    paintBrush = new SolidBrush(Color.FromArgb(0x7F654321));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x38:
                case 0x39:
                case 0x3A:
                case 0x3B:
                    paintPen = new Pen(Color.FromArgb(128, Color.Maroon));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Maroon));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
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
                case 0x4B:
                case 0x4C:
                    paintPen = new Pen(Color.FromArgb(128, Color.Sienna));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Sienna));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x5E:
                    paintPen = new Pen(Color.FromArgb(128, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
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
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x6C:
                case 0x6D:
                case 0x6E:
                case 0x6F:
                    paintPen = new Pen(Color.FromArgb(128, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0xA1:
                case 0xA2:
                case 0xA3:
                    paintPen = new Pen(Color.FromArgb(128, Color.Honeydew));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Honeydew));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0xA4:
                    paintPen = new Pen(Color.FromArgb(128, Color.Peru));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Peru));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0xA6:
                    paintPen = new Pen(Color.FromArgb(128, Color.SeaGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.SeaGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                default:
                    paintPen = new Pen(Color.FromArgb(128, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.White));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 8.65f);
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
                SetCollisionPainter(0x0);

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
            RestorePainter();
        }
        private void collisionPainterComboBox_ResetSelection(object sender, EventArgs e) {
            int collisionValue;

            if (collisionPainterComboBox.SelectedIndex == 0) {
                collisionValue = 0;
            } else if (collisionPainterComboBox.SelectedIndex == 1) {
                collisionValue = 0x80;
            } else {
                collisionValue = 1;
            }

            SetCollisionPainter(collisionValue);

            using (Graphics g = Graphics.FromImage(collisionPainterPictureBox.Image)) 
                g.Clear(Color.FromArgb(255, paintBrush.Color));

            collisionPainterPictureBox.Invalidate();
        }
        private void collisionPictureBox_Click(object sender, EventArgs e) {
            selectTypePanel.BackColor = Color.Transparent;
            typeGroupBox.Enabled = false;
            selectCollisionPanel.BackColor = Color.MidnightBlue;
            collisionGroupBox.Enabled = true;

            DrawCollisionGrid();
            RestorePainter();
        }
        private void exportMovButton_Click(object sender, EventArgs e) {
            SaveFileDialog em = new SaveFileDialog();
            em.Filter = "Permissions File (*.per)|*.per";
            em.FileName = selectMapComboBox.SelectedItem.ToString();
            if (em.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(em.FileName))) 
                writer.Write(currentMapFile.CollisionsToByteArray());

            MessageBox.Show("Permissions exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importMovButton_Click(object sender, EventArgs e) {
            OpenFileDialog ip = new OpenFileDialog();
            ip.Filter = "Permissions File (*.per)|*.per";
            if (ip.ShowDialog(this) != DialogResult.OK)
                return;

            currentMapFile.ImportPermissions(new FileStream(ip.FileName, FileMode.Open));

            DrawSmallCollision();
            DrawSmallTypeCollision();
            if (selectCollisionPanel.BackColor == Color.MidnightBlue) { 
                DrawCollisionGrid();
            } else {
                DrawTypeGrid();
            }
            RestorePainter();

            MessageBox.Show("Permissions imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void movPictureBox_Click(object sender, EventArgs e) {
            EditCell(movPictureBox.PointToClient(MousePosition).X / 19, movPictureBox.PointToClient(MousePosition).Y / 19);
        }
        private void movPictureBox_MouseMove(object sender, MouseEventArgs e) {
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left) {
                EditCell(e.Location.X / 19, e.Location.Y / 19);
            }
        }
        private void typePainterComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            string selectedType = collisionTypePainterComboBox.SelectedItem.ToString();
            updateTypeCollisions(Convert.ToInt32(selectedType.Substring(1, 2), 16));
        }
        private void typePainterUpDown_ValueChanged(object sender, EventArgs e) {
            int typeValue = (int)typePainterUpDown.Value;
            updateTypeCollisions(typeValue);
        }
        private void updateTypeCollisions(int typeValue) {
            SetTypePainter(typeValue);

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
        private void typePictureBox_Click(object sender, EventArgs e) {
            selectCollisionPanel.BackColor = Color.Transparent;
            collisionGroupBox.Enabled = false;
            selectTypePanel.BackColor = Color.MidnightBlue;
            typeGroupBox.Enabled = true;

            DrawTypeGrid();
            RestorePainter();
        }
        private void typesRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (knownTypesRadioButton.Checked) {
                typePainterUpDown.Enabled = false;
                collisionTypePainterComboBox.Enabled = true;
                typePainterComboBox_SelectedIndexChanged(null, null);
            }
        }
        private void valueTypeRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (valueTypeRadioButton.Checked) {
                collisionTypePainterComboBox.Enabled = false;
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

            if (mapTextureComboBox.SelectedIndex > 0) 
                currentMapFile.mapModel = LoadModelTextures(currentMapFile.mapModel, romInfo.mapTexturesDirPath, mapTextureComboBox.SelectedIndex - 1);
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);

            ModelSizeTXT.Text = currentMapFile.mapModelData.Length.ToString();
            MessageBox.Show("Map model imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void exportMapButton_Click(object sender, EventArgs e) {
            SaveFileDialog em = new SaveFileDialog();
            em.Filter = "NSBMD model (*.nsbmd)|*.nsbmd";
            em.FileName = selectMapComboBox.SelectedItem.ToString();
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
            if (RomInfo.gameVersion == "D" || RomInfo.gameVersion == "P")
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
            eb.FileName = selectMapComboBox.SelectedItem.ToString();
            if (eb.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(eb.FileName))) 
                writer.Write(currentMapFile.GetTerrain());

            TerrainSizeTXT.Text = currentMapFile.bdhc.Length.ToString() + "B";
            MessageBox.Show("Terrain settings exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void soundPlatesImportButton_Click(object sender, EventArgs e) {
            OpenFileDialog it = new OpenFileDialog();
            it.Filter = "BackGround Sound File (*.bgs)|*.bgs";

            if (it.ShowDialog(this) != DialogResult.OK)
                return;

            currentMapFile.ImportSoundPlates(new FileStream(it.FileName, FileMode.Open));
            MessageBox.Show("BackGround Sound data imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void soundPlatesExportButton_Click(object sender, EventArgs e) {
            SaveFileDialog eb = new SaveFileDialog();
            eb.Filter = "BackGround Sound File (*.bgs)|*.bgs";
            eb.FileName = selectMapComboBox.SelectedItem.ToString();
            if (eb.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(eb.FileName))) 
                writer.Write(currentMapFile.GetSoundPlates());

            BGSSizeTXT.Text = currentMapFile.bgs.Length.ToString() + "B";
            MessageBox.Show("BackGround Sound data exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        public EventFile currentEventFile;
        public Event selectedEvent;
        public Dictionary<uint, string> ow3DSpriteDict = new Dictionary<uint, string>();

        /* Painters to draw the matrix grid */
        public Pen eventPen;
        public Brush eventBrush;
        public Rectangle eventMatrixRectangle;
        #endregion

        #region Subroutines
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
        private void centerEventViewOnSelectedEvent_Click(object sender, EventArgs e) {
            if (selectedEvent == null) {
                MessageBox.Show("You haven't selected any event.", "Nothing to do here",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                eventMatrixXUpDown.Value = selectedEvent.xMatrixPosition;
                eventMatrixYUpDown.Value = selectedEvent.yMatrixPosition;
                Update();
            }
        }

        private void DisplayActiveEvents() {
            eventPictureBox.Image = new Bitmap(eventPictureBox.Width, eventPictureBox.Height);

            /* Draw spawnables */
            if (showSignsCheckBox.Checked) {
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
            if (showWarpsCheckBox.Checked) {
                for (int i = 0; i < currentEventFile.warps.Count; i++) {
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
            }

            /* Draw triggers */
            if (showTriggersCheckBox.Checked) {
                for (int i = 0; i < currentEventFile.triggers.Count; i++) {
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
                    short header = (short)eventMatrix.headers[(short)eventMatrixYUpDown.Value, (short)eventMatrixXUpDown.Value];
                    areaDataID = MapHeader.LoadFromARM9(header).areaDataID;
                } else areaDataID = (uint)eventAreaDataUpDown.Value;

                /* get texture file numbers from area data */
                AreaData areaData = LoadAreaData(areaDataID);

                /* Read map and building models, match them with textures and render them*/
                eventMapFile = LoadMapFile((int)mapIndex);
                eventMapFile.mapModel = LoadModelTextures(eventMapFile.mapModel, romInfo.mapTexturesDirPath, areaData.mapTileset);

                bool isInteriorMap = new bool();
                if ((RomInfo.gameVersion == "HG" || RomInfo.gameVersion == "SS")
                && areaData.areaType == 0x0)
                    isInteriorMap = true;

                for (int i = 0; i < eventMapFile.buildings.Count; i++) {
                    eventMapFile.buildings[i] = LoadBuildingModel(eventMapFile.buildings[i], isInteriorMap); // Load building nsbmd
                    eventMapFile.buildings[i].NSBMDFile = LoadModelTextures(eventMapFile.buildings[i].NSBMDFile, romInfo.buildingTexturesDirPath, areaData.buildingsTileset); // Load building textures                
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
                int archiveID = MatchOverworldIDToSpriteArchive(spriteID, romInfo.OWtablePath);
                if (archiveID == -1)
                    return (Bitmap)Properties.Resources.ResourceManager.GetObject("overworld"); // If id is -1, load bounding box
                else {
                    try {
                        FileStream stream = new FileStream(romInfo.OWSpriteDirPath + "\\" + archiveID.ToString("D4"), FileMode.Open);
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
            using (BinaryReader idReader = new BinaryReader(new FileStream(romInfo.OWtablePath, FileMode.Open))) {
                int archiveID;
                switch (RomInfo.gameVersion) {
                    case "D":
                    case "P":
                        idReader.BaseStream.Position = 0x22BCC;
                        archiveID = matchOverworldInTableDPPt(idReader, ID);
                        break;
                    case "Plat":
                        switch (RomInfo.gameLanguage) { // Go to the beginning of the overworld table
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
                    if (idFound == ID) {
                        return (int)idReader.ReadUInt16(); // If the entry is a match, stop and go to reading part
                    } else {
                        idReader.BaseStream.Position += 0x4; // If the entry is not a match, move forward
                    }
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
        private bool isEventOnCurrentMatrix(Event ev) {
            if (ev.xMatrixPosition == eventMatrixXUpDown.Value)
                if (ev.yMatrixPosition == eventMatrixYUpDown.Value)
                    return true;
            return false;
        }
        private bool isEventUnderMouse(Event ev, Point mouseTilePos) {
            if (isEventOnCurrentMatrix(ev)) {
                Point evLocalCoords = new Point(ev.xMapPosition, ev.yMapPosition);
                if (evLocalCoords.Equals(mouseTilePos))
                    return true;
            }
            return false;
        }
        #endregion
        private void addEventFileButton_Click(object sender, EventArgs e) {
            /* Add copy of event 0 to event folder */
            new EventFile(0).SaveToFileDefaultDir(selectEventComboBox.Items.Count);

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

            eventMatrix = new Matrix((int)eventMatrixUpDown.Value);
            eventMatrixXUpDown.Value = 0;
            eventMatrixYUpDown.Value = 0;
            eventMatrixXUpDown.Maximum = eventMatrix.width - 1;
            eventMatrixYUpDown.Maximum = eventMatrix.height - 1;
            DrawEventMatrix();
            MarkUsedCells();

            disableHandlers = false;
        }
        private void eventShiftLeftButton_Click(object sender, EventArgs e) {
            if (eventMatrixXUpDown.Value > 0) 
                eventMatrixXUpDown.Value -= 1;
        }
        private void eventShiftUpButton_Click(object sender, EventArgs e) {
            if (eventMatrixYUpDown.Value > 0) 
                eventMatrixYUpDown.Value -= 1;
        }
        private void eventShiftRightButton_Click(object sender, EventArgs e) {
            if (eventMatrixXUpDown.Value < eventMatrix.width - 1) 
                eventMatrixXUpDown.Value += 1;
        }
        private void eventShiftDownButton_Click(object sender, EventArgs e) {
            if (eventMatrixYUpDown.Value < eventMatrix.height - 1) 
                eventMatrixYUpDown.Value += 1;
        }
        private void eventMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;

            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            DisplayEventMap();
            DisplayActiveEvents();
        }
        private void eventMatrixYUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;

            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            DisplayEventMap();
            DisplayActiveEvents();
        }
        private void exportEventFileButton_Click(object sender, EventArgs e) {
            currentEventFile.SaveToFileExplorePath("Event File " + selectEventComboBox.SelectedIndex);
        }
        private void saveEventsButton_Click(object sender, EventArgs e) {
            currentEventFile.SaveToFileDefaultDir(selectEventComboBox.SelectedIndex);
        }
        private void importEventFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .evt file */
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Event File (*.evt)|*.evt";
            if (of.ShowDialog(this) != DialogResult.OK)
                return;

            /* Update matrix object in memory */
            string path = RomInfo.eventsDirPath + "\\" + selectEventComboBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Refresh controls */
            selectEventComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show("Events imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void removeEventFileButton_Click(object sender, EventArgs e) {
            /* Delete event file */
            File.Delete(RomInfo.eventsDirPath + "\\" + (selectEventComboBox.Items.Count - 1).ToString("D4"));

            /* Check if currently selected file is the last one, and in that case select the one before it */
            int lastIndex = selectEventComboBox.Items.Count - 1;
            if (selectEventComboBox.SelectedIndex == lastIndex) selectEventComboBox.SelectedIndex--;

            /* Remove item from ComboBox */
            selectEventComboBox.Items.RemoveAt(lastIndex);
        }
        private void selectEventComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            /* Load events data into EventFile class instance */
            currentEventFile = new EventFile(selectEventComboBox.SelectedIndex);

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
                if (showWarpsCheckBox.Checked)
                    for (int i = 0; i < currentEventFile.warps.Count; i++) {
                        Warp ev = currentEventFile.warps[i];
                        if (isEventUnderMouse(ev, mouseTilePos)) {
                            if (ev == selectedEvent) {
                                goToWarpDestination_Click(sender, e);
                                return;
                            }
                            selectedEvent = ev;
                            eventsTabControl.SelectedTab = warpsTabPage;
                            warpsListBox.SelectedIndex = i;
                            DisplayActiveEvents();
                            return;
                        }
                    }
                if (showSignsCheckBox.Checked)
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
                if (showOwsCheckBox.Checked)
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
                            goToWarpDestination_Click(sender, e);
                            return;
                        }
                    }
                }
            }
        }
        #region Spawnables Editor
        private void addSpawnableButton_Click(object sender, EventArgs e) {
            currentEventFile.spawnables.Add(new Spawnable((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));
            spawnablesListBox.Items.Add("Spawnable " + (currentEventFile.spawnables.Count - 1).ToString());
            spawnablesListBox.SelectedIndex = currentEventFile.spawnables.Count - 1;
        }
        private void removeSpawnableButton_Click(object sender, EventArgs e) {
            if (spawnablesListBox.SelectedIndex < 0) {
                return;
            }
                    
            disableHandlers = true;

            /* Remove trigger object from list and the corresponding entry in the ListBox */
            int spawnableNumber = spawnablesListBox.SelectedIndex;
            currentEventFile.spawnables.RemoveAt(spawnableNumber);
            spawnablesListBox.Items.RemoveAt(spawnableNumber);

            FillSpawnablesBox(); // Update ListBox

            disableHandlers = false;

            if (spawnableNumber > 0) { 
                spawnablesListBox.SelectedIndex = spawnableNumber - 1;
            } else {
                DisplayActiveEvents();
            }
        }
        private void duplicateSpawnableButton_Click(object sender, EventArgs e) {
            if (spawnablesListBox.SelectedIndex < 0) {
                return;
            }

            currentEventFile.spawnables.Add(new Spawnable((Spawnable)selectedEvent));
            spawnablesListBox.Items.Add("Spawnable " + (currentEventFile.spawnables.Count - 1).ToString());
            spawnablesListBox.SelectedIndex = currentEventFile.spawnables.Count - 1;
        }
        private void spawnablesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;
            disableHandlers = true;

            /* Set Event */
            selectedEvent = currentEventFile.spawnables[spawnablesListBox.SelectedIndex];

            /* Update Controls */
            spawnableDirComboBox.SelectedIndex = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].dir;
            spawnableTypeComboBox.SelectedIndex = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].type;

            spawnableScriptUpDown.Value = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].scriptNumber;
            spawnableMapXUpDown.Value = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].xMapPosition;
            spawnableMapYUpDown.Value = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].yMapPosition;
            spawnableUpDown.Value = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].zPosition;
            spawnableMatrixXUpDown.Value = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].xMatrixPosition;
            spawnableMatrixYUpDown.Value = currentEventFile.spawnables[spawnablesListBox.SelectedIndex].yMatrixPosition;

            DisplayActiveEvents();
            disableHandlers = false;
        }
        private void spawnableMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].xMatrixPosition = (ushort)spawnableMatrixXUpDown.Value;
            DisplayActiveEvents();
        }
        private void spawnableMatrixYUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].yMatrixPosition = (ushort)spawnableMatrixYUpDown.Value;
            DisplayActiveEvents();
        }
        private void spawnableScriptUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;
            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].scriptNumber = (ushort)spawnableScriptUpDown.Value;
        }
        private void spawnableMapXUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].xMapPosition = (short)spawnableMapXUpDown.Value;
            DisplayActiveEvents();
        }
        private void spawnableMapYUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].yMapPosition = (short)spawnableMapYUpDown.Value;
            DisplayActiveEvents();
        }
        private void spawnableZUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].zPosition = (short)spawnableUpDown.Value;
            DisplayActiveEvents();
        }
        private void spawnableDirComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].dir = (ushort)spawnableDirComboBox.SelectedIndex;
        }
        private void spawnableTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (spawnablesListBox.SelectedIndex < 0)
                return;

            if (spawnableTypeComboBox.SelectedIndex == Spawnable.TYPE_HIDDENITEM) {
                spawnableDirComboBox.Enabled = false;
            } else {
                spawnableDirComboBox.Enabled = true;
            }

            if (disableHandlers)
                return;

            currentEventFile.spawnables[spawnablesListBox.SelectedIndex].type = (ushort)spawnableTypeComboBox.SelectedIndex;
        }
        #endregion

        #region Overworlds Editor
        private void addOverworldButton_Click(object sender, EventArgs e) {
            currentEventFile.overworlds.Add(new Overworld(currentEventFile.overworlds.Count, (int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));
            overworldsListBox.Items.Add("Overworld " + (currentEventFile.overworlds.Count - 1).ToString());
            overworldsListBox.SelectedIndex = currentEventFile.overworlds.Count - 1;
        }        
        private void removeOverworldButton_Click(object sender, EventArgs e) {
            if (overworldsListBox.SelectedIndex < 0) {
                return;
            }

            disableHandlers = true;

            /* Remove overworld object from list and the corresponding entry in the ListBox */
            int owNumber = overworldsListBox.SelectedIndex;
            currentEventFile.overworlds.RemoveAt(owNumber);
            overworldsListBox.Items.RemoveAt(owNumber);

            FillOverworldsBox(); // Update ListBox
            disableHandlers = false;

            if (owNumber > 0) { 
                overworldsListBox.SelectedIndex = owNumber - 1;
            } else {
                DisplayActiveEvents();
            }
        }
        private void duplicateOverworldsButton_Click(object sender, EventArgs e) {
            if (overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEventFile.overworlds.Add(new Overworld((Overworld)selectedEvent));
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
        public static bool ScanScriptsCheckStandardizedItemNumbers() {
            ScriptFile itemScript = new ScriptFile(RomInfo.itemScriptFileNumber);

            for (ushort i = 0; i < itemScript.scripts.Count - 1; i++) {
                if (BitConverter.ToUInt16(itemScript.scripts[i].commands[0].parameterList[1], 0) != i || BitConverter.ToUInt16(itemScript.scripts[i].commands[1].parameterList[1], 0) != 1) {
                    return false;
                }
            }
            return true;
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
                if (isItemRadioButton.Checked) {
                    owItemComboBox.Enabled = true;
                    owItemLabel.Enabled = true;

                    currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(owScriptNumericUpDown.Value = 7000 + owItemComboBox.SelectedIndex);
                }
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

            if (normalRadioButton.Checked) {
                owScriptNumericUpDown.Value = 0;
            }

            /* Set trainer flag to false and correct script number */
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].type = 0x0;
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)owScriptNumericUpDown.Value;
        }
        private void owItemComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;

            owScriptNumericUpDown.Value = currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(7000 + owItemComboBox.SelectedIndex);
        }
        private void overworldsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            #region Disable Events for fast execution
            if (disableHandlers) 
                return;
            disableHandlers = true;
            #endregion

            int index = overworldsListBox.SelectedIndex;
            if (index > -1) {
                try {
                    selectedEvent = currentEventFile.overworlds[index];

                    /* Sprite index and image controls */
                    owSpriteComboBox.SelectedIndex = MatchOverworldIDToSpriteArchive(currentEventFile.overworlds[index].spriteID, romInfo.OWtablePath);
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
            
            currentEventFile.overworlds[overworldsListBox.SelectedIndex].spriteID = (ushort)owSpriteComboBox.SelectedIndex;
            owSpritePictureBox.BackgroundImage = GetOverworldImage(currentEventFile.overworlds[overworldsListBox.SelectedIndex].spriteID, currentEventFile.overworlds[overworldsListBox.SelectedIndex].orientation);
            DisplayActiveEvents();
            owSpritePictureBox.Invalidate();
            
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

            if (owPartnerTrainerCheckBox.Checked) 
                owScriptNumericUpDown.Value = currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(4999 + owTrainerComboBox.SelectedIndex);
            else
                owScriptNumericUpDown.Value = currentEventFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(2999 + owTrainerComboBox.SelectedIndex);
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
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;

            currentEventFile.overworlds[overworldsListBox.SelectedIndex].xMatrixPosition = (ushort)owXMatrixUpDown.Value;
            eventMatrixPictureBox.Image = new Bitmap(eventMatrixPictureBox.Width, eventMatrixPictureBox.Height);
            DrawEventMatrix(); // Redraw matrix to eliminate old used cells
            MarkUsedCells(); // Mark new used cells
            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            DisplayActiveEvents();
        }
        private void owYMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0)
                return;

            currentEventFile.overworlds[overworldsListBox.SelectedIndex].yMatrixPosition = (ushort)owYMatrixUpDown.Value;
            eventMatrixPictureBox.Image = new Bitmap(eventMatrixPictureBox.Width, eventMatrixPictureBox.Height);
            DrawEventMatrix();
            MarkUsedCells();
            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            DisplayActiveEvents();
        }
        
        #endregion

        #region Warps Editor
        private void addWarpButton_Click(object sender, EventArgs e) {
            currentEventFile.warps.Add(new Warp((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));
            warpsListBox.Items.Add("Warp " + (currentEventFile.warps.Count - 1).ToString());
            warpsListBox.SelectedIndex = currentEventFile.warps.Count - 1;
        }
        private void removeWarpButton_Click(object sender, EventArgs e) {
            if (warpsListBox.SelectedIndex < 0) {
                return;
            }

            disableHandlers = true;

            /* Remove warp object from list and the corresponding entry in the ListBox */
            int warpNumber = warpsListBox.SelectedIndex;
            currentEventFile.warps.RemoveAt(warpNumber);
            warpsListBox.Items.RemoveAt(warpNumber);

            FillWarpsBox(); // Update ListBox

            disableHandlers = false;

            if (warpNumber > 0) { 
                warpsListBox.SelectedIndex = warpNumber - 1;
            } else {
                DisplayActiveEvents();
            }
        }
        private void duplicateWarpsButton_Click(object sender, EventArgs e) {
            if (warpsListBox.SelectedIndex < 0) {
                return;
            }

            currentEventFile.warps.Add(new Warp((Warp)selectedEvent));
            warpsListBox.Items.Add("Warp " + (currentEventFile.warps.Count - 1).ToString());
            warpsListBox.SelectedIndex = currentEventFile.warps.Count - 1;
        }
        private void warpAnchorUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0)
                return;
            currentEventFile.warps[warpsListBox.SelectedIndex].anchor = (ushort)warpAnchorUpDown.Value;
        }
        private void warpHeaderUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0)
                return;
            currentEventFile.warps[warpsListBox.SelectedIndex].header = (ushort)warpHeaderUpDown.Value;
        }
        private void warpsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0)
                return;
            disableHandlers = true;

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
            if (disableHandlers || warpsListBox.SelectedIndex < 0)
                return;

            currentEventFile.warps[warpsListBox.SelectedIndex].yMapPosition = (short)warpYMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void warpZUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0) 
                return;

            currentEventFile.warps[warpsListBox.SelectedIndex].zPosition = (short)warpZUpDown.Value;
            DisplayActiveEvents();
        }
        private void goToWarpDestination_Click(object sender, EventArgs e) {
            int destAnchor = (int)warpAnchorUpDown.Value;
            short destHeader = (short)warpHeaderUpDown.Value;
            ushort destEventID = MapHeader.LoadFromARM9(destHeader).eventFileID;
            EventFile destEvent = new EventFile(destEventID);

            if (destEvent.warps.Count < destAnchor + 1) {
                DialogResult d = MessageBox.Show("The selected warp's destination anchor doesn't exist.\n" +
                    "Do you want to open the destination map anyway?", "Warp is not connected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (d == DialogResult.No)
                    return;
                else {
                    eventMatrixUpDown.Value = MapHeader.LoadFromARM9((short)warpHeaderUpDown.Value).matrixID;
                    eventAreaDataUpDown.Value = MapHeader.LoadFromARM9((short)warpHeaderUpDown.Value).areaDataID;
                    selectEventComboBox.SelectedIndex = destEventID;
                    centerEventviewOnEntities();
                    return;
                }
            }
            eventMatrixUpDown.Value = MapHeader.LoadFromARM9((short)warpHeaderUpDown.Value).matrixID;
            eventAreaDataUpDown.Value = MapHeader.LoadFromARM9((short)warpHeaderUpDown.Value).areaDataID;
            selectEventComboBox.SelectedIndex = destEventID;
            warpsListBox.SelectedIndex = destAnchor;
            centerEventViewOnSelectedEvent_Click(sender, e);
        }
        #endregion

        #region Triggers Editor
        private void addTriggerButton_Click(object sender, EventArgs e) {
            currentEventFile.triggers.Add(new Trigger((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));
            triggersListBox.Items.Add("Trigger " + (currentEventFile.triggers.Count - 1).ToString());
            triggersListBox.SelectedIndex = currentEventFile.triggers.Count - 1;
        }
        private void removeTriggerButton_Click(object sender, EventArgs e) {
            if (triggersListBox.SelectedIndex < 0) {
                return;
            }

            disableHandlers = true;

            /* Remove trigger object from list and the corresponding entry in the ListBox */
            int triggerNumber = triggersListBox.SelectedIndex;
            currentEventFile.triggers.RemoveAt(triggerNumber);
            triggersListBox.Items.RemoveAt(triggerNumber);

            FillTriggersBox(); // Update ListBox

            disableHandlers = false;

            if (triggerNumber > 0) {
                triggersListBox.SelectedIndex = triggerNumber - 1;
            } else {
                DisplayActiveEvents();
            }
        }
        private void duplicateTriggersButton_Click(object sender, EventArgs e) {
            if (triggersListBox.SelectedIndex < 0) {
                return;
            }

            currentEventFile.triggers.Add(new Trigger((Trigger)selectedEvent));
            triggersListBox.Items.Add("Trigger " + (currentEventFile.triggers.Count - 1).ToString());
            triggersListBox.SelectedIndex = currentEventFile.triggers.Count - 1;
        }
        private void triggersListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;
            disableHandlers = true;

            selectedEvent = currentEventFile.triggers[triggersListBox.SelectedIndex];

            triggerScriptUpDown.Value = currentEventFile.triggers[triggersListBox.SelectedIndex].scriptNumber;
            triggerVariableWatchedUpDown.Value = currentEventFile.triggers[triggersListBox.SelectedIndex].variableWatched;
            expectedVarValueTriggerUpDown.Value = currentEventFile.triggers[triggersListBox.SelectedIndex].expectedVarValue;

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
        private void triggerVariableWatchedUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;
            currentEventFile.triggers[triggersListBox.SelectedIndex].variableWatched = (ushort)triggerVariableWatchedUpDown.Value;
        }
        private void expectedVarValueTriggerUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].expectedVarValue = (ushort)expectedVarValueTriggerUpDown.Value;
        }
        private void triggerScriptUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0) 
                return;
            currentEventFile.triggers[triggersListBox.SelectedIndex].scriptNumber = (ushort)triggerScriptUpDown.Value;
        }
        private void triggerXMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0) 
                return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].xMapPosition = (short)triggerXMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerYMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0) 
                return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].yMapPosition = (short)triggerYMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerZUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0) 
                return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].zPosition = (short)triggerZUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerXMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].xMatrixPosition = (ushort)triggerXMatrixUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerYMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].yMatrixPosition = (ushort)triggerYMatrixUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerWidthUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].width = (ushort)triggerWidthUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerLengthUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEventFile.triggers[triggersListBox.SelectedIndex].length = (ushort)triggerLengthUpDown.Value;
            DisplayActiveEvents();
        }
        #endregion
        #endregion

        #region Script Editor

        #region Variables
        public ScriptFile currentScriptFile;
        public RichTextBox currentScriptBox;
        public RichTextBox currentLineNumbersBox;
        #endregion

        #region Helper Methods
        private void SetCurrentRTBfromSelectedScriptTab(object sender, EventArgs e) {
            if (scriptEditorTabControl.SelectedIndex == 0) {
                currentScriptBox = scriptTextBox;
                currentLineNumbersBox = LineNumberTextBoxScript;
            } else if (scriptEditorTabControl.SelectedIndex == 1) {
                currentScriptBox = functionTextBox;
                currentLineNumbersBox = LineNumberTextBoxFunc;
            } else if (scriptEditorTabControl.SelectedIndex == 2) {
                currentScriptBox = movementTextBox;
                currentLineNumbersBox = LineNumberTextBoxMov;
            } else {
                currentScriptBox = null;
                currentLineNumbersBox = null;
            }
        }
        #endregion
        #region LineNumbers
        public void UpdateLineNumbers(RichTextBox mainbox, RichTextBox numberBox) {
            if (disableHandlers)
                return;

            // get line indices
            int indexFirstCharDisplayed = mainbox.GetCharIndexFromPosition(new Point(0, mainbox.Font.Height/2));
            int firstLine = mainbox.GetLineFromCharIndex(indexFirstCharDisplayed);

            int indexLastCharDisplayed = mainbox.GetCharIndexFromPosition(new Point(0, mainbox.Height + mainbox.Font.Height / 2));
            int lastLine = mainbox.GetLineFromCharIndex(indexLastCharDisplayed);

            // align line numbers to center
            numberBox.SelectionAlignment = HorizontalAlignment.Center;

            // set LineNumberTextBox text to null & width to GetWidth() function value    
            numberBox.Text = "";
            numberBox.Width = CalculateLineNumbersWidth(mainbox);

            // now add each line number to LineNumberTextBox upto last line    
            for (int i = firstLine; i <= lastLine+1; i++) {
                numberBox.Text += i+1 + "\n";
            }
            numberBox.Invalidate();
        }   
        public int CalculateLineNumbersWidth(RichTextBox mainbox) {
            int w = 25;
            // get total lines of functionTextBox    
            int line = mainbox.Lines.Length;

            if (line <= 99) {
                w = 20 + (int)mainbox.Font.Size;
            } else if (line <= 999) {
                w = 30 + (int)mainbox.Font.Size;
            } else {
                w = 50 + (int)mainbox.Font.Size;
            }

            return w;
        }
        private void updateCurrentBoxLineNumbers(object sender, EventArgs e) {
            UpdateLineNumbers(currentScriptBox, currentLineNumbersBox);
        }
        #endregion
        private void addScriptFileButton_Click(object sender, EventArgs e) {
            /* Add new event file to event folder */
            string scriptFilePath = RomInfo.scriptDirPath + "\\" + selectScriptFileComboBox.Items.Count.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(scriptFilePath, FileMode.Create))) 
                writer.Write(new ScriptFile(0).ToByteArray());

            /* Update ComboBox and select new file */
            selectScriptFileComboBox.Items.Add("Script File " + selectScriptFileComboBox.Items.Count);
            selectScriptFileComboBox.SelectedIndex = selectScriptFileComboBox.Items.Count - 1;
        }
        private void exportScriptFileButton_Click(object sender, EventArgs e) {
            exportScriptFile();
        }
        private void exportScriptFile() {
            currentScriptFile.scripts.Clear();
            currentScriptFile.functions.Clear();
            currentScriptFile.movements.Clear();

            populateScriptCommands(currentScriptFile);
            populateFunctionCommands(currentScriptFile);
            populateMovementCommands(currentScriptFile);

            currentScriptFile.SaveToFileExplorePath("Script File " + selectScriptFileComboBox.SelectedIndex);
        }
        private void saveScriptFileButton_Click(object sender, EventArgs e) {
            /* Create new script objects */
            currentScriptFile.scripts.Clear();
            currentScriptFile.functions.Clear();
            currentScriptFile.movements.Clear();

            populateScriptCommands(currentScriptFile);
            populateFunctionCommands(currentScriptFile);
            populateMovementCommands(currentScriptFile);

            /* Write new scripts to file */
            currentScriptFile.SaveToFileDefaultDir(selectScriptFileComboBox.SelectedIndex);
        }
        private void populateScriptCommands(ScriptFile scrFile) {
            for (int i = 0; i < scriptTextBox.Lines.Length; i++) {
                if (scriptTextBox.Lines[i].Contains('@')) { // Move on until script header is found
                    i++;
                    while (scriptTextBox.Lines[i].Length == 0)
                        i++; //Skip all empty lines 

                    if (scriptTextBox.Lines[i].Contains("UseScript")) {
                        int scriptNumber = Int16.Parse(scriptTextBox.Lines[i].Substring(1 + scriptTextBox.Lines[i].IndexOf('#')));
                        scrFile.scripts.Add(new Script(useScript: scriptNumber));
                    } else {
                        /* Read script commands */

                        List<ScriptCommand> cmdList = new List<ScriptCommand>();
                        while (scriptTextBox.Lines[i] != "End" && !scriptTextBox.Lines[i].Contains("Jump Function") && i < scriptTextBox.Lines.Length - 1) {
                            Console.WriteLine("Script line " + (i + 1).ToString());
                            ScriptCommand cmd = new ScriptCommand(scriptTextBox.Lines[i]);
                            Console.WriteLine("----" + cmd + "----");
                            cmdList.Add(cmd);
                            i++;
                        }
                        cmdList.Add(new ScriptCommand(scriptTextBox.Lines[i])); // Add end or jump/call command
                        scrFile.scripts.Add(new Script(commandList: cmdList));
                    }
                }
            }
        }
        private void populateFunctionCommands(ScriptFile scrFile) {
            for (int i = 0; i < functionTextBox.Lines.Length; i++) {
                if (functionTextBox.Lines[i].Contains('@')) { // Move on until function header is found
                    i++;
                    while (functionTextBox.Lines[i].Length == 0)
                        i++; //Skip all empty lines 

                    /* Read function commands */
                    if (functionTextBox.Lines[i].Contains("UseScript")) {
                        int scriptNumber = Int16.Parse(functionTextBox.Lines[i].Substring(1 + functionTextBox.Lines[i].IndexOf('#')));
                        scrFile.functions.Add(new Script(useScript: scriptNumber));
                    } else {
                        List<ScriptCommand> cmdList = new List<ScriptCommand>();

                        while (functionTextBox.Lines[i] != "End" && !functionTextBox.Lines[i].Contains("Return") && !functionTextBox.Lines[i].Contains("Jump F")) {
                            cmdList.Add(new ScriptCommand(functionTextBox.Lines[i]));
                            i++;
                        }
                        cmdList.Add(new ScriptCommand(functionTextBox.Lines[i])); // Add end command
                        scrFile.functions.Add(new Script(commandList: cmdList));
                    }
                }
            }
        }
        private void populateMovementCommands(ScriptFile scrFile) {
            for (int i = 0; i < movementTextBox.Lines.Length; i++) {
                if (movementTextBox.Lines[i].Contains('@')) {  // Move on until script header is found
                    i++;
                    while (movementTextBox.Lines[i].Length == 0)
                        i++; //Skip all empty lines 

                    List<ScriptCommand> cmdList = new List<ScriptCommand>();
                    /* Read script commands */
                    while (movementTextBox.Lines[i] != "End") {
                        cmdList.Add(new ScriptCommand(movementTextBox.Lines[i], true));
                        i++;
                    }
                    cmdList.Add(new ScriptCommand(movementTextBox.Lines[i], true)); // Add end command

                    scrFile.movements.Add(new Script(commandList: cmdList));
                }
            }
        }
        private void importScriptFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .scr file */
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Script File (*.scr)|*.scr";
            if (of.ShowDialog(this) != DialogResult.OK) 
                return;

            /* Update scriptFile object in memory */
            string path = RomInfo.scriptDirPath + "\\" + selectScriptFileComboBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Refresh controls */
            selectScriptFileComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show("Scripts imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            File.Delete(RomInfo.scriptDirPath + "\\" + (selectScriptFileComboBox.Items.Count - 1).ToString("D4"));

            /* Check if currently selected file is the last one, and in that case select the one before it */
            int lastIndex = selectScriptFileComboBox.Items.Count - 1;
            if (selectScriptFileComboBox.SelectedIndex == lastIndex)
                selectScriptFileComboBox.SelectedIndex--;

            /* Remove item from ComboBox */
            selectScriptFileComboBox.Items.RemoveAt(lastIndex);
        }
        private void searchInScriptsButton_Click(object sender, EventArgs e) {
            if (searchInScriptsTextBox.Text == "")
                return;

            searchInScriptsResultListBox.Items.Clear();
            string searchString = searchInScriptsTextBox.Text;
            searchProgressBar.Maximum = selectScriptFileComboBox.Items.Count;

            string resultsBuffer = "";
            List<string> results = new List<string>();
            for (int i = 0; i < selectScriptFileComboBox.Items.Count; i++) {
                try {
                    Console.WriteLine("Attempting to load script " + i);
                    ScriptFile file = new ScriptFile(i);

                    if (scriptSearchCaseSensitiveCheckBox.Checked) {
                        results.AddRange(SearchInScripts(i, file.scripts, "Script", (string s) => s.Contains(searchString)));
                        results.AddRange(SearchInScripts(i, file.functions, "Function", (string s) => s.Contains(searchString)));
                    } else {
                        results.AddRange(SearchInScripts(i, file.scripts, "Script", (string s) => s.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0) );
                        results.AddRange(SearchInScripts(i, file.functions, "Function", (string s) => s.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0) );
                    }
                } catch { }
                searchProgressBar.Value = i;
            }
            searchProgressBar.Value = 0;
            searchInScriptsResultListBox.Items.AddRange(results.ToArray());
        }

        private List<string> SearchInScripts(int fileID, List<Script> ls, string entryType, Func<string, bool> criteria) {
            List<string> results = new List<string>();

            for (int j = 0; j < ls.Count; j++) {
                foreach (ScriptCommand cur in ls[j].commands) {
                    if (criteria(cur.cmdName))
                        results.Add("File " + fileID + " - " + entryType + " " + (j + 1) + ": " + cur.cmdName + Environment.NewLine);
                }
            }
            return results;
        }

        private void searchInScripts_GoToEntryResult(object sender, MouseEventArgs e) {
            if (searchInScriptsResultListBox.SelectedIndex < 0)
                return;

            string[] split = searchInScriptsResultListBox.SelectedItem.ToString().Split();
            selectScriptFileComboBox.SelectedIndex = int.Parse(split[1]);

            string cmdSearched = null;
            for (int i = 5; i < split.Length; i++) {
                cmdSearched += split[i] + " ";
            }
            cmdSearched = cmdSearched.TrimEnd();

            if (split[3].StartsWith("Script")) {
                if (scriptEditorTabControl.SelectedIndex != 0)
                    scriptEditorTabControl.SelectedIndex = 0;
                int keywordPos = scriptTextBox.Find("@Script_#" + split[4].Replace(":", ""), RichTextBoxFinds.MatchCase);
                TXTBoxScrollToResult(scriptTextBox, cmdSearched, keywordPos);
            } else if (split[3].StartsWith("Function")) {
                if (scriptEditorTabControl.SelectedIndex != 1)
                    scriptEditorTabControl.SelectedIndex = 1;
                int keywordPos = functionTextBox.Find("@Function_#" + split[4].Replace(":", ""), RichTextBoxFinds.MatchCase);
                TXTBoxScrollToResult(functionTextBox, cmdSearched, keywordPos);
            }
        }

        private void TXTBoxScrollToResult(RichTextBox tb, string cmdSearched, int after) {
            int cmdPos = tb.Find(cmdSearched, after, RichTextBoxFinds.MatchCase);
            try {
                tb.SelectionStart = cmdPos - 120;
            } catch (ArgumentOutOfRangeException) {
                tb.SelectionStart = 0;
            }
            tb.ScrollToCaret();

            tb.SelectionStart = cmdPos;
            tb.SelectionLength = cmdSearched.Length;
            tb.Focus();
        }
        private void searchInScriptsResultListBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                searchInScripts_GoToEntryResult(null, null);
            }
        }
        private void searchInScriptsTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                searchInScriptsButton_Click(null, null);
            }
        }
        private void selectScriptFileComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            /* clear controls */
            currentScriptFile = new ScriptFile(selectScriptFileComboBox.SelectedIndex); // Load script file
            
            scriptTextBox.Clear();
            functionTextBox.Clear();
            movementTextBox.Clear();

            if (currentScriptFile.isLevelScript) {
                scriptTextBox.Text += "Level script files are currently not supported.\nYou can use AdAstra's Level Scripts Editor.";
                functionTextBox.Enabled = false;
                movementTextBox.Enabled = false;
            } else {
                functionTextBox.Enabled = true;
                movementTextBox.Enabled = true;

                /* Add scripts */
                disableHandlers = true;

                string buffer = "";
                for (int i = 0; i < currentScriptFile.scripts.Count; i++) {
                    Script currentScript = currentScriptFile.scripts[i];

                    /* Write header */
                    string scrHeader = "----- " + "@Script_#" + (i + 1) + " -----" + Environment.NewLine;
                    buffer += scrHeader;
                    buffer += Environment.NewLine;

                    /* If current script is identical to another, print UseScript instead of commands */
                    if (currentScript.useScript < 0) {
                        for (int j = 0; j < currentScript.commands.Count; j++)
                            buffer += currentScript.commands[j].cmdName + Environment.NewLine;
                    } else {
                        buffer += ("UseScript_#" + currentScript.useScript + Environment.NewLine);
                    }
                }
                scriptTextBox.AppendText(buffer + Environment.NewLine, Color.FromArgb(0,140,0));
                buffer = "";

                /* Add functions */
                for (int i = 0; i < currentScriptFile.functions.Count; i++) {
                    Script currentFunction = currentScriptFile.functions[i];

                    /* Write Heaader */
                    string funcHeader = "----- " + "@Function_#" + (i + 1) + " -----" + Environment.NewLine;
                    buffer += funcHeader;
                    buffer += Environment.NewLine;

                    /* If current function is identical to a script, print UseScript instead of commands */
                    if (currentFunction.useScript < 0) {
                        for (int j = 0; j < currentFunction.commands.Count; j++)
                            buffer += currentFunction.commands[j].cmdName + Environment.NewLine;
                    } else {
                        buffer += ("UseScript_#" + currentFunction.useScript + Environment.NewLine);
                    }
                    
                }
                functionTextBox.AppendText(buffer + Environment.NewLine, Color.Blue);
                buffer = "";

                /* Add movements */
                for (int i = 0; i < currentScriptFile.movements.Count; i++) {
                    Script currentMovement = currentScriptFile.movements[i];

                    string movHeader = "----- " + "@Action_#" + (i + 1) + " -----" + Environment.NewLine;
                    buffer += movHeader;
                    buffer += Environment.NewLine;
                    for (int j = 0; j < currentMovement.commands.Count; j++)
                        buffer += currentMovement.commands[j].cmdName + Environment.NewLine;
                }
                movementTextBox.AppendText(buffer + Environment.NewLine, Color.FromArgb(192, 40, 40));
                buffer = "";
            }
            
            statusLabel.Text = "Ready";
            disableHandlers = false;
            UpdateLineNumbers(scriptTextBox, LineNumberTextBoxScript);
            UpdateLineNumbers(functionTextBox, LineNumberTextBoxFunc);
            UpdateLineNumbers(movementTextBox, LineNumberTextBoxMov);
        }

        #region Script Macros
        private void setflagButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert flag number (hex):", "hex")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nSetFlag 0x" + ((int)f.inputValUpDown.Value).ToString("X4"));
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void clearflagButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert flag number (hex):", "hex")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nClearFlag 0x" + ((int)f.inputValUpDown.Value).ToString("X4"));
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void callFunctionButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Number of Function to call (decimal):", "Decimal")) {
                f.ShowDialog();
                if (f.okSelected) {
                    scriptsTabPage.Text = scriptTextBox.Text.Insert(scriptTextBox.SelectionStart, "\nCall Function_#" + ((int)f.inputValUpDown.Value).ToString());
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void jumpToFuncButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Number of Function to jump to (decimal):", "Decimal")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nJump Function_#" + ((int)f.inputValUpDown.Value).ToString());
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }   
        }
        private void setvarButton_Click(object sender, EventArgs e) {
            //TODO - ASK FOR VAR VALUE
            string cmd = "";
            using (InsertValueDialog f = new InsertValueDialog("Insert variable number (hex):", "hex")) {
                f.ShowDialog();
                if (!f.okSelected) {
                    return;
                }
                cmd += "\nSetVar 0x" + ((int)f.inputValUpDown.Value).ToString("X4");

                f.ShowDialog();
                if (!f.okSelected) {
                    return;
                }
                cmd += "\nSetVar 0x" + ((int)f.inputValUpDown.Value).ToString("X4");

                currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, cmd);
                updateCurrentBoxLineNumbers(null, null);
                currentScriptBox.ScrollToCaret();
            }
        }
        private void messageButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert message number (decimal):", "dec")) {
                f.ShowDialog();
                if (f.okSelected) {
                    String cmd = "\nPlayFanfare 0x5DC" + "\nLockAll" + "\nFacePlayer" +
                        "\nMessage 0x" + ((int)f.inputValUpDown.Value).ToString("X") +
                        "\nWaitButton" + "\nCloseMessage" + "\nReleaseAll";
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, cmd);

                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void playCryButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert cry number (hex):", "hex")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nPlayCry 0x" + ((int)f.inputValUpDown.Value).ToString("X4") + " 0x1");

                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void routeSignButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void townSignButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void greySignButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void tipsSignButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void trainerBattleButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }

        private void playSoundButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert sound ID (hex):", "hex")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nPlaySound 0x" + ((int)f.inputValUpDown.Value).ToString("X4"));
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void waitSoundButton_Click(object sender, EventArgs e) {
            currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nWaitSound");
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void switchMusicButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void restartMusicButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void waitKeyPressButton_Click(object sender, EventArgs e) {
            currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nWaitButton");
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void closeMessageButton_Click(object sender, EventArgs e) {
            currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nCloseMessage");
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void wildBattleButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void legendaryBattleButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void checkItemButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void takePokémonButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void checkMoneyButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void givePokédexButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void giveNationalDexButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void giveShoesButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void givePokégearButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void checkBadgeButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void checkPokemonButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void LockCameraButton_Click(object sender, EventArgs e) {
            currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nCheckPlayerPosition 0x8004 0x8005\n" +
                "LockCam 0x8004 0x8005\n");
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void MoveCameraButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert movement ID to apply (decimal):", "dec")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nMovement Cam Action_#\n" + f.inputValUpDown.Value);
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void resetScreenButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void fadeScreenButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void applyMovementButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void setOwPositionButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void warpButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void followHeroButton_Click(object sender, EventArgs e) {
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        #endregion

        #region Overworlds
        private void giveItemButton_Click(object sender, EventArgs e) {
            using (GiveItemDialog f = new GiveItemDialog(GetItemNames())) {
                f.ShowDialog();
                if (f.okSelected) {
                    string firstLine = "SetVar 0x8004 0x" + f.itemComboBox.SelectedIndex.ToString("X");
                    string secondLine = "SetVar 0x8005 0x" + ((int)f.quantityNumericUpDown.Value).ToString("X");
                    string thirdLine = "CallStandard 0x7FC";

                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, firstLine + "\r" + secondLine + "\r" + thirdLine);
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void lockallButton_Click(object sender, EventArgs e) {
            currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nLockAll");
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void releaseallButton_Click(object sender, EventArgs e) {
            currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nReleaseAll");
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void lockButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("ID of the overworld to lock:", "dec")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nLock" + " " + "Overworld_#" + ((int)f.inputValUpDown.Value).ToString("D"));
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void releaseButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("ID of the overworld to release:", "dec")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nRelease" + " " + "Overworld_#" + ((int)f.inputValUpDown.Value).ToString("D"));
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void waitmovementButton_Click(object sender, EventArgs e) {
            currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "WaitMovement");
            updateCurrentBoxLineNumbers(null, null);
            currentScriptBox.ScrollToCaret();
        }
        private void addpeopleButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("ID of the Overworld to add:", "dec")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nAddOW" + " " + "Overworld_#" + ((int)f.inputValUpDown.Value).ToString("D"));
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void removepeopleButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("ID of the Overworld to remove:", "dec")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nRemoveOW" + " " + "Overworld_#" + ((int)f.inputValUpDown.Value).ToString("D"));
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        #endregion

        #region Give/Take
        private void givePokémonButton_Click(object sender, EventArgs e) {
            using (GivePokémonDialog f = new GivePokémonDialog(GetPokémonNames(), GetItemNames(), GetAttackNames())) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, f.command);
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void giveMoneyButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert money amount:", "dec")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nGiveMoney" + " " + "0x" + ((int)f.inputValUpDown.Value).ToString("X"));
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void takeMoneyButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert money amount:", "dec")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nTakeMoney" + " " + "0x" + ((int)f.inputValUpDown.Value).ToString("X"));
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
        }
        private void takeItemButton_Click(object sender, EventArgs e) {
            using (GiveItemDialog f = new GiveItemDialog(GetItemNames())) {
                f.ShowDialog();
                if (f.okSelected) {
                    string item = f.itemComboBox.SelectedIndex.ToString("X");
                    string quantity = ((int)f.quantityNumericUpDown.Value).ToString("X");

                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nTakeItem" + " " + "0x" + item + " " + "0x" + quantity + " " + "0x800C");
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }

            }
        }
        private void giveBadgeButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert badge number:", "dec")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nEnableBadge 0x" + ((int)f.inputValUpDown.Value).ToString("X"));
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
            
        }
        private void takeBadgeButton_Click(object sender, EventArgs e) {
            using (InsertValueDialog f = new InsertValueDialog("Insert badge number:", "dec")) {
                f.ShowDialog();
                if (f.okSelected) {
                    currentScriptBox.Text = currentScriptBox.Text.Insert(currentScriptBox.SelectionStart, "\nDisableBadge 0x" + ((int)f.inputValUpDown.Value).ToString("X"));
                    updateCurrentBoxLineNumbers(null, null);
                    currentScriptBox.ScrollToCaret();
                }
            }
            
        }
        #endregion

        #endregion

        #region Text Editor

        #region Variables
        TextArchive currentTextArchive;
        #endregion

        #region Subroutines

        #endregion

        private void addTextArchiveButton_Click(object sender, EventArgs e) {
            /* Add copy of message 0 to text archives folder */
            new TextArchive(0).SaveToFileDefaultDir(selectTextFileComboBox.Items.Count);

            /* Update ComboBox and select new file */
            selectTextFileComboBox.Items.Add("Text Archive " + selectTextFileComboBox.Items.Count);
            selectTextFileComboBox.SelectedIndex = selectTextFileComboBox.Items.Count - 1;
        }
        private void addStringButton_Click(object sender, EventArgs e) {
            currentTextArchive.messages.Add("");
            textEditorDataGridView.Rows.Add("");

            int rowInd = textEditorDataGridView.RowCount - 1;

            disableHandlers = true;
            textEditorDataGridView.Rows[rowInd].HeaderCell.Value = "0x" + rowInd.ToString("X");
            disableHandlers = false;

        }
        private void exportTextFileButton_Click(object sender, EventArgs e) {
            currentTextArchive.SaveToFileExplorePath("Text Archive " + selectTextFileComboBox.SelectedIndex);
        }
        private void saveTextArchiveButton_Click(object sender, EventArgs e) {
            currentTextArchive.SaveToFileDefaultDir(selectTextFileComboBox.SelectedIndex);
        }
        private void importTextFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .msg file */
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Text Archive (*.msg)|*.msg";
            if (of.ShowDialog(this) != DialogResult.OK) 
                return;

            /* Update Text Archive object in memory */
            string path = RomInfo.textArchivesPath + "\\" + selectTextFileComboBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Refresh controls */
            selectTextFileComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show("Text Archive imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void removeMessageFileButton_Click(object sender, EventArgs e) {
            /* Delete Text Archive */
            File.Delete(RomInfo.textArchivesPath + "\\" + (selectTextFileComboBox.Items.Count - 1).ToString("D4"));

            /* Check if currently selected file is the last one, and in that case select the one before it */
            int lastIndex = selectTextFileComboBox.Items.Count - 1;
            if (selectTextFileComboBox.SelectedIndex == lastIndex) 
                selectTextFileComboBox.SelectedIndex--;

            /* Remove item from ComboBox */
            selectTextFileComboBox.Items.RemoveAt(lastIndex);
        }
        private void removeStringButton_Click(object sender, EventArgs e) {
            if (currentTextArchive.messages.Count > 0) {
                currentTextArchive.messages.RemoveAt(currentTextArchive.messages.Count - 1);
                textEditorDataGridView.Rows.RemoveAt(textEditorDataGridView.Rows.Count - 1);
            }
        }
        private void searchMessageButton_Click(object sender, EventArgs e) {
            if (searchMessageTextBox.Text == "")
                return;

            int firstArchive;
            int lastArchive;

            if (searchOnlyCurrentCheckBox.Checked) {
                firstArchive = selectTextFileComboBox.SelectedIndex;
                lastArchive = firstArchive + 1;
            } else {
                firstArchive = 0;
                lastArchive = romInfo.GetTextArchivesCount();
            }

            textSearchResultsListBox.Items.Clear();

            if (lastArchive > 828)
                lastArchive = 828;
            textSearchProgressBar.Maximum = lastArchive;

            List<string> results = null;
            if (caseSensitiveTextSearchCheckbox.Checked) {
                results = searchTexts(firstArchive, lastArchive, (string x) => x.Contains(searchMessageTextBox.Text));
            } else {
                results = searchTexts(firstArchive, lastArchive, (string x) => x.IndexOf(searchMessageTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }

            textSearchResultsListBox.Items.AddRange(results.ToArray());
            textSearchProgressBar.Value = 0;
            caseSensitiveTextSearchCheckbox.Enabled = true;
        }

        private List<string> searchTexts(int firstArchive, int lastArchive, Func<string, bool> criteria) {
            List<string> results = new List<string>();

            for (int i = firstArchive; i < lastArchive; i++) {

                TextArchive file = new TextArchive(i);
                for (int j = 0; j < file.messages.Count; j++) {
                    if (criteria(file.messages[j])) {
                        results.Add("(" + i.ToString("D3") + ")" + " - #" + j.ToString("D2") + " --- " + file.messages[j].Substring(0, Math.Min(file.messages[j].Length, 40)));
                    }
                }
                textSearchProgressBar.Value = i;
            }
            return results;
        }

        private void searchMessageTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                searchMessageButton_Click(null, null);
        }
        private void replaceMessageButton_Click(object sender, EventArgs e) {
            if (searchMessageTextBox.Text == "")
                return;

            int firstArchive;
            int lastArchive;

            string specify;
            if (replaceOnlyCurrentCheckBox.Checked) {
                specify = " in the current text bank only";
                firstArchive = selectTextFileComboBox.SelectedIndex;
                lastArchive = firstArchive + 1;
            } else {
                specify = " in every Text Bank of the game";
                firstArchive = 0;
                lastArchive = romInfo.GetTextArchivesCount();
            }

            string message = "You are about to replace every occurrence of " + '"' + searchMessageTextBox.Text + '"'
                + " with " + '"' + replaceMessageTextBox.Text + '"' + specify +
                ".\nThe operation can't be interrupted nor undone.\n\nProceed?";
            DialogResult d = MessageBox.Show(message, "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {

                string searchString = searchMessageTextBox.Text;
                string replaceString = replaceMessageTextBox.Text;
                textSearchResultsListBox.Items.Clear();

                if (lastArchive > 828)
                    lastArchive = 828;
                textSearchProgressBar.Maximum = lastArchive;

                for (int k = firstArchive; k < lastArchive; k++) {
                    TextArchive file = new TextArchive(k);
                    currentTextArchive = file;
                    bool found = false;

                    if (caseSensitiveTextReplaceCheckbox.Checked) {
                        for (int j = 0; j < file.messages.Count; j++) {
                            if (file.messages[j].Contains(searchString)) {
                                file.messages[j] = file.messages[j].Replace(searchString, replaceString);
                                found = true;
                            }
                        }
                    } else {
                        for (int j = 0; j < file.messages.Count; j++) {
                            if (file.messages[j].IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                                file.messages[j] = file.messages[j].Replace(searchString, replaceString);
                                found = true;
                            }
                        }
                    }

                    textSearchProgressBar.Value = k;
                    if (found) {
                        disableHandlers = true;

                        textEditorDataGridView.Rows.Clear();
                        textSearchResultsListBox.Items.Add("Text archive (" + k + ") - Succesfully edited");
                        updateTextEditorFileView(false);

                        disableHandlers = false;
                        currentTextArchive.SaveToFileDefaultDir(k);
                    }
                    //else searchMessageResultTextBox.AppendText(searchString + " not found in this file");
                    //this.saveMessageFileButton_Click(sender, e);
                }
                updateTextEditorFileView(true);
                textSearchProgressBar.Value = 0;
            }
        }
        private void selectTextFileComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            updateTextEditorFileView(true);
        }
        private void updateTextEditorFileView(bool readAgain) {
            disableHandlers = true;

            textEditorDataGridView.Rows.Clear();
            if (currentTextArchive == null || readAgain) { 
                currentTextArchive = new TextArchive(selectTextFileComboBox.SelectedIndex);
            }

            foreach (string msg in currentTextArchive.messages) {
                textEditorDataGridView.Rows.Add(msg);
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
            for (int i = 0; i < currentTextArchive.messages.Count; i++) {
                textEditorDataGridView.Rows[i].HeaderCell.Value = "0x" + i.ToString("X");
            }
        }
        private void printTextEditorLinesDecimal() {
            for (int i = 0; i < currentTextArchive.messages.Count; i++) {
                textEditorDataGridView.Rows[i].HeaderCell.Value = i.ToString();
            }
        }
        private void textEditorDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (disableHandlers)
                return;
            if (e.RowIndex > -1)
                currentTextArchive.messages[e.RowIndex] = textEditorDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }
        private void textSearchResultsListBox_GoToEntryResult(object sender, MouseEventArgs e) {
            if (textSearchResultsListBox.SelectedIndex < 0)
                return;

            string[] msgResult = textSearchResultsListBox.Text.Split(new string[] { " --- "}, StringSplitOptions.RemoveEmptyEntries);
            string[] parts = msgResult[0].Substring(1).Split(new string[] { ") - #" }, StringSplitOptions.RemoveEmptyEntries);

            int msg;
            int line;
            if (Int32.TryParse((parts[0]), out msg)) {
                if (Int32.TryParse((parts[1]), out line)) {
                    selectTextFileComboBox.SelectedIndex = msg;
                    textEditorDataGridView.ClearSelection();
                    textEditorDataGridView.Rows[line].Selected = true;
                    textEditorDataGridView.Rows[line].Cells[0].Selected = true;
                    textEditorDataGridView.CurrentCell = textEditorDataGridView.Rows[line].Cells[0];

                    return;
                }
            }
        }
        private void textSearchResultsListBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                textSearchResultsListBox_GoToEntryResult(null, null);
            }
        }
        private void hexRadiobutton_CheckedChanged(object sender, EventArgs e) {
            updateTextEditorLineNumbers();
        }
        private void updateTextEditorLineNumbers() {
            disableHandlers = true;
            if (hexRadiobutton.Checked) {
                printTextEditorLinesHex();
            } else {
                printTextEditorLinesDecimal();
            }
            disableHandlers = false;
        }
        #endregion

        #region Tileset Editor
        public NSMBe4.NSBMD.NSBTX_File currentTileset;
        public AreaData currentAreaData;

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
            sf.FileName = "Texture Pack " + texturesListBox.SelectedIndex;
            if (sf.ShowDialog(this) != DialogResult.OK)
                return;

            string tilesetPath;
            if (mapTilesetRadioButton.Checked) 
                tilesetPath = romInfo.mapTexturesDirPath + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            else 
                tilesetPath = romInfo.buildingTexturesDirPath + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            File.Copy(tilesetPath, sf.FileName);
        }
        private void importNSBTXButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .nsbtx file */
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "NSBTX File (*.nsbtx)|*.nsbtx";
            if (ofd.ShowDialog(this) != DialogResult.OK) 
                return;

            /* Update nsbtx file */
            string tilesetPath;
            if (mapTilesetRadioButton.Checked) 
                tilesetPath = romInfo.mapTexturesDirPath + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            else 
                tilesetPath = romInfo.buildingTexturesDirPath + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            File.Copy(ofd.FileName, tilesetPath, true);

            /* Update nsbtx object in memory and controls */
            currentTileset = new NSMBe4.NSBMD.NSBTX_File(new FileStream(ofd.FileName, FileMode.Open));

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
            if (disableHandlers)
                return;
            disableHandlers = true;

            /* Clear ListBoxes */
            texturesListBox.Items.Clear();
            palettesListBox.Items.Clear();

            /* Load tileset file */
            string tilesetPath;
            if (mapTilesetRadioButton.Checked) {
                tilesetPath = romInfo.mapTexturesDirPath + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            } else {
                tilesetPath = romInfo.buildingTexturesDirPath + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            }

            currentTileset = new NSMBe4.NSBMD.NSBTX_File(new FileStream(tilesetPath, FileMode.Open));
            string currentItemName = texturePacksListBox.Items[texturePacksListBox.SelectedIndex].ToString();

            if (currentTileset.TexInfo.names == null || currentTileset.PalInfo.names == null) {
                if (!currentItemName.StartsWith("Error!"))
                    texturePacksListBox.Items[texturePacksListBox.SelectedIndex] = "Error! - " + currentItemName;
                disableHandlers = false;
                return;
            }
            /* Add textures and palette slot names to ListBoxes */
            texturesListBox.Items.AddRange(currentTileset.TexInfo.names.ToArray());
            palettesListBox.Items.AddRange(currentTileset.PalInfo.names.ToArray());

            disableHandlers = false;

            if (texturesListBox.Items.Count > 0)
                texturesListBox.SelectedIndex = 0;
        }
        private void texturesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers)
                return;

            string texSelected = texturesListBox.SelectedItem.ToString();
            string result = findAndSelectMatchingPalette(texSelected);
            if (result != null) {
                palettesListBox.SelectedItem = result;
                statusLabel.Text = "Ready";
            }

            try {
                texturePictureBox.Image = LoadTextureFromNSBTX(currentTileset, texturesListBox.SelectedIndex, palettesListBox.SelectedIndex);
            } catch { }
        }

        private string findAndSelectMatchingPalette(string findThis) {
            statusLabel.Text = "Searching palette...";

            string copy = findThis;
            while (copy.Length > 0) {
                if (palettesListBox.Items.Contains(copy + "_pl")) {
                    return copy + "_pl";
                }
                if (palettesListBox.Items.Contains(copy)) {
                    return copy;
                }
                copy = copy.Substring(0, copy.Length - 1);
            }

            foreach (string palette in palettesListBox.Items) {
                if (palette.StartsWith(findThis)) {
                    return palette;
                }
            }

            statusLabel.Text = "Couldn't find a palette to match " + '"' + findThis + '"';
            return null;
        }

        private void areaDataBuildingTilesetUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) 
                return;
            currentAreaData.buildingsTileset = (ushort)areaDataBuildingTilesetUpDown.Value;
        }
        private void areaDataDynamicTexturesUpDown_ValueChanged(object sender, EventArgs e) {
            if (areaDataDynamicTexturesNumericUpDown.Value == areaDataDynamicTexturesNumericUpDown.Maximum) {
                areaDataDynamicTexturesNumericUpDown.ForeColor = Color.Red;
            } else {
                areaDataDynamicTexturesNumericUpDown.ForeColor = Color.Black;
            }

            if (disableHandlers)
                return;
            currentAreaData.dynamicTextureType = (ushort)areaDataDynamicTexturesNumericUpDown.Value;
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
        private void saveAreaDataButton_Click(object sender, EventArgs e) {
            string areaDataPath = romInfo.areaDataDirPath + "\\" + selectAreaDataListBox.SelectedIndex.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(areaDataPath, FileMode.Create)))
                writer.Write(currentAreaData.Save(RomInfo.gameVersion));
        }
        private void selectAreaDataListBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentAreaData = LoadAreaData((uint)selectAreaDataListBox.SelectedIndex);

            areaDataBuildingTilesetUpDown.Value = currentAreaData.buildingsTileset;
            areaDataMapTilesetUpDown.Value = currentAreaData.mapTileset;
            areaDataLightTypeComboBox.SelectedIndex = currentAreaData.lightType;

            disableHandlers = true;
            switch (RomInfo.gameVersion) {
                case "D":
                case "P":
                case "Plat":
                    break;
                default:
                    areaDataDynamicTexturesNumericUpDown.Value = currentAreaData.dynamicTextureType;
                    if (currentAreaData.areaType == 0)
                        indoorAreaRadioButton.Checked = true;
                    else
                        outdoorAreaRadioButton.Checked = true;
                    break;
            }
            disableHandlers = false;
        }
        private void indoorAreaRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (indoorAreaRadioButton.Checked == true)
                currentAreaData.areaType = AreaData.TYPE_INDOOR; //0
            else
                currentAreaData.areaType = AreaData.TYPE_OUTDOOR; //1
        }
        private void addNSBTXButton_Click(object sender, EventArgs e) {
            /* Add new NSBTX file to the correct folder */
            if (mapTilesetRadioButton.Checked) {
                File.Copy(romInfo.mapTexturesDirPath + "\\" + 0.ToString("D4"), romInfo.mapTexturesDirPath + "\\" + texturePacksListBox.Items.Count.ToString("D4"));
            } else {
                File.Copy(romInfo.buildingTexturesDirPath + "\\" + 0.ToString("D4"), romInfo.buildingTexturesDirPath + "\\" + texturePacksListBox.Items.Count.ToString("D4"));
            }
           
            /* Update ComboBox and select new file */
            texturePacksListBox.Items.Add("Texture Pack " + texturePacksListBox.Items.Count);
            texturePacksListBox.SelectedIndex = texturePacksListBox.Items.Count - 1;
        }
        private void removeNSBTXButton_Click(object sender, EventArgs e) {
            if (texturePacksListBox.Items.Count > 1) {
                /* Delete NSBTX file */
                if (mapTilesetRadioButton.Checked)
                    File.Delete(romInfo.mapTexturesDirPath + "\\" + (texturePacksListBox.Items.Count - 1).ToString("D4"));
                else {
                    File.Delete(romInfo.buildingTexturesDirPath + "\\" + (texturePacksListBox.Items.Count - 1).ToString("D4"));
                }

                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = texturePacksListBox.Items.Count - 1;
                if (texturePacksListBox.SelectedIndex == lastIndex)
                    texturePacksListBox.SelectedIndex--;

                /* Remove item from ComboBox */
                texturePacksListBox.Items.RemoveAt(lastIndex);
            } else {
                MessageBox.Show("At least one tileset must be kept.", "Can't delete tileset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void addAreaDataButton_Click(object sender, EventArgs e) {
            /* Add new NSBTX file to the correct folder */
            File.Copy(romInfo.areaDataDirPath + "\\" + 0.ToString("D4"), romInfo.areaDataDirPath + "\\" + selectAreaDataListBox.Items.Count.ToString("D4"));

            /* Update ComboBox and select new file */
            selectAreaDataListBox.Items.Add("AreaData File " + selectAreaDataListBox.Items.Count);
            selectAreaDataListBox.SelectedIndex = selectAreaDataListBox.Items.Count - 1;
        }

        private void removeAreaDataButton_Click(object sender, EventArgs e) {
            if (selectAreaDataListBox.Items.Count > 1) {
                /* Delete AreaData file */
                File.Delete(romInfo.areaDataDirPath + "\\" + (selectAreaDataListBox.Items.Count - 1).ToString("D4"));

                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = selectAreaDataListBox.Items.Count - 1;
                if (selectAreaDataListBox.SelectedIndex == lastIndex)
                    selectAreaDataListBox.SelectedIndex--;

                /* Remove item from ComboBox */
                selectAreaDataListBox.Items.RemoveAt(lastIndex);
            } else {
                MessageBox.Show("At least one AreaData file must be kept.", "Can't delete AreaData", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void exportAreaDataButton_Click(object sender, EventArgs e) {
            if (selectAreaDataListBox.SelectedIndex < 0)
                return;

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "AreaData File (*.bin)|*.bin";
            sf.FileName = "AreaData File " + selectAreaDataListBox.SelectedIndex;
            if (sf.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(new FileStream(sf.FileName, FileMode.Create)))
                writer.Write(currentAreaData.Save(RomInfo.gameVersion));
        }
        private void importAreaDataButton_Click(object sender, EventArgs e) {
            if (selectAreaDataListBox.SelectedIndex < 0)
                return;

            /* Prompt user to select .evt file */
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "AreaData File (*.bin)|*.bin";
            if (of.ShowDialog(this) != DialogResult.OK)
                return;

            /* Update areadata object in memory */
            string path = romInfo.areaDataDirPath + "\\" + selectAreaDataListBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Refresh controls */
            selectAreaDataListBox_SelectedIndexChanged(sender, e);

            /* Display success message */
            MessageBox.Show("AreaData File imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

    }
}