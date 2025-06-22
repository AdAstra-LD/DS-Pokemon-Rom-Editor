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
    public partial class ReopenProjectConfirmation : Form
    {
        public ReopenProjectConfirmation()
        {
            InitializeComponent();
        }

        private void dontAskAgainCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.neverAskForOpening = dontAskAgainCheckbox.Checked;
        }
    }
}
