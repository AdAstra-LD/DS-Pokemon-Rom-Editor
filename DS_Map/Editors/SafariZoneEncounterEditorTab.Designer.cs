
namespace DSPRE.Editors
{
  partial class SafariZoneEncounterEditorTab
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
    private void InitializeComponent()
    {
      this.numericUpDownLevel = new System.Windows.Forms.NumericUpDown();
      this.comboBoxPokemon = new System.Windows.Forms.ComboBox();
      this.numericUpDownLevelObject = new System.Windows.Forms.NumericUpDown();
      this.comboBoxPokemonObject = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.listBoxEncountersObject = new DSPRE.ListBox2();
      this.listBoxEncounters = new DSPRE.ListBox2();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevelObject)).BeginInit();
      this.SuspendLayout();
      // 
      // numericUpDownLevel
      // 
      this.numericUpDownLevel.Location = new System.Drawing.Point(139, 180);
      this.numericUpDownLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDownLevel.Name = "numericUpDownLevel";
      this.numericUpDownLevel.Size = new System.Drawing.Size(58, 20);
      this.numericUpDownLevel.TabIndex = 20;
      this.numericUpDownLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDownLevel.ValueChanged += new System.EventHandler(this.numericUpDownLevel_ValueChanged);
      // 
      // comboBoxPokemon
      // 
      this.comboBoxPokemon.FormattingEnabled = true;
      this.comboBoxPokemon.Location = new System.Drawing.Point(6, 179);
      this.comboBoxPokemon.Name = "comboBoxPokemon";
      this.comboBoxPokemon.Size = new System.Drawing.Size(127, 21);
      this.comboBoxPokemon.TabIndex = 21;
      this.comboBoxPokemon.SelectedIndexChanged += new System.EventHandler(this.comboBoxPokemon_SelectedIndexChanged);
      // 
      // numericUpDownLevelObject
      // 
      this.numericUpDownLevelObject.Location = new System.Drawing.Point(339, 180);
      this.numericUpDownLevelObject.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDownLevelObject.Name = "numericUpDownLevelObject";
      this.numericUpDownLevelObject.Size = new System.Drawing.Size(58, 20);
      this.numericUpDownLevelObject.TabIndex = 20;
      this.numericUpDownLevelObject.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDownLevelObject.ValueChanged += new System.EventHandler(this.numericUpDownLevelObject_ValueChanged);
      // 
      // comboBoxPokemonObject
      // 
      this.comboBoxPokemonObject.FormattingEnabled = true;
      this.comboBoxPokemonObject.Location = new System.Drawing.Point(206, 179);
      this.comboBoxPokemonObject.Name = "comboBoxPokemonObject";
      this.comboBoxPokemonObject.Size = new System.Drawing.Size(127, 21);
      this.comboBoxPokemonObject.TabIndex = 21;
      this.comboBoxPokemonObject.SelectedIndexChanged += new System.EventHandler(this.comboBoxPokemonObject_SelectedIndexChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(86, 13);
      this.label2.TabIndex = 22;
      this.label2.Text = "Encounter Table";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(203, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(120, 13);
      this.label1.TabIndex = 22;
      this.label1.Text = "Object Encounter Table";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(136, 163);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(33, 13);
      this.label3.TabIndex = 23;
      this.label3.Text = "Level";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 163);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(52, 13);
      this.label4.TabIndex = 24;
      this.label4.Text = "Pokemon";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(203, 163);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(52, 13);
      this.label5.TabIndex = 24;
      this.label5.Text = "Pokemon";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(336, 163);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(33, 13);
      this.label6.TabIndex = 23;
      this.label6.Text = "Level";
      // 
      // listBoxEncountersObject
      // 
      this.listBoxEncountersObject.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.listBoxEncountersObject.FormattingEnabled = true;
      this.listBoxEncountersObject.ItemHeight = 14;
      this.listBoxEncountersObject.Location = new System.Drawing.Point(206, 16);
      this.listBoxEncountersObject.Name = "listBoxEncountersObject";
      this.listBoxEncountersObject.Size = new System.Drawing.Size(191, 144);
      this.listBoxEncountersObject.TabIndex = 1;
      this.listBoxEncountersObject.SelectedIndexChanged += new System.EventHandler(this.listBoxEncountersObject_SelectedIndexChanged);
      // 
      // listBoxEncounters
      // 
      this.listBoxEncounters.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.listBoxEncounters.FormattingEnabled = true;
      this.listBoxEncounters.ItemHeight = 14;
      this.listBoxEncounters.Location = new System.Drawing.Point(6, 16);
      this.listBoxEncounters.Name = "listBoxEncounters";
      this.listBoxEncounters.Size = new System.Drawing.Size(191, 144);
      this.listBoxEncounters.TabIndex = 1;
      this.listBoxEncounters.SelectedIndexChanged += new System.EventHandler(this.listBoxEncounters_SelectedIndexChanged);
      // 
      // SafariZoneEncounterEditorTab
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.comboBoxPokemonObject);
      this.Controls.Add(this.comboBoxPokemon);
      this.Controls.Add(this.numericUpDownLevelObject);
      this.Controls.Add(this.numericUpDownLevel);
      this.Controls.Add(this.listBoxEncountersObject);
      this.Controls.Add(this.listBoxEncounters);
      this.Name = "SafariZoneEncounterEditorTab";
      this.Size = new System.Drawing.Size(404, 208);
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevelObject)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    public ListBox2 listBoxEncounters;
    public System.Windows.Forms.NumericUpDown numericUpDownLevel;
    public System.Windows.Forms.ComboBox comboBoxPokemon;
    public ListBox2 listBoxEncountersObject;
    public System.Windows.Forms.NumericUpDown numericUpDownLevelObject;
    public System.Windows.Forms.ComboBox comboBoxPokemonObject;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
  }
}
