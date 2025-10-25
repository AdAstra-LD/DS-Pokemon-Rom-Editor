using DSPRE.Editors.Utils;
using DSPRE.Resources;
using DSPRE.ROMFiles;
using ScintillaNET;
using ScintillaNET.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace DSPRE.Editors
{
    public partial class ScriptEditor : UserControl
    {
        public bool scriptEditorIsReady { get; set; } = false;
        private Scintilla ScriptTextArea;
        private Scintilla FunctionTextArea;
        private Scintilla ActionTextArea;
        private SearchManager scriptSearchManager;
        private SearchManager functionSearchManager;
        private SearchManager actionSearchManager;
        private Scintilla currentScintillaEditor;
        private SearchManager currentSearchManager;
        private bool scriptsDirty = false;
        private bool functionsDirty = false;
        private bool actionsDirty = false;
        private string cmdKeyWords = "";
        private string secondaryKeyWords = "";
        private string altCaseKeywords = "";
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
        public ScriptEditor()
        {
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
        public void SetupScriptEditor(MainProgram parent, bool force = false)
        {
            if (scriptEditorIsReady && !force) { return; }
            scriptEditorIsReady = true;
            this._parent = parent;
            SetupScriptEditorTextAreas();
            /* Extract essential NARCs sub-archives*/
            Helpers.statusLabelMessage("Setting up Script Editor...");
            Update();
            DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames> { RomInfo.DirNames.scripts }); //12 = scripts Narc Dir

            if (Resources.ScriptDatabase.pokemonNames == null)
            {
                // Unpack text archives first if not unpacked yet
                DSUtils.TryUnpackNarcs(new List<RomInfo.DirNames> { RomInfo.DirNames.textArchives });

                Resources.ScriptDatabase.InitializePokemonNames();
                Resources.ScriptDatabase.InitializeItemNames();
                Resources.ScriptDatabase.InitializeMoveNames();
                Resources.ScriptDatabase.InitializeTrainerNames();
            }

            // Export scripts on first open with progress dialog, same as text archives
            int scriptCount = Filesystem.GetScriptCount();
            using (var loadingForm = new LoadingForm(scriptCount, "Loading script files..."))
            {
                loadingForm.Shown += (s, e) =>
                {
                    Task.Run(() =>
                    {
                        ROMFiles.ScriptFile.ExportAllScripts(
                            suppressErrors: true,
                            progressCallback: (current, total) =>
                            {
                                if (loadingForm.IsHandleCreated)
                                {
                                    loadingForm.Invoke((Action)(() => loadingForm.UpdateProgress(current)));
                                }
                            });

                        if (loadingForm.IsHandleCreated)
                        {
                            loadingForm.Invoke((Action)(() => loadingForm.Close()));
                        }
                    });
                };
                loadingForm.ShowDialog();

                var invalidCommands = ROMFiles.ScriptFile.GetInvalidCommands();
                if (invalidCommands.Count > 0)
                {
                    HandleInvalidScriptCommands(invalidCommands);
                }
            }

            populate_selectScriptFileComboBox(0);
            UpdateScriptNumberCheckBox((NumberStyles)SettingsManager.Settings.scriptEditorFormatPreference);
            Helpers.statusLabelMessage();
        }

        private void HandleInvalidScriptCommands(List<(int fileID, ushort commandID, long offset)> invalidCommands)
        {
            var uniqueCommands = invalidCommands.Select(c => c.commandID).Distinct().OrderBy(x => x).ToList();
            string commandList = string.Join(", ", uniqueCommands.Select(c => $"0x{c:X4}"));

            var affectedFiles = invalidCommands.Select(c => c.fileID).Distinct().OrderBy(x => x).ToList();
            string fileList = string.Join(", ", affectedFiles.Select(f => f.ToString("D4")));

            var result = MessageBox.Show(
                $"DSPRE could not interpret {invalidCommands.Count} script command(s) across {affectedFiles.Count} file(s).\n\n" +
                $"Affected files: {fileList}\n" +
                $"Unrecognized commands: {commandList}\n\n" +
                $"This may happen if your project uses custom script commands. " +
                $"Would you like to load a custom script command database for this project?\n\n" +
                $"If you select 'Yes', DSPRE will reparse all scripts with the new database.\nIf you select 'No', the affected files will be incomplete and read-only to prevent data loss.",
                "Invalid Script Commands Detected",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                LoadCustomScriptDatabase();
            }
        }

        private void LoadCustomScriptDatabase()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Select Custom Script Command Database";
                dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                dialog.InitialDirectory = Program.DatabasePath;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Helpers.statusLabelMessage("Loading custom script database...");
                        Update();

                        // Save database permanently to edited_databases
                        string editedDatabasesDir = Path.Combine(Program.DatabasePath, "edited_databases");
                        Directory.CreateDirectory(editedDatabasesDir);

                        string baseFileName = Path.GetFileNameWithoutExtension(RomInfo.projectName);
                        string romFileNameClean = baseFileName.EndsWith("_DSPRE_contents")
                            ? baseFileName.Substring(0, baseFileName.Length - "_DSPRE_contents".Length)
                            : baseFileName;

                        string targetJsonPath = Path.Combine(editedDatabasesDir, $"{romFileNameClean}_scrcmd_database.json");

                        File.Copy(dialog.FileName, targetJsonPath, overwrite: true);
                        AppLogger.Info($"Script database saved permanently to: {targetJsonPath}");

                        // Re-export with new database and progress dialog
                        int scriptCount = Filesystem.GetScriptCount();
                        List<(int fileID, ushort commandID, long offset)> remainingInvalidCommands = null;

                        using (var loadingForm = new LoadingForm(scriptCount, "Reparsing scripts with new database..."))
                        {
                            loadingForm.Shown += (s, e) =>
                            {
                                Task.Run(() =>
                                {
                                    remainingInvalidCommands = ROMFiles.ScriptFile.ReloadDatabaseAndReparseAll(
                                        targetJsonPath,
                                        progressCallback: (current, total) =>
                                        {
                                            if (loadingForm.IsHandleCreated)
                                            {
                                                loadingForm.Invoke((Action)(() => loadingForm.UpdateProgress(current)));
                                            }
                                        });

                                    if (loadingForm.IsHandleCreated)
                                    {
                                        loadingForm.Invoke((Action)(() => loadingForm.Close()));
                                    }
                                });
                            };

                            loadingForm.ShowDialog();
                        }

                        // Refresh the script editor
                        SetupScriptEditorTextAreas();
                        populate_selectScriptFileComboBox(selectScriptFileComboBox.SelectedIndex);

                        Helpers.statusLabelMessage();

                        // Check if there are still errors after loading custom database
                        if (remainingInvalidCommands.Count > 0)
                        {
                            var uniqueCommands = remainingInvalidCommands.Select(c => c.commandID).Distinct().OrderBy(x => x).ToList();
                            string commandList = string.Join(", ", uniqueCommands.Select(c => $"0x{c:X4}"));

                            var affectedFiles = remainingInvalidCommands.Select(c => c.fileID).Distinct().OrderBy(x => x).ToList();
                            string fileList = string.Join(", ", affectedFiles.Select(f => f.ToString("D4")));

                            MessageBox.Show(
                                $"Script database loaded, but {remainingInvalidCommands.Count} script command(s) across {affectedFiles.Count} file(s) still could not be parsed.\n\n" +
                                $"Affected files: {fileList}\n" +
                                $"Unrecognized commands: {commandList}\n\n" +
                                $"Affected script files were not exported to plaintext to prevent data loss. " +
                                $"You can load a different database or manually fix the database file to add these commands.",
                                "Partial Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Script database loaded successfully and all scripts have been reparsed.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        Helpers.statusLabelMessage();
                        MessageBox.Show($"Failed to load custom script database:\n{ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        public void OpenScriptEditor(MainProgram parent, int scriptFileID)
        {
            SetupScriptEditor(parent);

            scriptEditorTabControl.SelectedIndex = 0;
            selectScriptFileComboBox.SelectedIndex = scriptFileID;
            if (EditorPanels.PopoutRegistry.TryGetHost(this, out var host))
            {
                host.Focus();
            }
            else
            {
                EditorPanels.mainTabControl.SelectedTab = EditorPanels.scriptEditorTabPage;
            }
        }
        private void SetupScriptEditorTextAreas()
        {
            //PREPARE SCRIPT EDITOR KEYWORDS
            cmdKeyWords = String.Join(" ", RomInfo.ScriptCommandNamesDict.Values) +
                            " " + String.Join(" ", ScriptDatabase.movementsDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Name).Values);
            secondaryKeyWords = String.Join(" ", RomInfo.ScriptComparisonOperatorsDict.Values) +
                                " " + String.Join(" ", ScriptDatabase.specialOverworlds.Values) +
                                " " + String.Join(" ", ScriptDatabase.overworldDirections.Values) +
                                " " + String.Join(" ", ScriptDatabase.pokemonNames.Values) +
                                " " + String.Join(" ", ScriptDatabase.itemNames.Values) +
                                " " + String.Join(" ", ScriptDatabase.moveNames.Values) +
                                " " + String.Join(" ", ScriptDatabase.soundNames.Values) +
                                " " + String.Join(" ", ScriptDatabase.trainerNames.Values) +
                                " " + ScriptFile.ContainerTypes.Script.ToString() +
                                " " + ScriptFile.ContainerTypes.Function.ToString() +
                                " " + ScriptFile.ContainerTypes.Action.ToString() +
                                " " + Event.EventType.Overworld +
                                " " + Overworld.MovementCodeKW;

            altCaseKeywords += " " + cmdKeyWords.ToUpper() + " " + cmdKeyWords.ToLower();
            altCaseKeywords += " " + secondaryKeyWords.ToUpper() + " " + secondaryKeyWords.ToLower();

            // CREATE CONTROLS
            ScriptTextArea = new Scintilla();
            scriptSearchManager = new SearchManager(EditorPanels.MainProgram, ScriptTextArea, panelFindScriptTextBox, PanelSearchScripts);
            scintillaScriptsPanel.Controls.Clear();
            scintillaScriptsPanel.Controls.Add(ScriptTextArea);

            FunctionTextArea = new Scintilla();
            functionSearchManager = new SearchManager(EditorPanels.MainProgram, FunctionTextArea, panelFindFunctionTextBox, PanelSearchFunctions);
            scintillaFunctionsPanel.Controls.Clear();
            scintillaFunctionsPanel.Controls.Add(FunctionTextArea);

            ActionTextArea = new Scintilla();
            actionSearchManager = new SearchManager(EditorPanels.MainProgram, ActionTextArea, panelFindActionTextBox, PanelSearchActions);
            scintillaActionsPanel.Controls.Clear();
            scintillaActionsPanel.Controls.Add(ActionTextArea);

            currentScintillaEditor = ScriptTextArea;
            currentSearchManager = scriptSearchManager;

            // BASIC CONFIG
            ScriptTextArea.TextChanged += (OnTextChangedScript);
            FunctionTextArea.TextChanged += (OnTextChangedFunction);
            ActionTextArea.TextChanged += (OnTextChangedAction);

            ScriptTextArea.CharAdded += OnCharAdded;
            FunctionTextArea.CharAdded += OnCharAdded;
            ActionTextArea.CharAdded += OnCharAdded;

            // INITIAL VIEW CONFIG
            InitialViewConfig(ScriptTextArea);
            InitialViewConfig(FunctionTextArea);
            InitialViewConfig(ActionTextArea);

            InitSyntaxColoring(ScriptTextArea);
            InitSyntaxColoring(FunctionTextArea);
            InitSyntaxColoring(ActionTextArea);

            // NUMBER MARGIN
            InitNumberMargin(ScriptTextArea, ScriptTextArea_MarginClick);
            InitNumberMargin(FunctionTextArea, FunctionTextArea_MarginClick);
            InitNumberMargin(ActionTextArea, ActionTextArea_MarginClick);

            // BOOKMARK MARGIN
            InitBookmarkMargin(ScriptTextArea);
            InitBookmarkMargin(FunctionTextArea);
            InitBookmarkMargin(ActionTextArea);

            // CODE FOLDING MARGIN
            InitCodeFolding(ScriptTextArea);
            InitCodeFolding(FunctionTextArea);
            InitCodeFolding(ActionTextArea);

            // INIT HOTKEYS
            InitHotkeys(ScriptTextArea, scriptSearchManager);
            InitHotkeys(FunctionTextArea, functionSearchManager);
            InitHotkeys(ActionTextArea, actionSearchManager);

            // INIT TOOLTIPS DWELLING
            /*
            ScriptTextArea.MouseDwellTime = 300;
            ScriptTextArea.DwellEnd += TextArea_DwellEnd;
            ScriptTextArea.DwellStart += TextArea_DwellStart;
            FunctionTextArea.MouseDwellTime = 300;
            FunctionTextArea.DwellEnd += TextArea_DwellEnd;
            FunctionTextArea.DwellStart += TextArea_DwellStart;
            */
        }

        private void populate_selectScriptFileComboBox(int selectedIndex = 0)
        {
            selectScriptFileComboBox.Items.Clear();
            int scriptCount = Filesystem.GetScriptCount();
            for (int i = 0; i < scriptCount; i++)
            {
                // ScriptFile currentScriptFile = new ScriptFile(i, true, true);
                // selectScriptFileComboBox.Items.Add(currentScriptFile);
                selectScriptFileComboBox.Items.Add($"Script File {i}");
            }
            selectScriptFileComboBox.SelectedIndex = selectedIndex;
        }
        private void InitialViewConfig(Scintilla textArea)
        {
            textArea.Dock = DockStyle.Fill;
            textArea.WrapMode = WrapMode.Word;
            textArea.IndentationGuides = IndentView.LookBoth;
            textArea.CaretPeriod = 500;
            textArea.CaretForeColor = Color.White;
            textArea.SetSelectionBackColor(true, Color.FromArgb(0x114D9C));
            textArea.WrapIndentMode = WrapIndentMode.Same;

            // Auto Completion
            textArea.AutoCMaxHeight = 20;
            textArea.AutoCIgnoreCase = true;
            textArea.AutoCOrder = Order.Custom;
            textArea.AutoCCancelAtStart = false;
            textArea.AutoCAutoHide = false;
        }

        private void InitSyntaxColoring(Scintilla textArea)
        {
            // Configure the default style
            textArea.StyleResetDefault();
            textArea.Styles[Style.Default].Font = "Consolas";
            textArea.Styles[Style.Default].Size = 12;
            textArea.Styles[Style.Default].BackColor = Color.FromArgb(0x212121);
            textArea.Styles[Style.Default].ForeColor = Color.FromArgb(0xFFFFFF);
            textArea.StyleClearAll();

            // Configure the lexer styles
            textArea.Styles[Style.Python.Identifier].ForeColor = Color.FromArgb(0xD0DAE2);
            textArea.Styles[Style.Python.CommentLine].ForeColor = Color.FromArgb(0x40BF57);
            textArea.Styles[Style.Python.Number].ForeColor = Color.FromArgb(0xFFFF00);
            textArea.Styles[Style.Python.String].ForeColor = Color.FromArgb(0xFF00FF);
            textArea.Styles[Style.Python.Character].ForeColor = Color.FromArgb(0xE95454);
            textArea.Styles[Style.Python.Operator].ForeColor = Color.FromArgb(0xFFFF00);
            textArea.Styles[Style.Python.Word].ForeColor = Color.FromArgb(0x48A8EE);
            textArea.Styles[Style.Python.Word2].ForeColor = Color.FromArgb(0xF98906);

            textArea.Lexer = Lexer.Python;

            textArea.SetKeywords(0, cmdKeyWords);
            textArea.SetKeywords(1, secondaryKeyWords);
            textArea.SetKeywords(2, altCaseKeywords);
        }

        private void InitNumberMargin(Scintilla textArea, EventHandler<MarginClickEventArgs> textArea_MarginClick)
        {
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
        private void InitBookmarkMargin(Scintilla textArea)
        {
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
        private void InitCodeFolding(Scintilla textArea)
        {
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
            for (int i = 25; i <= 31; i++)
            {
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
        private void InitHotkeys(Scintilla scintillaTb, SearchManager sm)
        {
            // register the hotkeys with the form
            HotKeyManager.AddHotKey(scintillaTb, sm.OpenSearch, Keys.F, true);
            HotKeyManager.AddHotKey(scintillaTb, () => Uppercase(scintillaTb), Keys.U, true);
            HotKeyManager.AddHotKey(scintillaTb, () => Lowercase(scintillaTb), Keys.L, true);
            HotKeyManager.AddHotKey(scintillaTb, () => ZoomIn(scintillaTb), Keys.Oemplus, true);
            HotKeyManager.AddHotKey(scintillaTb, () => ZoomOut(scintillaTb), Keys.OemMinus, true);
            HotKeyManager.AddHotKey(scintillaTb, () => ZoomDefault(scintillaTb), Keys.D0, true);
            HotKeyManager.AddHotKey(scintillaTb, sm.CloseSearch, Keys.Escape);
            HotKeyManager.AddHotKey(scintillaTb, () => ToggleAutoComplete(scintillaTb), Keys.Space, true);
            HotKeyManager.AddHotKey(scintillaTb, () => SaveScriptFile(scintillaTb, false), Keys.S, true);

            // remove conflicting hotkeys from scintilla
            scintillaTb.ClearCmdKey(Keys.Control | Keys.F);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.Space);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.H);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.L);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.U);
            scintillaTb.ClearCmdKey(Keys.Control | Keys.S);

            // remove ctrl + space
            scintillaTb.KeyDown += (sender, e) =>
            {
                if (e.Control && e.KeyCode == Keys.Space)
                {
                    e.SuppressKeyPress = true; // Prevents the space from being inserted
                }
            };

        }
        private void Uppercase(Scintilla textArea)
        {
            // save the selection
            int start = textArea.SelectionStart;
            int end = textArea.SelectionEnd;
            // modify the selected text
            textArea.ReplaceSelection(textArea.GetTextRange(start, end - start).ToUpper());
            // preserve the original selection
            textArea.SetSelection(start, end);
        }
        private void Lowercase(Scintilla textArea)
        {
            // save the selection
            int start = textArea.SelectionStart;
            int end = textArea.SelectionEnd;
            // modify the selected text
            textArea.ReplaceSelection(textArea.GetTextRange(start, end - start).ToLower());
            // preserve the original selection
            textArea.SetSelection(start, end);
        }
        private void ZoomIn(Scintilla textArea)
        {
            textArea.ZoomIn();
        }
        private void ZoomOut(Scintilla textArea)
        {
            textArea.ZoomOut();
        }
        private void ZoomDefault(Scintilla textArea)
        {
            textArea.Zoom = 0;
        }

        private void ToggleAutoComplete(Scintilla textArea)
        {
            if (textArea.AutoCActive)
            {
                textArea.AutoCCancel();
                return;
            }
            CompleteCurrent(textArea);
        }

        private void CompleteCurrent(Scintilla textArea)
        {
            int currentPos = textArea.CurrentPosition;
            int wordStartPos = textArea.WordStartPosition(currentPos, true);

            int wordLen = Math.Max(currentPos - wordStartPos, 0);

            string currentWord = textArea.GetTextRange(wordStartPos, wordLen);

            textArea.AutoCShow(wordLen, cmdKeyWords + secondaryKeyWords);
        }

        private void SaveScriptFile(Scintilla textArea, bool showMessage)
        {
            /* Create new ScriptFile object using the values in the script editor */
            int fileID = currentScriptFile.fileID;

            if (currentScriptFile.parseFailedDueToInvalidCommand)
            {
                MessageBox.Show(
                    "This script file could not be fully parsed due to unrecognized commands and is READ-ONLY.\n\n" +
                    "To fix this, load a custom script command database that includes all commands used in this script, " +
                    "modify your db to include changes you made, " +
                    "or restore the script file from a backup.",
                    "Cannot Save Incomplete Script",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            ScriptTextArea.ReadOnly = true;
            FunctionTextArea.ReadOnly = true;
            ActionTextArea.ReadOnly = true;

            ScriptFile userEdited = new ScriptFile(
              scriptLines: ScriptTextArea.Lines.ToStringsList(trim: true),
              functionLines: FunctionTextArea.Lines.ToStringsList(trim: true),
              actionLines: ActionTextArea.Lines.ToStringsList(trim: true),
              fileID
            );

            if (userEdited.hasNoScripts)
            {
                MessageBox.Show("This " + nameof(ScriptFile) + " couldn't be saved. A minimum of one script is required.", "Can't save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //check if ScriptFile instance was created successfully
            if (userEdited.SaveToFileDefaultDir(fileID, showMessage))
            {
                currentScriptFile = userEdited;
                ScriptEditorSetClean();
            }

            ScriptTextArea.ReadOnly = false;
            FunctionTextArea.ReadOnly = false;
            ActionTextArea.ReadOnly = false;
        }

        private void ScriptEditorSetClean()
        {
            Helpers.DisableHandlers();

            scriptsTabPage.Text = ScriptFile.ContainerTypes.Script.ToString() + "s";
            functionsTabPage.Text = ScriptFile.ContainerTypes.Function.ToString() + "s";
            actionsTabPage.Text = ScriptFile.ContainerTypes.Action.ToString() + "s";
            scriptsDirty = functionsDirty = actionsDirty = false;

            Helpers.EnableHandlers();
        }

        private void OnTextChangedScript(object sender, EventArgs e)
        {
            ScriptTextArea.Margins[NUMBER_MARGIN].Width = ScriptTextArea.Lines.Count.ToString().Length * 13;

            if (scriptsDirty)
            {
                return;
            }

            scriptsDirty = true;
            scriptsTabPage.Text = ScriptFile.ContainerTypes.Script.ToString() + "s" + "*";
        }

        private void OnTextChangedFunction(object sender, EventArgs e)
        {
            FunctionTextArea.Margins[NUMBER_MARGIN].Width = FunctionTextArea.Lines.Count.ToString().Length * 13;

            if (functionsDirty)
            {
                return;
            }

            functionsDirty = true;
            functionsTabPage.Text = ScriptFile.ContainerTypes.Function.ToString() + "s" + "*";
        }

        private void OnTextChangedAction(object sender, EventArgs e)
        {
            ActionTextArea.Margins[NUMBER_MARGIN].Width = ActionTextArea.Lines.Count.ToString().Length * 13;

            if (actionsDirty)
            {
                return;
            }

            actionsDirty = true;
            actionsTabPage.Text = ScriptFile.ContainerTypes.Action.ToString() + "s" + "*";
        }

        private void OnCharAdded(object sender, EventArgs e)
        {
            if (!(sender is Scintilla textArea))
            {
                return;
            }

            if (!textArea.AutoCActive)
            {
                // If the AutoComplete is active, we don't want to do anything else
                return;
            }

            //CompleteCurrent(textArea);

        }

        private void ScriptTextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
            MarginClick(ScriptTextArea, e);
        }

        private void FunctionTextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
            MarginClick(FunctionTextArea, e);
        }

        private void ActionTextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
            MarginClick(ActionTextArea, e);
        }

        private void MarginClick(Scintilla textArea, MarginClickEventArgs e)
        {
            if (e.Margin == BOOKMARK_MARGIN)
            {
                // Do we have a marker for this line?
                const uint mask = (1 << BOOKMARK_MARKER);
                Line line = textArea.Lines[textArea.LineFromPosition(e.Position)];
                if ((line.MarkerGet() & mask) > 0)
                {
                    // Remove existing bookmark
                    line.MarkerDelete(BOOKMARK_MARKER);
                }
                else
                {
                    // Add bookmark
                    line.MarkerAdd(BOOKMARK_MARKER);
                }
            }
        }
        private void selectScriptFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayScript();
        }
        public void UpdateScriptNumberCheckBox(NumberStyles toSet)
        {
            Helpers.DisableHandlers();
            SettingsManager.Settings.scriptEditorFormatPreference = (int)toSet;
            switch ((NumberStyles)SettingsManager.Settings.scriptEditorFormatPreference)
            {
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
            AppLogger.Debug("changed style to " + SettingsManager.Settings.scriptEditorFormatPreference);
            Helpers.EnableHandlers();
        }
        private void UpdateScriptNumberFormat(NumberStyles numberStyle)
        {
            if (Helpers.HandlersEnabled)
            {
                NumberStyles old = (NumberStyles)SettingsManager.Settings.scriptEditorFormatPreference; //Local Backup
                SettingsManager.Settings.scriptEditorFormatPreference = (int)numberStyle;
                if (!DisplayScript())
                {
                    UpdateScriptNumberCheckBox(old); //Restore old checkbox status! Script couldn't be redrawn
                }
            }
        }
        private void UpdateScriptNumberFormatNoPref(object sender, EventArgs e)
        {
            UpdateScriptNumberFormat(NumberStyles.None);
        }
        private void UpdateScriptNumberFormatDec(object sender, EventArgs e)
        {
            UpdateScriptNumberFormat(NumberStyles.Integer);
        }
        private void UpdateScriptNumberFormatHex(object sender, EventArgs e)
        {
            UpdateScriptNumberFormat(NumberStyles.HexNumber);
        }
        private bool DisplayScript()
        {
            AppLogger.Debug("Script Reload has been requested");
            /* clear controls */
            if (Helpers.HandlersDisabled || selectScriptFileComboBox.SelectedItem == null)
            {
                return false;
            }

            if (scriptsDirty || functionsDirty || actionsDirty)
            {
                DialogResult d = MessageBox.Show("There are unsaved changes in this Script File.\nDo you wish to discard them?", "Unsaved work", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (!d.Equals(DialogResult.Yes))
                {
                    Helpers.DisableHandlers();
                    // selectScriptFileComboBox.SelectedItem = currentScriptFile;
                    selectScriptFileComboBox.SelectedIndex = (int)currentScriptFile.fileID;
                    Helpers.EnableHandlers();
                    return false;
                }
            }
            Helpers.DisableHandlers();

            ScriptFile lastScriptFile = currentScriptFile;
            int fileID = selectScriptFileComboBox.SelectedIndex;

            ScriptTextArea.ClearAll();
            FunctionTextArea.ClearAll();
            ActionTextArea.ClearAll();

            scriptsNavListbox.Items.Clear();
            functionsNavListbox.Items.Clear();
            actionsNavListbox.Items.Clear();

            if (TryLoadPlaintextDirect(fileID, out bool isLevelScript))
            {
                // Create a minimal ScriptFile just to hold the fileID for saving later
                currentScriptFile = new ScriptFile(new List<ScriptCommandContainer>(), new List<ScriptCommandContainer>(), new List<ScriptActionContainer>(), fileID);
                currentScriptFile.isLevelScript = isLevelScript;
            }
            else
            {
                // Fallback: load and parse normally (binary or older plaintext)
                currentScriptFile = new ScriptFile(fileID);
            }

            //prevent buttons from flickering when the combobox selection changes
            bool typeChanged = true;
            if (lastScriptFile != null)
            {
                typeChanged = lastScriptFile.isLevelScript != currentScriptFile.isLevelScript;
            }
            if (typeChanged)
            {
                if (currentScriptFile.isLevelScript)
                {
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
                }
                else
                {
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

            if (!currentScriptFile.isLevelScript)
            {
                displayScriptFile(ScriptFile.ContainerTypes.Script, currentScriptFile.allScripts, scriptsNavListbox, ScriptTextArea);
                displayScriptFile(ScriptFile.ContainerTypes.Function, currentScriptFile.allFunctions, functionsNavListbox, FunctionTextArea);
                displayScriptFileActions(ScriptFile.ContainerTypes.Action, currentScriptFile.allActions, actionsNavListbox, ActionTextArea);

                if (currentScriptFile.parseFailedDueToInvalidCommand)
                {
                    MessageBox.Show(
                        $"Warning: Script file {currentScriptFile.fileID:D4} could not be fully parsed due to unrecognized script commands.\n\n" +
                        "The script shown below is INCOMPLETE and may be missing commands at the end.\n" +
                        "This script is READ-ONLY to prevent data loss.\n\n" +
                        "To fix this, load a custom script command database that includes all commands used in this script, modify your existing one to account for changes you made or try to restore this script file if it is corrupted.",
                        "Incomplete Script File",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    ScriptTextArea.ReadOnly = true;
                    FunctionTextArea.ReadOnly = true;
                    ActionTextArea.ReadOnly = true;
                }
                else
                {
                    ScriptTextArea.ReadOnly = false;
                    FunctionTextArea.ReadOnly = false;
                    ActionTextArea.ReadOnly = false;
                }
            }

            ScriptEditorSetClean();
            Helpers.statusLabelMessage();
            Helpers.EnableHandlers();
            return true;
        }

        static void displayScriptFile(ScriptFile.ContainerTypes containerType, List<ScriptCommandContainer> commandList, ListBox navListBox, Scintilla textArea)
        {
            string buffer = "";
            /* Add commands */
            for (int i = 0; i < commandList.Count; i++)
            {
                ScriptCommandContainer scriptCommandContainer = commandList[i];

                /* Write header */
                string header = containerType + " " + (i + 1);
                buffer += header + ':' + Environment.NewLine;
                navListBox.Items.Add(header);

                /* If current command is identical to another, print UseScript instead of commands */
                if (scriptCommandContainer.usedScriptID < 0)
                {
                    if (scriptCommandContainer.commands != null)
                    {
                        for (int j = 0; j < scriptCommandContainer.commands.Count; j++)
                        {
                            ScriptCommand command = scriptCommandContainer.commands[j];
                            if (command.id != null && !ScriptDatabase.endCodes.Contains((ushort)command.id))
                            {
                                buffer += '\t';
                            }

                            buffer += command.name + Environment.NewLine;
                        }
                    }
                }
                else
                {
                    buffer += '\t' + "UseScript_#" + scriptCommandContainer.usedScriptID + Environment.NewLine;
                }

                textArea.AppendText(buffer + Environment.NewLine);
                buffer = "";
            }
        }

        static void displayScriptFileActions(ScriptFile.ContainerTypes containerType, List<ScriptActionContainer> commandList, ListBox navListBox, Scintilla textArea)
        {
            /* Add movements */
            string buffer = "";
            for (int i = 0; i < commandList.Count; i++)
            {
                ScriptActionContainer currentCommand = commandList[i];

                string header = containerType + " " + (i + 1);
                buffer += header + ':' + Environment.NewLine;
                navListBox.Items.Add(header);

                for (int j = 0; j < currentCommand.commands.Count; j++)
                {
                    ScriptAction command = currentCommand.commands[j];
                    if (!ScriptDatabase.movementEndCodes.Contains(command.id))
                    {
                        buffer += '\t';
                    }

                    buffer += command.name + Environment.NewLine;
                }

                textArea.AppendText(buffer + Environment.NewLine);
                buffer = "";
            }
        }

        private void scriptEditorZoomInButton_Click(object sender, EventArgs e)
        {
            ZoomIn(currentScintillaEditor);
        }

        private void scriptEditorZoomOutButton_Click(object sender, EventArgs e)
        {
            ZoomOut(currentScintillaEditor);
        }

        private void scriptEditorZoomResetButton_Click(object sender, EventArgs e)
        {
            ZoomDefault(currentScintillaEditor);
        }

        private void scriptEditorTabControl_TabIndexChanged(object sender, EventArgs e)
        {
            if (scriptEditorTabControl.SelectedTab == scriptsTabPage)
            {
                currentSearchManager = scriptSearchManager;
                currentScintillaEditor = ScriptTextArea;
            }
            else if (scriptEditorTabControl.SelectedTab == functionsTabPage)
            {
                currentSearchManager = functionSearchManager;
                currentScintillaEditor = FunctionTextArea;
            }
            else
            {
                //Actions
                currentSearchManager = actionSearchManager;
                currentScintillaEditor = ActionTextArea;
            }
        }

        private void removeScriptFileButton_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Are you sure you want to delete the last Script File?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d.Equals(DialogResult.Yes))
            {
                /* Delete script file */
                string path = Filesystem.GetScriptPath(selectScriptFileComboBox.Items.Count - 1);
                File.Delete(path);
                /* Check if currently selected file is the last one, and in that case select the one before it */
                int lastIndex = selectScriptFileComboBox.Items.Count - 1;
                if (selectScriptFileComboBox.SelectedIndex == lastIndex)
                {
                    selectScriptFileComboBox.SelectedIndex--;
                }
                /* Remove item from ComboBox */
                selectScriptFileComboBox.Items.RemoveAt(lastIndex);
            }
        }
        private void addScriptFileButton_Click(object sender, EventArgs e)
        {
            /* Add new event file to event folder */
            int fileID = selectScriptFileComboBox.Items.Count;

            ScriptFile scriptFile = new ScriptFile(
              scriptLines: new Scintilla { Text = "Script 1:\nEnd" }.Lines.ToStringsList(trim: true),
              functionLines: null,
              actionLines: null,
              fileID
            );

            //check if ScriptFile instance was created successfully
            if (scriptFile.SaveToFileDefaultDir(fileID, showSuccessMessage: false))
            {
                /* Update ComboBox and select new file */
                selectScriptFileComboBox.Items.Add(scriptFile);
                selectScriptFileComboBox.SelectedItem = scriptFile;
            }
        }

        private void saveScriptFileButton_Click(object sender, EventArgs e)
        {
            SaveScriptFile(null, true);
        }
        private void exportScriptFileButton_Click(object sender, EventArgs e)
        {
            string baseFileName = currentScriptFile.ToString();

            using (SaveFileDialog sf = new SaveFileDialog())
            {
                sf.Filter = "Script File (*.script)|*.script";
                sf.FileName = baseFileName + ".script";
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Build the file content with headers and sections
                        StringBuilder content = new StringBuilder();

                        // Add file header
                        content.AppendLine("/*");
                        content.AppendLine(" * DSPRE Script File");

                        string romFileName = Path.GetFileNameWithoutExtension(RomInfo.projectName);
                        string romFileNameClean = romFileName.EndsWith("_DSPRE_contents")
                            ? romFileName.Substring(0, romFileName.Length - "_DSPRE_contents".Length)
                            : romFileName;
                        content.AppendLine(" * Rom ID: " + romFileNameClean);
                        content.AppendLine(" * Game: " + RomInfo.gameFamily);
                        content.AppendLine($" * File: {baseFileName}");
                        content.AppendLine($" * Generated: {DateTime.Now}");
                        content.AppendLine(" */");
                        content.AppendLine();

                        // Add Scripts section
                        content.AppendLine("//===== SCRIPTS =====//");
                        content.AppendLine(ScriptTextArea.Text.Trim());
                        content.AppendLine();

                        // Add Functions section
                        content.AppendLine("//===== FUNCTIONS =====//");
                        content.AppendLine(FunctionTextArea.Text.Trim());
                        content.AppendLine();

                        // Add Actions section
                        content.AppendLine("//===== ACTIONS =====//");
                        content.AppendLine(ActionTextArea.Text.Trim());

                        File.WriteAllText(sf.FileName, content.ToString());
                        MessageBox.Show("Script file exported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting script file: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void importScriptFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog of = new OpenFileDialog())
            {
                of.Filter = "Script Files (*.script)|*.script|All Files (*.*)|*.*";

                if (of.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        string content = File.ReadAllText(of.FileName);

                        // Use shared loading logic (don't populate nav lists - user will save/reload)
                        if (!LoadPlaintextIntoEditor(content, false, out _))
                        {
                            throw new FormatException("Invalid script file format. Missing required section headers.");
                        }

                        MessageBox.Show("Script file imported successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error importing script file: {ex.Message}", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ImportMatchingFiles(string directory, string baseFileName)
        {
            // Try to import matching function file
            string funcFile = Path.Combine(directory, baseFileName + ".func");
            if (File.Exists(funcFile))
            {
                FunctionTextArea.Text = File.ReadAllText(funcFile);
            }

            // Try to import matching action file
            string actionFile = Path.Combine(directory, baseFileName + ".action");
            if (File.Exists(actionFile))
            {
                ActionTextArea.Text = File.ReadAllText(actionFile);
            }
        }
        private void viewLevelScriptButton_Click(object sender, EventArgs e)
        {
            EditorPanels.levelScriptEditor.OpenLevelScriptEditor(this._parent, selectScriptFileComboBox.SelectedIndex);
        }
        private void locateCurrentScriptFile_Click(object sender, EventArgs e)
        {
            string path = Filesystem.GetScriptPath(selectScriptFileComboBox.SelectedIndex);
            Helpers.ExplorerSelect(path);

        }

        private void openInVSCode_Click(object sender, EventArgs e)
        {
            if (currentScriptFile == null)
            {
                MessageBox.Show("No script file is currently loaded.", "Cannot Open", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (currentScriptFile.parseFailedDueToInvalidCommand)
            {
                MessageBox.Show(
                    "This script file could not be fully parsed due to unrecognized commands and is READ-ONLY.\n\n" +
                    "Opening it in VSCode is disabled to prevent accidental editing of incomplete data.",
                    "Cannot Open Incomplete Script",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            int fileID = selectScriptFileComboBox.SelectedIndex;
            var (binPath, txtPath) = ScriptFile.GetFilePaths(fileID);

            if (!File.Exists(txtPath))
            {
                MessageBox.Show(
                    "Plaintext script file not found. This may be a level script or the file hasn't been exported yet.",
                    "Cannot Open",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string scriptsFolder = Path.GetDirectoryName(txtPath);

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c code \"{scriptsFolder}\" \"{txtPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                };

                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to open VSCode. Make sure VSCode is installed and 'code' is in your PATH.\n\nError: {ex.Message}",
                    "VSCode Launch Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void findNext(SearchManager searchManager)
        {
            searchManager.Find(true, false);
            scrollResultToTop(searchManager);
        }
        private void findPrev(SearchManager searchManager)
        {
            searchManager.Find(false, false);
            scrollResultToTop(searchManager);
        }
        private void findCurrent(SearchManager searchManager)
        {
            searchManager.Find(true, true);
            scrollResultToTop(searchManager);
        }
        private void TxtFindKeyDown(SearchManager searchManager, KeyEventArgs e)
        {
            if (HotKeyManager.IsHotkey(e, Keys.Enter))
            {
                findNext(searchManager);
            }
            if (HotKeyManager.IsHotkey(e, Keys.Enter, true) || HotKeyManager.IsHotkey(e, Keys.Enter, false, true))
            {
                findPrev(searchManager);
            }
        }
        private void BtnNextFindScript_Click(object sender, EventArgs e)
        {
            findNext(scriptSearchManager);
        }
        private void BtnPrevFindScript_Click(object sender, EventArgs e)
        {
            findPrev(scriptSearchManager);
        }
        private void panelFindScriptTextBox_TextChanged(object sender, EventArgs e)
        {
            findCurrent(scriptSearchManager);
        }
        private void scriptTxtFind_KeyDown(object sender, KeyEventArgs e)
        {
            TxtFindKeyDown(scriptSearchManager, e);
        }
        private void BtnCloseFindScript_Click(object sender, EventArgs e)
        {
            scriptSearchManager.CloseSearch();
        }

        private void BtnNextFindFunc_Click(object sender, EventArgs e)
        {
            findNext(functionSearchManager);
        }

        private void BtnPrevFindFunc_Click(object sender, EventArgs e)
        {
            findNext(functionSearchManager);
        }

        private void panelFindFunctionTextBox_TextChanged(object sender, EventArgs e)
        {
            findNext(functionSearchManager);
        }

        private void functionTxtFind_KeyDown(object sender, KeyEventArgs e)
        {
            TxtFindKeyDown(functionSearchManager, e);
        }

        private void BtnCloseFindFunc_Click(object sender, EventArgs e)
        {
            functionSearchManager.CloseSearch();
        }

        private void BtnNextFindActions_Click(object sender, EventArgs e)
        {
            findNext(actionSearchManager);
        }

        private void BtnPrevFindActions_Click(object sender, EventArgs e)
        {
            findNext(actionSearchManager);
        }

        private void panelFindActionTextBox_TextChanged(object sender, EventArgs e)
        {
            findNext(actionSearchManager);
        }

        private void actionTxtFind_KeyDown(object sender, KeyEventArgs e)
        {
            TxtFindKeyDown(actionSearchManager, e);
        }

        private void BtnCloseFindActions_Click(object sender, EventArgs e)
        {
            actionSearchManager.CloseSearch();
        }

        void scrollResultToTop(SearchManager searchManager)
        {
            int resultStart = searchManager.textAreaScintilla.CurrentLine - ScriptEditorSearchResult.ResultsPadding;
            searchManager.textAreaScintilla.FirstVisibleLine = resultStart;
        }

        private void NavigatorGoTo(ListBox listBox, TabPage tabPage, SearchManager searchManager, ScriptFile.ContainerTypes containerType)
        {
            if (listBox.SelectedIndex < 0)
            {
                return;
            }

            scriptEditorTabControl.SelectedTab = tabPage;
            int commandNumber = listBox.SelectedIndex + 1;
            string CommandBlockOpen = $"{containerType} {commandNumber}:";
            searchManager.Find(true, false, CommandBlockOpen);

            scrollResultToTop(searchManager);
        }

        private void scriptsNavListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            NavigatorGoTo((ListBox)sender, scriptsTabPage, scriptSearchManager, ScriptFile.ContainerTypes.Script);
        }

        private void functionsNavListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            NavigatorGoTo((ListBox)sender, functionsTabPage, functionSearchManager, ScriptFile.ContainerTypes.Function);
        }

        private void actionsNavListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            NavigatorGoTo((ListBox)sender, actionsTabPage, actionSearchManager, ScriptFile.ContainerTypes.Action);
        }

        private void openFindScriptEditorButton_Click(object sender, EventArgs e)
        {
            currentSearchManager.OpenSearch();
        }

        private void ScriptEditorExpandButton_Click(object sender, EventArgs e)
        {
            currentScintillaEditor.FoldAll(FoldAction.Expand);
        }

        private void ScriptEditorCollapseButton_Click(object sender, EventArgs e)
        {
            currentScintillaEditor.FoldAll(FoldAction.Contract);
        }

        private void scriptEditorWordWrapCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ScriptTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? WrapMode.Word : WrapMode.None;
            FunctionTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? WrapMode.Word : WrapMode.None;
            ActionTextArea.WrapMode = scriptEditorWordWrapCheckbox.Checked ? WrapMode.Word : WrapMode.None;
        }

        private void viewWhiteSpacesButton_Click(object sender, EventArgs e)
        {
            ScriptTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
            FunctionTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
            ActionTextArea.ViewWhitespace = scriptEditorWhitespacesCheckbox.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
        }

        private void searchInScriptsTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                searchInScriptsButton_Click(null, null);
            }
        }
        /// <summary>
        /// Tries to load plaintext directly without parsing for fast display.
        /// Only works if plaintext exists and is newer than binary.
        /// Returns true if successful, false if parsing is needed.
        /// </summary>
        private bool TryLoadPlaintextDirect(int fileID, out bool isLevelScript)
        {
            isLevelScript = false;

            var (binPath, txtPath) = ScriptFile.GetFilePaths(fileID);

            if (!File.Exists(txtPath))
            {
                return false;
            }

            if (File.Exists(binPath) && File.GetLastWriteTimeUtc(txtPath) < File.GetLastWriteTimeUtc(binPath))
            {
                return false;
            }

            try
            {
                string content = File.ReadAllText(txtPath);
                if (!LoadPlaintextIntoEditor(content, true, out isLevelScript))
                {
                    return false;
                }

                AppLogger.Info($"Script file {fileID:D4} loaded directly from plaintext (fast path)");
                return true;
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Fast plaintext loading failed for {fileID:D4}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Shared logic to load plaintext content into editor text areas.
        /// Used by both auto-loading and manual import.
        /// </summary>
        /// <param name="content">The plaintext file content</param>
        /// <param name="populateNavLists">Whether to populate navigation lists (true for load, false for import that's followed by reload)</param>
        /// <param name="isLevelScript">Output: whether this is a level script</param>
        /// <returns>True if successful, false if format is invalid</returns>
        private bool LoadPlaintextIntoEditor(string content, bool populateNavLists, out bool isLevelScript)
        {
            isLevelScript = false;

            const string SCRIPTS_HEADER = "//===== SCRIPTS =====//";
            const string FUNCTIONS_HEADER = "//===== FUNCTIONS =====//";
            const string ACTIONS_HEADER = "//===== ACTIONS =====//";

            int scriptsStart = content.IndexOf(SCRIPTS_HEADER);
            int functionsStart = content.IndexOf(FUNCTIONS_HEADER);
            int actionsStart = content.IndexOf(ACTIONS_HEADER);

            if (scriptsStart == -1 || functionsStart == -1 || actionsStart == -1)
            {
                return false; // Invalid format
            }

            string scriptsSection = content.Substring(
                scriptsStart + SCRIPTS_HEADER.Length,
                functionsStart - scriptsStart - SCRIPTS_HEADER.Length
            ).Trim();

            string functionsSection = content.Substring(
                functionsStart + FUNCTIONS_HEADER.Length,
                actionsStart - functionsStart - FUNCTIONS_HEADER.Length
            ).Trim();

            string actionsSection = content.Substring(
                actionsStart + ACTIONS_HEADER.Length
            ).Trim();

            ScriptTextArea.Text = scriptsSection;
            FunctionTextArea.Text = functionsSection;
            ActionTextArea.Text = actionsSection;

            if (populateNavLists)
            {
                PopulateNavListFromText(scriptsSection, scriptsNavListbox, "Script");
                PopulateNavListFromText(functionsSection, functionsNavListbox, "Function");
                PopulateNavListFromText(actionsSection, actionsNavListbox, "Action");
            }

            isLevelScript = string.IsNullOrWhiteSpace(scriptsSection) || !scriptsSection.Contains("Script ");

            return true;
        }

        /// <summary>
        /// Populates navigation listbox by counting "Type X:" headers in text
        /// </summary>
        private void PopulateNavListFromText(string text, ListBox navListBox, string containerType)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                string trimmed = line.Trim();
                if (trimmed.StartsWith(containerType + " ", StringComparison.InvariantCultureIgnoreCase) && trimmed.Contains(':'))
                {
                    // Extract "Script 1:" or "Function 2:" etc.
                    int colonIndex = trimmed.IndexOf(':');
                    if (colonIndex > 0)
                    {
                        string header = trimmed.Substring(0, colonIndex);
                        navListBox.Items.Add(header);
                    }
                }
            }
        }

        public List<ScriptFile> getScriptsToSearch()
        {
            List<ScriptFile> scriptsToSearch = new List<ScriptFile>();
            if (searchOnlyCurrentScriptCheckBox.Checked)
            {
                this.UIThread(() =>
                {
                    searchProgressBar.Maximum = 1;
                });
                int i = selectScriptFileComboBox.SelectedIndex;
                ScriptFile scriptFile = new ScriptFile(i);
                scriptsToSearch.Add(scriptFile);
                this.UIThread(() =>
                {
                    searchProgressBar.IncrementNoAnimation();
                });
            }
            else
            {
                this.UIThread(() =>
                {
                    searchProgressBar.Maximum = selectScriptFileComboBox.Items.Count;
                });
                for (int i = 0; i < selectScriptFileComboBox.Items.Count; i++)
                {
                    ScriptFile scriptFile = new ScriptFile(i);
                    scriptsToSearch.Add(scriptFile);
                    this.UIThread(() =>
                    {
                        searchProgressBar.IncrementNoAnimation();
                    });
                }
            }
            return scriptsToSearch;
        }
        private void searchInScriptsButton_Click(object sender, EventArgs e)
        {
            if (searchInScriptsTextBox.Text == "")
            {
                return;
            }
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (_sender, args) =>
            {
                this.UIThread(() =>
                {
                    searchInScriptsResultListBox.Items.Clear();
                    searchProgressBar.Value = 0;
                });
                List<ScriptFile> scriptsToSearch = getScriptsToSearch();

                string searchString = searchInScriptsTextBox.Text;
                Func<string, bool> searchCriteriaCS = (string s) => s.IndexOf(searchString, StringComparison.InvariantCulture) >= 0;
                Func<string, bool> searchCriteriaCI = (string s) => s.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
                Func<string, bool> searchCriteria = scriptSearchCaseSensitiveCheckBox.Checked ? searchCriteriaCS : searchCriteriaCI;

                List<ScriptEditorSearchResult> results = new List<ScriptEditorSearchResult>();
                foreach (ScriptFile scriptFile in scriptsToSearch)
                {
                    List<ScriptEditorSearchResult> scriptResults = SearchInScripts(scriptFile, scriptFile.allScripts, searchCriteria);
                    List<ScriptEditorSearchResult> functionResults = SearchInScripts(scriptFile, scriptFile.allFunctions, searchCriteria);
                    // List<ScriptEditorSearchResult> actionResults = SearchInScripts(scriptFile, scriptFile.allActions, searchCriteria);
                    results.AddRange(scriptResults);
                    results.AddRange(functionResults);
                    // results.AddRange(actionResults);
                }
                this.UIThread(() =>
                {
                    searchInScriptsResultListBox.Items.AddRange(results.ToArray());
                    searchProgressBar.Value = 0;
                });
            };

            bw.RunWorkerAsync();
        }

        private List<ScriptEditorSearchResult> SearchInScripts(ScriptFile scriptFile, List<ScriptCommandContainer> commandContainers, Func<string, bool> criteria)
        {
            List<ScriptEditorSearchResult> results = new List<ScriptEditorSearchResult>();

            // Check if plaintext parsing failed
            if (commandContainers == null)
            {
                return results;
            }

            for (int j = 0; j < commandContainers.Count; j++)
            {
                if (commandContainers[j].commands is null)
                {
                    continue;
                }

                ScriptCommandContainer scriptCommandContainer = commandContainers[j];
                foreach (ScriptCommand scriptCommand in scriptCommandContainer.commands)
                {
                    if (criteria(scriptCommand.name))
                    {
                        results.Add(new ScriptEditorSearchResult(scriptFile, scriptCommandContainer.containerType, j + 1, scriptCommand));
                    }
                }
            }

            return results;
        }
        private void searchInScriptsResultListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                goToSearchResult();
            }
        }
        private void searchInScripts_GoToEntryResult(object sender, MouseEventArgs e)
        {
            goToSearchResult();
        }
        private void goToSearchResult()
        {
            if (searchInScriptsResultListBox.SelectedItem == null) { return; }
            ScriptEditorSearchResult searchResult = (ScriptEditorSearchResult)searchInScriptsResultListBox.SelectedItem;
            ScriptFile scriptFile = searchResult.scriptFile;
            ScriptFile.ContainerTypes containerType = searchResult.containerType;
            selectScriptFileComboBox.SelectedIndex = scriptFile.fileID;

            if (containerType == ScriptFile.ContainerTypes.Script)
            {
                displaySearchResult(scriptsTabPage, scriptSearchManager, searchResult);
            }
            else if (containerType == ScriptFile.ContainerTypes.Function)
            {
                displaySearchResult(functionsTabPage, functionSearchManager, searchResult);
            }
            else if (containerType == ScriptFile.ContainerTypes.Action)
            {
                displaySearchResult(actionsTabPage, actionSearchManager, searchResult);
            }
        }

        private void displaySearchResult(TabPage tabPage, SearchManager searchManager, ScriptEditorSearchResult searchResult)
        {
            if (scriptEditorTabControl.SelectedTab != tabPage)
            {
                scriptEditorTabControl.SelectedTab = tabPage;
            }

            searchManager.Find(true, false, searchResult.CommandBlockOpen);
            int blockStart = searchManager.textAreaScintilla.CurrentLine - ScriptEditorSearchResult.ResultsPadding;

            searchManager.Find(true, false, searchResult.scriptCommand.name);
            int resultStart = searchManager.textAreaScintilla.CurrentLine - ScriptEditorSearchResult.ResultsPadding;
            if (scrollToBlockStartcheckBox.Checked)
            {
                searchManager.textAreaScintilla.FirstVisibleLine = blockStart;
            }
            else
            {
                searchManager.textAreaScintilla.FirstVisibleLine = resultStart;
            }
        }

    }

    public class ScriptEditorSearchResult
    {
        public readonly ScriptFile scriptFile;
        public readonly ScriptFile.ContainerTypes containerType;
        public readonly int commandNumber;
        public readonly ScriptCommand scriptCommand;

        public const int ResultsPadding = 1;

        public ScriptEditorSearchResult(ScriptFile scriptFile, ScriptFile.ContainerTypes containerType, int commandNumber, ScriptCommand scriptCommand)
        {
            this.scriptFile = scriptFile;
            this.containerType = containerType;
            this.commandNumber = commandNumber;
            this.scriptCommand = scriptCommand;
        }

        public string CommandBlockOpen { get { return $"{containerType} {commandNumber}:"; } }

        public override string ToString()
        {
            return $"File {scriptFile.fileID} - {CommandBlockOpen} {scriptCommand.name}";
        }
    }
}
