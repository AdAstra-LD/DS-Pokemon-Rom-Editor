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
            this.syncChangesCheckbox = new System.Windows.Forms.CheckBox();
            this.syncChangesLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tabControl.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.personalPage);
            this.tabControl.Controls.Add(this.learnsetPage);
            this.tabControl.Controls.Add(this.evoPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl.Location = new System.Drawing.Point(0, 29);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1032, 552);
            this.tabControl.TabIndex = 0;
            // 
            // personalPage
            // 
            this.personalPage.Location = new System.Drawing.Point(4, 22);
            this.personalPage.Name = "personalPage";
            this.personalPage.Padding = new System.Windows.Forms.Padding(3);
            this.personalPage.Size = new System.Drawing.Size(1024, 526);
            this.personalPage.TabIndex = 0;
            this.personalPage.Text = "Personal Editor";
            this.personalPage.UseVisualStyleBackColor = true;
            // 
            // learnsetPage
            // 
            this.learnsetPage.Location = new System.Drawing.Point(4, 22);
            this.learnsetPage.Name = "learnsetPage";
            this.learnsetPage.Padding = new System.Windows.Forms.Padding(3);
            this.learnsetPage.Size = new System.Drawing.Size(792, 387);
            this.learnsetPage.TabIndex = 1;
            this.learnsetPage.Text = "Learnset Editor";
            this.learnsetPage.UseVisualStyleBackColor = true;
            // 
            // evoPage
            // 
            this.evoPage.Location = new System.Drawing.Point(4, 22);
            this.evoPage.Name = "evoPage";
            this.evoPage.Padding = new System.Windows.Forms.Padding(3);
            this.evoPage.Size = new System.Drawing.Size(792, 387);
            this.evoPage.TabIndex = 2;
            this.evoPage.Text = "Evolution Editor";
            this.evoPage.UseVisualStyleBackColor = true;
            // 
            // syncChangesCheckbox
            // 
            this.syncChangesCheckbox.AutoSize = true;
            this.syncChangesCheckbox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.syncChangesCheckbox.Location = new System.Drawing.Point(3, 3);
            this.syncChangesCheckbox.Name = "syncChangesCheckbox";
            this.syncChangesCheckbox.Size = new System.Drawing.Size(171, 17);
            this.syncChangesCheckbox.TabIndex = 1;
            this.syncChangesCheckbox.Text = "Synchronize Chosen Pokemon";
            this.syncChangesCheckbox.UseVisualStyleBackColor = true;
            // 
            // syncChangesLabel
            // 
            this.syncChangesLabel.AutoSize = true;
            this.syncChangesLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.syncChangesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.syncChangesLabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.syncChangesLabel.Location = new System.Drawing.Point(180, 10);
            this.syncChangesLabel.Name = "syncChangesLabel";
            this.syncChangesLabel.Size = new System.Drawing.Size(555, 13);
            this.syncChangesLabel.TabIndex = 2;
            this.syncChangesLabel.Text = "When this CheckBox is marked, all changes in any tab will be synchronized accross the other tabs.";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.syncChangesCheckbox);
            this.flowLayoutPanel1.Controls.Add(this.syncChangesLabel);
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
            this.ClientSize = new System.Drawing.Size(1032, 581);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.tabControl);
            this.DoubleBuffered = true;
            this.Name = "PokemonEditor";
            this.Text = "PokemonEditor";
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
        private System.Windows.Forms.Label syncChangesLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}