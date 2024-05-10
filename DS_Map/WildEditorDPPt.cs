using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE {
    public partial class WildEditorDPPt : Form {
        public string encounterFileFolder { get; private set; }
        EncounterFileDPPt currentFile;
       
        public WildEditorDPPt(string dirPath, string[] names, int encToOpen, int totalNumHeaderFiles) {
            InitializeComponent();
            encounterFileFolder = dirPath;
            Text = "DSPRE Reloaded " + GetDSPREVersion() + " - DPPt Encounters Editor";
            Helpers.DisableHandlers();

            MapHeader tempMapHeader;
            List<string> locationNames = RomInfo.GetLocationNames();
            Dictionary<int, List<string>> EncounterFileLocationNames = new Dictionary<int, List<string>>();

            for (ushort i = 0; i < totalNumHeaderFiles; i++) {
                if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                    tempMapHeader = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + i.ToString("D4"), i, 0);
                } else {
                    tempMapHeader = MapHeader.LoadFromARM9(i);
                }
                if (tempMapHeader.wildPokemon != MapHeader.DPPT_NULL_ENCOUNTER_FILE_ID) {                 
                    if (!EncounterFileLocationNames.ContainsKey(tempMapHeader.wildPokemon)) {
                        EncounterFileLocationNames[tempMapHeader.wildPokemon] = new List<string>();
                    }
                    EncounterFileLocationNames[tempMapHeader.wildPokemon].Add((gameFamily == GameFamilies.DP) ? 
                        locationNames[((HeaderDP)tempMapHeader).locationName] : 
                        locationNames[((HeaderPt)tempMapHeader).locationName]);
                }
            }

            for (int i = 0; i < Directory.GetFiles(encounterFileFolder).Length; i++) {
                if (EncounterFileLocationNames.ContainsKey(i)) {
                    selectEncounterComboBox.Items.Add("[" + i + "] " + String.Join(" + ", EncounterFileLocationNames[i]));
                } else {
                    selectEncounterComboBox.Items.Add("[" + i + "] " + " Unused");
                }
            }

            if (encToOpen > selectEncounterComboBox.Items.Count) {
                MessageBox.Show("This encounter file doesn't exist.\n" +
                "Enc #0 will be loaded, instead.", "WildPoké Data not found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                selectEncounterComboBox.SelectedIndex = 0;
            } else {
                selectEncounterComboBox.SelectedIndex = encToOpen;
            }

            currentFile = new EncounterFileDPPt(selectEncounterComboBox.SelectedIndex);

            /* Once the GUI overhaul is complete - i.e.: once everything is a TableLayoutPanel, 
             * this can be simplified a lot. */
            foreach (TabPage page in mainTabControl.TabPages) {
                foreach (Control g in page.Controls) {
                    if (g != null && g is GroupBox) {
                        foreach (Control c in g.Controls) {
                            if (c != null) {
                                if (c is InputComboBox) {
                                    (c as InputComboBox).DataSource = new BindingSource(names, string.Empty);
                                } else if (c is TableLayoutPanel) {
                                    TableLayoutPanel tbl = (c as TableLayoutPanel);
                                   
                                    foreach (Control tblC in tbl.Controls) {
                                        if (c != null) {
                                            if (tblC is InputComboBox) {
                                                (tblC as InputComboBox).DataSource = new BindingSource(names, string.Empty);
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }

            Helpers.EnableHandlers();

            SetupControls();
        }
        private void SetupControls() {
            Helpers.DisableHandlers();

            /* Setup encounter rates controls */
            walkingRateUpDown.Value = currentFile.walkingRate;
            surfRateUpDown.Value = currentFile.surfRate;
            oldRodRateUpDown.Value = currentFile.oldRodRate;
            goodRodRateUpDown.Value = currentFile.goodRodRate;
            superRodRateUpDown.Value = currentFile.superRodRate;

            /* Walking encounters controls setup */
            walkingTwentyFirstComboBox.SelectedIndex = (int)currentFile.walkingPokemon[0];
            walkingTwentyFirstUpDown.Value = currentFile.walkingLevels[0];
            walkingTwentySecondComboBox.SelectedIndex = (int)currentFile.walkingPokemon[1];
            walkingTwentySecondUpDown.Value = currentFile.walkingLevels[1];
            walkingTenFirstComboBox.SelectedIndex = (int)currentFile.walkingPokemon[2];
            walkingTenFirstUpDown.Value = currentFile.walkingLevels[2];
            walkingTenSecondComboBox.SelectedIndex = (int)currentFile.walkingPokemon[3];
            walkingTenSecondUpDown.Value = currentFile.walkingLevels[3];
            walkingTenThirdComboBox.SelectedIndex = (int)currentFile.walkingPokemon[4];
            walkingTenThirdUpDown.Value = currentFile.walkingLevels[4];
            walkingTenFourthComboBox.SelectedIndex = (int)currentFile.walkingPokemon[5];
            walkingTenFourthUpDown.Value = currentFile.walkingLevels[5];
            walkingFiveFirstComboBox.SelectedIndex = (int)currentFile.walkingPokemon[6];
            walkingFiveFirstUpDown.Value = currentFile.walkingLevels[6];
            walkingFiveSecondComboBox.SelectedIndex = (int)currentFile.walkingPokemon[7];
            walkingFiveSecondUpDown.Value = currentFile.walkingLevels[7];
            walkingFourFirstComboBox.SelectedIndex = (int)currentFile.walkingPokemon[8];
            walkingFourFirstUpDown.Value = currentFile.walkingLevels[8];
            walkingFourSecondComboBox.SelectedIndex = (int)currentFile.walkingPokemon[9];
            walkingFourSecondUpDown.Value = currentFile.walkingLevels[9];
            walkingOneFirstComboBox.SelectedIndex = (int)currentFile.walkingPokemon[10];
            walkingOneFirstUpDown.Value = currentFile.walkingLevels[10];
            walkingOneSecondComboBox.SelectedIndex = (int)currentFile.walkingPokemon[11];
            walkingOneSecondUpDown.Value = currentFile.walkingLevels[11];

            /* Time dependent encounters controls setup */
            morningFirstComboBox.SelectedIndex = (int)currentFile.morningPokemon[0];
            morningSecondComboBox.SelectedIndex = (int)currentFile.morningPokemon[1];
            nightFirstComboBox.SelectedIndex = (int)currentFile.nightPokemon[0];
            nightSecondComboBox.SelectedIndex = (int)currentFile.nightPokemon[1];
            swarmFirstComboBox.SelectedIndex = currentFile.swarmPokemon[0];
            swarmSecondComboBox.SelectedIndex = currentFile.swarmPokemon[1];

            /* Dual Slot encounters controls setup */
            rubyFirstComboBox.SelectedIndex = (int)currentFile.rubyPokemon[0];
            rubySecondComboBox.SelectedIndex = (int)currentFile.rubyPokemon[1];
            sapphireFirstComboBox.SelectedIndex = (int)currentFile.sapphirePokemon[0];
            sapphireSecondComboBox.SelectedIndex = (int)currentFile.sapphirePokemon[1];
            emeraldFirstComboBox.SelectedIndex = (int)currentFile.emeraldPokemon[0];
            emeraldSecondComboBox.SelectedIndex = (int)currentFile.emeraldPokemon[1];
            fireRedFirstComboBox.SelectedIndex = (int)currentFile.fireRedPokemon[0];
            fireRedSecondComboBox.SelectedIndex = (int)currentFile.fireRedPokemon[1];
            leafGreenFirstComboBox.SelectedIndex = (int)currentFile.leafGreenPokemon[0];
            leafGreenSecondComboBox.SelectedIndex = (int)currentFile.leafGreenPokemon[1];

            /* PokéRadar encounters controls setup */
            radarFirstComboBox.SelectedIndex = (int)currentFile.radarPokemon[0];
            radarSecondComboBox.SelectedIndex = (int)currentFile.radarPokemon[1];
            radarThirdComboBox.SelectedIndex = (int)currentFile.radarPokemon[2];
            radarFourthComboBox.SelectedIndex = (int)currentFile.radarPokemon[3];

            /* Water encounters controls setup */
            surfSixtyComboBox.SelectedIndex = currentFile.surfPokemon[0];
            surfSixtyMinLevelUpDown.Value = currentFile.surfMinLevels[0];
            surfSixtyMaxLevelUpDown.Value = currentFile.surfMaxLevels[0];

            surfThirtyComboBox.SelectedIndex = currentFile.surfPokemon[1];
            surfThirtyMinLevelUpDown.Value = currentFile.surfMinLevels[1];
            surfThirtyMaxLevelUpDown.Value = currentFile.surfMaxLevels[1];

            surfFiveComboBox.SelectedIndex = currentFile.surfPokemon[2];
            surfFiveMinLevelUpDown.Value = currentFile.surfMinLevels[2];
            surfFiveMaxLevelUpDown.Value = currentFile.surfMaxLevels[2];

            surfFourComboBox.SelectedIndex = currentFile.surfPokemon[3];
            surfFourMinLevelUpDown.Value = currentFile.surfMinLevels[3];
            surfFourMaxLevelUpDown.Value = currentFile.surfMaxLevels[3];

            surfOneComboBox.SelectedIndex = currentFile.surfPokemon[4];
            surfOneMinLevelUpDown.Value = currentFile.surfMinLevels[4];
            surfOneMaxLevelUpDown.Value = currentFile.surfMaxLevels[4];

            /* Old rod encounters controls setup */
            oldRodSixtyComboBox.SelectedIndex = currentFile.oldRodPokemon[0];
            oldRodSixtyMinLevelUpDown.Value = currentFile.oldRodMinLevels[0];
            oldRodSixtyMaxLevelUpDown.Value = currentFile.oldRodMaxLevels[0];

            oldRodThirtyComboBox.SelectedIndex = currentFile.oldRodPokemon[1];
            oldRodThirtyMinLevelUpDown.Value = currentFile.oldRodMinLevels[1];
            oldRodThirtyMaxLevelUpDown.Value = currentFile.oldRodMaxLevels[1];

            oldRodFiveComboBox.SelectedIndex = currentFile.oldRodPokemon[2];
            oldRodFiveMinLevelUpDown.Value = currentFile.oldRodMinLevels[2];
            oldRodFiveMaxLevelUpDown.Value = currentFile.oldRodMaxLevels[2];

            oldRodFourComboBox.SelectedIndex = currentFile.oldRodPokemon[3];
            oldRodFourMinLevelUpDown.Value = currentFile.oldRodMinLevels[3];
            oldRodFourMaxLevelUpDown.Value = currentFile.oldRodMaxLevels[3];

            oldRodOneComboBox.SelectedIndex = currentFile.oldRodPokemon[4];
            oldRodOneMinLevelUpDown.Value = currentFile.oldRodMinLevels[4];
            oldRodOneMaxLevelUpDown.Value = currentFile.oldRodMaxLevels[4];

            /* Good rod encounters controls setup */
            goodRodFirstFortyComboBox.SelectedIndex = currentFile.goodRodPokemon[0];
            goodRodFirstFortyMinLevelUpDown.Value = currentFile.goodRodMinLevels[0];
            goodRodFirstFortyMaxLevelUpDown.Value = currentFile.goodRodMaxLevels[0];

            goodRodSecondFortyComboBox.SelectedIndex = currentFile.goodRodPokemon[1];
            goodRodSecondFortyMinLevelUpDown.Value = currentFile.goodRodMinLevels[1];
            goodRodSecondFortyMaxLevelUpDown.Value = currentFile.goodRodMaxLevels[1];

            goodRodFifteenComboBox.SelectedIndex = currentFile.goodRodPokemon[2];
            goodRodFifteenMinLevelUpDown.Value = currentFile.goodRodMinLevels[2];
            goodRodFifteenMaxLevelUpDown.Value = currentFile.goodRodMaxLevels[2];

            goodRodFourComboBox.SelectedIndex = currentFile.goodRodPokemon[3];
            goodRodFourMinLevelUpDown.Value = currentFile.goodRodMinLevels[3];
            goodRodFourMaxLevelUpDown.Value = currentFile.goodRodMaxLevels[3];

            goodRodOneComboBox.SelectedIndex = currentFile.goodRodPokemon[4];
            goodRodOneMinLevelUpDown.Value = currentFile.goodRodMinLevels[4];
            goodRodOneMaxLevelUpDown.Value = currentFile.goodRodMaxLevels[4];

            /* Super rod encounters controls setup */
            superRodFirstFortyComboBox.SelectedIndex = currentFile.superRodPokemon[0];
            superRodFirstFortyMinLevelUpDown.Value = currentFile.superRodMinLevels[0];
            superRodFirstFortyMaxLevelUpDown.Value = currentFile.superRodMaxLevels[0];

            superRodSecondFortyComboBox.SelectedIndex = currentFile.superRodPokemon[1];
            superRodSecondFortyMinLevelUpDown.Value = currentFile.superRodMinLevels[1];
            superRodSecondFortyMaxLevelUpDown.Value = currentFile.superRodMaxLevels[1];

            superRodFifteenComboBox.SelectedIndex = currentFile.superRodPokemon[2];
            superRodFifteenMinLevelUpDown.Value = currentFile.superRodMinLevels[2];
            superRodFifteenMaxLevelUpDown.Value = currentFile.superRodMaxLevels[2];

            superRodFourComboBox.SelectedIndex = currentFile.superRodPokemon[3];
            superRodFourMinLevelUpDown.Value = currentFile.superRodMinLevels[3];
            superRodFourMaxLevelUpDown.Value = currentFile.superRodMaxLevels[3];

            superRodOneComboBox.SelectedIndex = currentFile.superRodPokemon[4];
            superRodOneMinLevelUpDown.Value = currentFile.superRodMinLevels[4];
            superRodOneMaxLevelUpDown.Value = currentFile.superRodMaxLevels[4];

            Helpers.EnableHandlers();
        }

        public string GetDSPREVersion() {
            return "" + Assembly.GetExecutingAssembly().GetName().Version.Major + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor +
                "." + Assembly.GetExecutingAssembly().GetName().Version.Build;
        }

        private void exportEncounterFileButton_Click(object sender, EventArgs e) {
            currentFile.SaveToFileExplorePath("Encounter File " + selectEncounterComboBox.SelectedIndex);
        }
        private void importEncounterFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .wld file */
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Wild Encounters File (" + "*." + EncounterFile.extension + ")" + "|" + "*." + EncounterFile.extension
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update encounter file object in memory */
            currentFile = new EncounterFileDPPt(new FileStream(of.FileName, FileMode.Open));

            /* Update controls */
            SetupControls();
        }
        private void selectEncounterComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }

            currentFile = new EncounterFileDPPt(selectEncounterComboBox.SelectedIndex);
            SetupControls();
        }
        private void saveEncountersButton_Click(object sender, EventArgs e) {
            currentFile.SaveToFileDefaultDir(selectEncounterComboBox.SelectedIndex);
        }
        private void walkingTwentyFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.walkingPokemon[0] = (uint)walkingTwentyFirstComboBox.SelectedIndex;
        }
        private void walkingTwentySecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.walkingPokemon[1] = (uint)walkingTwentySecondComboBox.SelectedIndex;
        }
        private void walkingTenFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.walkingPokemon[2] = (uint)walkingTenFirstComboBox.SelectedIndex;
        }
        private void walkingTenSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.walkingPokemon[3] = (uint)walkingTenSecondComboBox.SelectedIndex;
        }
        private void walkingTenThirdComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.walkingPokemon[4] = (uint)walkingTenThirdComboBox.SelectedIndex;
        }
        private void walkingTenFourthComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingPokemon[5] = (uint)walkingTenFourthComboBox.SelectedIndex;
        }
        private void walkingFiveFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingPokemon[6] = (uint)walkingFiveFirstComboBox.SelectedIndex;
        }
        private void walkingFiveSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingPokemon[7] = (uint)walkingFiveSecondComboBox.SelectedIndex;
        }
        private void walkingFourFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingPokemon[8] = (uint)walkingFourFirstComboBox.SelectedIndex;
        }
        private void walkingFourSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingPokemon[9] = (uint)walkingFourSecondComboBox.SelectedIndex;
        }
        private void walkingOneFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingPokemon[10] = (uint)walkingOneFirstComboBox.SelectedIndex;
        }
        private void walkingOneSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingPokemon[11] = (uint)walkingOneSecondComboBox.SelectedIndex;
        }
        private void morningFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[0] = (uint)morningFirstComboBox.SelectedIndex;
        }
        private void morningSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[1] = (uint)morningSecondComboBox.SelectedIndex;
        }
        private void nightFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.nightPokemon[0] = (uint)nightFirstComboBox.SelectedIndex;
        }
        private void nightSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.nightPokemon[1] = (uint)nightSecondComboBox.SelectedIndex;
        }
        private void swarmFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.swarmPokemon[0] = (ushort)swarmFirstComboBox.SelectedIndex;
        }
        private void swarmSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.swarmPokemon[1] = (ushort)swarmSecondComboBox.SelectedIndex;
        }
        private void rubyFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.rubyPokemon[0] = (uint)rubyFirstComboBox.SelectedIndex;
        }
        private void rubySecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.rubyPokemon[1] = (uint)rubySecondComboBox.SelectedIndex;
        }
        private void sapphireFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.sapphirePokemon[0] = (uint)sapphireFirstComboBox.SelectedIndex;
        }
        private void sapphireSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.sapphirePokemon[1] = (uint)sapphireSecondComboBox.SelectedIndex;
        }
        private void emeraldFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.emeraldPokemon[0] = (uint)emeraldFirstComboBox.SelectedIndex;
        }
        private void emeraldSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.emeraldPokemon[1] = (uint)emeraldSecondComboBox.SelectedIndex;
        }
        private void fireRedFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.fireRedPokemon[0] = (uint)fireRedFirstComboBox.SelectedIndex;
        }
        private void fireRedSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.fireRedPokemon[1] = (uint)fireRedSecondComboBox.SelectedIndex;
        }
        private void leafGreenFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.leafGreenPokemon[0] = (uint)leafGreenFirstComboBox.SelectedIndex;
        }
        private void leafGreenSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.leafGreenPokemon[1] = (uint)leafGreenSecondComboBox.SelectedIndex;
        }
        private void radarFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.radarPokemon[0] = (uint)radarFirstComboBox.SelectedIndex;
        }
        private void radarSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.radarPokemon[1] = (uint)radarSecondComboBox.SelectedIndex;
        }
        private void radarThirdComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.radarPokemon[2] = (uint)radarThirdComboBox.SelectedIndex;
        }
        private void radarFourthComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.radarPokemon[3] = (uint)radarFourthComboBox.SelectedIndex;
        }
        private void surfSixtyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.surfPokemon[0] = (ushort)surfSixtyComboBox.SelectedIndex;
        }
        private void surfThirtyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.surfPokemon[1] = (ushort)surfThirtyComboBox.SelectedIndex;
        }
        private void surfFiveComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.surfPokemon[2] = (ushort)surfFiveComboBox.SelectedIndex;
        }
        private void surfFourComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.surfPokemon[3] = (ushort)surfFourComboBox.SelectedIndex;
        }
        private void surfOneComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.surfPokemon[4] = (ushort)surfOneComboBox.SelectedIndex;
        }
        private void oldRodSixtyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.oldRodPokemon[0] = (ushort)oldRodSixtyComboBox.SelectedIndex;
        }
        private void oldRodThirtyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.oldRodPokemon[1] = (ushort)oldRodThirtyComboBox.SelectedIndex;
        }
        private void oldRodFiveComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.oldRodPokemon[2] = (ushort)oldRodFiveComboBox.SelectedIndex;
        }
        private void oldRodFourComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.oldRodPokemon[3] = (ushort)oldRodFourComboBox.SelectedIndex;
        }
        private void oldRodOneComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.oldRodPokemon[4] = (ushort)oldRodOneComboBox.SelectedIndex;
        }
        private void goodRodFirstFortyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.goodRodPokemon[0] = (ushort)goodRodFirstFortyComboBox.SelectedIndex;
        }
        private void goodRodSecondFortyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.goodRodPokemon[1] = (ushort)goodRodSecondFortyComboBox.SelectedIndex;
        }
        private void goodRodFifteenComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.goodRodPokemon[2] = (ushort)goodRodFifteenComboBox.SelectedIndex;
        }
        private void goodRodFourComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.goodRodPokemon[3] = (ushort)goodRodFourComboBox.SelectedIndex;
        }
        private void goodRodOneComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.goodRodPokemon[4] = (ushort)goodRodOneComboBox.SelectedIndex;
        }
        private void superRodFirstFortyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.superRodPokemon[0] = (ushort)superRodFirstFortyComboBox.SelectedIndex;
        }
        private void superRodSecondFortyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.superRodPokemon[1] = (ushort)superRodSecondFortyComboBox.SelectedIndex;
        }
        private void superRodFifteenComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.superRodPokemon[2] = (ushort)superRodFifteenComboBox.SelectedIndex;
        }
        private void superRodFourComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.superRodPokemon[3] = (ushort)superRodFourComboBox.SelectedIndex;
        }
        private void superRodOneComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.superRodPokemon[4] = (ushort)superRodOneComboBox.SelectedIndex;
        }

        /* Walking levels controls */
        private void walkingTwentyFirstUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[0] = (byte)walkingTwentyFirstUpDown.Value;
        }
        private void walkingTwentySecondUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.walkingLevels[1] = (byte)walkingTwentySecondUpDown.Value;
        }
        private void walkingTenFirstUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.walkingLevels[2] = (byte)walkingTenFirstUpDown.Value;
        }
        private void walkingTenSecondUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.walkingLevels[3] = (byte)walkingTenSecondUpDown.Value;
        }
        private void walkingTenThirdUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[4] = (byte)walkingTenThirdUpDown.Value;
        }
        private void walkingTenFourthUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[5] = (byte)walkingTenFourthUpDown.Value;
        }
        private void walkingFiveFirstUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[6] = (byte)walkingFiveFirstUpDown.Value;
        }
        private void walkingFiveSecondUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.walkingLevels[7] = (byte)walkingFiveSecondUpDown.Value;
        }
        private void walkingFourFirstUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.walkingLevels[8] = (byte)walkingFourFirstUpDown.Value;
        }
        private void walkingFourSecondUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.walkingLevels[9] = (byte)walkingFourSecondUpDown.Value;
        }
        private void walkingOneFirstUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.walkingLevels[10] = (byte)walkingOneFirstUpDown.Value;
        }
        private void walkingOneSecondUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[11] = (byte)walkingOneSecondUpDown.Value;
        }

        /* Water levels controls */
        private void surfSixtyMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.surfMinLevels[0] = (byte)surfSixtyMinLevelUpDown.Value;
        }
        private void surfThirtyMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.surfMinLevels[1] = (byte)surfThirtyMinLevelUpDown.Value;
        }
        private void surfFiveMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.surfMinLevels[2] = (byte)surfFiveMinLevelUpDown.Value;
        }
        private void surfFourMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.surfMinLevels[3] = (byte)surfFourMinLevelUpDown.Value;
        }
        private void surfOneMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.surfMinLevels[4] = (byte)surfOneMinLevelUpDown.Value;
        }
        private void surfSixtyMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.surfMaxLevels[0] = (byte)surfSixtyMaxLevelUpDown.Value;
        }
        private void surfThirtyMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.surfMaxLevels[1] = (byte)surfThirtyMaxLevelUpDown.Value;
        }
        private void surfFiveMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.surfMaxLevels[2] = (byte)surfFiveMaxLevelUpDown.Value;
        }
        private void surfFourMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.surfMaxLevels[3] = (byte)surfFourMaxLevelUpDown.Value;
        }
        private void surfOneMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.surfMaxLevels[4] = (byte)surfOneMaxLevelUpDown.Value;
        }

        private void oldRodSixtyMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.oldRodMinLevels[0] = (byte)oldRodSixtyMinLevelUpDown.Value;
        }
        private void oldRodThirtyMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.oldRodMinLevels[1] = (byte)oldRodThirtyMinLevelUpDown.Value;
        }
        private void oldRodFiveMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.oldRodMinLevels[2] = (byte)oldRodFiveMinLevelUpDown.Value;
        }
        private void oldRodFourMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.oldRodMinLevels[3] = (byte)oldRodFourMinLevelUpDown.Value;
        }
        private void oldRodOneMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.oldRodMinLevels[4] = (byte)oldRodOneMinLevelUpDown.Value;
        }
        private void oldRodSixtyMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.oldRodMaxLevels[0] = (byte)oldRodSixtyMaxLevelUpDown.Value;
        }
        private void oldRodThirtyMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.oldRodMaxLevels[1] = (byte)oldRodThirtyMaxLevelUpDown.Value;
        }
        private void oldRodFiveMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.oldRodMaxLevels[2] = (byte)oldRodFiveMaxLevelUpDown.Value;
        }
        private void oldRodFourMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.oldRodMaxLevels[3] = (byte)oldRodFourMaxLevelUpDown.Value;
        }
        private void oldRodOneMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.oldRodMaxLevels[4] = (byte)oldRodOneMaxLevelUpDown.Value;
        }

        private void goodRodFirstFortyMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.goodRodMinLevels[0] = (byte)goodRodFirstFortyMinLevelUpDown.Value;
        }
        private void goodRodSecondFortyMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.goodRodMinLevels[1] = (byte)goodRodSecondFortyMinLevelUpDown.Value;
        }
        private void goodRodFifteenMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.goodRodMinLevels[2] = (byte)goodRodFifteenMinLevelUpDown.Value;
        }
        private void goodRodFourMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.goodRodMinLevels[3] = (byte)goodRodFourMinLevelUpDown.Value;
        }
        private void goodRodOneMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.goodRodMinLevels[4] = (byte)goodRodOneMinLevelUpDown.Value;
        }
        private void goodRodFirstFortyMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.goodRodMaxLevels[0] = (byte)goodRodFirstFortyMaxLevelUpDown.Value;
        }
        private void goodRodSecondFortyMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.goodRodMaxLevels[1] = (byte)goodRodSecondFortyMaxLevelUpDown.Value;
        }
        private void goodRodFifteenMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.goodRodMaxLevels[2] = (byte)goodRodFifteenMaxLevelUpDown.Value;
        }
        private void goodRodFourMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.goodRodMaxLevels[3] = (byte)goodRodFourMaxLevelUpDown.Value;
        }
        private void goodRodOneMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.goodRodMaxLevels[4] = (byte)goodRodOneMaxLevelUpDown.Value;
        }

        private void superRodFirstFortyMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.superRodMinLevels[0] = (byte)superRodFirstFortyMinLevelUpDown.Value;
        }
        private void superRodSecondFortyMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.superRodMinLevels[1] = (byte)superRodSecondFortyMinLevelUpDown.Value;
        }
        private void superRodFifteenMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.superRodMinLevels[2] = (byte)superRodFifteenMinLevelUpDown.Value;
        }
        private void superRodFourMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.superRodMinLevels[3] = (byte)superRodFourMinLevelUpDown.Value;
        }
        private void superRodOneMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return; 
            }
            currentFile.superRodMinLevels[4] = (byte)superRodOneMinLevelUpDown.Value;
        }
        private void superRodFirstFortyMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.superRodMaxLevels[0] = (byte)superRodFirstFortyMaxLevelUpDown.Value;
        }
        private void superRodSecondFortyMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.superRodMaxLevels[1] = (byte)superRodSecondFortyMaxLevelUpDown.Value;
        }
        private void superRodFifteenMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return;
            }
            currentFile.superRodMaxLevels[2] = (byte)superRodFifteenMaxLevelUpDown.Value;
        }
        private void superRodFourMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.superRodMaxLevels[3] = (byte)superRodFourMaxLevelUpDown.Value;
        }
        private void superRodOneMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.superRodMaxLevels[4] = (byte)superRodOneMaxLevelUpDown.Value;
        }

        /* Encounter rate controls */
        private void walkingRateUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingRate = (byte)walkingRateUpDown.Value;
        }
        private void surfRateUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.surfRate = (byte)surfRateUpDown.Value;
        }
        private void oldRodRateUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.oldRodRate = (byte)oldRodRateUpDown.Value;
        }
        private void goodRodRateUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.goodRodRate = (byte)goodRodRateUpDown.Value;
        }

        private void superRodRateUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentFile.superRodRate = (byte)superRodRateUpDown.Value;
        }
                
        private void addEncounterFileButton_Click(object sender, EventArgs e) {
            int encounterCount = selectEncounterComboBox.Items.Count;

            /* Add new encounter file to encounter folder */
            string encounterFilePath = encounterFileFolder + "\\" + encounterCount.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(encounterFilePath, FileMode.Create))) {
                writer.Write(new EncounterFileDPPt().ToByteArray());
            }

            /* Update ComboBox*/
            selectEncounterComboBox.Items.Add("[New] Encounters File " + encounterCount.ToString());
            selectEncounterComboBox.SelectedIndex = encounterCount;
        }

        private void removeLastEncounterFileButton_Click(object sender, EventArgs e) {
            int encounterCount = selectEncounterComboBox.Items.Count;

            if (encounterCount > 1) {
                /* Delete encounter file file */
                int encounterToDelete = encounterCount - 1;

                string encounterFilePath = encounterFileFolder + "\\" + encounterToDelete.ToString("D4");
                File.Delete(encounterFilePath);

                /* Change selected index if the encounter file to be deleted is currently selected */
                if (selectEncounterComboBox.SelectedIndex == encounterToDelete)
                    selectEncounterComboBox.SelectedIndex--;

                /* Remove entry from ComboBox, and decrease encounter file count */
                selectEncounterComboBox.Items.RemoveAt(encounterToDelete);
            } else {
                MessageBox.Show("At least one encounter file must be kept.", "Can't delete encounter file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void repairAllButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("DSPRE is about to open every Encounter File and attempt to reset every corrupted field to its default value.\n" +
                "Do you wish to proceed?", "Repair all Encounter Files?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                for (int i = 0; i < Directory.GetFiles(encounterFileFolder).Length; i++) {
                    currentFile.SaveToFileDefaultDir(i, showSuccessMessage: false);
                }

                MessageBox.Show("All repairable fields have been fixed.", "Operation completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
