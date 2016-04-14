using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Tools.CSharp.Controls
{
    [ToolboxBitmap(typeof(DataGridViewCheckBoxColumn))]
    public sealed class DataGridCheckBoxColumnExtended : DataGridViewCheckBoxColumn
    {
        #region private
        private readonly DataGridViewCheckBoxColumnHeaderCell _columnHeaderCell;
        //---------------------------------------------------------------------
        private DataGridView _dataGridView;
        //---------------------------------------------------------------------
        private void _setDataGridView(DataGridView dataGridView)
        {
            if (_dataGridView != null)
            { _subscribeDataGridViewAllEvents(false); }

            _dataGridView = dataGridView;

            if (_dataGridView != null)
            { _subscribeDataGridViewAllEvents(true); }
        }

        private void _subscribeDataGridViewAllEvents(bool addEvents)
        {
            if (_dataGridView != null)
            {
                if (addEvents)
                {
                    _dataGridView.CurrentCellDirtyStateChanged += _dataGridViewOnCurrentCellDirtyStateChanged;
                    _dataGridView.CellValueChanged += _dataGridViewOnCellValueChanged;
                }
                else
                {
                    _dataGridView.CurrentCellDirtyStateChanged -= _dataGridViewOnCurrentCellDirtyStateChanged;
                    _dataGridView.CellValueChanged -= _dataGridViewOnCellValueChanged;
                }
            }
        }
        private void _dataGridViewOnCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var dataGridView = (DataGridView)sender;
            if ((e.ColumnIndex > -1 && e.ColumnIndex < dataGridView.ColumnCount)
                && (e.RowIndex > -1 && e.RowIndex < dataGridView.RowCount))
            { _columnHeaderCell.SetCellByCheckedValue(dataGridView[e.ColumnIndex, e.RowIndex]); }
        }
        private static void _dataGridViewOnCurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            var dataGridView = (DataGridView)sender;
            if (dataGridView.IsCurrentCellDirty)
            { dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit); }
        }
        //---------------------------------------------------------------------
        private void _subscribeColumnHeaderCellAllEvents(bool addEvents)
        {
            if (_columnHeaderCell != null)
            {
                if (addEvents)
                { _columnHeaderCell.OnCheckBoxClicked += _columnHeaderCellOnOnCheckBoxClicked; }
                else
                { _columnHeaderCell.OnCheckBoxClicked -= _columnHeaderCellOnOnCheckBoxClicked; }
            }
        }
        private void _columnHeaderCellOnOnCheckBoxClicked(object sender, DataGridViewCheckBoxColumnHeaderCellEventArgs e)
        {
            var dataGridView = DataGridView;

            dataGridView.RefreshEdit();
            foreach (var cell in dataGridView.Rows.Cast<DataGridViewRow>().Select(row => row.Cells[e.ColumnIndex]).Where(cell => !cell.ReadOnly))
            { cell.Value = e.CheckedState; }
            dataGridView.RefreshEdit();
        }
        #endregion
        #region protected
        protected override void OnDataGridViewChanged()
        {
            _setDataGridView(DataGridView);

            base.OnDataGridViewChanged();
        }
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    _subscribeColumnHeaderCellAllEvents(false);
                    _subscribeDataGridViewAllEvents(false);
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion
        public DataGridCheckBoxColumnExtended()
        {
            _columnHeaderCell = new DataGridViewCheckBoxColumnHeaderCell();
            _subscribeColumnHeaderCellAllEvents(true);

            HeaderCell = _columnHeaderCell;
            Width = 50;
        }

        //---------------------------------------------------------------------
        public IEnumerable<DataGridViewRow> GetCheckedRows()
        {
            var dataGridView = DataGridView;
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                var checkCell = row.Cells[DisplayIndex];
                if (checkCell.ValueType == typeof(bool))
                {
                    var cellValue = (bool)checkCell.Value;
                    if (cellValue)
                    { yield return row; }
                }
                else if (checkCell.ValueType == typeof(bool?))
                {
                    var cellValue = (bool?)checkCell.Value;
                    if (cellValue.HasValue && cellValue.Value)
                    { yield return row; }
                }
            }
        }
        //---------------------------------------------------------------------
    }
}
