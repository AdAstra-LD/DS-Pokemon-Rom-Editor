using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace DSPRE.CharMaps
{
    public partial class CharMapManagerForm : Form
    {

        private XmlDocument currentMap;
        Dictionary<ushort, string> decodeDict = new Dictionary<ushort, string>();
        Dictionary<string, ushort> aliasDict = new Dictionary<string, ushort>();

        private bool dirty = false;

        public CharMapManagerForm()
        {
            InitializeComponent();
            LoadCustomMap();
            ReadCurrentMap();
            PopulateListsFromDict();
        }

        private void SetDirty(bool isDirty)
        {
            dirty = isDirty;
            
            if (dirty)
            {
                this.Text = "Character Map Manager*";
            }
            else
            {
                this.Text = "Character Map Manager";
            }
        }

        private void LoadCustomMap()
        {
            // If file exists, try loading it
            if (!File.Exists(CharMapManager.customCharmapFilePath))
            {
                // File does not exist, this is okay and a valid state
                currentMap = null;
                EnableDisableControls(false);
                return;
            }

            try
            {
                currentMap = new XmlDocument();
                currentMap.PreserveWhitespace = true;
                currentMap.Load(CharMapManager.customCharmapFilePath);
                EnableDisableControls(true);
                return;
            }
            catch (Exception ex)
            {
                // File is somehow invalid, this should be considered a valid state
                currentMap = null;
                EnableDisableControls(false);
                AppLogger.Error("Failed to load custom charmap: " + ex.ToString());
                MessageBox.Show("Failed to load custom charmap: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void ReadCurrentMap()
        {
            charMapListBox.Items.Clear();
            aliasListBox.Items.Clear();
            codeComboBox.Items.Clear();
            codeComboBox.SelectedIndex = -1;
            decodeDict.Clear();

            SetDirty(false);

            if (currentMap == null)
            {
                return;
            }

            foreach (XmlNode entry in currentMap.SelectNodes("//entry"))
            {
                string codeString = entry.Attributes["code"]?.Value;
                string kind = entry.Attributes["kind"]?.Value;
                string text = entry.InnerText;

                if (codeString == null || kind == null || text == null)
                {
                    AppLogger.Warn("Found charmap entry with null value in custom map. Skipping.");
                    continue;
                }

                ushort code;

                if (!ushort.TryParse(codeString, System.Globalization.NumberStyles.HexNumber, null, out code))
                {
                    AppLogger.Error($"Invalid code value in charmap: {codeString}");
                    continue;
                }

                if (kind == "char")
                {
                    decodeDict[code] = text;
                }
                else if (kind == "alias")
                {
                    aliasDict[text] = code;
                }
            }
        }

        private void PopulateListsFromDict()
        {
            charMapListBox.Items.Clear();
            aliasListBox.Items.Clear();
            codeComboBox.Items.Clear();
            foreach (var kvp in decodeDict)
            {
                ushort code = kvp.Key;
                string value = kvp.Value;
                charMapListBox.Items.Add($"0x{code:X4} <-> {value}");
                codeComboBox.Items.Add($"0x{code:X4} <-> {value}");
            }
            foreach (var kvp in aliasDict)
            {
                string alias = kvp.Key;
                ushort code = kvp.Value;
                if (decodeDict.TryGetValue(code, out string originalValue))
                {
                    charMapListBox.Items.Add($"0x{code:X4} <- {alias} (alias)");
                    aliasListBox.Items.Add($"{alias} -> {originalValue} <-> 0x{code:X4}");
                }
            }
        }

        private void EnableDisableControls(bool enableControls)
        {
            addAliasButton.Enabled = enableControls;
            removeAliasButton.Enabled = enableControls;
            saveButton.Enabled = enableControls;
            deleteCustomMapButton.Enabled = enableControls;
        }

        private bool CheckUnsavedChanges()
        {
            if (dirty)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save them before proceeding?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    // For some reason perform click doesn't work here
                    saveButton_Click(null, null);
                    return true;
                }
                else if (result == DialogResult.No)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void addAliasButton_Click(object sender, EventArgs e)
        {
            if (currentMap == null)
            {
                return;
            }

            string alias = newAliasTextBox.Text.Trim();

            if (string.IsNullOrEmpty(alias))
            {
                MessageBox.Show("Alias name cannot be empty.", "Invalid Alias", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Warn if single character alias without brackets is used
            if (alias.Length == 1)
            {
                var result = MessageBox.Show("Unbracketed single character aliases are not recommended and may lead to encoding issues. " +
                    "Do you want to enclose the character in brackets?", "Single Character Alias", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    alias = "[" + alias + "]";
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            // Ensure that multi character aliased are enclosed in []
            else if (alias.Length > 1 && !(alias.StartsWith("[") && alias.EndsWith("]")))
            {
                alias = "[" + alias + "]";
            }

            // Check if alias already exists
            if (aliasDict.ContainsKey(alias))
            {
                MessageBox.Show("Alias already exists in the charmap.", "Duplicate Alias", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get selected code
            if (codeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a character code to alias.", "No Code Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedCodeStr = codeComboBox.SelectedItem.ToString().Split(' ')[0];
            ushort selectedCode = ushort.Parse(selectedCodeStr.Substring(2), System.Globalization.NumberStyles.HexNumber);

            // Add new alias entry to XML
            try
            {
                var root = currentMap.DocumentElement;
                XmlElement newEntry = currentMap.CreateElement("entry");
                newEntry.SetAttribute("code", selectedCode.ToString("X4"));
                newEntry.SetAttribute("kind", "alias");
                newEntry.InnerText = alias;

                // Find the last entry element
                XmlNode lastEntry = root.SelectSingleNode("entry[last()]");

                if (lastEntry != null)
                {
                    // Get the whitespace (newline + indent) before the last entry
                    XmlNode whitespaceBeforeLast = lastEntry.PreviousSibling;

                    // Insert: whitespace, then new entry
                    if (whitespaceBeforeLast != null && whitespaceBeforeLast.NodeType == XmlNodeType.Whitespace)
                    {
                        // Clone the whitespace pattern
                        XmlNode newWhitespace = currentMap.CreateTextNode(whitespaceBeforeLast.Value);
                        root.InsertAfter(newWhitespace, lastEntry);
                        root.InsertAfter(newEntry, newWhitespace);
                    }
                    else
                    {
                        // Fallback: add newline + 2 spaces
                        root.InsertAfter(currentMap.CreateTextNode("\n  "), lastEntry);
                        root.InsertAfter(newEntry, lastEntry.NextSibling);
                    }
                }
                else
                {
                    root.AppendChild(newEntry);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Failed to add alias to charmap: " + ex.ToString());
                MessageBox.Show("Failed to add alias to charmap: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Add alias to listbox and dictionary
            aliasDict[alias] = selectedCode;
            aliasListBox.Items.Add($"{alias} <-> 0x{selectedCode:X4}");
            aliasListBox.SelectedIndex = aliasListBox.Items.Count - 1;

            SetDirty(true);
        }

        private void removeAliasButton_Click(object sender, EventArgs e)
        {
            if (currentMap == null)
            {
                return;
            }

            if (aliasListBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an alias to remove.", "No Alias Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedAliasStr = aliasListBox.SelectedItem.ToString();
            string aliasName = selectedAliasStr.Split(' ')[0];

            // Find and remove the alias entry from XML
            try
            {
                var entryToRemove = currentMap.DocumentElement.SelectSingleNode($"entry[@kind='alias' and text()='{aliasName}']");
                if (entryToRemove != null)
                {
                    // Remove the whitespace after the entry (the newline following it)
                    // this is just to keep the XML tidy
                    XmlNode nextNode = entryToRemove.NextSibling;

                    entryToRemove.ParentNode.RemoveChild(entryToRemove);

                    // If next node was whitespace (newline + indent for next entry), remove it
                    if (nextNode != null && nextNode.NodeType == XmlNodeType.Whitespace)
                    {
                        nextNode.ParentNode.RemoveChild(nextNode);
                    }
                }
                else
                {
                    MessageBox.Show("Selected alias not found in charmap XML. " +
                        "This indicates some mismatch between display and the internal data.", "Alias Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Failed to remove alias from charmap: " + ex.ToString());
                MessageBox.Show("Failed to remove alias from charmap: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Remove alias from listbox and dictionary
            aliasListBox.Items.Remove(aliasListBox.SelectedItem);
            aliasListBox.SelectedIndex = aliasListBox.Items.Count - 1;

            aliasDict.Remove(aliasName);

            SetDirty(true);
        }

        private void createCustomMapButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(CharMapManager.customCharmapFilePath))
            {
                var result = MessageBox.Show("A custom charmap already exists. Do you want to overwrite it?", "Custom Charmap Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            CharMapManager.CreateCustomCharMapFile();
            LoadCustomMap();
            ReadCurrentMap();
            PopulateListsFromDict();
        }

        private void deleteCustomMapButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete the custom charmap? This action cannot be undone.", "Delete Custom Charmap", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                CharMapManager.DeleteCustomCharMapFile();
            }
        }

        private void reloadButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reload the custom charmap? All unsaved changes will be lost.", "Reload Custom Charmap", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                LoadCustomMap();
                ReadCurrentMap();
                PopulateListsFromDict();
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (currentMap == null)
            {
                return;
            }

            CharMapManager.SaveCustomCharMap(currentMap);
            SetDirty(false);
        }

        private void charMapListBox_DoubleClick(object sender, EventArgs e)
        {
            // Get clicked item
            if (charMapListBox.SelectedItem == null)
            {
                return;
            }

            string selectedItemStr = charMapListBox.SelectedItem.ToString();

            // Try to select corresponding code in combo box
            string codeStr = selectedItemStr.Split(' ')[0];

            for (int i = 0; i < codeComboBox.Items.Count; i++)
            {
                string comboItemStr = codeComboBox.Items[i].ToString();
                if (comboItemStr.StartsWith(codeStr))
                {
                    codeComboBox.SelectedIndex = i;
                    break;
                }
            }

            // Copy value to clipboard
            ushort code = ushort.Parse(codeStr.Substring(2), System.Globalization.NumberStyles.HexNumber);

            if (selectedItemStr.Contains("(alias)"))
            {
                // It's an alias, copy the alias name
                foreach (var kvp in aliasDict)
                {
                    if (kvp.Value == code)
                    {
                        Clipboard.SetText(kvp.Key);
                        break;
                    }
                }
            }
            else
            {
                // It's a normal char, copy the value
                if (decodeDict.TryGetValue(code, out string value))
                {
                    Clipboard.SetText(value);
                }
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();

            // Remove any filter if search term is empty
            if (string.IsNullOrEmpty(searchTerm))
            {
                PopulateListsFromDict();
                return;
            }

            // Filter charmap list based on search term
            charMapListBox.Items.Clear();
            foreach (var kvp in decodeDict)
            {
                ushort code = kvp.Key;
                string codeStr = $"0x{code.ToString("X4").ToLower()}";
                string value = kvp.Value;

                // If value contains search term or code matches, add to list
                if (value.Contains(searchTerm) || codeStr.Equals(searchTerm.ToLower()))
                {
                    charMapListBox.Items.Add($"0x{code:X4} <-> {value}");
                }
            }

            foreach (var kvp in aliasDict)
            {
                string alias = kvp.Key;
                ushort code = kvp.Value;
                if (decodeDict.TryGetValue(code, out string originalValue))
                {
                    string codeStr = $"0x{code.ToString("X4")}";
                    // If alias or original value contains search term or code matches, add to list
                    if (alias.Contains(searchTerm) || originalValue.Contains(searchTerm) || codeStr.Equals(searchTerm.ToLower()))
                    {
                        charMapListBox.Items.Add($"0x{code:X4} <- {alias} (alias)");
                    }
                }
            }

        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                searchButton.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void CharMapManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckUnsavedChanges())
            {
                e.Cancel = true;
            }
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            Helpers.OpenFileWithDefaultApp(CharMapManager.customCharmapFilePath);
        }
    }
}
