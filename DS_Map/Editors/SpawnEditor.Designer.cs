
namespace DSPRE {
    partial class SpawnEditor {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpawnEditor));
            this.spawnHeaderComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.matrixxUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.matrixyUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.localmapyUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.localmapxUpDown = new System.Windows.Forms.NumericUpDown();
            this.playerDirCombobox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.saveAndCloseSpawnEditorButton = new System.Windows.Forms.Button();
            this.readDefaultSpawnPosButton = new System.Windows.Forms.Button();
            this.locationNameLBL = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.initialMoneyUpDown = new System.Windows.Forms.NumericUpDown();
            this.resetFilterButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.matrixxUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixyUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.localmapyUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.localmapxUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.initialMoneyUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // spawnHeaderComboBox
            // 
            this.spawnHeaderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.spawnHeaderComboBox.FormattingEnabled = true;
            this.spawnHeaderComboBox.Location = new System.Drawing.Point(12, 28);
            this.spawnHeaderComboBox.Name = "spawnHeaderComboBox";
            this.spawnHeaderComboBox.Size = new System.Drawing.Size(167, 21);
            this.spawnHeaderComboBox.TabIndex = 0;
            this.spawnHeaderComboBox.SelectedIndexChanged += new System.EventHandler(this.spawnHeaderComboBox_IndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Spawn Header:";
            // 
            // matrixxUpDown
            // 
            this.matrixxUpDown.Location = new System.Drawing.Point(12, 95);
            this.matrixxUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.matrixxUpDown.Name = "matrixxUpDown";
            this.matrixxUpDown.Size = new System.Drawing.Size(59, 20);
            this.matrixxUpDown.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Matrix X";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(79, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Matrix Y";
            // 
            // matrixyUpDown
            // 
            this.matrixyUpDown.Location = new System.Drawing.Point(82, 95);
            this.matrixyUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.matrixyUpDown.Name = "matrixyUpDown";
            this.matrixyUpDown.Size = new System.Drawing.Size(59, 20);
            this.matrixyUpDown.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(292, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Local Map Y";
            // 
            // localmapyUpDown
            // 
            this.localmapyUpDown.Location = new System.Drawing.Point(295, 95);
            this.localmapyUpDown.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.localmapyUpDown.Name = "localmapyUpDown";
            this.localmapyUpDown.Size = new System.Drawing.Size(64, 20);
            this.localmapyUpDown.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(216, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Local Map X";
            // 
            // localmapxUpDown
            // 
            this.localmapxUpDown.Location = new System.Drawing.Point(219, 95);
            this.localmapxUpDown.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.localmapxUpDown.Name = "localmapxUpDown";
            this.localmapxUpDown.Size = new System.Drawing.Size(64, 20);
            this.localmapxUpDown.TabIndex = 6;
            // 
            // playerDirCombobox
            // 
            this.playerDirCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.playerDirCombobox.FormattingEnabled = true;
            this.playerDirCombobox.Location = new System.Drawing.Point(192, 28);
            this.playerDirCombobox.Name = "playerDirCombobox";
            this.playerDirCombobox.Size = new System.Drawing.Size(167, 21);
            this.playerDirCombobox.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(189, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Player Direction:";
            // 
            // saveAndCloseSpawnEditorButton
            // 
            this.saveAndCloseSpawnEditorButton.Image = ((System.Drawing.Image)(resources.GetObject("saveAndCloseSpawnEditorButton.Image")));
            this.saveAndCloseSpawnEditorButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.saveAndCloseSpawnEditorButton.Location = new System.Drawing.Point(241, 168);
            this.saveAndCloseSpawnEditorButton.Name = "saveAndCloseSpawnEditorButton";
            this.saveAndCloseSpawnEditorButton.Size = new System.Drawing.Size(110, 42);
            this.saveAndCloseSpawnEditorButton.TabIndex = 12;
            this.saveAndCloseSpawnEditorButton.Text = "Save Current\r\nSettings";
            this.saveAndCloseSpawnEditorButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveAndCloseSpawnEditorButton.UseVisualStyleBackColor = true;
            this.saveAndCloseSpawnEditorButton.Click += new System.EventHandler(this.saveSpawnEditorButton_Click);
            // 
            // readDefaultSpawnPosButton
            // 
            this.readDefaultSpawnPosButton.Image = global::DSPRE.Properties.Resources.resetIcon;
            this.readDefaultSpawnPosButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.readDefaultSpawnPosButton.Location = new System.Drawing.Point(123, 168);
            this.readDefaultSpawnPosButton.Name = "readDefaultSpawnPosButton";
            this.readDefaultSpawnPosButton.Size = new System.Drawing.Size(107, 42);
            this.readDefaultSpawnPosButton.TabIndex = 13;
            this.readDefaultSpawnPosButton.Text = "Load Saved\r\nSettings";
            this.readDefaultSpawnPosButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.readDefaultSpawnPosButton.UseVisualStyleBackColor = true;
            this.readDefaultSpawnPosButton.Click += new System.EventHandler(this.readDefaultSpawnPosButton_Click);
            // 
            // locationNameLBL
            // 
            this.locationNameLBL.AutoSize = true;
            this.locationNameLBL.Location = new System.Drawing.Point(9, 53);
            this.locationNameLBL.Name = "locationNameLBL";
            this.locationNameLBL.Size = new System.Drawing.Size(70, 13);
            this.locationNameLBL.TabIndex = 14;
            this.locationNameLBL.Text = "Location LBL";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(147, 122);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Initial Money:";
            // 
            // initialMoneyUpDown
            // 
            this.initialMoneyUpDown.Location = new System.Drawing.Point(148, 138);
            this.initialMoneyUpDown.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.initialMoneyUpDown.Name = "initialMoneyUpDown";
            this.initialMoneyUpDown.Size = new System.Drawing.Size(68, 20);
            this.initialMoneyUpDown.TabIndex = 16;
            // 
            // resetFilterButton
            // 
            this.resetFilterButton.Image = global::DSPRE.Properties.Resources.resetListIcon;
            this.resetFilterButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.resetFilterButton.Location = new System.Drawing.Point(19, 168);
            this.resetFilterButton.Name = "resetFilterButton";
            this.resetFilterButton.Size = new System.Drawing.Size(71, 42);
            this.resetFilterButton.TabIndex = 17;
            this.resetFilterButton.Text = "Reset\r\nFilter";
            this.resetFilterButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.resetFilterButton.UseVisualStyleBackColor = true;
            this.resetFilterButton.Click += new System.EventHandler(this.resetFilterButton_Click);
            // 
            // SpawnEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 214);
            this.Controls.Add(this.resetFilterButton);
            this.Controls.Add(this.initialMoneyUpDown);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.locationNameLBL);
            this.Controls.Add(this.readDefaultSpawnPosButton);
            this.Controls.Add(this.saveAndCloseSpawnEditorButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.playerDirCombobox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.localmapyUpDown);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.localmapxUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.matrixyUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.matrixxUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.spawnHeaderComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpawnEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Spawn Settings Editor";
            ((System.ComponentModel.ISupportInitialize)(this.matrixxUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixyUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.localmapyUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.localmapxUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.initialMoneyUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox spawnHeaderComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown matrixxUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown matrixyUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown localmapyUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown localmapxUpDown;
        private System.Windows.Forms.ComboBox playerDirCombobox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button saveAndCloseSpawnEditorButton;
        private System.Windows.Forms.Button readDefaultSpawnPosButton;
        private System.Windows.Forms.Label locationNameLBL;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown initialMoneyUpDown;
        private System.Windows.Forms.Button resetFilterButton;
    }
}