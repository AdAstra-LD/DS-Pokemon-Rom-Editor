using System;
using System.IO;
using System.Windows.Forms;

namespace DS_Map
{
    public partial class ASMHacksDialog : Form
    {
        RomInfo info;

        public bool standardizedItems = new bool();

        public ASMHacksDialog(RomInfo romInfo)
        {
            InitializeComponent();
            info = romInfo;
        }

        private void applyItemStandardizeButton_Click(object sender, EventArgs e)
        {
            string path = info.GetScriptFolderPath() + "\\" + info.GetItemScriptFileNumber().ToString("D4");

            ScriptFile file = new ScriptFile(new FileStream(path, FileMode.Open), info.getVersion());
            for (int i = 0; i < file.scripts.Count - 1; i++)
            {
                file.scripts[i].commands[0].parameters[1] = BitConverter.GetBytes((ushort)i); // Fix item index
                file.scripts[i].commands[1].parameters[1] = BitConverter.GetBytes((ushort)1); // Fix item quantity
            }
            using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create))) writer.Write(file.Save());

            standardizedItems = true;
        }

        private void applyARM9ExpansionButton_Click(object sender, EventArgs e)
        {

        }
    }
}
