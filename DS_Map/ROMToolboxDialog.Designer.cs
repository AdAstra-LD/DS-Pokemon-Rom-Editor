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
            this.StandardizePatchLBL = new System.Windows.Forms.Label();
            this.StandardizePatchTextLBL = new System.Windows.Forms.Label();
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
            this.SuspendLayout();
            // 
            // applyItemStandardizeButton
            // 
            this.applyItemStandardizeButton.Location = new System.Drawing.Point(330, 337);
            this.applyItemStandardizeButton.Name = "applyItemStandardizeButton";
            this.applyItemStandardizeButton.Size = new System.Drawing.Size(100, 31);
            this.applyItemStandardizeButton.TabIndex = 0;
            this.applyItemStandardizeButton.Text = "Apply Patch";
            this.applyItemStandardizeButton.UseVisualStyleBackColor = true;
            this.applyItemStandardizeButton.Click += new System.EventHandler(this.ApplyItemStandardizeButton_Click);
            // 
            // StandardizePatchLBL
            // 
            this.StandardizePatchLBL.AutoSize = true;
            this.StandardizePatchLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StandardizePatchLBL.Location = new System.Drawing.Point(15, 322);
            this.StandardizePatchLBL.Name = "StandardizePatchLBL";
            this.StandardizePatchLBL.Size = new System.Drawing.Size(155, 13);
            this.StandardizePatchLBL.TabIndex = 1;
            this.StandardizePatchLBL.Text = "Standardize Item Numbers";
            // 
            // StandardizePatchTextLBL
            // 
            this.StandardizePatchTextLBL.AutoSize = true;
            this.StandardizePatchTextLBL.Location = new System.Drawing.Point(15, 337);
            this.StandardizePatchTextLBL.Name = "StandardizePatchTextLBL";
            this.StandardizePatchTextLBL.Size = new System.Drawing.Size(305, 26);
            this.StandardizePatchTextLBL.TabIndex = 2;
            this.StandardizePatchTextLBL.Text = "Makes it so that PokéBall item scripts follow the order of items\r\nas indexed in t" +
    "he ROM. Needed for \'Item\' option in Event Editor\r\n";
            // 
            // arm9expansionTextLBL
            // 
            this.arm9expansionTextLBL.AutoSize = true;
            this.arm9expansionTextLBL.Location = new System.Drawing.Point(15, 32);
            this.arm9expansionTextLBL.Name = "arm9expansionTextLBL";
            this.arm9expansionTextLBL.Size = new System.Drawing.Size(298, 52);
            this.arm9expansionTextLBL.TabIndex = 5;
            this.arm9expansionTextLBL.Text = resources.GetString("arm9expansionTextLBL.Text");
            // 
            // arm9expansionLBL
            // 
            this.arm9expansionLBL.AutoSize = true;
            this.arm9expansionLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arm9expansionLBL.Location = new System.Drawing.Point(15, 17);
            this.arm9expansionLBL.Name = "arm9expansionLBL";
            this.arm9expansionLBL.Size = new System.Drawing.Size(103, 13);
            this.arm9expansionLBL.TabIndex = 4;
            this.arm9expansionLBL.Text = "ARM9 Expansion";
            // 
            // applyARM9ExpansionButton
            // 
            this.applyARM9ExpansionButton.Location = new System.Drawing.Point(330, 32);
            this.applyARM9ExpansionButton.Name = "applyARM9ExpansionButton";
            this.applyARM9ExpansionButton.Size = new System.Drawing.Size(100, 31);
            this.applyARM9ExpansionButton.TabIndex = 3;
            this.applyARM9ExpansionButton.Text = "Expand ARM9";
            this.applyARM9ExpansionButton.UseVisualStyleBackColor = true;
            this.applyARM9ExpansionButton.Click += new System.EventHandler(this.ApplyARM9ExpansionButton_Click);
            // 
            // BDHCAMpatchTextLBL
            // 
            this.BDHCAMpatchTextLBL.AutoSize = true;
            this.BDHCAMpatchTextLBL.Enabled = false;
            this.BDHCAMpatchTextLBL.Location = new System.Drawing.Point(14, 170);
            this.BDHCAMpatchTextLBL.Name = "BDHCAMpatchTextLBL";
            this.BDHCAMpatchTextLBL.Size = new System.Drawing.Size(231, 65);
            this.BDHCAMpatchTextLBL.TabIndex = 8;
            this.BDHCAMpatchTextLBL.Text = "Adds Dynamic BDHC Cameras to current ROM.\r\nWith this patch, you have more control" +
    " over \r\nthe game camera\'s rotation and position.\r\nYou will need Trifindo\'s PDSMS" +
    "\r\nin order to make BDHCAM Files.\r\n";
            // 
            // BDHCAMpatchLBL
            // 
            this.BDHCAMpatchLBL.AutoSize = true;
            this.BDHCAMpatchLBL.Enabled = false;
            this.BDHCAMpatchLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BDHCAMpatchLBL.Location = new System.Drawing.Point(14, 155);
            this.BDHCAMpatchLBL.Name = "BDHCAMpatchLBL";
            this.BDHCAMpatchLBL.Size = new System.Drawing.Size(107, 13);
            this.BDHCAMpatchLBL.TabIndex = 7;
            this.BDHCAMpatchLBL.Text = "Dynamic Cameras";
            // 
            // BDHCAMpatchButton
            // 
            this.BDHCAMpatchButton.Enabled = false;
            this.BDHCAMpatchButton.Location = new System.Drawing.Point(330, 183);
            this.BDHCAMpatchButton.Name = "BDHCAMpatchButton";
            this.BDHCAMpatchButton.Size = new System.Drawing.Size(100, 31);
            this.BDHCAMpatchButton.TabIndex = 6;
            this.BDHCAMpatchButton.Text = "Apply Patch";
            this.BDHCAMpatchButton.UseVisualStyleBackColor = true;
            this.BDHCAMpatchButton.Click += new System.EventHandler(this.BDHCAMPatchButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Enabled = false;
            this.label7.Location = new System.Drawing.Point(15, 263);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(200, 39);
            this.label7.TabIndex = 11;
            this.label7.Text = "BULBASAUR, IVYSAUR, VENUSAUR...\r\nbecome\r\nBulbasaur, Ivysaur, Venusaur...\r\n";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Enabled = false;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(15, 248);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(252, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Convert Pokémon names to Sentence Case";
            // 
            // namesToSentenceCaseButton
            // 
            this.namesToSentenceCaseButton.Enabled = false;
            this.namesToSentenceCaseButton.Location = new System.Drawing.Point(330, 263);
            this.namesToSentenceCaseButton.Name = "namesToSentenceCaseButton";
            this.namesToSentenceCaseButton.Size = new System.Drawing.Size(100, 31);
            this.namesToSentenceCaseButton.TabIndex = 9;
            this.namesToSentenceCaseButton.Text = "Coming soon";
            this.namesToSentenceCaseButton.UseVisualStyleBackColor = true;
            this.namesToSentenceCaseButton.Click += new System.EventHandler(this.SentenceCasePatchButton_Click);
            // 
            // overlay1uncompressedLBL
            // 
            this.overlay1uncompressedLBL.AutoSize = true;
            this.overlay1uncompressedLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overlay1uncompressedLBL.Location = new System.Drawing.Point(15, 100);
            this.overlay1uncompressedLBL.Name = "overlay1uncompressedLBL";
            this.overlay1uncompressedLBL.Size = new System.Drawing.Size(217, 13);
            this.overlay1uncompressedLBL.TabIndex = 12;
            this.overlay1uncompressedLBL.Text = "Configure Overlay1 as uncompressed";
            // 
            // overlay1patchtextLBL
            // 
            this.overlay1patchtextLBL.AutoSize = true;
            this.overlay1patchtextLBL.Location = new System.Drawing.Point(15, 115);
            this.overlay1patchtextLBL.Name = "overlay1patchtextLBL";
            this.overlay1patchtextLBL.Size = new System.Drawing.Size(223, 26);
            this.overlay1patchtextLBL.TabIndex = 13;
            this.overlay1patchtextLBL.Text = "Overlay1 won\'t have to be compressed again.\r\nThe operation is reversible.\r\n";
            // 
            // overlay1uncomprButton
            // 
            this.overlay1uncomprButton.Location = new System.Drawing.Point(330, 106);
            this.overlay1uncomprButton.Name = "overlay1uncomprButton";
            this.overlay1uncomprButton.Size = new System.Drawing.Size(100, 31);
            this.overlay1uncomprButton.TabIndex = 14;
            this.overlay1uncomprButton.Text = "Apply Patch";
            this.overlay1uncomprButton.UseVisualStyleBackColor = true;
            this.overlay1uncomprButton.Click += new System.EventHandler(this.overlay1uncomprButton_Click);
            // 
            // bdhcamARM9LBL
            // 
            this.bdhcamARM9requiredLBL.AutoSize = true;
            this.bdhcamARM9requiredLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bdhcamARM9requiredLBL.Location = new System.Drawing.Point(117, 155);
            this.bdhcamARM9requiredLBL.Name = "bdhcamARM9LBL";
            this.bdhcamARM9requiredLBL.Size = new System.Drawing.Size(165, 13);
            this.bdhcamARM9requiredLBL.TabIndex = 15;
            this.bdhcamARM9requiredLBL.Text = "(Requires ARM9 Expansion)";
            // 
            // ROMToolboxDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(442, 378);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button applyItemStandardizeButton;
        private System.Windows.Forms.Label StandardizePatchLBL;
        private System.Windows.Forms.Label StandardizePatchTextLBL;
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
    }
}