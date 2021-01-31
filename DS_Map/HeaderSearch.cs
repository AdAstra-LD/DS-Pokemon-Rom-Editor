using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DSPRE {
    public partial class HeaderSearch : Form {
        private List<string> searchableHeaderFieldsList = new List<string>() {
            "Area Data ID",
            "Camera Angle ID",
            "Event File ID",
            "Internal Name",
            "Level Script ID",
            "Matrix ID",
            "Music Day ID",
            "Music Night ID",
            "Script File ID",
            "Text Archive ID",
            "Weather ID",
        };
        private bool propertyIsNumeric = false;

        private List<string> headerSearchNumericOperatorsList = new List<string>() {
            "Is Less than",
            "Equals",
            "Is Greater than",
            "Is Less than or Equal to",
            "Is Greater than or Equal to",
            "Is Different than",
        };
        private List<string> headerSearchTextOperatorsList = new List<string>() {
            "Is Exactly",
            "Is Not",
            "Contains",
            "Does not contain"
        };

        private string[] searchableHeaderFields;
        private string[] headerSearchNumericOperators;
        private string[] headerSearchTextOperators;

        private List<string> intNames;
        private ListBox headerListBox;

        public string status = "Ready";

        public HeaderSearch(ref List<string> internalNames, ListBox headerListBox) {
            InitializeComponent();
            searchableHeaderFields = searchableHeaderFieldsList.ToArray();
            headerSearchNumericOperators = headerSearchNumericOperatorsList.ToArray();
            headerSearchTextOperators = headerSearchTextOperatorsList.ToArray();

            intNames = internalNames;
            this.headerListBox = headerListBox;

            fieldToSearch1ComboBox.Items.AddRange(searchableHeaderFields);
            fieldToSearch1ComboBox.SelectedIndex = 0;
            operator1ComboBox.SelectedIndex = 0;
        }

        private void fieldToSearch1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateOperators(operator1ComboBox, fieldToSearch1ComboBox);
        }

        #region Helper Methods
        private void UpdateOperators(ComboBox operatorComboBox, ComboBox fieldToSearchComboBox) {
            operatorComboBox.Items.Clear();

            if (fieldToSearchComboBox.SelectedItem.ToString().Contains("ID")) {
                operatorComboBox.Items.AddRange(headerSearchNumericOperators);
                propertyIsNumeric = true;
            } else {
                operatorComboBox.Items.AddRange(headerSearchTextOperators);
                propertyIsNumeric = false;
            }

            operatorComboBox.SelectedIndex = 0;
        }
        #endregion
        public static List<string> advancedSearch(short startID, short finalID, List<string> intNames, string fieldToSearch, string oper, string valToSearch) {
            if (fieldToSearch == "" || oper == "" || valToSearch == "")
                return null;

            List<string> result = new List<string>();

            for (short i = startID; i < finalID; i++) {
                if (fieldToSearch.Equals("Internal Name")) {
                    if (oper.Equals("Is Exactly"))
                        if (intNames[i].Equals(valToSearch)) {
                            result.Add(i.ToString("D3") + Header.nameSeparator + intNames[i]);
                        }
                    if (oper.Equals("Is Not"))
                        if (!intNames[i].Equals(valToSearch)) {
                            result.Add(i.ToString("D3") + Header.nameSeparator + intNames[i]);
                        }
                    if (oper.Equals("Contains"))
                        if (intNames[i].Contains(valToSearch)) {
                            result.Add(i.ToString("D3") + Header.nameSeparator + intNames[i]);
                        }
                    if (oper.Equals("Does not contain"))
                        if (!intNames[i].Contains(valToSearch)) {
                            result.Add(i.ToString("D3") + Header.nameSeparator + intNames[i]);
                        }
                } else {
                    string[] fieldSplit = fieldToSearch.Split();
                    fieldSplit[0] = fieldSplit[0].ToLower();
                    string property = String.Join("", fieldSplit);
                    var headerFieldEntry = typeof(Header).GetProperty(property).GetValue(Header.LoadFromARM9(i), null);

                    int headerField = int.Parse(headerFieldEntry.ToString());
                    int numToSearch = int.Parse(valToSearch);

                    if (oper.Equals("Is Less than")) {
                        if (headerField < numToSearch) {
                            result.Add(i.ToString("D3") + Header.nameSeparator + intNames[i]);
                        }
                    } else if (oper.Equals("Equals")) {
                        if (headerField == numToSearch) {
                            result.Add(i.ToString("D3") + Header.nameSeparator + intNames[i]);
                        }
                    } else if (oper.Equals("Is Greater")) {
                        if (headerField > numToSearch) {
                            result.Add(i.ToString("D3") + Header.nameSeparator + intNames[i]);
                        }
                    } else if (oper.StartsWith("Is Less than or Equal")) {
                        if (headerField <= numToSearch) {
                            result.Add(i.ToString("D3") + Header.nameSeparator + intNames[i]);
                        }
                    } else if (oper.StartsWith("Is Greater than or Equal")) {
                        if (headerField >= numToSearch) {
                            result.Add(i.ToString("D3") + Header.nameSeparator + intNames[i]);
                        }
                    } else if (oper.StartsWith("Is Different")) {
                        if (headerField != numToSearch) {
                            result.Add(i.ToString("D3") + Header.nameSeparator + intNames[i]);
                        }
                    }
                }
            }
            return result;
        }
        private void startSearchButton_Click(object sender, EventArgs e) {
            headerListBox.Items.Clear();
            List<string> result;

            try {
                result = advancedSearch(0, (short)intNames.Count, intNames, fieldToSearch1ComboBox.Text, operator1ComboBox.SelectedItem.ToString(), value1TextBox.Text);
            } catch (FormatException) {
                MessageBox.Show("Make sure the value to search is correct.", "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                headerListBox.Items.Add("Search parameters are invalid");
                headerListBox.Enabled = false;
                return;
            }

            if (result == null) {
                if (value1TextBox.Text == "") {
                    headerListBox.Items.Add("Value to search is empty");
                } else {
                    headerListBox.Items.Add("No result for " + '"' + value1TextBox.Text + '"');
                }
                headerListBox.Enabled = false;
            } else {
                headerListBox.Items.AddRange(result.ToArray());
                headerListBox.SelectedIndex = 0;
                headerListBox.Enabled = true;

                status = "Showing headers whose " + fieldToSearch1ComboBox.Text + " " + operator1ComboBox.Text.ToLower() + " " + '"' + value1TextBox.Text + '"';
                this.Dispose();
            }
        }
    }
}
