using DSPRE.ROMFiles;
using System.Collections.Generic;

using System;
using System.Windows.Forms;
using static DSPRE.RomInfo;
using System.Reflection;
using System.Linq;

namespace DSPRE {

    public partial class HeaderSearch : Form {
        public static readonly Dictionary<MapHeader.SearchableFields, string> searchableHeaderFieldsDict = new Dictionary<MapHeader.SearchableFields, string>() {
            [MapHeader.SearchableFields.AreaDataID] = "Area Data (ID)",
            [MapHeader.SearchableFields.CameraAngleID] = "Camera Angle (ID)",
            [MapHeader.SearchableFields.EventFileID] = "Event File (ID)",
            [MapHeader.SearchableFields.InternalName] = "Internal Name",
            [MapHeader.SearchableFields.LevelScriptID] = "Level Script (ID)",
            [MapHeader.SearchableFields.MatrixID] = "Matrix (ID)",
            [MapHeader.SearchableFields.MusicDayID] = "Music Day (ID)",
            //[MapHeader.SearchableFields.MusicDayName] = "Music Day (Name)",
            [MapHeader.SearchableFields.MusicNightID] = "Music Night (ID)",
            //[MapHeader.SearchableFields.MusicNightName] = "Music Night (Name)",
            [MapHeader.SearchableFields.ScriptFileID] = "Script File (ID)",
            [MapHeader.SearchableFields.TextArchiveID] = "Text Archive (ID)",
            [MapHeader.SearchableFields.WeatherID] = "Weather (ID)"
        };
        public enum NumOperators : byte {
            //Order matters!
            Equal,
            Different,
            Less,
            Greater,
            LessOrEqual,
            GreaterOrEqual
        };

        public enum TextOperators : byte {
            //Order matters!
            Contains,
            DoesNotContain,
            IsExactly,
            IsNot
        };

        private static readonly Dictionary<NumOperators, string> numOperatorsDict = new Dictionary<NumOperators, string>() {
            //Order matters!

            [NumOperators.Equal] = "Equals",
            [NumOperators.Different] = "Is Different than",
            [NumOperators.Less] = "Is Less than",
            [NumOperators.Greater] = "Is Greater than",
            [NumOperators.LessOrEqual] = "Is Less than or Equal to",
            [NumOperators.GreaterOrEqual] = "Is Greater than or Equal to",
        };

        private static readonly Dictionary<TextOperators, string> textOperatorsDict = new Dictionary<TextOperators, string>() {
            //Order matters!

            [TextOperators.Contains] = "Contains",
            [TextOperators.DoesNotContain] = "Does not contain",
            [TextOperators.IsExactly] = "Is Exactly",
            [TextOperators.IsNot] = "Is Not",
        };

        private List<string> intNames;
        private ListBox headerListBox;
        private ToolStripStatusLabel statusLabel;

        public string status = "Ready";

        public HeaderSearch(ref List<string> internalNames, ListBox headerListBox, ToolStripStatusLabel statusLabel) {
            InitializeComponent();

            intNames = internalNames;
            this.headerListBox = headerListBox;
            this.statusLabel = statusLabel;

            foreach (string elem in searchableHeaderFieldsDict.Values) {
                fieldToSearch1ComboBox.Items.Add(elem);
            }

            fieldToSearch1ComboBox.SelectedIndex = 0;
            operator1ComboBox.SelectedIndex = 0;
        }

        #region Helper Methods
        private void UpdateOperators(ComboBox operatorComboBox, ComboBox fieldToSearchComboBox) {
            operatorComboBox.Items.Clear();

            if (fieldToSearchComboBox.SelectedItem.ToString().Contains("ID")) {
                foreach (string elem in numOperatorsDict.Values) {
                    operatorComboBox.Items.Add(elem);
                }
                valueTextBox.MaxLength = 5;
            } else {
                foreach (string elem in textOperatorsDict.Values) {
                    operatorComboBox.Items.Add(elem);
                }
                valueTextBox.MaxLength = 16;
            }

            operatorComboBox.SelectedIndex = 0;
        }
        #endregion
        public static void ResetResults(ListBox headerListBox, List<string> intNames, bool prependNumbers) {
            if (headerListBox.Items.Count < intNames.Count) {

                headerListBox.Enabled = true;
                headerListBox.Items.Clear();
                
                if (prependNumbers) {
                    for (int i = 0; i < intNames.Count; i++) {
                        string name = intNames[i];
                        headerListBox.Items.Add(i.ToString("D3") + MapHeader.nameSeparator + name);
                    }
                } else {
                    headerListBox.Items.AddRange(intNames.ToArray());
                }
            }
        }
        public static HashSet<string> AdvancedSearch(ushort startID, ushort finalID, List<string> intNames, int fieldToSearch, int oper, string valToSearch) {
            if (fieldToSearch < 0 || oper < 0 || valToSearch == "") {
                return null;
            }

            HashSet<string> result = new HashSet<string>();

            switch (fieldToSearch) {
                case (int)MapHeader.SearchableFields.InternalName:
                    for (ushort i = startID; i < finalID; i++) {
                        switch (oper) {
                            case (int)TextOperators.IsExactly:
                                if (intNames[i].Equals(valToSearch)) {
                                    result.Add(i.ToString("D3") + MapHeader.nameSeparator + intNames[i]);
                                }
                                break;
                            case (int)TextOperators.IsNot:
                                if (!intNames[i].Equals(valToSearch)) {
                                    result.Add(i.ToString("D3") + MapHeader.nameSeparator + intNames[i]);
                                }
                                break;
                            case (int)TextOperators.Contains:
                                if (intNames[i].IndexOf(valToSearch, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                                    result.Add(i.ToString("D3") + MapHeader.nameSeparator + intNames[i]);
                                }
                                break;
                            case (int)TextOperators.DoesNotContain:
                                if (intNames[i].IndexOf(valToSearch, StringComparison.InvariantCultureIgnoreCase) < 0) {
                                    result.Add(i.ToString("D3") + MapHeader.nameSeparator + intNames[i]);
                                }
                                break;
                            default:
                                AppLogger.Error("Unrecognized operand!!!");
                                break;
                        }
                    }
                    break;
                //case (int)MapHeader.SearchableFields.MusicDayName:
                    //Maybe in the future
                    //break;
                //case (int)MapHeader.SearchableFields.MusicNightName:
                    //Maybe in the future
                    //break;
                default:
                    string[] fieldSplit = searchableHeaderFieldsDict[(MapHeader.SearchableFields)fieldToSearch].Split();

                    fieldSplit[0] = fieldSplit[0].ToLower();
                    fieldSplit[fieldSplit.Length - 1] = fieldSplit[fieldSplit.Length - 1].Replace("(", "").Replace(")", ""); //Remove ( and ) from string

                    PropertyInfo property = typeof(MapHeader).GetProperty(String.Join("", fieldSplit));
                    ushort numToSearch;

                    try {
                        numToSearch = ushort.Parse(valToSearch);
                    } catch (OverflowException) {
                        MessageBox.Show("Your input exceeds the range of 16-bit integers (" + ushort.MinValue + " - " + ushort.MaxValue + ").", "Overflow Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return null;
                    }

                    for (ushort i = startID; i < finalID; i++) {
                        MapHeader h;
                        if (PatchToolboxDialog.flag_DynamicHeadersPatchApplied || PatchToolboxDialog.CheckFilesDynamicHeadersPatchApplied()) {
                            h = MapHeader.LoadFromFile(RomInfo.gameDirs[DirNames.dynamicHeaders].unpackedDir + "\\" + i.ToString("D4"), i, 0);
                        } else {
                            h = MapHeader.LoadFromARM9(i);
                        }
                        
                        int headerField = int.Parse(property.GetValue(h, null).ToString());

                        switch (oper) {
                            case (int)NumOperators.Less:
                                if (headerField < numToSearch) {
                                    result.Add(i.ToString("D3") + MapHeader.nameSeparator + intNames[i]);
                                }
                                break;
                            case (int)NumOperators.Equal:
                                if (headerField == numToSearch) {
                                    result.Add(i.ToString("D3") + MapHeader.nameSeparator + intNames[i]);
                                }
                                break;
                            case (int)NumOperators.Greater: 
                                if (headerField > numToSearch) {
                                    result.Add(i.ToString("D3") + MapHeader.nameSeparator + intNames[i]);
                                }
                                break;
                            case (int)NumOperators.LessOrEqual:
                                if (headerField <= numToSearch) {
                                    result.Add(i.ToString("D3") + MapHeader.nameSeparator + intNames[i]);
                                }
                                break;
                            case (int)NumOperators.GreaterOrEqual:
                                if (headerField >= numToSearch) {
                                    result.Add(i.ToString("D3") + MapHeader.nameSeparator + intNames[i]);
                                }
                                break;
                            case (int)NumOperators.Different:
                                if (headerField != numToSearch) {
                                    result.Add(i.ToString("D3") + MapHeader.nameSeparator + intNames[i]);
                                }
                                break;
                            default:
                                AppLogger.Error("Unrecognized operand!!!");
                                break;
                        }
                    }
                    break;
            }
            return result;
        }
        private void startSearchButton_Click(object sender, EventArgs e) {
            StartSearch(showDialog: true);
        }

        private void StartSearch(bool showDialog = true) {
            if (valueTextBox.Text == "") {
                //if (showDialog) {
                //    MessageBox.Show("Value to search is empty", "Can't search", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //}
                headerSearchResetButton_Click(null, null);
                return;
            }

            HashSet<string> result;
            headerListBox.Items.Clear();
            
            try {
                result = AdvancedSearch(0, (ushort)intNames.Count, intNames, fieldToSearch1ComboBox.SelectedIndex, operator1ComboBox.SelectedIndex, valueTextBox.Text);
            } catch (FormatException) {
                if (showDialog) {
                    MessageBox.Show("Make sure the value to search is correct.", "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                valueTextBox.Clear();
                headerListBox.Items.Add("Search parameters are invalid");
                headerListBox.Enabled = false;
                return;
            }

            string searchConfiguration = fieldToSearch1ComboBox.Text + " " + operator1ComboBox.Text.ToLower() + " " + '"' + valueTextBox.Text + '"';
            if (result is null || result.Count <= 0) {
                string res = "No header's " + searchConfiguration;
                headerListBox.Items.Add(res);
                headerListBox.Enabled = false;
                statusLabel.Text = res;
            } else {
                string[] arr = new string[result.Count];
                result.CopyTo(arr);
                headerListBox.Items.AddRange(arr);
                headerListBox.SelectedIndex = 0;
                headerListBox.Enabled = true;

                statusLabel.Text = "Showing headers whose " + searchConfiguration;
            }
            Update();
        }

        private void valueTextBox_KeyUp(object sender, KeyEventArgs e) {
            if (autoSearchCB.Checked) {
                StartSearch(showDialog: false);
            } else if (e.KeyCode == Keys.Enter) {
                StartSearch(showDialog: true);
            }    
        }
        private void headerSearchResetButton_Click(object sender, EventArgs e) {
            ResetResults(headerListBox, intNames, prependNumbers: true);
            valueTextBox.Clear();
            statusLabel.Text = "Ready";
        }
        private void fieldToSearch1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateOperators(operator1ComboBox, fieldToSearch1ComboBox);
            if (autoSearchCB.Checked) {
                StartSearch(showDialog: false);
            }
        }
        private void operator1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (autoSearchCB.Checked) {
                StartSearch(showDialog: false);
            }
        }
    }
}
