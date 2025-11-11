using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DSPRE.Editors
{
    public partial class LearnsetBulkEditor : Form
    {
        private DataGridView dataGridView;
        private BindingList<LearnsetEntry> learnsetData;
        private string[] pokemonNames;
        private string[] moveNames;
        private ToolStrip toolStrip;
        private StatusStrip statusStrip;
        private ContextMenuStrip contextMenu;
        private bool isDirty = false;
        private bool changesSaved = false;

        public LearnsetBulkEditor(BindingList<LearnsetEntry> learnsetData, string[] pokemonNames, string[] moveNames)
        {
            //InitializeComponent(); // we set up controls manually
            this.learnsetData = learnsetData;
            this.pokemonNames = pokemonNames;
            this.moveNames = moveNames;
            SetupControls();

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isDirty && !changesSaved)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Are you sure you want to exit?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            this.DialogResult = changesSaved ? DialogResult.OK : DialogResult.Cancel;
            base.OnFormClosed(e);
        }

        private void SetupControls()
        {
            this.Size = new Size(1000, 700);
            this.Text = "Bulk Learnset Editor";
            UpdateWindowTitle();

            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            var idColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PokemonID",
                HeaderText = "ID",
                ReadOnly = true,
                Width = 50
            };

            var nameColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PokemonName",
                HeaderText = "Pokemon",
                ReadOnly = true,
                Width = 150
            };

            var levelColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Level",
                HeaderText = "Level",
                Width = 60
            };

            var moveColumn = new DataGridViewComboBoxColumn
            {
                DataPropertyName = "MoveID",
                HeaderText = "Move",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                Width = 200,
                ValueType = typeof(int)
            };

            dataGridView.Columns.AddRange(new DataGridViewColumn[] { idColumn, nameColumn, levelColumn, moveColumn });

            dataGridView.DataSource = learnsetData;

            var moveItems = moveNames.Select((name, index) => new { Index = index, Name = name })
                        .ToArray();
            moveColumn.DataSource = moveItems;
            moveColumn.DisplayMember = "Name";
            moveColumn.ValueMember = "Index";

            contextMenu = new ContextMenuStrip();
            var ctxCopyLearnset = new ToolStripMenuItem("Copy Learnset from this Pokemon");
            ctxCopyLearnset.Click += (s, e) => CopyLearnsetFromContext();
            contextMenu.Items.Add(ctxCopyLearnset);
            dataGridView.ContextMenuStrip = contextMenu;

            toolStrip = new ToolStrip { Dock = DockStyle.Top };

            var btnSave = new ToolStripButton("Save All");
            btnSave.Click += (s, e) => SaveAllChanges();

            var btnAddMove = new ToolStripButton("Add Move");
            btnAddMove.Click += (s, e) => AddMoveToSelectedPokemon();

            var btnDelete = new ToolStripButton("Delete Selected");
            btnDelete.Click += (s, e) => DeleteSelectedMoves();

            var btnSort = new ToolStripButton("Sort Learnsets");
            btnSort.Click += (s, e) => SortAllLearnsets();

            var btnBulkOps = new ToolStripDropDownButton("Bulk Operations");

            var btnCopyLearnset = new ToolStripMenuItem("Copy Learnset to Other Pokemon...");
            btnCopyLearnset.Click += (s, e) => CopyLearnsetToOthers();

            var btnRemoveMoveGlobally = new ToolStripMenuItem("Remove Move from All Learnsets...");
            btnRemoveMoveGlobally.Click += (s, e) => RemoveMoveFromAllLearnsets();

            var btnLevelAdjust = new ToolStripMenuItem("Adjust Levels for Selected...");
            btnLevelAdjust.Click += (s, e) => AdjustLevelsForSelected();

            var btnReplaceMove = new ToolStripMenuItem("Replace Move in All Learnsets...");
            btnReplaceMove.Click += (s, e) => ReplaceMoveGlobally();

            btnBulkOps.DropDownItems.AddRange(new ToolStripItem[] {
                btnCopyLearnset,
                btnRemoveMoveGlobally,
                btnLevelAdjust,
                btnReplaceMove
            });

            var sep = new ToolStripSeparator();

            var lblFilter = new ToolStripLabel("Filter:");
            var txtFilter = new ToolStripTextBox();
            txtFilter.TextChanged += (s, e) => FilterData(txtFilter.Text);

            toolStrip.Items.AddRange(new ToolStripItem[] {
                btnSave, btnAddMove, btnDelete, btnSort, btnBulkOps, sep, lblFilter, txtFilter
            });

            statusStrip = new StatusStrip { Dock = DockStyle.Bottom };
            var statusLabel = new ToolStripStatusLabel();
            statusStrip.Items.Add(statusLabel);

            this.Controls.AddRange(new Control[] { dataGridView, toolStrip, statusStrip });

            dataGridView.CellValueChanged += DataGridView_CellValueChanged;
            dataGridView.DataError += DataGridView_DataError;
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
            dataGridView.MouseClick += DataGridView_MouseClick;
            dataGridView.UserAddedRow += DataGridView_UserAddedRow;
            dataGridView.UserDeletedRow += DataGridView_UserDeletedRow;

            UpdateStatus();

            // Probably should move all this to winforms designer later
        }

        #region Dirty Tracking Methods
        private void SetDirty()
        {
            if (!isDirty)
            {
                isDirty = true;
                changesSaved = false;
                UpdateWindowTitle();
            }
        }

        private void SetClean()
        {
            if (isDirty)
            {
                isDirty = false;
                changesSaved = true;
                UpdateWindowTitle();
            }
        }

        private void UpdateWindowTitle()
        {
            string baseTitle = "Bulk Learnset Editor";
            this.Text = isDirty ? $"{baseTitle} *" : baseTitle;
        }
        #endregion

        #region Event Handlers
        private void DataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = dataGridView.HitTest(e.X, e.Y);
                if (hitTest.RowIndex >= 0 && hitTest.RowIndex < learnsetData.Count)
                {
                    dataGridView.ClearSelection();
                    dataGridView.Rows[hitTest.RowIndex].Selected = true;
                }
            }
        }

        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= learnsetData.Count) return;

            var entry = learnsetData[e.RowIndex];

            // Validate level since there is no level 0 thing in gen4 afaik
            if (entry.Level < 1) entry.Level = 1;
            if (entry.Level > 100) entry.Level = 100;

            if (e.ColumnIndex == 3) // MoveID column, maybe can be set as a constant
            {
                entry.MoveName = moveNames[entry.MoveID];
            }

            dataGridView.InvalidateRow(e.RowIndex);
            UpdateStatus();
            SetDirty();
        }

        private void DataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            SetDirty();
        }

        private void DataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            SetDirty();
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"Invalid value: {e.Exception.Message}", "Data Error",
                           MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.ThrowException = false;
        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateStatus();
        }
        #endregion

        #region Bulk Operations
        private void AddMoveToSelectedPokemon()
        {
            var selectedPokemon = GetSelectedPokemonIds();
            if (selectedPokemon.Count == 0)
            {
                MessageBox.Show("Please select at least one Pokemon row.", "No Selection",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var addForm = new AddMoveForm(moveNames))
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    foreach (var pokemonId in selectedPokemon)
                    {
                        learnsetData.Add(new LearnsetEntry
                        {
                            PokemonID = pokemonId,
                            PokemonName = pokemonNames[pokemonId],
                            Level = addForm.SelectedLevel,
                            MoveID = addForm.SelectedMoveId,
                            MoveName = moveNames[addForm.SelectedMoveId]
                        });
                    }
                    UpdateStatus();
                    SortAllLearnsets();
                    SetDirty();
                }
            }
        }

        private void DeleteSelectedMoves()
        {
            if (dataGridView.SelectedRows.Count == 0) return;

            var result = MessageBox.Show($"Delete {dataGridView.SelectedRows.Count} selected moves?",
                                       "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dataGridView.SelectedRows)
                {
                    if (!row.IsNewRow)
                        learnsetData.RemoveAt(row.Index);
                }
                UpdateStatus();
                SetDirty();
            }
        }

        private void SortAllLearnsets()
        {
            // Group by Pokemon and sort each learnset by level, maintaining Pokemon order
            var grouped = learnsetData
                .GroupBy(x => x.PokemonID)
                .OrderBy(g => g.Key) // Sort by Pokemon ID to maintain order
                .ToList();

            learnsetData.Clear();

            foreach (var group in grouped)
            {
                var sorted = group.OrderBy(x => x.Level);
                foreach (var entry in sorted)
                {
                    learnsetData.Add(entry);
                }
            }

            UpdateStatus();
            SetDirty();
        }

        private void FilterData(string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
            {
                dataGridView.DataSource = learnsetData;
            }
            else
            {
                var filtered = new BindingList<LearnsetEntry>(
                    learnsetData.Where(x =>
                        x.PokemonName.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        x.MoveName.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0
                    ).ToList()
                );
                dataGridView.DataSource = filtered;
            }
            UpdateStatus();
        }

        private void SaveAllChanges()
        {
            try
            {
                // Group by Pokemon ID and save each learnset
                var groupedData = learnsetData.GroupBy(x => x.PokemonID);

                foreach (var group in groupedData)
                {
                    var learnset = new LearnsetData(group.Key);
                    learnset.list.Clear();

                    foreach (var entry in group.OrderBy(x => x.Level))
                    {
                        learnset.list.Add(((byte level, ushort move))(entry.Level, entry.MoveID));
                    }
                    learnset.SaveToFileDefaultDir(group.Key, false);
                }

                SetClean();
                UpdateStatus("All changes saved successfully!");
                MessageBox.Show("All learnset changes have been saved.", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Save Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopyLearnsetToOthers()
        {
            var sourcePokemon = GetSingleSelectedPokemonId();
            if (sourcePokemon == -1)
            {
                MessageBox.Show("Please select exactly one Pokemon row to copy FROM.", "Selection Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new SelectPokemonForm(pokemonNames, "Select Pokemon to copy learnset TO:"))
            {
                if (form.ShowDialog() == DialogResult.OK && form.SelectedPokemonIds.Any())
                {
                    var sourceMoves = learnsetData.Where(x => x.PokemonID == sourcePokemon).ToList();

                    // Remove existing moves from target Pokemon
                    foreach (var targetId in form.SelectedPokemonIds)
                    {
                        var existingMoves = learnsetData.Where(x => x.PokemonID == targetId).ToList();
                        foreach (var move in existingMoves)
                        {
                            learnsetData.Remove(move);
                        }

                        // Add source moves to target Pokemon
                        foreach (var sourceMove in sourceMoves)
                        {
                            learnsetData.Add(new LearnsetEntry
                            {
                                PokemonID = targetId,
                                PokemonName = pokemonNames[targetId],
                                Level = sourceMove.Level,
                                MoveID = sourceMove.MoveID,
                                MoveName = sourceMove.MoveName
                            });
                        }
                    }

                    // Re-sort the entire list to maintain Pokemon ID order
                    var sortedEntries = learnsetData
                        .OrderBy(x => x.PokemonID)
                        .ThenBy(x => x.Level)
                        .ToList();

                    learnsetData.Clear();
                    foreach (var entry in sortedEntries)
                    {
                        learnsetData.Add(entry);
                    }

                    UpdateStatus($"Copied learnset from {pokemonNames[sourcePokemon]} to {form.SelectedPokemonIds.Count} Pokemon.");
                    SetDirty();
                }
            }
        }

        private void CopyLearnsetFromContext()
        {
            CopyLearnsetToOthers();
        }

        private void RemoveMoveFromAllLearnsets()
        {
            using (var form = new SelectMoveForm(moveNames, "Select move to remove from ALL learnsets:"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var moveId = form.SelectedMoveId;
                    var moveName = moveNames[moveId];

                    var result = MessageBox.Show($"This will remove {moveName} from EVERY Pokemon's learnset. Continue?",
                                               "Confirm Global Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        var movesToRemove = learnsetData.Where(x => x.MoveID == moveId).ToList();
                        foreach (var move in movesToRemove)
                        {
                            learnsetData.Remove(move);
                        }

                        UpdateStatus($"Removed {moveName} from {movesToRemove.Count} learnsets.");
                        SetDirty();
                    }
                }
            }
        }

        private void AdjustLevelsForSelected()
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select some moves to adjust levels.", "No Selection",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new AdjustLevelsForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var adjustment = form.LevelAdjustment;
                    var operation = form.AdjustmentOperation;

                    foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    {
                        if (row.IsNewRow) continue;

                        var entry = learnsetData[row.Index];
                        switch (operation)
                        {
                            case LevelOperation.Add:
                                entry.Level = Math.Max(1, Math.Min(100, entry.Level + adjustment));
                                break;
                            case LevelOperation.Subtract:
                                entry.Level = Math.Max(1, Math.Min(100, entry.Level - adjustment));
                                break;
                            case LevelOperation.Set:
                                entry.Level = Math.Max(1, Math.Min(100, adjustment));
                                break;
                        }
                    }

                    dataGridView.Refresh();
                    UpdateStatus($"Adjusted levels for {dataGridView.SelectedRows.Count} moves.");
                    SetDirty();
                }
            }
        }

        private void ReplaceMoveGlobally()
        {
            using (var form = new ReplaceMoveForm(moveNames))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var oldMoveId = form.OldMoveId;
                    var newMoveId = form.NewMoveId;
                    var oldMoveName = moveNames[oldMoveId];
                    var newMoveName = moveNames[newMoveId];

                    var affectedMoves = learnsetData.Where(x => x.MoveID == oldMoveId).ToList();

                    var result = MessageBox.Show($"Replace {oldMoveName} with {newMoveName} in {affectedMoves.Count} learnsets?",
                                               "Confirm Replacement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        foreach (var move in affectedMoves)
                        {
                            move.MoveID = newMoveId;
                            move.MoveName = newMoveName;
                        }

                        dataGridView.Refresh();
                        UpdateStatus($"Replaced {oldMoveName} with {newMoveName} in {affectedMoves.Count} learnsets.");
                        SetDirty();
                    }
                }
            }
        }
        #endregion

        #region Helper Methods
        private List<int> GetSelectedPokemonIds()
        {
            return dataGridView.SelectedRows
                .OfType<DataGridViewRow>()
                .Where(row => !row.IsNewRow)
                .Select(row => learnsetData[row.Index].PokemonID)
                .Distinct()
                .ToList();
        }

        private int GetSingleSelectedPokemonId()
        {
            var selectedIds = GetSelectedPokemonIds();
            return selectedIds.Count == 1 ? selectedIds[0] : -1;
        }

        private void UpdateStatus(string message = null)
        {
            if (statusStrip.Items.Count == 0) return;

            if (message != null)
            {
                statusStrip.Items[0].Text = message;
                return;
            }

            var selectedCount = dataGridView.SelectedRows.Count;
            var totalCount = learnsetData.Count;
            var pokemonCount = learnsetData.Select(x => x.PokemonID).Distinct().Count();

            statusStrip.Items[0].Text =
                $"{totalCount} moves across {pokemonCount} Pokemon. " +
                $"{(selectedCount > 0 ? $"{selectedCount} selected." : "")}" +
                $"{(isDirty ? " [Unsaved Changes]" : "")}";
        }
        #endregion
    }

    #region Supporting Enums and Classes
    public class LearnsetEntry
    {
        public int PokemonID { get; set; }
        public string PokemonName { get; set; }
        public int Level { get; set; }
        public int MoveID { get; set; }
        public string MoveName { get; set; }
    }

    public class AddMoveForm : Form
    {
        private NumericUpDown numLevel;
        private ComboBox cmbMove;
        private Button btnOK;
        private Button btnCancel;
        private string[] moveNames;

        public int SelectedLevel => (int)numLevel.Value;
        public int SelectedMoveId => cmbMove.SelectedIndex;

        public AddMoveForm(string[] moves)
        {
            moveNames = moves;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(300, 150);
            this.Text = "Add Move";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3
            };
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Level
            tableLayout.Controls.Add(new Label { Text = "Level:", TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            numLevel = new NumericUpDown { Minimum = 1, Maximum = 100, Value = 1, Dock = DockStyle.Fill };
            tableLayout.Controls.Add(numLevel, 1, 0);

            // Move
            tableLayout.Controls.Add(new Label { Text = "Move:", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            cmbMove = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMove.Items.AddRange(moveNames.Select((name, idx) => $"{idx:000} - {name}").ToArray());
            if (cmbMove.Items.Count > 0) cmbMove.SelectedIndex = 0;
            tableLayout.Controls.Add(cmbMove, 1, 1);

            // Buttons
            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel };
            btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK };
            buttonPanel.Controls.AddRange(new Control[] { btnOK, btnCancel });
            tableLayout.Controls.Add(buttonPanel, 0, 2);
            tableLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(tableLayout);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }

    public class SelectPokemonForm : Form
    {
        private CheckedListBox checkedListBox;
        private Button btnOK;
        private Button btnCancel;

        public List<int> SelectedPokemonIds =>
            checkedListBox.CheckedIndices.Cast<int>().ToList();

        public SelectPokemonForm(string[] pokemonNames, string title)
        {
            InitializeComponent(pokemonNames, title);
        }

        private void InitializeComponent(string[] pokemonNames, string title)
        {
            this.Size = new Size(300, 500);
            this.Text = title;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            checkedListBox = new CheckedListBox
            {
                Dock = DockStyle.Fill
            };

            foreach (var item in pokemonNames.Select((name, idx) => $"{idx:000} - {name}"))
            {
                checkedListBox.Items.Add(item);
            }

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft
            };
            btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel };
            btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK };
            buttonPanel.Controls.AddRange(new Control[] { btnOK, btnCancel });

            tableLayout.Controls.Add(checkedListBox, 0, 0);
            tableLayout.Controls.Add(buttonPanel, 0, 1);

            this.Controls.Add(tableLayout);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }

    public class SelectMoveForm : Form
    {
        private ComboBox cmbMove;
        private Button btnOK;
        private Button btnCancel;

        public int SelectedMoveId => cmbMove.SelectedIndex;

        public SelectMoveForm(string[] moveNames, string title)
        {
            InitializeComponent(moveNames, title);
        }

        private void InitializeComponent(string[] moveNames, string title)
        {
            this.Size = new Size(300, 150);
            this.Text = title;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };

            tableLayout.Controls.Add(new Label { Text = "Move:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            cmbMove = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMove.Items.AddRange(moveNames.Select((name, idx) => $"{idx:000} - {name}").ToArray());
            if (cmbMove.Items.Count > 0) cmbMove.SelectedIndex = 0;
            tableLayout.Controls.Add(cmbMove, 1, 0);

            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel };
            btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK };
            buttonPanel.Controls.AddRange(new Control[] { btnOK, btnCancel });
            tableLayout.Controls.Add(buttonPanel, 0, 1);
            tableLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(tableLayout);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }

    public class AdjustLevelsForm : Form
    {
        private NumericUpDown numAdjustment;
        private ComboBox cmbOperation;
        private Button btnOK;
        private Button btnCancel;

        public int LevelAdjustment => (int)numAdjustment.Value;
        public LevelOperation AdjustmentOperation => (LevelOperation)cmbOperation.SelectedIndex;

        public AdjustLevelsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(300, 180);
            this.Text = "Adjust Levels";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3
            };

            tableLayout.Controls.Add(new Label { Text = "Operation:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            cmbOperation = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbOperation.Items.AddRange(new string[] { "Add to level", "Subtract from level", "Set level to" });
            cmbOperation.SelectedIndex = 0;
            tableLayout.Controls.Add(cmbOperation, 1, 0);

            tableLayout.Controls.Add(new Label { Text = "Value:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            numAdjustment = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1, Maximum = 100, Value = 1 };
            tableLayout.Controls.Add(numAdjustment, 1, 1);

            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel };
            btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK };
            buttonPanel.Controls.AddRange(new Control[] { btnOK, btnCancel });
            tableLayout.Controls.Add(buttonPanel, 0, 2);
            tableLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(tableLayout);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }

    public class ReplaceMoveForm : Form
    {
        private ComboBox cmbOldMove;
        private ComboBox cmbNewMove;
        private Button btnOK;
        private Button btnCancel;

        public int OldMoveId => cmbOldMove.SelectedIndex;
        public int NewMoveId => cmbNewMove.SelectedIndex;

        public ReplaceMoveForm(string[] moveNames)
        {
            InitializeComponent(moveNames);
        }

        private void InitializeComponent(string[] moveNames)
        {
            this.Size = new Size(300, 180);
            this.Text = "Replace Move Globally";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3
            };

            tableLayout.Controls.Add(new Label { Text = "Replace:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            cmbOldMove = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbOldMove.Items.AddRange(moveNames.Select((name, idx) => $"{idx:000} - {name}").ToArray());
            if (cmbOldMove.Items.Count > 0) cmbOldMove.SelectedIndex = 0;
            tableLayout.Controls.Add(cmbOldMove, 1, 0);

            tableLayout.Controls.Add(new Label { Text = "With:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            cmbNewMove = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbNewMove.Items.AddRange(moveNames.Select((name, idx) => $"{idx:000} - {name}").ToArray());
            if (cmbNewMove.Items.Count > 0) cmbNewMove.SelectedIndex = 0;
            tableLayout.Controls.Add(cmbNewMove, 1, 1);

            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel };
            btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK };
            buttonPanel.Controls.AddRange(new Control[] { btnOK, btnCancel });
            tableLayout.Controls.Add(buttonPanel, 0, 2);
            tableLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(tableLayout);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }

    public enum LevelOperation
    {
        Add,
        Subtract,
        Set
    }

    #endregion
}