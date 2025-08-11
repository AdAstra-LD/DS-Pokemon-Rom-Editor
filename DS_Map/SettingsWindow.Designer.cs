namespace DSPRE
{
    partial class SettingsWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsWindow));
            this.romExportPathTextBox = new System.Windows.Forms.TextBox();
            this.mapImportPathTextBox = new System.Windows.Forms.TextBox();
            this.changePathButton1 = new System.Windows.Forms.Button();
            this.changePathButton2 = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.clearButtonExport = new System.Windows.Forms.Button();
            this.clearButtonMap = new System.Windows.Forms.Button();
            this.clearButtonOpenDefault = new System.Windows.Forms.Button();
            this.changeOpenDefaultPathButton = new System.Windows.Forms.Button();
            this.openDefaultRomTextBox = new System.Windows.Forms.TextBox();
            this.dontAskOpenCheckbox = new System.Windows.Forms.CheckBox();
            this.checkForUpdatesButton = new System.Windows.Forms.Button();
            this.currentVersionLabel = new System.Windows.Forms.Label();
            this.automaticCheckUpdateCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.advancedmodeCB = new System.Windows.Forms.PictureBox();
            this.enabledAdvancedModeButton = new System.Windows.Forms.Button();
            this.automaticCheckDBUpdateCheckbox = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.advancedmodeCB)).BeginInit();
            this.SuspendLayout();
            // 
            // romExportPathTextBox
            // 
            this.romExportPathTextBox.Location = new System.Drawing.Point(10, 19);
            this.romExportPathTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.romExportPathTextBox.Name = "romExportPathTextBox";
            this.romExportPathTextBox.ReadOnly = true;
            this.romExportPathTextBox.Size = new System.Drawing.Size(564, 22);
            this.romExportPathTextBox.TabIndex = 1;
            // 
            // mapImportPathTextBox
            // 
            this.mapImportPathTextBox.Location = new System.Drawing.Point(10, 20);
            this.mapImportPathTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.mapImportPathTextBox.Name = "mapImportPathTextBox";
            this.mapImportPathTextBox.ReadOnly = true;
            this.mapImportPathTextBox.Size = new System.Drawing.Size(564, 22);
            this.mapImportPathTextBox.TabIndex = 3;
            // 
            // changePathButton1
            // 
            this.changePathButton1.Location = new System.Drawing.Point(687, 19);
            this.changePathButton1.Margin = new System.Windows.Forms.Padding(4);
            this.changePathButton1.Name = "changePathButton1";
            this.changePathButton1.Size = new System.Drawing.Size(146, 30);
            this.changePathButton1.TabIndex = 4;
            this.changePathButton1.Text = "Change Path";
            this.changePathButton1.UseVisualStyleBackColor = true;
            this.changePathButton1.Click += new System.EventHandler(this.changePathButton1_Click);
            // 
            // changePathButton2
            // 
            this.changePathButton2.Location = new System.Drawing.Point(687, 20);
            this.changePathButton2.Margin = new System.Windows.Forms.Padding(4);
            this.changePathButton2.Name = "changePathButton2";
            this.changePathButton2.Size = new System.Drawing.Size(146, 28);
            this.changePathButton2.TabIndex = 5;
            this.changePathButton2.Text = "Change Path";
            this.changePathButton2.UseVisualStyleBackColor = true;
            this.changePathButton2.Click += new System.EventHandler(this.changePathButton2_Click);
            // 
            // saveButton
            // 
            this.saveButton.Image = global::DSPRE.Properties.Resources.saveButton;
            this.saveButton.Location = new System.Drawing.Point(769, 370);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4);
            this.saveButton.Name = "saveButton";
            this.saveButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.saveButton.Size = new System.Drawing.Size(85, 37);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "Save";
            this.saveButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // clearButtonExport
            // 
            this.clearButtonExport.Location = new System.Drawing.Point(582, 19);
            this.clearButtonExport.Margin = new System.Windows.Forms.Padding(4);
            this.clearButtonExport.Name = "clearButtonExport";
            this.clearButtonExport.Size = new System.Drawing.Size(100, 30);
            this.clearButtonExport.TabIndex = 7;
            this.clearButtonExport.Text = "Clear";
            this.clearButtonExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.clearButtonExport.UseMnemonic = false;
            this.clearButtonExport.UseVisualStyleBackColor = true;
            this.clearButtonExport.UseWaitCursor = true;
            this.clearButtonExport.Click += new System.EventHandler(this.clearButtonExport_Click);
            // 
            // clearButtonMap
            // 
            this.clearButtonMap.Location = new System.Drawing.Point(582, 20);
            this.clearButtonMap.Margin = new System.Windows.Forms.Padding(4);
            this.clearButtonMap.Name = "clearButtonMap";
            this.clearButtonMap.Size = new System.Drawing.Size(100, 28);
            this.clearButtonMap.TabIndex = 8;
            this.clearButtonMap.Text = "Clear";
            this.clearButtonMap.UseVisualStyleBackColor = true;
            this.clearButtonMap.Click += new System.EventHandler(this.clearButtonMap_Click);
            // 
            // clearButtonOpenDefault
            // 
            this.clearButtonOpenDefault.Location = new System.Drawing.Point(582, 23);
            this.clearButtonOpenDefault.Margin = new System.Windows.Forms.Padding(4);
            this.clearButtonOpenDefault.Name = "clearButtonOpenDefault";
            this.clearButtonOpenDefault.Size = new System.Drawing.Size(100, 30);
            this.clearButtonOpenDefault.TabIndex = 16;
            this.clearButtonOpenDefault.Text = "Clear";
            this.clearButtonOpenDefault.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.clearButtonOpenDefault.UseMnemonic = false;
            this.clearButtonOpenDefault.UseVisualStyleBackColor = true;
            this.clearButtonOpenDefault.UseWaitCursor = true;
            this.clearButtonOpenDefault.Click += new System.EventHandler(this.clearButtonOpenDefault_Click);
            // 
            // changeOpenDefaultPathButton
            // 
            this.changeOpenDefaultPathButton.Location = new System.Drawing.Point(688, 23);
            this.changeOpenDefaultPathButton.Margin = new System.Windows.Forms.Padding(4);
            this.changeOpenDefaultPathButton.Name = "changeOpenDefaultPathButton";
            this.changeOpenDefaultPathButton.Size = new System.Drawing.Size(145, 30);
            this.changeOpenDefaultPathButton.TabIndex = 15;
            this.changeOpenDefaultPathButton.Text = "Change Path";
            this.changeOpenDefaultPathButton.UseVisualStyleBackColor = true;
            this.changeOpenDefaultPathButton.Click += new System.EventHandler(this.changeOpenDefaultPathButton_Click);
            // 
            // openDefaultRomTextBox
            // 
            this.openDefaultRomTextBox.Location = new System.Drawing.Point(10, 23);
            this.openDefaultRomTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.openDefaultRomTextBox.Name = "openDefaultRomTextBox";
            this.openDefaultRomTextBox.ReadOnly = true;
            this.openDefaultRomTextBox.Size = new System.Drawing.Size(564, 22);
            this.openDefaultRomTextBox.TabIndex = 14;
            // 
            // dontAskOpenCheckbox
            // 
            this.dontAskOpenCheckbox.AutoSize = true;
            this.dontAskOpenCheckbox.Location = new System.Drawing.Point(10, 53);
            this.dontAskOpenCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.dontAskOpenCheckbox.Name = "dontAskOpenCheckbox";
            this.dontAskOpenCheckbox.Size = new System.Drawing.Size(148, 20);
            this.dontAskOpenCheckbox.TabIndex = 17;
            this.dontAskOpenCheckbox.Text = "Open without asking";
            this.dontAskOpenCheckbox.UseVisualStyleBackColor = true;
            // 
            // checkForUpdatesButton
            // 
            this.checkForUpdatesButton.Location = new System.Drawing.Point(13, 29);
            this.checkForUpdatesButton.Margin = new System.Windows.Forms.Padding(4);
            this.checkForUpdatesButton.Name = "checkForUpdatesButton";
            this.checkForUpdatesButton.Size = new System.Drawing.Size(183, 36);
            this.checkForUpdatesButton.TabIndex = 18;
            this.checkForUpdatesButton.Text = "Check App updates";
            this.checkForUpdatesButton.UseVisualStyleBackColor = true;
            this.checkForUpdatesButton.Click += new System.EventHandler(this.checkForUpdatesButton_Click);
            // 
            // currentVersionLabel
            // 
            this.currentVersionLabel.AutoSize = true;
            this.currentVersionLabel.Location = new System.Drawing.Point(13, 9);
            this.currentVersionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.currentVersionLabel.Name = "currentVersionLabel";
            this.currentVersionLabel.Size = new System.Drawing.Size(93, 16);
            this.currentVersionLabel.TabIndex = 19;
            this.currentVersionLabel.Text = "currentVersion";
            this.currentVersionLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // automaticCheckUpdateCheckbox
            // 
            this.automaticCheckUpdateCheckbox.AutoSize = true;
            this.automaticCheckUpdateCheckbox.Location = new System.Drawing.Point(16, 73);
            this.automaticCheckUpdateCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.automaticCheckUpdateCheckbox.Name = "automaticCheckUpdateCheckbox";
            this.automaticCheckUpdateCheckbox.Size = new System.Drawing.Size(233, 20);
            this.automaticCheckUpdateCheckbox.TabIndex = 20;
            this.automaticCheckUpdateCheckbox.Text = "Check App Updates Automatically";
            this.automaticCheckUpdateCheckbox.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.romExportPathTextBox);
            this.groupBox3.Controls.Add(this.clearButtonExport);
            this.groupBox3.Controls.Add(this.changePathButton1);
            this.groupBox3.Location = new System.Drawing.Point(13, 135);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(841, 62);
            this.groupBox3.TabIndex = 31;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Rom Export Path";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.openDefaultRomTextBox);
            this.groupBox1.Controls.Add(this.clearButtonOpenDefault);
            this.groupBox1.Controls.Add(this.changeOpenDefaultPathButton);
            this.groupBox1.Controls.Add(this.dontAskOpenCheckbox);
            this.groupBox1.Location = new System.Drawing.Point(13, 205);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(841, 87);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Open Default Rom";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mapImportPathTextBox);
            this.groupBox2.Controls.Add(this.clearButtonMap);
            this.groupBox2.Controls.Add(this.changePathButton2);
            this.groupBox2.Location = new System.Drawing.Point(13, 300);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(841, 62);
            this.groupBox2.TabIndex = 32;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Initial Map Import Path";
            // 
            // advancedmodeCB
            // 
            this.advancedmodeCB.Image = ((System.Drawing.Image)(resources.GetObject("advancedmodeCB.Image")));
            this.advancedmodeCB.Location = new System.Drawing.Point(686, 29);
            this.advancedmodeCB.Margin = new System.Windows.Forms.Padding(4);
            this.advancedmodeCB.Name = "advancedmodeCB";
            this.advancedmodeCB.Size = new System.Drawing.Size(27, 25);
            this.advancedmodeCB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.advancedmodeCB.TabIndex = 34;
            this.advancedmodeCB.TabStop = false;
            this.advancedmodeCB.Visible = false;
            // 
            // enabledAdvancedModeButton
            // 
            this.enabledAdvancedModeButton.Enabled = false;
            this.enabledAdvancedModeButton.Location = new System.Drawing.Point(721, 13);
            this.enabledAdvancedModeButton.Margin = new System.Windows.Forms.Padding(4);
            this.enabledAdvancedModeButton.Name = "enabledAdvancedModeButton";
            this.enabledAdvancedModeButton.Size = new System.Drawing.Size(133, 62);
            this.enabledAdvancedModeButton.TabIndex = 33;
            this.enabledAdvancedModeButton.Text = "Enable advanced mode (not implemented yet)";
            this.enabledAdvancedModeButton.UseVisualStyleBackColor = false;
            // 
            // automaticCheckDBUpdateCheckbox
            // 
            this.automaticCheckDBUpdateCheckbox.AutoSize = true;
            this.automaticCheckDBUpdateCheckbox.Location = new System.Drawing.Point(283, 73);
            this.automaticCheckDBUpdateCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.automaticCheckDBUpdateCheckbox.Name = "automaticCheckDBUpdateCheckbox";
            this.automaticCheckDBUpdateCheckbox.Size = new System.Drawing.Size(227, 20);
            this.automaticCheckDBUpdateCheckbox.TabIndex = 35;
            this.automaticCheckDBUpdateCheckbox.Text = "Update Databases Automatically";
            this.automaticCheckDBUpdateCheckbox.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(283, 29);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(183, 36);
            this.button1.TabIndex = 36;
            this.button1.Text = "Check Database updates";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.checkDBUpdatesButton_Click);
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 420);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.automaticCheckDBUpdateCheckbox);
            this.Controls.Add(this.advancedmodeCB);
            this.Controls.Add(this.enabledAdvancedModeButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.automaticCheckUpdateCheckbox);
            this.Controls.Add(this.currentVersionLabel);
            this.Controls.Add(this.checkForUpdatesButton);
            this.Controls.Add(this.saveButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SettingsWindow";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsWindow_FormClosing);
            this.Load += new System.EventHandler(this.SettingsWindow_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.advancedmodeCB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox romExportPathTextBox;
        private System.Windows.Forms.Button changePathButton1;
        private System.Windows.Forms.Button changePathButton2;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button clearButtonExport;
        private System.Windows.Forms.Button clearButtonMap;
        private System.Windows.Forms.TextBox mapImportPathTextBox;
        private System.Windows.Forms.Button clearButtonOpenDefault;
        private System.Windows.Forms.Button changeOpenDefaultPathButton;
        private System.Windows.Forms.TextBox openDefaultRomTextBox;
        private System.Windows.Forms.CheckBox dontAskOpenCheckbox;
        private System.Windows.Forms.Button checkForUpdatesButton;
        private System.Windows.Forms.Label currentVersionLabel;
        private System.Windows.Forms.CheckBox automaticCheckUpdateCheckbox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox advancedmodeCB;
        private System.Windows.Forms.Button enabledAdvancedModeButton;
        private System.Windows.Forms.CheckBox automaticCheckDBUpdateCheckbox;
        private System.Windows.Forms.Button button1;
    }
}