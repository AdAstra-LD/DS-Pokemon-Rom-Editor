using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace DSPRE.Editors
{
    public partial class SpriteCropper : Form
    {
        private System.ComponentModel.IContainer components = null;
        Bitmap[] tiles;
        PictureBox[] pictures;

        public Bitmap Chosen;

        Button Cancel;
        Label label1;

        int max_width = 900;
        int max_height = 900;
        int spacing = 8;

        public SpriteCropper(Bitmap[] input, string identifier)
        {
            label1 = new Label();
            label1.Text = ("Choose the tile for " + identifier + ":");
            tiles = input;
            InitializeComponent();
            LoadImages();
        }

        void InitializeComponent()
        {
            Chosen = null;
            pictures = new PictureBox[tiles.Length];
            Cancel = new Button();
            int x_index = spacing;
            int y_index = spacing;
            int total_x = 0;
            int total_y = 0;
            int y_offset = 0;
            SuspendLayout();
            label1.AutoSize = true;
            label1.Location = new Point(x_index, y_index);
            label1.Font = new Font("Arial", 24, FontStyle.Bold);
            Controls.Add(label1);
            y_index += (label1.Height + spacing);

            for (int i = 0; i < tiles.Length; i++)
            {
                if ((x_index + tiles[i].Width * 2) > max_width)
                {
                    if ((y_index + tiles[i].Height * 2) > max_height)
                    {
                        //Add scrollbar or fail
                    }
                    y_index += y_offset;
                    x_index = spacing;
                    y_offset = 0;
                }
                pictures[i] = new PictureBox();
                pictures[i].Size = new Size(tiles[i].Width * 2, tiles[i].Height * 2);
                pictures[i].Location = new Point(x_index, y_index);
                pictures[i].Name = ("" + i);
                pictures[i].Click += (Selected);
                Controls.Add(pictures[i]);
                x_index = x_index + pictures[i].Width + spacing;
                if (pictures[i].Height + spacing > y_offset)
                    y_offset = pictures[i].Height + spacing;
                if (x_index > total_x)
                    total_x = x_index;
                if (y_index + y_offset > total_y)
                    total_y = y_index + y_offset;
            }
            Cancel.AutoSize = true;
            Cancel.Text = "Cancel";
            Cancel.Location = new Point((total_x - Cancel.Width - spacing), total_y);
            Cancel.Click += (Cancel_Click);
            Controls.Add(Cancel);
            ResumeLayout();

            total_y = total_y + Cancel.Height + spacing;

            Width = total_x + 8;
            if (Width < label1.Width + spacing * 2)
                Width = label1.Width + spacing * 2;
            Height = total_y + 24;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Text = "Choose which sprite to use";
            Name = "SpriteCropper";
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        void LoadImages()
        {
            for (int i = 0; i < pictures.Length; i++)
            {
                pictures[i].Image = new Bitmap(tiles[i], tiles[i].Width * 2, tiles[i].Height * 2);
            }
        }

        void Selected(object sender, EventArgs e)
        {
            PictureBox source = sender as PictureBox;
            int index = Convert.ToInt32(source.Name);
            Chosen = tiles[index];
            DialogResult = DialogResult.OK;
            Close();
        }

        void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
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
