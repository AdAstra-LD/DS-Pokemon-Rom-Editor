using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace DSPRE {
    public partial class EvolutionsEditor : Form {
        private bool disableHandlers = false;

        private readonly string[] fileNames;
        private readonly string[] pokeNames;
        private readonly string[] moveNames;
        private readonly string[] itemNames;

        private int currentLoadedId = 0;
        private EvolutionFile currentLoadedFile = null;

        private static bool dirty = false;
        private static readonly string formName = "Evolutions Editor";

        private (ComboBox m, Label l, NumericUpDown p, ComboBox t)[] evoRows;
        public EvolutionsEditor() {
            this.pokeNames = RomInfo.GetPokemonNames();
            this.moveNames = RomInfo.GetAttackNames();
            this.itemNames = RomInfo.GetItemNames();

            int count = RomInfo.GetEvolutionFilesCount();
            List<string> fileNames = new List<string>(count);
            fileNames.AddRange(pokeNames);

            this.fileNames = fileNames.ToArray();
            InitializeComponent();
            
            disableHandlers = true;

            evoRows = new (ComboBox m, Label l, NumericUpDown p, ComboBox t)[EvolutionFile.numEvolutions] {
                (evoMethodComboBox1, descLabel1, evoParamUpDown1, evoTargetMonComboBox1),
                (evoMethodComboBox2, descLabel2, evoParamUpDown2, evoTargetMonComboBox2),
                (evoMethodComboBox3, descLabel3, evoParamUpDown3, evoTargetMonComboBox3),
                (evoMethodComboBox4, descLabel4, evoParamUpDown4, evoTargetMonComboBox4),
                (evoMethodComboBox5, descLabel5, evoParamUpDown5, evoTargetMonComboBox5),
                (evoMethodComboBox6, descLabel6, evoParamUpDown6, evoTargetMonComboBox6),
                (evoMethodComboBox7, descLabel7, evoParamUpDown7, evoTargetMonComboBox7),
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
                (ComboBox m, Label l, NumericUpDown p, ComboBox t) = evoRows[i];

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
            //Build a new EvolutionFile from the current GUI state

            EvolutionFile newFile = new EvolutionFile();
            List<EvolutionData> data = new List<EvolutionData>();

            for (int i = 0; i < EvolutionFile.numEvolutions; i++) {
                (ComboBox m, Label l, NumericUpDown p, ComboBox t) = evoRows[i];

                //Retrieve method from enum
                EvolutionMethod method = (EvolutionMethod)m.SelectedIndex;
                short param = (short)p.Value;
                short target = (short)t.SelectedIndex;

                EvolutionData ed = new EvolutionData() {
                    method = method,
                    param = param,
                    target = target
                };

                if (ed.isValid()) {
                    data.Add(ed);
                }
            }

            newFile.data = data.ToArray();
            newFile.SaveToFileDefaultDir(currentLoadedId, true);
            currentLoadedFile = newFile;

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

        private void evoMethodComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(0);
        }

        private void evoMethodComboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(1);
        }

        private void evoMethodComboBox3_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(2);
        }

        private void evoMethodComboBox4_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(3);
        }

        private void evoMethodComboBox5_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(4);
        }

        private void evoMethodComboBox6_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(5);
        }

        private void evoMethodComboBox7_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(6);
        }



        private void UpdateDescriptionLabel(int index) {
            if (index < 0 || index >= evoRows.Length) {
                throw new ArgumentOutOfRangeException("Index out of range: " + index);
            }
            (ComboBox m, Label l, NumericUpDown p, ComboBox t) = evoRows[index];

            if (m.SelectedIndex < 0 || m.SelectedIndex > Enum.GetValues(typeof(EvolutionMethod)).Length) {
                l.Text = "";
                return;
            }

            disableHandlers = true;

            EvolutionMethod method = (EvolutionMethod)m.SelectedIndex;
            EvolutionParamMeaning type = EvolutionFile.evoDescriptions[method];


            switch (type) {
                case EvolutionParamMeaning.Ignored:
                    l.Text = "";
                    p.Enabled = false;
                    if (p.Value != 0) {
                        Console.WriteLine("Warning: Evolution parameter is not 0, but it should be.");
                    }
                    p.Value = 0;
                    break;

                case EvolutionParamMeaning.FromLevel:
                    l.Text = "From Level: ";
                    p.Enabled = true;
                    p.Maximum = 100;
                    break;

                case EvolutionParamMeaning.ItemName:
                    l.Text = $"({itemNames[(int)p.Value]})";
                    p.Enabled = true;
                    p.Maximum = itemNames.Length - 1;
                    break;

                case EvolutionParamMeaning.MoveName:
                    l.Text = $"({moveNames[(int)p.Value]})";
                    p.Enabled = true;
                    p.Maximum = moveNames.Length - 1;
                    break;

                case EvolutionParamMeaning.PokemonName:
                    l.Text = $"({pokeNames[(int)p.Value]})";
                    p.Enabled = true;
                    p.Maximum = pokeNames.Length - 1;
                    break;

                case EvolutionParamMeaning.BeautyValue:
                    l.Text = "Beauty >=";
                    p.Enabled = true;
                    p.Maximum = 255;
                    break;

                default:
                    throw new Exception("Unknown evolution parameter type: " + type);
            }

            disableHandlers = false;
        }

        private void evoParamUpDown1_ValueChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(0);
        }

        private void evoParamUpDown2_ValueChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(1);
        }

        private void evoParamUpDown3_ValueChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(2);
        }

        private void evoParamUpDown4_ValueChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(3);
        }

        private void evoParamUpDown5_ValueChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(4);
        }

        private void evoParamUpDown6_ValueChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(5);
        }

        private void evoParamUpDown7_ValueChanged(object sender, EventArgs e) {
            UpdateDescriptionLabel(6);
        }
    }
}
