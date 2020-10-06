using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DS_Map
{
    public partial class InsertValueDialog : Form
    {
        public bool okSelected = new bool();

        public InsertValueDialog(string valueLabel, string format)
        {
            InitializeComponent();
            label1.Text = valueLabel;
            if (format == "hex") numericUpDown1.Hexadecimal = true;
            else numericUpDown1.Hexadecimal = false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            okSelected = true;
            this.Close();
        }
    }
}
