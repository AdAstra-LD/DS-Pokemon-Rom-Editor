using DSPRE;
using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static DSPRE.ROMFiles.EventFile;

namespace DSPRE {
    public partial class EventFileImport : Form { 
        private static readonly int eventTypesCount = Enum.GetValues(typeof(serializationOrder)).Length;

        private readonly bool[] toImport = new bool[eventTypesCount];
        private readonly CheckedListBox[] listBoxes;
        private readonly Button[,] checkButtons;

        public readonly int[][] userSelected = new int[eventTypesCount][];

        public EventFileImport(EventFile ef) {
            InitializeComponent();

            for(int i = 0; i < toImport.Length; i++) {
                toImport[i] = true;
            }

            listBoxes = new CheckedListBox[] {
                spawnablesCheckedListBox,
                overworldsCheckedListBox,
                warpsCheckedListBox,
                triggersCheckedListBox
            };

            checkButtons = new Button[4, 2] {
                { spawnablesCheckAllButton, spawnablesUncheckAllButton },
                { overworldsCheckAllButton, overworldsUncheckAllButton },
                { warpsCheckAllButton, warpsUncheckAllButton },
                { triggersCheckAllButton, triggersUncheckAllButton }
            };

            foreach (Spawnable s in ef.spawnables) {
                listBoxes[(int)serializationOrder.Spawnables].Items.Add(s);
            }
            foreach (Overworld ow in ef.overworlds) {
                listBoxes[(int)serializationOrder.Overworlds].Items.Add(ow);
            }
            foreach (Warp w in ef.warps) {
                listBoxes[(int)serializationOrder.Warps].Items.Add(w);
            }
            foreach (Trigger t in ef.triggers) {
                listBoxes[(int)serializationOrder.Triggers].Items.Add(t);
            }

            foreach (CheckedListBox clb in listBoxes) {
                clb.SetAllItemsChecked(true);
            }
        }

        private void confirmButton_Click(object sender, EventArgs e) {
            bool ok = false;
            for (int i = 0; i < toImport.Length; i++) {
                if (toImport[i] && listBoxes[i].CheckedItems.Count > 0) {
                    userSelected[i] = listBoxes[i].CheckedIndices.Cast<int>().ToArray();
                    if (userSelected[i].Length > 0) {
                        ok = true;
                    }
                }
            }

            if (ok) {
                DialogResult = DialogResult.OK;
                this.Dispose();
                return;
            }
            MessageBox.Show("You must tick at least one element.", "No selection performed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void importCheckBoxChanged(object sender, EventArgs e) {
            //Changing the checkboxes' columns in the GUI will break this
            CheckBox c = sender as CheckBox;
            int typeIndex = tableLayoutPanel1.GetPositionFromControl(c).Column;
            bool v = toImport[typeIndex / 2] =
                listBoxes[typeIndex / 2].Enabled
                = c.Checked;

            for (int i = 0; i < 2; i++) {
                checkButtons[typeIndex / 2, i].Enabled = v;
            }
        }
        private void checkAllButtonClicked(object sender, EventArgs e) {
            listBoxes[tableLayoutPanel1.GetPositionFromControl(sender as Button).Column / 2].SetAllItemsChecked(true);
        }
        private void uncheckAllButtonClicked(object sender, EventArgs e) {
            listBoxes[tableLayoutPanel1.GetPositionFromControl(sender as Button).Column / 2].SetAllItemsChecked(false);
        }
    }
}
