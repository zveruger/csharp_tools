using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Forms;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.FindDatas
{
    //-------------------------------------------------------------------------
    public class DataGridViewFindRowsIndexes : DataGridViewFindRows
    {
        #region private
        private static readonly ReadOnlyCollection<int> _EmptyRowIndexColeection = new List<int>().AsReadOnly();
        #endregion
        #region protected
        protected virtual void OnRowIndexFindedRaise(int rowIndex)
        {
            RowIndexFinded?.Invoke(this, new DataGridViewFindRowIndexEventArgs(rowIndex));
        }
        //---------------------------------------------------------------------
        protected override void FindTextAction(DataGridView dataGridView, string findText, out ReadOnlyCollection<int> rowIndexesResult)
        {
            if (dataGridView == null)
            { throw new ArgumentNullException(nameof(dataGridView)); }

            rowIndexesResult = _EmptyRowIndexColeection;

            if (!string.IsNullOrWhiteSpace(findText))
            {
                var rows = dataGridView.Rows;
                var columns = dataGridView.Columns;
                var saveColumnCount = columns.Count;
                var isNoFindedRowIndex = true;

                var excludedColumnIndexCollection = CreateExcludedColumnIndexCollection();
                excludedColumnIndexCollection = Interlocked.CompareExchange(ref excludedColumnIndexCollection, EmptyExcludedColumnIndexCollection, null);

                for (var rowCounter = 0; rowCounter < rows.Count; rowCounter++)
                {
                    if (IsCancelFindText)
                    { break; }

                    var row = rows[rowCounter];

                    if (IsMultiFindText || isNoFindedRowIndex)
                    {
                        for (var columnCounter = 0; columnCounter < columns.Count; columnCounter++)
                        {
                            if (IsCancelFindText)
                            { break; }

                            if (saveColumnCount > columns.Count)
                            {
                                saveColumnCount = columns.Count;
                                rowCounter = 0;
                                isNoFindedRowIndex = true;
                                break;
                            }

                            var columnAvailable = columns[columnCounter] as DataGridViewTextBoxColumn;
                            if (columnAvailable != null && columnAvailable.Visible)
                            {
                                if (!excludedColumnIndexCollection.Contains(columnAvailable.Index))
                                {
                                    var cellValue = row.Cells[columnAvailable.DisplayIndex].Value;
                                    var isFinded = cellValue != null && cellValue.ToString().FoundByPattern(findText);

                                    if (isFinded)
                                    {
                                        isNoFindedRowIndex = false;
                                        OnRowIndexFindedRaise(row.Index);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (saveColumnCount > columns.Count)
                        {
                            saveColumnCount = columns.Count;
                            rowCounter = 0;
                            isNoFindedRowIndex = true;
                        }
                        else
                        { break; }
                    }
                }
            }
        }
        #endregion
        public DataGridViewFindRowsIndexes(DataGridView dataGridView, bool multiFindText = true)
            : base(dataGridView, multiFindText, false, false)
        { }

        //---------------------------------------------------------------------
        public event EventHandler<DataGridViewFindRowIndexEventArgs> RowIndexFinded;
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public sealed class DataGridViewFindRowIndexEventArgs : EventArgs
    {
        #region private
        private readonly int _rowIndex;
        #endregion
        public DataGridViewFindRowIndexEventArgs(int rowIndex)
        {
            if (rowIndex < 0)
            { throw new ArgumentNullException(nameof(rowIndex)); }

            _rowIndex = rowIndex;
        }

        //---------------------------------------------------------------------
        public int RowIndex
        {
            get { return _rowIndex; }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}