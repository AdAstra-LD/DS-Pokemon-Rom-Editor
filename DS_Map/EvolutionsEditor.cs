using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace DSPRE {
    public partial class EvolutionsEditor : Form {
        private bool disableHandlers = false;

        private readonly string[] fileNames;
        private readonly string[] pokenames;

        private int currentLoadedId = 0;
        private EvolutionFile currentLoadedFile = null;

        private static bool dirty = false;
        private static readonly string formName = "Evolutions Editor";

        private (ComboBox m, NumericUpDown p, ComboBox t)[] evoRows;
        public EvolutionsEditor() {
            int count = RomInfo.GetEvolutionFilesCount();
            this.pokenames = RomInfo.GetPokemonNames();
            List<string> fileNames = new List<string>(count);
            fileNames.AddRange(pokenames);

            this.fileNames = fileNames.ToArray();
            InitializeComponent();
            
            disableHandlers = true;

            evoRows = new (ComboBox m, NumericUpDown p, ComboBox t)[EvolutionFile.numEvolutions] {
                (evoMethodComboBox1, evoParamUpDown1, evoTargetMonComboBox1),
                (evoMethodComboBox2, evoParamUpDown2, evoTargetMonComboBox2),
                (evoMethodComboBox3, evoParamUpDown3, evoTargetMonComboBox3),
                (evoMethodComboBox4, evoParamUpDown4, evoTargetMonComboBox4),
                (evoMethodComboBox5, evoParamUpDown5, evoTargetMonComboBox5),
                (evoMethodComboBox6, evoParamUpDown6, evoTargetMonComboBox6),
                (evoMethodComboBox7, evoParamUpDown7, evoTargetMonComboBox7),
            };

            BindingList<string> listMons = new BindingList<string>(fileNames);

            // Create as many rows as there are evolution types
            for (int i = 0; i < EvolutionFile.numEvolutions; i++) {
                //Create a binding source for the combobox in the first column
                evoRows[i].m.DataSource = new BindingSource(Enum.GetNames(typeof(EvolutionMethod)), string.Empty);
                evoRows[i].t.DataSource = new BindingSource(listMons, string.Empty);
            }

            monNumberNumericUpDown.Maximum = fileNames.Count - 1;

            pokemonNameInputComboBox.Items.AddRange(this.fileNames);

            disableHandlers = false;

            pokemonNameInputComboBox.SelectedIndex = 1;
        }

        private void pokemonNameInputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            disableHandlers = true;
            if (CheckDiscardChanges()) {
                int newNumber = pokemonNameInputComboBox.SelectedIndex;
                monNumberNumericUpDown.Value = newNumber;
                ChangeLoadedFile(newNumber);

            }
            disableHandlers = false;
        }

        private void monNumberNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            disableHandlers = true;
            if (CheckDiscardChanges()) {

                int newNumber = (int)monNumberNumericUpDown.Value;
                pokemonNameInputComboBox.SelectedIndex = newNumber;
                ChangeLoadedFile(newNumber);

            }
            disableHandlers = false;
        }
        private void ChangeLoadedFile(int toLoad) {
            currentLoadedId = toLoad;
            currentLoadedFile = new EvolutionFile(currentLoadedId);

            for (int i = 0; i < EvolutionFile.numEvolutions; i++) {
                (ComboBox m, NumericUpDown p, ComboBox t) = evoRows[i];

                ref EvolutionData data = ref currentLoadedFile.data[i];
                if (data.isValid()) {
                    m.SelectedIndex = (int)data.method;
                    p.Value = data.param;
                    t.SelectedIndex = data.target;
                } else {
                    m.SelectedIndex = -1;
                    p.Value = 0;
                    t.SelectedIndex = -1;
                }
            }

            pokemonPictureBox.Image = currentLoadedId > 0 ? DSUtils.GetPokePic(currentLoadedId, pokemonPictureBox.Width, pokemonPictureBox.Height) : Properties.Resources.IconPokeball;

            setDirty(false);
        }

        private void saveDataButton_Click(object sender, EventArgs e) {
            currentLoadedFile.SaveToFileDefaultDir(currentLoadedId, true);
            setDirty(false);
        }
        private bool CheckDiscardChanges() {
            if (!dirty) {
                return true;
            }

            DialogResult res = MessageBox.Show("There are unsaved changes to the current Evolution data.\nDiscard and proceed?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res.Equals(DialogResult.Yes)) {
                return true;
            }

            monNumberNumericUpDown.Value = currentLoadedId;
            pokemonNameInputComboBox.SelectedIndex = currentLoadedId;


            return false;
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
    }
}
