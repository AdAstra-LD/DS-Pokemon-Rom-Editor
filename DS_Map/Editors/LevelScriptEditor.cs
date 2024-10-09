using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DSPRE.ROMFiles;

namespace DSPRE.Editors {
    public partial class LevelScriptEditor : UserControl {
        public bool levelScriptEditorIsReady { get; set; } = false;
        LevelScriptFile _levelScriptFile;
        MainProgram _parent;

        public LevelScriptEditor() {
            InitializeComponent();

        }

        public void SetUpLevelScriptEditor(MainProgram parent, bool force = false) {
            if (levelScriptEditorIsReady && !force){ return; }
            levelScriptEditorIsReady = true;
            this._parent = parent;
            DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames> { RomInfo.DirNames.scripts }); //12 = scripts Narc Dir
            populate_selectScriptFileComboBox();
        }

        public void OpenLevelScriptEditor(MainProgram parent, int levelScriptID) {

            SetUpLevelScriptEditor(parent);

            selectScriptFileComboBox.SelectedIndex = levelScriptID;
            EditorPanels.mainTabControl.SelectedTab = EditorPanels.levelScriptEditorTabPage;
        }

        private void populate_selectScriptFileComboBox(int selectedIndex = 0) {
            selectScriptFileComboBox.Items.Clear();
            int scriptCount = Filesystem.GetScriptCount();
            for (int i = 0; i < scriptCount; i++) {
                // ScriptFile currentScriptFile = new ScriptFile(i, true, true);
                // selectScriptFileComboBox.Items.Add(currentScriptFile);
                selectScriptFileComboBox.Items.Add($"Script File {i}");
            }

            selectScriptFileComboBox.SelectedIndex = selectedIndex;
        }

        void disableButtons() {
            listBoxTriggers.DataSource = null;

            textBoxScriptID.Clear();
            textBoxVariableName.Clear();
            textBoxVariableValue.Clear();

            radioButtonVariableValue.Checked = false;
            radioButtonMapChange.Checked = false;
            radioButtonScreenReset.Checked = false;
            radioButtonLoadGame.Checked = false;

            textBoxScriptID.Enabled = false;

            radioButtonVariableValue.Enabled = false;
            radioButtonMapChange.Enabled = false;
            radioButtonScreenReset.Enabled = false;
            radioButtonLoadGame.Enabled = false;

            radioButtonAuto.Enabled = false;
            radioButtonHex.Enabled = false;
            radioButtonDecimal.Enabled = false;

            buttonImport.Enabled = false;
            buttonSave.Enabled = false;
            buttonExport.Enabled = false;
            checkBoxPadding.Enabled = false;

            buttonAdd.Enabled = false;
            buttonRemove.Enabled = false;
        }

        void enableButtons() {
            // textBoxScriptID.Enabled = true;
            // textBoxVariableName.Enabled = true;
            // textBoxVariableValue.Enabled = true;

            radioButtonVariableValue.Enabled = true;
            radioButtonMapChange.Enabled = true;
            radioButtonScreenReset.Enabled = true;
            radioButtonLoadGame.Enabled = true;

            radioButtonAuto.Enabled = true;
            radioButtonHex.Enabled = true;
            radioButtonDecimal.Enabled = true;

            buttonImport.Enabled = true;
            buttonSave.Enabled = true;
            buttonExport.Enabled = true;
            checkBoxPadding.Enabled = true;
        }

        void buttonAdd_logic() {
            buttonAdd.Enabled = false;

            if (radioButtonVariableValue.Checked) {
                if (!string.IsNullOrEmpty(textBoxScriptID.Text) && !string.IsNullOrEmpty(textBoxVariableName.Text) && !string.IsNullOrEmpty(textBoxVariableValue.Text)) {
                    buttonAdd.Enabled = true;
                }
            } else if (radioButtonMapChange.Checked || radioButtonScreenReset.Checked || radioButtonLoadGame.Checked) {
                if (!string.IsNullOrEmpty(textBoxScriptID.Text)) {
                    buttonAdd.Enabled = true;
                }
            }
        }

        private void selectScriptFileComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (selectScriptFileComboBox.SelectedIndex == -1) {
                buttonOpenSelectedScript.Enabled = false;
                buttonOpenHeaderScript.Enabled = false;
                buttonLocate.Enabled = false;
            } else {
                buttonOpenSelectedScript.Enabled = true;
                buttonOpenHeaderScript.Enabled = true;
                buttonLocate.Enabled = true;
            }

            disableButtons();

            try {
                _levelScriptFile = new LevelScriptFile(selectScriptFileComboBox.SelectedIndex);

                listBoxTriggers.DataSource = _levelScriptFile.bufferSet;
                if (listBoxTriggers.Items.Count > 0){ listBoxTriggers.SelectedIndex = 0; }
                // Check for 318767104
                enableButtons();
            } catch (InvalidDataException ex) { //not a level script
                disableButtons();
                Console.WriteLine(ex.Message);
            }
        }

        void listBoxTriggers_SelectedValueChanged(object sender, EventArgs e) {
            if (listBoxTriggers.SelectedItem == null) {
                buttonRemove.Enabled = false;
                return;
            }

            if (listBoxTriggers.SelectedItem is MapScreenLoadTrigger mapScreenLoadTrigger) {
                if (mapScreenLoadTrigger.triggerType == LevelScriptTrigger.LOADGAME) {
                    radioButtonLoadGame.Checked = true;
                } else if (mapScreenLoadTrigger.triggerType == LevelScriptTrigger.MAPCHANGE) {
                    radioButtonMapChange.Checked = true;
                } else if (mapScreenLoadTrigger.triggerType == LevelScriptTrigger.SCREENRESET) {
                    radioButtonScreenReset.Checked = true;
                }
            } else if (listBoxTriggers.SelectedItem is VariableValueTrigger variableValueTrigger) {
                if (variableValueTrigger.triggerType == LevelScriptTrigger.VARIABLEVALUE) {
                    radioButtonVariableValue.Checked = true;
                }
            }

            handleAutoFormat();
            handleHexFormat();
            handleDecimalFormat();

            textBoxScriptID.Enabled = true;
            buttonRemove.Enabled = true;
        }

        private void buttonAdd_Click(object sender, EventArgs e) {
            // try {
            if (_levelScriptFile == null) {
                _levelScriptFile = new LevelScriptFile();
            }

            int convertBase = 10; //decimal
            if (radioButtonHex.Checked) {
                convertBase = 16; //hex
            }

            if (radioButtonVariableValue.Checked) {
                int scriptID = Convert.ToInt16(textBoxScriptID.Text, convertBase);
                int variableName = Convert.ToInt16(textBoxVariableName.Text, convertBase);
                int variableValue = Convert.ToInt16(textBoxVariableValue.Text, convertBase);
                VariableValueTrigger variableValueTrigger = new VariableValueTrigger(scriptID, variableName, variableValue);
                _levelScriptFile.bufferSet.Add(variableValueTrigger);
            } else {
                int scriptID = Convert.ToInt16(textBoxScriptID.Text, convertBase);
                if (radioButtonMapChange.Checked) {
                    MapScreenLoadTrigger mapScreenLoadTrigger = new MapScreenLoadTrigger(LevelScriptTrigger.MAPCHANGE, scriptID);
                    _levelScriptFile.bufferSet.Add(mapScreenLoadTrigger);
                } else if (radioButtonScreenReset.Checked) {
                    MapScreenLoadTrigger mapScreenLoadTrigger = new MapScreenLoadTrigger(LevelScriptTrigger.SCREENRESET, scriptID);
                    _levelScriptFile.bufferSet.Add(mapScreenLoadTrigger);
                } else if (radioButtonLoadGame.Checked) {
                    MapScreenLoadTrigger mapScreenLoadTrigger = new MapScreenLoadTrigger(LevelScriptTrigger.LOADGAME, scriptID);
                    _levelScriptFile.bufferSet.Add(mapScreenLoadTrigger);
                }
            }

            textBoxScriptID.Clear();
            textBoxVariableName.Clear();
            textBoxVariableValue.Clear();
            // }
            // catch (Exception exception) {
            //   MessageBox.Show(exception.Message);
            // }
        }

        private void buttonRemove_Click(object sender, EventArgs e) {
            _levelScriptFile.bufferSet.RemoveAt(listBoxTriggers.SelectedIndex);
        }

        private void buttonOpenHeaderScript_Click(object sender, EventArgs e) {
            EditorPanels.scriptEditor.OpenScriptEditor(this._parent, (int)EditorPanels.MainProgram.scriptFileUpDown.Value);
        }

        private void buttonOpenSelectedScript_Click(object sender, EventArgs e) {
            EditorPanels.scriptEditor.OpenScriptEditor(this._parent, (int)EditorPanels.levelScriptEditor.selectScriptFileComboBox.SelectedIndex);
        }

        void buttonLocate_Click(object sender, EventArgs e) {
            if (_levelScriptFile == null){ return; }
            string path = Filesystem.GetScriptPath(_levelScriptFile.ID);
            Helpers.ExplorerSelect(path);
        }

        void buttonImport_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK) {
                try {
                    LevelScriptFile importedFile = new LevelScriptFile();
                    importedFile.parse_file(ofd.FileName);
                    _levelScriptFile.bufferSet.Clear();
                    foreach (LevelScriptTrigger trigger in importedFile.bufferSet) {
                        _levelScriptFile.bufferSet.Add(trigger);
                    }
                } catch (InvalidDataException ex) {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e) {
            string path = Filesystem.GetScriptPath(_levelScriptFile.ID);
            saveFile(path);
        }

        private void buttonExport_Click(object sender, EventArgs e) {
            SaveFileDialog sfd = new SaveFileDialog();
            try {
                sfd.InitialDirectory = Path.GetDirectoryName(sfd.FileName);
                sfd.FileName = Path.GetFileName(sfd.FileName);
            } catch (Exception ex) {
                sfd.InitialDirectory = Path.GetDirectoryName(Environment.SpecialFolder.UserProfile.ToString());
                sfd.FileName = Path.GetFileName(sfd.FileName);
            }

            if (sfd.ShowDialog() == DialogResult.OK) {
                saveFile(sfd.FileName);
            }
        }

        void saveFile(string path) {
            try {
                long bytes_written = _levelScriptFile.write_file(path);
                if (bytes_written <= 4) {
                    MessageBox.Show("Empty level script file was correctly saved.", "Success!");
                } else {
                    MessageBox.Show("File was correctly saved.", "Success!");
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }

        private void handleAutoFormat() {
            if (!radioButtonAuto.Checked){ return; }

            textBoxScriptID.Clear();
            textBoxVariableName.Clear();
            textBoxVariableValue.Clear();

            if (listBoxTriggers.SelectedItem is MapScreenLoadTrigger mapScreenLoadTrigger) {
                textBoxScriptID.Text = mapScreenLoadTrigger.scriptTriggered.ToString();
            } else if (listBoxTriggers.SelectedItem is VariableValueTrigger variableValueTrigger) {
                textBoxScriptID.Text = variableValueTrigger.scriptTriggered.ToString();
                textBoxVariableName.Text = "" + variableValueTrigger.variableToWatch.ToString("D");
                textBoxVariableValue.Text = "" + variableValueTrigger.expectedValue.ToString("D");
            }
        }

        private void handleHexFormat() {
            if (!radioButtonHex.Checked){ return; }

            textBoxScriptID.Clear();
            textBoxVariableName.Clear();
            textBoxVariableValue.Clear();

            if (listBoxTriggers.SelectedItem is MapScreenLoadTrigger mapScreenLoadTrigger) {
                textBoxScriptID.Text = mapScreenLoadTrigger.scriptTriggered.ToString();
            } else if (listBoxTriggers.SelectedItem is VariableValueTrigger variableValueTrigger) {
                textBoxScriptID.Text = variableValueTrigger.scriptTriggered.ToString();
                textBoxVariableName.Text = "0x" + variableValueTrigger.variableToWatch.ToString("X");
                textBoxVariableValue.Text = "0x" + variableValueTrigger.expectedValue.ToString("X");
            }
        }

        private void handleDecimalFormat() {
            if (!radioButtonDecimal.Checked){ return; }

            textBoxScriptID.Clear();
            textBoxVariableName.Clear();
            textBoxVariableValue.Clear();

            if (listBoxTriggers.SelectedItem is MapScreenLoadTrigger mapScreenLoadTrigger) {
                textBoxScriptID.Text = mapScreenLoadTrigger.scriptTriggered.ToString();
            } else if (listBoxTriggers.SelectedItem is VariableValueTrigger variableValueTrigger) {
                textBoxScriptID.Text = variableValueTrigger.scriptTriggered.ToString();
                textBoxVariableName.Text = "" + variableValueTrigger.variableToWatch.ToString("D");
                textBoxVariableValue.Text = "" + variableValueTrigger.expectedValue.ToString("D");
            }
        }

        private void radioButtonAuto_CheckedChanged(object sender, EventArgs e) {
            handleAutoFormat();
        }

        private void radioButtonHex_CheckedChanged(object sender, EventArgs e) {
            handleHexFormat();
        }

        private void radioButtonDecimal_CheckedChanged(object sender, EventArgs e) {
            handleDecimalFormat();
        }
        private void AssignGroupBoxScriptText() {
            if (radioButtonVariableValue.Checked) {
                groupBoxScript.Text = "Keep running this Script";
            } else {
                groupBoxScript.Text = "Run this Script";
            }
        }

        private void radioButtonVariableValue_CheckedChanged(object sender, EventArgs e) {
            groupBoxVariable.Visible = true;
            groupBoxValue.Visible = true;
            buttonAdd_logic();
            AssignGroupBoxScriptText();
        }

        private void radioButtonMapChange_CheckedChanged(object sender, EventArgs e) {
            groupBoxVariable.Visible = false;
            groupBoxValue.Visible = false;
            buttonAdd_logic();
            AssignGroupBoxScriptText();
        }

        private void radioButtonScreenReset_CheckedChanged(object sender, EventArgs e) {
            groupBoxVariable.Visible = false;
            groupBoxValue.Visible = false;
            buttonAdd_logic();
            AssignGroupBoxScriptText();
        }

        private void radioButtonLoadGame_CheckedChanged(object sender, EventArgs e) {
            groupBoxVariable.Visible = false;
            groupBoxValue.Visible = false;
            buttonAdd_logic();
            AssignGroupBoxScriptText();
        }

        void textBoxScriptID_TextChanged(object sender, EventArgs e) {
            buttonAdd_logic();
        }

        void textBoxVariableName_TextChanged(object sender, EventArgs e) {
            buttonAdd_logic();
        }

        void textBoxVariableValue_TextChanged(object sender, EventArgs e) {
            buttonAdd_logic();
        }
    }
}
