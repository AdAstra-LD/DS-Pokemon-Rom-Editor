namespace DSPRE.Editors
{
    partial class ItemEditor
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
            this.itemNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.holdEffectComboBox = new System.Windows.Forms.ComboBox();
            this.fieldPocketComboBox = new System.Windows.Forms.ComboBox();
            this.battlePocketComboBox = new System.Windows.Forms.ComboBox();
            this.itemNameInputComboBox = new System.Windows.Forms.ComboBox();
            this.priceNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.holdEffectLabel = new System.Windows.Forms.Label();
            this.fieldPocketLabel = new System.Windows.Forms.Label();
            this.battlePocketLabel = new System.Windows.Forms.Label();
            this.priceLabel = new System.Windows.Forms.Label();
            this.saveDataButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.itemNumberNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.priceNumericUpDown)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // itemNumberNumericUpDown
            // 
            this.itemNumberNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.itemNumberNumericUpDown.Location = new System.Drawing.Point(425, 43);
            this.itemNumberNumericUpDown.Name = "itemNumberNumericUpDown";
            this.itemNumberNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.itemNumberNumericUpDown.TabIndex = 0;
            this.itemNumberNumericUpDown.ValueChanged += new System.EventHandler(this.itemNumberNumericUpDown_ValueChanged);
            // 
            // holdEffectComboBox
            // 
            this.holdEffectComboBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.holdEffectComboBox.FormattingEnabled = true;
            this.holdEffectComboBox.Location = new System.Drawing.Point(36, 254);
            this.holdEffectComboBox.Name = "holdEffectComboBox";
            this.holdEffectComboBox.Size = new System.Drawing.Size(121, 21);
            this.holdEffectComboBox.TabIndex = 1;
            this.holdEffectComboBox.SelectedIndexChanged += new System.EventHandler(this.holdEffectComboBox_SelectedIndexChanged);
            // 
            // fieldPocketComboBox
            // 
            this.fieldPocketComboBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.fieldPocketComboBox.FormattingEnabled = true;
            this.fieldPocketComboBox.Location = new System.Drawing.Point(230, 254);
            this.fieldPocketComboBox.Name = "fieldPocketComboBox";
            this.fieldPocketComboBox.Size = new System.Drawing.Size(121, 21);
            this.fieldPocketComboBox.TabIndex = 2;
            this.fieldPocketComboBox.SelectedIndexChanged += new System.EventHandler(this.fieldPocketComboBox_SelectedIndexChanged);
            // 
            // battlePocketComboBox
            // 
            this.battlePocketComboBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.battlePocketComboBox.FormattingEnabled = true;
            this.battlePocketComboBox.Location = new System.Drawing.Point(424, 254);
            this.battlePocketComboBox.Name = "battlePocketComboBox";
            this.battlePocketComboBox.Size = new System.Drawing.Size(121, 21);
            this.battlePocketComboBox.TabIndex = 3;
            this.battlePocketComboBox.SelectedIndexChanged += new System.EventHandler(this.battlePocketComboBox_SelectedIndexChanged);
            // 
            // itemNameInputComboBox
            // 
            this.itemNameInputComboBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.itemNameInputComboBox.FormattingEnabled = true;
            this.itemNameInputComboBox.Location = new System.Drawing.Point(230, 42);
            this.itemNameInputComboBox.Name = "itemNameInputComboBox";
            this.itemNameInputComboBox.Size = new System.Drawing.Size(121, 21);
            this.itemNameInputComboBox.TabIndex = 4;
            this.itemNameInputComboBox.SelectedIndexChanged += new System.EventHandler(this.itemNameInputComboBox_SelectedIndexChanged);
            // 
            // priceNumericUpDown
            // 
            this.priceNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.priceNumericUpDown.Location = new System.Drawing.Point(619, 255);
            this.priceNumericUpDown.Name = "priceNumericUpDown";
            this.priceNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.priceNumericUpDown.TabIndex = 5;
            this.priceNumericUpDown.ValueChanged += new System.EventHandler(this.priceNumericUpDown_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.holdEffectLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.holdEffectComboBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.fieldPocketComboBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.battlePocketComboBox, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.priceNumericUpDown, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.itemNumberNumericUpDown, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.itemNameInputComboBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.fieldPocketLabel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.battlePocketLabel, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.priceLabel, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.saveDataButton, 3, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(776, 426);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // holdEffectLabel
            // 
            this.holdEffectLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.holdEffectLabel.AutoSize = true;
            this.holdEffectLabel.Location = new System.Drawing.Point(67, 199);
            this.holdEffectLabel.Name = "holdEffectLabel";
            this.holdEffectLabel.Size = new System.Drawing.Size(60, 13);
            this.holdEffectLabel.TabIndex = 6;
            this.holdEffectLabel.Text = "Hold Effect";
            this.holdEffectLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fieldPocketLabel
            // 
            this.fieldPocketLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.fieldPocketLabel.AutoSize = true;
            this.fieldPocketLabel.Location = new System.Drawing.Point(258, 199);
            this.fieldPocketLabel.Name = "fieldPocketLabel";
            this.fieldPocketLabel.Size = new System.Drawing.Size(66, 13);
            this.fieldPocketLabel.TabIndex = 7;
            this.fieldPocketLabel.Text = "Field Pocket";
            this.fieldPocketLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // battlePocketLabel
            // 
            this.battlePocketLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.battlePocketLabel.AutoSize = true;
            this.battlePocketLabel.Location = new System.Drawing.Point(449, 199);
            this.battlePocketLabel.Name = "battlePocketLabel";
            this.battlePocketLabel.Size = new System.Drawing.Size(71, 13);
            this.battlePocketLabel.TabIndex = 8;
            this.battlePocketLabel.Text = "Battle Pocket";
            this.battlePocketLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // priceLabel
            // 
            this.priceLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.priceLabel.AutoSize = true;
            this.priceLabel.Location = new System.Drawing.Point(663, 199);
            this.priceLabel.Name = "priceLabel";
            this.priceLabel.Size = new System.Drawing.Size(31, 13);
            this.priceLabel.TabIndex = 9;
            this.priceLabel.Text = "Price";
            this.priceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // saveDataButton
            // 
            this.saveDataButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.saveDataButton.Location = new System.Drawing.Point(631, 41);
            this.saveDataButton.Name = "saveDataButton";
            this.saveDataButton.Size = new System.Drawing.Size(96, 23);
            this.saveDataButton.TabIndex = 10;
            this.saveDataButton.Text = "Save Changes";
            this.saveDataButton.UseVisualStyleBackColor = true;
            this.saveDataButton.Click += new System.EventHandler(this.saveDataButton_Click);
            // 
            // ItemEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ItemEditor";
            this.Text = "ItemEditor";
            ((System.ComponentModel.ISupportInitialize)(this.itemNumberNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.priceNumericUpDown)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown itemNumberNumericUpDown;
        private System.Windows.Forms.ComboBox holdEffectComboBox;
        private System.Windows.Forms.ComboBox fieldPocketComboBox;
        private System.Windows.Forms.ComboBox battlePocketComboBox;
        private System.Windows.Forms.ComboBox itemNameInputComboBox;
        private System.Windows.Forms.NumericUpDown priceNumericUpDown;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label holdEffectLabel;
        private System.Windows.Forms.Label fieldPocketLabel;
        private System.Windows.Forms.Label battlePocketLabel;
        private System.Windows.Forms.Label priceLabel;
        private System.Windows.Forms.Button saveDataButton;
    }
}