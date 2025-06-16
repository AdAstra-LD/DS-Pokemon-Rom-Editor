using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace DSPRE
{
    public partial class DVCalcNatureViewerForm : Form
    {

        private List<DVIVNatureTriplet> data;
        private int sortedColumnIndex = 0;
        private bool sortAscending = true;
        public int selectedDV = -1;

        public DVCalcNatureViewerForm(List<DVIVNatureTriplet> data)
        {
            InitializeComponent();
            this.data = data;
            PopulateDataGridView();
        }

        private void PopulateDataGridView()
        {
            // Create a BindingList to bind to the DataGridView
            var bindingList = new BindingList<DVIVNatureTriplet>(data.ToList());

            // Set the DataSource of the DataGridView
            natureGridView.DataSource = bindingList;

            // Set the columns
            natureGridView.Columns[0].HeaderText = "DV";
            natureGridView.Columns[0].DataPropertyName = "DV";
            natureGridView.Columns[1].HeaderText = "IV";
            natureGridView.Columns[1].DataPropertyName = "IV";
            natureGridView.Columns[2].HeaderText = "Nature";
            natureGridView.Columns[2].DataPropertyName = "Nature";            

            // Adjust column widths
            natureGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void natureGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Get the selected triplet
                var selectedTriplet = data[e.RowIndex];
                selectedDV = selectedTriplet.DV;
                this.Close();
            }
        }

        private void natureGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sortedColumnIndex == e.ColumnIndex)
            {
                // If the column is already sorted, toggle the direction
                sortAscending = !sortAscending;
            }
            else
            {
                // If a different column is clicked, sort ascending by default
                // "Ascending" since the order is actually inverted for DV and IV
                sortAscending = true;
            }

            DVCalculator.SortTriplets(ref data, natureGridView.Columns[e.ColumnIndex].DataPropertyName, sortAscending);
            sortedColumnIndex = e.ColumnIndex;

            var bindingList = new BindingList<DVIVNatureTriplet>(data.ToList());
            natureGridView.DataSource = bindingList;
        }
    }
}
