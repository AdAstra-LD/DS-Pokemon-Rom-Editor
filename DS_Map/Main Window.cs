using DSPRE.Editors;
using DSPRE.Editors.BtxEditor;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using Microsoft.WindowsAPICodePack.Dialogs;
using NarcAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DSPRE.EditorPanels;
using static DSPRE.Helpers;
using static DSPRE.RomInfo;
namespace DSPRE
{


    public partial class MainProgram : Form
    {
        public MainProgram()
        {


#if DEBUG
            AppLogger.Initialize(this, minLevel: LogLevel.Debug);
#else
            AppLogger.Initialize(this, minLevel: LogLevel.Info);
#endif

            AppLogger.Info("=== Application started. === ");

            SettingsManager.Load();

            // Updates can't be checked if the application is not installed, hence the !debug
#if !DEBUG
            if(SettingsManager.Settings.automaticallyCheckForUpdates)
            {
                try
                {
                    Helpers.CheckForUpdates();
                }
                catch
                {
                    AppLogger.Error("Failed to check for updates.");
                    MessageBox.Show("Failed to check for updates.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

#endif
            InitializeComponent();
            Program.SetupDatabase();

            EditorPanels.Initialize(this);
            Helpers.Initialize(this);
            WireEditorsPopout();

            SetMenuLayout(SettingsManager.Settings.menuLayout); //Read user settings for menu layout
            Text = "DS Pokémon Rom Editor Reloaded " + GetDSPREVersion();

            string romFolder = SettingsManager.Settings.openDefaultRom;
            if (romFolder != string.Empty)
            {
                AppLogger.Info($"Detected stored ROM folder: {romFolder}");

                if (!SettingsManager.Settings.neverAskForOpening)
                {
                    AppLogger.Debug("Prompting user to confirm auto-opening the ROM folder.");

                    ReopenProjectConfirmation confirmOpen = new ReopenProjectConfirmation();
                    if (confirmOpen.ShowDialog() == DialogResult.No)
                    {
                        AppLogger.Info("User declined to reopen the previous ROM project.");
                        return;
                    }

                    AppLogger.Info("User confirmed reopening the previous ROM project.");
                }
                else
                {
                    AppLogger.Info("Auto-opening ROM without asking the user (neverAskForOpening is enabled).");
                }

                AppLogger.Info("Opening ROM project from saved folder.");
                OpenRomFromFolder(romFolder);
            }
            else
            {
                AppLogger.Debug("No stored ROM folder found on startup.");
            }

        }

        #region Program Window

        #region Variables
        public bool iconON = false;

        /* ROM Information */
        public static string gameCode;
        public static byte revisionByte;
        public RomInfo romInfo;
        public Dictionary<ushort /*evFile*/, ushort /*header*/> eventToHeader = new Dictionary<ushort, ushort>();

        #endregion

        #region Subroutines

        private void SetMenuLayout(byte layoutStyle)
        {
            AppLogger.Debug("Setting menuLayout to" + layoutStyle);

            IList list = menuViewToolStripMenuItem.DropDownItems;
            for (int i = 0; i < list.Count; i++)
            {
                (list[i] as ToolStripMenuItem).Checked = (i == layoutStyle);
            }

            SettingsManager.Settings.menuLayout = layoutStyle;

            switch (layoutStyle)
            {
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
                    buildNarcFromFolderToolStripButton.Visible = true;
                    unpackNARCtoFolderToolStripButton.Visible = true;
                    separator_afterNarcUtils.Visible = true;

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
                case 3:
                default:
                    foreach (ToolStripItem c in mainToolStrip.Items)
                    {
                        c.Visible = true;
                    }
                    break;
            }
        }
        private void MainProgram_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall && MessageBox.Show("Are you sure you want to quit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            SettingsManager.Save();
        }

        private void MainProgram_Shown(object sender, EventArgs e)
        {
            if (!DetectRequiredTools())
            {
                BeginInvoke(new Action(() => Application.Exit()));
                return;
            }
        }

        private void PaintGameIcon(object sender, PaintEventArgs e)
        {
            if (iconON)
            {
                FileStream banner;

                try
                {
                    banner = File.OpenRead(RomInfo.workDir + @"banner.bin");
                }
                catch (FileNotFoundException)
                {
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

                for (int i = 0; i < 16; i++)
                {
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
                for (int o = 0; o < 4; o++)
                {
                    for (int a = 0; a < 4; a++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            int iconX = xTile;

                            for (int counter = 0; counter < 4; counter++)
                            {
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

        public void SetupScriptEditor()
        {
            /* Extract essential NARCs sub-archives*/
            Helpers.statusLabelMessage("Setting up Script Editor...");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.scripts }); //12 = scripts Narc Dir

            EditorPanels.scriptEditor.selectScriptFileComboBox.Items.Clear();
            int scriptCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.scripts].unpackedDir).Length;
            for (int i = 0; i < scriptCount; i++)
            {
                EditorPanels.scriptEditor.selectScriptFileComboBox.Items.Add("Script File " + i);
            }

            EditorPanels.scriptEditor.UpdateScriptNumberCheckBox((NumberStyles)SettingsManager.Settings.scriptEditorFormatPreference);
            EditorPanels.scriptEditor.selectScriptFileComboBox.SelectedIndex = 0;
            Helpers.statusLabelMessage();
        }

        /// <summary>
        /// Check if extracted data for the ROM exists, and ask user if they want to load it.
        /// </summary>
        /// <returns>
        /// -1 - Do nothing, no data found
        ///  0 - User wants to abort loading
        ///  1 - User wants to load existing data
        ///  2 - User wants to re-extract data
        /// </returns>
        private int UnpackRomCheckUserChoice(string romDir)
        {
            switch (DSUtils.GetFolderType(romDir))
            {
                case -1:
                    return -1; // Do nothing case
                case 0:
                    MessageBox.Show("Extracted data of this ROM is not yet supported.\n" +
                        "Loading will be aborted.", "Unsupported ROM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0; //user wants to abort loading
                case 1:
                    DialogResult d2 = MessageBox.Show("Extracted data of this ROM has been found.\n" +
                        "Do you want to load it?", "Extracted data detected", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (d2 == DialogResult.Cancel)
                    {
                        return 0; //user wants to abort loading
                    }
                    else if (d2 == DialogResult.Yes)
                    {
                        return 1; //user wants to load data
                    }

                    DialogResult nd2 = MessageBox.Show("All data of this ROM will be re-extracted. Proceed?\n",
                        "Existing data will be deleted", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (nd2 == DialogResult.Yes)
                    {
                        return 2; //user wants to re-extract data
                    }

                    return 0; //user wants to abort loading
                default:
                    return -1; // Do nothing case

            }
        }
        #endregion

        private void romToolBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PatchToolboxDialog window = new PatchToolboxDialog())
            {
                window.ShowDialog();
                if (PatchToolboxDialog.flag_standardizedItems && EditorPanels.eventEditor.eventEditorIsReady)
                {
                    EditorPanels.eventEditor.selectEventComboBox_SelectedIndexChanged(null, null);
                    UpdateItemComboBox(RomInfo.GetItemNames());
                }
                if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied)
                {
                    EditorPanels.headerEditor.addHeaderBTN.Enabled = true;
                    EditorPanels.headerEditor.removeLastHeaderBTN.Enabled = true;
                }
            }
        }
        public void UpdateItemComboBox(string[] itemNames)
        {
            if (EditorPanels.eventEditor.itemComboboxIsUpToDate)
            {
                return;
            }
            EditorPanels.eventEditor.itemsSelectorHelpBtn.Visible = false;
            EditorPanels.eventEditor.owItemComboBox.Size = new Size(new Point(EditorPanels.eventEditor.owItemComboBox.Size.Width + 30, EditorPanels.eventEditor.owItemComboBox.Size.Height));
            EditorPanels.eventEditor.owItemComboBox.Items.Clear();
            EditorPanels.eventEditor.owItemComboBox.Items.AddRange(itemNames);
            EditorPanels.eventEditor.OWTypeChanged(null, null);
            EditorPanels.eventEditor.itemComboboxIsUpToDate = true;
        }
        private void scriptCommandsDatabaseToolStripButton_Click(object sender, EventArgs e)
        {
            OpenCommandsDatabase(RomInfo.ScriptCommandNamesDict, RomInfo.ScriptCommandParametersDict, RomInfo.ScriptActionNamesDict, RomInfo.ScriptComparisonOperatorsDict);
        }
        private void nsbmdExportTexButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = MapFile.TexturedNSBMDFilter
            };
            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            byte[] modelFile = DSUtils.ReadFromFile(of.FileName);
            if (NSBUtils.CheckNSBMDHeader(modelFile) == NSBUtils.NSBMD_DOESNTHAVE_TEXTURE)
            {
                MessageBox.Show("This NSBMD file is untextured.", "No textures to extract", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //============================================================
            MessageBox.Show("Choose where to save the textures.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SaveFileDialog texSf = new SaveFileDialog
            {
                Filter = "NSBTX File(*.nsbtx)|*.nsbtx",
                FileName = Path.GetFileNameWithoutExtension(of.FileName)
            };
            if (texSf.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            DSUtils.WriteToFile(texSf.FileName, NSBUtils.GetTexturesFromTexturedNSBMD(modelFile));
            MessageBox.Show("The textures of " + of.FileName + " have been extracted and saved.", "Textures saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void nsbmdRemoveTexButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = MapFile.TexturedNSBMDFilter
            };

            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            byte[] modelFile = DSUtils.ReadFromFile(of.FileName);
            if (NSBUtils.CheckNSBMDHeader(modelFile) == NSBUtils.NSBMD_DOESNTHAVE_TEXTURE)
            {
                MessageBox.Show("This NSBMD file is already untextured.", "No textures to remove", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string extramsg = "";
            DialogResult d = MessageBox.Show("Would you like to save the removed textures to a file?", "Save textures?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (d.Equals(DialogResult.Yes))
            {

                MessageBox.Show("Choose where to save the textures.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SaveFileDialog texSf = new SaveFileDialog
                {
                    Filter = "NSBTX File(*.nsbtx)|*.nsbtx",
                    FileName = Path.GetFileNameWithoutExtension(of.FileName)
                };

                if (texSf.ShowDialog() == DialogResult.OK)
                {
                    DSUtils.WriteToFile(texSf.FileName, NSBUtils.GetTexturesFromTexturedNSBMD(modelFile));
                    extramsg = " exported and";
                }
            }

            //============================================================
            MessageBox.Show("Choose where to save the untextured model.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = "Untextured NSBMD File(*.nsbmd)|*.nsbmd",
                FileName = Path.GetFileNameWithoutExtension(of.FileName) + "_untextured"
            };
            if (sf.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            DSUtils.WriteToFile(sf.FileName, NSBUtils.GetModelWithoutTextures(modelFile));
            MessageBox.Show("Textures correctly" + extramsg + " removed!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void nsbmdAddTexButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = MapFile.UntexturedNSBMDFilter
            };
            if (of.ShowDialog(this) != DialogResult.OK)
                return;

            byte[] modelFile = File.ReadAllBytes(of.FileName);
            if (NSBUtils.CheckNSBMDHeader(modelFile) == NSBUtils.NSBMD_HAS_TEXTURE)
            {
                DialogResult d = MessageBox.Show("This NSBMD file is already textured.\nDo you want to overwrite its textures?", "Textures found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (d.Equals(DialogResult.No))
                {
                    return;
                }
            }

            MessageBox.Show("Select the new NSBTX texture file.", "Choose NSBTX", MessageBoxButtons.OK, MessageBoxIcon.Information);

            OpenFileDialog openNsbtx = new OpenFileDialog
            {
                Filter = "NSBTX File(*.nsbtx)|*.nsbtx"
            };
            if (openNsbtx.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            byte[] textureFile = File.ReadAllBytes(openNsbtx.FileName);


            //============================================================
            MessageBox.Show("Choose where to save the new textured model.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string texturedPath = Path.GetFileNameWithoutExtension(of.FileName);
            if (texturedPath.Contains("_untextured"))
            {
                texturedPath = texturedPath.Substring(0, texturedPath.Length - "_untextured".Length);
            }

            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = MapFile.TexturedNSBMDFilter,
                FileName = Path.GetFileNameWithoutExtension(of.FileName) + "_textured"
            };

            if (sf.ShowDialog(this) != DialogResult.OK)
                return;

            DSUtils.WriteToFile(sf.FileName, NSBUtils.BuildNSBMDwithTextures(modelFile, textureFile), fmode: FileMode.Create);
            MessageBox.Show("Textures correctly written to NSBMD file.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void OpenCommandsDatabase(Dictionary<ushort, string> namesDict, Dictionary<ushort, byte[]> paramsDict, Dictionary<ushort, string> actionsDict,
            Dictionary<ushort, string> comparisonOPsDict)
        {
            Helpers.statusLabelMessage("Setting up Commands Database. Please wait...");
            Update();
            CommandsDatabase form = new CommandsDatabase(namesDict, paramsDict, actionsDict, comparisonOPsDict);
            form.Show();
            Helpers.statusLabelMessage();
        }
        private void headerSearchToolStripButton_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectedIndex = 0; //Select Header Editor
            using (HeaderSearch h = new HeaderSearch(ref EditorPanels.headerEditor.internalNames, EditorPanels.headerEditor.headerListBox, statusLabel))
            {
                h.ShowDialog();
            }
        }
        private void advancedHeaderSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            headerSearchToolStripButton_Click(null, null);
        }
        private void buildingEditorButton_Click(object sender, EventArgs e)
        {
            unpackBuildingEditorNARCs();

            BuildingEditor editor = new BuildingEditor(romInfo);
            editor.Show();
        }
        private void unpackBuildingEditorNARCs(bool forceUnpack = false)
        {
            toolStripProgressBar.Visible = true;

            Helpers.statusLabelMessage("Attempting to unpack Building Editor NARCs... Please wait. This might take a while");
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

            if (forceUnpack)
            {
                DSUtils.ForceUnpackNarcs(toUnpack);

                if (RomInfo.gameFamily == GameFamilies.HGSS)
                {
                    DSUtils.ForceUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });// Last = interior buildings dir
                }
            }
            else
            {
                DSUtils.TryUnpackNarcs(toUnpack);

                if (RomInfo.gameFamily == GameFamilies.HGSS)
                {
                    DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });
                }
            }

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;
            Helpers.statusLabelMessage();
            Update();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "DS Pokémon ROM Editor Reloaded by AdAstra, Mixone, Kuha, Yako & Kalaay" 
                + Environment.NewLine + "Version " + GetDSPREVersion() 
                + Environment.NewLine
                + Environment.NewLine + "Based on Nømura's DS Pokémon ROM Editor 1.0.4."
                + Environment.NewLine + "Largely inspired by Markitus95's \"Spiky's DS Map Editor\" (SDSME), from which certain assets were also reused." 
                + Environment.NewLine + "Credits go to Markitus, Ark, Zark, Florian, and everyone else who deserves credit for SDSME." 
                + Environment.NewLine
                + Environment.NewLine + "Special thanks to Trifindo, Mikelan98, JackHack96, Pleonex and BagBoy."
                + Environment.NewLine + "Their help, research and expertise in many fields of NDS ROM Hacking made the development of this tool possible.";

            MessageBox.Show(message, "About...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void loadRom_Click(object sender, EventArgs e)
        {
            OpenFileDialog openRom = new OpenFileDialog
            {
                Filter = DSUtils.NDSRomFilter
            }; // Select ROM
            if (openRom.ShowDialog(this) != DialogResult.OK)
            {
                AppLogger.Debug("User cancelled the ROM loading dialog.");
                return;
            }

            // Validate path and check for OneDrive
            if (!ValidateFilePath(openRom.FileName))
            {
                AppLogger.Warn("ROM path validation failed. Loading Aborted!");
                return;
            }

            string workDir = DSUtils.WorkDirPathFromFile(openRom.FileName);
            AppLogger.Info(workDir + " will be used as the working directory for the ROM.");

            int userchoice = UnpackRomCheckUserChoice(workDir);
            switch (userchoice)
            {
                case -1:
                    if (!DSUtils.UnpackRom(openRom.FileName, workDir))
                    {
                        AppLogger.Error($"Unpacking of ROM \"{openRom.FileName}\" has failed!");
                        Helpers.statusLabelError($"Unpacking of ROM ROM \"{openRom.FileName}\" has failed");
                        Update();
                        return; // Unpacking failed, abort loading
                    }
                    break;
                case 0:
                    AppLogger.Info("User chose to abort loading the ROM.");
                    Helpers.statusLabelMessage("Loading aborted");
                    Update();
                    return;
                case 1:
                    AppLogger.Info("User chose to load existing data from " + workDir);
                    Application.DoEvents();
                    break;
                case 2:
                    AppLogger.Info("User chose to re-extract data from " + openRom.FileName);
                    Application.DoEvents();
                    Helpers.statusLabelMessage("Deleting old data...");
                    Update();

                    try
                    {
                        Directory.Delete(workDir, true);
                        AppLogger.Debug(workDir + " was deleted successfully.");
                    }
                    catch (IOException)
                    {
                        AppLogger.Error("Concurrent access detected while trying to delete " + workDir);
                        MessageBox.Show("Concurrent access detected: \n" + workDir +
                            "\nMake sure no other process is using the extracted ROM folder while DSPRE is running.", "Concurrent Access", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!DSUtils.UnpackRom(openRom.FileName, workDir))
                    {
                        AppLogger.Error($"Unpacking of ROM \"{openRom.FileName}\" has failed!");
                        Helpers.statusLabelError("Unpacking of ROM \"" + openRom.FileName + "\" has failed");
                        Update();
                        return; // Unpacking failed, abort loading
                    }

                    break;
            }

            AppLogger.Info("ROM unpacked successfully, proceeding to open the ROM from " + workDir);
            Update();

            OpenRomFromFolder(workDir);

        }

        private bool ValidateFilePath(string fileName)
        {
            // Empty file name check
            if (string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("File path is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string fullPath = Path.GetFullPath(fileName);

            // File / directory existence check
            if (!File.Exists(fileName) && !Directory.Exists(fileName))
            {
                MessageBox.Show("The specified file at path " + fullPath + " does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // One drive check
            if (fullPath.ToLower().Contains("onedrive"))
            {
                MessageBox.Show("OneDrive was detected in the path. DSPRE is not compatible with OneDrive. " +
                    "Please move the ROM and unpacked folder to the same local drive DSPRE is stored on.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private bool DetectAndHandleWSL(string fileName)
        {
            string fullPath = Path.GetFullPath(fileName);
            if (!fullPath.ToLower().Contains("wsl."))
            {
                return false; // No WSL detected, proceed normally
            }

            MessageBox.Show("WSL was detected in the path. " +
                "You may experience some slow-downs, especially on WSL2.\n" +
                "If the slow-down is too excessive consider moving your files to the Windows filesystem.",
                "WSL Detected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AppLogger.Info("WSL detected in the path: " + fullPath);

            return true;
        }
        private bool DetectRequiredTools()
        {

            bool toolsMissing = false;
            List<string> missingToolsList = new List<string>();

            if (!File.Exists(@"Tools\ndstool.exe"))
            {
                toolsMissing = true;
                missingToolsList.Add("ndstool.exe");
            }
            if (!File.Exists(@"Tools\blz.exe"))
            {
                toolsMissing = true;
                missingToolsList.Add("blz.exe");
            }
            if (!File.Exists(@"Tools\apicula.exe"))
            {
                toolsMissing = true;
                missingToolsList.Add("apicula.exe");
            }

            if (toolsMissing)
            {
                string message = "The following required tools are missing from the DSPRE Tools folder:\n-" +
                    string.Join("\n-", missingToolsList) + "\n\n" +
                    "Please ensure that the Tools folder is intact and contains all necessary files.\n" +
                    "Common causes for this issue are:\n" +
                    "   - DSPRE is stored in OneDrive\n" +
                    "   - You opened DSPRE from Windows search\n" +
                    "   - Your Antivirus software has removed critical files\n\n" +
                    "DSPRE will now close.";
                MessageBox.Show(message, "Missing Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // If the program somehow doesn't close after this, we also disable the buttons and hope this is enough to dissuade the user from using it.
                this.loadRomButton.Enabled = false; // Disable Load ROM button
                this.readDataFromFolderButton.Enabled = false; // Disable Read Data from Folder button
                this.fileToolStripMenuItem.DropDownItems[0].Enabled = false; // Disable Open ROM menu item
                this.fileToolStripMenuItem.DropDownItems[1].Enabled = false; // Disable Open Folder menu item

                return false;
            }

            return true;
        }

        private void CheckROMLanguage()
        {
            versionLabel.Visible = true;
            languageLabel.Visible = true;

            versionLabel.Text = RomInfo.gameVersion.ToString() + " " + "[" + RomInfo.romID + "]";
            languageLabel.Text = "Lang: " + RomInfo.gameLanguage;

            if (RomInfo.gameLanguage == GameLanguages.English)
            {
                if (revisionByte == 0x0A)
                {
                    languageLabel.Text += " [Europe]";
                }
                else
                {
                    languageLabel.Text += " [America]";
                }
            }
        }

        private void readDataFromFolderButton_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog romFolder = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            };
            if (romFolder.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            string fileName = romFolder.FileName;
            OpenRomFromFolder(fileName);

        }

        private void OpenRomFromFolder(string romFolderPath)
        {
            AppLogger.Info($"Attempting to open ROM from folder: {romFolderPath}");

            // Validate path and check for OneDrive
            if (!ValidateFilePath(romFolderPath))
            {
                AppLogger.Warn("ROM path validation failed. Possibly invalid or on a restricted (OneDrive).");
                return;
            }
            
            DetectAndHandleWSL(romFolderPath);

            if (DSUtils.GetFolderType(romFolderPath) == -1)
            {
                AppLogger.Error("The selected folder does not contain a valid ROM folder structure.");
                MessageBox.Show("The selected folder does not contain a valid ROM folder structure.", "Invalid Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Invalid folder, abort loading
            }

            SetupROMLanguage(Path.Combine(romFolderPath, "header.bin"));
            AppLogger.Debug("ROM language setup completed.");

            romInfo = new RomInfo(gameCode, romFolderPath);

            if (string.IsNullOrWhiteSpace(RomInfo.romID) || string.IsNullOrWhiteSpace(RomInfo.projectName))
            {
                AppLogger.Error("ROM ID or filename is empty after initialization. Aborting.");
                return;
            }

            AppLogger.Info($"ROM loaded successfully: ID = {RomInfo.romID}, Project Name = {RomInfo.projectName}");

            CheckROMLanguage();
            AppLogger.Debug("ROM language checked and applied.");

            iconON = true;
            gameIcon.Refresh();  // Paint game icon
            AppLogger.Debug("Game icon refreshed.");

            if (!CheckAndDecompressARM9())
            {
                AppLogger.Error("ARM9 decompression failed. Aborting.");
                return;
            }

            ReadROMInitData();
            AppLogger.Info("ROM initialization data loaded.");
        }


        private void SetupROMLanguage(string headerPath)
        {
            using (DSUtils.EasyReader br = new DSUtils.EasyReader(headerPath, 0xC))
            {
                gameCode = Encoding.UTF8.GetString(br.ReadBytes(4));
                br.BaseStream.Position = 0x1E;
                revisionByte = br.ReadByte();
            }
        }

        private bool CheckAndDecompressARM9()
        {
            if (!ARM9.CheckCompressionMark())
            {
                return true; // ARM9 is not compressed, proceed normally
            }

            if (!RomInfo.gameFamily.Equals(GameFamilies.HGSS))
            {
                MessageBox.Show("Unexpected compressed ARM9. It is advised that you double check the ARM9.");
                return false;
            }

            ARM9.EditSize(-12); // Fix ARM9 size before decompression

            if (!ARM9.Decompress(RomInfo.arm9Path))
            {
                MessageBox.Show("ARM9 decompression failed. The program can't proceed.\nAborting.",
                            "Error with ARM9 decompression", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            AppLogger.Info("ARM9 decompressed and size fixed.");

            return true;
        }

        private void ReadROMInitData()
        {
            /* Setup essential editors */
            EditorPanels.headerEditor.SetupHeaderEditor(this);


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
            spawnEditorToolStripMenuItem.Enabled = true;
            otherEditorsToolStripMenuItem.Enabled = true;

            scriptCommandsButton.Enabled = true;
            if (!RomInfo.gameFamily.Equals(GameFamilies.HGSS))
            {
                mainTabControl.TabPages.Remove(tabPageEncountersEditor);
            }
            else
            {
                overlayEditorToolStripMenuItem.Enabled = true;
            }

            Helpers.statusLabelMessage();
            this.Text += "  -  " + RomInfo.projectName;
        }

        private void saveRom_Click(object sender, EventArgs e)
        {
            AppLogger.Info("Saving ROM...");

            SaveFileDialog saveRom = new SaveFileDialog
            {
                Filter = DSUtils.NDSRomFilter,
                InitialDirectory = SettingsManager.Settings.exportPath
            };
            if (saveRom.ShowDialog(this) != DialogResult.OK)
            {
                AppLogger.Debug("User cancelled the Save ROM dialog.");
                return;
            }

            var dateBegin = DateTime.Now;

            Helpers.statusLabelMessage("Repacking Expanded Files...");
            Update();

            // Turn expanded folders back into binary files
            // ToDo: Better system for tracking expanded folders instead of hardcoding all of them
            if (!TextArchive.BuildRequiredBins())
            {
                MessageBox.Show("An error occurred while rebuilding text archives. Save aborted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Helpers.statusLabelMessage("Repacking NARCS...");
            Update();
            

            // Repack NARCs
            foreach (KeyValuePair<DirNames, (string packedDir, string unpackedDir)> kvp in RomInfo.gameDirs)
            {
                DirectoryInfo di = new DirectoryInfo(kvp.Value.unpackedDir);
                if (di.Exists)
                {
                    Narc.FromFolder(kvp.Value.unpackedDir).Save(kvp.Value.packedDir); // Make new NARC from folder
                }
            }

            if (ARM9.CheckCompressionMark())
            {
                Helpers.statusLabelMessage("Awaiting user response...");
                DialogResult d = MessageBox.Show("The ARM9 file of this ROM is currently uncompressed, but marked as compressed.\n" +
                    "This will prevent your ROM from working on native hardware.\n\n" +
                "Do you want to mark the ARM9 as uncompressed?", "ARM9 compression mismatch detected",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (d == DialogResult.Yes)
                {
                    ARM9.WriteBytes(new byte[4] { 0, 0, 0, 0 }, (uint)(RomInfo.gameFamily == GameFamilies.DP ? 0xB7C : 0xBB4));
                }
            }

            Helpers.statusLabelMessage("Repacking ROM...");

            if (OverlayUtils.OverlayTable.IsDefaultCompressed(1))
            {
                if (PatchToolboxDialog.overlay1MustBeRestoredFromBackup)
                {
                    OverlayUtils.RestoreFromCompressedBackup(1, EditorPanels.eventEditor.eventEditorIsReady);
                }
                else
                {
                    if (!OverlayUtils.IsCompressed(1))
                    {
                        OverlayUtils.Compress(1);
                    }
                }
            }

            if (OverlayUtils.OverlayTable.IsDefaultCompressed(RomInfo.initialMoneyOverlayNumber))
            {
                if (!OverlayUtils.IsCompressed(RomInfo.initialMoneyOverlayNumber))
                {
                    OverlayUtils.Compress(RomInfo.initialMoneyOverlayNumber);
                }
            }


            Update();

            bool success = DSUtils.RepackROM(saveRom.FileName);

            if (RomInfo.gameFamily != GameFamilies.DP && RomInfo.gameFamily != GameFamilies.Plat)
            {
                if (EditorPanels.eventEditor.eventEditorIsReady)
                {
                    if (OverlayUtils.IsCompressed(1))
                    {
                        OverlayUtils.Decompress(1);
                    }
                }
            }

            SettingsManager.Save();
            Helpers.statusLabelMessage();
            var date = DateTime.Now;
            var StringDate = Helpers.formatTime(date.Hour) + ":" + Helpers.formatTime(date.Minute) + ":" + Helpers.formatTime(date.Second);
            int timeSpent = Helpers.CalculateTimeDifferenceInSeconds(dateBegin.Hour, dateBegin.Minute, dateBegin.Second, date.Hour, date.Minute, date.Second);

            if (!success)
            {
                AppLogger.Error("An error occurred while repacking the ROM. Save failed.");
                Helpers.statusLabelError("An error occurred while repacking the ROM. Save failed. Your ROM may have been corrupted.");
                return;
            }
            AppLogger.Info($"ROM saved successfully to {saveRom.FileName} in {timeSpent} seconds.");
            Helpers.statusLabelMessage("Ready - " + StringDate + " | Build time: " + timeSpent.ToString() + "s | " + saveRom.FileName);
        }


        private void unpackAllButton_Click(object sender, EventArgs e)
        {
            Helpers.statusLabelMessage("Awaiting user response...");
            DialogResult d = MessageBox.Show("Do you wish to unpack all extracted NARCS?\n" +
                "This operation might be long and can't be interrupted.\n\n" +
                "Any unsaved changes made to the ROM in this session will be lost." +
                "\nProceed?", "About to unpack all NARCS",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes)
            {
                toolStripProgressBar.Maximum = RomInfo.gameDirs.Count;
                toolStripProgressBar.Visible = true;
                toolStripProgressBar.Value = 0;
                Helpers.statusLabelMessage("Attempting to unpack all NARCs... Be patient. This might take a while...");
                Update();

                DSUtils.ForceUnpackNarcs(Enum.GetValues(typeof(DirNames)).Cast<DirNames>().ToList());
                MessageBox.Show("Operation completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                toolStripProgressBar.Value = 0;
                toolStripProgressBar.Visible = false;

                EditorPanels.headerEditor.SetupHeaderEditor(this);
                EditorPanels.matrixEditor.SetupMatrixEditor(this);
                EditorPanels.mapEditor.SetupMapEditor(this);
                nsbtxEditor.SetupNSBTXEditor(this);
                EditorPanels.eventEditor.SetupEventEditor(this);
                SetupScriptEditor();
                textEditor.SetupTextEditor(this);
                trainerEditor.SetupTrainerEditor(this);

                Helpers.statusLabelMessage();
                Update();
            }
        }
        private void updateMapNarcsButton_Click(object sender, EventArgs e)
        {
            Helpers.statusLabelMessage("Awaiting user response...");
            DialogResult d = MessageBox.Show("Do you wish to unpack all NARC files necessary for the Building Editor ?\n" +
               "This operation might be long and can't be interrupted.\n\n" +
               "Any unsaved changes made to building models and textures in this session will be lost." +
               "\nProceed?", "About to unpack Building NARCs",
               MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes)
            {
                unpackBuildingEditorNARCs(forceUnpack: true);

                MessageBox.Show("Operation completed.", "Success",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                Helpers.statusLabelMessage();

                if (EditorPanels.mapEditor.mapEditorIsReady)
                {
                    EditorPanels.mapEditor.updateBuildingListComboBox(EditorPanels.mapEditor.interiorbldRadioButton.Checked);
                }
                Update();
            }
        }
        private void diamondAndPearlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCommandsDatabase(RomInfo.BuildCommandNamesDatabase(GameFamilies.DP), RomInfo.BuildCommandParametersDatabase(GameFamilies.DP),
                RomInfo.BuildActionNamesDatabase(GameFamilies.DP), RomInfo.BuildComparisonOperatorsDatabase(GameFamilies.DP));
        }
        private void platinumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCommandsDatabase(RomInfo.BuildCommandNamesDatabase(GameFamilies.Plat), RomInfo.BuildCommandParametersDatabase(GameFamilies.Plat),
                RomInfo.BuildActionNamesDatabase(GameFamilies.Plat), RomInfo.BuildComparisonOperatorsDatabase(GameFamilies.Plat));
        }
        private void heartGoldAndSoulSilverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCommandsDatabase(RomInfo.BuildCommandNamesDatabase(GameFamilies.HGSS), RomInfo.BuildCommandParametersDatabase(GameFamilies.HGSS),
                RomInfo.BuildActionNamesDatabase(GameFamilies.HGSS), RomInfo.BuildComparisonOperatorsDatabase(GameFamilies.HGSS));
        }
        private void manageDatabasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomScrcmdManager editor = new CustomScrcmdManager();
            editor.Show();
        }
        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e) 
        {
            if (mainTabControl.SelectedTab == headerEditorTabPage) 
            {
                headerEditor.SetupHeaderEditor(this);
            }
            else if (mainTabControl.SelectedTab == EditorPanels.matrixEditorTabPage)
            {
                matrixEditor.SetupMatrixEditor(this);
            }
            else if (mainTabControl.SelectedTab == mapEditorTabPage)
            {
                mapEditor.SetupMapEditor(this);
            }
            else if (mainTabControl.SelectedTab == EditorPanels.nsbtxEditorTabPage)
            {
                nsbtxEditor.SetupNSBTXEditor(this);
            }
            else if (mainTabControl.SelectedTab == EditorPanels.eventEditorTabPage)
            {
                eventEditor.eventOpenGlControl.MakeCurrent();
                eventEditor.SetupEventEditor(this);
            }
            else if (mainTabControl.SelectedTab == EditorPanels.textEditorTabPage)
            {
                textEditor.SetupTextEditor(this);
            }
            else if (mainTabControl.SelectedTab == EditorPanels.trainerEditorTabPage)
            {
                trainerEditor.SetupTrainerEditor(this);
            }
            else if (mainTabControl.SelectedTab == EditorPanels.tabPageTableEditor)
            {
                EditorPanels.headerEditor.resetHeaderSearch();
                tableEditor.SetupConditionalMusicTable(this);
                tableEditor.SetupBattleEffectsTables(this);
            }
            else if (mainTabControl.SelectedTab == EditorPanels.scriptEditorTabPage)
            {
                scriptEditor.SetupScriptEditor(this);
            }
            else if (mainTabControl.SelectedTab == EditorPanels.levelScriptEditorTabPage)
            {
                levelScriptEditor.SetUpLevelScriptEditor(this);
            }
            else if (mainTabControl.SelectedTab == EditorPanels.tabPageEncountersEditor)
            {
                encountersEditor.SetupEncountersEditor();
            }
            else if (mainTabControl.SelectedTab == cameraEditorTabPage)
            {
                cameraEditor.SetupCameraEditor(this);
            }
        }

        private void spawnEditorToolStripButton_Click(object sender, EventArgs e)
        {
  
            matrixEditor.SetupMatrixEditor(this);

            using (SpawnEditor ed = new SpawnEditor(EditorPanels.headerEditor.headerListBoxNames))
            {
                ed.ShowDialog();
            }
        }
        private void spawnEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            spawnEditorToolStripButton_Click(null, null);
        }
        private void wildEditorButton_Click(object sender, EventArgs e)
        {
            openWildEditor(loadCurrent: false);
        }
        public void openWildEditor(bool loadCurrent)
        {
            Helpers.statusLabelMessage("Attempting to extract Wild Encounters NARC...");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames>() { DirNames.encounters, DirNames.monIcons });

            Helpers.statusLabelMessage("Passing control to Wild Pokémon Editor...");
            Update();

            int encToOpen = loadCurrent ? (int)EditorPanels.headerEditor.wildPokeUpDown.Value : 0;

            string wildPokeUnpackedPath = gameDirs[DirNames.encounters].unpackedDir;
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    WildEditorDPPt wildEditorDppt = new WildEditorDPPt(wildPokeUnpackedPath, RomInfo.GetPokemonNames(),
                        encToOpen, EditorPanels.headerEditor.internalNames.Count);
                        wildEditorDppt.Show();
                    break;
                default:
                    WildEditorHGSS wildEditorHgss = new WildEditorHGSS(wildPokeUnpackedPath, RomInfo.GetPokemonNames(),
                        encToOpen, EditorPanels.headerEditor.internalNames.Count);
                        wildEditorHgss.Show();
                    break;
            }
            Helpers.statusLabelMessage();
        }
        #endregion

        #region Tooltrip Menu
        private void unpackToFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = "NARC File (*.narc)|*.narc|All files (*.*)|*.*"
            };
            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            Narc userfile = Narc.Open(of.FileName);
            if (userfile is null)
            {
                MessageBox.Show("The file you selected is not a valid NARC.", "Cannot proceed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("Choose where to save the NARC content.\nDSPRE will automatically make a subdirectory.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CommonOpenFileDialog narcDir = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            };

            if (narcDir.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            string finalExtractedPath = narcDir.FileName + "\\" + Path.GetFileNameWithoutExtension(of.FileName);
            userfile.ExtractToFolder(finalExtractedPath);
            MessageBox.Show("The contents of " + of.FileName + " have been extracted and saved.", "NARC Extracted", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult d = MessageBox.Show("Do you want to rename the files according to their contents?", "Waiting for user", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (d.Equals(DialogResult.Yes))
            {
                ContentBasedBatchRename(this, new DirectoryInfo(finalExtractedPath));
            }
        }

        private void buildFromFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog narcDir = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            };

            if (narcDir.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            MessageBox.Show("Choose where to save the output NARC file.", "Name your NARC file", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = "NARC File (*.narc)|*.narc",
                FileName = Path.GetFileName(narcDir.FileName)
            };
            if (sf.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            Narc.FromFolder(narcDir.FileName).Save(sf.FileName);
            MessageBox.Show("The contents of folder \"" + narcDir.FileName + "\" have been packed.", "NARC Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listBasedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (DirectoryInfo d, FileInfo[] files) dirData = OpenNonEmptyDir(title: "List-Based Batch Rename Tool");
            DirectoryInfo d = dirData.d;
            FileInfo[] files = dirData.files;

            if (d == null || files == null)
            {
                return;
            }

            /*==================================================================*/

            MessageBox.Show("Choose your enumeration text file.", "Input list file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = "List File (*.txt; *.list)|*.txt;*.list"
            };

            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            /*==================================================================*/

            const string COMMENT_CHAR = "#";
            const string ISOLATED_FOLDERNAME = "DSPRE_IsolatedFiles";

            string[] listLines = File.ReadAllLines(of.FileName);
            listLines = listLines.Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith(COMMENT_CHAR)).ToArray();

            if (listLines.Length <= 0)
            {
                MessageBox.Show("The enumeration text file you selected is empty or only contains comment lines.\nCan't proceed.", "Invalid list file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string msg = "About to process ";
            int tot;
            string extra = "";

            int diff = files.Length - listLines.Length;
            if (diff < 0)
            { //listLines.Length > files.Length 
                tot = files.Length;
                extra = "(Please note that the length of the chosen list [" + listLines.Length + " entries] " +
                    "exceeds the number of files in the folder.)" + "\n\n";
            }
            else if (diff == 0)
            { //listLines.Length == files.Length
                tot = files.Length;
            }
            else
            { // diff > 0 --> listLines.Length < files.Length
                tot = listLines.Length;
                extra = "(Please note that there aren't enough entries in the list to rename all files in the chosen folder.\n" +
                    diff + " file" + (diff > 1 ? "s" : "") + " won't be renamed.)" + "\n\n";
            }

            msg += tot + " file" + (tot > 1 ? "s" : "");

            DialogResult dr = MessageBox.Show(msg + " from the input folder (taken in ascending order), " +
                "according to the list file you provided.\n" +
                "If a destination file already exists, DSPRE will append a number to its name.\n\n" + extra +
                "Do you want to proceed?", "Confirm operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr.Equals(DialogResult.Yes))
            {
                int i;
                for (i = 0; i < tot; i++)
                {
                    FileInfo f = files[i];
                    string destName = Path.GetDirectoryName(f.FullName) + "\\" + listLines[i];

                    if (string.IsNullOrWhiteSpace(destName))
                    {
                        continue;
                    }

                    File.Move(f.FullName, MakeUniqueName(destName));
                }

                MessageBox.Show("The contents of folder \"" + d.FullName + "\" have been renamed according to " + "\"" + of.FileName + "\".", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (listLines.Length < files.Length)
                {
                    dr = MessageBox.Show("Do you want to isolate the unnamed files by moving them to a dedicated folder?", "Waiting for user", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr.Equals(DialogResult.Yes))
                    {
                        string isolatedDir = d.FullName + "\\" + ISOLATED_FOLDERNAME;
                        if (Directory.Exists(isolatedDir))
                        {
                            Directory.Delete(isolatedDir);
                        }
                        Directory.CreateDirectory(d.FullName + "\\" + ISOLATED_FOLDERNAME);

                        while (i < files.Length)
                        {
                            FileInfo f = files[i];
                            string destName = d.FullName + "\\" + ISOLATED_FOLDERNAME + "\\" + f.Name;
                            File.Move(f.FullName, destName);
                            i++;
                        }
                        MessageBox.Show("Isolated files have been moved to " + "\"" + ISOLATED_FOLDERNAME + "\"", "Files moved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void contentBasedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContentBasedBatchRename(this);
        }

        private void addressHelperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddressHelper form = new AddressHelper();
            form.Show();
        }


        private void overworldEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {

            BtxEditor form = new BtxEditor();
            form.Show();
        }

        private void generateCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Helpers.statusLabelMessage("Exporting to CSV...");
            Update();
            DocTool.ExportAll();

            Helpers.statusLabelMessage();
            Update();
        }

        private void flyWarpEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var flyEditor = new FlyEditor(gameFamily, EditorPanels.headerEditor.headerListBoxNames);
            flyEditor.Show();
        }

        private void tradeEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.tradeData });

            TradeEditor tradeEditor = new TradeEditor();
            tradeEditor.Show();
        }

        private void itemEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Helpers.statusLabelMessage("Setting up Item Data Editor...");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.itemData });
            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.itemIcons });

            ItemEditor itemEditor = new ItemEditor(
                RomInfo.GetItemNames()
            );
            itemEditor.Show();

            Helpers.statusLabelMessage();
            Update();
        }


        private void fromFolderContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (DirectoryInfo d, FileInfo[] files) dirData = OpenNonEmptyDir(title: "Folder-Based List Builder");
            DirectoryInfo d = dirData.d;
            FileInfo[] filePaths = dirData.files;

            if (d == null || filePaths == null)
            {
                return;
            }

            MessageBox.Show("Choose where to save the output list file.", "Name your list file", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = "List File (*.txt; *.list)|*.txt;*.list",
                FileName = d.Name + ".list"
            };
            if (sf.ShowDialog(this) != DialogResult.OK)
            {
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
        private void enumBasedListBuilderToolStripButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Pick a C Enum File [with entries on different lines].", "Enum-Based List Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);

            OpenFileDialog of = new OpenFileDialog
            {
                Filter = "Any Text File(*.*)|*.*"
            };
            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                Dictionary<int, string> entries = new Dictionary<int, string>();

                string[] cFileLines = File.ReadAllLines(of.FileName);
                cFileLines = cFileLines.Select(x => x.Trim()).ToArray();

                int enumStartLine;
                for (enumStartLine = 0; enumStartLine < cFileLines.Length; enumStartLine++)
                {
                    if (cFileLines[enumStartLine].Replace(" ", "").Contains("enum{"))
                    {
                        break;
                    }
                }

                if (cFileLines.Length - 1 == enumStartLine)
                {
                    MessageBox.Show("Abrupt termination of enum file.\nAborting.", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int terminationLine;
                for (terminationLine = enumStartLine + 1; terminationLine < cFileLines.Length; terminationLine++)
                {
                    if (cFileLines[terminationLine].Replace(" ", "").Contains("};"))
                    {
                        break;
                    }
                }

                if (terminationLine >= cFileLines.Length - 1)
                {
                    MessageBox.Show("Enum file is malformed.\nAborting", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                ;

                if (terminationLine - enumStartLine <= 2)
                {
                    MessageBox.Show("This utility needs at least 2 enum entries.\nAborting.", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                int indexFirstDifferentChar = cFileLines[enumStartLine + 1].Zip(cFileLines[enumStartLine + 2], (char1, char2) => char1 == char2).TakeWhile(b => b).Count();
                int lastCommonUnderscore = cFileLines[enumStartLine + 1].Substring(0, indexFirstDifferentChar).LastIndexOf('_');

                int lastNumber = 0;

                MessageBox.Show("Choose where to save the output list file.", "Name your list file", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string shortFileName = Path.GetFileNameWithoutExtension(of.FileName);

                SaveFileDialog sf = new SaveFileDialog
                {
                    Filter = "List File (*.txt; *.list)|*.txt;*.list",
                    FileName = shortFileName + ".list"
                };
                if (sf.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                for (int s = enumStartLine + 1; s < terminationLine; s++)
                {
                    string withoutComment;

                    int indexOfComment = cFileLines[s].IndexOf("//");
                    if (indexOfComment > 0)
                    {
                        withoutComment = cFileLines[s].Substring(0, indexOfComment);
                    }
                    else
                    {
                        withoutComment = cFileLines[s];
                    }

                    string differentSubstring = withoutComment.Substring(lastCommonUnderscore + 1).Trim().Replace(",", "");
                    int indexOfEquals = differentSubstring.LastIndexOf('=');

                    string entry = differentSubstring.Substring(0, indexOfEquals).Trim();
                    if (indexOfEquals > 0)
                    {
                        string numstr = differentSubstring.Substring(indexOfEquals + 1);
                        string[] split = numstr.Split(new char[] { ' ' }, options: StringSplitOptions.RemoveEmptyEntries);

                        if (split.Length > 1)
                        {
                            throw new Exception();
                        }

                        lastNumber = int.Parse(split[0]);
                    }

                    int posOfUnderscore = entry.LastIndexOf('_');
                    if (posOfUnderscore >= 0)
                    {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("The input enum file couldn't be read correctly.\nNo output file has been written." +
                    "\n\nAborting.", "Parser error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Details: " + ex.Message, "Failure details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void simpleToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            ToolStripMenuItem tsmi = (sender as ToolStripMenuItem);
            SetMenuLayout((byte)tsmi.GetCurrentParent().Items.IndexOf(tsmi));
        }

        private void overlayEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Helpers.statusLabelMessage("Setting up Overlay Editor...");
            Update();
            OverlayEditor ovlEditor = new OverlayEditor();
            ovlEditor.Show();

            Helpers.statusLabelMessage();
            Update();
        }

        private void moveDataEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Helpers.statusLabelMessage("Setting up Move Data Editor...");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.moveData });

            string[] moveDescriptions = new TextArchive(RomInfo.moveDescriptionsTextNumbers).messages.Select(
            x => x.Replace("\\n", Environment.NewLine)).ToArray();

            MoveDataEditor mde = new MoveDataEditor(
                new TextArchive(RomInfo.moveNamesTextNumbers).messages.ToArray(),
                moveDescriptions
            );
            mde.Show();

            Helpers.statusLabelMessage();
            Update();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow editor = new SettingsWindow();
             editor.Show();
        }

        private void pokemonDataEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] itemNames = RomInfo.GetItemNames();
            string[] abilityNames = RomInfo.GetAbilityNames();
            string[] moveNames = RomInfo.GetAttackNames();

            Helpers.statusLabelMessage("Setting up Pokémon Data Editor...");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.personalPokeData, DirNames.learnsets, DirNames.evolutions, DirNames.monIcons });
            RomInfo.SetMonIconsPalTableAddress();

            PokemonEditor pde = new PokemonEditor(itemNames, abilityNames, moveNames);
            Helpers.statusLabelMessage();
            Update();

            pde.Show();
        }


        #endregion

        private void trainerEditorStatButton_Click(object sender, EventArgs e)
        {
            string[] trcNames = RomInfo.GetTrainerClassNames();
            string[] pokeNames = RomInfo.GetPokemonNames();
            string[] trainerNames = GetSimpleTrainerNames();
            for (int i = 0; i < trcNames.Length; i++)
            {
                trcNames[i] = trcNames[i].Replace("♂", " M").Replace("♀", " F");
            }

            Dictionary<string, Dictionary<string, int>> trainerUsage = new Dictionary<string, Dictionary<string, int>>();

            for (int i = 0; i < trainerNames.Length; i++)
            {
                if (trainerNames[i].Equals("Angelica") || trainerNames[i].Equals("Mickey"))
                {
                    continue;
                }
                string suffix = "\\" + i.ToString("D4");

                TrainerFile f = new TrainerFile(
                    new TrainerProperties(
                        (ushort)trainerComboBox.SelectedIndex,
                        new FileStream(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir + suffix, FileMode.Open)
                    ),
                    new FileStream(RomInfo.gameDirs[DirNames.trainerParty].unpackedDir + suffix, FileMode.Open),
                    trainerNames[i]
                );

                if (f.party.CountNonEmptyMons() == 0)
                {
                    continue;
                }

                string className = trcNames[f.trp.trainerClass];


                if (trainerUsage.TryGetValue(className, out Dictionary<string, int> innerDict) == false)
                {
                    innerDict = trainerUsage[className] = new Dictionary<string, int>();
                }

                for (int p = 0; p < f.trp.partyCount; p++)
                {
                    PartyPokemon pp = f.party[p];
                    if (pp.CheckEmpty())
                    {
                        continue;
                    }
                    string pokeName = pokeNames[(int)pp.pokeID];

                    if (innerDict.TryGetValue(pokeName, out int occurrences))
                    {
                        innerDict[pokeName]++;
                    }
                    else
                    {
                        innerDict[pokeName] = 1;
                    }
                }
            }

            Helpers.ExportTrainerUsageToCSV(trainerUsage, "Report.csv");
        }

        #region Poppout Buttons
        private readonly Dictionary<Button, EditorPopoutConfig> _popouts = new Dictionary<Button, EditorPopoutConfig>();
        void WireEditorsPopout()
        {
            Register(EditorPanels.scriptEditor, scriptEditorPoppedOutLabel, popoutScriptEditorButton);
            Register(EditorPanels.levelScriptEditor, LSEditorPoppedOutLabel, popoutLevelScriptEditorButton);
            Register(EditorPanels.textEditor, textEditorPoppedOutLabel, popoutTextEditorButton);
            Register(EditorPanels.trainerEditor, trainerEditorPoppedOutLabel, popoutTrainerEditorButton);
            Register(EditorPanels.nsbtxEditor, nsbtxEditorPopOutLabel, popoutNsbtxEditorButton);
            Register(EditorPanels.eventEditor, eventEditorPopOutLabel, popoutEventEditorButton);
        }

        void Register(Control control, Label lbl, Button btn)
        {
            var cfg = new EditorPopoutConfig(control, lbl, btn);
            _popouts[btn] = cfg;
            btn.Click += popoutEditorClickHandler;
        }

        private void popoutEditorClickHandler(object sender, EventArgs e)
        {
            var currentTab = EditorPanels.mainTabControl.SelectedTab;
            if (sender is Button btn && _popouts.TryGetValue(btn, out var cfg))
            {
                Helpers.PopOutEditor(cfg.Control, currentTab.Text, cfg.PlaceholderLabel, cfg.PopoutButton,
                    mainTabImageList.Images[currentTab.ImageIndex]);
            }
        }

        #endregion

 
    }
}