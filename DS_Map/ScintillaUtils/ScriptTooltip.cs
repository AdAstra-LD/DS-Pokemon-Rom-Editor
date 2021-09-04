using ScintillaNET;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE.ScintillaUtils {
    public partial class ScriptTooltip : Form {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectangleRegion
            (
                int nLeftRect,     // x-coordinate of upper-left corner
                int nTopRect,      // y-coordinate of upper-left corner
                int nRightRect,    // x-coordinate of lower-right corner
                int nBottomRect,   // y-coordinate of lower-right corner
                int nWidthEllipse, // height of ellipse
                int nHeightEllipse // width of ellipse
            );

        public string textBuffer { get; set; } = "";

        public ScriptTooltip(string mainKeywords, string textBuffer) {
            InitializeComponent();
            this.textBuffer = textBuffer;
            this.FormBorderStyle = FormBorderStyle.None;
            
            ctrl.ReadOnly = false;
            ctrl.StyleResetDefault();
            ctrl.Styles[Style.Default].Font = "Consolas";
            ctrl.Styles[Style.Default].Size = 10;
            ctrl.Styles[Style.Default].BackColor = Color.FromArgb(0x2F2F2F);
            ctrl.Styles[Style.Default].ForeColor = Color.FromArgb(0xFFFFFF);
            ctrl.StyleClearAll();
            
            // Configure the lexer styles
            ctrl.Styles[Style.Cpp.Identifier].ForeColor = Color.FromArgb(0xD0DAE2);
            ctrl.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0x40BF57);
            ctrl.Styles[Style.Cpp.Number].ForeColor = Color.FromArgb(0xFFFF00);
            ctrl.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(0xFF00FF);
            ctrl.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(0xE95454);
            ctrl.Styles[Style.Cpp.Operator].ForeColor = Color.FromArgb(0xFFFF00);
            ctrl.Styles[Style.Cpp.Word].ForeColor = Color.FromArgb(0x48A8EE);
            ctrl.Styles[Style.Cpp.Word2].ForeColor = Color.FromArgb(0xF98906);
            
            ctrl.SetKeywords(0, mainKeywords);
            Size newSize = TextRenderer.MeasureText(textBuffer, new Font(ctrl.Styles[Style.Default].Font, ctrl.Styles[Style.Default].Size), ctrl.ClientSize, TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            this.ClientSize = new Size(this.ClientSize.Width, newSize.Height + this.panel1.Padding.All);
            Region = Region.FromHrgn(CreateRoundRectangleRegion(0, 0, Width, Height, 10, 10));
        }
        public void WriteText(int delay = 15) {
            ctrl.Text = "";
            ctrl.BufferedDraw = true;

            foreach (char c in this.textBuffer) {
                Thread.Sleep(delay);
                ctrl.Text += c;
                ctrl.Update();
            }
            ctrl.ReadOnly = true;
        }

        public override string ToString() {
            return this.textBuffer;
        }
    }
}
