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

        private PokemonEditor _parent;
        private readonly string[] fileNames;
        private readonly string[] pokenames;
        private readonly string[] moveNames;

        private bool editMode = false;
        public bool dirty = false;
        private int currentLoadedId = 0;
        private LearnsetData currentLoadedFile = null;

        private readonly string formName = "Learnset Editor";

        private readonly string[] editButtonNames = new string[] {
            "Edit",
            "Confirm"
        };

        private readonly string[] deleteButtonNames = new string[] {
            "Delete",
            "Discard"
        };
        

        public LearnsetEditor(string[] moveNames, Control parent, PokemonEditor pokeEditor) {

            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = parent.Size;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            Helpers.DisableHandlers();

            this._parent = pokeEditor;
            this.moveNames = moveNames;
            this.pokenames = RomInfo.GetPokemonNames();
            this.fileNames = GetFileNames();

            InitDataRanges();

            /* ---------------- */

            descriptorLabel.Text = "";
            statusLabel.Text = "";

            ChangeLoadedFile(1);

            Helpers.EnableHandlers();            
        }

        private void InitDataRanges()
        {
            moveInputComboBox.DataSource = moveNames;
            pokemonNameInputComboBox.DataSource = fileNames;
            monNumberNumericUpDown.Minimum = 0;
            monNumberNumericUpDown.Maximum = RomInfo.GetLearnsetFilesCount() - 1;

            pokemonNameInputComboBox.SelectedIndex = 1;
            monNumberNumericUpDown.Value = 1;
            levelNumericUpDown.Value = 1;
            
        }

        private string[] GetFileNames() 
        {

            int learnsetCount = RomInfo.GetLearnsetFilesCount();

            List<string> fileNames = new List<string>(RomInfo.GetLearnsetFilesCount());
            fileNames.AddRange(pokenames);
            for (int i = 0; i < PokeDatabase.PersonalData.personalExtraFiles.Length; i++) {
                PokeDatabase.PersonalData.PersonalExtraFiles altFormEntry = PokeDatabase.PersonalData.personalExtraFiles[i];
                fileNames.Add(fileNames[altFormEntry.monId] + " - " + altFormEntry.description);
            }

            for (int i = 0; i < learnsetCount - fileNames.Count; i++)
            {
                fileNames.Add($"Extra entry {fileNames.Count}");
            }

            return fileNames.ToArray();
        }

        public void ChangeLoadedFile(int learnsetID)
        {
            currentLoadedFile = new LearnsetData(learnsetID);
            currentLoadedId = learnsetID;

            monNumberNumericUpDown.Value = currentLoadedId;
            pokemonNameInputComboBox.SelectedIndex = currentLoadedId;

            UpdateMovesListFromFile();
            UpdateEntryCountLabel();
            UpdateButtonsOnMoveSelection();
            UpdatePokePic();
            UpdateAddEditStatus();
            SetDirty(false);
        }

        private void UpdateMovesListFromFile()
        {
            movesListBox.BeginUpdate();
            movesListBox.Items.Clear();
            foreach (var elem in currentLoadedFile.list)
            {
                movesListBox.Items.Add(ElemToString(elem));
            }
            movesListBox.EndUpdate();
        }

        private void UpdateEntryCountLabel()
        {
            StringBuilder labelText = new StringBuilder("Entry Count: ");
            labelText.Append(movesListBox.Items.Count);

            if (movesListBox.Items.Count > LearnsetData.VanillaLimit)
            {
                labelText.Append("!");
                entryCountLabel.ForeColor = Color.FromArgb(210, 120, 0);
                entryCountLabel.Font = new Font(entryCountLabel.Font, FontStyle.Bold);
            }
            else
            {
                entryCountLabel.ForeColor = Color.Black;
                entryCountLabel.Font = new Font(entryCountLabel.Font, FontStyle.Regular);
            }

            entryCountLabel.Text = labelText.ToString();
        }

        private void UpdatePokePic() 
        {
            int excess = currentLoadedId - pokenames.Length;
            int toLoad = currentLoadedId; // Default to the current ID
            try
            {
                if (excess >= 0)
                {
                    toLoad = PokeDatabase.PersonalData.personalExtraFiles[excess].iconId;
                }
            }
            catch (IndexOutOfRangeException)
            {
                toLoad = 0;
            }
            finally
            {
                pokemonPictureBox.Image = DSUtils.GetPokePic(toLoad, pokemonPictureBox.Width, pokemonPictureBox.Height);
            }
        }

        private void UpdateAddEditStatus()
        {
            (byte level, ushort move) newEntry = ((byte)levelNumericUpDown.Value, (ushort)moveInputComboBox.SelectedIndex);

            if (currentLoadedFile == null)
            {
                return;
            }

            bool duplicate = currentLoadedFile.list.Contains(newEntry);

            if (duplicate)
            {
                if (editMode)
                {
                    editMoveButton.Enabled = false;
                }
                else
                {
                    addMoveButton.Enabled = false;
                }
                statusLabel.Text = "Entry already exists!";
            }
            else
            {
                bool addable = IsValidEntry();
                if (addable)
                {
                    statusLabel.Text = "";
                }
                else
                {
                    statusLabel.Text = "Invalid Move ID or Level!";
                }

                if (editMode)
                {
                    editMoveButton.Enabled = addable;
                }
                else
                {
                    addMoveButton.Enabled = addable;
                }
            }
        }

        public void SaveLearnsetData() {
            if (currentLoadedFile == null) {
                return;
            }
            currentLoadedFile.SaveToFileDefaultDir(currentLoadedId, false);
            SetDirty(false);
        }

        private void SetDirty(bool status) {
            if (status) {
                dirty = true;
                this.Text = formName + "*";
            } else {
                dirty = false;
                this.Text = formName;
            }
            _parent.UpdateTabPageNames();
        }
        public bool CheckDiscardChanges() 
        {
            if (!dirty) {
                return true;
            }

            DialogResult res = MessageBox.Show(this, 
                "You have unsaved changes. Do you want to save them before switching Pokémon?",
                "Learnset Editor - Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {
                SaveLearnsetData();
            }
            else if (res == DialogResult.Cancel)
            {
                monNumberNumericUpDown.Value = currentLoadedId;
                pokemonNameInputComboBox.SelectedIndex = currentLoadedId;
                return false; // Cancel the change
            }          

            return true;
        }
        private string ElemToString((ushort level, ushort move) elem) {
            return $"Lv. {elem.level}: {moveNames[elem.move]}";
        }
        private bool IsValidEntry()
        {
            return levelNumericUpDown.Value > 0 &&
                moveInputComboBox.SelectedIndex > 0;
        }

        private void UpdateByEditMode()
        {
            int index = editMode == false ? 0 : 1;
            editMoveButton.Text = editButtonNames[index];
            deleteMoveButton.Text = deleteButtonNames[index];
        }

        private void UpdateButtonsOnMoveSelection()
        {
            int sel = movesListBox.SelectedIndex;

            if (sel < 0)
            {
                editMoveButton.Enabled = false;
                deleteMoveButton.Enabled = false;
                moveUpButton.Enabled = false;
                moveDownButton.Enabled = false;
                return;
            }

            editMoveButton.Enabled = true;
            deleteMoveButton.Enabled = true;

            int moveLevel = currentLoadedFile.list[sel].level;
            int previousLevel = sel > 0 ? currentLoadedFile.list[sel - 1].level : 0;
            int nextLevel = sel < currentLoadedFile.list.Count - 1 ? currentLoadedFile.list[sel + 1].level : 255;

            // Allow reordering only if the levels are the same
            moveUpButton.Enabled = moveLevel == previousLevel;
            moveDownButton.Enabled = moveLevel == nextLevel;
        }

        private int InsertEntrySafe(int level, int moveID)
        {
            if (currentLoadedFile == null || !IsValidEntry())
            {
                return -1;
            }

            (byte level, ushort move) newEntry = ((byte)level, (ushort)moveID);

            if (currentLoadedFile.list.Contains(newEntry))
            {
                MessageBox.Show(this, "This entry already exists!\nThis should not have happened.", "Learnset Editor - Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                AppLogger.Error("Learnset Editor: Attempted to insert a duplicate entry: " + newEntry);
                return -1;
            }

            int index = currentLoadedFile.list.FindIndex(x => x.level > newEntry.level || (x.level == newEntry.level && x.move > newEntry.move));

            if (index < 0)
            {
                currentLoadedFile.list.Add(newEntry);
                return currentLoadedFile.list.Count - 1; // Add to the end if no larger entry is found
            }
            else
            {
                currentLoadedFile.list.Insert(index, newEntry);
                return index; // Insert at the found index
            }

        }

        private bool ShiftEntry(int index, int direction)
        {
            if (currentLoadedFile == null || index < 0 || index >= currentLoadedFile.list.Count)
            {
                return false; // Invalid index
            }

            int newIndex = index + direction;
            if (newIndex < 0 || newIndex >= currentLoadedFile.list.Count)
            {
                return false; // Out of bounds
            }
            // Move the entry safely
            SwapEntriesSafe(index, newIndex);
            return true;
        }

        private bool SwapEntriesSafe(int indexA, int indexB)
        {
            if (currentLoadedFile == null || indexA < 0 || indexB < 0 || indexA >= currentLoadedFile.list.Count || indexB >= currentLoadedFile.list.Count)
            {
                return false; // Invalid indices
            }
            (byte level, ushort move) entryA = currentLoadedFile.list[indexA];
            (byte level, ushort move) entryB = currentLoadedFile.list[indexB];
            // Swap the entries
            currentLoadedFile.list[indexA] = entryB;
            currentLoadedFile.list[indexB] = entryA;

            return true; // Successfully swapped
        }

        #region Event Handlers
        private void saveDataButton_Click(object sender, EventArgs e) {
            SaveLearnsetData();
        }

        private void pokemonNameInputComboBox_SelectedIndexChanged(object sender, EventArgs e) 
        {
            if (Helpers.HandlersDisabled)
            {
                return;
            }

            Update();

            this._parent.TrySyncIndices((ComboBox)sender);
            Helpers.DisableHandlers();
            if (CheckDiscardChanges()) 
            {
                ChangeLoadedFile(pokemonNameInputComboBox.SelectedIndex);
            }
            Helpers.EnableHandlers();
        }

        private void monNumberNumericUpDown_ValueChanged(object sender, EventArgs e) {

            if (Helpers.HandlersDisabled)
            {
                return;
            }

            Update();
            
            this._parent.TrySyncIndices((NumericUpDown)sender);
            Helpers.DisableHandlers();
            if (CheckDiscardChanges()) 
            {
                ChangeLoadedFile((int)monNumberNumericUpDown.Value);
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

        private void levelNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (currentLoadedFile == null) {
                return;
            }
            UpdateAddEditStatus();
        }

        private void addMoveButton_Click(object sender, EventArgs e) {

            int index = InsertEntrySafe((byte)levelNumericUpDown.Value, (ushort)moveInputComboBox.SelectedIndex);

            if (index < 0) {
                return; // Insertion failed, likely due to invalid entry
            }

            UpdateMovesListFromFile();
            movesListBox.SelectedIndex = index;
            UpdateAddEditStatus();
            SetDirty(true);
        }

        private void deleteMoveButton_Click(object sender, EventArgs e) {

            if (currentLoadedFile == null) {
                return;
            }

            int sel = movesListBox.SelectedIndex;
            if (sel < 0) {
                return;
            }

            if (editMode) 
            {
                editMode = false;
                movesListBox.Enabled = true;
                UpdateButtonsOnMoveSelection();
            } 
            else 
            {
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
            SetDirty(true);
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

                if (newEntry.level == oldLevel)
                {
                    currentLoadedFile.list[sel] = newEntry;
                    movesListBox.Items[sel] = ElemToString(newEntry);
                    newSelection = sel;
                }
                else
                {
                    currentLoadedFile.list.RemoveAt(sel);
                    newSelection = InsertEntrySafe(newEntry.level, newEntry.move);
                    UpdateMovesListFromFile();
                }             
                
                UpdateEntryCountLabel();
                movesListBox.SelectedIndex = newSelection;
                editMode = false;
                movesListBox.Enabled = true;
            } else {  
                editMode = true;
                movesListBox.Enabled = false;
                moveUpButton.Enabled = false;
                moveDownButton.Enabled = false;

                editMoveButton.Text = editButtonNames[1];
                deleteMoveButton.Text = deleteButtonNames[1];

                (ushort level, ushort move) = currentLoadedFile.list[sel];
                moveInputComboBox.SelectedIndex = move;
                levelNumericUpDown.Value = level;
            }

            UpdateByEditMode();
            UpdateAddEditStatus();
            SetDirty(true);
        }

        private void movesListBox_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateButtonsOnMoveSelection();
        }

        private void moveUpButton_Click(object sender, EventArgs e)
        {
            int sel = movesListBox.SelectedIndex;
            if (!ShiftEntry(sel, -1))
            {
                return;
            }
            UpdateMovesListFromFile();
            movesListBox.SelectedIndex = sel-1;
            SetDirty(true);
        }

        private void moveDownButton_Click(object sender, EventArgs e)
        {
            int sel = movesListBox.SelectedIndex;
            if (!ShiftEntry(sel, 1))
            {
                return;
            }
            UpdateMovesListFromFile();
            movesListBox.SelectedIndex = sel+1;            
            SetDirty(true);
        }

        #endregion

    }
}
