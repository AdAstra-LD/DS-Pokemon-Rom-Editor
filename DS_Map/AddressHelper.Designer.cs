namespace DSPRE
{
    partial class AddressHelper
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddressHelper));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AddressResult = new System.Windows.Forms.Label();
            this.searchAddressButton = new System.Windows.Forms.Button();
            this.AddressInput = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.addressesGrid = new System.Windows.Forms.DataGridView();
            this.overlayCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OffsetCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.addressesGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "0x";
            // 
            // AddressResult
            // 
            this.AddressResult.AutoSize = true;
            this.AddressResult.Location = new System.Drawing.Point(16, 79);
            this.AddressResult.Name = "AddressResult";
            this.AddressResult.Size = new System.Drawing.Size(0, 13);
            this.AddressResult.TabIndex = 3;
            // 
            // searchAddressButton
            // 
            this.searchAddressButton.Location = new System.Drawing.Point(158, 28);
            this.searchAddressButton.Name = "searchAddressButton";
            this.searchAddressButton.Size = new System.Drawing.Size(75, 20);
            this.searchAddressButton.TabIndex = 4;
            this.searchAddressButton.Text = "Search";
            this.searchAddressButton.UseVisualStyleBackColor = true;
            this.searchAddressButton.Click += new System.EventHandler(this.searchAddressButton_Click);
            // 
            // AddressInput
            // 
            this.AddressInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AddressInput.Location = new System.Drawing.Point(32, 29);
            this.AddressInput.MaxLength = 8;
            this.AddressInput.Name = "AddressInput";
            this.AddressInput.Size = new System.Drawing.Size(120, 20);
            this.AddressInput.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.addressesGrid);
            this.groupBox1.Location = new System.Drawing.Point(-3, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(338, 156);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // addressesGrid
            // 
            this.addressesGrid.AllowUserToAddRows = false;
            this.addressesGrid.AllowUserToDeleteRows = false;
            this.addressesGrid.AllowUserToOrderColumns = true;
            this.addressesGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.addressesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.addressesGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.overlayCol,
            this.OffsetCol});
            this.addressesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addressesGrid.Location = new System.Drawing.Point(3, 16);
            this.addressesGrid.Name = "addressesGrid";
            this.addressesGrid.ReadOnly = true;
            this.addressesGrid.Size = new System.Drawing.Size(332, 137);
            this.addressesGrid.TabIndex = 0;
            // 
            // overlayCol
            // 
            this.overlayCol.HeaderText = "Overlay";
            this.overlayCol.Name = "overlayCol";
            this.overlayCol.ReadOnly = true;
            // 
            // OffsetCol
            // 
            this.OffsetCol.HeaderText = "Offset";
            this.OffsetCol.Name = "OffsetCol";
            this.OffsetCol.ReadOnly = true;
            // 
            // AddressHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 211);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.AddressInput);
            this.Controls.Add(this.searchAddressButton);
            this.Controls.Add(this.AddressResult);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(350, 1000);
            this.MinimumSize = new System.Drawing.Size(350, 250);
            this.Name = "AddressHelper";
            this.Text = "AddressHelper";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.addressesGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label AddressResult;
        private System.Windows.Forms.Button searchAddressButton;
        private System.Windows.Forms.TextBox AddressInput;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView addressesGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn overlayCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn OffsetCol;
    }
}