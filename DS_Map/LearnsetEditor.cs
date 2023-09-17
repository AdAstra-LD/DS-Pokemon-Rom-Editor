﻿using DSPRE.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using static ScintillaNET.Style;
using MessageBox = System.Windows.Forms.MessageBox;

namespace DSPRE {
    public partial class LearnsetEditor : Form {
        private bool disableHandlers = false;

        private readonly string[] fileNames;
        private readonly string[] pokenames;
        private readonly string[] moveNames;

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

        private static bool dirty = false;
        private static readonly string formName = "Learnset Editor";

        public LearnsetEditor(string[] moveNames) {
            this.moveNames = moveNames;

            InitializeComponent();

            disableHandlers = true;

            BindingList<string> listMoveNames = new BindingList<string>(moveNames);
            moveInputComboBox.DataSource = listMoveNames;

            /* ---------------- */
            int count = RomInfo.GetLearnsetFilesCount();
            this.pokenames = RomInfo.GetPokemonNames();
            List<string> fileNames = new List<string>(count);
            fileNames.AddRange(pokenames);

            for(int i = 0; i < count-pokenames.Length; i++) {
                PokeDatabase.PersonalData.PersonalExtraFiles extraEntry = PokeDatabase.PersonalData.personalExtraFiles[i];
                fileNames.Add(fileNames[extraEntry.monId] + " - " + extraEntry.description);
            }

            this.fileNames = fileNames.ToArray();
            monNumberNumericUpDown.Maximum = fileNames.Count - 1;
            pokemonNameInputComboBox.Items.AddRange(this.fileNames);
            /* ---------------- */

            descriptorLabel.Text = "";
            statusLabel.Text = "";

            disableHandlers = false;

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
        private bool CheckDiscardChanges() {
            if (!dirty) {
                return true;
            }

            DialogResult res = MessageBox.Show(this, "There are unsaved changes to the current Pokémon data.\nDiscard and proceed?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
        private void ChangeLoadedFile(int toLoad) {
            currentLoadedId = toLoad;
            currentLoadedFile = new LearnsetData(currentLoadedId);
            PopulateAllFromCurrentFile();
            UpdateButtonsOnMoveSelection();
            
            int excess = toLoad - pokenames.Length;
            if (excess >= 0) {
                toLoad = PokeDatabase.PersonalData.personalExtraFiles[excess].iconId;
            }
            pokemonPictureBox.Image = DSUtils.GetPokePic(toLoad, pokemonPictureBox.Width, pokemonPictureBox.Height);
            
            setDirty(false);
        }

        private void PopulateAllFromCurrentFile() {
            movesListBox.BeginUpdate();
            movesListBox.Items.Clear();
            foreach (var elem in currentLoadedFile.list) {
                movesListBox.Items.Add(ElemToString(elem));
            }
            movesListBox.EndUpdate();
        }

        //-------------------------------
        private void saveDataButton_Click(object sender, EventArgs e) {
            currentLoadedFile.SaveToFileDefaultDir(currentLoadedId, true);
            setDirty(false);
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
            }

            UpdateByEditMode();
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
    }
}
