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
using NarcAPI;
using Tao.OpenGl;
using LibNDSFormats.NSBMD;
using LibNDSFormats.NSBTX;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using static DSPRE.RomInfo;
using Images;
using Ekona.Images;
using Microsoft.WindowsAPICodePack.Dialogs;
using ScintillaNET;
using ScintillaNET.Utils;
using System.Globalization;
using static DSPRE.ROMFiles.Event;

namespace DSPRE {
    public partial class MainProgram : Form {
        public MainProgram() {
            InitializeComponent();
            SetMenuLayout(Properties.Settings.Default.menuLayout); //Read user settings for menu layout
        }

        #region Program Window

        #region Variables
        public bool disableHandlers = false;
        public bool iconON = false;

        /* Editors Setup */
        public bool matrixEditorIsReady { get; private set; } = false;
        public bool mapEditorIsReady { get; private set; } = false;
        public bool nsbtxEditorIsReady { get; private set; } = false;
        public bool eventEditorIsReady { get; private set; } = false;
        public bool scriptEditorIsReady { get; private set; } = false;
        public bool textEditorIsReady { get; private set; } = false;
        public bool cameraEditorIsReady { get; private set; } = false;
        public bool trainerEditorIsReady { get; private set; } = false;
        public bool tableEditorIsReady { get; private set; } = false;

        /* ROM Information */
        public static string gameCode;
        public static byte europeByte;
        RomInfo romInfo;

        #endregion

        #region Subroutines
        private void MainProgram_FormClosing(object sender, FormClosingEventArgs e) {
            if (MessageBox.Show("Are you sure you want to quit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
                e.Cancel = true;
            }
            Properties.Settings.Default.Save();
        }
        private string[] GetBuildingsList(bool interior) {
            List<string> names = new List<string>();
            string path = romInfo.GetBuildingModelsDirPath(interior);
            int buildModelsCount = Directory.GetFiles(path).Length;

            for (int i = 0; i < buildModelsCount; i++) {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(path + "\\" + i.ToString("D4"), 0x38)) {
                    string nsbmdName = Encoding.UTF8.GetString(reader.ReadBytes(16)).TrimEnd();
                    names.Add(nsbmdName);
                }
            }
            return names.ToArray();
        }
        private string[] GetTrainerNames() {
            List<string> trainerList = new List<string>();

            /* Store all trainer names and classes */
            TextArchive trainerClasses = new TextArchive(RomInfo.trainerClassMessageNumber);
            TextArchive trainerNames = new TextArchive(RomInfo.trainerNamesMessageNumber);
            string trainerPropertiesUnpackedDir = RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir;

            int trainerCount = Directory.GetFiles(trainerPropertiesUnpackedDir).Length;

            for (int i = 0; i < trainerCount; i++) {
                int classMessageID = BitConverter.ToUInt16(DSUtils.ReadFromFile(trainerPropertiesUnpackedDir + "\\" + i.ToString("D4"), startOffset: 1, 2), 0);
                string currentTrainerName;

                if (i < trainerNames.messages.Count) {
                    currentTrainerName = trainerNames.messages[i];
                } else {
                    currentTrainerName = TrainerFile.NAME_NOT_FOUND;
                }
                
                trainerList.Add("[" + i.ToString("D2") + "] " + trainerClasses.messages[classMessageID] + " " + currentTrainerName);

            }
            return trainerList.ToArray();
        }
        private void PaintGameIcon(object sender, PaintEventArgs e) {
            if (iconON) {
                FileStream banner;

                try {
                    banner = File.OpenRead(RomInfo.workDir + @"banner.bin");
                } catch (FileNotFoundException) {
                    MessageBox.Show("Couldn't load " + '"' + "banner.bin" + '"' + '.', "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                BinaryReader readIcon = new BinaryReader(banner);
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
                        palB |= (1 << 4);
                    if ((firstByte & (1 << 5)) != 0)
                        palB |= (1 << 3);
                    if ((firstByte & (1 << 4)) != 0)
                        palB |= (1 << 2);
                    if ((firstByte & (1 << 3)) != 0)
                        palB |= (1 << 1);
                    if ((firstByte & (1 << 2)) != 0)
                        palB |= (1 << 0);
                    if ((firstByte & (1 << 1)) != 0)
                        palG |= (1 << 4);
                    if ((firstByte & (1 << 0)) != 0)
                        palG |= (1 << 3);
                    if ((secondByte & (1 << 7)) != 0)
                        palG |= (1 << 2);
                    if ((secondByte & (1 << 6)) != 0)
                        palG |= (1 << 1);
                    if ((secondByte & (1 << 5)) != 0)
                        palG |= (1 << 0);
                    if ((secondByte & (1 << 4)) != 0)
                        palR |= (1 << 4);
                    if ((secondByte & (1 << 3)) != 0)
                        palR |= (1 << 3);
                    if ((secondByte & (1 << 2)) != 0)
                        palR |= (1 << 2);
                    if ((secondByte & (1 << 1)) != 0)
                        palR |= (1 << 1);
                    if ((secondByte & (1 << 0)) != 0)
                        palR |= (1 << 0);

                    paletteArray[palCounter++] = palR * 8;
                    paletteArray[palCounter++] = palG * 8;
                    paletteArray[palCounter++] = palB * 8;
                }
                #endregion
                #region Read Icon Image
                readIcon.BaseStream.Position = 0x20;
                int iconY = 0;
                int xTile = 0;
                int yTile = 0;
                for (int o = 0; o < 4; o++) {
                    for (int a = 0; a < 4; a++) {
                        for (int i = 0; i < 8; i++) {
                            int iconX = xTile;

                            for (int counter = 0; counter < 4; counter++) {
                                byte pixelByte = readIcon.ReadByte();
                                int pixelPalId = pixelByte & 0x0F;
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
            }
        }
        private void updateBuildingListComboBox(bool interior) {
            string[] bldList = GetBuildingsList(interior);

            buildIndexComboBox.Items.Clear();
            for (int i = 0; i < bldList.Length; i++) {
                buildIndexComboBox.Items.Add("[" + i + "] " + bldList[i]);
            }
            toolStripProgressBar.Value++;
        }

        public void SetupScriptEditor() {
            /* Extract essential NARCs sub-archives*/
            statusLabel.Text = "Setting up Script Editor...";
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.scripts }); //12 = scripts Narc Dir

            selectScriptFileComboBox.Items.Clear();
            int scriptCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.scripts].unpackedDir).Length;
            for (int i = 0; i < scriptCount; i++) {
                selectScriptFileComboBox.Items.Add("Script File " + i);
            }

            UpdateScriptNumberCheckBox((NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference);
            selectScriptFileComboBox.SelectedIndex = 0;
            statusLabel.Text = "Ready";
        }
        private void SetupTextEditor() {
            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.textArchives });

            statusLabel.Text = "Setting up Text Editor...";
            Update();

            selectTextFileComboBox.Items.Clear();
            int textCount = romInfo.GetTextArchivesCount();
            for (int i = 0; i < textCount; i++) {
                selectTextFileComboBox.Items.Add("Text Archive " + i);
            }

            disableHandlers = true;
            hexRadiobutton.Checked = Properties.Settings.Default.textEditorPreferHex;
            disableHandlers = false;

            selectTextFileComboBox.SelectedIndex = 0;
            statusLabel.Text = "Ready";
        }

        private int UnpackRomCheckUserChoice() {
            // Check if extracted data for the ROM exists, and ask user if they want to load it.
            // Returns true if user aborted the process
            if (Directory.Exists(RomInfo.workDir)) {
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
        private bool UnpackRom(string ndsFileName) {
            statusLabel.Text = "Unpacking ROM contents to " + RomInfo.workDir + " ...";
            Update();

            Directory.CreateDirectory(RomInfo.workDir);
            Process unpack = new Process();
            unpack.StartInfo.FileName = @"Tools\ndstool.exe";
            unpack.StartInfo.Arguments = "-x " + '"' + ndsFileName + '"'
                + " -9 " + '"' + RomInfo.arm9Path + '"'
                + " -7 " + '"' + RomInfo.workDir + "arm7.bin" + '"'
                + " -y9 " + '"' + RomInfo.workDir + "y9.bin" + '"'
                + " -y7 " + '"' + RomInfo.workDir + "y7.bin" + '"'
                + " -d " + '"' + RomInfo.workDir + "data" + '"'
                + " -y " + '"' + RomInfo.workDir + "overlay" + '"'
                + " -t " + '"' + RomInfo.workDir + "banner.bin" + '"'
                + " -h " + '"' + RomInfo.workDir + "header.bin" + '"';
            Application.DoEvents();
            unpack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            unpack.StartInfo.CreateNoWindow = true;
            try {
                unpack.Start();
                unpack.WaitForExit();
            } catch (System.ComponentModel.Win32Exception) {
                MessageBox.Show("Failed to call ndstool.exe" + Environment.NewLine + "Make sure DSPRE's Tools folder is intact.",
                    "Couldn't unpack ROM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        #endregion
        private void romToolBoxToolStripMenuItem_Click(object sender, EventArgs e) {
            using (ROMToolboxDialog window = new ROMToolboxDialog()) {
                window.ShowDialog();
                if (ROMToolboxDialog.flag_standardizedItems && eventEditorIsReady) {
                    UpdateItemComboBox(RomInfo.GetItemNames());
                }
                if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied) {
                    addHeaderBTN.Enabled = true;
                    removeLastHeaderBTN.Enabled = true;
                }
            }
        }
        private void UpdateItemComboBox(string[] itemNames) {
            if (itemComboboxIsUpToDate) {
                return;
            }
            itemsSelectorHelpBtn.Visible = false;
            owItemComboBox.Size = new Size(new Point(owItemComboBox.Size.Width + 30, owItemComboBox.Size.Height));
            owItemComboBox.Items.Clear();
            owItemComboBox.Items.AddRange(itemNames);
            OWTypeChanged(null, null);
            itemComboboxIsUpToDate = true;
        }
        private void scriptCommandsDatabaseToolStripButton_Click(object sender, EventArgs e) {
            OpenCommandsDatabase(RomInfo.ScriptCommandNamesDict, RomInfo.ScriptCommandParametersDict, RomInfo.ScriptActionNamesDict, RomInfo.ScriptComparisonOperatorsDict);
        }
        private void nsbmdExportTexButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = MapFile.TexturedNSBMDFilter
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            byte[] modelFile = DSUtils.ReadFromFile(of.FileName);
            if (DSUtils.CheckNSBMDHeader(modelFile) == DSUtils.NSBMD_DOESNTHAVE_TEXTURE) {
                MessageBox.Show("This NSBMD file is untextured.", "No textures to extract", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //============================================================
            MessageBox.Show("Choose where to save the textures.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SaveFileDialog texSf = new SaveFileDialog {
                Filter = "NSBTX File(*.nsbtx)|*.nsbtx",
                FileName = Path.GetFileNameWithoutExtension(of.FileName)
            };
            if (texSf.ShowDialog() != DialogResult.OK) {
                return;
            }

            DSUtils.WriteToFile(texSf.FileName, DSUtils.GetTexturesFromTexturedNSBMD(modelFile));
            MessageBox.Show("The textures of " + of.FileName + " have been extracted and saved.", "Textures saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void nsbmdRemoveTexButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = MapFile.TexturedNSBMDFilter
            };
            
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            byte[] modelFile = DSUtils.ReadFromFile(of.FileName);
            if (DSUtils.CheckNSBMDHeader(modelFile) == DSUtils.NSBMD_DOESNTHAVE_TEXTURE) {
                MessageBox.Show("This NSBMD file is already untextured.", "No textures to remove", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string extramsg = "";
            DialogResult d = MessageBox.Show("Would you like to save the removed textures to a file?", "Save textures?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (d.Equals(DialogResult.Yes)) {

                MessageBox.Show("Choose where to save the textures.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SaveFileDialog texSf = new SaveFileDialog {
                    Filter = "NSBTX File(*.nsbtx)|*.nsbtx",
                    FileName = Path.GetFileNameWithoutExtension(of.FileName)
                };

                if (texSf.ShowDialog() == DialogResult.OK) {
                    DSUtils.WriteToFile(texSf.FileName, DSUtils.GetTexturesFromTexturedNSBMD(modelFile));
                    extramsg = " exported and";
                }
            }

            //============================================================
            MessageBox.Show("Choose where to save the untextured model.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SaveFileDialog sf = new SaveFileDialog {
                Filter = "Untextured NSBMD File(*.nsbmd)|*.nsbmd",
                FileName = Path.GetFileNameWithoutExtension(of.FileName) + "_untextured"
            };
            if (sf.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            DSUtils.WriteToFile(sf.FileName, DSUtils.GetModelWithoutTextures(modelFile));
            MessageBox.Show("Textures correctly" + extramsg + " removed!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void nsbmdAddTexButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = MapFile.UntexturedNSBMDFilter
            };
            if (of.ShowDialog(this) != DialogResult.OK)
                return;

            byte[] modelFile = File.ReadAllBytes(of.FileName);
            if (DSUtils.CheckNSBMDHeader(modelFile) == DSUtils.NSBMD_HAS_TEXTURE) {
                DialogResult d = MessageBox.Show("This NSBMD file is already textured.\nDo you want to overwrite its textures?", "Textures found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (d.Equals(DialogResult.No)) {
                    return;
                }
            }

            MessageBox.Show("Select the new NSBTX texture file.", "Choose NSBTX", MessageBoxButtons.OK, MessageBoxIcon.Information);

            OpenFileDialog openNsbtx = new OpenFileDialog {
                Filter = "NSBTX File(*.nsbtx)|*.nsbtx"
            };
            if (openNsbtx.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            byte[] textureFile = File.ReadAllBytes(openNsbtx.FileName);


            //============================================================
            MessageBox.Show("Choose where to save the new textured model.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string texturedPath = Path.GetFileNameWithoutExtension(of.FileName);
            if (texturedPath.Contains("_untextured")) {
                texturedPath = texturedPath.Substring(0, texturedPath.Length - "_untextured".Length);
            }

            SaveFileDialog sf = new SaveFileDialog {
                Filter = MapFile.TexturedNSBMDFilter,
                FileName = Path.GetFileNameWithoutExtension(of.FileName) + "_textured"
            };

            if (sf.ShowDialog(this) != DialogResult.OK)
                return;

            DSUtils.WriteToFile(sf.FileName, DSUtils.BuildNSBMDwithTextures(modelFile, textureFile), fmode: FileMode.Create);
            MessageBox.Show("Textures correctly written to NSBMD file.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void OpenCommandsDatabase(Dictionary<ushort, string> namesDict, Dictionary<ushort, byte[]> paramsDict, Dictionary<ushort, string> actionsDict,
            Dictionary<ushort, string> comparisonOPsDict) {
            statusLabel.Text = "Setting up Commands Database. Please wait...";
            Update();
            CommandsDatabase form = new CommandsDatabase(namesDict, paramsDict, actionsDict, comparisonOPsDict);
            form.Show();
            statusLabel.Text = "Ready";
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
        private void unpackBuildingEditorNARCs(bool forceUnpack = false) {
            toolStripProgressBar.Visible = true;

            statusLabel.Text = "Attempting to unpack Building Editor NARCs... Please wait. This might take a while";
            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 4;
            toolStripProgressBar.Value = 0;
            Update();

            List<DirNames> toUnpack = new List<DirNames> {
                DirNames.exteriorBuildingModels,
                DirNames.buildingConfigFiles,
                DirNames.buildingTextures,
                DirNames.areaData
            };

            if (forceUnpack) {
                DSUtils.ForceUnpackNarcs(toUnpack);

                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    DSUtils.ForceUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });// Last = interior buildings dir
                }
            } else {
                DSUtils.TryUnpackNarcs(toUnpack);

                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });
                }
            }

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;
            statusLabel.Text = "Ready";
            Update();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            string message = "DS Pokémon ROM Editor by Nømura and AdAstra/LD3005" + Environment.NewLine + "version 1.7" + Environment.NewLine
                + Environment.NewLine + "This tool was largely inspired by Markitus95's \"Spiky's DS Map Editor\" (SDSME), from which certain assets were also recycled. " +
                "Credits go to Markitus, Ark, Zark, Florian, and everyone else who deserves credit for SDSME." + Environment.NewLine
                + Environment.NewLine + "Special thanks to Trifindo, Mikelan98, JackHack96, Pleonex and BagBoy."
                + Environment.NewLine + "Their help, research and expertise in many fields of NDS ROM Hacking made the development of this tool possible.";

            MessageBox.Show(message, "About...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void loadRom_Click(object sender, EventArgs e) {
            OpenFileDialog openRom = new OpenFileDialog {
                Filter = DSUtils.NDSRomFilter
            }; // Select ROM
            if (openRom.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            SetupROMLanguage(openRom.FileName);
            /* Set ROM gameVersion and language */
            romInfo = new RomInfo(gameCode, openRom.FileName, useSuffix: true);

            if (string.IsNullOrWhiteSpace(RomInfo.romID) || string.IsNullOrWhiteSpace(RomInfo.fileName)) {
                return;
            }

            CheckROMLanguage();

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
                            Directory.Delete(RomInfo.workDir, true);
                        } catch (IOException) {
                            MessageBox.Show("Concurrent access detected: \n" + RomInfo.workDir +
                                "\nMake sure no other process is using the extracted ROM folder while DSPRE is running.", "Concurrent Access", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        Update();
                    }

                    try {
                        if (!UnpackRom(openRom.FileName)) {
                            statusLabel.Text = "Error";
                            languageLabel.Text = "";
                            versionLabel.Text = "Error";
                            return;
                        }
                        DSUtils.ARM9.EditSize(-12);
                    } catch (IOException) {
                        MessageBox.Show("Can't access temp directory: \n" + RomInfo.workDir + "\nThis might be a temporary issue.\nMake sure no other process is using it and try again.", "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        statusLabel.Text = "Error: concurrent access to " + RomInfo.workDir;
                        Update();
                        return;
                    }
                    break;
            }

            iconON = true;
            gameIcon.Refresh();  // Paint game icon
            statusLabel.Text = "Attempting to unpack NARCs from folder...";
            Update();

            ReadROMInitData();
        }

        private void CheckROMLanguage() {
            versionLabel.Visible = true;
            languageLabel.Visible = true;

            versionLabel.Text = RomInfo.gameVersion.ToString() + " " + "[" + RomInfo.romID + "]";
            languageLabel.Text = "Lang: " + RomInfo.gameLanguage;

            if (RomInfo.gameLanguage == gLangEnum.English) {
                if (europeByte == 0x0A) {
                    languageLabel.Text += " [Europe]";
                } else {
                    languageLabel.Text += " [America]";
                }
            }
        }

        private void readDataFromFolderButton_Click(object sender, EventArgs e) {
            CommonOpenFileDialog romFolder = new CommonOpenFileDialog {
                IsFolderPicker = true,
                Multiselect = false
            };
            if (romFolder.ShowDialog() != CommonFileDialogResult.Ok) {
                return;
            }

            try {
                SetupROMLanguage(Directory.GetFiles(romFolder.FileName).First(x => x.Contains("header.bin")));
            } catch (InvalidOperationException) {
                MessageBox.Show("This folder does not seem to contain any data from a NDS Pokémon ROM.", "No ROM Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            /* Set ROM gameVersion and language */
            romInfo = new RomInfo(gameCode, romFolder.FileName, useSuffix: false);

            if (string.IsNullOrWhiteSpace(RomInfo.romID) || string.IsNullOrWhiteSpace(RomInfo.fileName)) {
                return;
            }

            CheckROMLanguage();
            
            iconON = true;
            gameIcon.Refresh();  // Paint game icon

            ReadROMInitData();
        }

        private void SetupROMLanguage(string headerPath) {
            using (DSUtils.EasyReader br = new DSUtils.EasyReader(headerPath, 0xC)) {
                gameCode = Encoding.UTF8.GetString(br.ReadBytes(4));
                br.BaseStream.Position = 0x1E;
                europeByte = br.ReadByte();
            }
        }

        private void ReadROMInitData() {
            if ( DSUtils.ARM9.CheckCompressionMark() ) {
                if ( !RomInfo.gameFamily.Equals(gFamEnum.HGSS) ) {
                    MessageBox.Show("Unexpected compressed ARM9. It is advised that you double check the ARM9.");
                }
                if (!DSUtils.ARM9.Decompress(RomInfo.arm9Path)) {
                    MessageBox.Show("ARM9 decompression failed. The program can't proceed.\nAborting.",
                                "Error with ARM9 decompression", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            /* Setup essential editors */
            SetupHeaderEditor();
            eventOpenGlControl.InitializeContexts();
            mapOpenGlControl.InitializeContexts();

            mainTabControl.Show();
            loadRomButton.Enabled = false;
            readDataFromFolderButton.Enabled = false;
            saveRomButton.Enabled = true;
            saveROMToolStripMenuItem.Enabled = true;
            openROMToolStripMenuItem.Enabled = false;
            openFolderToolStripMenuItem.Enabled = false;

            unpackAllButton.Enabled = true;
            updateMapNarcsButton.Enabled = true;

            buildingEditorButton.Enabled = true;
            wildEditorButton.Enabled = true;

            romToolboxToolStripButton.Enabled = true;
            romToolboxToolStripMenuItem.Enabled = true;
            headerSearchToolStripButton.Enabled = true;
            headerSearchToolStripMenuItem.Enabled = true;
            spawnEditorToolStripButton.Enabled = true;
            spawnEditorToolStripMenuItem.Enabled = true;

            scriptCommandsButton.Enabled = true;
            statusLabel.Text = "Ready";
            this.Text += "  -  " + RomInfo.fileName;
        }

        private void saveRom_Click(object sender, EventArgs e) {
            SaveFileDialog saveRom = new SaveFileDialog {
                Filter = DSUtils.NDSRomFilter
            };
            if (saveRom.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            statusLabel.Text = "Repacking NARCS...";
            Update();

            // Repack NARCs
            foreach (KeyValuePair<DirNames, (string packedDir, string unpackedDir)> kvp in RomInfo.gameDirs) {
                DirectoryInfo di = new DirectoryInfo(kvp.Value.unpackedDir);
                if (di.Exists) {
                    Narc.FromFolder(kvp.Value.unpackedDir).Save(kvp.Value.packedDir); // Make new NARC from folder
                }
            }


            if ( DSUtils.ARM9.CheckCompressionMark() ) {
                statusLabel.Text = "Awaiting user response...";
                DialogResult d = MessageBox.Show("The ARM9 file of this ROM is currently uncompressed, but marked as compressed.\n" +
                    "This will prevent your ROM from working on native hardware.\n\n" +
                "Do you want to mark the ARM9 as uncompressed?", "ARM9 compression mismatch detected",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (d == DialogResult.Yes) {
                    DSUtils.ARM9.WriteBytes( new byte[4] { 0, 0, 0, 0 }, (uint)(RomInfo.gameFamily == gFamEnum.DP ? 0xB7C : 0xBB4) );
                }
            }

            statusLabel.Text = "Repacking ROM...";

            if (DSUtils.CheckOverlayHasCompressionFlag(1)) {
                if (ROMToolboxDialog.overlay1MustBeRestoredFromBackup) {
                    DSUtils.RestoreOverlayFromCompressedBackup(1, eventEditorIsReady);
                } else {
                    if (!DSUtils.OverlayIsCompressed(1)) {
                        DSUtils.CompressOverlay(1);
                    }
                }
            }

            if (DSUtils.CheckOverlayHasCompressionFlag(RomInfo.initialMoneyOverlayNumber)) {
                if (!DSUtils.OverlayIsCompressed(RomInfo.initialMoneyOverlayNumber)) {
                    DSUtils.CompressOverlay(RomInfo.initialMoneyOverlayNumber);
                }
            }

            
            Update();

            DSUtils.RepackROM(saveRom.FileName);

            if (RomInfo.gameFamily != gFamEnum.DP && RomInfo.gameFamily != gFamEnum.Plat) {
                if (eventEditorIsReady) {
                    if (DSUtils.OverlayIsCompressed(1)) {
                        DSUtils.DecompressOverlay(1);
                    }
                }
            }

            Properties.Settings.Default.Save();
            statusLabel.Text = "Ready";
        }
        private void unpackAllButton_Click(object sender, EventArgs e) {
            statusLabel.Text = "Awaiting user response...";
            DialogResult d = MessageBox.Show("Do you wish to unpack all extracted NARCS?\n" +
                "This operation might be long and can't be interrupted.\n\n" +
                "Any unsaved changes made to the ROM in this session will be lost." +
                "\nProceed?", "About to unpack all NARCS",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                toolStripProgressBar.Maximum = RomInfo.gameDirs.Count;
                toolStripProgressBar.Visible = true;
                toolStripProgressBar.Value = 0;
                statusLabel.Text = "Attempting to unpack all NARCs... Be patient. This might take a while...";
                Update();

                DSUtils.ForceUnpackNarcs(Enum.GetValues(typeof(DirNames)).Cast<DirNames>().ToList());
                MessageBox.Show("Operation completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                toolStripProgressBar.Value = 0;
                toolStripProgressBar.Visible = false;

                SetupHeaderEditor();
                SetupMatrixEditor();
                SetupMapEditor();
                SetupNSBTXEditor();
                SetupEventEditor();
                SetupScriptEditorTextAreas();
                SetupScriptEditor();
                SetupTextEditor();
                SetupTrainerEditor();

                statusLabel.Text = "Ready";
                Update();
            }
        }
        private void updateMapNarcsButton_Click(object sender, EventArgs e) {
            statusLabel.Text = "Awaiting user response...";
            DialogResult d = MessageBox.Show("Do you wish to unpack all NARC files necessary for the Building Editor ?\n" +
               "This operation might be long and can't be interrupted.\n\n" +
               "Any unsaved changes made to building models and textures in this session will be lost." +
               "\nProceed?", "About to unpack Building NARCs",
               MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                unpackBuildingEditorNARCs(forceUnpack: true);

                MessageBox.Show("Operation completed.", "Success",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusLabel.Text = "Ready";

                if (mapEditorIsReady) {
                    updateBuildingListComboBox(interiorbldRadioButton.Checked);
                }
                Update();
            }
        }
        private void diamondAndPearlToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenCommandsDatabase(RomInfo.BuildCommandNamesDatabase(gFamEnum.DP), RomInfo.BuildCommandParametersDatabase(gFamEnum.DP),
                RomInfo.BuildActionNamesDatabase(gFamEnum.DP), RomInfo.BuildComparisonOperatorsDatabase(gFamEnum.DP));
        }
        private void platinumToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenCommandsDatabase(RomInfo.BuildCommandNamesDatabase(gFamEnum.Plat), RomInfo.BuildCommandParametersDatabase(gFamEnum.Plat),
                RomInfo.BuildActionNamesDatabase(gFamEnum.Plat), RomInfo.BuildComparisonOperatorsDatabase(gFamEnum.Plat));
        }
        private void heartGoldAndSoulSilverToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenCommandsDatabase(RomInfo.BuildCommandNamesDatabase(gFamEnum.HGSS), RomInfo.BuildCommandParametersDatabase(gFamEnum.HGSS),
                RomInfo.BuildActionNamesDatabase(gFamEnum.HGSS), RomInfo.BuildComparisonOperatorsDatabase(gFamEnum.HGSS));
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
                    mapOpenGlControl.MouseWheel += new MouseEventHandler(mapOpenGlControl_MouseWheel);
                    mapEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == nsbtxEditorTabPage) {
                if (!nsbtxEditorIsReady) {
                    SetupNSBTXEditor();
                    nsbtxEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == eventEditorTabPage) {
                if (!eventEditorIsReady) {
                    SetupEventEditor();
                    eventEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == scriptEditorTabPage) {
                if (!scriptEditorIsReady) {
                    SetupScriptEditorTextAreas();
                    SetupScriptEditor();
                    scriptEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == textEditorTabPage) {
                if (!textEditorIsReady) {
                    SetupTextEditor();
                    textEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == cameraEditorTabPage) {
                if (!cameraEditorIsReady) {
                    SetupCameraEditor();
                    cameraEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == trainerEditorTabPage) {
                if (!trainerEditorIsReady) {
                    SetupTrainerEditor();
                    trainerEditorIsReady = true;
                }
            } else if (mainTabControl.SelectedTab == tableEditorTabPage) {
                if(!tableEditorIsReady) {
                    SetupConditionalMusicTable();
                    SetupBattleEffectsTables();
                    tableEditorIsReady = true;
                }
            }
        }

        private void spawnEditorToolStripButton_Click(object sender, EventArgs e) {
            if (!matrixEditorIsReady) {
                SetupMatrixEditor();
            }
            using (SpawnEditor ed = new SpawnEditor(headerListBoxNames)) {
                ed.ShowDialog();
            }
        }
        private void spawnEditorToolStripMenuItem_Click(object sender, EventArgs e) {
            spawnEditorToolStripButton_Click(null, null);
        }
        private void wildEditorButton_Click(object sender, EventArgs e) {
            openWildEditor(loadCurrent: false);
        }
        private void openWildEditorWithIdButtonClick(object sender, EventArgs e) {
            openWildEditor(loadCurrent: true);
        }
        private void openWildEditor(bool loadCurrent) {
            statusLabel.Text = "Attempting to extract Wild Encounters NARC...";
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames>() { DirNames.encounters });

            statusLabel.Text = "Passing control to Wild Pokémon Editor...";
            Update();

            int encToOpen = loadCurrent ? (int)wildPokeUpDown.Value : 0;

            string wildPokeUnpackedPath = gameDirs[DirNames.encounters].unpackedDir;
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    using (WildEditorDPPt editor = new WildEditorDPPt(wildPokeUnpackedPath, RomInfo.GetPokémonNames(), encToOpen))
                        editor.ShowDialog();
                    break;
                default:
                    using (WildEditorHGSS editor = new WildEditorHGSS(wildPokeUnpackedPath, RomInfo.GetPokémonNames(), encToOpen))
                        editor.ShowDialog();
                    break;
            }
            statusLabel.Text = "Ready";
        }
        #endregion

        #region Header Editor

        #region Variables
        public MapHeader currentHeader;
        public List<string> internalNames;
        public List<string> headerListBoxNames;
        #endregion
        private void SetupHeaderEditor() {
            /* Extract essential NARCs sub-archives*/

            statusLabel.Text = "Attempting to unpack Header Editor NARCs... Please wait.";
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.synthOverlay, DirNames.textArchives, DirNames.dynamicHeaders });

            statusLabel.Text = "Reading internal names... Please wait.";
            Update();

            internalNames = new List<string>();
            headerListBoxNames = new List<string>();
            int headerCount;
            if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                addHeaderBTN.Enabled = true;
                removeLastHeaderBTN.Enabled = true;
                headerCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir).Length;
            } else {
                headerCount = RomInfo.GetHeaderCount();
            }

            /* Read Header internal names */
            try {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(RomInfo.internalNamesLocation)) {
                    for (int i = 0; i < headerCount; i++) {
                        byte[] row = reader.ReadBytes(RomInfo.internalNameLength);

                        string internalName = Encoding.ASCII.GetString(row);//.TrimEnd();
                        headerListBoxNames.Add(i.ToString("D3") + MapHeader.nameSeparator + internalName);
                        internalNames.Add(internalName.TrimEnd('\0'));
                    }
                }

                headerListBox.Items.Clear();
                headerListBox.Items.AddRange(headerListBoxNames.ToArray());
            } catch (FileNotFoundException) {
                MessageBox.Show(RomInfo.internalNamesLocation + " doesn't exist.", "Couldn't read internal names", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            /*Add list of options to each control */
            currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);
            ReloadHeaderEditorLocationsList(currentTextArchive.messages);

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    areaIconComboBox.Enabled = false;
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject("dpareaicon");
                    areaSettingsLabel.Text = "Show nametag:";
                    cameraComboBox.Items.Clear();
                    musicDayComboBox.Items.Clear();
                    musicNightComboBox.Items.Clear();
                    areaSettingsComboBox.Items.Clear();
                    cameraComboBox.Items.AddRange(PokeDatabase.CameraAngles.DPPtCameraDict.Values.ToArray());
                    musicDayComboBox.Items.AddRange(PokeDatabase.MusicDB.DPMusicDict.Values.ToArray());
                    musicNightComboBox.Items.AddRange(PokeDatabase.MusicDB.DPMusicDict.Values.ToArray());
                    areaSettingsComboBox.Items.AddRange(PokeDatabase.ShowName.DPShowNameValues);
                    weatherComboBox.Items.AddRange(PokeDatabase.Weather.DPWeatherDict.Values.ToArray());
                    wildPokeUpDown.Maximum = 65535;

                    battleBackgroundLabel.Location = new Point(battleBackgroundLabel.Location.X - 25, battleBackgroundLabel.Location.Y - 8);
                    battleBackgroundUpDown.Location = new Point(battleBackgroundUpDown.Location.X - 25, battleBackgroundUpDown.Location.Y - 8);
                    break;
                case gFamEnum.Plat:
                    areaSettingsLabel.Text = "Show nametag:";
                    areaIconComboBox.Items.Clear();
                    cameraComboBox.Items.Clear();
                    musicDayComboBox.Items.Clear();
                    musicNightComboBox.Items.Clear();
                    areaSettingsComboBox.Items.Clear();
                    weatherComboBox.Items.Clear();
                    areaIconComboBox.Items.AddRange(PokeDatabase.Area.PtAreaIconValues);
                    cameraComboBox.Items.AddRange(PokeDatabase.CameraAngles.DPPtCameraDict.Values.ToArray());
                    musicDayComboBox.Items.AddRange(PokeDatabase.MusicDB.PtMusicDict.Values.ToArray());
                    musicNightComboBox.Items.AddRange(PokeDatabase.MusicDB.PtMusicDict.Values.ToArray());
                    areaSettingsComboBox.Items.AddRange(PokeDatabase.ShowName.PtShowNameValues);
                    weatherComboBox.Items.AddRange(PokeDatabase.Weather.PtWeatherDict.Values.ToArray());
                    wildPokeUpDown.Maximum = 65535;

                    battleBackgroundLabel.Location = new Point(battleBackgroundLabel.Location.X - 25, battleBackgroundLabel.Location.Y - 8);
                    battleBackgroundUpDown.Location = new Point(battleBackgroundUpDown.Location.X - 25, battleBackgroundUpDown.Location.Y - 8);
                    break;
                default:
                    areaSettingsLabel.Text = "Area Settings:";
                    areaIconComboBox.Items.Clear();
                    cameraComboBox.Items.Clear();
                    areaSettingsComboBox.Items.Clear();
                    musicDayComboBox.Items.Clear();
                    musicNightComboBox.Items.Clear();
                    weatherComboBox.Items.Clear();
                    areaIconComboBox.Items.AddRange(PokeDatabase.Area.HGSSAreaIconsDict.Values.ToArray());
                    cameraComboBox.Items.AddRange(PokeDatabase.CameraAngles.HGSSCameraDict.Values.ToArray());
                    areaSettingsComboBox.Items.AddRange(PokeDatabase.Area.HGSSAreaProperties);
                    musicDayComboBox.Items.AddRange(PokeDatabase.MusicDB.HGSSMusicDict.Values.ToArray());
                    musicNightComboBox.Items.AddRange(PokeDatabase.MusicDB.HGSSMusicDict.Values.ToArray());
                    weatherComboBox.Items.AddRange(PokeDatabase.Weather.HGSSWeatherDict.Values.ToArray());
                    wildPokeUpDown.Maximum = 255;

                    followModeComboBox.Visible = true;
                    followModeLabel.Visible = true;
                    johtoRadioButton.Visible = true;
                    kantoRadioButton.Visible = true;

                    flag6CheckBox.Visible = true;
                    flag5CheckBox.Visible = true;
                    flag4CheckBox.Visible = true;
                    flag6CheckBox.Text = "Flag ?";
                    flag5CheckBox.Text = "Flag ?";
                    flag4CheckBox.Text = "Flag ?";

                    worldmapCoordsGroupBox.Enabled = true;
                    break;
            }
            if (headerListBox.Items.Count > 0) {
                headerListBox.SelectedIndex = 0;
            }
            statusLabel.Text = "Ready";
        }
        private void addHeaderBTN_Click(object sender, EventArgs e) {
            // Add new file in the dynamic headers directory
            string sourcePath = RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + "0000";
            string destPath = RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + RomInfo.GetHeaderCount().ToString("D4");
            File.Copy(sourcePath, destPath);

            // Add row to internal names table
            string nameString = "4E 45 57 4D 41 50 00 00 00 00 00 00 00 00 00 00";
            DSUtils.WriteToFile(RomInfo.internalNamesLocation, DSUtils.HexStringToByteArray(nameString), (uint)RomInfo.GetHeaderCount() * RomInfo.internalNameLength);

            // Update headers ListBox and internal names list
            const string newmap = "NEWMAP";
            headerListBox.Items.Add(headerListBox.Items.Count + MapHeader.nameSeparator + " " + newmap);
            headerListBoxNames.Add(headerListBox.Items.Count + MapHeader.nameSeparator + " " + newmap);
            internalNames.Add(newmap);

            // Select new header
            headerListBox.SelectedIndex = headerListBox.Items.Count - 1;
        }
        private void removeLastHeaderBTN_Click(object sender, EventArgs e) {
            /* Check if currently selected file is the last one, and in that case select the one before it */
            int lastIndex = headerListBox.Items.Count - 1;

            if (lastIndex > 0) { //there are at least 2 elements
                if (headerListBox.SelectedIndex == lastIndex) {
                    headerListBox.SelectedIndex--;
                }

                /* Physically delete last header file */
                File.Delete(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + lastIndex.ToString("D4"));

                /* Remove item from collections */
                headerListBox.Items.RemoveAt(lastIndex);
                internalNames.RemoveAt(lastIndex);
                headerListBoxNames.RemoveAt(lastIndex);
            } else {
                MessageBox.Show("You must have at least one header!", "Can't delete last header", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void areaDataUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
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
            if (disableHandlers) {
                return;
            }

            string imageName;
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    break;
                case gFamEnum.Plat:
                    ((HeaderPt)currentHeader).areaIcon = (byte)areaIconComboBox.SelectedIndex;
                    imageName = "areaicon0" + areaIconComboBox.SelectedIndex.ToString();
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                    break;
                default:
                    ((HeaderHGSS)currentHeader).areaIcon = (byte)areaIconComboBox.SelectedIndex;
                    imageName = PokeDatabase.System.AreaPics.hgssAreaPicDict[areaIconComboBox.SelectedIndex];
                    areaIconPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                    break;
            }
        }
        private void eventFileUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentHeader.eventFileID = (ushort)eventFileUpDown.Value;
        }
        private void battleBackgroundUpDown_ValueChanged(object sender, EventArgs e) {
            currentHeader.battleBackground = (byte)battleBackgroundUpDown.Value;
        }
        private void followModeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                HeaderHGSS currentHeaderHGSS = (HeaderHGSS)currentHeader;
                currentHeaderHGSS.followMode = (byte)followModeComboBox.SelectedIndex;
            }
        }

        private void kantoRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                HeaderHGSS currentHeaderHGSS = (HeaderHGSS)currentHeader;
                currentHeaderHGSS.kantoFlag = kantoRadioButton.Checked;
            }
        }
        private void headerFlagsCheckBoxes_CheckedChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            byte flagVal = 0;
            if (flag0CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 0);

            if (flag1CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 1);

            if (flag2CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 2);

            if (flag3CheckBox.Checked)
                flagVal += (byte)Math.Pow(2, 3);

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                if (flag4CheckBox.Checked)
                    flagVal += (byte)Math.Pow(2, 4);
                if (flag5CheckBox.Checked)
                    flagVal += (byte)Math.Pow(2, 5);
                if (flag6CheckBox.Checked)
                    flagVal += (byte)Math.Pow(2, 6);
                //if (flag7CheckBox.Checked)
                //    flagVal += (byte)Math.Pow(2, 7);
            }
            currentHeader.flags = flagVal;
        }
        private void headerListBox_SelectedValueChanged(object sender, EventArgs e) {
            if (disableHandlers || headerListBox.SelectedIndex < 0) {
                return;
            }

            /* Obtain current header ID from listbox*/
            ushort headerNumber = ushort.Parse(headerListBox.SelectedItem.ToString().Substring(0, internalNames.Count.ToString().Length));

            /* Check if dynamic headers patch has been applied, and load header from arm9 or a/0/5/0 accordingly */
            if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                currentHeader = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + headerNumber.ToString("D4"), headerNumber, 0);
            } else {
                currentHeader = MapHeader.LoadFromARM9(headerNumber);
            }
            RefreshHeaderEditorFields();
        }

        private void RefreshHeaderEditorFields() {
            /* Setup controls for common fields across headers */
            if (currentHeader == null) {
                return;
            }

            internalNameBox.Text = internalNames[currentHeader.ID];
            matrixUpDown.Value = currentHeader.matrixID;
            areaDataUpDown.Value = currentHeader.areaDataID;
            scriptFileUpDown.Value = currentHeader.scriptFileID;
            levelScriptUpDown.Value = currentHeader.levelScriptID;
            eventFileUpDown.Value = currentHeader.eventFileID;
            textFileUpDown.Value = currentHeader.textArchiveID;
            wildPokeUpDown.Value = currentHeader.wildPokémon;
            weatherUpDown.Value = currentHeader.weatherID;
            cameraUpDown.Value = currentHeader.cameraAngleID;
            battleBackgroundUpDown.Value = currentHeader.battleBackground;

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                areaSettingsComboBox.SelectedIndex = ((HeaderHGSS)currentHeader).locationType;
            }

            openWildEditorWithIdButton.Enabled = currentHeader.wildPokémon != RomInfo.nullEncounterID;

            /* Setup controls for fields with version-specific differences */
            try {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP: {
                            HeaderDP h = (HeaderDP)currentHeader;

                            locationNameComboBox.SelectedIndex = h.locationName;
                            musicDayUpDown.Value = h.musicDayID;
                            musicNightUpDown.Value = h.musicNightID;
                            areaSettingsComboBox.SelectedIndex = areaSettingsComboBox.FindString("[" + $"{currentHeader.locationSpecifier:D3}");
                            break;
                        }
                    case gFamEnum.Plat: {
                            HeaderPt h = (HeaderPt)currentHeader;

                            areaIconComboBox.SelectedIndex = h.areaIcon;
                            locationNameComboBox.SelectedIndex = h.locationName;
                            musicDayUpDown.Value = h.musicDayID;
                            musicNightUpDown.Value = h.musicNightID;
                            areaSettingsComboBox.SelectedIndex = areaSettingsComboBox.FindString("[" + $"{currentHeader.locationSpecifier:D3}");
                            break;
                        }
                    default: {
                            HeaderHGSS h = (HeaderHGSS)currentHeader;

                            areaIconComboBox.SelectedIndex = h.areaIcon;
                            locationNameComboBox.SelectedIndex = h.locationName;
                            musicDayUpDown.Value = h.musicDayID;
                            musicNightUpDown.Value = h.musicNightID;
                            worldmapXCoordUpDown.Value = h.worldmapX;
                            worldmapYCoordUpDown.Value = h.worldmapY;
                            followModeComboBox.SelectedIndex = h.followMode;
                            kantoRadioButton.Checked = h.kantoFlag;
                            johtoRadioButton.Checked = !h.kantoFlag;
                            break;
                        }
                }
            } catch (ArgumentOutOfRangeException) {
                MessageBox.Show("This header contains an irregular/unsupported field.", "Error loading header file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            RefreshFlags();
            updateWeatherPicAndComboBox();
            updateCameraPicAndComboBox();
        }
        private void RefreshFlags() {
            BitArray ba = new BitArray(new byte[] { currentHeader.flags });

            flag0CheckBox.Checked = ba[0];
            flag1CheckBox.Checked = ba[1];
            flag2CheckBox.Checked = ba[2];
            flag3CheckBox.Checked = ba[3];

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                flag4CheckBox.Checked = ba[4];
                flag5CheckBox.Checked = ba[5];
                flag6CheckBox.Checked = ba[6];
                //flag6CheckBox.Checked = ba[7];
            }
        }
        private void eventsTabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (eventsTabControl.SelectedTab == signsTabPage) {
                if (spawnablesListBox.Items.Count > 0) {
                    spawnablesListBox.SelectedIndex = 0;
                }
            } else if (eventsTabControl.SelectedTab == overworldsTabPage) {
                if (overworldsListBox.Items.Count > 0) {
                    overworldsListBox.SelectedIndex = 0;
                }
            } else if (eventsTabControl.SelectedTab == warpsTabPage) {
                if (warpsListBox.Items.Count > 0) {
                    warpsListBox.SelectedIndex = 0;
                }
            } else if (eventsTabControl.SelectedTab == triggersTabPage) {
                if (triggersListBox.Items.Count > 0) {
                    triggersListBox.SelectedIndex = 0;
                }
            }
        }
        private void headerListBox_Leave(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            headerListBox.Refresh();
        }
        private void levelScriptUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentHeader.levelScriptID = (ushort)levelScriptUpDown.Value;
        }
        private void mapNameComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    ((HeaderDP)currentHeader).locationName = (ushort)locationNameComboBox.SelectedIndex;
                    break;
                case gFamEnum.Plat:
                    ((HeaderPt)currentHeader).locationName = (byte)locationNameComboBox.SelectedIndex;
                    break;
                default:
                    ((HeaderHGSS)currentHeader).locationName = (byte)locationNameComboBox.SelectedIndex;
                    break;
            }
        }
        private void matrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentHeader.matrixID = (ushort)matrixUpDown.Value;
        }
        private void musicDayComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.DPMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
                case gFamEnum.Plat:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.PtMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
                default:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.HGSSMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
            }
        }
        private void musicNightComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.DPMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
                case gFamEnum.Plat:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.PtMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
                default:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.HGSSMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
            }
        }
        private void musicDayUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            disableHandlers = true;
            ushort updValue = (ushort)((NumericUpDown)sender).Value;
            currentHeader.musicDayID = updValue;
            try {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.DPMusicDict[updValue];
                        break;
                    case gFamEnum.Plat:
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
            if (disableHandlers) {
                return;
            }

            disableHandlers = true;
            ushort updValue = (ushort)((NumericUpDown)sender).Value;
            currentHeader.musicNightID = updValue;
            try {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.DPMusicDict[updValue];
                        break;
                    case gFamEnum.Plat:
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
        private void worldmapXCoordUpDown_ValueChanged(object sender, EventArgs e) {
            ((HeaderHGSS)currentHeader).worldmapX = (byte)worldmapXCoordUpDown.Value;
        }
        private void worldmapYCoordUpDown_ValueChanged(object sender, EventArgs e) {
            ((HeaderHGSS)currentHeader).worldmapY = (byte)worldmapYCoordUpDown.Value;
        }
        private void updateWeatherPicAndComboBox() {
            if (disableHandlers) {
                return;
            }

            /* Update Weather Combobox*/
            disableHandlers = true;
            try {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.DPWeatherDict[currentHeader.weatherID];
                        break;
                    case gFamEnum.Plat:
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
                Dictionary<byte[], string> dict;
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                        dict = PokeDatabase.System.WeatherPics.dpWeatherImageDict;
                        break;
                    case gFamEnum.Plat:
                        dict = PokeDatabase.System.WeatherPics.ptWeatherImageDict;
                        break;
                    default:
                        dict = PokeDatabase.System.WeatherPics.hgssweatherImageDict;
                        break;
                }

                bool found = false;
                foreach (KeyValuePair<byte[], string> dictEntry in dict) {
                    if (Array.IndexOf(dictEntry.Key, (byte)weatherUpDown.Value) >= 0) {
                        weatherPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(dictEntry.Value);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    throw new KeyNotFoundException();
            } catch (KeyNotFoundException) {
                weatherPictureBox.Image = null;
            }
        }
        private void updateCameraPicAndComboBox() {
            if (disableHandlers) {
                return;
            }

            /* Update Camera Combobox*/
            disableHandlers = true;
            try {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                        cameraComboBox.SelectedItem = PokeDatabase.CameraAngles.DPPtCameraDict[currentHeader.cameraAngleID];
                        break;
                    case gFamEnum.Plat:
                        cameraComboBox.SelectedItem = PokeDatabase.CameraAngles.DPPtCameraDict[currentHeader.cameraAngleID];
                        break;
                    default:
                        cameraComboBox.SelectedItem = PokeDatabase.CameraAngles.HGSSCameraDict[currentHeader.cameraAngleID];
                        break;
                }
            } catch (KeyNotFoundException) {
                cameraComboBox.SelectedItem = null;
            }
            disableHandlers = false;

            /* Update Camera Picture */
            string imageName;
            try {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "dpcamera" + cameraUpDown.Value.ToString();
                        break;
                    case gFamEnum.Plat:
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "ptcamera" + cameraUpDown.Value.ToString();
                        break;
                    default:
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "hgsscamera" + cameraUpDown.Value.ToString();
                        break;
                }
                cameraPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
            } catch (NullReferenceException) {
                MessageBox.Show("The current header uses an unrecognized camera.\n", "Unknown camera settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void weatherComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || weatherComboBox.SelectedIndex < 0) {
                return;
            }

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    weatherUpDown.Value = PokeDatabase.Weather.DPWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
                case gFamEnum.Plat:
                    weatherUpDown.Value = PokeDatabase.Weather.PtWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
                default:
                    weatherUpDown.Value = PokeDatabase.Weather.HGSSWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
            }
            currentHeader.weatherID = (byte)weatherUpDown.Value;
        }
        private void weatherUpDown_ValueChanged(object sender, EventArgs e) {
            currentHeader.weatherID = (byte)weatherUpDown.Value;
            updateWeatherPicAndComboBox();
        }
        private void cameraComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || cameraComboBox.SelectedIndex < 0) {
                return;
            }

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    cameraUpDown.Value = PokeDatabase.CameraAngles.DPPtCameraDict.Keys.ElementAt(cameraComboBox.SelectedIndex);
                    break;
                case gFamEnum.Plat:
                    cameraUpDown.Value = PokeDatabase.CameraAngles.DPPtCameraDict.Keys.ElementAt(cameraComboBox.SelectedIndex);
                    break;
                default:
                    cameraUpDown.Value = PokeDatabase.CameraAngles.HGSSCameraDict.Keys.ElementAt(cameraComboBox.SelectedIndex);
                    break;
            }
            currentHeader.cameraAngleID = (byte)cameraUpDown.Value;
        }
        private void cameraUpDown_ValueChanged(object sender, EventArgs e) {
            currentHeader.cameraAngleID = (byte)cameraUpDown.Value;
            updateCameraPicAndComboBox();
        }
        private void openAreaDataButton_Click(object sender, EventArgs e) {
            if (!nsbtxEditorIsReady) {
                SetupNSBTXEditor();
                nsbtxEditorIsReady = true;
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

            if (matrixUpDown.Value != 0) {
                eventAreaDataUpDown.Value = areaDataUpDown.Value; // Use Area Data for textures if matrix is not 0
            }

            eventMatrixUpDown.Value = matrixUpDown.Value; // Open the right matrix in event editor
            selectEventComboBox.SelectedIndex = (int)eventFileUpDown.Value; // Select event file
            mainTabControl.SelectedTab = eventEditorTabPage;

            eventMatrixUpDown_ValueChanged(null, null);
            CenterEventViewOnEntities();
        }
        private void openMatrixButton_Click(object sender, EventArgs e) {
            if (!matrixEditorIsReady) {
                SetupMatrixEditor();
                matrixEditorIsReady = true;
            }
            mainTabControl.SelectedTab = matrixEditorTabPage;
            int matrixNumber = (int)matrixUpDown.Value;
            selectMatrixComboBox.SelectedIndex = matrixNumber;

            if (currentMatrix.hasHeadersSection) {
                matrixTabControl.SelectedTab = headersTabPage;

                //Autoselect cell containing current header, if such cell exists [and if current matrix has headers sections]
                for (int i = 0; i < headersGridView.RowCount; i++) {
                    for (int j = 0; j < headersGridView.ColumnCount; j++) {
                        if (currentHeader.ID.ToString() == headersGridView.Rows[i].Cells[j].Value.ToString()) {
                            headersGridView.CurrentCell = headersGridView.Rows[i].Cells[j];
                            return;
                        }
                    }
                }
            }
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
            /* Check if dynamic headers patch has been applied, and save header to arm9 or a/0/5/0 accordingly */
            if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                DSUtils.WriteToFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + currentHeader.ID.ToString("D4"), currentHeader.ToByteArray(), 0, 0, fmode: FileMode.Create);
            } else {
                uint headerOffset = (uint)(RomInfo.headerTableOffset + MapHeader.length * currentHeader.ID);
                DSUtils.ARM9.WriteBytes(currentHeader.ToByteArray(), headerOffset);
            }
            disableHandlers = true;

            updateCurrentInternalName();
            updateHeaderNameShown(headerListBox.SelectedIndex);
            headerListBox.Focus();
            disableHandlers = false;
        }
        private void updateCurrentInternalName() {
            /* Update internal name according to internalNameBox text*/
            if (currentHeader.ID != null) {
                ushort headerID = (ushort)currentHeader.ID;

                using (DSUtils.EasyWriter writer = new DSUtils.EasyWriter(RomInfo.internalNamesLocation, headerID * RomInfo.internalNameLength)) { 
                    writer.Write(Encoding.ASCII.GetBytes(internalNameBox.Text.PadRight(16, '\0')));
                }

                internalNames[headerID] = internalNameBox.Text;
                string elem = headerID.ToString("D3") + MapHeader.nameSeparator + internalNames[headerID];
                headerListBoxNames[headerID] = elem;

                if (eventEditorIsReady) {
                    eventEditorWarpHeaderListBox.Items[headerID] = elem;
                }
            }
        }
        private void updateHeaderNameShown(int thisIndex) {
            disableHandlers = true;
            string val = (string)(headerListBox.Items[thisIndex] = headerListBoxNames[(ushort)currentHeader.ID]);
            if (eventEditorIsReady) {
                eventEditorWarpHeaderListBox.Items[thisIndex] = val;
            }
            disableHandlers = false;
        }
        private void resetButton_Click(object sender, EventArgs e) {
            searchLocationTextBox.Clear();
            HeaderSearch.ResetResults(headerListBox, headerListBoxNames, prependNumbers: false);
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

                /* Check if dynamic headers patch has been applied, and load header from arm9 or a/0/5/0 accordingly */
                for (ushort i = 0; i < internalNames.Count; i++) {
                    MapHeader h;
                    if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                        h = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + i.ToString("D4"), i, 0);
                    } else {
                        h = MapHeader.LoadFromARM9(i);
                    }

                    string locationName = "";
                    switch (RomInfo.gameFamily) {
                        case gFamEnum.DP:
                            locationName = locationNameComboBox.Items[((HeaderDP)h).locationName].ToString();
                            break;
                        case gFamEnum.Plat:
                            locationName = locationNameComboBox.Items[((HeaderPt)h).locationName].ToString();
                            break;
                        case gFamEnum.HGSS:
                            locationName = locationNameComboBox.Items[((HeaderHGSS)h).locationName].ToString();
                            break;
                    }

                    if (locationName.IndexOf(searchLocationTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                        headerListBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + internalNames[i]);
                        noResult = false;
                    }
                }


                if (noResult) {
                    headerListBox.Items.Add("No result for " + '"' + searchLocationTextBox.Text + '"');
                    headerListBox.Enabled = false;
                } else {
                    headerListBox.SelectedIndex = 0;
                    headerListBox.Enabled = true;
                }
            } else if (headerListBox.Items.Count < internalNames.Count) {
                HeaderSearch.ResetResults(headerListBox, headerListBoxNames, prependNumbers: false);
            }
        }
        private void scriptFileUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentHeader.scriptFileID = (ushort)scriptFileUpDown.Value;
        }
        private void areaSettingsComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || areaSettingsComboBox.SelectedItem is null) {
                return;
            }

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    currentHeader.locationSpecifier = Byte.Parse(areaSettingsComboBox.SelectedItem.ToString().Substring(1, 3));
                    break;
                case gFamEnum.HGSS:
                    HeaderHGSS ch = (HeaderHGSS)currentHeader;
                    ch.locationType = (byte)areaSettingsComboBox.SelectedIndex;
                    //areaImageLabel.Text = "Area icon";
                    //areaIconComboBox.Enabled = true;
                    //areaIconPictureBox.Visible = true;
                    break;
            }
        }
        private void textFileUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentHeader.textArchiveID = (ushort)textFileUpDown.Value;
        }

        private void wildPokeUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

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
            OpenFileDialog of = new OpenFileDialog {
                Filter = MapHeader.DefaultFilter
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            MapHeader h = null;
            try {
                if (new FileInfo(of.FileName).Length > 48)
                    throw new FileFormatException();

                h = MapHeader.LoadFromFile(of.FileName, currentHeader.ID, 0);
                if (h == null)
                    throw new FileFormatException();

            } catch (FileFormatException) {
                MessageBox.Show("The file you tried to import is either malformed or not a Header file.\nNo changes have been made.",
                        "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentHeader = h;
            /* Check if dynamic headers patch has been applied, and save header to arm9 or a/0/5/0 accordingly */
            if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                DSUtils.WriteToFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + currentHeader.ID.ToString("D4"), currentHeader.ToByteArray(), 0, 0, fmode: FileMode.Create);
            } else {
                uint headerOffset = (uint)(RomInfo.headerTableOffset + MapHeader.length * currentHeader.ID);
                DSUtils.ARM9.WriteBytes(currentHeader.ToByteArray(), headerOffset);
            }

            try {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(of.FileName, MapHeader.length + 8)) {
                    internalNameBox.Text = Encoding.UTF8.GetString(reader.ReadBytes(RomInfo.internalNameLength));
                }
                updateCurrentInternalName();
                updateHeaderNameShown(headerListBox.SelectedIndex);
            } catch (EndOfStreamException) { }

            RefreshHeaderEditorFields();
        }

        private void exportHeaderToFileButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog {
                Filter = MapHeader.DefaultFilter,
                FileName = "Header " + currentHeader.ID + " - " + internalNames[currentHeader.ID] + " (" + locationNameComboBox.SelectedItem.ToString() + ")"
            };

            if (sf.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            using (DSUtils.EasyWriter writer = new DSUtils.EasyWriter(sf.FileName)) {
                writer.Write(currentHeader.ToByteArray()); //Write full header
                writer.Write((byte)0x00); //Padding
                writer.Write(Encoding.UTF8.GetBytes("INTNAME")); //Signature
                writer.Write(Encoding.UTF8.GetBytes(internalNames[currentHeader.ID])); //Save Internal name
            }
        }

        #region CopyPaste Buttons
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
        int followingPokeCopy;
        bool kantoFlagCopy;

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
            flagsCopy = currentHeader.flags;
            followingPokeCopy = followModeComboBox.SelectedIndex;
            kantoFlagCopy = kantoRadioButton.Checked;

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

            pasteMapSettingsButton.Enabled = true;

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
            followingPokeCopy = followModeComboBox.SelectedIndex;
            kantoFlagCopy = kantoRadioButton.Checked;
            pasteMapSettingsButton.Enabled = true;
        }

        /* Paste Buttons */
        private void pasteHeaderButton_Click(object sender, EventArgs e) {
            locationNameComboBox.SelectedIndex = locationNameCopy;
            internalNameBox.Text = internalNameCopy;
            wildPokeUpDown.Value = encountersIDCopy;

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    areaSettingsComboBox.SelectedIndex = shownameCopy;
                    break;
                case gFamEnum.HGSS:
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
            worldmapXCoordUpDown.Value = worldmapXCoordCopy;
            worldmapYCoordUpDown.Value = worldmapYCoordCopy;
            battleBackgroundUpDown.Value = battleBGCopy;
            RefreshFlags();
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
            if (areaIconComboBox.Enabled) { 
                areaIconComboBox.SelectedIndex = areaIconCopy;
            }
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

            followModeComboBox.SelectedIndex = followingPokeCopy;
            kantoRadioButton.Checked = kantoFlagCopy;
            RefreshFlags();
        }
        #endregion

        #endregion

        #region Matrix Editor

        GameMatrix currentMatrix;

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
        private (Color background, Color foreground) FormatMapCell(uint cellValue) {
            foreach (KeyValuePair<List<uint>, (Color background, Color foreground)> entry in RomInfo.MapCellsColorDictionary) {
                if (entry.Key.Contains(cellValue))
                    return entry.Value;
            }
            return (Color.White, Color.Black);
        }
        private void GenerateMatrixTables() {
            /* Generate table columns */
            if (currentMatrix is null) {
                return;
            }

            for (int i = 0; i < currentMatrix.width; i++) {
                headersGridView.Columns.Add("Column" + i, i.ToString("D"));
                headersGridView.Columns[i].Width = 32; // Set column size
                headersGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                headersGridView.Columns[i].Frozen = false;

                heightsGridView.Columns.Add("Column" + i, i.ToString("D"));
                heightsGridView.Columns[i].Width = 21; // Set column size
                heightsGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                heightsGridView.Columns[i].Frozen = false;

                mapFilesGridView.Columns.Add("Column" + i, i.ToString("D"));
                mapFilesGridView.Columns[i].Width = 32; // Set column size
                mapFilesGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                mapFilesGridView.Columns[i].Frozen = false;
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

            if (currentMatrix.hasHeadersSection) {
                matrixTabControl.TabPages.Add(headersTabPage);
            }

            if (currentMatrix.hasHeightsSection) {
                matrixTabControl.TabPages.Add(heightsTabPage);
            }
        }
        #endregion
        private void SetupMatrixEditor() {
            statusLabel.Text = "Setting up Matrix Editor...";
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.matrices });

            disableHandlers = true;

            /* Add matrix entries to ComboBox */
            selectMatrixComboBox.Items.Clear();
            selectMatrixComboBox.Items.Add("Matrix 0 - Main");
            for (int i = 1; i < romInfo.GetMatrixCount(); i++) {
                selectMatrixComboBox.Items.Add(new GameMatrix(i));
            }

            if (!ReadColorTable(Properties.Settings.Default.lastColorTablePath, silent: true)) {
                RomInfo.LoadMapCellsColorDictionary();
            }
            RomInfo.SetupSpawnSettings();

            disableHandlers = false;
            selectMatrixComboBox.SelectedIndex = 0;
            statusLabel.Text = "Ready";
        }
        private void addHeaderSectionButton_Click(object sender, EventArgs e) {
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
            GameMatrix blankMatrix = new GameMatrix();

            /* Add new matrix file to matrix folder */
            blankMatrix.SaveToFile(RomInfo.gameDirs[DirNames.matrices].unpackedDir + "\\" + romInfo.GetMatrixCount().ToString("D4"), false);

            /* Update ComboBox*/
            selectMatrixComboBox.Items.Add( selectMatrixComboBox.Items.Count.ToString() + blankMatrix );
            selectMatrixComboBox.SelectedIndex = selectMatrixComboBox.Items.Count - 1;
        }
        private void exportMatrixButton_Click(object sender, EventArgs e) {
            currentMatrix.SaveToFileExplorePath("Matrix " + selectMatrixComboBox.SelectedIndex);
        }
        private void saveMatrixButton_Click(object sender, EventArgs e) {
            currentMatrix.SaveToFileDefaultDir(selectMatrixComboBox.SelectedIndex);
            GameMatrix saved = new GameMatrix(selectMatrixComboBox.SelectedIndex);
            selectMatrixComboBox.Items[selectMatrixComboBox.SelectedIndex] = saved.ToString();
            eventMatrix = saved;
        }
        private void headersGridView_SelectionChanged(object sender, EventArgs e) {
            DisplaySelection(headersGridView.SelectedCells);
        }

        private void heightsGridView_SelectionChanged(object sender, EventArgs e) {
            DisplaySelection(heightsGridView.SelectedCells);
        }

        private void mapFilesGridView_SelectionChanged(object sender, EventArgs e) {
            DisplaySelection(mapFilesGridView.SelectedCells);
        }
        private void DisplaySelection(DataGridViewSelectedCellCollection selectedCells) {
            if (selectedCells.Count > 0) {
                statusLabel.Text = "Selection:   " + selectedCells[0].ColumnIndex + ", " + selectedCells[0].RowIndex;
            }
        }
        private void headersGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (headerListBox.Items.Count < internalNames.Count) {
                HeaderSearch.ResetResults(headerListBox, headerListBoxNames, prependNumbers: false);
            }

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) {
                int headerNumber = Convert.ToInt32(headersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                headerListBox.SelectedIndex = headerNumber;
                mainTabControl.SelectedTab = headerEditorTabPage;
            }
        }
        private void headersGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (disableHandlers) {
                return;
            }
            if (e.RowIndex > -1 && e.ColumnIndex > -1) {
                /* If input is junk, use 0000 as placeholder value */
                ushort cellValue;
                try {
                    if (!ushort.TryParse(headersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out cellValue)) {
                        throw new NullReferenceException();
                    }
                } catch (NullReferenceException) {
                    cellValue = 0;
                }
                /* Change value in matrix object */
                currentMatrix.headers[e.RowIndex, e.ColumnIndex] = cellValue;
            }
        }
        private void headersGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.Value is null) {
                return;
            }

            disableHandlers = true;

            /* Format table cells corresponding to border maps or void */
            if (!ushort.TryParse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out ushort colorValue)) {
                colorValue = GameMatrix.EMPTY;
            }

            (Color back, Color fore) = FormatMapCell(colorValue);
            e.CellStyle.BackColor = back;
            e.CellStyle.ForeColor = fore;

            /* If invalid input is entered, show 00 */
            if (!ushort.TryParse(headersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out _)) {
                e.Value = 0;
            }

            disableHandlers = false;

        }
        private void heightsGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            if (e.RowIndex > -1 && e.ColumnIndex > -1) {
                /* If input is junk, use 00 as placeholder value */
                byte cellValue = 0;
                try {
                    cellValue = byte.Parse(heightsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                } catch { }

                /* Change value in matrix object */
                currentMatrix.altitudes[e.RowIndex, e.ColumnIndex] = cellValue;
            }
        }
        private void widthUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
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
                        mapFilesGridView.Rows[j].Cells[index].Value = GameMatrix.EMPTY;
                    }
                }
            }

            /* Modify matrix object */
            currentMatrix.ResizeMatrix((int)heightUpDown.Value, (int)widthUpDown.Value);
            disableHandlers = false;
        }
        private void heightUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            disableHandlers = true;

            /* Add or remove rows in DataGridView control */
            int delta = (int)heightUpDown.Value - currentMatrix.height;
            for (int i = 0; i < Math.Abs(delta); i++) {
                if (delta < 0) { // Remove rows
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
                        mapFilesGridView.Rows[index].Cells[j].Value = GameMatrix.EMPTY;
                    }
                }
            }

            /* Modify matrix object */
            currentMatrix.ResizeMatrix((int)heightUpDown.Value, (int)widthUpDown.Value);
            disableHandlers = false;
        }
        private void heightsGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            if (e.Value is null) {
                return;
            }

            disableHandlers = true;

            /* Format table cells corresponding to border maps or void */
            ushort colorValue = 0;
            try {
                colorValue = ushort.Parse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            } catch { }

            (Color back, Color fore) = FormatMapCell(colorValue);
            e.CellStyle.BackColor = back;
            e.CellStyle.ForeColor = fore;

            /* If invalid input is entered, show 00 */
            byte cellValue = 0;
            try {
                cellValue = byte.Parse(heightsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            } catch { }

            e.Value = cellValue;
            disableHandlers = false;
        }
        private void importMatrixButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .mtx file */
            if (selectMatrixComboBox.SelectedIndex == 0) {
                statusLabel.Text = "Awaiting user response...";
                DialogResult d = MessageBox.Show("Replacing a matrix - especially Matrix 0 - with a new file is risky.\n" +
                    "Do not do it unless you are absolutely sure.\nProceed?", "Risky operation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (d == DialogResult.No) {
                    return;
                }
            }

            OpenFileDialog importMatrix = new OpenFileDialog {
                Filter = GameMatrix.DefaultFilter
            };
            if (importMatrix.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update matrix object in memory */
            currentMatrix = new GameMatrix(new FileStream(importMatrix.FileName, FileMode.Open));

            /* Refresh DataGridView tables */
            ClearMatrixTables();
            GenerateMatrixTables();

            /* Setup matrix editor controls */
            disableHandlers = true;
            matrixNameTextBox.Text = currentMatrix.name;
            widthUpDown.Value = currentMatrix.width;
            heightUpDown.Value = currentMatrix.height;
            disableHandlers = false;

            /* Display success message */
            MessageBox.Show("Matrix imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusLabel.Text = "Ready";
        }
        private void mapFilesGridView_CellMouseDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) {
                if (currentMatrix.maps[e.RowIndex, e.ColumnIndex] == GameMatrix.EMPTY) {
                    MessageBox.Show("You can't load an empty map.\nSelect a valid map and try again.\n\n" +
                        "If you only meant to change the value of this cell, wait some time between one mouse click and the other.\n" +
                        "Alternatively, highlight the cell and press F2 on your keyboard.",
                        "User attempted to load VOID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!mapEditorIsReady) {
                    SetupMapEditor();
                    mapOpenGlControl.MouseWheel += new MouseEventHandler(mapOpenGlControl_MouseWheel);
                    mapEditorIsReady = true;
                }

                int mapCount = romInfo.GetMapCount();
                if ( currentMatrix.maps[e.RowIndex, e.ColumnIndex] >= mapCount) {
                    MessageBox.Show("This matrix cell points to a map file that doesn't exist.",
                        "There " + ((mapCount > 1) ? "are only " + mapCount + " map files." : "is only 1 map file."), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                /* Determine area data */
                ushort headerID = 0;
                if (currentMatrix.hasHeadersSection) {
                    headerID = currentMatrix.headers[e.RowIndex, e.ColumnIndex];
                } else {
                    List<string> result = HeaderSearch.AdvancedSearch(0, (ushort)internalNames.Count, internalNames, (int)MapHeader.SearchableFields.MatrixID, (int)HeaderSearch.NumOperators.Equal, selectMatrixComboBox.SelectedIndex.ToString());
                    if (result.Count < 1) {
                        headerID = currentHeader.ID;
                        statusLabel.Text = "This Matrix is not linked to any Header. DSPRE can't determine the most appropriate AreaData (and textures) to use.\nDisplaying Textures from the last selected Header (" + headerID + ")'s AreaData...";
                    } else {
                        if (result.Count > 1) {
                            if (gameFamily.Equals(gFamEnum.DP)) {
                                foreach (string r in result) {
                                    HeaderDP hdp = (HeaderDP)MapHeader.LoadFromARM9(ushort.Parse(r.Split()[0]));
                                    if (hdp.locationName != 0) {
                                        headerID = hdp.ID;
                                        break;
                                    }
                                }
                            } else {
                                foreach (string r in result) {
                                    HeaderPt hpt = (HeaderPt)MapHeader.LoadFromARM9(ushort.Parse(r.Split()[0]), gFamEnum.Plat);
                                    if (hpt.locationName != 0) {
                                        headerID = hpt.ID;
                                        break;
                                    }
                                }
                            }

                            statusLabel.Text = "Multiple Headers are using this Matrix. Header " + headerID + "'s textures are currently being displayed.";
                        } else {
                            headerID = ushort.Parse(result.First().Split()[0]);
                            statusLabel.Text = "Loading Header " + headerID + "'s textures.";
                        }
                    }
                }
                Update();

                if (headerID > internalNames.Count) {
                    MessageBox.Show("This map is associated to a non-existent header.\nThis will lead to unpredictable behaviour and, possibily, problems, if you attempt to load it in game.",
                        "Invalid header", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    headerID = 0;
                }

                /* get texture file numbers from area data */
                MapHeader h;
                if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                    h = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + headerID.ToString("D4"), headerID, 0);
                } else {
                    h = MapHeader.LoadFromARM9(headerID);
                }

                /* Load Map File and switch to Map Editor tab */
                disableHandlers = true;

                AreaData areaData = new AreaData(h.areaDataID);
                selectMapComboBox.SelectedIndex = currentMatrix.maps[e.RowIndex, e.ColumnIndex];
                mapTextureComboBox.SelectedIndex = areaData.mapTileset + 1;
                buildTextureComboBox.SelectedIndex = areaData.buildingsTileset + 1;
                mainTabControl.SelectedTab = mapEditorTabPage;

                if (areaData.areaType == AreaData.TYPE_INDOOR) {
                    interiorbldRadioButton.Checked = true;
                } else {
                    exteriorbldRadioButton.Checked = true;
                }

                disableHandlers = false;
                selectMapComboBox_SelectedIndexChanged(null, null);
            }
        }
        private void mapFilesGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (disableHandlers) {
                return;
            }
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) {
                /* If input is junk, use '\' (FF FF) as placeholder value */
                ushort cellValue = GameMatrix.EMPTY;
                try {
                    cellValue = ushort.Parse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                } catch { }

                /* Change value in matrix object */
                currentMatrix.maps[e.RowIndex, e.ColumnIndex] = cellValue;
            }
        }
        private void mapFilesGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
            disableHandlers = true;

            /* Format table cells corresponding to border maps or void */
            ushort colorValue = GameMatrix.EMPTY;
            try {
                colorValue = ushort.Parse(mapFilesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
            } catch { }

            (Color backColor, Color foreColor) cellColors = FormatMapCell(colorValue);
            e.CellStyle.BackColor = cellColors.backColor;
            e.CellStyle.ForeColor = cellColors.foreColor;

            if (colorValue == GameMatrix.EMPTY)
                e.Value = '-';

            disableHandlers = false;
        }
        private void matrixNameTextBox_TextChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
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
                DialogResult d = MessageBox.Show("Are you sure you want to delete the last Map BIN File?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (d.Equals(DialogResult.Yes)) {
                    /* Delete matrix file */
                    int matrixToDelete = romInfo.GetMatrixCount() - 1;

                    string matrixPath = RomInfo.gameDirs[DirNames.matrices].unpackedDir + "\\" + matrixToDelete.ToString("D4");
                    File.Delete(matrixPath);

                    /* Change selected index if the matrix to be deleted is currently selected */
                    if (selectMatrixComboBox.SelectedIndex == matrixToDelete)
                        selectMatrixComboBox.SelectedIndex--;

                    /* Remove entry from ComboBox, and decrease matrix count */
                    selectMatrixComboBox.Items.RemoveAt(matrixToDelete);
                }
            } else {
                MessageBox.Show("At least one matrix must be kept.", "Can't delete Matrix", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void setSpawnPointButton_Click(object sender, EventArgs e) {
            DataGridViewCell selectedCell = null;
            switch (matrixTabControl.SelectedIndex) {
                case 0: //Maps
                    selectedCell = mapFilesGridView.SelectedCells[0];
                    selectedCell = headersGridView.Rows[selectedCell.RowIndex].Cells[selectedCell.ColumnIndex];
                    break;
                case 1: //Headers
                    selectedCell = headersGridView.SelectedCells[0];
                    break;
                case 2: //Altitudes
                    selectedCell = heightsGridView.SelectedCells[0];
                    selectedCell = headersGridView.Rows[selectedCell.RowIndex].Cells[selectedCell.ColumnIndex];
                    break;
            }

            ushort headerNumber = 0;
            List<string> result = null;
            if (currentMatrix.hasHeadersSection) {
                headerNumber = Convert.ToUInt16(selectedCell.Value);
            } else {
                DialogResult d;
                d = MessageBox.Show("This Matrix doesn't have a Header Tab. " +
                    Environment.NewLine + "Do you want to check if any Header uses this Matrix and choose that one as your Spawn Header? " +
                    Environment.NewLine + "\nChoosing 'No' will pick the last selected Header.", "Couldn't find Header Tab", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (d == DialogResult.Yes) {
                    result = HeaderSearch.AdvancedSearch(0, (ushort)internalNames.Count, internalNames, (int)MapHeader.SearchableFields.MatrixID, (int)HeaderSearch.NumOperators.Equal, selectMatrixComboBox.SelectedIndex.ToString());
                    if (result.Count < 1) {
                        MessageBox.Show("The current Matrix isn't assigned to any Header.\nThe default choice has been set to the last selected Header.", "No result", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        headerNumber = currentHeader.ID;
                    } else if (result.Count == 1) {
                        headerNumber = ushort.Parse(result.First().Split()[0]);
                    } else {
                        MessageBox.Show("Multiple Headers are using this Matrix.\nPick one from the list or reset the filter results to choose a different Header.", "Multiple results", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                } else {
                    headerNumber = currentHeader.ID;
                }
            }

            int matrixX = selectedCell.ColumnIndex;
            int matrixY = selectedCell.RowIndex;

            using (SpawnEditor ed = new SpawnEditor(result, headerListBoxNames, headerNumber, matrixX, matrixY)) {
                ed.ShowDialog();
            }
        }
        private void selectMatrixComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            ClearMatrixTables();
            currentMatrix = new GameMatrix(selectMatrixComboBox.SelectedIndex);
            GenerateMatrixTables();

            /* Setup matrix editor controls */
            disableHandlers = true;
            matrixNameTextBox.Text = currentMatrix.name;
            widthUpDown.Value = currentMatrix.width;
            heightUpDown.Value = currentMatrix.height;
            disableHandlers = false;
        }
        private void importColorTableButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = "DSPRE Color Table File (*.ctb)|*.ctb"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            ReadColorTable(of.FileName, silent: false);
        }

        private bool ReadColorTable(string fileName, bool silent) {
            if (string.IsNullOrWhiteSpace(fileName)) {
                return false;
            }

            string[] fileTableContent = File.ReadAllLines(fileName);

            if (fileTableContent.Length > 0) {
                const string mapKeyword = "[Maplist]";
                const string colorKeyword = "[Color]";
                const string textColorKeyword = "[TextColor]";
                const string dashSeparator = "-";
                string problematicSegment = "incomplete line";

                Dictionary<List<uint>, (Color background, Color foreground)> colorsDict = new Dictionary<List<uint>, (Color background, Color foreground)>();
                List<string> linesWithErrors = new List<string>();

                for (int i = 0; i < fileTableContent.Length; i++) {
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

                            colorsDict.Add(mapList, (Color.FromArgb(r, g, b), Color.FromName(lineParts[j++])));
                        } catch {
                            if (!silent) {
                                linesWithErrors.Add(i + 1 + " (err. " + problematicSegment + ")\n");
                            }
                            continue;
                        }
                    }
                }
                colorsDict.Add(new List<uint> { GameMatrix.EMPTY }, (Color.Black, Color.White));

                string errorMsg = "";
                MessageBoxIcon iconType = MessageBoxIcon.Information;
                
                if (!silent) {
                    if (linesWithErrors.Count > 0) {
                        errorMsg = "\nHowever, the following lines couldn't be parsed correctly:\n";

                        foreach (string s in linesWithErrors) {
                            errorMsg += "- Line " + s;
                        }

                        iconType = MessageBoxIcon.Warning;
                    }
                }
                romInfo.SetMapCellsColorDictionary(colorsDict);
                ClearMatrixTables();
                GenerateMatrixTables();

                Properties.Settings.Default.lastColorTablePath = fileName;

                if (!silent) { 
                    MessageBox.Show("Color file has been read." + errorMsg, "Operation completed", MessageBoxButtons.OK, iconType);
                }
                return true;
            } else {
                if (!silent) {
                    MessageBox.Show("No readable content was found in this file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
        }

        public void Swap(ref uint a, ref uint b) {
            uint temp = a;
            a = b;
            b = temp;
        }
        private void resetColorTableButton_Click(object sender, EventArgs e) {
            RomInfo.LoadMapCellsColorDictionary();
            ClearMatrixTables();
            GenerateMatrixTables();

            Properties.Settings.Default.lastColorTablePath = "";
        }

        /*
        private void ExportAllMovePermissionsInMatrix(object sender, EventArgs e) {
            CommonOpenFileDialog romFolder = new CommonOpenFileDialog();
            romFolder.IsFolderPicker = true;
            romFolder.Multiselect = false;

            if (romFolder.ShowDialog() != CommonFileDialogResult.Ok) {
                return;
            }

            for (int i = 0; i < currentMatrix.height; i++) {
                for (int j = 0; j < currentMatrix.width; j++) {
                    ushort val = currentMatrix.maps[i, j];
                    if (val < ushort.MaxValue) {
                        string path = romFolder.FileName + "\\" + currentMatrix.id + j.ToString("D2") + "_" + i.ToString("D2") + ".per";
                        File.WriteAllBytes(path, new MapFile(val).CollisionsToByteArray());
                    }
                }
            }
        }
        */
        #endregion

        #region Map Editor

        #region Variables & Constants 
        public const int mapEditorSquareSize = 19;

        /* Map Rotation vars */
        public bool lRot;
        public bool rRot;
        public bool uRot;
        public bool dRot;

        /* Screenshot Interpolation mode */
        public InterpolationMode intMode;

        /*  Camera settings */
        public bool hideBuildings = new bool();
        public bool mapTexturesOn = true;
        public bool showBuildingTextures = true;
        public static float ang = 0.0f;
        public static float dist = 12.8f;
        public static float elev = 50.0f;
        public float perspective = 45f;

        private byte bldDecimalPositions = 1;

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

            uint id = 0;

            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                id = currentMapFile.buildings[i].modelID;
                string baseName = (i + 1).ToString("D2") + MapHeader.nameSeparator;
                try {
                    buildingsListBox.Items.Add(baseName + buildIndexComboBox.Items[(int)id]);
                } catch (ArgumentOutOfRangeException) {
                    DialogResult d = MessageBox.Show("Building #" + id + " couldn't be found in the Building List.\n" +
                        "Do you want to load Building 0 in its place?\n" +
                        "(Choosing \"Cancel\" will discard this building altogether.)", "Building not found", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                    if (d == DialogResult.Yes) {
                        buildingsListBox.Items.Add(baseName + buildIndexComboBox.Items[0]);
                    } else if (d == DialogResult.No) {
                        buildingsListBox.Items.Add(baseName + "MISSING " + (int)id + '!');
                    } // else do nothing
                }
            }

        }
        private void MW_LoadModelTextures(NSBMD model, string textureFolder, int fileID) {
            if (fileID < 0) {
                return;
            }
            string texturePath = textureFolder + "\\" + fileID.ToString("D4");
            model.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(File.ReadAllBytes(texturePath)), out model.Textures, out model.Palettes);
            try {
                model.MatchTextures();
            } catch { }
        }
        private void RenderMap(ref NSBMDGlRenderer mapRenderer, ref NSBMDGlRenderer buildingsRenderer, ref MapFile mapFile, float ang, float dist, float elev, float perspective, int width, int height, bool mapTexturesON = true, bool buildingTexturesON = true) {
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
            if (!mapTexturesON) {
                Gl.glDisable(Gl.GL_TEXTURE_2D);
            } else {
                Gl.glEnable(Gl.GL_TEXTURE_2D);
            }

            mapRenderer.RenderModel("", ani, aniframeS, aniframeS, aniframeS, aniframeS, aniframeS, ca, false, -1, 0.0f, 0.0f, dist, elev, ang, true, tp, mapFile.mapModel); // Render map model

            if (!hideBuildings) {
                if (buildingTexturesON) {
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                } else {
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                }

                for (int i = 0; i < mapFile.buildings.Count; i++) {
                    NSBMD file = mapFile.buildings[i].NSBMDFile;
                    if (file is null) {
                        Console.WriteLine("Null building can't be rendered");
                    } else {
                        buildingsRenderer.Model = file.models[0];
                        ScaleTranslateBuilding(mapFile.buildings[i]);
                        buildingsRenderer.RenderModel("", ani, aniframeS, aniframeS, aniframeS, aniframeS, aniframeS, ca, false, -1, 0.0f, 0.0f, dist, elev, ang, true, tp, file);
                    }
                }
            }
        }
        private void ScaleTranslateBuilding(Building building) {
            float fullXcoord = building.xPosition + building.xFraction / 65536f;
            float fullYcoord = building.yPosition + building.yFraction / 65536f;
            float fullZcoord = building.zPosition + building.zFraction / 65536f;

            float scaleFactor = (building.NSBMDFile.models[0].modelScale / 1024);
            float translateFactor = 256 / building.NSBMDFile.models[0].modelScale;

            Gl.glScalef(scaleFactor * building.width, scaleFactor * building.height, scaleFactor * building.length);
            Gl.glTranslatef(fullXcoord * translateFactor / building.width, fullYcoord * translateFactor / building.height, fullZcoord * translateFactor / building.length);
        }
        private void SetupRenderer(float ang, float dist, float elev, float perspective, int width, int height) {
            //TODO: improve this
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
            Glu.gluPerspective(perspective, aspect, 0.2f, 500.0f);//0.02f, 32.0f);
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
        private void SetupMapEditor() {
            /* Extract essential NARCs sub-archives*/
            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 9;
            toolStripProgressBar.Value = 0;
            statusLabel.Text = "Attempting to unpack Map Editor NARCs... Please wait.";
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.maps,
                DirNames.exteriorBuildingModels,
                DirNames.buildingConfigFiles,
                DirNames.buildingTextures,
                DirNames.mapTextures,
                DirNames.areaData,
            });

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });
            }

            disableHandlers = true;

            collisionPainterPictureBox.Image = new Bitmap(100, 100);
            typePainterPictureBox.Image = new Bitmap(100, 100);
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    mapPartsTabControl.TabPages.Remove(bgsTabPage);
                    break;
                default:
                    interiorbldRadioButton.Enabled = true;
                    exteriorbldRadioButton.Enabled = true;
                    break;
            };


            /* Add map names to box */
            selectMapComboBox.Items.Clear();
            int mapCount = romInfo.GetMapCount();

            for (int i = 0; i < mapCount; i++) {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(RomInfo.gameDirs[DirNames.maps].unpackedDir + "\\" + i.ToString("D4"))) {
                    switch (RomInfo.gameFamily) {
                        case gFamEnum.DP:
                        case gFamEnum.Plat:
                            reader.BaseStream.Position = 0x10 + reader.ReadUInt32() + reader.ReadUInt32();
                            break;
                        default:
                            reader.BaseStream.Position = 0x12;
                            short bgsSize = reader.ReadInt16();
                            long backupPos = reader.BaseStream.Position;

                            reader.BaseStream.Position = 0;
                            reader.BaseStream.Position = backupPos + bgsSize + reader.ReadUInt32() + reader.ReadUInt32();
                            break;
                    };

                    reader.BaseStream.Position += 0x14;
                    selectMapComboBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + DSUtils.ReadNSBMDname(reader));
                }

            }
            toolStripProgressBar.Value++;

            /* Fill building models list */
            updateBuildingListComboBox(false);

            /*  Fill map textures list */
            mapTextureComboBox.Items.Clear();
            mapTextureComboBox.Items.Add("Untextured");
            for (int i = 0; i < romInfo.GetMapTexturesCount(); i++) {
                mapTextureComboBox.Items.Add("Map Texture Pack [" + i.ToString("D2") + "]");
            }
            toolStripProgressBar.Value++;

            /*  Fill building textures list */
            buildTextureComboBox.Items.Clear();
            buildTextureComboBox.Items.Add("Untextured");
            for (int i = 0; i < romInfo.GetBuildingTexturesCount(); i++) {
                buildTextureComboBox.Items.Add("Building Texture Pack [" + i.ToString("D2") + "]");
            }

            toolStripProgressBar.Value++;

            collisionPainterComboBox.Items.Clear();
            foreach (string s in PokeDatabase.System.MapCollisionPainters.Values) {
                collisionPainterComboBox.Items.Add(s);
            }

            collisionTypePainterComboBox.Items.Clear();
            foreach (string s in PokeDatabase.System.MapCollisionTypePainters.Values) {
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
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    mapTextureComboBox.SelectedIndex = 7;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
                case gFamEnum.HGSS:
                    mapTextureComboBox.SelectedIndex = 3;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
                default:
                    mapTextureComboBox.SelectedIndex = 2;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
            };

            statusLabel.Text = "Ready";
        }
        private void addMapFileButton_Click(object sender, EventArgs e) {
            /* Add new map file to map folder */
            new MapFile(0, RomInfo.gameFamily, discardMoveperms: true).SaveToFileDefaultDir(selectMapComboBox.Items.Count);

            /* Update ComboBox and select new file */
            selectMapComboBox.Items.Add(selectMapComboBox.Items.Count.ToString("D3") + MapHeader.nameSeparator + "newmap");
            selectMapComboBox.SelectedIndex = selectMapComboBox.Items.Count - 1;
        }
        private void replaceMapBinButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .bin file */
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Map BIN File (*.bin)|*.bin"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            MapFile temp = new MapFile(of.FileName, RomInfo.gameFamily, false);

            if (temp.correctnessFlag) {
                UpdateMapBinAndRefresh(temp, "Map BIN imported successfully!");
                return;
            } else {
                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    //If HGSS didn't work try reading as Platinum Map
                    temp = new MapFile(of.FileName, gFamEnum.Plat, false); 
                } else {
                    //If Plat didn't work try reading as HGSS Map
                    temp = new MapFile(of.FileName, gFamEnum.HGSS, false);
                }

                if (temp.correctnessFlag) {
                    UpdateMapBinAndRefresh(temp, "Map BIN imported and adapted successfully!");
                    return;
                }
            }
            
            MessageBox.Show("The BIN file you imported is corrupted!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void UpdateMapBinAndRefresh(MapFile newerVersion, string message) {
            currentMapFile = newerVersion;

            /* Update map BIN file */
            currentMapFile.SaveToFileDefaultDir(selectMapComboBox.SelectedIndex, showSuccessMessage: false);

            /* Refresh controls */
            selectMapComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buildTextureComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            int btIndex = buildTextureComboBox.SelectedIndex;
            
            if (disableHandlers || btIndex < 0) {
                return;
            }

            if (btIndex == 0) {
                showBuildingTextures = false;
            } else {
                string texturePath = RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + (btIndex - 1).ToString("D4");
                byte[] textureFile = File.ReadAllBytes(texturePath);

                Stream str = new MemoryStream(textureFile);
                foreach (Building building in currentMapFile.buildings) {
                    str.Position = 0;
                    NSBMD file = building.NSBMDFile;

                    if (file != null) {
                        file.materials = NSBTXLoader.LoadNsbtx(str, out file.Textures, out file.Palettes);

                        try {
                            file.MatchTextures();
                            showBuildingTextures = true;
                        } catch {
                            string itemAtIndex = buildTextureComboBox.Items[btIndex].ToString();
                            if (!itemAtIndex.StartsWith("Error!")) {
                                disableHandlers = true;
                                buildTextureComboBox.Items[btIndex] = itemAtIndex.Insert(0, "Error! - ");
                                disableHandlers = false;
                            }
                            showBuildingTextures = false;
                        }
                    }
                }
                //buildTextureComboBox.Items[buildTextureComboBox.SelectedIndex] = "Error - Building Texture Pack too small [" + (buildTextureComboBox.SelectedIndex - 1).ToString("D2") + "]";
            }

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void mapTextureComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }

            if (mapTextureComboBox.SelectedIndex == 0)
                mapTexturesOn = false;
            else {
                mapTexturesOn = true;

                string texturePath = RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4");
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
        private void mapOpenGlControl_MouseWheel(object sender, MouseEventArgs e) {
            if (mapPartsTabControl.SelectedTab == buildingsTabPage && bldPlaceWithMouseCheckbox.Checked) {
                return;
            }
            dist -= (float)e.Delta / 200;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void mapOpenGlControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            byte multiplier = 2;
            if (e.Modifiers == Keys.Shift) {
                multiplier = 1;
            } else if (e.Modifiers == Keys.Control) {
                multiplier = 4;
            }

            switch (e.KeyCode) {
                case Keys.Right:
                    rRot = true;
                    lRot = false;
                    break;
                case Keys.Left:
                    rRot = false;
                    lRot = true;
                    break;
                case Keys.Up:
                    dRot = false;
                    uRot = true;
                    break;
                case Keys.Down:
                    dRot = true;
                    uRot = false;
                    break;
            }

            if (rRot ^ lRot) {
                if (rRot) {
                    ang += 1 * multiplier;
                } else if (lRot) {
                    ang -= 1 * multiplier;
                }
            }

            if (uRot ^ dRot) {
                if (uRot) {
                    elev -= 1 * multiplier;
                } else if (dRot) {
                    elev += 1 * multiplier;
                }
            }
            
            mapOpenGlControl.Invalidate();
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void mapOpenGlControl_KeyUp(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Right:
                    rRot = false;
                    break;
                case Keys.Left:
                    lRot = false;
                    break;
                case Keys.Up:
                    uRot = false;
                    break;
                case Keys.Down:
                    dRot = false;
                    break;
            }
        }
        private void mapOpenGlControl_Click(object sender, EventArgs e) {
            if (radio2D.Checked && bldPlaceWithMouseCheckbox.Checked) {
                PointF coordinates = mapRenderPanel.PointToClient(Cursor.Position);
                PointF mouseTilePos = new PointF(coordinates.X / mapEditorSquareSize, coordinates.Y / mapEditorSquareSize);

                if (buildingsListBox.SelectedIndex > -1) {
                    if (!bldPlaceLockXcheckbox.Checked)
                        xBuildUpDown.Value = (decimal)(Math.Round(mouseTilePos.X, bldDecimalPositions) - 16);
                    if (!bldPlaceLockZcheckbox.Checked)
                        zBuildUpDown.Value = (decimal)(Math.Round(mouseTilePos.Y, bldDecimalPositions) - 16);
                }
            }
        }
        private void bldRoundWhole_CheckedChanged(object sender, EventArgs e) {
            bldDecimalPositions = 0;
        }
        private void bldRoundDec_CheckedChanged(object sender, EventArgs e) {
            bldDecimalPositions = 1;
        }
        private void bldRoundCent_CheckedChanged(object sender, EventArgs e) {
            bldDecimalPositions = 2;
        }
        private void bldRoundMil_CheckedChanged(object sender, EventArgs e) {
            bldDecimalPositions = 3;
        }
        private void bldRoundDecmil_CheckedChanged(object sender, EventArgs e) {
            bldDecimalPositions = 4;
        }
        private void bldRoundCentMil_CheckedChanged(object sender, EventArgs e) {
            bldDecimalPositions = 5;
        }
        private void bldPlaceWithMouseCheckbox_CheckedChanged(object sender, EventArgs e) {
            bool status = bldPlaceWithMouseCheckbox.Checked && radio2D.Checked;
            bldPlaceLockXcheckbox.Enabled = status;
            bldPlaceLockZcheckbox.Enabled = status;
            bldRoundGroupbox.Enabled = status;
            lockXZgroupbox.Enabled = status;

            if (status) {
                SetCam2D();
            }
        }
        private void bldPlaceLockXcheckbox_CheckedChanged(object sender, EventArgs e) {
            ExclusiveCBInvert(bldPlaceLockZcheckbox);
        }

        private void bldPlaceLockZcheckbox_CheckedChanged(object sender, EventArgs e) {
            ExclusiveCBInvert(bldPlaceLockXcheckbox);
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

                SetCam2D();
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
            bool _2dmodeSelected = radio2D.Checked;

            if (_2dmodeSelected) {
                SetCam2D();
            } else {
                SetCam3D();
            }

            bldPlaceWithMouseCheckbox.Enabled = _2dmodeSelected;
            radio3D.Checked = !_2dmodeSelected;

            bldPlaceWithMouseCheckbox_CheckedChanged(null, null);
        }
        private void SetCam2D() {
            perspective = 4f;
            ang = 0f;
            dist = 115.2f;
            elev = 90f;

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void SetCam3D() {
            perspective = 45f;
            ang = 0f;
            dist = 12.8f;
            elev = 50.0f;

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void mapScreenshotButton_Click(object sender, EventArgs e) {
            MessageBox.Show("Choose where to save the map screenshot.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SaveFileDialog imageSFD = new SaveFileDialog {
                Filter = "PNG File(*.png)|*.png",
            };
            if (imageSFD.ShowDialog() != DialogResult.OK) {
                return;
            }

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
            ang, dist, elev, perspective,
            mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);

            int newW = 512, newH = 512;
            Bitmap newImage = new Bitmap(newW, newH);
            using (var graphCtr = Graphics.FromImage(newImage)) {
                graphCtr.SmoothingMode = SmoothingMode.HighQuality;
                graphCtr.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphCtr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphCtr.DrawImage(GrabMapScreenshot(mapOpenGlControl.Width, mapOpenGlControl.Height), 0, 0, newW, newH);
            }
            newImage.Save(imageSFD.FileName);
            MessageBox.Show("Screenshot saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void removeLastMapFileButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("Are you sure you want to delete the last Map BIN File?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d.Equals(DialogResult.Yes)) {
                /* Delete last map file */
                File.Delete(RomInfo.gameDirs[DirNames.maps].unpackedDir + "\\" + (selectMapComboBox.Items.Count - 1).ToString("D4"));

                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = selectMapComboBox.Items.Count - 1;
                if (selectMapComboBox.SelectedIndex == lastIndex)
                    selectMapComboBox.SelectedIndex--;

                /* Remove item from ComboBox */
                selectMapComboBox.Items.RemoveAt(lastIndex);
            }
        }
        private void saveMapButton_Click(object sender, EventArgs e) {
            currentMapFile.SaveToFileDefaultDir(selectMapComboBox.SelectedIndex);
        }
        private void exportCurrentMapBinButton_Click(object sender, EventArgs e) {
            currentMapFile.SaveToFileExplorePath(selectMapComboBox.SelectedItem.ToString());
        }
        private void selectMapComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            /* Load map data into MapFile class instance */
            currentMapFile = new MapFile(selectMapComboBox.SelectedIndex, RomInfo.gameFamily);

            /* Load map textures for renderer */
            if (mapTextureComboBox.SelectedIndex > 0) {
                MW_LoadModelTextures(currentMapFile.mapModel, RomInfo.gameDirs[DirNames.mapTextures].unpackedDir, mapTextureComboBox.SelectedIndex - 1);
            }

            /* Load buildings nsbmd and textures for renderer into MapFile's building objects */
            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                currentMapFile.buildings[i].LoadModelData(romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked)); // Load building nsbmd
                if (buildTextureComboBox.SelectedIndex > 0) {
                    MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
                }
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
            if (buildingsListBox.Items.Count > 0) {
                buildingsListBox.SelectedIndex = 0;
            }

            modelSizeLBL.Text = currentMapFile.mapModelData.Length.ToString() + " B";
            terrainSizeLBL.Text = currentMapFile.bdhc.Length.ToString() + " B";

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
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
            AddBuildingToMap(new Building());
        }
        private void duplicateBuildingButton_Click(object sender, EventArgs e) {
            if (buildingsListBox.SelectedIndex > -1) {
                AddBuildingToMap(new Building(currentMapFile.buildings[buildingsListBox.SelectedIndex]));
            }
        }
        private void AddBuildingToMap(Building b) {
            currentMapFile.buildings.Add(b);

            /* Load new building's model and textures for the renderer */
            b.LoadModelData(romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked));
            MW_LoadModelTextures(b.NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1);
            currentMapFile.buildings[currentMapFile.buildings.Count - 1] = b;

            /* Add new entry to buildings ListBox */
            buildingsListBox.Items.Add((buildingsListBox.Items.Count + 1).ToString("D2") + MapHeader.nameSeparator + buildIndexComboBox.Items[(int)b.modelID]);
            buildingsListBox.SelectedIndex = buildingsListBox.Items.Count - 1;

            /* Redraw scene with new building */
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void buildIndexComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || buildingsListBox.SelectedIndex < 0) { 
                return;
            }

            disableHandlers = true;
            buildingsListBox.Items[buildingsListBox.SelectedIndex] = (buildingsListBox.SelectedIndex + 1).ToString("D2") + MapHeader.nameSeparator + buildIndexComboBox.SelectedItem;
            disableHandlers = false;

            currentMapFile.buildings[buildingsListBox.SelectedIndex].modelID = (uint)buildIndexComboBox.SelectedIndex;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].LoadModelData(romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked));
            MW_LoadModelTextures(currentMapFile.buildings[buildingsListBox.SelectedIndex].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1);

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void buildingsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            int buildingNumber = buildingsListBox.SelectedIndex;
            if (disableHandlers || buildingNumber < 0) {
                return;
            }
            disableHandlers = true;

            Building selected = currentMapFile.buildings[buildingNumber];
            if (selected.NSBMDFile != null) {
                buildIndexComboBox.SelectedIndex = (int)selected.modelID;

                xBuildUpDown.Value = selected.xPosition + (decimal)selected.xFraction / 65535;
                yBuildUpDown.Value = selected.yPosition + (decimal)selected.yFraction / 65535;
                zBuildUpDown.Value = selected.zPosition + (decimal)selected.zFraction / 65535;

                buildingWidthUpDown.Value = selected.width;
                buildingHeightUpDown.Value = selected.height;
                buildingLengthUpDown.Value = selected.length;
            }

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
            SaveFileDialog sf = new SaveFileDialog {
                Filter = MapFile.BuildingsFilter,
                FileName = selectMapComboBox.SelectedItem.ToString()
            };
            if (sf.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            File.WriteAllBytes(sf.FileName, currentMapFile.BuildingsToByteArray());

            MessageBox.Show("Buildings exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importBuildingsButton_Click(object sender, EventArgs e) {
            OpenFileDialog ib = new OpenFileDialog {
                Filter = MapFile.BuildingsFilter
            };
            if (ib.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            currentMapFile.ImportBuildings(File.ReadAllBytes(ib.FileName));
            FillBuildingsBox();
            if (buildingsListBox.Items.Count > 0) buildingsListBox.SelectedIndex = 0;

            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                currentMapFile.buildings[i].LoadModelData(romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked)); // Load building nsbmd
                MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
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
                buildIndexComboBox.SelectedIndex = 0;
                currentMapFile.buildings[buildIndexComboBox.SelectedIndex].modelID = 0;
            }

            /* Load buildings nsbmd and textures for renderer into MapFile's building objects */
            for (int i = 0; i < currentMapFile.buildings.Count; i++) {
                currentMapFile.buildings[i].LoadModelData(romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked)); // Load building nsbmd
                MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
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
            if (disableHandlers || buildingsListBox.SelectedIndex < 0) {
                return;
            }

            var wholePart = Math.Truncate(xBuildUpDown.Value);
            var decPart = xBuildUpDown.Value - wholePart;

            if (decPart < 0) {
                decPart += 1;
                wholePart -= 1;
            }

            currentMapFile.buildings[buildingsListBox.SelectedIndex].xPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].xFraction = (ushort)(decPart * 65535);
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void zBuildUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || buildingsListBox.SelectedIndex < 0)
                return;

            var wholePart = Math.Truncate(zBuildUpDown.Value);
            var decPart = zBuildUpDown.Value - wholePart;

            if (decPart < 0) {
                decPart += 1;
                wholePart -= 1;
            }

            currentMapFile.buildings[buildingsListBox.SelectedIndex].zPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].zFraction = (ushort)(decPart * 65535);
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);
        }
        private void yBuildUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || buildingsListBox.SelectedIndex < 0)
                return;

            var wholePart = Math.Truncate(yBuildUpDown.Value);
            var decPart = yBuildUpDown.Value - wholePart;

            if (decPart < 0) {
                decPart += 1;
                wholePart -= 1;
            }

            currentMapFile.buildings[buildingsListBox.SelectedIndex].yPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].yFraction = (ushort)(decPart * 65535);
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
                        PrepareCollisionPainterGraphics(currentMapFile.collisions[i, j]);

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
                        PrepareCollisionPainterGraphics(currentMapFile.collisions[i, j]);

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
                        PrepareTypePainterGraphics(currentMapFile.types[i, j]);

                        /* Draw cell with color */
                        mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                        gMain.DrawRectangle(paintPen, mainCell);
                        gMain.FillRectangle(paintBrush, mainCell);

                        /* Draw byte on cell */
                        StringFormat sf = new StringFormat {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };
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
                        PrepareTypePainterGraphics(paintByte = currentMapFile.types[i, j]);

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
                mainCell = new Rectangle(xPosition * mapEditorSquareSize, yPosition * mapEditorSquareSize, mapEditorSquareSize, mapEditorSquareSize);
                smallCell = new Rectangle(xPosition * 3, yPosition * 3, 3, 3);

                using (Graphics mainG = Graphics.FromImage(movPictureBox.Image)) {
                    /*  Draw new cell on main grid */
                    mainG.SetClip(mainCell);
                    mainG.Clear(Color.Transparent);
                    mainG.DrawRectangle(paintPen, mainCell);
                    mainG.FillRectangle(paintBrush, mainCell);
                    if (selectTypePanel.BackColor == Color.MidnightBlue) {
                        sf = new StringFormat {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };
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
        private void FloodFillUtil(byte[,] screen, int x, int y, byte prevC, byte newC, int sizeX, int sizeY) {
            // Base cases 
            if (x < 0 || x >= sizeX || y < 0 || y >= sizeY) {
                return;
            }

            if (screen[y, x] != prevC) {
                return;
            }

            // Replace the color at (x, y) 
            screen[y, x] = newC;

            // Recur for north, east, south and west 
            FloodFillUtil(screen, x + 1, y, prevC, newC, sizeX, sizeY);
            FloodFillUtil(screen, x - 1, y, prevC, newC, sizeX, sizeY);
            FloodFillUtil(screen, x, y + 1, prevC, newC, sizeX, sizeY);
            FloodFillUtil(screen, x, y - 1, prevC, newC, sizeX, sizeY);
        }
        private void FloodFillCell(int x, int y) {
            byte toPaint = paintByte;
            if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                if (currentMapFile.collisions[y, x] != paintByte) {
                    FloodFillUtil(currentMapFile.collisions, x, y, currentMapFile.collisions[y, x], paintByte, 32, 32);
                    DrawCollisionGrid();
                    DrawSmallCollision();
                    PrepareCollisionPainterGraphics(paintByte);
                }
            } else {
                if (currentMapFile.types[y, x] != paintByte) {
                    FloodFillUtil(currentMapFile.types, x, y, currentMapFile.types[y, x], paintByte, 32, 32);
                    DrawTypeGrid();
                    DrawSmallTypeCollision();
                    PrepareTypePainterGraphics(paintByte);
                }
            }

            /* Draw permissions in the small selection boxes */


        }
        private void RestorePainter() {
            if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                collisionPainterComboBox_SelectedIndexChange(null, null);
            } else if (collisionTypePainterComboBox.Enabled) {
                typePainterComboBox_SelectedIndexChanged(null, null);
            } else {
                typePainterUpDown_ValueChanged(null, null);
            }
        }
        private void PrepareCollisionPainterGraphics(byte collisionValue) {
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
        }
        private void PrepareTypePainterGraphics(byte typeValue) {
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
        }
        #endregion

        private void clearCurrentButton_Click(object sender, EventArgs e) {
            PictureBox smallBox = selectCollisionPanel.BackColor == Color.MidnightBlue ? collisionPictureBox : typePictureBox;

            using (Graphics smallG = Graphics.FromImage(smallBox.Image)) {
                using (Graphics mainG = Graphics.FromImage(movPictureBox.Image)) {
                    smallG.Clear(Color.Transparent);
                    mainG.Clear(Color.Transparent);
                    PrepareCollisionPainterGraphics(0x0);

                    for (int i = 0; i < 32; i++) {
                        for (int j = 0; j < 32; j++) {
                            mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                            mainG.DrawRectangle(paintPen, mainCell);
                            mainG.FillRectangle(paintBrush, mainCell);
                        }
                    }
                }
            }

            if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                currentMapFile.collisions = new byte[32, 32]; // Set all collision bytes to clear (0x0)               
            } else {
                currentMapFile.types = new byte[32, 32]; // Set all type bytes to clear (0x0)
            }

            movPictureBox.Invalidate(); // Refresh main image
            smallBox.Invalidate();
            RestorePainter();
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
            SaveFileDialog em = new SaveFileDialog {
                Filter = MapFile.MovepermsFilter,
                FileName = selectMapComboBox.SelectedItem.ToString()
            };
            if (em.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            File.WriteAllBytes(em.FileName, currentMapFile.CollisionsToByteArray());

            MessageBox.Show("Permissions exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importMovButton_Click(object sender, EventArgs e) {
            OpenFileDialog ip = new OpenFileDialog {
                Filter = MapFile.MovepermsFilter
            };
            if (ip.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            currentMapFile.ImportPermissions(File.ReadAllBytes(ip.FileName));

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
            MouseEventArgs mea = (MouseEventArgs)e;

            int xCoord = movPictureBox.PointToClient(MousePosition).X / mapEditorSquareSize;
            int yCoord = movPictureBox.PointToClient(MousePosition).Y / mapEditorSquareSize;

            if (mea.Button == MouseButtons.Middle) {
                FloodFillCell(xCoord, yCoord);
            } else if (mea.Button == MouseButtons.Left) {
                EditCell(xCoord, yCoord);
            } else {
                if (selectCollisionPanel.BackColor == Color.MidnightBlue) {
                    byte newValue = currentMapFile.collisions[yCoord, xCoord];
                    updateCollisions(newValue);
                } else {
                    byte newValue = currentMapFile.types[yCoord, xCoord];
                    typePainterUpDown.Value = newValue;
                    updateTypeCollisions(newValue);
                };
            }
        }
        private void movPictureBox_MouseMove(object sender, MouseEventArgs e) {
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left) {
                EditCell(e.Location.X / mapEditorSquareSize, e.Location.Y / mapEditorSquareSize);
            }
        }
        private void collisionPainterComboBox_SelectedIndexChange(object sender, EventArgs e) {
            byte? collisionByte = StringToCollisionByte((string)collisionPainterComboBox.SelectedItem);

            if (collisionByte != null) {
                updateCollisions((byte)collisionByte);
            }
        }
        private void typePainterComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            byte? collisionByte = StringToCollisionByte((string)collisionTypePainterComboBox.SelectedItem);

            if (collisionByte != null) {
                updateTypeCollisions((byte)collisionByte);
            }
        }

        private byte? StringToCollisionByte(string selectedItem) {
            byte? result;
            try {
                result = Convert.ToByte(selectedItem.Substring(1, 2), 16);
            } catch (FormatException) {
                Console.WriteLine("Format incompatible");
                result = null;
            }
            return result;
        }
        private void typePainterUpDown_ValueChanged(object sender, EventArgs e) {
            updateTypeCollisions((byte)typePainterUpDown.Value);
        }
        private void updateCollisions(byte typeValue) {
            PrepareCollisionPainterGraphics(typeValue);
            paintByte = (byte)typeValue;

            sf = new StringFormat {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            using (Graphics g = Graphics.FromImage(collisionPainterPictureBox.Image)) {
                g.Clear(Color.FromArgb(255, paintBrush.Color));
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(typeValue.ToString("X2"), new Font("Microsoft Sans Serif", 24), textBrush, painterBox, sf);
            }

            if (PokeDatabase.System.MapCollisionPainters.TryGetValue(typeValue, out string dictResult)) {
                collisionPainterComboBox.SelectedItem = dictResult;
            }
            collisionPainterPictureBox.Invalidate();
        }
        private void updateTypeCollisions(byte typeValue) {
            PrepareTypePainterGraphics(typeValue);
            paintByte = typeValue;

            sf = new StringFormat {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            using (Graphics g = Graphics.FromImage(typePainterPictureBox.Image)) {
                g.Clear(Color.FromArgb(255, paintBrush.Color));
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(typeValue.ToString("X2"), new Font("Microsoft Sans Serif", 24), textBrush, painterBox, sf);
            }

            if (PokeDatabase.System.MapCollisionTypePainters.TryGetValue(typeValue, out string dictResult)) {
                collisionTypePainterComboBox.SelectedItem = dictResult;
            } else {
                valueTypeRadioButton.Checked = true;
                typePainterUpDown.Value = typeValue;
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
        public const ushort MAPMODEL_CRITICALSIZE = 61000;
        private void importMapButton_Click(object sender, EventArgs e) {
            OpenFileDialog im = new OpenFileDialog {
                Filter = MapFile.NSBMDFilter
            };
            if (im.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            currentMapFile.LoadMapModel(DSUtils.ReadFromFile(im.FileName));

            if (mapTextureComboBox.SelectedIndex > 0) {
                MW_LoadModelTextures(currentMapFile.mapModel, RomInfo.gameDirs[DirNames.mapTextures].unpackedDir, mapTextureComboBox.SelectedIndex - 1);
            }
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, showBuildingTextures);

            modelSizeLBL.Text = currentMapFile.mapModelData.Length.ToString() + " B";

            string message;
            string title;
            if (currentMapFile.mapModelData.Length > MAPMODEL_CRITICALSIZE) {
                message = "You imported a map model that exceeds " + MAPMODEL_CRITICALSIZE + " bytes." + Environment.NewLine
                    + "This may lead to unexpected behavior in game.";
                title = "Imported correctly, but...";
            } else {
                message = "Map model imported successfully!";
                title = "Success!";
            }
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void exportMapButton_Click(object sender, EventArgs e) {
            SaveFileDialog em = new SaveFileDialog {
                FileName = selectMapComboBox.SelectedItem.ToString()
            };

            byte[] modelToWrite;

            if (embedTexturesInMapModelCheckBox.Checked) { /* Textured NSBMD file */
                em.Filter = MapFile.TexturedNSBMDFilter;
                if (em.ShowDialog(this) != DialogResult.OK) {
                    return;
                }
                
                string texturePath = RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4");
                byte[] texturesToEmbed = File.ReadAllBytes(texturePath);
                modelToWrite = DSUtils.BuildNSBMDwithTextures(currentMapFile.mapModelData, texturesToEmbed);
            } else { /* Untextured NSBMD file */
                em.Filter = MapFile.UntexturedNSBMDFilter;
                if (em.ShowDialog(this) != DialogResult.OK) {
                    return;
                }

                modelToWrite = currentMapFile.mapModelData;
            }

            File.WriteAllBytes(em.FileName, modelToWrite);
            MessageBox.Show("Map model exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void daeExportButton_Click(object sender, EventArgs e) {
            DSUtils.ModelToDAE(
                modelName: selectMapComboBox.SelectedItem.ToString().TrimEnd('\0'),
                modelData: currentMapFile.mapModelData,
                textureData: mapTextureComboBox.SelectedIndex < 0 ? null : File.ReadAllBytes(RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4"))
            );
        }
        #endregion

        #region BDHC Editor
        private void bdhcImportButton_Click(object sender, EventArgs e) {
            OpenFileDialog it = new OpenFileDialog() {
                Filter = RomInfo.gameFamily == gFamEnum.DP ? MapFile.BDHCFilter : MapFile.BDHCamFilter
            };

            if (it.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            currentMapFile.ImportTerrain(File.ReadAllBytes(it.FileName));
            terrainSizeLBL.Text = currentMapFile.bdhc.Length.ToString() + " B";
            MessageBox.Show("Terrain settings imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void bdhcExportButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog {
                FileName = selectMapComboBox.SelectedItem.ToString(),
                Filter = RomInfo.gameFamily == gFamEnum.DP ? MapFile.BDHCFilter : MapFile.BDHCamFilter
            };

            if (sf.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            File.WriteAllBytes(sf.FileName, currentMapFile.bdhc);

            terrainSizeLBL.Text = currentMapFile.bdhc.Length.ToString() + " B";
            MessageBox.Show("Terrain settings exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void soundPlatesImportButton_Click(object sender, EventArgs e) {
            OpenFileDialog it = new OpenFileDialog {
                Filter = MapFile.BGSFilter
            };

            if (it.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            currentMapFile.ImportSoundPlates(File.ReadAllBytes(it.FileName));
            BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            MessageBox.Show("BackGround Sound data imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void soundPlatesExportButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog {
                Filter = MapFile.BGSFilter,
                FileName = selectMapComboBox.SelectedItem.ToString()
            };
            if (sf.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            File.WriteAllBytes(sf.FileName, currentMapFile.bgs);

            BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            MessageBox.Show("BackGround Sound data exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void soundPlatesBlankButton_Click(object sender, EventArgs e) {
            currentMapFile.bgs = MapFile.blankBGS;
            BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            MessageBox.Show("BackGround Sound data successfull blanked.\nRemember to save the current map file.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #endregion

        #region Event Editor

        #region Variables      
        private bool itemComboboxIsUpToDate = false;
        public static NSBMDGlRenderer eventMapRenderer = new NSBMDGlRenderer();
        public static NSBMDGlRenderer eventBuildingsRenderer = new NSBMDGlRenderer();
        public static MapFile eventMapFile;
        public NSMBe4.NSBMD.NSBTX_File overworldFrames;
        public GameMatrix eventMatrix;

        public const byte eventScreenSquareSize = 17;
        public EventFile currentEvFile;
        public Event selectedEvent;

        /* Painters to draw the matrix grid */
        public Pen eventPen;
        public Brush eventBrush;
        public Rectangle eventMatrixRectangle;
        #endregion

        #region Subroutines
        private void itemsSelectorHelpBtn_Click(object sender, EventArgs e) {
            MessageBox.Show("This selector allows you to pick a preset Ground Item script from the game data.\n" +
                "Unlike in previous DSPRE versions, you can now change the Ground Item to be obtained even if you decided not to apply the Standardize Items patch from the Rom ToolBox.\n\n" +
                "However, some items are unavailable by default. The aforementioned patch can neutralize this limitation.\n\n", 
                "About Ground Items", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void CenterEventViewOnEntities() {
            try {
                if (currentEvFile.overworlds.Count > 0) {
                    eventMatrixXUpDown.Value = currentEvFile.overworlds[0].xMatrixPosition;
                    eventMatrixYUpDown.Value = currentEvFile.overworlds[0].yMatrixPosition;
                } else if (currentEvFile.warps.Count > 0) {
                    eventMatrixXUpDown.Value = currentEvFile.warps[0].xMatrixPosition;
                    eventMatrixYUpDown.Value = currentEvFile.warps[0].yMatrixPosition;
                } else if (currentEvFile.spawnables.Count > 0) {
                    eventMatrixXUpDown.Value = currentEvFile.spawnables[0].xMatrixPosition;
                    eventMatrixYUpDown.Value = currentEvFile.spawnables[0].yMatrixPosition;
                } else if (currentEvFile.triggers.Count > 0) {
                    eventMatrixXUpDown.Value = currentEvFile.triggers[0].xMatrixPosition;
                    eventMatrixYUpDown.Value = currentEvFile.triggers[0].yMatrixPosition;
                } else {
                    eventMatrixXUpDown.Value = 0;
                    eventMatrixYUpDown.Value = 0;
                }
            } catch (ArgumentOutOfRangeException) {
                MessageBox.Show("One of the events tried to reference a bigger Matrix.\nMake sure the Header File associated to this Event File is using the correct Matrix.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void centerEventViewOnSelectedEvent_Click(object sender, EventArgs e) {
            if (selectedEvent is null) {
                MessageBox.Show("You haven't selected any event.", "Nothing to do here",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                try {
                    eventMatrixXUpDown.Value = selectedEvent.xMatrixPosition;
                    eventMatrixYUpDown.Value = selectedEvent.yMatrixPosition;
                } catch (ArgumentOutOfRangeException) {
                    MessageBox.Show("Event is out of range.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Update();
            }
        }
        private void eventPictureBox_MouseMove(object sender, MouseEventArgs e) {
            Point coordinates = eventPictureBox.PointToClient(Cursor.Position);
            Point mouseTilePos = new Point(coordinates.X / eventScreenSquareSize, coordinates.Y / eventScreenSquareSize);
            statusLabel.Text = "Local: " + mouseTilePos.X + ", " + mouseTilePos.Y + "   |   " + "Global: " + (eventMatrixXUpDown.Value * MapFile.mapSize + mouseTilePos.X).ToString() + ", " + (eventMatrixYUpDown.Value * MapFile.mapSize + mouseTilePos.Y).ToString();
        }

        private void DisplayActiveEvents() {
            eventPictureBox.Image = new Bitmap(eventPictureBox.Width, eventPictureBox.Height);

            using (Graphics g = Graphics.FromImage(eventPictureBox.Image)) {
                Bitmap icon;

                /* Draw spawnables */
                if (showSpawnablesCheckBox.Checked) {
                    icon = (Bitmap)Properties.Resources.ResourceManager.GetObject("sign");

                    for (int i = 0; i < currentEvFile.spawnables.Count; i++) {
                        Spawnable spawnable = currentEvFile.spawnables[i];

                        if (isEventOnCurrentMatrix(spawnable)) {
                            g.DrawImage(icon, spawnable.xMapPosition * 17, spawnable.yMapPosition * 17);
                            if (selectedEvent == spawnable) { // Draw selection rectangle if event is the selected one
                                DrawSelectionRectangle(g, spawnable);
                            }
                        }
                    }
                }

                /* Draw overworlds */
                if (showOwsCheckBox.Checked) {
                    for (int i = 0; i < currentEvFile.overworlds.Count; i++) {
                        Overworld overworld = currentEvFile.overworlds[i];

                        if (isEventOnCurrentMatrix(overworld)) { // Draw image only if event is in current map
                            Bitmap sprite = GetOverworldImage(overworld.overlayTableEntry, overworld.orientation);
                            sprite.MakeTransparent();
                            g.DrawImage(sprite, (overworld.xMapPosition) * 17 - 7 + (32 - sprite.Width) / 2, (overworld.yMapPosition - 1) * 17 + (32 - sprite.Height));

                            if (selectedEvent == overworld) {
                                DrawSelectionRectangleOverworld(g, overworld);
                            }
                        }
                    }
                }

                /* Draw warps */
                if (showWarpsCheckBox.Checked) {
                    icon = (Bitmap)Properties.Resources.ResourceManager.GetObject("warpCollision");

                    if (eventMapFile != null) {
                        for (int y = 0; y < MapFile.mapSize; y++) {
                            for (int x = 0; x < MapFile.mapSize; x++) {
                                byte moveperm = eventMapFile.types[x, y];

                                if (PokeDatabase.System.MapCollisionTypePainters.TryGetValue(moveperm, out string val)) {
                                    if (val.IndexOf("Warp", StringComparison.InvariantCultureIgnoreCase) >= 0) {
                                        //Console.WriteLine("Found warp at " + i + ", " + j);
                                        g.DrawImage(icon, y * 17, x * 17);
                                    }
                                }
                            }
                        }
                    }

                    icon = (Bitmap)Properties.Resources.ResourceManager.GetObject("warp");
                    for (int i = 0; i < currentEvFile.warps.Count; i++) {
                        Warp warp = currentEvFile.warps[i];

                        if (isEventOnCurrentMatrix(warp)) {
                            g.DrawImage(icon, warp.xMapPosition * 17, warp.yMapPosition * 17);

                            if (selectedEvent == warp) { // Draw selection rectangle if event is the selected one
                                DrawSelectionRectangle(g, warp);
                            }
                        }
                    }
                }

                /* Draw triggers */
                if (showTriggersCheckBox.Checked) {
                    icon = (Bitmap)Properties.Resources.ResourceManager.GetObject("trigger");

                    for (int i = 0; i < currentEvFile.triggers.Count; i++) {
                        Trigger trigger = currentEvFile.triggers[i];

                        if (isEventOnCurrentMatrix(trigger)) {
                            for (int y = 0; y < currentEvFile.triggers[i].heightY; y++) {
                                for (int x = 0; x < currentEvFile.triggers[i].widthX; x++) {
                                    g.DrawImage(icon, (trigger.xMapPosition + x) * 17, (trigger.yMapPosition + y) * 17);
                                }
                            }
                            if (selectedEvent == trigger) {// Draw selection rectangle if event is the selected one
                                DrawSelectionRectangleTrigger(g, trigger);
                            }
                        }
                    }
                }
            }

            eventPictureBox.Invalidate();
        }
        private void DrawSelectionRectangle(Graphics g, Event ev) {
            eventPen = Pens.Red;
            g.DrawRectangle(eventPen, (ev.xMapPosition) * 17 - 1, (ev.yMapPosition) * 17 - 1, 18, 18);
            g.DrawRectangle(eventPen, (ev.xMapPosition) * 17 - 2, (ev.yMapPosition) * 17 - 2, 20, 20);
        }
        private void DrawSelectionRectangleTrigger(Graphics g, Trigger t) {
            eventPen = Pens.Red;
            g.DrawRectangle(eventPen, (t.xMapPosition) * 17 - 1, (t.yMapPosition) * 17 - 1, 17 * t.widthX + 1, 17 * t.heightY + 1);
            g.DrawRectangle(eventPen, (t.xMapPosition) * 17 - 2, (t.yMapPosition) * 17 - 2, 17 * t.widthX + 3, 17 * t.heightY + 3);

        }
        private void DrawSelectionRectangleOverworld(Graphics g, Overworld ow) {
            eventPen = Pens.Red;
            g.DrawRectangle(eventPen, (ow.xMapPosition) * 17 - 8, (ow.yMapPosition - 1) * 17, 34, 34);
            g.DrawRectangle(eventPen, (ow.xMapPosition) * 17 - 9, (ow.yMapPosition - 1) * 17 - 1, 36, 36);
        }
        private void DisplayEventMap(bool readGraphicsFromHeader = true) {
            /* Determine map file to open and open it in BinaryReader, unless map is VOID */
            uint mapIndex = GameMatrix.EMPTY;
            if (eventMatrixXUpDown.Value > eventMatrix.width || eventMatrixYUpDown.Value > eventMatrix.height) {
                String errorMsg = "This event file contains elements located on an unreachable map, beyond the current matrix.\n" +
                    "It is strongly advised that you bring every Overworld, Spawnable, Warp and Trigger of this event to a map that belongs to the matrix's range.";
                MessageBox.Show(errorMsg, "Can't load proper map", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else {
                mapIndex = eventMatrix.maps[(int)(eventMatrixYUpDown.Value), (int)(eventMatrixXUpDown.Value)];
            }

            if (mapIndex == GameMatrix.EMPTY) {
                eventPictureBox.BackgroundImage = new Bitmap(eventPictureBox.Width, eventPictureBox.Height);
                using (Graphics g = Graphics.FromImage(eventPictureBox.BackgroundImage)) g.Clear(Color.Black);
            } else {
                /* Determine area data */
                byte areaDataID;
                if (eventMatrix.hasHeadersSection && readGraphicsFromHeader) {
                    ushort headerID = (ushort)eventMatrix.headers[(short)eventMatrixYUpDown.Value, (short)eventMatrixXUpDown.Value];
                    MapHeader h;
                    if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                        h = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + headerID.ToString("D4"), headerID, 0);
                    } else {
                        h = MapHeader.LoadFromARM9(headerID);
                    }

                    areaDataID = h.areaDataID;

                    bool disableHandlersbackup = disableHandlers;
                    disableHandlers = true;
                    eventAreaDataUpDown.Value = h.areaDataID;
                    disableHandlers = disableHandlersbackup;
                } else {
                    areaDataID = (byte)eventAreaDataUpDown.Value;
                }

                /* get texture file numbers from area data */
                AreaData areaData = new AreaData(areaDataID);

                /* Read map and building models, match them with textures and render them*/
                eventMapFile = new MapFile( (int)mapIndex, RomInfo.gameFamily, discardMoveperms: false);
                MW_LoadModelTextures(eventMapFile.mapModel, RomInfo.gameDirs[DirNames.mapTextures].unpackedDir, areaData.mapTileset);

                bool isInteriorMap = false;
                if ((RomInfo.gameVersion == gVerEnum.HeartGold || RomInfo.gameVersion == gVerEnum.SoulSilver) && areaData.areaType == 0x0)
                    isInteriorMap = true;

                for (int i = 0; i < eventMapFile.buildings.Count; i++) {
                    eventMapFile.buildings[i].LoadModelData(romInfo.GetBuildingModelsDirPath(isInteriorMap)); // Load building nsbmd
                    MW_LoadModelTextures(eventMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, areaData.buildingsTileset); // Load building textures                
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
            for (int i = 0; i < currentEvFile.spawnables.Count; i++) {
                spawnablesListBox.Items.Add($"{Event.EventType.Spawnable} {i}");
            }
        }
        private void FillOverworldsBox() {
            overworldsListBox.Items.Clear();
            for (int i = 0; i < currentEvFile.overworlds.Count; i++) {
                overworldsListBox.Items.Add($"{Event.EventType.Overworld} {i}");
            }
        }
        private void FillWarpsBox() {
            warpsListBox.Items.Clear();
            int count = currentEvFile.warps.Count;

            for (int i = 0; i < count; i++) {
                warpsListBox.Items.Add(i.ToString("D3") + ": " + currentEvFile.warps[i].ToString());
            }
        }
        private void FillTriggersBox() {
            triggersListBox.Items.Clear();
            for (int i = 0; i < currentEvFile.triggers.Count; i++) {
                triggersListBox.Items.Add($"{Event.EventType.Trigger} {i}");
            }
        }
        private Bitmap GetOverworldImage(ushort eventEntryID, ushort orientation) {
            /* Find sprite corresponding to ID and load it*/
            if (RomInfo.ow3DSpriteDict.TryGetValue(eventEntryID, out string imageName)) { // If overworld is 3D, load image from dictionary
                return (Bitmap)Properties.Resources.ResourceManager.GetObject(imageName);
            }

            if (!RomInfo.OverworldTable.TryGetValue(eventEntryID, out (uint spriteID, ushort properties) result)) { // try loading image from dictionary
                return (Bitmap)Properties.Resources.ResourceManager.GetObject("overworld"); //if there's no match, load bounding box
            }

            try {
                FileStream stream = new FileStream(RomInfo.gameDirs[DirNames.OWSprites].unpackedDir + "\\" + result.spriteID.ToString("D4"), FileMode.Open);
                NSMBe4.NSBMD.NSBTX_File nsbtx = new NSMBe4.NSBMD.NSBTX_File(stream);

                if (nsbtx.TexInfo.num_objs <= 1) {
                    return LoadTextureFromNSBTX(nsbtx, 0, 0); // Read nsbtx slot 0 if ow has only 2 frames
                }
                if (nsbtx.TexInfo.num_objs <= 4) {
                    switch (orientation) {
                        case 0:
                            return LoadTextureFromNSBTX(nsbtx, 0, 0);
                        case 1:
                            return LoadTextureFromNSBTX(nsbtx, 1, 0);
                        case 2:
                            return LoadTextureFromNSBTX(nsbtx, 2, 0);
                        default:
                            return LoadTextureFromNSBTX(nsbtx, 3, 0);
                    }
                }
                if (nsbtx.TexInfo.num_objs <= 8) { //Read nsbtx slot corresponding to overworld's movement
                    switch (orientation) {
                        case 0:
                            return LoadTextureFromNSBTX(nsbtx, 0, 0);
                        case 1:
                            return LoadTextureFromNSBTX(nsbtx, 2, 0);
                        case 2:
                            return LoadTextureFromNSBTX(nsbtx, 4, 0);
                        default:
                            return LoadTextureFromNSBTX(nsbtx, 6, 0);
                    }
                }
                if (nsbtx.TexInfo.num_objs <= 16) { // Read nsbtx slot corresponding to overworld's movement
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
                } else {
                    switch (orientation) {
                        case 0:
                            return LoadTextureFromNSBTX(nsbtx, 0, 0);
                        case 1:
                            return LoadTextureFromNSBTX(nsbtx, 27, 0);
                        case 2:
                            return LoadTextureFromNSBTX(nsbtx, 2, 0);
                        default:
                            return LoadTextureFromNSBTX(nsbtx, 4, 0);
                    }
                }
            } catch { // Load bounding box if sprite cannot be found
                return (Bitmap)Properties.Resources.ResourceManager.GetObject("overworldUnreadable");
            }
        }
        private void MarkUsedCells() {
            using (Graphics g = Graphics.FromImage(eventMatrixPictureBox.Image)) {
                eventBrush = Brushes.Orange;

                for (int i = 0; i < currentEvFile.spawnables.Count; i++) {
                    eventMatrixRectangle = new Rectangle(2 + 16 * currentEvFile.spawnables[i].xMatrixPosition, 2 + 16 * currentEvFile.spawnables[i].yMatrixPosition, 13, 13);
                    g.FillRectangle(eventBrush, eventMatrixRectangle);
                }
                for (int i = 0; i < currentEvFile.overworlds.Count; i++) {
                    eventMatrixRectangle = new Rectangle(2 + 16 * currentEvFile.overworlds[i].xMatrixPosition, 2 + 16 * currentEvFile.overworlds[i].yMatrixPosition, 13, 13);
                    g.FillRectangle(eventBrush, eventMatrixRectangle);
                }
                for (int i = 0; i < currentEvFile.warps.Count; i++) {
                    eventMatrixRectangle = new Rectangle(2 + 16 * currentEvFile.warps[i].xMatrixPosition, 2 + 16 * currentEvFile.warps[i].yMatrixPosition, 13, 13);
                    g.FillRectangle(eventBrush, eventMatrixRectangle);
                }
                for (int i = 0; i < currentEvFile.triggers.Count; i++) {
                    eventMatrixRectangle = new Rectangle(2 + 16 * currentEvFile.triggers[i].xMatrixPosition, 2 + 16 * currentEvFile.triggers[i].yMatrixPosition, 13, 13);
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
                            ushort p = (ushort)(nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].Image[j * 2] + (nsbtx.TexInfo.infoBlock.TexInfo[imageIndex].Image[j * 2 + 1] << 8));
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
            if (ev.xMatrixPosition == eventMatrixXUpDown.Value) {
                if (ev.yMatrixPosition == eventMatrixYUpDown.Value) {
                    return true;
                }
            }

            return false;
        }
        private bool isEventUnderMouse(Event ev, Point mouseTilePos, int widthX = 0, int heightY = 0) {
            if (isEventOnCurrentMatrix(ev)) {
                Point evLocalCoords = new Point(ev.xMapPosition, ev.yMapPosition);
                Func<int, int, int, bool> checkRange = (mouseCoord, evCoord, extension) => mouseCoord >= evCoord && mouseCoord <= evCoord + extension;

                if (checkRange(mouseTilePos.X, evLocalCoords.X, widthX) && checkRange(mouseTilePos.Y, evLocalCoords.Y, heightY)) {
                    return true;
                }
            }
            return false;
        }
        #endregion
        private void SetupEventEditor() {
            /* Extract essential NARCs sub-archives*/

            statusLabel.Text = "Attempting to unpack Event Editor NARCs... Please wait. This might take a while";
            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 12;
            toolStripProgressBar.Value = 0;
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.matrices,
                DirNames.maps,
                DirNames.exteriorBuildingModels,
                DirNames.buildingConfigFiles,
                DirNames.buildingTextures,
                DirNames.mapTextures,
                DirNames.areaData,

                DirNames.eventFiles,
                DirNames.trainerProperties,
                DirNames.OWSprites,

                DirNames.scripts, 
            });

            RomInfo.SetOWtable();
            RomInfo.Set3DOverworldsDict();

            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });
            }

            disableHandlers = true;
            if (File.Exists(RomInfo.OWtablePath)) {
                switch (RomInfo.gameFamily) {
                    case gFamEnum.DP:
                    case gFamEnum.Plat:
                        break;
                    default:
                        // HGSS Overlay 1 must be decompressed in order to read the overworld table
                        if (DSUtils.CheckOverlayHasCompressionFlag(1)) {
                            if (DSUtils.OverlayIsCompressed(1)) {
                                if (DSUtils.DecompressOverlay(1) < 0) {
                                    MessageBox.Show("Overlay 1 couldn't be decompressed.\nOverworld sprites in the Event Editor will be " +
                                "displayed incorrectly or not displayed at all.", "Unexpected EOF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }

                        break;
                }
            }

            /* Add event file numbers to box */
            statusLabel.Text = "Loading Events... Please wait.";
            Update();

            int eventCount = RomInfo.GetEventFileCount();
            int owSpriteCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.OWSprites].unpackedDir).Length;
            RomInfo.ReadOWTable();

            eventEditorWarpHeaderListBox.Items.Clear();
            eventEditorWarpHeaderListBox.Items.AddRange(headerListBoxNames.ToArray());
            eventEditorHeaderLocationNameLabel.Text = "";

            string[] trainerNames = GetTrainerNames();
            toolStripProgressBar.Maximum = (int)(eventCount + RomInfo.OverworldTable.Keys.Max() + trainerNames.Length);
            toolStripProgressBar.Value = 0;
            Update();

            /* Add event list to event combobox */
            selectEventComboBox.Items.Clear();
            for (int i = 0; i < eventCount; i++) {
                selectEventComboBox.Items.Add("Event File " + i);
                toolStripProgressBar.Value++;
            }

            /* Add sprite list to ow sprite box */
            owSpriteComboBox.Items.Clear();
            foreach (ushort key in RomInfo.OverworldTable.Keys) {
                owSpriteComboBox.Items.Add("OW Entry " + key);
                toolStripProgressBar.Value++;
            }

            /* Add trainer list to ow trainer box */
            owTrainerComboBox.Items.Clear();
            owTrainerComboBox.Items.AddRange(trainerNames);

            /* Add item list to ow item box */
            string[] itemNames = RomInfo.GetItemNames();
            if (ROMToolboxDialog.CheckScriptsStandardizedItemNumbers()) {
                UpdateItemComboBox(itemNames);
            } else {
                ScriptFile itemScript = new ScriptFile(RomInfo.itemScriptFileNumber);
                owItemComboBox.Items.Clear();
                foreach (CommandContainer cont in itemScript.allScripts) {
                    if (cont.commands.Count > 4) {
                        continue;
                    }
                    owItemComboBox.Items.Add(BitConverter.ToUInt16(cont.commands[1].cmdParams[1], 0) + "x " + itemNames[BitConverter.ToUInt16(cont.commands[0].cmdParams[1], 0)]);
                }
            }

            /* Add ow movement list to box */
            owMovementComboBox.Items.Clear();
            spawnableDirComboBox.Items.Clear();
            spawnableTypeComboBox.Items.Clear();
            owMovementComboBox.Items.AddRange(PokeDatabase.EventEditor.Overworlds.movementsArray);
            spawnableDirComboBox.Items.AddRange(PokeDatabase.EventEditor.Spawnables.orientationsArray);
            spawnableTypeComboBox.Items.AddRange(PokeDatabase.EventEditor.Spawnables.typesArray);

            /* Draw matrix 0 in matrix navigator */
            eventMatrix = new GameMatrix(0);

            showSpawnablesCheckBox.Checked = Properties.Settings.Default.renderSpawnables;
            showOwsCheckBox.Checked = Properties.Settings.Default.renderOverworlds;
            showWarpsCheckBox.Checked = Properties.Settings.Default.renderWarps;
            showTriggersCheckBox.Checked = Properties.Settings.Default.renderTriggers;

            if (owOrientationComboBox.SelectedIndex < 0 && overworldsListBox.Items.Count <= 0) {
                owOrientationComboBox.SelectedIndex = 2;
            }

            if (owMovementComboBox.SelectedIndex < 0 && overworldsListBox.Items.Count <= 0) {
                owOrientationComboBox.SelectedIndex = 1;
            }

            disableHandlers = false;

            selectEventComboBox.SelectedIndex = 0;
            owItemComboBox.SelectedIndex = 0;
            owTrainerComboBox.SelectedIndex = 0;

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;

            statusLabel.Text = "Ready";
        }
        private void addEventFileButton_Click(object sender, EventArgs e) {
            /* Add copy of event 0 to event folder */
            new EventFile().SaveToFileDefaultDir(selectEventComboBox.Items.Count);

            /* Update ComboBox and select new file */
            selectEventComboBox.Items.Add("Event File " + selectEventComboBox.Items.Count);
            selectEventComboBox.SelectedIndex = selectEventComboBox.Items.Count - 1;
        }
        private void eventEditorTabPage_Enter(object sender, EventArgs e) {
            eventOpenGlControl.MakeCurrent();
        }
        private void eventMatrixPictureBox_Click(object sender, EventArgs e) {
            const int squareSize = 16;
            Point coordinates = eventMatrixPictureBox.PointToClient(Cursor.Position);
            Point mouseTilePos = new Point(coordinates.X / squareSize, coordinates.Y / squareSize);

            MarkActiveCell(mouseTilePos.X, mouseTilePos.Y);
            eventMatrixXUpDown.Value = mouseTilePos.X;
            eventMatrixYUpDown.Value = mouseTilePos.Y;
        }
        private void eventMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            disableHandlers = true;

            eventMatrix = new GameMatrix((int)eventMatrixUpDown.Value);
            eventMatrixXUpDown.Value = 0;
            eventMatrixYUpDown.Value = 0;
            eventMatrixXUpDown.Maximum = eventMatrix.width - 1;
            eventMatrixYUpDown.Maximum = eventMatrix.height - 1;
            DrawEventMatrix();
            MarkUsedCells();

            disableHandlers = false;
        }
        private void eventShiftLeftButton_Click(object sender, EventArgs e) {
            if (eventMatrixXUpDown.Value > 0) {
                eventMatrixXUpDown.Value -= 1;
            }
        }
        private void eventShiftUpButton_Click(object sender, EventArgs e) {
            if (eventMatrixYUpDown.Value > 0) {
                eventMatrixYUpDown.Value -= 1;
            }
        }
        private void eventShiftRightButton_Click(object sender, EventArgs e) {
            if (eventMatrixXUpDown.Value < eventMatrix.width - 1)
                eventMatrixXUpDown.Value += 1;
        }
        private void eventShiftDownButton_Click(object sender, EventArgs e) {
            if (eventMatrixYUpDown.Value < eventMatrix.height - 1) {
                eventMatrixYUpDown.Value += 1;
            }
        }
        private void eventMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            DisplayEventMap();
            DisplayActiveEvents();
        }
        private void eventMatrixYUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            DisplayEventMap();
            DisplayActiveEvents();
        }
        private void exportEventFileButton_Click(object sender, EventArgs e) {
            currentEvFile.SaveToFileExplorePath("Event File " + selectEventComboBox.SelectedIndex);
        }
        private void saveEventsButton_Click(object sender, EventArgs e) {
            currentEvFile.SaveToFileDefaultDir(selectEventComboBox.SelectedIndex);
        }
        private void importEventFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .evt file */
            OpenFileDialog of = new OpenFileDialog {
                Filter = EventFile.DefaultFilter
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update event object on disk */
            string path = RomInfo.gameDirs[DirNames.eventFiles].unpackedDir + "\\" + selectEventComboBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Refresh controls */
            selectEventComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show("Events imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void removeEventFileButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("Are you sure you want to delete the last Event File?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d.Equals(DialogResult.Yes)) {
                /* Delete event file */
                File.Delete(RomInfo.gameDirs[DirNames.eventFiles].unpackedDir + "\\" + (selectEventComboBox.Items.Count - 1).ToString("D4"));

                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = selectEventComboBox.Items.Count - 1;
                if (selectEventComboBox.SelectedIndex == lastIndex) {
                    selectEventComboBox.SelectedIndex--;
                }

                /* Remove item from ComboBox */
                selectEventComboBox.Items.RemoveAt(lastIndex);
            }
        }
        private void selectEventComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            /* Load events data into EventFile class instance */
            currentEvFile = new EventFile(selectEventComboBox.SelectedIndex);

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
            if (disableHandlers) {
                return;
            }

            DisplayActiveEvents();
            Properties.Settings.Default.renderSpawnables = showSpawnablesCheckBox.Checked;
            Properties.Settings.Default.renderOverworlds = showOwsCheckBox.Checked;
            Properties.Settings.Default.renderWarps = showWarpsCheckBox.Checked;
            Properties.Settings.Default.renderTriggers = showTriggersCheckBox.Checked;
        }
        private void eventAreaDataUpDown_ValueChanged(object sender, EventArgs e) {
            DisplayEventMap(readGraphicsFromHeader: false);
        }
        private void eventPictureBox_Click(object sender, EventArgs e) { 
            Point coordinates = eventPictureBox.PointToClient(Cursor.Position);
            Point mouseTilePos = new Point(coordinates.X / eventScreenSquareSize, coordinates.Y / eventScreenSquareSize);
            MouseEventArgs mea = (MouseEventArgs)e;

            if (mea.Button == MouseButtons.Left) {
                if (selectedEvent != null) {
                    switch (selectedEvent.evType) {
                        case Event.EventType.Spawnable:
                            if (!showSpawnablesCheckBox.Checked) {
                                return;
                            }
                            spawnablexMapUpDown.Value = (short)mouseTilePos.X;
                            spawnableYMapUpDown.Value = (short)mouseTilePos.Y;
                            spawnableXMatrixUpDown.Value = (short)eventMatrixXUpDown.Value;
                            spawnableYMatrixUpDown.Value = (short)eventMatrixYUpDown.Value;

                            break;
                        case Event.EventType.Overworld:
                            if (!showOwsCheckBox.Checked) {
                                return;
                            }
                            owXMapUpDown.Value = (short)mouseTilePos.X;
                            owYMapUpDown.Value = (short)mouseTilePos.Y;
                            owXMatrixUpDown.Value = (short)eventMatrixXUpDown.Value;
                            owYMatrixUpDown.Value = (short)eventMatrixYUpDown.Value;

                            break;
                        case Event.EventType.Warp:
                            if (!showWarpsCheckBox.Checked) {
                                return;
                            }
                            warpXMapUpDown.Value = (short)mouseTilePos.X;
                            warpYMapUpDown.Value = (short)mouseTilePos.Y;
                            warpXMatrixUpDown.Value = (short)eventMatrixXUpDown.Value;
                            warpYMatrixUpDown.Value = (short)eventMatrixYUpDown.Value;

                            break;
                        case Event.EventType.Trigger:
                            if (!showTriggersCheckBox.Checked) {
                                return;
                            }
                            triggerXMapUpDown.Value = (short)mouseTilePos.X;
                            triggerYMapUpDown.Value = (short)mouseTilePos.Y;
                            triggerXMatrixUpDown.Value = (short)eventMatrixXUpDown.Value;
                            triggerYMatrixUpDown.Value = (short)eventMatrixYUpDown.Value;
                            
                            break;
                    }
                    DisplayActiveEvents();
                }
            } else if (mea.Button == MouseButtons.Right) {
                if (showWarpsCheckBox.Checked)
                    for (int i = 0; i < currentEvFile.warps.Count; i++) {
                        Warp ev = currentEvFile.warps[i];

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
                if (showSpawnablesCheckBox.Checked)
                    for (int i = 0; i < currentEvFile.spawnables.Count; i++) {
                        Spawnable ev = currentEvFile.spawnables[i];

                        if (isEventUnderMouse(ev, mouseTilePos)) {
                            selectedEvent = ev;
                            eventsTabControl.SelectedTab = signsTabPage;
                            spawnablesListBox.SelectedIndex = i;
                            DisplayActiveEvents();
                            return;
                        }
                    }
                if (showOwsCheckBox.Checked)
                    for (int i = 0; i < currentEvFile.overworlds.Count; i++) {
                        Overworld ev = currentEvFile.overworlds[i];

                        if (isEventUnderMouse(ev, mouseTilePos)) {
                            selectedEvent = ev;
                            eventsTabControl.SelectedTab = overworldsTabPage;
                            overworldsListBox.SelectedIndex = i;
                            DisplayActiveEvents();
                            return;
                        }
                    }
                for (int i = 0; i < currentEvFile.triggers.Count; i++) {
                    Trigger ev = currentEvFile.triggers[i];

                    if (isEventUnderMouse(ev, mouseTilePos, ev.widthX - 1, ev.heightY - 1)) {
                        selectedEvent = ev;
                        eventsTabControl.SelectedTab = triggersTabPage;
                        triggersListBox.SelectedIndex = i;
                        DisplayActiveEvents();
                        return;
                    }
                }
            } else if (mea.Button == MouseButtons.Middle) {
                for (int i = 0; i < currentEvFile.warps.Count; i++) {
                    Warp ev = currentEvFile.warps[i];

                    if (isEventUnderMouse(ev, mouseTilePos)) {
                        if (ev == selectedEvent) {
                            goToWarpDestination_Click(sender, e);
                            return;
                        }
                    }
                }
            }
        }
        #region Spawnables Tab
        private void addSpawnableButton_Click(object sender, EventArgs e) {
            currentEvFile.spawnables.Add(new Spawnable((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));
            spawnablesListBox.Items.Add("Spawnable " + (currentEvFile.spawnables.Count - 1).ToString());
            spawnablesListBox.SelectedIndex = currentEvFile.spawnables.Count - 1;
        }
        private void removeSpawnableButton_Click(object sender, EventArgs e) {
            if (spawnablesListBox.SelectedIndex < 0) {
                return;
            }

            disableHandlers = true;

            /* Remove trigger object from list and the corresponding entry in the ListBox */
            int spawnableNumber = spawnablesListBox.SelectedIndex;
            currentEvFile.spawnables.RemoveAt(spawnableNumber);
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

            currentEvFile.spawnables.Add(new Spawnable((Spawnable)selectedEvent));
            spawnablesListBox.Items.Add("Spawnable " + (currentEvFile.spawnables.Count - 1).ToString());
            spawnablesListBox.SelectedIndex = currentEvFile.spawnables.Count - 1;
        }
        private void spawnablesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;
            disableHandlers = true;

            /* Set Event */
            selectedEvent = currentEvFile.spawnables[spawnablesListBox.SelectedIndex];

            /* Update Controls */
            spawnableDirComboBox.SelectedIndex = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].dir;
            spawnableTypeComboBox.SelectedIndex = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].type;

            spawnableScriptUpDown.Value = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].scriptNumber;
            spawnablexMapUpDown.Value = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].xMapPosition;
            spawnableYMapUpDown.Value = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].yMapPosition;
            spawnableUpDown.Value = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].zPosition;
            spawnableXMatrixUpDown.Value = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].xMatrixPosition;
            spawnableYMatrixUpDown.Value = currentEvFile.spawnables[spawnablesListBox.SelectedIndex].yMatrixPosition;

            DisplayActiveEvents();
            disableHandlers = false;
        }
        private void spawnableMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEvFile.spawnables[spawnablesListBox.SelectedIndex].xMatrixPosition = (ushort)spawnableXMatrixUpDown.Value;
            DisplayActiveEvents();
        }
        private void spawnableMatrixYUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEvFile.spawnables[spawnablesListBox.SelectedIndex].yMatrixPosition = (ushort)spawnableYMatrixUpDown.Value;
            DisplayActiveEvents();
        }
        private void spawnableScriptUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;
            currentEvFile.spawnables[spawnablesListBox.SelectedIndex].scriptNumber = (ushort)spawnableScriptUpDown.Value;
        }
        private void spawnableMapXUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEvFile.spawnables[spawnablesListBox.SelectedIndex].xMapPosition = (short)spawnablexMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void spawnableMapYUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEvFile.spawnables[spawnablesListBox.SelectedIndex].yMapPosition = (short)spawnableYMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void spawnableZUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEvFile.spawnables[spawnablesListBox.SelectedIndex].zPosition = (short)spawnableUpDown.Value;
            DisplayActiveEvents();
        }
        private void spawnableDirComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || spawnablesListBox.SelectedIndex < 0)
                return;

            currentEvFile.spawnables[spawnablesListBox.SelectedIndex].dir = (ushort)spawnableDirComboBox.SelectedIndex;
        }
        private void spawnableTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (spawnablesListBox.SelectedIndex < 0)
                return;

            if (spawnableTypeComboBox.SelectedIndex == Spawnable.TYPE_HIDDENITEM) {
                spawnableDirComboBox.Enabled = false;
            } else {
                spawnableDirComboBox.Enabled = true;
            }

            if (disableHandlers) {
                return;
            }

            currentEvFile.spawnables[spawnablesListBox.SelectedIndex].type = (ushort)spawnableTypeComboBox.SelectedIndex;
        }
        #endregion

        #region Overworlds Tab
        private void addOverworldButton_Click(object sender, EventArgs e) {
            currentEvFile.overworlds.Add(new Overworld(currentEvFile.overworlds.Count + 1, (int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));
            overworldsListBox.Items.Add("Overworld " + (currentEvFile.overworlds.Count - 1).ToString());
            overworldsListBox.SelectedIndex = currentEvFile.overworlds.Count - 1;
        }
        private void removeOverworldButton_Click(object sender, EventArgs e) {
            if (overworldsListBox.SelectedIndex < 0) {
                return;
            }

            disableHandlers = true;

            /* Remove overworld object from list and the corresponding entry in the ListBox */
            int owNumber = overworldsListBox.SelectedIndex;
            currentEvFile.overworlds.RemoveAt(owNumber);
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
            Overworld NPCcopy = new Overworld((Overworld)selectedEvent);
            currentEvFile.overworlds.Add(NPCcopy);
            selectedEvent = NPCcopy;

            overworldsListBox.Items.Add("Overworld " + (currentEvFile.overworlds.Count - 1).ToString());
            overworldsListBox.SelectedIndex = currentEvFile.overworlds.Count - 1;
        }
        private void OWTypeChanged(object sender, EventArgs e) {
            if (overworldsListBox.SelectedIndex < 0) {
                return;
            }

            if (normalRadioButton.Checked == true) {
                owScriptNumericUpDown.Enabled = true;
                owSpecialGroupBox.Enabled = false;

                if (disableHandlers) {
                    return;
                }
                currentEvFile.overworlds[overworldsListBox.SelectedIndex].type = 0x0;
                currentEvFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(owScriptNumericUpDown.Value = 0);
            } else if (isItemRadioButton.Checked == true) {
                owScriptNumericUpDown.Enabled = false;

                owSpecialGroupBox.Enabled = true;
                owTrainerComboBox.Enabled = false;
                owTrainerLabel.Enabled = false;
                owSightRangeUpDown.Enabled = false;
                owSightRangeLabel.Enabled = false;
                owPartnerTrainerCheckBox.Enabled = false;

                if (disableHandlers) {
                    return;
                }
                if (isItemRadioButton.Enabled) {
                    owItemComboBox.Enabled = true;
                    itemsSelectorHelpBtn.Enabled = true;
                    owItemLabel.Enabled = true;

                    currentEvFile.overworlds[overworldsListBox.SelectedIndex].type = 0x3;
                    currentEvFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(owScriptNumericUpDown.Value = 7000 + owItemComboBox.SelectedIndex);
                }
            } else { //trainer
                owScriptNumericUpDown.Enabled = false;

                owSpecialGroupBox.Enabled = true;
                owTrainerComboBox.Enabled = true;
                owTrainerLabel.Enabled = true;
                owItemLabel.Enabled = false;
                owSightRangeUpDown.Enabled = true;
                owSightRangeLabel.Enabled = true;
                owPartnerTrainerCheckBox.Enabled = true;

                owItemComboBox.Enabled = false;
                itemsSelectorHelpBtn.Enabled = false;

                if (disableHandlers) {
                    return;
                }
                currentEvFile.overworlds[overworldsListBox.SelectedIndex].type = 0x1;
                if (owTrainerComboBox.SelectedIndex >= 0) {
                    owTrainerComboBox_SelectedIndexChanged(null, null);
                }
            }
        }
        private void owItemComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            owScriptNumericUpDown.Value = currentEvFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(7000 + owItemComboBox.SelectedIndex);
        }
        private void overworldsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            int index = overworldsListBox.SelectedIndex;

            if (disableHandlers || index < 0) {
                return;
            }
            disableHandlers = true;

            selectedEvent = currentEvFile.overworlds[index];
            Overworld selectedOw = (Overworld)selectedEvent;
            try {
                /* Sprite index and image controls */
                owSpriteComboBox.SelectedIndex = Array.IndexOf(RomInfo.overworldTableKeys, selectedOw.overlayTableEntry);
                owSpritePictureBox.BackgroundImage = GetOverworldImage(selectedOw.overlayTableEntry, selectedOw.orientation);
            } catch (ArgumentOutOfRangeException) {
                String errorMsg = "This Overworld's sprite ID couldn't be read correctly.";
                MessageBox.Show(errorMsg, "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try {
                /* Special settings controls */
                if (selectedOw.type == (ushort)Overworld.OwType.TRAINER) {
                    isTrainerRadioButton.Checked = true;
                    if (selectedOw.scriptNumber >= 4999) {
                        owTrainerComboBox.SelectedIndex = Math.Max(selectedOw.scriptNumber - 4999, 0); // Partner of double battle trainer
                        owPartnerTrainerCheckBox.Checked = true;
                    } else {
                        owTrainerComboBox.SelectedIndex = Math.Max(selectedOw.scriptNumber - 2999, 0); // Normal trainer
                        owPartnerTrainerCheckBox.Checked = false;
                    }
                } else if (selectedOw.type == (ushort)Overworld.OwType.ITEM || selectedOw.scriptNumber >= 7000 && selectedOw.scriptNumber <= 8000) {
                    isItemRadioButton.Checked = true;
                    owItemComboBox.SelectedIndex = Math.Max(selectedOw.scriptNumber - 7000, 0);
                } else {
                    normalRadioButton.Checked = true;
                }

                /* Set coordinates controls */
                owXMapUpDown.Value = selectedOw.xMapPosition;
                owYMapUpDown.Value = selectedOw.yMapPosition;
                owXMatrixUpDown.Value = selectedOw.xMatrixPosition;
                owYMatrixUpDown.Value = selectedOw.yMatrixPosition;
                owZPositionUpDown.Value = selectedOw.zPosition;

                /*ID, Flag and Script number controls */
                owIDNumericUpDown.Value = selectedOw.owID;
                owFlagNumericUpDown.Value = selectedOw.flag;
                owScriptNumericUpDown.Value = selectedOw.scriptNumber;

                /* Movement settings */
                owMovementComboBox.SelectedIndex = selectedOw.movement;
                owOrientationComboBox.SelectedIndex = selectedOw.orientation;
                owSightRangeUpDown.Value = selectedOw.sightRange;
                owXRangeUpDown.Value = selectedOw.xRange;
                owYRangeUpDown.Value = selectedOw.yRange;

                try {
                    uint spriteID = RomInfo.OverworldTable[currentEvFile.overworlds[overworldsListBox.SelectedIndex].overlayTableEntry].spriteID;
                    if (spriteID == 0x3D3D) {
                        spriteIDlabel.Text = "3D Overworld";
                    } else {
                        spriteIDlabel.Text = "Sprite ID: " + spriteID;
                    }
                } catch { }
                DisplayActiveEvents();
            } catch (ArgumentOutOfRangeException) {
                String errorMsg = "There was a problem loading the overworld events of this Event file.";
                MessageBox.Show(errorMsg, "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            disableHandlers = false;
        }
        private void owFlagNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.overworlds[overworldsListBox.SelectedIndex].flag = (ushort)owFlagNumericUpDown.Value;
        }
        private void owIDNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.overworlds[overworldsListBox.SelectedIndex].owID = (ushort)owIDNumericUpDown.Value;
        }
        private void owMovementComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.overworlds[overworldsListBox.SelectedIndex].movement = (ushort)owMovementComboBox.SelectedIndex;
        }
        private void owOrientationComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            ushort orientation = (ushort)owOrientationComboBox.SelectedIndex;
            if (owSpriteComboBox.SelectedIndex < 0 || orientation < 0) {
                return;
            }

            if (overworldsListBox.SelectedIndex >= 0) {
                owSpritePictureBox.BackgroundImage = GetOverworldImage(currentEvFile.overworlds[overworldsListBox.SelectedIndex].overlayTableEntry, orientation);

                if (!disableHandlers) {
                    currentEvFile.overworlds[overworldsListBox.SelectedIndex].orientation = orientation;
                    DisplayActiveEvents();
                }
            } else {
                owSpritePictureBox.BackgroundImage = GetOverworldImage((ushort)owSpriteComboBox.SelectedIndex, orientation);
            }

            owSpritePictureBox.Invalidate();
        }
        private void owScriptNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)owScriptNumericUpDown.Value;
        }
        private void owSightRangeUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.overworlds[overworldsListBox.SelectedIndex].sightRange = (ushort)owSightRangeUpDown.Value;
        }
        private void owSpriteComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (owSpriteComboBox.SelectedIndex < 0) {
                return;
            }
            ushort overlayTableEntryID = (ushort)RomInfo.OverworldTable.Keys.ElementAt(owSpriteComboBox.SelectedIndex);
            uint spriteID = RomInfo.OverworldTable[overlayTableEntryID].spriteID;

            if (spriteID == 0x3D3D) {
                spriteIDlabel.Text = "3D Overworld";
            } else {
                spriteIDlabel.Text = "Sprite ID: " + spriteID;
            }

            if (overworldsListBox.SelectedIndex >= 0) {
                owSpritePictureBox.BackgroundImage = GetOverworldImage(overlayTableEntryID, currentEvFile.overworlds[overworldsListBox.SelectedIndex].orientation);

                if (!disableHandlers) {
                    currentEvFile.overworlds[overworldsListBox.SelectedIndex].overlayTableEntry = overlayTableEntryID;
                    DisplayActiveEvents();
                }
            } else {
                owSpritePictureBox.BackgroundImage = GetOverworldImage(overlayTableEntryID, (ushort)owOrientationComboBox.SelectedIndex);
            }
            owSpritePictureBox.Invalidate();
        }
        private void owPartnerTrainerCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            if (owPartnerTrainerCheckBox.Checked) {
                currentEvFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber += 2000;
            } else {
                currentEvFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber -= 2000;
            }
        }
        private void owTrainerComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            owScriptNumericUpDown.Value = owPartnerTrainerCheckBox.Checked
                ? (currentEvFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(4999 + owTrainerComboBox.SelectedIndex))
                : (currentEvFile.overworlds[overworldsListBox.SelectedIndex].scriptNumber = (ushort)(2999 + owTrainerComboBox.SelectedIndex));
        }
        private void owXMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.overworlds[overworldsListBox.SelectedIndex].xMapPosition = (short)owXMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void owXRangeUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.overworlds[overworldsListBox.SelectedIndex].xRange = (ushort)owXRangeUpDown.Value;
        }
        private void owYRangeUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.overworlds[overworldsListBox.SelectedIndex].yRange = (ushort)owYRangeUpDown.Value;
        }
        private void owYMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.overworlds[overworldsListBox.SelectedIndex].yMapPosition = (short)owYMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void owZPositionUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.overworlds[overworldsListBox.SelectedIndex].zPosition = (short)owZPositionUpDown.Value;
        }
        private void owXMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.overworlds[overworldsListBox.SelectedIndex].xMatrixPosition = (ushort)owXMatrixUpDown.Value;
            eventMatrixPictureBox.Image = new Bitmap(eventMatrixPictureBox.Width, eventMatrixPictureBox.Height);
            DrawEventMatrix(); // Redraw matrix to eliminate old used cells
            MarkUsedCells(); // Mark new used cells
            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            DisplayActiveEvents();
        }
        private void owYMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || overworldsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.overworlds[overworldsListBox.SelectedIndex].yMatrixPosition = (ushort)owYMatrixUpDown.Value;
            eventMatrixPictureBox.Image = new Bitmap(eventMatrixPictureBox.Width, eventMatrixPictureBox.Height);
            DrawEventMatrix();
            MarkUsedCells();
            MarkActiveCell((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            DisplayActiveEvents();
        }

        #endregion

        #region Warps Tab
        private void addWarpButton_Click(object sender, EventArgs e) {
            Warp n = new Warp((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value);
            currentEvFile.warps.Add(n);

            int index = currentEvFile.warps.Count - 1;
            warpsListBox.Items.Add(index.ToString("D3") + ": " + n.ToString());
            warpsListBox.SelectedIndex = index;

            eventEditorWarpHeaderListBox.SelectedIndex = n.header;
        }
        private void removeWarpButton_Click(object sender, EventArgs e) {
            if (warpsListBox.SelectedIndex < 0) {
                return;
            }

            disableHandlers = true;

            /* Remove warp object from list and the corresponding entry in the ListBox */
            int warpNumber = warpsListBox.SelectedIndex;
            currentEvFile.warps.RemoveAt(warpNumber);
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

            Warp n = new Warp((Warp)selectedEvent);
            currentEvFile.warps.Add(n);

            int index = currentEvFile.warps.Count - 1;
            warpsListBox.Items.Add(index.ToString("D3") + ": " + n.ToString());

            eventEditorWarpHeaderListBox.SelectedIndex = n.header;
            warpsListBox.SelectedIndex = index;
        }
        private void warpAnchorUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.warps[warpsListBox.SelectedIndex].anchor = (ushort)warpAnchorUpDown.Value;
            updateSelectedWarpName();
        }
        private void eventEditorWarpHeaderListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (eventEditorWarpHeaderListBox.SelectedIndex < 0) {
                eventEditorHeaderLocationNameLabel.Text = "";
                return;
            }


            ushort destHeaderID = (ushort)eventEditorWarpHeaderListBox.SelectedIndex;

            MapHeader destHeader;
            if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                destHeader = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + destHeaderID.ToString("D4"), destHeaderID, 0);
            } else {
                destHeader = MapHeader.LoadFromARM9(destHeaderID);
            }

            int locNum;
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP: {
                        HeaderDP h = (HeaderDP)destHeader;

                        locNum = h.locationName;
                        break;
                    }
                case gFamEnum.Plat: {
                        HeaderPt h = (HeaderPt)destHeader;

                        locNum = h.locationName;
                        break;
                    }
                default: {
                        HeaderHGSS h = (HeaderHGSS)destHeader;

                        locNum = h.locationName;
                        break;
                    }
            }

            eventEditorHeaderLocationNameLabel.Text = (string)locationNameComboBox.Items[locNum];

            if (disableHandlers) {
                return;
            }
            currentEvFile.warps[warpsListBox.SelectedIndex].header = destHeaderID;
            updateSelectedWarpName();
        }
        private void updateSelectedWarpName() {
            int index = warpsListBox.SelectedIndex;
            warpsListBox.Items[index] = index.ToString("D3") + ": " + (selectedEvent as Warp).ToString();
        }
        private void warpsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0) {
                return;
            }


            selectedEvent = currentEvFile.warps[warpsListBox.SelectedIndex];
            eventEditorWarpHeaderListBox.SelectedIndex = currentEvFile.warps[warpsListBox.SelectedIndex].header;

            disableHandlers = true;

            warpAnchorUpDown.Value = currentEvFile.warps[warpsListBox.SelectedIndex].anchor;
            warpXMapUpDown.Value = currentEvFile.warps[warpsListBox.SelectedIndex].xMapPosition;
            warpYMapUpDown.Value = currentEvFile.warps[warpsListBox.SelectedIndex].yMapPosition;
            warpZUpDown.Value = currentEvFile.warps[warpsListBox.SelectedIndex].zPosition;
            warpXMatrixUpDown.Value = currentEvFile.warps[warpsListBox.SelectedIndex].xMatrixPosition;
            warpYMatrixUpDown.Value = currentEvFile.warps[warpsListBox.SelectedIndex].yMatrixPosition;

            DisplayActiveEvents(); // Redraw events to show selection box

            #region Re-enable events
            disableHandlers = false;
            #endregion
        }
        private void warpMatrixXUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.warps[warpsListBox.SelectedIndex].xMatrixPosition = (ushort)warpXMatrixUpDown.Value;
            DisplayActiveEvents();
        }
        private void warpMatrixYUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.warps[warpsListBox.SelectedIndex].yMatrixPosition = (ushort)warpYMatrixUpDown.Value;
            DisplayActiveEvents();
        }
        private void warpXMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.warps[warpsListBox.SelectedIndex].xMapPosition = (short)warpXMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void warpYMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.warps[warpsListBox.SelectedIndex].yMapPosition = (short)warpYMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void warpZUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || warpsListBox.SelectedIndex < 0) {
                return;
            }

            currentEvFile.warps[warpsListBox.SelectedIndex].zPosition = (short)warpZUpDown.Value;
            DisplayActiveEvents();
        }
        private void goToWarpDestination_Click(object sender, EventArgs e) {
            if (warpsListBox.SelectedIndex < 0) {
                return;
            }

            int destAnchor = (int)warpAnchorUpDown.Value;
            ushort destHeaderID = (ushort)eventEditorWarpHeaderListBox.SelectedIndex;

            MapHeader destHeader;
            if (ROMToolboxDialog.flag_DynamicHeadersPatchApplied || ROMToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                destHeader = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + destHeaderID.ToString("D4"), destHeaderID, 0);
            } else {
                destHeader = MapHeader.LoadFromARM9(destHeaderID);
            }

            if (new EventFile(destHeader.eventFileID).warps.Count < destAnchor + 1) {
                DialogResult d = MessageBox.Show("The selected warp's destination anchor doesn't exist.\n" +
                    "Do you want to open the destination map anyway?", "Warp is not connected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (d == DialogResult.No)
                    return;
                else {
                    eventMatrixUpDown.Value = destHeader.matrixID;
                    eventAreaDataUpDown.Value = destHeader.areaDataID;
                    selectEventComboBox.SelectedIndex = destHeader.eventFileID;
                    CenterEventViewOnEntities();
                    return;
                }
            }

            eventMatrixUpDown.Value = destHeader.matrixID;
            eventAreaDataUpDown.Value = destHeader.areaDataID;
            selectEventComboBox.SelectedIndex = destHeader.eventFileID;

            warpsListBox.SelectedIndex = destAnchor;
            centerEventViewOnSelectedEvent_Click(sender, e);
        }
        #endregion

        #region Triggers Tab
        private void addTriggerButton_Click(object sender, EventArgs e) {
            currentEvFile.triggers.Add(new Trigger((int)eventMatrixXUpDown.Value, (int)eventMatrixYUpDown.Value));
            triggersListBox.Items.Add("Trigger " + (currentEvFile.triggers.Count - 1).ToString());
            triggersListBox.SelectedIndex = currentEvFile.triggers.Count - 1;
        }
        private void removeTriggerButton_Click(object sender, EventArgs e) {
            if (triggersListBox.SelectedIndex < 0) {
                return;
            }

            disableHandlers = true;

            /* Remove trigger object from list and the corresponding entry in the ListBox */
            int triggerNumber = triggersListBox.SelectedIndex;
            currentEvFile.triggers.RemoveAt(triggerNumber);
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

            currentEvFile.triggers.Add(new Trigger((Trigger)selectedEvent));
            triggersListBox.Items.Add("Trigger " + (currentEvFile.triggers.Count - 1).ToString());
            triggersListBox.SelectedIndex = currentEvFile.triggers.Count - 1;
        }
        private void triggersListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            disableHandlers = true;

            selectedEvent = currentEvFile.triggers[triggersListBox.SelectedIndex];

            triggerScriptUpDown.Value = currentEvFile.triggers[triggersListBox.SelectedIndex].scriptNumber;
            triggerVariableWatchedUpDown.Value = currentEvFile.triggers[triggersListBox.SelectedIndex].variableWatched;
            expectedVarValueTriggerUpDown.Value = currentEvFile.triggers[triggersListBox.SelectedIndex].expectedVarValue;

            triggerWidthUpDown.Value = currentEvFile.triggers[triggersListBox.SelectedIndex].widthX;
            triggerLengthUpDown.Value = currentEvFile.triggers[triggersListBox.SelectedIndex].heightY;

            triggerXMapUpDown.Value = currentEvFile.triggers[triggersListBox.SelectedIndex].xMapPosition;
            triggerYMapUpDown.Value = currentEvFile.triggers[triggersListBox.SelectedIndex].yMapPosition;
            triggerZUpDown.Value = currentEvFile.triggers[triggersListBox.SelectedIndex].zPosition;
            triggerXMatrixUpDown.Value = currentEvFile.triggers[triggersListBox.SelectedIndex].xMatrixPosition;
            triggerYMatrixUpDown.Value = currentEvFile.triggers[triggersListBox.SelectedIndex].yMatrixPosition;

            DisplayActiveEvents();

            #region Re-enable events
            disableHandlers = false;
            #endregion
        }
        private void triggerVariableWatchedUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;
            currentEvFile.triggers[triggersListBox.SelectedIndex].variableWatched = (ushort)triggerVariableWatchedUpDown.Value;
        }
        private void expectedVarValueTriggerUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEvFile.triggers[triggersListBox.SelectedIndex].expectedVarValue = (ushort)expectedVarValueTriggerUpDown.Value;
        }
        private void triggerScriptUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;
            currentEvFile.triggers[triggersListBox.SelectedIndex].scriptNumber = (ushort)triggerScriptUpDown.Value;
        }
        private void triggerXMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEvFile.triggers[triggersListBox.SelectedIndex].xMapPosition = (short)triggerXMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerYMapUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEvFile.triggers[triggersListBox.SelectedIndex].yMapPosition = (short)triggerYMapUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerZUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEvFile.triggers[triggersListBox.SelectedIndex].zPosition = (ushort)triggerZUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerXMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEvFile.triggers[triggersListBox.SelectedIndex].xMatrixPosition = (ushort)triggerXMatrixUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerYMatrixUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEvFile.triggers[triggersListBox.SelectedIndex].yMatrixPosition = (ushort)triggerYMatrixUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerWidthUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEvFile.triggers[triggersListBox.SelectedIndex].widthX = (ushort)triggerWidthUpDown.Value;
            DisplayActiveEvents();
        }
        private void triggerLengthUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers || triggersListBox.SelectedIndex < 0)
                return;

            currentEvFile.triggers[triggersListBox.SelectedIndex].heightY = (ushort)triggerLengthUpDown.Value;
            DisplayActiveEvents();
        }
        #endregion
        #endregion

        #region Script Editor
        #region Variables
        //private static Mutex tooltipMutex = new Mutex();
        //private ScriptTooltip customTooltip;

        private bool scriptsDirty = false;
        private bool functionsDirty = false;
        private bool actionsDirty = false;

        private string cmdKeyWords = "";
        private string secondaryKeyWords = "";
        private ScriptFile currentScriptFile;
        #endregion
        #region Helper Methods
        private ScintillaNET.Scintilla ScriptTextArea;
        private ScintillaNET.Scintilla FunctionTextArea;
        private ScintillaNET.Scintilla ActionTextArea;

        private SearchManager scriptSearchManager;
        private SearchManager functionSearchManager;
        private SearchManager actionSearchManager;

        private Scintilla currentScintillaEditor;
        private SearchManager currentSearchManager;
        private void ScriptEditorSetClean() {
            disableHandlers = true;
            
            scriptsTabPage.Text = ScriptFile.containerTypes.Script.ToString() + "s";
            functionsTabPage.Text = ScriptFile.containerTypes.Function.ToString() + "s";
            actionsTabPage.Text = ScriptFile.containerTypes.Action.ToString() + "s";
            scriptsDirty = functionsDirty = actionsDirty = false;

            disableHandlers = false;
        }
        private void scriptEditorTabControl_TabIndexChanged(object sender, EventArgs e) {
            if (scriptEditorTabControl.SelectedTab == scriptsTabPage) {
                currentSearchManager = scriptSearchManager;
                currentScintillaEditor = ScriptTextArea;
            } else if (scriptEditorTabControl.SelectedTab == functionsTabPage) {
                currentSearchManager = functionSearchManager;
                currentScintillaEditor = FunctionTextArea;
            } else { //Actions
                currentSearchManager = actionSearchManager;
                currentScintillaEditor = ActionTextArea;
            }
        }
        private void SetupScriptEditorTextAreas() {
            //PREPARE SCRIPT EDITOR KEYWORDS
            cmdKeyWords = String.Join(" ", ScriptCommandNamesDict.Values) + 
                " " + String.Join(" ", ScriptDatabase.movementsDictIDName.Values);
            cmdKeyWords += " " + cmdKeyWords.ToUpper() + " " + cmdKeyWords.ToLower();

            secondaryKeyWords = String.Join(" ", RomInfo.ScriptComparisonOperatorsDict.Values) +
                " " + String.Join(" ", ScriptDatabase.specialOverworlds.Values) +
                " " + String.Join(" ", ScriptDatabase.overworldDirections.Values) +
                " " + ScriptFile.containerTypes.Script.ToString() +
                " " + ScriptFile.containerTypes.Function.ToString() +
                " " + ScriptFile.containerTypes.Action.ToString() +
                " " + EventType.Overworld +
                " " + Overworld.MovementCodeKW;
            secondaryKeyWords += " " + secondaryKeyWords.ToUpper() + " " + secondaryKeyWords.ToLower();


            // CREATE CONTROLS
            ScriptTextArea = new ScintillaNET.Scintilla();
            scriptSearchManager = new SearchManager(this, ScriptTextArea, panelSearchScriptTextBox, PanelSearchScripts);
            scintillaScriptsPanel.Controls.Add(ScriptTextArea);

            FunctionTextArea = new ScintillaNET.Scintilla();
            functionSearchManager = new SearchManager(this, FunctionTextArea, panelSearchFunctionTextBox, PanelSearchFunctions);
            scintillaFunctionsPanel.Controls.Add(FunctionTextArea);

            ActionTextArea = new ScintillaNET.Scintilla();
            actionSearchManager = new SearchManager(this, ActionTextArea, panelSearchActionTextBox, PanelSearchActions);
            scintillaActionsPanel.Controls.Add(ActionTextArea);

            currentScintillaEditor = ScriptTextArea;
            currentSearchManager = scriptSearchManager;

            // BASIC CONFIG
            ScriptTextArea.TextChanged += (this.OnTextChangedScript);
            FunctionTextArea.TextChanged += (this.OnTextChangedFunction);
            ActionTextArea.TextChanged += (this.OnTextChangedAction);

            // INITIAL VIEW CONFIG
            InitialViewConfig(ScriptTextArea);
            InitialViewConfig(FunctionTextArea);
            InitialViewConfig(ActionTextArea);

            InitSyntaxColoring(ScriptTextArea);
            InitSyntaxColoring(FunctionTextArea);
            InitSyntaxColoring(ActionTextArea);

            // NUMBER MARGIN
            InitNumberMargin(ScriptTextArea, ScriptTextArea_MarginClick);
            InitNumberMargin(FunctionTextArea, FunctionTextArea_MarginClick);
            InitNumberMargin(ActionTextArea, ActionTextArea_MarginClick);

            // BOOKMARK MARGIN
            InitBookmarkMargin(ScriptTextArea);
            InitBookmarkMargin(FunctionTextArea);
            InitBookmarkMargin(ActionTextArea);

            // CODE FOLDING MARGIN
            InitCodeFolding(ScriptTextArea);
            InitCodeFolding(FunctionTextArea);
            InitCodeFolding(ActionTextArea);

            // INIT HOTKEYS
            InitHotkeys(ScriptTextArea, scriptSearchManager);
            InitHotkeys(FunctionTextArea, functionSearchManager);
            InitHotkeys(ActionTextArea, actionSearchManager);

            // INIT TOOLTIPS DWELLING
            /*
            ScriptTextArea.MouseDwellTime = 300;
            ScriptTextArea.DwellEnd += TextArea_DwellEnd;
            ScriptTextArea.DwellStart += TextArea_DwellStart;

            FunctionTextArea.MouseDwellTime = 300;
            FunctionTextArea.DwellEnd += TextArea_DwellEnd;
            FunctionTextArea.DwellStart += TextArea_DwellStart;
            */
        }

        /*
        private void TextArea_DwellStart(object sender, DwellEventArgs e) {
            TextArea_DwellEnd(sender, e);
            Scintilla ctr = sender as Scintilla;
            string hoveredWord = ctr.GetWordFromPosition(e.Position);
            ushort cmdID;

            string commandName = "";
            if (RomInfo.ScriptCommandNamesReverseDict.TryGetValue(hoveredWord, out cmdID)) {
                commandName = hoveredWord;
            } else {
                if (!ushort.TryParse(hoveredWord, NumberStyles.HexNumber, new CultureInfo("en-US"), out cmdID)) {
                    return;
                }
            }
            string tip = "";

            tooltipMutex.WaitOne();
            tip += cmdID.ToString("X4") + ": " + commandName + "(";
            byte[] parameters = ScriptCommandParametersDict[cmdID];
            for (int i = 0; i < parameters.Length; i++) {
                if (parameters[i] == 0) {
                    break;
                } else if (parameters[i] == 1) {
                    tip += "byte";
                } else {
                    tip += "uint" + 8 * parameters[i];
                }
                if (i != parameters.Length - 1) {
                    tip += ", ";
                }
            }
            tip += ")";
            tip += Environment.NewLine + "Command descriptions aren't available yet.";

            Point globalCtrCoords = ctr.PointToScreen(ctr.Location);
            Point incrementedCoords = new Point(globalCtrCoords.X + e.X, globalCtrCoords.Y + e.Y);

            customTooltip = new ScriptTooltip(cmdKeyWords, tip);
            customTooltip.Visible = false;
            customTooltip.Show();

            int newy = incrementedCoords.Y - customTooltip.Size.Height - 5;
            customTooltip.Location = new Point(incrementedCoords.X, newy);
            customTooltip.BringToFront();
            customTooltip.Visible = true;
            Thread t = new Thread(() => {
                customTooltip.Invoke((MethodInvoker)delegate {
                    customTooltip.ctrl.Visible = true;
                    customTooltip.FadeIn(16, 9);
                    customTooltip.WriteText(4);
                });
            });
            t.Start();
            tooltipMutex.ReleaseMutex();
        }
        private void TextArea_DwellEnd(object sender, DwellEventArgs e) {
            if (customTooltip != null && !customTooltip.IsDisposed) {
                tooltipMutex.WaitOne();
                Thread t = new Thread(() => {
                    customTooltip.Invoke((MethodInvoker)delegate {
                        customTooltip.FadeOut(16, 9);
                        customTooltip.Close();
                        customTooltip.Dispose();
                    });

                });
                t.Start();
                tooltipMutex.ReleaseMutex();
            }
        }
        */

        private void InitNumberMargin(Scintilla textArea, EventHandler<MarginClickEventArgs> textArea_MarginClick) {
            textArea.Styles[Style.LineNumber].BackColor = BACK_COLOR;
            textArea.Styles[Style.LineNumber].ForeColor = FORE_COLOR;
            textArea.Styles[Style.IndentGuide].ForeColor = FORE_COLOR;
            textArea.Styles[Style.IndentGuide].BackColor = BACK_COLOR;

            var nums = textArea.Margins[NUMBER_MARGIN];
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            textArea.MarginClick += textArea_MarginClick;
        }

        private void InitHotkeys(Scintilla scintillaTb, SearchManager sm) {
            // register the hotkeys with the form
            HotKeyManager.AddHotKey(scintillaTb, sm.OpenSearch, Keys.F, true);
            HotKeyManager.AddHotKey(scintillaTb, () => Uppercase(scintillaTb), Keys.U, true);
            HotKeyManager.AddHotKey(scintillaTb, () => Lowercase(scintillaTb), Keys.L, true);
            HotKeyManager.AddHotKey(scintillaTb, () => ZoomIn(scintillaTb), Keys.Oemplus, true);
            HotKeyManager.AddHotKey(scintillaTb, () => ZoomOut(scintillaTb), Keys.OemMinus, true);
            HotKeyManager.AddHotKey(scintillaTb, () => ZoomDefault(scintillaTb), Keys.D0, true);
            HotKeyManager.AddHotKey(scintillaTb, sm.CloseSearch, Keys.Escape);

            // remove conflicting hotkeys from scintilla
            scintillaTb.ClearCmdKey(Keys.Control | Keys.F);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.R);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.H);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.L);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.U);
        }

        private void InitSyntaxColoring(Scintilla textArea) {

            // Configure the default style
            textArea.StyleResetDefault();
            textArea.Styles[Style.Default].Font = "Consolas";
            textArea.Styles[Style.Default].Size = 12;
            textArea.Styles[Style.Default].BackColor = Color.FromArgb(0x212121);
            textArea.Styles[Style.Default].ForeColor = Color.FromArgb(0xFFFFFF);
            textArea.StyleClearAll();

            // Configure the lexer styles
            textArea.Styles[Style.Python.Identifier].ForeColor = Color.FromArgb(0xD0DAE2);
            textArea.Styles[Style.Python.CommentLine].ForeColor = Color.FromArgb(0x40BF57);
            textArea.Styles[Style.Python.Number].ForeColor = Color.FromArgb(0xFFFF00);
            textArea.Styles[Style.Python.String].ForeColor = Color.FromArgb(0xFF00FF);
            textArea.Styles[Style.Python.Character].ForeColor = Color.FromArgb(0xE95454);
            textArea.Styles[Style.Python.Operator].ForeColor = Color.FromArgb(0xFFFF00);
            textArea.Styles[Style.Python.Word].ForeColor = Color.FromArgb(0x48A8EE);
            textArea.Styles[Style.Python.Word2].ForeColor = Color.FromArgb(0xF98906);

            textArea.Lexer = Lexer.Python;

            textArea.SetKeywords(0, cmdKeyWords);
            textArea.SetKeywords(1, secondaryKeyWords);
        }
        private void openSearchScriptEditorButton_Click(object sender, EventArgs e) {
            currentSearchManager.OpenSearch();
        }

        private void OnTextChangedScript(object sender, EventArgs e) {
            ScriptTextArea.Margins[NUMBER_MARGIN].Width = ScriptTextArea.Lines.Count.ToString().Length * 13;
            scriptsDirty = true;
            scriptsTabPage.Text = ScriptFile.containerTypes.Script.ToString() + "s" + "*";
        }
        private void OnTextChangedFunction(object sender, EventArgs e) {
            FunctionTextArea.Margins[NUMBER_MARGIN].Width = FunctionTextArea.Lines.Count.ToString().Length * 13;
            functionsDirty = true;
            functionsTabPage.Text = ScriptFile.containerTypes.Function.ToString() + "s" + "*";
        }
        private void OnTextChangedAction(object sender, EventArgs e) {
            ActionTextArea.Margins[NUMBER_MARGIN].Width = ActionTextArea.Lines.Count.ToString().Length * 13;
            actionsDirty = true;
            actionsTabPage.Text = ScriptFile.containerTypes.Action.ToString() + "s" + "*";
        }


        #region Numbers, Bookmarks, Code Folding

        /// <summary>
        /// the background color of the text area
        /// </summary>
        private readonly Color BACK_COLOR = Color.FromArgb(0x2A211C);

        /// <summary>
        /// default text color of the text area
        /// </summary>
        private readonly Color FORE_COLOR = Color.FromArgb(0xB7B7B7);

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;
        private const int BOOKMARK_MARKER = 2;

        /// <summary>
        /// change this to whatever margin you want the code folding tree (+/-) to show in
        /// </summary>
        private const int FOLDING_MARGIN = 3;

        /// <summary>
        /// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
        /// </summary>
        private const bool CODEFOLDING_CIRCULAR = true;


        private void InitialViewConfig(Scintilla textArea) {
            textArea.Dock = DockStyle.Fill;
            textArea.WrapMode = ScintillaNET.WrapMode.Word;
            textArea.IndentationGuides = IndentView.LookBoth;
            textArea.CaretPeriod = 500;
            textArea.CaretForeColor = Color.White;
            textArea.SetSelectionBackColor(true, Color.FromArgb(0x114D9C));
            textArea.WrapIndentMode = WrapIndentMode.Same;
        }

        private void InitBookmarkMargin(Scintilla textArea) {
            //TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

            var margin = textArea.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BOOKMARK_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            var marker = textArea.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(Color.FromArgb(0xFF003B));
            marker.SetForeColor(Color.FromArgb(0x000000));
            marker.SetAlpha(100);
        }

        private void InitCodeFolding(Scintilla textArea) {
            textArea.SetFoldMarginColor(true, BACK_COLOR);
            textArea.SetFoldMarginHighlightColor(true, BACK_COLOR);

            // Enable code folding
            textArea.SetProperty("fold", "1");
            textArea.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            textArea.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
            textArea.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
            textArea.Margins[FOLDING_MARGIN].Sensitive = true;
            textArea.Margins[FOLDING_MARGIN].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++) {
                textArea.Markers[i].SetForeColor(BACK_COLOR); // styles for [+] and [-]
                textArea.Markers[i].SetBackColor(FORE_COLOR); // styles for [+] and [-]
            }

            // Configure folding markers with respective symbols
            textArea.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
            textArea.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
            textArea.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
            textArea.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            textArea.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
            textArea.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            textArea.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            textArea.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }

        private void ScriptTextArea_MarginClick(object sender, MarginClickEventArgs e) {
            MarginClick(ScriptTextArea, e);
        }

        private void FunctionTextArea_MarginClick(object sender, MarginClickEventArgs e) {
            MarginClick(FunctionTextArea, e);
        }

        private void ActionTextArea_MarginClick(object sender, MarginClickEventArgs e) {
            MarginClick(ActionTextArea, e);
        }

        private void MarginClick(Scintilla textArea, MarginClickEventArgs e) {
            if (e.Margin == BOOKMARK_MARGIN) {
                // Do we have a marker for this line?
                const uint mask = (1 << BOOKMARK_MARKER);
                var line = textArea.Lines[textArea.LineFromPosition(e.Position)];
                if ((line.MarkerGet() & mask) > 0) {
                    // Remove existing bookmark
                    line.MarkerDelete(BOOKMARK_MARKER);
                } else {
                    // Add bookmark
                    line.MarkerAdd(BOOKMARK_MARKER);
                }
            }
        }

        #endregion

        #region Main Menu Commands

        //private void selectLineToolStripMenuItem_Click(object sender, EventArgs e) {
        //    Line line = TextArea.Lines[TextArea.CurrentLine];
        //    TextArea.SetSelection(line.Position + line.Length, line.Position);
        //}
        private void scriptEditorWordWrapCheckbox_CheckedChanged(object sender, EventArgs e) {
            ScriptTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? ScintillaNET.WrapMode.Word : ScintillaNET.WrapMode.None;
            FunctionTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? ScintillaNET.WrapMode.Word : ScintillaNET.WrapMode.None;
            ActionTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? ScintillaNET.WrapMode.Word : ScintillaNET.WrapMode.None;
        }
        //private void indentGuidesCheckbox_CheckedChanged(object sender, EventArgs e) {
        //    ScriptTextArea.IndentationGuides = scriptEditorIndentGuidesCheckbox.Checked ? IndentView.LookBoth : IndentView.None;
        //    FunctionTextArea.IndentationGuides = scriptEditorIndentGuidesCheckbox.Checked ? IndentView.LookBoth : IndentView.None;
        //    ActionTextArea.IndentationGuides = scriptEditorIndentGuidesCheckbox.Checked ? IndentView.LookBoth : IndentView.None;
        //}

        private void viewWhiteSpacesButton_Click(object sender, EventArgs e) {
            ScriptTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
            FunctionTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
            ActionTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
        }

        private void scriptEditorZoomInButton_Click(object sender, EventArgs e) {
            ZoomIn(currentScintillaEditor);
        }
        
        private void scriptEditorZoomOutButton_Click(object sender, EventArgs e) {
            ZoomOut(currentScintillaEditor);
        }
        
        private void scriptEditorZoomResetButton_Click(object sender, EventArgs e) {
            ZoomDefault(currentScintillaEditor);
        }

        private void ScriptEditorCollapseButton_Click(object sender, EventArgs e) {
            currentScintillaEditor.FoldAll(FoldAction.Contract);
        }

        private void ScriptEditorExpandButton_Click(object sender, EventArgs e) {
            currentScintillaEditor.FoldAll(FoldAction.Expand);
        }


        #endregion

        #region Uppercase / Lowercase

        private void Lowercase(Scintilla textArea) {

            // save the selection
            int start = textArea.SelectionStart;
            int end = textArea.SelectionEnd;

            // modify the selected text
            textArea.ReplaceSelection(textArea.GetTextRange(start, end - start).ToLower());

            // preserve the original selection
            textArea.SetSelection(start, end);
        }

        private void Uppercase(Scintilla textArea) {
            // save the selection
            int start = textArea.SelectionStart;
            int end = textArea.SelectionEnd;

            // modify the selected text
            textArea.ReplaceSelection(textArea.GetTextRange(start, end - start).ToUpper());

            // preserve the original selection
            textArea.SetSelection(start, end);
        }

        #endregion

        #region Indent / Outdent

        private void GenerateKeystrokes(string keys, Scintilla textArea) {
            //Example
            //GenerateKeystrokes("+{TAB}");
            HotKeyManager.Enable = false;
            textArea.Focus();
            SendKeys.Send(keys);
            HotKeyManager.Enable = true;
        }

        #endregion

        #region Zoom

        private void ZoomIn(Scintilla textArea) {
            textArea.ZoomIn();
        }

        private void ZoomOut(Scintilla textArea) {
            textArea.ZoomOut();
        }

        private void ZoomDefault(Scintilla textArea) {
            textArea.Zoom = 0;
        }
        #endregion

        #region Quick Search Bar
        private void BtnPrevSearchScript_Click(object sender, EventArgs e) {
            scriptSearchManager.Find(false, false);
        }

        private void BtnNextSearchScript_Click(object sender, EventArgs e) {
            scriptSearchManager.Find(true, false);
        }

        private void BtnPrevSearchFunc_Click(object sender, EventArgs e) {
            functionSearchManager.Find(false, false);
        }

        private void BtnNextSearchFunc_Click(object sender, EventArgs e) {
            functionSearchManager.Find(true, false);
        }

        private void BtnPrevSearchActions_Click(object sender, EventArgs e) {
            actionSearchManager.Find(false, false);
        }

        private void BtnNextSearchActions_Click(object sender, EventArgs e) {
            actionSearchManager.Find(true, false);
        }

        private void BtnCloseSearchScript_Click(object sender, EventArgs e) {
            scriptSearchManager.CloseSearch();
        }

        private void BtnCloseSearchFunc_Click(object sender, EventArgs e) {
            functionSearchManager.CloseSearch();
        }

        private void BtnCloseSearchActions_Click(object sender, EventArgs e) {
            actionSearchManager.CloseSearch();
        }

        private void scriptTxtSearch_KeyDown(object sender, KeyEventArgs e) {
            TxtSearchKeyDown(scriptSearchManager, e);
        }
        private void functionTxtSearch_KeyDown(object sender, KeyEventArgs e) {
            TxtSearchKeyDown(functionSearchManager, e);
        }
        private void actiontTxtSearch_KeyDown(object sender, KeyEventArgs e) {
            TxtSearchKeyDown(actionSearchManager, e);
        }

        private void TxtSearchKeyDown(SearchManager sm, KeyEventArgs e) {
            if (HotKeyManager.IsHotkey(e, Keys.Enter)) {
                sm.Find(true, false);
            }
            if (HotKeyManager.IsHotkey(e, Keys.Enter, true) || HotKeyManager.IsHotkey(e, Keys.Enter, false, true)) {
                sm.Find(false, false);
            }
        }

        private void panelSearchScriptTextBox_TextChanged(object sender, EventArgs e) {
            scriptSearchManager.Find(true, true);
        }
        private void panelSearchFunctionTextBox_TextChanged(object sender, EventArgs e) {
            functionSearchManager.Find(true, true);
        }
        private void panelSearchActionTextBox_TextChanged(object sender, EventArgs e) {
            actionSearchManager.Find(true, true);
        }

        #endregion
        private void addScriptFileButton_Click(object sender, EventArgs e) {
            /* Add new event file to event folder */
            string scriptFilePath = RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + selectScriptFileComboBox.Items.Count.ToString("D4");
            File.WriteAllBytes(scriptFilePath, new ScriptFile(0).ToByteArray());

            /* Update ComboBox and select new file */
            selectScriptFileComboBox.Items.Add("Script File " + selectScriptFileComboBox.Items.Count);
            selectScriptFileComboBox.SelectedIndex = selectScriptFileComboBox.Items.Count - 1;
        }
        private void exportScriptFileButton_Click(object sender, EventArgs e) {
            string suggestion = "Script File ";
            if (currentScriptFile.isLevelScript) {
                suggestion = "Level " + suggestion;
            }
            currentScriptFile.SaveToFileExplorePath(suggestion + selectScriptFileComboBox.SelectedIndex, blindmode: true);
        }
        private void saveScriptFileButton_Click(object sender, EventArgs e) {
            /* Create new ScriptFile object */
            int idToAssign = selectScriptFileComboBox.SelectedIndex;

            ScriptFile userEdited = new ScriptFile(
                scriptLines: ScriptTextArea.Lines.ToStringsList(trim: true), 
                functionLines: FunctionTextArea.Lines.ToStringsList(trim: true), 
                actionLines: ActionTextArea.Lines.ToStringsList(trim: true), 
                selectScriptFileComboBox.SelectedIndex
            );

            /* Write new scripts to file */
            if (userEdited.fileID == null) {
                MessageBox.Show("This " + typeof(ScriptFile).Name + " couldn't be saved, due to a processing error.", "Can't save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else if (userEdited.fileID == int.MaxValue) {
                MessageBox.Show("This " + typeof(ScriptFile).Name + " is couldn't be saved since it's empty.", "Can't save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else { //check if ScriptFile instance was created succesfully
                userEdited.SaveToFileDefaultDir(selectScriptFileComboBox.SelectedIndex);
                currentScriptFile = userEdited;
                ScriptEditorSetClean();
            }
        }
        private void clearCurrentLevelScriptButton_Click(object sender, EventArgs e) {
            string path = RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + selectScriptFileComboBox.SelectedIndex.ToString("D4");
            File.WriteAllBytes(path, new byte[4]);
            MessageBox.Show("Level script correctly cleared.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importScriptFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .scr or .bin file */
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Script File (*.scr, *.bin)|*.scr;*.bin"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update scriptFile object in memory */
            string path = RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + selectScriptFileComboBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Refresh controls */
            selectScriptFileComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show("Scripts imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void openScriptButton_Click(object sender, EventArgs e) {
            if (!scriptEditorIsReady) {
                SetupScriptEditorTextAreas();
                SetupScriptEditor();
                scriptEditorIsReady = true;
            }

            scriptEditorTabControl.SelectedIndex = 0;
            selectScriptFileComboBox.SelectedIndex = (int)scriptFileUpDown.Value;
            mainTabControl.SelectedTab = scriptEditorTabPage;
        }
        private void openLevelScriptButton_Click(object sender, EventArgs e) {
            if (!scriptEditorIsReady) {
                SetupScriptEditorTextAreas();
                SetupScriptEditor();
                scriptEditorIsReady = true;
            }

            selectScriptFileComboBox.SelectedIndex = (int)levelScriptUpDown.Value;
            mainTabControl.SelectedTab = scriptEditorTabPage;
        }
        private void removeScriptFileButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("Are you sure you want to delete the last Script File?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d.Equals(DialogResult.Yes)) {
                /* Delete script file */
                File.Delete(RomInfo.gameDirs[DirNames.scripts].unpackedDir + "\\" + (selectScriptFileComboBox.Items.Count - 1).ToString("D4"));

                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = selectScriptFileComboBox.Items.Count - 1;
                if (selectScriptFileComboBox.SelectedIndex == lastIndex) {
                    selectScriptFileComboBox.SelectedIndex--;
                }

                /* Remove item from ComboBox */
                selectScriptFileComboBox.Items.RemoveAt(lastIndex);
            }
        }
        private void searchInScriptsButton_Click(object sender, EventArgs e) {
            if (searchInScriptsTextBox.Text == "") {
                return;
            }

            int firstArchive;
            int lastArchive;

            if (searchOnlyCurrentScriptCheckBox.Checked) {
                firstArchive = selectScriptFileComboBox.SelectedIndex;
                lastArchive = firstArchive + 1;
            } else {
                firstArchive = 0;
                lastArchive = romInfo.GetScriptCount();
            }

            searchInScriptsResultListBox.Items.Clear();
            string searchString = searchInScriptsTextBox.Text;
            searchProgressBar.Maximum = selectScriptFileComboBox.Items.Count;

            List<string> results = new List<string>();

            string scriptKw = ScriptFile.containerTypes.Script.ToString();
            string functionKw = ScriptFile.containerTypes.Function.ToString();

            for (int i = firstArchive; i < lastArchive; i++) {
                try {
                    Console.WriteLine("Attempting to load script " + i);
                    ScriptFile file = new ScriptFile(i, readActions: false);

                    if (scriptSearchCaseSensitiveCheckBox.Checked) {
                        results.AddRange(SearchInScripts(i, file.allScripts, scriptKw, (string s) => s.Contains(searchString)));
                        results.AddRange(SearchInScripts(i, file.allFunctions, functionKw, (string s) => s.Contains(searchString)));
                    } else {
                        results.AddRange(SearchInScripts(i, file.allScripts, scriptKw, (string s) => s.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0));
                        results.AddRange(SearchInScripts(i, file.allFunctions, functionKw, (string s) => s.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0));
                    }
                } catch { }
                searchProgressBar.Value = i;
            }

            searchProgressBar.Value = 0;
            searchInScriptsResultListBox.Items.AddRange(results.ToArray());
        }
        private List<string> SearchInScripts(int fileID, List<CommandContainer> cmdList, string entryType, Func<string, bool> criteria) {
            List<string> results = new List<string>();

            for (int j = 0; j < cmdList.Count; j++) { 
                if (cmdList[j].commands is null) {
                    continue;
                }
                foreach (ScriptCommand cur in cmdList[j].commands) {
                    if (criteria(cur.name)) {
                        results.Add($"File {fileID} - {entryType} {j + 1}: {cur.name}{Environment.NewLine}");
                    }
                }
            }
            return results;
        }
        private void searchInScripts_GoToEntryResult(object sender, MouseEventArgs e) {
            if (searchInScriptsResultListBox.SelectedIndex < 0) {
                return;
            }

            string[] split = searchInScriptsResultListBox.SelectedItem.ToString().Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            selectScriptFileComboBox.SelectedIndex = int.Parse(split[1]);
            string cmdNameAndParams = String.Join(" ", split.Skip(5).Take(split.Length - 5));

            if (split[3].StartsWith(ScriptFile.containerTypes.Script.ToString())) {
                if (scriptEditorTabControl.SelectedTab != scriptsTabPage) {
                    scriptEditorTabControl.SelectedTab = scriptsTabPage;
                }
                scriptSearchManager.Find(true, false, ScriptFile.containerTypes.Script.ToString() + " " + split[4].Replace(":", ""));
                scriptSearchManager.Find(true, false, cmdNameAndParams);
            } else if (split[3].StartsWith(ScriptFile.containerTypes.Function.ToString())) {
                if (scriptEditorTabControl.SelectedTab != functionsTabPage) {
                    scriptEditorTabControl.SelectedTab = functionsTabPage;
                }
                functionSearchManager.Find(true, false, ScriptFile.containerTypes.Function.ToString() + " " + split[4].Replace(":", ""));
                functionSearchManager.Find(true, false, cmdNameAndParams);
            } else if (split[3].StartsWith(ScriptFile.containerTypes.Action.ToString())) {
                if (scriptEditorTabControl.SelectedTab != actionsTabPage) {
                    scriptEditorTabControl.SelectedTab = actionsTabPage;
                }
                actionSearchManager.Find(true, false, ScriptFile.containerTypes.Action.ToString() + " " + split[4].Replace(":", ""));
                actionSearchManager.Find(true, false, cmdNameAndParams);
            }
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
            ReloadScript();
        }

        private bool ReloadScript() {
            Console.WriteLine("Script Reload has been requested");
            /* clear controls */
            if (disableHandlers || selectScriptFileComboBox.SelectedIndex < 0) {
                return false;
            }

            if (scriptsDirty || functionsDirty || actionsDirty) {
                DialogResult d = MessageBox.Show("There are unsaved changes in this Script File.\nDo you wish to discard them?", "Unsaved work", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                
                if (!d.Equals(DialogResult.Yes)) {
                    disableHandlers = true;
                    selectScriptFileComboBox.SelectedIndex = (int)currentScriptFile.fileID;
                    disableHandlers = false;
                    return false;
                }
            }
            currentScriptFile = new ScriptFile(selectScriptFileComboBox.SelectedIndex); // Load script file

            ScriptTextArea.ClearAll();
            FunctionTextArea.ClearAll();
            ActionTextArea.ClearAll();

            scriptsNavListbox.Items.Clear();
            functionsNavListbox.Items.Clear();
            actionsNavListbox.Items.Clear();

            if (currentScriptFile.isLevelScript) {
                ScriptTextArea.Text += "LevelScript files are currently not supported.\nYou can use AdAstra's Level Scripts Editor.";
                addScriptFileButton.Visible = false;
                removeScriptFileButton.Visible = false;

                clearCurrentLevelScriptButton.Visible = true;
            } else {
                disableHandlers = true;
                addScriptFileButton.Visible = true;
                removeScriptFileButton.Visible = true;

                clearCurrentLevelScriptButton.Visible = false;

                string buffer = "";

                /* Add scripts */
                for (int i = 0; i < currentScriptFile.allScripts.Count; i++) {
                    CommandContainer currentScript = currentScriptFile.allScripts[i];

                    /* Write header */
                    string header = ScriptFile.containerTypes.Script.ToString() + " " + (i + 1);
                    buffer += header + ':' + Environment.NewLine;
                    scriptsNavListbox.Items.Add(header);

                    /* If current script is identical to another, print UseScript instead of commands */
                    if (currentScript.usedScript < 0) {
                        for (int j = 0; j < currentScript.commands.Count; j++) {
                            if (!ScriptDatabase.endCodes.Contains(currentScript.commands[j].id)) {
                                buffer += '\t';
                            }
                            buffer += currentScript.commands[j].name + Environment.NewLine;
                        }
                    } else {
                        buffer += '\t' + "UseScript_#" + currentScript.usedScript + Environment.NewLine;
                    }

                    ScriptTextArea.AppendText(buffer + Environment.NewLine);
                    buffer = "";
                }


                /* Add functions */
                for (int i = 0; i < currentScriptFile.allFunctions.Count; i++) {
                    CommandContainer currentFunction = currentScriptFile.allFunctions[i];

                    /* Write Heaader */
                    string header = ScriptFile.containerTypes.Function.ToString() + " " + (i + 1);
                    buffer += header + ':' + Environment.NewLine;
                    functionsNavListbox.Items.Add(header);

                    /* If current function is identical to a script, print UseScript instead of commands */
                    if (currentFunction.usedScript < 0) {
                        for (int j = 0; j < currentFunction.commands.Count; j++) {
                            if (!ScriptDatabase.endCodes.Contains(currentFunction.commands[j].id)) {
                                buffer += '\t';
                            }
                            buffer += currentFunction.commands[j].name + Environment.NewLine;
                        }
                    } else {
                        buffer += '\t' + "UseScript_#" + currentFunction.usedScript + Environment.NewLine;
                    }

                    FunctionTextArea.AppendText(buffer + Environment.NewLine);
                    buffer = "";
                }

                /* Add movements */
                for (int i = 0; i < currentScriptFile.allActions.Count; i++) {
                    ActionContainer currentAction = currentScriptFile.allActions[i];

                    string header = ScriptFile.containerTypes.Action.ToString() + " " + (i + 1);
                    buffer += header + ':' + Environment.NewLine;
                    actionsNavListbox.Items.Add(header);

                    for (int j = 0; j < currentAction.actionCommandsList.Count; j++) {
                        if (currentAction.actionCommandsList[j].id != 0x00FE) {
                            buffer += '\t';
                        }
                        buffer += currentAction.actionCommandsList[j].name + Environment.NewLine;
                    }

                    ActionTextArea.AppendText(buffer + Environment.NewLine);
                    buffer = "";
                }
            }

            ScriptEditorSetClean();
            statusLabel.Text = "Ready";
            disableHandlers = false;
            return true;
        }

        private void UpdateScriptNumberFormatDec(object sender, EventArgs e) {
            if (!disableHandlers && scriptEditorNumberFormatDecimal.Checked) {
                NumberStyles old = (NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference; //Local Backup
                Properties.Settings.Default.scriptEditorFormatPreference = (int)NumberStyles.Integer;
                

                if (!ReloadScript()) {
                    UpdateScriptNumberCheckBox(old); //Restore old checkbox status! Script couldn't be redrawn
                }
            }
        }
        private void UpdateScriptNumberFormatHex(object sender, EventArgs e) {
            if (!disableHandlers && scriptEditorNumberFormatHex.Checked) {
                NumberStyles old = (NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference; //Local Backup
                Properties.Settings.Default.scriptEditorFormatPreference = (int)NumberStyles.HexNumber;

                if (!ReloadScript()) {
                    UpdateScriptNumberCheckBox(old); //Restore old checkbox status! Script couldn't be redrawn
                }
            }
        }
        private void UpdateScriptNumberFormatNoPref(object sender, EventArgs e) {
            if (!disableHandlers && scriptEditorNumberFormatNoPreference.Checked) {
                NumberStyles old = (NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference; //Local Backup
                Properties.Settings.Default.scriptEditorFormatPreference = (int)NumberStyles.None;

                if (!ReloadScript()) {
                    UpdateScriptNumberCheckBox(old); //Restore old checkbox status! Script couldn't be redrawn
                }
            }
        }

        private void UpdateScriptNumberCheckBox(NumberStyles toSet) {
                
            disableHandlers = true;
            Properties.Settings.Default.scriptEditorFormatPreference = (int)toSet;

            switch ((NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference) {
                case NumberStyles.None:
                    scriptEditorNumberFormatNoPreference.Checked = true;
                    break;
                case NumberStyles.HexNumber:
                    scriptEditorNumberFormatHex.Checked = true;
                    break;
                case NumberStyles.Integer:
                    scriptEditorNumberFormatDecimal.Checked = true;
                    break;
            }
            Console.WriteLine("changed style to " + Properties.Settings.Default.scriptEditorFormatPreference);
            disableHandlers = false;
        }

        private void scriptsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {
            NavigatorGoTo((ListBox)sender, 0, scriptSearchManager, ScriptFile.containerTypes.Script.ToString());
        }

        private void functionsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {
            NavigatorGoTo((ListBox)sender, 1, functionSearchManager, ScriptFile.containerTypes.Function.ToString());
        }

        private void actionsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {
            NavigatorGoTo((ListBox)sender, 2, actionSearchManager, ScriptFile.containerTypes.Action.ToString());
        }

        private void NavigatorGoTo(ListBox currentLB, int indexToSwitchTo, SearchManager entrusted, string keyword) {
            if (currentLB.SelectedIndex < 0) {
                return;
            }
            
            if (scriptEditorTabControl.SelectedIndex != indexToSwitchTo) {
                scriptEditorTabControl.SelectedIndex = indexToSwitchTo;
            }

            entrusted.Find(true, false, keyword + ' ' + (currentLB.SelectedIndex + 1) + ':');
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
            new TextArchive(0, new List<string>() { "Your text here." }, discardLines: true).SaveToFileDefaultDir(selectTextFileComboBox.Items.Count);

            /* Update ComboBox and select new file */
            selectTextFileComboBox.Items.Add("Text Archive " + selectTextFileComboBox.Items.Count);
            selectTextFileComboBox.SelectedIndex = selectTextFileComboBox.Items.Count - 1;
        }
        private void addStringButton_Click(object sender, EventArgs e) {
            currentTextArchive.messages.Add("");
            textEditorDataGridView.Rows.Add("");

            int rowInd = textEditorDataGridView.RowCount - 1;

            disableHandlers = true;

            string format = "X";
            string prefix = "0x";
            if (decimalRadioButton.Checked) {
                format = "D";
                prefix = "";
            }

            textEditorDataGridView.Rows[rowInd].HeaderCell.Value = prefix + rowInd.ToString(format);
            disableHandlers = false;

        }
        private void exportTextFileButton_Click(object sender, EventArgs e) {
            currentTextArchive.SaveToFileExplorePath("Text Archive " + selectTextFileComboBox.SelectedIndex);
            if (selectTextFileComboBox.SelectedIndex == RomInfo.locationNamesTextNumber) {
                ReloadHeaderEditorLocationsList(currentTextArchive.messages);
            }
        }

        private void saveTextArchiveButton_Click(object sender, EventArgs e) {
            currentTextArchive.SaveToFileDefaultDir(selectTextFileComboBox.SelectedIndex);
            if (selectTextFileComboBox.SelectedIndex == RomInfo.locationNamesTextNumber) {
                ReloadHeaderEditorLocationsList(currentTextArchive.messages);
            }
        }
        private void selectedLineMoveUpButton_Click(object sender, EventArgs e) {
            int cc = textEditorDataGridView.CurrentCell.RowIndex;

            if (cc > 0) {
                DataGridViewRowCollection rows = textEditorDataGridView.Rows;
                DataGridViewCell current = rows[cc].Cells[0];
                DataGridViewCell previous = rows[cc-1].Cells[0];

                (current.Value, previous.Value) = (previous.Value, current.Value);
                textEditorDataGridView.CurrentCell = previous;
            }
        }

        private void selectedLineMoveDownButton_Click(object sender, EventArgs e) {
            int cc = textEditorDataGridView.CurrentCell.RowIndex;

            if (cc < textEditorDataGridView.RowCount - 1) {
                DataGridViewRowCollection rows = textEditorDataGridView.Rows;
                DataGridViewCell current = rows[cc].Cells[0];
                DataGridViewCell next = rows[cc + 1].Cells[0];

                (current.Value, next.Value) = (next.Value, current.Value);
                textEditorDataGridView.CurrentCell = next;
            }
        }
        private void ReloadHeaderEditorLocationsList(IEnumerable<string> contents) {
            int selection = locationNameComboBox.SelectedIndex;
            locationNameComboBox.Items.Clear();
            locationNameComboBox.Items.AddRange(contents.ToArray());
            locationNameComboBox.SelectedIndex = selection;
        }
        private void importTextFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .msg file */
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Text Archive (*.msg)|*.msg"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update Text Archive object in memory */
            string path = RomInfo.gameDirs[DirNames.textArchives].unpackedDir + "\\" + selectTextFileComboBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Refresh controls */
            selectTextFileComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show("Text Archive imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void removeMessageFileButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("Are you sure you want to delete the last Text Archive?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d.Equals(DialogResult.Yes)) {
                /* Delete Text Archive */
                File.Delete(RomInfo.gameDirs[DirNames.textArchives].unpackedDir + "\\" + (selectTextFileComboBox.Items.Count - 1).ToString("D4"));

                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = selectTextFileComboBox.Items.Count - 1;
                if (selectTextFileComboBox.SelectedIndex == lastIndex) {
                    selectTextFileComboBox.SelectedIndex--;
                }

                /* Remove item from ComboBox */
                selectTextFileComboBox.Items.RemoveAt(lastIndex);
            }
        }
        private void removeStringButton_Click(object sender, EventArgs e) {
            if (currentTextArchive.messages.Count > 0) {
                currentTextArchive.messages.RemoveAt(currentTextArchive.messages.Count - 1);
                textEditorDataGridView.Rows.RemoveAt(textEditorDataGridView.Rows.Count - 1);
            }
        }
        private void searchMessageButton_Click(object sender, EventArgs e) {
            if (searchMessageTextBox.Text == "") {
                return;
            }

            int firstArchiveNumber;
            int lastArchiveNumber;

            if (searchAllArchivesCheckBox.Checked) {
                firstArchiveNumber = 0;
                lastArchiveNumber = romInfo.GetTextArchivesCount();
            } else {
                firstArchiveNumber = selectTextFileComboBox.SelectedIndex;
                lastArchiveNumber = firstArchiveNumber + 1;
            }

            textSearchResultsListBox.Items.Clear();

            lastArchiveNumber = Math.Min(lastArchiveNumber, 828);

            textSearchProgressBar.Maximum = lastArchiveNumber;

            List<string> results = null;
            if (caseSensitiveTextSearchCheckbox.Checked) {
                results = searchTexts(firstArchiveNumber, lastArchiveNumber, (string x) => x.Contains(searchMessageTextBox.Text));
            } else {
                results = searchTexts(firstArchiveNumber, lastArchiveNumber, (string x) => x.IndexOf(searchMessageTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0);
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
            if (e.KeyCode == Keys.Enter) {
                searchMessageButton_Click(null, null);
            }
        }
        private void replaceMessageButton_Click(object sender, EventArgs e) {
            if (searchMessageTextBox.Text == "") {
                return;
            }

            int firstArchiveNumber;
            int lastArchiveNumber;

            string specify;
            if (searchAllArchivesCheckBox.Checked) {
                firstArchiveNumber = 0;
                lastArchiveNumber = romInfo.GetTextArchivesCount();
                specify = " in every Text Bank of the game (" + firstArchiveNumber + " to " + lastArchiveNumber + ")";
            } else {
                firstArchiveNumber = selectTextFileComboBox.SelectedIndex;
                lastArchiveNumber = firstArchiveNumber + 1;
                specify = " in the current text bank only (" + firstArchiveNumber + ")";
            }

            string message = "You are about to replace every occurrence of " + '"' + searchMessageTextBox.Text + '"'
                + " with " + '"' + replaceMessageTextBox.Text + '"' + specify +
                ".\nThe operation can't be interrupted nor undone.\n\nProceed?";
            DialogResult d = MessageBox.Show(message, "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                string searchString = searchMessageTextBox.Text;
                string replaceString = replaceMessageTextBox.Text;
                textSearchResultsListBox.Items.Clear();

                lastArchiveNumber = Math.Min(lastArchiveNumber, 828);
                textSearchProgressBar.Maximum = lastArchiveNumber;

                for (int cur = firstArchiveNumber; cur < lastArchiveNumber; cur++) { 
                    currentTextArchive = new TextArchive(cur);
                    bool found = false;

                    if (caseSensitiveTextReplaceCheckbox.Checked) {
                        for (int j = 0; j < currentTextArchive.messages.Count; j++) {
                            while (currentTextArchive.messages[j].IndexOf(searchString) >= 0) {
                                currentTextArchive.messages[j] = currentTextArchive.messages[j].Replace(searchString, replaceString);
                                found = true;
                            }
                        }
                    } else {
                        for (int j = 0; j < currentTextArchive.messages.Count; j++) {
                            int posFound;
                            while ( (posFound = currentTextArchive.messages[j].IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase)) >= 0) { 
                                currentTextArchive.messages[j] = currentTextArchive.messages[j].Substring(0, posFound) + replaceString + currentTextArchive.messages[j].Substring(posFound + searchString.Length);
                                found = true;
                            }
                        }
                    }

                    textSearchProgressBar.Value = cur;
                    if (found) {
                        disableHandlers = true;

                        textSearchResultsListBox.Items.Add("Text archive (" + cur + ") - Succesfully edited");
                        currentTextArchive.SaveToFileDefaultDir(cur, showSuccessMessage: false);

                        if (cur == lastArchiveNumber) {
                            UpdateTextEditorFileView(false);
                        }

                        disableHandlers = false;
                    }
                    //else searchMessageResultTextBox.AppendText(searchString + " not found in this file");
                    //this.saveMessageFileButton_Click(sender, e);
                }
                MessageBox.Show("Operation completed.", "Replace All Text", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateTextEditorFileView(readAgain: true);
                textSearchProgressBar.Value = 0;
            }
        }
        private void selectTextFileComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateTextEditorFileView(true);
        }
        private void UpdateTextEditorFileView(bool readAgain) {
            disableHandlers = true;

            textEditorDataGridView.Rows.Clear();
            if (currentTextArchive is null || readAgain) {
                currentTextArchive = new TextArchive(selectTextFileComboBox.SelectedIndex);
            }

            foreach (string msg in currentTextArchive.messages) {
                textEditorDataGridView.Rows.Add(msg);
            }

            if (hexRadiobutton.Checked) {
                PrintTextEditorLinesHex();
            } else {
                PrintTextEditorLinesDecimal();
            }

            disableHandlers = false;

            textEditorDataGridView_CurrentCellChanged(textEditorDataGridView, null);
        }
        private void PrintTextEditorLinesHex() {
            int final = Math.Min(textEditorDataGridView.Rows.Count, currentTextArchive.messages.Count);

            for (int i = 0; i < final; i++) {
                textEditorDataGridView.Rows[i].HeaderCell.Value = "0x" + i.ToString("X");
            }
        }
        private void PrintTextEditorLinesDecimal() {
            int final = Math.Min(textEditorDataGridView.Rows.Count, currentTextArchive.messages.Count);

            for (int i = 0; i < final; i++) {
                textEditorDataGridView.Rows[i].HeaderCell.Value = i.ToString();
            }
        }
        private void textEditorDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (disableHandlers) {
                return;
            }
            if (e.RowIndex > -1 && e.ColumnIndex > -1) {
                try {
                    currentTextArchive.messages[e.RowIndex] = textEditorDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                } catch (NullReferenceException) {
                    currentTextArchive.messages[e.RowIndex] = "";
                }
            }
        }
        private void textEditorDataGridView_CurrentCellChanged(object sender, EventArgs e) {
            DataGridView dgv = sender as DataGridView;
            if (disableHandlers || dgv == null || dgv.CurrentCell == null) {
                return;
            }

            Console.WriteLine("R: " + dgv.CurrentCell.RowIndex);
            Console.WriteLine("Last index: " + (dgv.RowCount - 1).ToString());

            if (dgv.CurrentCell.RowIndex > 0) {
                selectedLineMoveUpButton.Enabled = true;
            } else {
                selectedLineMoveUpButton.Enabled = false;
            }

            if (dgv.CurrentCell.RowIndex < dgv.RowCount - 1) {
                selectedLineMoveDownButton.Enabled = true;
            } else {
                selectedLineMoveDownButton.Enabled = false;
            }
        }
        private void textSearchResultsListBox_GoToEntryResult(object sender, MouseEventArgs e) {
            if (textSearchResultsListBox.SelectedIndex < 0) {
                return;
            }

            string[] msgResult = textSearchResultsListBox.Text.Split(new string[] { " --- " }, StringSplitOptions.RemoveEmptyEntries);
            string[] parts = msgResult[0].Substring(1).Split(new string[] { ") - #" }, StringSplitOptions.RemoveEmptyEntries);
            
            if (int.TryParse(parts[0], out int msg)) {
                if (int.TryParse(parts[1], out int line)) {
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
            Properties.Settings.Default.textEditorPreferHex = hexRadiobutton.Checked;
        }
        private void updateTextEditorLineNumbers() {
            disableHandlers = true;
            if (hexRadiobutton.Checked) {
                PrintTextEditorLinesHex();
            } else {
                PrintTextEditorLinesDecimal();
            }
            disableHandlers = false;
        }
        #endregion

        #region NSBTX Editor
        public NSMBe4.NSBMD.NSBTX_File currentTileset;
        public AreaData currentAreaData;

        public void FillTilesetBox() {
            texturePacksListBox.Items.Clear();

            int tilesetFileCount;
            if (mapTilesetRadioButton.Checked) {
                tilesetFileCount = romInfo.GetMapTexturesCount();
            } else {
                tilesetFileCount = romInfo.GetBuildingTexturesCount();
            }

            for (int i = 0; i < tilesetFileCount; i++) {
                texturePacksListBox.Items.Add("Texture Pack " + i);
            }
        }
        private void SetupNSBTXEditor() {
            statusLabel.Text = "Attempting to unpack Tileset Editor NARCs... Please wait.";
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> {
                DirNames.buildingTextures,
                DirNames.mapTextures,
                DirNames.buildingConfigFiles,
                DirNames.areaData
            });

            /* Fill Tileset ListBox */
            FillTilesetBox();

            /* Fill AreaData ComboBox */
            selectAreaDataListBox.Items.Clear();
            int areaDataCount = romInfo.GetAreaDataCount();
            for (int i = 0; i < areaDataCount; i++) {
                selectAreaDataListBox.Items.Add("AreaData File " + i);
            }

            /* Enable gameVersion-specific controls */
            string[] lightTypes;

            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                case gFamEnum.Plat:
                    lightTypes = new string[3] { "Day/Night Light", "Model's light", "Unknown Light" };
                    break;
                default:
                    lightTypes = new string[3] { "Model's light", "Day/Night Light", "Unknown Light" };
                    areaDataDynamicTexturesNumericUpDown.Enabled = true;
                    areaTypeGroupbox.Enabled = true;
                    break;
            };

            areaDataLightTypeComboBox.Items.Clear();
            areaDataLightTypeComboBox.Items.AddRange(lightTypes);

            if (selectAreaDataListBox.Items.Count > 0) {
                selectAreaDataListBox.SelectedIndex = 0;
            }

            if (texturePacksListBox.Items.Count > 0) {
                texturePacksListBox.SelectedIndex = 0;
            }

            if (texturesListBox.Items.Count > 0) {
                texturesListBox.SelectedIndex = 0;
            }

            if (palettesListBox.Items.Count > 0) {
                palettesListBox.SelectedIndex = 0;
            }
            statusLabel.Text = "Ready";
        }
        private void buildingsTilesetRadioButton_CheckedChanged(object sender, EventArgs e) {
            FillTilesetBox();
            texturePacksListBox.SelectedIndex = (int)areaDataBuildingTilesetUpDown.Value;
            if (texturesListBox.Items.Count > 0) {
                texturesListBox.SelectedIndex = 0;
            }
            if (palettesListBox.Items.Count > 0) {
                palettesListBox.SelectedIndex = 0;
            }
        }
        private void exportNSBTXButton_Click(object sender, EventArgs e) {
            SaveFileDialog sf = new SaveFileDialog {
                Filter = "NSBTX File (*.nsbtx)|*.nsbtx",
                FileName = "Texture Pack " + texturePacksListBox.SelectedIndex
            };
            if (sf.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            string tilesetPath = mapTilesetRadioButton.Checked
                ? RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4")
                : RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            File.Copy(tilesetPath, sf.FileName);

            MessageBox.Show("NSBTX tileset exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importNSBTXButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .nsbtx file */
            OpenFileDialog ofd = new OpenFileDialog {
                Filter = "NSBTX File (*.nsbtx)|*.nsbtx"
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update nsbtx file */
            string tilesetPath = mapTilesetRadioButton.Checked
                ? RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4")
                : RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");
            File.Copy(ofd.FileName, tilesetPath, true);

            /* Update nsbtx object in memory and controls */
            currentTileset = new NSMBe4.NSBMD.NSBTX_File(new FileStream(ofd.FileName, FileMode.Open));
            texturePacksListBox_SelectedIndexChanged(null, null);
            MessageBox.Show("NSBTX tileset imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void mapTilesetRadioButton_CheckedChanged(object sender, EventArgs e) {
            FillTilesetBox();

            try {
                if (mapTilesetRadioButton.Checked) {
                    texturePacksListBox.SelectedIndex = (int)areaDataMapTilesetUpDown.Value;
                } else if (buildingsTilesetRadioButton.Checked) {
                    texturePacksListBox.SelectedIndex = (int)areaDataBuildingTilesetUpDown.Value;
                }
            } catch (ArgumentOutOfRangeException) {
                texturePacksListBox.SelectedIndex = 0;
            }
        }
        private void palettesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            try {
                texturePictureBox.Image = LoadTextureFromNSBTX(currentTileset, texturesListBox.SelectedIndex, palettesListBox.SelectedIndex);
            } catch { }
        }
        private void texturePacksListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            disableHandlers = true;

            /* Clear ListBoxes */
            texturesListBox.Items.Clear();
            palettesListBox.Items.Clear();

            /* Load tileset file */
            string tilesetPath = mapTilesetRadioButton.Checked
                ? RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4")
                : RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + texturePacksListBox.SelectedIndex.ToString("D4");

            currentTileset = new NSMBe4.NSBMD.NSBTX_File(new FileStream(tilesetPath, FileMode.Open));
            string currentItemName = texturePacksListBox.Items[texturePacksListBox.SelectedIndex].ToString();

            if (currentTileset.TexInfo.names is null || currentTileset.PalInfo.names is null) {
                if (!currentItemName.StartsWith("Error!")) {
                    texturePacksListBox.Items[texturePacksListBox.SelectedIndex] = "Error! - " + currentItemName;
                }

                disableHandlers = false;
                return;
            }
            /* Add textures and palette slot names to ListBoxes */
            texturesListBox.Items.AddRange(currentTileset.TexInfo.names.ToArray());
            palettesListBox.Items.AddRange(currentTileset.PalInfo.names.ToArray());

            disableHandlers = false;

            if (texturesListBox.Items.Count > 0) {
                texturesListBox.SelectedIndex = 0;
            }
        }
        private void texturesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

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
            if (disableHandlers) {
                return;
            }
            currentAreaData.buildingsTileset = (ushort)areaDataBuildingTilesetUpDown.Value;
        }
        private void areaDataDynamicTexturesUpDown_ValueChanged(object sender, EventArgs e) {
            if (areaDataDynamicTexturesNumericUpDown.Value == areaDataDynamicTexturesNumericUpDown.Maximum) {
                areaDataDynamicTexturesNumericUpDown.ForeColor = Color.Red;
            } else {
                areaDataDynamicTexturesNumericUpDown.ForeColor = Color.Black;
            }

            if (disableHandlers) {
                return;
            }
            currentAreaData.dynamicTextureType = (ushort)areaDataDynamicTexturesNumericUpDown.Value;
        }
        private void areaDataLightTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentAreaData.lightType = (byte)areaDataLightTypeComboBox.SelectedIndex;
        }
        private void areaDataMapTilesetUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentAreaData.mapTileset = (ushort)areaDataMapTilesetUpDown.Value;
        }
        private void saveAreaDataButton_Click(object sender, EventArgs e) {
            currentAreaData.SaveToFileDefaultDir(selectAreaDataListBox.SelectedIndex);
        }
        private void selectAreaDataListBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentAreaData = new AreaData((byte)selectAreaDataListBox.SelectedIndex);

            areaDataBuildingTilesetUpDown.Value = currentAreaData.buildingsTileset;
            areaDataMapTilesetUpDown.Value = currentAreaData.mapTileset;
            areaDataLightTypeComboBox.SelectedIndex = currentAreaData.lightType;

            disableHandlers = true;
            if (RomInfo.gameFamily == gFamEnum.HGSS) {
                areaDataDynamicTexturesNumericUpDown.Value = currentAreaData.dynamicTextureType;

                bool interior = currentAreaData.areaType == 0;
                indoorAreaRadioButton.Checked = interior;
                outdoorAreaRadioButton.Checked = !interior;
            }
            disableHandlers = false;
        }
        private void indoorAreaRadioButton_CheckedChanged(object sender, EventArgs e) {
            currentAreaData.areaType = indoorAreaRadioButton.Checked ? AreaData.TYPE_INDOOR : AreaData.TYPE_OUTDOOR;
        }
        private void addNSBTXButton_Click(object sender, EventArgs e) {
            /* Add new NSBTX file to the correct folder */
            if (mapTilesetRadioButton.Checked) {
                File.Copy(RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + 0.ToString("D4"), RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + texturePacksListBox.Items.Count.ToString("D4"));
                
                if (mapEditorIsReady) {
                    mapTextureComboBox.Items.Add("Map Texture Pack [" + mapTextureComboBox.Items.Count.ToString("D2") + "]");
                }
            } else {
                File.Copy(RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + 0.ToString("D4"), RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + texturePacksListBox.Items.Count.ToString("D4"));
                File.Copy(RomInfo.gameDirs[DirNames.buildingConfigFiles].unpackedDir + "\\" + 0.ToString("D4"), RomInfo.gameDirs[DirNames.buildingConfigFiles].unpackedDir + "\\" + texturePacksListBox.Items.Count.ToString("D4"));
               
                if (mapEditorIsReady) {
                    buildTextureComboBox.Items.Add("Building Texture Pack [" + buildTextureComboBox.Items.Count.ToString("D2") + "]");
                }
            }

            /* Update ComboBox and select new file */
            texturePacksListBox.Items.Add("Texture Pack " + texturePacksListBox.Items.Count);
            texturePacksListBox.SelectedIndex = texturePacksListBox.Items.Count - 1;
        }
        private void removeNSBTXButton_Click(object sender, EventArgs e) {
            if (texturePacksListBox.Items.Count > 1) {
                /* Delete NSBTX file */
                DialogResult d = MessageBox.Show("Are you sure you want to delete the last Texture Pack?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (d.Equals(DialogResult.Yes)) {
                    if (mapTilesetRadioButton.Checked) {
                        File.Delete(RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (texturePacksListBox.Items.Count - 1).ToString("D4"));

                        if (mapEditorIsReady) {
                            mapTextureComboBox.Items.RemoveAt(mapTextureComboBox.Items.Count - 1);
                        }
                    } else {
                        File.Delete(RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + (texturePacksListBox.Items.Count - 1).ToString("D4"));
                        File.Delete(RomInfo.gameDirs[DirNames.buildingConfigFiles].unpackedDir + "\\" + (texturePacksListBox.Items.Count - 1).ToString("D4"));

                        if (mapEditorIsReady) {
                            buildTextureComboBox.Items.RemoveAt(buildTextureComboBox.Items.Count - 1);
                        }
                    }

                    /* Check if currently selected file is the last one, and in that case select the one before it */
                    int lastIndex = texturePacksListBox.Items.Count - 1;
                    if (texturePacksListBox.SelectedIndex == lastIndex) {
                        texturePacksListBox.SelectedIndex--;
                    }

                    /* Remove item from ComboBox */
                    texturePacksListBox.Items.RemoveAt(lastIndex);
                }
            } else {
                MessageBox.Show("At least one tileset must be kept.", "Can't delete tileset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void addAreaDataButton_Click(object sender, EventArgs e) {
            /* Add new NSBTX file to the correct folder */
            string areaDataDirPath = RomInfo.gameDirs[DirNames.areaData].unpackedDir;
            File.Copy(areaDataDirPath + "\\" + 0.ToString("D4"), areaDataDirPath + "\\" + selectAreaDataListBox.Items.Count.ToString("D4"));

            /* Update ComboBox and select new file */
            selectAreaDataListBox.Items.Add("AreaData File " + selectAreaDataListBox.Items.Count);
            selectAreaDataListBox.SelectedIndex = selectAreaDataListBox.Items.Count - 1;
        }
        private void removeAreaDataButton_Click(object sender, EventArgs e) {
            if (selectAreaDataListBox.Items.Count > 1) {
                /* Delete AreaData file */
                File.Delete(RomInfo.gameDirs[DirNames.areaData].unpackedDir + "\\" + (selectAreaDataListBox.Items.Count - 1).ToString("D4"));

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
            currentAreaData.SaveToFileExplorePath("Area Data " + selectAreaDataListBox.SelectedIndex);
        }
        private void importAreaDataButton_Click(object sender, EventArgs e) {
            if (selectAreaDataListBox.SelectedIndex < 0) {
                return;
            }

            OpenFileDialog of = new OpenFileDialog {
                Filter = "AreaData File (*.bin)|*.bin"
            };
            
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update areadata object in memory */
            string path = RomInfo.gameDirs[DirNames.areaData].unpackedDir + "\\" + selectAreaDataListBox.SelectedIndex.ToString("D4");
            File.Copy(of.FileName, path, true);

            /* Refresh controls */
            selectAreaDataListBox_SelectedIndexChanged(sender, e);

            /* Display success message */
            MessageBox.Show("AreaData File imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Camera Editor
        GameCamera[] currentCameraTable;
        uint overlayCameraTblOffset;

        private void SetupCameraEditor() {
            RomInfo.PrepareCameraData();
            cameraEditorDataGridView.Rows.Clear();

            if (DSUtils.CheckOverlayHasCompressionFlag(RomInfo.cameraTblOverlayNumber)) {
                DialogResult d1 = MessageBox.Show("It is STRONGLY recommended to configure Overlay1 as uncompressed before proceeding.\n\n" +
                        "More details in the following dialog.\n\n" + "Do you want to know more?",
                        "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                bool userConfirmed = (d1 == DialogResult.Yes ? ROMToolboxDialog.ConfigureOverlay1Uncompressed() : false);
                

                if (!userConfirmed) {
                    MessageBox.Show("You chose not to apply the patch. Use this editor responsibly.\n\n" +
                            "If you change your mind, you can apply it later by accessing the ROM Toolbox.",
                            "Caution", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (DSUtils.OverlayIsCompressed(RomInfo.cameraTblOverlayNumber)) {
                        DSUtils.DecompressOverlay(RomInfo.cameraTblOverlayNumber);
                    }
                }
            }


            uint[] RAMaddresses = new uint[RomInfo.cameraTblOffsetsToRAMaddress.Length];
            string camOverlayPath = DSUtils.GetOverlayPath(RomInfo.cameraTblOverlayNumber);
            using (DSUtils.EasyReader br = new DSUtils.EasyReader(camOverlayPath)) {
                for (int i = 0; i < RomInfo.cameraTblOffsetsToRAMaddress.Length; i++) {
                    br.BaseStream.Position = RomInfo.cameraTblOffsetsToRAMaddress[i];
                    RAMaddresses[i] = br.ReadUInt32();
                }
            }

            uint referenceAddress = RAMaddresses[0];
            for (int i = 1; i < RAMaddresses.Length; i++) {
                uint ramAddress = RAMaddresses[i];
                if (ramAddress != referenceAddress) {
                    MessageBox.Show("Value of RAM Pointer to the overlay table is different between Offset #1 and Offset #" + (i + 1) + Environment.NewLine +
                        "The camera values might be wrong.", "Possible errors ahead", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            using (DSUtils.EasyReader br = new DSUtils.EasyReader(camOverlayPath, RAMaddresses[0] - DSUtils.GetOverlayRAMAddress(RomInfo.cameraTblOverlayNumber))) {
                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    currentCameraTable = new GameCamera[17];
                    for (int i = 0; i < currentCameraTable.Length; i++) {
                        currentCameraTable[i] = new GameCamera(br.ReadUInt32(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(),
                                                br.ReadInt16(), br.ReadByte(), br.ReadByte(),
                                                br.ReadUInt16(), br.ReadUInt32(), br.ReadUInt32(),
                                                br.ReadInt32(), br.ReadInt32(), br.ReadInt32());

                    }
                } else {
                    currentCameraTable = new GameCamera[16];
                    for (int i = 0; i < 3; i++) {
                        cameraEditorDataGridView.Columns.RemoveAt(cameraEditorDataGridView.Columns.Count - 3);
                    }
                    for (int i = 0; i < currentCameraTable.Length; i++) {
                        currentCameraTable[i] = new GameCamera(br.ReadUInt32(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16(),
                                                br.ReadInt16(), br.ReadByte(), br.ReadByte(),
                                                br.ReadUInt16(), br.ReadUInt32(), br.ReadUInt32());
                    }
                }

                cameraEditorDataGridView.RowTemplate.Height = 32 * 16 / currentCameraTable.Length;
                for (int i = 0; i < currentCameraTable.Length; i++) {
                    currentCameraTable[i].ShowInGridView(cameraEditorDataGridView, i);
                }
            }
        }
        private void saveCameraTableButton_Click(object sender, EventArgs e) {
            SaveCameraTable(DSUtils.GetOverlayPath(RomInfo.cameraTblOverlayNumber), overlayCameraTblOffset);
        }
        private void cameraEditorDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e) {
            //cameraEditorDataGridView.Columns[0].ValueType = typeof(int);
            currentCameraTable[e.RowIndex][e.ColumnIndex] = cameraEditorDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            cameraEditorDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = currentCameraTable[e.RowIndex][e.ColumnIndex];
        }
        private void exportCameraTableButton_Click(object sender, EventArgs e) {
            SaveFileDialog of = new SaveFileDialog {
                Filter = "Camera Table File (*.bin)|*.bin",
                FileName = Path.GetFileNameWithoutExtension(RomInfo.fileName) + " - CameraTable.bin"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            File.Delete(of.FileName);
            SaveCameraTable(of.FileName, 0);
        }
        private void SaveCameraTable(string path, uint destFileOffset) {
            for (int i = 0; i < currentCameraTable.Length; i++) {
                DSUtils.WriteToFile(path, currentCameraTable[i].ToByteArray(), (uint)(destFileOffset + i * RomInfo.cameraSize));
            }
            MessageBox.Show("Camera table correctly saved.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void cameraEditorDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            var senderTable = (DataGridView)sender;

            if (senderTable.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0) {
                string type = "Camera File";
                if (e.ColumnIndex == cameraEditorDataGridView.Columns.Count - 2) { //Export
                    SaveFileDialog sf = new SaveFileDialog {
                        Filter = type + " (*.bin)|*.bin",
                        FileName = Path.GetFileNameWithoutExtension(RomInfo.fileName) + " - Camera " + e.RowIndex + ".bin"
                    };

                    if (sf.ShowDialog(this) != DialogResult.OK) {
                        return;
                    }

                    DSUtils.WriteToFile(sf.FileName, currentCameraTable[e.RowIndex].ToByteArray(), fmode: FileMode.Create);
                    MessageBox.Show("Camera correctly saved.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else if (e.ColumnIndex == cameraEditorDataGridView.Columns.Count - 1) { //Import
                    OpenFileDialog of = new OpenFileDialog {
                        Filter = type + " (*.bin)|*.bin",
                    };

                    if (of.ShowDialog(this) != DialogResult.OK) {
                        return;
                    }

                    currentCameraTable[e.RowIndex] = new GameCamera(File.ReadAllBytes(of.FileName));
                    currentCameraTable[e.RowIndex].ShowInGridView(senderTable, e.RowIndex);
                    MessageBox.Show("Camera correctly imported.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void importCameraTableButton_Click(object sender, EventArgs e) {
            string fileType = "Camera Table File";
            OpenFileDialog of = new OpenFileDialog {
                Filter = fileType + " (*.bin)|*.bin",
            };

            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            long l = new FileInfo(of.FileName).Length;
            if (l % RomInfo.cameraSize != 0) {
                MessageBox.Show("This is not a " + RomInfo.gameFamily + ' ' + fileType +
                    "\nMake sure the file length is a multiple of " + RomInfo.cameraSize + " and try again.", "Wrong file!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte nCameras = (byte)(l / RomInfo.cameraSize);
            for (byte b = 0; b < nCameras; b++) {
                currentCameraTable[b] = new GameCamera(DSUtils.ReadFromFile(of.FileName, b * RomInfo.cameraSize, RomInfo.cameraSize));
                currentCameraTable[b].ShowInGridView(cameraEditorDataGridView, b);
            }
            MessageBox.Show("Camera Table imported correctly.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Trainer Editor
        private List<ComboBox> partyPokemonComboboxList = new List<ComboBox>();
        private List<ComboBox> partyItemsComboboxList = new List<ComboBox>();
        private List<GroupBox> partyMovesGroupboxList = new List<GroupBox>();
        private List<NumericUpDown> partyLevelUpdownList = new List<NumericUpDown>();
        private List<NumericUpDown> partyIVUpdownList = new List<NumericUpDown>();
        private List<NumericUpDown> partyBallUpdownList = new List<NumericUpDown>();
        private List<GroupBox> partyGroupboxList = new List<GroupBox>();
        private List<PictureBox> partyPokemonPictureBoxList = new List<PictureBox>();
        private List<PictureBox> partyPokemonItemIconList = new List<PictureBox>();

        TrainerFile currentTrainerFile;
        PaletteBase pal;
        ImageBase tiles;
        SpriteBase sprite;

        Dictionary<byte, (uint entryOffset, ushort musicD, ushort? musicN)> trainerClassEncounterMusicDict;
        private void SetupTrainerClassEncounterMusicTable() {
            RomInfo.SetEncounterMusicTableOffsetToRAMAddress();
            trainerClassEncounterMusicDict = new Dictionary<byte, (uint entryOffset, ushort musicD, ushort? musicN)>();

            uint encounterMusicTableTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.encounterMusicTableOffsetToRAMAddress, 4), 0) - 0x02000000;
            
            uint entrySize = 4;
            uint tableSizeOffset = 10;
            if (gameFamily == gFamEnum.HGSS) {
                entrySize += 2;
                tableSizeOffset += 2;
                encounterSSEQAltUpDown.Enabled = true;
            }

            byte tableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.encounterMusicTableOffsetToRAMAddress - tableSizeOffset);
            using (DSUtils.ARM9.Reader ar = new DSUtils.ARM9.Reader(encounterMusicTableTableStartAddress) ) {
                for (int i = 0; i < tableEntriesCount; i++) {
                    uint entryOffset = (uint)ar.BaseStream.Position;
                    byte tclass = (byte)ar.ReadUInt16();
                    ushort musicD = ar.ReadUInt16();
                    ushort? musicN = gameFamily == gFamEnum.HGSS ? ar.ReadUInt16() : (ushort?)null;
                    trainerClassEncounterMusicDict[tclass] = (entryOffset, musicD, musicN);
                }
            }
        }
        private void SetupTrainerEditor() {
            disableHandlers = true;
            SetupTrainerClassEncounterMusicTable();
            /* Extract essential NARCs sub-archives*/
            statusLabel.Text = "Setting up Trainer Editor...";
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { 
                DirNames.trainerProperties, 
                DirNames.trainerParty, 
                DirNames.trainerGraphics, 
                DirNames.textArchives 
            });

            partyPokemonComboboxList.Clear();
            partyPokemonComboboxList.Add(partyPokemon1ComboBox);
            partyPokemonComboboxList.Add(partyPokemon2ComboBox);
            partyPokemonComboboxList.Add(partyPokemon3ComboBox);
            partyPokemonComboboxList.Add(partyPokemon4ComboBox);
            partyPokemonComboboxList.Add(partyPokemon5ComboBox);
            partyPokemonComboboxList.Add(partyPokemon6ComboBox);

            partyItemsComboboxList.Clear();
            partyItemsComboboxList.Add(partyItem1ComboBox);
            partyItemsComboboxList.Add(partyItem2ComboBox);
            partyItemsComboboxList.Add(partyItem3ComboBox);
            partyItemsComboboxList.Add(partyItem4ComboBox);
            partyItemsComboboxList.Add(partyItem5ComboBox);
            partyItemsComboboxList.Add(partyItem6ComboBox);

            partyLevelUpdownList.Clear();
            partyLevelUpdownList.Add(partyLevel1UpDown);
            partyLevelUpdownList.Add(partyLevel2UpDown);
            partyLevelUpdownList.Add(partyLevel3UpDown);
            partyLevelUpdownList.Add(partyLevel4UpDown);
            partyLevelUpdownList.Add(partyLevel5UpDown);
            partyLevelUpdownList.Add(partyLevel6UpDown);

            partyIVUpdownList.Clear();
            partyIVUpdownList.Add(partyIV1UpDown);
            partyIVUpdownList.Add(partyIV2UpDown);
            partyIVUpdownList.Add(partyIV3UpDown);
            partyIVUpdownList.Add(partyIV4UpDown);
            partyIVUpdownList.Add(partyIV5UpDown);
            partyIVUpdownList.Add(partyIV6UpDown);

            partyBallUpdownList.Clear();
            partyBallUpdownList.Add(partyBall1UpDown);
            partyBallUpdownList.Add(partyBall2UpDown);
            partyBallUpdownList.Add(partyBall3UpDown);
            partyBallUpdownList.Add(partyBall4UpDown);
            partyBallUpdownList.Add(partyBall5UpDown);
            partyBallUpdownList.Add(partyBall6UpDown);

            partyMovesGroupboxList.Clear();
            partyMovesGroupboxList.Add(poke1MovesGroupBox);
            partyMovesGroupboxList.Add(poke2MovesGroupBox);
            partyMovesGroupboxList.Add(poke3MovesGroupBox);
            partyMovesGroupboxList.Add(poke4MovesGroupBox);
            partyMovesGroupboxList.Add(poke5MovesGroupBox);
            partyMovesGroupboxList.Add(poke6MovesGroupBox);

            partyGroupboxList.Clear();
            partyGroupboxList.Add(party1GroupBox);
            partyGroupboxList.Add(party2GroupBox);
            partyGroupboxList.Add(party3GroupBox);
            partyGroupboxList.Add(party4GroupBox);
            partyGroupboxList.Add(party5GroupBox);
            partyGroupboxList.Add(party6GroupBox);

            partyPokemonPictureBoxList.Clear();
            partyPokemonPictureBoxList.Add(partyPokemon1PictureBox);
            partyPokemonPictureBoxList.Add(partyPokemon2PictureBox);
            partyPokemonPictureBoxList.Add(partyPokemon3PictureBox);
            partyPokemonPictureBoxList.Add(partyPokemon4PictureBox);
            partyPokemonPictureBoxList.Add(partyPokemon5PictureBox);
            partyPokemonPictureBoxList.Add(partyPokemon6PictureBox);

            partyPokemonItemIconList.Clear();
            partyPokemonItemIconList.Add(partyPokemonItemPictureBox1);
            partyPokemonItemIconList.Add(partyPokemonItemPictureBox2);
            partyPokemonItemIconList.Add(partyPokemonItemPictureBox3);
            partyPokemonItemIconList.Add(partyPokemonItemPictureBox4);
            partyPokemonItemIconList.Add(partyPokemonItemPictureBox5);
            partyPokemonItemIconList.Add(partyPokemonItemPictureBox6);

            int trainerCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir).Length;
            trainerComboBox.Items.Clear();
            trainerComboBox.Items.AddRange(GetTrainerNames());

            string[] classNames = RomInfo.GetTrainerClassNames();
            trainerClassListBox.Items.Clear();
            if (classNames.Length > byte.MaxValue + 1) {
                MessageBox.Show("There can't be more than 256 trainer classes! [Found " + classNames.Length + "].\nAborting.",
                    "Too many trainer classes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            for (int i = 0; i < classNames.Length; i++) {
                trainerClassListBox.Items.Add("[" + i.ToString("D3") + "]" + " " + classNames[i]);
            }

            string[] itemNames = RomInfo.GetItemNames();
            string[] pokeNames = RomInfo.GetPokémonNames();
            string[] moveNames = RomInfo.GetAttackNames();

            foreach(Control c in trainerItemsGroupBox.Controls) {
                ComboBox cbox = c as ComboBox;
                cbox.Items.Clear();
                cbox.Items.AddRange(itemNames);
            }

            foreach (ComboBox CB in partyPokemonComboboxList) {
                CB.Items.Clear();
                CB.Items.AddRange(pokeNames);
            }

            foreach (ComboBox CB in partyItemsComboboxList) {
                CB.Items.Clear();
                CB.Items.AddRange(itemNames);
            }

            foreach (GroupBox movesGroup in partyMovesGroupboxList) {
                foreach (ComboBox CB in movesGroup.Controls) {
                    CB.Items.Clear();
                    CB.Items.AddRange(moveNames);
                }
            }

            trainerComboBox.SelectedIndex = 0;

            disableHandlers = false;
            trainerComboBox_SelectedIndexChanged(null, null);
            statusLabel.Text = "Ready";
        }
        private void trainerComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            disableHandlers = true;

            int currentIndex = trainerComboBox.SelectedIndex;
            string suffix = "\\" + currentIndex.ToString("D4");
            string[] trNames = RomInfo.GetSimpleTrainerNames();

            bool error = currentIndex >= trNames.Length;

            currentTrainerFile = new TrainerFile(
                new TrainerProperties(
                    (ushort)trainerComboBox.SelectedIndex, 
                    new FileStream(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir + suffix, FileMode.Open)
                ),
                new FileStream(RomInfo.gameDirs[DirNames.trainerParty].unpackedDir + suffix, FileMode.Open),
                error ? TrainerFile.NAME_NOT_FOUND : trNames[currentIndex]
            );

            RefreshTrainerPartyGUI();
            RefreshTrainerPropertiesGUI();

            disableHandlers = false;

            if (error) {
                MessageBox.Show("This Trainer File doesn't have a corresponding name.\n\n" +
                    "If you edited this ROM's Trainers with another tool before, don't worry.\n" +
                    "DSPRE will attempt to add the missing line to the Trainer Names Text Archive [" + RomInfo.trainerNamesMessageNumber + "] upon resaving.",
                    "Trainer name not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void RefreshTrainerPropertiesGUI() {
            trainerNameTextBox.Text = currentTrainerFile.name;

            trainerClassListBox.SelectedIndex = currentTrainerFile.trp.trainerClass;
            trainerDoubleCheckBox.Checked = currentTrainerFile.trp.doubleBattle;
            trainerMovesCheckBox.Checked = currentTrainerFile.trp.hasMoves;
            trainerItemsCheckBox.Checked = currentTrainerFile.trp.hasItems;
            partyCountUpDown.Value = currentTrainerFile.trp.partyCount;

            IList trainerItems = trainerItemsGroupBox.Controls;
            for (int i = 0; i < trainerItems.Count; i++) {
                (trainerItems[i] as ComboBox).SelectedIndex = currentTrainerFile.trp.trainerItems[i];
            }

            IList trainerAI = TrainerAIGroupBox.Controls;
            for (int i = 0; i < trainerAI.Count; i++) {
                (trainerAI[i] as CheckBox).Checked = currentTrainerFile.trp.AI[i];
            }
        }
        public void RefreshTrainerPartyGUI() {
            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
                partyPokemonComboboxList[i].SelectedIndex = currentTrainerFile.party[i].pokeID ?? 0;
                partyItemsComboboxList[i].SelectedIndex = currentTrainerFile.party[i].heldItem ?? 0;
                partyLevelUpdownList[i].Value = Math.Max((ushort)1, currentTrainerFile.party[i].level);
                partyIVUpdownList[i].Value = currentTrainerFile.party[i].unknown1_DATASTART;
                partyBallUpdownList[i].Value = currentTrainerFile.party[i].unknown2_DATAEND;

                if (currentTrainerFile.party[i].moves == null) {
                    for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
                        (partyMovesGroupboxList[i].Controls[j] as ComboBox).SelectedIndex = 0;
                    }
                } else {
                    for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
                        (partyMovesGroupboxList[i].Controls[j] as ComboBox).SelectedIndex = currentTrainerFile.party[i].moves[j];
                    }
                }
            }
        }
        private string FixPokenameString(string toFix) {
            toFix = toFix.ToLower();
            toFix = toFix.Replace(".", "_"); //Fix Mime Jr. and Mr. Mime
            toFix = toFix.Replace(" ", "_");
            toFix = toFix.Replace("'", "_");
            toFix = toFix.Replace("’", "_");
            toFix = toFix.Replace(":", "_");
            toFix = toFix.Replace("-", "_");
            toFix = toFix.Replace("_D", "_d"); //Fix Farfetch'd

            toFix = toFix.Replace("♀", "F");
            toFix = toFix.Replace("♂", "M");
            return toFix;
        }
        private void showTrainerEditorPokePic(byte partyPos) {
            ComboBox cb = partyPokemonComboboxList[partyPos];
            partyPokemonPictureBoxList[partyPos].Image = cb.SelectedIndex > 0 ? (Image)Properties.PokePics.ResourceManager.GetObject(FixPokenameString(PokeDatabase.System.pokeNames[(ushort)cb.SelectedIndex])) : global::DSPRE.Properties.Resources.IconPokeball;
        }
        private void partyPokemon1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorPokePic(0);
        }
        private void partyPokemon2ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorPokePic(1);
        }

        private void partyPokemon3ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorPokePic(2);
        }

        private void partyPokemon4ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorPokePic(3);
        }

        private void partyPokemon5ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorPokePic(4);
        }

        private void partyPokemon6ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorPokePic(5);
        }

        private void showTrainerEditorItemPic(byte partyPos) {
            ComboBox cb = partyItemsComboboxList[partyPos];
            partyPokemonItemIconList[partyPos].Visible = cb.SelectedIndex > 0;
        }

        private void partyItem1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorItemPic(0);
        }

        private void partyItem2ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorItemPic(1);
        }

        private void partyItem3ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorItemPic(2);
        }

        private void partyItem4ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorItemPic(3);
        }

        private void partyItem5ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorItemPic(4);
        }

        private void partyItem6ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            showTrainerEditorItemPic(5);
        }

        private void DVExplainButton_Click(object sender, EventArgs e) {
            MessageBox.Show("DV, or \"Difficulty Value\", is used by the game engine to calculate how tough an opponent Pokemon should be.\n" +
                "The DV affects a Pokemon's Nature and IVs - the higher the value, the stronger the Pokemon.\n" +
                "DVs will go from 1 (0 IVs) to 255 (31 IVs). Natures are chosen semi-randomly." +
                "\nIVs will be the same value for all Stats at any DV, so Hidden Power will only be Fighting or Dark Type." +
                "\n\nFor the time being, DSPRE Reloaded is unable to calculate the target DV of a Pokémon for a given Nature and set of IVs.", "Difficulty Value", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void partyCountUpDown_ValueChanged(object sender, EventArgs e) {
            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
                partyGroupboxList[i].Enabled = (partyCountUpDown.Value > i);
                partyPokemonPictureBoxList[i].Visible = partyGroupboxList[i].Enabled;
            }
            for (int i = Math.Min(currentTrainerFile.trp.partyCount, (int)partyCountUpDown.Value); i < TrainerFile.POKE_IN_PARTY; i++) {
                currentTrainerFile.party[i] = new PartyPokemon(currentTrainerFile.trp.hasItems, currentTrainerFile.trp.hasMoves);
            }

            //if (!disableHandlers) {
            //    RefreshTrainerPartyGUI();
            //    RefreshTrainerPropertiesGUI();
            //}
        }

        private void trainerMovesCheckBox_CheckedChanged(object sender, EventArgs e) {
            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
                for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
                    (partyMovesGroupboxList[i].Controls[j] as ComboBox).Enabled = trainerMovesCheckBox.Checked;
                }
                currentTrainerFile.party[i].moves = trainerMovesCheckBox.Checked ? new ushort[Party.MOVES_PER_POKE] : null;
            }
        }
        private void trainerItemsCheckBox_CheckedChanged(object sender, EventArgs e) {
            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
                partyItemsComboboxList[i].Enabled = trainerItemsCheckBox.Checked;
            }
        }
        private void partyMoveComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!disableHandlers) {
                for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
                    ushort[] moves = currentTrainerFile.party[i].moves;

                    if (moves != null) {
                        for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
                            moves[j] = (ushort)(partyMovesGroupboxList[i].Controls[j] as ComboBox).SelectedIndex;
                        }
                    }
                }
            }
        }

        private void trainerSaveCurrentButton_Click(object sender, EventArgs e) {
            currentTrainerFile.trp.partyCount = (byte)partyCountUpDown.Value;
            currentTrainerFile.trp.hasMoves = trainerMovesCheckBox.Checked;
            currentTrainerFile.trp.hasItems = trainerItemsCheckBox.Checked;
            currentTrainerFile.trp.doubleBattle = trainerDoubleCheckBox.Checked;

            IList trainerItems = trainerItemsGroupBox.Controls;
            for (int i = 0; i < trainerItems.Count; i++) {
                currentTrainerFile.trp.trainerItems[i] = (ushort)(trainerItems[i] as ComboBox).SelectedIndex;
            }

            IList trainerAI = TrainerAIGroupBox.Controls;
            for (int i = 0; i < trainerAI.Count; i++) {
                currentTrainerFile.trp.AI[i] = (trainerAI[i] as CheckBox).Checked;
            }

            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++) {
                currentTrainerFile.party[i].moves = trainerMovesCheckBox.Checked ? new ushort[4] : null;
            }

            for (int i = 0; i < partyCountUpDown.Value; i++) {
                currentTrainerFile.party[i].pokeID = (ushort)partyPokemonComboboxList[i].SelectedIndex;
                currentTrainerFile.party[i].level = (ushort)partyLevelUpdownList[i].Value;

                if (trainerMovesCheckBox.Checked) {
                    IList movesList = partyMovesGroupboxList[i].Controls;
                    for (int j = 0; j < Party.MOVES_PER_POKE; j++) {
                        currentTrainerFile.party[i].moves[j] = (ushort)(movesList[j] as ComboBox).SelectedIndex;
                    }
                }

                if (trainerItemsCheckBox.Checked) {
                    currentTrainerFile.party[i].heldItem = (ushort)partyItemsComboboxList[i].SelectedIndex;
                }

                currentTrainerFile.party[i].unknown1_DATASTART = (ushort)partyIVUpdownList[i].Value;
                currentTrainerFile.party[i].unknown2_DATAEND = (ushort)partyBallUpdownList[i].Value;
            }

            /*Write to File*/
            string indexStr = "\\" + trainerComboBox.SelectedIndex.ToString("D4");
            File.WriteAllBytes(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir + indexStr, currentTrainerFile.trp.ToByteArray());
            File.WriteAllBytes(RomInfo.gameDirs[DirNames.trainerParty].unpackedDir + indexStr, currentTrainerFile.party.ToByteArray());

            UpdateCurrentTrainerName(newName: trainerNameTextBox.Text);
            UpdateCurrentTrainerShownName();

            MessageBox.Show("Trainer saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateCurrentTrainerShownName() {
            string trClass = GetTrainerClassNameFromListbox(trainerClassListBox.SelectedItem);

            string editedTrainer = "[" + currentTrainerFile.trp.trainerID.ToString("D2") + "] " + trClass + " " + currentTrainerFile.name;

            disableHandlers = true;
            trainerComboBox.Items[trainerComboBox.SelectedIndex] = editedTrainer;
            disableHandlers = false;

            if (eventEditorIsReady) {
                owTrainerComboBox.Items[trainerComboBox.SelectedIndex] = editedTrainer;
            }
        }

        private string GetTrainerClassNameFromListbox(object selectedItem) {
            string lbname = selectedItem.ToString();
            return lbname.Substring(lbname.IndexOf(" ") + 1);
        }

        private void UpdateCurrentTrainerName(string newName) {
            currentTrainerFile.name = newName;
            TextArchive trainerNames = new TextArchive(RomInfo.trainerNamesMessageNumber);
            if (currentTrainerFile.trp.trainerID < trainerNames.messages.Count) {
                trainerNames.messages[currentTrainerFile.trp.trainerID] = newName;
            } else {
                trainerNames.messages.Add(newName);
            }
            trainerNames.SaveToFileDefaultDir(RomInfo.trainerNamesMessageNumber, showSuccessMessage: false);
        }
        private void UpdateCurrentTrainerClassName(string newName) {             
            TextArchive trainerClassNames = new TextArchive(RomInfo.trainerClassMessageNumber);
            trainerClassNames.messages[trainerClassListBox.SelectedIndex] = newName;
            trainerClassNames.SaveToFileDefaultDir(RomInfo.trainerClassMessageNumber, showSuccessMessage: false);
        }

        private void trainerClassListBox_SelectedIndexChanged(object sender, EventArgs e) {
            int selection = trainerClassListBox.SelectedIndex;
            if (selection < 0) {
                return;
            }

            try {
                int maxFrames = LoadTrainerClassPic(selection);
                UpdateTrainerClassPic(trainerClassPicBox);

                trClassFramePreviewUpDown.Maximum = maxFrames;
                trainerClassFrameMaxLabel.Text = "/" + maxFrames;
            } catch {
                trClassFramePreviewUpDown.Maximum = 0;
            }

            trainerClassNameTextbox.Text = GetTrainerClassNameFromListbox(trainerClassListBox.SelectedItem);

            if (trainerClassEncounterMusicDict.TryGetValue((byte)selection, out (uint entryOffset, ushort musicD, ushort? musicN) output)) {
                encounterSSEQMainUpDown.Enabled = eyeContactMusicLabel.Enabled = true;
                encounterSSEQMainUpDown.Value = output.musicD;
            } else {
                encounterSSEQMainUpDown.Enabled = eyeContactMusicLabel.Enabled = false;
                encounterSSEQMainUpDown.Value = 0;
            }

            eyeContactMusicAltLabel.Enabled = encounterSSEQAltUpDown.Enabled = (encounterSSEQMainUpDown.Enabled && gameFamily == gFamEnum.HGSS);
            encounterSSEQAltUpDown.Value = output.musicN != null ? (ushort)output.musicN : 0;
            currentTrainerFile.trp.trainerClass = (byte)selection;
        }

        private int LoadTrainerClassPic(int trClassID) {
            int paletteFileID = (trClassID * 5 + 1);
            string paletteFilename = paletteFileID.ToString("D4");
            pal = new NCLR(gameDirs[DirNames.trainerGraphics].unpackedDir + "\\" + paletteFilename, paletteFileID, paletteFilename);

            int tilesFileID = trClassID * 5;
            string tilesFilename = tilesFileID.ToString("D4");
            tiles = new NCGR(gameDirs[DirNames.trainerGraphics].unpackedDir + "\\" + tilesFilename, tilesFileID, tilesFilename);

            if (gameFamily == gFamEnum.DP) {
                return 0;
            }
            
            int spriteFileID = (trClassID * 5 + 2);
            string spriteFilename = spriteFileID.ToString("D4");
            sprite = new NCER(gameDirs[DirNames.trainerGraphics].unpackedDir + "\\" + spriteFilename, spriteFileID, spriteFilename);

            return sprite.Banks.Length - 1;
        }
        private void UpdateTrainerClassPic(PictureBox pb, int frameNumber = 0) {
            if (sprite == null) {
                Console.WriteLine("Sprite is null!");
                return;
            }

            int bank0OAMcount = sprite.Banks[0].oams.Length;
            int[] OAMenabled = new int[bank0OAMcount];
            for (int i = 0; i < OAMenabled.Length; i++) {
                OAMenabled[i] = i;
            }

            frameNumber = Math.Min(sprite.Banks.Length, frameNumber);
            Image trSprite = sprite.Get_Image(tiles, pal, frameNumber, trainerClassPicBox.Width, trainerClassPicBox.Height, false, false, false, true, true, -1, OAMenabled);
            pb.Image = trSprite;
            pb.Update();
        }

        private void addTrainerButton_Click(object sender, EventArgs e) {
            /* Add new trainer file to 2 folders */
            string suffix = "\\" + trainerComboBox.Items.Count.ToString("D4");

            string trainerPropertiesPath = gameDirs[DirNames.trainerProperties].unpackedDir + suffix;
            string partyFilePath = gameDirs[DirNames.trainerParty].unpackedDir + suffix;

            File.WriteAllBytes(trainerPropertiesPath, new TrainerProperties((ushort)trainerComboBox.Items.Count).ToByteArray());
            File.WriteAllBytes(partyFilePath, new PartyPokemon().ToByteArray());

            TextArchive trainerClasses = new TextArchive(RomInfo.trainerClassMessageNumber);
            TextArchive trainerNames = new TextArchive(RomInfo.trainerNamesMessageNumber);

            /* Update ComboBox and select new file */
            trainerComboBox.Items.Add(trainerClasses.messages[0]);
            trainerNames.messages.Add("");
            trainerNames.SaveToFileDefaultDir(RomInfo.trainerNamesMessageNumber, showSuccessMessage: false);

            trainerComboBox.SelectedIndex = trainerComboBox.Items.Count - 1;
            UpdateCurrentTrainerShownName();
        }

        private void exportTrainerButton_Click(object sender, EventArgs e) {
            currentTrainerFile.SaveToFileExplorePath("G4 Trainer File " + trainerComboBox.SelectedItem);
        }

        private void importTrainerButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Gen IV Trainer File (*.trf)|*.trf"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update trainer on disk */
            using (DSUtils.EasyReader reader = new DSUtils.EasyReader(of.FileName)) {
                string trName = reader.ReadString();

                byte datSize = reader.ReadByte();
                byte[] trDat = reader.ReadBytes(datSize);

                byte partySize = reader.ReadByte();
                byte[] pDat = reader.ReadBytes(partySize);

                string pathData = RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir + "\\" + trainerComboBox.SelectedIndex.ToString("D4");
                string pathParty = RomInfo.gameDirs[DirNames.trainerParty].unpackedDir + "\\" + trainerComboBox.SelectedIndex.ToString("D4");
                File.WriteAllBytes(pathData, trDat);
                File.WriteAllBytes(pathParty, pDat);

                UpdateCurrentTrainerName(trName);
            }
            /* Refresh controls and re-read file */
            trainerComboBox_SelectedIndexChanged(null, null);
            UpdateCurrentTrainerShownName();

            /* Display success message */
            MessageBox.Show("Trainer File imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exportPropertiesButton_Click(object sender, EventArgs e) {
            currentTrainerFile.trp.SaveToFileExplorePath("G4 Trainer Properties " + trainerComboBox.SelectedItem);
        }

        private void importReplacePropertiesButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Gen IV Trainer Properties (*.trp)|*.trp"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update trp object in memory */
            currentTrainerFile.trp = new TrainerProperties((ushort)trainerComboBox.SelectedIndex, new FileStream(of.FileName, FileMode.Open));
            RefreshTrainerPropertiesGUI();

            /* Display success message */
            MessageBox.Show("Trainer Properties imported successfully!\nRemember to save the current Trainer File.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exportPartyButton_Click(object sender, EventArgs e) {
            currentTrainerFile.party.exportCondensedData = true;
            currentTrainerFile.party.SaveToFileExplorePath("G4 Party Data " + trainerComboBox.SelectedItem);
            currentTrainerFile.party.exportCondensedData = false;
        }

        private void importReplacePartyButton_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Gen IV Party File (*.pdat)|*.pdat"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update trp object in memory */
            currentTrainerFile.party = new Party(readFirstByte: true, TrainerFile.POKE_IN_PARTY, new FileStream(of.FileName, FileMode.Open), currentTrainerFile.trp);
            RefreshTrainerPropertiesGUI();
            RefreshTrainerPartyGUI();

            /* Display success message */
            MessageBox.Show("Trainer Party imported successfully!\nRemember to save the current Trainer File.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveTrainerClassButton_Click(object sender, EventArgs e) {
            disableHandlers = true;

            int selectedTrClass = trainerClassListBox.SelectedIndex;

            byte b_selectedTrClass = (byte)selectedTrClass;
            ushort eyeMusicID = (ushort)encounterSSEQMainUpDown.Value;

            if (trainerClassEncounterMusicDict.TryGetValue(b_selectedTrClass, out var dictEntry)) {
                DSUtils.ARM9.WriteBytes(BitConverter.GetBytes(eyeMusicID), dictEntry.entryOffset);
                trainerClassEncounterMusicDict[b_selectedTrClass] = (dictEntry.entryOffset, eyeMusicID, dictEntry.musicN);
            }

            string newName = trainerClassNameTextbox.Text;
            UpdateCurrentTrainerClassName(newName);
            trainerClassListBox.Items[selectedTrClass] = "[" + selectedTrClass.ToString("D3") + "]" + " " + newName;

            if (currentTrainerFile.trp.trainerClass == trainerClassListBox.SelectedIndex) { 
                UpdateCurrentTrainerShownName();
            }
            disableHandlers = false;

            //trainerClassListBox_SelectedIndexChanged(null, null);
            if ( gameFamily.Equals(gFamEnum.HGSS) && tableEditorIsReady ) { 
                pbEffectsTrainerCombobox.Items[selectedTrClass] = trainerClassListBox.Items[selectedTrClass];
                for (int i = 0; i < vsTrainerEffectsList.Count; i++) {
                    if (vsTrainerEffectsList[i].trainerClass == selectedTrClass) {
                        pbEffectsVsTrainerListbox.Items[i] = pbEffectsTrainerCombobox.Items[selectedTrClass] + " uses Combo #" + vsTrainerEffectsList[i].comboID;
                    }
                }
            }
            MessageBox.Show("Trainer Class settings saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void trClassFramePreviewUpDown_ValueChanged(object sender, EventArgs e) {
            UpdateTrainerClassPic(trainerClassPicBox, (int)((NumericUpDown)sender).Value);
        }
        #endregion

        #region Table Editor
        #region Variables

        string[] pokeNames;
        string[] trcNames;

        List<(ushort header, ushort flag, ushort music)> conditionalMusicTable;
        uint conditionalMusicTableStartAddress;

        List<(int trainerClass, int comboID)> vsTrainerEffectsList;
        List<(int pokemonID, int comboID)> vsPokemonEffectsList;
        List<(ushort vsGraph, ushort battleSSEQ)> effectsComboTable;

        uint vsTrainerTableStartAddress;
        uint vsPokemonTableStartAddress;
        uint effectsComboMainTableStartAddress;
        #endregion

        private void SetupConditionalMusicTable() {
            switch (RomInfo.gameFamily) {
                case gFamEnum.HGSS:
                    RomInfo.SetConditionalMusicTableOffsetToRAMAddress();
                    conditionalMusicTable = new List<(ushort, ushort, ushort)>();

                    conditionalMusicTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.conditionalMusicTableOffsetToRAMAddress, 4), 0) - 0x02000000;
                    byte tableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.conditionalMusicTableOffsetToRAMAddress - 8);

                    conditionalMusicTableListBox.Items.Clear();
                    using (DSUtils.ARM9.Reader ar = new DSUtils.ARM9.Reader(conditionalMusicTableStartAddress)) {
                        for (int i = 0; i < tableEntriesCount; i++) {
                            ushort header = ar.ReadUInt16();
                            ushort flag = ar.ReadUInt16();
                            ushort musicID = ar.ReadUInt16();

                            conditionalMusicTable.Add((header, flag, musicID));
                            conditionalMusicTableListBox.Items.Add(headerListBox.Items[header]);
                        }
                    }

                    headerConditionalMusicComboBox.Items.Clear();
                    foreach (string location in headerListBox.Items) {
                        headerConditionalMusicComboBox.Items.Add(location);
                    }

                    if (conditionalMusicTableListBox.Items.Count > 0) {
                        conditionalMusicTableListBox.SelectedIndex = 0;
                    }
                    break;

                case gFamEnum.Plat:
                    pbEffectsMonGroupBox.Enabled = false;
                    pbEffectsTrainerGroupBox.Enabled = false;
                    break;

                default:
                    pbEffectsGroupBox.Enabled = false;
                    pbEffectsMonGroupBox.Enabled = false;
                    pbEffectsTrainerGroupBox.Enabled = false;
                    conditionalMusicGroupBox.Enabled = false;
                    break;
            }
        }
        private void SetupBattleEffectsTables() {
            if (RomInfo.gameFamily == gFamEnum.HGSS || RomInfo.gameFamily == gFamEnum.Plat) {
                DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.trainerGraphics, DirNames.textArchives });
                RomInfo.SetBattleEffectsData();

                effectsComboTable = new List<(ushort vsGraph, ushort battleSSEQ)>();
                
                effectsComboMainTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.effectsComboTableOffsetToRAMAddress, 4), 0);
                ROMToolboxDialog.flag_MainComboTableRepointed = (effectsComboMainTableStartAddress >= ROMToolboxDialog.synthOverlayLoadAddress);
                effectsComboMainTableStartAddress -= ROMToolboxDialog.flag_MainComboTableRepointed ? ROMToolboxDialog.synthOverlayLoadAddress : DSUtils.ARM9.address;

                byte comboTableEntriesCount;

                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    comboTableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.effectsComboTableOffsetToSizeLimiter);

                    vsPokemonEffectsList = new List<(int pokemonID, int comboID)>();
                    vsTrainerEffectsList = new List<(int trainerClass, int comboID)>();

                    vsPokemonTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.vsPokemonEntryTableOffsetToRAMAddress, 4), 0);
                    ROMToolboxDialog.flag_PokemonBattleTableRepointed = (vsPokemonTableStartAddress >= ROMToolboxDialog.synthOverlayLoadAddress);
                    vsPokemonTableStartAddress -= ROMToolboxDialog.flag_PokemonBattleTableRepointed ? ROMToolboxDialog.synthOverlayLoadAddress : DSUtils.ARM9.address;

                    vsTrainerTableStartAddress = BitConverter.ToUInt32(DSUtils.ARM9.ReadBytes(RomInfo.vsTrainerEntryTableOffsetToRAMAddress, 4), 0);
                    ROMToolboxDialog.flag_TrainerClassBattleTableRepointed = (vsTrainerTableStartAddress >= ROMToolboxDialog.synthOverlayLoadAddress);
                    vsTrainerTableStartAddress -= ROMToolboxDialog.flag_TrainerClassBattleTableRepointed ? ROMToolboxDialog.synthOverlayLoadAddress : DSUtils.ARM9.address;


                    pbEffectsPokemonCombobox.Items.Clear();
                    pokeNames = RomInfo.GetPokémonNames();
                    for (int i = 0; i < pokeNames.Length; i++) {
                        pbEffectsPokemonCombobox.Items.Add("[" + i + "]" + " " + pokeNames[i]);
                    }

                    RepopulateTableEditorTrainerClasses();

                    pbEffectsVsTrainerListbox.Items.Clear();
                    pbEffectsVsPokemonListbox.Items.Clear();
                } else {
                    comboTableEntriesCount = 35;
                }

                pbEffectsCombosListbox.Items.Clear();

                String expArmPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + ROMToolboxDialog.expandedARMfileID.ToString("D4");

                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    using (DSUtils.EasyReader ar = new DSUtils.EasyReader(ROMToolboxDialog.flag_TrainerClassBattleTableRepointed ? expArmPath : RomInfo.arm9Path, vsTrainerTableStartAddress)) {
                        byte trainerTableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.vsTrainerEntryTableOffsetToSizeLimiter);

                        for (int i = 0; i < trainerTableEntriesCount; i++) {
                            ushort entry = ar.ReadUInt16();
                            int classID = entry & 1023;
                            int comboID = entry >> 10;
                            vsTrainerEffectsList.Add((classID, comboID));
                            pbEffectsVsTrainerListbox.Items.Add(pbEffectsTrainerCombobox.Items[classID] + " uses Combo #" + comboID);
                        }
                    }

                    using (DSUtils.EasyReader ar = new DSUtils.EasyReader(ROMToolboxDialog.flag_PokemonBattleTableRepointed ? expArmPath : RomInfo.arm9Path, vsPokemonTableStartAddress)) {
                        byte pokemonTableEntriesCount = DSUtils.ARM9.ReadByte(RomInfo.vsPokemonEntryTableOffsetToSizeLimiter);

                        for (int i = 0; i < pokemonTableEntriesCount; i++) {
                            ushort entry = ar.ReadUInt16();
                            int pokeID = entry & 1023;
                            int comboID = entry >> 10;
                            vsPokemonEffectsList.Add((pokeID, comboID));

                            string pokeName;
                            try {
                                pokeName = pokeNames[pokeID];
                            } catch (IndexOutOfRangeException) {
                                pokeName = "UNKNOWN";
                            }
                            pbEffectsVsPokemonListbox.Items.Add("[" + pokeID.ToString("D3") + "]" + " " + pokeName + " uses Combo #" + comboID);
                        }
                    }
                }

                using (DSUtils.EasyReader ar = new DSUtils.EasyReader(ROMToolboxDialog.flag_MainComboTableRepointed ? expArmPath : RomInfo.arm9Path, effectsComboMainTableStartAddress)) {
                    for (int i = 0; i < comboTableEntriesCount; i++) {
                        ushort battleIntroEffect = ar.ReadUInt16();
                        ushort battleMusic = ar.ReadUInt16();
                        effectsComboTable.Add((battleIntroEffect, battleMusic));
                        pbEffectsCombosListbox.Items.Add("Combo " + i.ToString("D2") + " - " + "Effect #" + battleIntroEffect + ", " + "Music #" + battleMusic);
                    }
                }

                if (RomInfo.gameFamily == gFamEnum.HGSS) {
                    var items = pbEffectsCombosListbox.Items.Cast<Object>().ToArray();

                    pbEffectsPokemonChooseMainCombobox.Items.Clear();
                    pbEffectsPokemonChooseMainCombobox.Items.AddRange(items);
                    pbEffectsTrainerChooseMainCombobox.Items.Clear();
                    pbEffectsTrainerChooseMainCombobox.Items.AddRange(items);

                    if (pbEffectsVsTrainerListbox.Items.Count > 0) {
                        pbEffectsVsTrainerListbox.SelectedIndex = 0;
                    }
                    if (pbEffectsVsPokemonListbox.Items.Count > 0) {
                        pbEffectsVsPokemonListbox.SelectedIndex = 0;
                    }
                }

                if (pbEffectsCombosListbox.Items.Count > 0) {
                    pbEffectsCombosListbox.SelectedIndex = 0;
                }
                
            } else {
                pbEffectsGroupBox.Enabled = false;
            }
        }

        private void RepopulateTableEditorTrainerClasses() {
            pbEffectsTrainerCombobox.Items.Clear();
            trcNames = RomInfo.GetTrainerClassNames();
            for (int i = 0; i < trcNames.Length; i++) {
                pbEffectsTrainerCombobox.Items.Add("[" + i.ToString("D3") + "]" + " " + trcNames[i]);
            }
        }

        private void conditionalMusicTableListBox_SelectedIndexChanged(object sender, EventArgs e) {
            int selection = conditionalMusicTableListBox.SelectedIndex;
            headerConditionalMusicComboBox.SelectedIndex = conditionalMusicTable[selection].header;

            disableHandlers = true;

            flagConditionalMusicUpDown.Value = conditionalMusicTable[selection].flag;
            musicIDconditionalMusicUpDown.Value = conditionalMusicTable[selection].music;
            
            disableHandlers = false;
        }
        private void headerConditionalMusicComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            (ushort header, ushort flag, ushort music) oldTuple = conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex];
            (ushort header, ushort flag, ushort music) newTuple = ((ushort)headerConditionalMusicComboBox.SelectedIndex, oldTuple.flag, oldTuple.music);
            conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex] = newTuple;

            MapHeader selected = MapHeader.LoadFromARM9(newTuple.header);
            switch (RomInfo.gameFamily) {
                case gFamEnum.DP:
                    locationNameConditionalMusicLBL.Text = RomInfo.GetLocationNames()[(selected as HeaderDP).locationName];
                    break;
                case gFamEnum.Plat:
                    locationNameConditionalMusicLBL.Text = RomInfo.GetLocationNames()[(selected as HeaderPt).locationName];
                    break;
                case gFamEnum.HGSS:
                    locationNameConditionalMusicLBL.Text = RomInfo.GetLocationNames()[(selected as HeaderHGSS).locationName];
                    break;
            }
        }
        private void flagConditionalMusicUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            (ushort header, ushort flag, ushort music) oldTuple = conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex];
            conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex] = (oldTuple.header, (ushort)flagConditionalMusicUpDown.Value, oldTuple.music);
        }

        private void musicIDconditionalMusicUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            (ushort header, ushort flag, ushort music) oldTuple = conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex];
            conditionalMusicTable[conditionalMusicTableListBox.SelectedIndex] = (oldTuple.header, oldTuple.flag, (ushort)musicIDconditionalMusicUpDown.Value);
        }
        private void HOWconditionalMusicTableButton_Click(object sender, EventArgs e) {
            MessageBox.Show("For each Location in the list, override Header's music with chosen Music ID, if Flag is set.", "How this table works", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveConditionalMusicTableBTN_Click(object sender, EventArgs e) {
            for (int i = 0; i < conditionalMusicTable.Count; i++) {
                DSUtils.ARM9.WriteBytes(BitConverter.GetBytes(conditionalMusicTable[i].header), (uint)(conditionalMusicTableStartAddress + 6 * i));
                DSUtils.ARM9.WriteBytes(BitConverter.GetBytes(conditionalMusicTable[i].flag), (uint)(conditionalMusicTableStartAddress + 6 * i + 2));
                DSUtils.ARM9.WriteBytes(BitConverter.GetBytes(conditionalMusicTable[i].music), (uint)(conditionalMusicTableStartAddress + 6 * i + 4));
            }
        }

        private void TBLEditortrainerClassPreviewPic_ValueChanged(object sender, EventArgs e) {
            UpdateTrainerClassPic(tbEditorTrClassPictureBox, (int)((NumericUpDown)sender).Value);
        }

        private void saveEffectComboBTN_Click(object sender, EventArgs e) {
            int index = pbEffectsCombosListbox.SelectedIndex;
            ushort battleIntroEffect = (ushort)pbEffectsVSAnimationUpDown.Value;
            ushort battleMusic = (ushort)pbEffectsBattleSSEQUpDown.Value;

            effectsComboTable[index] = (battleIntroEffect, battleMusic);

            String expArmPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + ROMToolboxDialog.expandedARMfileID.ToString("D4");
            using (DSUtils.EasyWriter wr = new DSUtils.EasyWriter(ROMToolboxDialog.flag_MainComboTableRepointed ? expArmPath : RomInfo.arm9Path, effectsComboMainTableStartAddress + 4 * index)) {
                wr.Write(battleIntroEffect);
                wr.Write(battleMusic);
            };

            disableHandlers = true;
            pbEffectsCombosListbox.Items[index] = pbEffectsTrainerChooseMainCombobox.Items[index] = pbEffectsPokemonChooseMainCombobox.Items[index] = "Combo " + index.ToString("D2") + " - " + "Effect #" + battleIntroEffect + ", " + "Music #" + battleMusic;
            disableHandlers = false;
        }

        private void saveVSPokemonEntryBTN_Click(object sender, EventArgs e) {
            int index = pbEffectsVsPokemonListbox.SelectedIndex;
            ushort pokemonID = (ushort)pbEffectsPokemonCombobox.SelectedIndex;
            ushort comboID = (ushort)pbEffectsPokemonChooseMainCombobox.SelectedIndex;

            vsPokemonEffectsList[index] = (pokemonID, comboID);

            String expArmPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + ROMToolboxDialog.expandedARMfileID.ToString("D4");
            using (DSUtils.EasyWriter wr = new DSUtils.EasyWriter(ROMToolboxDialog.flag_PokemonBattleTableRepointed ? expArmPath : RomInfo.arm9Path, vsPokemonTableStartAddress + 2 * index)) {
                wr.Write((ushort)((pokemonID & 1023) + (comboID << 10))); //PokemonID
            };

            disableHandlers = true;
            pbEffectsVsPokemonListbox.Items[index] = "[" + pokemonID.ToString("D3") + "]" + " " + pokeNames[pokemonID] + " uses Combo #" + comboID;
            disableHandlers = false;
        }

        private void saveVSTrainerEntryBTN_Click(object sender, EventArgs e) {
            int index = pbEffectsVsTrainerListbox.SelectedIndex;
            ushort trainerClass = (ushort)pbEffectsTrainerCombobox.SelectedIndex;
            ushort comboID = (ushort)pbEffectsTrainerChooseMainCombobox.SelectedIndex;

            vsTrainerEffectsList[index] = (trainerClass, comboID);
            String expArmPath = RomInfo.gameDirs[DirNames.synthOverlay].unpackedDir + '\\' + ROMToolboxDialog.expandedARMfileID.ToString("D4");
            using (DSUtils.EasyWriter wr = new DSUtils.EasyWriter(ROMToolboxDialog.flag_TrainerClassBattleTableRepointed ? expArmPath : RomInfo.arm9Path, vsTrainerTableStartAddress + 2 * index)) { 
                wr.Write((ushort)((trainerClass & 1023) + (comboID << 10))); 
            };

            disableHandlers = true;
            pbEffectsVsTrainerListbox.Items[index] = "[" + trainerClass.ToString("D3") + "]" + " " + trcNames[trainerClass] + " uses Combo #" + comboID;
            disableHandlers = false;
        }

        private void HOWpbEffectsTableButton_Click(object sender, EventArgs e) {
            MessageBox.Show("An entry of this table is a combination of VS. Graphics + Battle Theme.\n\n" +
                (RomInfo.gameFamily.Equals(gFamEnum.HGSS) ? "Each entry can be \"inherited\" by one or more Pokémon or Trainer classes." : ""), 
                "How this table works", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void HOWvsPokemonButton_Click(object sender, EventArgs e) {
            MessageBox.Show("Each entry of this table links a \"Wild\" Pokémon to an Effect Combo from the Combos Table.\n\n" +
                "Whenever that Pokémon is encountered in the tall grass or via script command, its VS. Sequence and Battle Theme will be automatically triggered.",
                 "How this table works", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void HOWVsTrainerButton_Click(object sender, EventArgs e) {
            MessageBox.Show("Each entry of this table links a Trainer Class to an Effect Combo from the Combos Table.\n\n" +
                "Every Trainer Class with a given combo will start the same VS. Sequence and Battle Theme.", "How this table works", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pbEffectsVsTrainerListbox_SelectedIndexChanged(object sender, EventArgs e) {
            int trainerSelection = pbEffectsVsTrainerListbox.SelectedIndex;
            if (disableHandlers || trainerSelection < 0) {
                return;
            }

            (int trainerClass, int comboID) entry = vsTrainerEffectsList[trainerSelection];
            pbEffectsTrainerCombobox.SelectedIndex = entry.trainerClass;
            pbEffectsCombosListbox.SelectedIndex = pbEffectsTrainerChooseMainCombobox.SelectedIndex = entry.comboID;

            tbEditorTrClassFramePreviewUpDown.Value = 0;
        }

        private void pbEffectsVsPokemonListbox_SelectedIndexChanged(object sender, EventArgs e) {
            int pokemonSelection = pbEffectsVsPokemonListbox.SelectedIndex;

            if (disableHandlers || pokemonSelection < 0) {
                return;
            }

            (int pokemonID, int comboID) entry = vsPokemonEffectsList[pokemonSelection];

            try {
                pbEffectsPokemonCombobox.SelectedIndex = entry.pokemonID;
            } catch (ArgumentOutOfRangeException) {
                pbEffectsPokemonCombobox.SelectedIndex = 0;
            }
            pbEffectsCombosListbox.SelectedIndex = pbEffectsPokemonChooseMainCombobox.SelectedIndex = entry.comboID;
        }

        private void pbEffectsCombosListbox_SelectedIndexChanged(object sender, EventArgs e) {
            int comboSelection = pbEffectsCombosListbox.SelectedIndex;

            if (disableHandlers || comboSelection < 0) {
                return;
            }

            (ushort vsGraph, ushort battleSSEQ) entry = effectsComboTable[comboSelection];
            pbEffectsBattleSSEQUpDown.Value = entry.battleSSEQ;
            pbEffectsVSAnimationUpDown.Value = entry.vsGraph;
        }

        private void pbEffectsTrainerCombobox_SelectedIndexChanged(object sender, EventArgs e) {
            int maxFrames = LoadTrainerClassPic((sender as ComboBox).SelectedIndex);
            UpdateTrainerClassPic(tbEditorTrClassPictureBox);

            tbEditorTrClassFramePreviewUpDown.Maximum = maxFrames;
            tbEditortrainerClassFrameMaxLabel.Text = "/" + maxFrames;
        }
        private void pbEffectsPokemonCombobox_SelectedIndexChanged(object sender, EventArgs e) {
            ComboBox cb = sender as ComboBox;
            Image pokeIcon = cb.SelectedIndex > 0 ? (Image)Properties.PokePics.ResourceManager.GetObject(FixPokenameString(PokeDatabase.System.pokeNames[(ushort)cb.SelectedIndex])) : null;
            tbEditorPokeminiPictureBox.Image = pokeIcon;
            tbEditorPokeminiPictureBox.Update();
        }

        #endregion
        private void ExclusiveCBInvert(CheckBox cb) {
            if (disableHandlers) {
                return;
            }

            disableHandlers = true;

            if (cb.Checked) {
                cb.Checked = !cb.Checked;
            }

            disableHandlers = false;
        }

        private void unpackToFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                Filter = "NARC File (*.narc)|*.narc"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Narc userfile = Narc.Open(of.FileName);
            if (userfile is null) {
                MessageBox.Show("The file you selected is not a valid NARC.", "Cannot proceed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("Choose where to save the NARC content.\nDSPRE will automatically make a subdirectory.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CommonOpenFileDialog narcDir = new CommonOpenFileDialog {
                IsFolderPicker = true,
                Multiselect = false
            };

            if (narcDir.ShowDialog() != CommonFileDialogResult.Ok) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string finalExtractedPath = narcDir.FileName + "\\" + Path.GetFileNameWithoutExtension(of.FileName);
            userfile.ExtractToFolder(finalExtractedPath);
            MessageBox.Show("The contents of " + of.FileName + " have been extracted and saved.", "NARC Extracted", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult d = MessageBox.Show("Do you want to rename the files according to their contents?", "Waiting for user", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (d.Equals(DialogResult.Yes)) {
                ContentBasedBatchRename(new DirectoryInfo(finalExtractedPath));
            }
        }

        private void buildFromFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            CommonOpenFileDialog narcDir = new CommonOpenFileDialog {
                IsFolderPicker = true,
                Multiselect = false
            };

            if (narcDir.ShowDialog() != CommonFileDialogResult.Ok) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("Choose where to save the output NARC file.", "Name your NARC file", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SaveFileDialog sf = new SaveFileDialog {
                Filter = "NARC File (*.narc)|*.narc",
                FileName = Path.GetFileName(narcDir.FileName)
            };
            if (sf.ShowDialog(this) != DialogResult.OK) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Narc.FromFolder(narcDir.FileName).Save(sf.FileName);
            MessageBox.Show("The contents of folder \"" + narcDir.FileName + "\" have been packed.", "NARC Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listBasedToolStripMenuItem_Click(object sender, EventArgs e) {
            (DirectoryInfo d, FileInfo[] files) dirData = OpenNonEmptyDir(title: "List-Based Batch Rename Tool");
            DirectoryInfo d = dirData.d;
            FileInfo[] files = dirData.files;

            if (d == null || files == null) {
                return;
            }

            /*==================================================================*/

            MessageBox.Show("Choose your enumeration text file.", "Input list file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            OpenFileDialog of = new OpenFileDialog {
                Filter = "List File (*.txt; *.list)|*.txt;*.list"
            };

            if (of.ShowDialog(this) != DialogResult.OK) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            /*==================================================================*/

            const string COMMENT_CHAR = "#";
            const string ISOLATED_FOLDERNAME = "DSPRE_IsolatedFiles";

            string[] listLines = File.ReadAllLines(of.FileName);
            listLines = listLines.Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith(COMMENT_CHAR)).ToArray();

            if (listLines.Length <= 0) {
                MessageBox.Show("The enumeration text file you selected is empty or only contains comment lines.\nCan't proceed.", "Invalid list file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string msg = "About to process ";
            int tot;
            string extra = "";

            int diff = files.Length - listLines.Length;
            if ( diff < 0 ) { //listLines.Length > files.Length 
                tot = files.Length;
                extra = "(Please note that the length of the chosen list [" + listLines.Length + " entries] " +
                    "exceeds the number of files in the folder.)" + "\n\n";
            } else if ( diff == 0 ) { //listLines.Length == files.Length
                tot = files.Length;
            } else { // diff > 0 --> listLines.Length < files.Length
                tot = listLines.Length;
                extra = "(Please note that there aren't enough entries in the list to rename all files in the chosen folder.\n" +
                    diff + " file" + (diff > 1 ? "s" : "") + " won't be renamed.)" + "\n\n";
            }

            msg += tot + " file" + (tot > 1 ? "s" : "");

            DialogResult dr = MessageBox.Show(msg + " from the input folder (taken in ascending order), " +
                "according to the list file you provided.\n" +
                "If a destination file already exists, DSPRE will append a number to its name.\n\n" + extra +
                "Do you want to proceed?", "Confirm operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr.Equals(DialogResult.Yes)) {
                int i;
                for (i = 0; i < tot; i++) {
                    FileInfo f = files[i];
                    Console.WriteLine(f.Name);
                    string destName = Path.GetDirectoryName(f.FullName) + "\\" + listLines[i];

                    if (string.IsNullOrWhiteSpace(destName)) {
                        continue;
                    }

                    File.Move(f.FullName, MakeUniqueName(destName));
                }

                MessageBox.Show("The contents of folder \"" + d.FullName + "\" have been renamed according to " + "\"" + of.FileName + "\".", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (listLines.Length < files.Length) {
                    dr = MessageBox.Show("Do you want to isolate the unnamed files by moving them to a dedicated folder?", "Waiting for user", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr.Equals(DialogResult.Yes)) {
                        string isolatedDir = d.FullName + "\\" + ISOLATED_FOLDERNAME;
                        if (Directory.Exists(isolatedDir)) {
                            Directory.Delete(isolatedDir);
                        }
                        Directory.CreateDirectory(d.FullName + "\\" + ISOLATED_FOLDERNAME);

                        while ( i < files.Length ) {
                            FileInfo f = files[i];
                            Console.WriteLine(f.Name);
                            string destName = d.FullName + "\\" + ISOLATED_FOLDERNAME + "\\" + f.Name;
                            File.Move(f.FullName, destName);
                            i++;
                        }
                        MessageBox.Show("Isolated files have been moved to " + "\"" + ISOLATED_FOLDERNAME + "\"", "Files moved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            } else {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void contentBasedToolStripMenuItem_Click(object sender, EventArgs e) {
            ContentBasedBatchRename();
        }

        private void ContentBasedBatchRename(DirectoryInfo d = null) {
            (DirectoryInfo d, FileInfo[] files) dirData = OpenNonEmptyDir(d, title: "Content-Based Batch Rename Tool");
            d = dirData.d;
            FileInfo[] files = dirData.files;
            
            if (d == null || files == null) {
                return;
            }

            DialogResult dr = MessageBox.Show("About to rename " + files.Length + " file" + (files.Length > 1 ? "s" : "") +
                " from the input folder (taken in ascending order), according to their content.\n" +
                "If a destination file already exists, DSPRE will append a number to its name.\n\n" +
                "Do you want to proceed?", "Confirm operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr.Equals(DialogResult.Yes)) {
                List<string> enumerationFile = new List<string> {
                    "#============================================================================",
                    "# File enumeration definition for folder " + "\"" + d.Name + "\"",
                    "#============================================================================"
                };
                int initialLength = enumerationFile.Count;

                const byte toRead = 16;
                foreach (FileInfo f in files) {
                    Console.WriteLine(f.Name);

                    string fileNameOnly = Path.GetFileNameWithoutExtension(f.FullName);
                    string dirNameOnly = Path.GetDirectoryName(f.FullName);

                    string destName = "";
                    byte[] b = DSUtils.ReadFromFile(f.FullName, 0, toRead);

                    if (b == null || b.Length < toRead) {
                        continue;
                    }

                    string magic = "";

                    if (b[0] == 'B' && b[3] == '0') { //B**0
                        ushort nameOffset;

                        destName = dirNameOnly + "\\"; //Full filename can be changed
                        nameOffset = (ushort)(52 + (4 * (BitConverter.ToUInt16(b, 0xE) - 1)));

                        if (b[1] == 'T' && b[2] == 'X') { //BTX0
#if false
                            nameOffset += 0xEC;
#else
                            destName = fileNameOnly;
#endif
                        }

                        string nameRead = Encoding.UTF8.GetString(DSUtils.ReadFromFile(f.FullName, nameOffset, 16)).TrimEnd(new char[] { (char)0 });
                        
                        if (nameRead.Length <= 0 || nameRead.IndexOfAny(Path.GetInvalidPathChars()) >= 0 ) {
                            destName = fileNameOnly; //Filename can't be changed, only extension
                        } else {
                            destName += nameRead;
                        }

                        destName += ".ns";
                        for (int i = 0; i < 3; i++) {
                            magic += Char.ToLower((char)b[i]);
                        }
                    } else {
                        destName = fileNameOnly + ".";
                        byte offset = 0;

                        if (b[5] == 'R' && b[8] == 'N') { 
                            offset = 5;
                        }

                        for (int i = 0; i < 4; i++) {
                            magic += Char.ToLower((char)b[offset + i]);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(magic) || !magic.All(char.IsLetterOrDigit)) {
                        continue;
                    }

                    destName += magic;

                    if (string.IsNullOrWhiteSpace(destName)) {
                        continue;
                    }

                    destName = MakeUniqueName(destName, fileNameOnly = null, dirNameOnly);
                    File.Move(f.FullName, Path.Combine(Path.GetDirectoryName(f.FullName), Path.GetFileName(destName)));

                    enumerationFile.Add(Path.GetFileName(destName));
                }

                if (enumerationFile.Count > initialLength) {
                    MessageBox.Show("Files inside folder \"" + d.FullName + "\" have been renamed according to their contents.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DialogResult response = MessageBox.Show("Do you want to save a file enumeration list?", "Waiting for user", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (response.Equals(DialogResult.Yes)) {
                        MessageBox.Show("Choose where to save the output list file.", "Name your list file", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        SaveFileDialog sf = new SaveFileDialog {
                            Filter = "List File (*.txt; *.list)|*.txt;*.list",
                            FileName = d.Name + ".list"
                        };
                        if (sf.ShowDialog(this) != DialogResult.OK) {
                            MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        File.WriteAllLines(sf.FileName, enumerationFile);
                        MessageBox.Show("List file saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                } else {
                    MessageBox.Show("No file content could be recognized.", "Operation terminated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } else {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void fromFolderContentsToolStripMenuItem_Click(object sender, EventArgs e) {
            (DirectoryInfo d, FileInfo[] files) dirData = OpenNonEmptyDir(title: "Folder-Based List Builder");
            DirectoryInfo d = dirData.d;
            FileInfo[] filePaths = dirData.files;

            if (d == null || filePaths == null) {
                return;
            }

            MessageBox.Show("Choose where to save the output list file.", "Name your list file", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SaveFileDialog sf = new SaveFileDialog {
                Filter = "List File (*.txt; *.list)|*.txt;*.list",
                FileName = d.Name + ".list"
            };
            if (sf.ShowDialog(this) != DialogResult.OK) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            File.WriteAllLines(sf.FileName, new string[] {
                "#============================================================================",
                "# File enumeration definition for folder " + "\"" + d.Name + "\"",
                "#============================================================================"
            });
            File.AppendAllLines(sf.FileName, filePaths.Select(f => f.Name).ToArray());

            MessageBox.Show("List file saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void enumBasedListBuilderToolStripButton_Click(object sender, EventArgs e) {
            MessageBox.Show("Pick a C Enum File [with entries on different lines].", "Enum-Based List Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);

            OpenFileDialog of = new OpenFileDialog {
                Filter = "Any Text File(*.*)|*.*"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try {
                Dictionary<int, string> entries = new Dictionary<int, string>();

                string[] cFileLines = File.ReadAllLines(of.FileName);
                cFileLines = cFileLines.Select(x => x.Trim()).ToArray();

                int enumStartLine;
                for (enumStartLine = 0; enumStartLine < cFileLines.Length; enumStartLine++) {
                    if (cFileLines[enumStartLine].Replace(" ", "").Contains("enum{")) {
                        break;
                    }
                }

                if (cFileLines.Length - 1 == enumStartLine) {
                    MessageBox.Show("Abrupt termination of enum file.\nAborting.", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int terminationLine;
                for (terminationLine = enumStartLine + 1; terminationLine < cFileLines.Length; terminationLine++) {
                    if (cFileLines[terminationLine].Replace(" ", "").Contains("};")) {
                        break;
                    }
                }

                if (terminationLine >= cFileLines.Length - 1) {
                    MessageBox.Show("Enum file is malformed.\nAborting", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                if (terminationLine - enumStartLine <= 2) {
                    MessageBox.Show("This utility needs at least 2 enum entries.\nAborting.", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                int indexFirstDifferentChar = cFileLines[enumStartLine + 1].Zip(cFileLines[enumStartLine + 2], (char1, char2) => char1 == char2).TakeWhile(b => b).Count();
                int lastCommonUnderscore = cFileLines[enumStartLine + 1].Substring(0, indexFirstDifferentChar).LastIndexOf('_');

                int lastNumber = 0;

                MessageBox.Show("Choose where to save the output list file.", "Name your list file", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string shortFileName = Path.GetFileNameWithoutExtension(of.FileName);

                SaveFileDialog sf = new SaveFileDialog {
                    Filter = "List File (*.txt; *.list)|*.txt;*.list",
                    FileName = shortFileName + ".list"
                };
                if (sf.ShowDialog(this) != DialogResult.OK) {
                    MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                for (int s = enumStartLine + 1; s < terminationLine; s++) {
                    string withoutComment;

                    int indexOfComment = cFileLines[s].IndexOf("//");
                    if (indexOfComment > 0) {
                        withoutComment = cFileLines[s].Substring(0, indexOfComment);
                    } else {
                        withoutComment = cFileLines[s];
                    }

                    string differentSubstring = withoutComment.Substring(lastCommonUnderscore + 1).Trim().Replace(",", "");
                    int indexOfEquals = differentSubstring.LastIndexOf('=');

                    string entry = differentSubstring.Substring(0, indexOfEquals).Trim();
                    if (indexOfEquals > 0) {
                        string numstr = differentSubstring.Substring(indexOfEquals + 1);
                        string[] split = numstr.Split(new char[] { ' ' }, options: StringSplitOptions.RemoveEmptyEntries);

                        if (split.Length > 1) {
                            throw new Exception();
                        }

                        lastNumber = int.Parse(split[0]);
                    }

                    int posOfUnderscore = entry.LastIndexOf('_');
                    if (posOfUnderscore >= 0) {
                        entry = entry.Remove(posOfUnderscore, 1).Insert(posOfUnderscore, ".");
                    }

                    entries.Add(lastNumber, entry);
                    lastNumber++;
                }

                IEnumerable<KeyValuePair<int, string>> sortedEntries = entries.OrderBy(kvp => kvp.Key);

                File.WriteAllLines(sf.FileName, new string[] {
                    "#============================================================================",
                    "# File enumeration definition based on " + "\"" + shortFileName + "\"",
                    "#============================================================================"
                });
                File.AppendAllLines(sf.FileName, sortedEntries.Select(kvp => kvp.Value));

                MessageBox.Show("List file saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show("The input enum file couldn't be read correctly.\nNo output file has been written." +
                    "\n\nAborting.", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Details: " + ex.Message, "Failure details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string MakeUniqueName(string fileName, string fileNameOnly = null, string dirNameOnly = null, string extension = null) {
            if (fileNameOnly == null) {
                fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            }
            if (dirNameOnly == null) {
                dirNameOnly = Path.GetDirectoryName(fileName);
            }
            if (extension == null) {
                extension = Path.GetExtension(fileName);
            }

            int append = 1;

            while (File.Exists(Path.Combine(dirNameOnly, fileName)) ) {
                string tmp = fileNameOnly + "(" + (append++) + ")";
                fileName = Path.Combine(dirNameOnly, tmp + extension);
            }
            return fileName;
        }

        private (DirectoryInfo, FileInfo[]) OpenNonEmptyDir(DirectoryInfo d = null, string title = "Waiting for user") {
            /*==================================================================*/
            if (d == null) {
                MessageBox.Show("Choose a source folder.", title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                CommonOpenFileDialog sourceDirDialog = new CommonOpenFileDialog {
                    IsFolderPicker = true,
                    Multiselect = false
                };

                if (sourceDirDialog.ShowDialog() != CommonFileDialogResult.Ok) {
                    MessageBox.Show("Operation cancelled.", "User discarded operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return (null, null);
                }

                d = new DirectoryInfo(sourceDirDialog.FileName);
            }

            FileInfo[] tempfiles = d.GetFiles();
            FileInfo[] files = tempfiles.OrderBy(n => System.Text.RegularExpressions.Regex.Replace(n.Name, @"\d+", e => e.Value.PadLeft(tempfiles.Length.ToString().Length, '0'))).ToArray();

            if (files.Length <= 0) {
                MessageBox.Show("Folder " + "\"" + d.FullName + "\"" + " is empty.\nCan't proceed.", "Invalid folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (null, null);
            };

            return (d, files);
        }

        private void simpleToolStripMenuItem_MouseDown(object sender, MouseEventArgs e) {
            ToolStripMenuItem tsmi = (sender as ToolStripMenuItem);
            SetMenuLayout((byte)tsmi.GetCurrentParent().Items.IndexOf(tsmi));
        }

        private void SetMenuLayout(byte layoutStyle) {
            Console.WriteLine("Setting menuLayout to" + layoutStyle);

            IList list = menuViewToolStripMenuItem.DropDownItems;
            for (int i = 0; i < list.Count; i++) {
                (list[i] as ToolStripMenuItem).Checked = (i == layoutStyle);
            }

            Properties.Settings.Default.menuLayout = layoutStyle;

            switch (layoutStyle) {
                case 0:
                    buildNarcFromFolderToolStripButton.Visible = false;
                    unpackNARCtoFolderToolStripButton.Visible = false;
                    separator_afterNarcUtils.Visible = false;

                    listBasedBatchRenameToolStripButton.Visible = false;
                    contentBasedBatchRenameToolStripButton.Visible = false;
                    separator_afterRenameUtils.Visible = false;

                    enumBasedListBuilderToolStripButton.Visible = false;
                    folderBasedListBuilderToolStriButton.Visible = false;
                    separator_afterListUtils.Visible = false;

                    nsbmdAddTexButton.Visible = false;
                    nsbmdRemoveTexButton.Visible = false;
                    nsbmdExportTexButton.Visible = false;
                    separator_afterNsbmdUtils.Visible = false;

                    wildEditorButton.Visible = false;
                    romToolboxToolStripButton.Visible = false;
                    break;
                case 1:
                    buildNarcFromFolderToolStripButton.Visible = false;
                    unpackNARCtoFolderToolStripButton.Visible = false;
                    separator_afterNarcUtils.Visible = false;

                    listBasedBatchRenameToolStripButton.Visible = false;
                    contentBasedBatchRenameToolStripButton.Visible = false;
                    separator_afterRenameUtils.Visible = false;

                    enumBasedListBuilderToolStripButton.Visible = false;
                    folderBasedListBuilderToolStriButton.Visible = false;
                    separator_afterListUtils.Visible = false;

                    nsbmdAddTexButton.Visible = true;
                    nsbmdRemoveTexButton.Visible = true;
                    nsbmdExportTexButton.Visible = true;
                    separator_afterNsbmdUtils.Visible = true;

                    wildEditorButton.Visible = true;
                    romToolboxToolStripButton.Visible = true;
                    break;
                case 2:
                default:
                    foreach (ToolStripItem c in mainToolStrip.Items) {
                        c.Visible = true;
                    }
                    break;
            }
        }

        //Locate File - buttons
        public static void ExplorerSelect(string path) {
            if (File.Exists(path)) {
                Process.Start("explorer.exe", "/select" + "," + "\"" + path + "\"");
            }
        }

        private void locateCurrentMatrixFile_Click(object sender, EventArgs e) {
            ExplorerSelect(Path.Combine(gameDirs[DirNames.matrices].unpackedDir, selectMatrixComboBox.SelectedIndex.ToString("D4")));
        }

        private void locateCurrentMapBin_Click(object sender, EventArgs e) {
            ExplorerSelect(Path.Combine(gameDirs[DirNames.maps].unpackedDir, selectMapComboBox.SelectedIndex.ToString("D4")));
        }

        private void locateCurrentNsbtx_Click(object sender, EventArgs e) {
            if (mapTilesetRadioButton.Checked) {
                ExplorerSelect(Path.Combine(gameDirs[DirNames.mapTextures].unpackedDir, texturePacksListBox.SelectedIndex.ToString("D4")));
            } else {
                ExplorerSelect(Path.Combine(gameDirs[DirNames.buildingTextures].unpackedDir, texturePacksListBox.SelectedIndex.ToString("D4")));
            }
        }

        private void locateCurrentAreaData_Click(object sender, EventArgs e) {
            ExplorerSelect(Path.Combine(gameDirs[DirNames.areaData].unpackedDir, selectAreaDataListBox.SelectedIndex.ToString("D4")));
        }
        private void locateCurrentEvFile_Click(object sender, EventArgs e) {
            ExplorerSelect(Path.Combine(gameDirs[DirNames.eventFiles].unpackedDir, selectEventComboBox.SelectedIndex.ToString("D4")));
        }
        private void locateCurrentScriptFile_Click(object sender, EventArgs e) {
            ExplorerSelect(Path.Combine(gameDirs[DirNames.scripts].unpackedDir, selectScriptFileComboBox.SelectedIndex.ToString("D4")));
        }
        private void locateCurrentTextArchive_Click(object sender, EventArgs e) {
            ExplorerSelect(Path.Combine(gameDirs[DirNames.textArchives].unpackedDir, selectTextFileComboBox.SelectedIndex.ToString("D4")));
        }
    }
}