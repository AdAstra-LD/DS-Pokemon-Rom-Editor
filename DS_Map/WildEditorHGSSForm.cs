using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE
{
    public partial class ConfirmBulkEditForm : Form
    {
        private CheckBox doNotAskAgainCheckBox;
        private Button yesButton;
        private Button noButton;
        private Label warningLabel;

        public bool DoNotAskAgain => doNotAskAgainCheckBox.Checked;

        public ConfirmBulkEditForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.warningLabel = new System.Windows.Forms.Label();
            this.doNotAskAgainCheckBox = new System.Windows.Forms.CheckBox();
            this.yesButton = new System.Windows.Forms.Button();
            this.noButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // warningLabel
            // 
            this.warningLabel.AutoSize = true;
            this.warningLabel.Location = new System.Drawing.Point(10, 22);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(264, 13);
            this.warningLabel.TabIndex = 0;
            this.warningLabel.Text = "This will edit all encounters for this selection. Proceed?";
            // 
            // doNotAskAgainCheckBox
            // 
            this.doNotAskAgainCheckBox.AutoSize = true;
            this.doNotAskAgainCheckBox.Location = new System.Drawing.Point(88, 61);
            this.doNotAskAgainCheckBox.Name = "doNotAskAgainCheckBox";
            this.doNotAskAgainCheckBox.Size = new System.Drawing.Size(107, 17);
            this.doNotAskAgainCheckBox.TabIndex = 1;
            this.doNotAskAgainCheckBox.Text = "Do not ask again";
            this.doNotAskAgainCheckBox.UseVisualStyleBackColor = true;
            // 
            // yesButton
            // 
            this.yesButton.Location = new System.Drawing.Point(120, 93);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(75, 23);
            this.yesButton.TabIndex = 2;
            this.yesButton.Text = "Yes";
            this.yesButton.UseVisualStyleBackColor = true;
            this.yesButton.Click += new System.EventHandler(this.yesButton_Click);
            // 
            // noButton
            // 
            this.noButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.noButton.Location = new System.Drawing.Point(201, 93);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(75, 23);
            this.noButton.TabIndex = 3;
            this.noButton.Text = "No";
            this.noButton.UseVisualStyleBackColor = true;
            this.noButton.Click += new System.EventHandler(this.noButton_Click);
            // 
            // ConfirmBulkEditForm
            // 
            this.AcceptButton = this.yesButton;
            this.CancelButton = this.noButton;
            this.ClientSize = new System.Drawing.Size(284, 127);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.yesButton);
            this.Controls.Add(this.doNotAskAgainCheckBox);
            this.Controls.Add(this.warningLabel);
            this.Name = "ConfirmBulkEditForm";
            this.Text = "Confirm Bulk Edit";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            DialogResult= DialogResult.Cancel;
        }
    }
}
