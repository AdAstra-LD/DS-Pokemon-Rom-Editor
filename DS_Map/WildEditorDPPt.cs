using DSPRE.ROMFiles;
using System;
using System.IO;
using System.Windows.Forms;

namespace DSPRE {
    public partial class WildEditorDPPt : Form {
        public string encounterFileFolder { get; private set; }
        EncounterFileDPPt currentFile;
        bool disableHandlers = new bool();

        public WildEditorDPPt(string dirPath, string[] names, int encToOpen) {
            InitializeComponent();
            encounterFileFolder = dirPath;

            for (int i = 0; i < Directory.GetFiles(encounterFileFolder).Length; i++) {
                selectEncounterComboBox.Items.Add("Encounters File " + i.ToString());
            }

            foreach (TabPage page in mainTabControl.TabPages) {
                foreach (Control g in page.Controls) {
                    if (g != null && g.GetType() == typeof(GroupBox)) {
                        foreach (Control c in g.Controls) {
                            if (c != null && c.GetType() == typeof(ComboBox)) {
                                (c as ComboBox).Items.AddRange(names);
                            }
                        }
                    }
                }
            }

            if (encToOpen > selectEncounterComboBox.Items.Count) {
                MessageBox.Show("This encounter file doesn't exist.\n" +
                "Enc #0 will be loaded, instead.", "WildPoké Data not found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                selectEncounterComboBox.SelectedIndex = 0;
            } else {
                selectEncounterComboBox.SelectedIndex = encToOpen;
            }
        }
        private void SetupControls() {
            disableHandlers = true;

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
            surfSixtyMinUpDown.Value = currentFile.surfMinLevels[0];
            surfSixtyMaxUpDown.Value = currentFile.surfMaxLevels[0];

            surfThirtyComboBox.SelectedIndex = currentFile.surfPokemon[1];
            surfThirtyMinUpDown.Value = currentFile.surfMinLevels[1];
            surfThirtyMaxUpDown.Value = currentFile.surfMaxLevels[1];

            surfFiveComboBox.SelectedIndex = currentFile.surfPokemon[2];
            surfFiveMinUpDown.Value = currentFile.surfMinLevels[2];
            surfFiveMaxUpDown.Value = currentFile.surfMaxLevels[2];

            surfFourComboBox.SelectedIndex = currentFile.surfPokemon[3];
            surfFourMinUpDown.Value = currentFile.surfMinLevels[3];
            surfFourMaxUpDown.Value = currentFile.surfMaxLevels[3];

            surfOneComboBox.SelectedIndex = currentFile.surfPokemon[4];
            surfOneMinUpDown.Value = currentFile.surfMinLevels[4];
            surfOneMaxUpDown.Value = currentFile.surfMaxLevels[4];

            oldRodSixtyComboBox.SelectedIndex = currentFile.oldRodPokemon[0];
            oldRodSixtyMinUpDown.Value = currentFile.oldRodMinLevels[0];
            oldRodSixtyMinUpDown.Value = currentFile.oldRodMaxLevels[0];

            oldRodThirtyComboBox.SelectedIndex = currentFile.oldRodPokemon[1];
            oldRodThirtyMinUpDown.Value = currentFile.oldRodMinLevels[1];
            oldRodThirtyMaxUpDown.Value = currentFile.oldRodMaxLevels[1];

            oldRodFiveComboBox.SelectedIndex = currentFile.oldRodPokemon[2];
            oldRodFiveMinUpDown.Value = currentFile.oldRodMinLevels[2];
            oldRodFiveMaxUpDown.Value = currentFile.oldRodMaxLevels[2];

            oldRodFourComboBox.SelectedIndex = currentFile.oldRodPokemon[3];
            oldRodFourMinUpDown.Value = currentFile.oldRodMinLevels[3];
            oldRodFourMaxUpDown.Value = currentFile.oldRodMaxLevels[3];

            oldRodOneComboBox.SelectedIndex = currentFile.oldRodPokemon[4];
            oldRodOneMinUpDown.Value = currentFile.oldRodMinLevels[4];
            oldRodOneMaxUpDown.Value = currentFile.oldRodMaxLevels[4];

            goodRodSixtyComboBox.SelectedIndex = currentFile.goodRodPokemon[0];
            goodRodSixtyMinUpDown.Value = currentFile.goodRodMinLevels[0];
            goodRodSixtyMinUpDown.Value = currentFile.goodRodMaxLevels[0];

            goodRodThirtyComboBox.SelectedIndex = currentFile.goodRodPokemon[1];
            goodRodThirtyMinUpDown.Value = currentFile.goodRodMinLevels[1];
            goodRodThirtyMaxUpDown.Value = currentFile.goodRodMaxLevels[1];

            goodRodFiveComboBox.SelectedIndex = currentFile.goodRodPokemon[2];
            goodRodFiveMinUpDown.Value = currentFile.goodRodMinLevels[2];
            goodRodFiveMaxUpDown.Value = currentFile.goodRodMaxLevels[2];

            goodRodFourComboBox.SelectedIndex = currentFile.goodRodPokemon[3];
            goodRodFourMinUpDown.Value = currentFile.goodRodMinLevels[3];
            goodRodFourMaxUpDown.Value = currentFile.goodRodMaxLevels[3];

            goodRodOneComboBox.SelectedIndex = currentFile.goodRodPokemon[4];
            goodRodOneMinUpDown.Value = currentFile.goodRodMinLevels[4];
            goodRodOneMaxUpDown.Value = currentFile.goodRodMaxLevels[4];

            superRodSixtyComboBox.SelectedIndex = currentFile.superRodPokemon[0];
            superRodSixtyMinUpDown.Value = currentFile.superRodMinLevels[0];
            superRodSixtyMinUpDown.Value = currentFile.superRodMaxLevels[0];

            superRodThirtyComboBox.SelectedIndex = currentFile.superRodPokemon[1];
            superRodThirtyMinUpDown.Value = currentFile.superRodMinLevels[1];
            superRodThirtyMaxUpDown.Value = currentFile.superRodMaxLevels[1];

            superRodFiveComboBox.SelectedIndex = currentFile.superRodPokemon[2];
            superRodFiveMinUpDown.Value = currentFile.superRodMinLevels[2];
            superRodFiveMaxUpDown.Value = currentFile.superRodMaxLevels[2];

            superRodFourComboBox.SelectedIndex = currentFile.superRodPokemon[3];
            superRodFourMinUpDown.Value = currentFile.superRodMinLevels[3];
            superRodFourMaxUpDown.Value = currentFile.superRodMaxLevels[3];

            superRodOneComboBox.SelectedIndex = currentFile.superRodPokemon[4];
            superRodOneMinUpDown.Value = currentFile.superRodMinLevels[4];
            superRodOneMaxUpDown.Value = currentFile.superRodMaxLevels[4];

            disableHandlers = false;
        }

        private void exportEncounterFileButton_Click(object sender, EventArgs e) {
            currentFile.SaveToFileExplorePath("Encounter File " + selectEncounterComboBox.SelectedIndex);
        }
        private void importEncounterFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .wld file */
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Wild Encounters File (" + "*." + EncounterFile.extension + ")" + "|" + "*." + EncounterFile.extension;
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update encounter file object in memory */
            currentFile = new EncounterFileDPPt(new FileStream(of.FileName, FileMode.Open));

            /* Update controls */
            SetupControls();
        }
        private void selectEncounterComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            currentFile = new EncounterFileDPPt(selectEncounterComboBox.SelectedIndex);
            SetupControls();
        }
        private void saveEncountersButton_Click(object sender, EventArgs e) {
            currentFile.SaveToFileDefaultDir(selectEncounterComboBox.SelectedIndex);
        }
        private void walkingTwentyFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.walkingPokemon[0] = (uint)walkingTwentyFirstComboBox.SelectedIndex;
        }
        private void walkingTwentySecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.walkingPokemon[1] = (uint)walkingTwentySecondComboBox.SelectedIndex;
        }
        private void walkingTenFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.walkingPokemon[2] = (uint)walkingTenFirstComboBox.SelectedIndex;
        }
        private void walkingTenSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.walkingPokemon[3] = (uint)walkingTenSecondComboBox.SelectedIndex;
        }
        private void walkingTenThirdComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.walkingPokemon[4] = (uint)walkingTenThirdComboBox.SelectedIndex;
        }
        private void walkingTenFourthComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingPokemon[5] = (uint)walkingTenFourthComboBox.SelectedIndex;
        }
        private void walkingFiveFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingPokemon[6] = (uint)walkingFiveFirstComboBox.SelectedIndex;
        }
        private void walkingFiveSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingPokemon[7] = (uint)walkingFiveSecondComboBox.SelectedIndex;
        }
        private void walkingFourFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingPokemon[8] = (uint)walkingFourFirstComboBox.SelectedIndex;
        }
        private void walkingFourSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingPokemon[9] = (uint)walkingFourSecondComboBox.SelectedIndex;
        }
        private void walkingOneFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingPokemon[10] = (uint)walkingOneFirstComboBox.SelectedIndex;
        }
        private void walkingOneSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingPokemon[11] = (uint)walkingOneSecondComboBox.SelectedIndex;
        }
        private void morningFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.morningPokemon[0] = (uint)morningFirstComboBox.SelectedIndex;
        }
        private void morningSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.morningPokemon[1] = (uint)morningSecondComboBox.SelectedIndex;
        }
        private void nightFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.nightPokemon[0] = (uint)nightFirstComboBox.SelectedIndex;
        }
        private void nightSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.nightPokemon[1] = (uint)nightSecondComboBox.SelectedIndex;
        }
        private void swarmFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.swarmPokemon[0] = (ushort)swarmFirstComboBox.SelectedIndex;
        }
        private void swarmSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.swarmPokemon[1] = (ushort)swarmSecondComboBox.SelectedIndex;
        }
        private void rubyFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.rubyPokemon[0] = (uint)rubyFirstComboBox.SelectedIndex;
        }
        private void rubySecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.rubyPokemon[1] = (uint)rubySecondComboBox.SelectedIndex;
        }
        private void sapphireFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.sapphirePokemon[0] = (uint)sapphireFirstComboBox.SelectedIndex;
        }
        private void sapphireSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.sapphirePokemon[1] = (uint)sapphireSecondComboBox.SelectedIndex;
        }
        private void emeraldFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.emeraldPokemon[0] = (uint)emeraldFirstComboBox.SelectedIndex;
        }
        private void emeraldSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.emeraldPokemon[1] = (uint)emeraldSecondComboBox.SelectedIndex;
        }
        private void fireRedFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.fireRedPokemon[0] = (uint)fireRedFirstComboBox.SelectedIndex;
        }
        private void fireRedSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.fireRedPokemon[1] = (uint)fireRedSecondComboBox.SelectedIndex;
        }
        private void leafGreenFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.leafGreenPokemon[0] = (uint)leafGreenFirstComboBox.SelectedIndex;
        }
        private void leafGreenSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.leafGreenPokemon[1] = (uint)leafGreenSecondComboBox.SelectedIndex;
        }
        private void radarFirstComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.radarPokemon[0] = (uint)radarFirstComboBox.SelectedIndex;
        }
        private void radarSecondComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.radarPokemon[1] = (uint)radarSecondComboBox.SelectedIndex;
        }
        private void radarThirdComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.radarPokemon[2] = (uint)radarThirdComboBox.SelectedIndex;
        }
        private void radarFourthComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.radarPokemon[3] = (uint)radarThirdComboBox.SelectedIndex;
        }
        private void surfSixtyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.surfPokemon[0] = (ushort)surfSixtyComboBox.SelectedIndex;
        }
        private void surfThirtyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.surfPokemon[1] = (ushort)surfThirtyComboBox.SelectedIndex;
        }
        private void surfFiveComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.surfPokemon[2] = (ushort)surfFiveComboBox.SelectedIndex;
        }
        private void surfFourComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.surfPokemon[3] = (ushort)surfFourComboBox.SelectedIndex;
        }
        private void surfOneComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.surfPokemon[4] = (ushort)surfOneComboBox.SelectedIndex;
        }
        private void oldRodSixtyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.oldRodPokemon[0] = (ushort)oldRodSixtyComboBox.SelectedIndex;
        }
        private void oldRodThirtyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.oldRodPokemon[1] = (ushort)oldRodThirtyComboBox.SelectedIndex;
        }
        private void oldRodFiveComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.oldRodPokemon[2] = (ushort)oldRodFiveComboBox.SelectedIndex;
        }
        private void oldRodFourComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.oldRodPokemon[3] = (ushort)oldRodFourComboBox.SelectedIndex;
        }
        private void oldRodOneComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.oldRodPokemon[4] = (ushort)oldRodOneComboBox.SelectedIndex;
        }
        private void goodRodSixtyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.goodRodPokemon[0] = (ushort)goodRodSixtyComboBox.SelectedIndex;
        }
        private void goodRodThirtyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.goodRodPokemon[1] = (ushort)goodRodThirtyComboBox.SelectedIndex;
        }
        private void goodRodFiveComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.goodRodPokemon[2] = (ushort)goodRodFiveComboBox.SelectedIndex;
        }
        private void goodRodFourComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.goodRodPokemon[3] = (ushort)goodRodFourComboBox.SelectedIndex;
        }
        private void goodRodOneComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.goodRodPokemon[4] = (ushort)goodRodOneComboBox.SelectedIndex;
        }
        private void superRodSixtyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.superRodPokemon[0] = (ushort)superRodSixtyComboBox.SelectedIndex;
        }
        private void superRodThirtyComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.superRodPokemon[1] = (ushort)superRodThirtyComboBox.SelectedIndex;
        }
        private void superRodFiveComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.superRodPokemon[2] = (ushort)superRodFiveComboBox.SelectedIndex;
        }
        private void superRodFourComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.superRodPokemon[3] = (ushort)superRodFourComboBox.SelectedIndex;
        }
        private void superRodOneComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.superRodPokemon[4] = (ushort)superRodOneComboBox.SelectedIndex;
        }

        /* Walking levels controls */
        private void walkingTwentyFirstUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingLevels[0] = (byte)walkingTwentyFirstUpDown.Value;
        }
        private void walkingTwentySecondUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.walkingLevels[1] = (byte)walkingTwentySecondUpDown.Value;
        }
        private void walkingTenFirstUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.walkingLevels[2] = (byte)walkingTenFirstUpDown.Value;
        }
        private void walkingTenSecondUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.walkingLevels[3] = (byte)walkingTenSecondUpDown.Value;
        }
        private void walkingTenThirdUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingLevels[4] = (byte)walkingTenThirdUpDown.Value;
        }
        private void walkingTenFourthUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingLevels[5] = (byte)walkingTenFourthUpDown.Value;
        }
        private void walkingFiveFirstUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingLevels[6] = (byte)walkingFiveFirstUpDown.Value;
        }
        private void walkingFiveSecondUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.walkingLevels[7] = (byte)walkingFiveSecondUpDown.Value;
        }
        private void walkingFourFirstUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.walkingLevels[8] = (byte)walkingFourFirstUpDown.Value;
        }
        private void walkingFourSecondUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.walkingLevels[9] = (byte)walkingFourSecondUpDown.Value;
        }
        private void walkingOneFirstUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.walkingLevels[10] = (byte)walkingOneFirstUpDown.Value;
        }
        private void walkingOneSecondUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingLevels[11] = (byte)walkingOneSecondUpDown.Value;
        }

        /* Water levels controls */
        private void surfSixtyMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.surfMinLevels[0] = (byte)surfSixtyMinUpDown.Value;
        }
        private void surfThirtyMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.surfMinLevels[1] = (byte)surfThirtyMinUpDown.Value;
        }
        private void surfFiveMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.surfMinLevels[2] = (byte)surfFiveMinUpDown.Value;
        }
        private void surfFourMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.surfMinLevels[3] = (byte)surfFourMinUpDown.Value;
        }
        private void surfOneMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.surfMinLevels[4] = (byte)surfOneMinUpDown.Value;
        }
        private void surfSixtyMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.surfMaxLevels[0] = (byte)surfSixtyMaxUpDown.Value;
        }
        private void surfThirtyMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.surfMaxLevels[1] = (byte)surfThirtyMaxUpDown.Value;
        }
        private void surfFiveMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.surfMaxLevels[2] = (byte)surfFiveMaxUpDown.Value;
        }
        private void surfFourMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.surfMaxLevels[3] = (byte)surfFourMaxUpDown.Value;
        }
        private void surfOneMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.surfMaxLevels[4] = (byte)surfOneMaxUpDown.Value;
        }

        private void oldRodSixtyMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.oldRodMinLevels[0] = (byte)oldRodSixtyMinUpDown.Value;
        }
        private void oldRodThirtyMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.oldRodMinLevels[1] = (byte)oldRodThirtyMinUpDown.Value;
        }
        private void oldRodFiveMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.oldRodMinLevels[2] = (byte)oldRodFiveMinUpDown.Value;
        }
        private void oldRodFourMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.oldRodMinLevels[3] = (byte)oldRodFourMinUpDown.Value;
        }
        private void oldRodOneMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.oldRodMinLevels[4] = (byte)oldRodOneMinUpDown.Value;
        }
        private void oldRodSixtyMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.oldRodMaxLevels[0] = (byte)oldRodSixtyMaxUpDown.Value;
        }
        private void oldRodThirtyMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.oldRodMaxLevels[1] = (byte)oldRodThirtyMaxUpDown.Value;
        }
        private void oldRodFiveMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.oldRodMaxLevels[2] = (byte)oldRodFiveMaxUpDown.Value;
        }
        private void oldRodFourMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.oldRodMaxLevels[3] = (byte)oldRodFourMaxUpDown.Value;
        }
        private void oldRodOneMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.oldRodMaxLevels[4] = (byte)oldRodOneMaxUpDown.Value;
        }

        private void goodRodSixtyMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.goodRodMinLevels[0] = (byte)goodRodSixtyMinUpDown.Value;
        }
        private void goodRodThirtyMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.goodRodMinLevels[1] = (byte)goodRodThirtyMinUpDown.Value;
        }
        private void goodRodFiveMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.goodRodMinLevels[2] = (byte)goodRodFiveMinUpDown.Value;
        }
        private void goodRodFourMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.goodRodMinLevels[3] = (byte)goodRodFourMinUpDown.Value;
        }
        private void goodRodOneMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.goodRodMinLevels[4] = (byte)goodRodOneMinUpDown.Value;
        }
        private void goodRodSixtyMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.goodRodMaxLevels[0] = (byte)goodRodSixtyMaxUpDown.Value;
        }
        private void goodRodThirtyMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.goodRodMaxLevels[1] = (byte)goodRodThirtyMaxUpDown.Value;
        }
        private void goodRodFiveMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.goodRodMaxLevels[2] = (byte)goodRodFiveMaxUpDown.Value;
        }
        private void goodRodFourMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.goodRodMaxLevels[3] = (byte)goodRodFourMaxUpDown.Value;
        }
        private void goodRodOneMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.goodRodMaxLevels[4] = (byte)goodRodOneMaxUpDown.Value;
        }

        private void superRodSixtyMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.superRodMinLevels[0] = (byte)superRodSixtyMinUpDown.Value;
        }
        private void superRodThirtyMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.superRodMinLevels[1] = (byte)superRodThirtyMinUpDown.Value;
        }
        private void superRodFiveMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.superRodMinLevels[2] = (byte)superRodFiveMinUpDown.Value;
        }
        private void superRodFourMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.superRodMinLevels[3] = (byte)superRodFourMinUpDown.Value;
        }
        private void superRodOneMinUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return; 
            }
            currentFile.superRodMinLevels[4] = (byte)superRodOneMinUpDown.Value;
        }
        private void superRodSixtyMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.superRodMaxLevels[0] = (byte)superRodSixtyMaxUpDown.Value;
        }
        private void superRodThirtyMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.superRodMaxLevels[1] = (byte)superRodThirtyMaxUpDown.Value;
        }
        private void superRodFiveMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return;
            }
            currentFile.superRodMaxLevels[2] = (byte)superRodFiveMaxUpDown.Value;
        }
        private void superRodFourMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.superRodMaxLevels[3] = (byte)superRodFourMaxUpDown.Value;
        }
        private void superRodOneMaxUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.superRodMaxLevels[4] = (byte)superRodOneMaxUpDown.Value;
        }

        /* Encounter rate controls */
        private void walkingRateUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.walkingRate = (byte)walkingRateUpDown.Value;
        }
        private void surfRateUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.surfRate = (byte)surfRateUpDown.Value;
        }
        private void oldRodRateUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }
            currentFile.oldRodRate = (byte)oldRodRateUpDown.Value;
        }
        private void goodRodRateUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentFile.goodRodRate = (byte)goodRodRateUpDown.Value;
        }

        private void superRodRateUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
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
