﻿using System.ComponentModel;

namespace DSPRE.Editors {
  partial class LevelScriptEditor {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.buttonSave = new System.Windows.Forms.Button();
      this.buttonExport = new System.Windows.Forms.Button();
      this.radioButtonVariableValue = new System.Windows.Forms.RadioButton();
      this.radioButtonMapChange = new System.Windows.Forms.RadioButton();
      this.radioButtonScreenReset = new System.Windows.Forms.RadioButton();
      this.radioButtonLoadGame = new System.Windows.Forms.RadioButton();
      this.textBoxScriptID = new System.Windows.Forms.TextBox();
      this.textBoxVariableName = new System.Windows.Forms.TextBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.groupBoxVariable = new System.Windows.Forms.GroupBox();
      this.groupBoxValue = new System.Windows.Forms.GroupBox();
      this.textBoxVariableValue = new System.Windows.Forms.TextBox();
      this.checkBoxPadding = new System.Windows.Forms.CheckBox();
      this.buttonAdd = new System.Windows.Forms.Button();
      this.buttonRemove = new System.Windows.Forms.Button();
      this.radioButtonDecimal = new System.Windows.Forms.RadioButton();
      this.radioButtonHex = new System.Windows.Forms.RadioButton();
      this.radioButtonAuto = new System.Windows.Forms.RadioButton();
      this.selectScriptFileComboBox = new System.Windows.Forms.ComboBox();
      this.listBoxTriggers = new DSPRE.ListBox2();
      this.buttonOpenHeaderScript = new System.Windows.Forms.Button();
      this.buttonOpenSelectedScript = new System.Windows.Forms.Button();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.buttonLocate = new System.Windows.Forms.Button();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.buttonImport = new System.Windows.Forms.Button();
      this.groupBox5 = new System.Windows.Forms.GroupBox();
      this.groupBox6 = new System.Windows.Forms.GroupBox();
      this.groupBox7 = new System.Windows.Forms.GroupBox();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBoxVariable.SuspendLayout();
      this.groupBoxValue.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox4.SuspendLayout();
      this.groupBox5.SuspendLayout();
      this.groupBox6.SuspendLayout();
      this.groupBox7.SuspendLayout();
      this.SuspendLayout();
      // 
      // buttonSave
      // 
      this.buttonSave.Location = new System.Drawing.Point(70, 19);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(53, 23);
      this.buttonSave.TabIndex = 5;
      this.buttonSave.Text = "Save";
      this.buttonSave.UseVisualStyleBackColor = true;
      this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
      // 
      // buttonExport
      // 
      this.buttonExport.Location = new System.Drawing.Point(129, 19);
      this.buttonExport.Name = "buttonExport";
      this.buttonExport.Size = new System.Drawing.Size(51, 23);
      this.buttonExport.TabIndex = 6;
      this.buttonExport.Text = "Export";
      this.buttonExport.UseVisualStyleBackColor = true;
      this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
      // 
      // radioButtonVariableValue
      // 
      this.radioButtonVariableValue.AutoSize = true;
      this.radioButtonVariableValue.Checked = true;
      this.radioButtonVariableValue.Location = new System.Drawing.Point(6, 19);
      this.radioButtonVariableValue.Name = "radioButtonVariableValue";
      this.radioButtonVariableValue.Size = new System.Drawing.Size(107, 17);
      this.radioButtonVariableValue.TabIndex = 12;
      this.radioButtonVariableValue.TabStop = true;
      this.radioButtonVariableValue.Text = "Variable == value";
      this.radioButtonVariableValue.UseVisualStyleBackColor = true;
      this.radioButtonVariableValue.CheckedChanged += new System.EventHandler(this.radioButtonVariableValue_CheckedChanged);
      // 
      // radioButtonMapChange
      // 
      this.radioButtonMapChange.AutoSize = true;
      this.radioButtonMapChange.Location = new System.Drawing.Point(119, 19);
      this.radioButtonMapChange.Name = "radioButtonMapChange";
      this.radioButtonMapChange.Size = new System.Drawing.Size(86, 17);
      this.radioButtonMapChange.TabIndex = 13;
      this.radioButtonMapChange.Text = "Player enters";
      this.radioButtonMapChange.UseVisualStyleBackColor = true;
      this.radioButtonMapChange.CheckedChanged += new System.EventHandler(this.radioButtonMapChange_CheckedChanged);
      // 
      // radioButtonScreenReset
      // 
      this.radioButtonScreenReset.AutoSize = true;
      this.radioButtonScreenReset.Location = new System.Drawing.Point(211, 19);
      this.radioButtonScreenReset.Name = "radioButtonScreenReset";
      this.radioButtonScreenReset.Size = new System.Drawing.Size(88, 17);
      this.radioButtonScreenReset.TabIndex = 14;
      this.radioButtonScreenReset.Text = "Screen fades";
      this.radioButtonScreenReset.UseVisualStyleBackColor = true;
      this.radioButtonScreenReset.CheckedChanged += new System.EventHandler(this.radioButtonScreenReset_CheckedChanged);
      // 
      // radioButtonLoadGame
      // 
      this.radioButtonLoadGame.AutoSize = true;
      this.radioButtonLoadGame.Location = new System.Drawing.Point(305, 19);
      this.radioButtonLoadGame.Name = "radioButtonLoadGame";
      this.radioButtonLoadGame.Size = new System.Drawing.Size(81, 17);
      this.radioButtonLoadGame.TabIndex = 15;
      this.radioButtonLoadGame.Text = "Game loads";
      this.radioButtonLoadGame.UseVisualStyleBackColor = true;
      this.radioButtonLoadGame.CheckedChanged += new System.EventHandler(this.radioButtonLoadGame_CheckedChanged);
      // 
      // textBoxScriptID
      // 
      this.textBoxScriptID.Location = new System.Drawing.Point(6, 19);
      this.textBoxScriptID.Name = "textBoxScriptID";
      this.textBoxScriptID.Size = new System.Drawing.Size(116, 20);
      this.textBoxScriptID.TabIndex = 16;
      this.textBoxScriptID.TextChanged += new System.EventHandler(this.textBoxScriptID_TextChanged);
      // 
      // textBoxVariableName
      // 
      this.textBoxVariableName.Location = new System.Drawing.Point(6, 19);
      this.textBoxVariableName.Name = "textBoxVariableName";
      this.textBoxVariableName.Size = new System.Drawing.Size(116, 20);
      this.textBoxVariableName.TabIndex = 17;
      this.textBoxVariableName.TextChanged += new System.EventHandler(this.textBoxVariableName_TextChanged);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.radioButtonVariableValue);
      this.groupBox1.Controls.Add(this.radioButtonMapChange);
      this.groupBox1.Controls.Add(this.radioButtonScreenReset);
      this.groupBox1.Controls.Add(this.radioButtonLoadGame);
      this.groupBox1.Location = new System.Drawing.Point(3, 366);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(397, 48);
      this.groupBox1.TabIndex = 11;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "When this happens in the map:";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.textBoxScriptID);
      this.groupBox2.Location = new System.Drawing.Point(3, 420);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(128, 48);
      this.groupBox2.TabIndex = 12;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Run this script ID";
      // 
      // groupBoxVariable
      // 
      this.groupBoxVariable.Controls.Add(this.textBoxVariableName);
      this.groupBoxVariable.Location = new System.Drawing.Point(137, 420);
      this.groupBoxVariable.Name = "groupBoxVariable";
      this.groupBoxVariable.Size = new System.Drawing.Size(128, 48);
      this.groupBoxVariable.TabIndex = 13;
      this.groupBoxVariable.TabStop = false;
      this.groupBoxVariable.Text = "when this variable ==";
      // 
      // groupBoxValue
      // 
      this.groupBoxValue.Controls.Add(this.textBoxVariableValue);
      this.groupBoxValue.Location = new System.Drawing.Point(271, 420);
      this.groupBoxValue.Name = "groupBoxValue";
      this.groupBoxValue.Size = new System.Drawing.Size(129, 48);
      this.groupBoxValue.TabIndex = 13;
      this.groupBoxValue.TabStop = false;
      this.groupBoxValue.Text = "this value";
      // 
      // textBoxVariableValue
      // 
      this.textBoxVariableValue.Location = new System.Drawing.Point(6, 19);
      this.textBoxVariableValue.Name = "textBoxVariableValue";
      this.textBoxVariableValue.Size = new System.Drawing.Size(117, 20);
      this.textBoxVariableValue.TabIndex = 18;
      this.textBoxVariableValue.TextChanged += new System.EventHandler(this.textBoxVariableValue_TextChanged);
      // 
      // checkBoxPadding
      // 
      this.checkBoxPadding.AutoSize = true;
      this.checkBoxPadding.Location = new System.Drawing.Point(186, 23);
      this.checkBoxPadding.Name = "checkBoxPadding";
      this.checkBoxPadding.Size = new System.Drawing.Size(141, 17);
      this.checkBoxPadding.TabIndex = 7;
      this.checkBoxPadding.Text = "Word-alignment padding";
      this.checkBoxPadding.UseVisualStyleBackColor = true;
      // 
      // buttonAdd
      // 
      this.buttonAdd.Location = new System.Drawing.Point(6, 19);
      this.buttonAdd.Name = "buttonAdd";
      this.buttonAdd.Size = new System.Drawing.Size(75, 23);
      this.buttonAdd.TabIndex = 19;
      this.buttonAdd.Text = "Add";
      this.buttonAdd.UseVisualStyleBackColor = true;
      this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
      // 
      // buttonRemove
      // 
      this.buttonRemove.Location = new System.Drawing.Point(87, 19);
      this.buttonRemove.Name = "buttonRemove";
      this.buttonRemove.Size = new System.Drawing.Size(75, 23);
      this.buttonRemove.TabIndex = 20;
      this.buttonRemove.Text = "Remove";
      this.buttonRemove.UseVisualStyleBackColor = true;
      this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
      // 
      // radioButtonDecimal
      // 
      this.radioButtonDecimal.AutoSize = true;
      this.radioButtonDecimal.Location = new System.Drawing.Point(109, 19);
      this.radioButtonDecimal.Name = "radioButtonDecimal";
      this.radioButtonDecimal.Size = new System.Drawing.Size(63, 17);
      this.radioButtonDecimal.TabIndex = 10;
      this.radioButtonDecimal.Text = "Decimal";
      this.radioButtonDecimal.UseVisualStyleBackColor = true;
      this.radioButtonDecimal.CheckedChanged += new System.EventHandler(this.radioButtonDecimal_CheckedChanged);
      // 
      // radioButtonHex
      // 
      this.radioButtonHex.AutoSize = true;
      this.radioButtonHex.Location = new System.Drawing.Point(59, 19);
      this.radioButtonHex.Name = "radioButtonHex";
      this.radioButtonHex.Size = new System.Drawing.Size(44, 17);
      this.radioButtonHex.TabIndex = 9;
      this.radioButtonHex.Text = "Hex";
      this.radioButtonHex.UseVisualStyleBackColor = true;
      this.radioButtonHex.CheckedChanged += new System.EventHandler(this.radioButtonHex_CheckedChanged);
      // 
      // radioButtonAuto
      // 
      this.radioButtonAuto.AutoSize = true;
      this.radioButtonAuto.Checked = true;
      this.radioButtonAuto.Location = new System.Drawing.Point(6, 19);
      this.radioButtonAuto.Name = "radioButtonAuto";
      this.radioButtonAuto.Size = new System.Drawing.Size(47, 17);
      this.radioButtonAuto.TabIndex = 8;
      this.radioButtonAuto.TabStop = true;
      this.radioButtonAuto.Text = "Auto";
      this.radioButtonAuto.UseVisualStyleBackColor = true;
      this.radioButtonAuto.CheckedChanged += new System.EventHandler(this.radioButtonAuto_CheckedChanged);
      // 
      // selectScriptFileComboBox
      // 
      this.selectScriptFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.selectScriptFileComboBox.FormattingEnabled = true;
      this.selectScriptFileComboBox.Location = new System.Drawing.Point(6, 19);
      this.selectScriptFileComboBox.Name = "selectScriptFileComboBox";
      this.selectScriptFileComboBox.Size = new System.Drawing.Size(152, 21);
      this.selectScriptFileComboBox.TabIndex = 1;
      this.selectScriptFileComboBox.SelectedIndexChanged += new System.EventHandler(this.selectScriptFileComboBox_SelectedIndexChanged);
      // 
      // listBoxTriggers
      // 
      this.listBoxTriggers.FormattingEnabled = true;
      this.listBoxTriggers.Location = new System.Drawing.Point(3, 226);
      this.listBoxTriggers.Name = "listBoxTriggers";
      this.listBoxTriggers.Size = new System.Drawing.Size(397, 134);
      this.listBoxTriggers.TabIndex = 11;
      this.listBoxTriggers.SelectedValueChanged += new System.EventHandler(this.listBoxTriggers_SelectedValueChanged);
      // 
      // buttonOpenHeaderScript
      // 
      this.buttonOpenHeaderScript.Location = new System.Drawing.Point(6, 19);
      this.buttonOpenHeaderScript.Name = "buttonOpenHeaderScript";
      this.buttonOpenHeaderScript.Size = new System.Drawing.Size(88, 23);
      this.buttonOpenHeaderScript.TabIndex = 2;
      this.buttonOpenHeaderScript.Text = "Header Script";
      this.buttonOpenHeaderScript.UseVisualStyleBackColor = true;
      this.buttonOpenHeaderScript.Click += new System.EventHandler(this.buttonOpenHeaderScript_Click);
      // 
      // buttonOpenSelectedScript
      // 
      this.buttonOpenSelectedScript.Location = new System.Drawing.Point(100, 19);
      this.buttonOpenSelectedScript.Name = "buttonOpenSelectedScript";
      this.buttonOpenSelectedScript.Size = new System.Drawing.Size(94, 23);
      this.buttonOpenSelectedScript.TabIndex = 3;
      this.buttonOpenSelectedScript.Text = "Selected Script";
      this.buttonOpenSelectedScript.UseVisualStyleBackColor = true;
      this.buttonOpenSelectedScript.Click += new System.EventHandler(this.buttonOpenSelectedScript_Click);
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.buttonLocate);
      this.groupBox3.Controls.Add(this.buttonOpenSelectedScript);
      this.groupBox3.Controls.Add(this.buttonOpenHeaderScript);
      this.groupBox3.Location = new System.Drawing.Point(3, 61);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(263, 52);
      this.groupBox3.TabIndex = 18;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Open";
      // 
      // buttonLocate
      // 
      this.buttonLocate.Location = new System.Drawing.Point(200, 19);
      this.buttonLocate.Name = "buttonLocate";
      this.buttonLocate.Size = new System.Drawing.Size(56, 23);
      this.buttonLocate.TabIndex = 3;
      this.buttonLocate.Text = "Locate";
      this.buttonLocate.UseVisualStyleBackColor = true;
      this.buttonLocate.Click += new System.EventHandler(this.buttonLocate_Click);
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.buttonImport);
      this.groupBox4.Controls.Add(this.buttonSave);
      this.groupBox4.Controls.Add(this.buttonExport);
      this.groupBox4.Controls.Add(this.checkBoxPadding);
      this.groupBox4.Location = new System.Drawing.Point(3, 119);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(329, 53);
      this.groupBox4.TabIndex = 20;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "File";
      // 
      // buttonImport
      // 
      this.buttonImport.Location = new System.Drawing.Point(6, 19);
      this.buttonImport.Name = "buttonImport";
      this.buttonImport.Size = new System.Drawing.Size(58, 23);
      this.buttonImport.TabIndex = 4;
      this.buttonImport.Text = "Import";
      this.buttonImport.UseVisualStyleBackColor = true;
      this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
      // 
      // groupBox5
      // 
      this.groupBox5.Controls.Add(this.buttonRemove);
      this.groupBox5.Controls.Add(this.buttonAdd);
      this.groupBox5.Location = new System.Drawing.Point(231, 474);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Size = new System.Drawing.Size(169, 52);
      this.groupBox5.TabIndex = 21;
      this.groupBox5.TabStop = false;
      this.groupBox5.Text = "Trigger";
      // 
      // groupBox6
      // 
      this.groupBox6.Controls.Add(this.radioButtonAuto);
      this.groupBox6.Controls.Add(this.radioButtonDecimal);
      this.groupBox6.Controls.Add(this.radioButtonHex);
      this.groupBox6.Location = new System.Drawing.Point(3, 178);
      this.groupBox6.Name = "groupBox6";
      this.groupBox6.Size = new System.Drawing.Size(178, 42);
      this.groupBox6.TabIndex = 22;
      this.groupBox6.TabStop = false;
      this.groupBox6.Text = "Number Format";
      // 
      // groupBox7
      // 
      this.groupBox7.Controls.Add(this.selectScriptFileComboBox);
      this.groupBox7.Location = new System.Drawing.Point(3, 3);
      this.groupBox7.Name = "groupBox7";
      this.groupBox7.Size = new System.Drawing.Size(164, 52);
      this.groupBox7.TabIndex = 23;
      this.groupBox7.TabStop = false;
      this.groupBox7.Text = "Script File";
      // 
      // LevelScriptEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.groupBox7);
      this.Controls.Add(this.groupBox6);
      this.Controls.Add(this.groupBox5);
      this.Controls.Add(this.groupBox4);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBoxValue);
      this.Controls.Add(this.groupBoxVariable);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.listBoxTriggers);
      this.Location = new System.Drawing.Point(15, 15);
      this.Name = "LevelScriptEditor";
      this.Size = new System.Drawing.Size(408, 622);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBoxVariable.ResumeLayout(false);
      this.groupBoxVariable.PerformLayout();
      this.groupBoxValue.ResumeLayout(false);
      this.groupBoxValue.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      this.groupBox5.ResumeLayout(false);
      this.groupBox6.ResumeLayout(false);
      this.groupBox6.PerformLayout();
      this.groupBox7.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    private System.Windows.Forms.Button buttonImport;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.GroupBox groupBox5;

    private System.Windows.Forms.Button buttonOpenSelectedScript;
    private System.Windows.Forms.GroupBox groupBox3;

    public System.Windows.Forms.ComboBox selectScriptFileComboBox;

    #endregion
        private System.Windows.Forms.Button buttonSave;
        private DSPRE.ListBox2 listBoxTriggers;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.RadioButton radioButtonVariableValue;
    private System.Windows.Forms.RadioButton radioButtonMapChange;
    private System.Windows.Forms.RadioButton radioButtonScreenReset;
    private System.Windows.Forms.RadioButton radioButtonLoadGame;
    private System.Windows.Forms.TextBox textBoxScriptID;
    private System.Windows.Forms.TextBox textBoxVariableName;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.GroupBox groupBoxVariable;
    private System.Windows.Forms.GroupBox groupBoxValue;
    private System.Windows.Forms.TextBox textBoxVariableValue;
    private System.Windows.Forms.CheckBox checkBoxPadding;
    private System.Windows.Forms.Button buttonAdd;
    private System.Windows.Forms.Button buttonRemove;
    private System.Windows.Forms.RadioButton radioButtonDecimal;
    private System.Windows.Forms.RadioButton radioButtonHex;
    private System.Windows.Forms.RadioButton radioButtonAuto;
    private System.Windows.Forms.Button buttonOpenHeaderScript;
    private System.Windows.Forms.GroupBox groupBox6;
    private System.Windows.Forms.GroupBox groupBox7;
    private System.Windows.Forms.Button buttonLocate;
  }
}

