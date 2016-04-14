using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Tools.CSharp.Collections;
using Tools.CSharp.Controls.Extensions;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Controls.Utils.Extensions
{
    public static class DataGridViewExtensions
    {
        #region private
        private static readonly object _CreateRowsKey = new object();
        //---------------------------------------------------------------------
        private static void _SetVerticalOffset(DataGridView dataGridView, int verticalOffset)
        {
            var verticalScrollBarProperty = dataGridView.GetType().GetProperty("VerticalScrollBar", BindingFlags.NonPublic | BindingFlags.Instance);
            var verticalScrollBar = (ScrollBar)verticalScrollBarProperty.GetValue(dataGridView, null);

            _SetVerticalOffset(dataGridView, verticalScrollBar, verticalOffset);
        }
        private static void _SetMaxVerticalOffset(DataGridView dataGridView)
        {
            var verticalScrollBarProperty = dataGridView.GetType().GetProperty("VerticalScrollBar", BindingFlags.NonPublic | BindingFlags.Instance);
            var verticalScrollBar = (ScrollBar)verticalScrollBarProperty.GetValue(dataGridView, null);

            _SetVerticalOffset(dataGridView, verticalScrollBar, verticalScrollBar.Maximum);
        }
        private static void _SetMinVerticalOffset(DataGridView dataGridView)
        {
            var verticalScrollBarProperty = dataGridView.GetType().GetProperty("VerticalScrollBar", BindingFlags.NonPublic | BindingFlags.Instance);
            var verticalScrollBar = (ScrollBar)verticalScrollBarProperty.GetValue(dataGridView, null);

            _SetVerticalOffset(dataGridView, verticalScrollBar, verticalScrollBar.Minimum);
        }
        private static void _SetVerticalOffset(DataGridView dataGridView, ScrollBar scrollBar, int verticalOffset)
        {
            var newVerticalOffset = verticalOffset.GetValue(scrollBar.Minimum, scrollBar.Maximum);
            var verticalOffsetProperty = dataGridView.GetType().GetProperty("VerticalOffset", BindingFlags.NonPublic | BindingFlags.Instance);
            try { verticalOffsetProperty.SetValue(dataGridView, newVerticalOffset, null); }
            catch (TargetInvocationException) { }
        }
        #endregion
        //---------------------------------------------------------------------
        public static bool IsCreatingRows(this DataGridView dataGridView)
        {
            return dataGridView != null && ReferenceEquals(dataGridView.Tag, _CreateRowsKey);
        }

        public static int CreateRows(this DataGridView dataGridView, int rowCount)
        {
            if (dataGridView == null)
            { throw new ArgumentNullException(nameof(dataGridView)); }

            if (rowCount < 0)
            { return dataGridView.RowCount; }

            var oldRowCount = dataGridView.RowCount;
            var renameRows = oldRowCount - rowCount;

            if (renameRows != 0)
            {
                var isVisible = dataGridView.Visible;

                try
                {
                    dataGridView.Tag = _CreateRowsKey;

                    if (isVisible)
                    { dataGridView.SuspendDrawing(); }

                    if (renameRows > 0)
                    {
                        while (renameRows != 0)
                        {
                            Application.DoEvents();

                            renameRows--;

                            var lastIndex = dataGridView.RowCount - 1;
                            if (lastIndex >= 0)
                            { dataGridView.Rows.RemoveAt(lastIndex); }
                        }
                    }
                    else if (renameRows < 0)
                    {
                        if (oldRowCount == 0)
                        {
                            var row = new DataGridViewRow();
                            row.CreateCells(dataGridView);
                            dataGridView.Rows.Add(row);

                            renameRows += 1;
                        }

                        if (renameRows < 0)
                        { dataGridView.Rows.AddCopies(0, (-1) * renameRows); }
                    }
                }
                finally
                {
                    dataGridView.Tag = null;

                    if (isVisible)
                    { dataGridView.ResumeDrawing(); }
                }
            }

            return dataGridView.RowCount;
        }
        //---------------------------------------------------------------------
        public static void Enabled(this DataGridViewCell cell, bool enable)
        {
            if (cell == null)
            { throw new ArgumentNullException(nameof(cell)); }

            cell.ReadOnly = !enable;
            cell.Style.ForeColor = enable ? cell.OwningColumn.DefaultCellStyle.ForeColor : Color.DarkGray;
        }
        //---------------------------------------------------------------------
        public static int GetVerticalOffset(this DataGridView dataGridView)
        {
            if (dataGridView == null)
            { throw new ArgumentNullException(nameof(dataGridView)); }

            return dataGridView.VerticalScrollingOffset;
        }
        //---------------------------------------------------------------------
        public static void UpdateSelectedRows<TViewModel, TViewModelCollection>(
            this DoubleBufferedDataGridView dataGridView,
            SortedItemCollection<TViewModel, TViewModelCollection> collection,
            Action<DoubleBufferedDataGridView, bool> action,
            int verticalOffset,
            IEnumerable<int> selectedRowIndexes
        ) where TViewModelCollection : IList<TViewModel>
        {
            if (dataGridView == null)
            { throw new ArgumentNullException(nameof(dataGridView)); }
            if (collection == null)
            { throw new ArgumentNullException(nameof(collection)); }

            if (!collection.IsAvailable)
            { return; }

            var rowCount = collection.Count;
            var dataGridViewRowCount = dataGridView.RowCount;

            if (rowCount > dataGridViewRowCount)
            { return; }

            if (action == null)
            { throw new ArgumentNullException(nameof(action)); }

            action(dataGridView, false);
            dataGridView.ClearSelection();
            bool? firstOrLastOrEmptySelectedRowIndex = null;
            var lastRowIndex = rowCount - 1;

            foreach (var selectedRowIndex in selectedRowIndexes)
            {
                if (selectedRowIndex == 0) { firstOrLastOrEmptySelectedRowIndex = true; }
                else if (selectedRowIndex == lastRowIndex) { firstOrLastOrEmptySelectedRowIndex = false; }

                if (selectedRowIndex < dataGridViewRowCount)
                { dataGridView.Rows[selectedRowIndex].Selected = true; }
            }

            action(dataGridView, true);

            if (dataGridView.SelectedRows.Count != 0)
            {
                if (firstOrLastOrEmptySelectedRowIndex.HasValue)
                {
                    if (firstOrLastOrEmptySelectedRowIndex.Value)
                    { _SetMinVerticalOffset(dataGridView); }
                    else
                    { _SetMaxVerticalOffset(dataGridView); }
                }
                else
                { _SetVerticalOffset(dataGridView, verticalOffset); }
            }

            if (dataGridViewRowCount != 0 && dataGridView.SelectedRows.Count == 0)
            {
                dataGridView.Rows[0].Selected = true;
                _SetMinVerticalOffset(dataGridView);
            }
        }
        //---------------------------------------------------------------------
    }
}
