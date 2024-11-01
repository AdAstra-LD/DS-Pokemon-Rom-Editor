using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE {
    public partial class WildEditorHGSS : Form {
        public string encounterFileFolder { get; private set; }
        EncounterFileHGSS currentFile;

        public WildEditorHGSS(string dirPath, string[] names, int encToOpen, int totalNumHeaderFiles) {
            InitializeComponent();
            encounterFileFolder = dirPath;

            Helpers.DisableHandlers();
            Text = "DSPRE Reloaded " + GetDSPREVersion() + " - HGSS Encounters Editor";
            MapHeader tempMapHeader;
            List<string> locationNames = RomInfo.GetLocationNames();
            Dictionary<int, List<string>> EncounterFileLocationNames = new Dictionary<int, List<string>>();

            for (ushort i = 0; i < totalNumHeaderFiles; i++)
            {
                if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied())
                {
                    tempMapHeader = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + i.ToString("D4"), i, 0);
                }
                else
                {
                    tempMapHeader = MapHeader.LoadFromARM9(i);
                }

                if (tempMapHeader.wildPokemon != MapHeader.HGSS_NULL_ENCOUNTER_FILE_ID) {
                    if (!EncounterFileLocationNames.ContainsKey(tempMapHeader.wildPokemon)) {
                        EncounterFileLocationNames[tempMapHeader.wildPokemon] = new List<string>();
                    }
                    EncounterFileLocationNames[tempMapHeader.wildPokemon].Add(locationNames[((HeaderHGSS)tempMapHeader).locationName] );
                }
            }


            for (int i = 0; i < Directory.GetFiles(encounterFileFolder).Length; i++) {
                if (EncounterFileLocationNames.ContainsKey(i))
                    selectEncounterComboBox.Items.Add( "[" + i + "] " + String.Join(" + ", EncounterFileLocationNames[i]));
                else
                    selectEncounterComboBox.Items.Add("[" + i + "] " + " Unused");
            }

            if (encToOpen > selectEncounterComboBox.Items.Count) {
                MessageBox.Show("This encounter file doesn't exist.\n" +
                "Enc #0 will be loaded, instead.", "WildPoké Data not found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                selectEncounterComboBox.SelectedIndex = 0;
            } else {
                selectEncounterComboBox.SelectedIndex = encToOpen;
            }

            currentFile = new EncounterFileHGSS(selectEncounterComboBox.SelectedIndex);

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

        public string GetDSPREVersion() {
            return "" + Assembly.GetExecutingAssembly().GetName().Version.Major + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor +
                "." + Assembly.GetExecutingAssembly().GetName().Version.Build;
        }

        public void SetupControls() {
            Helpers.DisableHandlers();

            /* Setup encounter rates controls */
            walkingRateUpDown.Value = currentFile.walkingRate;
            surfRateUpDown.Value = currentFile.surfRate;
            rockSmashRateUpDown.Value = currentFile.rockSmashRate;
            oldRodRateUpDown.Value = currentFile.oldRodRate;
            goodRodRateUpDown.Value = currentFile.goodRodRate;
            superRodRateUpDown.Value = currentFile.superRodRate;

            /* Setup walking level controls */
            twentyFirstLevelUpDown.Value = currentFile.walkingLevels[0];
            twentySecondLevelUpDown.Value = currentFile.walkingLevels[1];
            tenFirstLevelUpDown.Value = currentFile.walkingLevels[2];
            tenSecondLevelUpDown.Value = currentFile.walkingLevels[3];
            tenThirdLevelUpDown.Value = currentFile.walkingLevels[4];
            tenFourthLevelUpDown.Value = currentFile.walkingLevels[5];
            fiveFirstLevelUpDown.Value = currentFile.walkingLevels[6];
            fiveSecondLevelUpDown.Value = currentFile.walkingLevels[7];
            fourFirstLevelUpDown.Value = currentFile.walkingLevels[8];
            fourSecondLevelUpDown.Value = currentFile.walkingLevels[9];
            oneFirstLevelUpDown.Value = currentFile.walkingLevels[10];
            oneSecondLevelUpDown.Value = currentFile.walkingLevels[11];

            /* Setup walking encounters controls */
            morningTwentyFirstComboBox.SelectedIndex = currentFile.morningPokemon[0];
            morningTwentySecondComboBox.SelectedIndex = currentFile.morningPokemon[1];
            morningTenFirstComboBox.SelectedIndex = currentFile.morningPokemon[2];
            morningTenSecondComboBox.SelectedIndex = currentFile.morningPokemon[3];
            morningTenThirdComboBox.SelectedIndex = currentFile.morningPokemon[4];
            morningTenFourthComboBox.SelectedIndex = currentFile.morningPokemon[5];
            morningFiveFirstComboBox.SelectedIndex = currentFile.morningPokemon[6];
            morningFiveSecondComboBox.SelectedIndex = currentFile.morningPokemon[7];
            morningFourFirstComboBox.SelectedIndex = currentFile.morningPokemon[8];
            morningFourSecondComboBox.SelectedIndex = currentFile.morningPokemon[9];
            morningOneFirstComboBox.SelectedIndex = currentFile.morningPokemon[10];
            morningOneSecondComboBox.SelectedIndex = currentFile.morningPokemon[11];

            dayTwentyFirstComboBox.SelectedIndex = currentFile.dayPokemon[0];
            dayTwentySecondComboBox.SelectedIndex = currentFile.dayPokemon[1];
            dayTenFirstComboBox.SelectedIndex = currentFile.dayPokemon[2];
            dayTenSecondComboBox.SelectedIndex = currentFile.dayPokemon[3];
            dayTenThirdComboBox.SelectedIndex = currentFile.dayPokemon[4];
            dayTenFourthComboBox.SelectedIndex = currentFile.dayPokemon[5];
            dayFiveFirstComboBox.SelectedIndex = currentFile.dayPokemon[6];
            dayFiveSecondComboBox.SelectedIndex = currentFile.dayPokemon[7];
            dayFourFirstComboBox.SelectedIndex = currentFile.dayPokemon[8];
            dayFourSecondComboBox.SelectedIndex = currentFile.dayPokemon[9];
            dayOneFirstComboBox.SelectedIndex = currentFile.dayPokemon[10];
            dayOneSecondComboBox.SelectedIndex = currentFile.dayPokemon[11];

            nightTwentyFirstComboBox.SelectedIndex = currentFile.nightPokemon[0];
            nightTwentySecondComboBox.SelectedIndex = currentFile.nightPokemon[1];
            nightTenFirstComboBox.SelectedIndex = currentFile.nightPokemon[2];
            nightTenSecondComboBox.SelectedIndex = currentFile.nightPokemon[3];
            nightTenThirdComboBox.SelectedIndex = currentFile.nightPokemon[4];
            nightTenFourthComboBox.SelectedIndex = currentFile.nightPokemon[5];
            nightFiveFirstComboBox.SelectedIndex = currentFile.nightPokemon[6];
            nightFiveSecondComboBox.SelectedIndex = currentFile.nightPokemon[7];
            nightFourFirstComboBox.SelectedIndex = currentFile.nightPokemon[8];
            nightFourSecondComboBox.SelectedIndex = currentFile.nightPokemon[9];
            nightOneFirstComboBox.SelectedIndex = currentFile.nightPokemon[10];
            nightOneSecondComboBox.SelectedIndex = currentFile.nightPokemon[11];

            /* Setup radio sound encounters controls */
            hoennFirstComboBox.SelectedIndex = currentFile.hoennMusicPokemon[0];
            hoennSecondComboBox.SelectedIndex = currentFile.hoennMusicPokemon[1];
            sinnohFirstComboBox.SelectedIndex = currentFile.sinnohMusicPokemon[0];
            sinnohSecondComboBox.SelectedIndex = currentFile.sinnohMusicPokemon[1];

            /* Setup rock smash controls */
            rockSmashNinetyComboBox.SelectedIndex = currentFile.rockSmashPokemon[0];
            rockSmashTenComboBox.SelectedIndex = currentFile.rockSmashPokemon[1];
            rockSmashNinetyMinLevelUpDown.Value = currentFile.rockSmashMinLevels[0];
            rockSmashTenMinLevelUpDown.Value = currentFile.rockSmashMinLevels[1];
            rockSmashNinetyMaxLevelUpDown.Value = currentFile.rockSmashMaxLevels[0];
            rockSmashTenMaxLevelUpDown.Value = currentFile.rockSmashMaxLevels[1];

            /* Setup swarm encounters controls */
            grassSwarmComboBox.SelectedIndex = currentFile.swarmPokemon[0];
            surfSwarmComboBox.SelectedIndex = currentFile.swarmPokemon[1];
            goodRodSwarmComboBox.SelectedIndex = currentFile.swarmPokemon[2];
            superRodSwarmComboBox.SelectedIndex = currentFile.swarmPokemon[3];

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
            currentFile = new EncounterFileHGSS(new FileStream(of.FileName, FileMode.Open));

            /* Update controls */
            SetupControls();
        }
		private void selectEncounterComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            
            currentFile = new EncounterFileHGSS(selectEncounterComboBox.SelectedIndex);
            SetupControls();
        }
        private void saveEncountersButton_Click(object sender, EventArgs e) {
            currentFile.SaveToFileDefaultDir(selectEncounterComboBox.SelectedIndex);
        }

        private void walkingRateUpDown_ValueChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingRate = (byte)walkingRateUpDown.Value;
        }
        private void rockSmashRateUpDown_ValueChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.rockSmashRate = (byte)rockSmashRateUpDown.Value;
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
        private void morningTwentyFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[0] = (ushort)morningTwentyFirstComboBox.SelectedIndex;
        }
        private void morningTwentySecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[1] = (ushort)morningTwentySecondComboBox.SelectedIndex;
        }
        private void morningTenFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[2] = (ushort)morningTenFirstComboBox.SelectedIndex;
        }
        private void morningTenSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[3] = (ushort)morningTenSecondComboBox.SelectedIndex;
        }
        private void morningTenThirdComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[4] = (ushort)morningTenThirdComboBox.SelectedIndex;
        }
        private void morningTenFourthComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[5] = (ushort)morningTenFourthComboBox.SelectedIndex;
        }
        private void morningFiveFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[6] = (ushort)morningFiveFirstComboBox.SelectedIndex;
        }
        private void morningFiveSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[7] = (ushort)morningFiveSecondComboBox.SelectedIndex;
        }
        private void morningFourFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[8] = (ushort)morningFourFirstComboBox.SelectedIndex;
        }
        private void morningFourSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[9] = (ushort)morningFourSecondComboBox.SelectedIndex;
        }
        private void morningOneFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[10] = (ushort)morningOneFirstComboBox.SelectedIndex;
        }
        private void morningOneSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.morningPokemon[11] = (ushort)morningOneSecondComboBox.SelectedIndex;
        }
        private void dayTwentyFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.dayPokemon[0] = (ushort)dayTwentyFirstComboBox.SelectedIndex;
        }
        private void dayTwentySecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.dayPokemon[1] = (ushort)dayTwentySecondComboBox.SelectedIndex;
        }
        private void dayTenFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.dayPokemon[2] = (ushort)dayTenFirstComboBox.SelectedIndex;
        }
        private void dayTenSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.dayPokemon[3] = (ushort)dayTenSecondComboBox.SelectedIndex;
        }
        private void dayTenThirdComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.dayPokemon[4] = (ushort)dayTenThirdComboBox.SelectedIndex;
        }
        private void dayTenFourthComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.dayPokemon[5] = (ushort)dayTenFourthComboBox.SelectedIndex;
        }
        private void dayFiveFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.dayPokemon[6] = (ushort)dayFiveFirstComboBox.SelectedIndex;
        }
        private void dayFiveSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.dayPokemon[7] = (ushort)dayFiveSecondComboBox.SelectedIndex;
        }
        private void dayFourFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.dayPokemon[8] = (ushort)dayFourFirstComboBox.SelectedIndex;
        }
        private void dayFourSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.dayPokemon[9] = (ushort)dayFourSecondComboBox.SelectedIndex;
        }
        private void dayOneFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.dayPokemon[10] = (ushort)dayOneFirstComboBox.SelectedIndex;
        }
        private void dayOneSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.dayPokemon[11] = (ushort)dayOneSecondComboBox.SelectedIndex;
        }
        private void nightTwentyFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.nightPokemon[0] = (ushort)nightTwentyFirstComboBox.SelectedIndex;
        }
        private void nightTwentySecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.nightPokemon[1] = (ushort)nightTwentySecondComboBox.SelectedIndex;
        }
        private void nightTenFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.nightPokemon[2] = (ushort)nightTenFirstComboBox.SelectedIndex;
        }
        private void nightTenSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.nightPokemon[3] = (ushort)nightTenSecondComboBox.SelectedIndex;
        }
        private void nightTenThirdComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.nightPokemon[4] = (ushort)nightTenThirdComboBox.SelectedIndex;
        }
        private void nightTenFourthComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.nightPokemon[5] = (ushort)nightTenFourthComboBox.SelectedIndex;
        }
        private void nightFiveFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.nightPokemon[6] = (ushort)nightFiveFirstComboBox.SelectedIndex;
        }
        private void nightFiveSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.nightPokemon[7] = (ushort)nightFiveSecondComboBox.SelectedIndex;
        }
        private void nightFourFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.nightPokemon[8] = (ushort)nightFourFirstComboBox.SelectedIndex;
        }
        private void nightFourSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.nightPokemon[9] = (ushort)nightFourSecondComboBox.SelectedIndex;
        }
        private void nightOneFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.nightPokemon[10] = (ushort)nightOneFirstComboBox.SelectedIndex;
        }
        private void nightOneSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.nightPokemon[11] = (ushort)nightOneSecondComboBox.SelectedIndex;
        }

        private void twentyFirstLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[0] = (byte)twentyFirstLevelUpDown.Value;
        }
        private void twentySecondLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[1] = (byte)twentySecondLevelUpDown.Value;
        }
        private void tenFirstLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[2] = (byte)tenFirstLevelUpDown.Value;
        }
        private void tenSecondLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[3] = (byte)tenSecondLevelUpDown.Value;
        }
        private void tenThirdLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[4] = (byte)tenThirdLevelUpDown.Value;
        }
        private void tenFourthLevelUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[5] = (byte)tenFourthLevelUpDown.Value;
        }
        private void fiveFirstLevelUpDown_ValueChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[6] = (byte)fiveFirstLevelUpDown.Value;
        }
        private void fiveSecondLevelUpDown_ValueChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[7] = (byte)fiveSecondLevelUpDown.Value;
        }
        private void fourFirstLevelUpDown_ValueChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[8] = (byte)fourFirstLevelUpDown.Value;
        }
        private void fourSecondLevelUpDown_ValueChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[9] = (byte)fourSecondLevelUpDown.Value;
        }
        private void oneFirstLevelUpDown_ValueChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[10] = (byte)oneFirstLevelUpDown.Value;
        }
        private void oneSecondLevelUpDown_ValueChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.walkingLevels[11] = (byte)oneSecondLevelUpDown.Value;
        }

        private void hoennFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.hoennMusicPokemon[0] = (ushort)hoennFirstComboBox.SelectedIndex;
        }
        private void hoennSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.hoennMusicPokemon[1] = (ushort)hoennSecondComboBox.SelectedIndex;
        }
        private void sinnohFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.sinnohMusicPokemon[0] = (ushort)sinnohFirstComboBox.SelectedIndex;
        }
        private void sinnohSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.sinnohMusicPokemon[1] = (ushort)sinnohSecondComboBox.SelectedIndex;
        }

        private void rockSmashNinetyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.rockSmashPokemon[0] = (ushort)rockSmashNinetyComboBox.SelectedIndex;
        }
        private void rockSmashTenComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.rockSmashPokemon[1] = (ushort)rockSmashTenComboBox.SelectedIndex;
        }
        private void rockSmashNinetyMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.rockSmashMinLevels[0] = (byte)rockSmashNinetyMinLevelUpDown.Value;
        }
        private void rockSmashTenMinLevelUpDown_ValueChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.rockSmashMinLevels[1] = (byte)rockSmashTenMinLevelUpDown.Value;
        }
        private void rockSmashNinetyMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.rockSmashMaxLevels[0] = (byte)rockSmashNinetyMaxLevelUpDown.Value;
        }
        private void rockSmashTenMaxLevelUpDown_ValueChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.rockSmashMaxLevels[1] = (byte)rockSmashNinetyMaxLevelUpDown.Value;
        }
        private void rockSmashNinetyMaxLevelUpDown_ValueChanged_1(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.rockSmashMaxLevels[0] = (byte)rockSmashNinetyMaxLevelUpDown.Value;
        }
        private void rockSmashTenMaxLevelUpDown_ValueChanged_1(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.rockSmashMaxLevels[1] = (byte)rockSmashTenMaxLevelUpDown.Value;
        }

        private void grassSwarmComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.swarmPokemon[0] = (ushort)grassSwarmComboBox.SelectedIndex;
        }
        private void surfSwarmComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.swarmPokemon[1] = (ushort)surfSwarmComboBox.SelectedIndex;
        }
        private void goodRodSwarmComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.swarmPokemon[2] = (ushort)goodRodSwarmComboBox.SelectedIndex;
        }
        private void superRodSwarmComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        	if (Helpers.HandlersDisabled) { 
                return; 
            }
            currentFile.swarmPokemon[3] = (ushort)superRodSwarmComboBox.SelectedIndex;
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

        private void addEncounterFileButton_Click(object sender, EventArgs e) {
            int encounterCount = selectEncounterComboBox.Items.Count;

            /* Add new encounter file to encounter folder */
            string encounterFilePath = encounterFileFolder + "\\" + encounterCount.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(encounterFilePath, FileMode.Create))) {
                writer.Write(new EncounterFileHGSS().ToByteArray());
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
                if (selectEncounterComboBox.SelectedIndex == encounterToDelete) {
                    selectEncounterComboBox.SelectedIndex--;
                }

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
