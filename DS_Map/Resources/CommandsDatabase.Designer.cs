
namespace DSPRE.Resources {
    partial class CommandsDatabase {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.scriptcmdDataGridView = new System.Windows.Forms.DataGridView();
            this.cmdSearchTextBox = new System.Windows.Forms.TextBox();
            this.startSearchButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.criteriaGroupBox = new System.Windows.Forms.GroupBox();
            this.matchCB = new System.Windows.Forms.RadioButton();
            this.containsCB = new System.Windows.Forms.RadioButton();
            this.startsWithCB = new System.Windows.Forms.RadioButton();
            this.CommandID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CommandName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParamsCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Params = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.scriptcmdDataGridView)).BeginInit();
            this.criteriaGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // scriptcmdDataGridView
            // 
            this.scriptcmdDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.scriptcmdDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.scriptcmdDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.scriptcmdDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CommandID,
            this.CommandName,
            this.ParamsCount,
            this.Params});
            this.scriptcmdDataGridView.Location = new System.Drawing.Point(17, 60);
            this.scriptcmdDataGridView.Name = "scriptcmdDataGridView";
            this.scriptcmdDataGridView.ReadOnly = true;
            this.scriptcmdDataGridView.RowHeadersVisible = false;
            this.scriptcmdDataGridView.Size = new System.Drawing.Size(501, 602);
            this.scriptcmdDataGridView.TabIndex = 0;
            // 
            // cmdSearchTextBox
            // 
            this.cmdSearchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSearchTextBox.Location = new System.Drawing.Point(9, 26);
            this.cmdSearchTextBox.Name = "cmdSearchTextBox";
            this.cmdSearchTextBox.Size = new System.Drawing.Size(145, 22);
            this.cmdSearchTextBox.TabIndex = 1;
            this.cmdSearchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmdSearchTextBox_KeyDown);
            // 
            // startSearchButton
            // 
            this.startSearchButton.Image = global::DSPRE.Properties.Resources.wideLensImage;
            this.startSearchButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.startSearchButton.Location = new System.Drawing.Point(445, 11);
            this.startSearchButton.Name = "startSearchButton";
            this.startSearchButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.startSearchButton.Size = new System.Drawing.Size(82, 39);
            this.startSearchButton.TabIndex = 17;
            this.startSearchButton.Text = "Start\r\nSearch";
            this.startSearchButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.startSearchButton.UseVisualStyleBackColor = true;
            this.startSearchButton.Click += new System.EventHandler(this.startSearchButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Search command name or ID:";
            // 
            // criteriaGroupBox
            // 
            this.criteriaGroupBox.Controls.Add(this.matchCB);
            this.criteriaGroupBox.Controls.Add(this.containsCB);
            this.criteriaGroupBox.Controls.Add(this.startsWithCB);
            this.criteriaGroupBox.Location = new System.Drawing.Point(164, 5);
            this.criteriaGroupBox.Name = "criteriaGroupBox";
            this.criteriaGroupBox.Size = new System.Drawing.Size(271, 45);
            this.criteriaGroupBox.TabIndex = 19;
            this.criteriaGroupBox.TabStop = false;
            this.criteriaGroupBox.Text = "Criteria";
            // 
            // matchCB
            // 
            this.matchCB.Appearance = System.Windows.Forms.Appearance.Button;
            this.matchCB.AutoSize = true;
            this.matchCB.Location = new System.Drawing.Point(147, 15);
            this.matchCB.Name = "matchCB";
            this.matchCB.Size = new System.Drawing.Size(113, 23);
            this.matchCB.TabIndex = 2;
            this.matchCB.Text = "Match (Ignore Case)";
            this.matchCB.UseVisualStyleBackColor = true;
            // 
            // containsCB
            // 
            this.containsCB.Appearance = System.Windows.Forms.Appearance.Button;
            this.containsCB.AutoSize = true;
            this.containsCB.Checked = true;
            this.containsCB.Location = new System.Drawing.Point(8, 15);
            this.containsCB.Name = "containsCB";
            this.containsCB.Size = new System.Drawing.Size(58, 23);
            this.containsCB.TabIndex = 1;
            this.containsCB.TabStop = true;
            this.containsCB.Text = "Contains";
            this.containsCB.UseVisualStyleBackColor = true;
            // 
            // startsWithCB
            // 
            this.startsWithCB.Appearance = System.Windows.Forms.Appearance.Button;
            this.startsWithCB.AutoSize = true;
            this.startsWithCB.Location = new System.Drawing.Point(74, 15);
            this.startsWithCB.Name = "startsWithCB";
            this.startsWithCB.Size = new System.Drawing.Size(66, 23);
            this.startsWithCB.TabIndex = 0;
            this.startsWithCB.Text = "Starts with";
            this.startsWithCB.UseVisualStyleBackColor = true;
            // 
            // CommandID
            // 
            this.CommandID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Format = "X4";
            this.CommandID.DefaultCellStyle = dataGridViewCellStyle1;
            this.CommandID.FillWeight = 30F;
            this.CommandID.HeaderText = "Command ID";
            this.CommandID.MaxInputLength = 10;
            this.CommandID.Name = "CommandID";
            this.CommandID.ReadOnly = true;
            this.CommandID.Width = 70;
            // 
            // CommandName
            // 
            this.CommandName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CommandName.FillWeight = 60F;
            this.CommandName.HeaderText = "Command Name";
            this.CommandName.MaxInputLength = 200;
            this.CommandName.MinimumWidth = 90;
            this.CommandName.Name = "CommandName";
            this.CommandName.ReadOnly = true;
            this.CommandName.Width = 101;
            // 
            // ParamsCount
            // 
            this.ParamsCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ParamsCount.FillWeight = 90F;
            this.ParamsCount.HeaderText = "Parameter Count";
            this.ParamsCount.MaxInputLength = 10;
            this.ParamsCount.MinimumWidth = 20;
            this.ParamsCount.Name = "ParamsCount";
            this.ParamsCount.ReadOnly = true;
            // 
            // Params
            // 
            this.Params.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Params.FillWeight = 190F;
            this.Params.HeaderText = "Parameters";
            this.Params.MaxInputLength = 200;
            this.Params.MinimumWidth = 85;
            this.Params.Name = "Params";
            this.Params.ReadOnly = true;
            // 
            // CommandsDatabase
            // 
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(536, 676);
            this.Controls.Add(this.criteriaGroupBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.startSearchButton);
            this.Controls.Add(this.cmdSearchTextBox);
            this.Controls.Add(this.scriptcmdDataGridView);
            this.DoubleBuffered = true;
            this.Name = "CommandsDatabase";
            this.Text = "Script Commands Database";
            ((System.ComponentModel.ISupportInitialize)(this.scriptcmdDataGridView)).EndInit();
            this.criteriaGroupBox.ResumeLayout(false);
            this.criteriaGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView scriptcmdDataGridView;
        private System.Windows.Forms.TextBox cmdSearchTextBox;
        private System.Windows.Forms.Button startSearchButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox criteriaGroupBox;
        private System.Windows.Forms.RadioButton matchCB;
        private System.Windows.Forms.RadioButton containsCB;
        private System.Windows.Forms.RadioButton startsWithCB;
        private System.Windows.Forms.DataGridViewTextBoxColumn CommandID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CommandName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParamsCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Params;
    }
}