using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using System.Drawing;

namespace DSPRE.Editors
{
    public partial class LoadingForm : Form
    {
        System.ComponentModel.IContainer components = null;

        CheckBox Autofill;
        CheckBox MatchPalette;
        TextBox[] Filenames;
        Button[] LoadPng;
        Button Done;
        RadioButton FBack;
        RadioButton MBack;
        RadioButton FFront;
        RadioButton MFront;
        RadioButton Other;

        public string[] files;
        public bool result;
        public bool paletteMatch;
        public int shinymatch;

        public LoadingForm()
        {
            InitializeComponent();
        }

        void InitializeComponent()
        {
            files = new string[5];
            int spacing = 8;
            int x_start = 8;
            int y_start = 8;
            int default_textbox_width = 260;
            int default_button_width = 110;
            int verticalIndex = y_start;
            shinymatch = 4;
            SuspendLayout();

            Filenames = new TextBox[5];
            LoadPng = new Button[5];

            for (int i = 0; i < 5; i++)
            {
                Filenames[i] = new TextBox();
                Filenames[i].Location = new Point(x_start, verticalIndex);
                Filenames[i].Width = default_textbox_width;
                LoadPng[i] = new Button();
                LoadPng[i].Location = new Point((x_start + Filenames[i].Width + spacing), verticalIndex);
                LoadPng[i].Width = default_button_width;
                LoadPng[i].Click += (Load_PNG);
                LoadPng[i].TabIndex = i;
                Controls.Add(Filenames[i]);
                Controls.Add(LoadPng[i]);
                verticalIndex += (spacing + LoadPng[i].Height);
            }
            LoadPng[0].Text = "Female Backsprites";
            LoadPng[1].Text = "Male Backsprites";
            LoadPng[2].Text = "Female Frontsprites";
            LoadPng[3].Text = "Male Frontsprites";
            LoadPng[4].Text = "Shiny Sprite(any)";

            FBack = new RadioButton();
            MBack = new RadioButton();
            FFront = new RadioButton();
            MFront = new RadioButton();
            Other = new RadioButton();
            int position = x_start;
            verticalIndex -= 4;
            FBack.Text = "Female Back";
            FBack.Location = new Point(position, verticalIndex);
            FBack.Width = 87;
            position += FBack.Width;
            MBack.Text = "Male Back";
            MBack.Location = new Point(position, verticalIndex);
            MBack.Width = 76;
            position += MBack.Width;
            FFront.Text = "Female Front";
            FFront.Location = new Point(position, verticalIndex);
            FFront.Width = 86;
            position += FFront.Width;
            MFront.Text = "Male Front";
            MFront.Location = new Point(position, verticalIndex);
            MFront.Width = 75;
            position += MFront.Width;
            Other.Text = "Other";
            Other.Location = new Point(position, verticalIndex);
            Other.Width = 60;
            Other.Checked = true;
            Controls.Add(FBack);
            Controls.Add(MBack);
            Controls.Add(FFront);
            Controls.Add(MFront);
            Controls.Add(Other);
            verticalIndex += (4 + Other.Height);

            Autofill = new CheckBox();
            Autofill.Text = "Autofill";
            Autofill.Checked = true;
            Autofill.Location = new Point(x_start, verticalIndex);

            MatchPalette = new CheckBox();
            MatchPalette.Text = "Match Current Palette";
            MatchPalette.Width = 140;
            MatchPalette.Checked = false;
            MatchPalette.Location = new Point(x_start + Autofill.Width + spacing, verticalIndex);

            Done = new Button();
            Done.Text = "Done";
            Done.Location = new Point((default_button_width + default_textbox_width + 16 - Done.Width), verticalIndex);
            Done.Click += (Done_Click);

            Controls.Add(Autofill);
            Controls.Add(MatchPalette);
            Controls.Add(Done);

            Height = (verticalIndex + Done.Height + spacing + 30);
            Width = (default_button_width + default_textbox_width + 30);
            ResumeLayout();
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Text = "Select Sprite Files";
            Name = "LoadingForm";
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;

        }

        void Load_PNG(object sender, EventArgs e)
        {
            Button source = sender as Button;
            int index = source.TabIndex;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose an image";
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "Supported fomats: *.bmp, *.gif, *.png | *.bmp; *.gif; *.png";
            openFileDialog.ShowHelp = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Filenames[index].Text = openFileDialog.FileName;
            }
        }

        void Done_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                files[i] = Filenames[i].Text;
            }
            result = Autofill.Checked;
            paletteMatch = MatchPalette.Checked;
            if (FBack.Checked)
                shinymatch = 0;
            if (MBack.Checked)
                shinymatch = 1;
            if (FFront.Checked)
                shinymatch = 2;
            if (MFront.Checked)
                shinymatch = 3;
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

    }
}
