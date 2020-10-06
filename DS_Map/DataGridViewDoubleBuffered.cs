// DataGridView with double buffering to speed up performance

using System.Windows.Forms;

namespace DS_Map
{
    public class DataGridViewDoubleBuffered : DataGridView
    {
        public DataGridViewDoubleBuffered()
        {
            DoubleBuffered = true;
        }
    }
}