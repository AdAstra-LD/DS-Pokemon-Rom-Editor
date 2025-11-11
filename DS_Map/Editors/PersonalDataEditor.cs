using DSPRE.Resources;
using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace DSPRE {
    public partial class PersonalDataEditor : Form {

        private readonly string[] fileNames;
        private readonly string[] pokenames;

        private int currentLoadedId = 0;
        private PokemonPersonalData currentLoadedFile = null;

        public bool dirty = false;
        private bool modifiedAbilities = false;
        private static readonly string formName = "Personal Data Editor";

        PokemonEditor _parent;

        public PersonalDataEditor(string[] itemNames, string[] abilityNames, System.Windows.Forms.Control parent, PokemonEditor pokeEditor) {
            this.fileNames = RomInfo.GetPokemonNames().ToArray();;
            this._parent = pokeEditor;
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = parent.Size;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            Helpers.DisableHandlers();
            ScriptDatabase.InitializeMoveNamesIfNeeded();
            BindingList<string> listItemNames = new BindingList<string>(itemNames);
            item1InputComboBox.DataSource = new BindingSource(listItemNames, string.Empty);
            item2InputComboBox.DataSource = new BindingSource(listItemNames, string.Empty);

            BindingList<string> listTypeNames = new BindingList<string>(RomInfo.GetTypeNames());
            type1InputComboBox.DataSource = new BindingSource(listTypeNames, string.Empty);
            type2InputComboBox.DataSource = new BindingSource(listTypeNames, string.Empty);

            BindingList<string> listAbilityNames = new BindingList<string>(abilityNames);
            ability1InputComboBox.DataSource = new BindingSource(listAbilityNames, string.Empty);
            ability2InputComboBox.DataSource = new BindingSource(listAbilityNames, string.Empty);

            BindingList<string> listEggGroups = new BindingList<string>(Enum.GetNames(typeof(PokemonEggGroup)));
            eggGroup1InputCombobox.DataSource = new BindingSource(listEggGroups, string.Empty);
            eggGroup2InputCombobox.DataSource = new BindingSource(listEggGroups, string.Empty);

            growthCurveInputComboBox.Items.AddRange(Enum.GetNames(typeof(PokemonGrowthCurve)));
            
            dexColorInputComboBox.Items.AddRange(Enum.GetNames(typeof(PokemonDexColor)));


            /* ---------------- */
            int count = RomInfo.GetPersonalFilesCount();
            this.pokenames = RomInfo.GetPokemonNames();
            List<string> fileNames = new List<string>(count);
            fileNames.AddRange(pokenames);

            for (int i = 0; i < PokeDatabase.PersonalData.personalExtraFiles.Length; i++) {
                PokeDatabase.PersonalData.PersonalExtraFiles altFormEntry = PokeDatabase.PersonalData.personalExtraFiles[i];
                fileNames.Add(fileNames[altFormEntry.monId] + " - " + altFormEntry.description);
            }

            int extraEntries = fileNames.Count;
            for (int i = 0; i < count - extraEntries; i++) {
                fileNames.Add($"Extra entry {fileNames.Count}");
            }
            
            this.fileNames = fileNames.ToArray();
            monNumberNumericUpDown.Maximum = fileNames.Count - 1;
            pokemonNameInputComboBox.Items.AddRange(this.fileNames);
            hatchResultComboBox.DataSource = fileNames.ToArray();
            /* ---------------- */

            Helpers.EnableHandlers();


            pokemonNameInputComboBox.SelectedIndex = 1;
        }
        private void setDirty(bool status) {
            if (status) {
                dirty = true;
                this.Text = formName + "*";
            } else {
                dirty = false;
                this.Text = formName;
            }
            _parent.UpdateTabPageNames();
        }
        private void baseHpNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.baseHP = (byte)baseHpNumericUpDown.Value;
            setDirty(true);
        }

        private void baseAtkNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.baseAtk = (byte)baseAtkNumericUpDown.Value;
            setDirty(true);
        }
        private void baseDefNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.baseDef = (byte)baseDefNumericUpDown.Value;
            setDirty(true);
        }

        private void baseSpAtkNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.baseSpAtk = (byte)baseSpAtkNumericUpDown.Value;
            setDirty(true);
        }

        private void baseSpDefNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.baseSpDef = (byte)baseSpDefNumericUpDown.Value;
            setDirty(true);
        }

        private void baseSpeedNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.baseSpeed = (byte)baseSpeedNumericUpDown.Value;
            setDirty(true);
        }

        private void evHpNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.evHP = (byte)evHpNumericUpDown.Value;
            setDirty(true);
        }

        private void evAtkNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.evAtk = (byte)evAtkNumericUpDown.Value;
            setDirty(true);
        }

        private void evDefNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.evDef = (byte)evDefNumericUpDown.Value;
            setDirty(true);
        }

        private void evSpAtkNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.evSpAtk = (byte)evSpAtkNumericUpDown.Value;
            setDirty(true);
        }

        private void evSpDefNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.evSpDef = (byte)evSpDefNumericUpDown.Value;
            setDirty(true);
        }

        private void evSpeedNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.evSpeed = (byte)evSpeedNumericUpDown.Value;
            setDirty(true);
        }


        private void type1InputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.type1 = (PokemonType)type1InputComboBox.SelectedIndex;
            setDirty(true);
        }

        private void type2InputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.type2 = (PokemonType)type2InputComboBox.SelectedIndex;
            setDirty(true);
        }

        private void growthCurveInputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.growthCurve = (PokemonGrowthCurve)growthCurveInputComboBox.SelectedIndex;
            setDirty(true);
        }

        private void baseExpYieldNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.givenExp = (byte)baseExpYieldNumericUpDown.Value;
            setDirty(true);
        }

        private void dexColorInputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.color = (PokemonDexColor)dexColorInputComboBox.SelectedIndex;
            setDirty(true);
        }

        private void flipFlagCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.flip = flipFlagCheckBox.Checked;
            setDirty(true);
        }

        private void escapeRateNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.escapeRate = (byte)escapeRateNumericUpDown.Value;
            setDirty(true);
        }

        private void catchRateNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.catchRate = (byte)catchRateNumericUpDown.Value;
            setDirty(true);
        }

        private void genderProbabilityNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.genderVec = (byte)genderProbabilityNumericUpDown.Value;
            genderLabel.Text = GetGenderText(currentLoadedFile.genderVec);

            setDirty(true);
        }

        private string GetGenderText(int vec) {
            switch (vec) {
                case (byte)PokemonGender.Male:
                case (byte)PokemonGender.Female:
                    return $"100% {Enum.GetName(typeof(PokemonGender), vec)}";
                case (byte)PokemonGender.Unknown:
                    return "Gender Unknown";
                default: 
                    {
                        vec++;
                        float femaleProb = 100 * ((float)vec / 256);
                        return $"{100 - femaleProb}% Male\n\n{femaleProb}% Female";
                    }
            }
        }

        private void ability1InputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.firstAbility = (byte)ability1InputComboBox.SelectedIndex;
            setDirty(true);
            modifiedAbilities = true;
        }
        private void ability2InputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.secondAbility = (byte)ability2InputComboBox.SelectedIndex;
            setDirty(true);
            modifiedAbilities = true;
        }
        private void eggGroup1InputCombobox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.eggGroup1 = (byte)eggGroup1InputCombobox.SelectedIndex;
            setDirty(true);
        }

        private void eggGroup2InputCombobox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.eggGroup2 = (byte)eggGroup2InputCombobox.SelectedIndex;
            setDirty(true);
        }

        private void eggStepsNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.eggSteps = (byte)eggStepsNumericUpDown.Value;
            setDirty(true);
        }

        private void item1InputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.item1 = (ushort)item1InputComboBox.SelectedIndex;
            setDirty(true);
        }

        private void item2InputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.item2 = (ushort)item2InputComboBox.SelectedIndex;
            setDirty(true);
        }

        private void baseFriendshipNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (Helpers.HandlersDisabled) {
                return;
            }
            currentLoadedFile.baseFriendship = (byte)baseFriendshipNumericUpDown.Value;
            setDirty(true);
        }


        private void addMachineButton_Click(object sender, EventArgs e) {
            int elemAdd = addableMachinesListBox.SelectedIndex;
            if (elemAdd < 0) {
                return;
            }
            int id = ZeroBasedIndexFromMachineName((string)addableMachinesListBox.SelectedItem);

            currentLoadedFile.machines.Add((byte)id);

            RebuildMachinesListBoxes(false, true);

            int count = addableMachinesListBox.Items.Count;
            if (count > 0) {
                addableMachinesListBox.SelectedIndex = Math.Min(count-1, elemAdd);
            }
            setDirty(true);
        }

        private void removeMachineButton_Click(object sender, EventArgs e) {
            int elemRemove = addedMachinesListBox.SelectedIndex;
            if (elemRemove < 0) {
                return;
            }
            int id = ZeroBasedIndexFromMachineName((string)addedMachinesListBox.SelectedItem);
            currentLoadedFile.machines.Remove((byte)id);

            RebuildMachinesListBoxes(true, false);

            int count = addedMachinesListBox.Items.Count;
            if (count > 0) {
                addedMachinesListBox.SelectedIndex = Math.Max(0, elemRemove - 1);
            }
            setDirty(true);
        }

        private void addAllMachinesButton_Click(object sender, EventArgs e) {
            int tot = PokemonPersonalData.tmsCount + PokemonPersonalData.hmsCount;
            if (currentLoadedFile.machines.Count == tot) {
                return;
            }

            currentLoadedFile.machines = new SortedSet<byte>();
            for (byte i = 0; i < tot; i++) {
                currentLoadedFile.machines.Add(i);
            }
            RebuildMachinesListBoxes();
            setDirty(true);
        }

        private void removeAllMachinesButton_Click(object sender, EventArgs e) {
            if (currentLoadedFile.machines.Count == 0) {
                return;
            }
            currentLoadedFile.machines.Clear();
            RebuildMachinesListBoxes();
            setDirty(true);
        }
        private void saveDataButton_Click(object sender, EventArgs e) {
            currentLoadedFile.SaveToFileDefaultDir(currentLoadedId, true);
            WriteHatchResult(currentLoadedId);
            //if (modifiedAbilities) {
            //    EditorPanels.MainProgram.RefreshAbilities(currentLoadedId);
            //    modifiedAbilities = false;
            //}
            setDirty(false);
        }
        //-------------------------------
        public bool CheckDiscardChanges() {
            if (!dirty) {
                return true;
            }

            DialogResult res = MessageBox.Show("Personal Editor\nThere are unsaved changes to the current Personal data.\nDiscard and proceed?", "Personal Editor - Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res.Equals(DialogResult.Yes)) {
                return true;
            }

            monNumberNumericUpDown.Value = currentLoadedId;
            pokemonNameInputComboBox.SelectedIndex = currentLoadedId;


            return false;
        }

        private void pokemonNameInputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            Update();
            if (Helpers.HandlersDisabled) {
                return;
            }
            this._parent.TrySyncIndices((System.Windows.Forms.ComboBox)sender);
            Helpers.DisableHandlers();
            if (CheckDiscardChanges()) {
                int newNumber = pokemonNameInputComboBox.SelectedIndex;
                monNumberNumericUpDown.Value = newNumber;
                ChangeLoadedFile(newNumber);
            }
            Helpers.EnableHandlers();
        }

        private void monNumberNumericUpDown_ValueChanged(object sender, EventArgs e) {
            Update();
            if (Helpers.HandlersDisabled) {
                return;
            }
            this._parent.TrySyncIndices((NumericUpDown)sender);
            Helpers.DisableHandlers();
            if (CheckDiscardChanges()) {
                int newNumber = (int)monNumberNumericUpDown.Value;
                pokemonNameInputComboBox.SelectedIndex = newNumber;
                ChangeLoadedFile(newNumber);
            }
            Helpers.EnableHandlers();
        }


        private void hatchResultComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (Helpers.HandlersDisabled)
            {
                return;
            }

            setDirty(true);
        }

        public void ChangeLoadedFile(int toLoad) {
            currentLoadedId = toLoad;
            currentLoadedFile = new PokemonPersonalData(currentLoadedId);

            baseHpNumericUpDown.Value = currentLoadedFile.baseHP;
            baseAtkNumericUpDown.Value = currentLoadedFile.baseAtk;
            baseDefNumericUpDown.Value = currentLoadedFile.baseDef;
            baseSpeedNumericUpDown.Value = currentLoadedFile.baseSpeed;
            baseSpAtkNumericUpDown.Value = currentLoadedFile.baseSpAtk;
            baseSpDefNumericUpDown.Value = currentLoadedFile.baseSpDef;

            type1InputComboBox.SelectedIndex = (byte)currentLoadedFile.type1;
            type2InputComboBox.SelectedIndex = (byte)currentLoadedFile.type2;

            catchRateNumericUpDown.Value = currentLoadedFile.catchRate;
            baseExpYieldNumericUpDown.Value = currentLoadedFile.givenExp;

            evHpNumericUpDown.Value = currentLoadedFile.evHP;
            evAtkNumericUpDown.Value = currentLoadedFile.evAtk;
            evDefNumericUpDown.Value = currentLoadedFile.evDef;
            evSpeedNumericUpDown.Value = currentLoadedFile.evSpeed;
            evSpAtkNumericUpDown.Value = currentLoadedFile.evSpAtk;
            evSpDefNumericUpDown.Value = currentLoadedFile.evSpDef;

            item1InputComboBox.SelectedIndex = currentLoadedFile.item1;
            item2InputComboBox.SelectedIndex = currentLoadedFile.item2;

            genderProbabilityNumericUpDown.Value = currentLoadedFile.genderVec;
            eggStepsNumericUpDown.Value = currentLoadedFile.eggSteps;
            baseFriendshipNumericUpDown.Value = currentLoadedFile.baseFriendship;
            growthCurveInputComboBox.SelectedIndex = (byte)currentLoadedFile.growthCurve;

            eggGroup1InputCombobox.SelectedIndex = currentLoadedFile.eggGroup1;
            eggGroup2InputCombobox.SelectedIndex = currentLoadedFile.eggGroup2;
            hatchResultComboBox.SelectedIndex = GetHatchResult(currentLoadedId);

            ability1InputComboBox.SelectedIndex = currentLoadedFile.firstAbility;
            ability2InputComboBox.SelectedIndex = currentLoadedFile.secondAbility;
            escapeRateNumericUpDown.Value = currentLoadedFile.escapeRate;

            dexColorInputComboBox.SelectedIndex = (byte)currentLoadedFile.color;
            flipFlagCheckBox.Checked = currentLoadedFile.flip;

            genderLabel.Text = GetGenderText(currentLoadedFile.genderVec);
            RebuildMachinesListBoxes();

            int excess = toLoad - pokenames.Length;
            try {
                if (excess >= 0) {
                    toLoad = PokeDatabase.PersonalData.personalExtraFiles[excess].iconId;
                }
            } catch (IndexOutOfRangeException) {
                toLoad = 0;
            } finally {
                pokemonPictureBox.Image = DSUtils.GetPokePic(toLoad, pokemonPictureBox.Width, pokemonPictureBox.Height);
            }
            setDirty(false);
        }

        private void RebuildMachinesListBoxes(bool keepAddableSelection = true, bool keepAddedSelection = true) {
            addableMachinesListBox.BeginUpdate();
            addedMachinesListBox.BeginUpdate();

            string addableSel = null;
            if (keepAddableSelection) {
                addableSel = (string)addableMachinesListBox.SelectedItem;
            }
            string addedSel = null;
            if (keepAddedSelection) {
                addedSel = (string)addableMachinesListBox.SelectedItem;
            }

            addedMachinesListBox.Items.Clear();
            addableMachinesListBox.Items.Clear();

            int dataIndex = 0;
            byte tot = (byte)(PokemonPersonalData.tmsCount + PokemonPersonalData.hmsCount);
            for (byte i = 0; i < tot; i++) {
                string currentItem = MachineNameFromZeroBasedIndex(i);
                if (dataIndex < currentLoadedFile.machines.Count && currentLoadedFile.machines.Contains(i)) {
                    addedMachinesListBox.Items.Add(currentItem);
                    dataIndex++;
                } else {
                    addableMachinesListBox.Items.Add(currentItem);
                }
            }

            addableMachinesListBox.EndUpdate();
            addedMachinesListBox.EndUpdate();

            if (keepAddableSelection) { 
                int addableCount = addableMachinesListBox.Items.Count;
                if (addableCount > 0) {
                    addableMachinesListBox.SelectedItem = addableSel;
                }
            }

            int addedCount = addedMachinesListBox.Items.Count;
            if (addedCount > 0) {
                addedMachinesListBox.SelectedItem = addedSel;
            }
        }

        private int GetHatchResult(int monID)
        {
            if (monID < 0) {
                return 0;
            }

            // Open PMS file to find the hatch result
            // This isn't a narc despite the name, it's a binary file. It's also in the same location for all games and languages.
            FileStream stream = new FileStream(Path.Combine(RomInfo.dataPath, @"poketool/personal/pms.narc"), FileMode.Open);

            using (BinaryReader reader = new BinaryReader(stream)) 
            {
                // Each entry is 2 bytes long
                int offset = monID * 2;
                if (offset + 1 > stream.Length) {
                    return 0; // Out of bounds
                }
                stream.Seek(offset, SeekOrigin.Begin);
                ushort hatchResult = reader.ReadUInt16();
                stream.Close();
                return hatchResult;
            }
        }

        private void WriteHatchResult(int monID) {
            if (monID < 0) {
                return;
            }
            // Open PMS file to write the hatch result
            FileStream stream = new FileStream(Path.Combine(RomInfo.dataPath, @"poketool/personal/pms.narc"), FileMode.Open);
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Each entry is 2 bytes long
                int offset = monID * 2;
                if (offset + 1 > stream.Length)
                {
                    return; // Out of bounds
                }
                stream.Seek(offset, SeekOrigin.Begin);
                writer.Write((ushort)hatchResultComboBox.SelectedIndex);
                stream.Close();
            }
        }

        private static string MachineNameFromZeroBasedIndex(int n)
        {
            //0-91 --> TMs (TM001-TM092)
            //>=92 --> HMs (HM01-HM08)

            string moveName = "Unknown";

            // Get the move name from the machine moves array
            if (n < MachineMoves.Length)
            {
                int moveId = MachineMoves[n];
                if (ScriptDatabase.moveNames.ContainsKey((ushort)moveId))
                {
                    string moveKey = ScriptDatabase.moveNames[(ushort)moveId];
                    // Convert MOVE_LEECH_SEED to Leech Seed
                    moveName = MoveKeyToHumanReadable(moveKey);
                }
            }

            n += 1;
            if (n <= PokemonPersonalData.tmsCount)
            {
                // TM
                return $"TM {n:D3} - {moveName}";
            }
            else
            {
                // HM
                int hmNumber = n - PokemonPersonalData.tmsCount;
                return $"HM {hmNumber:D2} - {moveName}";
            }
        }

        private static string MoveKeyToHumanReadable(string moveKey)
        {
            if (string.IsNullOrEmpty(moveKey))
                return "Unknown";

            // Remove "MOVE_" prefix if present
            if (moveKey.StartsWith("MOVE_"))
            {
                moveKey = moveKey.Substring(5);
            }

            // Split by underscores and capitalize each word
            string[] words = moveKey.Split('_');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    // Capitalize first letter, lowercase the rest
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }

            return string.Join(" ", words);
        }

        private static int ZeroBasedIndexFromMachineName(string machineName)
        {
            // Split the machineName to get the prefix (TM or HM) and the number
            // Format: "TM001 - Focus Punch" or "HM01 - Cut"
            Console.WriteLine($"{machineName}");
            string[] fullNameBroken = machineName.Split('-');
            string[] parts = fullNameBroken[0].Split(' ');
            Console.WriteLine(parts[0]);
            if (parts.Length >= 2)
            {
                // Check if the first part is "TM" or "HM" (case-insensitive)
                bool isTM = parts[0].Equals("TM", StringComparison.OrdinalIgnoreCase);
                bool isHM = parts[0].Equals("HM", StringComparison.OrdinalIgnoreCase);

                if (isTM || isHM)
                {
                    // Extract just the number part (remove any leading zeros if needed)
                    string numberPart = parts[1];
                    if (int.TryParse(numberPart, out int number))
                    {
                        number--; // Convert to zero-based

                        // Calculate the index based on the prefix (TM or HM)
                        int index = isTM ? number : number + PokemonPersonalData.tmsCount;
                        return index;
                    }
                }
            }

            // Return -1 to indicate an invalid input or failure to parse
            return -1;
        }

        private static readonly int[] MachineMoves = new int[]
        {
            // TMs 001-092
            264, // MOVE_FOCUS_PUNCH - TM001
            337, // MOVE_DRAGON_CLAW - TM002
            352, // MOVE_WATER_PULSE - TM003
            347, // MOVE_CALM_MIND - TM004
            46,  // MOVE_ROAR - TM005
            92,  // MOVE_TOXIC - TM006
            258, // MOVE_HAIL - TM007
            339, // MOVE_BULK_UP - TM008
            331, // MOVE_BULLET_SEED - TM009
            237, // MOVE_HIDDEN_POWER - TM010
            241, // MOVE_SUNNY_DAY - TM011
            269, // MOVE_TAUNT - TM012
            58,  // MOVE_ICE_BEAM - TM013
            59,  // MOVE_BLIZZARD - TM014
            63,  // MOVE_HYPER_BEAM - TM015
            113, // MOVE_LIGHT_SCREEN - TM016
            182, // MOVE_PROTECT - TM017
            240, // MOVE_RAIN_DANCE - TM018
            202, // MOVE_GIGA_DRAIN - TM019
            219, // MOVE_SAFEGUARD - TM020
            218, // MOVE_FRUSTRATION - TM021
            76,  // MOVE_SOLAR_BEAM - TM022
            231, // MOVE_IRON_TAIL - TM023
            85,  // MOVE_THUNDERBOLT - TM024
            87,  // MOVE_THUNDER - TM025
            89,  // MOVE_EARTHQUAKE - TM026
            216, // MOVE_RETURN - TM027
            91,  // MOVE_DIG - TM028
            94,  // MOVE_PSYCHIC - TM029
            247, // MOVE_SHADOW_BALL - TM030
            280, // MOVE_BRICK_BREAK - TM031
            104, // MOVE_DOUBLE_TEAM - TM032
            115, // MOVE_REFLECT - TM033
            351, // MOVE_SHOCK_WAVE - TM034
            53,  // MOVE_FLAMETHROWER - TM035
            188, // MOVE_SLUDGE_BOMB - TM036
            201, // MOVE_SANDSTORM - TM037
            126, // MOVE_FIRE_BLAST - TM038
            317, // MOVE_ROCK_TOMB - TM039
            332, // MOVE_AERIAL_ACE - TM040
            259, // MOVE_TORMENT - TM041
            263, // MOVE_FACADE - TM042
            290, // MOVE_SECRET_POWER - TM043
            156, // MOVE_REST - TM044
            213, // MOVE_ATTRACT - TM045
            168, // MOVE_THIEF - TM046
            211, // MOVE_STEEL_WING - TM047
            285, // MOVE_SKILL_SWAP - TM048
            289, // MOVE_SNATCH - TM049
            315, // MOVE_OVERHEAT - TM050
            355, // MOVE_ROOST - TM051
            411, // MOVE_FOCUS_BLAST - TM052
            412, // MOVE_ENERGY_BALL - TM053
            206, // MOVE_FALSE_SWIPE - TM054
            362, // MOVE_BRINE - TM055
            374, // MOVE_FLING - TM056
            451, // MOVE_CHARGE_BEAM - TM057
            203, // MOVE_ENDURE - TM058
            406, // MOVE_DRAGON_PULSE - TM059
            409, // MOVE_DRAIN_PUNCH - TM060
            261, // MOVE_WILL_O_WISP - TM061
            318, // MOVE_SILVER_WIND - TM062
            373, // MOVE_EMBARGO - TM063
            153, // MOVE_EXPLOSION - TM064
            421, // MOVE_SHADOW_CLAW - TM065
            371, // MOVE_PAYBACK - TM066
            278, // MOVE_RECYCLE - TM067
            416, // MOVE_GIGA_IMPACT - TM068
            397, // MOVE_ROCK_POLISH - TM069
            148, // MOVE_FLASH - TM070
            444, // MOVE_STONE_EDGE - TM071
            419, // MOVE_AVALANCHE - TM072
            86,  // MOVE_THUNDER_WAVE - TM073
            360, // MOVE_GYRO_BALL - TM074
            14,  // MOVE_SWORDS_DANCE - TM075
            446, // MOVE_STEALTH_ROCK - TM076
            244, // MOVE_PSYCH_UP - TM077
            445, // MOVE_CAPTIVATE - TM078
            399, // MOVE_DARK_PULSE - TM079
            157, // MOVE_ROCK_SLIDE - TM080
            404, // MOVE_X_SCISSOR - TM081
            214, // MOVE_SLEEP_TALK - TM082
            363, // MOVE_NATURAL_GIFT - TM083
            398, // MOVE_POISON_JAB - TM084
            138, // MOVE_DREAM_EATER - TM085
            447, // MOVE_GRASS_KNOT - TM086
            207, // MOVE_SWAGGER - TM087
            365, // MOVE_PLUCK - TM088
            369, // MOVE_U_TURN - TM089
            164, // MOVE_SUBSTITUTE - TM090
            430, // MOVE_FLASH_CANNON - TM091
            433, // MOVE_TRICK_ROOM - TM092

            // HMs 01-08
            15,  // MOVE_CUT - HM01
            19,  // MOVE_FLY - HM02
            57,  // MOVE_SURF - HM03
            70,  // MOVE_STRENGTH - HM04
            250, // MOVE_WHIRLPOOL - HM05
            249, // MOVE_ROCK_SMASH - HM06
            127, // MOVE_WATERFALL - HM07
            431, // MOVE_ROCK_CLIMB - HM08
        };
    }
}
