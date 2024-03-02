namespace DSPRE {
    partial class PokemonEditor {
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.personalPage = new System.Windows.Forms.TabPage();
            this.learnsetPage = new System.Windows.Forms.TabPage();
            this.evoPage = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.personalPage);
            this.tabControl.Controls.Add(this.learnsetPage);
            this.tabControl.Controls.Add(this.evoPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(800, 450);
            this.tabControl.TabIndex = 0;
            // 
            // personalPage
            // 
            this.personalPage.Location = new System.Drawing.Point(4, 22);
            this.personalPage.Name = "personalPage";
            this.personalPage.Padding = new System.Windows.Forms.Padding(3);
            this.personalPage.Size = new System.Drawing.Size(792, 424);
            this.personalPage.TabIndex = 0;
            this.personalPage.Text = "Personal Editor";
            this.personalPage.UseVisualStyleBackColor = true;
            // 
            // learnsetPage
            // 
            this.learnsetPage.Location = new System.Drawing.Point(4, 22);
            this.learnsetPage.Name = "learnsetPage";
            this.learnsetPage.Padding = new System.Windows.Forms.Padding(3);
            this.learnsetPage.Size = new System.Drawing.Size(792, 424);
            this.learnsetPage.TabIndex = 1;
            this.learnsetPage.Text = "Learnset Editor";
            this.learnsetPage.UseVisualStyleBackColor = true;
            // 
            // evoPage
            // 
            this.evoPage.Location = new System.Drawing.Point(4, 22);
            this.evoPage.Name = "evoPage";
            this.evoPage.Padding = new System.Windows.Forms.Padding(3);
            this.evoPage.Size = new System.Drawing.Size(792, 424);
            this.evoPage.TabIndex = 2;
            this.evoPage.Text = "Evolution Editor";
            this.evoPage.UseVisualStyleBackColor = true;
            // 
            // PokemonEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl);
            this.Name = "PokemonEditor";
            this.Text = "PokemonEditor";
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage personalPage;
        private System.Windows.Forms.TabPage learnsetPage;
        private System.Windows.Forms.TabPage evoPage;
    }
}