namespace DSPRE {
    partial class OverlayEditor {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OverlayEditor));
            this.overlayDataGrid = new System.Windows.Forms.DataGridView();
            this.isCompressedButton = new System.Windows.Forms.Button();
            this.isMarkedCompressedButton = new System.Windows.Forms.Button();
            this.saveChangesButton = new System.Windows.Forms.Button();
            this.revertChangesButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.overlayDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // overlayDataGrid
            // 
            this.overlayDataGrid.AllowUserToAddRows = false;
            this.overlayDataGrid.AllowUserToDeleteRows = false;
            this.overlayDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.overlayDataGrid.Location = new System.Drawing.Point(12, 12);
            this.overlayDataGrid.Name = "overlayDataGrid";
            this.overlayDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.overlayDataGrid.Size = new System.Drawing.Size(517, 271);
            this.overlayDataGrid.TabIndex = 0;
            this.overlayDataGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.overlayDataGrid_CellFormatting);
            this.overlayDataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.overlayDataGrid_CellValueChanged);
            this.overlayDataGrid.SelectionChanged += new System.EventHandler(this.overlayDataGrid_SelectionChanged);
            // 
            // isCompressedButton
            // 
            this.isCompressedButton.Location = new System.Drawing.Point(13, 290);
            this.isCompressedButton.Name = "isCompressedButton";
            this.isCompressedButton.Size = new System.Drawing.Size(127, 42);
            this.isCompressedButton.TabIndex = 1;
            this.isCompressedButton.Text = "Compress/Decompress All";
            this.isCompressedButton.UseVisualStyleBackColor = true;
            this.isCompressedButton.Click += new System.EventHandler(this.isCompressedButton_Click);
            // 
            // isMarkedCompressedButton
            // 
            this.isMarkedCompressedButton.Location = new System.Drawing.Point(146, 290);
            this.isMarkedCompressedButton.Name = "isMarkedCompressedButton";
            this.isMarkedCompressedButton.Size = new System.Drawing.Size(123, 43);
            this.isMarkedCompressedButton.TabIndex = 2;
            this.isMarkedCompressedButton.Text = "Mark/Unmark Compression All";
            this.isMarkedCompressedButton.UseVisualStyleBackColor = true;
            this.isMarkedCompressedButton.Click += new System.EventHandler(this.isMarkedCompressedButton_Click);
            // 
            // saveChangesButton
            // 
            this.saveChangesButton.Location = new System.Drawing.Point(428, 290);
            this.saveChangesButton.Name = "saveChangesButton";
            this.saveChangesButton.Size = new System.Drawing.Size(101, 42);
            this.saveChangesButton.TabIndex = 3;
            this.saveChangesButton.Text = "Save Changes";
            this.saveChangesButton.UseVisualStyleBackColor = true;
            this.saveChangesButton.Click += new System.EventHandler(this.saveChangesButton_Click);
            // 
            // revertChangesButton
            // 
            this.revertChangesButton.Location = new System.Drawing.Point(310, 290);
            this.revertChangesButton.Name = "revertChangesButton";
            this.revertChangesButton.Size = new System.Drawing.Size(112, 42);
            this.revertChangesButton.TabIndex = 4;
            this.revertChangesButton.Text = "Revert Current Changes";
            this.revertChangesButton.UseVisualStyleBackColor = true;
            this.revertChangesButton.Click += new System.EventHandler(this.revertChangesButton_Click);
            // 
            // OverlayEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 344);
            this.Controls.Add(this.revertChangesButton);
            this.Controls.Add(this.saveChangesButton);
            this.Controls.Add(this.isMarkedCompressedButton);
            this.Controls.Add(this.isCompressedButton);
            this.Controls.Add(this.overlayDataGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OverlayEditor";
            this.Text = "OverlayEditor";
            ((System.ComponentModel.ISupportInitialize)(this.overlayDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView overlayDataGrid;
        private System.Windows.Forms.Button isCompressedButton;
        private System.Windows.Forms.Button isMarkedCompressedButton;
        private System.Windows.Forms.Button saveChangesButton;
        private System.Windows.Forms.Button revertChangesButton;
    }
}