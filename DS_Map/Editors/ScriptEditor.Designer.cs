
namespace DSPRE.Editors
{
    partial class ScriptEditor
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
            this.selectScriptFileComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.scriptEditorTabControl = new System.Windows.Forms.TabControl();
            this.scriptsTabPage = new System.Windows.Forms.TabPage();
            this.PanelSearchScripts = new System.Windows.Forms.Panel();
            this.BtnNextFindScript = new System.Windows.Forms.Button();
            this.BtnPrevFindScript = new System.Windows.Forms.Button();
            this.BtnCloseFindScript = new System.Windows.Forms.Button();
            this.panelFindScriptTextBox = new System.Windows.Forms.TextBox();
            this.scintillaScriptsPanel = new System.Windows.Forms.Panel();
            this.functionsTabPage = new System.Windows.Forms.TabPage();
            this.PanelSearchFunctions = new System.Windows.Forms.Panel();
            this.BtnNextFindFunc = new System.Windows.Forms.Button();
            this.BtnPrevFindFunc = new System.Windows.Forms.Button();
            this.BtnCloseFindFunc = new System.Windows.Forms.Button();
            this.panelFindFunctionTextBox = new System.Windows.Forms.TextBox();
            this.scintillaFunctionsPanel = new System.Windows.Forms.Panel();
            this.actionsTabPage = new System.Windows.Forms.TabPage();
            this.PanelSearchActions = new System.Windows.Forms.Panel();
            this.BtnNextFindActions = new System.Windows.Forms.Button();
            this.BtnPrevFindActions = new System.Windows.Forms.Button();
            this.BtnCloseFindActions = new System.Windows.Forms.Button();
            this.panelFindActionTextBox = new System.Windows.Forms.TextBox();
            this.scintillaActionsPanel = new System.Windows.Forms.Panel();
            this.addScriptFileButton = new System.Windows.Forms.Button();
            this.removeScriptFileButton = new System.Windows.Forms.Button();
            this.saveScriptFileButton = new System.Windows.Forms.Button();
            this.exportScriptFileButton = new System.Windows.Forms.Button();
            this.importScriptFileButton = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.searchInScriptsButton = new System.Windows.Forms.Button();
            this.searchOnlyCurrentScriptCheckBox = new System.Windows.Forms.CheckBox();
            this.scrollToBlockStartcheckBox = new System.Windows.Forms.CheckBox();
            this.scriptSearchCaseSensitiveCheckBox = new System.Windows.Forms.CheckBox();
            this.searchInScriptsTextBox = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.searchProgressBar = new System.Windows.Forms.ProgressBar();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.searchInScriptsResultListBox = new System.Windows.Forms.ListBox();
            this.groupBox24 = new System.Windows.Forms.GroupBox();
            this.ScriptNavigatorTabControl = new System.Windows.Forms.TabControl();
            this.ScriptsNavTab = new System.Windows.Forms.TabPage();
            this.scriptsNavListbox = new System.Windows.Forms.ListBox();
            this.FunctionsNavTab = new System.Windows.Forms.TabPage();
            this.functionsNavListbox = new System.Windows.Forms.ListBox();
            this.ActionsNavTab = new System.Windows.Forms.TabPage();
            this.actionsNavListbox = new System.Windows.Forms.ListBox();
            this.openFindScriptEditorButton = new System.Windows.Forms.Button();
            this.expandScriptTextButton = new System.Windows.Forms.Button();
            this.compressScriptTextButton = new System.Windows.Forms.Button();
            this.scriptEditorWordWrapCheckbox = new System.Windows.Forms.CheckBox();
            this.scriptEditorWhitespacesCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox26 = new System.Windows.Forms.GroupBox();
            this.scriptEditorNumberFormatNoPreference = new System.Windows.Forms.RadioButton();
            this.scriptEditorNumberFormatDecimal = new System.Windows.Forms.RadioButton();
            this.scriptEditorNumberFormatHex = new System.Windows.Forms.RadioButton();
            this.viewLevelScriptButton = new System.Windows.Forms.Button();
            this.locateCurrentScriptFile = new System.Windows.Forms.Button();
            this.scriptEditorTabControl.SuspendLayout();
            this.scriptsTabPage.SuspendLayout();
            this.PanelSearchScripts.SuspendLayout();
            this.functionsTabPage.SuspendLayout();
            this.PanelSearchFunctions.SuspendLayout();
            this.actionsTabPage.SuspendLayout();
            this.PanelSearchActions.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.ScriptNavigatorTabControl.SuspendLayout();
            this.ScriptsNavTab.SuspendLayout();
            this.FunctionsNavTab.SuspendLayout();
            this.ActionsNavTab.SuspendLayout();
            this.groupBox26.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectScriptFileComboBox
            // 
            this.selectScriptFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectScriptFileComboBox.FormattingEnabled = true;
            this.selectScriptFileComboBox.Location = new System.Drawing.Point(7, 24);
            this.selectScriptFileComboBox.Name = "selectScriptFileComboBox";
            this.selectScriptFileComboBox.Size = new System.Drawing.Size(152, 21);
            this.selectScriptFileComboBox.TabIndex = 0;
            this.selectScriptFileComboBox.SelectedIndexChanged += new System.EventHandler(this.selectScriptFileComboBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Script File";
            // 
            // scriptEditorTabControl
            // 
            this.scriptEditorTabControl.Controls.Add(this.scriptsTabPage);
            this.scriptEditorTabControl.Controls.Add(this.functionsTabPage);
            this.scriptEditorTabControl.Controls.Add(this.actionsTabPage);
            this.scriptEditorTabControl.Location = new System.Drawing.Point(481, 22);
            this.scriptEditorTabControl.Name = "scriptEditorTabControl";
            this.scriptEditorTabControl.SelectedIndex = 0;
            this.scriptEditorTabControl.Size = new System.Drawing.Size(692, 591);
            this.scriptEditorTabControl.TabIndex = 18;
            this.scriptEditorTabControl.SelectedIndexChanged += new System.EventHandler(this.scriptEditorTabControl_TabIndexChanged);
            // 
            // scriptsTabPage
            // 
            this.scriptsTabPage.Controls.Add(this.PanelSearchScripts);
            this.scriptsTabPage.Controls.Add(this.scintillaScriptsPanel);
            this.scriptsTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scriptsTabPage.Location = new System.Drawing.Point(4, 22);
            this.scriptsTabPage.Name = "scriptsTabPage";
            this.scriptsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.scriptsTabPage.Size = new System.Drawing.Size(684, 565);
            this.scriptsTabPage.TabIndex = 0;
            this.scriptsTabPage.Text = "Scripts";
            this.scriptsTabPage.UseVisualStyleBackColor = true;
            // 
            // PanelSearchScripts
            // 
            this.PanelSearchScripts.BackColor = System.Drawing.Color.White;
            this.PanelSearchScripts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelSearchScripts.Controls.Add(this.BtnNextFindScript);
            this.PanelSearchScripts.Controls.Add(this.BtnPrevFindScript);
            this.PanelSearchScripts.Controls.Add(this.BtnCloseFindScript);
            this.PanelSearchScripts.Controls.Add(this.panelFindScriptTextBox);
            this.PanelSearchScripts.Location = new System.Drawing.Point(386, 3);
            this.PanelSearchScripts.Name = "PanelSearchScripts";
            this.PanelSearchScripts.Size = new System.Drawing.Size(292, 40);
            this.PanelSearchScripts.TabIndex = 14;
            this.PanelSearchScripts.Visible = false;
            // 
            // BtnNextFindScript
            // 
            this.BtnNextFindScript.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnNextFindScript.ForeColor = System.Drawing.Color.White;
            this.BtnNextFindScript.Image = global::DSPRE.Properties.Resources.arrowdown;
            this.BtnNextFindScript.Location = new System.Drawing.Point(233, 4);
            this.BtnNextFindScript.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnNextFindScript.Name = "BtnNextFindScript";
            this.BtnNextFindScript.Size = new System.Drawing.Size(25, 30);
            this.BtnNextFindScript.TabIndex = 32;
            this.BtnNextFindScript.Tag = "Find next (Enter)";
            this.BtnNextFindScript.UseVisualStyleBackColor = true;
            this.BtnNextFindScript.Click += new System.EventHandler(this.BtnNextFindScript_Click);
            // 
            // BtnPrevFindScript
            // 
            this.BtnPrevFindScript.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnPrevFindScript.ForeColor = System.Drawing.Color.White;
            this.BtnPrevFindScript.Image = global::DSPRE.Properties.Resources.arrowup;
            this.BtnPrevFindScript.Location = new System.Drawing.Point(205, 4);
            this.BtnPrevFindScript.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnPrevFindScript.Name = "BtnPrevFindScript";
            this.BtnPrevFindScript.Size = new System.Drawing.Size(25, 30);
            this.BtnPrevFindScript.TabIndex = 31;
            this.BtnPrevFindScript.Tag = "Find previous (Shift+Enter)";
            this.BtnPrevFindScript.UseVisualStyleBackColor = true;
            this.BtnPrevFindScript.Click += new System.EventHandler(this.BtnPrevFindScript_Click);
            // 
            // BtnCloseFindScript
            // 
            this.BtnCloseFindScript.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCloseFindScript.ForeColor = System.Drawing.Color.White;
            this.BtnCloseFindScript.Image = global::DSPRE.Properties.Resources.Cross;
            this.BtnCloseFindScript.Location = new System.Drawing.Point(261, 4);
            this.BtnCloseFindScript.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnCloseFindScript.Name = "BtnCloseFindScript";
            this.BtnCloseFindScript.Size = new System.Drawing.Size(25, 30);
            this.BtnCloseFindScript.TabIndex = 33;
            this.BtnCloseFindScript.Tag = "Close (Esc)";
            this.BtnCloseFindScript.UseVisualStyleBackColor = true;
            this.BtnCloseFindScript.Click += new System.EventHandler(this.BtnCloseFindScript_Click);
            // 
            // panelFindScriptTextBox
            // 
            this.panelFindScriptTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.panelFindScriptTextBox.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelFindScriptTextBox.Location = new System.Drawing.Point(10, 6);
            this.panelFindScriptTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelFindScriptTextBox.Name = "panelFindScriptTextBox";
            this.panelFindScriptTextBox.Size = new System.Drawing.Size(189, 25);
            this.panelFindScriptTextBox.TabIndex = 30;
            this.panelFindScriptTextBox.TextChanged += new System.EventHandler(this.panelFindScriptTextBox_TextChanged);
            this.panelFindScriptTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.scriptTxtFind_KeyDown);
            // 
            // scintillaScriptsPanel
            // 
            this.scintillaScriptsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintillaScriptsPanel.Location = new System.Drawing.Point(3, 3);
            this.scintillaScriptsPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.scintillaScriptsPanel.Name = "scintillaScriptsPanel";
            this.scintillaScriptsPanel.Size = new System.Drawing.Size(678, 559);
            this.scintillaScriptsPanel.TabIndex = 19;
            // 
            // functionsTabPage
            // 
            this.functionsTabPage.Controls.Add(this.PanelSearchFunctions);
            this.functionsTabPage.Controls.Add(this.scintillaFunctionsPanel);
            this.functionsTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.functionsTabPage.Location = new System.Drawing.Point(4, 22);
            this.functionsTabPage.Name = "functionsTabPage";
            this.functionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.functionsTabPage.Size = new System.Drawing.Size(684, 565);
            this.functionsTabPage.TabIndex = 1;
            this.functionsTabPage.Text = "Functions";
            this.functionsTabPage.UseVisualStyleBackColor = true;
            // 
            // PanelSearchFunctions
            // 
            this.PanelSearchFunctions.BackColor = System.Drawing.Color.White;
            this.PanelSearchFunctions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelSearchFunctions.Controls.Add(this.BtnNextFindFunc);
            this.PanelSearchFunctions.Controls.Add(this.BtnPrevFindFunc);
            this.PanelSearchFunctions.Controls.Add(this.BtnCloseFindFunc);
            this.PanelSearchFunctions.Controls.Add(this.panelFindFunctionTextBox);
            this.PanelSearchFunctions.Location = new System.Drawing.Point(386, 3);
            this.PanelSearchFunctions.Name = "PanelSearchFunctions";
            this.PanelSearchFunctions.Size = new System.Drawing.Size(292, 40);
            this.PanelSearchFunctions.TabIndex = 16;
            this.PanelSearchFunctions.Visible = false;
            // 
            // BtnNextFindFunc
            // 
            this.BtnNextFindFunc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnNextFindFunc.ForeColor = System.Drawing.Color.White;
            this.BtnNextFindFunc.Image = global::DSPRE.Properties.Resources.arrowdown;
            this.BtnNextFindFunc.Location = new System.Drawing.Point(233, 4);
            this.BtnNextFindFunc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnNextFindFunc.Name = "BtnNextFindFunc";
            this.BtnNextFindFunc.Size = new System.Drawing.Size(25, 30);
            this.BtnNextFindFunc.TabIndex = 36;
            this.BtnNextFindFunc.Tag = "Find next (Enter)";
            this.BtnNextFindFunc.UseVisualStyleBackColor = true;
            this.BtnNextFindFunc.Click += new System.EventHandler(this.BtnNextFindFunc_Click);
            // 
            // BtnPrevFindFunc
            // 
            this.BtnPrevFindFunc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnPrevFindFunc.ForeColor = System.Drawing.Color.White;
            this.BtnPrevFindFunc.Image = global::DSPRE.Properties.Resources.arrowup;
            this.BtnPrevFindFunc.Location = new System.Drawing.Point(205, 4);
            this.BtnPrevFindFunc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnPrevFindFunc.Name = "BtnPrevFindFunc";
            this.BtnPrevFindFunc.Size = new System.Drawing.Size(25, 30);
            this.BtnPrevFindFunc.TabIndex = 35;
            this.BtnPrevFindFunc.Tag = "Find previous (Shift+Enter)";
            this.BtnPrevFindFunc.UseVisualStyleBackColor = true;
            this.BtnPrevFindFunc.Click += new System.EventHandler(this.BtnPrevFindFunc_Click);
            // 
            // BtnCloseFindFunc
            // 
            this.BtnCloseFindFunc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCloseFindFunc.ForeColor = System.Drawing.Color.White;
            this.BtnCloseFindFunc.Image = global::DSPRE.Properties.Resources.Cross;
            this.BtnCloseFindFunc.Location = new System.Drawing.Point(261, 4);
            this.BtnCloseFindFunc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnCloseFindFunc.Name = "BtnCloseFindFunc";
            this.BtnCloseFindFunc.Size = new System.Drawing.Size(25, 30);
            this.BtnCloseFindFunc.TabIndex = 37;
            this.BtnCloseFindFunc.Tag = "Close (Esc)";
            this.BtnCloseFindFunc.UseVisualStyleBackColor = true;
            this.BtnCloseFindFunc.Click += new System.EventHandler(this.BtnCloseFindFunc_Click);
            // 
            // panelFindFunctionTextBox
            // 
            this.panelFindFunctionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.panelFindFunctionTextBox.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelFindFunctionTextBox.Location = new System.Drawing.Point(10, 6);
            this.panelFindFunctionTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelFindFunctionTextBox.Name = "panelFindFunctionTextBox";
            this.panelFindFunctionTextBox.Size = new System.Drawing.Size(189, 25);
            this.panelFindFunctionTextBox.TabIndex = 34;
            this.panelFindFunctionTextBox.TextChanged += new System.EventHandler(this.panelFindFunctionTextBox_TextChanged);
            this.panelFindFunctionTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionTxtFind_KeyDown);
            // 
            // scintillaFunctionsPanel
            // 
            this.scintillaFunctionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintillaFunctionsPanel.Location = new System.Drawing.Point(3, 3);
            this.scintillaFunctionsPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.scintillaFunctionsPanel.Name = "scintillaFunctionsPanel";
            this.scintillaFunctionsPanel.Size = new System.Drawing.Size(678, 559);
            this.scintillaFunctionsPanel.TabIndex = 20;
            // 
            // actionsTabPage
            // 
            this.actionsTabPage.Controls.Add(this.PanelSearchActions);
            this.actionsTabPage.Controls.Add(this.scintillaActionsPanel);
            this.actionsTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.actionsTabPage.Location = new System.Drawing.Point(4, 22);
            this.actionsTabPage.Name = "actionsTabPage";
            this.actionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.actionsTabPage.Size = new System.Drawing.Size(684, 565);
            this.actionsTabPage.TabIndex = 2;
            this.actionsTabPage.Text = "Actions";
            this.actionsTabPage.UseVisualStyleBackColor = true;
            // 
            // PanelSearchActions
            // 
            this.PanelSearchActions.BackColor = System.Drawing.Color.White;
            this.PanelSearchActions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelSearchActions.Controls.Add(this.BtnNextFindActions);
            this.PanelSearchActions.Controls.Add(this.BtnPrevFindActions);
            this.PanelSearchActions.Controls.Add(this.BtnCloseFindActions);
            this.PanelSearchActions.Controls.Add(this.panelFindActionTextBox);
            this.PanelSearchActions.Location = new System.Drawing.Point(386, 3);
            this.PanelSearchActions.Name = "PanelSearchActions";
            this.PanelSearchActions.Size = new System.Drawing.Size(292, 40);
            this.PanelSearchActions.TabIndex = 16;
            this.PanelSearchActions.Visible = false;
            // 
            // BtnNextFindActions
            // 
            this.BtnNextFindActions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnNextFindActions.ForeColor = System.Drawing.Color.White;
            this.BtnNextFindActions.Image = global::DSPRE.Properties.Resources.arrowdown;
            this.BtnNextFindActions.Location = new System.Drawing.Point(233, 4);
            this.BtnNextFindActions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnNextFindActions.Name = "BtnNextFindActions";
            this.BtnNextFindActions.Size = new System.Drawing.Size(25, 30);
            this.BtnNextFindActions.TabIndex = 40;
            this.BtnNextFindActions.Tag = "Find next (Enter)";
            this.BtnNextFindActions.UseVisualStyleBackColor = true;
            this.BtnNextFindActions.Click += new System.EventHandler(this.BtnNextFindActions_Click);
            // 
            // BtnPrevFindActions
            // 
            this.BtnPrevFindActions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnPrevFindActions.ForeColor = System.Drawing.Color.White;
            this.BtnPrevFindActions.Image = global::DSPRE.Properties.Resources.arrowup;
            this.BtnPrevFindActions.Location = new System.Drawing.Point(205, 4);
            this.BtnPrevFindActions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnPrevFindActions.Name = "BtnPrevFindActions";
            this.BtnPrevFindActions.Size = new System.Drawing.Size(25, 30);
            this.BtnPrevFindActions.TabIndex = 39;
            this.BtnPrevFindActions.Tag = "Find previous (Shift+Enter)";
            this.BtnPrevFindActions.UseVisualStyleBackColor = true;
            this.BtnPrevFindActions.Click += new System.EventHandler(this.BtnPrevFindActions_Click);
            // 
            // BtnCloseFindActions
            // 
            this.BtnCloseFindActions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCloseFindActions.ForeColor = System.Drawing.Color.White;
            this.BtnCloseFindActions.Image = global::DSPRE.Properties.Resources.Cross;
            this.BtnCloseFindActions.Location = new System.Drawing.Point(261, 4);
            this.BtnCloseFindActions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnCloseFindActions.Name = "BtnCloseFindActions";
            this.BtnCloseFindActions.Size = new System.Drawing.Size(25, 30);
            this.BtnCloseFindActions.TabIndex = 41;
            this.BtnCloseFindActions.Tag = "Close (Esc)";
            this.BtnCloseFindActions.UseVisualStyleBackColor = true;
            this.BtnCloseFindActions.Click += new System.EventHandler(this.BtnCloseFindActions_Click);
            // 
            // panelFindActionTextBox
            // 
            this.panelFindActionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.panelFindActionTextBox.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelFindActionTextBox.Location = new System.Drawing.Point(10, 6);
            this.panelFindActionTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelFindActionTextBox.Name = "panelFindActionTextBox";
            this.panelFindActionTextBox.Size = new System.Drawing.Size(189, 25);
            this.panelFindActionTextBox.TabIndex = 38;
            this.panelFindActionTextBox.TextChanged += new System.EventHandler(this.panelFindActionTextBox_TextChanged);
            this.panelFindActionTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.actionTxtFind_KeyDown);
            // 
            // scintillaActionsPanel
            // 
            this.scintillaActionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintillaActionsPanel.Location = new System.Drawing.Point(3, 3);
            this.scintillaActionsPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.scintillaActionsPanel.Name = "scintillaActionsPanel";
            this.scintillaActionsPanel.Size = new System.Drawing.Size(678, 559);
            this.scintillaActionsPanel.TabIndex = 21;
            // 
            // addScriptFileButton
            // 
            this.addScriptFileButton.Image = global::DSPRE.Properties.Resources.addIcon;
            this.addScriptFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.addScriptFileButton.Location = new System.Drawing.Point(369, 667);
            this.addScriptFileButton.Name = "addScriptFileButton";
            this.addScriptFileButton.Size = new System.Drawing.Size(106, 25);
            this.addScriptFileButton.TabIndex = 4;
            this.addScriptFileButton.Text = "Add Script File";
            this.addScriptFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addScriptFileButton.UseVisualStyleBackColor = true;
            this.addScriptFileButton.Click += new System.EventHandler(this.addScriptFileButton_Click);
            // 
            // removeScriptFileButton
            // 
            this.removeScriptFileButton.Image = global::DSPRE.Properties.Resources.deleteIcon;
            this.removeScriptFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.removeScriptFileButton.Location = new System.Drawing.Point(369, 694);
            this.removeScriptFileButton.Name = "removeScriptFileButton";
            this.removeScriptFileButton.Size = new System.Drawing.Size(106, 25);
            this.removeScriptFileButton.TabIndex = 5;
            this.removeScriptFileButton.Text = "Remove Last";
            this.removeScriptFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.removeScriptFileButton.UseVisualStyleBackColor = true;
            this.removeScriptFileButton.Click += new System.EventHandler(this.removeScriptFileButton_Click);
            // 
            // saveScriptFileButton
            // 
            this.saveScriptFileButton.Image = global::DSPRE.Properties.Resources.saveButton;
            this.saveScriptFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.saveScriptFileButton.Location = new System.Drawing.Point(6, 50);
            this.saveScriptFileButton.Name = "saveScriptFileButton";
            this.saveScriptFileButton.Size = new System.Drawing.Size(154, 23);
            this.saveScriptFileButton.TabIndex = 1;
            this.saveScriptFileButton.Text = "&Save Current File";
            this.saveScriptFileButton.UseVisualStyleBackColor = true;
            this.saveScriptFileButton.Click += new System.EventHandler(this.saveScriptFileButton_Click);
            // 
            // exportScriptFileButton
            // 
            this.exportScriptFileButton.Image = global::DSPRE.Properties.Resources.exportArrow;
            this.exportScriptFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.exportScriptFileButton.Location = new System.Drawing.Point(294, 667);
            this.exportScriptFileButton.Name = "exportScriptFileButton";
            this.exportScriptFileButton.Size = new System.Drawing.Size(70, 52);
            this.exportScriptFileButton.TabIndex = 3;
            this.exportScriptFileButton.Text = "&Export \r\nFile";
            this.exportScriptFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.exportScriptFileButton.UseVisualStyleBackColor = true;
            this.exportScriptFileButton.Click += new System.EventHandler(this.exportScriptFileButton_Click);
            // 
            // importScriptFileButton
            // 
            this.importScriptFileButton.Image = global::DSPRE.Properties.Resources.importArrow;
            this.importScriptFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.importScriptFileButton.Location = new System.Drawing.Point(219, 667);
            this.importScriptFileButton.Name = "importScriptFileButton";
            this.importScriptFileButton.Size = new System.Drawing.Size(70, 52);
            this.importScriptFileButton.TabIndex = 2;
            this.importScriptFileButton.Text = "&Import\r\nFile";
            this.importScriptFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.importScriptFileButton.UseVisualStyleBackColor = true;
            this.importScriptFileButton.Click += new System.EventHandler(this.importScriptFileButton_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.searchInScriptsButton);
            this.groupBox8.Controls.Add(this.searchOnlyCurrentScriptCheckBox);
            this.groupBox8.Controls.Add(this.scrollToBlockStartcheckBox);
            this.groupBox8.Controls.Add(this.scriptSearchCaseSensitiveCheckBox);
            this.groupBox8.Controls.Add(this.searchInScriptsTextBox);
            this.groupBox8.Controls.Add(this.label31);
            this.groupBox8.Controls.Add(this.searchProgressBar);
            this.groupBox8.Controls.Add(this.label30);
            this.groupBox8.Controls.Add(this.label29);
            this.groupBox8.Controls.Add(this.searchInScriptsResultListBox);
            this.groupBox8.Location = new System.Drawing.Point(2, 315);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(472, 298);
            this.groupBox8.TabIndex = 18;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Search for commands:";
            // 
            // searchInScriptsButton
            // 
            this.searchInScriptsButton.Image = global::DSPRE.Properties.Resources.SearchMiniIcon;
            this.searchInScriptsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.searchInScriptsButton.Location = new System.Drawing.Point(272, 27);
            this.searchInScriptsButton.Name = "searchInScriptsButton";
            this.searchInScriptsButton.Size = new System.Drawing.Size(69, 36);
            this.searchInScriptsButton.TabIndex = 13;
            this.searchInScriptsButton.Text = "Search";
            this.searchInScriptsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.searchInScriptsButton.UseVisualStyleBackColor = true;
            this.searchInScriptsButton.Click += new System.EventHandler(this.searchInScriptsButton_Click);
            // 
            // searchOnlyCurrentScriptCheckBox
            // 
            this.searchOnlyCurrentScriptCheckBox.AutoSize = true;
            this.searchOnlyCurrentScriptCheckBox.Location = new System.Drawing.Point(347, 15);
            this.searchOnlyCurrentScriptCheckBox.Name = "searchOnlyCurrentScriptCheckBox";
            this.searchOnlyCurrentScriptCheckBox.Size = new System.Drawing.Size(84, 17);
            this.searchOnlyCurrentScriptCheckBox.TabIndex = 14;
            this.searchOnlyCurrentScriptCheckBox.Text = "Only Current";
            this.searchOnlyCurrentScriptCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.searchOnlyCurrentScriptCheckBox.UseVisualStyleBackColor = true;
            // 
            // scrollToBlockStartcheckBox
            // 
            this.scrollToBlockStartcheckBox.AutoSize = true;
            this.scrollToBlockStartcheckBox.Location = new System.Drawing.Point(347, 61);
            this.scrollToBlockStartcheckBox.Name = "scrollToBlockStartcheckBox";
            this.scrollToBlockStartcheckBox.Size = new System.Drawing.Size(116, 17);
            this.scrollToBlockStartcheckBox.TabIndex = 16;
            this.scrollToBlockStartcheckBox.Text = "Scroll to block start";
            this.scrollToBlockStartcheckBox.UseVisualStyleBackColor = true;
            // 
            // scriptSearchCaseSensitiveCheckBox
            // 
            this.scriptSearchCaseSensitiveCheckBox.AutoSize = true;
            this.scriptSearchCaseSensitiveCheckBox.Location = new System.Drawing.Point(347, 38);
            this.scriptSearchCaseSensitiveCheckBox.Name = "scriptSearchCaseSensitiveCheckBox";
            this.scriptSearchCaseSensitiveCheckBox.Size = new System.Drawing.Size(83, 17);
            this.scriptSearchCaseSensitiveCheckBox.TabIndex = 15;
            this.scriptSearchCaseSensitiveCheckBox.Text = "Match Case";
            this.scriptSearchCaseSensitiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // searchInScriptsTextBox
            // 
            this.searchInScriptsTextBox.Location = new System.Drawing.Point(11, 36);
            this.searchInScriptsTextBox.Name = "searchInScriptsTextBox";
            this.searchInScriptsTextBox.Size = new System.Drawing.Size(255, 20);
            this.searchInScriptsTextBox.TabIndex = 12;
            this.searchInScriptsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchInScriptsTextBox_KeyDown);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(11, 261);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(48, 13);
            this.label31.TabIndex = 37;
            this.label31.Text = "Progress";
            // 
            // searchProgressBar
            // 
            this.searchProgressBar.Location = new System.Drawing.Point(11, 277);
            this.searchProgressBar.Name = "searchProgressBar";
            this.searchProgressBar.Size = new System.Drawing.Size(452, 14);
            this.searchProgressBar.TabIndex = 36;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(11, 68);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(42, 13);
            this.label30.TabIndex = 35;
            this.label30.Text = "Results";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(11, 20);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(77, 13);
            this.label29.TabIndex = 33;
            this.label29.Text = "Line to search:";
            // 
            // searchInScriptsResultListBox
            // 
            this.searchInScriptsResultListBox.Location = new System.Drawing.Point(11, 84);
            this.searchInScriptsResultListBox.Name = "searchInScriptsResultListBox";
            this.searchInScriptsResultListBox.Size = new System.Drawing.Size(452, 173);
            this.searchInScriptsResultListBox.TabIndex = 17;
            this.searchInScriptsResultListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchInScriptsResultListBox_KeyDown);
            this.searchInScriptsResultListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.searchInScripts_GoToEntryResult);
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.ScriptNavigatorTabControl);
            this.groupBox24.Location = new System.Drawing.Point(3, 75);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Size = new System.Drawing.Size(472, 234);
            this.groupBox24.TabIndex = 42;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "Navigator";
            // 
            // ScriptNavigatorTabControl
            // 
            this.ScriptNavigatorTabControl.Controls.Add(this.ScriptsNavTab);
            this.ScriptNavigatorTabControl.Controls.Add(this.FunctionsNavTab);
            this.ScriptNavigatorTabControl.Controls.Add(this.ActionsNavTab);
            this.ScriptNavigatorTabControl.Location = new System.Drawing.Point(6, 16);
            this.ScriptNavigatorTabControl.Name = "ScriptNavigatorTabControl";
            this.ScriptNavigatorTabControl.SelectedIndex = 0;
            this.ScriptNavigatorTabControl.Size = new System.Drawing.Size(456, 209);
            this.ScriptNavigatorTabControl.TabIndex = 8;
            // 
            // ScriptsNavTab
            // 
            this.ScriptsNavTab.Controls.Add(this.scriptsNavListbox);
            this.ScriptsNavTab.Location = new System.Drawing.Point(4, 22);
            this.ScriptsNavTab.Name = "ScriptsNavTab";
            this.ScriptsNavTab.Padding = new System.Windows.Forms.Padding(3);
            this.ScriptsNavTab.Size = new System.Drawing.Size(448, 183);
            this.ScriptsNavTab.TabIndex = 0;
            this.ScriptsNavTab.Text = "Scripts";
            this.ScriptsNavTab.UseVisualStyleBackColor = true;
            // 
            // scriptsNavListbox
            // 
            this.scriptsNavListbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptsNavListbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scriptsNavListbox.ItemHeight = 15;
            this.scriptsNavListbox.Location = new System.Drawing.Point(3, 3);
            this.scriptsNavListbox.Name = "scriptsNavListbox";
            this.scriptsNavListbox.Size = new System.Drawing.Size(442, 177);
            this.scriptsNavListbox.TabIndex = 9;
            this.scriptsNavListbox.SelectedIndexChanged += new System.EventHandler(this.scriptsNavListbox_SelectedIndexChanged);
            // 
            // FunctionsNavTab
            // 
            this.FunctionsNavTab.Controls.Add(this.functionsNavListbox);
            this.FunctionsNavTab.Location = new System.Drawing.Point(4, 22);
            this.FunctionsNavTab.Name = "FunctionsNavTab";
            this.FunctionsNavTab.Padding = new System.Windows.Forms.Padding(3);
            this.FunctionsNavTab.Size = new System.Drawing.Size(448, 183);
            this.FunctionsNavTab.TabIndex = 1;
            this.FunctionsNavTab.Text = "Functions";
            this.FunctionsNavTab.UseVisualStyleBackColor = true;
            // 
            // functionsNavListbox
            // 
            this.functionsNavListbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.functionsNavListbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.functionsNavListbox.ItemHeight = 15;
            this.functionsNavListbox.Location = new System.Drawing.Point(3, 3);
            this.functionsNavListbox.Name = "functionsNavListbox";
            this.functionsNavListbox.Size = new System.Drawing.Size(442, 177);
            this.functionsNavListbox.TabIndex = 10;
            this.functionsNavListbox.SelectedIndexChanged += new System.EventHandler(this.functionsNavListbox_SelectedIndexChanged);
            // 
            // ActionsNavTab
            // 
            this.ActionsNavTab.Controls.Add(this.actionsNavListbox);
            this.ActionsNavTab.Location = new System.Drawing.Point(4, 22);
            this.ActionsNavTab.Name = "ActionsNavTab";
            this.ActionsNavTab.Padding = new System.Windows.Forms.Padding(3);
            this.ActionsNavTab.Size = new System.Drawing.Size(448, 183);
            this.ActionsNavTab.TabIndex = 2;
            this.ActionsNavTab.Text = "Actions";
            this.ActionsNavTab.UseVisualStyleBackColor = true;
            // 
            // actionsNavListbox
            // 
            this.actionsNavListbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionsNavListbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.actionsNavListbox.ItemHeight = 15;
            this.actionsNavListbox.Location = new System.Drawing.Point(3, 3);
            this.actionsNavListbox.Name = "actionsNavListbox";
            this.actionsNavListbox.Size = new System.Drawing.Size(442, 177);
            this.actionsNavListbox.TabIndex = 11;
            this.actionsNavListbox.SelectedIndexChanged += new System.EventHandler(this.actionsNavListbox_SelectedIndexChanged);
            // 
            // openFindScriptEditorButton
            // 
            this.openFindScriptEditorButton.Image = global::DSPRE.Properties.Resources.SearchMiniIcon;
            this.openFindScriptEditorButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.openFindScriptEditorButton.Location = new System.Drawing.Point(1079, 11);
            this.openFindScriptEditorButton.Name = "openFindScriptEditorButton";
            this.openFindScriptEditorButton.Size = new System.Drawing.Size(24, 24);
            this.openFindScriptEditorButton.TabIndex = 27;
            this.openFindScriptEditorButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.openFindScriptEditorButton.UseVisualStyleBackColor = true;
            this.openFindScriptEditorButton.Click += new System.EventHandler(this.openFindScriptEditorButton_Click);
            // 
            // expandScriptTextButton
            // 
            this.expandScriptTextButton.Image = global::DSPRE.Properties.Resources.expandArrow;
            this.expandScriptTextButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.expandScriptTextButton.Location = new System.Drawing.Point(1107, 11);
            this.expandScriptTextButton.Name = "expandScriptTextButton";
            this.expandScriptTextButton.Size = new System.Drawing.Size(24, 24);
            this.expandScriptTextButton.TabIndex = 28;
            this.expandScriptTextButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.expandScriptTextButton.UseVisualStyleBackColor = true;
            this.expandScriptTextButton.Click += new System.EventHandler(this.ScriptEditorExpandButton_Click);
            // 
            // compressScriptTextButton
            // 
            this.compressScriptTextButton.Image = global::DSPRE.Properties.Resources.compressArrow;
            this.compressScriptTextButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.compressScriptTextButton.Location = new System.Drawing.Point(1135, 11);
            this.compressScriptTextButton.Name = "compressScriptTextButton";
            this.compressScriptTextButton.Size = new System.Drawing.Size(24, 24);
            this.compressScriptTextButton.TabIndex = 29;
            this.compressScriptTextButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.compressScriptTextButton.UseVisualStyleBackColor = true;
            this.compressScriptTextButton.Click += new System.EventHandler(this.ScriptEditorCollapseButton_Click);
            // 
            // scriptEditorWordWrapCheckbox
            // 
            this.scriptEditorWordWrapCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.scriptEditorWordWrapCheckbox.AutoSize = true;
            this.scriptEditorWordWrapCheckbox.Checked = true;
            this.scriptEditorWordWrapCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.scriptEditorWordWrapCheckbox.Location = new System.Drawing.Point(907, 12);
            this.scriptEditorWordWrapCheckbox.Name = "scriptEditorWordWrapCheckbox";
            this.scriptEditorWordWrapCheckbox.Size = new System.Drawing.Size(72, 23);
            this.scriptEditorWordWrapCheckbox.TabIndex = 25;
            this.scriptEditorWordWrapCheckbox.Text = "Word Wrap";
            this.scriptEditorWordWrapCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.scriptEditorWordWrapCheckbox.UseVisualStyleBackColor = true;
            this.scriptEditorWordWrapCheckbox.CheckedChanged += new System.EventHandler(this.scriptEditorWordWrapCheckbox_CheckedChanged);
            // 
            // scriptEditorWhitespacesCheckbox
            // 
            this.scriptEditorWhitespacesCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.scriptEditorWhitespacesCheckbox.AutoSize = true;
            this.scriptEditorWhitespacesCheckbox.Location = new System.Drawing.Point(981, 12);
            this.scriptEditorWhitespacesCheckbox.Name = "scriptEditorWhitespacesCheckbox";
            this.scriptEditorWhitespacesCheckbox.Size = new System.Drawing.Size(79, 23);
            this.scriptEditorWhitespacesCheckbox.TabIndex = 26;
            this.scriptEditorWhitespacesCheckbox.Text = "Whitespaces";
            this.scriptEditorWhitespacesCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.scriptEditorWhitespacesCheckbox.UseVisualStyleBackColor = true;
            this.scriptEditorWhitespacesCheckbox.CheckedChanged += new System.EventHandler(this.viewWhiteSpacesButton_Click);
            // 
            // groupBox26
            // 
            this.groupBox26.Controls.Add(this.scriptEditorNumberFormatNoPreference);
            this.groupBox26.Controls.Add(this.scriptEditorNumberFormatDecimal);
            this.groupBox26.Controls.Add(this.scriptEditorNumberFormatHex);
            this.groupBox26.Location = new System.Drawing.Point(700, 4);
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.Size = new System.Drawing.Size(190, 36);
            this.groupBox26.TabIndex = 50;
            this.groupBox26.TabStop = false;
            this.groupBox26.Text = "Number Format Preference";
            // 
            // scriptEditorNumberFormatNoPreference
            // 
            this.scriptEditorNumberFormatNoPreference.AutoSize = true;
            this.scriptEditorNumberFormatNoPreference.Checked = true;
            this.scriptEditorNumberFormatNoPreference.Location = new System.Drawing.Point(11, 14);
            this.scriptEditorNumberFormatNoPreference.Name = "scriptEditorNumberFormatNoPreference";
            this.scriptEditorNumberFormatNoPreference.Size = new System.Drawing.Size(47, 17);
            this.scriptEditorNumberFormatNoPreference.TabIndex = 22;
            this.scriptEditorNumberFormatNoPreference.TabStop = true;
            this.scriptEditorNumberFormatNoPreference.Text = "Auto";
            this.scriptEditorNumberFormatNoPreference.UseVisualStyleBackColor = true;
            this.scriptEditorNumberFormatNoPreference.CheckedChanged += new System.EventHandler(this.UpdateScriptNumberFormatNoPref);
            // 
            // scriptEditorNumberFormatDecimal
            // 
            this.scriptEditorNumberFormatDecimal.AutoSize = true;
            this.scriptEditorNumberFormatDecimal.Location = new System.Drawing.Point(121, 14);
            this.scriptEditorNumberFormatDecimal.Name = "scriptEditorNumberFormatDecimal";
            this.scriptEditorNumberFormatDecimal.Size = new System.Drawing.Size(63, 17);
            this.scriptEditorNumberFormatDecimal.TabIndex = 24;
            this.scriptEditorNumberFormatDecimal.Text = "Decimal";
            this.scriptEditorNumberFormatDecimal.UseVisualStyleBackColor = true;
            this.scriptEditorNumberFormatDecimal.CheckedChanged += new System.EventHandler(this.UpdateScriptNumberFormatDec);
            // 
            // scriptEditorNumberFormatHex
            // 
            this.scriptEditorNumberFormatHex.AutoSize = true;
            this.scriptEditorNumberFormatHex.Location = new System.Drawing.Point(68, 14);
            this.scriptEditorNumberFormatHex.Name = "scriptEditorNumberFormatHex";
            this.scriptEditorNumberFormatHex.Size = new System.Drawing.Size(44, 17);
            this.scriptEditorNumberFormatHex.TabIndex = 23;
            this.scriptEditorNumberFormatHex.Text = "Hex";
            this.scriptEditorNumberFormatHex.UseVisualStyleBackColor = true;
            this.scriptEditorNumberFormatHex.CheckedChanged += new System.EventHandler(this.UpdateScriptNumberFormatHex);
            // 
            // viewLevelScriptButton
            // 
            this.viewLevelScriptButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.viewLevelScriptButton.Location = new System.Drawing.Point(481, 681);
            this.viewLevelScriptButton.Name = "viewLevelScriptButton";
            this.viewLevelScriptButton.Size = new System.Drawing.Size(91, 25);
            this.viewLevelScriptButton.TabIndex = 6;
            this.viewLevelScriptButton.Text = "View level script";
            this.viewLevelScriptButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.viewLevelScriptButton.UseVisualStyleBackColor = true;
            this.viewLevelScriptButton.Click += new System.EventHandler(this.viewLevelScriptButton_Click);
            // 
            // locateCurrentScriptFile
            // 
            this.locateCurrentScriptFile.Image = global::DSPRE.Properties.Resources.open_file;
            this.locateCurrentScriptFile.Location = new System.Drawing.Point(423, 29);
            this.locateCurrentScriptFile.Name = "locateCurrentScriptFile";
            this.locateCurrentScriptFile.Size = new System.Drawing.Size(42, 40);
            this.locateCurrentScriptFile.TabIndex = 7;
            this.locateCurrentScriptFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.locateCurrentScriptFile.UseVisualStyleBackColor = true;
            this.locateCurrentScriptFile.Click += new System.EventHandler(this.locateCurrentScriptFile_Click);
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.locateCurrentScriptFile);
            this.Controls.Add(this.groupBox24);
            this.Controls.Add(this.viewLevelScriptButton);
            this.Controls.Add(this.selectScriptFileComboBox);
            this.Controls.Add(this.groupBox26);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.scriptEditorWhitespacesCheckbox);
            this.Controls.Add(this.scriptEditorWordWrapCheckbox);
            this.Controls.Add(this.addScriptFileButton);
            this.Controls.Add(this.compressScriptTextButton);
            this.Controls.Add(this.removeScriptFileButton);
            this.Controls.Add(this.expandScriptTextButton);
            this.Controls.Add(this.saveScriptFileButton);
            this.Controls.Add(this.openFindScriptEditorButton);
            this.Controls.Add(this.exportScriptFileButton);
            this.Controls.Add(this.importScriptFileButton);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.scriptEditorTabControl);
            this.Name = "ScriptEditor";
            this.Size = new System.Drawing.Size(1177, 735);
            this.scriptEditorTabControl.ResumeLayout(false);
            this.scriptsTabPage.ResumeLayout(false);
            this.PanelSearchScripts.ResumeLayout(false);
            this.PanelSearchScripts.PerformLayout();
            this.functionsTabPage.ResumeLayout(false);
            this.PanelSearchFunctions.ResumeLayout(false);
            this.PanelSearchFunctions.PerformLayout();
            this.actionsTabPage.ResumeLayout(false);
            this.PanelSearchActions.ResumeLayout(false);
            this.PanelSearchActions.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox24.ResumeLayout(false);
            this.ScriptNavigatorTabControl.ResumeLayout(false);
            this.ScriptsNavTab.ResumeLayout(false);
            this.FunctionsNavTab.ResumeLayout(false);
            this.ActionsNavTab.ResumeLayout(false);
            this.groupBox26.ResumeLayout(false);
            this.groupBox26.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.CheckBox scrollToBlockStartcheckBox;

        #endregion

        public System.Windows.Forms.ComboBox selectScriptFileComboBox;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TabControl scriptEditorTabControl;
        private System.Windows.Forms.TabPage scriptsTabPage;
        private System.Windows.Forms.Panel PanelSearchScripts;
        private System.Windows.Forms.Button BtnNextFindScript;
        private System.Windows.Forms.Button BtnPrevFindScript;
        private System.Windows.Forms.Button BtnCloseFindScript;
        private System.Windows.Forms.TextBox panelFindScriptTextBox;
        private System.Windows.Forms.Panel scintillaScriptsPanel;
        private System.Windows.Forms.TabPage functionsTabPage;
        private System.Windows.Forms.Panel PanelSearchFunctions;
        private System.Windows.Forms.Button BtnNextFindFunc;
        private System.Windows.Forms.Button BtnPrevFindFunc;
        private System.Windows.Forms.Button BtnCloseFindFunc;
        private System.Windows.Forms.TextBox panelFindFunctionTextBox;
        private System.Windows.Forms.Panel scintillaFunctionsPanel;
        private System.Windows.Forms.TabPage actionsTabPage;
        private System.Windows.Forms.Panel PanelSearchActions;
        private System.Windows.Forms.Button BtnNextFindActions;
        private System.Windows.Forms.Button BtnPrevFindActions;
        private System.Windows.Forms.Button BtnCloseFindActions;
        private System.Windows.Forms.TextBox panelFindActionTextBox;
        private System.Windows.Forms.Panel scintillaActionsPanel;
        private System.Windows.Forms.Button addScriptFileButton;
        private System.Windows.Forms.Button removeScriptFileButton;
        private System.Windows.Forms.Button saveScriptFileButton;
        private System.Windows.Forms.Button exportScriptFileButton;
        private System.Windows.Forms.Button importScriptFileButton;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckBox searchOnlyCurrentScriptCheckBox;
        private System.Windows.Forms.CheckBox scriptSearchCaseSensitiveCheckBox;
        private System.Windows.Forms.TextBox searchInScriptsTextBox;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.ProgressBar searchProgressBar;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Button searchInScriptsButton;
        private System.Windows.Forms.ListBox searchInScriptsResultListBox;
        private System.Windows.Forms.GroupBox groupBox24;
        private System.Windows.Forms.TabControl ScriptNavigatorTabControl;
        private System.Windows.Forms.TabPage ScriptsNavTab;
        private System.Windows.Forms.ListBox scriptsNavListbox;
        private System.Windows.Forms.TabPage FunctionsNavTab;
        private System.Windows.Forms.ListBox functionsNavListbox;
        private System.Windows.Forms.TabPage ActionsNavTab;
        private System.Windows.Forms.ListBox actionsNavListbox;
        private System.Windows.Forms.Button openFindScriptEditorButton;
        private System.Windows.Forms.Button expandScriptTextButton;
        private System.Windows.Forms.Button compressScriptTextButton;
        private System.Windows.Forms.CheckBox scriptEditorWordWrapCheckbox;
        private System.Windows.Forms.CheckBox scriptEditorWhitespacesCheckbox;
        private System.Windows.Forms.GroupBox groupBox26;
        private System.Windows.Forms.RadioButton scriptEditorNumberFormatNoPreference;
        private System.Windows.Forms.RadioButton scriptEditorNumberFormatDecimal;
        private System.Windows.Forms.RadioButton scriptEditorNumberFormatHex;
        private System.Windows.Forms.Button viewLevelScriptButton;
        private System.Windows.Forms.Button locateCurrentScriptFile;
    }
}