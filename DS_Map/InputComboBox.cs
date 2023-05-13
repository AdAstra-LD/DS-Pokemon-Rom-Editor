using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DSPRE {
    public partial class InputComboBox : ComboBox {
        private Color normalColor;

        public InputComboBox() {
            normalColor = this.BackColor;
            DropDownStyle = ComboBoxStyle.DropDown;

            AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void UpdateText() {
            string input = Text;
            int index = FindStringExact(input.Trim());
            if (index == -1) {
                this.BackColor = Color.IndianRed;
            } else {
                this.BackColor = normalColor;
                SelectedIndex = index;
            }
        }
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Enter) {
                UpdateText();
            }
        }
        protected override void OnLeave(EventArgs e) {
            base.OnLeave(e);
            UpdateText();
        }

        [Browsable(false)]
        public new ComboBoxStyle DropDownStyle {
            get { return base.DropDownStyle; }
            set { base.DropDownStyle = ComboBoxStyle.DropDown; }
        }
    }
}
