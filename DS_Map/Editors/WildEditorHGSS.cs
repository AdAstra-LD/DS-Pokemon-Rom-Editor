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
        public bool walkingDirty { get; private set; } = false;
        public bool waterDirty { get; private set; } = false;

        private int loadedEncounterFileIndex = 0;

        EncounterFileHGSS currentFile;

        public WildEditorHGSS(string dirPath, string[] names, int encToOpen, int totalNumHeaderFiles) 
        {
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

            AddPokemonNamesBinding(names);
            RegisterMarkDirtyHandlers();
            AddTooltips();
            SetupControls();
            

            Helpers.EnableHandlers();            
        }

        public string GetDSPREVersion() 
        {
            return "" + Assembly.GetExecutingAssembly().GetName().Version.Major + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor +
                "." + Assembly.GetExecutingAssembly().GetName().Version.Build;
        }

        public void SetupControls() 
        {
            Helpers.DisableHandlers();

            loadedEncounterFileIndex = selectEncounterComboBox.SelectedIndex;

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
            nightFishingComboBox.SelectedIndex = currentFile.swarmPokemon[2];
            rodSwarmComboBox.SelectedIndex = currentFile.swarmPokemon[3];

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
            oldRodFortyComboBox.SelectedIndex = currentFile.oldRodPokemon[0];
            oldRodFortyMinLevelUpDown.Value = currentFile.oldRodMinLevels[0];
            oldRodFortyMaxLevelUpDown.Value = currentFile.oldRodMaxLevels[0];

            oldRodThirtyComboBox.SelectedIndex = currentFile.oldRodPokemon[1];
            oldRodThirtyMinLevelUpDown.Value = currentFile.oldRodMinLevels[1];
            oldRodThirtyMaxLevelUpDown.Value = currentFile.oldRodMaxLevels[1];

            oldRodFifteenComboBox.SelectedIndex = currentFile.oldRodPokemon[2];
            oldRodFifteenMinLevelUpDown.Value = currentFile.oldRodMinLevels[2];
            oldRodFifteenMaxLevelUpDown.Value = currentFile.oldRodMaxLevels[2];

            oldRodTenComboBox.SelectedIndex = currentFile.oldRodPokemon[3];
            oldRodTenMinLevelUpDown.Value = currentFile.oldRodMinLevels[3];
            oldRodTenMaxLevelUpDown.Value = currentFile.oldRodMaxLevels[3];

            oldRodFiveComboBox.SelectedIndex = currentFile.oldRodPokemon[4];
            oldRodFiveMinLevelUpDown.Value = currentFile.oldRodMinLevels[4];
            oldRodFiveMaxLevelUpDown.Value = currentFile.oldRodMaxLevels[4];

            /* Good rod encounters controls setup */
            goodRodFortyComboBox.SelectedIndex = currentFile.goodRodPokemon[0];
            goodRodFortyMinLevelUpDown.Value = currentFile.goodRodMinLevels[0];
            goodRodFortyMaxLevelUpDown.Value = currentFile.goodRodMaxLevels[0];

            goodRodThirtyComboBox.SelectedIndex = currentFile.goodRodPokemon[1];
            goodRodThirtyMinLevelUpDown.Value = currentFile.goodRodMinLevels[1];
            goodRodThirtyMaxLevelUpDown.Value = currentFile.goodRodMaxLevels[1];

            goodRodFifteenComboBox.SelectedIndex = currentFile.goodRodPokemon[2];
            goodRodFifteenMinLevelUpDown.Value = currentFile.goodRodMinLevels[2];
            goodRodFifteenMaxLevelUpDown.Value = currentFile.goodRodMaxLevels[2];

            goodRodTenComboBox.SelectedIndex = currentFile.goodRodPokemon[3];
            goodRodTenMinLevelUpDown.Value = currentFile.goodRodMinLevels[3];
            goodRodTenMaxLevelUpDown.Value = currentFile.goodRodMaxLevels[3];

            goodRodFiveComboBox.SelectedIndex = currentFile.goodRodPokemon[4];
            goodRodFiveMinLevelUpDown.Value = currentFile.goodRodMinLevels[4];
            goodRodFiveMaxLevelUpDown.Value = currentFile.goodRodMaxLevels[4];

            /* Super rod encounters controls setup */
            superRodFortyComboBox.SelectedIndex = currentFile.superRodPokemon[0];
            superRodFortyMinLevelUpDown.Value = currentFile.superRodMinLevels[0];
            superRodFortyMaxLevelUpDown.Value = currentFile.superRodMaxLevels[0];

            superRodThirtyComboBox.SelectedIndex = currentFile.superRodPokemon[1];
            superRodThirtyMinLevelUpDown.Value = currentFile.superRodMinLevels[1];
            superRodThirtyMaxLevelUpDown.Value = currentFile.superRodMaxLevels[1];

            superRodFifteenComboBox.SelectedIndex = currentFile.superRodPokemon[2];
            superRodFifteenMinLevelUpDown.Value = currentFile.superRodMinLevels[2];
            superRodFifteenMaxLevelUpDown.Value = currentFile.superRodMaxLevels[2];

            superRodTenComboBox.SelectedIndex = currentFile.superRodPokemon[3];
            superRodTenMinLevelUpDown.Value = currentFile.superRodMinLevels[3];
            superRodTenMaxLevelUpDown.Value = currentFile.superRodMaxLevels[3];

            superRodFiveComboBox.SelectedIndex = currentFile.superRodPokemon[4];
            superRodFiveMinLevelUpDown.Value = currentFile.superRodMinLevels[4];
            superRodFiveMaxLevelUpDown.Value = currentFile.superRodMaxLevels[4];

            SetDirtyWalking(false);
            SetDirtyWater(false);

            Helpers.EnableHandlers();
        }

        private void AddPokemonNamesBinding(string[] names)
        {
            /*Grass encounters*/
            morningTwentyFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            morningTwentySecondComboBox.DataSource = new BindingSource(names, string.Empty);
            morningTenFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            morningTenSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            morningTenThirdComboBox.DataSource = new BindingSource(names, string.Empty);
            morningTenFourthComboBox.DataSource = new BindingSource(names, string.Empty);
            morningFiveFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            morningFiveSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            morningFourFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            morningFourSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            morningOneFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            morningOneSecondComboBox.DataSource = new BindingSource(names, string.Empty);

            dayTwentyFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            dayTwentySecondComboBox.DataSource = new BindingSource(names, string.Empty);
            dayTenFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            dayTenSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            dayTenThirdComboBox.DataSource = new BindingSource(names, string.Empty);
            dayTenFourthComboBox.DataSource = new BindingSource(names, string.Empty);
            dayFiveFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            dayFiveSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            dayFourFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            dayFourSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            dayOneFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            dayOneSecondComboBox.DataSource = new BindingSource(names, string.Empty);

            nightTwentyFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            nightTwentySecondComboBox.DataSource = new BindingSource(names, string.Empty);
            nightTenFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            nightTenSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            nightTenThirdComboBox.DataSource = new BindingSource(names, string.Empty);
            nightTenFourthComboBox.DataSource = new BindingSource(names, string.Empty);
            nightFiveFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            nightFiveSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            nightFourFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            nightFourSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            nightOneFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            nightOneSecondComboBox.DataSource = new BindingSource(names, string.Empty);

            /*Radio sound encounters*/
            hoennFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            hoennSecondComboBox.DataSource = new BindingSource(names, string.Empty);
            sinnohFirstComboBox.DataSource = new BindingSource(names, string.Empty);
            sinnohSecondComboBox.DataSource = new BindingSource(names, string.Empty);

            /*Rock smash encounters*/
            rockSmashNinetyComboBox.DataSource = new BindingSource(names, string.Empty);
            rockSmashTenComboBox.DataSource = new BindingSource(names, string.Empty);

            /*Water encounters*/
            surfSixtyComboBox.DataSource = new BindingSource(names, string.Empty);
            surfThirtyComboBox.DataSource = new BindingSource(names, string.Empty);
            surfFiveComboBox.DataSource = new BindingSource(names, string.Empty);
            surfFourComboBox.DataSource = new BindingSource(names, string.Empty);
            surfOneComboBox.DataSource = new BindingSource(names, string.Empty);

            oldRodFortyComboBox.DataSource = new BindingSource(names, string.Empty);
            oldRodThirtyComboBox.DataSource = new BindingSource(names, string.Empty);
            oldRodFifteenComboBox.DataSource = new BindingSource(names, string.Empty);
            oldRodTenComboBox.DataSource = new BindingSource(names, string.Empty);
            oldRodFiveComboBox.DataSource = new BindingSource(names, string.Empty);

            goodRodFortyComboBox.DataSource = new BindingSource(names, string.Empty);
            goodRodThirtyComboBox.DataSource = new BindingSource(names, string.Empty);
            goodRodFifteenComboBox.DataSource = new BindingSource(names, string.Empty);
            goodRodTenComboBox.DataSource = new BindingSource(names, string.Empty);
            goodRodFiveComboBox.DataSource = new BindingSource(names, string.Empty);

            superRodFortyComboBox.DataSource = new BindingSource(names, string.Empty);
            superRodThirtyComboBox.DataSource = new BindingSource(names, string.Empty);
            superRodFifteenComboBox.DataSource = new BindingSource(names, string.Empty);
            superRodTenComboBox.DataSource = new BindingSource(names, string.Empty);
            superRodFiveComboBox.DataSource = new BindingSource(names, string.Empty);

            /*Swarm encounters*/
            grassSwarmComboBox.DataSource = new BindingSource(names, string.Empty);
            surfSwarmComboBox.DataSource = new BindingSource(names, string.Empty);
            rodSwarmComboBox.DataSource = new BindingSource(names, string.Empty);

            nightFishingComboBox.DataSource = new BindingSource(names, string.Empty);
        }

        private void RegisterMarkDirtyHandlers()
        {
            //* Walking encounters *//
            // Walking encounters - Morning
            morningTwentyFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            morningTwentySecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            morningTenFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            morningTenSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            morningTenThirdComboBox.SelectedIndexChanged += MarkDirtyWalking;
            morningTenFourthComboBox.SelectedIndexChanged += MarkDirtyWalking;
            morningFiveFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            morningFiveSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            morningFourFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            morningFourSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            morningOneFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            morningOneSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;

            // Walking encounters - Day
            dayTwentyFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            dayTwentySecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            dayTenFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            dayTenSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            dayTenThirdComboBox.SelectedIndexChanged += MarkDirtyWalking;
            dayTenFourthComboBox.SelectedIndexChanged += MarkDirtyWalking;
            dayFiveFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            dayFiveSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            dayFourFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            dayFourSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            dayOneFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            dayOneSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;

            // Walking encounters - Night
            nightTwentyFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightTwentySecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightTenFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightTenSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightTenThirdComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightTenFourthComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightFiveFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightFiveSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightFourFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightFourSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightOneFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            nightOneSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;

            // Radio music encounters
            hoennFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            hoennSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;
            sinnohFirstComboBox.SelectedIndexChanged += MarkDirtyWalking;
            sinnohSecondComboBox.SelectedIndexChanged += MarkDirtyWalking;

            // Rock smash encounters
            rockSmashNinetyComboBox.SelectedIndexChanged += MarkDirtyWalking;
            rockSmashTenComboBox.SelectedIndexChanged += MarkDirtyWalking;

            // Swarm encounters
            grassSwarmComboBox.SelectedIndexChanged += MarkDirtyWalking;

            // Walking levels
            twentyFirstLevelUpDown.ValueChanged += MarkDirtyWalking;
            twentySecondLevelUpDown.ValueChanged += MarkDirtyWalking;
            tenFirstLevelUpDown.ValueChanged += MarkDirtyWalking;
            tenSecondLevelUpDown.ValueChanged += MarkDirtyWalking;
            tenThirdLevelUpDown.ValueChanged += MarkDirtyWalking;
            tenFourthLevelUpDown.ValueChanged += MarkDirtyWalking;
            fiveFirstLevelUpDown.ValueChanged += MarkDirtyWalking;
            fiveSecondLevelUpDown.ValueChanged += MarkDirtyWalking;
            fourFirstLevelUpDown.ValueChanged += MarkDirtyWalking;
            fourSecondLevelUpDown.ValueChanged += MarkDirtyWalking;
            oneFirstLevelUpDown.ValueChanged += MarkDirtyWalking;
            oneSecondLevelUpDown.ValueChanged += MarkDirtyWalking;

            // Rock smash levels
            rockSmashNinetyMinLevelUpDown.ValueChanged += MarkDirtyWalking;
            rockSmashTenMinLevelUpDown.ValueChanged += MarkDirtyWalking;
            rockSmashNinetyMaxLevelUpDown.ValueChanged += MarkDirtyWalking;
            rockSmashTenMaxLevelUpDown.ValueChanged += MarkDirtyWalking;

            // Encounter rates
            walkingRateUpDown.ValueChanged += MarkDirtyWalking;
            rockSmashRateUpDown.ValueChanged += MarkDirtyWalking;

            //* Water encounters *//
            // Surf encounters
            surfSixtyComboBox.SelectedIndexChanged += MarkDirtyWater;
            surfThirtyComboBox.SelectedIndexChanged += MarkDirtyWater;
            surfFiveComboBox.SelectedIndexChanged += MarkDirtyWater;
            surfFourComboBox.SelectedIndexChanged += MarkDirtyWater;
            surfOneComboBox.SelectedIndexChanged += MarkDirtyWater;
            surfSwarmComboBox.SelectedIndexChanged += MarkDirtyWater;

            // Fishing encounters - Old rod
            oldRodFortyComboBox.SelectedIndexChanged += MarkDirtyWater;
            oldRodThirtyComboBox.SelectedIndexChanged += MarkDirtyWater;
            oldRodFifteenComboBox.SelectedIndexChanged += MarkDirtyWater;
            oldRodTenComboBox.SelectedIndexChanged += MarkDirtyWater;
            oldRodFiveComboBox.SelectedIndexChanged += MarkDirtyWater;

            // Fishing encounters - Good rod
            goodRodFortyComboBox.SelectedIndexChanged += MarkDirtyWater;
            goodRodThirtyComboBox.SelectedIndexChanged += MarkDirtyWater;
            goodRodFifteenComboBox.SelectedIndexChanged += MarkDirtyWater;
            goodRodTenComboBox.SelectedIndexChanged += MarkDirtyWater;
            goodRodFiveComboBox.SelectedIndexChanged += MarkDirtyWater;

            // Fishing encounters - Super rod
            superRodFortyComboBox.SelectedIndexChanged += MarkDirtyWater;
            superRodThirtyComboBox.SelectedIndexChanged += MarkDirtyWater;
            superRodFifteenComboBox.SelectedIndexChanged += MarkDirtyWater;
            superRodTenComboBox.SelectedIndexChanged += MarkDirtyWater;
            superRodFiveComboBox.SelectedIndexChanged += MarkDirtyWater;

            // Swarm encounters (water)
            nightFishingComboBox.SelectedIndexChanged += MarkDirtyWater;
            rodSwarmComboBox.SelectedIndexChanged += MarkDirtyWater;

            // Surf levels
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

            // Old rod levels
            oldRodFortyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodFortyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodThirtyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodThirtyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodFifteenMinLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodFifteenMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodTenMinLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodTenMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodFiveMinLevelUpDown.ValueChanged += MarkDirtyWater;
            oldRodFiveMaxLevelUpDown.ValueChanged += MarkDirtyWater;

            // Good rod levels
            goodRodFortyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodFortyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodThirtyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodThirtyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodFifteenMinLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodFifteenMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodTenMinLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodTenMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodFiveMinLevelUpDown.ValueChanged += MarkDirtyWater;
            goodRodFiveMaxLevelUpDown.ValueChanged += MarkDirtyWater;

            // Super rod levels
            superRodFortyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodFortyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodThirtyMinLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodThirtyMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodFifteenMinLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodFifteenMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodTenMinLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodTenMaxLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodFiveMinLevelUpDown.ValueChanged += MarkDirtyWater;
            superRodFiveMaxLevelUpDown.ValueChanged += MarkDirtyWater;

            // Water encounter rates
            surfRateUpDown.ValueChanged += MarkDirtyWater;
            oldRodRateUpDown.ValueChanged += MarkDirtyWater;
            goodRodRateUpDown.ValueChanged += MarkDirtyWater;
            superRodRateUpDown.ValueChanged += MarkDirtyWater;
        }

        private void SaveWalking()
        {
            if (currentFile == null) 
            {
                return;
            }

            //* Time based encounters *//
            /* Morning */
            currentFile.morningPokemon[0] = (ushort)morningTwentyFirstComboBox.SelectedIndex;
            currentFile.morningPokemon[1] = (ushort)morningTwentySecondComboBox.SelectedIndex;
            currentFile.morningPokemon[2] = (ushort)morningTenFirstComboBox.SelectedIndex;
            currentFile.morningPokemon[3] = (ushort)morningTenSecondComboBox.SelectedIndex;
            currentFile.morningPokemon[4] = (ushort)morningTenThirdComboBox.SelectedIndex;
            currentFile.morningPokemon[5] = (ushort)morningTenFourthComboBox.SelectedIndex;
            currentFile.morningPokemon[6] = (ushort)morningFiveFirstComboBox.SelectedIndex;
            currentFile.morningPokemon[7] = (ushort)morningFiveSecondComboBox.SelectedIndex;
            currentFile.morningPokemon[8] = (ushort)morningFourFirstComboBox.SelectedIndex;
            currentFile.morningPokemon[9] = (ushort)morningFourSecondComboBox.SelectedIndex;
            currentFile.morningPokemon[10] = (ushort)morningOneFirstComboBox.SelectedIndex;
            currentFile.morningPokemon[11] = (ushort)morningOneSecondComboBox.SelectedIndex;

            /* Day */
            currentFile.dayPokemon[0] = (ushort)dayTwentyFirstComboBox.SelectedIndex;
            currentFile.dayPokemon[1] = (ushort)dayTwentySecondComboBox.SelectedIndex;
            currentFile.dayPokemon[2] = (ushort)dayTenFirstComboBox.SelectedIndex;
            currentFile.dayPokemon[3] = (ushort)dayTenSecondComboBox.SelectedIndex;
            currentFile.dayPokemon[4] = (ushort)dayTenThirdComboBox.SelectedIndex;
            currentFile.dayPokemon[5] = (ushort)dayTenFourthComboBox.SelectedIndex;
            currentFile.dayPokemon[6] = (ushort)dayFiveFirstComboBox.SelectedIndex;
            currentFile.dayPokemon[7] = (ushort)dayFiveSecondComboBox.SelectedIndex;
            currentFile.dayPokemon[8] = (ushort)dayFourFirstComboBox.SelectedIndex;
            currentFile.dayPokemon[9] = (ushort)dayFourSecondComboBox.SelectedIndex;
            currentFile.dayPokemon[10] = (ushort)dayOneFirstComboBox.SelectedIndex;
            currentFile.dayPokemon[11] = (ushort)dayOneSecondComboBox.SelectedIndex;

            /* Night */
            currentFile.nightPokemon[0] = (ushort)nightTwentyFirstComboBox.SelectedIndex;
            currentFile.nightPokemon[1] = (ushort)nightTwentySecondComboBox.SelectedIndex;
            currentFile.nightPokemon[2] = (ushort)nightTenFirstComboBox.SelectedIndex;
            currentFile.nightPokemon[3] = (ushort)nightTenSecondComboBox.SelectedIndex;
            currentFile.nightPokemon[4] = (ushort)nightTenThirdComboBox.SelectedIndex;
            currentFile.nightPokemon[5] = (ushort)nightTenFourthComboBox.SelectedIndex;
            currentFile.nightPokemon[6] = (ushort)nightFiveFirstComboBox.SelectedIndex;
            currentFile.nightPokemon[7] = (ushort)nightFiveSecondComboBox.SelectedIndex;
            currentFile.nightPokemon[8] = (ushort)nightFourFirstComboBox.SelectedIndex;
            currentFile.nightPokemon[9] = (ushort)nightFourSecondComboBox.SelectedIndex;
            currentFile.nightPokemon[10] = (ushort)nightOneFirstComboBox.SelectedIndex;
            currentFile.nightPokemon[11] = (ushort)nightOneSecondComboBox.SelectedIndex;

            //* Other encounters *//
            /* Radio music encounters */
            currentFile.hoennMusicPokemon[0] = (ushort)hoennFirstComboBox.SelectedIndex;
            currentFile.hoennMusicPokemon[1] = (ushort)hoennSecondComboBox.SelectedIndex;
            currentFile.sinnohMusicPokemon[0] = (ushort)sinnohFirstComboBox.SelectedIndex;
            currentFile.sinnohMusicPokemon[1] = (ushort)sinnohSecondComboBox.SelectedIndex;

            /* Rock smash encounters */
            currentFile.rockSmashPokemon[0] = (ushort)rockSmashNinetyComboBox.SelectedIndex;
            currentFile.rockSmashPokemon[1] = (ushort)rockSmashTenComboBox.SelectedIndex;

            /* Swarm encounters */
            currentFile.swarmPokemon[0] = (ushort)grassSwarmComboBox.SelectedIndex;

            //* Levels *//
            /* Time based encounters */
            currentFile.walkingLevels[0] = (byte)twentyFirstLevelUpDown.Value;
            currentFile.walkingLevels[1] = (byte)twentySecondLevelUpDown.Value;
            currentFile.walkingLevels[2] = (byte)tenFirstLevelUpDown.Value;
            currentFile.walkingLevels[3] = (byte)tenSecondLevelUpDown.Value;
            currentFile.walkingLevels[4] = (byte)tenThirdLevelUpDown.Value;
            currentFile.walkingLevels[5] = (byte)tenFourthLevelUpDown.Value;
            currentFile.walkingLevels[6] = (byte)fiveFirstLevelUpDown.Value;
            currentFile.walkingLevels[7] = (byte)fiveSecondLevelUpDown.Value;
            currentFile.walkingLevels[8] = (byte)fourFirstLevelUpDown.Value;
            currentFile.walkingLevels[9] = (byte)fourSecondLevelUpDown.Value;
            currentFile.walkingLevels[10] = (byte)oneFirstLevelUpDown.Value;
            currentFile.walkingLevels[11] = (byte)oneSecondLevelUpDown.Value;

            /* Rock smash encounters */
            currentFile.rockSmashMinLevels[0] = (byte)rockSmashNinetyMinLevelUpDown.Value;
            currentFile.rockSmashMinLevels[1] = (byte)rockSmashTenMinLevelUpDown.Value;
            currentFile.rockSmashMaxLevels[0] = (byte)rockSmashNinetyMaxLevelUpDown.Value;
            currentFile.rockSmashMaxLevels[1] = (byte)rockSmashTenMaxLevelUpDown.Value;

            //* Encounters rates *//
            currentFile.walkingRate = (byte)walkingRateUpDown.Value;
            currentFile.rockSmashRate = (byte)rockSmashRateUpDown.Value;

            SetDirtyWalking(false);

        }

        private void SaveWater()
        {
            if (currentFile == null) 
            {
                return;
            }
            /* Surf encounters */
            currentFile.surfPokemon[0] = (ushort)surfSixtyComboBox.SelectedIndex;
            currentFile.surfPokemon[1] = (ushort)surfThirtyComboBox.SelectedIndex;
            currentFile.surfPokemon[2] = (ushort)surfFiveComboBox.SelectedIndex;
            currentFile.surfPokemon[3] = (ushort)surfFourComboBox.SelectedIndex;
            currentFile.surfPokemon[4] = (ushort)surfOneComboBox.SelectedIndex;

            currentFile.swarmPokemon[1] = (ushort)surfSwarmComboBox.SelectedIndex;

            //* Fishing encounters *//
            /* Old rod */
            currentFile.oldRodPokemon[0] = (ushort)oldRodFortyComboBox.SelectedIndex;
            currentFile.oldRodPokemon[1] = (ushort)oldRodThirtyComboBox.SelectedIndex;
            currentFile.oldRodPokemon[2] = (ushort)oldRodFifteenComboBox.SelectedIndex;
            currentFile.oldRodPokemon[3] = (ushort)oldRodTenComboBox.SelectedIndex;
            currentFile.oldRodPokemon[4] = (ushort)oldRodFiveComboBox.SelectedIndex;

            /* Good rod */
            currentFile.goodRodPokemon[0] = (ushort)goodRodFortyComboBox.SelectedIndex;
            currentFile.goodRodPokemon[1] = (ushort)goodRodThirtyComboBox.SelectedIndex;
            currentFile.goodRodPokemon[2] = (ushort)goodRodFifteenComboBox.SelectedIndex;
            currentFile.goodRodPokemon[3] = (ushort)goodRodTenComboBox.SelectedIndex;
            currentFile.goodRodPokemon[4] = (ushort)goodRodFiveComboBox.SelectedIndex;

            /* Super rod */
            currentFile.superRodPokemon[0] = (ushort)superRodFortyComboBox.SelectedIndex;
            currentFile.superRodPokemon[1] = (ushort)superRodThirtyComboBox.SelectedIndex;
            currentFile.superRodPokemon[2] = (ushort)superRodFifteenComboBox.SelectedIndex;
            currentFile.superRodPokemon[3] = (ushort)superRodTenComboBox.SelectedIndex;
            currentFile.superRodPokemon[4] = (ushort)superRodFiveComboBox.SelectedIndex;

            /* Swarm encounters */
            currentFile.swarmPokemon[2] = (ushort)nightFishingComboBox.SelectedIndex; // Night fishing is part of the swarm array
            currentFile.swarmPokemon[3] = (ushort)rodSwarmComboBox.SelectedIndex;

            //* Levels *//
            /* Surf */
            currentFile.surfMinLevels[0] = (byte)surfSixtyMinLevelUpDown.Value;
            currentFile.surfMaxLevels[0] = (byte)surfSixtyMaxLevelUpDown.Value;
            currentFile.surfMinLevels[1] = (byte)surfThirtyMinLevelUpDown.Value;
            currentFile.surfMaxLevels[1] = (byte)surfThirtyMaxLevelUpDown.Value;
            currentFile.surfMinLevels[2] = (byte)surfFiveMinLevelUpDown.Value;
            currentFile.surfMaxLevels[2] = (byte)surfFiveMaxLevelUpDown.Value;
            currentFile.surfMinLevels[3] = (byte)surfFourMinLevelUpDown.Value;
            currentFile.surfMaxLevels[3] = (byte)surfFourMaxLevelUpDown.Value;
            currentFile.surfMinLevels[4] = (byte)surfOneMinLevelUpDown.Value;
            currentFile.surfMaxLevels[4] = (byte)surfOneMaxLevelUpDown.Value;

            /* Fishing */
            /* Old rod */
            currentFile.oldRodMinLevels[0] = (byte)oldRodFortyMinLevelUpDown.Value;
            currentFile.oldRodMaxLevels[0] = (byte)oldRodFortyMaxLevelUpDown.Value;
            currentFile.oldRodMinLevels[1] = (byte)oldRodThirtyMinLevelUpDown.Value;
            currentFile.oldRodMaxLevels[1] = (byte)oldRodThirtyMaxLevelUpDown.Value;
            currentFile.oldRodMinLevels[2] = (byte)oldRodFifteenMinLevelUpDown.Value;
            currentFile.oldRodMaxLevels[2] = (byte)oldRodFifteenMaxLevelUpDown.Value;
            currentFile.oldRodMinLevels[3] = (byte)oldRodTenMinLevelUpDown.Value;
            currentFile.oldRodMaxLevels[3] = (byte)oldRodTenMaxLevelUpDown.Value;
            currentFile.oldRodMinLevels[4] = (byte)oldRodFiveMinLevelUpDown.Value;
            currentFile.oldRodMaxLevels[4] = (byte)oldRodFiveMaxLevelUpDown.Value;

            /* Good rod */
            currentFile.goodRodMinLevels[0] = (byte)goodRodFortyMinLevelUpDown.Value;
            currentFile.goodRodMaxLevels[0] = (byte)goodRodFortyMaxLevelUpDown.Value;
            currentFile.goodRodMinLevels[1] = (byte)goodRodThirtyMinLevelUpDown.Value;
            currentFile.goodRodMaxLevels[1] = (byte)goodRodThirtyMaxLevelUpDown.Value;
            currentFile.goodRodMinLevels[2] = (byte)goodRodFifteenMinLevelUpDown.Value;
            currentFile.goodRodMaxLevels[2] = (byte)goodRodFifteenMaxLevelUpDown.Value;
            currentFile.goodRodMinLevels[3] = (byte)goodRodTenMinLevelUpDown.Value;
            currentFile.goodRodMaxLevels[3] = (byte)goodRodTenMaxLevelUpDown.Value;
            currentFile.goodRodMinLevels[4] = (byte)goodRodFiveMinLevelUpDown.Value;
            currentFile.goodRodMaxLevels[4] = (byte)goodRodFiveMaxLevelUpDown.Value;

            /* Super rod */
            currentFile.superRodMinLevels[0] = (byte)superRodFortyMinLevelUpDown.Value;
            currentFile.superRodMaxLevels[0] = (byte)superRodFortyMaxLevelUpDown.Value;
            currentFile.superRodMinLevels[1] = (byte)superRodThirtyMinLevelUpDown.Value;
            currentFile.superRodMaxLevels[1] = (byte)superRodThirtyMaxLevelUpDown.Value;
            currentFile.superRodMinLevels[2] = (byte)superRodFifteenMinLevelUpDown.Value;
            currentFile.superRodMaxLevels[2] = (byte)superRodFifteenMaxLevelUpDown.Value;
            currentFile.superRodMinLevels[3] = (byte)superRodTenMinLevelUpDown.Value;
            currentFile.superRodMaxLevels[3] = (byte)superRodTenMaxLevelUpDown.Value;
            currentFile.superRodMinLevels[4] = (byte)superRodFiveMinLevelUpDown.Value;
            currentFile.superRodMaxLevels[4] = (byte)superRodFiveMaxLevelUpDown.Value;

            //* Rates *//
            currentFile.surfRate = (byte)surfRateUpDown.Value;
            currentFile.oldRodRate = (byte)oldRodRateUpDown.Value;
            currentFile.goodRodRate = (byte)goodRodRateUpDown.Value;
            currentFile.superRodRate = (byte)superRodRateUpDown.Value;

            SetDirtyWater(false);
        }

        private void AddTooltips() 
        {
            // Radio music encounters
            SetToolTipsForControls(new Control[] { hoennFirstComboBox, sinnohFirstComboBox },
                "Replaces the first two 10% slots in the morning, day and night.");
            SetToolTipsForControls(new Control[] { hoennSecondComboBox, sinnohSecondComboBox },
                "Replaces the second two 10% slots in the morning, day and night.");

            SetToolTipsForControls(new Control[] { radioMusicGroupBox }, "Radio Music Encounters:\nThese replace the 10% slots.");
            SetToolTipsForControls(new Control[] {morningTenFirstComboBox, dayTenFirstComboBox, nightTenFirstComboBox,
                morningTenSecondComboBox, dayTenSecondComboBox, nightTenSecondComboBox}, "Replaced by radio slot 1 when radio is active.");
            SetToolTipsForControls(new Control[] {morningTenThirdComboBox, dayTenThirdComboBox, nightTenThirdComboBox,
                morningTenFourthComboBox, dayTenFourthComboBox, nightTenFourthComboBox}, "Replaced by radio slot 2 when radio is active.");

            // Swarm encounters
            SetToolTipsForControls(new Control[] { grassSwarmComboBox }, "This slot replaces both 20% slots in the morning, day and night.");
            SetToolTipsForControls(new Control[] {morningTwentyFirstComboBox, dayTwentyFirstComboBox, nightTwentyFirstComboBox,
                morningTwentySecondComboBox, dayTwentySecondComboBox, nightTwentySecondComboBox},"Replaced by grass swarm slot when swarm is active.");

            SetToolTipsForControls(new Control[] { surfSwarmComboBox }, "Replaces the 60% surfing slot.");
            SetToolTipsForControls(new Control[] { surfSixtyComboBox }, "Replaced by surf swarm slot when swarm is active.");

            SetToolTipsForControls(new Control[] { rodSwarmComboBox }, "Replaced slot depends on the rod used:\n" +
                "Old Rod: 15% slot\n" +
                "Good Rod: 40% slot\n" +
                "Super Rod: all slots");
            SetToolTipsForControls(new Control[] { oldRodFifteenComboBox, goodRodFortyComboBox, 
            superRodFortyComboBox, superRodFifteenComboBox, superRodTenComboBox, superRodFiveComboBox },
                "Replaced by fishing swarm slot when swarm is active.");

            // Night fishing
            SetToolTipsForControls(new Control[] { nightFishingComboBox }, "Replaced slot depends on the rod used:\n" +
                "Good Rod: 10% slot\n" +
                "Super Rod: 30% slot");
            SetToolTipsForControls(new Control[] { goodRodTenComboBox }, "Replaced by night fishing slot at night.");
            SetToolTipsForControls(new Control[] { superRodThirtyComboBox }, "Replaced by night fishing slot at night.\nReplaced by fishing swarm slot when swarm is active.");
        }

        private void SetToolTipsForControls(IEnumerable<Control> controls, string text)
        {
            foreach (var control in controls)
            {
                toolTip.SetToolTip(control, text);
            }
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
            currentFile = new EncounterFileHGSS(new FileStream(of.FileName, FileMode.Open));

            /* Update controls */
            SetupControls();
        }
		private void selectEncounterComboBox_SelectedIndexChanged(object sender, EventArgs e) 
        {
            if (Helpers.HandlersDisabled) {
                return;
            }

            Helpers.DisableHandlers();

            if (!ContinueUnsavedChanges()) {
                selectEncounterComboBox.SelectedIndex = loadedEncounterFileIndex;
                Helpers.EnableHandlers();
                return; // Cancel the change
            }

            currentFile = new EncounterFileHGSS(selectEncounterComboBox.SelectedIndex);
            SetupControls();
            Helpers.EnableHandlers();
        }
        private void saveEncountersButton_Click(object sender, EventArgs e) 
        {
            SaveWalking();
            SaveWater();
            currentFile.SaveToFileDefaultDir(selectEncounterComboBox.SelectedIndex, false);
        }        

        private void addEncounterFileButton_Click(object sender, EventArgs e) 
        {
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

        private void removeLastEncounterFileButton_Click(object sender, EventArgs e) 
        {
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

        private void repairAllButton_Click(object sender, EventArgs e) 
        {
            DialogResult d = MessageBox.Show("DSPRE is about to open every Encounter File and attempt to reset every corrupted field to its default value.\n" +
                "Do you wish to proceed?", "Repair all Encounter Files?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes) {
                for (int i = 0; i < Directory.GetFiles(encounterFileFolder).Length; i++) {
                    currentFile.SaveToFileDefaultDir(i, showSuccessMessage: false);
                }

                MessageBox.Show("All repairable fields have been fixed.", "Operation completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void WildEditorHGSS_FormClosing(object sender, FormClosingEventArgs e) 
        {
            if (!ContinueUnsavedChanges()) 
            {
                e.Cancel = true; // Cancel closing the form
            }
        }

        #endregion
    }
}
