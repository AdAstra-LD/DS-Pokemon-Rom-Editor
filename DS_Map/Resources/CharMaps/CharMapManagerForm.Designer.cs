namespace DSPRE.CharMaps
{
    partial class CharMapManagerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.aliasLabel = new System.Windows.Forms.Label();
            this.aliasListBox = new System.Windows.Forms.ListBox();
            this.reloadButton = new System.Windows.Forms.Button();
            this.addAliasButton = new System.Windows.Forms.Button();
            this.newAliasTextBox = new System.Windows.Forms.TextBox();
            this.newAliasLabel = new System.Windows.Forms.Label();
            this.charLabel = new System.Windows.Forms.Label();
            this.codeComboBox = new System.Windows.Forms.ComboBox();
            this.aliasGroupBox = new System.Windows.Forms.GroupBox();
            this.removeAliasButton = new System.Windows.Forms.Button();
            this.charmapGroupBox = new System.Windows.Forms.GroupBox();
            this.charmapLabel = new System.Windows.Forms.Label();
            this.charMapListBox = new System.Windows.Forms.ListBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.createCustomMapButton = new System.Windows.Forms.Button();
            this.deleteCustomMapButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.aliasGroupBox.SuspendLayout();
            this.charmapGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // aliasLabel
            // 
            this.aliasLabel.AutoSize = true;
            this.aliasLabel.Location = new System.Drawing.Point(3, 29);
            this.aliasLabel.Name = "aliasLabel";
            this.aliasLabel.Size = new System.Drawing.Size(84, 13);
            this.aliasLabel.TabIndex = 0;
            this.aliasLabel.Text = "Avalable Aliases";
            // 
            // aliasListBox
            // 
            this.aliasListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aliasListBox.FormattingEnabled = true;
            this.aliasListBox.ItemHeight = 20;
            this.aliasListBox.Location = new System.Drawing.Point(6, 46);
            this.aliasListBox.Name = "aliasListBox";
            this.aliasListBox.Size = new System.Drawing.Size(151, 224);
            this.aliasListBox.TabIndex = 1;
            // 
            // reloadButton
            // 
            this.reloadButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reloadButton.Image = global::DSPRE.Properties.Resources.resetIcon;
            this.reloadButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.reloadButton.Location = new System.Drawing.Point(214, 163);
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.Size = new System.Drawing.Size(100, 50);
            this.reloadButton.TabIndex = 2;
            this.reloadButton.Text = "Reload \r\nMappings";
            this.reloadButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.reloadButton, "Reload mappings from file. Unsaved edits will be lost!");
            this.reloadButton.UseVisualStyleBackColor = true;
            this.reloadButton.Click += new System.EventHandler(this.reloadButton_Click);
            // 
            // addAliasButton
            // 
            this.addAliasButton.Image = global::DSPRE.Properties.Resources.addIcon;
            this.addAliasButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addAliasButton.Location = new System.Drawing.Point(163, 132);
            this.addAliasButton.Name = "addAliasButton";
            this.addAliasButton.Size = new System.Drawing.Size(151, 23);
            this.addAliasButton.TabIndex = 3;
            this.addAliasButton.Text = "Add New Alias";
            this.addAliasButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.addAliasButton, "Add new alias");
            this.addAliasButton.UseVisualStyleBackColor = true;
            this.addAliasButton.Click += new System.EventHandler(this.addAliasButton_Click);
            // 
            // newAliasTextBox
            // 
            this.newAliasTextBox.Location = new System.Drawing.Point(163, 62);
            this.newAliasTextBox.Name = "newAliasTextBox";
            this.newAliasTextBox.Size = new System.Drawing.Size(151, 20);
            this.newAliasTextBox.TabIndex = 4;
            // 
            // newAliasLabel
            // 
            this.newAliasLabel.AutoSize = true;
            this.newAliasLabel.Location = new System.Drawing.Point(160, 46);
            this.newAliasLabel.Name = "newAliasLabel";
            this.newAliasLabel.Size = new System.Drawing.Size(29, 13);
            this.newAliasLabel.TabIndex = 5;
            this.newAliasLabel.Text = "Alias";
            // 
            // charLabel
            // 
            this.charLabel.AutoSize = true;
            this.charLabel.Location = new System.Drawing.Point(160, 86);
            this.charLabel.Name = "charLabel";
            this.charLabel.Size = new System.Drawing.Size(53, 13);
            this.charLabel.TabIndex = 6;
            this.charLabel.Text = "Character";
            // 
            // codeComboBox
            // 
            this.codeComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.codeComboBox.FormattingEnabled = true;
            this.codeComboBox.Location = new System.Drawing.Point(163, 102);
            this.codeComboBox.Name = "codeComboBox";
            this.codeComboBox.Size = new System.Drawing.Size(151, 24);
            this.codeComboBox.TabIndex = 7;
            // 
            // aliasGroupBox
            // 
            this.aliasGroupBox.Controls.Add(this.removeAliasButton);
            this.aliasGroupBox.Controls.Add(this.aliasListBox);
            this.aliasGroupBox.Controls.Add(this.newAliasLabel);
            this.aliasGroupBox.Controls.Add(this.charLabel);
            this.aliasGroupBox.Controls.Add(this.codeComboBox);
            this.aliasGroupBox.Controls.Add(this.aliasLabel);
            this.aliasGroupBox.Controls.Add(this.addAliasButton);
            this.aliasGroupBox.Controls.Add(this.newAliasTextBox);
            this.aliasGroupBox.Location = new System.Drawing.Point(12, 12);
            this.aliasGroupBox.Name = "aliasGroupBox";
            this.aliasGroupBox.Size = new System.Drawing.Size(320, 280);
            this.aliasGroupBox.TabIndex = 8;
            this.aliasGroupBox.TabStop = false;
            this.aliasGroupBox.Text = "Manage Aliases";
            // 
            // removeAliasButton
            // 
            this.removeAliasButton.Image = global::DSPRE.Properties.Resources.deleteIcon;
            this.removeAliasButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.removeAliasButton.Location = new System.Drawing.Point(163, 161);
            this.removeAliasButton.Name = "removeAliasButton";
            this.removeAliasButton.Size = new System.Drawing.Size(151, 23);
            this.removeAliasButton.TabIndex = 8;
            this.removeAliasButton.Text = "Remove Alias";
            this.removeAliasButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.removeAliasButton, "Remove selected alias");
            this.removeAliasButton.UseVisualStyleBackColor = true;
            this.removeAliasButton.Click += new System.EventHandler(this.removeAliasButton_Click);
            // 
            // charmapGroupBox
            // 
            this.charmapGroupBox.Controls.Add(this.charmapLabel);
            this.charmapGroupBox.Controls.Add(this.charMapListBox);
            this.charmapGroupBox.Controls.Add(this.saveButton);
            this.charmapGroupBox.Controls.Add(this.createCustomMapButton);
            this.charmapGroupBox.Controls.Add(this.deleteCustomMapButton);
            this.charmapGroupBox.Controls.Add(this.reloadButton);
            this.charmapGroupBox.Location = new System.Drawing.Point(338, 12);
            this.charmapGroupBox.Name = "charmapGroupBox";
            this.charmapGroupBox.Size = new System.Drawing.Size(320, 280);
            this.charmapGroupBox.TabIndex = 9;
            this.charmapGroupBox.TabStop = false;
            this.charmapGroupBox.Text = "Manage Custom Charmap";
            // 
            // charmapLabel
            // 
            this.charmapLabel.AutoSize = true;
            this.charmapLabel.Location = new System.Drawing.Point(6, 28);
            this.charmapLabel.Name = "charmapLabel";
            this.charmapLabel.Size = new System.Drawing.Size(68, 13);
            this.charmapLabel.TabIndex = 13;
            this.charmapLabel.Text = "Full Charmap";
            // 
            // charMapListBox
            // 
            this.charMapListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.charMapListBox.FormattingEnabled = true;
            this.charMapListBox.ItemHeight = 20;
            this.charMapListBox.Location = new System.Drawing.Point(6, 45);
            this.charMapListBox.Name = "charMapListBox";
            this.charMapListBox.Size = new System.Drawing.Size(202, 224);
            this.charMapListBox.TabIndex = 12;
            this.toolTip.SetToolTip(this.charMapListBox, "List of all mappings. Double-click to select character.\r\n");
            this.charMapListBox.DoubleClick += new System.EventHandler(this.charMapListBox_DoubleClick);
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.Image = global::DSPRE.Properties.Resources.save_rom;
            this.saveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveButton.Location = new System.Drawing.Point(214, 219);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(100, 50);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "Save\r\nMappings";
            this.saveButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.saveButton, "Save edits to file.");
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // createCustomMapButton
            // 
            this.createCustomMapButton.Image = global::DSPRE.Properties.Resources.addIcon;
            this.createCustomMapButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.createCustomMapButton.Location = new System.Drawing.Point(214, 51);
            this.createCustomMapButton.Name = "createCustomMapButton";
            this.createCustomMapButton.Size = new System.Drawing.Size(100, 50);
            this.createCustomMapButton.TabIndex = 10;
            this.createCustomMapButton.Text = "Create Custom Charmap";
            this.createCustomMapButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.createCustomMapButton, "Create new custom charmap. Will overwrite existing map.");
            this.createCustomMapButton.UseVisualStyleBackColor = true;
            this.createCustomMapButton.Click += new System.EventHandler(this.createCustomMapButton_Click);
            // 
            // deleteCustomMapButton
            // 
            this.deleteCustomMapButton.Image = global::DSPRE.Properties.Resources.deleteIcon;
            this.deleteCustomMapButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.deleteCustomMapButton.Location = new System.Drawing.Point(214, 107);
            this.deleteCustomMapButton.Name = "deleteCustomMapButton";
            this.deleteCustomMapButton.Size = new System.Drawing.Size(100, 50);
            this.deleteCustomMapButton.TabIndex = 9;
            this.deleteCustomMapButton.Text = "Delete Custom Charmap";
            this.deleteCustomMapButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.deleteCustomMapButton, "Delete charmap file. Once you close the editor this can not be undone.");
            this.deleteCustomMapButton.UseVisualStyleBackColor = true;
            this.deleteCustomMapButton.Click += new System.EventHandler(this.deleteCustomMapButton_Click);
            // 
            // CharMapManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 303);
            this.Controls.Add(this.charmapGroupBox);
            this.Controls.Add(this.aliasGroupBox);
            this.Name = "CharMapManagerForm";
            this.Text = "Character Map Manager";
            this.aliasGroupBox.ResumeLayout(false);
            this.aliasGroupBox.PerformLayout();
            this.charmapGroupBox.ResumeLayout(false);
            this.charmapGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label aliasLabel;
        private System.Windows.Forms.ListBox aliasListBox;
        private System.Windows.Forms.Button reloadButton;
        private System.Windows.Forms.Button addAliasButton;
        private System.Windows.Forms.TextBox newAliasTextBox;
        private System.Windows.Forms.Label newAliasLabel;
        private System.Windows.Forms.Label charLabel;
        private System.Windows.Forms.ComboBox codeComboBox;
        private System.Windows.Forms.GroupBox aliasGroupBox;
        private System.Windows.Forms.Button removeAliasButton;
        private System.Windows.Forms.GroupBox charmapGroupBox;
        private System.Windows.Forms.Button deleteCustomMapButton;
        private System.Windows.Forms.Button createCustomMapButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label charmapLabel;
        private System.Windows.Forms.ListBox charMapListBox;
        private System.Windows.Forms.ToolTip toolTip;
    }
}