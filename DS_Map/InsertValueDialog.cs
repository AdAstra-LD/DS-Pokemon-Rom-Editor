using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE
{
    public partial class InsertValueDialog : Form
    {
        public bool okSelected = new bool();

        public InsertValueDialog(string valueLabel, string format)
        {
            InitializeComponent();
            label1.Text = valueLabel;
            numericUpDown1.Hexadecimal = (format == "hex");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            okSelected = true;
            this.Close();
        }
    }
}
