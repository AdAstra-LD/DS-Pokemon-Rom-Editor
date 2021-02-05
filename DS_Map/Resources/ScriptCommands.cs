using System;
using System.Linq;
using System.Windows.Forms;

namespace DSPRE.Resources {
    public partial class ScriptCommands : Form {
        private DataGridViewRow currentrow;

        public ScriptCommands() {
            InitializeComponent();
            SetupFromScriptDictionaries();
        }

        private void SetupFromScriptDictionaries() {
            for (int i = 0; i < RomInfo.scriptParametersDict.Count - 1; i++)
                scriptcmdDataGridView.Rows.Add();

            foreach (DataGridViewRow r in scriptcmdDataGridView.Rows) { //loop through 
                ushort u = (ushort)r.Index;

                r.Cells[0].Value = u.ToString("X4");

                try {
                    r.Cells[1].Value = RomInfo.scriptCommandNamesDict[u];
                } catch { }

                try {
                    if (RomInfo.scriptParametersDict[u][0] == 0) {
                        r.Cells[2].Value = 0;
                    } else {
                        r.Cells[2].Value = RomInfo.scriptParametersDict[u].Length;//.ToString();
                    }
                } catch { }

                string paramSize = "";
                try {
                    foreach (byte size in RomInfo.scriptParametersDict[u]) {
                        if (size != 0) {
                            paramSize += size + "B;  ";
                        }
                    }
                } catch { }

                scriptcmdDataGridView.Rows[u].Cells[3].Value = paramSize;
            }
        }

        private void startSearchButton_Click(object sender, EventArgs e) {
            try {
                if (containsCB.Checked)
                    scanAllRows(() => currentrow.Cells[1].Value.ToString().IndexOf(cmdSearchTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0);
                else if (startsWithCB.Checked)
                    scanAllRows(() => currentrow.Cells[1].Value.ToString().StartsWith(cmdSearchTextBox.Text, StringComparison.InvariantCultureIgnoreCase));
                else
                    scanAllRows(() => currentrow.Cells[1].Value.ToString().Equals(cmdSearchTextBox.Text, StringComparison.InvariantCultureIgnoreCase));
            } catch (OperationCanceledException) {
                scriptcmdDataGridView.ClearSelection();
                scriptcmdDataGridView.FirstDisplayedScrollingRowIndex = currentrow.Index;
                currentrow.Selected = true;
                return;
            }
        }

        private void scanAllRows(Func<bool> expression) {
            for (int i = 0; i < scriptcmdDataGridView.Rows.Count; i++) {
                currentrow = scriptcmdDataGridView.Rows[i];

                try {
                    if (expression()) { //Cancel research when found
                        throw new OperationCanceledException();
                    }
                } catch (NullReferenceException) { }
            }
        }

        private void cmdSearchTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                startSearchButton_Click(null, null);
            }
        }

    }
}
