namespace DSPRE.Editors.BtxEditor
{
    partial class BtxEditor
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
            this.overworldList = new System.Windows.Forms.ComboBox();
            this.overworldPictureBox = new System.Windows.Forms.PictureBox();
            this.showBtxFileButton = new System.Windows.Forms.Button();
            this.exportImagePng = new System.Windows.Forms.Button();
            this.importImagePng = new System.Windows.Forms.Button();
            this.shinyCheckbox = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.saveSelected_Button = new System.Windows.Forms.Button();
            this.SaveAll_Button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.overworldPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Overworld";
            // 
            // overworldList
            // 
            this.overworldList.FormattingEnabled = true;
            this.overworldList.Location = new System.Drawing.Point(12, 29);
            this.overworldList.Name = "overworldList";
            this.overworldList.Size = new System.Drawing.Size(125, 21);
            this.overworldList.TabIndex = 1;
            this.overworldList.SelectedIndexChanged += new System.EventHandler(this.overworldList_SelectedIndexChanged);
            // 
            // overworldPictureBox
            // 
            this.overworldPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.overworldPictureBox.Location = new System.Drawing.Point(3, 0);
            this.overworldPictureBox.Name = "overworldPictureBox";
            this.overworldPictureBox.Size = new System.Drawing.Size(117, 209);
            this.overworldPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.overworldPictureBox.TabIndex = 2;
            this.overworldPictureBox.TabStop = false;
            // 
            // showBtxFileButton
            // 
            this.showBtxFileButton.Enabled = false;
            this.showBtxFileButton.Image = global::DSPRE.Properties.Resources.lens;
            this.showBtxFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.showBtxFileButton.Location = new System.Drawing.Point(148, 27);
            this.showBtxFileButton.Name = "showBtxFileButton";
            this.showBtxFileButton.Size = new System.Drawing.Size(121, 23);
            this.showBtxFileButton.TabIndex = 3;
            this.showBtxFileButton.Text = "Show BTX File";
            this.showBtxFileButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.showBtxFileButton.UseVisualStyleBackColor = true;
            this.showBtxFileButton.Click += new System.EventHandler(this.showBtxFileButton_Click);
            // 
            // exportImagePng
            // 
            this.exportImagePng.Enabled = false;
            this.exportImagePng.Image = global::DSPRE.Properties.Resources.exportArrow;
            this.exportImagePng.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.exportImagePng.Location = new System.Drawing.Point(148, 145);
            this.exportImagePng.Name = "exportImagePng";
            this.exportImagePng.Size = new System.Drawing.Size(121, 23);
            this.exportImagePng.TabIndex = 4;
            this.exportImagePng.Text = "Export PNG";
            this.exportImagePng.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.exportImagePng.UseVisualStyleBackColor = true;
            this.exportImagePng.Click += new System.EventHandler(this.exportImagePng_Click);
            // 
            // importImagePng
            // 
            this.importImagePng.Enabled = false;
            this.importImagePng.Image = global::DSPRE.Properties.Resources.importArrow;
            this.importImagePng.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.importImagePng.Location = new System.Drawing.Point(16, 145);
            this.importImagePng.Name = "importImagePng";
            this.importImagePng.Size = new System.Drawing.Size(121, 23);
            this.importImagePng.TabIndex = 5;
            this.importImagePng.Text = "Import PNG";
            this.importImagePng.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.importImagePng.UseVisualStyleBackColor = true;
            this.importImagePng.Click += new System.EventHandler(this.importImagePng_Click);
            // 
            // shinyCheckbox
            // 
            this.shinyCheckbox.AutoSize = true;
            this.shinyCheckbox.Enabled = false;
            this.shinyCheckbox.Location = new System.Drawing.Point(13, 57);
            this.shinyCheckbox.Name = "shinyCheckbox";
            this.shinyCheckbox.Size = new System.Drawing.Size(52, 17);
            this.shinyCheckbox.TabIndex = 6;
            this.shinyCheckbox.Text = "Shiny";
            this.shinyCheckbox.UseVisualStyleBackColor = true;
            this.shinyCheckbox.CheckedChanged += new System.EventHandler(this.shinyCheckbox_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.overworldPictureBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(283, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(123, 209);
            this.panel1.TabIndex = 7;
            // 
            // saveSelected_Button
            // 
            this.saveSelected_Button.Image = global::DSPRE.Properties.Resources.saveButton;
            this.saveSelected_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.saveSelected_Button.Location = new System.Drawing.Point(16, 174);
            this.saveSelected_Button.Name = "saveSelected_Button";
            this.saveSelected_Button.Size = new System.Drawing.Size(121, 23);
            this.saveSelected_Button.TabIndex = 8;
            this.saveSelected_Button.Text = "Save Selected";
            this.saveSelected_Button.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.saveSelected_Button.UseVisualStyleBackColor = true;
            this.saveSelected_Button.Click += new System.EventHandler(this.saveSelected_Button_Click);
            // 
            // SaveAll_Button
            // 
            this.SaveAll_Button.Image = global::DSPRE.Properties.Resources.saveButton;
            this.SaveAll_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SaveAll_Button.Location = new System.Drawing.Point(148, 174);
            this.SaveAll_Button.Name = "SaveAll_Button";
            this.SaveAll_Button.Size = new System.Drawing.Size(121, 23);
            this.SaveAll_Button.TabIndex = 9;
            this.SaveAll_Button.Text = "Save All";
            this.SaveAll_Button.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.SaveAll_Button.UseVisualStyleBackColor = true;
            this.SaveAll_Button.Click += new System.EventHandler(this.SaveAll_Button_Click);
            // 
            // BtxEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 209);
            this.Controls.Add(this.SaveAll_Button);
            this.Controls.Add(this.saveSelected_Button);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.shinyCheckbox);
            this.Controls.Add(this.importImagePng);
            this.Controls.Add(this.exportImagePng);
            this.Controls.Add(this.showBtxFileButton);
            this.Controls.Add(this.overworldList);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(422, 248);
            this.MinimumSize = new System.Drawing.Size(422, 248);
            this.Name = "BtxEditor";
            this.ShowIcon = false;
            this.Text = "Overworld (BTX) Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BtxEditor_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.overworldPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox overworldList;
        private System.Windows.Forms.PictureBox overworldPictureBox;
        private System.Windows.Forms.Button showBtxFileButton;
        private System.Windows.Forms.Button exportImagePng;
        private System.Windows.Forms.Button importImagePng;
        private System.Windows.Forms.CheckBox shinyCheckbox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button saveSelected_Button;
        private System.Windows.Forms.Button SaveAll_Button;
    }
}