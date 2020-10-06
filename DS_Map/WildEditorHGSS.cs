using System;
using System.IO;
using System.Windows.Forms;

namespace DS_Map
{
    public partial class WildEditorHGSS : Form
    {
        string encounterFileFolder;
        EncounterFileHGSS currentFile;
        bool disableHandlers = new bool();

        public WildEditorHGSS(string folderPath, string[] names)
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

        public void SetupControls()
        {
            disableHandlers = true;
            
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
            morningTwentyFirstComboBox.SelectedIndex = currentFile.morningPokémon[0];
            morningTwentySecondComboBox.SelectedIndex = currentFile.morningPokémon[1];
            morningTenFirstComboBox.SelectedIndex = currentFile.morningPokémon[2];
            morningTenSecondComboBox.SelectedIndex = currentFile.morningPokémon[3];
            morningTenThirdComboBox.SelectedIndex = currentFile.morningPokémon[4];
            morningTenFourthComboBox.SelectedIndex = currentFile.morningPokémon[5];
            morningFiveFirstComboBox.SelectedIndex = currentFile.morningPokémon[6];
            morningFiveSecondComboBox.SelectedIndex = currentFile.morningPokémon[7];
            morningFourFirstComboBox.SelectedIndex = currentFile.morningPokémon[8];
            morningFourSecondComboBox.SelectedIndex = currentFile.morningPokémon[9];
            morningOneFirstComboBox.SelectedIndex = currentFile.morningPokémon[10];
            morningOneSecondComboBox.SelectedIndex = currentFile.morningPokémon[11];

            dayTwentyFirstComboBox.SelectedIndex = currentFile.dayPokémon[0];
            dayTwentySecondComboBox.SelectedIndex = currentFile.dayPokémon[1];
            dayTenFirstComboBox.SelectedIndex = currentFile.dayPokémon[2];
            dayTenSecondComboBox.SelectedIndex = currentFile.dayPokémon[3];
            dayTenThirdComboBox.SelectedIndex = currentFile.dayPokémon[4];
            dayTenFourthComboBox.SelectedIndex = currentFile.dayPokémon[5];
            dayFiveFirstComboBox.SelectedIndex = currentFile.dayPokémon[6];
            dayFiveSecondComboBox.SelectedIndex = currentFile.dayPokémon[7];
            dayFourFirstComboBox.SelectedIndex = currentFile.dayPokémon[8];
            dayFourSecondComboBox.SelectedIndex = currentFile.dayPokémon[9];
            dayOneFirstComboBox.SelectedIndex = currentFile.dayPokémon[10];
            dayOneSecondComboBox.SelectedIndex = currentFile.dayPokémon[11];

            nightTwentyFirstComboBox.SelectedIndex = currentFile.nightPokémon[0];
            nightTwentySecondComboBox.SelectedIndex = currentFile.nightPokémon[1];
            nightTenFirstComboBox.SelectedIndex = currentFile.nightPokémon[2];
            nightTenSecondComboBox.SelectedIndex = currentFile.nightPokémon[3];
            nightTenThirdComboBox.SelectedIndex = currentFile.nightPokémon[4];
            nightTenFourthComboBox.SelectedIndex = currentFile.nightPokémon[5];
            nightFiveFirstComboBox.SelectedIndex = currentFile.nightPokémon[6];
            nightFiveSecondComboBox.SelectedIndex = currentFile.nightPokémon[7];
            nightFourFirstComboBox.SelectedIndex = currentFile.nightPokémon[8];
            nightFourSecondComboBox.SelectedIndex = currentFile.nightPokémon[9];
            nightOneFirstComboBox.SelectedIndex = currentFile.nightPokémon[10];
            nightOneSecondComboBox.SelectedIndex = currentFile.nightPokémon[11];

            /* Setup radio sound encounters controls */
            hoennFirstComboBox.SelectedIndex = currentFile.hoennMusicPokémon[0];
            hoennSecondComboBox.SelectedIndex = currentFile.hoennMusicPokémon[1];
            sinnohFirstComboBox.SelectedIndex = currentFile.sinnohMusicPokémon[0];
            sinnohSecondComboBox.SelectedIndex = currentFile.sinnohMusicPokémon[1];

            /* Setup rock smash controls */
            rockSmashNinetyComboBox.SelectedIndex = currentFile.rockSmashPokémon[0];
            rockSmashTenComboBox.SelectedIndex = currentFile.rockSmashPokémon[1];
            rockSmashNinetyMinUpDown.Value = currentFile.rockSmashMinLevels[0];
            rockSmashTenMinUpDown.Value = currentFile.rockSmashMinLevels[1];
            rockSmashNinetyMaxUpDown.Value = currentFile.rockSmashMaxLevels[0];
            rockSmashTenMaxUpDown.Value = currentFile.rockSmashMaxLevels[1];

            /* Setup sawrm encounters controls */
            grassSwarmComboBox.SelectedIndex = currentFile.swarmPokémon[0];
            surfSwarmComboBox.SelectedIndex = currentFile.swarmPokémon[1];
            goodRodSwarmComboBox.SelectedIndex = currentFile.swarmPokémon[2];
            superRodSwarmComboBox.SelectedIndex = currentFile.swarmPokémon[3];

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
            currentFile = new EncounterFileHGSS(new FileStream(of.FileName, FileMode.Open));

            /* Update controls */
            SetupControls();
        }
        private void saveEncountersButton_Click(object sender, EventArgs e)
        {
            string filePath = encounterFileFolder + "\\" + selectEncounterComboBox.SelectedIndex.ToString("D4");
            using (BinaryWriter writer = new BinaryWriter(new FileStream(filePath, FileMode.Create))) writer.Write(currentFile.SaveEncounterFile());
        }
        private void selectEncounterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string encounterFileIndex = selectEncounterComboBox.SelectedIndex.ToString("D4");
            currentFile = new EncounterFileHGSS(new FileStream(encounterFileFolder + "\\" + encounterFileIndex, FileMode.Open));
            SetupControls();
        }

        private void walkingRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingRate = (byte)walkingRateUpDown.Value;
        }
        private void rockSmashRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.rockSmashRate = (byte)rockSmashNinetyMaxUpDown.Value;
        }
        private void surfRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.surfRate = (byte)surfRateUpDown.Value;
        }
        private void oldRodRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.oldRodRate = (byte)oldRodRateUpDown.Value;
        }
        private void goodRodRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.goodRodRate = (byte)goodRodRateUpDown.Value;
        }
        private void superRodRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.superRodRate = (byte)superRodRateUpDown.Value;
        }

        private void morningTwentyFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.morningPokémon[0] = (ushort)morningTwentyFirstComboBox.SelectedIndex;
        }
        private void morningTwentySecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.morningPokémon[1] = (ushort)morningTwentySecondComboBox.SelectedIndex;
        }
        private void morningTenFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.morningPokémon[2] = (ushort)morningTenFirstComboBox.SelectedIndex;
        }
        private void morningTenSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.morningPokémon[3] = (ushort)morningTenSecondComboBox.SelectedIndex;
        }
        private void morningTenThirdComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.morningPokémon[4] = (ushort)morningTenThirdComboBox.SelectedIndex;
        }
        private void morningTenFourthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.morningPokémon[5] = (ushort)morningTenFourthComboBox.SelectedIndex;
        }
        private void morningFiveFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.morningPokémon[6] = (ushort)morningFiveFirstComboBox.SelectedIndex;
        }
        private void morningFiveSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.morningPokémon[7] = (ushort)morningFiveSecondComboBox.SelectedIndex;
        }
        private void morningFourFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.morningPokémon[8] = (ushort)morningFourFirstComboBox.SelectedIndex;
        }
        private void morningFourSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.morningPokémon[9] = (ushort)morningFourSecondComboBox.SelectedIndex;
        }
        private void morningOneFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.morningPokémon[10] = (ushort)morningOneFirstComboBox.SelectedIndex;
        }
        private void morningOneSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.morningPokémon[11] = (ushort)morningOneSecondComboBox.SelectedIndex;
        }
        private void dayTwentyFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.dayPokémon[0] = (ushort)dayTwentyFirstComboBox.SelectedIndex;
        }
        private void dayTwentySecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.dayPokémon[1] = (ushort)dayTwentySecondComboBox.SelectedIndex;
        }
        private void dayTenFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.dayPokémon[2] = (ushort)dayTenFirstComboBox.SelectedIndex;
        }
        private void dayTenSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.dayPokémon[3] = (ushort)dayTenSecondComboBox.SelectedIndex;
        }
        private void dayTenThirdComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.dayPokémon[4] = (ushort)dayTenThirdComboBox.SelectedIndex;
        }
        private void dayTenFourthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.dayPokémon[5] = (ushort)dayTenFourthComboBox.SelectedIndex;
        }
        private void dayFiveFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.dayPokémon[6] = (ushort)dayFiveFirstComboBox.SelectedIndex;
        }
        private void dayFiveSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.dayPokémon[7] = (ushort)dayFiveSecondComboBox.SelectedIndex;
        }
        private void dayFourFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.dayPokémon[8] = (ushort)dayFourFirstComboBox.SelectedIndex;
        }
        private void dayFourSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.dayPokémon[9] = (ushort)dayFourSecondComboBox.SelectedIndex;
        }
        private void dayOneFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.dayPokémon[10] = (ushort)dayOneFirstComboBox.SelectedIndex;
        }
        private void dayOneSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.dayPokémon[11] = (ushort)dayOneSecondComboBox.SelectedIndex;
        }
        private void nightTwentyFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.nightPokémon[0] = (ushort)nightTwentyFirstComboBox.SelectedIndex;
        }
        private void nightTwentySecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.nightPokémon[1] = (ushort)nightTwentySecondComboBox.SelectedIndex;
        }
        private void nightTenFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.nightPokémon[2] = (ushort)nightTenFirstComboBox.SelectedIndex;
        }
        private void nightTenSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.nightPokémon[3] = (ushort)nightTenSecondComboBox.SelectedIndex;
        }
        private void nightTenThirdComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.nightPokémon[4] = (ushort)nightTenThirdComboBox.SelectedIndex;
        }
        private void nightTenFourthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.nightPokémon[5] = (ushort)nightTenFourthComboBox.SelectedIndex;
        }
        private void nightFiveFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.nightPokémon[6] = (ushort)nightFiveFirstComboBox.SelectedIndex;
        }
        private void nightFiveSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.nightPokémon[7] = (ushort)nightFiveSecondComboBox.SelectedIndex;
        }
        private void nightFourFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.nightPokémon[8] = (ushort)nightFourFirstComboBox.SelectedIndex;
        }
        private void nightFourSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.nightPokémon[9] = (ushort)nightFourSecondComboBox.SelectedIndex;
        }
        private void nightOneFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.nightPokémon[10] = (ushort)nightOneFirstComboBox.SelectedIndex;
        }
        private void nightOneSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.nightPokémon[11] = (ushort)nightOneSecondComboBox.SelectedIndex;
        }

        private void twentyFirstLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingLevels[0] = (byte)twentyFirstLevelUpDown.Value;
        }
        private void twentySecondLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingLevels[1] = (byte)twentySecondLevelUpDown.Value;
        }
        private void tenFirstLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingLevels[2] = (byte)tenFirstLevelUpDown.Value;
        }
        private void tenSecondLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingLevels[3] = (byte)tenSecondLevelUpDown.Value;
        }
        private void tenThirdLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingLevels[4] = (byte)tenThirdLevelUpDown.Value;
        }
        private void tenFourthLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingLevels[5] = (byte)tenFourthLevelUpDown.Value;
        }
        private void fiveFirstLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingLevels[6] = (byte)fiveFirstLevelUpDown.Value;
        }
        private void fiveSecondLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingLevels[7] = (byte)fiveSecondLevelUpDown.Value;
        }
        private void fourFirstLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingLevels[8] = (byte)fourFirstLevelUpDown.Value;
        }
        private void fourSecondLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingLevels[9] = (byte)fourSecondLevelUpDown.Value;
        }
        private void oneFirstLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingLevels[10] = (byte)oneFirstLevelUpDown.Value;
        }
        private void oneSecondLevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.walkingLevels[11] = (byte)oneSecondLevelUpDown.Value;
        }

        private void hoennFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.hoennMusicPokémon[0] = (ushort)hoennFirstComboBox.SelectedIndex;
        }
        private void hoennSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.hoennMusicPokémon[1] = (ushort)hoennSecondComboBox.SelectedIndex;
        }
        private void sinnohFirstComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.sinnohMusicPokémon[0] = (ushort)sinnohFirstComboBox.SelectedIndex;
        }
        private void sinnohSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.sinnohMusicPokémon[1] = (ushort)sinnohSecondComboBox.SelectedIndex;
        }

        private void rockSmashNinetyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.rockSmashPokémon[0] = (ushort)rockSmashNinetyComboBox.SelectedIndex;
        }
        private void rockSmashTenComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.rockSmashPokémon[1] = (ushort)rockSmashTenComboBox.SelectedIndex;
        }
        private void rockSmashNinetyMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.rockSmashMinLevels[0] = (byte)rockSmashNinetyMinUpDown.Value;
        }
        private void rockSmashTenMinUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.rockSmashMinLevels[1] = (byte)rockSmashTenMinUpDown.Value;
        }
        private void rockSmashNinetyMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.rockSmashMaxLevels[0] = (byte)rockSmashNinetyMaxUpDown.Value;
        }
        private void rockSmashTenMaxUpDown_ValueChanged(object sender, EventArgs e)
        {
            currentFile.rockSmashMaxLevels[1] = (byte)rockSmashNinetyMaxUpDown.Value;
        }
        private void rockSmashNinetyMaxUpDown_ValueChanged_1(object sender, EventArgs e)
        {
            currentFile.rockSmashMaxLevels[0] = (byte)rockSmashNinetyMaxUpDown.Value;
        }
        private void rockSmashTenMaxUpDown_ValueChanged_1(object sender, EventArgs e)
        {
            currentFile.rockSmashMaxLevels[1] = (byte)rockSmashTenMaxUpDown.Value;
        }

        private void grassSwarmComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.swarmPokémon[0] = (ushort)grassSwarmComboBox.SelectedIndex;
        }
        private void surfSwarmComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.swarmPokémon[1] = (ushort)surfSwarmComboBox.SelectedIndex;
        }
        private void goodRodSwarmComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.swarmPokémon[2] = (ushort)goodRodSwarmComboBox.SelectedIndex;
        }
        private void superRodSwarmComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFile.swarmPokémon[3] = (ushort)superRodSwarmComboBox.SelectedIndex;
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
            currentFile.superRodMinLevels[1] = (byte)superRodThirtyMinUpDown.Value;
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
    }
}
