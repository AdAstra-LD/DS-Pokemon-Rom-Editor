namespace DS_Map
{
    partial class GivePokémonDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.speciesComboBox = new System.Windows.Forms.ComboBox();
            this.levelNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.movesetCheckBox = new System.Windows.Forms.CheckBox();
            this.movesetGroupBox = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.move4ComboBox = new System.Windows.Forms.ComboBox();
            this.move3ComboBox = new System.Windows.Forms.ComboBox();
            this.move2ComboBox = new System.Windows.Forms.ComboBox();
            this.move1ComboBox = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.itemLabel = new System.Windows.Forms.Label();
            this.itemComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.levelNumericUpDown)).BeginInit();
            this.movesetGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Species";
            // 
            // speciesComboBox
            // 
            this.speciesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.speciesComboBox.FormattingEnabled = true;
            this.speciesComboBox.Location = new System.Drawing.Point(12, 28);
            this.speciesComboBox.Name = "speciesComboBox";
            this.speciesComboBox.Size = new System.Drawing.Size(151, 21);
            this.speciesComboBox.TabIndex = 1;
            // 
            // levelNumericUpDown
            // 
            this.levelNumericUpDown.Location = new System.Drawing.Point(202, 28);
            this.levelNumericUpDown.Name = "levelNumericUpDown";
            this.levelNumericUpDown.Size = new System.Drawing.Size(95, 20);
            this.levelNumericUpDown.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(199, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Level";
            // 
            // movesetCheckBox
            // 
            this.movesetCheckBox.AutoSize = true;
            this.movesetCheckBox.Enabled = false;
            this.movesetCheckBox.Location = new System.Drawing.Point(202, 87);
            this.movesetCheckBox.Name = "movesetCheckBox";
            this.movesetCheckBox.Size = new System.Drawing.Size(128, 17);
            this.movesetCheckBox.TabIndex = 7;
            this.movesetCheckBox.Text = "Moveset (HGSS only)";
            this.movesetCheckBox.UseVisualStyleBackColor = true;
            this.movesetCheckBox.CheckedChanged += new System.EventHandler(this.movesetCheckBox_CheckedChanged);
            // 
            // movesetGroupBox
            // 
            this.movesetGroupBox.Controls.Add(this.label6);
            this.movesetGroupBox.Controls.Add(this.label5);
            this.movesetGroupBox.Controls.Add(this.label4);
            this.movesetGroupBox.Controls.Add(this.label3);
            this.movesetGroupBox.Controls.Add(this.move4ComboBox);
            this.movesetGroupBox.Controls.Add(this.move3ComboBox);
            this.movesetGroupBox.Controls.Add(this.move2ComboBox);
            this.movesetGroupBox.Controls.Add(this.move1ComboBox);
            this.movesetGroupBox.Enabled = false;
            this.movesetGroupBox.Location = new System.Drawing.Point(11, 112);
            this.movesetGroupBox.Name = "movesetGroupBox";
            this.movesetGroupBox.Size = new System.Drawing.Size(319, 145);
            this.movesetGroupBox.TabIndex = 8;
            this.movesetGroupBox.TabStop = false;
            this.movesetGroupBox.Text = "Moveset";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(160, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Move 4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Move 3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(160, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Move 2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Move 1";
            // 
            // move4ComboBox
            // 
            this.move4ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.move4ComboBox.FormattingEnabled = true;
            this.move4ComboBox.Location = new System.Drawing.Point(163, 103);
            this.move4ComboBox.Name = "move4ComboBox";
            this.move4ComboBox.Size = new System.Drawing.Size(147, 21);
            this.move4ComboBox.TabIndex = 3;
            // 
            // move3ComboBox
            // 
            this.move3ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.move3ComboBox.FormattingEnabled = true;
            this.move3ComboBox.Location = new System.Drawing.Point(8, 103);
            this.move3ComboBox.Name = "move3ComboBox";
            this.move3ComboBox.Size = new System.Drawing.Size(144, 21);
            this.move3ComboBox.TabIndex = 2;
            // 
            // move2ComboBox
            // 
            this.move2ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.move2ComboBox.FormattingEnabled = true;
            this.move2ComboBox.Location = new System.Drawing.Point(163, 42);
            this.move2ComboBox.Name = "move2ComboBox";
            this.move2ComboBox.Size = new System.Drawing.Size(147, 21);
            this.move2ComboBox.TabIndex = 1;
            // 
            // move1ComboBox
            // 
            this.move1ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.move1ComboBox.FormattingEnabled = true;
            this.move1ComboBox.Location = new System.Drawing.Point(8, 42);
            this.move1ComboBox.Name = "move1ComboBox";
            this.move1ComboBox.Size = new System.Drawing.Size(144, 21);
            this.move1ComboBox.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(247, 263);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // itemLabel
            // 
            this.itemLabel.AutoSize = true;
            this.itemLabel.Location = new System.Drawing.Point(10, 63);
            this.itemLabel.Name = "itemLabel";
            this.itemLabel.Size = new System.Drawing.Size(27, 13);
            this.itemLabel.TabIndex = 11;
            this.itemLabel.Text = "Item";
            // 
            // itemComboBox
            // 
            this.itemComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.itemComboBox.FormattingEnabled = true;
            this.itemComboBox.Location = new System.Drawing.Point(13, 80);
            this.itemComboBox.Name = "itemComboBox";
            this.itemComboBox.Size = new System.Drawing.Size(150, 21);
            this.itemComboBox.TabIndex = 12;
            // 
            // GivePokémonDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 292);
            this.Controls.Add(this.itemComboBox);
            this.Controls.Add(this.itemLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.movesetGroupBox);
            this.Controls.Add(this.movesetCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.levelNumericUpDown);
            this.Controls.Add(this.speciesComboBox);
            this.Controls.Add(this.label1);
            this.Name = "GivePokémonDialog";
            this.Text = "Give Pokémon";
            ((System.ComponentModel.ISupportInitialize)(this.levelNumericUpDown)).EndInit();
            this.movesetGroupBox.ResumeLayout(false);
            this.movesetGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox speciesComboBox;
        private System.Windows.Forms.NumericUpDown levelNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox movesetCheckBox;
        private System.Windows.Forms.GroupBox movesetGroupBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox move4ComboBox;
        private System.Windows.Forms.ComboBox move3ComboBox;
        private System.Windows.Forms.ComboBox move2ComboBox;
        private System.Windows.Forms.ComboBox move1ComboBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label itemLabel;
        private System.Windows.Forms.ComboBox itemComboBox;
    }
}