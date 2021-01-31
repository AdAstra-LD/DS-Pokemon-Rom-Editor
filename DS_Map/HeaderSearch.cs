using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DSPRE {
    public partial class HeaderSearch : Form
    {
        private List<string> searchableHeaderFieldsList = new List<string>() {
            "AreaData ID",
            "Camera Angle ID",
            "Event File ID",
            "Internal Name",
            "Level Script File ID",
            "Location Name",
            "Matrix ID",
            "Music Day ID",
            "Music Night ID",
            "Script File ID",
            "Text Archive ID",
            "Weather ID",
        };
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
            "Contains",
            "Does not contain"
        };

        private string[] searchableHeaderFields;
        private string[] headerSearchNumericOperators;
        private string[] headerSearchTextOperators;

        private List<string> intNames;

        public HeaderSearch(ref List<string> internalNames) {
            InitializeComponent();
            searchableHeaderFields = searchableHeaderFieldsList.ToArray();
            headerSearchNumericOperators = headerSearchNumericOperatorsList.ToArray();
            headerSearchTextOperators = headerSearchTextOperatorsList.ToArray();

            intNames = internalNames;

            fieldToSearch1ComboBox.Items.AddRange(searchableHeaderFields);
            fieldToSearch2ComboBox.Items.AddRange(searchableHeaderFields);

            fieldToSearch1ComboBox.SelectedIndex = 0;
            operator1ComboBox.SelectedIndex = 0;
        }

        private void fieldToSearch1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateOperators(operator1ComboBox, fieldToSearch1ComboBox);
        }

        private void operator1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void fieldToSearch2ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateOperators(operator2ComboBox, fieldToSearch2ComboBox);
        }

        private void operator2ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        #region Helper Methods
        private void UpdateOperators(ComboBox operatorComboBox, ComboBox fieldToSearchComboBox) {
            operatorComboBox.Items.Clear();

            if (fieldToSearchComboBox.SelectedItem.ToString().Contains("ID")) {
                operatorComboBox.Items.AddRange(headerSearchNumericOperators);
            } else {
                operatorComboBox.Items.AddRange(headerSearchTextOperators);
            }

            fieldToSearchComboBox.SelectedIndex = 0;
            operatorComboBox.SelectedIndex = 0;
        }
        #endregion

        private void startSearchButton_Click(object sender, EventArgs e) {
            for (short i = 0; i < intNames.Count; i++) {
                Header h = Header.LoadFromARM9(i);

            } 
        }
    }
}
