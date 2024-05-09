using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DSPRE
{
    public partial class DVCalc : Form
    {
        public DVCalc(int TrainerIndex, int TrainerClassIndex)
        {
            InitializeComponent();
            PopulateComboBox();
            SetTrainerData(TrainerIndex, TrainerClassIndex);

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

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

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
            this.helpButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pokeLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trainerClassIdx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trainerIdx)).BeginInit();
            this.SuspendLayout();
            // 
            // poke_label
            // 
            this.poke_label.AutoSize = true;
            this.poke_label.Location = new System.Drawing.Point(28, 154);
            this.poke_label.Name = "poke_label";
            this.poke_label.Size = new System.Drawing.Size(65, 16);
            this.poke_label.TabIndex = 3;
            this.poke_label.Text = "Pokémon";
            // 
            // trainerClassIdx_label
            // 
            this.trainerClassIdx_label.AutoSize = true;
            this.trainerClassIdx_label.Location = new System.Drawing.Point(28, 87);
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
            this.pokeLVL_label.Location = new System.Drawing.Point(278, 179);
            this.pokeLVL_label.Name = "pokeLVL_label";
            this.pokeLVL_label.Size = new System.Drawing.Size(33, 16);
            this.pokeLVL_label.TabIndex = 6;
            this.pokeLVL_label.Text = "LVL.";
            // 
            // pokeLevel
            // 
            this.pokeLevel.Location = new System.Drawing.Point(318, 177);
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
            this.trainerClassIdx.Location = new System.Drawing.Point(28, 110);
            this.trainerClassIdx.Maximum = new decimal(new int[] {
            120,
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
            this.natureSelect.Location = new System.Drawing.Point(192, 44);
            this.natureSelect.Name = "natureSelect";
            this.natureSelect.Size = new System.Drawing.Size(207, 24);
            this.natureSelect.TabIndex = 11;
            // 
            // nature_label
            // 
            this.nature_label.AutoSize = true;
            this.nature_label.Location = new System.Drawing.Point(192, 21);
            this.nature_label.Name = "nature_label";
            this.nature_label.Size = new System.Drawing.Size(47, 16);
            this.nature_label.TabIndex = 12;
            this.nature_label.Text = "Nature";
            // 
            // DV_label
            // 
            this.DV_label.AutoSize = true;
            this.DV_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DV_label.Location = new System.Drawing.Point(443, 44);
            this.DV_label.Name = "DV_label";
            this.DV_label.Size = new System.Drawing.Size(162, 25);
            this.DV_label.TabIndex = 13;
            this.DV_label.Text = "Difficulty Value: 0";
            // 
            // calcButton
            // 
            this.calcButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.calcButton.Location = new System.Drawing.Point(491, 179);
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
            this.maleCheck.Location = new System.Drawing.Point(192, 111);
            this.maleCheck.Name = "maleCheck";
            this.maleCheck.Size = new System.Drawing.Size(112, 20);
            this.maleCheck.TabIndex = 15;
            this.maleCheck.Text = "Trainer Male?";
            this.maleCheck.UseVisualStyleBackColor = true;
            // 
            // maxDVNature_label
            // 
            this.maxDVNature_label.AutoSize = true;
            this.maxDVNature_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxDVNature_label.Location = new System.Drawing.Point(323, 127);
            this.maxDVNature_label.Name = "maxDVNature_label";
            this.maxDVNature_label.Size = new System.Drawing.Size(125, 20);
            this.maxDVNature_label.TabIndex = 16;
            this.maxDVNature_label.Text = "DV 255 Nature:";
            // 
            // IV_label
            // 
            this.IV_label.AutoSize = true;
            this.IV_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IV_label.Location = new System.Drawing.Point(323, 87);
            this.IV_label.Name = "IV_label";
            this.IV_label.Size = new System.Drawing.Size(113, 20);
            this.IV_label.TabIndex = 17;
            this.IV_label.Text = "Resulting IVs:";
            // 
            // pokemonSelector
            // 
            this.pokemonSelector.FormattingEnabled = true;
            this.pokemonSelector.Location = new System.Drawing.Point(28, 176);
            this.pokemonSelector.Name = "pokemonSelector";
            this.pokemonSelector.Size = new System.Drawing.Size(244, 24);
            this.pokemonSelector.TabIndex = 18;
            // 
            // showAllButton
            // 
            this.showAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showAllButton.Location = new System.Drawing.Point(391, 192);
            this.showAllButton.Name = "showAllButton";
            this.showAllButton.Size = new System.Drawing.Size(94, 29);
            this.showAllButton.TabIndex = 19;
            this.showAllButton.Text = "Show All";
            this.showAllButton.UseVisualStyleBackColor = true;
            this.showAllButton.Click += new System.EventHandler(this.ShowAllButton_Click);
            // 
            // helpButton
            // 
            this.helpButton.Location = new System.Drawing.Point(541, 2);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(75, 23);
            this.helpButton.TabIndex = 20;
            this.helpButton.Text = "Help";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // DVCalc
            // 
            this.ClientSize = new System.Drawing.Size(628, 242);
            this.Controls.Add(this.helpButton);
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

        private void SetTrainerData(int TrainerIndex, int TrainerClassIndex)
        {
            trainerIdx.Value = TrainerIndex;
            trainerClassIdx.Value = TrainerClassIndex;
        }

        private void CalcButton_Click(object sender, EventArgs e)
        {
            // Natures are sorted by their index so this works
            uint nature = (uint)natureSelect.SelectedIndex;

            int pokemonIndex = 1;

            if (pokemonSelector.SelectedItem != null)
            {
                KeyValuePair<int, string> selectedPokemon = (KeyValuePair<int, string>)pokemonSelector.SelectedItem;

                pokemonIndex = (int)selectedPokemon.Key;
            }

            int DV = DVCalculator.findHighestDV((int)trainerIdx.Value, (int)trainerClassIdx.Value, maleCheck.Checked, pokemonIndex, (int)pokeLevel.Value, nature);

            // Determine nature for max DV (max IV) for convenience
            uint maxDVNature = DVCalculator.getNatureFromPID(DVCalculator.generatePID((int)trainerIdx.Value, (int)trainerClassIdx.Value, maleCheck.Checked, pokemonIndex, (int)pokeLevel.Value, 255));

            // Display results
            DV_label.Text = "Difficulty Value: " + DV;
            IV_label.Text = "Resulting IVs: " + (DV * 31 / 255);
            maxDVNature_label.Text = "DV 255 Nature: " + DVCalculator.Natures[(int)maxDVNature];


        }

        private void ShowAllButton_Click(object sender, EventArgs e)
        {
            int pokemonIndex = 1;

            if (pokemonSelector.SelectedItem != null)
            {
                KeyValuePair<int, string> selectedPokemon = (KeyValuePair<int, string>)pokemonSelector.SelectedItem;

                pokemonIndex = (int)selectedPokemon.Key;

            }

            // Create a list of DV-IV-Nature Triplets
            List<DVIVNatureTriplet> natureDict = DVCalculator.getAllNatures((int)trainerIdx.Value, (int)trainerClassIdx.Value, maleCheck.Checked, pokemonIndex, (int)pokeLevel.Value);

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
    }
}
