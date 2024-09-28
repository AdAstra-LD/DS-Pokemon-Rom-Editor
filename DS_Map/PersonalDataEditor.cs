using DSPRE.Resources;
using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private static bool dirty = false;
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

            BindingList<string> listItemNames = new BindingList<string>(itemNames);
            item1InputComboBox.DataSource = new BindingSource(listItemNames, string.Empty);
            item2InputComboBox.DataSource = new BindingSource(listItemNames, string.Empty);

            BindingList<string> listTypeNames = new BindingList<string>(Enum.GetNames(typeof(PokemonType)));
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

            for (int i = 0; i < count - pokenames.Length; i++) {
                PokeDatabase.PersonalData.PersonalExtraFiles extraEntry = PokeDatabase.PersonalData.personalExtraFiles[i];
                fileNames.Add(fileNames[extraEntry.monId] + " - " + extraEntry.description);
            }

            
            this.fileNames = fileNames.ToArray();
            monNumberNumericUpDown.Maximum = fileNames.Count - 1;
            pokemonNameInputComboBox.Items.AddRange(this.fileNames);
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
            if (modifiedAbilities) {
                EditorPanels.MainProgram.RefreshAbilities(currentLoadedId);
                modifiedAbilities = false;
            }
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

            ability1InputComboBox.SelectedIndex = currentLoadedFile.firstAbility;
            ability2InputComboBox.SelectedIndex = currentLoadedFile.secondAbility;
            escapeRateNumericUpDown.Value = currentLoadedFile.escapeRate;

            dexColorInputComboBox.SelectedIndex = (byte)currentLoadedFile.color;
            flipFlagCheckBox.Checked = currentLoadedFile.flip;

            genderLabel.Text = GetGenderText(currentLoadedFile.genderVec);
            RebuildMachinesListBoxes();

            int excess = toLoad - pokenames.Length;
            if (excess >= 0) {
                toLoad = PokeDatabase.PersonalData.personalExtraFiles[excess].iconId;
            }
            pokemonPictureBox.Image = DSUtils.GetPokePic(toLoad, pokemonPictureBox.Width, pokemonPictureBox.Height);

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

        private static string MachineNameFromZeroBasedIndex(int n) {
            //0-91 --> TMs
            //>=92 --> HM
            n += 1;
            int diff = n - PokemonPersonalData.tmsCount;
            string item = diff > 0 ? "HM " + diff : "TM " + n;
            return item;
        }
        private static int ZeroBasedIndexFromMachineName(string machineName) {
            // Split the machineName to get the prefix (TM or HM) and the number
            string[] parts = machineName.Split(' ');

            if (parts.Length == 2) {
                // Check if the first part is "TM" (case-insensitive)
                bool isTM = parts[0].Equals("TM", StringComparison.OrdinalIgnoreCase);

                if (isTM || parts[0].Equals("HM", StringComparison.OrdinalIgnoreCase)) {
                    if (int.TryParse(parts[1], out int number)) {
                        number--;
                        // Calculate the index based on the prefix (TM or HM)
                        int index = isTM ? number : number + PokemonPersonalData.tmsCount;
                        return index;
                    }
                }
            }

            // Return -1 to indicate an invalid input or failure to parse
            return -1;
        }
    }
}
