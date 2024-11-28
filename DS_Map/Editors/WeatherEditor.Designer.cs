namespace DSPRE.Editors
{
    partial class WeatherEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WeatherEditor));
            this.weatherPicture = new System.Windows.Forms.PictureBox();
            this.weatherLabel = new System.Windows.Forms.Label();
            this.weatherSelector = new System.Windows.Forms.ComboBox();
            this.weatherUpOrDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.totalPercentage = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.weatherApparitionPercentage = new System.Windows.Forms.NumericUpDown();
            this.totalPercentageFill = new System.Windows.Forms.ProgressBar();
            this.addWeatherButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.WeatherId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WeatherName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Percentage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Scripts = new System.Windows.Forms.TabPage();
            this.scriptCodeView = new ScintillaNET.Scintilla();
            this.Functions = new System.Windows.Forms.TabPage();
            this.functionsPanel = new System.Windows.Forms.Panel();
            this.removeLastRow = new System.Windows.Forms.Button();
            this.removeFirstRow = new System.Windows.Forms.Button();
            this.randomVarNum = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.weatherVarNum = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.currWeatherVarNum = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.functionCodeView = new ScintillaNET.Scintilla();
            ((System.ComponentModel.ISupportInitialize)(this.weatherPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weatherUpOrDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weatherApparitionPercentage)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.Scripts.SuspendLayout();
            this.Functions.SuspendLayout();
            this.functionsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.randomVarNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weatherVarNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currWeatherVarNum)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // weatherPicture
            // 
            this.weatherPicture.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.weatherPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.weatherPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.weatherPicture.Location = new System.Drawing.Point(416, 59);
            this.weatherPicture.Name = "weatherPicture";
            this.weatherPicture.Size = new System.Drawing.Size(256, 192);
            this.weatherPicture.TabIndex = 0;
            this.weatherPicture.TabStop = false;
            // 
            // weatherLabel
            // 
            this.weatherLabel.AutoSize = true;
            this.weatherLabel.Location = new System.Drawing.Point(18, 21);
            this.weatherLabel.Name = "weatherLabel";
            this.weatherLabel.Size = new System.Drawing.Size(48, 13);
            this.weatherLabel.TabIndex = 1;
            this.weatherLabel.Text = "Weather";
            // 
            // weatherSelector
            // 
            this.weatherSelector.FormattingEnabled = true;
            this.weatherSelector.Location = new System.Drawing.Point(61, 42);
            this.weatherSelector.Name = "weatherSelector";
            this.weatherSelector.Size = new System.Drawing.Size(206, 21);
            this.weatherSelector.TabIndex = 2;
            this.weatherSelector.SelectedIndexChanged += new System.EventHandler(this.weatherSelector_SelectedIndexChanged);
            // 
            // weatherUpOrDown
            // 
            this.weatherUpOrDown.Location = new System.Drawing.Point(21, 42);
            this.weatherUpOrDown.Name = "weatherUpOrDown";
            this.weatherUpOrDown.Size = new System.Drawing.Size(34, 20);
            this.weatherUpOrDown.TabIndex = 3;
            this.weatherUpOrDown.ValueChanged += new System.EventHandler(this.weatherUpOrDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Total Percentage";
            // 
            // totalPercentage
            // 
            this.totalPercentage.Location = new System.Drawing.Point(21, 33);
            this.totalPercentage.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.totalPercentage.Name = "totalPercentage";
            this.totalPercentage.Size = new System.Drawing.Size(114, 20);
            this.totalPercentage.TabIndex = 5;
            this.totalPercentage.ValueChanged += new System.EventHandler(this.totalPercentage_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(270, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Weather Percentage";
            // 
            // weatherApparitionPercentage
            // 
            this.weatherApparitionPercentage.Location = new System.Drawing.Point(273, 43);
            this.weatherApparitionPercentage.Name = "weatherApparitionPercentage";
            this.weatherApparitionPercentage.Size = new System.Drawing.Size(114, 20);
            this.weatherApparitionPercentage.TabIndex = 7;
            // 
            // totalPercentageFill
            // 
            this.totalPercentageFill.Location = new System.Drawing.Point(12, 257);
            this.totalPercentageFill.Name = "totalPercentageFill";
            this.totalPercentageFill.Size = new System.Drawing.Size(661, 23);
            this.totalPercentageFill.TabIndex = 8;
            // 
            // addWeatherButton
            // 
            this.addWeatherButton.Image = global::DSPRE.Properties.Resources.addIcon;
            this.addWeatherButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addWeatherButton.Location = new System.Drawing.Point(21, 69);
            this.addWeatherButton.Name = "addWeatherButton";
            this.addWeatherButton.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.addWeatherButton.Size = new System.Drawing.Size(366, 31);
            this.addWeatherButton.TabIndex = 9;
            this.addWeatherButton.Text = "Add Weather";
            this.addWeatherButton.UseVisualStyleBackColor = true;
            this.addWeatherButton.Click += new System.EventHandler(this.addWeatherButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.weatherLabel);
            this.groupBox1.Controls.Add(this.weatherSelector);
            this.groupBox1.Controls.Add(this.addWeatherButton);
            this.groupBox1.Controls.Add(this.weatherUpOrDown);
            this.groupBox1.Controls.Add(this.weatherApparitionPercentage);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(11, 135);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(398, 116);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Weather Settings";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.WeatherId,
            this.WeatherName,
            this.Percentage});
            this.dataGridView1.Location = new System.Drawing.Point(11, 295);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(661, 166);
            this.dataGridView1.TabIndex = 11;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // WeatherId
            // 
            this.WeatherId.HeaderText = "Weather ID";
            this.WeatherId.Name = "WeatherId";
            this.WeatherId.ReadOnly = true;
            // 
            // WeatherName
            // 
            this.WeatherName.HeaderText = "Weather Name";
            this.WeatherName.Name = "WeatherName";
            this.WeatherName.ReadOnly = true;
            // 
            // Percentage
            // 
            this.Percentage.HeaderText = "Percentage";
            this.Percentage.Name = "Percentage";
            this.Percentage.ReadOnly = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Scripts);
            this.tabControl1.Controls.Add(this.Functions);
            this.tabControl1.Location = new System.Drawing.Point(7, 491);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(665, 284);
            this.tabControl1.TabIndex = 12;
            // 
            // Scripts
            // 
            this.Scripts.Controls.Add(this.scriptCodeView);
            this.Scripts.Location = new System.Drawing.Point(4, 22);
            this.Scripts.Name = "Scripts";
            this.Scripts.Padding = new System.Windows.Forms.Padding(3);
            this.Scripts.Size = new System.Drawing.Size(657, 258);
            this.Scripts.TabIndex = 0;
            this.Scripts.Text = "Scripts";
            this.Scripts.UseVisualStyleBackColor = true;
            // 
            // scriptCodeView
            // 
            this.scriptCodeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scriptCodeView.EdgeColor = System.Drawing.Color.Black;
            this.scriptCodeView.Location = new System.Drawing.Point(0, 0);
            this.scriptCodeView.Name = "scriptCodeView";
            this.scriptCodeView.Size = new System.Drawing.Size(661, 255);
            this.scriptCodeView.TabIndex = 0;
            // 
            // Functions
            // 
            this.Functions.Controls.Add(this.functionsPanel);
            this.Functions.Location = new System.Drawing.Point(4, 22);
            this.Functions.Name = "Functions";
            this.Functions.Padding = new System.Windows.Forms.Padding(3);
            this.Functions.Size = new System.Drawing.Size(657, 258);
            this.Functions.TabIndex = 1;
            this.Functions.Text = "Functions";
            this.Functions.UseVisualStyleBackColor = true;
            // 
            // functionsPanel
            // 
            this.functionsPanel.Controls.Add(this.functionCodeView);
            this.functionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.functionsPanel.Location = new System.Drawing.Point(3, 3);
            this.functionsPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.functionsPanel.Name = "functionsPanel";
            this.functionsPanel.Size = new System.Drawing.Size(651, 252);
            this.functionsPanel.TabIndex = 21;
            // 
            // removeLastRow
            // 
            this.removeLastRow.Image = global::DSPRE.Properties.Resources.deleteIcon;
            this.removeLastRow.Location = new System.Drawing.Point(501, 467);
            this.removeLastRow.Name = "removeLastRow";
            this.removeLastRow.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.removeLastRow.Size = new System.Drawing.Size(171, 31);
            this.removeLastRow.TabIndex = 13;
            this.removeLastRow.Text = "Remove Last Row";
            this.removeLastRow.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.removeLastRow.UseVisualStyleBackColor = true;
            this.removeLastRow.Click += new System.EventHandler(this.removeLastRow_Click);
            // 
            // removeFirstRow
            // 
            this.removeFirstRow.Image = global::DSPRE.Properties.Resources.deleteIcon;
            this.removeFirstRow.Location = new System.Drawing.Point(324, 467);
            this.removeFirstRow.Name = "removeFirstRow";
            this.removeFirstRow.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.removeFirstRow.Size = new System.Drawing.Size(171, 31);
            this.removeFirstRow.TabIndex = 14;
            this.removeFirstRow.Text = "Remove First Row";
            this.removeFirstRow.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.removeFirstRow.UseVisualStyleBackColor = true;
            this.removeFirstRow.Click += new System.EventHandler(this.removeFirstRow_Click);
            // 
            // randomVarNum
            // 
            this.randomVarNum.Location = new System.Drawing.Point(23, 36);
            this.randomVarNum.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.randomVarNum.Name = "randomVarNum";
            this.randomVarNum.Size = new System.Drawing.Size(101, 20);
            this.randomVarNum.TabIndex = 15;
            this.randomVarNum.ValueChanged += new System.EventHandler(this.randomVarNum_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Random Variable";
            // 
            // weatherVarNum
            // 
            this.weatherVarNum.Location = new System.Drawing.Point(143, 36);
            this.weatherVarNum.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.weatherVarNum.Name = "weatherVarNum";
            this.weatherVarNum.Size = new System.Drawing.Size(85, 20);
            this.weatherVarNum.TabIndex = 17;
            this.weatherVarNum.ValueChanged += new System.EventHandler(this.weatherVarNum_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(140, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Weather Variable";
            // 
            // currWeatherVarNum
            // 
            this.currWeatherVarNum.Location = new System.Drawing.Point(245, 36);
            this.currWeatherVarNum.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.currWeatherVarNum.Name = "currWeatherVarNum";
            this.currWeatherVarNum.Size = new System.Drawing.Size(85, 20);
            this.currWeatherVarNum.TabIndex = 19;
            this.currWeatherVarNum.ValueChanged += new System.EventHandler(this.currWeatherVarNum_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(242, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(145, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Current Weather Var (Debug)";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.randomVarNum);
            this.groupBox2.Controls.Add(this.currWeatherVarNum);
            this.groupBox2.Controls.Add(this.weatherVarNum);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(11, 59);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(398, 76);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Variables";
            // 
            // functionCodeView
            // 
            this.functionCodeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.functionCodeView.EdgeColor = System.Drawing.Color.Black;
            this.functionCodeView.Location = new System.Drawing.Point(-5, -1);
            this.functionCodeView.Name = "functionCodeView";
            this.functionCodeView.Size = new System.Drawing.Size(661, 255);
            this.functionCodeView.TabIndex = 1;
            // 
            // WeatherEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(681, 791);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.removeFirstRow);
            this.Controls.Add(this.removeLastRow);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.totalPercentageFill);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.totalPercentage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.weatherPicture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WeatherEditor";
            this.Text = "Weather Editor";
            this.Load += new System.EventHandler(this.WeatherEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.weatherPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weatherUpOrDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weatherApparitionPercentage)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.Scripts.ResumeLayout(false);
            this.Functions.ResumeLayout(false);
            this.functionsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.randomVarNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weatherVarNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currWeatherVarNum)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox weatherPicture;
        private System.Windows.Forms.Label weatherLabel;
        private System.Windows.Forms.ComboBox weatherSelector;
        private System.Windows.Forms.NumericUpDown weatherUpOrDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown totalPercentage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown weatherApparitionPercentage;
        private System.Windows.Forms.ProgressBar totalPercentageFill;
        private System.Windows.Forms.Button addWeatherButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn WeatherId;
        private System.Windows.Forms.DataGridViewTextBoxColumn WeatherName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Percentage;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Functions;
        private System.Windows.Forms.Button removeLastRow;
        private System.Windows.Forms.Button removeFirstRow;
        private System.Windows.Forms.Panel functionsPanel;
        private System.Windows.Forms.TabPage Scripts;
        private ScintillaNET.Scintilla scriptCodeView;
        private System.Windows.Forms.NumericUpDown randomVarNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown weatherVarNum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown currWeatherVarNum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private ScintillaNET.Scintilla functionCodeView;
    }
}