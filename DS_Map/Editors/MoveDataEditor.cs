using DSPRE.Resources;
using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static DSPRE.MoveData;
using MessageBox = System.Windows.Forms.MessageBox;

namespace DSPRE {
    public partial class MoveDataEditor : Form {
        private bool disableHandlers = false;

        private readonly string[] fileNames;
        private readonly string[] moveDescriptions;

        private int currentLoadedId = 0;
        private MoveData currentLoadedFile = null;

        private static bool dirty = false;
        private static readonly string formName = "Move Data Editor";

        public MoveDataEditor(string[] fileNames, string[] moveDescriptions) {
            this.fileNames = fileNames.ToArray();
            this.moveDescriptions = moveDescriptions;

            InitializeComponent();

            disableHandlers = true;

            moveNumberNumericUpDown.Maximum = fileNames.Length - 1;
            moveNameInputComboBox.Items.AddRange(this.fileNames);
            string[] battleSequenceFiles = RomInfo.GetBattleEffectSequenceFiles();

            for (int i = 0; i < battleSequenceFiles.Length; i++) {
                string[] db = PokeDatabase.MoveData.battleSequenceDescriptions;
                
                if (i >= db.Length || db[i] is null) {
                    battleSeqComboBox.Items.Add($"{i:D3} - Undocumented");
                } else {
                    battleSeqComboBox.Items.Add($"{i:D3} - {db[i]}");
                }
            }

            moveSplitComboBox.Items.AddRange(Enum.GetNames(typeof(MoveData.MoveSplit)));

            string[] names = Enum.GetNames(typeof(MoveData.AttackRange));
            System.Collections.IList list = rangesTableLayoutPanel.Controls;
            for (int i = 0; i < list.Count; i++) {
                CheckBox cb = list[i] as CheckBox;
                cb.Text = names[i + 1];
                cb.CheckedChanged += AttackRangeCheckBox_CheckedChanged;
            }

            names = Enum.GetNames(typeof(MoveData.MoveFlags));
            list = flagsTableLayoutPanel.Controls;
            for (int i = 0; i < list.Count; i++) {
                CheckBox cb = list[i] as CheckBox;
                cb.Text = names[i];
                cb.CheckedChanged += FlagsCheckBox_CheckedChanged;
            }

            contestConditionComboBox.Items.AddRange(Enum.GetNames(typeof(MoveData.ContestCondition)));

            moveTypeComboBox.Items.AddRange(Enum.GetNames(typeof(PokemonType)));

            disableHandlers = false;

            moveNameInputComboBox.SelectedIndex = 1;
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
            
            DialogResult res = MessageBox.Show(this, "There are unsaved changes to the current Move data.\nDiscard and proceed?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res.Equals(DialogResult.Yes)) {
                return true;
            }

            moveNumberNumericUpDown.Value = currentLoadedId;
            moveNameInputComboBox.SelectedIndex = currentLoadedId;


            return false;
        }
        private void ChangeLoadedFile(int toLoad) {
            currentLoadedId = toLoad;
            currentLoadedFile = new MoveData(toLoad);
            PopulateAllFromCurrentFile();
            setDirty(false);
        }
        private void PopulateAllFromCurrentFile() {
            moveTypeComboBox.SelectedIndex = (int)currentLoadedFile.movetype;

            System.Collections.IList list = rangesTableLayoutPanel.Controls;
            for (int i = 0; i < list.Count; i++) {
                (list[i] as CheckBox).Checked = ((currentLoadedFile.target & (1 << i)) != 0);
            }

            list = flagsTableLayoutPanel.Controls;
            for (int i = 0; i < list.Count; i++) {
                (list[i] as CheckBox).Checked = ((currentLoadedFile.flagField & (1 << i)) != 0);
            }

            textBox1.Text = moveDescriptions[currentLoadedId];

            battleSeqComboBox.SelectedIndex = (int)currentLoadedFile.battleeffect;
            moveSplitComboBox.SelectedIndex = (int)currentLoadedFile.split;
            sideEffectProbabilityUpDown.Value = currentLoadedFile.sideEffectProbability;
            contestConditionComboBox.SelectedIndex = (int)currentLoadedFile.contestConditionType;
            contestAppealNumericUpDown.Value = currentLoadedFile.contestAppeal;
            priorityNumericUpDown.Value = currentLoadedFile.priority;
            
            powerNumericUpDown.Value = currentLoadedFile.damage;
            accuracyNumericUpDown.Value = currentLoadedFile.accuracy;

            ppUpDown.Value = currentLoadedFile.pp;
        }

        //-------------------------------
        private void saveDataButton_Click(object sender, EventArgs e) {
            currentLoadedFile.SaveToFileDefaultDir(currentLoadedId, true);
            setDirty(false);
        }

        private void FlagsCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            disableHandlers = true;

            System.Collections.IList list = flagsTableLayoutPanel.Controls;
            
            currentLoadedFile.flagField = 0; 
            for (int i = 0; i < list.Count; i++) {
                int en = (list[i] as CheckBox).Checked ? 1 : 0;
                currentLoadedFile.flagField |= (byte)(en << i);
            }

            setDirty(true);
            disableHandlers = false;
        }
        private void AttackRangeCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            disableHandlers = true;

            System.Collections.IList list = rangesTableLayoutPanel.Controls;

            currentLoadedFile.target = 0;
            for (int i = 0; i < list.Count; i++) {
                int en = (list[i] as CheckBox).Checked ? 1 : 0;
                currentLoadedFile.target |= (ushort)(en << i);
            }

            setDirty(true);
            disableHandlers = false;
        }
        private void moveNameInputComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            disableHandlers = true;

            if (CheckDiscardChanges()) {
                int newNumber = moveNameInputComboBox.SelectedIndex;
                moveNumberNumericUpDown.Value = newNumber;
                ChangeLoadedFile(newNumber);
            }

            disableHandlers = false;
        }

        private void moveNumberNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) { 
                return; 
            }

            disableHandlers = true;

            if (CheckDiscardChanges()) {
                int newNumber = (int)moveNumberNumericUpDown.Value;
                moveNameInputComboBox.SelectedIndex = newNumber;
                ChangeLoadedFile(newNumber);
            }
            
            disableHandlers = false;
        }

        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e) {
            string suggestedFilename = this.fileNames[currentLoadedId];
            currentLoadedFile.SaveToFileExplorePath(suggestedFilename, true);
        }

        private void ppUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            currentLoadedFile.pp = (byte)ppUpDown.Value;
            setDirty(true);
        }

        private void moveSplitComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            currentLoadedFile.split = (MoveSplit)moveSplitComboBox.SelectedIndex;
            setDirty(true);
        }

        private void moveTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            currentLoadedFile.movetype = (PokemonType)moveTypeComboBox.SelectedIndex;
            setDirty(true);
        }

        private void battleSeqComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            currentLoadedFile.battleeffect = (ushort)battleSeqComboBox.SelectedIndex;
            setDirty(true);
        }

        private void contestConditionComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            currentLoadedFile.contestConditionType = (ContestCondition)contestConditionComboBox.SelectedIndex;
            setDirty(true);
        }
        private void contestAppealNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            currentLoadedFile.contestAppeal = (byte)contestAppealNumericUpDown.Value;
            setDirty(true);
        }

        private void powerNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            currentLoadedFile.damage = (byte)powerNumericUpDown.Value;
            setDirty(true);
        }

        private void accuracyNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }

            currentLoadedFile.accuracy = (byte)accuracyNumericUpDown.Value;
            setDirty(true);
        }

        private void priorityNumericUpDown_ValueChanged(object sender, EventArgs e) {
            if (currentLoadedFile.priority == 0) {
                prioPictureBox.Image = null;
            } else if (currentLoadedFile.priority > 0) {
                prioPictureBox.Image = Properties.Resources.addIcon;
            } else {
                prioPictureBox.Image = Properties.Resources.deleteIcon;
            }

            if (disableHandlers) {
                return;
            }
            currentLoadedFile.priority = (sbyte)priorityNumericUpDown.Value;

            setDirty(true);
        }

        private void sideEffectUpDown_ValueChanged(object sender, EventArgs e) {
            if (disableHandlers) {
                return;
            }
            currentLoadedFile.sideEffectProbability = (byte)sideEffectProbabilityUpDown.Value;

            setDirty(true);
        }
    }
}
