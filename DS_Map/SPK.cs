using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Reflection;

namespace WindowsFormsApplication1
{
    public partial class SPK : Form
    {
        ResourceManager rm = new ResourceManager("WindowsFormsApplication1.WinFormStrings", Assembly.GetExecutingAssembly());
        #region DPPt Flags
        bool adae = false;
        bool adas = false;
        bool adaf = false;
        bool adai = false;
        bool adag = false;
        bool adaj = false;
        bool adak = false;
        bool apae = false;
        bool apas = false;
        bool apaf = false;
        bool apai = false;
        bool apag = false;
        bool apaj = false;
        bool apak = false;
        bool cpue = false;
        bool cpus = false;
        bool cpuf = false;
        bool cpui = false;
        bool cpug = false;
        bool cpuj = false;
        bool cpuk = false;
        #endregion
        #region HGSS Flags
        bool ipke = false;
        bool ipks = false;
        bool ipkf = false;
        bool ipki = false;
        bool ipkg = false;
        bool ipkj = false;
        bool ipkk = false;
        bool ipge = false;
        bool ipgs = false;
        bool ipgf = false;
        bool ipgi = false;
        bool ipgg = false;
        bool ipgj = false;
        bool ipgk = false;
        #endregion

        public SPK()
        {
            InitializeComponent();
        }

        private void SPK_Load(object sender, EventArgs e)
        {
            checkedListBox2.Items.Add(rm.GetString("diamondb") + " " + rm.GetString("usa"), true);
            checkedListBox2.Items.Add(rm.GetString("diamondb") + " " + rm.GetString("spa"), true);
            checkedListBox2.Items.Add(rm.GetString("diamondb") + " " + rm.GetString("fra"), true);
            checkedListBox2.Items.Add(rm.GetString("diamondb") + " " + rm.GetString("ita"), true);
            checkedListBox2.Items.Add(rm.GetString("diamondb") + " " + rm.GetString("ger"), true);
            checkedListBox2.Items.Add(rm.GetString("diamondb") + " " + rm.GetString("jap"), true);
            checkedListBox2.Items.Add(rm.GetString("diamondb") + " " + rm.GetString("kor"), true);
            checkedListBox2.Items.Add(rm.GetString("pearlb") + " " + rm.GetString("usa"), true);
            checkedListBox2.Items.Add(rm.GetString("pearlb") + " " + rm.GetString("spa"), true);
            checkedListBox2.Items.Add(rm.GetString("pearlb") + " " + rm.GetString("fra"), true);
            checkedListBox2.Items.Add(rm.GetString("pearlb") + " " + rm.GetString("ita"), true);
            checkedListBox2.Items.Add(rm.GetString("pearlb") + " " + rm.GetString("ger"), true);
            checkedListBox2.Items.Add(rm.GetString("pearlb") + " " + rm.GetString("jap"), true);
            checkedListBox2.Items.Add(rm.GetString("pearlb") + " " + rm.GetString("kor"), true);
            checkedListBox2.Items.Add(rm.GetString("platinumb") + " " + rm.GetString("usa"), true);
            checkedListBox2.Items.Add(rm.GetString("platinumb") + " " + rm.GetString("spa"), true);
            checkedListBox2.Items.Add(rm.GetString("platinumb") + " " + rm.GetString("fra"), true);
            checkedListBox2.Items.Add(rm.GetString("platinumb") + " " + rm.GetString("ita"), true);
            checkedListBox2.Items.Add(rm.GetString("platinumb") + " " + rm.GetString("ger"), true);
            checkedListBox2.Items.Add(rm.GetString("platinumb") + " " + rm.GetString("jap"), true);
            checkedListBox2.Items.Add(rm.GetString("platinumb") + " " + rm.GetString("kor"), true);
            checkedListBox2.Items.Add(rm.GetString("heartgoldb") + " " + rm.GetString("usa"), true);
            checkedListBox2.Items.Add(rm.GetString("heartgoldb") + " " + rm.GetString("spa"), true);
            checkedListBox2.Items.Add(rm.GetString("heartgoldb") + " " + rm.GetString("fra"), true);
            checkedListBox2.Items.Add(rm.GetString("heartgoldb") + " " + rm.GetString("ita"), true);
            checkedListBox2.Items.Add(rm.GetString("heartgoldb") + " " + rm.GetString("ger"), true);
            checkedListBox2.Items.Add(rm.GetString("heartgoldb") + " " + rm.GetString("jap"), true);
            checkedListBox2.Items.Add(rm.GetString("heartgoldb") + " " + rm.GetString("kor"), true);
            checkedListBox2.Items.Add(rm.GetString("soulsilverb") + " " + rm.GetString("usa"), true);
            checkedListBox2.Items.Add(rm.GetString("soulsilverb") + " " + rm.GetString("spa"), true);
            checkedListBox2.Items.Add(rm.GetString("soulsilverb") + " " + rm.GetString("fra"), true);
            checkedListBox2.Items.Add(rm.GetString("soulsilverb") + " " + rm.GetString("ita"), true);
            checkedListBox2.Items.Add(rm.GetString("soulsilverb") + " " + rm.GetString("ger"), true);
            checkedListBox2.Items.Add(rm.GetString("soulsilverb") + " " + rm.GetString("jap"), true);
            checkedListBox2.Items.Add(rm.GetString("soulsilverb") + " " + rm.GetString("kor"), true);
            checkedListBox2.Items.Add(rm.GetString("blackb") + " " + rm.GetString("usa"), true);
            checkedListBox2.Items.Add(rm.GetString("blackb") + " " + rm.GetString("spa"), true);
            checkedListBox2.Items.Add(rm.GetString("blackb") + " " + rm.GetString("fra"), true);
            checkedListBox2.Items.Add(rm.GetString("blackb") + " " + rm.GetString("ita"), true);
            checkedListBox2.Items.Add(rm.GetString("blackb") + " " + rm.GetString("ger"), true);
            checkedListBox2.Items.Add(rm.GetString("blackb") + " " + rm.GetString("jap"), true);
            checkedListBox2.Items.Add(rm.GetString("blackb") + " " + rm.GetString("kor"), true);
            checkedListBox2.Items.Add(rm.GetString("whiteb") + " " + rm.GetString("usa"), true);
            checkedListBox2.Items.Add(rm.GetString("whiteb") + " " + rm.GetString("spa"), true);
            checkedListBox2.Items.Add(rm.GetString("whiteb") + " " + rm.GetString("fra"), true);
            checkedListBox2.Items.Add(rm.GetString("whiteb") + " " + rm.GetString("ita"), true);
            checkedListBox2.Items.Add(rm.GetString("whiteb") + " " + rm.GetString("ger"), true);
            checkedListBox2.Items.Add(rm.GetString("whiteb") + " " + rm.GetString("jap"), true);
            checkedListBox2.Items.Add(rm.GetString("whiteb") + " " + rm.GetString("kor"), true);
            checkedListBox2.Items.Add(rm.GetString("black2b") + " " + rm.GetString("usa"), true);
            checkedListBox2.Items.Add(rm.GetString("black2b") + " " + rm.GetString("spa"), true);
            checkedListBox2.Items.Add(rm.GetString("black2b") + " " + rm.GetString("fra"), true);
            checkedListBox2.Items.Add(rm.GetString("black2b") + " " + rm.GetString("ita"), true);
            checkedListBox2.Items.Add(rm.GetString("black2b") + " " + rm.GetString("ger"), true);
            checkedListBox2.Items.Add(rm.GetString("black2b") + " " + rm.GetString("jap"), true);
            checkedListBox2.Items.Add(rm.GetString("black2b") + " " + rm.GetString("kor"), true);
            checkedListBox2.Items.Add(rm.GetString("white2b") + " " + rm.GetString("usa"), true);
            checkedListBox2.Items.Add(rm.GetString("white2b") + " " + rm.GetString("spa"), true);
            checkedListBox2.Items.Add(rm.GetString("white2b") + " " + rm.GetString("fra"), true);
            checkedListBox2.Items.Add(rm.GetString("white2b") + " " + rm.GetString("ita"), true);
            checkedListBox2.Items.Add(rm.GetString("white2b") + " " + rm.GetString("ger"), true);
            checkedListBox2.Items.Add(rm.GetString("white2b") + " " + rm.GetString("jap"), true);
            checkedListBox2.Items.Add(rm.GetString("white2b") + " " + rm.GetString("kor"), true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openSPK = new OpenFileDialog();
            if (openSPK.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openSPK.FileName;
                BinaryReader readSPK = new BinaryReader(File.OpenRead(openSPK.FileName));
                #region Flags
                int flagD = readSPK.ReadByte();
                if ((flagD & (1 << 0)) != 0) adae = true;
                if ((flagD & (1 << 1)) != 0) adas = true;
                if ((flagD & (1 << 2)) != 0) adaf = true;
                if ((flagD & (1 << 3)) != 0) adai = true;
                if ((flagD & (1 << 4)) != 0) adag = true;
                if ((flagD & (1 << 5)) != 0) adaj = true;
                if ((flagD & (1 << 6)) != 0) adak = true;
                int flagP = readSPK.ReadByte();
                if ((flagP & (1 << 0)) != 0) apae = true;
                if ((flagP & (1 << 1)) != 0) apas = true;
                if ((flagP & (1 << 2)) != 0) apaf = true;
                if ((flagP & (1 << 3)) != 0) apai = true;
                if ((flagP & (1 << 4)) != 0) apag = true;
                if ((flagP & (1 << 5)) != 0) apaj = true;
                if ((flagP & (1 << 6)) != 0) apak = true;
                int flagPt = readSPK.ReadByte();
                if ((flagPt & (1 << 0)) != 0) cpue = true;
                if ((flagPt & (1 << 1)) != 0) cpus = true;
                if ((flagPt & (1 << 2)) != 0) cpuf = true;
                if ((flagPt & (1 << 3)) != 0) cpui = true;
                if ((flagPt & (1 << 4)) != 0) cpug = true;
                if ((flagPt & (1 << 5)) != 0) cpuj = true;
                if ((flagPt & (1 << 6)) != 0) cpuk = true;
                int flagHG = readSPK.ReadByte();
                if ((flagHG & (1 << 0)) != 0) ipke = true;
                if ((flagHG & (1 << 1)) != 0) ipks = true;
                if ((flagHG & (1 << 2)) != 0) ipkf = true;
                if ((flagHG & (1 << 3)) != 0) ipki = true;
                if ((flagHG & (1 << 4)) != 0) ipkg = true;
                if ((flagHG & (1 << 5)) != 0) ipkj = true;
                if ((flagHG & (1 << 6)) != 0) ipkk = true;
                int flagSS = readSPK.ReadByte();
                if ((flagSS & (1 << 0)) != 0) ipge = true;
                if ((flagSS & (1 << 1)) != 0) ipgs = true;
                if ((flagSS & (1 << 2)) != 0) ipgf = true;
                if ((flagSS & (1 << 3)) != 0) ipgi = true;
                if ((flagSS & (1 << 4)) != 0) ipgg = true;
                if ((flagSS & (1 << 5)) != 0) ipgj = true;
                if ((flagSS & (1 << 6)) != 0) ipgk = true;
                int flagB = readSPK.ReadByte();
                int flagW = readSPK.ReadByte();
                int flagB2 = readSPK.ReadByte();
                int flagW2 = readSPK.ReadByte();
                #endregion
                readSPK.BaseStream.Position = 0xD;
                string description = "";
                int titleLength = readSPK.ReadUInt16();
                description += Encoding.UTF8.GetString(readSPK.ReadBytes(titleLength)) + "\r\n\r\n";
                int descriptionLength = readSPK.ReadUInt16();
                description += Encoding.UTF8.GetString(readSPK.ReadBytes(descriptionLength));
                //byte[] test = Encoding.UTF8.GetBytes(description);
                textBox2.Text = description;
                int count = readSPK.ReadUInt16();
                for (int i = 0; i < count; i++)
                {
                    int filenameLength = (int)readSPK.ReadUInt32();
                    checkedListBox1.Items.Add(Encoding.UTF8.GetString(readSPK.ReadBytes(filenameLength)));
                    checkedListBox1.SetItemChecked(i, true);
                }
                readSPK.Close();
                button2.Enabled = true;
            }
        }

    }
}
