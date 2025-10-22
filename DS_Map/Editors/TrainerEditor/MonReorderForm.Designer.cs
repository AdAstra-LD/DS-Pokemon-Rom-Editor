namespace DSPRE.Editors
{
    partial class MonReorderForm
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
            this.monListBox = new System.Windows.Forms.ListBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.moveUpButton = new System.Windows.Forms.Button();
            this.moveDownButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // monListBox
            // 
            this.monListBox.AllowDrop = true;
            this.monListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monListBox.FormattingEnabled = true;
            this.monListBox.ItemHeight = 20;
            this.monListBox.Location = new System.Drawing.Point(12, 12);
            this.monListBox.Name = "monListBox";
            this.monListBox.Size = new System.Drawing.Size(209, 184);
            this.monListBox.TabIndex = 0;
            this.monListBox.SelectedIndexChanged += new System.EventHandler(this.monListBox_SelectedIndexChanged);
            this.monListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.monListBox_DragDrop);
            this.monListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.monListBox_DragEnter);
            this.monListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.monListBox_MouseDown);
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.Location = new System.Drawing.Point(227, 166);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 30);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // moveUpButton
            // 
            this.moveUpButton.Image = global::DSPRE.Properties.Resources.arrowup;
            this.moveUpButton.Location = new System.Drawing.Point(227, 12);
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Size = new System.Drawing.Size(75, 23);
            this.moveUpButton.TabIndex = 2;
            this.moveUpButton.UseVisualStyleBackColor = true;
            this.moveUpButton.Click += new System.EventHandler(this.moveUpButton_Click);
            // 
            // moveDownButton
            // 
            this.moveDownButton.Image = global::DSPRE.Properties.Resources.arrowdown;
            this.moveDownButton.Location = new System.Drawing.Point(227, 41);
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.Size = new System.Drawing.Size(75, 23);
            this.moveDownButton.TabIndex = 3;
            this.moveDownButton.UseVisualStyleBackColor = true;
            this.moveDownButton.Click += new System.EventHandler(this.moveDownButton_Click);
            // 
            // MonReorderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 206);
            this.Controls.Add(this.moveDownButton);
            this.Controls.Add(this.moveUpButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.monListBox);
            this.MinimumSize = new System.Drawing.Size(325, 245);
            this.Name = "MonReorderForm";
            this.Text = "Reorder Party";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MonReorderForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox monListBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button moveUpButton;
        private System.Windows.Forms.Button moveDownButton;
    }
}