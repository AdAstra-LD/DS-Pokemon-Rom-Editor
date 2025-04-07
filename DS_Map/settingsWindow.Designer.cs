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
            this.clearVSCPath = new System.Windows.Forms.Button();
            this.changePathButton3 = new System.Windows.Forms.Button();
            this.VSCPathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // romExportPathLabel
            // 
            this.romExportPathLabel.AutoSize = true;
            this.romExportPathLabel.Location = new System.Drawing.Point(12, 9);
            this.romExportPathLabel.Name = "romExportPathLabel";
            this.romExportPathLabel.Size = new System.Drawing.Size(87, 13);
            this.romExportPathLabel.TabIndex = 0;
            this.romExportPathLabel.Text = "Rom Export Path";
            // 
            // romExportPathTextBox
            // 
            this.romExportPathTextBox.Location = new System.Drawing.Point(12, 25);
            this.romExportPathTextBox.Name = "romExportPathTextBox";
            this.romExportPathTextBox.ReadOnly = true;
            this.romExportPathTextBox.Size = new System.Drawing.Size(628, 20);
            this.romExportPathTextBox.TabIndex = 1;
            // 
            // mapImportBasePathLabel
            // 
            this.mapImportBasePathLabel.AutoSize = true;
            this.mapImportBasePathLabel.Location = new System.Drawing.Point(15, 89);
            this.mapImportBasePathLabel.Name = "mapImportBasePathLabel";
            this.mapImportBasePathLabel.Size = new System.Drawing.Size(112, 13);
            this.mapImportBasePathLabel.TabIndex = 2;
            this.mapImportBasePathLabel.Text = "Initial Map Import Path";
            this.mapImportBasePathLabel.Click += new System.EventHandler(this.mapImportBasePathLabel_Click);
            // 
            // mapImportPathTextBox
            // 
            this.mapImportPathTextBox.Location = new System.Drawing.Point(18, 106);
            this.mapImportPathTextBox.Name = "mapImportPathTextBox";
            this.mapImportPathTextBox.ReadOnly = true;
            this.mapImportPathTextBox.Size = new System.Drawing.Size(622, 20);
            this.mapImportPathTextBox.TabIndex = 3;
            // 
            // changePathButton1
            // 
            this.changePathButton1.Location = new System.Drawing.Point(533, 51);
            this.changePathButton1.Name = "changePathButton1";
            this.changePathButton1.Size = new System.Drawing.Size(107, 23);
            this.changePathButton1.TabIndex = 4;
            this.changePathButton1.Text = "Change Path";
            this.changePathButton1.UseVisualStyleBackColor = true;
            this.changePathButton1.Click += new System.EventHandler(this.changePathButton1_Click);
            // 
            // changePathButton2
            // 
            this.changePathButton2.Location = new System.Drawing.Point(533, 132);
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
            this.saveButton.Location = new System.Drawing.Point(617, 256);
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
            this.clearButtonExport.Location = new System.Drawing.Point(452, 50);
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
            this.clearButtonMap.Location = new System.Drawing.Point(452, 132);
            this.clearButtonMap.Name = "clearButtonMap";
            this.clearButtonMap.Size = new System.Drawing.Size(75, 23);
            this.clearButtonMap.TabIndex = 8;
            this.clearButtonMap.Text = "Clear";
            this.clearButtonMap.UseVisualStyleBackColor = true;
            this.clearButtonMap.Click += new System.EventHandler(this.clearButtonMap_Click);
            // 
            // clearVSCPath
            // 
            this.clearVSCPath.Location = new System.Drawing.Point(452, 213);
            this.clearVSCPath.Name = "clearVSCPath";
            this.clearVSCPath.Size = new System.Drawing.Size(75, 23);
            this.clearVSCPath.TabIndex = 12;
            this.clearVSCPath.Text = "Clear";
            this.clearVSCPath.UseVisualStyleBackColor = true;
            this.clearVSCPath.Click += new System.EventHandler(this.clearVSCPath_Click);
            // 
            // changePathButton3
            // 
            this.changePathButton3.Location = new System.Drawing.Point(533, 213);
            this.changePathButton3.Name = "changePathButton3";
            this.changePathButton3.Size = new System.Drawing.Size(107, 23);
            this.changePathButton3.TabIndex = 11;
            this.changePathButton3.Text = "Change Path";
            this.changePathButton3.UseVisualStyleBackColor = true;
            this.changePathButton3.Click += new System.EventHandler(this.changePathButton3_Click);
            // 
            // VSCPathTextBox
            // 
            this.VSCPathTextBox.Location = new System.Drawing.Point(18, 187);
            this.VSCPathTextBox.Name = "VSCPathTextBox";
            this.VSCPathTextBox.ReadOnly = true;
            this.VSCPathTextBox.Size = new System.Drawing.Size(622, 20);
            this.VSCPathTextBox.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 170);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Visual Studio Code Path";
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 298);
            this.Controls.Add(this.clearVSCPath);
            this.Controls.Add(this.changePathButton3);
            this.Controls.Add(this.VSCPathTextBox);
            this.Controls.Add(this.label1);
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
        private System.Windows.Forms.TextBox mapImportPathTextBox;
        private System.Windows.Forms.Button changePathButton1;
        private System.Windows.Forms.Button changePathButton2;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button clearButtonExport;
        private System.Windows.Forms.Button clearButtonMap;
        private System.Windows.Forms.Button clearVSCPath;
        private System.Windows.Forms.Button changePathButton3;
        private System.Windows.Forms.TextBox VSCPathTextBox;
        private System.Windows.Forms.Label label1;
    }
}