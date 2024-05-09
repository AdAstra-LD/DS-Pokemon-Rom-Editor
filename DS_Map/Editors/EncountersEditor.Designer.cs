
namespace DSPRE.Editors
{
  partial class EncountersEditor
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageHeadbuttEditor = new System.Windows.Forms.TabPage();
            this.headbuttEncounterEditor = new DSPRE.Editors.HeadbuttEncounterEditor();
            this.tabPageSafariZoneEditor = new System.Windows.Forms.TabPage();
            this.safariZoneEditor = new DSPRE.Editors.SafariZoneEditor();
            this.tabControl.SuspendLayout();
            this.tabPageHeadbuttEditor.SuspendLayout();
            this.tabPageSafariZoneEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageHeadbuttEditor);
            this.tabControl.Controls.Add(this.tabPageSafariZoneEditor);
            this.tabControl.Location = new System.Drawing.Point(4, 4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1103, 654);
            this.tabControl.TabIndex = 2;
            // 
            // tabPageHeadbuttEditor
            // 
            this.tabPageHeadbuttEditor.Controls.Add(this.headbuttEncounterEditor);
            this.tabPageHeadbuttEditor.Location = new System.Drawing.Point(4, 22);
            this.tabPageHeadbuttEditor.Name = "tabPageHeadbuttEditor";
            this.tabPageHeadbuttEditor.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHeadbuttEditor.Size = new System.Drawing.Size(1095, 628);
            this.tabPageHeadbuttEditor.TabIndex = 0;
            this.tabPageHeadbuttEditor.Text = "Headbutt";
            this.tabPageHeadbuttEditor.UseVisualStyleBackColor = true;
            this.tabPageHeadbuttEditor.Enter += new System.EventHandler(this.tabPageHeadbuttEditor_Enter);
            // 
            // headbuttEncounterEditor
            // 
            this.headbuttEncounterEditor.BackColor = System.Drawing.SystemColors.Control;
            this.headbuttEncounterEditor.headbuttEncounterEditorIsReady = false;
            this.headbuttEncounterEditor.Location = new System.Drawing.Point(6, 6);
            this.headbuttEncounterEditor.Name = "headbuttEncounterEditor";
            this.headbuttEncounterEditor.Size = new System.Drawing.Size(1081, 621);
            this.headbuttEncounterEditor.TabIndex = 1;
            // 
            // tabPageSafariZoneEditor
            // 
            this.tabPageSafariZoneEditor.Controls.Add(this.safariZoneEditor);
            this.tabPageSafariZoneEditor.Location = new System.Drawing.Point(4, 22);
            this.tabPageSafariZoneEditor.Name = "tabPageSafariZoneEditor";
            this.tabPageSafariZoneEditor.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSafariZoneEditor.Size = new System.Drawing.Size(1095, 628);
            this.tabPageSafariZoneEditor.TabIndex = 1;
            this.tabPageSafariZoneEditor.Text = "Safari Zone";
            this.tabPageSafariZoneEditor.UseVisualStyleBackColor = true;
            this.tabPageSafariZoneEditor.Enter += new System.EventHandler(this.tabPageSafariZoneEditor_Enter);
            // 
            // safariZoneEditor
            // 
            this.safariZoneEditor.Location = new System.Drawing.Point(6, 6);
            this.safariZoneEditor.Name = "safariZoneEditor";
            this.safariZoneEditor.safariZoneEditorIsReady = false;
            this.safariZoneEditor.Size = new System.Drawing.Size(996, 341);
            this.safariZoneEditor.TabIndex = 1;
            // 
            // EncountersEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Name = "EncountersEditor";
            this.Size = new System.Drawing.Size(1111, 664);
            this.tabControl.ResumeLayout(false);
            this.tabPageHeadbuttEditor.ResumeLayout(false);
            this.tabPageSafariZoneEditor.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    public HeadbuttEncounterEditor headbuttEncounterEditor;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage tabPageHeadbuttEditor;
    private System.Windows.Forms.TabPage tabPageSafariZoneEditor;
    public SafariZoneEditor safariZoneEditor;
  }
}
