using System.ComponentModel;
using System.Windows.Forms;

namespace DSPRE {
    public partial class InputComboBox : ComboBox {
        public InputComboBox() {
            DropDownStyle = ComboBoxStyle.DropDown;

            AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Enter) {
                string input = Text;
                int index = FindStringExact(input.Trim());
                if (index != -1) {
                    SelectedIndex = index;
                }
            }
        }

        [Browsable(false)]
        public new ComboBoxStyle DropDownStyle {
            get { return base.DropDownStyle; }
            set { base.DropDownStyle = ComboBoxStyle.DropDown; }
        }
    }

}
