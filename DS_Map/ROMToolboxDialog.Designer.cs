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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.applyARM9ExpansionButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.namesToSentenceCaseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // applyItemStandardizeButton
            // 
            this.applyItemStandardizeButton.Location = new System.Drawing.Point(330, 282);
            this.applyItemStandardizeButton.Name = "applyItemStandardizeButton";
            this.applyItemStandardizeButton.Size = new System.Drawing.Size(100, 31);
            this.applyItemStandardizeButton.TabIndex = 0;
            this.applyItemStandardizeButton.Text = "Apply";
            this.applyItemStandardizeButton.UseVisualStyleBackColor = true;
            this.applyItemStandardizeButton.Click += new System.EventHandler(this.applyItemStandardizeButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 267);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Standardize Item Numbers";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 282);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(305, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "Makes it so that PokéBall item scripts follow the order of items\r\nas indexed in t" +
    "he ROM. Needed for \'Item\' option in Event Editor\r\n";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(298, 52);
            this.label3.TabIndex = 5;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "ARM9 Expansion";
            // 
            // applyARM9ExpansionButton
            // 
            this.applyARM9ExpansionButton.Location = new System.Drawing.Point(330, 32);
            this.applyARM9ExpansionButton.Name = "applyARM9ExpansionButton";
            this.applyARM9ExpansionButton.Size = new System.Drawing.Size(100, 31);
            this.applyARM9ExpansionButton.TabIndex = 3;
            this.applyARM9ExpansionButton.Text = "Expand ARM9";
            this.applyARM9ExpansionButton.UseVisualStyleBackColor = true;
            this.applyARM9ExpansionButton.Click += new System.EventHandler(this.applyARM9ExpansionButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(314, 52);
            this.label5.TabIndex = 8;
            this.label5.Text = "Adds Dynamic BDHC Cameras to current ROM.\r\nWith this patch, you can make the game" +
    " camera move\r\nbased on the player\'s position.\r\nYou will need Trifindo\'s PDSMS, i" +
    "n order to make BDHCAM Files.\r\n";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(14, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(269, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Dynamic Cameras (Requires ARM9 Expansion)";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(330, 118);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 31);
            this.button1.TabIndex = 6;
            this.button1.Text = "Coming soon";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 208);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(200, 39);
            this.label7.TabIndex = 11;
            this.label7.Text = "BULBASAUR, IVYSAUR, VENUSAUR...\r\nbecome\r\nBulbasaur, Ivysaur, Venusaur...\r\n";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(15, 193);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(252, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Convert Pokémon names to Sentence Case";
            // 
            // namesToSentenceCaseButton
            // 
            this.namesToSentenceCaseButton.Enabled = false;
            this.namesToSentenceCaseButton.Location = new System.Drawing.Point(330, 208);
            this.namesToSentenceCaseButton.Name = "namesToSentenceCaseButton";
            this.namesToSentenceCaseButton.Size = new System.Drawing.Size(100, 31);
            this.namesToSentenceCaseButton.TabIndex = 9;
            this.namesToSentenceCaseButton.Text = "Coming soon";
            this.namesToSentenceCaseButton.UseVisualStyleBackColor = true;
            this.namesToSentenceCaseButton.Click += new System.EventHandler(this.applyPokemonNamesToSentenceCase_Click);
            // 
            // ROMToolboxDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(442, 328);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.namesToSentenceCaseButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.applyARM9ExpansionButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.applyItemStandardizeButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ROMToolboxDialog";
            this.Text = "ROM Toolbox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button applyItemStandardizeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button applyARM9ExpansionButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button namesToSentenceCaseButton;
    }
}