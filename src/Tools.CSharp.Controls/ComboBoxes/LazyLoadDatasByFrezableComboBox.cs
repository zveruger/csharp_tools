using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Tools.CSharp.Collections;

namespace Tools.CSharp.Controls
{
    //-------------------------------------------------------------------------
    public class LazyLoadDatasByFrezableComboBox<T> : BaseDisposable
        where T : IComparable<T>, IEquatable<T>
    {
        #region private
        private readonly FrezableTextComboBox _comboBox;
        private readonly string _emptyText;
        private readonly string _waitText;
        private readonly string _format = "";
        private readonly bool _isFormat;
        //-------------------------------------------------------------------
        private T _currentValue;
        private AddMode _addMode = AddMode.Collection;
        private T[] _availableArray;
        private ReadOnlyCollection<T> _availableCollection;
        private BaseFastCollection<T> _availableFastCollection;
        private bool _oldComboFoxFrizableTextAvailable;
        //--------------------------------------------------------------------
        private enum AddMode
        {
            Collection,
            Array,
            FastCollection
        }
        //---------------------------------------------------------------------
        private IEnumerable<object> _getData()
        {
            Func<T, object> createDataByFormat = _createDataByFormat;

            switch (_addMode)
            {
                case AddMode.Array:
                    { return _availableArray.Select(createDataByFormat); }
                case AddMode.Collection:
                    { return _availableCollection.Select(createDataByFormat); }
                case AddMode.FastCollection:
                    { return _availableFastCollection.Select(createDataByFormat); }
                default:
                    { throw new InvalidOperationException(); }
            }
        }
        //--------------------------------------------------------------------
        private void _subscribeComboBoxGotFocusEvents(bool addEvents)
        {
            if (_comboBox != null)
            {
                if (addEvents)
                { _comboBox.GotFocus += _comboBoxGotFocus; }
                else
                { _comboBox.GotFocus -= _comboBoxGotFocus; }
            }
        }
        private void _comboBoxGotFocus(object sender, EventArgs e)
        {
            _comboBoxLoadDataAction();
        }
        //---------------------------------------------------------------------
        private void _comboBoxLoadDataAction()
        {
            if (_comboBox.Enabled)
            {
                OnLoadDatas(LazyLoadDatasByFrezableComboBoxState.Loading);
                _subscribeComboBoxGotFocusEvents(false);
                _comboBoxInitLoadingDatas();
                _comboBoxLoadDatas();
                _comboBoxLoadedDatas(_currentValue);
                OnLoadDatas(LazyLoadDatasByFrezableComboBoxState.Loaded);
            }
        }

        private void _comboBoxInitLoadingDatas()
        {
            _oldComboFoxFrizableTextAvailable = _comboBox.IsFrizableText;
            _comboBox.IsFrizableText = true;
            _comboBox.Text = _waitText;
            _comboBox.IsFrizableText = _oldComboFoxFrizableTextAvailable;
            _comboBox.Refresh();
        }
        private void _comboBoxLoadDatas()
        {
            _comboBox.BeginUpdate();

            foreach (var data in _getData())
            { _comboBox.Items.Add(data); }

            _comboBox.EndUpdate();
        }
        private void _comboBoxLoadedDatas(T currentValue)
        {
            var currentValueToString = _createDataByFormatText(currentValue);

            _comboBox.IsFrizableText = true;
            _comboBox.Text = currentValueToString;
            _comboBox.IsFrizableText = _oldComboFoxFrizableTextAvailable;
            if (_comboBox.DroppedDown)
            {
                _comboBox.DroppedDown = false;
                _comboBox.DroppedDown = true;
            }

            try
            {
                var insertIndex = _comboBox.FindStringExact(currentValueToString);
                if (insertIndex == -1)
                {
                    insertIndex = _BinarySearchPosition(_currentValue);

                    if (insertIndex != -1)
                    { _comboBox.Items.Insert(insertIndex, currentValueToString); }
                    else
                    { insertIndex = _comboBox.Items.Add(currentValueToString); }
                }

                _comboBoxSelectedIndex(insertIndex);
            }
            catch (InvalidCastException) { }
            catch (InvalidOperationException) { }
        }

        private void _comboBoxSelectedIndex(int index)
        {
            CheckThrowObjectDisposedException();

            if (_comboBox.Items.Count != 0)
            { _comboBox.SelectedIndex = index; }
        }
        private void _comboBoxClearDatas()
        {
            CheckThrowObjectDisposedException();

            _subscribeComboBoxGotFocusEvents(false);

            if (_comboBox.Items.Count != 0)
            { _comboBox.Items.Clear(); }

            _comboBoxTextEmptyValue();
        }
        private void _comboBoxTextEmptyValue()
        {
            CheckThrowObjectDisposedException();

            _comboBox.Text = _emptyText;
        }
        //-------------------------------------------------------------------
        private object _createDataByFormat(T source)
        {
            return _isFormat ? (object)string.Format(_format, source) : source;
        }
        private string _createDataByFormatText(T source)
        {
            var dataFormat = _createDataByFormat(source);
            return dataFormat?.ToString() ?? string.Empty;
        }
        //-------------------------------------------------------------------
        private int _BinarySearchPosition(T currentValue)
        {
            var searchIndex = -1;
            switch (_addMode)
            {
                case AddMode.Array:
                    { searchIndex = _BinarySearchPosition(_availableArray, currentValue); }
                    break;
                case AddMode.Collection:
                    { searchIndex = _BinarySearchPosition(_availableCollection, currentValue); }
                    break;
                case AddMode.FastCollection:
                    { searchIndex = _BinarySearchPosition(_availableFastCollection, currentValue); }
                    break;
            }
            return searchIndex;
        }

        private static int _BinarySearchPosition(IList<T> list, T currentValue)
        {
            var isFindPosition = false;
            var searchIndex = -1;

            if (list != null)
            {
                var arrayCount = list.Count - 1;
                if (arrayCount >= 0)
                {
                    var leftValue = 0;
                    var rightValue = arrayCount;

                    do
                    {
                        searchIndex = leftValue + (rightValue - leftValue) / 2;

                        if (currentValue.CompareTo(list[searchIndex]) < 0)
                        { rightValue = searchIndex - 1; }
                        else if (currentValue.CompareTo(list[searchIndex]) > 0)
                        { leftValue = searchIndex + 1; }
                        else
                        { isFindPosition = true; }

                        if (leftValue > rightValue)
                        {
                            searchIndex = leftValue;
                            isFindPosition = true;
                        }
                    }
                    while (!isFindPosition);
                }
            }

            return searchIndex;
        }
        private static int _BinarySearchPosition(BaseFastCollection<T> collection, T currentValue)
        {
            var isFindPosition = false;
            var searchIndex = -1;

            if (collection != null)
            {
                var collectionCount = collection.Count > int.MaxValue ? int.MaxValue : (int)collection.Count;
                var arrayCount = collectionCount - 1;
                if (arrayCount >= 0)
                {
                    var leftValue = 0;
                    var rightValue = arrayCount;

                    do
                    {
                        searchIndex = leftValue + (rightValue - leftValue) / 2;

                        if (currentValue.CompareTo(collection[(ulong)searchIndex]) < 0)
                        { rightValue = searchIndex - 1; }
                        else if (currentValue.CompareTo(collection[(ulong)searchIndex]) > 0)
                        { leftValue = searchIndex + 1; }
                        else
                        { isFindPosition = true; }

                        if (leftValue > rightValue)
                        {
                            searchIndex = leftValue;
                            isFindPosition = true;
                        }
                    }
                    while (!isFindPosition);
                }
            }

            return searchIndex;
        }
        #endregion
        #region protected
        protected virtual void OnLoadDatas(LazyLoadDatasByFrezableComboBoxState state)
        {
            LoadDatas?.Invoke(this, new LazyLoadDatasByFrezableComboBoxEventArgs(state));
        }
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    if (_comboBox != null)
                    { _subscribeComboBoxGotFocusEvents(false); }
                }
            }
            finally
            { base.Dispose(disposing); }
           
        }
        #endregion
        public LazyLoadDatasByFrezableComboBox(FrezableTextComboBox comboBox, string emptyText, string waitText)
            : this(comboBox, emptyText, waitText, string.Empty)
        {

        }
        public LazyLoadDatasByFrezableComboBox(FrezableTextComboBox comboBox, string emptyText, string waitText, string format)
        {
            if (comboBox == null)
            { throw new ArgumentNullException(nameof(comboBox)); }

            _emptyText = emptyText;
            _waitText = waitText;

            _comboBox = comboBox;
            _comboBox.Sorted = false;

            _format = format;
            _isFormat = !string.IsNullOrWhiteSpace(_format);

            _comboBoxClearDatas();
        }
        ~LazyLoadDatasByFrezableComboBox()
        {
            Dispose(false);
        }

        //---------------------------------------------------------------------
        public int Count
        {
            get
            {
                if (_availableCollection != null && _addMode == AddMode.Collection)
                { return _availableCollection.Count; }

                if (_availableArray != null)
                { return _availableArray.Length; }

                return -1;
            }
        }
        public T this[int index]
        {
            get
            {
                if (_availableCollection != null && _addMode == AddMode.Collection)
                { return _availableCollection[index]; }

                if (_availableArray != null)
                { return _availableArray[index]; }

                return default(T);
            }
        }
        //---------------------------------------------------------------------
        public void AddCollection(ReadOnlyCollection<T> collection, T currentValue)
        {
            if (collection == null)
            { throw new ArgumentNullException(nameof(collection)); }

            CheckThrowObjectDisposedException();

            _addMode = AddMode.Collection;
            _availableCollection = collection;
            _currentValue = currentValue;

            _comboBoxClearDatas();

            if (Equals(currentValue, default(T)))
            {
                if (_availableCollection.Count != 0)
                {
                    _currentValue = _availableCollection[0];
                    _comboBox.Text = _createDataByFormatText(_currentValue);
                }
                else
                { _comboBoxTextEmptyValue(); }
            }
            else
            { _comboBox.Text = _createDataByFormatText(currentValue); }

            if (_availableCollection.Count != 0)
            { _subscribeComboBoxGotFocusEvents(true); }
        }
        public void AddCollection(BaseFastCollection<T> collection, T currentValue)
        {
            if (collection == null)
            { throw new ArgumentNullException(nameof(collection)); }

            CheckThrowObjectDisposedException();

            _addMode = AddMode.FastCollection;
            _availableFastCollection = collection;
            _currentValue = currentValue;

            _comboBoxClearDatas();

            if (Equals(currentValue, default(T)))
            {
                if (_availableFastCollection.Count != 0)
                {
                    _currentValue = _availableFastCollection[0];
                    _comboBox.Text = _createDataByFormatText(_currentValue);
                }
                else
                { _comboBoxTextEmptyValue(); }
            }
            else
            { _comboBox.Text = _createDataByFormatText(currentValue); }

            if (_availableFastCollection.Count != 0)
            { _subscribeComboBoxGotFocusEvents(true); }
        }
        public void AddArray(T[] array, T currentValue)
        {
            if (array == null)
            { throw new ArgumentNullException(nameof(array)); }

            CheckThrowObjectDisposedException();

            _addMode = AddMode.Array;
            _availableArray = array;
            _currentValue = currentValue;

            _comboBoxClearDatas();

            if (Equals(currentValue, default(T)))
            {
                if (_availableArray.Length != 0)
                {
                    _currentValue = _availableArray[0];
                    _comboBox.Text = _createDataByFormatText(_currentValue);
                }
                else
                { _comboBox.Text = _emptyText; }
            }
            else
            { _comboBox.Text = _createDataByFormatText(currentValue); }

            if (_availableArray.Length != 0)
            { _subscribeComboBoxGotFocusEvents(true); }
        }
        //---------------------------------------------------------------------
        public void Clear()
        {
            _comboBoxClearDatas();
        }
        public T GetCurrentValue
        {
            get { return _currentValue; }
        }
        //---------------------------------------------------------------------
        public void SetValueByText(T value)
        {
            CheckThrowObjectDisposedException();

            _currentValue = value;
            _comboBox.Text = _createDataByFormatText(value);
        }
        //---------------------------------------------------------------------
        public event EventHandler<LazyLoadDatasByFrezableComboBoxEventArgs> LoadDatas;
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public enum LazyLoadDatasByFrezableComboBoxState
    {
        Loading,
        Loaded
    }
    //-------------------------------------------------------------------------
    public sealed class LazyLoadDatasByFrezableComboBoxEventArgs : EventArgs
    {
        #region private
        private readonly LazyLoadDatasByFrezableComboBoxState _state;
        #endregion
        public LazyLoadDatasByFrezableComboBoxEventArgs(LazyLoadDatasByFrezableComboBoxState state)
        {
            _state = state;
        }

        //---------------------------------------------------------------------
        public LazyLoadDatasByFrezableComboBoxState State
        {
            get { return _state; }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}
