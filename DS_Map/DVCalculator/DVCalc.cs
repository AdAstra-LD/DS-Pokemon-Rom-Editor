using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DSPRE
{
    public partial class DVCalc : Form
    {
        public DVCalc()
        {
            InitializeComponent();
            PopulateComboBox();

            //make Pokemon searchable
            pokemonSelector.TextChanged += PokemonSelector_TextChanged;
            pokemonSelector.AutoCompleteMode = AutoCompleteMode.Suggest;
            pokemonSelector.AutoCompleteSource = AutoCompleteSource.CustomSource;

            AutoCompleteStringCollection autoCompleteSource = new AutoCompleteStringCollection();
            foreach (KeyValuePair<int, string> item in pokemonSelector.Items)
            {
                autoCompleteSource.Add(item.Value);
            }

            pokemonSelector.AutoCompleteCustomSource = autoCompleteSource;

            Version version = new Version(major: 1, minor: 1);

            this.Text = "DVCalc " + version.ToString();

        }

        public DVCalc(uint TrainerIndex, uint TrainerClassIndex) : this()
        {
            trainerIdx.Value = TrainerIndex;
            trainerClassIdx.Value = TrainerClassIndex;
        }

        private void InitializeComponent()
        {
            poke_label = new Label();
            trainerClassIdx_label = new Label();
            trainerIdx_label = new Label();
            pokeLVL_label = new Label();
            pokeLevel = new NumericUpDown();
            trainerClassIdx = new NumericUpDown();
            trainerIdx = new NumericUpDown();
            natureSelect = new ComboBox();
            nature_label = new Label();
            DV_label = new Label();
            calcButton = new Button();
            maleCheck = new CheckBox();
            maxDVNature_label = new Label();
            IV_label = new Label();
            pokemonSelector = new ComboBox();
            showAllButton = new Button();
            radioButtonMale = new RadioButton();
            radioButtonFemale = new RadioButton();
            radioButtonAbility1 = new RadioButton();
            radioButtonAbility2 = new RadioButton();
            numericUpDownGender = new NumericUpDown();
            groupBoxGender = new GroupBox();
            radioButtonIngoreGender = new RadioButton();
            groupBoxAbility = new GroupBox();
            radioButtonIgnoreAbility = new RadioButton();
            labelGenderRatio = new Label();
            groupBoxHGSS = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)pokeLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trainerClassIdx).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trainerIdx).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownGender).BeginInit();
            groupBoxGender.SuspendLayout();
            groupBoxAbility.SuspendLayout();
            groupBoxHGSS.SuspendLayout();
            SuspendLayout();
            // 
            // poke_label
            // 
            poke_label.AutoSize = true;
            poke_label.Location = new Point(22, 78);
            poke_label.Name = "poke_label";
            poke_label.Size = new Size(58, 15);
            poke_label.TabIndex = 3;
            poke_label.Text = "Pokémon";
            // 
            // trainerClassIdx_label
            // 
            trainerClassIdx_label.AutoSize = true;
            trainerClassIdx_label.Location = new Point(172, 22);
            trainerClassIdx_label.Name = "trainerClassIdx_label";
            trainerClassIdx_label.Size = new Size(104, 15);
            trainerClassIdx_label.TabIndex = 4;
            trainerClassIdx_label.Text = "Trainer Class Index";
            // 
            // trainerIdx_label
            // 
            trainerIdx_label.AutoSize = true;
            trainerIdx_label.Location = new Point(28, 21);
            trainerIdx_label.Name = "trainerIdx_label";
            trainerIdx_label.Size = new Size(74, 15);
            trainerIdx_label.TabIndex = 5;
            trainerIdx_label.Text = "Trainer Index";
            // 
            // pokeLVL_label
            // 
            pokeLVL_label.AutoSize = true;
            pokeLVL_label.Location = new Point(315, 102);
            pokeLVL_label.Name = "pokeLVL_label";
            pokeLVL_label.Size = new Size(28, 15);
            pokeLVL_label.TabIndex = 6;
            pokeLVL_label.Text = "LVL.";
            // 
            // pokeLevel
            // 
            pokeLevel.Location = new Point(349, 100);
            pokeLevel.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            pokeLevel.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            pokeLevel.Name = "pokeLevel";
            pokeLevel.Size = new Size(46, 23);
            pokeLevel.TabIndex = 7;
            pokeLevel.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // trainerClassIdx
            // 
            trainerClassIdx.Location = new Point(172, 45);
            trainerClassIdx.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            trainerClassIdx.Name = "trainerClassIdx";
            trainerClassIdx.Size = new Size(125, 23);
            trainerClassIdx.TabIndex = 9;
            // 
            // trainerIdx
            // 
            trainerIdx.Location = new Point(28, 44);
            trainerIdx.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            trainerIdx.Name = "trainerIdx";
            trainerIdx.Size = new Size(125, 23);
            trainerIdx.TabIndex = 10;
            // 
            // natureSelect
            // 
            natureSelect.FormattingEnabled = true;
            natureSelect.Location = new Point(433, 44);
            natureSelect.Name = "natureSelect";
            natureSelect.Size = new Size(227, 23);
            natureSelect.TabIndex = 11;
            // 
            // nature_label
            // 
            nature_label.AutoSize = true;
            nature_label.Location = new Point(433, 21);
            nature_label.Name = "nature_label";
            nature_label.Size = new Size(43, 15);
            nature_label.TabIndex = 12;
            nature_label.Text = "Nature";
            // 
            // DV_label
            // 
            DV_label.AutoSize = true;
            DV_label.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DV_label.Location = new Point(316, 139);
            DV_label.Name = "DV_label";
            DV_label.Size = new Size(130, 21);
            DV_label.TabIndex = 13;
            DV_label.Text = "Difficulty Value: 0";
            // 
            // calcButton
            // 
            calcButton.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            calcButton.Location = new Point(546, 292);
            calcButton.Name = "calcButton";
            calcButton.Size = new Size(114, 51);
            calcButton.TabIndex = 14;
            calcButton.Text = "Calculate";
            calcButton.UseVisualStyleBackColor = true;
            calcButton.Click += CalcButton_Click;
            // 
            // maleCheck
            // 
            maleCheck.AutoSize = true;
            maleCheck.Checked = true;
            maleCheck.CheckState = CheckState.Checked;
            maleCheck.Location = new Point(303, 46);
            maleCheck.Name = "maleCheck";
            maleCheck.Size = new Size(95, 19);
            maleCheck.TabIndex = 15;
            maleCheck.Text = "Trainer Male?";
            maleCheck.UseVisualStyleBackColor = true;
            // 
            // maxDVNature_label
            // 
            maxDVNature_label.AutoSize = true;
            maxDVNature_label.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            maxDVNature_label.Location = new Point(316, 170);
            maxDVNature_label.Name = "maxDVNature_label";
            maxDVNature_label.Size = new Size(109, 20);
            maxDVNature_label.TabIndex = 16;
            maxDVNature_label.Text = "DV 255 Nature:";
            // 
            // IV_label
            // 
            IV_label.AutoSize = true;
            IV_label.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            IV_label.Location = new Point(315, 198);
            IV_label.Name = "IV_label";
            IV_label.Size = new Size(96, 20);
            IV_label.TabIndex = 17;
            IV_label.Text = "Resulting IVs:";
            // 
            // pokemonSelector
            // 
            pokemonSelector.FormattingEnabled = true;
            pokemonSelector.Location = new Point(22, 99);
            pokemonSelector.Name = "pokemonSelector";
            pokemonSelector.Size = new Size(287, 23);
            pokemonSelector.TabIndex = 18;
            // 
            // showAllButton
            // 
            showAllButton.Location = new Point(566, 88);
            showAllButton.Name = "showAllButton";
            showAllButton.Size = new Size(94, 29);
            showAllButton.TabIndex = 19;
            showAllButton.Text = "Show All";
            showAllButton.UseVisualStyleBackColor = true;
            showAllButton.Click += ShowAllButton_Click;
            // 
            // radioButtonMale
            // 
            radioButtonMale.AutoSize = true;
            radioButtonMale.Location = new Point(6, 47);
            radioButtonMale.Name = "radioButtonMale";
            radioButtonMale.Size = new Size(83, 19);
            radioButtonMale.TabIndex = 20;
            radioButtonMale.TabStop = true;
            radioButtonMale.Text = "Force Male";
            radioButtonMale.TextAlign = ContentAlignment.TopCenter;
            radioButtonMale.UseVisualStyleBackColor = true;
            // 
            // radioButtonFemale
            // 
            radioButtonFemale.AutoSize = true;
            radioButtonFemale.Location = new Point(6, 72);
            radioButtonFemale.Name = "radioButtonFemale";
            radioButtonFemale.Size = new Size(95, 19);
            radioButtonFemale.TabIndex = 21;
            radioButtonFemale.TabStop = true;
            radioButtonFemale.Text = "Force Female";
            radioButtonFemale.UseVisualStyleBackColor = true;
            // 
            // radioButtonAbility1
            // 
            radioButtonAbility1.AutoSize = true;
            radioButtonAbility1.Location = new Point(6, 47);
            radioButtonAbility1.Name = "radioButtonAbility1";
            radioButtonAbility1.Size = new Size(100, 19);
            radioButtonAbility1.TabIndex = 22;
            radioButtonAbility1.TabStop = true;
            radioButtonAbility1.Text = "Force Ability 1";
            radioButtonAbility1.UseVisualStyleBackColor = true;
            // 
            // radioButtonAbility2
            // 
            radioButtonAbility2.AutoSize = true;
            radioButtonAbility2.Location = new Point(6, 72);
            radioButtonAbility2.Name = "radioButtonAbility2";
            radioButtonAbility2.Size = new Size(100, 19);
            radioButtonAbility2.TabIndex = 23;
            radioButtonAbility2.TabStop = true;
            radioButtonAbility2.Text = "Force Ability 2";
            radioButtonAbility2.UseVisualStyleBackColor = true;
            // 
            // numericUpDownGender
            // 
            numericUpDownGender.Enabled = false;
            numericUpDownGender.Location = new Point(6, 170);
            numericUpDownGender.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numericUpDownGender.Name = "numericUpDownGender";
            numericUpDownGender.Size = new Size(120, 23);
            numericUpDownGender.TabIndex = 24;
            // 
            // groupBoxGender
            // 
            groupBoxGender.Controls.Add(radioButtonIngoreGender);
            groupBoxGender.Controls.Add(radioButtonMale);
            groupBoxGender.Controls.Add(radioButtonFemale);
            groupBoxGender.Location = new Point(11, 22);
            groupBoxGender.Name = "groupBoxGender";
            groupBoxGender.Size = new Size(132, 112);
            groupBoxGender.TabIndex = 25;
            groupBoxGender.TabStop = false;
            groupBoxGender.Text = "Pokémon Gender";
            // 
            // radioButtonIngoreGender
            // 
            radioButtonIngoreGender.AutoSize = true;
            radioButtonIngoreGender.Checked = true;
            radioButtonIngoreGender.Location = new Point(6, 22);
            radioButtonIngoreGender.Name = "radioButtonIngoreGender";
            radioButtonIngoreGender.Size = new Size(66, 19);
            radioButtonIngoreGender.TabIndex = 22;
            radioButtonIngoreGender.TabStop = true;
            radioButtonIngoreGender.Text = "No Flag";
            radioButtonIngoreGender.TextAlign = ContentAlignment.TopCenter;
            radioButtonIngoreGender.UseVisualStyleBackColor = true;
            radioButtonIngoreGender.CheckedChanged += radioButtonNoFlag_CheckedChanged;
            // 
            // groupBoxAbility
            // 
            groupBoxAbility.Controls.Add(radioButtonIgnoreAbility);
            groupBoxAbility.Controls.Add(radioButtonAbility1);
            groupBoxAbility.Controls.Add(radioButtonAbility2);
            groupBoxAbility.Location = new Point(143, 22);
            groupBoxAbility.Name = "groupBoxAbility";
            groupBoxAbility.Size = new Size(138, 112);
            groupBoxAbility.TabIndex = 26;
            groupBoxAbility.TabStop = false;
            groupBoxAbility.Text = "Pokémon Ability";
            // 
            // radioButtonIgnoreAbility
            // 
            radioButtonIgnoreAbility.AutoSize = true;
            radioButtonIgnoreAbility.Checked = true;
            radioButtonIgnoreAbility.Location = new Point(6, 22);
            radioButtonIgnoreAbility.Name = "radioButtonIgnoreAbility";
            radioButtonIgnoreAbility.Size = new Size(66, 19);
            radioButtonIgnoreAbility.TabIndex = 24;
            radioButtonIgnoreAbility.TabStop = true;
            radioButtonIgnoreAbility.Text = "No Flag";
            radioButtonIgnoreAbility.UseVisualStyleBackColor = true;
            radioButtonIgnoreAbility.CheckedChanged += radioButtonNoFlag_CheckedChanged;
            // 
            // labelGenderRatio
            // 
            labelGenderRatio.AutoSize = true;
            labelGenderRatio.Location = new Point(6, 148);
            labelGenderRatio.Name = "labelGenderRatio";
            labelGenderRatio.Size = new Size(129, 15);
            labelGenderRatio.TabIndex = 27;
            labelGenderRatio.Text = "Pokémon Gender Ratio";
            // 
            // groupBoxHGSS
            // 
            groupBoxHGSS.Controls.Add(groupBoxGender);
            groupBoxHGSS.Controls.Add(labelGenderRatio);
            groupBoxHGSS.Controls.Add(numericUpDownGender);
            groupBoxHGSS.Controls.Add(groupBoxAbility);
            groupBoxHGSS.Location = new Point(22, 139);
            groupBoxHGSS.Name = "groupBoxHGSS";
            groupBoxHGSS.Size = new Size(287, 204);
            groupBoxHGSS.TabIndex = 28;
            groupBoxHGSS.TabStop = false;
            groupBoxHGSS.Text = "HGSS Only";
            // 
            // MainForm
            // 
            ClientSize = new Size(664, 347);
            Controls.Add(groupBoxHGSS);
            Controls.Add(showAllButton);
            Controls.Add(pokemonSelector);
            Controls.Add(IV_label);
            Controls.Add(maxDVNature_label);
            Controls.Add(maleCheck);
            Controls.Add(calcButton);
            Controls.Add(DV_label);
            Controls.Add(nature_label);
            Controls.Add(natureSelect);
            Controls.Add(trainerIdx);
            Controls.Add(trainerClassIdx);
            Controls.Add(pokeLevel);
            Controls.Add(pokeLVL_label);
            Controls.Add(trainerIdx_label);
            Controls.Add(trainerClassIdx_label);
            Controls.Add(poke_label);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DVCalc";
            ((System.ComponentModel.ISupportInitialize)pokeLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)trainerClassIdx).EndInit();
            ((System.ComponentModel.ISupportInitialize)trainerIdx).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownGender).EndInit();
            groupBoxGender.ResumeLayout(false);
            groupBoxGender.PerformLayout();
            groupBoxAbility.ResumeLayout(false);
            groupBoxAbility.PerformLayout();
            groupBoxHGSS.ResumeLayout(false);
            groupBoxHGSS.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private void PopulateComboBox()
        {
            // Populate Nature ComboBox (static readonly List)
            natureSelect.DataSource = DVCalculator.Natures;

            // Fill the pokemon selector combo box with pokemon names from the ROM
            string[] pokeNames = RomInfo.GetPokemonNames();

            pokemonSelector.Items.Clear();

            for (int id = 0; id < pokeNames.Length; id++)
            {
                pokemonSelector.Items.Add(new KeyValuePair<int, string>(id, pokeNames[id]));
            }

            pokemonSelector.SelectedIndex = 0;

        }

        private void CalcButton_Click(object sender, EventArgs e)
        {

            uint nature = (uint)natureSelect.SelectedIndex;

            uint pokemonIndex = 1;

            if (pokemonSelector.SelectedItem != null)
            {
                KeyValuePair<int, string> selectedPokemon = (KeyValuePair<int, string>)pokemonSelector.SelectedItem;

                pokemonIndex = (uint)selectedPokemon.Key;
            }

            int genderOverride = 0;
            int abilityOverride = 0;

            if (radioButtonMale.Checked) { genderOverride = 1; }
            else if (radioButtonFemale.Checked) { genderOverride = 2; }

            if (radioButtonAbility1.Checked) { abilityOverride = 1; }
            else if (radioButtonAbility2.Checked) { abilityOverride = 2; }

            int DV = DVCalculator.findHighestDV(
                (uint)trainerIdx.Value, (uint)trainerClassIdx.Value, maleCheck.Checked, pokemonIndex,
                (byte)pokeLevel.Value, (byte)numericUpDownGender.Value, genderOverride, abilityOverride, nature);

            uint maxDVNature = DVCalculator.getNatureFromPID(DVCalculator.generatePID(

                (uint)trainerIdx.Value, (uint)trainerClassIdx.Value, maleCheck.Checked, pokemonIndex,
                (byte)pokeLevel.Value, (byte)numericUpDownGender.Value, genderOverride, abilityOverride, 255));

            DV_label.Text = "Difficulty Value: " + DV;


            IV_label.Text = "Resulting IVs: " + (DV * 31 / 255);
            maxDVNature_label.Text = "DV 255 Nature: " + DVCalculator.Natures[(int)maxDVNature];


        }

        private void ShowAllButton_Click(object sender, EventArgs e)
        {
            uint pokemonIndex = 1;

            if (pokemonSelector.SelectedItem != null)
            {
                KeyValuePair<int, string> selectedPokemon = (KeyValuePair<int, string>)pokemonSelector.SelectedItem;

                pokemonIndex = (uint)selectedPokemon.Key;

            }

            int genderOverride = 0;
            int abilityOverride = 0;

            if (radioButtonMale.Checked) { genderOverride = 1; }
            else if (radioButtonFemale.Checked) { genderOverride = 2; }

            if (radioButtonAbility1.Checked) { abilityOverride = 1; }
            else if (radioButtonAbility2.Checked) { abilityOverride = 2; }

            // Create a list of DV-IV-Nature Triplets
            List<DVIVNatureTriplet> natureDict = DVCalculator.getAllNatures(
                (uint)trainerIdx.Value, (uint)trainerClassIdx.Value, maleCheck.Checked, pokemonIndex,
                (byte)pokeLevel.Value, (byte)numericUpDownGender.Value, genderOverride, abilityOverride);

            // Create an instance of the view form and pass the data
            // There might be a better way to do this?
            DVCalcNatureViewerForm natureViewer = new DVCalcNatureViewerForm(natureDict);
            natureViewer.ShowDialog();
        }

        private void PokemonSelector_TextChanged(object sender, EventArgs e)
        {
            if (sender == null || !(sender is ComboBox)) { return; }

            ComboBox comboBox = (ComboBox)sender;
            string enteredText = comboBox.Text.ToLower();

            // If name of pokemon is typed select that item
            foreach (KeyValuePair<int, string> item in comboBox.Items)
            {
                if (item.Value.ToLower().Equals(enteredText))
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }
        }

        private void radioButtonNoFlag_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonIgnoreAbility.Checked && radioButtonIngoreGender.Checked)
            {
                numericUpDownGender.Enabled = false;
                return;
            }

            numericUpDownGender.Enabled = true;

        }
    }
}
