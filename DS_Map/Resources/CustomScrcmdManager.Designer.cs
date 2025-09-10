namespace DSPRE.Resources
{
    partial class CustomScrcmdManager
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
            this.CustomScrcmdDataGrid = new System.Windows.Forms.DataGridView();
            this.customScrcmdName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AddScrcmdButton = new System.Windows.Forms.Button();
            this.RemoveScrcmdButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.CustomScrcmdDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // CustomScrcmdDataGrid
            // 
            this.CustomScrcmdDataGrid.AllowUserToAddRows = false;
            this.CustomScrcmdDataGrid.AllowUserToDeleteRows = false;
            this.CustomScrcmdDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CustomScrcmdDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.customScrcmdName});
            this.CustomScrcmdDataGrid.Location = new System.Drawing.Point(12, 12);
            this.CustomScrcmdDataGrid.MultiSelect = false;
            this.CustomScrcmdDataGrid.Name = "CustomScrcmdDataGrid";
            this.CustomScrcmdDataGrid.RowHeadersWidth = 51;
            this.CustomScrcmdDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.CustomScrcmdDataGrid.Size = new System.Drawing.Size(463, 284);
            this.CustomScrcmdDataGrid.TabIndex = 0;
            // 
            // customScrcmdName
            // 
            this.customScrcmdName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.customScrcmdName.HeaderText = "Name";
            this.customScrcmdName.MinimumWidth = 6;
            this.customScrcmdName.Name = "customScrcmdName";
            // 
            // AddScrcmdButton
            // 
            this.AddScrcmdButton.Image = global::DSPRE.Properties.Resources.scriptDBIconImport;
            this.AddScrcmdButton.Location = new System.Drawing.Point(12, 302);
            this.AddScrcmdButton.Name = "AddScrcmdButton";
            this.AddScrcmdButton.Size = new System.Drawing.Size(99, 37);
            this.AddScrcmdButton.TabIndex = 1;
            this.AddScrcmdButton.Text = "Import DB";
            this.AddScrcmdButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.AddScrcmdButton.UseVisualStyleBackColor = true;
            this.AddScrcmdButton.Click += new System.EventHandler(this.ImportScrcmdButton_Click);
            // 
            // RemoveScrcmdButton
            // 
            this.RemoveScrcmdButton.Image = global::DSPRE.Properties.Resources.scriptDBIconExport;
            this.RemoveScrcmdButton.Location = new System.Drawing.Point(117, 302);
            this.RemoveScrcmdButton.Name = "RemoveScrcmdButton";
            this.RemoveScrcmdButton.Size = new System.Drawing.Size(99, 37);
            this.RemoveScrcmdButton.TabIndex = 2;
            this.RemoveScrcmdButton.Text = "Export DB";
            this.RemoveScrcmdButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.RemoveScrcmdButton.UseVisualStyleBackColor = true;
            this.RemoveScrcmdButton.Click += new System.EventHandler(this.ExportScrcmdButton_Click);
            // 
            // button1
            // 
            this.button1.Image = global::DSPRE.Properties.Resources.open_file;
            this.button1.Location = new System.Drawing.Point(337, 302);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(138, 37);
            this.button1.TabIndex = 3;
            this.button1.Text = "Open DB Folder";
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OpenScrcmdFolderButton_Click);
            // 
            // CustomScrcmdManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 348);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.RemoveScrcmdButton);
            this.Controls.Add(this.AddScrcmdButton);
            this.Controls.Add(this.CustomScrcmdDataGrid);
            this.MaximumSize = new System.Drawing.Size(506, 387);
            this.MinimumSize = new System.Drawing.Size(506, 382);
            this.Name = "CustomScrcmdManager";
            this.ShowIcon = false;
            this.Text = "ScrCmd Database Manager";
            ((System.ComponentModel.ISupportInitialize)(this.CustomScrcmdDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView CustomScrcmdDataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn customScrcmdName;
        private System.Windows.Forms.Button AddScrcmdButton;
        private System.Windows.Forms.Button RemoveScrcmdButton;
        private System.Windows.Forms.Button button1;
    }
}