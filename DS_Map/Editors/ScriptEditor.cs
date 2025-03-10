using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using ScintillaNET;
using ScintillaNET.Utils;
using System.Globalization;

namespace DSPRE.Editors {
    public partial class ScriptEditor : UserControl {
        public bool scriptEditorIsReady { get; set; } = false;
        private SearchManager scriptSearchManager;
        private Scintilla ScriptTextArea;
        private bool scriptsDirty = false;
        private bool functionsDirty = false;
        private bool actionsDirty = false;
        private string cmdKeyWords = "";
        private string secondaryKeyWords = "";
        private ScriptFile currentScriptFile;
        MainProgram _parent;
        /// <summary>
        /// the background color of the text area
        /// </summary>
        private readonly Color BACK_COLOR = Color.FromArgb(0x2A211C);

        /// <summary>
        /// default text color of the text area
        /// </summary>
        private readonly Color FORE_COLOR = Color.FromArgb(0xB7B7B7);

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;

        private const int BOOKMARK_MARKER = 2;

        /// <summary>
        /// change this to whatever margin you want the code folding tree (+/-) to show in
        /// </summary>
        private const int FOLDING_MARGIN = 3;

        /// <summary>
        /// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
        /// </summary>
        private const bool CODE_FOLDING_CIRCULAR = true;

        readonly Point initial_importScriptFileButton_location;
        readonly Point initial_exportScriptFileButton_location;
        readonly Point initial_addScriptFileButton_location;
        readonly Point initial_removeScriptFileButton_location;
        readonly Point initial_viewLevelScript_location;

        readonly Point new_importScriptFileButton_location;
        readonly Point new_exportScriptFileButton_location;
        readonly Point new_addScriptFileButton_location;
        readonly Point new_removeScriptFileButton_location;
        readonly Point new_viewLevelScript_location;

        public ScriptEditor() {
            InitializeComponent();


            //initially, these buttons are off the canvas so they can be interacted with in the designer
            //they are then moved as needed
            initial_importScriptFileButton_location = importScriptFileButton.Location;
            initial_exportScriptFileButton_location = exportScriptFileButton.Location;
            initial_addScriptFileButton_location = addScriptFileButton.Location;
            initial_removeScriptFileButton_location = removeScriptFileButton.Location;
            initial_viewLevelScript_location = viewLevelScriptButton.Location;

            new_importScriptFileButton_location = new Point(164, 22);
            new_exportScriptFileButton_location = new Point(239, 22);
            new_addScriptFileButton_location = new Point(314, 22);
            new_removeScriptFileButton_location = new Point(314, 49);
            new_viewLevelScript_location = new Point(326, 37);

            importScriptFileButton.Enabled = false;
            exportScriptFileButton.Enabled = false;
            addScriptFileButton.Enabled = false;
            removeScriptFileButton.Enabled = false;
            viewLevelScriptButton.Enabled = false;
        }

        public void SetupScriptEditor(MainProgram parent, bool force = false) {
            if (scriptEditorIsReady && !force) { return; }
            scriptEditorIsReady = true;
            this._parent = parent;
            SetupScriptEditorTextAreas();

            /* Extract essential NARCs sub-archives*/
            Helpers.statusLabelMessage("Setting up Script Editor...");
            Update();

            DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames> { RomInfo.DirNames.scripts }); //12 = scripts Narc Dir

            populate_selectScriptFileComboBox(0);

            UpdateScriptNumberCheckBox((NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference);
            Helpers.statusLabelMessage();
        }

        public void OpenScriptEditor(MainProgram parent, int scriptFileID) {
            SetupScriptEditor(parent);

            selectScriptFileComboBox.SelectedIndex = scriptFileID;
            EditorPanels.mainTabControl.SelectedTab = EditorPanels.scriptEditorTabPage;
        }

        private void SetupScriptEditorTextAreas() {
            //PREPARE SCRIPT EDITOR KEYWORDS
            cmdKeyWords = String.Join(" ", RomInfo.ScriptCommandNamesDict.Values) +
                          " " + String.Join(" ", ScriptDatabase.movementsDictIDName.Values);
            cmdKeyWords += " " + cmdKeyWords.ToUpper() + " " + cmdKeyWords.ToLower();

            secondaryKeyWords = String.Join(" ", RomInfo.ScriptComparisonOperatorsDict.Values) +
                                " " + String.Join(" ", ScriptDatabase.specialOverworlds.Values) +
                                " " + String.Join(" ", ScriptDatabase.overworldDirections.Values) +
                                " " + Event.EventType.Overworld +
                                " " + Overworld.MovementCodeKW;
            secondaryKeyWords += " " + secondaryKeyWords.ToUpper() + " " + secondaryKeyWords.ToLower();

            // CREATE CONTROLS
            ScriptTextArea = new Scintilla();
            scriptSearchManager = new SearchManager(EditorPanels.MainProgram, ScriptTextArea, panelFindScriptTextBox, PanelSearchScripts);
            scintillaScriptsPanel.Controls.Clear();
            scintillaScriptsPanel.Controls.Add(ScriptTextArea);

            // BASIC CONFIG
            ScriptTextArea.TextChanged += (OnTextChangedScript);

            // INITIAL VIEW CONFIG
            InitialViewConfig(ScriptTextArea);

            InitSyntaxColoring(ScriptTextArea);

            // NUMBER MARGIN
            InitNumberMargin(ScriptTextArea, ScriptTextArea_MarginClick);

            // BOOKMARK MARGIN
            InitBookmarkMargin(ScriptTextArea);

            // CODE FOLDING MARGIN
            InitCodeFolding(ScriptTextArea);

            // INIT HOTKEYS
            InitHotkeys(ScriptTextArea, scriptSearchManager);

            // INIT TOOLTIPS DWELLING
            /*
            ScriptTextArea.MouseDwellTime = 300;
            ScriptTextArea.DwellEnd += TextArea_DwellEnd;
            ScriptTextArea.DwellStart += TextArea_DwellStart;

            FunctionTextArea.MouseDwellTime = 300;
            FunctionTextArea.DwellEnd += TextArea_DwellEnd;
            FunctionTextArea.DwellStart += TextArea_DwellStart;
            */

            // Style for prefixed words (label_*, script_*)
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

        private void InitialViewConfig(Scintilla textArea) {
            textArea.Dock = DockStyle.Fill;
            textArea.WrapMode = WrapMode.Word;
            textArea.IndentationGuides = IndentView.LookBoth;
            textArea.CaretPeriod = 500;
            textArea.CaretForeColor = Color.White;
            textArea.SetSelectionBackColor(true, Color.FromArgb(0x114D9C));
            textArea.WrapIndentMode = WrapIndentMode.Same;
        }

        private void InitSyntaxColoring(Scintilla textArea) {
            textArea.StyleResetDefault();
            textArea.Styles[Style.Default].Font = "Consolas";
            textArea.Styles[Style.Default].Size = 12;
            textArea.Styles[Style.Default].BackColor = Color.FromArgb(0x212121);
            textArea.Styles[Style.Default].ForeColor = Color.FromArgb(0xFFFFFF);
            textArea.StyleClearAll();

            // Configure Assembly lexer styles
            textArea.Styles[Style.Asm.Identifier].ForeColor = Color.FromArgb(0xD0DAE2);
            textArea.Styles[Style.Asm.Number].ForeColor = Color.FromArgb(0xFFFF00);
            textArea.Styles[Style.Asm.String].ForeColor = Color.FromArgb(0xFF00FF);
            textArea.Styles[Style.Asm.Character].ForeColor = Color.FromArgb(0xE95454);
            textArea.Styles[Style.Asm.Operator].ForeColor = Color.FromArgb(0xFFFF00);
            textArea.Styles[Style.Asm.Comment].ForeColor = Color.FromArgb(0x40BF57);

            // For command keywords - use CpuInstruction style
            textArea.Styles[Style.Asm.CpuInstruction].ForeColor = Color.FromArgb(0x48A8EE);

            // For secondary keywords - use Directive style
            textArea.Styles[Style.Asm.Directive].ForeColor = Color.FromArgb(0xF98906);

            // Configure indicators for prefix highlighting
            textArea.Indicators[0].Style = IndicatorStyle.TextFore;
            textArea.Indicators[0].ForeColor = Color.FromArgb(0x8A2BE2); // Purple for label_*
            textArea.Indicators[0].Under = false;  // Draw over the lexer's styling

            textArea.Indicators[1].Style = IndicatorStyle.TextFore;
            textArea.Indicators[1].ForeColor = Color.FromArgb(0x00CED1); // Cyan for script_*
            textArea.Indicators[1].Under = false;  // Draw over the lexer's styling

            // Set the lexer and keywords
            textArea.Lexer = Lexer.Asm;
            textArea.SetKeywords(0, cmdKeyWords);     // CPU Instructions index
            textArea.SetKeywords(3, secondaryKeyWords); // Directives index

            // Apply the highlighting
            textArea.TextChanged += (sender, e) => HighlightPrefixedWords(textArea);

            // Initial highlighting
            HighlightPrefixedWords(textArea);
        }

        private void HighlightPrefixedWords(Scintilla textArea) {
            // Clear existing indicators
            textArea.IndicatorCurrent = 0;
            textArea.IndicatorClearRange(0, textArea.TextLength);
            textArea.IndicatorCurrent = 1;
            textArea.IndicatorClearRange(0, textArea.TextLength);

            // Process each line individually
            for (int i = 0; i < textArea.Lines.Count; i++) {
                string lineText = textArea.Lines[i].Text;
                int linePos = textArea.Lines[i].Position;

                // Trim for detection but use original text for positions
                string trimmedLine = lineText.Trim();

                // Handle script_ lines
                if (trimmedLine.Contains("script_") && trimmedLine.EndsWith(":")) {
                    int startPos = linePos + lineText.IndexOf("script_");
                    int endPos = linePos + lineText.IndexOf(":", lineText.IndexOf("script_")) + 1;

                    textArea.IndicatorCurrent = 1;
                    textArea.IndicatorFillRange(startPos, endPos - startPos);
                }

                // Handle label_ lines
                if (trimmedLine.Contains("label_") && trimmedLine.EndsWith(":")) {
                    int startPos = linePos + lineText.IndexOf("label_");
                    int endPos = linePos + lineText.IndexOf(":", lineText.IndexOf("label_")) + 1;

                    textArea.IndicatorCurrent = 0;
                    textArea.IndicatorFillRange(startPos, endPos - startPos);
                }
            }
        }

        private void InitNumberMargin(Scintilla textArea, EventHandler<MarginClickEventArgs> textArea_MarginClick) {
            textArea.Styles[Style.LineNumber].BackColor = BACK_COLOR;
            textArea.Styles[Style.LineNumber].ForeColor = FORE_COLOR;
            textArea.Styles[Style.IndentGuide].ForeColor = FORE_COLOR;
            textArea.Styles[Style.IndentGuide].BackColor = BACK_COLOR;

            Margin nums = textArea.Margins[NUMBER_MARGIN];
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            textArea.MarginClick += textArea_MarginClick;
        }

        private void InitBookmarkMargin(Scintilla textArea) {
            //TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

            Margin margin = textArea.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BOOKMARK_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            Marker marker = textArea.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(Color.FromArgb(0xFF003B));
            marker.SetForeColor(Color.FromArgb(0x000000));
            marker.SetAlpha(100);
        }

        private void InitCodeFolding(Scintilla textArea) {
            textArea.SetFoldMarginColor(true, BACK_COLOR);
            textArea.SetFoldMarginHighlightColor(true, BACK_COLOR);

            // Enable code folding
            textArea.SetProperty("fold", "1");
            textArea.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            textArea.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
            textArea.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
            textArea.Margins[FOLDING_MARGIN].Sensitive = true;
            textArea.Margins[FOLDING_MARGIN].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++) {
                textArea.Markers[i].SetForeColor(BACK_COLOR); // styles for [+] and [-]
                textArea.Markers[i].SetBackColor(FORE_COLOR); // styles for [+] and [-]
            }

            // Configure folding markers with respective symbols
            textArea.Markers[Marker.Folder].Symbol = CODE_FOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
            textArea.Markers[Marker.FolderOpen].Symbol = CODE_FOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
            textArea.Markers[Marker.FolderEnd].Symbol = CODE_FOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
            textArea.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            textArea.Markers[Marker.FolderOpenMid].Symbol = CODE_FOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
            textArea.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            textArea.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            textArea.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }

        private void InitHotkeys(Scintilla scintillaTb, SearchManager sm) {
            // register the hotkeys with the form
            HotKeyManager.AddHotKey(scintillaTb, sm.OpenSearch, Keys.F, true);
            HotKeyManager.AddHotKey(scintillaTb, () => Uppercase(scintillaTb), Keys.U, true);
            HotKeyManager.AddHotKey(scintillaTb, () => Lowercase(scintillaTb), Keys.L, true);
            HotKeyManager.AddHotKey(scintillaTb, () => ZoomIn(scintillaTb), Keys.Oemplus, true);
            HotKeyManager.AddHotKey(scintillaTb, () => ZoomOut(scintillaTb), Keys.OemMinus, true);
            HotKeyManager.AddHotKey(scintillaTb, () => ZoomDefault(scintillaTb), Keys.D0, true);
            HotKeyManager.AddHotKey(scintillaTb, sm.CloseSearch, Keys.Escape);

            // remove conflicting hotkeys from scintilla
            scintillaTb.ClearCmdKey(Keys.Control | Keys.F);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.R);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.H);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.L);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.U);
        }

        private void Uppercase(Scintilla textArea) {
            // save the selection
            int start = textArea.SelectionStart;
            int end = textArea.SelectionEnd;

            // modify the selected text
            textArea.ReplaceSelection(textArea.GetTextRange(start, end - start).ToUpper());

            // preserve the original selection
            textArea.SetSelection(start, end);
        }

        private void Lowercase(Scintilla textArea) {
            // save the selection
            int start = textArea.SelectionStart;
            int end = textArea.SelectionEnd;

            // modify the selected text
            textArea.ReplaceSelection(textArea.GetTextRange(start, end - start).ToLower());

            // preserve the original selection
            textArea.SetSelection(start, end);
        }

        private void ZoomIn(Scintilla textArea) {
            textArea.ZoomIn();
        }

        private void ZoomOut(Scintilla textArea) {
            textArea.ZoomOut();
        }

        private void ZoomDefault(Scintilla textArea) {
            textArea.Zoom = 0;
        }

        private void ScriptEditorSetClean() {
            Helpers.DisableHandlers();

            //scriptsTabPage.Text = ScriptFile.ContainerTypes.Script.ToString() + "s";
            scriptsDirty = false;

            Helpers.EnableHandlers();
        }

        private void OnTextChangedScript(object sender, EventArgs e) {
            ScriptTextArea.Margins[NUMBER_MARGIN].Width = ScriptTextArea.Lines.Count.ToString().Length * 13;
            scriptsDirty = true;
            //scriptsTabPage.Text = ScriptFile.ContainerTypes.Script.ToString() + "s" + "*";
        }

        private void ScriptTextArea_MarginClick(object sender, MarginClickEventArgs e) {
            MarginClick(ScriptTextArea, e);
        }

        private void MarginClick(Scintilla textArea, MarginClickEventArgs e) {
            if (e.Margin == BOOKMARK_MARGIN) {
                // Do we have a marker for this line?
                const uint mask = (1 << BOOKMARK_MARKER);
                Line line = textArea.Lines[textArea.LineFromPosition(e.Position)];
                if ((line.MarkerGet() & mask) > 0) {
                    // Remove existing bookmark
                    line.MarkerDelete(BOOKMARK_MARKER);
                } else {
                    // Add bookmark
                    line.MarkerAdd(BOOKMARK_MARKER);
                }
            }
        }

        private void selectScriptFileComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            DisplayScript();
        }

        public void UpdateScriptNumberCheckBox(NumberStyles toSet) {
            Helpers.DisableHandlers();
            Properties.Settings.Default.scriptEditorFormatPreference = (int)toSet;

            switch ((NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference) {
                case NumberStyles.None:
                    scriptEditorNumberFormatNoPreference.Checked = true;
                    break;
                case NumberStyles.HexNumber:
                    scriptEditorNumberFormatHex.Checked = true;
                    break;
                case NumberStyles.Integer:
                    scriptEditorNumberFormatDecimal.Checked = true;
                    break;
            }

            Console.WriteLine("changed style to " + Properties.Settings.Default.scriptEditorFormatPreference);
            Helpers.EnableHandlers();
        }

        private void UpdateScriptNumberFormat(NumberStyles numberStyle) {
            if (Helpers.HandlersEnabled) {
                NumberStyles old = (NumberStyles)Properties.Settings.Default.scriptEditorFormatPreference; //Local Backup
                Properties.Settings.Default.scriptEditorFormatPreference = (int)numberStyle;

                if (!DisplayScript()) {
                    UpdateScriptNumberCheckBox(old); //Restore old checkbox status! Script couldn't be redrawn
                }
            }
        }

        private void UpdateScriptNumberFormatNoPref(object sender, EventArgs e) {
            UpdateScriptNumberFormat(NumberStyles.None);
        }

        private void UpdateScriptNumberFormatDec(object sender, EventArgs e) {
            UpdateScriptNumberFormat(NumberStyles.Integer);
        }

        private void UpdateScriptNumberFormatHex(object sender, EventArgs e) {
            UpdateScriptNumberFormat(NumberStyles.HexNumber);
        }

        private bool DisplayScript() {
            Console.WriteLine("Script Reload has been requested");

            /* clear controls */
            if (Helpers.HandlersDisabled || selectScriptFileComboBox.SelectedItem == null) {
                return false;
            }

            // Keep all the code that handles unsaved changes
            if (scriptsDirty || functionsDirty || actionsDirty) {
                DialogResult d = MessageBox.Show("There are unsaved changes in this Script File.\nDo you wish to discard them?", "Unsaved work", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (!d.Equals(DialogResult.Yes)) {
                    Helpers.DisableHandlers();
                    selectScriptFileComboBox.SelectedIndex = (int)currentScriptFile.fileID;
                    Helpers.EnableHandlers();
                    return false;
                }
            }

            Helpers.DisableHandlers();

            ScriptFile lastScriptFile = currentScriptFile;
            // Load the script file using our new label-based constructor
            currentScriptFile = new ScriptFile(selectScriptFileComboBox.SelectedIndex);

            // Clear only the script text area and nav listbox
            ScriptTextArea.ClearAll();
            scriptsNavListbox.Items.Clear();

            bool typeChanged = true;
            if (lastScriptFile != null) {
                typeChanged = lastScriptFile.isLevelScript != currentScriptFile.isLevelScript;
            }

            if (typeChanged) {
                if (currentScriptFile.isLevelScript) {
                    importScriptFileButton.Location = initial_importScriptFileButton_location;
                    exportScriptFileButton.Location = initial_exportScriptFileButton_location;
                    addScriptFileButton.Location = initial_addScriptFileButton_location;
                    removeScriptFileButton.Location = initial_removeScriptFileButton_location;

                    viewLevelScriptButton.Location = new_viewLevelScript_location;

                    importScriptFileButton.Enabled = false;
                    exportScriptFileButton.Enabled = false;
                    addScriptFileButton.Enabled = false;
                    removeScriptFileButton.Enabled = false;

                    viewLevelScriptButton.Enabled = true;
                } else {
                    importScriptFileButton.Location = new_importScriptFileButton_location;
                    exportScriptFileButton.Location = new_exportScriptFileButton_location;
                    addScriptFileButton.Location = new_addScriptFileButton_location;
                    removeScriptFileButton.Location = new_removeScriptFileButton_location;

                    viewLevelScriptButton.Location = initial_viewLevelScript_location;

                    importScriptFileButton.Enabled = true;
                    exportScriptFileButton.Enabled = true;
                    addScriptFileButton.Enabled = true;
                    removeScriptFileButton.Enabled = true;

                    viewLevelScriptButton.Enabled = false;
                }
            }

            if (!currentScriptFile.isLevelScript) {
                displayScriptFile(scriptsNavListbox, ScriptTextArea);
            }

            ScriptEditorSetClean();

            Helpers.statusLabelMessage();
            Helpers.EnableHandlers();

            return true;
        }

        private void displayScriptFile(ListBox navListBox, Scintilla textArea) {
            if (currentScriptFile.CommandSequence == null || currentScriptFile.CommandSequence.Count == 0) {
                return;
            }

            // First add all labels to the nav listbox
            HashSet<string> addedLabels = new HashSet<string>();
            foreach (var cmdPos in currentScriptFile.CommandSequence) {
                if (!string.IsNullOrEmpty(cmdPos.Label) && !addedLabels.Contains(cmdPos.Label)) {
                    navListBox.Items.Add(cmdPos.Label);
                    addedLabels.Add(cmdPos.Label);
                }
            }

            // Generate the script text
            string scriptText = currentScriptFile.ToText();
            textArea.Text = scriptText;
        }

        private void scriptEditorZoomInButton_Click(object sender, EventArgs e) {
            ZoomIn(ScriptTextArea);
        }

        private void scriptEditorZoomOutButton_Click(object sender, EventArgs e) {
            ZoomOut(ScriptTextArea);
        }

        private void scriptEditorZoomResetButton_Click(object sender, EventArgs e) {
            ZoomDefault(ScriptTextArea);
        }

        private void removeScriptFileButton_Click(object sender, EventArgs e) {
            DialogResult d = MessageBox.Show("Are you sure you want to delete the last Script File?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d.Equals(DialogResult.Yes)) {
                /* Delete script file */
                string path = Filesystem.GetScriptPath(selectScriptFileComboBox.Items.Count - 1);
                File.Delete(path);

                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = selectScriptFileComboBox.Items.Count - 1;
                if (selectScriptFileComboBox.SelectedIndex == lastIndex) {
                    selectScriptFileComboBox.SelectedIndex--;
                }

                /* Remove item from ComboBox */
                selectScriptFileComboBox.Items.RemoveAt(lastIndex);
            }
        }

        private void addScriptFileButton_Click(object sender, EventArgs e) {
            /* Add new event file to event folder */
            int fileID = selectScriptFileComboBox.Items.Count;

            // Create a simple script with one labeled section
            List<string> scriptLines = new List<string> {
                "script_0:",
                "\tEnd"
            };

            // Use the new constructor that just takes script lines
            ScriptFile scriptFile = new ScriptFile(scriptLines, fileID);

            // Check if ScriptFile instance was created successfully
            if (scriptFile.SaveToFileDefaultDir(fileID, showSuccessMessage: false)) {
                /* Update ComboBox and select new file */
                selectScriptFileComboBox.Items.Add($"Script File {fileID}");
                selectScriptFileComboBox.SelectedIndex = selectScriptFileComboBox.Items.Count - 1;
            }
        }

        private void saveScriptFileButton_Click(object sender, EventArgs e) {
            /* Create new ScriptFile object using the values in the script editor */
            int fileID = currentScriptFile.fileID;

            // We only need the script text area now, not function or action areas
            ScriptFile userEdited = new ScriptFile(
                ScriptTextArea.Lines.ToStringsList(trim: true),
                fileID
            );

            // Check if ScriptFile instance was created successfully
            if (userEdited.SaveToFileDefaultDir(fileID)) {
                currentScriptFile = userEdited;
                ScriptEditorSetClean();
            }
        }

        private void exportScriptFileButton_Click(object sender, EventArgs e) {
            currentScriptFile.SaveToFileExplorePath(currentScriptFile.ToString(), blindmode: true);
        }

        private void importScriptFileButton_Click(object sender, EventArgs e) {
            /* Prompt user to select .scr or .bin file */
            OpenFileDialog of = new OpenFileDialog {
                Filter = "Script File (*.scr, *.bin)|*.scr;*.bin"
            };
            if (of.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            /* Update scriptFile object in memory */
            int i = selectScriptFileComboBox.SelectedIndex;
            string path = Filesystem.GetScriptPath(i);
            File.Copy(of.FileName, path, true);

            populate_selectScriptFileComboBox(i);

            /* Refresh controls */
            selectScriptFileComboBox_SelectedIndexChanged(null, null);

            /* Display success message */
            MessageBox.Show("Scripts imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void viewLevelScriptButton_Click(object sender, EventArgs e) {
            EditorPanels.levelScriptEditor.OpenLevelScriptEditor(this._parent, selectScriptFileComboBox.SelectedIndex);
        }

        private void locateCurrentScriptFile_Click(object sender, EventArgs e) {
            string path = Filesystem.GetScriptPath(selectScriptFileComboBox.SelectedIndex);
            Helpers.ExplorerSelect(path);
        }

        private void findNext(SearchManager searchManager) {
            searchManager.Find(true, false);
            scrollResultToTop(searchManager);
        }

        private void findPrev(SearchManager searchManager) {
            searchManager.Find(false, false);
            scrollResultToTop(searchManager);
        }

        private void findCurrent(SearchManager searchManager) {
            searchManager.Find(true, true);
            scrollResultToTop(searchManager);
        }

        private void TxtFindKeyDown(SearchManager searchManager, KeyEventArgs e) {
            if (HotKeyManager.IsHotkey(e, Keys.Enter)) {
                findNext(searchManager);
            }

            if (HotKeyManager.IsHotkey(e, Keys.Enter, true) || HotKeyManager.IsHotkey(e, Keys.Enter, false, true)) {
                findPrev(searchManager);
            }
        }

        private void BtnNextFindScript_Click(object sender, EventArgs e) {
            findNext(scriptSearchManager);
        }

        private void BtnPrevFindScript_Click(object sender, EventArgs e) {
            findPrev(scriptSearchManager);
        }

        private void panelFindScriptTextBox_TextChanged(object sender, EventArgs e) {
            findCurrent(scriptSearchManager);
        }

        private void scriptTxtFind_KeyDown(object sender, KeyEventArgs e) {
            TxtFindKeyDown(scriptSearchManager, e);
        }

        private void BtnCloseFindScript_Click(object sender, EventArgs e) {
            scriptSearchManager.CloseSearch();
        }


        void scrollResultToTop(SearchManager searchManager) {
            int resultStart = searchManager.textAreaScintilla.CurrentLine - ScriptEditorSearchResult.ResultsPadding;
            searchManager.textAreaScintilla.FirstVisibleLine = resultStart;
        }

        private void NavigatorGoTo(ListBox listBox, ScriptFile.ContainerTypes containerType) {
            if (listBox.SelectedIndex < 0) {
                return;
            }

            int commandNumber = listBox.SelectedIndex + 1;
            string CommandBlockOpen = $"{containerType} {commandNumber}:";
            scriptSearchManager.Find(true, false, CommandBlockOpen);

            scrollResultToTop(scriptSearchManager);
        }

        private void scriptsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {
            NavigatorGoTo((ListBox)sender, ScriptFile.ContainerTypes.Script);
        }

        private void functionsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {
            //NavigatorGoTo((ListBox)sender, functionsTabPage, functionSearchManager, ScriptFile.ContainerTypes.Function);
        }

        private void actionsNavListbox_SelectedIndexChanged(object sender, EventArgs e) {
            //NavigatorGoTo((ListBox)sender, actionsTabPage, actionSearchManager, ScriptFile.ContainerTypes.Action);
        }

        private void openFindScriptEditorButton_Click(object sender, EventArgs e) {
            scriptSearchManager.OpenSearch();
        }

        private void ScriptEditorExpandButton_Click(object sender, EventArgs e) {
            ScriptTextArea.FoldAll(FoldAction.Expand);
        }

        private void ScriptEditorCollapseButton_Click(object sender, EventArgs e) {
            ScriptTextArea.FoldAll(FoldAction.Contract);
        }

        private void scriptEditorWordWrapCheckbox_CheckedChanged(object sender, EventArgs e) {
            ScriptTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? WrapMode.Word : WrapMode.None;
        }

        private void viewWhiteSpacesButton_Click(object sender, EventArgs e) {
            ScriptTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
        }

        private void searchInScriptsTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                searchInScriptsButton_Click(null, null);
            }
        }

        public List<ScriptFile> getScriptsToSearch() {
            List<ScriptFile> scriptsToSearch = new List<ScriptFile>();

            if (searchOnlyCurrentScriptCheckBox.Checked) {
                this.UIThread(() => {
                    searchProgressBar.Maximum = 1;
                });
                int i = selectScriptFileComboBox.SelectedIndex;
                ScriptFile scriptFile = new ScriptFile(i);
                Console.WriteLine("Attempting to load script " + scriptFile.fileID);
                scriptsToSearch.Add(scriptFile);
                this.UIThread(() => {
                    searchProgressBar.IncrementNoAnimation();
                });
            } else {
                this.UIThread(() => {
                    searchProgressBar.Maximum = selectScriptFileComboBox.Items.Count;
                });
                for (int i = 0; i < selectScriptFileComboBox.Items.Count; i++) {
                    ScriptFile scriptFile = new ScriptFile(i);
                    Console.WriteLine("Attempting to load script " + scriptFile.fileID);
                    scriptsToSearch.Add(scriptFile);
                    this.UIThread(() => {
                        searchProgressBar.IncrementNoAnimation();
                    });
                }
            }

            return scriptsToSearch;
        }

        private void searchInScriptsButton_Click(object sender, EventArgs e) {
            if (searchInScriptsTextBox.Text == "") {
                return;
            }

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (_sender, args) => {
                this.UIThread(() => {
                    searchInScriptsResultListBox.Items.Clear();
                    searchProgressBar.Value = 0;
                });

                List<ScriptFile> scriptsToSearch = getScriptsToSearch();

                string searchString = searchInScriptsTextBox.Text;
                bool searchCriteriaCS(string s) => s.IndexOf(searchString, StringComparison.InvariantCulture) >= 0;
                bool searchCriteriaCI(string s) => s.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
                
                Func<string, bool> searchCriteria;
                if (scriptSearchCaseSensitiveCheckBox.Checked) {
                    searchCriteria = searchCriteriaCS;
                } else {
                    searchCriteria = searchCriteriaCI;
                }

                List<ScriptEditorSearchResult> results = new List<ScriptEditorSearchResult>();
                foreach (ScriptFile scriptFile in scriptsToSearch) {
                    List<ScriptEditorSearchResult> scriptResults = SearchInScripts(scriptFile, searchCriteria);
                    results.AddRange(scriptResults);
                    // results.AddRange(actionResults);
                }

                this.UIThread(() => {
                    searchInScriptsResultListBox.Items.AddRange(results.ToArray());
                    searchProgressBar.Value = 0;
                });
            };

            bw.RunWorkerAsync();
        }

        private List<ScriptEditorSearchResult> SearchInScripts(ScriptFile scriptFile, Func<string, bool> criteria) {
            List<ScriptEditorSearchResult> results = new List<ScriptEditorSearchResult>();


            return results;
        }

        private void searchInScriptsResultListBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                goToSearchResult();
            }
        }

        private void searchInScripts_GoToEntryResult(object sender, MouseEventArgs e) {
            goToSearchResult();
        }

        private void goToSearchResult() {
            if (searchInScriptsResultListBox.SelectedItem == null) { return; }

            ScriptEditorSearchResult searchResult = (ScriptEditorSearchResult)searchInScriptsResultListBox.SelectedItem;
            ScriptFile scriptFile = searchResult.scriptFile;
            ScriptFile.ContainerTypes containerType = searchResult.containerType;

            selectScriptFileComboBox.SelectedIndex = scriptFile.fileID;

            if (containerType == ScriptFile.ContainerTypes.Script) {
                displaySearchResult(scriptSearchManager, searchResult);
            }
        }

        private void displaySearchResult(SearchManager searchManager, ScriptEditorSearchResult searchResult) {
            searchManager.Find(true, false, searchResult.CommandBlockOpen);
            int blockStart = searchManager.textAreaScintilla.CurrentLine - ScriptEditorSearchResult.ResultsPadding;

            searchManager.Find(true, false, searchResult.scriptCommand.name);
            int resultStart = searchManager.textAreaScintilla.CurrentLine - ScriptEditorSearchResult.ResultsPadding;

            if (scrollToBlockStartcheckBox.Checked) {
                searchManager.textAreaScintilla.FirstVisibleLine = blockStart;
            } else {
                searchManager.textAreaScintilla.FirstVisibleLine = resultStart;
            }
        }
    }

    public class ScriptEditorSearchResult {
        public readonly ScriptFile scriptFile;
        public readonly ScriptFile.ContainerTypes containerType;
        public readonly int commandNumber;
        public readonly ScriptCommand scriptCommand;

        public const int ResultsPadding = 1;

        public ScriptEditorSearchResult(ScriptFile scriptFile, ScriptFile.ContainerTypes containerType, int commandNumber, ScriptCommand scriptCommand) {
            this.scriptFile = scriptFile;
            this.containerType = containerType;
            this.commandNumber = commandNumber;
            this.scriptCommand = scriptCommand;
        }

        public string CommandBlockOpen { get { return $"{containerType} {commandNumber}:"; } }

        public override string ToString() {
            return $"File {scriptFile.fileID} - {CommandBlockOpen} {scriptCommand.name}";
        }
    }
}
