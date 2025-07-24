namespace DSPRE.Editors
{
    partial class TextEditor
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.selectedLineMoveUpButton = new System.Windows.Forms.Button();
            this.selectTextFileComboBox = new System.Windows.Forms.ComboBox();
            this.locateCurrentTextArchive = new System.Windows.Forms.Button();
            this.saveTextArchiveButton = new System.Windows.Forms.Button();
            this.importTextFileButton = new System.Windows.Forms.Button();
            this.exportTextFileButton = new System.Windows.Forms.Button();
            this.addTextArchiveButton = new System.Windows.Forms.Button();
            this.removeMessageFileButton = new System.Windows.Forms.Button();
            this.selectedLineMoveDownButton = new System.Windows.Forms.Button();
            this.LineNumbersFormatgroupBox = new System.Windows.Forms.GroupBox();
            this.decimalRadioButton = new System.Windows.Forms.RadioButton();
            this.hexRadiobutton = new System.Windows.Forms.RadioButton();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.label67 = new System.Windows.Forms.Label();
            this.searchAllArchivesCheckBox = new System.Windows.Forms.CheckBox();
            this.caseSensitiveTextReplaceCheckbox = new System.Windows.Forms.CheckBox();
            this.textSearchResultsListBox = new System.Windows.Forms.ListBox();
            this.replaceTextLabel = new System.Windows.Forms.Label();
            this.replaceMessageTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textSearchProgressBar = new System.Windows.Forms.ProgressBar();
            this.caseSensitiveTextSearchCheckbox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.searchMessageTextBox = new System.Windows.Forms.TextBox();
            this.searchMessageButton = new System.Windows.Forms.Button();
            this.replaceMessageButton = new System.Windows.Forms.Button();
            this.textEditorDataGridView = new System.Windows.Forms.DataGridView();
            this.messageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.removeStringButton = new System.Windows.Forms.Button();
            this.addStringButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.LineNumbersFormatgroupBox.SuspendLayout();
            this.groupBox13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEditorDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 12;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.selectedLineMoveUpButton, 8, 1);
            this.tableLayoutPanel1.Controls.Add(this.selectTextFileComboBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.locateCurrentTextArchive, 6, 1);
            this.tableLayoutPanel1.Controls.Add(this.saveTextArchiveButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.importTextFileButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.exportTextFileButton, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.addTextArchiveButton, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.removeMessageFileButton, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.selectedLineMoveDownButton, 9, 1);
            this.tableLayoutPanel1.Controls.Add(this.LineNumbersFormatgroupBox, 11, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1161, 85);
            this.tableLayoutPanel1.TabIndex = 71;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Text Archive";
            // 
            // selectedLineMoveUpButton
            // 
            this.selectedLineMoveUpButton.Image = global::DSPRE.Properties.Resources.arrowup;
            this.selectedLineMoveUpButton.Location = new System.Drawing.Point(681, 16);
            this.selectedLineMoveUpButton.Name = "selectedLineMoveUpButton";
            this.tableLayoutPanel1.SetRowSpan(this.selectedLineMoveUpButton, 2);
            this.selectedLineMoveUpButton.Size = new System.Drawing.Size(42, 40);
            this.selectedLineMoveUpButton.TabIndex = 65;
            this.selectedLineMoveUpButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.selectedLineMoveUpButton.UseVisualStyleBackColor = true;
            this.selectedLineMoveUpButton.Click += new System.EventHandler(this.selectedLineMoveUpButton_Click);
            // 
            // selectTextFileComboBox
            // 
            this.selectTextFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectTextFileComboBox.FormattingEnabled = true;
            this.selectTextFileComboBox.Location = new System.Drawing.Point(3, 16);
            this.selectTextFileComboBox.Name = "selectTextFileComboBox";
            this.selectTextFileComboBox.Size = new System.Drawing.Size(184, 21);
            this.selectTextFileComboBox.TabIndex = 17;
            this.selectTextFileComboBox.SelectedIndexChanged += new System.EventHandler(this.selectTextFileComboBox_SelectedIndexChanged);
            // 
            // locateCurrentTextArchive
            // 
            this.locateCurrentTextArchive.Image = global::DSPRE.Properties.Resources.open_file;
            this.locateCurrentTextArchive.Location = new System.Drawing.Point(613, 16);
            this.locateCurrentTextArchive.Name = "locateCurrentTextArchive";
            this.tableLayoutPanel1.SetRowSpan(this.locateCurrentTextArchive, 2);
            this.locateCurrentTextArchive.Size = new System.Drawing.Size(42, 40);
            this.locateCurrentTextArchive.TabIndex = 64;
            this.locateCurrentTextArchive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.locateCurrentTextArchive.UseVisualStyleBackColor = true;
            this.locateCurrentTextArchive.Click += new System.EventHandler(this.locateCurrentTextArchive_Click);
            // 
            // saveTextArchiveButton
            // 
            this.saveTextArchiveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveTextArchiveButton.Image = global::DSPRE.Properties.Resources.saveButton;
            this.saveTextArchiveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.saveTextArchiveButton.Location = new System.Drawing.Point(3, 43);
            this.saveTextArchiveButton.Name = "saveTextArchiveButton";
            this.saveTextArchiveButton.Size = new System.Drawing.Size(184, 39);
            this.saveTextArchiveButton.TabIndex = 21;
            this.saveTextArchiveButton.Text = "&Save Current Archive";
            this.saveTextArchiveButton.UseVisualStyleBackColor = true;
            this.saveTextArchiveButton.Click += new System.EventHandler(this.saveTextArchiveButton_Click);
            // 
            // importTextFileButton
            // 
            this.importTextFileButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.importTextFileButton.Image = global::DSPRE.Properties.Resources.importArrow;
            this.importTextFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.importTextFileButton.Location = new System.Drawing.Point(193, 16);
            this.importTextFileButton.Name = "importTextFileButton";
            this.tableLayoutPanel1.SetRowSpan(this.importTextFileButton, 2);
            this.importTextFileButton.Size = new System.Drawing.Size(94, 66);
            this.importTextFileButton.TabIndex = 22;
            this.importTextFileButton.Text = "&Replace\r\nCurrent";
            this.importTextFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.importTextFileButton.UseVisualStyleBackColor = true;
            this.importTextFileButton.Click += new System.EventHandler(this.importTextFileButton_Click);
            // 
            // exportTextFileButton
            // 
            this.exportTextFileButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exportTextFileButton.Image = global::DSPRE.Properties.Resources.exportArrow;
            this.exportTextFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.exportTextFileButton.Location = new System.Drawing.Point(293, 16);
            this.exportTextFileButton.Name = "exportTextFileButton";
            this.tableLayoutPanel1.SetRowSpan(this.exportTextFileButton, 2);
            this.exportTextFileButton.Size = new System.Drawing.Size(94, 66);
            this.exportTextFileButton.TabIndex = 23;
            this.exportTextFileButton.Text = "&Export File";
            this.exportTextFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.exportTextFileButton.UseVisualStyleBackColor = true;
            this.exportTextFileButton.Click += new System.EventHandler(this.exportTextFileButton_Click);
            // 
            // addTextArchiveButton
            // 
            this.addTextArchiveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addTextArchiveButton.Image = global::DSPRE.Properties.Resources.addIcon;
            this.addTextArchiveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addTextArchiveButton.Location = new System.Drawing.Point(413, 16);
            this.addTextArchiveButton.Name = "addTextArchiveButton";
            this.tableLayoutPanel1.SetRowSpan(this.addTextArchiveButton, 2);
            this.addTextArchiveButton.Size = new System.Drawing.Size(91, 66);
            this.addTextArchiveButton.TabIndex = 19;
            this.addTextArchiveButton.Text = "Add Text \r\nArchive";
            this.addTextArchiveButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addTextArchiveButton.UseVisualStyleBackColor = true;
            this.addTextArchiveButton.Click += new System.EventHandler(this.addTextArchiveButton_Click);
            // 
            // removeMessageFileButton
            // 
            this.removeMessageFileButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.removeMessageFileButton.Image = global::DSPRE.Properties.Resources.deleteIcon;
            this.removeMessageFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.removeMessageFileButton.Location = new System.Drawing.Point(510, 16);
            this.removeMessageFileButton.Name = "removeMessageFileButton";
            this.tableLayoutPanel1.SetRowSpan(this.removeMessageFileButton, 2);
            this.removeMessageFileButton.Size = new System.Drawing.Size(97, 66);
            this.removeMessageFileButton.TabIndex = 20;
            this.removeMessageFileButton.Text = "Remove \r\nLast Archive";
            this.removeMessageFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.removeMessageFileButton.UseVisualStyleBackColor = true;
            this.removeMessageFileButton.Click += new System.EventHandler(this.removeMessageFileButton_Click);
            // 
            // selectedLineMoveDownButton
            // 
            this.selectedLineMoveDownButton.Image = global::DSPRE.Properties.Resources.arrowdown;
            this.selectedLineMoveDownButton.Location = new System.Drawing.Point(729, 16);
            this.selectedLineMoveDownButton.Name = "selectedLineMoveDownButton";
            this.tableLayoutPanel1.SetRowSpan(this.selectedLineMoveDownButton, 2);
            this.selectedLineMoveDownButton.Size = new System.Drawing.Size(42, 40);
            this.selectedLineMoveDownButton.TabIndex = 66;
            this.selectedLineMoveDownButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.selectedLineMoveDownButton.UseVisualStyleBackColor = true;
            this.selectedLineMoveDownButton.Click += new System.EventHandler(this.selectedLineMoveDownButton_Click);
            // 
            // LineNumbersFormatgroupBox
            // 
            this.LineNumbersFormatgroupBox.Controls.Add(this.decimalRadioButton);
            this.LineNumbersFormatgroupBox.Controls.Add(this.hexRadiobutton);
            this.LineNumbersFormatgroupBox.Location = new System.Drawing.Point(797, 16);
            this.LineNumbersFormatgroupBox.Name = "LineNumbersFormatgroupBox";
            this.tableLayoutPanel1.SetRowSpan(this.LineNumbersFormatgroupBox, 2);
            this.LineNumbersFormatgroupBox.Size = new System.Drawing.Size(134, 35);
            this.LineNumbersFormatgroupBox.TabIndex = 34;
            this.LineNumbersFormatgroupBox.TabStop = false;
            this.LineNumbersFormatgroupBox.Text = "Line Number Format";
            // 
            // decimalRadioButton
            // 
            this.decimalRadioButton.AutoSize = true;
            this.decimalRadioButton.Checked = true;
            this.decimalRadioButton.Location = new System.Drawing.Point(68, 14);
            this.decimalRadioButton.Name = "decimalRadioButton";
            this.decimalRadioButton.Size = new System.Drawing.Size(63, 17);
            this.decimalRadioButton.TabIndex = 35;
            this.decimalRadioButton.TabStop = true;
            this.decimalRadioButton.Text = "Decimal";
            this.decimalRadioButton.UseVisualStyleBackColor = true;
            // 
            // hexRadiobutton
            // 
            this.hexRadiobutton.AutoSize = true;
            this.hexRadiobutton.Location = new System.Drawing.Point(6, 14);
            this.hexRadiobutton.Name = "hexRadiobutton";
            this.hexRadiobutton.Size = new System.Drawing.Size(44, 17);
            this.hexRadiobutton.TabIndex = 34;
            this.hexRadiobutton.Text = "Hex";
            this.hexRadiobutton.UseVisualStyleBackColor = true;
            this.hexRadiobutton.CheckedChanged += new System.EventHandler(this.hexRadiobutton_CheckedChanged);
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.label67);
            this.groupBox13.Controls.Add(this.searchAllArchivesCheckBox);
            this.groupBox13.Controls.Add(this.caseSensitiveTextReplaceCheckbox);
            this.groupBox13.Controls.Add(this.textSearchResultsListBox);
            this.groupBox13.Controls.Add(this.replaceTextLabel);
            this.groupBox13.Controls.Add(this.replaceMessageTextBox);
            this.groupBox13.Controls.Add(this.label8);
            this.groupBox13.Controls.Add(this.textSearchProgressBar);
            this.groupBox13.Controls.Add(this.caseSensitiveTextSearchCheckbox);
            this.groupBox13.Controls.Add(this.label7);
            this.groupBox13.Controls.Add(this.searchMessageTextBox);
            this.groupBox13.Controls.Add(this.searchMessageButton);
            this.groupBox13.Controls.Add(this.replaceMessageButton);
            this.groupBox13.Location = new System.Drawing.Point(891, 94);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(282, 491);
            this.groupBox13.TabIndex = 70;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Search / Replace";
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Enabled = false;
            this.label67.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label67.Location = new System.Drawing.Point(190, 133);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(74, 13);
            this.label67.TabIndex = 41;
            this.label67.Text = "[Coming soon]";
            // 
            // searchAllArchivesCheckBox
            // 
            this.searchAllArchivesCheckBox.AutoSize = true;
            this.searchAllArchivesCheckBox.Checked = true;
            this.searchAllArchivesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.searchAllArchivesCheckBox.Location = new System.Drawing.Point(190, 22);
            this.searchAllArchivesCheckBox.Name = "searchAllArchivesCheckBox";
            this.searchAllArchivesCheckBox.Size = new System.Drawing.Size(81, 17);
            this.searchAllArchivesCheckBox.TabIndex = 40;
            this.searchAllArchivesCheckBox.Text = "All Archives";
            this.searchAllArchivesCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.searchAllArchivesCheckBox.UseVisualStyleBackColor = true;
            // 
            // caseSensitiveTextReplaceCheckbox
            // 
            this.caseSensitiveTextReplaceCheckbox.AutoSize = true;
            this.caseSensitiveTextReplaceCheckbox.Enabled = false;
            this.caseSensitiveTextReplaceCheckbox.Location = new System.Drawing.Point(190, 115);
            this.caseSensitiveTextReplaceCheckbox.Name = "caseSensitiveTextReplaceCheckbox";
            this.caseSensitiveTextReplaceCheckbox.Size = new System.Drawing.Size(77, 17);
            this.caseSensitiveTextReplaceCheckbox.TabIndex = 39;
            this.caseSensitiveTextReplaceCheckbox.Text = "Copy Case";
            this.caseSensitiveTextReplaceCheckbox.UseVisualStyleBackColor = true;
            // 
            // textSearchResultsListBox
            // 
            this.textSearchResultsListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textSearchResultsListBox.ItemHeight = 15;
            this.textSearchResultsListBox.Location = new System.Drawing.Point(9, 182);
            this.textSearchResultsListBox.Name = "textSearchResultsListBox";
            this.textSearchResultsListBox.Size = new System.Drawing.Size(267, 244);
            this.textSearchResultsListBox.TabIndex = 38;
            this.textSearchResultsListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textSearchResultsListBox_KeyDown);
            // 
            // replaceTextLabel
            // 
            this.replaceTextLabel.AutoSize = true;
            this.replaceTextLabel.Location = new System.Drawing.Point(6, 87);
            this.replaceTextLabel.Name = "replaceTextLabel";
            this.replaceTextLabel.Size = new System.Drawing.Size(93, 13);
            this.replaceTextLabel.TabIndex = 37;
            this.replaceTextLabel.Text = "Replacement text:";
            // 
            // replaceMessageTextBox
            // 
            this.replaceMessageTextBox.Location = new System.Drawing.Point(8, 103);
            this.replaceMessageTextBox.MaxLength = 100;
            this.replaceMessageTextBox.Name = "replaceMessageTextBox";
            this.replaceMessageTextBox.Size = new System.Drawing.Size(173, 20);
            this.replaceMessageTextBox.TabIndex = 36;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 439);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 13);
            this.label8.TabIndex = 35;
            this.label8.Text = "Progress";
            // 
            // textSearchProgressBar
            // 
            this.textSearchProgressBar.Location = new System.Drawing.Point(9, 455);
            this.textSearchProgressBar.Name = "textSearchProgressBar";
            this.textSearchProgressBar.Size = new System.Drawing.Size(267, 27);
            this.textSearchProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.textSearchProgressBar.TabIndex = 34;
            // 
            // caseSensitiveTextSearchCheckbox
            // 
            this.caseSensitiveTextSearchCheckbox.AutoSize = true;
            this.caseSensitiveTextSearchCheckbox.Location = new System.Drawing.Point(190, 50);
            this.caseSensitiveTextSearchCheckbox.Name = "caseSensitiveTextSearchCheckbox";
            this.caseSensitiveTextSearchCheckbox.Size = new System.Drawing.Size(83, 17);
            this.caseSensitiveTextSearchCheckbox.TabIndex = 33;
            this.caseSensitiveTextSearchCheckbox.Text = "Match Case";
            this.caseSensitiveTextSearchCheckbox.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 166);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "Results";
            // 
            // searchMessageTextBox
            // 
            this.searchMessageTextBox.Location = new System.Drawing.Point(6, 20);
            this.searchMessageTextBox.MaxLength = 100;
            this.searchMessageTextBox.Name = "searchMessageTextBox";
            this.searchMessageTextBox.Size = new System.Drawing.Size(175, 20);
            this.searchMessageTextBox.TabIndex = 27;
            this.searchMessageTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchMessageTextBox_KeyDown);
            // 
            // searchMessageButton
            // 
            this.searchMessageButton.Image = global::DSPRE.Properties.Resources.SearchMiniIcon;
            this.searchMessageButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.searchMessageButton.Location = new System.Drawing.Point(6, 44);
            this.searchMessageButton.Name = "searchMessageButton";
            this.searchMessageButton.Size = new System.Drawing.Size(175, 27);
            this.searchMessageButton.TabIndex = 30;
            this.searchMessageButton.Text = "Search";
            this.searchMessageButton.UseVisualStyleBackColor = true;
            this.searchMessageButton.Click += new System.EventHandler(this.searchMessageButton_Click);
            // 
            // replaceMessageButton
            // 
            this.replaceMessageButton.Location = new System.Drawing.Point(8, 127);
            this.replaceMessageButton.Name = "replaceMessageButton";
            this.replaceMessageButton.Size = new System.Drawing.Size(173, 27);
            this.replaceMessageButton.TabIndex = 30;
            this.replaceMessageButton.Text = "Search and Replace All";
            this.replaceMessageButton.UseVisualStyleBackColor = true;
            this.replaceMessageButton.Click += new System.EventHandler(this.replaceMessageButton_Click);
            // 
            // textEditorDataGridView
            // 
            this.textEditorDataGridView.AllowDrop = true;
            this.textEditorDataGridView.AllowUserToAddRows = false;
            this.textEditorDataGridView.AllowUserToDeleteRows = false;
            this.textEditorDataGridView.AllowUserToResizeColumns = false;
            this.textEditorDataGridView.AllowUserToResizeRows = false;
            this.textEditorDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.textEditorDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.textEditorDataGridView.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textEditorDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.textEditorDataGridView.ColumnHeadersHeight = 29;
            this.textEditorDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.textEditorDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.messageColumn});
            this.textEditorDataGridView.Location = new System.Drawing.Point(12, 94);
            this.textEditorDataGridView.Name = "textEditorDataGridView";
            this.textEditorDataGridView.RowHeadersWidth = 68;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textEditorDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.textEditorDataGridView.Size = new System.Drawing.Size(873, 491);
            this.textEditorDataGridView.TabIndex = 67;
            this.textEditorDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.textEditorDataGridView_CellValueChanged);
            // 
            // messageColumn
            // 
            this.messageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.messageColumn.HeaderText = "Message";
            this.messageColumn.MinimumWidth = 6;
            this.messageColumn.Name = "messageColumn";
            this.messageColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.messageColumn.Width = 56;
            // 
            // removeStringButton
            // 
            this.removeStringButton.Image = global::DSPRE.Properties.Resources.deleteIcon;
            this.removeStringButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.removeStringButton.Location = new System.Drawing.Point(113, 588);
            this.removeStringButton.Name = "removeStringButton";
            this.removeStringButton.Size = new System.Drawing.Size(124, 34);
            this.removeStringButton.TabIndex = 69;
            this.removeStringButton.Text = "Remove Last Line";
            this.removeStringButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.removeStringButton.UseVisualStyleBackColor = true;
            this.removeStringButton.Click += new System.EventHandler(this.removeStringButton_Click);
            // 
            // addStringButton
            // 
            this.addStringButton.Image = global::DSPRE.Properties.Resources.addIcon;
            this.addStringButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addStringButton.Location = new System.Drawing.Point(12, 588);
            this.addStringButton.Name = "addStringButton";
            this.addStringButton.Size = new System.Drawing.Size(95, 34);
            this.addStringButton.TabIndex = 68;
            this.addStringButton.Text = "&Append Line";
            this.addStringButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addStringButton.UseVisualStyleBackColor = true;
            this.addStringButton.Click += new System.EventHandler(this.addStringButton_Click);
            // 
            // TextEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.groupBox13);
            this.Controls.Add(this.textEditorDataGridView);
            this.Controls.Add(this.removeStringButton);
            this.Controls.Add(this.addStringButton);
            this.Name = "TextEditor";
            this.Size = new System.Drawing.Size(1193, 646);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.LineNumbersFormatgroupBox.ResumeLayout(false);
            this.LineNumbersFormatgroupBox.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEditorDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button selectedLineMoveUpButton;
        private System.Windows.Forms.ComboBox selectTextFileComboBox;
        private System.Windows.Forms.Button locateCurrentTextArchive;
        private System.Windows.Forms.Button saveTextArchiveButton;
        private System.Windows.Forms.Button importTextFileButton;
        private System.Windows.Forms.Button exportTextFileButton;
        private System.Windows.Forms.Button addTextArchiveButton;
        private System.Windows.Forms.Button removeMessageFileButton;
        private System.Windows.Forms.Button selectedLineMoveDownButton;
        private System.Windows.Forms.GroupBox LineNumbersFormatgroupBox;
        private System.Windows.Forms.RadioButton decimalRadioButton;
        private System.Windows.Forms.RadioButton hexRadiobutton;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.CheckBox searchAllArchivesCheckBox;
        private System.Windows.Forms.CheckBox caseSensitiveTextReplaceCheckbox;
        private System.Windows.Forms.ListBox textSearchResultsListBox;
        private System.Windows.Forms.Label replaceTextLabel;
        private System.Windows.Forms.TextBox replaceMessageTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ProgressBar textSearchProgressBar;
        private System.Windows.Forms.CheckBox caseSensitiveTextSearchCheckbox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox searchMessageTextBox;
        private System.Windows.Forms.Button searchMessageButton;
        private System.Windows.Forms.Button replaceMessageButton;
        private System.Windows.Forms.DataGridView textEditorDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageColumn;
        private System.Windows.Forms.Button removeStringButton;
        private System.Windows.Forms.Button addStringButton;
    }
}
