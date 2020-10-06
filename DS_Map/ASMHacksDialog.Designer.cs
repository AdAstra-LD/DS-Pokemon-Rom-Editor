namespace DS_Map
{
    partial class ASMHacksDialog
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
            this.applyItemStandardizeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.applyARM9ExpansionButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // applyItemStandardizeButton
            // 
            this.applyItemStandardizeButton.Location = new System.Drawing.Point(330, 107);
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
            this.label1.Location = new System.Drawing.Point(15, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Standardize Item Numbers";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 107);
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
            this.label3.Size = new System.Drawing.Size(309, 39);
            this.label3.TabIndex = 5;
            this.label3.Text = "Expands ARM9 by repointing the Map Headers table to another \r\nmemory region, leav" +
    "ing free space in the original location. \r\nNeeded to add new Headers and for oth" +
    "er ASM hacks.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(187, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "ARM9 Expansion (Experimental)";
            // 
            // applyARM9ExpansionButton
            // 
            this.applyARM9ExpansionButton.Enabled = false;
            this.applyARM9ExpansionButton.Location = new System.Drawing.Point(330, 30);
            this.applyARM9ExpansionButton.Name = "applyARM9ExpansionButton";
            this.applyARM9ExpansionButton.Size = new System.Drawing.Size(100, 31);
            this.applyARM9ExpansionButton.TabIndex = 3;
            this.applyARM9ExpansionButton.Text = "Work in progress";
            this.applyARM9ExpansionButton.UseVisualStyleBackColor = true;
            this.applyARM9ExpansionButton.Click += new System.EventHandler(this.applyARM9ExpansionButton_Click);
            // 
            // ASMHacksDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 156);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.applyARM9ExpansionButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.applyItemStandardizeButton);
            this.Name = "ASMHacksDialog";
            this.Text = "ASM Hacks";
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
    }
}