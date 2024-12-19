namespace DSPRE {
    partial class LearnsetEditor {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LearnsetEditor));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.saveDataButton = new System.Windows.Forms.Button();
            this.pokemonPictureBox = new System.Windows.Forms.PictureBox();
            this.monNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.movesListBox = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.descriptorLabel = new System.Windows.Forms.Label();
            this.addMoveButton = new System.Windows.Forms.Button();
            this.deleteMoveButton = new System.Windows.Forms.Button();
            this.editMoveButton = new System.Windows.Forms.Button();
            this.levelNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.moveInputComboBox = new DSPRE.InputComboBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.pokemonNameInputComboBox = new DSPRE.InputComboBox();
            this.entryCountLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pokemonPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.monNumberNumericUpDown)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.levelNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.41322F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60.49587F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.97891F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.43285F));
            this.tableLayoutPanel1.Controls.Add(this.saveDataButton, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.pokemonPictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.monNumberNumericUpDown, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pokemonNameInputComboBox, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.58022F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.41978F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(491, 449);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // saveDataButton
            // 
            this.saveDataButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveDataButton.Image = ((System.Drawing.Image)(resources.GetObject("saveDataButton.Image")));
            this.saveDataButton.Location = new System.Drawing.Point(441, 3);
            this.saveDataButton.Name = "saveDataButton";
            this.saveDataButton.Size = new System.Drawing.Size(47, 45);
            this.saveDataButton.TabIndex = 30;
            this.saveDataButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveDataButton.UseVisualStyleBackColor = true;
            this.saveDataButton.Click += new System.EventHandler(this.saveDataButton_Click);
            // 
            // pokemonPictureBox
            // 
            this.pokemonPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pokemonPictureBox.Location = new System.Drawing.Point(3, 3);
            this.pokemonPictureBox.Name = "pokemonPictureBox";
            this.pokemonPictureBox.Size = new System.Drawing.Size(44, 45);
            this.pokemonPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pokemonPictureBox.TabIndex = 12;
            this.pokemonPictureBox.TabStop = false;
            // 
            // monNumberNumericUpDown
            // 
            this.monNumberNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.monNumberNumericUpDown.Location = new System.Drawing.Point(349, 15);
            this.monNumberNumericUpDown.Name = "monNumberNumericUpDown";
            this.monNumberNumericUpDown.Size = new System.Drawing.Size(86, 20);
            this.monNumberNumericUpDown.TabIndex = 16;
            this.monNumberNumericUpDown.ValueChanged += new System.EventHandler(this.monNumberNumericUpDown_ValueChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 4);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.303955F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.303955F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox3, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 54);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 382F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(485, 392);
            this.tableLayoutPanel2.TabIndex = 17;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.movesListBox);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(8, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(231, 376);
            this.groupBox2.TabIndex = 34;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "View";
            // 
            // movesListBox
            // 
            this.movesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.movesListBox.FormattingEnabled = true;
            this.movesListBox.Location = new System.Drawing.Point(3, 16);
            this.movesListBox.Margin = new System.Windows.Forms.Padding(2);
            this.movesListBox.Name = "movesListBox";
            this.movesListBox.Size = new System.Drawing.Size(225, 357);
            this.movesListBox.TabIndex = 0;
            this.movesListBox.SelectedIndexChanged += new System.EventHandler(this.movesListBox_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tableLayoutPanel3);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(245, 8);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(232, 376);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Edit";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Controls.Add(this.entryCountLabel, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.descriptorLabel, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.addMoveButton, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.deleteMoveButton, 2, 4);
            this.tableLayoutPanel3.Controls.Add(this.editMoveButton, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.levelNumericUpDown, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.moveInputComboBox, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.statusLabel, 0, 3);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34.50088F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65.49912F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 144F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(226, 357);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // descriptorLabel
            // 
            this.descriptorLabel.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.descriptorLabel, 3);
            this.descriptorLabel.Location = new System.Drawing.Point(5, 43);
            this.descriptorLabel.Name = "descriptorLabel";
            this.descriptorLabel.Padding = new System.Windows.Forms.Padding(1, 5, 1, 5);
            this.descriptorLabel.Size = new System.Drawing.Size(57, 23);
            this.descriptorLabel.TabIndex = 8;
            this.descriptorLabel.Text = "Descriptor";
            // 
            // addMoveButton
            // 
            this.addMoveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addMoveButton.Enabled = false;
            this.addMoveButton.Image = global::DSPRE.Properties.Resources.addIcon;
            this.addMoveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addMoveButton.Location = new System.Drawing.Point(4, 286);
            this.addMoveButton.Margin = new System.Windows.Forms.Padding(2);
            this.addMoveButton.Name = "addMoveButton";
            this.addMoveButton.Size = new System.Drawing.Size(70, 40);
            this.addMoveButton.TabIndex = 1;
            this.addMoveButton.Text = "Add";
            this.addMoveButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addMoveButton.UseVisualStyleBackColor = true;
            this.addMoveButton.Click += new System.EventHandler(this.addMoveButton_Click);
            // 
            // deleteMoveButton
            // 
            this.deleteMoveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deleteMoveButton.Enabled = false;
            this.deleteMoveButton.Image = global::DSPRE.Properties.Resources.deleteIcon;
            this.deleteMoveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.deleteMoveButton.Location = new System.Drawing.Point(152, 286);
            this.deleteMoveButton.Margin = new System.Windows.Forms.Padding(2);
            this.deleteMoveButton.Name = "deleteMoveButton";
            this.deleteMoveButton.Size = new System.Drawing.Size(70, 40);
            this.deleteMoveButton.TabIndex = 2;
            this.deleteMoveButton.Text = "Delete";
            this.deleteMoveButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.deleteMoveButton.UseVisualStyleBackColor = true;
            this.deleteMoveButton.Click += new System.EventHandler(this.deleteMoveButton_Click);
            // 
            // editMoveButton
            // 
            this.editMoveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editMoveButton.Enabled = false;
            this.editMoveButton.Image = global::DSPRE.Properties.Resources.RenameIcon;
            this.editMoveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.editMoveButton.Location = new System.Drawing.Point(78, 286);
            this.editMoveButton.Margin = new System.Windows.Forms.Padding(2);
            this.editMoveButton.Name = "editMoveButton";
            this.editMoveButton.Size = new System.Drawing.Size(70, 40);
            this.editMoveButton.TabIndex = 3;
            this.editMoveButton.Text = "Edit";
            this.editMoveButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.editMoveButton.UseVisualStyleBackColor = true;
            this.editMoveButton.Click += new System.EventHandler(this.editMoveButton_Click);
            // 
            // levelNumericUpDown
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.levelNumericUpDown, 2);
            this.levelNumericUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.levelNumericUpDown.Location = new System.Drawing.Point(78, 122);
            this.levelNumericUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.levelNumericUpDown.Name = "levelNumericUpDown";
            this.levelNumericUpDown.Size = new System.Drawing.Size(144, 20);
            this.levelNumericUpDown.TabIndex = 4;
            this.levelNumericUpDown.ValueChanged += new System.EventHandler(this.levelNumericUpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 123);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Level:";
            // 
            // moveInputComboBox
            // 
            this.moveInputComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.moveInputComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.moveInputComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.tableLayoutPanel3.SetColumnSpan(this.moveInputComboBox, 3);
            this.moveInputComboBox.FormattingEnabled = true;
            this.moveInputComboBox.Location = new System.Drawing.Point(4, 12);
            this.moveInputComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.moveInputComboBox.Name = "moveInputComboBox";
            this.moveInputComboBox.Size = new System.Drawing.Size(218, 21);
            this.moveInputComboBox.TabIndex = 6;
            this.moveInputComboBox.SelectedIndexChanged += new System.EventHandler(this.moveInputComboBox_SelectedIndexChanged);
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusLabel.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.statusLabel, 3);
            this.statusLabel.Location = new System.Drawing.Point(5, 261);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Padding = new System.Windows.Forms.Padding(1, 5, 1, 5);
            this.statusLabel.Size = new System.Drawing.Size(39, 23);
            this.statusLabel.TabIndex = 7;
            this.statusLabel.Text = "Status";
            // 
            // pokemonNameInputComboBox
            // 
            this.pokemonNameInputComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pokemonNameInputComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.pokemonNameInputComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.pokemonNameInputComboBox.FormattingEnabled = true;
            this.pokemonNameInputComboBox.Location = new System.Drawing.Point(53, 15);
            this.pokemonNameInputComboBox.Name = "pokemonNameInputComboBox";
            this.pokemonNameInputComboBox.Size = new System.Drawing.Size(290, 21);
            this.pokemonNameInputComboBox.TabIndex = 31;
            this.pokemonNameInputComboBox.SelectedIndexChanged += new System.EventHandler(this.pokemonNameInputComboBox_SelectedIndexChanged);
            // 
            // entryCountLabel
            // 
            this.entryCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.entryCountLabel.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.entryCountLabel, 3);
            this.entryCountLabel.Location = new System.Drawing.Point(5, 332);
            this.entryCountLabel.Name = "entryCountLabel";
            this.entryCountLabel.Padding = new System.Windows.Forms.Padding(1, 5, 1, 5);
            this.entryCountLabel.Size = new System.Drawing.Size(70, 23);
            this.entryCountLabel.TabIndex = 9;
            this.entryCountLabel.Text = "Entry Count: ";
            this.entryCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LearnsetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 459);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LearnsetEditor";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Learnset Editor";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pokemonPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.monNumberNumericUpDown)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.levelNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pokemonPictureBox;
        private System.Windows.Forms.Button saveDataButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox movesListBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button addMoveButton;
        private System.Windows.Forms.Button deleteMoveButton;
        private System.Windows.Forms.Button editMoveButton;
        private System.Windows.Forms.NumericUpDown levelNumericUpDown;
        private System.Windows.Forms.Label label1;
        private InputComboBox moveInputComboBox;
        private System.Windows.Forms.Label descriptorLabel;
        private System.Windows.Forms.Label statusLabel;
        public InputComboBox pokemonNameInputComboBox;
        public System.Windows.Forms.NumericUpDown monNumberNumericUpDown;
        private System.Windows.Forms.Label entryCountLabel;
    }
}