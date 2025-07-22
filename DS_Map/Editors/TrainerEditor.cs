using DSPRE.ROMFiles;
using Ekona.Images;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DSPRE.RomInfo;
using static DSPRE.ROMFiles.SpeciesFile;
using Images;

namespace DSPRE.Editors
{
    public partial class TrainerEditor : UserControl
    {
        MainProgram _parent;
        public bool trainerEditorIsReady { get; set; } = false;
        public TrainerEditor()
        {
            InitializeComponent();
        }

        private List<ComboBox> partyPokemonComboboxList = new List<ComboBox>();
        private List<ComboBox> partyItemsComboboxList = new List<ComboBox>();
        private List<GroupBox> partyMovesGroupboxList = new List<GroupBox>();
        private List<NumericUpDown> partyLevelUpdownList = new List<NumericUpDown>();
        private List<ComboBox> partyGenderComboBoxList = new List<ComboBox>();
        private List<ComboBox> partyAbilityComboBoxList = new List<ComboBox>();
        private List<ComboBox> partyFormComboBoxList = new List<ComboBox>();
        private List<NumericUpDown> partyIVUpdownList = new List<NumericUpDown>();
        private List<NumericUpDown> partyBallUpdownList = new List<NumericUpDown>();
        private List<GroupBox> partyGroupboxList = new List<GroupBox>();
        private List<PictureBox> partyPokemonPictureBoxList = new List<PictureBox>();
        private List<PictureBox> partyPokemonItemIconList = new List<PictureBox>();

        private const int TRAINER_PARTY_POKEMON_GENDER_DEFAULT_INDEX = 0;
        private const int TRAINER_PARTY_POKEMON_GENDER_MALE_INDEX = 1;
        private const int TRAINER_PARTY_POKEMON_GENDER_FEMALE_INDEX = 2;
        private const int TRAINER_PARTY_POKEMON_ABILITY_DEFAULT_INDEX = 0;
        private const int TRAINER_PARTY_POKEMON_ABILITY_SLOT1_INDEX = 1;
        private const int TRAINER_PARTY_POKEMON_ABILITY_SLOT2_INDEX = 2;


        string[] abilityNames;
        SpeciesFile[] pokemonSpecies;

        private (int abi1, int abi2)[] pokemonSpeciesAbilities;
        private List<(int monIdx, int moveIdx1, int moveIdx2)> duplicateMoves = new List<(int monIdx, int moveIdx1, int moveIdx2)>();

        TrainerFile currentTrainerFile;
        public PaletteBase trainerPal;
        public ImageBase trainerTile;
        public SpriteBase trainerSprite;

        Dictionary<byte, (uint entryOffset, ushort musicD, ushort? musicN)> trainerClassEncounterMusicDict;
        private void SetupTrainerClassEncounterMusicTable()
        {
            RomInfo.SetEncounterMusicTableOffsetToRAMAddress();
            trainerClassEncounterMusicDict = new Dictionary<byte, (uint entryOffset, ushort musicD, ushort? musicN)>();

            uint encounterMusicTableTableStartAddress = BitConverter.ToUInt32(ARM9.ReadBytes(RomInfo.encounterMusicTableOffsetToRAMAddress, 4), 0) - ARM9.address;
            uint tableSizeOffset = 10;
            if (gameFamily == GameFamilies.HGSS)
            {
                tableSizeOffset += 2;
                encounterSSEQAltUpDown.Enabled = true;
            }

            byte tableEntriesCount = ARM9.ReadByte(RomInfo.encounterMusicTableOffsetToRAMAddress - tableSizeOffset);
            using (ARM9.Reader ar = new ARM9.Reader(encounterMusicTableTableStartAddress))
            {
                for (int i = 0; i < tableEntriesCount; i++)
                {
                    uint entryOffset = (uint)ar.BaseStream.Position;
                    byte tclass = (byte)ar.ReadUInt16();
                    ushort musicD = ar.ReadUInt16();
                    ushort? musicN = gameFamily == GameFamilies.HGSS ? ar.ReadUInt16() : (ushort?)null;
                    trainerClassEncounterMusicDict[tclass] = (entryOffset, musicD, musicN);
                }
            }
        }

        public void RefreshAbilities(int forPokemon)
        {
            DialogResult res = MessageBox.Show("You have modified a Pokemon's ability.\nDo you wish to refresh the Trainer Editor so your changes are available?", "Refresh Trainer Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res.Equals(DialogResult.Yes))
            {
                int currentIndex = trainerComboBox.SelectedIndex;
                SetupTrainerEditor(_parent);
                trainerComboBox.SelectedIndex = currentIndex;
            }
        }

        public int LoadTrainerClassPic(int trClassID)
        {
            int paletteFileID = (trClassID * 5 + 1);
            string paletteFilename = paletteFileID.ToString("D4");
            trainerPal = new NCLR(gameDirs[DirNames.trainerGraphics].unpackedDir + "\\" + paletteFilename, paletteFileID, paletteFilename);

            int tilesFileID = trClassID * 5;
            string tilesFilename = tilesFileID.ToString("D4");
            trainerTile = new NCGR(gameDirs[DirNames.trainerGraphics].unpackedDir + "\\" + tilesFilename, tilesFileID, tilesFilename);

            if (gameFamily == GameFamilies.DP)
            {
                return 0;
            }

            int spriteFileID = (trClassID * 5 + 2);
            string spriteFilename = spriteFileID.ToString("D4");
            trainerSprite = new NCER(gameDirs[DirNames.trainerGraphics].unpackedDir + "\\" + spriteFilename, spriteFileID, spriteFilename);

            return trainerSprite.Banks.Length - 1;
        }
        public void UpdateTrainerClassPic(PictureBox pb, int frameNumber = 0)
        {
            if (trainerSprite == null)
            {
                Console.WriteLine("Sprite is null!");
                return;
            }

            int bank0OAMcount = trainerSprite.Banks[0].oams.Length;
            int[] OAMenabled = new int[bank0OAMcount];
            for (int i = 0; i < OAMenabled.Length; i++)
            {
                OAMenabled[i] = i;
            }

            frameNumber = Math.Min(trainerSprite.Banks.Length, frameNumber);
            Image trSprite = trainerSprite.Get_Image(trainerTile, trainerPal, frameNumber, trainerClassPicBox.Width, trainerClassPicBox.Height, false, false, false, true, true, -1, OAMenabled);
            pb.Image = trSprite;
            pb.Update();
        }


        public void SetupTrainerEditor(MainProgram parent, bool force=false)
        {
            if (trainerEditorIsReady && !force) return;

            trainerEditorIsReady = true;
            _parent = parent;
            Helpers.DisableHandlers();

            //SetTrainerNameMaxLen();
            SetupTrainerClassEncounterMusicTable();
            /* Extract essential NARCs sub-archives*/
            Helpers.statusLabelMessage("Setting up Trainer Editor...");
            Update();

            DSUtils.TryUnpackNarcs(new List<DirNames> {
                DirNames.trainerProperties,
                DirNames.trainerParty,
                DirNames.trainerGraphics,
                DirNames.textArchives,
                DirNames.monIcons,
                DirNames.personalPokeData,
                DirNames.learnsets
            });

            int numPokemonSpecies = Directory.GetFiles(RomInfo.gameDirs[DirNames.personalPokeData].unpackedDir, "*").Count();
            pokemonSpeciesAbilities = new (int abi1, int abi2)[numPokemonSpecies];
            pokemonSpecies = new SpeciesFile[numPokemonSpecies];

            RomInfo.SetMonIconsPalTableAddress();
            RomInfo.SetAIBackportEnabled();

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

            partyGenderComboBoxList.Clear();
            partyGenderComboBoxList.Add(partyGender1ComboBox);
            partyGenderComboBoxList.Add(partyGender2ComboBox);
            partyGenderComboBoxList.Add(partyGender3ComboBox);
            partyGenderComboBoxList.Add(partyGender4ComboBox);
            partyGenderComboBoxList.Add(partyGender5ComboBox);
            partyGenderComboBoxList.Add(partyGender6ComboBox);

            partyAbilityComboBoxList.Clear();
            partyAbilityComboBoxList.Add(partyAbility1ComboBox);
            partyAbilityComboBoxList.Add(partyAbility2ComboBox);
            partyAbilityComboBoxList.Add(partyAbility3ComboBox);
            partyAbilityComboBoxList.Add(partyAbility4ComboBox);
            partyAbilityComboBoxList.Add(partyAbility5ComboBox);
            partyAbilityComboBoxList.Add(partyAbility6ComboBox);

            partyFormComboBoxList.Clear();
            partyFormComboBoxList.Add(partyForm1ComboBox);
            partyFormComboBoxList.Add(partyForm2ComboBox);
            partyFormComboBoxList.Add(partyForm3ComboBox);
            partyFormComboBoxList.Add(partyForm4ComboBox);
            partyFormComboBoxList.Add(partyForm5ComboBox);
            partyFormComboBoxList.Add(partyForm6ComboBox);

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
            trainerComboBox.Items.AddRange(Helpers.GetTrainerNames());

            string[] classNames = RomInfo.GetTrainerClassNames();
            trainerClassListBox.Items.Clear();
            if (classNames.Length > byte.MaxValue + 1)
            {
                MessageBox.Show("There can't be more than 256 trainer classes! [Found " + classNames.Length + "].\nAborting.",
                    "Too many trainer classes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            for (int i = 0; i < classNames.Length; i++)
            {
                trainerClassListBox.Items.Add("[" + i.ToString("D3") + "]" + " " + classNames[i]);
            }

            for (int i = 0; i < numPokemonSpecies; i++)
            {
                pokemonSpecies[i] = new SpeciesFile(new FileStream(RomInfo.gameDirs[DirNames.personalPokeData].unpackedDir + "\\" + i.ToString("D4"), FileMode.Open));
            }

            if (gameFamily == GameFamilies.HGSS || RomInfo.AIBackportEnabled)
            {
                foreach (ComboBox partyGenderComboBox in partyGenderComboBoxList)
                {
                    partyGenderComboBox.Visible = true;
                    partyGenderComboBox.Items.Add("Default Gender");
                    partyGenderComboBox.Items.Add("Male");
                    partyGenderComboBox.Items.Add("Female");
                }
            }
            else
            {
                foreach (ComboBox partyGenderComboBox in partyGenderComboBoxList)
                {
                    partyGenderComboBox.Visible = false;
                }
            }

            if (gameFamily == GameFamilies.DP)
            {
                foreach (ComboBox partyFormComboBox in partyFormComboBoxList)
                {
                    partyFormComboBox.Visible = false;
                }

                foreach (NumericUpDown partyBallSealUpDown in partyBallUpdownList)
                {
                    partyBallSealUpDown.Enabled = false;
                }
            }
            else
            {
                foreach (ComboBox partyFormComboBox in partyFormComboBoxList)
                {
                    partyFormComboBox.Visible = true;
                }

                foreach (NumericUpDown partyBallSealUpDown in partyBallUpdownList)
                {
                    partyBallSealUpDown.Enabled = true;
                }
            }

            string[] itemNames = RomInfo.GetItemNames();
            string[] pokeNames = RomInfo.GetPokemonNames();
            string[] moveNames = RomInfo.GetAttackNames();
            abilityNames = RomInfo.GetAbilityNames();

            pokemonSpeciesAbilities = getPokemonAbilities(numPokemonSpecies);

            foreach (Control c in trainerItemsGroupBox.Controls)
            {
                if (c is ComboBox)
                {
                    (c as ComboBox).DataSource = new BindingSource(itemNames, string.Empty);
                }
            }

            foreach (ComboBox CB in partyPokemonComboboxList)
            {
                CB.DataSource = new BindingSource(pokeNames, string.Empty);
            }

            foreach (ComboBox CB in partyItemsComboboxList)
            {
                CB.DataSource = new BindingSource(itemNames, string.Empty);
            }

            foreach (GroupBox movesGroup in partyMovesGroupboxList)
            {
                foreach (Control c in movesGroup.Controls)
                {
                    if (c is ComboBox)
                    {
                        (c as ComboBox).DataSource = new BindingSource(moveNames, string.Empty);
                    }
                }
            }

            trainerComboBox.SelectedIndex = 0;

            Helpers.EnableHandlers();
            trainerComboBox_SelectedIndexChanged(null, null);
            Helpers.statusLabelMessage();
        }
        private void trainerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            Helpers.DisableHandlers();

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

            UpdateDuplicateMovesList();

            Helpers.EnableHandlers();

            if (error)
            {
                MessageBox.Show("This Trainer File doesn't have a corresponding name.\n\n" +
                    "If you edited this ROM's Trainers with another tool before, don't worry.\n" +
                    "DSPRE will attempt to add the missing line to the Trainer Names Text Archive [" + RomInfo.trainerNamesMessageNumber + "] upon resaving.",
                    "Trainer name not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void RefreshTrainerPropertiesGUI()
        {
            trainerNameTextBox.Text = currentTrainerFile.name;

            trainerClassListBox.SelectedIndex = currentTrainerFile.trp.trainerClass;
            trainerDoubleCheckBox.Checked = currentTrainerFile.trp.doubleBattle;
            trainerMovesCheckBox.Checked = currentTrainerFile.trp.chooseMoves;
            trainerItemsCheckBox.Checked = currentTrainerFile.trp.chooseItems;
            partyCountUpDown.Value = currentTrainerFile.trp.partyCount;

            IList trainerItems = trainerItemsGroupBox.Controls;
            for (int i = 0; i < trainerItems.Count; i++)
            {
                (trainerItems[i] as ComboBox).SelectedIndex = currentTrainerFile.trp.trainerItems[i];
            }

            IList trainerAI = TrainerAIGroupBox.Controls;
            for (int i = 0; i < trainerAI.Count; i++)
            {
                (trainerAI[i] as CheckBox).Checked = currentTrainerFile.trp.AI[i];
            }
        }
        public void RefreshTrainerPartyGUI()
        {
            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++)
            {
                partyPokemonComboboxList[i].SelectedIndex = currentTrainerFile.party[i].pokeID ?? 0;
                partyItemsComboboxList[i].SelectedIndex = currentTrainerFile.party[i].heldItem ?? 0;
                partyLevelUpdownList[i].Value = Math.Max((ushort)1, currentTrainerFile.party[i].level);
                partyIVUpdownList[i].Value = currentTrainerFile.party[i].difficulty;
                partyBallUpdownList[i].Value = currentTrainerFile.party[i].ballSeals;

                setTrainerPartyPokemonAbilities(i);
                setTrainerPartyPokemonForm(i);
                setTrainerPokemonGender(i);

                if (currentTrainerFile.party[i].genderAndAbilityFlags.HasFlag(PartyPokemon.GenderAndAbilityFlags.ABILITY_SLOT1))
                {
                    partyAbilityComboBoxList[i].SelectedIndex = TRAINER_PARTY_POKEMON_ABILITY_SLOT1_INDEX;
                }
                else if (currentTrainerFile.party[i].genderAndAbilityFlags.HasFlag(PartyPokemon.GenderAndAbilityFlags.ABILITY_SLOT2))
                {
                    partyAbilityComboBoxList[i].SelectedIndex = TRAINER_PARTY_POKEMON_ABILITY_SLOT2_INDEX;
                }
                else
                {
                    partyAbilityComboBoxList[i].SelectedIndex = TRAINER_PARTY_POKEMON_ABILITY_DEFAULT_INDEX;
                }

                partyFormComboBoxList[i].SelectedIndex = currentTrainerFile.party[i].formID;

                if (currentTrainerFile.party[i].moves == null)
                {
                    for (int j = 0; j < Party.MOVES_PER_POKE; j++)
                    {
                        (partyMovesGroupboxList[i].Controls[j] as ComboBox).SelectedIndex = 0;
                    }
                }
                else
                {
                    for (int j = 0; j < Party.MOVES_PER_POKE; j++)
                    {
                        (partyMovesGroupboxList[i].Controls[j] as ComboBox).SelectedIndex = currentTrainerFile.party[i].moves[j];
                        (partyMovesGroupboxList[i].Controls[j] as ComboBox).ForeColor = SystemColors.WindowText;
                    }
                }
            }

            MarkDuplicateMoves();

        }

        private void ShowPartyPokemonPic(byte partyPos)
        {
            ComboBox cb = partyPokemonComboboxList[partyPos];
            int species = cb.SelectedIndex > 0 ? cb.SelectedIndex : 0;

            PictureBox pb = partyPokemonPictureBoxList[partyPos];

            partyPokemonPictureBoxList[partyPos].Image = DSUtils.GetPokePic(species, pb.Width, pb.Height);
        }

        private void partyPokemon1ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowPartyPokemonPic(0);

            //event handler is called before currentTrainerFile is set, need to null check to avoid null object reference
            if (currentTrainerFile != null)
            {
                setTrainerPartyPokemonAbilities(0);
                setTrainerPartyPokemonForm(0);
                setTrainerPokemonGender(0);
            }
        }
        private void partyPokemon2ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowPartyPokemonPic(1);
            if (currentTrainerFile != null)
            {
                setTrainerPartyPokemonAbilities(1);
                setTrainerPartyPokemonForm(1);
                setTrainerPokemonGender(1);
            }
        }

        private void partyPokemon3ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowPartyPokemonPic(2);
            if (currentTrainerFile != null)
            {
                setTrainerPartyPokemonAbilities(2);
                setTrainerPartyPokemonForm(2);
                setTrainerPokemonGender(2);
            }
        }

        private void partyPokemon4ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowPartyPokemonPic(3);
            if (currentTrainerFile != null)
            {
                setTrainerPartyPokemonAbilities(3);
                setTrainerPartyPokemonForm(3);
                setTrainerPokemonGender(3);
            }
        }

        private void partyPokemon5ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowPartyPokemonPic(4);
            if (currentTrainerFile != null)
            {
                setTrainerPartyPokemonAbilities(4);
                setTrainerPartyPokemonForm(4);
                setTrainerPokemonGender(4);
            }
        }

        private void partyPokemon6ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowPartyPokemonPic(5);
            if (currentTrainerFile != null)
            {
                setTrainerPartyPokemonAbilities(5);
                setTrainerPartyPokemonForm(5);
                setTrainerPokemonGender(5);
            }
        }

        private void showTrainerEditorItemPic(byte partyPos)
        {
            ComboBox cb = partyItemsComboboxList[partyPos];
            partyPokemonItemIconList[partyPos].Visible = cb.SelectedIndex > 0;
        }

        private void partyItem1ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            showTrainerEditorItemPic(0);
        }

        private void partyItem2ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            showTrainerEditorItemPic(1);
        }

        private void partyItem3ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            showTrainerEditorItemPic(2);
        }

        private void partyItem4ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            showTrainerEditorItemPic(3);
        }

        private void partyItem5ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            showTrainerEditorItemPic(4);
        }

        private void partyItem6ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            showTrainerEditorItemPic(5);
        }

        private void DVExplainButton_Click(object sender, EventArgs e)
        {

            DVCalc DVCalcForm = new DVCalc(currentTrainerFile);
            DVCalcForm.ShowDialog();

            currentTrainerFile = DVCalcForm.trainerFile;
            Helpers.DisableHandlers();
            RefreshTrainerPartyGUI();
            Helpers.EnableHandlers();

        }

        private void partyCountUpDown_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++)
            {
                partyGroupboxList[i].Enabled = (partyCountUpDown.Value > i);
                partyPokemonPictureBoxList[i].Visible = partyGroupboxList[i].Enabled;
            }
            for (int i = Math.Min(currentTrainerFile.trp.partyCount, (int)partyCountUpDown.Value); i < TrainerFile.POKE_IN_PARTY; i++)
            {
                currentTrainerFile.party[i] = new PartyPokemon(currentTrainerFile.trp.chooseItems, currentTrainerFile.trp.chooseMoves);
            }
        }

        private void trainerMovesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++)
            {
                for (int j = 0; j < Party.MOVES_PER_POKE; j++)
                {
                    (partyMovesGroupboxList[i].Controls[j] as ComboBox).Enabled = trainerMovesCheckBox.Checked;
                }
                if (trainerMovesCheckBox.Checked && i < currentTrainerFile.trp.partyCount && Helpers.HandlersEnabled)
                {
                    Helpers.BackUpDisableHandler();
                    Helpers.DisableHandlers();
                    LearnsetData learnset = new LearnsetData((int)currentTrainerFile.party[i].pokeID);
                    int level = currentTrainerFile.party[i].level;
                    currentTrainerFile.party[i].moves = learnset.GetLearnsetAtLevel(level);
                    Debug.Print("Changing the moves of Pokemon " + i.ToString() + " which is Pokemon " + currentTrainerFile.party[i].pokeID);
                    Debug.Print("The new moves will be: " + string.Join(", ", currentTrainerFile.party[i].moves));
                    for (int j = 0; j < Party.MOVES_PER_POKE; j++)
                    {
                        (partyMovesGroupboxList[i].Controls[j] as ComboBox).SelectedIndex = currentTrainerFile.party[i].moves[j];
                        Debug.Print("Move for dropdwon " + j.ToString() + " is " + currentTrainerFile.party[i].moves[j].ToString());
                    }
                    Helpers.RestoreDisableHandler();
                }
                else
                {
                    //currentTrainerFile.party[i].moves = null;
                }
            }
            RefreshTrainerPartyGUI();
        }
        private void trainerItemsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++)
            {
                partyItemsComboboxList[i].Enabled = trainerItemsCheckBox.Checked;
            }
        }

        private void partyMoveComboBox_DropDown(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }
            ((ComboBox)sender).ForeColor = SystemColors.WindowText;
        }

        private void partyMoveComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            MarkDuplicateMoves();

        }

        private void MarkDuplicateMoves()
        {
            foreach (var cbGroup in partyMovesGroupboxList)
            {
                foreach (var cb in cbGroup.Controls)
                {
                    ((ComboBox)cb).ForeColor = SystemColors.WindowText;
                }
            }

            UpdateDuplicateMovesList();

            foreach (var duplicate in duplicateMoves)
            {
                ((ComboBox)partyMovesGroupboxList[duplicate.monIdx].Controls[duplicate.moveIdx1]).ForeColor = Color.Red;
                ((ComboBox)partyMovesGroupboxList[duplicate.monIdx].Controls[duplicate.moveIdx2]).ForeColor = Color.Red;
            }
        }

        private void UpdateDuplicateMovesList()
        {
            duplicateMoves.Clear();

            for (int pokemonIndex = 0; pokemonIndex < partyMovesGroupboxList.Count; pokemonIndex++)
            {
                for (int i = 0; i < Party.MOVES_PER_POKE - 1; i++)
                {
                    for (int j = i + 1; j < Party.MOVES_PER_POKE; j++)
                    {
                        ComboBox cb1 = (ComboBox)partyMovesGroupboxList[pokemonIndex].Controls[i];
                        ComboBox cb2 = (ComboBox)partyMovesGroupboxList[pokemonIndex].Controls[j];
                        if (cb1.SelectedIndex != 0 && cb1.SelectedIndex == cb2.SelectedIndex)
                        {
                            duplicateMoves.Add((pokemonIndex, i, j));
                        }
                    }
                }
            }
        }

        private void trainerSaveCurrentButton_Click(object sender, EventArgs e)
        {
            currentTrainerFile.trp.partyCount = (byte)partyCountUpDown.Value;
            currentTrainerFile.trp.chooseMoves = trainerMovesCheckBox.Checked;
            currentTrainerFile.trp.chooseItems = trainerItemsCheckBox.Checked;
            currentTrainerFile.trp.doubleBattle = trainerDoubleCheckBox.Checked;

            IList trainerItems = trainerItemsGroupBox.Controls;
            for (int i = 0; i < trainerItems.Count; i++)
            {
                currentTrainerFile.trp.trainerItems[i] = (ushort)(trainerItems[i] as ComboBox).SelectedIndex;
            }

            IList trainerAI = TrainerAIGroupBox.Controls;
            for (int i = 0; i < trainerAI.Count; i++)
            {
                currentTrainerFile.trp.AI[i] = (trainerAI[i] as CheckBox).Checked;
            }

            for (int i = 0; i < TrainerFile.POKE_IN_PARTY; i++)
            {
                currentTrainerFile.party[i].moves = trainerMovesCheckBox.Checked ? new ushort[4] : null;
            }


            for (int i = 0; i < partyCountUpDown.Value; i++)
            {
                currentTrainerFile.party[i].pokeID = (ushort)partyPokemonComboboxList[i].SelectedIndex;
                currentTrainerFile.party[i].formID = (ushort)partyFormComboBoxList[i].SelectedIndex;
                currentTrainerFile.party[i].level = (ushort)partyLevelUpdownList[i].Value;

                if (trainerMovesCheckBox.Checked)
                {
                    IList movesList = partyMovesGroupboxList[i].Controls;
                    for (int j = 0; j < Party.MOVES_PER_POKE; j++)
                    {
                        currentTrainerFile.party[i].moves[j] = (ushort)(movesList[j] as ComboBox).SelectedIndex;
                    }
                }

                if (trainerItemsCheckBox.Checked)
                {
                    currentTrainerFile.party[i].heldItem = (ushort)partyItemsComboboxList[i].SelectedIndex;
                }

                currentTrainerFile.party[i].difficulty = (byte)partyIVUpdownList[i].Value;

                if (hasMoreThanOneGender((int)currentTrainerFile.party[i].pokeID, pokemonSpecies) && (gameFamily == GameFamilies.HGSS || RomInfo.AIBackportEnabled))
                {
                    switch (partyGenderComboBoxList[i].SelectedIndex)
                    {
                        case TRAINER_PARTY_POKEMON_GENDER_DEFAULT_INDEX:
                            currentTrainerFile.party[i].genderAndAbilityFlags = PartyPokemon.GenderAndAbilityFlags.NO_FLAGS;
                            break;
                        case TRAINER_PARTY_POKEMON_GENDER_MALE_INDEX:
                            currentTrainerFile.party[i].genderAndAbilityFlags = PartyPokemon.GenderAndAbilityFlags.FORCE_MALE;
                            break;
                        case TRAINER_PARTY_POKEMON_GENDER_FEMALE_INDEX:
                            currentTrainerFile.party[i].genderAndAbilityFlags = PartyPokemon.GenderAndAbilityFlags.FORCE_FEMALE;
                            break;
                    }
                }
                else
                    currentTrainerFile.party[i].genderAndAbilityFlags = PartyPokemon.GenderAndAbilityFlags.NO_FLAGS;

                if (partyAbilityComboBoxList[i].SelectedIndex == TRAINER_PARTY_POKEMON_ABILITY_SLOT1_INDEX)
                {
                    currentTrainerFile.party[i].genderAndAbilityFlags |= PartyPokemon.GenderAndAbilityFlags.ABILITY_SLOT1;
                }
                else if (partyAbilityComboBoxList[i].SelectedIndex == TRAINER_PARTY_POKEMON_ABILITY_SLOT2_INDEX)
                {
                    currentTrainerFile.party[i].genderAndAbilityFlags |= PartyPokemon.GenderAndAbilityFlags.ABILITY_SLOT2;
                }

                currentTrainerFile.party[i].ballSeals = (ushort)partyBallUpdownList[i].Value;
            }

            if (duplicateMoves.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Duplicate moves have been found:\n");

                foreach (var duplicate in duplicateMoves)
                {
                    string move = partyMovesGroupboxList[duplicate.monIdx].Controls[duplicate.moveIdx1].Text;
                    sb.Append($"- {partyPokemonComboboxList[duplicate.monIdx].Text} has duplicate move \"{move}\"\n");
                }
                sb.Append("This will cause issues in game.");

                MessageBox.Show(sb.ToString(), "Duplicate Moves", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            /*Write to File*/
            string indexStr = "\\" + trainerComboBox.SelectedIndex.ToString("D4");
            File.WriteAllBytes(RomInfo.gameDirs[DirNames.trainerProperties].unpackedDir + indexStr, currentTrainerFile.trp.ToByteArray());
            File.WriteAllBytes(RomInfo.gameDirs[DirNames.trainerParty].unpackedDir + indexStr, currentTrainerFile.party.ToByteArray());

            UpdateCurrentTrainerName(newName: trainerNameTextBox.Text);
            UpdateCurrentTrainerShownName();

            if (trainerNameTextBox.Text.Length > RomInfo.trainerNameMaxLen)
            { //Subtract 1 to account for special end character. 
                //Expose a smaller limit to the user
                if (RomInfo.trainerNameLenOffset >= 0)
                {
                    MessageBox.Show($"Trainer File saved successfully. However:\nYou attempted to save a Trainer whose name exceeds {RomInfo.trainerNameMaxLen} characters.\nThis may lead to issues in game." +
                        (PatchToolboxDialog.flag_TrainerNamesExpanded ? "\n\nIt's recommended that you use a shorter name." : "\n\nRefer to the Patch Toolbox to extend Trainer names."),
                        "Saved successfully, but...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Trainer File saved successfully. However:\nThe Trainer name length could not be safely determined for this ROM.\n" +
                        $"You attempted to save a Trainer whose name exceeds {RomInfo.trainerNameMaxLen} characters.\nThis will most likely lead to issues in game.",
                        "Saved successfully, but...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Trainer saved successfully!", "Saved successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateCurrentTrainerShownName()
        {
            string trClass = GetTrainerClassNameFromListbox(trainerClassListBox.SelectedItem);

            string editedTrainer = "[" + currentTrainerFile.trp.trainerID.ToString("D2") + "] " + trClass + " " + currentTrainerFile.name;

            Helpers.DisableHandlers();
            trainerComboBox.Items[trainerComboBox.SelectedIndex] = editedTrainer;
            Helpers.EnableHandlers();

            if (_parent.eventEditorIsReady)
            {
                _parent.owTrainerComboBox.Items[trainerComboBox.SelectedIndex] = editedTrainer;
            }
        }

        private string GetTrainerClassNameFromListbox(object selectedItem)
        {
            string lbname = selectedItem.ToString();
            return lbname.Substring(lbname.IndexOf(" ") + 1);
        }

        private void UpdateCurrentTrainerName(string newName)
        {
            currentTrainerFile.name = newName;
            TextArchive trainerNames = new TextArchive(RomInfo.trainerNamesMessageNumber);
            if (currentTrainerFile.trp.trainerID < trainerNames.messages.Count)
            {
                trainerNames.messages[currentTrainerFile.trp.trainerID] = newName;
            }
            else
            {
                trainerNames.messages.Add(newName);
            }
            trainerNames.SaveToFileDefaultDir(RomInfo.trainerNamesMessageNumber, showSuccessMessage: false);
        }
        private void UpdateCurrentTrainerClassName(string newName)
        {
            TextArchive trainerClassNames = new TextArchive(RomInfo.trainerClassMessageNumber);
            trainerClassNames.messages[trainerClassListBox.SelectedIndex] = newName;
            trainerClassNames.SaveToFileDefaultDir(RomInfo.trainerClassMessageNumber, showSuccessMessage: false);
        }

        private void trainerClassListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selection = trainerClassListBox.SelectedIndex;
            if (selection < 0)
            {
                return;
            }

            try
            {
                int maxFrames = LoadTrainerClassPic(selection);
                UpdateTrainerClassPic(trainerClassPicBox);

                trClassFramePreviewUpDown.Maximum = maxFrames;
                trainerClassFrameMaxLabel.Text = "/" + maxFrames;
            }
            catch
            {
                trClassFramePreviewUpDown.Maximum = 0;
            }

            trainerClassNameTextbox.Text = GetTrainerClassNameFromListbox(trainerClassListBox.SelectedItem);

            if (trainerClassEncounterMusicDict.TryGetValue((byte)selection, out (uint entryOffset, ushort musicD, ushort? musicN) output))
            {
                encounterSSEQMainUpDown.Enabled = eyeContactMusicLabel.Enabled = true;
                encounterSSEQMainUpDown.Value = output.musicD;
            }
            else
            {
                encounterSSEQMainUpDown.Enabled = eyeContactMusicLabel.Enabled = false;
                encounterSSEQMainUpDown.Value = 0;
            }

            eyeContactMusicAltLabel.Enabled = encounterSSEQAltUpDown.Enabled = (encounterSSEQMainUpDown.Enabled && gameFamily == GameFamilies.HGSS);
            encounterSSEQAltUpDown.Value = output.musicN != null ? (ushort)output.musicN : 0;
            currentTrainerFile.trp.trainerClass = (byte)selection;
        }

        private void addTrainerButton_Click(object sender, EventArgs e)
        {
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

        private void exportTrainerButton_Click(object sender, EventArgs e)
        {
            currentTrainerFile.SaveToFileExplorePath("G4 Trainer File " + trainerComboBox.SelectedItem);
        }

        private void importTrainerButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = "Gen IV Trainer File (*.trf)|*.trf"
            };
            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            /* Update trainer on disk */
            using (DSUtils.EasyReader reader = new DSUtils.EasyReader(of.FileName))
            {
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

        private void exportPropertiesButton_Click(object sender, EventArgs e)
        {
            currentTrainerFile.trp.SaveToFileExplorePath("G4 Trainer Properties " + trainerComboBox.SelectedItem);
        }

        private void replacePropertiesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = "Gen IV Trainer Properties (*.trp)|*.trp"
            };
            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            /* Update trp object in memory */
            currentTrainerFile.trp = new TrainerProperties((ushort)trainerComboBox.SelectedIndex, new FileStream(of.FileName, FileMode.Open));
            RefreshTrainerPropertiesGUI();

            /* Display success message */
            MessageBox.Show("Trainer Properties imported successfully!\nRemember to save the current Trainer File.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exportPartyButton_Click(object sender, EventArgs e)
        {
            currentTrainerFile.party.exportCondensedData = true;
            currentTrainerFile.party.SaveToFileExplorePath("G4 Party Data " + trainerComboBox.SelectedItem);
            currentTrainerFile.party.exportCondensedData = false;
        }

        private void importReplacePartyButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = "Gen IV Party File (*.pdat)|*.pdat"
            };
            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            /* Update trp object in memory */
            currentTrainerFile.party = new Party(readFirstByte: true, TrainerFile.POKE_IN_PARTY, new FileStream(of.FileName, FileMode.Open), currentTrainerFile.trp);
            RefreshTrainerPropertiesGUI();
            RefreshTrainerPartyGUI();

            /* Display success message */
            MessageBox.Show("Trainer Party imported successfully!\nRemember to save the current Trainer File.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveTrainerClassButton_Click(object sender, EventArgs e)
        {
            Helpers.DisableHandlers();

            int selectedTrClass = trainerClassListBox.SelectedIndex;

            byte b_selectedTrClass = (byte)selectedTrClass;
            ushort eyeMusicID = (ushort)encounterSSEQMainUpDown.Value;
            ushort altEyeMusicID = (ushort)encounterSSEQAltUpDown.Value;

            if (trainerClassEncounterMusicDict.TryGetValue(b_selectedTrClass, out var dictEntry))
            {
                ARM9.WriteBytes(BitConverter.GetBytes(eyeMusicID), dictEntry.entryOffset + 2);

                if (gameFamily.Equals(GameFamilies.HGSS))
                {
                    ARM9.WriteBytes(BitConverter.GetBytes(altEyeMusicID), dictEntry.entryOffset + 4);
                }

                trainerClassEncounterMusicDict[b_selectedTrClass] = (dictEntry.entryOffset, eyeMusicID, altEyeMusicID);
            }

            string newName = trainerClassNameTextbox.Text;
            UpdateCurrentTrainerClassName(newName);
            trainerClassListBox.Items[selectedTrClass] = "[" + selectedTrClass.ToString("D3") + "]" + " " + newName;

            if (currentTrainerFile.trp.trainerClass == trainerClassListBox.SelectedIndex)
            {
                UpdateCurrentTrainerShownName();
            }
            Helpers.EnableHandlers();

            if (gameFamily.Equals(GameFamilies.HGSS) && EditorPanels.tableEditor.battleTableEditorIsReady && EditorPanels.tableEditor.conditionnalMusicTableEditorIsReady)
            {
                EditorPanels.tableEditor.pbEffectsTrainerCombobox.Items[selectedTrClass] = trainerClassListBox.Items[selectedTrClass];
                for (int i = 0; i < EditorPanels.tableEditor.vsTrainerEffectsList.Count; i++)
                {
                    if (EditorPanels.tableEditor.vsTrainerEffectsList[i].trainerClass == selectedTrClass)
                    {
                        EditorPanels.tableEditor.pbEffectsVsTrainerListbox.Items[i] = EditorPanels.tableEditor.pbEffectsTrainerCombobox.Items[selectedTrClass] + " uses Combo #" + EditorPanels.tableEditor.vsTrainerEffectsList[i].comboID;
                    }
                }
            }
            MessageBox.Show("Trainer Class settings saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void trClassFramePreviewUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateTrainerClassPic(trainerClassPicBox);
        }

        private (int abi1, int abi2)[] getPokemonAbilities(int numPokemonSpecies)
        {
            var pokemonSpeciesAbilities = new (int abi1, int abi2)[numPokemonSpecies];

            for (int i = 0; i < numPokemonSpecies; i++)
            {
                pokemonSpeciesAbilities[i] = (pokemonSpecies[i].Ability1, pokemonSpecies[i].Ability2);
            }

            return pokemonSpeciesAbilities;
        }

        private (string ability1, string ability2) getPokemonAbilityNames(int pokemonID)
        {
            return (abilityNames[pokemonSpeciesAbilities[pokemonID].abi1],
                    abilityNames[pokemonSpeciesAbilities[pokemonID].abi2]);
        }

        private void setTrainerPartyPokemonAbilities(int partyPokemonPosition)
        {
            (string ability1, string ability2) = getPokemonAbilityNames(partyPokemonComboboxList[partyPokemonPosition].SelectedIndex);
            string noFlags = "No Flag";

            partyAbilityComboBoxList[partyPokemonPosition].Items.Clear();

            // In DPPt just show ability 1 and do not allow editing
            if (RomInfo.gameFamily != GameFamilies.HGSS && !RomInfo.AIBackportEnabled)
            {
                partyAbilityComboBoxList[partyPokemonPosition].Items.Add(ability1);
                partyAbilityComboBoxList[partyPokemonPosition].Enabled = false;
                return;
            }

            // In HGSS allow editing of ability flags
            partyAbilityComboBoxList[partyPokemonPosition].Items.Add(noFlags);
            partyAbilityComboBoxList[partyPokemonPosition].Items.Add(ability1);

            string stringAbi2 = ability2;
            if (ability2.Equals(ability1))
            {
                stringAbi2 += " (2nd Slot)";
            }
            else if (ability2.Equals(" -"))
            {
                stringAbi2 = ability1 += " (2nd Slot)";
            }
            partyAbilityComboBoxList[partyPokemonPosition].Items.Add(stringAbi2);


        }

        private void setTrainerPokemonGender(int partyPokemonPosition)
        {
            int currentPokemonGenderRatio = pokemonSpecies[partyPokemonComboboxList[partyPokemonPosition].SelectedIndex].GenderRatioMaleToFemale;
            PartyPokemon.GenderAndAbilityFlags currentPokemonGenderAndAbilityFlags = currentTrainerFile.party[partyPokemonPosition].genderAndAbilityFlags;

            if (gameFamily == GameFamilies.HGSS || RomInfo.AIBackportEnabled)
            {
                switch (currentPokemonGenderRatio)
                {
                    case GENDER_RATIO_MALE:
                        partyGenderComboBoxList[partyPokemonPosition].SelectedIndex = TRAINER_PARTY_POKEMON_GENDER_MALE_INDEX;
                        partyGenderComboBoxList[partyPokemonPosition].Enabled = false;
                        break;
                    case GENDER_RATIO_FEMALE:
                        partyGenderComboBoxList[partyPokemonPosition].SelectedIndex = TRAINER_PARTY_POKEMON_GENDER_FEMALE_INDEX;
                        partyGenderComboBoxList[partyPokemonPosition].Enabled = false;
                        break;
                    case GENDER_RATIO_GENDERLESS:
                        partyGenderComboBoxList[partyPokemonPosition].SelectedIndex = TRAINER_PARTY_POKEMON_GENDER_DEFAULT_INDEX;
                        partyGenderComboBoxList[partyPokemonPosition].Enabled = false;
                        break;
                    default:
                        partyGenderComboBoxList[partyPokemonPosition].Enabled = true;

                        if (currentPokemonGenderAndAbilityFlags.HasFlag(PartyPokemon.GenderAndAbilityFlags.FORCE_MALE))
                            partyGenderComboBoxList[partyPokemonPosition].SelectedIndex = TRAINER_PARTY_POKEMON_GENDER_MALE_INDEX;
                        else if (currentPokemonGenderAndAbilityFlags.HasFlag(PartyPokemon.GenderAndAbilityFlags.FORCE_FEMALE))
                            partyGenderComboBoxList[partyPokemonPosition].SelectedIndex = TRAINER_PARTY_POKEMON_GENDER_FEMALE_INDEX;
                        else
                            partyGenderComboBoxList[partyPokemonPosition].SelectedIndex = TRAINER_PARTY_POKEMON_GENDER_DEFAULT_INDEX;
                        break;
                }
            }
        }

        private List<string> getPokemonFormNames(int pokemonID)
        {
            List<string> pokemonFormNames = new List<string>();

            switch (pokemonID)
            {
                case PICHU_ID_NUM:
                    if (RomInfo.gameFamily == GameFamilies.HGSS)
                    {
                        pokemonFormNames.Add("Non-Spiky-Eared");
                        pokemonFormNames.Add("Spiky-Eared");
                    }
                    else
                    {
                        pokemonFormNames.Add("No Alt Form");
                    }
                    break;
                case UNOWN_ID_NUM:
                    for (char c = 'A'; c <= 'Z'; c++)
                        pokemonFormNames.Add(c + " Form");

                    pokemonFormNames.Add("! Form");
                    pokemonFormNames.Add("? Form");
                    break;
                case CASTFORM_ID_NUM:
                    pokemonFormNames.Add("Normal Form");
                    pokemonFormNames.Add("Sunny Form");
                    pokemonFormNames.Add("Rainy Form");
                    pokemonFormNames.Add("Snowy Form");
                    break;
                case DEOXYS_ID_NUM:
                    pokemonFormNames.Add("Normal Form");
                    pokemonFormNames.Add("Attack Form");
                    pokemonFormNames.Add("Defense Form");
                    pokemonFormNames.Add("Speed Form");
                    break;
                case BURMY_ID_NUM:
                case WORMADAM_ID_NUM:
                    pokemonFormNames.Add("Plant Cloak");
                    pokemonFormNames.Add("Sand Cloak");
                    pokemonFormNames.Add("Trash Cloak");
                    break;
                case SHELLOS_ID_NUM:
                case GASTRODON_ID_NUM:
                    pokemonFormNames.Add("West sea");
                    pokemonFormNames.Add("East sea");
                    break;
                case ROTOM_ID_NUM:
                    pokemonFormNames.Add("Rotom");
                    pokemonFormNames.Add("Heat Rotom");
                    pokemonFormNames.Add("Wash Rotom");
                    pokemonFormNames.Add("Frost Rotom");
                    pokemonFormNames.Add("Fan Rotom");
                    pokemonFormNames.Add("Mow Rotom");
                    break;
                case SHAYMIN_ID_NUM:
                    pokemonFormNames.Add("Land Form");
                    pokemonFormNames.Add("Sky Form");
                    break;
                default:
                    pokemonFormNames.Add("No Alt Form");
                    break;
            }
            return pokemonFormNames;

        }

        private void setTrainerPartyPokemonForm(int partyPokemonPosition)
        {
            if (gameFamily != GameFamilies.DP)
            {
                partyFormComboBoxList[partyPokemonPosition].Items.Clear();
                List<string> currentPokemonFormName = getPokemonFormNames(partyPokemonComboboxList[partyPokemonPosition].SelectedIndex);
                foreach (string formName in currentPokemonFormName)
                    partyFormComboBoxList[partyPokemonPosition].Items.Add(formName);

                partyFormComboBoxList[partyPokemonPosition].Enabled = currentPokemonFormName.Count > 1;
                partyFormComboBoxList[partyPokemonPosition].SelectedIndex = 0;
            }

        }

    }
}
