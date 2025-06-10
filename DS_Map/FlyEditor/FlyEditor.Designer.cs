namespace DSPRE.Editors
{
    partial class FlyEditor
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btn_SaveChanges = new System.Windows.Forms.ToolStripButton();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tab_GameOver = new System.Windows.Forms.TabPage();
            this.tab_FlyWarp = new System.Windows.Forms.TabPage();
            this.dt_GameOverWarps = new System.Windows.Forms.DataGridView();
            this.dt_FlyWarps = new System.Windows.Forms.DataGridView();
            this.tab_Unlock = new System.Windows.Forms.TabPage();
            this.dt_UnlockSettings = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tab_GameOver.SuspendLayout();
            this.tab_FlyWarp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dt_GameOverWarps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dt_FlyWarps)).BeginInit();
            this.tab_Unlock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dt_UnlockSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_SaveChanges});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btn_SaveChanges
            // 
            this.btn_SaveChanges.Image = global::DSPRE.Properties.Resources.saveButton;
            this.btn_SaveChanges.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_SaveChanges.Name = "btn_SaveChanges";
            this.btn_SaveChanges.Size = new System.Drawing.Size(100, 22);
            this.btn_SaveChanges.Text = "Save Changes";
            this.btn_SaveChanges.Click += new System.EventHandler(this.btn_SaveChanges_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tab_GameOver);
            this.tabControl.Controls.Add(this.tab_FlyWarp);
            this.tabControl.Controls.Add(this.tab_Unlock);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 25);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(800, 425);
            this.tabControl.TabIndex = 1;
            // 
            // tab_GameOver
            // 
            this.tab_GameOver.Controls.Add(this.dt_GameOverWarps);
            this.tab_GameOver.Location = new System.Drawing.Point(4, 22);
            this.tab_GameOver.Name = "tab_GameOver";
            this.tab_GameOver.Padding = new System.Windows.Forms.Padding(3);
            this.tab_GameOver.Size = new System.Drawing.Size(792, 399);
            this.tab_GameOver.TabIndex = 0;
            this.tab_GameOver.Text = "Game Over Warps";
            this.tab_GameOver.UseVisualStyleBackColor = true;
            // 
            // tab_FlyWarp
            // 
            this.tab_FlyWarp.Controls.Add(this.dt_FlyWarps);
            this.tab_FlyWarp.Location = new System.Drawing.Point(4, 22);
            this.tab_FlyWarp.Name = "tab_FlyWarp";
            this.tab_FlyWarp.Padding = new System.Windows.Forms.Padding(3);
            this.tab_FlyWarp.Size = new System.Drawing.Size(792, 399);
            this.tab_FlyWarp.TabIndex = 1;
            this.tab_FlyWarp.Text = "Fly Warps";
            this.tab_FlyWarp.UseVisualStyleBackColor = true;
            // 
            // dt_GameOverWarps
            // 
            this.dt_GameOverWarps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dt_GameOverWarps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dt_GameOverWarps.Location = new System.Drawing.Point(3, 3);
            this.dt_GameOverWarps.Name = "dt_GameOverWarps";
            this.dt_GameOverWarps.Size = new System.Drawing.Size(786, 393);
            this.dt_GameOverWarps.TabIndex = 0;
            // 
            // dt_FlyWarps
            // 
            this.dt_FlyWarps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dt_FlyWarps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dt_FlyWarps.Location = new System.Drawing.Point(3, 3);
            this.dt_FlyWarps.Name = "dt_FlyWarps";
            this.dt_FlyWarps.Size = new System.Drawing.Size(786, 393);
            this.dt_FlyWarps.TabIndex = 0;
            // 
            // tab_Unlock
            // 
            this.tab_Unlock.Controls.Add(this.dt_UnlockSettings);
            this.tab_Unlock.Location = new System.Drawing.Point(4, 22);
            this.tab_Unlock.Name = "tab_Unlock";
            this.tab_Unlock.Padding = new System.Windows.Forms.Padding(3);
            this.tab_Unlock.Size = new System.Drawing.Size(792, 399);
            this.tab_Unlock.TabIndex = 2;
            this.tab_Unlock.Text = "Unlock Settings";
            this.tab_Unlock.UseVisualStyleBackColor = true;
            // 
            // dt_UnlockSettings
            // 
            this.dt_UnlockSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dt_UnlockSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dt_UnlockSettings.Location = new System.Drawing.Point(3, 3);
            this.dt_UnlockSettings.Name = "dt_UnlockSettings";
            this.dt_UnlockSettings.Size = new System.Drawing.Size(786, 393);
            this.dt_UnlockSettings.TabIndex = 0;
            // 
            // FlyEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "FlyEditor";
            this.Text = "Fly Editor";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tab_GameOver.ResumeLayout(false);
            this.tab_FlyWarp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dt_GameOverWarps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dt_FlyWarps)).EndInit();
            this.tab_Unlock.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dt_UnlockSettings)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btn_SaveChanges;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tab_GameOver;
        private System.Windows.Forms.DataGridView dt_GameOverWarps;
        private System.Windows.Forms.TabPage tab_FlyWarp;
        private System.Windows.Forms.DataGridView dt_FlyWarps;
        private System.Windows.Forms.TabPage tab_Unlock;
        private System.Windows.Forms.DataGridView dt_UnlockSettings;
    }
}