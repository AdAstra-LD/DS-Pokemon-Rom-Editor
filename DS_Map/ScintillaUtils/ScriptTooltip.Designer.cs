
namespace DSPRE.ScintillaUtils {
    partial class ScriptTooltip {
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.ctrl = new ScintillaNET.Scintilla();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panel1.Controls.Add(this.ctrl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(15);
            this.panel1.Size = new System.Drawing.Size(326, 126);
            this.panel1.TabIndex = 1;
            // 
            // ctrl
            // 
            this.ctrl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ctrl.CaretPeriod = 500;
            this.ctrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrl.EdgeColor = System.Drawing.Color.White;
            this.ctrl.EdgeMode = ScintillaNET.EdgeMode.MultiLine;
            this.ctrl.HScrollBar = false;
            this.ctrl.Lexer = ScintillaNET.Lexer.Cpp;
            this.ctrl.Location = new System.Drawing.Point(15, 15);
            this.ctrl.Margin = new System.Windows.Forms.Padding(0);
            this.ctrl.Margins.Capacity = 0;
            this.ctrl.Margins.Left = 0;
            this.ctrl.Margins.Right = 0;
            this.ctrl.MouseDwellTime = 350;
            this.ctrl.Name = "ctrl";
            this.ctrl.PhasesDraw = ScintillaNET.Phases.Multiple;
            this.ctrl.Size = new System.Drawing.Size(296, 96);
            this.ctrl.TabDrawMode = ScintillaNET.TabDrawMode.Strikeout;
            this.ctrl.TabIndex = 0;
            this.ctrl.Text = "Empty";
            this.ctrl.Visible = false;
            this.ctrl.VScrollBar = false;
            this.ctrl.WrapIndentMode = ScintillaNET.WrapIndentMode.Same;
            this.ctrl.WrapMode = ScintillaNET.WrapMode.Word;
            // 
            // ScriptTooltip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.ClientSize = new System.Drawing.Size(326, 126);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScriptTooltip";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ScriptTooltip";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        public ScintillaNET.Scintilla ctrl;
    }
}