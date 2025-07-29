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
            this.romExportPathLabel = new System.Windows.Forms.Label();
            this.romExportPathTextBox = new System.Windows.Forms.TextBox();
            this.mapImportBasePathLabel = new System.Windows.Forms.Label();
            this.mapImportPathTextBox = new System.Windows.Forms.TextBox();
            this.changePathButton1 = new System.Windows.Forms.Button();
            this.changePathButton2 = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.clearButtonExport = new System.Windows.Forms.Button();
            this.clearButtonMap = new System.Windows.Forms.Button();
            this.clearButtonOpenDefault = new System.Windows.Forms.Button();
            this.changeOpenDefaultPathButton = new System.Windows.Forms.Button();
            this.openDefaultRomTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dontAskOpenCheckbox = new System.Windows.Forms.CheckBox();
            this.checkForUpdatesButton = new System.Windows.Forms.Button();
            this.currentVersionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // romExportPathLabel
            // 
            this.romExportPathLabel.AutoSize = true;
            this.romExportPathLabel.Location = new System.Drawing.Point(12, 80);
            this.romExportPathLabel.Name = "romExportPathLabel";
            this.romExportPathLabel.Size = new System.Drawing.Size(87, 13);
            this.romExportPathLabel.TabIndex = 0;
            this.romExportPathLabel.Text = "Rom Export Path";
            // 
            // romExportPathTextBox
            // 
            this.romExportPathTextBox.Location = new System.Drawing.Point(12, 96);
            this.romExportPathTextBox.Name = "romExportPathTextBox";
            this.romExportPathTextBox.ReadOnly = true;
            this.romExportPathTextBox.Size = new System.Drawing.Size(628, 20);
            this.romExportPathTextBox.TabIndex = 1;
            // 
            // mapImportBasePathLabel
            // 
            this.mapImportBasePathLabel.AutoSize = true;
            this.mapImportBasePathLabel.Location = new System.Drawing.Point(15, 219);
            this.mapImportBasePathLabel.Name = "mapImportBasePathLabel";
            this.mapImportBasePathLabel.Size = new System.Drawing.Size(112, 13);
            this.mapImportBasePathLabel.TabIndex = 2;
            this.mapImportBasePathLabel.Text = "Initial Map Import Path";
            this.mapImportBasePathLabel.Click += new System.EventHandler(this.mapImportBasePathLabel_Click);
            // 
            // mapImportPathTextBox
            // 
            this.mapImportPathTextBox.Location = new System.Drawing.Point(18, 236);
            this.mapImportPathTextBox.Name = "mapImportPathTextBox";
            this.mapImportPathTextBox.ReadOnly = true;
            this.mapImportPathTextBox.Size = new System.Drawing.Size(622, 20);
            this.mapImportPathTextBox.TabIndex = 3;
            // 
            // changePathButton1
            // 
            this.changePathButton1.Location = new System.Drawing.Point(533, 122);
            this.changePathButton1.Name = "changePathButton1";
            this.changePathButton1.Size = new System.Drawing.Size(107, 23);
            this.changePathButton1.TabIndex = 4;
            this.changePathButton1.Text = "Change Path";
            this.changePathButton1.UseVisualStyleBackColor = true;
            this.changePathButton1.Click += new System.EventHandler(this.changePathButton1_Click);
            // 
            // changePathButton2
            // 
            this.changePathButton2.Location = new System.Drawing.Point(533, 262);
            this.changePathButton2.Name = "changePathButton2";
            this.changePathButton2.Size = new System.Drawing.Size(107, 23);
            this.changePathButton2.TabIndex = 5;
            this.changePathButton2.Text = "Change Path";
            this.changePathButton2.UseVisualStyleBackColor = true;
            this.changePathButton2.Click += new System.EventHandler(this.changePathButton2_Click);
            // 
            // saveButton
            // 
            this.saveButton.Image = global::DSPRE.Properties.Resources.saveButton;
            this.saveButton.Location = new System.Drawing.Point(577, 301);
            this.saveButton.Name = "saveButton";
            this.saveButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.saveButton.Size = new System.Drawing.Size(64, 30);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "Save";
            this.saveButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // clearButtonExport
            // 
            this.clearButtonExport.Location = new System.Drawing.Point(452, 121);
            this.clearButtonExport.Name = "clearButtonExport";
            this.clearButtonExport.Size = new System.Drawing.Size(75, 24);
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
            this.clearButtonMap.Location = new System.Drawing.Point(452, 262);
            this.clearButtonMap.Name = "clearButtonMap";
            this.clearButtonMap.Size = new System.Drawing.Size(75, 23);
            this.clearButtonMap.TabIndex = 8;
            this.clearButtonMap.Text = "Clear";
            this.clearButtonMap.UseVisualStyleBackColor = true;
            this.clearButtonMap.Click += new System.EventHandler(this.clearButtonMap_Click);
            // 
            // clearButtonOpenDefault
            // 
            this.clearButtonOpenDefault.Location = new System.Drawing.Point(455, 191);
            this.clearButtonOpenDefault.Name = "clearButtonOpenDefault";
            this.clearButtonOpenDefault.Size = new System.Drawing.Size(75, 24);
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
            this.changeOpenDefaultPathButton.Location = new System.Drawing.Point(536, 192);
            this.changeOpenDefaultPathButton.Name = "changeOpenDefaultPathButton";
            this.changeOpenDefaultPathButton.Size = new System.Drawing.Size(107, 23);
            this.changeOpenDefaultPathButton.TabIndex = 15;
            this.changeOpenDefaultPathButton.Text = "Change Path";
            this.changeOpenDefaultPathButton.UseVisualStyleBackColor = true;
            this.changeOpenDefaultPathButton.Click += new System.EventHandler(this.changeOpenDefaultPathButton_Click);
            // 
            // openDefaultRomTextBox
            // 
            this.openDefaultRomTextBox.Location = new System.Drawing.Point(15, 166);
            this.openDefaultRomTextBox.Name = "openDefaultRomTextBox";
            this.openDefaultRomTextBox.ReadOnly = true;
            this.openDefaultRomTextBox.Size = new System.Drawing.Size(628, 20);
            this.openDefaultRomTextBox.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 150);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Open Default Rom";
            // 
            // dontAskOpenCheckbox
            // 
            this.dontAskOpenCheckbox.AutoSize = true;
            this.dontAskOpenCheckbox.Location = new System.Drawing.Point(326, 195);
            this.dontAskOpenCheckbox.Name = "dontAskOpenCheckbox";
            this.dontAskOpenCheckbox.Size = new System.Drawing.Size(123, 17);
            this.dontAskOpenCheckbox.TabIndex = 17;
            this.dontAskOpenCheckbox.Text = "Open without asking";
            this.dontAskOpenCheckbox.UseVisualStyleBackColor = true;
            this.dontAskOpenCheckbox.CheckedChanged += new System.EventHandler(this.dontAskOpenCheckbox_CheckedChanged);
            // 
            // checkForUpdatesButton
            // 
            this.checkForUpdatesButton.Location = new System.Drawing.Point(504, 30);
            this.checkForUpdatesButton.Name = "checkForUpdatesButton";
            this.checkForUpdatesButton.Size = new System.Drawing.Size(137, 29);
            this.checkForUpdatesButton.TabIndex = 18;
            this.checkForUpdatesButton.Text = "Check for updates";
            this.checkForUpdatesButton.UseVisualStyleBackColor = true;
            this.checkForUpdatesButton.Click += new System.EventHandler(this.checkForUpdatesButton_Click);
            // 
            // currentVersionLabel
            // 
            this.currentVersionLabel.AutoSize = true;
            this.currentVersionLabel.Location = new System.Drawing.Point(510, 9);
            this.currentVersionLabel.Name = "currentVersionLabel";
            this.currentVersionLabel.Size = new System.Drawing.Size(75, 13);
            this.currentVersionLabel.TabIndex = 19;
            this.currentVersionLabel.Text = "currentVersion";
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 341);
            this.Controls.Add(this.currentVersionLabel);
            this.Controls.Add(this.checkForUpdatesButton);
            this.Controls.Add(this.dontAskOpenCheckbox);
            this.Controls.Add(this.clearButtonOpenDefault);
            this.Controls.Add(this.changeOpenDefaultPathButton);
            this.Controls.Add(this.openDefaultRomTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.clearButtonMap);
            this.Controls.Add(this.clearButtonExport);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.changePathButton2);
            this.Controls.Add(this.changePathButton1);
            this.Controls.Add(this.mapImportPathTextBox);
            this.Controls.Add(this.mapImportBasePathLabel);
            this.Controls.Add(this.romExportPathTextBox);
            this.Controls.Add(this.romExportPathLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SettingsWindow";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsWindow_FormClosing);
            this.Load += new System.EventHandler(this.SettingsWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label romExportPathLabel;
        private System.Windows.Forms.TextBox romExportPathTextBox;
        private System.Windows.Forms.Label mapImportBasePathLabel;
        private System.Windows.Forms.Button changePathButton1;
        private System.Windows.Forms.Button changePathButton2;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button clearButtonExport;
        private System.Windows.Forms.Button clearButtonMap;
        private System.Windows.Forms.TextBox mapImportPathTextBox;
        private System.Windows.Forms.Button clearButtonOpenDefault;
        private System.Windows.Forms.Button changeOpenDefaultPathButton;
        private System.Windows.Forms.TextBox openDefaultRomTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox dontAskOpenCheckbox;
        private System.Windows.Forms.Button checkForUpdatesButton;
        private System.Windows.Forms.Label currentVersionLabel;
    }
}