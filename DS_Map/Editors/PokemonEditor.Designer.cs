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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PokemonEditor));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.personalPage = new System.Windows.Forms.TabPage();
            this.learnsetPage = new System.Windows.Forms.TabPage();
            this.evoPage = new System.Windows.Forms.TabPage();
            this.spritePage = new System.Windows.Forms.TabPage();
            this.syncChangesCheckbox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.FormClosing += PokemonEditor_FormClosing;
            this.tabControl.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.personalPage);
            this.tabControl.Controls.Add(this.learnsetPage);
            this.tabControl.Controls.Add(this.evoPage);
            this.tabControl.Controls.Add(this.spritePage);
            this.tabControl.Location = new System.Drawing.Point(0, 26);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1032, 843);
            this.tabControl.TabIndex = 0;
            // 
            // personalPage
            // 
            this.personalPage.Location = new System.Drawing.Point(4, 22);
            this.personalPage.Name = "personalPage";
            this.personalPage.Padding = new System.Windows.Forms.Padding(3);
            this.personalPage.Size = new System.Drawing.Size(1024, 817);
            this.personalPage.TabIndex = 0;
            this.personalPage.Text = "Personal Editor";
            this.personalPage.UseVisualStyleBackColor = true;
            // 
            // learnsetPage
            // 
            this.learnsetPage.Location = new System.Drawing.Point(4, 22);
            this.learnsetPage.Name = "learnsetPage";
            this.learnsetPage.Padding = new System.Windows.Forms.Padding(3);
            this.learnsetPage.Size = new System.Drawing.Size(1024, 817);
            this.learnsetPage.TabIndex = 1;
            this.learnsetPage.Text = "Learnset Editor";
            this.learnsetPage.UseVisualStyleBackColor = true;
            // 
            // evoPage
            // 
            this.evoPage.Location = new System.Drawing.Point(4, 22);
            this.evoPage.Name = "evoPage";
            this.evoPage.Padding = new System.Windows.Forms.Padding(3);
            this.evoPage.Size = new System.Drawing.Size(1024, 817);
            this.evoPage.TabIndex = 2;
            this.evoPage.Text = "Evolution Editor";
            this.evoPage.UseVisualStyleBackColor = true;
            // 
            // spritePage
            // 
            this.spritePage.Location = new System.Drawing.Point(4, 22);
            this.spritePage.Name = "spritePage";
            this.spritePage.Padding = new System.Windows.Forms.Padding(3);
            this.spritePage.Size = new System.Drawing.Size(1024, 817);
            this.spritePage.TabIndex = 3;
            this.spritePage.Text = "Sprite Editor";
            this.spritePage.UseVisualStyleBackColor = true;
            // 
            // syncChangesCheckbox
            // 
            this.syncChangesCheckbox.AutoSize = true;
            this.syncChangesCheckbox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.syncChangesCheckbox.Location = new System.Drawing.Point(3, 3);
            this.syncChangesCheckbox.Name = "syncChangesCheckbox";
            this.syncChangesCheckbox.Size = new System.Drawing.Size(177, 17);
            this.syncChangesCheckbox.TabIndex = 1;
            this.syncChangesCheckbox.Text = "Synchronize Pokémon selection";
            this.syncChangesCheckbox.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.syncChangesCheckbox);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1032, 23);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // PokemonEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(1032, 861);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.tabControl);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PokemonEditor";
            this.Text = "Pokémon Editor";
            this.tabControl.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage personalPage;
        private System.Windows.Forms.TabPage learnsetPage;
        private System.Windows.Forms.TabPage evoPage;
        private System.Windows.Forms.CheckBox syncChangesCheckbox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TabPage spritePage;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}