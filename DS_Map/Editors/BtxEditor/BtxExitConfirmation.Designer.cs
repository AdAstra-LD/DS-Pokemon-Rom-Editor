namespace DSPRE.Editors.BtxEditor
{
    partial class BtxExitConfirmation
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
            this.exitWithoutSavingButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.unsavedFileCountLabel = new System.Windows.Forms.Label();
            this.modifiedOverworldsDetails = new System.Windows.Forms.ListBox();
            this.beforeSpriteBox = new System.Windows.Forms.PictureBox();
            this.afterSpriteBox = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.beforeSpriteBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.afterSpriteBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // exitWithoutSavingButton
            // 
            this.exitWithoutSavingButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.exitWithoutSavingButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.exitWithoutSavingButton.Location = new System.Drawing.Point(12, 165);
            this.exitWithoutSavingButton.Name = "exitWithoutSavingButton";
            this.exitWithoutSavingButton.Size = new System.Drawing.Size(112, 43);
            this.exitWithoutSavingButton.TabIndex = 0;
            this.exitWithoutSavingButton.Text = "Exit Without Saving";
            this.exitWithoutSavingButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.cancelButton.Location = new System.Drawing.Point(141, 165);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(112, 43);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // unsavedFileCountLabel
            // 
            this.unsavedFileCountLabel.AutoSize = true;
            this.unsavedFileCountLabel.Location = new System.Drawing.Point(12, 9);
            this.unsavedFileCountLabel.Name = "unsavedFileCountLabel";
            this.unsavedFileCountLabel.Size = new System.Drawing.Size(35, 13);
            this.unsavedFileCountLabel.TabIndex = 2;
            this.unsavedFileCountLabel.Text = "label1";
            // 
            // modifiedOverworldsDetails
            // 
            this.modifiedOverworldsDetails.FormattingEnabled = true;
            this.modifiedOverworldsDetails.Location = new System.Drawing.Point(12, 25);
            this.modifiedOverworldsDetails.Name = "modifiedOverworldsDetails";
            this.modifiedOverworldsDetails.Size = new System.Drawing.Size(241, 134);
            this.modifiedOverworldsDetails.TabIndex = 3;
            this.modifiedOverworldsDetails.SelectedIndexChanged += new System.EventHandler(this.modifiedOverworldsDetails_SelectedIndexChanged);
            // 
            // beforeSpriteBox
            // 
            this.beforeSpriteBox.Location = new System.Drawing.Point(0, 0);
            this.beforeSpriteBox.Name = "beforeSpriteBox";
            this.beforeSpriteBox.Size = new System.Drawing.Size(48, 190);
            this.beforeSpriteBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.beforeSpriteBox.TabIndex = 4;
            this.beforeSpriteBox.TabStop = false;
            // 
            // afterSpriteBox
            // 
            this.afterSpriteBox.Location = new System.Drawing.Point(0, 0);
            this.afterSpriteBox.Name = "afterSpriteBox";
            this.afterSpriteBox.Size = new System.Drawing.Size(48, 190);
            this.afterSpriteBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.afterSpriteBox.TabIndex = 5;
            this.afterSpriteBox.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackgroundImage = global::DSPRE.Properties.Resources.arrowright;
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox4.InitialImage = global::DSPRE.Properties.Resources.arrowright;
            this.pictureBox4.Location = new System.Drawing.Point(327, 98);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(29, 28);
            this.pictureBox4.TabIndex = 6;
            this.pictureBox4.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.beforeSpriteBox);
            this.panel1.Location = new System.Drawing.Point(272, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(51, 196);
            this.panel1.TabIndex = 7;
            this.panel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.panel1_Scroll);
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.afterSpriteBox);
            this.panel2.Location = new System.Drawing.Point(362, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(51, 196);
            this.panel2.TabIndex = 8;
            this.panel2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.panel2_Scroll);
            // 
            // BtxExitConfirmation
            // 
            this.AcceptButton = this.exitWithoutSavingButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(425, 243);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.modifiedOverworldsDetails);
            this.Controls.Add(this.unsavedFileCountLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.exitWithoutSavingButton);
            this.MaximumSize = new System.Drawing.Size(441, 282);
            this.MinimumSize = new System.Drawing.Size(441, 259);
            this.Name = "BtxExitConfirmation";
            this.ShowIcon = false;
            this.Text = "Unsaved modifications";
            ((System.ComponentModel.ISupportInitialize)(this.beforeSpriteBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.afterSpriteBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button exitWithoutSavingButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label unsavedFileCountLabel;
        private System.Windows.Forms.ListBox modifiedOverworldsDetails;
        private System.Windows.Forms.PictureBox beforeSpriteBox;
        private System.Windows.Forms.PictureBox afterSpriteBox;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}