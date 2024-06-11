
namespace DSPRE.Editors
{
  partial class HeadbuttEncounterEditorTab
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

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
            this.buttonDuplicateTreeGroup = new System.Windows.Forms.Button();
            this.buttonRemoveTreeGroup = new System.Windows.Forms.Button();
            this.numericUpDownMaxLevel = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMinLevel = new System.Windows.Forms.NumericUpDown();
            this.buttonAddTreeGroup = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxPokemon = new System.Windows.Forms.ComboBox();
            this.listBoxTreeGroups = new DSPRE.ListBox2();
            this.listBoxTrees = new DSPRE.ListBox2();
            this.listBoxEncounters = new DSPRE.ListBox2();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonDuplicateTreeGroup
            // 
            this.buttonDuplicateTreeGroup.Image = global::DSPRE.Properties.Resources.copyIcon_small;
            this.buttonDuplicateTreeGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonDuplicateTreeGroup.Location = new System.Drawing.Point(177, 386);
            this.buttonDuplicateTreeGroup.Name = "buttonDuplicateTreeGroup";
            this.buttonDuplicateTreeGroup.Size = new System.Drawing.Size(81, 26);
            this.buttonDuplicateTreeGroup.TabIndex = 17;
            this.buttonDuplicateTreeGroup.Text = "Duplicate";
            this.buttonDuplicateTreeGroup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonDuplicateTreeGroup.UseVisualStyleBackColor = true;
            this.buttonDuplicateTreeGroup.Click += new System.EventHandler(this.buttonDuplicateTreeGroup_Click);
            // 
            // buttonRemoveTreeGroup
            // 
            this.buttonRemoveTreeGroup.Image = global::DSPRE.Properties.Resources.deleteIcon;
            this.buttonRemoveTreeGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRemoveTreeGroup.Location = new System.Drawing.Point(90, 386);
            this.buttonRemoveTreeGroup.Name = "buttonRemoveTreeGroup";
            this.buttonRemoveTreeGroup.Size = new System.Drawing.Size(81, 26);
            this.buttonRemoveTreeGroup.TabIndex = 17;
            this.buttonRemoveTreeGroup.Text = "Remove";
            this.buttonRemoveTreeGroup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRemoveTreeGroup.UseVisualStyleBackColor = true;
            this.buttonRemoveTreeGroup.Click += new System.EventHandler(this.buttonRemoveTreeGroup_Click);
            // 
            // numericUpDownMaxLevel
            // 
            this.numericUpDownMaxLevel.Location = new System.Drawing.Point(200, 210);
            this.numericUpDownMaxLevel.Name = "numericUpDownMaxLevel";
            this.numericUpDownMaxLevel.Size = new System.Drawing.Size(58, 20);
            this.numericUpDownMaxLevel.TabIndex = 15;
            this.numericUpDownMaxLevel.ValueChanged += new System.EventHandler(this.numericUpDownMaxLevel_ValueChanged);
            // 
            // numericUpDownMinLevel
            // 
            this.numericUpDownMinLevel.Location = new System.Drawing.Point(136, 210);
            this.numericUpDownMinLevel.Name = "numericUpDownMinLevel";
            this.numericUpDownMinLevel.Size = new System.Drawing.Size(58, 20);
            this.numericUpDownMinLevel.TabIndex = 15;
            this.numericUpDownMinLevel.ValueChanged += new System.EventHandler(this.numericUpDownMinLevel_ValueChanged);
            // 
            // buttonAddTreeGroup
            // 
            this.buttonAddTreeGroup.Image = global::DSPRE.Properties.Resources.addIcon;
            this.buttonAddTreeGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonAddTreeGroup.Location = new System.Drawing.Point(3, 386);
            this.buttonAddTreeGroup.Name = "buttonAddTreeGroup";
            this.buttonAddTreeGroup.Size = new System.Drawing.Size(81, 26);
            this.buttonAddTreeGroup.TabIndex = 17;
            this.buttonAddTreeGroup.Text = "Add";
            this.buttonAddTreeGroup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAddTreeGroup.UseVisualStyleBackColor = true;
            this.buttonAddTreeGroup.Click += new System.EventHandler(this.buttonAddTreeGroup_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 233);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Tree Groups";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 415);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Trees";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 193);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Pokemon";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(133, 193);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Min Level";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Encounter Table";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(197, 193);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Max Level";
            // 
            // comboBoxPokemon
            // 
            this.comboBoxPokemon.FormattingEnabled = true;
            this.comboBoxPokemon.Location = new System.Drawing.Point(3, 209);
            this.comboBoxPokemon.Name = "comboBoxPokemon";
            this.comboBoxPokemon.Size = new System.Drawing.Size(127, 21);
            this.comboBoxPokemon.TabIndex = 19;
            this.comboBoxPokemon.SelectedIndexChanged += new System.EventHandler(this.comboBoxPokemon_SelectedIndexChanged);
            // 
            // listBoxTreeGroups
            // 
            this.listBoxTreeGroups.DisplayMember = "DisplayName";
            this.listBoxTreeGroups.FormattingEnabled = true;
            this.listBoxTreeGroups.Location = new System.Drawing.Point(3, 249);
            this.listBoxTreeGroups.Name = "listBoxTreeGroups";
            this.listBoxTreeGroups.Size = new System.Drawing.Size(255, 134);
            this.listBoxTreeGroups.TabIndex = 11;
            this.listBoxTreeGroups.SelectedIndexChanged += new System.EventHandler(this.listBoxTreeGroups_SelectedIndexChanged);
            // 
            // listBoxTrees
            // 
            this.listBoxTrees.DisplayMember = "DisplayName";
            this.listBoxTrees.FormattingEnabled = true;
            this.listBoxTrees.Location = new System.Drawing.Point(3, 431);
            this.listBoxTrees.Name = "listBoxTrees";
            this.listBoxTrees.Size = new System.Drawing.Size(255, 82);
            this.listBoxTrees.TabIndex = 11;
            // 
            // listBoxEncounters
            // 
            this.listBoxEncounters.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxEncounters.FormattingEnabled = true;
            this.listBoxEncounters.ItemHeight = 14;
            this.listBoxEncounters.Location = new System.Drawing.Point(3, 15);
            this.listBoxEncounters.Name = "listBoxEncounters";
            this.listBoxEncounters.Size = new System.Drawing.Size(255, 172);
            this.listBoxEncounters.TabIndex = 13;
            this.listBoxEncounters.SelectedIndexChanged += new System.EventHandler(this.listBoxEncounters_SelectedIndexChanged);
            // 
            // HeadbuttEncounterEditorTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboBoxPokemon);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxTreeGroups);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonDuplicateTreeGroup);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonRemoveTreeGroup);
            this.Controls.Add(this.listBoxTrees);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericUpDownMaxLevel);
            this.Controls.Add(this.listBoxEncounters);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericUpDownMinLevel);
            this.Controls.Add(this.buttonAddTreeGroup);
            this.Name = "HeadbuttEncounterEditorTab";
            this.Size = new System.Drawing.Size(262, 517);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    public DSPRE.ListBox2 listBoxTreeGroups;
    private System.Windows.Forms.Button buttonDuplicateTreeGroup;
    private System.Windows.Forms.Button buttonRemoveTreeGroup;
    public ListBox2 listBoxTrees;
    public System.Windows.Forms.NumericUpDown numericUpDownMaxLevel;
    public DSPRE.ListBox2 listBoxEncounters;
    public System.Windows.Forms.NumericUpDown numericUpDownMinLevel;
    private System.Windows.Forms.Button buttonAddTreeGroup;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label4;
    public System.Windows.Forms.ComboBox comboBoxPokemon;
  }
}
