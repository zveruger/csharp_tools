using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Tools.CSharp.Controls
{
    internal sealed class DataGridViewCheckBoxColumnHeaderCell : DataGridViewColumnHeaderCell
    {
        #region private
        private Point _checkBoxLocation;
        private Size _checkBoxSize;
        private CheckBoxState _checkBoxState = CheckBoxState.UncheckedNormal;
        private Point _cellLocation;
        private bool? _checkedAll = false;
        private readonly Dictionary<DataGridViewCell, object> _cache = new Dictionary<DataGridViewCell, object>();
        private readonly Dictionary<DataGridViewCell, object> _cacheMixed = new Dictionary<DataGridViewCell, object>();
        //---------------------------------------------------------------------
        private static int _CacheItemsCount(Dictionary<DataGridViewCell, object> cache)
        {
            return cache.Count;
        }
        private static void _AddInCache(Dictionary<DataGridViewCell, object> cache, DataGridViewCell cell)
        {
            object value;
            if (!cache.TryGetValue(cell, out value))
            { cache.Add(cell, cell.Value); }
        }
        private static void _RemoveFromCache(Dictionary<DataGridViewCell, object> cache, DataGridViewCell cell)
        {
            cache.Remove(cell);
        }

        private void _addOrRemoveFromCache(DataGridViewCell cell)
        {
            var isCellValueChanged = true;
            if (cell.ValueType == typeof(bool))
            {
                var cellValue = (bool)cell.Value;
                _addOrRemoveFromCache(cell, cellValue);
            }
            else if (cell.ValueType == typeof(bool?))
            {
                var cellValue = (bool?)cell.Value;
                _addOrRemoveFromCache(cell, cellValue);
            }
            else
            { isCellValueChanged = false; }

            if (isCellValueChanged)
            { _checkedAllChanged(); }
        }
        private void _addOrRemoveFromCache(DataGridViewCell cell, bool cellValue)
        {
            if (cellValue)
            { _AddInCache(_cache, cell); }
            else
            { _RemoveFromCache(_cache, cell); }
        }
        private void _addOrRemoveFromCache(DataGridViewCell cell, bool? cellValue)
        {
            if (cellValue.HasValue)
            {
                _RemoveFromCache(_cacheMixed, cell);
                _addOrRemoveFromCache(cell, cellValue.Value);
            }
            else
            { _AddInCache(_cacheMixed, cell); }
        }
        //---------------------------------------------------------------------
        private void _checkedAllChanged()
        {
            var cacheCount = _CacheItemsCount(_cache);
            var cacheMixedCount = _CacheItemsCount(_cacheMixed);
            var rowCount = DataGridView.RowCount;

            if (cacheCount == 0)
            { _checkedAll = cacheMixedCount == 0 ? (bool?)false : null; }
            else
            { _checkedAll = cacheCount == rowCount ? (bool?)true : null; }

            DataGridView.Invalidate();
        }
        //---------------------------------------------------------------------
        private CheckBoxState _getCheckBoxState(bool? checkedAll)
        {
            if (checkedAll.HasValue)
            {
                return checkedAll.Value ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
            }
            return CheckBoxState.MixedNormal;
        }
        //---------------------------------------------------------------------
        private void _onCheckBoxClicked(int columnIndex, bool checkedState)
        {
            OnCheckBoxClicked?.Invoke(this, new DataGridViewCheckBoxColumnHeaderCellEventArgs(columnIndex, checkedState));
        }
        #endregion
        #region protected
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates dataGridViewElementState, object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts
            )
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            _cellLocation = cellBounds.Location;
            _checkBoxState = _getCheckBoxState(_checkedAll);
            _checkBoxSize = CheckBoxRenderer.GetGlyphSize(graphics, _checkBoxState);
            _checkBoxLocation = new Point(
                cellBounds.Location.X + (cellBounds.Width / 2) - (_checkBoxSize.Width / 2),
                cellBounds.Location.Y + (cellBounds.Height / 2) - (_checkBoxSize.Height / 2)
                );

            CheckBoxRenderer.DrawCheckBox(graphics, _checkBoxLocation, _checkBoxState);
        }
        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            var point = new Point(e.X + _cellLocation.X, e.Y + _cellLocation.Y);
            if ((point.X >= _checkBoxLocation.X && point.X <= (_checkBoxLocation.X + _checkBoxSize.Width))
                && (point.Y >= _checkBoxLocation.Y && point.Y <= (_checkBoxLocation.Y + _checkBoxSize.Height))
                )
            {
                if (DataGridView != null && DataGridView.RowCount != 0)
                {
                    _checkedAll = !_checkedAll.HasValue || !_checkedAll.Value;
                    _onCheckBoxClicked(e.ColumnIndex, _checkedAll.Value);
                }
            }

            base.OnMouseClick(e);
        }
        #endregion
        //---------------------------------------------------------------------
        public bool? CheckedAll
        {
            get { return _checkedAll; }
            set
            {
                if (value.HasValue)
                { _checkedAll = value.Value; }
            }
        }
        //---------------------------------------------------------------------
        internal void SetCellByCheckedValue(DataGridViewCell cell)
        {
            if (cell != null)
            { _addOrRemoveFromCache(cell); }
        }
        //---------------------------------------------------------------------
        internal event EventHandler<DataGridViewCheckBoxColumnHeaderCellEventArgs> OnCheckBoxClicked;
        //---------------------------------------------------------------------
    }
}
