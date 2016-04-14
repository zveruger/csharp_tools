using System.Drawing;
using System.Windows.Forms;

namespace Tools.CSharp.Controls
{
    public class DoubleBufferedDataGridViewExtended : DoubleBufferedDataGridView
    {
        public DoubleBufferedDataGridViewExtended()
        {
            ScrollBars = ScrollBars.Vertical;
            RowHeadersDefaultCellStyle.BackColor = Color.White;
            AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //-------------------------------
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToOrderColumns = false;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            MultiSelect = true;
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            ColumnHeadersVisible = true;
            BackgroundColor = Color.Gainsboro;
            RowHeadersVisible = false;
        }
    }
}
