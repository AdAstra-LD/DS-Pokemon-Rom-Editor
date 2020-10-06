using System;
using System.IO;
using System.Windows.Forms;

namespace DS_Map
{
    public partial class WildEditorDPPt : Form
    {
        string encounterFileFolder;
        EncounterFileDPPt currentFile;
        bool disableHandlers = new bool();

        public WildEditorDPPt(string folderPath, string[] names)
        {
            InitializeComponent();
            encounterFileFolder = folderPath;

            for (int i = 0; i < Directory.GetFiles(folderPath).Length; i++) selectEncounterComboBox.Items.Add("Encounters File " + i.ToString());
            foreach (TabPage page in mainTabControl.TabPages)
            {
                foreach (Control g in page.Controls)
                {
                    GroupBox group = g as GroupBox;
                    if (group != null)
                    {
                        foreach (Control c in group.Controls)
                        {
                            ComboBox box = c as ComboBox;
                            if (box != null) box.Items.AddRange(names);
                        }
                    }                  
                }               
            }           
        }
        private void WildEditorDPPt_Load(object sender, EventArgs e)
        {
            
        }

        private void SetupControls()
        {
            disableHandlers = true;

            /* Setup encounter rates controls */
            walkingRateUpDown.Value = currentFile.walkingRate;
            surfRateUpDown.Value = currentFile.surfRate;
            oldRodRateUpDown.Value = currentFile.oldRodRate;
            goodRodRateUpDown.Value = currentFile.goodRodRate;
            superRodRateUpDown.Value = currentFile.superRodRate;

            /* Walking encounters controls setup */
            walkingTwentyFirstComboBox.SelectedIndex = (int)currentFile.walkingPokémon[0];
            walkingTwentyFirstUpDown.Value = currentFile.walkingLevels[0];
            walkingTwentySecondComboBox.SelectedIndex = (int)currentFile.walkingPokémon[1];
            walkingTwentySecondUpDown.Value = currentFile.walkingLevels[1];
            walkingTenFirstComboBox.SelectedIndex = (int)currentFile.walkingPokémon[2];
            walkingTenFirstUpDown.Value = currentFile.walkingLevels[2];
            walkingTenSecondComboBox.SelectedIndex = (int)currentFile.walkingPokémon[3];
            walkingTenSecondUpDown.Value = currentFile.walkingLevels[3];
            walkingTenThirdComboBox.SelectedIndex = (int)currentFile.walkingPokémon[4];
            walkingTenThirdUpDown.Value = currentFile.walkingLevels[4];
            walkingTenFourthComboBox.SelectedIndex = (int)currentFile.walkingPokémon[5];
            walkingTenFourthUpDown.Value = currentFile.walkingLevels[5];
            walkingFiveFirstComboBox.SelectedIndex = (int)currentFile.walkingPokémon[6];
            walkingFiveFirstUpDown.Value = currentFile.walkingLevels[6];
            walkingFiveSecondComboBox.SelectedIndex = (int)currentFile.walkingPokémon[7];
            walkingFiveSecondUpDown.Value = currentFile.walkingLevels[7];
            walkingFourFirstComboBox.SelectedIndex = (int)currentFile.walkingPokémon[8];
            walkingFourFirstUpDown.Value = currentFile.walkingLevels[8];
            walkingFourSecondComboBox.SelectedIndex = (int)currentFile.walkingPokémon[9];
            walkingFourSecondUpDown.Value = currentFile.walkingLevels[9];
            walkingOneFirstComboBox.SelectedIndex = (int)currentFile.walkingPokémon[10];
            walkingOneFirstUpDown.Value = currentFile.walkingLevels[10];
            walkingOneSecondComboBox.SelectedIndex = (int)currentFile.walkingPokémon[11];
            walkingOneSecondUpDown.Value = currentFile.walkingLevels[11];

            /* Time dependent encounters controls setup */
            morningFirstComboBox.SelectedIndex = (int)currentFile.morningPokémon[0];
            morningSecondComboBox.SelectedIndex = (int)currentFile.morningPokémon[1];
            nightFirstComboBox.SelectedIndex = (int)currentFile.nightPokémon[0];
            nightSecondComboBox.SelectedIndex = (int)currentFile.nightPokémon[1];
            swarmFirstComboBox.SelectedIndex = currentFile.swarmPokémon[0];
            swarmSecondComboBox.SelectedIndex = currentFile.swarmPokémon[1];

            /* Dual Slot encounters controls setup */
            rubyFirstComboBox.SelectedIndex = (int)currentFile.rubyPokémon[0];
            rubySecondComboBox.SelectedIndex = (int)currentFile.rubyPokémon[1];
            sapphireFirstComboBox.SelectedIndex = (int)currentFile.sapphirePokémon[0];
            sapphireSecondComboBox.SelectedIndex = (int)currentFile.sapphirePokémon[1];
            emeraldFirstComboBox.SelectedIndex = (int)currentFile.emeraldPokémon[0];
            emeraldSecondComboBox.SelectedIndex = (int)currentFile.emeraldPokémon[1];
            fireRedFirstComboBox.SelectedIndex = (int)currentFile.fireRedPokémon[0];
            fireRedSecondComboBox.SelectedIndex = (int)currentFile.fireRedPokémon[1];
            leafGreenFirstComboBox.SelectedIndex = (int)currentFile.leafGreenPokémon[0];
            leafGreenSecondComboBox.SelectedIndex = (int)currentFile.leafGreenPokémon[1];

            /* PokéRadar encounters controls setup */
            radarFirstComboBox.SelectedIndex = (int)currentFile.radarPokémon[0];
            radarSecondComboBox.SelectedIndex = (int)currentFile.radarPokémon[1];
            radarThirdComboBox.SelectedIndex = (int)currentFile.radarPokémon[2];
            radarFourthComboBox.SelectedIndex = (int)currentFile.radarPokémon[3];

            /* Water encounters controls setup */
            surfSixtyComboBox.SelectedIndex = currentFile.surfPokémon[0];
            surfSixtyMinUpDown.Value = currentFile.surfMinLevels[0];
            surfSixtyMaxUpDown.Value = currentFile.surfMaxLevels[0];

            surfThirtyComboBox.SelectedIndex = currentFile.surfPokémon[1];
            surfThirtyMinUpDown.Value = currentFile.surfMinLevels[1];
            surfThirtyMaxUpDown.Value = currentFile.surfMaxLevels[1];

            surfFiveComboBox.SelectedIndex = currentFile.surfPokémon[2];
            surfFiveMinUpDown.Value = currentFile.surfMinLevels[2];
            surfFiveMaxUpDown.Value = currentFile.surfMaxLevels[2];

            surfFourComboBox.SelectedIndex = currentFile.surfPokémon[3];
            surfFourMinUpDown.Value = currentFile.surfMinLevels[3];
            surfFourMaxUpDown.Value = currentFile.surfMaxLevels[3];

            surfOneComboBox.SelectedIndex = currentFile.surfPokémon[4];
            surfOneMinUpDown.Value = currentFile.surfMinLevels[4];
            surfOneMaxUpDown.Value = currentFile.surfMaxLevels[4];

            oldRodSixtyComboBox.SelectedIndex = currentFile.oldRodPokémon[0];
            oldRodSixtyMinUpDown.Value = currentFile.oldRodMinLevels[0];
            oldRodSixtyMinUpDown.Value = currentFile.oldRodMaxLevels[0];

            oldRodThirtyComboBox.SelectedIndex = currentFile.oldRodPokémon[1];
            oldRodThirtyMinUpDown.Value = currentFile.oldRodMinLevels[1];
            oldRodThirtyMaxUpDown.Value = currentFile.oldRodMaxLevels[1];

            oldRodFiveComboBox.SelectedIndex = currentFile.oldRodPokémon[2];
            oldRodFiveMinUpDown.Value = currentFile.oldRodMinLevels[2];
            oldRodFiveMaxUpDown.Value = currentFile.oldRodMaxLevels[2];

            oldRodFourComboBox.SelectedIndex = currentFile.oldRodPokémon[3];
            oldRodFourMinUpDown.Value = currentFile.oldRodMinLevels[3];
            oldRodFourMaxUpDown.Value = currentFile.oldRodMaxLevels[3];

            oldRodOneComboBox.SelectedIndex = currentFile.oldRodPokémon[4];
            oldRodOneMinUpDown.Value = currentFile.oldRodMinLevels[4];
            oldRodOneMaxUpDown.Value = currentFile.oldRodMaxLevels[4];

            goodRodSixtyComboBox.SelectedIndex = currentFile.goodRodPokémon[0];
            goodRodSixtyMinUpDown.Value = currentFile.goodRodMinLevels[0];
            goodRodSixtyMinUpDown.Value = currentFile.goodRodMaxLevels[0];

            goodRodThirtyComboBox.SelectedIndex = currentFile.goodRodPokémon[1];
            goodRodThirtyMinUpDown.Value = currentFile.goodRodMinLevels[1];
            goodRodThirtyMaxUpDown.Value = currentFile.goodRodMaxLevels[1];

            goodRodFiveComboBox.SelectedIndex = currentFile.goodRodPokémon[2];
            goodRodFiveMinUpDown.Value = currentFile.goodRodMinLevels[2];
            goodRodFiveMaxUpDown.Value = currentFile.goodRodMaxLevels[2];

            goodRodFourComboBox.SelectedIndex = currentFile.goodRodPokémon[3];
            goodRodFourMinUpDown.Value = currentFile.goodRodMinLevels[3];
            goodRodFourMaxUpDown.Value = currentFile.goodRodMaxLevels[3];

            goodRodOneComboBox.SelectedIndex = currentFile.goodRodPokémon[4];
            goodRodOneMinUpDown.Value = currentFile.goodRodMinLevels[4];
            goodRodOneMaxUpDown.Value = currentFile.goodRodMaxLevels[4];

            superRodSixtyComboBox.SelectedIndex = currentFile.superRodPokémon[0];
            superRodSixtyMinUpDown.Value = currentFile.superRodMinLevels[0];
            superRodSixtyMinUpDown.Value = currentFile.superRodMaxLevels[0];

            superRodThirtyComboBox.SelectedIndex = currentFile.superRodPokémon[1];
            superRodThirtyMinUpDown.Value = currentFile.superRodMinLevels[1];
            superRodThirtyMaxUpDown.Value = currentFile.superRodMaxLevels[1];

            superRodFiveComboBox.SelectedIndex = currentFile.superRodPokémon[2];
            superRodFiveMinUpDown.Value = currentFile.superRodMinLevels[2];
            superRodFiveMaxUpDown.Value = currentFile.superRodMaxLevels[2];

            superRodFourComboBox.SelectedIndex = currentFile.superRodPokémon[3];
            superRodFourMinUpDown.Value = currentFile.superRodMinLevels[3];
            superRodFourMaxUpDown.Value = currentFile.superRodMaxLevels[3];

            superRodOneComboBox.SelectedIndex = currentFile.superRodPokémon[4];
            superRodOneMinUpDown.Value = currentFile.superRodMinLevels[4];
            superRodOneMaxUpDown.Value = currentFile.superRodMaxLevels[4];

            disableHandlers = false;
        }

        private void exportEncounterFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Wild Encounters File (*.wld)|*.wld";
            if (sf.ShowDialog(this) != DialogResult.OK)
                return;

            using (BinaryWriter writer = new BinaryWriter(new FileStream(sf.FileName, FileMode.Create))) writer.Write(currentFile.SaveEncounterFile());
        }
        private void importEncounterFileButton_Click(object sender, EventArgs e)
        {
            /* Prompt user to select .wld file */
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Wild Encounters File (*.wld)|*.wld";
            if (of.ShowDialog(this) != DialogResult.OK) return;

            /* Update encounter file object in memory */
            currentFile = new EncounterFileDPPt(new FileStream(of.FileName, FileMode.Open));

            /* Update controls */
            SetupControls();
        }
        private void selectEncounterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string encounterFileIndex = selectEncounterComboBox.SelectedIndex.ToString("D4");
            currentFile = new EncounterFileDPPt(new FileStream(encounterFileFolder + "\\" + encounterFileIndex, FileMode.Open));
            SetupControls();
            
        }
        private void saveEventsButton_Click(object sender, EventArgs e)
        {
            string filePath = encounterFileFolder + "\\" + selectEncounterComboBox.SelectedIndex.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(filePath, FileMode.Create))) writer.Write(currentFile.SaveEncounterFile());
        }

        private void walkingTwentyFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingPokémon[0] = (uint)walkingTwentyFirstComboBox.SelectedIndex;
        }
        private void walkingTwentySecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingPokémon[1] = (uint)walkingTwentySecondComboBox.SelectedIndex;
        }
        private void walkingTenFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingPokémon[2] = (uint)walkingTenFirstComboBox.SelectedIndex;
        }
        private void walkingTenSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingPokémon[3] = (uint)walkingTenSecondComboBox.SelectedIndex;
        }
        private void walkingTenThirdComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingPokémon[4] = (uint)walkingTenThirdComboBox.SelectedIndex;
        }
        private void walkingTenFourthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingPokémon[5] = (uint)walkingTenFourthComboBox.SelectedIndex;
        }
        private void walkingFiveFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingPokémon[6] = (uint)walkingFiveFirstComboBox.SelectedIndex;
        }
        private void walkingFiveSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingPokémon[7] = (uint)walkingFiveSecondComboBox.SelectedIndex;
        }
        private void walkingFourFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingPokémon[8] = (uint)walkingFourFirstComboBox.SelectedIndex;
        }
        private void walkingFourSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingPokémon[9] = (uint)walkingFourSecondComboBox.SelectedIndex;
        }
        private void walkingOneFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingPokémon[10] = (uint)walkingOneFirstComboBox.SelectedIndex;
        }
        private void walkingOneSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingPokémon[11] = (uint)walkingOneSecondComboBox.SelectedIndex;
        }
        private void morningFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.morningPokémon[0] = (uint)morningFirstComboBox.SelectedIndex;
        }
        private void morningSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.morningPokémon[1] = (uint)morningSecondComboBox.SelectedIndex;
        }
        private void nightFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.nightPokémon[0] = (uint)nightFirstComboBox.SelectedIndex;
        }
        private void nightSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.nightPokémon[0] = (uint)nightSecondComboBox.SelectedIndex;
        }
        private void swarmFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.swarmPokémon[0] = (ushort)swarmFirstComboBox.SelectedIndex;
        }
        private void swarmSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.swarmPokémon[1] = (ushort)swarmSecondComboBox.SelectedIndex;
        }
        private void rubyFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.rubyPokémon[0] = (uint)rubyFirstComboBox.SelectedIndex;
        }
        private void rubySecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.rubyPokémon[1] = (uint)rubySecondComboBox.SelectedIndex;
        }
        private void sapphireFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.sapphirePokémon[0] = (uint)sapphireFirstComboBox.SelectedIndex;
        }
        private void sapphireSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.sapphirePokémon[1] = (uint)sapphireSecondComboBox.SelectedIndex;
        }
        private void emeraldFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.emeraldPokémon[0] = (uint)emeraldFirstComboBox.SelectedIndex;
        }
        private void emeraldSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.emeraldPokémon[1] = (uint)emeraldSecondComboBox.SelectedIndex;
        }
        private void fireRedFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.fireRedPokémon[0] = (uint)fireRedFirstComboBox.SelectedIndex;
        }
        private void fireRedSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.fireRedPokémon[1] = (uint)fireRedSecondComboBox.SelectedIndex;
        }
        private void leafGreenFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.leafGreenPokémon[0] = (uint)leafGreenFirstComboBox.SelectedIndex;
        }
        private void leafGreenSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.leafGreenPokémon[1] = (uint)leafGreenSecondComboBox.SelectedIndex;
        }
        private void radarFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.radarPokémon[0] = (uint)radarFirstComboBox.SelectedIndex;
        }
        private void radarSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.radarPokémon[1] = (uint)radarSecondComboBox.SelectedIndex;
        }
        private void radarThirdComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.radarPokémon[2] = (uint)radarThirdComboBox.SelectedIndex;
        }
        private void radarFourthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.radarPokémon[3] = (uint)radarThirdComboBox.SelectedIndex;
        }
        private void surfSixtyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfPokémon[0] = (ushort)surfSixtyComboBox.SelectedIndex;
        }
        private void surfThirtyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfPokémon[1] = (ushort)surfThirtyComboBox.SelectedIndex;
        }
        private void surfFiveComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfPokémon[2] = (ushort)surfFiveComboBox.SelectedIndex;
        }
        private void surfFourComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfPokémon[3] = (ushort)surfFourComboBox.SelectedIndex;
        }
        private void surfOneComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfPokémon[4] = (ushort)surfOneComboBox.SelectedIndex;
        }
        private void oldRodSixtyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodPokémon[0] = (ushort)oldRodSixtyComboBox.SelectedIndex;
        }
        private void oldRodThirtyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodPokémon[1] = (ushort)oldRodThirtyComboBox.SelectedIndex;
        }
        private void oldRodFiveComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodPokémon[2] = (ushort)oldRodFiveComboBox.SelectedIndex;
        }
        private void oldRodFourComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodPokémon[3] = (ushort)oldRodFourComboBox.SelectedIndex;
        }
        private void oldRodOneComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodPokémon[4] = (ushort)oldRodOneComboBox.SelectedIndex;
        }
        private void goodRodSixtyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodPokémon[0] = (ushort)goodRodSixtyComboBox.SelectedIndex;
        }
        private void goodRodThirtyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodPokémon[1] = (ushort)goodRodThirtyComboBox.SelectedIndex;
        }
        private void goodRodFiveComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodPokémon[2] = (ushort)goodRodFiveComboBox.SelectedIndex;
        }
        private void goodRodFourComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodPokémon[3] = (ushort)goodRodFourComboBox.SelectedIndex;
        }
        private void goodRodOneComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodPokémon[4] = (ushort)goodRodOneComboBox.SelectedIndex;
        }
        private void superRodSixtyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodPokémon[0] = (ushort)superRodSixtyComboBox.SelectedIndex;
        }
        private void superRodThirtyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodPokémon[1] = (ushort)superRodThirtyComboBox.SelectedIndex;
        }
        private void superRodFiveComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodPokémon[2] = (ushort)superRodFiveComboBox.SelectedIndex;
        }
        private void superRodFourComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodPokémon[3] = (ushort)superRodFourComboBox.SelectedIndex;
        }
        private void superRodOneComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodPokémon[4] = (ushort)superRodOneComboBox.SelectedIndex;
        }

        /* Walking levels controls */
        private void walkingTwentyFirstUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingLevels[0] = (byte)walkingTwentyFirstUpDown.Value;
        }
        private void walkingTwentySecondUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingLevels[1] = (byte)walkingTwentySecondUpDown.Value;
        }
        private void walkingTenFirstUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingLevels[2] = (byte)walkingTenFirstUpDown.Value;
        }
        private void walkingTenSecondUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingLevels[3] = (byte)walkingTenSecondUpDown.Value;
        }
        private void walkingTenThirdUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingLevels[4] = (byte)walkingTenThirdUpDown.Value;
        }
        private void walkingTenFourthUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingLevels[5] = (byte)walkingTenFourthUpDown.Value;
        }
        private void walkingFiveFirstUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingLevels[6] = (byte)walkingFiveFirstUpDown.Value;
        }
        private void walkingFiveSecondUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingLevels[7] = (byte)walkingFiveSecondUpDown.Value;
        }
        private void walkingFourFirstUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingLevels[8] = (byte)walkingFourFirstUpDown.Value;
        }
        private void walkingFourSecondUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingLevels[9] = (byte)walkingFourSecondUpDown.Value;
        }
        private void walkingOneFirstUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingLevels[10] = (byte)walkingOneFirstUpDown.Value;
        }
        private void walkingOneSecondUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingLevels[11] = (byte)walkingOneSecondUpDown.Value;
        }

        /* Water levels controls */
        private void surfSixtyMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfMinLevels[0] = (byte)surfSixtyMinUpDown.Value;
        }
        private void surfThirtyMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfMinLevels[1] = (byte)surfThirtyMinUpDown.Value;
        }
        private void surfFiveMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfMinLevels[2] = (byte)surfFiveMinUpDown.Value;
        }
        private void surfFourMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfMinLevels[3] = (byte)surfFourMinUpDown.Value;
        }
        private void surfOneMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfMinLevels[4] = (byte)surfOneMinUpDown.Value;
        }
        private void surfSixtyMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfMaxLevels[0] = (byte)surfSixtyMaxUpDown.Value;
        }
        private void surfThirtyMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfMaxLevels[1] = (byte)surfThirtyMaxUpDown.Value;
        }
        private void surfFiveMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfMaxLevels[2] = (byte)surfFiveMaxUpDown.Value;
        }
        private void surfFourMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfMaxLevels[3] = (byte)surfFourMaxUpDown.Value;
        }
        private void surfOneMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfMaxLevels[4] = (byte)surfOneMaxUpDown.Value;
        }

        private void oldRodSixtyMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodMinLevels[0] = (byte)oldRodSixtyMinUpDown.Value;
        }
        private void oldRodThirtyMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodMinLevels[1] = (byte)oldRodThirtyMinUpDown.Value;
        }
        private void oldRodFiveMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodMinLevels[2] = (byte)oldRodFiveMinUpDown.Value;
        }
        private void oldRodFourMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodMinLevels[3] = (byte)oldRodFourMinUpDown.Value;
        }
        private void oldRodOneMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodMinLevels[4] = (byte)oldRodOneMinUpDown.Value;
        }
        private void oldRodSixtyMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodMaxLevels[0] = (byte)oldRodSixtyMaxUpDown.Value;
        }
        private void oldRodThirtyMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodMaxLevels[1] = (byte)oldRodThirtyMaxUpDown.Value;
        }
        private void oldRodFiveMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodMaxLevels[2] = (byte)oldRodFiveMaxUpDown.Value;
        }
        private void oldRodFourMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodMaxLevels[3] = (byte)oldRodFourMaxUpDown.Value;
        }
        private void oldRodOneMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodMaxLevels[4] = (byte)oldRodOneMaxUpDown.Value;
        }

        private void goodRodSixtyMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodMinLevels[0] = (byte)goodRodSixtyMinUpDown.Value;
        }
        private void goodRodThirtyMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodMinLevels[1] = (byte)goodRodThirtyMinUpDown.Value;
        }
        private void goodRodFiveMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodMinLevels[2] = (byte)goodRodFiveMinUpDown.Value;
        }
        private void goodRodFourMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodMinLevels[3] = (byte)goodRodFourMinUpDown.Value;
        }
        private void goodRodOneMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodMinLevels[4] = (byte)goodRodOneMinUpDown.Value;
        }
        private void goodRodSixtyMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodMaxLevels[0] = (byte)goodRodSixtyMaxUpDown.Value;
        }
        private void goodRodThirtyMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodMaxLevels[1] = (byte)goodRodThirtyMaxUpDown.Value;
        }
        private void goodRodFiveMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodMaxLevels[2] = (byte)goodRodFiveMaxUpDown.Value;
        }
        private void goodRodFourMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodMaxLevels[3] = (byte)goodRodFourMaxUpDown.Value;
        }
        private void goodRodOneMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodMaxLevels[4] = (byte)goodRodOneMaxUpDown.Value;
        }

        private void superRodSixtyMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodMinLevels[0] = (byte)superRodSixtyMinUpDown.Value;
        }
        private void superRodThirtyMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodMinLevels[1]= (byte)superRodThirtyMinUpDown.Value;
        }
        private void superRodFiveMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodMinLevels[2] = (byte)superRodFiveMinUpDown.Value;
        }
        private void superRodFourMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodMinLevels[3] = (byte)superRodFourMinUpDown.Value;
        }
        private void superRodOneMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodMinLevels[4] = (byte)superRodOneMinUpDown.Value;
        }
        private void superRodSixtyMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodMaxLevels[0] = (byte)superRodSixtyMaxUpDown.Value;
        }
        private void superRodThirtyMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodMaxLevels[1] = (byte)superRodThirtyMaxUpDown.Value;
        }
        private void superRodFiveMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodMaxLevels[2] = (byte)superRodFiveMaxUpDown.Value;
        }
        private void superRodFourMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodMaxLevels[3] = (byte)superRodFourMaxUpDown.Value;
        }
        private void superRodOneMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodMaxLevels[4] = (byte)superRodOneMaxUpDown.Value;
        }

        /* Encounter rate controls */
        private void walkingRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.walkingRate = (byte)walkingRateUpDown.Value;
        }
        private void surfRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.surfRate = (byte)surfRateUpDown.Value;
        }
        private void oldRodRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.oldRodRate = (byte)oldRodRateUpDown.Value;
        }
        private void goodRodRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.goodRodRate = (byte)goodRodRateUpDown.Value;
        }
        private void superRodRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (disableHandlers) return;
            currentFile.superRodRate = (byte)superRodRateUpDown.Value;
        }

    }
}
