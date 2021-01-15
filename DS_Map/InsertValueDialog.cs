using System;
using System.Windows.Forms;

namespace DSPRE
{
    public partial class InsertValueDialog : Form
    {
        public bool okSelected = new bool();

        public InsertValueDialog(string valueLabel, string format) {
            InitializeComponent();
            numericUpDown1.Focus();
            label1.Text = valueLabel;
            numericUpDown1.Hexadecimal = (format == "hex");
        }

        private void okButton_Click(object sender, EventArgs e) {
            okSelected = true;
            this.Close();
        }

        private void backButton_Click(object sender, EventArgs e) {
            okSelected = false;
            this.Close();
        }

        private void numericUpDown1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                okSelected = false;
                this.Close();
            }
            if (e.KeyCode == Keys.Enter) {
                okSelected = true;
                this.Close();
            }
        }
    }
}
