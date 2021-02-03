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
            this.value1TextBox = new System.Windows.Forms.TextBox();
            this.startSearchButton = new System.Windows.Forms.Button();
            this.headerSearchResetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // arm9expansionTextLBL
            // 
            this.arm9expansionTextLBL.Location = new System.Drawing.Point(0, 0);
            this.arm9expansionTextLBL.Name = "arm9expansionTextLBL";
            this.arm9expansionTextLBL.Size = new System.Drawing.Size(100, 23);
            this.arm9expansionTextLBL.TabIndex = 15;
            // 
            // fieldToSearch1ComboBox
            // 
            this.fieldToSearch1ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fieldToSearch1ComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fieldToSearch1ComboBox.FormattingEnabled = true;
            this.fieldToSearch1ComboBox.Location = new System.Drawing.Point(11, 37);
            this.fieldToSearch1ComboBox.Name = "fieldToSearch1ComboBox";
            this.fieldToSearch1ComboBox.Size = new System.Drawing.Size(190, 24);
            this.fieldToSearch1ComboBox.TabIndex = 6;
            this.fieldToSearch1ComboBox.SelectedIndexChanged += new System.EventHandler(this.fieldToSearch1ComboBox_SelectedIndexChanged);
            // 
            // explanationLabel
            // 
            this.explanationLabel.AutoSize = true;
            this.explanationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.explanationLabel.Location = new System.Drawing.Point(6, 10);
            this.explanationLabel.Name = "explanationLabel";
            this.explanationLabel.Size = new System.Drawing.Size(269, 15);
            this.explanationLabel.TabIndex = 7;
            this.explanationLabel.Text = "Find all headers that meet this condition:";
            // 
            // operator1ComboBox
            // 
            this.operator1ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.operator1ComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.operator1ComboBox.FormattingEnabled = true;
            this.operator1ComboBox.Location = new System.Drawing.Point(11, 72);
            this.operator1ComboBox.Name = "operator1ComboBox";
            this.operator1ComboBox.Size = new System.Drawing.Size(190, 24);
            this.operator1ComboBox.TabIndex = 8;
            // 
            // value1TextBox
            // 
            this.value1TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.value1TextBox.Location = new System.Drawing.Point(11, 108);
            this.value1TextBox.MaxLength = 16;
            this.value1TextBox.Name = "value1TextBox";
            this.value1TextBox.Size = new System.Drawing.Size(190, 22);
            this.value1TextBox.TabIndex = 9;
            this.value1TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.value1TextBox_KeyDown);
            // 
            // startSearchButton
            // 
            this.startSearchButton.Image = global::DSPRE.Properties.Resources.wideLensImage;
            this.startSearchButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.startSearchButton.Location = new System.Drawing.Point(209, 37);
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
            this.headerSearchResetButton.Location = new System.Drawing.Point(209, 87);
            this.headerSearchResetButton.Name = "headerSearchResetButton";
            this.headerSearchResetButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerSearchResetButton.Size = new System.Drawing.Size(86, 43);
            this.headerSearchResetButton.TabIndex = 17;
            this.headerSearchResetButton.Text = "Reset\r\nResults";
            this.headerSearchResetButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.headerSearchResetButton.UseVisualStyleBackColor = true;
            this.headerSearchResetButton.Click += new System.EventHandler(this.headerSearchResetButton_Click);
            // 
            // HeaderSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(304, 145);
            this.Controls.Add(this.headerSearchResetButton);
            this.Controls.Add(this.startSearchButton);
            this.Controls.Add(this.value1TextBox);
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
        private System.Windows.Forms.TextBox value1TextBox;
        private System.Windows.Forms.Button startSearchButton;
        private System.Windows.Forms.Button headerSearchResetButton;
    }
}