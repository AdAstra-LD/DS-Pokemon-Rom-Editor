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
    public partial class SizeChooser : Form
    {
        System.ComponentModel.IContainer components = null;

        ComboBox Sizes;
        Button Done;
        Button Cancel;

        public int choice;

        public SizeChooser()
        {
            InitializeComponent();

        }

        void InitializeComponent()
        {
            Sizes = new ComboBox();
            Done = new Button();
            Cancel = new Button();
            SuspendLayout();
            Sizes.Items.Add("64x64");
            Sizes.Items.Add("80x80");
            Sizes.Items.Add("160x80");
            Sizes.Location = new Point(78, 18);
            Done.AutoSize = true;
            Done.Location = new Point(60, 46);
            Done.Text = "Done";
            Done.Click += (Done_Click);
            Cancel.AutoSize = true;
            Cancel.Location = new Point(150, 46);
            Cancel.Text = "Cancel";
            Cancel.Click += (Cancel_Click);
            Controls.Add(Sizes);
            Controls.Add(Done);
            Controls.Add(Cancel);
            ResumeLayout();
            Height = 120;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Text = "Choose a tile size";
            Name = "SizeChooser";
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Sizes.SelectedIndex = 0;
        }

        void Done_Click(object sender, EventArgs e)
        {
            choice = Sizes.SelectedIndex;
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
