using System.Windows.Forms;

namespace DSPRE
{
    public partial class GivePokémonDialog : Form
    {
        public string command = "\nGivePokémon ";
        public bool okSelected;
        public GivePokémonDialog(string[] pokémonNames, string[] itemNames, string[] moveNames)
        {
            InitializeComponent();
            speciesComboBox.DataSource = pokémonNames;
            itemComboBox.DataSource = itemNames;
            move1ComboBox.Items.AddRange(moveNames);
            move2ComboBox.Items.AddRange(moveNames);
            move3ComboBox.Items.AddRange(moveNames);
            move4ComboBox.Items.AddRange(moveNames);
        }

        private void movesetCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (movesetCheckBox.Checked) movesetGroupBox.Enabled = true;
            else movesetGroupBox.Enabled = false;
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            command += "0x" + speciesComboBox.SelectedIndex.ToString("X") + " ";
            command += "0x" + ((int)levelNumericUpDown.Value).ToString("X") + " ";
            command += "0x" + itemComboBox.SelectedIndex.ToString("X") + " ";
            command += "0x800C";
            okSelected = true;
            this.Close();
        }
    }
}
