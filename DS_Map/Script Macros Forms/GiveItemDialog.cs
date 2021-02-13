using System;
using System.Windows.Forms;

namespace DSPRE
{
    public partial class GiveItemDialog : Form
    {
        public bool okSelected = new bool();
        public string command;

        public GiveItemDialog(string[] itemNames)
        {
            InitializeComponent();
            itemComboBox.DataSource = itemNames;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            okSelected = true;
            this.Close();
        }
    }
}