namespace DSPRE {
    partial class HeaderSearch {
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
            this.arm9expansionTextLBL = new System.Windows.Forms.Label();
            this.fieldToSearch1ComboBox = new System.Windows.Forms.ComboBox();
            this.explanationLabel = new System.Windows.Forms.Label();
            this.operator1ComboBox = new System.Windows.Forms.ComboBox();
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.startSearchButton = new System.Windows.Forms.Button();
            this.headerSearchResetButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.autoSearchCB = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // arm9expansionTextLBL
            // 
            this.arm9expansionTextLBL.Location = new System.Drawing.Point(0, -10);
            this.arm9expansionTextLBL.Name = "arm9expansionTextLBL";
            this.arm9expansionTextLBL.Size = new System.Drawing.Size(100, 23);
            this.arm9expansionTextLBL.TabIndex = 15;
            // 
            // fieldToSearch1ComboBox
            // 
            this.fieldToSearch1ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fieldToSearch1ComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fieldToSearch1ComboBox.FormattingEnabled = true;
            this.fieldToSearch1ComboBox.Location = new System.Drawing.Point(11, 27);
            this.fieldToSearch1ComboBox.Name = "fieldToSearch1ComboBox";
            this.fieldToSearch1ComboBox.Size = new System.Drawing.Size(190, 24);
            this.fieldToSearch1ComboBox.TabIndex = 6;
            this.fieldToSearch1ComboBox.SelectedIndexChanged += new System.EventHandler(this.fieldToSearch1ComboBox_SelectedIndexChanged);
            // 
            // explanationLabel
            // 
            this.explanationLabel.AutoSize = true;
            this.explanationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.explanationLabel.Location = new System.Drawing.Point(8, 9);
            this.explanationLabel.Name = "explanationLabel";
            this.explanationLabel.Size = new System.Drawing.Size(111, 15);
            this.explanationLabel.TabIndex = 7;
            this.explanationLabel.Text = "Header Property";
            // 
            // operator1ComboBox
            // 
            this.operator1ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.operator1ComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.operator1ComboBox.FormattingEnabled = true;
            this.operator1ComboBox.Location = new System.Drawing.Point(11, 82);
            this.operator1ComboBox.Name = "operator1ComboBox";
            this.operator1ComboBox.Size = new System.Drawing.Size(190, 24);
            this.operator1ComboBox.TabIndex = 8;
            this.operator1ComboBox.SelectedIndexChanged += new System.EventHandler(this.operator1ComboBox_SelectedIndexChanged);
            // 
            // valueTextBox
            // 
            this.valueTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valueTextBox.Location = new System.Drawing.Point(11, 110);
            this.valueTextBox.MaxLength = 16;
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.Size = new System.Drawing.Size(190, 22);
            this.valueTextBox.TabIndex = 9;
            this.valueTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.valueTextBox_KeyUp);
            // 
            // startSearchButton
            // 
            this.startSearchButton.Image = global::DSPRE.Properties.Resources.wideLensImage;
            this.startSearchButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.startSearchButton.Location = new System.Drawing.Point(209, 11);
            this.startSearchButton.Name = "startSearchButton";
            this.startSearchButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.startSearchButton.Size = new System.Drawing.Size(86, 43);
            this.startSearchButton.TabIndex = 16;
            this.startSearchButton.Text = "Start\r\nSearch";
            this.startSearchButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.startSearchButton.UseVisualStyleBackColor = true;
            this.startSearchButton.Click += new System.EventHandler(this.startSearchButton_Click);
            // 
            // headerSearchResetButton
            // 
            this.headerSearchResetButton.Image = global::DSPRE.Properties.Resources.wideLensImageTransp;
            this.headerSearchResetButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.headerSearchResetButton.Location = new System.Drawing.Point(209, 60);
            this.headerSearchResetButton.Name = "headerSearchResetButton";
            this.headerSearchResetButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerSearchResetButton.Size = new System.Drawing.Size(86, 43);
            this.headerSearchResetButton.TabIndex = 17;
            this.headerSearchResetButton.Text = "Reset\r\nResults";
            this.headerSearchResetButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.headerSearchResetButton.UseVisualStyleBackColor = true;
            this.headerSearchResetButton.Click += new System.EventHandler(this.headerSearchResetButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 15);
            this.label1.TabIndex = 18;
            this.label1.Text = "Criteria";
            // 
            // autoSearchCB
            // 
            this.autoSearchCB.AutoSize = true;
            this.autoSearchCB.Checked = true;
            this.autoSearchCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoSearchCB.Location = new System.Drawing.Point(212, 113);
            this.autoSearchCB.Name = "autoSearchCB";
            this.autoSearchCB.Size = new System.Drawing.Size(82, 17);
            this.autoSearchCB.TabIndex = 19;
            this.autoSearchCB.Text = "AutoSearch";
            this.autoSearchCB.UseVisualStyleBackColor = true;
            // 
            // HeaderSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(304, 144);
            this.Controls.Add(this.autoSearchCB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.headerSearchResetButton);
            this.Controls.Add(this.startSearchButton);
            this.Controls.Add(this.valueTextBox);
            this.Controls.Add(this.operator1ComboBox);
            this.Controls.Add(this.explanationLabel);
            this.Controls.Add(this.fieldToSearch1ComboBox);
            this.Controls.Add(this.arm9expansionTextLBL);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HeaderSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced Header Search (Experimental)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label arm9expansionTextLBL;
        private System.Windows.Forms.ComboBox fieldToSearch1ComboBox;
        private System.Windows.Forms.Label explanationLabel;
        private System.Windows.Forms.ComboBox operator1ComboBox;
        private System.Windows.Forms.TextBox valueTextBox;
        private System.Windows.Forms.Button startSearchButton;
        private System.Windows.Forms.Button headerSearchResetButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox autoSearchCB;
    }
}