using DSPRE.Editors;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using Ekona.Images;
using Images;
using LibNDSFormats.NSBMD;
using LibNDSFormats.NSBTX;
using Microsoft.WindowsAPICodePack.Dialogs;
using NarcAPI;
using NSMBe4.NSBMD;
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
using NSMBe4.NSBMD;
using System.Reflection;
using System.ComponentModel;
using DSPRE.Editors;
using DSPRE.Editors.BtxEditor;
using static DSPRE.Helpers;
using Velopack.Sources;
using Velopack;
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
            try
            {
                Helpers.CheckForUpdates();
            }
            catch
            {
                AppLogger.Error("Failed to check for updates.");
                MessageBox.Show("Failed to check for updates.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif
            InitializeComponent();

            EditorPanels.Initialize(this);
            Helpers.Initialize(this);

            SetMenuLayout(SettingsManager.Settings.menuLayout); //Read user settings for menu layout
            Text = "DS Pokémon Rom Editor Reloaded " + GetDSPREVersion() + " (Nømura, AdAstra/LD3005, Mixone)";

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
        public bool wslDetected = false; // Not technically necessary rn, but it might be useful in the future

        /* Editors Setup */
        public bool mapEditorIsReady { get;  set; } = false;

        /* ROM Information */
        public static string gameCode;
        public static byte europeByte;
        public RomInfo romInfo;
        public Dictionary<ushort /*evFile*/, ushort /*header*/> eventToHeader = new Dictionary<ushort, ushort>();

        #endregion

        #region Subroutines

        private void SetMenuLayout(byte layoutStyle)
        {
            Console.WriteLine("Setting menuLayout to" + layoutStyle);

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

        private string[] GetBuildingsList(bool interior)
        {
            List<string> names = new List<string>();
            string path = romInfo.GetBuildingModelsDirPath(interior);
            int buildModelsCount = Directory.GetFiles(path).Length;

            for (int i = 0; i < buildModelsCount; i++)
            {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(path + "\\" + i.ToString("D4"), 0x38))
                {
                    string nsbmdName = Encoding.UTF8.GetString(reader.ReadBytes(16)).TrimEnd();
                    names.Add(nsbmdName);
                }
            }
            return names.ToArray();
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
        private void updateBuildingListComboBox(bool interior)
        {
            string[] bldList = GetBuildingsList(interior);

            buildIndexComboBox.Items.Clear();
            for (int i = 0; i < bldList.Length; i++)
            {
                buildIndexComboBox.Items.Add("[" + i + "] " + bldList[i]);
            }
            toolStripProgressBar.Value++;
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

        private int UnpackRomCheckUserChoice()
        {
            // Check if extracted data for the ROM exists, and ask user if they want to load it.
            // Returns true if user aborted the process
            if (Directory.Exists(RomInfo.workDir))
            {
                DialogResult d = MessageBox.Show("Extracted data of this ROM has been found.\n" +
                    "Do you want to load it and unpack it?", "Data detected", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (d == DialogResult.Cancel)
                {
                    return -1; //user wants to abort loading
                }
                else if (d == DialogResult.Yes)
                {
                    return 0; //user wants to load data
                }
                else
                {
                    DialogResult nd = MessageBox.Show("All data of this ROM will be re-extracted. Proceed?\n",
                        "Existing data will be deleted", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (nd == DialogResult.No)
                    {
                        return -1; //user wants to abort loading
                    }
                    else
                    {
                        return 1; //user wants to re-extract data
                    }
                }
            }
            else
            {
                return 2; //No data found
            }
        }
        private bool UnpackRom(string ndsFileName)
        {
            Helpers.statusLabelMessage("Unpacking ROM contents to " + RomInfo.workDir + " ...");
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
            try
            {
                unpack.Start();
                unpack.WaitForExit();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Failed to call ndstool.exe" + Environment.NewLine + "Make sure DSPRE's Tools folder is intact.",
                    "Couldn't unpack ROM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
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
                    addHeaderBTN.Enabled = true;
                    removeLastHeaderBTN.Enabled = true;
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
            using (HeaderSearch h = new HeaderSearch(ref internalNames, headerListBox, statusLabel))
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

            using (BuildingEditor editor = new BuildingEditor(romInfo))
                editor.ShowDialog();
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
            string message = "DS Pokémon ROM Editor Reloaded by AdAstra/LD3005" + Environment.NewLine + "version " + GetDSPREVersion() + Environment.NewLine
                + Environment.NewLine + "Based on Nømura's DS Pokémon ROM Editor 1.0.4."
                + Environment.NewLine + "Largely inspired by Markitus95's \"Spiky's DS Map Editor\" (SDSME), from which certain assets were also reused." +
                "Credits go to Markitus, Ark, Zark, Florian, and everyone else who deserves credit for SDSME." + Environment.NewLine
                + Environment.NewLine + "Special thanks to Trifindo, Mikelan98, Mixone, JackHack96, Pleonex and BagBoy."
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
                return;
            }

            // Validate path and check for OneDrive
            if (!ValidateFilePath(openRom.FileName))
            {
                return;
            }

            if (!detectAndHandleWSL(openRom.FileName))
            {
                return; // User chose not to create a new work directory
            }

            // Handle WSL
            if (wslDetected)
            {
                string executablePath = Path.GetDirectoryName(Application.ExecutablePath);
                string buildFolderPath = Path.Combine(executablePath, "build");
                // Create a new work directory in the same folder as DSPRE
                if (!Directory.Exists(buildFolderPath))
                {
                    Directory.CreateDirectory(buildFolderPath);
                }

                // Copy the ROM to the build folder
                string newRomPath = Path.Combine(buildFolderPath, Path.GetFileName(openRom.FileName));

                // Check if file already exists and ask to overwrite
                if (File.Exists(newRomPath))
                {
                    DialogResult overwriteResult = MessageBox.Show("The ROM file already exists in the build folder. Do you want to overwrite it?", "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (overwriteResult != DialogResult.Yes)
                    {
                        return; // User chose not to overwrite
                    }
                }

                try
                {
                    File.Copy(openRom.FileName, newRomPath, true);
                    openRom.FileName = newRomPath; // Update the file name to the new path
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Failed to copy ROM to build folder: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            SetupROMLanguage(openRom.FileName);
            /* Set ROM gameVersion and language */
            romInfo = new RomInfo(gameCode, openRom.FileName, useSuffix: true);
            Helpers.romInfo = new RomInfo(gameCode, openRom.FileName, useSuffix: true);

            if (string.IsNullOrWhiteSpace(RomInfo.romID) || string.IsNullOrWhiteSpace(RomInfo.fileName))
            {
                return;
            }

            CheckROMLanguage();

            int userchoice = UnpackRomCheckUserChoice();
            switch (userchoice)
            {
                case -1:
                    Helpers.statusLabelMessage("Loading aborted");
                    Update();
                    return;
                case 0:
                    break;
                case 1:
                case 2:
                    Application.DoEvents();
                    if (userchoice == 1)
                    {
                        Helpers.statusLabelMessage("Deleting old data...");
                        try
                        {
                            Directory.Delete(RomInfo.workDir, true);
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("Concurrent access detected: \n" + RomInfo.workDir +
                                "\nMake sure no other process is using the extracted ROM folder while DSPRE is running.", "Concurrent Access", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        Update();
                    }

                    try
                    {
                        if (!UnpackRom(openRom.FileName))
                        {
                            Helpers.statusLabelError("ERROR");
                            languageLabel.Text = "";
                            versionLabel.Text = "Error";
                            return;
                        }
                        ARM9.EditSize(-12);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Can't access temp directory: \n" + RomInfo.workDir + "\nThis might be a temporary issue.\nMake sure no other process is using it and try again.", "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Helpers.statusLabelError("ERROR: Concurrent access to " + RomInfo.workDir);
                        Update();
                        return;
                    }
                    break;
            }

            iconON = true;
            gameIcon.Refresh();  // Paint game icon
            Helpers.statusLabelMessage("Attempting to unpack NARCs from folder...");
            Update();
            //for (int i = 0; i < 128; i++) {
            //    if (OverlayUtils.IsCompressed(i)) {
            //        OverlayUtils.Decompress(i);
            //    }
            //}
            ReadROMInitData();
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

        private bool detectAndHandleWSL(string fileName)
        {
            string fullPath = Path.GetFullPath(fileName);

            if (!fullPath.ToLower().Contains("wsl."))
            {
                return true; // No WSL detected, proceed normally
            }
            if (Directory.Exists(fullPath))
            {
                MessageBox.Show("WSL was detected in the path. " +
                    "The associated unpacked folder of a ROM should not be stored on the WSL file system! " +
                    "Please move the folder to the same drive that DSPRE is located on.", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            DialogResult result = MessageBox.Show("WSL was detected in the path. " +
                "Do you want to create a build directory in the same folder as DSPRE to unpack to?", "WSL Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            wslDetected = true;

            if (result == DialogResult.Yes)
            {
                return true; // User wants to create a new work directory
            }
            else
            {
                MessageBox.Show("Unpacking will not be possible without a valid work directory.", "Unpacking aborted", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
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
                if (europeByte == 0x0A)
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

            if (!detectAndHandleWSL(romFolderPath))
            {
                AppLogger.Info("ROM path validation failed. Possibly invalid or on a restricted (WSL).");
                return;
            }

            try
            {
                string headerFile = Directory.GetFiles(romFolderPath).First(x => x.Contains("header.bin"));
                AppLogger.Debug($"Found header file: {headerFile}");
                SetupROMLanguage(headerFile);
            }
            catch (InvalidOperationException)
            {
                AppLogger.Error("No 'header.bin' file found in ROM folder. Cannot initialize ROM.");
                MessageBox.Show("This folder does not seem to contain any data from a NDS Pokémon ROM.", "No ROM Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            romInfo = new RomInfo(gameCode, romFolderPath, useSuffix: false);

            if (string.IsNullOrWhiteSpace(RomInfo.romID) || string.IsNullOrWhiteSpace(RomInfo.fileName))
            {
                AppLogger.Error("ROM ID or filename is empty after initialization. Aborting.");
                return;
            }

            AppLogger.Info($"ROM loaded successfully: ID = {RomInfo.romID}, Name = {RomInfo.fileName}");

            CheckROMLanguage();
            AppLogger.Debug("ROM language checked and applied.");

            iconON = true;
            gameIcon.Refresh();  // Paint game icon
            AppLogger.Debug("Game icon refreshed.");

            ReadROMInitData();
            AppLogger.Info("ROM initialization data loaded.");
        }


        private void SetupROMLanguage(string headerPath)
        {
            using (DSUtils.EasyReader br = new DSUtils.EasyReader(headerPath, 0xC))
            {
                gameCode = Encoding.UTF8.GetString(br.ReadBytes(4));
                br.BaseStream.Position = 0x1E;
                europeByte = br.ReadByte();
            }
        }

        private void ReadROMInitData()
        {
            if (ARM9.CheckCompressionMark())
            {
                if (!RomInfo.gameFamily.Equals(GameFamilies.HGSS))
                {
                    MessageBox.Show("Unexpected compressed ARM9. It is advised that you double check the ARM9.");
                }
                if (!ARM9.Decompress(RomInfo.arm9Path))
                {
                    MessageBox.Show("ARM9 decompression failed. The program can't proceed.\nAborting.",
                                "Error with ARM9 decompression", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            /* Setup essential editors */
            SetupHeaderEditor();
            EditorPanels.eventEditor.eventOpenGlControl.InitializeContexts();
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
            this.Text += "  -  " + RomInfo.fileName;
        }

        private void saveRom_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveRom = new SaveFileDialog
            {
                Filter = DSUtils.NDSRomFilter,
                InitialDirectory = SettingsManager.Settings.exportPath
            };
            if (saveRom.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            Helpers.statusLabelMessage("Repacking NARCS...");
            Update();
            var dateBegin = DateTime.Now;

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

            //for (int i = 0; i < 128; i++) {
            //    if (!OverlayUtils.IsCompressed(i)) {
            //        OverlayUtils.Compress(i);
            //    }
            //}

            DSUtils.RepackROM(saveRom.FileName);

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

                SetupHeaderEditor();
                EditorPanels.matrixEditor.SetupMatrixEditor(this);
                SetupMapEditor();
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

                if (mapEditorIsReady)
                {
                    updateBuildingListComboBox(interiorbldRadioButton.Checked);
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
        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainTabControl.SelectedTab == headerEditorTabPage)
            {
                //
            }
            else if (mainTabControl.SelectedTab == EditorPanels.matrixEditorTabPage)
            {
                    matrixEditor.SetupMatrixEditor(this);
            }
            else if (mainTabControl.SelectedTab == mapEditorTabPage)
            {
                if (!mapEditorIsReady)
                {
                    SetupMapEditor();
                    mapOpenGlControl.MouseWheel += new MouseEventHandler(mapOpenGlControl_MouseWheel);
                    mapEditorIsReady = true;
                }
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
                resetHeaderSearch();
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

        private void tabPageScriptEditor_Enter(object sender, EventArgs e)
        {
            scriptEditor.SetupScriptEditor(this);
        }

        private void tabPageLevelScriptEditor_Enter(object sender, EventArgs e)
        {
            levelScriptEditor.SetUpLevelScriptEditor(this);
        }

        private void tabPageEncountersEditor_Enter(object sender, EventArgs e)
        {
            encountersEditor.SetupEncountersEditor();
        }

        private void spawnEditorToolStripButton_Click(object sender, EventArgs e)
        {
  
            matrixEditor.SetupMatrixEditor(this);

            using (SpawnEditor ed = new SpawnEditor(headerListBoxNames))
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
        private void openWildEditorWithIdButtonClick(object sender, EventArgs e)
        {
            openWildEditor(loadCurrent: true);
        }
        private void openWildEditor(bool loadCurrent)
        {
            Helpers.statusLabelMessage("Attempting to extract Wild Encounters NARC...");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames>() { DirNames.encounters, DirNames.monIcons });

            Helpers.statusLabelMessage("Passing control to Wild Pokémon Editor...");
            Update();

            int encToOpen = loadCurrent ? (int)wildPokeUpDown.Value : 0;

            string wildPokeUnpackedPath = gameDirs[DirNames.encounters].unpackedDir;
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    using (WildEditorDPPt editor = new WildEditorDPPt(wildPokeUnpackedPath, RomInfo.GetPokemonNames(), encToOpen, internalNames.Count))
                        editor.ShowDialog();
                    break;
                default:
                    using (WildEditorHGSS editor = new WildEditorHGSS(wildPokeUnpackedPath, RomInfo.GetPokemonNames(), encToOpen, internalNames.Count))
                        editor.ShowDialog();
                    break;
            }
            Helpers.statusLabelMessage();
        }
        #endregion

        #region Header Editor

        #region Variables
        public MapHeader currentHeader;
        public List<string> internalNames;
        public List<string> headerListBoxNames;
        #endregion
        private void SetupHeaderEditor()
        {
            /* Extract essential NARCs sub-archives*/

            Helpers.statusLabelMessage("Attempting to unpack Header Editor NARCs... Please wait.");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.synthOverlay, DirNames.textArchives, DirNames.dynamicHeaders });

            Helpers.statusLabelMessage("Reading internal names... Please wait.");
            Update();

            internalNames = new List<string>();
            headerListBoxNames = new List<string>();
            int headerCount;
            if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied())
            {
                addHeaderBTN.Enabled = true;
                removeLastHeaderBTN.Enabled = true;
                headerCount = Directory.GetFiles(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir).Length;
            }
            else
            {
                headerCount = RomInfo.GetHeaderCount();
            }

            /* Read Header internal names */
            try
            {
                headerListBoxNames = Helpers.getHeaderListBoxNames();
                internalNames = Helpers.getInternalNames();

                headerListBox.Items.Clear();
                headerListBox.Items.AddRange(headerListBoxNames.ToArray());
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(RomInfo.internalNamesPath + " doesn't exist.", "Couldn't read internal names", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            /*Add list of options to each control */
            textEditor.currentTextArchive = new TextArchive(RomInfo.locationNamesTextNumber);
            textEditor.ReloadHeaderEditorLocationsList(textEditor.currentTextArchive.messages, this);

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
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
                case GameFamilies.Plat:
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
            if (headerListBox.Items.Count > 0)
            {
                headerListBox.SelectedIndex = 0;
            }
            Helpers.statusLabelMessage();
        }
        private void addHeaderBTN_Click(object sender, EventArgs e)
        {
            // Add new file in the dynamic headers directory
            string sourcePath = RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + "0000";
            string destPath = RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + RomInfo.GetHeaderCount().ToString("D4");
            File.Copy(sourcePath, destPath);

            // Add row to internal names table
            const string newmap = "NEWMAP";
            DSUtils.WriteToFile(RomInfo.internalNamesPath, StringToInternalName(newmap), (uint)RomInfo.GetHeaderCount() * RomInfo.internalNameLength);

            // Update headers ListBox and internal names list
            headerListBox.Items.Add(headerListBox.Items.Count + MapHeader.nameSeparator + " " + newmap);
            headerListBoxNames.Add(headerListBox.Items.Count + MapHeader.nameSeparator + " " + newmap);
            internalNames.Add(newmap);

            // Select new header
            headerListBox.SelectedIndex = headerListBox.Items.Count - 1;
        }
        private void removeLastHeaderBTN_Click(object sender, EventArgs e)
        {
            /* Check if currently selected file is the last one, and in that case select the one before it */
            int lastIndex = headerListBox.Items.Count - 1;

            if (lastIndex > 0)
            { //there are at least 2 elements
                if (headerListBox.SelectedIndex == lastIndex)
                {
                    headerListBox.SelectedIndex--;
                }

                /* Physically delete last header file */
                File.Delete(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + lastIndex.ToString("D4"));
                using (DSUtils.EasyWriter ew = new DSUtils.EasyWriter(RomInfo.internalNamesPath))
                {
                    ew.EditSize(-internalNameLength); //Delete internalNameLength amount of bytes from file end
                }

                /* Remove item from collections */
                headerListBox.Items.RemoveAt(lastIndex);
                internalNames.RemoveAt(lastIndex);
                headerListBoxNames.RemoveAt(lastIndex);
            }
            else
            {
                MessageBox.Show("You must have at least one header!", "Can't delete last header", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void areaDataUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            currentHeader.areaDataID = (byte)areaDataUpDown.Value;
        }
        private void internalNameBox_TextChanged(object sender, EventArgs e)
        {
            if (internalNameBox.Text.Length > 13)
            {
                internalNameLenLabel.ForeColor = Color.FromArgb(255, 0, 0);
            }
            else if (internalNameBox.Text.Length > 7)
            {
                internalNameLenLabel.ForeColor = Color.FromArgb(190, 190, 0);
            }
            else
            {
                internalNameLenLabel.ForeColor = Color.FromArgb(0, 180, 0);
            }

            internalNameLenLabel.Text = "[ " + (internalNameBox.Text.Length) + " ]";
        }
        private void areaIconComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            string imageName;
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    break;
                case GameFamilies.Plat:
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
        private void eventFileUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            currentHeader.eventFileID = (ushort)eventFileUpDown.Value;
        }
        private void battleBackgroundUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentHeader.battleBackground = (byte)battleBackgroundUpDown.Value;
        }
        private void followModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                HeaderHGSS currentHeaderHGSS = (HeaderHGSS)currentHeader;
                currentHeaderHGSS.followMode = (byte)followModeComboBox.SelectedIndex;
            }
        }

        private void kantoRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                HeaderHGSS currentHeaderHGSS = (HeaderHGSS)currentHeader;
                currentHeaderHGSS.kantoFlag = kantoRadioButton.Checked;
            }
        }
        private void headerFlagsCheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
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

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                if (flag4CheckBox.Checked)
                    flagVal += (byte)Math.Pow(2, 4);
                if (flag5CheckBox.Checked)
                    flagVal += (byte)Math.Pow(2, 5);
                if (flag6CheckBox.Checked)
                    flagVal += (byte)Math.Pow(2, 6);
            }
            currentHeader.flags = flagVal;
        }
        private void headerListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || headerListBox.SelectedIndex < 0)
            {
                return;
            }

            /* Obtain current header ID from listbox*/
            if (!ushort.TryParse(headerListBox.SelectedItem.ToString().Substring(0, 3), out ushort headerNumber))
            {
                headerListBox.SelectedIndex = -1;
                return;
            }

            /* Check if dynamic headers patch has been applied, and load header from arm9 or a/0/5/0 accordingly */
            if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied())
            {
                currentHeader = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + headerNumber.ToString("D4"), headerNumber, 0);
            }
            else
            {
                currentHeader = MapHeader.LoadFromARM9(headerNumber);
            }
            RefreshHeaderEditorFields();
        }

        private void RefreshHeaderEditorFields()
        {
            /* Setup controls for common fields across headers */
            if (currentHeader == null)
            {
                return;
            }

            internalNameBox.Text = internalNames[currentHeader.ID];
            matrixUpDown.Value = currentHeader.matrixID;
            areaDataUpDown.Value = currentHeader.areaDataID;
            scriptFileUpDown.Value = currentHeader.scriptFileID;
            levelScriptUpDown.Value = currentHeader.levelScriptID;
            eventFileUpDown.Value = currentHeader.eventFileID;
            textFileUpDown.Value = currentHeader.textArchiveID;
            wildPokeUpDown.Value = currentHeader.wildPokemon;
            weatherUpDown.Value = currentHeader.weatherID;
            cameraUpDown.Value = currentHeader.cameraAngleID;
            battleBackgroundUpDown.Value = currentHeader.battleBackground;

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                areaSettingsComboBox.SelectedIndex = ((HeaderHGSS)currentHeader).locationType;
            }

            openWildEditorWithIdButton.Enabled = currentHeader.wildPokemon != RomInfo.nullEncounterID;

            /* Setup controls for fields with version-specific differences */
            try
            {
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        {
                            HeaderDP h = (HeaderDP)currentHeader;

                            locationNameComboBox.SelectedIndex = h.locationName;
                            musicDayUpDown.Value = h.musicDayID;
                            musicNightUpDown.Value = h.musicNightID;
                            areaSettingsComboBox.SelectedIndex = areaSettingsComboBox.FindString("[" + $"{currentHeader.locationSpecifier:D3}");
                            break;
                        }
                    case GameFamilies.Plat:
                        {
                            HeaderPt h = (HeaderPt)currentHeader;

                            areaIconComboBox.SelectedIndex = h.areaIcon;
                            locationNameComboBox.SelectedIndex = h.locationName;
                            musicDayUpDown.Value = h.musicDayID;
                            musicNightUpDown.Value = h.musicNightID;
                            areaSettingsComboBox.SelectedIndex = areaSettingsComboBox.FindString("[" + $"{currentHeader.locationSpecifier:D3}");
                            break;
                        }
                    default:
                        {
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
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("This header contains an irregular/unsupported field.", "Error loading header file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            RefreshFlags();
            updateWeatherPicAndComboBox();
            updateCameraPicAndComboBox();
        }
        private void RefreshFlags()
        {
            BitArray ba = new BitArray(new byte[] { currentHeader.flags });

            flag0CheckBox.Checked = ba[0];
            flag1CheckBox.Checked = ba[1];
            flag2CheckBox.Checked = ba[2];
            flag3CheckBox.Checked = ba[3];

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                flag4CheckBox.Checked = ba[4];
                flag5CheckBox.Checked = ba[5];
                flag6CheckBox.Checked = ba[6];
            }
        }

        private void headerListBox_Leave(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            headerListBox.Refresh();
        }
        private void levelScriptUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            currentHeader.levelScriptID = (ushort)levelScriptUpDown.Value;
        }
        private void mapNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    ((HeaderDP)currentHeader).locationName = (ushort)locationNameComboBox.SelectedIndex;
                    break;
                case GameFamilies.Plat:
                    ((HeaderPt)currentHeader).locationName = (byte)locationNameComboBox.SelectedIndex;
                    break;
                default:
                    ((HeaderHGSS)currentHeader).locationName = (byte)locationNameComboBox.SelectedIndex;
                    break;
            }
        }
        private void matrixUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            currentHeader.matrixID = (ushort)matrixUpDown.Value;
        }

        private void openScriptButton_Click(object sender, EventArgs e)
        {
            EditorPanels.scriptEditor.OpenScriptEditor(this, (int)scriptFileUpDown.Value);
        }

        private void openLevelScriptButton_Click(object sender, EventArgs e)
        {
            EditorPanels.levelScriptEditor.OpenLevelScriptEditor(this, (int)levelScriptUpDown.Value);
        }

        private void musicDayComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.DPMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
                case GameFamilies.Plat:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.PtMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
                default:
                    currentHeader.musicDayID = (ushort)(musicDayUpDown.Value = PokeDatabase.MusicDB.HGSSMusicDict.Keys.ElementAt(musicDayComboBox.SelectedIndex));
                    break;
            }
        }
        private void musicNightComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.DPMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
                case GameFamilies.Plat:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.PtMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
                default:
                    currentHeader.musicNightID = (ushort)(musicNightUpDown.Value = PokeDatabase.MusicDB.HGSSMusicDict.Keys.ElementAt(musicNightComboBox.SelectedIndex));
                    break;
            }
        }
        private void musicDayUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            Helpers.DisableHandlers();
            ushort updValue = (ushort)((NumericUpDown)sender).Value;
            currentHeader.musicDayID = updValue;
            try
            {
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.DPMusicDict[updValue];
                        break;
                    case GameFamilies.Plat:
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.PtMusicDict[updValue];
                        break;
                    default:
                        musicDayComboBox.SelectedItem = PokeDatabase.MusicDB.HGSSMusicDict[updValue];
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                musicDayComboBox.SelectedItem = null;
            }
            Helpers.EnableHandlers();
        }
        private void musicNightUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            Helpers.DisableHandlers();
            ushort updValue = (ushort)((NumericUpDown)sender).Value;
            currentHeader.musicNightID = updValue;
            try
            {
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.DPMusicDict[updValue];
                        break;
                    case GameFamilies.Plat:
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.PtMusicDict[updValue];
                        break;
                    default:
                        musicNightComboBox.SelectedItem = PokeDatabase.MusicDB.HGSSMusicDict[updValue];
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                musicNightComboBox.SelectedItem = null;
            }
            Helpers.EnableHandlers();
        }
        private void worldmapXCoordUpDown_ValueChanged(object sender, EventArgs e)
        {
            ((HeaderHGSS)currentHeader).worldmapX = (byte)worldmapXCoordUpDown.Value;
        }
        private void worldmapYCoordUpDown_ValueChanged(object sender, EventArgs e)
        {
            ((HeaderHGSS)currentHeader).worldmapY = (byte)worldmapYCoordUpDown.Value;
        }
        private void updateWeatherPicAndComboBox()
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            /* Update Weather Combobox*/
            Helpers.DisableHandlers();
            try
            {
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.DPWeatherDict[currentHeader.weatherID];
                        break;
                    case GameFamilies.Plat:
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.PtWeatherDict[currentHeader.weatherID];
                        break;
                    default:
                        weatherComboBox.SelectedItem = PokeDatabase.Weather.HGSSWeatherDict[currentHeader.weatherID];
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                weatherComboBox.SelectedItem = null;
            }
            Helpers.EnableHandlers();

            /* Update Weather Picture */
            try
            {
                Dictionary<byte[], string> dict;
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        dict = PokeDatabase.System.WeatherPics.dpWeatherImageDict;
                        break;
                    case GameFamilies.Plat:
                        dict = PokeDatabase.System.WeatherPics.ptWeatherImageDict;
                        break;
                    default:
                        dict = PokeDatabase.System.WeatherPics.hgssweatherImageDict;
                        break;
                }

                bool found = false;
                foreach (KeyValuePair<byte[], string> dictEntry in dict)
                {
                    if (Array.IndexOf(dictEntry.Key, (byte)weatherUpDown.Value) >= 0)
                    {
                        weatherPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(dictEntry.Value);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    throw new KeyNotFoundException();
            }
            catch (KeyNotFoundException)
            {
                weatherPictureBox.Image = null;
            }
        }
        private void updateCameraPicAndComboBox()
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            /* Update Camera Combobox*/
            Helpers.DisableHandlers();
            try
            {
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        cameraComboBox.SelectedItem = PokeDatabase.CameraAngles.DPPtCameraDict[currentHeader.cameraAngleID];
                        break;
                    case GameFamilies.Plat:
                        cameraComboBox.SelectedItem = PokeDatabase.CameraAngles.DPPtCameraDict[currentHeader.cameraAngleID];
                        break;
                    default:
                        cameraComboBox.SelectedItem = PokeDatabase.CameraAngles.HGSSCameraDict[currentHeader.cameraAngleID];
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                cameraComboBox.SelectedItem = null;
            }
            Helpers.EnableHandlers();

            /* Update Camera Picture */
            string imageName;
            try
            {
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "dpcamera" + cameraUpDown.Value.ToString();
                        break;
                    case GameFamilies.Plat:
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "ptcamera" + cameraUpDown.Value.ToString();
                        break;
                    default:
                        currentHeader.cameraAngleID = (byte)cameraComboBox.SelectedIndex;
                        imageName = "hgsscamera" + cameraUpDown.Value.ToString();
                        break;
                }
                cameraPictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("The current header uses an unrecognized camera.\n", "Unknown camera settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void weatherComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || weatherComboBox.SelectedIndex < 0)
            {
                return;
            }

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    weatherUpDown.Value = PokeDatabase.Weather.DPWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
                case GameFamilies.Plat:
                    weatherUpDown.Value = PokeDatabase.Weather.PtWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
                default:
                    weatherUpDown.Value = PokeDatabase.Weather.HGSSWeatherDict.Keys.ElementAt(weatherComboBox.SelectedIndex);
                    break;
            }
            currentHeader.weatherID = (byte)weatherUpDown.Value;
        }
        private void weatherUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentHeader.weatherID = (byte)weatherUpDown.Value;
            updateWeatherPicAndComboBox();
        }
        private void cameraComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || cameraComboBox.SelectedIndex < 0)
            {
                return;
            }

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    cameraUpDown.Value = PokeDatabase.CameraAngles.DPPtCameraDict.Keys.ElementAt(cameraComboBox.SelectedIndex);
                    break;
                case GameFamilies.Plat:
                    cameraUpDown.Value = PokeDatabase.CameraAngles.DPPtCameraDict.Keys.ElementAt(cameraComboBox.SelectedIndex);
                    break;
                default:
                    cameraUpDown.Value = PokeDatabase.CameraAngles.HGSSCameraDict.Keys.ElementAt(cameraComboBox.SelectedIndex);
                    break;
            }
            currentHeader.cameraAngleID = (byte)cameraUpDown.Value;
        }
        private void cameraUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentHeader.cameraAngleID = (byte)cameraUpDown.Value;
            updateCameraPicAndComboBox();
        }
        private void openAreaDataButton_Click(object sender, EventArgs e)
        {
            nsbtxEditor.SetupNSBTXEditor(this);

            nsbtxEditor.selectAreaDataListBox.SelectedIndex = (int)areaDataUpDown.Value;
            nsbtxEditor.texturePacksListBox.SelectedIndex = (nsbtxEditor.mapTilesetRadioButton.Checked ? (int)nsbtxEditor.areaDataMapTilesetUpDown.Value : (int)nsbtxEditor.areaDataBuildingTilesetUpDown.Value);
            mainTabControl.SelectedTab = nsbtxEditorTabPage;

            if (nsbtxEditor.texturesListBox.Items.Count > 0)
                nsbtxEditor.texturesListBox.SelectedIndex = 0;
            if (nsbtxEditor.palettesListBox.Items.Count > 0)
                nsbtxEditor.palettesListBox.SelectedIndex = 0;
        }
        private void openEventsButton_Click(object sender, EventArgs e)
        {
            EditorPanels.eventEditor.SetupEventEditor(this);


            if (matrixUpDown.Value != 0)
            {
                EditorPanels.eventEditor.eventAreaDataUpDown.Value = areaDataUpDown.Value; // Use Area Data for textures if matrix is not 0
            }

            EditorPanels.eventEditor.eventMatrixUpDown.Value = matrixUpDown.Value; // Open the right matrix in event editor
            EditorPanels.eventEditor.selectEventComboBox.SelectedIndex = (int)eventFileUpDown.Value; // Select event file
            mainTabControl.SelectedTab = eventEditorTabPage;

            EditorPanels.eventEditor.eventMatrixUpDown_ValueChanged(null, null);
        }
        private void openMatrixButton_Click(object sender, EventArgs e)
        {

            matrixEditor.SetupMatrixEditor(this);
            mainTabControl.SelectedTab = matrixEditorTabPage;
            int matrixNumber = (int)matrixUpDown.Value;
            matrixEditor.selectMatrixComboBox.SelectedIndex = matrixNumber;

            if (matrixEditor.currentMatrix.hasHeadersSection)
            {
                matrixEditor.matrixTabControl.SelectedTab = matrixEditor.headersTabPage;

                //Autoselect cell containing current header, if such cell exists [and if current matrix has headers sections]
                for (int i = 0; i < matrixEditor.headersGridView.RowCount; i++)
                {
                    for (int j = 0; j < matrixEditor.headersGridView.ColumnCount; j++)
                    {
                        if (currentHeader.ID.ToString() == matrixEditor.headersGridView.Rows[i].Cells[j].Value.ToString())
                        {
                            matrixEditor.headersGridView.CurrentCell = matrixEditor.headersGridView.Rows[i].Cells[j];
                            return;
                        }
                    }
                }
            }
        }
        private void openTextArchiveButton_Click(object sender, EventArgs e)
        {
            textEditor.OpenTextEditor(this, (int)textFileUpDown.Value, locationNameComboBox);
        }
        private void saveHeaderButton_Click(object sender, EventArgs e)
        {
            /* Check if dynamic headers patch has been applied, and save header to arm9 or a/0/5/0 accordingly */
            if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied())
            {
                DSUtils.WriteToFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + currentHeader.ID.ToString("D4"), currentHeader.ToByteArray(), 0, 0, fmode: FileMode.Create);
            }
            else
            {
                uint headerOffset = (uint)(RomInfo.headerTableOffset + MapHeader.length * currentHeader.ID);
                ARM9.WriteBytes(currentHeader.ToByteArray(), headerOffset);
            }
            Helpers.DisableHandlers();

            updateCurrentInternalName();
            updateHeaderNameShown(headerListBox.SelectedIndex);
            headerListBox.Focus();
            Helpers.EnableHandlers();
        }
        private byte[] StringToInternalName(string text)
        {
            if (text.Length > internalNameLength)
            {
                MessageBox.Show("Internal names can't be longer than " + internalNameLength + " characters!", "Length error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Encoding.ASCII.GetBytes(text.Substring(0, Math.Min(text.Length, internalNameLength)).PadRight(internalNameLength, '\0'));
        }
        private void updateCurrentInternalName()
        {
            /* Update internal name according to internalNameBox text*/
            ushort headerID = currentHeader.ID;

            using (DSUtils.EasyWriter writer = new DSUtils.EasyWriter(RomInfo.internalNamesPath, headerID * RomInfo.internalNameLength))
            {
                writer.Write(StringToInternalName(internalNameBox.Text));
            }

            internalNames[headerID] = internalNameBox.Text;
            string elem = headerID.ToString("D3") + MapHeader.nameSeparator + internalNames[headerID];
            headerListBoxNames[headerID] = elem;

            if (EditorPanels.eventEditor.eventEditorIsReady)
            {
                EditorPanels.eventEditor.eventEditorWarpHeaderListBox.Items[headerID] = elem;
            }
        }
        private void updateHeaderNameShown(int thisIndex)
        {
            Helpers.DisableHandlers();
            string val = (string)(headerListBox.Items[thisIndex] = headerListBoxNames[currentHeader.ID]);
            if (EditorPanels.eventEditor.eventEditorIsReady)
            {
                EditorPanels.eventEditor.eventEditorWarpHeaderListBox.Items[thisIndex] = val;
            }
            Helpers.EnableHandlers();
        }
        private void resetButton_Click(object sender, EventArgs e)
        {
            resetHeaderSearch();
        }

        void resetHeaderSearch()
        {
            searchLocationTextBox.Clear();
            HeaderSearch.ResetResults(headerListBox, headerListBoxNames, prependNumbers: false);
            Helpers.statusLabelMessage();
        }

        private void searchHeaderTextBox_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                startSearchGameLocation();
            }
        }
        private void searchHeaderButton_Click(object sender, EventArgs e)
        {
            startSearchGameLocation();
        }
        private void startSearchGameLocation()
        {
            if (searchLocationTextBox.Text.Length != 0)
            {
                headerListBox.Items.Clear();
                bool noResult = true;

                /* Check if dynamic headers patch has been applied, and load header from arm9 or a/0/5/0 accordingly */
                for (ushort i = 0; i < internalNames.Count; i++)
                {
                    MapHeader h;
                    if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied())
                    {
                        h = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + i.ToString("D4"), i, 0);
                    }
                    else
                    {
                        h = MapHeader.LoadFromARM9(i);
                    }

                    string locationName = "";
                    switch (RomInfo.gameFamily)
                    {
                        case GameFamilies.DP:
                            locationName = locationNameComboBox.Items[((HeaderDP)h).locationName].ToString();
                            break;
                        case GameFamilies.Plat:
                            locationName = locationNameComboBox.Items[((HeaderPt)h).locationName].ToString();
                            break;
                        case GameFamilies.HGSS:
                            locationName = locationNameComboBox.Items[((HeaderHGSS)h).locationName].ToString();
                            break;
                    }

                    if (locationName.IndexOf(searchLocationTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        headerListBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + internalNames[i]);
                        noResult = false;
                    }
                }


                if (noResult)
                {
                    headerListBox.Items.Add("No result for " + '"' + searchLocationTextBox.Text + '"');
                    headerListBox.Enabled = false;
                }
                else
                {
                    headerListBox.SelectedIndex = 0;
                    headerListBox.Enabled = true;
                }
            }
            else if (headerListBox.Items.Count < internalNames.Count)
            {
                HeaderSearch.ResetResults(headerListBox, headerListBoxNames, prependNumbers: false);
            }
        }
        private void PrintMapHeadersSummary()
        {
            List<string> output = new List<string>();
            int sameInARow = 0;

            MapHeader[] hBuff = new MapHeader[2] {
                null,
                MapHeader.LoadFromARM9(0),
            };


            string[] locBuff = new string[2];
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                    locBuff[1] = locationNameComboBox.Items[((HeaderDP)hBuff[1]).locationName].ToString();
                    break;
                case GameFamilies.Plat:
                    locBuff[1] = locationNameComboBox.Items[((HeaderPt)hBuff[1]).locationName].ToString();
                    break;
                case GameFamilies.HGSS:
                    locBuff[1] = locationNameComboBox.Items[((HeaderHGSS)hBuff[1]).locationName].ToString();
                    break;
            }

            for (ushort i = 0; i < internalNames.Count; i++)
            {
                hBuff[0] = hBuff[1];
                hBuff[1] = MapHeader.LoadFromARM9((ushort)(i + 1));

                string lastName = locBuff[0]; //Kind of a locBuff[-1]
                locBuff[0] = locBuff[1];
                switch (RomInfo.gameFamily)
                {
                    case GameFamilies.DP:
                        locBuff[1] = locationNameComboBox.Items[((HeaderDP)hBuff[1]).locationName].ToString();
                        break;
                    case GameFamilies.Plat:
                        locBuff[1] = locationNameComboBox.Items[((HeaderPt)hBuff[1]).locationName].ToString();
                        break;
                    case GameFamilies.HGSS:
                        locBuff[1] = locationNameComboBox.Items[((HeaderHGSS)hBuff[1]).locationName].ToString();
                        break;
                }


                string newStr = i.ToString("D3") + " - " + internalNames[i] + " - " + locBuff[0];

                if (output.Count > 0)
                {
                    if (lastName.Equals(locBuff[0]))
                    {
                        output.Add(newStr);
                        sameInARow++;
                    }
                    else
                    {
                        if (sameInARow > 0 || (sameInARow == 0 && locBuff[0].Equals(locBuff[1])))
                        {
                            output.Add("");
                        }
                        output.Add(newStr);
                        sameInARow = 0;
                    }
                }
                else
                {
                    output.Add(newStr);
                }
            }

            //File.WriteAllLines("dummy.txt", output);
        }
        private void scriptFileUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            currentHeader.scriptFileID = (ushort)scriptFileUpDown.Value;
        }
        private void areaSettingsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || areaSettingsComboBox.SelectedItem is null)
            {
                return;
            }

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    currentHeader.locationSpecifier = Byte.Parse(areaSettingsComboBox.SelectedItem.ToString().Substring(1, 3));
                    break;
                case GameFamilies.HGSS:
                    HeaderHGSS ch = (HeaderHGSS)currentHeader;
                    ch.locationType = (byte)areaSettingsComboBox.SelectedIndex;
                    break;
            }
        }
        private void textFileUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            currentHeader.textArchiveID = (ushort)textFileUpDown.Value;
        }

        private void wildPokeUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            currentHeader.wildPokemon = (ushort)wildPokeUpDown.Value;
            if (wildPokeUpDown.Value == RomInfo.nullEncounterID)
            {
                wildPokeUpDown.ForeColor = Color.Red;
            }
            else
            {
                wildPokeUpDown.ForeColor = Color.Black;
            }

            if (currentHeader.wildPokemon == RomInfo.nullEncounterID)
                openWildEditorWithIdButton.Enabled = false;
            else
                openWildEditorWithIdButton.Enabled = true;
        }
        private void importHeaderFromFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = MapHeader.DefaultFilter
            };
            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            MapHeader h = null;
            try
            {
                if (new FileInfo(of.FileName).Length > 48)
                    throw new FileFormatException();

                h = MapHeader.LoadFromFile(of.FileName, currentHeader.ID, 0);
                if (h == null)
                    throw new FileFormatException();

            }
            catch (FileFormatException)
            {
                MessageBox.Show("The file you tried to import is either malformed or not a Header file.\nNo changes have been made.",
                        "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentHeader = h;
            /* Check if dynamic headers patch has been applied, and save header to arm9 or a/0/5/0 accordingly */
            if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied())
            {
                DSUtils.WriteToFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + currentHeader.ID.ToString("D4"), currentHeader.ToByteArray(), 0, 0, fmode: FileMode.Create);
            }
            else
            {
                uint headerOffset = (uint)(RomInfo.headerTableOffset + MapHeader.length * currentHeader.ID);
                ARM9.WriteBytes(currentHeader.ToByteArray(), headerOffset);
            }

            try
            {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(of.FileName, MapHeader.length + 8))
                {
                    internalNameBox.Text = Encoding.UTF8.GetString(reader.ReadBytes(RomInfo.internalNameLength));
                }
                updateCurrentInternalName();
                updateHeaderNameShown(headerListBox.SelectedIndex);
            }
            catch (EndOfStreamException) { }

            RefreshHeaderEditorFields();
        }

        private void exportHeaderToFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = MapHeader.DefaultFilter,
                FileName = "Header " + currentHeader.ID + " - " + internalNames[currentHeader.ID] + " (" + locationNameComboBox.SelectedItem.ToString() + ")"
            };

            if (sf.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            using (DSUtils.EasyWriter writer = new DSUtils.EasyWriter(sf.FileName))
            {
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
        private void copyHeaderButton_Click(object sender, EventArgs e)
        {
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
        private void copyInternalNameButton_Click(object sender, EventArgs e)
        {
            internalNameCopy = internalNameBox.Text;
            Clipboard.SetData(DataFormats.Text, internalNameCopy);
            pasteInternalNameButton.Enabled = true;
        }
        private void copyLocationNameButton_Click(object sender, EventArgs e)
        {
            locationNameCopy = locationNameComboBox.SelectedIndex;
            pasteLocationNameButton.Enabled = true;
        }
        private void copyAreaSettingsButton_Click(object sender, EventArgs e)
        {
            areaSettingsCopy = areaSettingsComboBox.SelectedIndex;
            pasteAreaSettingsButton.Enabled = true;
        }
        private void copyAreaIconButton_Click(object sender, EventArgs e)
        {
            areaIconCopy = areaIconComboBox.SelectedIndex;
            pasteAreaIconButton.Enabled = true;
        }
        private void copyWildEncountersButton_Click(object sender, EventArgs e)
        {
            encountersIDCopy = wildPokeUpDown.Value;
            Clipboard.SetData(DataFormats.Text, encountersIDCopy);
            pasteWildEncountersButton.Enabled = true;
        }
        private void copyMusicDayButton_Click(object sender, EventArgs e)
        {
            musicdayCopy = musicDayComboBox.SelectedIndex;
            pasteMusicDayButton.Enabled = true;
        }
        private void copyWeatherButton_Click(object sender, EventArgs e)
        {
            weatherCopy = weatherComboBox.SelectedIndex;
            pasteWeatherButton.Enabled = true;
        }
        private void copyMusicNightButton_Click(object sender, EventArgs e)
        {
            musicnightCopy = musicNightComboBox.SelectedIndex;
            pasteMusicNightButton.Enabled = true;
        }
        private void copyCameraAngleButton_Click(object sender, EventArgs e)
        {
            camAngleCopy = cameraComboBox.SelectedIndex;
            pasteCameraAngleButton.Enabled = true;
        }
        private void copyScriptsButton_Click(object sender, EventArgs e)
        {
            scriptsCopy = scriptFileUpDown.Value;
            Clipboard.SetData(DataFormats.Text, scriptsCopy);
            pasteScriptsButton.Enabled = true;
        }
        private void copyLevelScriptsButton_Click(object sender, EventArgs e)
        {
            levelScriptsCopy = levelScriptUpDown.Value;
            Clipboard.SetData(DataFormats.Text, levelScriptsCopy);
            pasteLevelScriptsButton.Enabled = true;
        }
        private void copyEventsButton_Click(object sender, EventArgs e)
        {
            eventsCopy = eventFileUpDown.Value;
            Clipboard.SetData(DataFormats.Text, eventsCopy);
            pasteEventsButton.Enabled = true;
        }
        private void copyTextsButton_Click(object sender, EventArgs e)
        {
            textsCopy = textFileUpDown.Value;
            Clipboard.SetData(DataFormats.Text, textsCopy);
            pasteTextsButton.Enabled = true;
        }
        private void copyMatrixButton_Click(object sender, EventArgs e)
        {
            matrixCopy = matrixUpDown.Value;
            Clipboard.SetData(DataFormats.Text, matrixCopy);
            pasteMatrixButton.Enabled = true;
        }
        private void copyAreaDataButton_Click(object sender, EventArgs e)
        {
            areadataCopy = areaDataUpDown.Value;
            Clipboard.SetData(DataFormats.Text, areadataCopy);
            pasteAreaDataButton.Enabled = true;
        }
        private void worldmapCoordsCopyButton_Click(object sender, EventArgs e)
        {
            worldmapXCoordCopy = worldmapXCoordUpDown.Value;
            worldmapYCoordCopy = worldmapYCoordUpDown.Value;
            worldmapCoordsPasteButton.Enabled = true;
        }
        private void copyMapSettingsButton_Click(object sender, EventArgs e)
        {
            flagsCopy = currentHeader.flags;
            battleBGCopy = currentHeader.battleBackground;
            followingPokeCopy = followModeComboBox.SelectedIndex;
            kantoFlagCopy = kantoRadioButton.Checked;
            pasteMapSettingsButton.Enabled = true;
        }

        /* Paste Buttons */
        private void pasteHeaderButton_Click(object sender, EventArgs e)
        {
            locationNameComboBox.SelectedIndex = locationNameCopy;
            internalNameBox.Text = internalNameCopy;
            wildPokeUpDown.Value = encountersIDCopy;

            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    areaSettingsComboBox.SelectedIndex = shownameCopy;
                    break;
                case GameFamilies.HGSS:
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
        private void pasteInternalNameButton_Click(object sender, EventArgs e)
        {
            internalNameBox.Text = internalNameCopy;
        }
        private void pasteLocationNameButton_Click(object sender, EventArgs e)
        {
            locationNameComboBox.SelectedIndex = locationNameCopy;
        }
        private void pasteAreaSettingsButton_Click(object sender, EventArgs e)
        {
            areaSettingsComboBox.SelectedIndex = shownameCopy;
        }
        private void pasteAreaIconButton_Click(object sender, EventArgs e)
        {
            if (areaIconComboBox.Enabled)
            {
                areaIconComboBox.SelectedIndex = areaIconCopy;
            }
        }
        private void pasteWildEncountersButton_Click(object sender, EventArgs e)
        {
            wildPokeUpDown.Value = encountersIDCopy;
        }
        private void pasteMusicDayButton_Click(object sender, EventArgs e)
        {
            musicDayComboBox.SelectedIndex = musicdayCopy;
        }
        private void pasteScriptsButton_Click(object sender, EventArgs e)
        {
            scriptFileUpDown.Value = scriptsCopy;
        }
        private void pasteLevelScriptsButton_Click(object sender, EventArgs e)
        {
            levelScriptUpDown.Value = levelScriptsCopy;
        }
        private void pasteEventsButton_Click(object sender, EventArgs e)
        {
            eventFileUpDown.Value = eventsCopy;
        }
        private void pasteTextsButton_Click(object sender, EventArgs e)
        {
            textFileUpDown.Value = textsCopy;
        }
        private void pasteMatrixButton_Click(object sender, EventArgs e)
        {
            matrixUpDown.Value = matrixCopy;
        }
        private void pasteAreaDataButton_Click(object sender, EventArgs e)
        {
            areaDataUpDown.Value = areadataCopy;
        }
        private void pasteWeatherButton_Click(object sender, EventArgs e)
        {
            weatherComboBox.SelectedIndex = weatherCopy;
        }
        private void pasteMusicNightButton_Click(object sender, EventArgs e)
        {
            musicNightComboBox.SelectedIndex = musicnightCopy;
        }
        private void pasteCameraAngleButton_Click(object sender, EventArgs e)
        {
            cameraComboBox.SelectedIndex = camAngleCopy;
        }
        private void worldmapCoordsPasteButton_Click(object sender, EventArgs e)
        {
            worldmapXCoordUpDown.Value = worldmapXCoordCopy;
            worldmapYCoordUpDown.Value = worldmapYCoordCopy;
        }
        private void pasteMapSettingsButton_Click(object sender, EventArgs e)
        {
            currentHeader.flags = flagsCopy;
            battleBackgroundUpDown.Value = battleBGCopy;

            followModeComboBox.SelectedIndex = followingPokeCopy;
            kantoRadioButton.Checked = kantoFlagCopy;
            RefreshFlags();
        }
        #endregion

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
        public bool bldTexturesOn = true;
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
        public int Transparency = 128;
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
        private void FillBuildingsBox()
        {
            buildingsListBox.Items.Clear();

            uint id = 0;

            for (int i = 0; i < currentMapFile.buildings.Count; i++)
            {
                id = currentMapFile.buildings[i].modelID;
                string baseName = (i + 1).ToString("D2") + MapHeader.nameSeparator;
                try
                {
                    buildingsListBox.Items.Add(baseName + buildIndexComboBox.Items[(int)id]);
                }
                catch (ArgumentOutOfRangeException)
                {
                    DialogResult d = MessageBox.Show("Building #" + id + " couldn't be found in the Building List.\n" +
                        "Do you want to load Building 0 in its place?\n" +
                        "(Choosing \"Cancel\" will discard this building altogether.)", "Building not found", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                    if (d == DialogResult.Yes)
                    {
                        buildingsListBox.Items.Add(baseName + buildIndexComboBox.Items[0]);
                    }
                    else if (d == DialogResult.No)
                    {
                        buildingsListBox.Items.Add(baseName + "MISSING " + (int)id + '!');
                    } // else do nothing
                }
            }

        }
        private void MW_LoadModelTextures(NSBMD model, string textureFolder, int fileID)
        {
            if (fileID < 0)
            {
                return;
            }
            string texturePath = textureFolder + "\\" + fileID.ToString("D4");
            model.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(File.ReadAllBytes(texturePath)), out model.Textures, out model.Palettes);
            try
            {
                model.MatchTextures();
            }
            catch { }
        }
        public void RenderMap(ref NSBMDGlRenderer mapRenderer, ref NSBMDGlRenderer buildingsRenderer, ref MapFile mapFile, float ang, float dist, float elev, float perspective, int width, int height, bool mapTexturesON = true, bool buildingTexturesON = true)
        {
            #region Useless variables that the rendering API still needs
            MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File ani = new MKDS_Course_Editor.NSBTA.NSBTA.NSBTA_File();
            MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File tp = new MKDS_Course_Editor.NSBTP.NSBTP.NSBTP_File();
            MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File ca = new MKDS_Course_Editor.NSBCA.NSBCA.NSBCA_File();
            int[] aniframeS = new int[0];
            #endregion

            /* Invalidate drawing surfaces */
            mapOpenGlControl.Invalidate();
            EditorPanels.eventEditor.eventOpenGlControl.Invalidate();

            /* Adjust rendering settings */
            SetupRenderer(ang, dist, elev, perspective, width, height);

            /* Render the map model */
            mapRenderer.Model = mapFile.mapModel.models[0];
            Gl.glScalef(mapFile.mapModel.models[0].modelScale / 64, mapFile.mapModel.models[0].modelScale / 64, mapFile.mapModel.models[0].modelScale / 64);

            /* Determine if map textures must be rendered */
            if (!mapTexturesON)
            {
                Gl.glDisable(Gl.GL_TEXTURE_2D);
            }
            else
            {
                Gl.glEnable(Gl.GL_TEXTURE_2D);
            }

            mapRenderer.RenderModel("", ani, aniframeS, aniframeS, aniframeS, aniframeS, aniframeS, ca, false, -1, 0.0f, 0.0f, dist, elev, ang, true, tp, mapFile.mapModel); // Render map model

            if (!hideBuildings)
            {
                if (buildingTexturesON)
                {
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                }
                else
                {
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                }

                for (int i = 0; i < mapFile.buildings.Count; i++)
                {
                    NSBMD file = mapFile.buildings[i].NSBMDFile;
                    if (file is null)
                    {
                        Console.WriteLine("Null building can't be rendered");
                    }
                    else
                    {
                        buildingsRenderer.Model = file.models[0];
                        ScaleTranslateRotateBuilding(mapFile.buildings[i]);
                        buildingsRenderer.RenderModel("", ani, aniframeS, aniframeS, aniframeS, aniframeS, aniframeS, ca, false, -1, 0.0f, 0.0f, dist, elev, ang, true, tp, file);
                    }
                }
            }
        }
        private void ScaleTranslateRotateBuilding(Building building)
        {
            float fullXcoord = building.xPosition + building.xFraction / 65536f;
            float fullYcoord = building.yPosition + building.yFraction / 65536f;
            float fullZcoord = building.zPosition + building.zFraction / 65536f;

            float scaleFactor = building.NSBMDFile.models[0].modelScale / 1024;
            float translateFactor = 256 / building.NSBMDFile.models[0].modelScale;

            Gl.glScalef(scaleFactor * building.width, scaleFactor * building.height, scaleFactor * building.length);
            Gl.glTranslatef(fullXcoord * translateFactor / building.width, fullYcoord * translateFactor / building.height, fullZcoord * translateFactor / building.length);
            Gl.glRotatef(Building.U16ToDeg(building.xRotation), 1, 0, 0);
            Gl.glRotatef(Building.U16ToDeg(building.yRotation), 0, 1, 0);
            Gl.glRotatef(Building.U16ToDeg(building.zRotation), 0, 0, 1);
        }
        private void SetupRenderer(float ang, float dist, float elev, float perspective, int width, int height)
        {
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
        public void SetupMapEditor()
        {
            /* Extract essential NARCs sub-archives*/
            toolStripProgressBar.Visible = true;
            toolStripProgressBar.Maximum = 9;
            toolStripProgressBar.Value = 0;
            Helpers.statusLabelMessage("Attempting to unpack Map Editor NARCs... Please wait.");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.maps,
                DirNames.exteriorBuildingModels,
                DirNames.buildingConfigFiles,
                DirNames.buildingTextures,
                DirNames.mapTextures,
                DirNames.areaData,
            });

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.interiorBuildingModels });
            }

            Helpers.DisableHandlers();

            collisionPainterPictureBox.Image = new Bitmap(100, 100);
            typePainterPictureBox.Image = new Bitmap(100, 100);
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    mapPartsTabControl.TabPages.Remove(bgsTabPage);
                    break;
                default:
                    interiorbldRadioButton.Enabled = true;
                    exteriorbldRadioButton.Enabled = true;
                    break;
            }
            ;


            /* Add map names to box */
            selectMapComboBox.Items.Clear();
            int mapCount = romInfo.GetMapCount();

            for (int i = 0; i < mapCount; i++)
            {
                using (DSUtils.EasyReader reader = new DSUtils.EasyReader(RomInfo.gameDirs[DirNames.maps].unpackedDir + "\\" + i.ToString("D4")))
                {
                    switch (RomInfo.gameFamily)
                    {
                        case GameFamilies.DP:
                        case GameFamilies.Plat:
                            reader.BaseStream.Position = 0x10 + reader.ReadUInt32() + reader.ReadUInt32();
                            break;
                        default:
                            reader.BaseStream.Position = 0x12;
                            short bgsSize = reader.ReadInt16();
                            long backupPos = reader.BaseStream.Position;

                            reader.BaseStream.Position = 0;
                            reader.BaseStream.Position = backupPos + bgsSize + reader.ReadUInt32() + reader.ReadUInt32();
                            break;
                    }
                    ;

                    reader.BaseStream.Position += 0x14;
                    selectMapComboBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + NSBUtils.ReadNSBMDname(reader));
                }

            }
            toolStripProgressBar.Value++;

            /* Fill building models list */
            updateBuildingListComboBox(false);

            /*  Fill map textures list */
            mapTextureComboBox.Items.Clear();
            mapTextureComboBox.Items.Add("Untextured");
            for (int i = 0; i < romInfo.GetMapTexturesCount(); i++)
            {
                mapTextureComboBox.Items.Add("Map Texture Pack [" + i.ToString("D2") + "]");
            }
            toolStripProgressBar.Value++;

            /*  Fill building textures list */
            buildTextureComboBox.Items.Clear();
            buildTextureComboBox.Items.Add("Untextured");
            for (int i = 0; i < romInfo.GetBuildingTexturesCount(); i++)
            {
                buildTextureComboBox.Items.Add("Building Texture Pack [" + i.ToString("D2") + "]");
            }

            toolStripProgressBar.Value++;

            collisionPainterComboBox.Items.Clear();
            foreach (string s in PokeDatabase.System.MapCollisionPainters.Values)
            {
                collisionPainterComboBox.Items.Add(s);
            }

            collisionTypePainterComboBox.Items.Clear();
            foreach (string s in PokeDatabase.System.MapCollisionTypePainters.Values)
            {
                collisionTypePainterComboBox.Items.Add(s);
            }

            toolStripProgressBar.Value++;

            /* Set controls' initial values */
            selectCollisionPanel.BackColor = Color.MidnightBlue;
            collisionTypePainterComboBox.SelectedIndex = 0;
            collisionPainterComboBox.SelectedIndex = 1;

            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;
            Helpers.EnableHandlers();

            //Default selections
            selectMapComboBox.SelectedIndex = 0;
            exteriorbldRadioButton.Checked = true;
            switch (RomInfo.gameFamily)
            {
                case GameFamilies.DP:
                case GameFamilies.Plat:
                    mapTextureComboBox.SelectedIndex = 7;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
                case GameFamilies.HGSS:
                    mapTextureComboBox.SelectedIndex = 3;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
                default:
                    mapTextureComboBox.SelectedIndex = 2;
                    buildTextureComboBox.SelectedIndex = 1;
                    break;
            }
            ;

            Helpers.statusLabelMessage();
        }
        private void addMapFileButton_Click(object sender, EventArgs e)
        {
            /* Add new map file to map folder */
            new MapFile(0, RomInfo.gameFamily, discardMoveperms: true).SaveToFileDefaultDir(selectMapComboBox.Items.Count);

            /* Update ComboBox and select new file */
            selectMapComboBox.Items.Add(selectMapComboBox.Items.Count.ToString("D3") + MapHeader.nameSeparator + "newmap");
            selectMapComboBox.SelectedIndex = selectMapComboBox.Items.Count - 1;
        }
        private void replaceMapBinButton_Click(object sender, EventArgs e)
        {
            /* Prompt user to select .bin file */
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = "Map BIN File (*.bin)|*.bin"
            };
            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            MapFile temp = new MapFile(of.FileName, RomInfo.gameFamily, false);

            if (temp.correctnessFlag)
            {
                UpdateMapBinAndRefresh(temp, "Map BIN imported successfully!");
                return;
            }
            else
            {
                if (RomInfo.gameFamily == GameFamilies.HGSS)
                {
                    //If HGSS didn't work try reading as Platinum Map
                    temp = new MapFile(of.FileName, GameFamilies.Plat, false);
                }
                else
                {
                    //If Plat didn't work try reading as HGSS Map
                    temp = new MapFile(of.FileName, GameFamilies.HGSS, false);
                }

                if (temp.correctnessFlag)
                {
                    UpdateMapBinAndRefresh(temp, "Map BIN imported and adapted successfully!");
                    return;
                }
            }

            MessageBox.Show("The BIN file you imported is corrupted!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void UpdateMapBinAndRefresh(MapFile newerVersion, string message)
        {
            currentMapFile = newerVersion;

            /* Update map BIN file */
            currentMapFile.SaveToFileDefaultDir(selectMapComboBox.SelectedIndex, showSuccessMessage: false);

            /* Refresh controls */
            selectMapComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buildTextureComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int btIndex = buildTextureComboBox.SelectedIndex;

            if (Helpers.HandlersDisabled || btIndex < 0)
            {
                return;
            }

            if (btIndex == 0)
            {
                bldTexturesOn = false;
            }
            else
            {
                string texturePath = RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir + "\\" + (btIndex - 1).ToString("D4");
                byte[] textureFile = File.ReadAllBytes(texturePath);

                Stream str = new MemoryStream(textureFile);
                foreach (Building building in currentMapFile.buildings)
                {
                    str.Position = 0;
                    NSBMD file = building.NSBMDFile;

                    if (file != null)
                    {
                        file.materials = NSBTXLoader.LoadNsbtx(str, out file.Textures, out file.Palettes);

                        try
                        {
                            file.MatchTextures();
                            bldTexturesOn = true;
                        }
                        catch
                        {
                            string itemAtIndex = buildTextureComboBox.Items[btIndex].ToString();
                            if (!itemAtIndex.StartsWith("Error!"))
                            {
                                Helpers.DisableHandlers();
                                buildTextureComboBox.Items[btIndex] = itemAtIndex.Insert(0, "Error! - ");
                                Helpers.EnableHandlers();
                            }
                            bldTexturesOn = false;
                        }
                    }
                }
                //buildTextureComboBox.Items[buildTextureComboBox.SelectedIndex] = "Error - Building Texture Pack too small [" + (buildTextureComboBox.SelectedIndex - 1).ToString("D2") + "]";
            }

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapTextureComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            if (mapTextureComboBox.SelectedIndex == 0)
                mapTexturesOn = false;
            else
            {
                mapTexturesOn = true;

                string texturePath = RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4");
                currentMapFile.mapModel.materials = NSBTXLoader.LoadNsbtx(new MemoryStream(File.ReadAllBytes(texturePath)), out currentMapFile.mapModel.Textures, out currentMapFile.mapModel.Palettes);
                try
                {
                    currentMapFile.mapModel.MatchTextures();
                }
                catch { }
            }
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapEditorTabPage_Enter(object sender, EventArgs e)
        {
            mapOpenGlControl.MakeCurrent();
            if (selectMapComboBox.SelectedIndex > -1)
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        public void mapOpenGlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (mapPartsTabControl.SelectedTab == buildingsTabPage && bldPlaceWithMouseCheckbox.Checked)
            {
                return;
            }
            dist -= (float)e.Delta / 200;
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapOpenGlControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            byte multiplier = 2;
            if (e.Modifiers == Keys.Shift)
            {
                multiplier = 1;
            }
            else if (e.Modifiers == Keys.Control)
            {
                multiplier = 4;
            }

            switch (e.KeyCode)
            {
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

            if (rRot ^ lRot)
            {
                if (rRot)
                {
                    ang += 1 * multiplier;
                }
                else if (lRot)
                {
                    ang -= 1 * multiplier;
                }
            }

            if (uRot ^ dRot)
            {
                if (uRot)
                {
                    elev -= 1 * multiplier;
                }
                else if (dRot)
                {
                    elev += 1 * multiplier;
                }
            }

            mapOpenGlControl.Invalidate();
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapOpenGlControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
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
        private void mapOpenGlControl_Click(object sender, EventArgs e)
        {
            if (radio2D.Checked && bldPlaceWithMouseCheckbox.Checked)
            {
                PointF coordinates = mapRenderPanel.PointToClient(Cursor.Position);
                PointF mouseTilePos = new PointF(coordinates.X / mapEditorSquareSize, coordinates.Y / mapEditorSquareSize);

                if (buildingsListBox.SelectedIndex > -1)
                {
                    if (!bldPlaceLockXcheckbox.Checked)
                        xBuildUpDown.Value = (decimal)(Math.Round(mouseTilePos.X, bldDecimalPositions) - 16);
                    if (!bldPlaceLockZcheckbox.Checked)
                        zBuildUpDown.Value = (decimal)(Math.Round(mouseTilePos.Y, bldDecimalPositions) - 16);
                }
            }
        }
        private void bldRoundWhole_CheckedChanged(object sender, EventArgs e)
        {
            bldDecimalPositions = 0;
        }
        private void bldRoundDec_CheckedChanged(object sender, EventArgs e)
        {
            bldDecimalPositions = 1;
        }
        private void bldRoundCent_CheckedChanged(object sender, EventArgs e)
        {
            bldDecimalPositions = 2;
        }
        private void bldRoundMil_CheckedChanged(object sender, EventArgs e)
        {
            bldDecimalPositions = 3;
        }
        private void bldRoundDecmil_CheckedChanged(object sender, EventArgs e)
        {
            bldDecimalPositions = 4;
        }
        private void bldRoundCentMil_CheckedChanged(object sender, EventArgs e)
        {
            bldDecimalPositions = 5;
        }
        private void bldPlaceWithMouseCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            bool status = bldPlaceWithMouseCheckbox.Checked && radio2D.Checked;
            bldPlaceLockXcheckbox.Enabled = status;
            bldPlaceLockZcheckbox.Enabled = status;
            bldRoundGroupbox.Enabled = status;
            lockXZgroupbox.Enabled = status;

            if (status)
            {
                SetCam2D();
            }
        }
        private void bldPlaceLockXcheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ExclusiveCBInvert(bldPlaceLockZcheckbox);
        }

        private void bldPlaceLockZcheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ExclusiveCBInvert(bldPlaceLockXcheckbox);
        }
        private void mapPartsTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mapPartsTabControl.SelectedTab == buildingsTabPage)
            {
                radio2D.Checked = false;

                hideBuildings = false;
                radio3D.Enabled = true;
                radio2D.Enabled = true;
                wireframeCheckBox.Enabled = true;

                mapOpenGlControl.BringToFront();

                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
            else if (mapPartsTabControl.SelectedTab == permissionsTabPage)
            {
                radio2D.Checked = true;

                hideBuildings = false;
                radio3D.Enabled = false;
                radio2D.Enabled = false;
                wireframeCheckBox.Enabled = false;

                SetCam2D();
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

                movPictureBox.BackgroundImage = GrabMapScreenshot(movPictureBox.Width, movPictureBox.Height);
                movPictureBox.BringToFront();
            }
            else if (mapPartsTabControl.SelectedTab == modelTabPage)
            {
                radio2D.Checked = false;

                hideBuildings = true;
                radio3D.Enabled = true;
                radio2D.Enabled = true;
                wireframeCheckBox.Enabled = true;

                mapOpenGlControl.BringToFront();

                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
            else
            { // Terrain and BGS
                radio2D.Checked = true;

                hideBuildings = false;
                radio3D.Enabled = false;
                radio2D.Enabled = false;
                wireframeCheckBox.Enabled = false;

                mapOpenGlControl.BringToFront();

                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
        }
        private void radio2D_CheckedChanged(object sender, EventArgs e)
        {
            bool _2dmodeSelected = radio2D.Checked;

            if (_2dmodeSelected)
            {
                SetCam2D();
            }
            else
            {
                SetCam3D();
            }

            bldPlaceWithMouseCheckbox.Enabled = _2dmodeSelected;
            radio3D.Checked = !_2dmodeSelected;

            bldPlaceWithMouseCheckbox_CheckedChanged(null, null);
        }
        private void SetCam2D()
        {
            perspective = 4f;
            ang = 0f;
            dist = 115.2f;
            elev = 90f;

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void SetCam3D()
        {
            perspective = 45f;
            ang = 0f;
            dist = 12.8f;
            elev = 50.0f;

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
                ang, dist, elev, perspective,
                mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        private void mapScreenshotButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Choose where to save the map screenshot.", "Choose destination path", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SaveFileDialog imageSFD = new SaveFileDialog
            {
                Filter = "PNG File(*.png)|*.png",
            };
            if (imageSFD.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile,
            ang, dist, elev, perspective,
            mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            int newW = 512, newH = 512;
            Bitmap newImage = new Bitmap(newW, newH);
            using (var graphCtr = Graphics.FromImage(newImage))
            {
                graphCtr.SmoothingMode = SmoothingMode.HighQuality;
                graphCtr.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphCtr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphCtr.DrawImage(GrabMapScreenshot(mapOpenGlControl.Width, mapOpenGlControl.Height), 0, 0, newW, newH);
            }
            newImage.Save(imageSFD.FileName);
            MessageBox.Show("Screenshot saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void removeLastMapFileButton_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Are you sure you want to delete the last Map BIN File?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d.Equals(DialogResult.Yes))
            {
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
        private void saveMapButton_Click(object sender, EventArgs e)
        {
            currentMapFile.SaveToFileDefaultDir(selectMapComboBox.SelectedIndex);
        }
        private void exportCurrentMapBinButton_Click(object sender, EventArgs e)
        {
            currentMapFile.SaveToFileExplorePath(selectMapComboBox.SelectedItem.ToString());
        }
        public void selectMapComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            /* Load map data into MapFile class instance */
            currentMapFile = new MapFile(selectMapComboBox.SelectedIndex, RomInfo.gameFamily);

            /* Load map textures for renderer */
            if (mapTextureComboBox.SelectedIndex > 0)
            {
                MW_LoadModelTextures(currentMapFile.mapModel, RomInfo.gameDirs[DirNames.mapTextures].unpackedDir, mapTextureComboBox.SelectedIndex - 1);
            }

            /* Load buildings nsbmd and textures for renderer into MapFile's building objects */
            for (int i = 0; i < currentMapFile.buildings.Count; i++)
            {
                currentMapFile.buildings[i].LoadModelData(romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked)); // Load building nsbmd
                if (buildTextureComboBox.SelectedIndex > 0)
                {
                    MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
                }
            }

            /* Render the map */
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            /* Draw permissions in the small selection boxes */
            DrawSmallCollision();
            DrawSmallTypeCollision();

            /* Draw selected permissions category */
            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
            {
                DrawCollisionGrid();
            }
            else
            {
                DrawTypeGrid();
            }
            /* Set map screenshot as background picture in permissions editor PictureBox */
            movPictureBox.BackgroundImage = GrabMapScreenshot(movPictureBox.Width, movPictureBox.Height);

            RestorePainter();

            /* Fill buildings ListBox, and if not empty select first item */
            FillBuildingsBox();
            if (buildingsListBox.Items.Count > 0)
            {
                buildingsListBox.SelectedIndex = 0;
            }

            modelSizeLBL.Text = currentMapFile.mapModelData.Length.ToString() + " B";
            terrainSizeLBL.Text = currentMapFile.bdhc.Length.ToString() + " B";

            if (RomInfo.gameFamily == GameFamilies.HGSS)
            {
                BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            }
        }
        private void wireframeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (wireframeCheckBox.Checked)
            {
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
            }
            else
            {
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            }

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        #region Building Editor
        private void addBuildingButton_Click(object sender, EventArgs e)
        {
            AddBuildingToMap(new Building());
        }

        private void duplicateBuildingButton_Click(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex > -1)
            {
                AddBuildingToMap(new Building(currentMapFile.buildings[buildingsListBox.SelectedIndex]));
            }
        }

        private void AddBuildingToMap(Building b)
        {
            currentMapFile.buildings.Add(b);

            /* Load new building's model and textures for the renderer */
            b.LoadModelData(romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked));
            MW_LoadModelTextures(b.NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1);
            currentMapFile.buildings[currentMapFile.buildings.Count - 1] = b;

            /* Add new entry to buildings ListBox */
            buildingsListBox.Items.Add((buildingsListBox.Items.Count + 1).ToString("D2") + MapHeader.nameSeparator + buildIndexComboBox.Items[(int)b.modelID]);
            buildingsListBox.SelectedIndex = buildingsListBox.Items.Count - 1;

            /* Redraw scene with new building */
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        private void buildIndexComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || buildingsListBox.SelectedIndex < 0)
            {
                return;
            }

            Helpers.DisableHandlers();
            buildingsListBox.Items[buildingsListBox.SelectedIndex] = (buildingsListBox.SelectedIndex + 1).ToString("D2") + MapHeader.nameSeparator + buildIndexComboBox.SelectedItem;
            Helpers.EnableHandlers();

            currentMapFile.buildings[buildingsListBox.SelectedIndex].modelID = (uint)buildIndexComboBox.SelectedIndex;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].LoadModelData(romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked));
            MW_LoadModelTextures(currentMapFile.buildings[buildingsListBox.SelectedIndex].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1);

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        private void buildingsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int buildingNumber = buildingsListBox.SelectedIndex;
            if (Helpers.HandlersDisabled || buildingNumber < 0)
            {
                return;
            }
            Helpers.BackUpDisableHandler();
            Helpers.DisableHandlers();

            Building selected = currentMapFile.buildings[buildingNumber];
            if (selected.NSBMDFile != null)
            {
                buildIndexComboBox.SelectedIndex = (int)selected.modelID;

                xBuildUpDown.Value = selected.xPosition + (decimal)selected.xFraction / 65535;
                yBuildUpDown.Value = selected.yPosition + (decimal)selected.yFraction / 65535;
                zBuildUpDown.Value = selected.zPosition + (decimal)selected.zFraction / 65535;

                xRotBuildUpDown.Value = selected.xRotation;
                yRotBuildUpDown.Value = selected.yRotation;
                zRotBuildUpDown.Value = selected.zRotation;

                xRotDegBldUpDown.Value = (decimal)Building.U16ToDeg((ushort)xRotBuildUpDown.Value);
                yRotDegBldUpDown.Value = (decimal)Building.U16ToDeg((ushort)yRotBuildUpDown.Value);
                zRotDegBldUpDown.Value = (decimal)Building.U16ToDeg((ushort)zRotBuildUpDown.Value);

                buildingWidthUpDown.Value = selected.width;
                buildingHeightUpDown.Value = selected.height;
                buildingLengthUpDown.Value = selected.length;
            }

            Helpers.RestoreDisableHandler();
        }

        private void xRotBuildUpDown_ValueChanged(object sender, EventArgs e)
        {
            int selection = buildingsListBox.SelectedIndex;

            if (selection <= -1 || Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();
            currentMapFile.buildings[selection].xRotation = (ushort)((int)xRotBuildUpDown.Value & ushort.MaxValue);
            xRotDegBldUpDown.Value = (decimal)Building.U16ToDeg(currentMapFile.buildings[selection].xRotation);
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.EnableHandlers();
            Console.WriteLine("X Rot " + currentMapFile.buildings[selection].xRotation.ToString());
        }

        private void yRotBuildUpDown_ValueChanged(object sender, EventArgs e)
        {
            int selection = buildingsListBox.SelectedIndex;

            if (selection <= -1 || Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();

            yRotDegBldUpDown.Value = (decimal)Building.U16ToDeg(currentMapFile.buildings[selection].yRotation = (ushort)((int)yRotBuildUpDown.Value & ushort.MaxValue));
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.EnableHandlers();
        }

        private void zRotBuildUpDown_ValueChanged(object sender, EventArgs e)
        {
            int selection = buildingsListBox.SelectedIndex;

            if (selection <= -1 || Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();

            zRotDegBldUpDown.Value = (decimal)Building.U16ToDeg(currentMapFile.buildings[selection].zRotation = (ushort)((int)zRotBuildUpDown.Value & ushort.MaxValue));
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.EnableHandlers();
        }

        private void xRotDegBldUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex <= -1 || Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();

            currentMapFile.buildings[buildingsListBox.SelectedIndex].xRotation = (ushort)(xRotBuildUpDown.Value =
                Building.DegToU16((float)xRotDegBldUpDown.Value));
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.EnableHandlers();
        }

        private void yRotDegBldUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex <= -1 || Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();

            currentMapFile.buildings[buildingsListBox.SelectedIndex].yRotation = (ushort)(yRotBuildUpDown.Value =
                Building.DegToU16((float)yRotDegBldUpDown.Value));
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.EnableHandlers();
        }

        private void zRotDegBldUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex <= -1 || Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();

            currentMapFile.buildings[buildingsListBox.SelectedIndex].zRotation = (ushort)(zRotBuildUpDown.Value =
                Building.DegToU16((float)zRotDegBldUpDown.Value));
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            Helpers.EnableHandlers();
        }

        private void buildingHeightUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex > -1)
            {
                currentMapFile.buildings[buildingsListBox.SelectedIndex].height = (uint)buildingHeightUpDown.Value;
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
        }

        private void buildingLengthUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex > -1)
            {
                currentMapFile.buildings[buildingsListBox.SelectedIndex].length = (uint)buildingLengthUpDown.Value;
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
        }

        private void buildingWidthUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (buildingsListBox.SelectedIndex > -1)
            {
                currentMapFile.buildings[buildingsListBox.SelectedIndex].width = (uint)buildingWidthUpDown.Value;
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            }
        }

        private void exportBuildingsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = MapFile.BuildingsFilter,
                FileName = selectMapComboBox.SelectedItem.ToString()
            };
            if (sf.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            File.WriteAllBytes(sf.FileName, currentMapFile.BuildingsToByteArray());

            MessageBox.Show("Buildings exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void importBuildingsButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ib = new OpenFileDialog
            {
                Filter = MapFile.BuildingsFilter
            };
            if (ib.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            currentMapFile.ImportBuildings(File.ReadAllBytes(ib.FileName));
            FillBuildingsBox();
            if (buildingsListBox.Items.Count > 0) { buildingsListBox.SelectedIndex = 0; }

            for (int i = 0; i < currentMapFile.buildings.Count; i++)
            {
                currentMapFile.buildings[i].LoadModelData(romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked)); // Load building nsbmd
                MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
            }

            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            MessageBox.Show("Buildings imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void interiorRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.DisableHandlers();
            int index = buildIndexComboBox.SelectedIndex;
            buildIndexComboBox.Items.Clear();

            /* Fill building models list */
            updateBuildingListComboBox(interiorbldRadioButton.Checked);
            FillBuildingsBox();

            try
            {
                buildIndexComboBox.SelectedIndex = index;
            }
            catch (ArgumentOutOfRangeException)
            {
                buildIndexComboBox.SelectedIndex = 0;
                currentMapFile.buildings[buildIndexComboBox.SelectedIndex].modelID = 0;
            }

            /* Load buildings nsbmd and textures for renderer into MapFile's building objects */
            for (int i = 0; i < currentMapFile.buildings.Count; i++)
            {
                currentMapFile.buildings[i].LoadModelData(romInfo.GetBuildingModelsDirPath(interiorbldRadioButton.Checked)); // Load building nsbmd
                MW_LoadModelTextures(currentMapFile.buildings[i].NSBMDFile, RomInfo.gameDirs[DirNames.buildingTextures].unpackedDir, buildTextureComboBox.SelectedIndex - 1); // Load building textures                
            }

            /* Render the map */
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
            Helpers.EnableHandlers();
        }

        private void removeBuildingButton_Click(object sender, EventArgs e)
        {
            int toRemoveListBoxID = buildingsListBox.SelectedIndex;
            if (toRemoveListBoxID > -1)
            {
                Helpers.DisableHandlers();

                /* Remove building object from list and the corresponding entry in the ListBox */

                currentMapFile.buildings.RemoveAt(toRemoveListBoxID);
                buildingsListBox.Items.RemoveAt(toRemoveListBoxID);

                FillBuildingsBox(); // Update ListBox
                RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

                Helpers.EnableHandlers();

                if (buildingsListBox.Items.Count > 0)
                {
                    if (toRemoveListBoxID > 0)
                    {
                        buildingsListBox.SelectedIndex = toRemoveListBoxID - 1;
                    }
                    else
                    {
                        buildingsListBox.SelectedIndex = 0;
                    }
                }
            }
        }

        private void xBuildUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || buildingsListBox.SelectedIndex < 0)
            {
                return;
            }

            var wholePart = Math.Truncate(xBuildUpDown.Value);
            var decPart = xBuildUpDown.Value - wholePart;

            if (decPart < 0)
            {
                decPart += 1;
                wholePart -= 1;
            }

            currentMapFile.buildings[buildingsListBox.SelectedIndex].xPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].xFraction = (ushort)(decPart * 65535);
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        private void zBuildUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || buildingsListBox.SelectedIndex < 0)
                return;

            var wholePart = Math.Truncate(zBuildUpDown.Value);
            var decPart = zBuildUpDown.Value - wholePart;

            if (decPart < 0)
            {
                decPart += 1;
                wholePart -= 1;
            }

            currentMapFile.buildings[buildingsListBox.SelectedIndex].zPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].zFraction = (ushort)(decPart * 65535);
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }

        private void yBuildUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled || buildingsListBox.SelectedIndex < 0)
                return;

            var wholePart = Math.Truncate(yBuildUpDown.Value);
            var decPart = yBuildUpDown.Value - wholePart;

            if (decPart < 0)
            {
                decPart += 1;
                wholePart -= 1;
            }

            currentMapFile.buildings[buildingsListBox.SelectedIndex].yPosition = (short)wholePart;
            currentMapFile.buildings[buildingsListBox.SelectedIndex].yFraction = (ushort)(decPart * 65535);
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);
        }
        #endregion

        #region Movement Permissions Editor

        #region Subroutines
        private Bitmap GrabMapScreenshot(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Gl.glReadPixels(0, 0, width, height, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }
        private void DrawCollisionGrid()
        {
            Bitmap mainBm = new Bitmap(608, 608);
            using (Graphics gMain = Graphics.FromImage(mainBm))
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
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
        private void DrawSmallCollision()
        {
            Bitmap smallBm = new Bitmap(100, 100);
            using (Graphics gSmall = Graphics.FromImage(smallBm))
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
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
        private void DrawTypeGrid()
        {
            Bitmap mainBm = new Bitmap(608, 608);
            using (Graphics gMain = Graphics.FromImage(mainBm))
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        PrepareTypePainterGraphics(currentMapFile.types[i, j]);

                        /* Draw cell with color */
                        mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                        gMain.DrawRectangle(paintPen, mainCell);
                        gMain.FillRectangle(paintBrush, mainCell);

                        /* Draw byte on cell */
                        StringFormat sf = new StringFormat
                        {
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
        private void DrawSmallTypeCollision()
        {
            Bitmap smallBm = new Bitmap(100, 100);
            using (Graphics gSmall = Graphics.FromImage(smallBm))
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        PrepareTypePainterGraphics(currentMapFile.types[i, j]);

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
        private void scanUsedCollisionTypesButton_Click(object sender, EventArgs e)
        {
            SortedSet<byte> allUsed = FindUsedCollisions();

            List<byte> lst = allUsed.ToList();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < allUsed.Count; i++)
            {
                sb.Append("0x");
                sb.Append(lst[i].ToString("X2"));

                if (i != allUsed.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            string report = sb.ToString();

            MessageBox.Show($"This report has been copied to the clipboard as well, for your convenience.\n\nUsed types (in all Map BINs): \n{report}", "Used collision types report", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Clipboard.SetText(report);
        }

        private SortedSet<byte> FindUsedCollisions()
        {
            int mapCount = romInfo.GetMapCount();

            SortedSet<byte> allUsedTypes = new SortedSet<byte>();

            for (int i = 0; i < mapCount; i++)
            {
                allUsedTypes.UnionWith(new MapFile(i, gameFamily, false, false).GetUsedTypes());
            }

            return allUsedTypes;
        }
        private SortedSet<byte> FindUnusedCollisions()
        {
            int mapCount = romInfo.GetMapCount();

            SortedSet<byte> allUnusedTypes = new SortedSet<byte>();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                allUnusedTypes.Add((byte)i);
            }
            allUnusedTypes.ExceptWith(FindUsedCollisions());

            return allUnusedTypes;
        }
        private void EditCell(int xPosition, int yPosition)
        {
            try
            {
                mainCell = new Rectangle(xPosition * mapEditorSquareSize, yPosition * mapEditorSquareSize, mapEditorSquareSize, mapEditorSquareSize);
                smallCell = new Rectangle(xPosition * 3, yPosition * 3, 3, 3);

                using (Graphics mainG = Graphics.FromImage(movPictureBox.Image))
                {
                    /*  Draw new cell on main grid */
                    mainG.SetClip(mainCell);
                    mainG.Clear(Color.Transparent);
                    mainG.DrawRectangle(paintPen, mainCell);
                    mainG.FillRectangle(paintBrush, mainCell);
                    if (selectTypePanel.BackColor == Color.MidnightBlue)
                    {
                        sf = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };
                        mainG.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        mainG.DrawString(paintByte.ToString("X2"), textFont, textBrush, mainCell, sf);
                    }
                }

                if (selectCollisionPanel.BackColor == Color.MidnightBlue)
                {
                    using (Graphics smallG = Graphics.FromImage(collisionPictureBox.Image))
                    {
                        /* Draw new cell on small grid */
                        smallG.SetClip(smallCell);
                        smallG.Clear(Color.Transparent);
                        smallG.DrawRectangle(paintPen, smallCell);
                        smallG.FillRectangle(paintBrush, smallCell);
                    }
                    currentMapFile.collisions[yPosition, xPosition] = paintByte;
                    collisionPictureBox.Invalidate();
                }
                else
                {
                    using (Graphics smallG = Graphics.FromImage(typePictureBox.Image))
                    {
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
            }
            catch { return; }
        }
        private void FloodFillUtil(byte[,] screen, int x, int y, byte prevC, byte newC, int sizeX, int sizeY)
        {
            // Base cases 
            if (x < 0 || x >= sizeX || y < 0 || y >= sizeY)
            {
                return;
            }

            if (screen[y, x] != prevC)
            {
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
        private void FloodFillCell(int x, int y)
        {
            byte toPaint = paintByte;
            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
            {
                if (currentMapFile.collisions[y, x] != paintByte)
                {
                    FloodFillUtil(currentMapFile.collisions, x, y, currentMapFile.collisions[y, x], paintByte, 32, 32);
                    DrawCollisionGrid();
                    DrawSmallCollision();
                    PrepareCollisionPainterGraphics(paintByte);
                }
            }
            else
            {
                if (currentMapFile.types[y, x] != paintByte)
                {
                    FloodFillUtil(currentMapFile.types, x, y, currentMapFile.types[y, x], paintByte, 32, 32);
                    DrawTypeGrid();
                    DrawSmallTypeCollision();
                    PrepareTypePainterGraphics(paintByte);
                }
            }

            /* Draw permissions in the small selection boxes */


        }
        private void RestorePainter()
        {
            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
            {
                collisionPainterComboBox_SelectedIndexChange(null, null);
            }
            else if (collisionTypePainterComboBox.Enabled)
            {
                typePainterComboBox_SelectedIndexChanged(null, null);
            }
            else
            {
                typePainterUpDown_ValueChanged(null, null);
            }
        }
        private void PrepareCollisionPainterGraphics(byte collisionValue)
        {
            switch (collisionValue)
            {
                case 0x01: // Snow
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Lavender));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Lavender));
                    break;
                case 0x02: // Leaves
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.ForestGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.ForestGreen));
                    break;
                case 0x04: // Grass
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.LimeGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.LimeGreen));
                    break;
                case 0x06: // Stairs and ice
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.PowderBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.PowderBlue));
                    break;
                case 0x07: // Metal
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Silver));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Silver));
                    break;
                case 0x0A: // Stone
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.DimGray));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.DimGray));
                    break;
                case 0x0D: // Wood
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.SaddleBrown));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.SaddleBrown));
                    break;
                case 0x80:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Red));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Red));
                    break;
                default: // 0x00 - Walkeable               
                    paintPen = new Pen(Color.FromArgb(32, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(32, Color.White));
                    break;
            }
        }

        private void PrepareTypePainterGraphics(byte typeValue)
        {
            switch (typeValue)
            {
                case 0x0:
                    paintPen = new Pen(Color.FromArgb(32, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(32, Color.White));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x2:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.LimeGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.LimeGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x3:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Green));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Green));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x6:
                    paintPen = new Pen(Color.FromArgb(128, Color.YellowGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.YellowGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x7:
                    paintPen = new Pen(Color.FromArgb(128, Color.DarkGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.DarkGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x8:
                    paintPen = new Pen(Color.FromArgb(128, Color.BurlyWood));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.BurlyWood));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x9:
                    paintPen = new Pen(Color.FromArgb(128, Color.SlateBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.SlateBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0xA:
                    paintPen = new Pen(Color.FromArgb(128, Color.Tomato));
                    paintBrush = new SolidBrush(Color.FromArgb(128, Color.Tomato));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0xC:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.BurlyWood));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.BurlyWood));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x10:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.SkyBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.SkyBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x13:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.SteelBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.SteelBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x15:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.RoyalBlue));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.RoyalBlue));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x16:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.LightSlateGray));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.LightSlateGray));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x20:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Cyan));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Cyan));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x21:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.PeachPuff));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.PeachPuff));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x30:
                case 0x31:
                case 0x32:
                case 0x33:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Red));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Red));
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
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Maroon));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Maroon));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x40:
                case 0x41:
                case 0x42:
                case 0x43:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Gold));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Gold));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x4B:
                case 0x4C:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Sienna));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Sienna));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 9.0f);
                    break;
                case 0x5E:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x5F:
                case 0x62:
                case 0x63:
                case 0x64:
                case 0x65:
                case 0x69:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0x6C:
                case 0x6D:
                case 0x6E:
                case 0x6F:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.DarkOrchid));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.DarkOrchid));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0xA1:
                case 0xA2:
                case 0xA3:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Honeydew));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Honeydew));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0xA4:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.Peru));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.Peru));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                case 0xA6:
                    paintPen = new Pen(Color.FromArgb(Transparency, Color.SeaGreen));
                    paintBrush = new SolidBrush(Color.FromArgb(Transparency, Color.SeaGreen));
                    textBrush = new SolidBrush(Color.White);
                    textFont = new Font("Arial", 8.65f);
                    break;
                default:
                    paintPen = new Pen(Color.FromArgb(32, Color.White));
                    paintBrush = new SolidBrush(Color.FromArgb(32, Color.White));
                    textBrush = new SolidBrush(Color.Black);
                    textFont = new Font("Arial", 8.65f);
                    break;
            }
        }
        #endregion

        private void clearCurrentButton_Click(object sender, EventArgs e)
        {
            PictureBox smallBox = selectCollisionPanel.BackColor == Color.MidnightBlue ? collisionPictureBox : typePictureBox;

            using (Graphics smallG = Graphics.FromImage(smallBox.Image))
            {
                using (Graphics mainG = Graphics.FromImage(movPictureBox.Image))
                {
                    smallG.Clear(Color.Transparent);
                    mainG.Clear(Color.Transparent);
                    PrepareCollisionPainterGraphics(0x0);

                    for (int i = 0; i < 32; i++)
                    {
                        for (int j = 0; j < 32; j++)
                        {
                            mainCell = new Rectangle(19 * j, 19 * i, 19, 19);
                            mainG.DrawRectangle(paintPen, mainCell);
                            mainG.FillRectangle(paintBrush, mainCell);
                        }
                    }
                }
            }

            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
            {
                currentMapFile.collisions = new byte[32, 32]; // Set all collision bytes to clear (0x0)               
            }
            else
            {
                currentMapFile.types = new byte[32, 32]; // Set all type bytes to clear (0x0)
            }

            movPictureBox.Invalidate(); // Refresh main image
            smallBox.Invalidate();
            RestorePainter();
        }

        private void collisionPictureBox_Click(object sender, EventArgs e)
        {
            selectTypePanel.BackColor = Color.Transparent;
            typeGroupBox.Enabled = false;
            selectCollisionPanel.BackColor = Color.MidnightBlue;
            collisionGroupBox.Enabled = true;

            DrawCollisionGrid();
            RestorePainter();
        }
        private void exportMovButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog em = new SaveFileDialog
            {
                Filter = MapFile.MovepermsFilter,
                FileName = selectMapComboBox.SelectedItem.ToString()
            };
            if (em.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            File.WriteAllBytes(em.FileName, currentMapFile.CollisionsToByteArray());

            MessageBox.Show("Permissions exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void importMovButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ip = new OpenFileDialog
            {
                Filter = MapFile.MovepermsFilter
            };
            if (ip.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            currentMapFile.ImportPermissions(File.ReadAllBytes(ip.FileName));

            DrawSmallCollision();
            DrawSmallTypeCollision();

            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
            {
                DrawCollisionGrid();
            }
            else
            {
                DrawTypeGrid();
            }
            RestorePainter();

            MessageBox.Show("Permissions imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void movPictureBox_Click(object sender, EventArgs e)
        {
            MouseEventArgs mea = (MouseEventArgs)e;

            int xCoord = movPictureBox.PointToClient(MousePosition).X / mapEditorSquareSize;
            int yCoord = movPictureBox.PointToClient(MousePosition).Y / mapEditorSquareSize;

            if (mea.Button == MouseButtons.Middle)
            {
                FloodFillCell(xCoord, yCoord);
            }
            else if (mea.Button == MouseButtons.Left)
            {
                EditCell(xCoord, yCoord);
            }
            else
            {
                if (selectCollisionPanel.BackColor == Color.MidnightBlue)
                {
                    byte newValue = currentMapFile.collisions[yCoord, xCoord];
                    updateCollisions(newValue);
                }
                else
                {
                    byte newValue = currentMapFile.types[yCoord, xCoord];
                    typePainterUpDown.Value = newValue;
                    updateTypeCollisions(newValue);
                }
                ;
            }
        }
        private void movPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            {
                EditCell(e.Location.X / mapEditorSquareSize, e.Location.Y / mapEditorSquareSize);
            }
        }
        private void collisionPainterComboBox_SelectedIndexChange(object sender, EventArgs e)
        {
            byte? collisionByte = StringToCollisionByte((string)collisionPainterComboBox.SelectedItem);

            if (collisionByte != null)
            {
                updateCollisions((byte)collisionByte);
            }
        }
        private void typePainterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte? collisionByte = StringToCollisionByte((string)collisionTypePainterComboBox.SelectedItem);

            if (collisionByte != null)
            {
                updateTypeCollisions((byte)collisionByte);
            }
        }

        private byte? StringToCollisionByte(string selectedItem)
        {
            byte? result;
            try
            {
                result = Convert.ToByte(selectedItem.Substring(1, 2), 16);
            }
            catch (FormatException)
            {
                Console.WriteLine("Format incompatible");
                result = null;
            }
            return result;
        }
        private void typePainterUpDown_ValueChanged(object sender, EventArgs e)
        {
            updateTypeCollisions((byte)typePainterUpDown.Value);
        }
        private void updateCollisions(byte typeValue)
        {
            PrepareCollisionPainterGraphics(typeValue);
            paintByte = (byte)typeValue;

            sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            using (Graphics g = Graphics.FromImage(collisionPainterPictureBox.Image))
            {
                g.Clear(Color.FromArgb(255, paintBrush.Color));
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(typeValue.ToString("X2"), new Font("Microsoft Sans Serif", 24), textBrush, painterBox, sf);
            }

            if (PokeDatabase.System.MapCollisionPainters.TryGetValue(typeValue, out string dictResult))
            {
                collisionPainterComboBox.SelectedItem = dictResult;
            }
            collisionPainterPictureBox.Invalidate();
        }
        private void updateTypeCollisions(byte typeValue)
        {
            PrepareTypePainterGraphics(typeValue);
            paintByte = typeValue;

            sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            using (Graphics g = Graphics.FromImage(typePainterPictureBox.Image))
            {
                g.Clear(Color.FromArgb(255, paintBrush.Color));
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(typeValue.ToString("X2"), new Font("Microsoft Sans Serif", 24), textBrush, painterBox, sf);
            }

            if (PokeDatabase.System.MapCollisionTypePainters.TryGetValue(typeValue, out string dictResult))
            {
                collisionTypePainterComboBox.SelectedItem = dictResult;
            }
            else
            {
                valueTypeRadioButton.Checked = true;
                typePainterUpDown.Value = typeValue;
            }
            typePainterPictureBox.Invalidate();
        }
        private void typePictureBox_Click(object sender, EventArgs e)
        {
            selectCollisionPanel.BackColor = Color.Transparent;
            collisionGroupBox.Enabled = false;
            selectTypePanel.BackColor = Color.MidnightBlue;
            typeGroupBox.Enabled = true;

            DrawTypeGrid();
            RestorePainter();
        }
        private void typesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (knownTypesRadioButton.Checked)
            {
                typePainterUpDown.Enabled = false;
                collisionTypePainterComboBox.Enabled = true;
                typePainterComboBox_SelectedIndexChanged(null, null);
            }
        }
        private void valueTypeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (valueTypeRadioButton.Checked)
            {
                collisionTypePainterComboBox.Enabled = false;
                typePainterUpDown.Enabled = true;
                typePainterUpDown_ValueChanged(null, null);
            }
        }
        #endregion

        #region 3D Model Editor
        public const ushort MAPMODEL_CRITICALSIZE = 61000;

        private void importMapButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog im = new OpenFileDialog
            {
                Filter = MapFile.NSBMDFilter,
                InitialDirectory = SettingsManager.Settings.mapImportStarterPoint
            };
            if (im.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            currentMapFile.LoadMapModel(DSUtils.ReadFromFile(im.FileName));

            if (mapTextureComboBox.SelectedIndex > 0)
            {
                MW_LoadModelTextures(currentMapFile.mapModel, RomInfo.gameDirs[DirNames.mapTextures].unpackedDir, mapTextureComboBox.SelectedIndex - 1);
            }
            RenderMap(ref mapRenderer, ref buildingsRenderer, ref currentMapFile, ang, dist, elev, perspective, mapOpenGlControl.Width, mapOpenGlControl.Height, mapTexturesOn, bldTexturesOn);

            modelSizeLBL.Text = currentMapFile.mapModelData.Length.ToString() + " B";

            string message;
            string title;
            if (currentMapFile.mapModelData.Length > MAPMODEL_CRITICALSIZE)
            {
                message = "You imported a map model that exceeds " + MAPMODEL_CRITICALSIZE + " bytes." + Environment.NewLine
                    + "This may lead to unexpected behavior in game.";
                title = "Imported correctly, but...";
            }
            else
            {
                message = "Map model imported successfully!";
                title = "Success!";
            }
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exportMapButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog em = new SaveFileDialog
            {
                FileName = selectMapComboBox.SelectedItem.ToString()
            };

            byte[] modelToWrite;

            if (embedTexturesInMapModelCheckBox.Checked)
            { /* Textured NSBMD file */
                em.Filter = MapFile.TexturedNSBMDFilter;
                if (em.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                string texturePath = RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4");
                byte[] texturesToEmbed = File.ReadAllBytes(texturePath);
                modelToWrite = NSBUtils.BuildNSBMDwithTextures(currentMapFile.mapModelData, texturesToEmbed);
            }
            else
            { /* Untextured NSBMD file */
                em.Filter = MapFile.UntexturedNSBMDFilter;
                if (em.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                modelToWrite = currentMapFile.mapModelData;
            }

            File.WriteAllBytes(em.FileName, modelToWrite);
            MessageBox.Show("Map model exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void daeExportButton_Click(object sender, EventArgs e)
        {
            ModelUtils.ModelToDAE(
                modelName: selectMapComboBox.SelectedItem.ToString().TrimEnd('\0'),
                modelData: currentMapFile.mapModelData,
                textureData: mapTextureComboBox.SelectedIndex < 0 ? null : File.ReadAllBytes(RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4"))
            );
        }

        private void glbExportButton_Click(object sender, EventArgs e)
        {
            ModelUtils.ModelToGLB(
                modelName: selectMapComboBox.SelectedItem.ToString().TrimEnd('\0'),
                modelData: currentMapFile.mapModelData,
                textureData: mapTextureComboBox.SelectedIndex < 0 ? null : File.ReadAllBytes(RomInfo.gameDirs[DirNames.mapTextures].unpackedDir + "\\" + (mapTextureComboBox.SelectedIndex - 1).ToString("D4"))
            );
        }
        #endregion


        #region BDHC I/O
        private void bdhcImportButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog it = new OpenFileDialog()
            {
                Filter = RomInfo.gameFamily == GameFamilies.DP ? MapFile.BDHCFilter : MapFile.BDHCamFilter
            };

            if (it.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            currentMapFile.ImportTerrain(File.ReadAllBytes(it.FileName));
            terrainSizeLBL.Text = currentMapFile.bdhc.Length.ToString() + " B";
            MessageBox.Show("Terrain settings imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void bdhcExportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                FileName = selectMapComboBox.SelectedItem.ToString(),
                Filter = RomInfo.gameFamily == GameFamilies.DP ? MapFile.BDHCFilter : MapFile.BDHCamFilter
            };

            if (sf.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            File.WriteAllBytes(sf.FileName, currentMapFile.bdhc);

            terrainSizeLBL.Text = currentMapFile.bdhc.Length.ToString() + " B";
            MessageBox.Show("Terrain settings exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Soundplates I/O
        private void soundPlatesImportButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog it = new OpenFileDialog
            {
                Filter = MapFile.BGSFilter
            };

            if (it.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            currentMapFile.ImportSoundPlates(File.ReadAllBytes(it.FileName));
            BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            MessageBox.Show("BackGround Sound data imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void soundPlatesExportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = MapFile.BGSFilter,
                FileName = selectMapComboBox.SelectedItem.ToString()
            };
            if (sf.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            File.WriteAllBytes(sf.FileName, currentMapFile.bgs);

            BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            MessageBox.Show("BackGround Sound data exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void soundPlatesBlankButton_Click(object sender, EventArgs e)
        {
            currentMapFile.bgs = MapFile.blankBGS;
            BGSSizeLBL.Text = currentMapFile.bgs.Length.ToString() + " B";
            MessageBox.Show("BackGround Sound data successfull blanked.\nRemember to save the current map file.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
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
                    Console.WriteLine(f.Name);
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
                            Console.WriteLine(f.Name);
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
        private void exportScriptDatabaseJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet");
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
            var flyEditor = new FlyEditor(gameFamily, headerListBoxNames);
            flyEditor.Show();
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
            itemEditor.ShowDialog();

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
            ovlEditor.ShowDialog();

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
            mde.ShowDialog();

            Helpers.statusLabelMessage();
            Update();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SettingsWindow editor = new SettingsWindow())
                editor.ShowDialog();
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

            pde.ShowDialog();
        }


        #endregion

        private void locateCurrentMapBin_Click(object sender, EventArgs e)
        {
            ExplorerSelect(Path.Combine(gameDirs[DirNames.maps].unpackedDir, selectMapComboBox.SelectedIndex.ToString("D4")));
        }

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

        private void MainProgram_Load(object sender, EventArgs e)
        {
            transparencyBar.Value = Transparency;
        }

        private void transparencyBar_Scroll(object sender, EventArgs e)
        {
            Transparency = transparencyBar.Value;
            if (selectCollisionPanel.BackColor == Color.MidnightBlue)
            {
                DrawCollisionGrid();
            }
            else
            {
                DrawTypeGrid();
            }

        }

        #region Poppout Buttons

        private void popoutTextEditorButton_Click(object sender, EventArgs e)
        {
            textEditorPoppedOutLabel.Visible = true; // Show Editor popped-out label
            popoutTextEditorButton.Enabled = false; // Disable popout button

            Helpers.PopOutEditor(textEditor, "Text Editor", mainTabImageList.Images[4], ctrl =>
            {
                textEditorPoppedOutLabel.Visible = false; // Hide Editor popped-out label
                popoutTextEditorButton.Enabled = true; // Enable popout button
            });
        }

        private void popoutLevelScriptEditorButton_Click(object sender, EventArgs e)
        {
            LSEditorPoppedOutLabel.Visible = true; // Show Editor popped-out label
            popoutLevelScriptEditorButton.Enabled = false; // Disable popout button

            Helpers.PopOutEditor(levelScriptEditor, "Level Script Editor", mainTabImageList.Images[3], ctrl =>
            {
                LSEditorPoppedOutLabel.Visible = false; // Hide Editor popped-out label
                popoutLevelScriptEditorButton.Enabled = true; // Enable popout button
            });
        }

        private void popoutScriptEditorButton_Click(object sender, EventArgs e)
        {
            scriptEditorPoppedOutLabel.Visible = true; // Show Editor popped-out label
            popoutScriptEditorButton.Enabled = false; // Disable popout button

            Helpers.PopOutEditor(scriptEditor, "Script Editor", mainTabImageList.Images[3], ctrl =>
            {
                scriptEditorPoppedOutLabel.Visible = false; // Hide Editor popped-out label
                popoutScriptEditorButton.Enabled = true; // Enable popout button
            });
        }

        private void popoutTrainerEditorButton_Click(object sender, EventArgs e)
        {
            trainerEditorPoppedOutLabel.Visible = true; // Show Editor popped-out label
            popoutTrainerEditorButton.Enabled = false; // Disable popout button

            Helpers.PopOutEditor(trainerEditor, "Trainer Editor", mainTabImageList.Images[8], ctrl =>
            {
                trainerEditorPoppedOutLabel.Visible = false; // Hide Editor popped-out label
                popoutTrainerEditorButton.Enabled = true; // Enable popout button
            });
        }
        #endregion
    }
}