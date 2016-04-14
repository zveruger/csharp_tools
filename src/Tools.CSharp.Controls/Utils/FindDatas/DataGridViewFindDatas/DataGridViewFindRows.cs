using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Forms;
using Tools.CSharp.Controls.Extensions;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.FindDatas
{
    //-------------------------------------------------------------------------
    public class DataGridViewFindRows : FindData
    {
        #region private
        private readonly DataGridView _dataGridView;
        private readonly bool _multiFindText;
        private readonly bool _isClearSelection;
        private readonly bool _isSelectRowFinded;
        #endregion
        #region protected
        protected static ReadOnlyCollection<int> EmptyExcludedColumnIndexCollection = new List<int>().AsReadOnly(); 
        //---------------------------------------------------------------------
        protected virtual ReadOnlyCollection<int> CreateExcludedColumnIndexCollection()
        {
            return EmptyExcludedColumnIndexCollection;
        }
        //---------------------------------------------------------------------
        protected virtual void FindTextAction(DataGridView dataGridView, string findText, out ReadOnlyCollection<int> rowIndexesResult)
        {
            if (dataGridView == null)
            { throw new ArgumentNullException(nameof(dataGridView)); }

            var isFindTextNullOrWhiteSpace = string.IsNullOrWhiteSpace(findText);
            var findedRowIndexes = new List<int>();

            if (_isClearSelection || isFindTextNullOrWhiteSpace)
            { dataGridView.Invoke(dataGridView.ClearSelection); }

            if (!isFindTextNullOrWhiteSpace)
            {
                var isNoClearSelection = !_isClearSelection;;
                var rows = dataGridView.Rows;
                var columns = dataGridView.Columns;
                var saveRowCount = rows.Count;
                var saveColumnCount = columns.Count;
                var excludedColumnIndexCollection = CreateExcludedColumnIndexCollection();
                excludedColumnIndexCollection = Interlocked.CompareExchange(ref excludedColumnIndexCollection, EmptyExcludedColumnIndexCollection, null);

                for (var rowCounter = 0; rowCounter < rows.Count; rowCounter++)
                {
                    if (IsCancelFindText)
                    { break; }

                    var row = rows[rowCounter];

                    if ((isNoClearSelection) || (saveRowCount <= rowCounter))
                    { dataGridView.Invoke(delegate { row.Selected = false; }); }

                    if (_multiFindText || findedRowIndexes.Count == 0)
                    {
                        for (var columnCounter = 0; columnCounter < columns.Count; columnCounter++)
                        {
                            if (IsCancelFindText)
                            { break; }

                            if (saveColumnCount > columns.Count)
                            {
                                saveColumnCount = columns.Count;
                                rowCounter = 0;
                                findedRowIndexes.Clear();
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
                                        if (_isSelectRowFinded)
                                        {
                                            if (findedRowIndexes.Count == 0)
                                            { findedRowIndexes.Add(row.Index); }

                                            dataGridView.Invoke(delegate { row.Selected = true; });
                                        }
                                        else
                                        { findedRowIndexes.Add(row.Index); }
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
                            findedRowIndexes.Clear();
                        }
                    }
                }

                if (IsCancelFindText || _isSelectRowFinded)
                { findedRowIndexes.Clear(); }
            }

            rowIndexesResult = findedRowIndexes.AsReadOnly();
        }
        protected override void FindTextAction(string findText, out object result)
        {
            ReadOnlyCollection<int> rowIndexesResult;
            FindTextAction(_dataGridView, findText, out rowIndexesResult);
            result = rowIndexesResult;
        }
        #endregion
        public DataGridViewFindRows(DataGridView dataGridView, bool multiFindText = true, bool isClearSelection = true, bool isSelectRowFinded = true)
        {
            if (dataGridView == null)
            { throw new ArgumentNullException(nameof(dataGridView)); }

            _dataGridView = dataGridView;
            _multiFindText = multiFindText;
            _isClearSelection = isClearSelection;
            _isSelectRowFinded = isSelectRowFinded;
        }

        //---------------------------------------------------------------------
        public bool IsClearSelection
        {
            get { return _isClearSelection; }
        }
        public bool IsMultiFindText
        {
            get { return _multiFindText; }
        }
        public bool IsSelectRowFinded
        {
            get { return _isSelectRowFinded; }
        }
        //---------------------------------------------------------------------
    }


    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
}
