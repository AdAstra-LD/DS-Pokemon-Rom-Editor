using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE {
    public partial class WildEditorDPPt : Form {

        #region Enums

        public enum ShellosForm
        {
            WestSea,
            EastSea
        }

        public enum UnownTable
        {
            MostForms,
            OnlyF,
            OnlyR,
            OnlyI,
            OnlyN,
            OnlyE,
            OnlyD,
            ExclamQuestion
        }

        #endregion

        public string encounterFileFolder { get; private set; }
        public bool walkingDirty { get; private set; } = false;
        public bool waterDirty { get; private set; } = false;

        private int loadedEncounterFileIndex = 0;

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

            AddPokemonNamesBinding(names);
            RegisterMarkDirtyHandlers();
            SetupControls();

            Helpers.EnableHandlers();            
        }

        public string GetDSPREVersion()
        {
            return "" + Assembly.GetExecutingAssembly().GetName().Version.Major + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor +
                "." + Assembly.GetExecutingAssembly().GetName().Version.Build;
        }

        // Listing them all like this is more work, but it is easier to read and maintain.
        private void AddPokemonNamesBinding(string[] names)
        {
            /* Walking encounters */
            walkingTwentyFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            walkingTwentySecondComboBox.DataSource = new BindingSource(names, string.Empty);
            walkingTenFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            walkingTenSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            walkingTenThirdComboBox.DataSource = new BindingSource(names, string.Empty);
            walkingTenFourthComboBox.DataSource = new BindingSource(names, string.Empty);
            walkingFiveFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            walkingFiveSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            walkingFourFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            walkingFourSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            walkingOneFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            walkingOneSecondComboBox.DataSource = new BindingSource(names, string.Empty);

            /* Time dependent encounters */
            dayFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            daySecondComboBox.DataSource = new BindingSource(names, string.Empty);
            nightFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            nightSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            swarmFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            swarmSecondComboBox.DataSource = new BindingSource(names, string.Empty);

            /* Dual Slot encounters */
            rubyFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            rubySecondComboBox.DataSource = new BindingSource(names, string.Empty);
            sapphireFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            sapphireSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            emeraldFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            emeraldSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            fireRedFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            fireRedSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            leafGreenFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            leafGreenSecondComboBox.DataSource = new BindingSource(names, string.Empty);

            /* PokéRadar encounters */
            radarFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            radarSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            radarThirdComboBox.DataSource = new BindingSource(names, string.Empty);
            radarFourthComboBox.DataSource = new BindingSource(names, string.Empty);

            /* Water encounters */
            surfSixtyComboBox.DataSource = new BindingSource(names, string.Empty);
            surfThirtyComboBox.DataSource = new BindingSource(names, string.Empty);
            surfFiveComboBox.DataSource = new BindingSource(names, string.Empty);
            surfFourComboBox.DataSource = new BindingSource(names, string.Empty);
            surfOneComboBox.DataSource = new BindingSource(names, string.Empty);

            /* Old rod encounters */
            oldRodSixtyComboBox.DataSource = new BindingSource(names, string.Empty);
            oldRodThirtyComboBox.DataSource = new BindingSource(names, string.Empty);
            oldRodFiveComboBox.DataSource = new BindingSource(names, string.Empty);
            oldRodFourComboBox.DataSource = new BindingSource(names, string.Empty);
            oldRodOneComboBox.DataSource = new BindingSource(names, string.Empty);

            /* Good rod encounters */
            goodRodFirstFortyComboBox.DataSource = new BindingSource(names, string.Empty);
            goodRodSecondFortyComboBox.DataSource = new BindingSource(names, string.Empty);
            goodRodFifteenComboBox.DataSource = new BindingSource(names, string.Empty);
            goodRodFourComboBox.DataSource = new BindingSource(names, string.Empty);
            goodRodOneComboBox.DataSource = new BindingSource(names, string.Empty);

            /* Super rod encounters */
            superRodFirstFortyComboBox.DataSource = new BindingSource(names, string.Empty);
            superRodSecondFortyComboBox.DataSource = new BindingSource(names, string.Empty);
            superRodFifteenComboBox.DataSource = new BindingSource(names, string.Empty);
            superRodFourComboBox.DataSource = new BindingSource(names, string.Empty);
            superRodOneComboBox.DataSource = new BindingSource(names, string.Empty);

            /* Form Data */
            shellosComboBox.DataSource = Enum.GetValues(typeof(ShellosForm));
            gastrodonComboBox.DataSource = Enum.GetValues(typeof(ShellosForm));
            unownComboBox.DataSource = Enum.GetValues(typeof(UnownTable));
        }

        private void SetupControls() {
            Helpers.DisableHandlers();

            loadedEncounterFileIndex = selectEncounterComboBox.SelectedIndex;

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
            dayFirstComboBox.SelectedIndex = (int)currentFile.dayPokemon[0];
            daySecondComboBox.SelectedIndex = (int)currentFile.dayPokemon[1];
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

            /* Form data controls setup */
            shellosComboBox.SelectedIndex = (currentFile.regionalForms[0] == 0) ? (int)ShellosForm.WestSea : (int)ShellosForm.EastSea;
            gastrodonComboBox.SelectedIndex = (currentFile.regionalForms[1] == 0) ? (int)ShellosForm.WestSea : (int)ShellosForm.EastSea;
            unownComboBox.SelectedIndex = (currentFile.unknownTable == 0) ? 0 : (int)currentFile.unknownTable - 1;

            SetDirtyWalking(false);
            SetDirtyWater(false);

            Helpers.EnableHandlers();
        }

        private void RegisterMarkDirtyHandlers()
        {
            //* Walking encounters *//
            /*ComboBoxes*/
            walkingTwentyFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            walkingTwentySecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            walkingTenFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            walkingTenSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            walkingTenThirdComboBox.SelectedIndexChanged += MarkDirtyWalking;
            walkingTenFourthComboBox.SelectedIndexChanged += MarkDirtyWalking;
            walkingFiveFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            walkingFiveSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            walkingFourFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            walkingFourSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            walkingOneFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            walkingOneSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;

            dayFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            daySecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;

            swarmFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            swarmSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;

            rubyFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            rubySecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            sapphireFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            sapphireSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            emeraldFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            emeraldSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            fireRedFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            fireRedSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            leafGreenFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            leafGreenSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;

            radarFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            radarSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            radarThirdComboBox.SelectedIndexChanged += MarkDirtyWalking;
            radarFourthComboBox.SelectedIndexChanged += MarkDirtyWalking;

            /*NumericUpDowns*/
            walkingTwentyFirstUpDown.ValueChanged += MarkDirtyWalking;
            walkingTwentySecondUpDown.ValueChanged += MarkDirtyWalking;
            walkingTenFirstUpDown.ValueChanged += MarkDirtyWalking;
            walkingTenSecondUpDown.ValueChanged += MarkDirtyWalking;
            walkingTenThirdUpDown.ValueChanged += MarkDirtyWalking;
            walkingTenFourthUpDown.ValueChanged += MarkDirtyWalking;
            walkingFiveFirstUpDown.ValueChanged += MarkDirtyWalking;
            walkingFiveSecondUpDown.ValueChanged += MarkDirtyWalking;
            walkingFourFirstUpDown.ValueChanged += MarkDirtyWalking;
            walkingFourSecondUpDown.ValueChanged += MarkDirtyWalking;
            walkingOneFirstUpDown.ValueChanged += MarkDirtyWalking;
            walkingOneSecondUpDown.ValueChanged += MarkDirtyWalking;

            walkingRateUpDown.ValueChanged += MarkDirtyWalking;

            //* Water encounters *//
            /*ComboBoxes*/
            surfSixtyComboBox.SelectedIndexChanged += MarkDirtyWater;
            surfThirtyComboBox.SelectedIndexChanged += MarkDirtyWater;
            surfFiveComboBox.SelectedIndexChanged += MarkDirtyWater;
            surfFourComboBox.SelectedIndexChanged += MarkDirtyWater;
            surfOneComboBox.SelectedIndexChanged += MarkDirtyWater;

            oldRodSixtyComboBox.SelectedIndexChanged += MarkDirtyWater;
            oldRodThirtyComboBox.SelectedIndexChanged += MarkDirtyWater;
            oldRodFiveComboBox.SelectedIndexChanged += MarkDirtyWater;
            oldRodFourComboBox.SelectedIndexChanged += MarkDirtyWater;
            oldRodOneComboBox.SelectedIndexChanged += MarkDirtyWater;

            goodRodFirstFortyComboBox.SelectedIndexChanged += MarkDirtyWater;
            goodRodSecondFortyComboBox.SelectedIndexChanged += MarkDirtyWater;
            goodRodFifteenComboBox.SelectedIndexChanged += MarkDirtyWater;
            goodRodFourComboBox.SelectedIndexChanged += MarkDirtyWater;
            goodRodOneComboBox.SelectedIndexChanged += MarkDirtyWater;

            superRodFirstFortyComboBox.SelectedIndexChanged += MarkDirtyWater;
            superRodSecondFortyComboBox.SelectedIndexChanged += MarkDirtyWater;
            superRodFifteenComboBox.SelectedIndexChanged += MarkDirtyWater;
            superRodFourComboBox.SelectedIndexChanged += MarkDirtyWater;
            superRodOneComboBox.SelectedIndexChanged += MarkDirtyWater;

            /*NumericUpDowns*/
            surfSixtyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            surfSixtyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            surfThirtyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            surfThirtyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            surfFiveMinLevelUpDown.ValueChanged += MarkDirtyWater;
            surfFiveMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            surfFourMinLevelUpDown.ValueChanged += MarkDirtyWater;
            surfFourMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            surfOneMinLevelUpDown.ValueChanged += MarkDirtyWater;
            surfOneMaxLevelUpDown.ValueChanged += MarkDirtyWater;

            oldRodSixtyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodSixtyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodThirtyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodThirtyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodFiveMinLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodFiveMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodFourMinLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodFourMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodOneMinLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodOneMaxLevelUpDown.ValueChanged += MarkDirtyWater;

            goodRodFirstFortyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodFirstFortyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodSecondFortyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodSecondFortyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodFifteenMinLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodFifteenMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodFourMinLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodFourMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodOneMinLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodOneMaxLevelUpDown.ValueChanged += MarkDirtyWater;

            superRodFirstFortyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodFirstFortyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodSecondFortyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodSecondFortyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodFifteenMinLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodFifteenMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodFourMinLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodFourMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodOneMinLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodOneMaxLevelUpDown.ValueChanged += MarkDirtyWater;

            surfRateUpDown.ValueChanged += MarkDirtyWater;
            oldRodRateUpDown.ValueChanged += MarkDirtyWater;
            goodRodRateUpDown.ValueChanged += MarkDirtyWater;
            superRodRateUpDown.ValueChanged += MarkDirtyWater;

            /* Form Data */
            shellosComboBox.SelectedIndexChanged += MarkDirtyWalking; // technically also applies to water
            gastrodonComboBox.SelectedIndexChanged += MarkDirtyWalking;
            unownComboBox.SelectedIndexChanged += MarkDirtyWalking;

        }

        private void DrawConnectingLines()
        {
            var panel = walkingTableLayoutPanel;
            using (Graphics g = panel.CreateGraphics())
            {
                Pen dashedPen = new Pen(Color.Gray, 1.5f);
                dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                // Loop through all rows
                for (int row = 0; row < panel.RowCount; row++)
                {
                    // The updown is always in column 7
                    Control updown = panel.GetControlFromPosition(7, row);

                    // The combo box is the rightmost control besides the updown
                    Control combo = null;
                    for (int col = 6; col >= 0; col--)
                    {
                        Control ctrl = panel.GetControlFromPosition(col, row);
                        if (ctrl is InputComboBox)
                        {
                            combo = ctrl;
                            break;
                        }
                    }

                    if (combo is InputComboBox && updown is NumericUpDown)
                    {
                        // Get the location of the controls relative to the panel
                        Point comboPoint = panel.PointToClient(combo.Parent.PointToScreen(combo.Location));
                        Point updownPoint = panel.PointToClient(updown.Parent.PointToScreen(updown.Location));

                        // Calculate the Y center of each control
                        int comboY = comboPoint.Y + combo.Height / 2;
                        int updownY = updownPoint.Y + updown.Height / 2;

                        // Draw from right edge of ComboBox to left edge of NumericUpDown
                        int comboX = comboPoint.X + combo.Width + 5;
                        int updownX = updownPoint.X - 5;

                        if (updownX > comboX)
                        {
                            g.DrawLine(dashedPen, comboX, comboY, updownX, updownY);
                        }
                        
                    }
                }
            }
        }

        private void CalculateLabelWidths()
        {
            Label[] imageLabels = { pokeRadarLabel1, pokeRadarLabel2, rubyLabel, sapphireLabel, emeraldLabel, fireRedLabel, leafGreenLabel };

            foreach (Label label in imageLabels)
            {
                if (label.Text.Length > 0)
                {
                    // Measure the width of the text
                    SizeF size = TextRenderer.MeasureText(label.Text, label.Font);
                    int imageWidth = label.Image != null ? label.Image.Width : 0;
                    // Set the width of the label to fit the text
                    label.Width = (int)size.Width + imageWidth + 5; // Add image width and some padding
                }
            }
        }

        private void SaveWalking()
        {
            if (currentFile == null) return;

            /* Encounters */
            currentFile.walkingPokemon[0] = (uint)walkingTwentyFirstComboBox.SelectedIndex;
            currentFile.walkingPokemon[1] = (uint)walkingTwentySecondComboBox.SelectedIndex;
            currentFile.walkingPokemon[2] = (uint)walkingTenFirstComboBox.SelectedIndex;
            currentFile.walkingPokemon[3] = (uint)walkingTenSecondComboBox.SelectedIndex;
            currentFile.walkingPokemon[4] = (uint)walkingTenThirdComboBox.SelectedIndex;
            currentFile.walkingPokemon[5] = (uint)walkingTenFourthComboBox.SelectedIndex;
            currentFile.walkingPokemon[6] = (uint)walkingFiveFirstComboBox.SelectedIndex;
            currentFile.walkingPokemon[7] = (uint)walkingFiveSecondComboBox.SelectedIndex;
            currentFile.walkingPokemon[8] = (uint)walkingFourFirstComboBox.SelectedIndex;
            currentFile.walkingPokemon[9] = (uint)walkingFourSecondComboBox.SelectedIndex;
            currentFile.walkingPokemon[10] = (uint)walkingOneFirstComboBox.SelectedIndex;
            currentFile.walkingPokemon[11] = (uint)walkingOneSecondComboBox.SelectedIndex;

            currentFile.dayPokemon[0] = (uint)dayFirstComboBox.SelectedIndex;
            currentFile.dayPokemon[1] = (uint)daySecondComboBox.SelectedIndex;
            currentFile.nightPokemon[0] = (uint)nightFirstComboBox.SelectedIndex;
            currentFile.nightPokemon[1] = (uint)nightSecondComboBox.SelectedIndex;

            currentFile.swarmPokemon[0] = (ushort)swarmFirstComboBox.SelectedIndex;
            currentFile.swarmPokemon[1] = (ushort)swarmSecondComboBox.SelectedIndex;

            currentFile.rubyPokemon[0] = (uint)rubyFirstComboBox.SelectedIndex;
            currentFile.rubyPokemon[1] = (uint)rubySecondComboBox.SelectedIndex;
            currentFile.sapphirePokemon[0] = (uint)sapphireFirstComboBox.SelectedIndex;
            currentFile.sapphirePokemon[1] = (uint)sapphireSecondComboBox.SelectedIndex;
            currentFile.emeraldPokemon[0] = (uint)emeraldFirstComboBox.SelectedIndex;
            currentFile.emeraldPokemon[1] = (uint)emeraldSecondComboBox.SelectedIndex;
            currentFile.fireRedPokemon[0] = (uint)fireRedFirstComboBox.SelectedIndex;
            currentFile.fireRedPokemon[1] = (uint)fireRedSecondComboBox.SelectedIndex;
            currentFile.leafGreenPokemon[0] = (uint)leafGreenFirstComboBox.SelectedIndex;
            currentFile.leafGreenPokemon[1] = (uint)leafGreenSecondComboBox.SelectedIndex;

            currentFile.radarPokemon[0] = (uint)radarFirstComboBox.SelectedIndex;
            currentFile.radarPokemon[1] = (uint)radarSecondComboBox.SelectedIndex;
            currentFile.radarPokemon[2] = (uint)radarThirdComboBox.SelectedIndex;
            currentFile.radarPokemon[3] = (uint)radarFourthComboBox.SelectedIndex;

            /* Form Data */
            currentFile.regionalForms[0] = (uint)(shellosComboBox.SelectedIndex);
            currentFile.regionalForms[1] = (uint)(gastrodonComboBox.SelectedIndex);
            currentFile.unknownTable = (uint)(unownComboBox.SelectedIndex + 1);

            /* Levels */
            currentFile.walkingLevels[0] = (byte)walkingTwentyFirstUpDown.Value;
            currentFile.walkingLevels[1] = (byte)walkingTwentySecondUpDown.Value;
            currentFile.walkingLevels[2] = (byte)walkingTenFirstUpDown.Value;
            currentFile.walkingLevels[3] = (byte)walkingTenSecondUpDown.Value;
            currentFile.walkingLevels[4] = (byte)walkingTenThirdUpDown.Value;
            currentFile.walkingLevels[5] = (byte)walkingTenFourthUpDown.Value;
            currentFile.walkingLevels[6] = (byte)walkingFiveFirstUpDown.Value;
            currentFile.walkingLevels[7] = (byte)walkingFiveSecondUpDown.Value;
            currentFile.walkingLevels[8] = (byte)walkingFourFirstUpDown.Value;
            currentFile.walkingLevels[9] = (byte)walkingFourSecondUpDown.Value;
            currentFile.walkingLevels[10] = (byte)walkingOneFirstUpDown.Value;
            currentFile.walkingLevels[11] = (byte)walkingOneSecondUpDown.Value;

            currentFile.walkingRate = (byte)walkingRateUpDown.Value;

            SetDirtyWalking(false);
        }

        private void SaveWater()
        {
            if (currentFile == null) return;

            //* Encounters *//
            /* Surfing */
            currentFile.surfPokemon[0] = (ushort)surfSixtyComboBox.SelectedIndex;
            currentFile.surfPokemon[1] = (ushort)surfThirtyComboBox.SelectedIndex;
            currentFile.surfPokemon[2] = (ushort)surfFiveComboBox.SelectedIndex;
            currentFile.surfPokemon[3] = (ushort)surfFourComboBox.SelectedIndex;
            currentFile.surfPokemon[4] = (ushort)surfOneComboBox.SelectedIndex;

            /* Old rod */
            currentFile.oldRodPokemon[0] = (ushort)oldRodSixtyComboBox.SelectedIndex;
            currentFile.oldRodPokemon[1] = (ushort)oldRodThirtyComboBox.SelectedIndex;
            currentFile.oldRodPokemon[2] = (ushort)oldRodFiveComboBox.SelectedIndex;
            currentFile.oldRodPokemon[3] = (ushort)oldRodFourComboBox.SelectedIndex;
            currentFile.oldRodPokemon[4] = (ushort)oldRodOneComboBox.SelectedIndex;

            /* Good rod */
            currentFile.goodRodPokemon[0] = (ushort)goodRodFirstFortyComboBox.SelectedIndex;
            currentFile.goodRodPokemon[1] = (ushort)goodRodSecondFortyComboBox.SelectedIndex;
            currentFile.goodRodPokemon[2] = (ushort)goodRodFifteenComboBox.SelectedIndex;
            currentFile.goodRodPokemon[3] = (ushort)goodRodFourComboBox.SelectedIndex;
            currentFile.goodRodPokemon[4] = (ushort)goodRodOneComboBox.SelectedIndex;

            /* Super rod */
            currentFile.superRodPokemon[0] = (ushort)superRodFirstFortyComboBox.SelectedIndex;
            currentFile.superRodPokemon[1] = (ushort)superRodSecondFortyComboBox.SelectedIndex;
            currentFile.superRodPokemon[2] = (ushort)superRodFifteenComboBox.SelectedIndex;
            currentFile.superRodPokemon[3] = (ushort)superRodFourComboBox.SelectedIndex;
            currentFile.superRodPokemon[4] = (ushort)superRodOneComboBox.SelectedIndex;

            //* Levels *//
            /* Surfing */
            currentFile.surfMinLevels[0] = (byte)surfSixtyMinLevelUpDown.Value;
            currentFile.surfMinLevels[1] = (byte)surfThirtyMinLevelUpDown.Value;
            currentFile.surfMinLevels[2] = (byte)surfFiveMinLevelUpDown.Value;
            currentFile.surfMinLevels[3] = (byte)surfFourMinLevelUpDown.Value;
            currentFile.surfMinLevels[4] = (byte)surfOneMinLevelUpDown.Value;
            currentFile.surfMaxLevels[0] = (byte)surfSixtyMaxLevelUpDown.Value;
            currentFile.surfMaxLevels[1] = (byte)surfThirtyMaxLevelUpDown.Value;
            currentFile.surfMaxLevels[2] = (byte)surfFiveMaxLevelUpDown.Value;
            currentFile.surfMaxLevels[3] = (byte)surfFourMaxLevelUpDown.Value;
            currentFile.surfMaxLevels[4] = (byte)surfOneMaxLevelUpDown.Value;

            /* Old rod */
            currentFile.oldRodMinLevels[0] = (byte)oldRodSixtyMinLevelUpDown.Value;
            currentFile.oldRodMinLevels[1] = (byte)oldRodThirtyMinLevelUpDown.Value;
            currentFile.oldRodMinLevels[2] = (byte)oldRodFiveMinLevelUpDown.Value;
            currentFile.oldRodMinLevels[3] = (byte)oldRodFourMinLevelUpDown.Value;
            currentFile.oldRodMinLevels[4] = (byte)oldRodOneMinLevelUpDown.Value;
            currentFile.oldRodMaxLevels[0] = (byte)oldRodSixtyMaxLevelUpDown.Value;
            currentFile.oldRodMaxLevels[1] = (byte)oldRodThirtyMaxLevelUpDown.Value;
            currentFile.oldRodMaxLevels[2] = (byte)oldRodFiveMaxLevelUpDown.Value;
            currentFile.oldRodMaxLevels[3] = (byte)oldRodFourMaxLevelUpDown.Value;
            currentFile.oldRodMaxLevels[4] = (byte)oldRodOneMaxLevelUpDown.Value;

            /* Good rod */
            currentFile.goodRodMinLevels[0] = (byte)goodRodFirstFortyMinLevelUpDown.Value;
            currentFile.goodRodMinLevels[1] = (byte)goodRodSecondFortyMinLevelUpDown.Value;
            currentFile.goodRodMinLevels[2] = (byte)goodRodFifteenMinLevelUpDown.Value;
            currentFile.goodRodMinLevels[3] = (byte)goodRodFourMinLevelUpDown.Value;
            currentFile.goodRodMinLevels[4] = (byte)goodRodOneMinLevelUpDown.Value;
            currentFile.goodRodMaxLevels[0] = (byte)goodRodFirstFortyMaxLevelUpDown.Value;
            currentFile.goodRodMaxLevels[1] = (byte)goodRodSecondFortyMaxLevelUpDown.Value;
            currentFile.goodRodMaxLevels[2] = (byte)goodRodFifteenMaxLevelUpDown.Value;
            currentFile.goodRodMaxLevels[3] = (byte)goodRodFourMaxLevelUpDown.Value;
            currentFile.goodRodMaxLevels[4] = (byte)goodRodOneMaxLevelUpDown.Value;

            /* Super rod */
            currentFile.superRodMinLevels[0] = (byte)superRodFirstFortyMinLevelUpDown.Value;
            currentFile.superRodMinLevels[1] = (byte)superRodSecondFortyMinLevelUpDown.Value;
            currentFile.superRodMinLevels[2] = (byte)superRodFifteenMinLevelUpDown.Value;
            currentFile.superRodMinLevels[3] = (byte)superRodFourMinLevelUpDown.Value;
            currentFile.superRodMinLevels[4] = (byte)superRodOneMinLevelUpDown.Value;
            currentFile.superRodMaxLevels[0] = (byte)superRodFirstFortyMaxLevelUpDown.Value;
            currentFile.superRodMaxLevels[1] = (byte)superRodSecondFortyMaxLevelUpDown.Value;
            currentFile.superRodMaxLevels[2] = (byte)superRodFifteenMaxLevelUpDown.Value;
            currentFile.superRodMaxLevels[3] = (byte)superRodFourMaxLevelUpDown.Value;
            currentFile.superRodMaxLevels[4] = (byte)superRodOneMaxLevelUpDown.Value;

            /* Rates */
            currentFile.surfRate = (byte)surfRateUpDown.Value;
            currentFile.oldRodRate = (byte)oldRodRateUpDown.Value;
            currentFile.goodRodRate = (byte)goodRodRateUpDown.Value;
            currentFile.superRodRate = (byte)superRodRateUpDown.Value;

            SetDirtyWater(false);
        }

        private bool ContinueUnsavedChanges()
        {
            if (walkingDirty || waterDirty)
            {
                DialogResult d = MessageBox.Show("There are unsaved changes. Do you want to save them before proceeding?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (d == DialogResult.Yes)
                {
                    SaveWalking();
                    SaveWater();
                    currentFile.SaveToFileDefaultDir(loadedEncounterFileIndex, showSuccessMessage: false);
                    return true;
                }
                else if (d == DialogResult.No)
                {
                    return true;
                }
                else
                {
                    return false; // Cancel
                }
            }
            return true; // No unsaved changes
        }

        private void SetDirtyWalking(bool dirty)
        {
            if (dirty && !walkingDirty)
            {
                walkingDirty = true;
                grassGroundTabPage.Text += "*";

                if (!waterDirty)
                {
                    this.Text += "*";
                }

            }
            else if (!dirty && walkingDirty)
            {
                walkingDirty = false;
                grassGroundTabPage.Text = grassGroundTabPage.Text.TrimEnd('*');

                if (!waterDirty)
                {
                    this.Text = this.Text.TrimEnd('*');
                }
            }
        }
        private void SetDirtyWater(bool dirty)
        {
            if (dirty && !waterDirty)
            {
                waterDirty = true;
                waterTabPage.Text += "*";

                if (!walkingDirty)
                {
                    this.Text += "*";
                }

            }
            else if (!dirty && waterDirty)
            {
                waterDirty = false;
                waterTabPage.Text = waterTabPage.Text.TrimEnd('*');

                if (!walkingDirty)
                {
                    this.Text = this.Text.TrimEnd('*');
                }
            }
        }

        #region Event Handlers

        private void MarkDirtyWalking(object sender, EventArgs e) 
        {
            SetDirtyWalking(true);            
        }
        private void MarkDirtyWater(object sender, EventArgs e) 
        {
            SetDirtyWater(true);
        }

        private void exportEncounterFileButton_Click(object sender, EventArgs e) {
            SaveWalking();
            SaveWater();
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

            Helpers.DisableHandlers();

            if (!ContinueUnsavedChanges()) {
                selectEncounterComboBox.SelectedIndex = loadedEncounterFileIndex;
                Helpers.EnableHandlers();
                return; // Cancel the change
            }

            currentFile = new EncounterFileDPPt(selectEncounterComboBox.SelectedIndex);
            SetupControls();
            Helpers.EnableHandlers();
        }
        private void saveEncountersButton_Click(object sender, EventArgs e) {
            SaveWalking();
            SaveWater();
            currentFile.SaveToFileDefaultDir(selectEncounterComboBox.SelectedIndex, false);
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

        private void walkingTableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            DrawConnectingLines();
            CalculateLabelWidths();
        }

        private void WildEditorDPPt_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ContinueUnsavedChanges())
            {
                e.Cancel = true; // Cancel the form closing
                return;
            }
        }


        #endregion

    }
}
