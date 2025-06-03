using DSPRE.ROMFiles;
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

            maleCheck.Checked = DVCalculator.TrainerClassGender.GetTrainerClassGender((int)TrainerClassIndex);
        }

        private void InitializeComponent()
        {
            this.poke_label = new System.Windows.Forms.Label();
            this.trainerClassIdx_label = new System.Windows.Forms.Label();
            this.trainerIdx_label = new System.Windows.Forms.Label();
            this.pokeLVL_label = new System.Windows.Forms.Label();
            this.pokeLevel = new System.Windows.Forms.NumericUpDown();
            this.trainerClassIdx = new System.Windows.Forms.NumericUpDown();
            this.trainerIdx = new System.Windows.Forms.NumericUpDown();
            this.natureSelect = new System.Windows.Forms.ComboBox();
            this.nature_label = new System.Windows.Forms.Label();
            this.DV_label = new System.Windows.Forms.Label();
            this.calcButton = new System.Windows.Forms.Button();
            this.maleCheck = new System.Windows.Forms.CheckBox();
            this.maxDVNature_label = new System.Windows.Forms.Label();
            this.IV_label = new System.Windows.Forms.Label();
            this.pokemonSelector = new System.Windows.Forms.ComboBox();
            this.showAllButton = new System.Windows.Forms.Button();
            this.radioButtonMale = new System.Windows.Forms.RadioButton();
            this.radioButtonFemale = new System.Windows.Forms.RadioButton();
            this.radioButtonAbility1 = new System.Windows.Forms.RadioButton();
            this.radioButtonAbility2 = new System.Windows.Forms.RadioButton();
            this.numericUpDownGender = new System.Windows.Forms.NumericUpDown();
            this.groupBoxGender = new System.Windows.Forms.GroupBox();
            this.radioButtonIngoreGender = new System.Windows.Forms.RadioButton();
            this.groupBoxAbility = new System.Windows.Forms.GroupBox();
            this.radioButtonIgnoreAbility = new System.Windows.Forms.RadioButton();
            this.labelGenderRatio = new System.Windows.Forms.Label();
            this.groupBoxHGSS = new System.Windows.Forms.GroupBox();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonHGSS = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pokeLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trainerClassIdx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trainerIdx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGender)).BeginInit();
            this.groupBoxGender.SuspendLayout();
            this.groupBoxAbility.SuspendLayout();
            this.groupBoxHGSS.SuspendLayout();
            this.SuspendLayout();
            // 
            // poke_label
            // 
            this.poke_label.AutoSize = true;
            this.poke_label.Location = new System.Drawing.Point(22, 78);
            this.poke_label.Name = "poke_label";
            this.poke_label.Size = new System.Drawing.Size(65, 16);
            this.poke_label.TabIndex = 3;
            this.poke_label.Text = "Pokémon";
            // 
            // trainerClassIdx_label
            // 
            this.trainerClassIdx_label.AutoSize = true;
            this.trainerClassIdx_label.Location = new System.Drawing.Point(172, 22);
            this.trainerClassIdx_label.Name = "trainerClassIdx_label";
            this.trainerClassIdx_label.Size = new System.Drawing.Size(122, 16);
            this.trainerClassIdx_label.TabIndex = 4;
            this.trainerClassIdx_label.Text = "Trainer Class Index";
            // 
            // trainerIdx_label
            // 
            this.trainerIdx_label.AutoSize = true;
            this.trainerIdx_label.Location = new System.Drawing.Point(28, 21);
            this.trainerIdx_label.Name = "trainerIdx_label";
            this.trainerIdx_label.Size = new System.Drawing.Size(85, 16);
            this.trainerIdx_label.TabIndex = 5;
            this.trainerIdx_label.Text = "Trainer Index";
            // 
            // pokeLVL_label
            // 
            this.pokeLVL_label.AutoSize = true;
            this.pokeLVL_label.Location = new System.Drawing.Point(315, 102);
            this.pokeLVL_label.Name = "pokeLVL_label";
            this.pokeLVL_label.Size = new System.Drawing.Size(33, 16);
            this.pokeLVL_label.TabIndex = 6;
            this.pokeLVL_label.Text = "LVL.";
            // 
            // pokeLevel
            // 
            this.pokeLevel.Location = new System.Drawing.Point(349, 100);
            this.pokeLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.pokeLevel.Name = "pokeLevel";
            this.pokeLevel.Size = new System.Drawing.Size(46, 22);
            this.pokeLevel.TabIndex = 7;
            this.pokeLevel.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // trainerClassIdx
            // 
            this.trainerClassIdx.Location = new System.Drawing.Point(172, 45);
            this.trainerClassIdx.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.trainerClassIdx.Name = "trainerClassIdx";
            this.trainerClassIdx.Size = new System.Drawing.Size(125, 22);
            this.trainerClassIdx.TabIndex = 9;
            // 
            // trainerIdx
            // 
            this.trainerIdx.Location = new System.Drawing.Point(28, 44);
            this.trainerIdx.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.trainerIdx.Name = "trainerIdx";
            this.trainerIdx.Size = new System.Drawing.Size(125, 22);
            this.trainerIdx.TabIndex = 10;
            // 
            // natureSelect
            // 
            this.natureSelect.FormattingEnabled = true;
            this.natureSelect.Location = new System.Drawing.Point(433, 44);
            this.natureSelect.Name = "natureSelect";
            this.natureSelect.Size = new System.Drawing.Size(227, 24);
            this.natureSelect.TabIndex = 11;
            // 
            // nature_label
            // 
            this.nature_label.AutoSize = true;
            this.nature_label.Location = new System.Drawing.Point(433, 21);
            this.nature_label.Name = "nature_label";
            this.nature_label.Size = new System.Drawing.Size(47, 16);
            this.nature_label.TabIndex = 12;
            this.nature_label.Text = "Nature";
            // 
            // DV_label
            // 
            this.DV_label.AutoSize = true;
            this.DV_label.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DV_label.Location = new System.Drawing.Point(316, 139);
            this.DV_label.Name = "DV_label";
            this.DV_label.Size = new System.Drawing.Size(162, 28);
            this.DV_label.TabIndex = 13;
            this.DV_label.Text = "Difficulty Value: 0";
            // 
            // calcButton
            // 
            this.calcButton.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.calcButton.Location = new System.Drawing.Point(546, 292);
            this.calcButton.Name = "calcButton";
            this.calcButton.Size = new System.Drawing.Size(114, 51);
            this.calcButton.TabIndex = 14;
            this.calcButton.Text = "Calculate";
            this.calcButton.UseVisualStyleBackColor = true;
            this.calcButton.Click += new System.EventHandler(this.CalcButton_Click);
            // 
            // maleCheck
            // 
            this.maleCheck.AutoSize = true;
            this.maleCheck.Checked = true;
            this.maleCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.maleCheck.Location = new System.Drawing.Point(303, 46);
            this.maleCheck.Name = "maleCheck";
            this.maleCheck.Size = new System.Drawing.Size(112, 20);
            this.maleCheck.TabIndex = 15;
            this.maleCheck.Text = "Trainer Male?";
            this.maleCheck.UseVisualStyleBackColor = true;
            // 
            // maxDVNature_label
            // 
            this.maxDVNature_label.AutoSize = true;
            this.maxDVNature_label.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxDVNature_label.Location = new System.Drawing.Point(316, 170);
            this.maxDVNature_label.Name = "maxDVNature_label";
            this.maxDVNature_label.Size = new System.Drawing.Size(133, 25);
            this.maxDVNature_label.TabIndex = 16;
            this.maxDVNature_label.Text = "DV 255 Nature:";
            // 
            // IV_label
            // 
            this.IV_label.AutoSize = true;
            this.IV_label.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IV_label.Location = new System.Drawing.Point(315, 198);
            this.IV_label.Name = "IV_label";
            this.IV_label.Size = new System.Drawing.Size(116, 25);
            this.IV_label.TabIndex = 17;
            this.IV_label.Text = "Resulting IVs:";
            // 
            // pokemonSelector
            // 
            this.pokemonSelector.FormattingEnabled = true;
            this.pokemonSelector.Location = new System.Drawing.Point(22, 99);
            this.pokemonSelector.Name = "pokemonSelector";
            this.pokemonSelector.Size = new System.Drawing.Size(287, 24);
            this.pokemonSelector.TabIndex = 18;
            this.pokemonSelector.TextChanged += new System.EventHandler(this.PokemonSelector_TextChanged);
            // 
            // showAllButton
            // 
            this.showAllButton.Location = new System.Drawing.Point(566, 88);
            this.showAllButton.Name = "showAllButton";
            this.showAllButton.Size = new System.Drawing.Size(94, 29);
            this.showAllButton.TabIndex = 19;
            this.showAllButton.Text = "Show All";
            this.showAllButton.UseVisualStyleBackColor = true;
            this.showAllButton.Click += new System.EventHandler(this.ShowAllButton_Click);
            // 
            // radioButtonMale
            // 
            this.radioButtonMale.AutoSize = true;
            this.radioButtonMale.Location = new System.Drawing.Point(6, 47);
            this.radioButtonMale.Name = "radioButtonMale";
            this.radioButtonMale.Size = new System.Drawing.Size(96, 20);
            this.radioButtonMale.TabIndex = 20;
            this.radioButtonMale.TabStop = true;
            this.radioButtonMale.Text = "Force Male";
            this.radioButtonMale.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.radioButtonMale.UseVisualStyleBackColor = true;
            // 
            // radioButtonFemale
            // 
            this.radioButtonFemale.AutoSize = true;
            this.radioButtonFemale.Location = new System.Drawing.Point(6, 72);
            this.radioButtonFemale.Name = "radioButtonFemale";
            this.radioButtonFemale.Size = new System.Drawing.Size(112, 20);
            this.radioButtonFemale.TabIndex = 21;
            this.radioButtonFemale.TabStop = true;
            this.radioButtonFemale.Text = "Force Female";
            this.radioButtonFemale.UseVisualStyleBackColor = true;
            // 
            // radioButtonAbility1
            // 
            this.radioButtonAbility1.AutoSize = true;
            this.radioButtonAbility1.Location = new System.Drawing.Point(6, 47);
            this.radioButtonAbility1.Name = "radioButtonAbility1";
            this.radioButtonAbility1.Size = new System.Drawing.Size(112, 20);
            this.radioButtonAbility1.TabIndex = 22;
            this.radioButtonAbility1.TabStop = true;
            this.radioButtonAbility1.Text = "Force Ability 1";
            this.radioButtonAbility1.UseVisualStyleBackColor = true;
            // 
            // radioButtonAbility2
            // 
            this.radioButtonAbility2.AutoSize = true;
            this.radioButtonAbility2.Location = new System.Drawing.Point(6, 72);
            this.radioButtonAbility2.Name = "radioButtonAbility2";
            this.radioButtonAbility2.Size = new System.Drawing.Size(112, 20);
            this.radioButtonAbility2.TabIndex = 23;
            this.radioButtonAbility2.TabStop = true;
            this.radioButtonAbility2.Text = "Force Ability 2";
            this.radioButtonAbility2.UseVisualStyleBackColor = true;
            // 
            // numericUpDownGender
            // 
            this.numericUpDownGender.Enabled = false;
            this.numericUpDownGender.Location = new System.Drawing.Point(6, 170);
            this.numericUpDownGender.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownGender.Name = "numericUpDownGender";
            this.numericUpDownGender.Size = new System.Drawing.Size(120, 22);
            this.numericUpDownGender.TabIndex = 24;
            // 
            // groupBoxGender
            // 
            this.groupBoxGender.Controls.Add(this.radioButtonIngoreGender);
            this.groupBoxGender.Controls.Add(this.radioButtonMale);
            this.groupBoxGender.Controls.Add(this.radioButtonFemale);
            this.groupBoxGender.Location = new System.Drawing.Point(11, 22);
            this.groupBoxGender.Name = "groupBoxGender";
            this.groupBoxGender.Size = new System.Drawing.Size(132, 112);
            this.groupBoxGender.TabIndex = 25;
            this.groupBoxGender.TabStop = false;
            this.groupBoxGender.Text = "Pokémon Gender";
            // 
            // radioButtonIngoreGender
            // 
            this.radioButtonIngoreGender.AutoSize = true;
            this.radioButtonIngoreGender.Checked = true;
            this.radioButtonIngoreGender.Location = new System.Drawing.Point(6, 22);
            this.radioButtonIngoreGender.Name = "radioButtonIngoreGender";
            this.radioButtonIngoreGender.Size = new System.Drawing.Size(76, 20);
            this.radioButtonIngoreGender.TabIndex = 22;
            this.radioButtonIngoreGender.TabStop = true;
            this.radioButtonIngoreGender.Text = "No Flag";
            this.radioButtonIngoreGender.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.radioButtonIngoreGender.UseVisualStyleBackColor = true;
            this.radioButtonIngoreGender.CheckedChanged += new System.EventHandler(this.radioButtonNoFlag_CheckedChanged);
            // 
            // groupBoxAbility
            // 
            this.groupBoxAbility.Controls.Add(this.radioButtonIgnoreAbility);
            this.groupBoxAbility.Controls.Add(this.radioButtonAbility1);
            this.groupBoxAbility.Controls.Add(this.radioButtonAbility2);
            this.groupBoxAbility.Location = new System.Drawing.Point(143, 22);
            this.groupBoxAbility.Name = "groupBoxAbility";
            this.groupBoxAbility.Size = new System.Drawing.Size(138, 112);
            this.groupBoxAbility.TabIndex = 26;
            this.groupBoxAbility.TabStop = false;
            this.groupBoxAbility.Text = "Pokémon Ability";
            // 
            // radioButtonIgnoreAbility
            // 
            this.radioButtonIgnoreAbility.AutoSize = true;
            this.radioButtonIgnoreAbility.Checked = true;
            this.radioButtonIgnoreAbility.Location = new System.Drawing.Point(6, 22);
            this.radioButtonIgnoreAbility.Name = "radioButtonIgnoreAbility";
            this.radioButtonIgnoreAbility.Size = new System.Drawing.Size(76, 20);
            this.radioButtonIgnoreAbility.TabIndex = 24;
            this.radioButtonIgnoreAbility.TabStop = true;
            this.radioButtonIgnoreAbility.Text = "No Flag";
            this.radioButtonIgnoreAbility.UseVisualStyleBackColor = true;
            this.radioButtonIgnoreAbility.CheckedChanged += new System.EventHandler(this.radioButtonNoFlag_CheckedChanged);
            // 
            // labelGenderRatio
            // 
            this.labelGenderRatio.AutoSize = true;
            this.labelGenderRatio.Location = new System.Drawing.Point(6, 148);
            this.labelGenderRatio.Name = "labelGenderRatio";
            this.labelGenderRatio.Size = new System.Drawing.Size(148, 16);
            this.labelGenderRatio.TabIndex = 27;
            this.labelGenderRatio.Text = "Pokémon Gender Ratio";
            // 
            // groupBoxHGSS
            // 
            this.groupBoxHGSS.Controls.Add(this.buttonHGSS);
            this.groupBoxHGSS.Controls.Add(this.groupBoxGender);
            this.groupBoxHGSS.Controls.Add(this.labelGenderRatio);
            this.groupBoxHGSS.Controls.Add(this.numericUpDownGender);
            this.groupBoxHGSS.Controls.Add(this.groupBoxAbility);
            this.groupBoxHGSS.Location = new System.Drawing.Point(22, 139);
            this.groupBoxHGSS.Name = "groupBoxHGSS";
            this.groupBoxHGSS.Size = new System.Drawing.Size(287, 204);
            this.groupBoxHGSS.TabIndex = 28;
            this.groupBoxHGSS.TabStop = false;
            this.groupBoxHGSS.Text = "HGSS Only";
            // 
            // buttonHelp
            // 
            this.buttonHelp.Location = new System.Drawing.Point(591, 123);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(69, 29);
            this.buttonHelp.TabIndex = 29;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // buttonHGSS
            // 
            this.buttonHGSS.Location = new System.Drawing.Point(170, 163);
            this.buttonHGSS.Name = "buttonHGSS";
            this.buttonHGSS.Size = new System.Drawing.Size(102, 29);
            this.buttonHGSS.TabIndex = 28;
            this.buttonHGSS.Text = "HGSS Help";
            this.buttonHGSS.UseVisualStyleBackColor = true;
            this.buttonHGSS.Click += new System.EventHandler(this.buttonHGSS_Click);
            // 
            // DVCalc
            // 
            this.ClientSize = new System.Drawing.Size(664, 347);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.groupBoxHGSS);
            this.Controls.Add(this.showAllButton);
            this.Controls.Add(this.pokemonSelector);
            this.Controls.Add(this.IV_label);
            this.Controls.Add(this.maxDVNature_label);
            this.Controls.Add(this.maleCheck);
            this.Controls.Add(this.calcButton);
            this.Controls.Add(this.DV_label);
            this.Controls.Add(this.nature_label);
            this.Controls.Add(this.natureSelect);
            this.Controls.Add(this.trainerIdx);
            this.Controls.Add(this.trainerClassIdx);
            this.Controls.Add(this.pokeLevel);
            this.Controls.Add(this.pokeLVL_label);
            this.Controls.Add(this.trainerIdx_label);
            this.Controls.Add(this.trainerClassIdx_label);
            this.Controls.Add(this.poke_label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "DVCalc";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DVCalc";
            ((System.ComponentModel.ISupportInitialize)(this.pokeLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trainerClassIdx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trainerIdx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGender)).EndInit();
            this.groupBoxGender.ResumeLayout(false);
            this.groupBoxGender.PerformLayout();
            this.groupBoxAbility.ResumeLayout(false);
            this.groupBoxAbility.PerformLayout();
            this.groupBoxHGSS.ResumeLayout(false);
            this.groupBoxHGSS.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("DV, or \"Difficulty Value\", is used by the game engine to calculate how tough an opponent Pokemon should be.\n" +
               "The DV affects a Pokemon's Nature and IVs - the higher the value, the stronger the Pokemon.\n" +
               "DVs will go from 0 (0 IVs) to 255 (31 IVs). Natures are chosen semi-randomly." +
               "\nIVs will be the same value for all Stats at any DV, so Hidden Power will only be Fighting or Dark Type." +
               "\nThis calculator allows you to choose a desired Nature and then find the highest possible DV that will yield that Nature." +
               "\nIf you want a specific combination of IVs and Nature instead, please click the \"Show All\" button and find the one you want."
               , "Difficulty Value", MessageBoxButtons.OK, MessageBoxIcon.Information);
     
        }

        private void buttonHGSS_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In Diamond, Pearl and Platinum Pokemon will always have the same gender as their trainer and will always have ability 1. " +
                "Heartgold and Soulsilver allow for the Pokemon's Gender and Ability to be set using special flags. " +
                "Setting those flags will also affect a Pokemon's Nature. " +
                "In order for this calculator to work it needs to know which flags have been set. " +
                "You can follow this guide:\n" +
                "- If the Pokemon whose nature you are trying to calculate has ability 2 then \"Force Ability 2\" must be selected.\n" +
                "- If the Pokemon whose nature you are trying to calculate has a non-default gender then \"Force Male\" / \"Force Female\" must be selected.\n" +
                "- If the Pokemon whose nature you are trying to calculate has a non-default gender and ability 1 then \"Force Ability 1\" must also be selected.\n" +
                "If you are changing a Pokemon's gender you must also provide the correct Gender Ratio which you can look up using DSPRE's \"Pokémon Editor\".", "How to use" ,MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
