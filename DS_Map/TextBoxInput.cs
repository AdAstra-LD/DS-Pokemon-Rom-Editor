using System;
using System.Drawing;
using System.Windows.Forms;

namespace DSPRE {
    public class TextBoxInput : Form {
        private readonly TextBox userInputAreaTextBox;

        public string userInput { get; private set; }
        public string[] lines { get; private set; }

        public TextBoxInput(string infoText = null, bool multiline = true, int maxLen = 32767) {
            Text = "Text Input";
            ClientSize = new Size(464, 412);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(480, 140);
            Padding = new Padding(3);

            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Padding = new Padding(2),
                RowCount = 3
            };
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle());

            Label infoLabel = new Label {
                AutoSize = true,
                Text = "Info:"
            };

            Label infoGuideTextLabel = new Label {
                AutoSize = true,
                Dock = DockStyle.Fill,
                MaximumSize = new Size(370, 0),
                Text = string.IsNullOrWhiteSpace(infoText) ? "Input your text in the box below." : infoText,
                TextAlign = ContentAlignment.MiddleLeft
            };

            userInputAreaTextBox = new TextBox {
                Dock = DockStyle.Fill,
                Margin = new Padding(3, 6, 3, 6),
                MaxLength = maxLen,
                Multiline = multiline,
                ScrollBars = ScrollBars.Both
            };

            Button confirmButton = new Button {
                Dock = DockStyle.Fill,
                Text = "Confirm"
            };
            confirmButton.Click += confirmButton_Click;

            tableLayoutPanel.Controls.Add(infoLabel, 0, 0);
            tableLayoutPanel.Controls.Add(infoGuideTextLabel, 1, 0);
            tableLayoutPanel.Controls.Add(userInputAreaTextBox, 1, 1);
            tableLayoutPanel.Controls.Add(confirmButton, 1, 2);

            Controls.Add(tableLayoutPanel);
            AcceptButton = confirmButton;
        }

        private void confirmButton_Click(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(userInputAreaTextBox.Text)) {
                DialogResult = DialogResult.No;
                return;
            }

            userInput = userInputAreaTextBox.Text;
            lines = userInputAreaTextBox.Lines;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
