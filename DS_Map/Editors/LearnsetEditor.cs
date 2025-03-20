using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace DSPRE {
    public partial class LearnsetEditor : Form {

        private readonly string[] fileNames;
        private readonly string[] pokenames;
        private readonly string[] moveNames;
        private PokemonEditor _parent;
        private readonly string[] editButtonNames = new string[] {
            "Edit",
            "Confirm"
        };

        private readonly string[] deleteButtonNames = new string[] {
            "Delete",
            "Discard"
        };

        private bool editMode = false;
        private int currentLoadedId = 0;
        private LearnsetData currentLoadedFile = null;

        private bool dirty = false;
        private readonly string formName = "Learnset Editor";

        public LearnsetEditor(string[] moveNames, Control parent, PokemonEditor pokeEditor) {
            this.moveNames = moveNames;
            this._parent = pokeEditor;
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = parent.Size;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            Helpers.DisableHandlers();

            BindingList<string> listMoveNames = new BindingList<string>(moveNames);
            moveInputComboBox.DataSource = listMoveNames;

            /* ---------------- */
            int count = RomInfo.GetLearnsetFilesCount();
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
            /* ---------------- */

            descriptorLabel.Text = "";
            statusLabel.Text = "";

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
        public bool CheckDiscardChanges() {
            if (!dirty) {
                return true;
            }

            DialogResult res = MessageBox.Show(this, "Learnsets Editor\nThere are unsaved changes to the current Learnset data.\nDiscard and proceed?", "Learnset Editor - Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res.Equals(DialogResult.Yes)) {
                return true;
            }

            monNumberNumericUpDown.Value = currentLoadedId;
            pokemonNameInputComboBox.SelectedIndex = currentLoadedId;


            return false;
        }
        private string ElemToString((ushort level, ushort move) elem) {
            return $"Lv. {elem.level}: {moveNames[elem.move]}";
        }
        public void ChangeLoadedFile(int toLoad) {
            currentLoadedId = toLoad;
            currentLoadedFile = new LearnsetData(currentLoadedId);
            PopulateAllFromCurrentFile();
            UpdateButtonsOnMoveSelection();
            
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

        private void PopulateAllFromCurrentFile() {
            movesListBox.BeginUpdate();
            movesListBox.Items.Clear();
            foreach (var elem in currentLoadedFile.list) {
                movesListBox.Items.Add(ElemToString(elem));
            }

            UpdateEntryCountLabel();
            movesListBox.EndUpdate();
        }

        //-------------------------------
        private void saveDataButton_Click(object sender, EventArgs e) {
            currentLoadedFile.SaveToFileDefaultDir(currentLoadedId, true);
            setDirty(false);
        }

        private void pokemonNameInputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            Update();
            if (Helpers.HandlersDisabled) {
                return;
            }
            this._parent.TrySyncIndices((ComboBox)sender);
            Helpers.DisableHandlers();
            UpdateAddEditStatus();
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
            UpdateAddEditStatus();
            if (CheckDiscardChanges()) {
                int newNumber = (int)monNumberNumericUpDown.Value;
                pokemonNameInputComboBox.SelectedIndex = newNumber;
                ChangeLoadedFile(newNumber);                
            }
            Helpers.EnableHandlers();
        }

        private void moveInputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (currentLoadedFile == null) {
                return;
            }
            UpdateAddEditStatus();
            descriptorLabel.Text = "Move ID: " + moveInputComboBox.SelectedIndex;
        }

        private bool CheckValidEntry() {
            return levelNumericUpDown.Value > 0 && 
                moveInputComboBox.SelectedIndex > 0;
        }
        private void levelNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (currentLoadedFile == null) {
                return;
            }
            UpdateAddEditStatus();
        }

        private void UpdateAddEditStatus() {
            (byte level, ushort move) newEntry = ((byte)levelNumericUpDown.Value, (ushort)moveInputComboBox.SelectedIndex);

            if (currentLoadedFile == null)
            {
                return;
            }

            bool duplicate = currentLoadedFile.list.Contains(newEntry);

            if (duplicate) {
                if (editMode) {
                    editMoveButton.Enabled = false;
                } else {
                    addMoveButton.Enabled = false;
                }
                statusLabel.Text = "Entry already exists!";
            } else {
                bool addable = CheckValidEntry();
                if (addable) {
                    statusLabel.Text = "";
                } else {
                    statusLabel.Text = "Invalid Move ID or Level!";
                }

                if (editMode) {
                    editMoveButton.Enabled = addable;
                } else {
                    addMoveButton.Enabled = addable;
                }
            }
        }

        private void addMoveButton_Click(object sender, EventArgs e) {
            (byte level, ushort move) newEntry = ((byte)levelNumericUpDown.Value, (ushort)moveInputComboBox.SelectedIndex);
            currentLoadedFile.list.Add(newEntry);
                
            currentLoadedFile.list.Sort();
            PopulateAllFromCurrentFile();
            movesListBox.SelectedIndex = currentLoadedFile.list.FindIndex(x => x == newEntry);
            UpdateAddEditStatus();
            setDirty(true);
        }

        private void deleteMoveButton_Click(object sender, EventArgs e) {
            int sel = movesListBox.SelectedIndex;
            if (sel < 0) {
                return;
            }

            if (editMode) {
                editMode = false;
                movesListBox.Enabled = true;
                UpdateButtonsOnMoveSelection();
            } else {
                currentLoadedFile.list.RemoveAt(sel);
                movesListBox.Items.RemoveAt(sel);

                int count = movesListBox.Items.Count;
                if (count > 0) {
                    movesListBox.SelectedIndex = Math.Max(0, sel - 1);
                }

                UpdateEntryCountLabel();
            }

            UpdateByEditMode();
            UpdateAddEditStatus();
            setDirty(true);
        }

        private void editMoveButton_Click(object sender, EventArgs e) {
            int sel = movesListBox.SelectedIndex;
            if (sel < 0) {
                return;
            }

            if (editMode) {
                (byte level, ushort move) newEntry = ((byte)levelNumericUpDown.Value, (ushort)moveInputComboBox.SelectedIndex);

                int newSelection;
                int oldLevel = currentLoadedFile.list[sel].level;
                currentLoadedFile.list[sel] = newEntry;

                if (newEntry.level == oldLevel) {
                    movesListBox.Items[sel] = ElemToString(newEntry);
                    newSelection = sel;
                } else {
                    currentLoadedFile.list.Sort();
                    PopulateAllFromCurrentFile();
                    newSelection = currentLoadedFile.list.FindIndex(x => x == newEntry);
                }
                
                UpdateEntryCountLabel();
                movesListBox.SelectedIndex = newSelection;
                editMode = false;
                movesListBox.Enabled = true;
            } else {  
                editMode = true;
                movesListBox.Enabled = false;

                editMoveButton.Text = editButtonNames[1];
                deleteMoveButton.Text = deleteButtonNames[1];

                (ushort level, ushort move) = currentLoadedFile.list[sel];
                moveInputComboBox.SelectedIndex = move;
                levelNumericUpDown.Value = level;
            }

            UpdateByEditMode();
            addMoveButton.Enabled = (editMode == false && CheckValidEntry());
            setDirty(true);
        }

        private void UpdateByEditMode() {
            UpdateButtonNames(editMode);
        }

        private void UpdateButtonNames(bool editMode) {
            int index = editMode == false ? 0 : 1;
            editMoveButton.Text = editButtonNames[index];
            deleteMoveButton.Text = deleteButtonNames[index];
        }

        private void movesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateButtonsOnMoveSelection();
        }

        private void UpdateButtonsOnMoveSelection() {
            int sel = movesListBox.SelectedIndex;
            if (sel < 0) {
                editMoveButton.Enabled = false;
                deleteMoveButton.Enabled = false;
                return;
            }

            editMoveButton.Enabled = true;
            deleteMoveButton.Enabled = true;
        }

        private void UpdateEntryCountLabel(){
            StringBuilder labelText = new StringBuilder("Entry Count: ");
            labelText.Append(movesListBox.Items.Count);

            if (movesListBox.Items.Count > LearnsetData.VanillaLimit) {
                labelText.Append("!");
                entryCountLabel.ForeColor = Color.FromArgb(210, 120, 0);
                entryCountLabel.Font = new Font(entryCountLabel.Font, FontStyle.Bold);
            } else {
                entryCountLabel.ForeColor = Color.Black;
                entryCountLabel.Font = new Font(entryCountLabel.Font, FontStyle.Regular);
            }

            entryCountLabel.Text = labelText.ToString();
        }
    }
}
