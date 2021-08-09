
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.scriptcmdDataGridView = new System.Windows.Forms.DataGridView();
            this.CommandID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CommandName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParamsCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Params = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.scriptcmdSearchTextBox = new System.Windows.Forms.TextBox();
            this.startSearchButtonScripts = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.criteriaGroupBoxScripts = new System.Windows.Forms.GroupBox();
            this.matchCBScripts = new System.Windows.Forms.RadioButton();
            this.containsCBScripts = new System.Windows.Forms.RadioButton();
            this.startsWithCBScripts = new System.Windows.Forms.RadioButton();
            this.criteriaGroupBoxActions = new System.Windows.Forms.GroupBox();
            this.matchCBActions = new System.Windows.Forms.RadioButton();
            this.containsCBActions = new System.Windows.Forms.RadioButton();
            this.startsWithCBActions = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.startSearchButtonActions = new System.Windows.Forms.Button();
            this.actioncmdSearchTextBox = new System.Windows.Forms.TextBox();
            this.actionDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.compOPDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.scriptcmdDataGridView)).BeginInit();
            this.criteriaGroupBoxScripts.SuspendLayout();
            this.criteriaGroupBoxActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actionDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.compOPDataGridView)).BeginInit();
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
            this.scriptcmdDataGridView.Location = new System.Drawing.Point(14, 63);
            this.scriptcmdDataGridView.Name = "scriptcmdDataGridView";
            this.scriptcmdDataGridView.ReadOnly = true;
            this.scriptcmdDataGridView.RowHeadersVisible = false;
            this.scriptcmdDataGridView.Size = new System.Drawing.Size(506, 622);
            this.scriptcmdDataGridView.TabIndex = 0;
            // 
            // CommandID
            // 
            this.CommandID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Format = "X4";
            this.CommandID.DefaultCellStyle = dataGridViewCellStyle4;
            this.CommandID.FillWeight = 15F;
            this.CommandID.HeaderText = "Command ID";
            this.CommandID.MaxInputLength = 10;
            this.CommandID.Name = "CommandID";
            this.CommandID.ReadOnly = true;
            // 
            // CommandName
            // 
            this.CommandName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CommandName.FillWeight = 35F;
            this.CommandName.HeaderText = "Script Command Name";
            this.CommandName.MaxInputLength = 200;
            this.CommandName.MinimumWidth = 90;
            this.CommandName.Name = "CommandName";
            this.CommandName.ReadOnly = true;
            // 
            // ParamsCount
            // 
            this.ParamsCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ParamsCount.FillWeight = 20F;
            this.ParamsCount.HeaderText = "Parameter Count";
            this.ParamsCount.MaxInputLength = 10;
            this.ParamsCount.MinimumWidth = 20;
            this.ParamsCount.Name = "ParamsCount";
            this.ParamsCount.ReadOnly = true;
            // 
            // Params
            // 
            this.Params.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Params.FillWeight = 30F;
            this.Params.HeaderText = "Parameters";
            this.Params.MaxInputLength = 200;
            this.Params.MinimumWidth = 85;
            this.Params.Name = "Params";
            this.Params.ReadOnly = true;
            // 
            // scriptcmdSearchTextBox
            // 
            this.scriptcmdSearchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scriptcmdSearchTextBox.Location = new System.Drawing.Point(14, 26);
            this.scriptcmdSearchTextBox.Name = "scriptcmdSearchTextBox";
            this.scriptcmdSearchTextBox.Size = new System.Drawing.Size(141, 22);
            this.scriptcmdSearchTextBox.TabIndex = 1;
            this.scriptcmdSearchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.scriptcmdSearchTextBox_KeyDown);
            // 
            // startSearchButtonScripts
            // 
            this.startSearchButtonScripts.Image = global::DSPRE.Properties.Resources.wideLensImage;
            this.startSearchButtonScripts.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.startSearchButtonScripts.Location = new System.Drawing.Point(433, 15);
            this.startSearchButtonScripts.Name = "startSearchButtonScripts";
            this.startSearchButtonScripts.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.startSearchButtonScripts.Size = new System.Drawing.Size(87, 43);
            this.startSearchButtonScripts.TabIndex = 17;
            this.startSearchButtonScripts.Text = "Start\r\nSearch";
            this.startSearchButtonScripts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.startSearchButtonScripts.UseVisualStyleBackColor = true;
            this.startSearchButtonScripts.Click += new System.EventHandler(this.startSearchButtonScripts_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Command name or ID:";
            // 
            // criteriaGroupBoxScripts
            // 
            this.criteriaGroupBoxScripts.Controls.Add(this.matchCBScripts);
            this.criteriaGroupBoxScripts.Controls.Add(this.containsCBScripts);
            this.criteriaGroupBoxScripts.Controls.Add(this.startsWithCBScripts);
            this.criteriaGroupBoxScripts.Location = new System.Drawing.Point(161, 10);
            this.criteriaGroupBoxScripts.Name = "criteriaGroupBoxScripts";
            this.criteriaGroupBoxScripts.Size = new System.Drawing.Size(266, 46);
            this.criteriaGroupBoxScripts.TabIndex = 19;
            this.criteriaGroupBoxScripts.TabStop = false;
            this.criteriaGroupBoxScripts.Text = "Search Criteria";
            // 
            // matchCBScripts
            // 
            this.matchCBScripts.Appearance = System.Windows.Forms.Appearance.Button;
            this.matchCBScripts.AutoSize = true;
            this.matchCBScripts.Location = new System.Drawing.Point(144, 15);
            this.matchCBScripts.Name = "matchCBScripts";
            this.matchCBScripts.Size = new System.Drawing.Size(113, 23);
            this.matchCBScripts.TabIndex = 2;
            this.matchCBScripts.Text = "Match (Ignore Case)";
            this.matchCBScripts.UseVisualStyleBackColor = true;
            // 
            // containsCBScripts
            // 
            this.containsCBScripts.Appearance = System.Windows.Forms.Appearance.Button;
            this.containsCBScripts.AutoSize = true;
            this.containsCBScripts.Checked = true;
            this.containsCBScripts.Location = new System.Drawing.Point(8, 15);
            this.containsCBScripts.Name = "containsCBScripts";
            this.containsCBScripts.Size = new System.Drawing.Size(58, 23);
            this.containsCBScripts.TabIndex = 1;
            this.containsCBScripts.TabStop = true;
            this.containsCBScripts.Text = "Contains";
            this.containsCBScripts.UseVisualStyleBackColor = true;
            // 
            // startsWithCBScripts
            // 
            this.startsWithCBScripts.Appearance = System.Windows.Forms.Appearance.Button;
            this.startsWithCBScripts.AutoSize = true;
            this.startsWithCBScripts.Location = new System.Drawing.Point(72, 15);
            this.startsWithCBScripts.Name = "startsWithCBScripts";
            this.startsWithCBScripts.Size = new System.Drawing.Size(66, 23);
            this.startsWithCBScripts.TabIndex = 0;
            this.startsWithCBScripts.Text = "Starts with";
            this.startsWithCBScripts.UseVisualStyleBackColor = true;
            // 
            // criteriaGroupBoxActions
            // 
            this.criteriaGroupBoxActions.Controls.Add(this.matchCBActions);
            this.criteriaGroupBoxActions.Controls.Add(this.containsCBActions);
            this.criteriaGroupBoxActions.Controls.Add(this.startsWithCBActions);
            this.criteriaGroupBoxActions.Location = new System.Drawing.Point(703, 9);
            this.criteriaGroupBoxActions.Name = "criteriaGroupBoxActions";
            this.criteriaGroupBoxActions.Size = new System.Drawing.Size(266, 46);
            this.criteriaGroupBoxActions.TabIndex = 24;
            this.criteriaGroupBoxActions.TabStop = false;
            this.criteriaGroupBoxActions.Text = "Search Criteria";
            // 
            // matchCBActions
            // 
            this.matchCBActions.Appearance = System.Windows.Forms.Appearance.Button;
            this.matchCBActions.AutoSize = true;
            this.matchCBActions.Location = new System.Drawing.Point(144, 15);
            this.matchCBActions.Name = "matchCBActions";
            this.matchCBActions.Size = new System.Drawing.Size(113, 23);
            this.matchCBActions.TabIndex = 2;
            this.matchCBActions.Text = "Match (Ignore Case)";
            this.matchCBActions.UseVisualStyleBackColor = true;
            // 
            // containsCBActions
            // 
            this.containsCBActions.Appearance = System.Windows.Forms.Appearance.Button;
            this.containsCBActions.AutoSize = true;
            this.containsCBActions.Checked = true;
            this.containsCBActions.Location = new System.Drawing.Point(8, 15);
            this.containsCBActions.Name = "containsCBActions";
            this.containsCBActions.Size = new System.Drawing.Size(58, 23);
            this.containsCBActions.TabIndex = 1;
            this.containsCBActions.TabStop = true;
            this.containsCBActions.Text = "Contains";
            this.containsCBActions.UseVisualStyleBackColor = true;
            // 
            // startsWithCBActions
            // 
            this.startsWithCBActions.Appearance = System.Windows.Forms.Appearance.Button;
            this.startsWithCBActions.AutoSize = true;
            this.startsWithCBActions.Location = new System.Drawing.Point(72, 15);
            this.startsWithCBActions.Name = "startsWithCBActions";
            this.startsWithCBActions.Size = new System.Drawing.Size(66, 23);
            this.startsWithCBActions.TabIndex = 0;
            this.startsWithCBActions.Text = "Starts with";
            this.startsWithCBActions.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(554, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Action name or ID:";
            // 
            // startSearchButtonActions
            // 
            this.startSearchButtonActions.Image = global::DSPRE.Properties.Resources.wideLensImage;
            this.startSearchButtonActions.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.startSearchButtonActions.Location = new System.Drawing.Point(975, 14);
            this.startSearchButtonActions.Name = "startSearchButtonActions";
            this.startSearchButtonActions.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.startSearchButtonActions.Size = new System.Drawing.Size(87, 43);
            this.startSearchButtonActions.TabIndex = 22;
            this.startSearchButtonActions.Text = "Start\r\nSearch";
            this.startSearchButtonActions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.startSearchButtonActions.UseVisualStyleBackColor = true;
            this.startSearchButtonActions.Click += new System.EventHandler(this.startSearchButtonActions_Click);
            // 
            // actioncmdSearchTextBox
            // 
            this.actioncmdSearchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.actioncmdSearchTextBox.Location = new System.Drawing.Point(556, 25);
            this.actioncmdSearchTextBox.Name = "actioncmdSearchTextBox";
            this.actioncmdSearchTextBox.Size = new System.Drawing.Size(141, 22);
            this.actioncmdSearchTextBox.TabIndex = 21;
            this.actioncmdSearchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.actioncmdSearchTextBox_KeyDown);
            // 
            // actionDataGridView
            // 
            this.actionDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.actionDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.actionDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.actionDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.actionDataGridView.Location = new System.Drawing.Point(556, 62);
            this.actionDataGridView.Name = "actionDataGridView";
            this.actionDataGridView.ReadOnly = true;
            this.actionDataGridView.RowHeadersVisible = false;
            this.actionDataGridView.Size = new System.Drawing.Size(245, 622);
            this.actionDataGridView.TabIndex = 20;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.Format = "X4";
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn1.FillWeight = 30F;
            this.dataGridViewTextBoxColumn1.HeaderText = "Command ID";
            this.dataGridViewTextBoxColumn1.MaxInputLength = 10;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.FillWeight = 60F;
            this.dataGridViewTextBoxColumn2.HeaderText = "Action Command Name";
            this.dataGridViewTextBoxColumn2.MaxInputLength = 200;
            this.dataGridViewTextBoxColumn2.MinimumWidth = 90;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // compOPDataGridView
            // 
            this.compOPDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.compOPDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.compOPDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.compOPDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.compOPDataGridView.Location = new System.Drawing.Point(817, 62);
            this.compOPDataGridView.Name = "compOPDataGridView";
            this.compOPDataGridView.ReadOnly = true;
            this.compOPDataGridView.RowHeadersVisible = false;
            this.compOPDataGridView.Size = new System.Drawing.Size(245, 622);
            this.compOPDataGridView.TabIndex = 25;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.Format = "X4";
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewTextBoxColumn3.FillWeight = 30F;
            this.dataGridViewTextBoxColumn3.HeaderText = "Operator ID";
            this.dataGridViewTextBoxColumn3.MaxInputLength = 10;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn4.FillWeight = 60F;
            this.dataGridViewTextBoxColumn4.HeaderText = "Comparison Operator Name";
            this.dataGridViewTextBoxColumn4.MaxInputLength = 200;
            this.dataGridViewTextBoxColumn4.MinimumWidth = 90;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // CommandsDatabase
            // 
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1075, 697);
            this.Controls.Add(this.compOPDataGridView);
            this.Controls.Add(this.criteriaGroupBoxActions);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.startSearchButtonActions);
            this.Controls.Add(this.actioncmdSearchTextBox);
            this.Controls.Add(this.actionDataGridView);
            this.Controls.Add(this.criteriaGroupBoxScripts);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.startSearchButtonScripts);
            this.Controls.Add(this.scriptcmdSearchTextBox);
            this.Controls.Add(this.scriptcmdDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommandsDatabase";
            this.Text = "Script Commands Database";
            ((System.ComponentModel.ISupportInitialize)(this.scriptcmdDataGridView)).EndInit();
            this.criteriaGroupBoxScripts.ResumeLayout(false);
            this.criteriaGroupBoxScripts.PerformLayout();
            this.criteriaGroupBoxActions.ResumeLayout(false);
            this.criteriaGroupBoxActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actionDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.compOPDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView scriptcmdDataGridView;
        private System.Windows.Forms.TextBox scriptcmdSearchTextBox;
        private System.Windows.Forms.Button startSearchButtonScripts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox criteriaGroupBoxScripts;
        private System.Windows.Forms.RadioButton matchCBScripts;
        private System.Windows.Forms.RadioButton containsCBScripts;
        private System.Windows.Forms.RadioButton startsWithCBScripts;
        private System.Windows.Forms.GroupBox criteriaGroupBoxActions;
        private System.Windows.Forms.RadioButton matchCBActions;
        private System.Windows.Forms.RadioButton containsCBActions;
        private System.Windows.Forms.RadioButton startsWithCBActions;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button startSearchButtonActions;
        private System.Windows.Forms.TextBox actioncmdSearchTextBox;
        private System.Windows.Forms.DataGridView actionDataGridView;
        private System.Windows.Forms.DataGridView compOPDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn CommandID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CommandName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParamsCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Params;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}