namespace DSPRE
{
    partial class ROMToolboxDialog
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
        private void InitializeComponent() { 
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ROMToolboxDialog));
            this.applyItemStandardizeButton = new System.Windows.Forms.Button();
            this.arm9expansionTextLBL = new System.Windows.Forms.Label();
            this.arm9expansionLBL = new System.Windows.Forms.Label();
            this.applyARM9ExpansionButton = new System.Windows.Forms.Button();
            this.BDHCAMpatchTextLBL = new System.Windows.Forms.Label();
            this.BDHCAMpatchLBL = new System.Windows.Forms.Label();
            this.BDHCAMpatchButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.namesToSentenceCaseButton = new System.Windows.Forms.Button();
            this.overlay1uncompressedLBL = new System.Windows.Forms.Label();
            this.overlay1patchtextLBL = new System.Windows.Forms.Label();
            this.overlay1uncomprButton = new System.Windows.Forms.Button();
            this.bdhcamARM9requiredLBL = new System.Windows.Forms.Label();
            this.arm9patchCB = new System.Windows.Forms.PictureBox();
            this.overlay1CB = new System.Windows.Forms.PictureBox();
            this.bdhcamCB = new System.Windows.Forms.PictureBox();
            this.sentenceCaseCB = new System.Windows.Forms.PictureBox();
            this.itemNumbersCB = new System.Windows.Forms.PictureBox();
            this.StandardizePatchLBL = new System.Windows.Forms.Label();
            this.StandardizePatchTextLBL = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.arm9patchCB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overlay1CB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdhcamCB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sentenceCaseCB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemNumbersCB)).BeginInit();
            this.SuspendLayout();
            // 
            // applyItemStandardizeButton
            // 
            this.applyItemStandardizeButton.Location = new System.Drawing.Point(335, 346);
            this.applyItemStandardizeButton.Name = "applyItemStandardizeButton";
            this.applyItemStandardizeButton.Size = new System.Drawing.Size(100, 50);
            this.applyItemStandardizeButton.TabIndex = 0;
            this.applyItemStandardizeButton.Text = "Apply Patch";
            this.applyItemStandardizeButton.UseVisualStyleBackColor = true;
            this.applyItemStandardizeButton.Click += new System.EventHandler(this.ApplyItemStandardizeButton_Click);
            // 
            // arm9expansionTextLBL
            // 
            this.arm9expansionTextLBL.Location = new System.Drawing.Point(15, 34);
            this.arm9expansionTextLBL.Name = "arm9expansionTextLBL";
            this.arm9expansionTextLBL.Size = new System.Drawing.Size(288, 58);
            this.arm9expansionTextLBL.TabIndex = 5;
            this.arm9expansionTextLBL.Text = resources.GetString("arm9expansionTextLBL.Text");
            this.arm9expansionTextLBL.UseMnemonic = false;
            // 
            // arm9expansionLBL
            // 
            this.arm9expansionLBL.AutoSize = true;
            this.arm9expansionLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arm9expansionLBL.Location = new System.Drawing.Point(15, 19);
            this.arm9expansionLBL.Name = "arm9expansionLBL";
            this.arm9expansionLBL.Size = new System.Drawing.Size(103, 13);
            this.arm9expansionLBL.TabIndex = 4;
            this.arm9expansionLBL.Text = "ARM9 Expansion";
            // 
            // applyARM9ExpansionButton
            // 
            this.applyARM9ExpansionButton.Location = new System.Drawing.Point(335, 29);
            this.applyARM9ExpansionButton.Name = "applyARM9ExpansionButton";
            this.applyARM9ExpansionButton.Size = new System.Drawing.Size(100, 50);
            this.applyARM9ExpansionButton.TabIndex = 3;
            this.applyARM9ExpansionButton.Text = "Expand ARM9";
            this.applyARM9ExpansionButton.UseVisualStyleBackColor = true;
            this.applyARM9ExpansionButton.Click += new System.EventHandler(this.ApplyARM9ExpansionButton_Click);
            // 
            // BDHCAMpatchTextLBL
            // 
            this.BDHCAMpatchTextLBL.Enabled = false;
            this.BDHCAMpatchTextLBL.Location = new System.Drawing.Point(15, 184);
            this.BDHCAMpatchTextLBL.Name = "BDHCAMpatchTextLBL";
            this.BDHCAMpatchTextLBL.Size = new System.Drawing.Size(293, 67);
            this.BDHCAMpatchTextLBL.TabIndex = 8;
            this.BDHCAMpatchTextLBL.Text = "Adds Dynamic BDHC Cameras to current ROM.\r\nWith this patch, you have more control" +
    " over \r\nthe game camera\'s rotation and position.\r\nYou will need Trifindo\'s PDSMS" +
    "\r\nin order to make BDHCAM Files.\r\n";
            this.BDHCAMpatchTextLBL.UseMnemonic = false;
            // 
            // BDHCAMpatchLBL
            // 
            this.BDHCAMpatchLBL.AutoSize = true;
            this.BDHCAMpatchLBL.Enabled = false;
            this.BDHCAMpatchLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BDHCAMpatchLBL.Location = new System.Drawing.Point(14, 168);
            this.BDHCAMpatchLBL.Name = "BDHCAMpatchLBL";
            this.BDHCAMpatchLBL.Size = new System.Drawing.Size(107, 13);
            this.BDHCAMpatchLBL.TabIndex = 7;
            this.BDHCAMpatchLBL.Text = "Dynamic Cameras";
            // 
            // BDHCAMpatchButton
            // 
            this.BDHCAMpatchButton.Enabled = false;
            this.BDHCAMpatchButton.Location = new System.Drawing.Point(335, 183);
            this.BDHCAMpatchButton.Name = "BDHCAMpatchButton";
            this.BDHCAMpatchButton.Size = new System.Drawing.Size(100, 50);
            this.BDHCAMpatchButton.TabIndex = 6;
            this.BDHCAMpatchButton.Text = "Apply Patch";
            this.BDHCAMpatchButton.UseVisualStyleBackColor = true;
            this.BDHCAMpatchButton.Click += new System.EventHandler(this.BDHCAMPatchButton_Click);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(15, 283);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(293, 49);
            this.label7.TabIndex = 11;
            this.label7.Text = "BULBASAUR, IVYSAUR, VENUSAUR...\r\nbecome\r\nBulbasaur, Ivysaur, Venusaur...\r\n";
            this.label7.UseMnemonic = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(15, 268);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(252, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Convert Pokémon names to Sentence Case";
            // 
            // namesToSentenceCaseButton
            // 
            this.namesToSentenceCaseButton.Location = new System.Drawing.Point(335, 271);
            this.namesToSentenceCaseButton.Name = "namesToSentenceCaseButton";
            this.namesToSentenceCaseButton.Size = new System.Drawing.Size(100, 50);
            this.namesToSentenceCaseButton.TabIndex = 9;
            this.namesToSentenceCaseButton.Text = "Apply Patch";
            this.namesToSentenceCaseButton.UseVisualStyleBackColor = true;
            this.namesToSentenceCaseButton.Click += new System.EventHandler(this.SentenceCasePatchButton_Click);
            // 
            // overlay1uncompressedLBL
            // 
            this.overlay1uncompressedLBL.AutoSize = true;
            this.overlay1uncompressedLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overlay1uncompressedLBL.Location = new System.Drawing.Point(15, 105);
            this.overlay1uncompressedLBL.Name = "overlay1uncompressedLBL";
            this.overlay1uncompressedLBL.Size = new System.Drawing.Size(217, 13);
            this.overlay1uncompressedLBL.TabIndex = 12;
            this.overlay1uncompressedLBL.Text = "Configure Overlay1 as uncompressed";
            // 
            // overlay1patchtextLBL
            // 
            this.overlay1patchtextLBL.Location = new System.Drawing.Point(15, 120);
            this.overlay1patchtextLBL.Name = "overlay1patchtextLBL";
            this.overlay1patchtextLBL.Size = new System.Drawing.Size(293, 29);
            this.overlay1patchtextLBL.TabIndex = 13;
            this.overlay1patchtextLBL.Text = "Overlay1 won\'t have to be compressed again.\r\nThe operation is reversible.\r\n";
            this.overlay1patchtextLBL.UseMnemonic = false;
            // 
            // overlay1uncomprButton
            // 
            this.overlay1uncomprButton.Location = new System.Drawing.Point(335, 101);
            this.overlay1uncomprButton.Name = "overlay1uncomprButton";
            this.overlay1uncomprButton.Size = new System.Drawing.Size(100, 50);
            this.overlay1uncomprButton.TabIndex = 14;
            this.overlay1uncomprButton.Text = "Apply Patch";
            this.overlay1uncomprButton.UseVisualStyleBackColor = true;
            this.overlay1uncomprButton.Click += new System.EventHandler(this.overlay1uncomprButton_Click);
            // 
            // bdhcamARM9requiredLBL
            // 
            this.bdhcamARM9requiredLBL.AutoSize = true;
            this.bdhcamARM9requiredLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bdhcamARM9requiredLBL.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.bdhcamARM9requiredLBL.Location = new System.Drawing.Point(118, 168);
            this.bdhcamARM9requiredLBL.Name = "bdhcamARM9requiredLBL";
            this.bdhcamARM9requiredLBL.Size = new System.Drawing.Size(165, 13);
            this.bdhcamARM9requiredLBL.TabIndex = 15;
            this.bdhcamARM9requiredLBL.Text = "(Requires ARM9 Expansion)";
            // 
            // arm9patchCB
            // 
            this.arm9patchCB.Image = global::DSPRE.Properties.Resources.checkmark;
            this.arm9patchCB.Location = new System.Drawing.Point(309, 45);
            this.arm9patchCB.Name = "arm9patchCB";
            this.arm9patchCB.Size = new System.Drawing.Size(20, 20);
            this.arm9patchCB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.arm9patchCB.TabIndex = 16;
            this.arm9patchCB.TabStop = false;
            this.arm9patchCB.Visible = false;
            // 
            // overlay1CB
            // 
            this.overlay1CB.Image = global::DSPRE.Properties.Resources.checkmark;
            this.overlay1CB.Location = new System.Drawing.Point(309, 116);
            this.overlay1CB.Name = "overlay1CB";
            this.overlay1CB.Size = new System.Drawing.Size(20, 20);
            this.overlay1CB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.overlay1CB.TabIndex = 17;
            this.overlay1CB.TabStop = false;
            this.overlay1CB.Visible = false;
            // 
            // bdhcamCB
            // 
            this.bdhcamCB.Image = global::DSPRE.Properties.Resources.checkmark;
            this.bdhcamCB.Location = new System.Drawing.Point(309, 199);
            this.bdhcamCB.Name = "bdhcamCB";
            this.bdhcamCB.Size = new System.Drawing.Size(20, 20);
            this.bdhcamCB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.bdhcamCB.TabIndex = 18;
            this.bdhcamCB.TabStop = false;
            this.bdhcamCB.Visible = false;
            // 
            // sentenceCaseCB
            // 
            this.sentenceCaseCB.Image = global::DSPRE.Properties.Resources.checkmark;
            this.sentenceCaseCB.Location = new System.Drawing.Point(309, 286);
            this.sentenceCaseCB.Name = "sentenceCaseCB";
            this.sentenceCaseCB.Size = new System.Drawing.Size(20, 20);
            this.sentenceCaseCB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.sentenceCaseCB.TabIndex = 19;
            this.sentenceCaseCB.TabStop = false;
            this.sentenceCaseCB.Visible = false;
            // 
            // itemNumbersCB
            // 
            this.itemNumbersCB.Image = global::DSPRE.Properties.Resources.checkmark;
            this.itemNumbersCB.Location = new System.Drawing.Point(309, 359);
            this.itemNumbersCB.Name = "itemNumbersCB";
            this.itemNumbersCB.Size = new System.Drawing.Size(20, 20);
            this.itemNumbersCB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.itemNumbersCB.TabIndex = 20;
            this.itemNumbersCB.TabStop = false;
            this.itemNumbersCB.Visible = false;
            // 
            // StandardizePatchLBL
            // 
            this.StandardizePatchLBL.AutoSize = true;
            this.StandardizePatchLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StandardizePatchLBL.Location = new System.Drawing.Point(15, 343);
            this.StandardizePatchLBL.Name = "StandardizePatchLBL";
            this.StandardizePatchLBL.Size = new System.Drawing.Size(155, 13);
            this.StandardizePatchLBL.TabIndex = 1;
            this.StandardizePatchLBL.Text = "Standardize Item Numbers";
            // 
            // StandardizePatchTextLBL
            // 
            this.StandardizePatchTextLBL.Location = new System.Drawing.Point(15, 358);
            this.StandardizePatchTextLBL.Name = "StandardizePatchTextLBL";
            this.StandardizePatchTextLBL.Size = new System.Drawing.Size(239, 40);
            this.StandardizePatchTextLBL.TabIndex = 2;
            this.StandardizePatchTextLBL.Text = "Makes it so that PokéBall item scripts follow the order of items\r\nas indexed in t" +
    "he ROM. Needed for \'Item\' option in Event Editor\r\n";
            this.StandardizePatchTextLBL.UseMnemonic = false;
            // 
            // ROMToolboxDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(448, 411);
            this.Controls.Add(this.itemNumbersCB);
            this.Controls.Add(this.sentenceCaseCB);
            this.Controls.Add(this.bdhcamCB);
            this.Controls.Add(this.overlay1CB);
            this.Controls.Add(this.arm9patchCB);
            this.Controls.Add(this.bdhcamARM9requiredLBL);
            this.Controls.Add(this.overlay1uncomprButton);
            this.Controls.Add(this.overlay1patchtextLBL);
            this.Controls.Add(this.overlay1uncompressedLBL);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.namesToSentenceCaseButton);
            this.Controls.Add(this.BDHCAMpatchTextLBL);
            this.Controls.Add(this.BDHCAMpatchLBL);
            this.Controls.Add(this.BDHCAMpatchButton);
            this.Controls.Add(this.arm9expansionTextLBL);
            this.Controls.Add(this.arm9expansionLBL);
            this.Controls.Add(this.applyARM9ExpansionButton);
            this.Controls.Add(this.StandardizePatchTextLBL);
            this.Controls.Add(this.StandardizePatchLBL);
            this.Controls.Add(this.applyItemStandardizeButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ROMToolboxDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ROM Toolbox";
            ((System.ComponentModel.ISupportInitialize)(this.arm9patchCB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overlay1CB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdhcamCB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sentenceCaseCB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemNumbersCB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button applyItemStandardizeButton;
        private System.Windows.Forms.Label arm9expansionTextLBL;
        private System.Windows.Forms.Label arm9expansionLBL;
        private System.Windows.Forms.Button applyARM9ExpansionButton;
        private System.Windows.Forms.Label BDHCAMpatchTextLBL;
        private System.Windows.Forms.Label BDHCAMpatchLBL;
        private System.Windows.Forms.Button BDHCAMpatchButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button namesToSentenceCaseButton;
        private System.Windows.Forms.Label overlay1uncompressedLBL;
        private System.Windows.Forms.Label overlay1patchtextLBL;
        private System.Windows.Forms.Button overlay1uncomprButton;
        private System.Windows.Forms.Label bdhcamARM9requiredLBL;
        private System.Windows.Forms.PictureBox arm9patchCB;
        private System.Windows.Forms.PictureBox overlay1CB;
        private System.Windows.Forms.PictureBox bdhcamCB;
        private System.Windows.Forms.PictureBox sentenceCaseCB;
        private System.Windows.Forms.PictureBox itemNumbersCB;
        private System.Windows.Forms.Label StandardizePatchLBL;
        private System.Windows.Forms.Label StandardizePatchTextLBL;
    }
}