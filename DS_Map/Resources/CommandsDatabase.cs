using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DSPRE.Resources {
    public partial class CommandsDatabase : Form {
        private DataGridViewRow currentrow;
        private Dictionary<ushort, string> namesDict;
        private Dictionary<ushort, byte[]> paramsDict;
        private Dictionary<ushort, string> actionsDict;
        private Dictionary<ushort, string> comparisonOPsDict;

        private List<RadioButton> scriptRadioButtons;
        private List<RadioButton> actionRadioButtons;

        public CommandsDatabase(Dictionary<ushort, string> namesDict, Dictionary<ushort, byte[]> paramsDict, Dictionary<ushort, string> actionsDict,
            Dictionary<ushort, string> comparisonOPsDict) {
            this.namesDict = namesDict;
            this.paramsDict = paramsDict;

            this.actionsDict = actionsDict;
            this.comparisonOPsDict = comparisonOPsDict;

            InitializeComponent();
            SetupFromScriptDictionaries(scriptcmdDataGridView, paramsDict.Keys.Max(), namesDict, paramsDict);
            SetupFromScriptDictionaries(actionDataGridView, actionsDict.Keys.Max(), actionsDict);
            SetupFromScriptDictionaries(compOPDataGridView, comparisonOPsDict.Keys.Max(), comparisonOPsDict);

            scriptRadioButtons = new List<RadioButton>() {
                containsCBScripts,
                startsWithCBScripts,
                matchCBScripts
            };
            actionRadioButtons = new List<RadioButton>() {
                containsCBActions,
                startsWithCBActions,
                matchCBActions
            };
        }

        private void SetupFromScriptDictionaries(DataGridView table, int entriesCount, Dictionary<ushort, string> sourceNamesDict, Dictionary<ushort, byte[]> sourceParamsDict = null) {
            table.Rows.Clear();
            for (int i = 0; i < entriesCount; i++) {
                table.Rows.Add();
            }

            DataGridViewRowCollection list = table.Rows;
            for (ushort i = 0; i < list.Count; i++) { //loop through 
                DataGridViewRow r = list[i];
                ushort currentID = i;

                string buffer = "";
                sourceNamesDict.TryGetValue(i, out buffer);
                string commandName = buffer;

                r.Cells[0].Value = currentID.ToString("X4");
                r.Cells[1].Value = commandName;

                if (sourceParamsDict != null) {
                    var paramDictValues = sourceParamsDict.Values;
                    try {
                        if (paramDictValues.ElementAt(i)[0] == 0) {
                            r.Cells[2].Value = 0;
                        } else {
                            r.Cells[2].Value = paramDictValues.ElementAt(i).Length;//.ToString();
                        }
                    } catch { }

                    string paramSize = "";
                    try {
                        foreach (byte size in paramDictValues.ElementAt(i)) {
                            if (size != 0) {
                                paramSize += size + "B;  ";
                            }
                        }
                    } catch { }

                    table.Rows[i].Cells[3].Value = paramSize;
                }
            }
        }

        private void startSearchButtonScripts_Click(object sender, EventArgs e) {
            StartSearch(scriptcmdDataGridView, scriptcmdSearchTextBox, scriptRadioButtons);
        }
        private void startSearchButtonActions_Click(object sender, EventArgs e) {
            StartSearch(actionDataGridView, actioncmdSearchTextBox, actionRadioButtons);
        }
        private void StartSearch(DataGridView table, TextBox searchBox, List<RadioButton> rbl) {
            try {
                if (rbl[0].Checked) { //Contains
                    scanAllRows(table,
                        (x) => x.Value.ToString().IndexOf(searchBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0);
                } else if (rbl[1].Checked) { //StartsWith
                    scanAllRows(table, 
                        (x) => x.Value.ToString().StartsWith(searchBox.Text, StringComparison.InvariantCultureIgnoreCase));
                } else if (rbl[2].Checked) { //Exact Match
                    scanAllRows(table,
                        (x) => x.Value.ToString().Equals(searchBox.Text, StringComparison.InvariantCultureIgnoreCase));
                }
            } catch (OperationCanceledException) {
                table.ClearSelection();
                table.FirstDisplayedScrollingRowIndex = currentrow.Index;
                currentrow.Selected = true;
                return;
            }
        }

        private void scanAllRows(DataGridView table, Func<DataGridViewCell, bool> function) {
            for (int i = 0; i < table.Rows.Count; i++) {
                currentrow = table.Rows[i];

                if (currentrow.Cells[1].Value == null) {
                    continue;
                }
                try {
                    if (function(currentrow.Cells[1])) { //Cancel research when found
                        throw new OperationCanceledException();
                    }
                } catch (NullReferenceException) { }
            }
        }

        private void scriptcmdSearchTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                StartSearch(scriptcmdDataGridView, scriptcmdSearchTextBox, scriptRadioButtons);
            }
        }

        private void actioncmdSearchTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                StartSearch(actionDataGridView, actioncmdSearchTextBox, actionRadioButtons);
            }
        }
    }
}
