using DSPRE.ROMFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DSPRE.RomInfo;
using static Tao.Platform.Windows.Winmm;

namespace DSPRE.Editors
{
    public partial class TextEditor : UserControl
    {
        
        public TextEditor()
        {
            InitializeComponent();
            this.textSearchResultsListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.textSearchResultsListBox_GoToEntryResult);

        }

        #region Text Editor

        #region Variables
        MainProgram _parent;
        public bool textEditorIsReady { get; set; } = false;

        public TextArchive currentTextArchive;
        private Dictionary<string, Color> highlightPatterns = new Dictionary<string, Color>
        {
            {"{STRVAR[^}]*}", Color.Blue},
            {"{YESNO[^}]*}", Color.Green},
            {"{PAUSE[^}]*}", Color.Green},
            {"{WAIT[^}]*}", Color.Green},
            {"{CURSOR[^}]*}", Color.Green},
            {"{ALN[^}]*}", Color.Green},
            {"{UNK[^}]*}", Color.Red},
            {"{COLOR[^}]*}", Color.Gray},
            {"{SIZE[^}]*}", Color.Green}
        };
        #endregion

        #region syntax highlighting
        private string ApplyHighlightMarkers(string text)
        {
            string markedText = text;
            foreach (var pattern in highlightPatterns)
            {
                markedText = System.Text.RegularExpressions.Regex.Replace(markedText, pattern.Key, match =>
                    $"[COLOR={pattern.Value.Name.ToLower()}]{match.Value}[/COLOR]");
            }
            //return markedText;
            return text;
        }

        private void textEditorDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 0) return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border | DataGridViewPaintParts.Focus | DataGridViewPaintParts.SelectionBackground);

            using (SolidBrush clearBrush = new SolidBrush(e.CellStyle.BackColor))
            {
                e.Graphics.FillRectangle(clearBrush, e.CellBounds);
            }

            string cellText = e.FormattedValue?.ToString() ?? "";
            Rectangle rect = e.CellBounds;
            int x = rect.X + 2;
            int y = rect.Y + 2;

            using (Brush brush = new SolidBrush(e.CellStyle.ForeColor))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near })
            {
                int pos = 0;
                while (pos < cellText.Length)
                {
                    int colorStart = cellText.IndexOf("[COLOR=", pos, StringComparison.Ordinal);
                    if (colorStart < 0)
                    {
                        // No more color tags, draw remaining text
                        e.Graphics.DrawString(cellText.Substring(pos), e.CellStyle.Font, brush, x, y, sf);
                        break;
                    }

                    // Draw text before color tag
                    if (colorStart > pos)
                    {
                        e.Graphics.DrawString(cellText.Substring(pos, colorStart - pos), e.CellStyle.Font, brush, x, y, sf);
                        x += (int)e.Graphics.MeasureString(cellText.Substring(pos, colorStart - pos), e.CellStyle.Font).Width;
                    }

                    // Parse color tag
                    int colorEnd = cellText.IndexOf("]", colorStart);
                    if (colorEnd < 0) break;
                    string colorName = cellText.Substring(colorStart + 7, colorEnd - colorStart - 7).ToLower();
                    Color color = Color.FromName(colorName) == Color.Empty ? e.CellStyle.ForeColor : Color.FromName(colorName);

                    // Find end of colored text
                    int textEnd = cellText.IndexOf("[/COLOR]", colorEnd);
                    if (textEnd < 0) break;
                    string coloredText = cellText.Substring(colorEnd + 1, textEnd - colorEnd - 1);

                    // Draw colored text
                    using (Brush colorBrush = new SolidBrush(color))
                    {
                        e.Graphics.DrawString(coloredText, e.CellStyle.Font, colorBrush, x, y, sf);
                        x += (int)e.Graphics.MeasureString(coloredText, e.CellStyle.Font).Width;
                    }

                    pos = textEnd + 8; // Skip [/COLOR]
                }
            }

            e.Handled = true; // Prevent default painting
        }

        private void textEditorDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (Helpers.HandlersDisabled || e.RowIndex < 0 || e.ColumnIndex < 0) return;

            try
            {
                string originalText = textEditorDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";
                //string cleanedText = System.Text.RegularExpressions.Regex.Replace(originalText, @"\[COLOR=[a-z]+\](.*?)\[/COLOR\]", "$1");
                string cleanedText = originalText;
                currentTextArchive.messages[e.RowIndex] = cleanedText;
                textEditorDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = ApplyHighlightMarkers(cleanedText); // Reapply markers
            }
            catch (NullReferenceException)
            {
                currentTextArchive.messages[e.RowIndex] = "";
                textEditorDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = ApplyHighlightMarkers("");
            }
        }

        #endregion

        private void addTextArchiveButton_Click(object sender, EventArgs e)
        {
            /* Add copy of message 0 to text archives folder */
            new TextArchive(selectTextFileComboBox.Items.Count, new List<string>() { "Your text here." }, discardLines: true).SaveToFileDefaultDir(selectTextFileComboBox.Items.Count);

            /* Update ComboBox and select new file */
            selectTextFileComboBox.Items.Add("Text Archive " + selectTextFileComboBox.Items.Count);
            selectTextFileComboBox.SelectedIndex = selectTextFileComboBox.Items.Count - 1;
        }

        private void locateCurrentTextArchive_Click(object sender, EventArgs e)
        {
            Helpers.ExplorerSelect(Path.Combine(gameDirs[DirNames.textArchives].unpackedDir, EditorPanels.textEditor.currentTextArchive.initialKey.ToString("D4")));
        }

        private void addStringButton_Click(object sender, EventArgs e)
        {
            currentTextArchive.messages.Add("");
            textEditorDataGridView.Rows.Add("");

            int rowInd = textEditorDataGridView.RowCount - 1;

            Helpers.DisableHandlers();

            string format = "X";
            string prefix = "0x";
            if (decimalRadioButton.Checked)
            {
                format = "D";
                prefix = "";
            }

            textEditorDataGridView.Rows[rowInd].HeaderCell.Value = prefix + rowInd.ToString(format);
            Helpers.EnableHandlers();

        }
        private void exportTextFileButton_Click(object sender, EventArgs e)
        {
            int textSelection = selectTextFileComboBox.SelectedIndex;

            string msgFileType = "Gen IV Text Archive";
            string txtFileType = "Plaintext file";
            string suggestedFileName = "Text Archive " + textSelection;
            bool showSuccessMessage = true;

            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = $"{msgFileType} (*.msg)|*.msg|{txtFileType} (*.txt)|*.txt"
            };

            if (!string.IsNullOrWhiteSpace(suggestedFileName))
            {
                sf.FileName = suggestedFileName;
            }

            if (sf.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string selectedExtension = Path.GetExtension(sf.FileName);
            string type = currentTextArchive.GetType().Name;

            if (selectedExtension == ".msg")
            {
                // Handle .msg case
                currentTextArchive.SaveToFile(sf.FileName, showSuccessMessage);
            }
            else if (selectedExtension == ".txt")
            {
                // Handle .txt case
                const int txtLinesWarningThreshold = 300;
                if (currentTextArchive.messages.Count > txtLinesWarningThreshold)
                {
                    DialogResult result = MessageBox.Show($"This {type} has over {txtLinesWarningThreshold} messages. Writing a large text file may take a long time, especially on slow machines.\n\nAre you sure you want to proceed?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }
                File.WriteAllText(sf.FileName, currentTextArchive.ToString());

                if (showSuccessMessage)
                {
                    MessageBox.Show($"{type} saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            if (textSelection == RomInfo.locationNamesTextNumber)
            {
                ReloadHeaderEditorLocationsList(currentTextArchive.messages, _parent);
            }
        }

        private void saveTextArchiveButton_Click(object sender, EventArgs e)
        {
            currentTextArchive.SaveToFileDefaultDir(selectTextFileComboBox.SelectedIndex);
            if (selectTextFileComboBox.SelectedIndex == RomInfo.locationNamesTextNumber)
            {
                ReloadHeaderEditorLocationsList(currentTextArchive.messages, _parent);
            }
        }
        private void selectedLineMoveUpButton_Click(object sender, EventArgs e)
        {
            int cc = textEditorDataGridView.CurrentCell.RowIndex;

            if (cc > 0)
            {
                DataGridViewRowCollection rows = textEditorDataGridView.Rows;
                DataGridViewCell current = rows[cc].Cells[0];
                DataGridViewCell previous = rows[cc - 1].Cells[0];

                (current.Value, previous.Value) = (previous.Value, current.Value);
                textEditorDataGridView.CurrentCell = previous;
            }
        }

        private void selectedLineMoveDownButton_Click(object sender, EventArgs e)
        {
            int cc = textEditorDataGridView.CurrentCell.RowIndex;

            if (cc < textEditorDataGridView.RowCount - 1)
            {
                DataGridViewRowCollection rows = textEditorDataGridView.Rows;
                DataGridViewCell current = rows[cc].Cells[0];
                DataGridViewCell next = rows[cc + 1].Cells[0];

                (current.Value, next.Value) = (next.Value, current.Value);
                textEditorDataGridView.CurrentCell = next;
            }
        }
        //TODO : Externalize this function in a helper
        public void ReloadHeaderEditorLocationsList(IEnumerable<string> contents, MainProgram parent=null)
        {
            if(parent != null) _parent = parent;
            int selection = EditorPanels.headerEditor.locationNameComboBox.SelectedIndex;
            EditorPanels.headerEditor.locationNameComboBox.Items.Clear();
            EditorPanels.headerEditor.locationNameComboBox.Items.AddRange(contents.ToArray());
            EditorPanels.headerEditor.locationNameComboBox.SelectedIndex = selection;
        }
        private void importTextFileButton_Click(object sender, EventArgs e)
        {
            /* Prompt user to select .msg or .txt file */
            OpenFileDialog of = new OpenFileDialog
            {
                Filter = "Text Archive (*.msg;*.txt)|*.msg;*.txt|Gen IV Text Archive (*.msg)|*.msg|Plaintext file (*.txt)|*.txt"
            };
            if (of.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            /* Update Text Archive object in memory */
            string path = RomInfo.gameDirs[DirNames.textArchives].unpackedDir + "\\" + selectTextFileComboBox.SelectedIndex.ToString("D4");
            string selectedExtension = Path.GetExtension(of.FileName);

            bool readagain = false;

            if (selectedExtension == ".msg")
            {
                // Handle .msg case
                File.Copy(of.FileName, path, true);
                readagain = true;
            }
            else if (selectedExtension == ".txt")
            {
                // Handle .txt case
                try
                {
                    string[] lines = File.ReadAllLines(of.FileName);
                    currentTextArchive.messages.Clear();
                    currentTextArchive.messages.AddRange(lines);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to import text file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            /* Refresh controls */
            UpdateTextEditorFileView(readagain);

            /* Display success message */
            MessageBox.Show("Text Archive imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void removeMessageFileButton_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Are you sure you want to delete the last Text Archive?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d.Equals(DialogResult.Yes))
            {
                /* Delete Text Archive */
                File.Delete(RomInfo.gameDirs[DirNames.textArchives].unpackedDir + "\\" + (selectTextFileComboBox.Items.Count - 1).ToString("D4"));

                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = selectTextFileComboBox.Items.Count - 1;
                if (selectTextFileComboBox.SelectedIndex == lastIndex)
                {
                    selectTextFileComboBox.SelectedIndex--;
                }

                /* Remove item from ComboBox */
                selectTextFileComboBox.Items.RemoveAt(lastIndex);
            }
        }
        private void removeStringButton_Click(object sender, EventArgs e)
        {
            if (currentTextArchive.messages.Count > 0)
            {
                currentTextArchive.messages.RemoveAt(currentTextArchive.messages.Count - 1);
                textEditorDataGridView.Rows.RemoveAt(textEditorDataGridView.Rows.Count - 1);
            }
        }
        private void searchMessageButton_Click(object sender, EventArgs e)
        {
            if (searchMessageTextBox.Text == "")
            {
                return;
            }

            int firstArchiveNumber;
            int lastArchiveNumber;

            if (searchAllArchivesCheckBox.Checked)
            {
                firstArchiveNumber = 0;
                lastArchiveNumber = _parent.romInfo.GetTextArchivesCount();
            }
            else
            {
                firstArchiveNumber = selectTextFileComboBox.SelectedIndex;
                lastArchiveNumber = firstArchiveNumber + 1;
            }

            textSearchResultsListBox.Items.Clear();

            lastArchiveNumber = Math.Min(lastArchiveNumber, 828);

            textSearchProgressBar.Maximum = lastArchiveNumber;

            List<string> results = null;
            if (caseSensitiveTextSearchCheckbox.Checked)
            {
                results = searchTexts(firstArchiveNumber, lastArchiveNumber, (string x) => x.Contains(searchMessageTextBox.Text));
            }
            else
            {
                results = searchTexts(firstArchiveNumber, lastArchiveNumber, (string x) => x.IndexOf(searchMessageTextBox.Text, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }

            textSearchResultsListBox.Items.AddRange(results.ToArray());
            textSearchProgressBar.Value = 0;
            caseSensitiveTextSearchCheckbox.Enabled = true;
        }

        private List<string> searchTexts(int firstArchive, int lastArchive, Func<string, bool> criteria)
        {
            List<string> results = new List<string>();

            for (int i = firstArchive; i < lastArchive; i++)
            {

                TextArchive file = new TextArchive(i);
                for (int j = 0; j < file.messages.Count; j++)
                {
                    if (criteria(file.messages[j]))
                    {
                        results.Add("(" + i.ToString("D3") + ")" + " - #" + j.ToString("D2") + " --- " + file.messages[j].Substring(0, Math.Min(file.messages[j].Length, 40)));
                    }
                }
                textSearchProgressBar.Value = i;
            }
            return results;
        }

        private void searchMessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                searchMessageButton_Click(null, null);
            }
        }
        private void replaceMessageButton_Click(object sender, EventArgs e)
        {
            if (searchMessageTextBox.Text == "")
            {
                return;
            }

            int firstArchiveNumber;
            int lastArchiveNumber;

            string specify;
            if (searchAllArchivesCheckBox.Checked)
            {
                firstArchiveNumber = 0;
                lastArchiveNumber = _parent.romInfo.GetTextArchivesCount();
                specify = " in every Text Bank of the game (" + firstArchiveNumber + " to " + lastArchiveNumber + ")";
            }
            else
            {
                firstArchiveNumber = selectTextFileComboBox.SelectedIndex;
                lastArchiveNumber = firstArchiveNumber + 1;
                specify = " in the current text bank only (" + firstArchiveNumber + ")";
            }

            string message = "You are about to replace every occurrence of " + '"' + searchMessageTextBox.Text + '"'
                + " with " + '"' + replaceMessageTextBox.Text + '"' + specify +
                ".\nThe operation can't be interrupted nor undone.\n\nProceed?";
            DialogResult d = MessageBox.Show(message, "Confirm to proceed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (d == DialogResult.Yes)
            {
                string searchString = searchMessageTextBox.Text;
                string replaceString = replaceMessageTextBox.Text;
                textSearchResultsListBox.Items.Clear();

                lastArchiveNumber = Math.Min(lastArchiveNumber, 828);
                textSearchProgressBar.Maximum = lastArchiveNumber;

                for (int cur = firstArchiveNumber; cur < lastArchiveNumber; cur++)
                {
                    currentTextArchive = new TextArchive(cur);
                    bool found = false;

                    if (caseSensitiveTextReplaceCheckbox.Checked)
                    {
                        for (int j = 0; j < currentTextArchive.messages.Count; j++)
                        {
                            while (currentTextArchive.messages[j].IndexOf(searchString) >= 0)
                            {
                                currentTextArchive.messages[j] = currentTextArchive.messages[j].Replace(searchString, replaceString);
                                found = true;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < currentTextArchive.messages.Count; j++)
                        {
                            int posFound;
                            while ((posFound = currentTextArchive.messages[j].IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase)) >= 0)
                            {
                                currentTextArchive.messages[j] = currentTextArchive.messages[j].Substring(0, posFound) + replaceString + currentTextArchive.messages[j].Substring(posFound + searchString.Length);
                                found = true;
                            }
                        }
                    }

                    textSearchProgressBar.Value = cur;
                    if (found)
                    {
                        Helpers.DisableHandlers();

                        textSearchResultsListBox.Items.Add("Text archive (" + cur + ") - Succesfully edited");
                        currentTextArchive.SaveToFileDefaultDir(cur, showSuccessMessage: false);

                        if (cur == lastArchiveNumber)
                        {
                            UpdateTextEditorFileView(false);
                        }

                        Helpers.EnableHandlers();
                    }
                    //else searchMessageResultTextBox.AppendText(searchString + " not found in this file");
                    //this.saveMessageFileButton_Click(sender, e);
                }
                MessageBox.Show("Operation completed.", "Replace All Text", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateTextEditorFileView(readAgain: true);
                textSearchProgressBar.Value = 0;
            }
        }
        private void selectTextFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTextEditorFileView(true);
        }
        private void UpdateTextEditorFileView(bool readAgain)
        {
            Helpers.DisableHandlers();

            textEditorDataGridView.Rows.Clear();
            if (currentTextArchive is null || readAgain)
            {
                currentTextArchive = new TextArchive(selectTextFileComboBox.SelectedIndex);
            }

            foreach (string msg in currentTextArchive.messages)
            {
                textEditorDataGridView.Rows.Add(ApplyHighlightMarkers(msg)); // Apply markers before adding
            }

            if (hexRadiobutton.Checked)
            {
                PrintTextEditorLinesHex();
            }
            else
            {
                PrintTextEditorLinesDecimal();
            }

            // Ensure single event subscription
            textEditorDataGridView.CellPainting -= textEditorDataGridView_CellPainting;
            textEditorDataGridView.CellPainting += textEditorDataGridView_CellPainting;
            textEditorDataGridView.Invalidate(); // Force repaint

            Helpers.EnableHandlers();
            textEditorDataGridView_CurrentCellChanged(textEditorDataGridView, null);
        }
        private void PrintTextEditorLinesHex()
        {
            int final = Math.Min(textEditorDataGridView.Rows.Count, currentTextArchive.messages.Count);

            for (int i = 0; i < final; i++)
            {
                textEditorDataGridView.Rows[i].HeaderCell.Value = "0x" + i.ToString("X");
            }
        }
        private void PrintTextEditorLinesDecimal()
        {
            int final = Math.Min(textEditorDataGridView.Rows.Count, currentTextArchive.messages.Count);

            for (int i = 0; i < final; i++)
            {
                textEditorDataGridView.Rows[i].HeaderCell.Value = i.ToString();
            }
        }
        
        private void textEditorDataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (Helpers.HandlersDisabled || dgv == null || dgv.CurrentCell == null)
            {
                return;
            }

            AppLogger.Debug("R: " + dgv.CurrentCell.RowIndex);
            AppLogger.Debug("Last index: " + (dgv.RowCount - 1).ToString());

            if (dgv.CurrentCell.RowIndex > 0)
            {
                selectedLineMoveUpButton.Enabled = true;
            }
            else
            {
                selectedLineMoveUpButton.Enabled = false;
            }

            if (dgv.CurrentCell.RowIndex < dgv.RowCount - 1)
            {
                selectedLineMoveDownButton.Enabled = true;
            }
            else
            {
                selectedLineMoveDownButton.Enabled = false;
            }
        }

        private void textSearchResultsListBox_GoToEntryResult(object sender, MouseEventArgs e)
        {
            if (textSearchResultsListBox.SelectedIndex < 0)
            {
                return;
            }

            string[] msgResult = textSearchResultsListBox.Text.Split(new string[] { " --- " }, StringSplitOptions.RemoveEmptyEntries);
            string[] parts = msgResult[0].Substring(1).Split(new string[] { ") - #" }, StringSplitOptions.RemoveEmptyEntries);

            if (int.TryParse(parts[0], out int msg))
            {
                if (int.TryParse(parts[1], out int line))
                {
                    selectTextFileComboBox.SelectedIndex = msg;
                    textEditorDataGridView.ClearSelection();
                    textEditorDataGridView.Rows[line].Selected = true;
                    textEditorDataGridView.Rows[line].Cells[0].Selected = true;
                    textEditorDataGridView.CurrentCell = textEditorDataGridView.Rows[line].Cells[0];

                    return;
                }
            }
        }
        private void textSearchResultsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textSearchResultsListBox_GoToEntryResult(null, null);
            }
        }
        private void hexRadiobutton_CheckedChanged(object sender, EventArgs e)
        {
            updateTextEditorLineNumbers();
            SettingsManager.Settings.textEditorPreferHex = hexRadiobutton.Checked;
        }
        private void updateTextEditorLineNumbers()
        {
            Helpers.DisableHandlers();
            if (hexRadiobutton.Checked)
            {
                PrintTextEditorLinesHex();
            }
            else
            {
                PrintTextEditorLinesDecimal();
            }
            Helpers.EnableHandlers();
        }
        #endregion
        public void OpenTextEditor(MainProgram parent, int TextArchiveID, ComboBox locationNameComboBox)
        {

            SetupTextEditor(parent);

            selectTextFileComboBox.SelectedIndex = TextArchiveID;
            EditorPanels.mainTabControl.SelectedTab = EditorPanels.textEditorTabPage;
        }

        public void SetupTextEditor(MainProgram parent, bool force = false)
        {
            if (textEditorIsReady && !force) { return; }
            textEditorIsReady = true;
            this._parent = parent;

            DSUtils.TryUnpackNarcs(new List<DirNames> { DirNames.textArchives });
            Helpers.statusLabelMessage("Setting up Text Editor...");
            Update();

            selectTextFileComboBox.Items.Clear();
            int textCount = parent.romInfo.GetTextArchivesCount();
            for (int i = 0; i < textCount; i++)
            {
                Helpers.statusLabelMessage($"Parsing text file {i} of {textCount}");
                ExpandTextFile(i);
                selectTextFileComboBox.Items.Add("Text Archive " + i);
            }

            Helpers.DisableHandlers();
            hexRadiobutton.Checked = SettingsManager.Settings.textEditorPreferHex;
            Helpers.EnableHandlers();

            selectTextFileComboBox.SelectedIndex = 0;
            Helpers.statusLabelMessage();
        }

        public static void ExpandTextFile(int ID)
        {
            string baseDir = RomInfo.gameDirs[DirNames.textArchives].unpackedDir;
            string expandedDir = Path.Combine(baseDir, "expanded");
            string path = Path.Combine(baseDir, ID.ToString("D4"));
            string expandedPath = Path.Combine(expandedDir, ID.ToString("D4") + ".txt");
            string toolPath = Path.Combine(Application.StartupPath, "Tools", "msgenc.exe");
            string charmapPath = Path.Combine("Tools", "charmap.txt");

            if (!Directory.Exists(expandedDir))
            {
                try
                {
                    Directory.CreateDirectory(expandedDir);
                    AppLogger.Info("Created expanded folder \"" + expandedDir + "\".");
                }
                catch (IOException)
                {
                    MessageBox.Show("Text File has not been extracted.\nCan't create directory: \n" + expandedDir + "\nThis might be a temporary issue.\nMake sure no other process is using it and try again.", "Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (File.Exists(expandedPath) && File.GetLastWriteTimeUtc(expandedPath) >= File.GetLastWriteTimeUtc(path))
            {
                AppLogger.Info($"Skipped expanding {ID:D4} — already up to date.");
                return;
            }

            Process expand = new Process();
            expand.StartInfo.FileName = toolPath;
            expand.StartInfo.Arguments = $"-d -c \"{charmapPath}\" \"{path}\" \"{expandedPath}\"";
            expand.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            expand.StartInfo.CreateNoWindow = false;

            try
            {
                expand.Start();
                expand.WaitForExit();
                AppLogger.Info($"Expanded {ID:D4}");
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show($"Failed to start msgenc.exe: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool CompressTextFile(int ID)
        {
            string baseDir = RomInfo.gameDirs[DirNames.textArchives].unpackedDir;
            string expandedDir = Path.Combine(baseDir, "expanded");
            string path = Path.Combine(baseDir, ID.ToString("D4"));
            string expandedPath = Path.Combine(expandedDir, ID.ToString("D4") + ".txt");
            string toolPath = Path.Combine(Application.StartupPath, "Tools", "msgenc.exe");
            string charmapPath = Path.Combine("Tools", "charmap.txt");

            if (!Directory.Exists(expandedDir))
            {
                 MessageBox.Show("No expanded files to compress.", "Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 return false;                
            }

            if (File.Exists(path) && File.GetLastWriteTimeUtc(path) >= File.GetLastWriteTimeUtc(expandedPath))
            {
                AppLogger.Info($"Skipped expanding {ID:D4} — already up to date.");
                return false;
            }

            Process expand = new Process();
            expand.StartInfo.FileName = toolPath;
            expand.StartInfo.Arguments = $"-e -c \"{charmapPath}\" \"{expandedPath}\" \"{path}\"";
            expand.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            expand.StartInfo.CreateNoWindow = false;

            try
            {
                expand.Start();
                expand.WaitForExit();
                AppLogger.Info($"Compressed {ID:D4}");
                return true;
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show($"Failed to start msgenc.exe: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
    }
}
