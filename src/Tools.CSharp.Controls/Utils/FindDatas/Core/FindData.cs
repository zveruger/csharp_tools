using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Tools.CSharp.Controls;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.FindDatas
{
    //-------------------------------------------------------------------------
    public abstract class FindData : BaseDisposable
    {
        #region private
        private readonly BackgroundWorker _findWorker = new BackgroundWorker();
        private const char _FoundOfficialSymbol = StringExtensions.DefaultOfficialSymbol;
        private readonly string _foundOfficialSymbolMessage = _FoundOfficialSymbol.ToString(CultureInfo.CurrentCulture);
        //---------------------------------------------------------------------
        private PositionFoundOfficialSymbol _positionFoundOfficialSymbol = PositionFoundOfficialSymbol.Start | PositionFoundOfficialSymbol.End;
        private PositionFoundOfficialSymbol _positionFoundOfficialSymbolByClipboardPasteMessage = PositionFoundOfficialSymbol.Start | PositionFoundOfficialSymbol.End;
        //---------------------------------------------------------------------
        private TextBoxBase _textBox;
        private TextBoxExtended _textBoxExtended;
        private string _findTextBuf;
        private bool _isSetFoundOfficialSymbol = true;
        private bool _isInputCharAvailable;
        private bool _isNextStart;
        //---------------------------------------------------------------------
        private static TextBoxBase _getControl(object control)
        {
            return control as TextBoxBase;
        }
        //---------------------------------------------------------------------
        private void _findTextRestart()
        {
            _findTextStop();
            _findTextStart();
        }
        private void _findTextRestart(string text)
        {
            _findTextStop();
            _findTextStart(text);
        }
        private void _findTextStart()
        {
            _findTextStart(_findTextBuf);
        }
        private void _findTextStart(string text)
        {
            _findTextBuf = text;

            if (_findWorker.IsBusy)
            { _isNextStart = true; }
            else
            { _findWorker.RunWorkerAsync(_findTextBuf); }
        }
        private void _findTextStop()
        {
            if (_isFindWorking)
            {
                _isNextStart = false;
                _findWorker.CancelAsync();
            }
        }

        private bool _isFindWorking
        {
            get { return _findWorker.IsBusy; }
        }
        //---------------------------------------------------------------------
        private void _subscribeFindWorkerAllEvents(bool addEvents)
        {
            if (addEvents)
            {
                _findWorker.DoWork += _findWorkerOnDoWork;
                _findWorker.ProgressChanged += _findWorkerOnProgressChanged;
                _findWorker.RunWorkerCompleted += _findWorkerOnRunWorkerCompleted;
            }
            else
            {
                _findWorker.DoWork -= _findWorkerOnDoWork;
                _findWorker.ProgressChanged -= _findWorkerOnProgressChanged;
                _findWorker.RunWorkerCompleted -= _findWorkerOnRunWorkerCompleted;
            }
        }

        private void _findWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            var findWorker = (BackgroundWorker)sender;
            var findText = e.Argument as string;

            e.Result = findText;

            findWorker.ReportProgress(0, null);

            Thread.Sleep(250);
            object findedObject;
            FindTextAction(findText, out findedObject);

            findWorker.ReportProgress(1, new FindDataResultEventArgs(findText, findedObject));
        }
        private void _findWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            { OnFindingRaise(); }
            else
            { OnFindedRaise((FindDataResultEventArgs)e.UserState); }
        }
        private void _findWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var isCompleted = true;
            if (e.Error == null)
            {
                var findText = e.Result as string;
                if (_isNextStart || !string.Equals(_findTextBuf, findText))
                {
                    _isNextStart = false;
                    isCompleted = false;
                    _findTextStart();
                }
            }

            if (isCompleted)
            { OnFindedCompleted(); }
        }
        //---------------------------------------------------------------------
        private void _subscribeTextBoxExtendedAllEvents(bool addEvents)
        {
            if (_textBoxExtended != null)
            {
                if (addEvents)
                { _textBoxExtended.ClipboardPasteMessage += _textBoxExtendedClipboardPasteMessage; }
                else
                { _textBoxExtended.ClipboardPasteMessage -= _textBoxExtendedClipboardPasteMessage; }
            }
        }

        private void _textBoxExtendedClipboardPasteMessage(object sender, TextBoxClipboardMessageEventArgs e)
        {
            var control = _getControl(sender);
            var textBoxExt = control as TextBoxExtended;
            e.Cancel = (textBoxExt == null || textBoxExt.IsPaste(e.Message)) && (_setPositionFoundOfficialSymbolAndCursor(control, e.Message, _positionFoundOfficialSymbolByClipboardPasteMessage));
        }
        //---------------------------------------------------------------------
        private void _subscribeTextBoxAllEvents(bool addEvents)
        {
            if (_textBox != null)
            {
                if (addEvents)
                {
                    _textBox.TextChanged += _textBoxOnTextChanged;
                    _textBox.KeyPress += _textBoxOnKeyPress;
                    _textBox.KeyDown += _textBoxKeyDown;
                }
                else
                {
                    _textBox.TextChanged -= _textBoxOnTextChanged;
                    _textBox.KeyPress -= _textBoxOnKeyPress;
                    _textBox.KeyDown -= _textBoxKeyDown;
                }
            }
        }

        private void _textBoxKeyDown(object sender, KeyEventArgs e)
        {
            _isInputCharAvailable = false;

            var keys = e.KeyCode;
            if (e.Control)
            {
                if (keys == Keys.A)
                {
                    var control = _getControl(sender);
                    control?.SelectAll();
                }
            }
            else if (keys != Keys.ShiftKey && keys != Keys.Back)
            { _isInputCharAvailable = true; }
        }
        private void _textBoxOnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (_isInputCharAvailable)
            {
                var key = (Keys)e.KeyChar;
                switch (key)
                {
                    case Keys.Enter:
                        { _findTextStart(); }
                        break;
                    case Keys.Escape:
                        { _findTextStop(); }
                        break;
                    default:
                        {
                            var control = _getControl(sender);
                            e.Handled = _setPositionFoundOfficialSymbolAndCursor(control, e.KeyChar.ToString(CultureInfo.CurrentCulture), _positionFoundOfficialSymbol);
                        }
                        break;
                }
            }
        }
        private void _textBoxOnTextChanged(object sender, EventArgs eventArgs)
        {
            _findTextRestart(((TextBox)sender).Text);
        }
        //---------------------------------------------------------------------
        private bool _setPositionFoundOfficialSymbolAndCursor(TextBoxBase textBox, string message, PositionFoundOfficialSymbol positionFoundOfficialSymbol)
        {
            var result = false;
            if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text.Length == textBox.SelectionLength)
            {
                result = true;
                var position = _setPositionFoundOfficialSymbol(textBox, message, _isSetFoundOfficialSymbol, positionFoundOfficialSymbol, _foundOfficialSymbolMessage);
                _setPositionCursor(textBox, position);
            }
            return result;
        }
        private static int _setPositionFoundOfficialSymbol(Control control, string message, bool isPositionFoundOfficialSymbol, PositionFoundOfficialSymbol positionFoundOfficialSymbol, string foundOfficialSymbol)
        {
            var position = -1;
            if (!string.IsNullOrEmpty(message))
            {
                var text = "";
                var isMessageNoEqualsFoundOfficialSymbol = !string.Equals(message, foundOfficialSymbol, StringComparison.OrdinalIgnoreCase);
                var foundOfficialSymbolLength = foundOfficialSymbol.Length;

                if (isMessageNoEqualsFoundOfficialSymbol && isPositionFoundOfficialSymbol)
                {
                    if ((positionFoundOfficialSymbol & PositionFoundOfficialSymbol.Start) == PositionFoundOfficialSymbol.Start)
                    {
                        if ((message.Length < foundOfficialSymbolLength) || (message.IndexOf(foundOfficialSymbol, 0, foundOfficialSymbolLength, StringComparison.OrdinalIgnoreCase) == -1))
                        { text = foundOfficialSymbol; }
                    }
                }

                text += message;
                position = text.Length;

                if (isMessageNoEqualsFoundOfficialSymbol && isPositionFoundOfficialSymbol)
                {
                    if ((positionFoundOfficialSymbol & PositionFoundOfficialSymbol.End) == PositionFoundOfficialSymbol.End)
                    {
                        if ((message.Length < foundOfficialSymbolLength) || (message.IndexOf(foundOfficialSymbol, message.Length - foundOfficialSymbolLength, foundOfficialSymbolLength, StringComparison.OrdinalIgnoreCase) == -1))
                        { text += foundOfficialSymbol; }
                    }
                }

                control.Text = text;
            }

            return position;
        }
        private static void _setPositionCursor(TextBoxBase textBox, int position)
        {
            textBox.SetPositionCursor(position);
        }
        #endregion
        #region protected
        protected bool IsCancelFindText
        {
            get { return _findWorker.CancellationPending; }
        }
        protected bool IsOfficialSymbol(char symbol)
        {
            return symbol.Equals(_FoundOfficialSymbol);
        }
        //---------------------------------------------------------------------
        protected void OnFindingRaise()
        {
            Finding?.Invoke(this, EventArgs.Empty);
        }
        protected void OnFindedRaise(FindDataResultEventArgs arg)
        {
            if (arg == null)
            { throw new ArgumentNullException(nameof(arg)); }

            Finded?.Invoke(this, arg);
        }
        protected void OnFindedRaise(string findText, object result)
        {
            Finded?.Invoke(this, new FindDataResultEventArgs(findText, result));
        }
        protected void OnFindedCompleted()
        {
            FindedCompleted?.Invoke(this, EventArgs.Empty);
        }
        //---------------------------------------------------------------------
        protected abstract void FindTextAction(string findText, out object result);
        //---------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    if (_findWorker != null)
                    {
                        _subscribeFindWorkerAllEvents(false);
                        _findTextStop();

                        while (_findWorker.IsBusy)
                        { }

                        _findWorker.Dispose();
                    }

                    _subscribeTextBoxExtendedAllEvents(false);
                    _subscribeTextBoxAllEvents(false);
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        protected FindData()
        {
            _findWorker.WorkerReportsProgress = true;
            _findWorker.WorkerSupportsCancellation = true;

            _subscribeFindWorkerAllEvents(true);
        }

        //---------------------------------------------------------------------
        [Browsable(false)]
        public TextBoxBase TextBox
        {
            get { return _textBox; }
            set
            {
                if (_textBox != value)
                {
                    if (_textBox != null)
                    { _subscribeTextBoxAllEvents(false); }
                    if (_textBoxExtended != null)
                    { _subscribeTextBoxExtendedAllEvents(false); }


                    _textBox = value;
                    _textBoxExtended = _textBox as TextBoxExtended;
                    _findTextBuf = _textBox == null ? string.Empty : _textBox.Text;

                    if (_textBoxExtended != null)
                    { _subscribeTextBoxExtendedAllEvents(true); }
                    if (_textBox != null)
                    { _subscribeTextBoxAllEvents(true); }
                }
            }
        }
        //--------------------------------------------------------------------
        [Browsable(false)]
        public string FindText
        {
            get { return _findTextBuf; }
        }
        //--------------------------------------------------------------------
        [DefaultValue(true)]
        public bool IsSetFoundOfficialSymbol
        {
            get { return _isSetFoundOfficialSymbol; }
            set { _isSetFoundOfficialSymbol = value; }
        }
        public bool IsFind
        {
            get { return _findWorker.IsBusy; }
        }
        //--------------------------------------------------------------------
        [Browsable(false), DefaultValue(typeof(PositionFoundOfficialSymbol), "End")]
        public PositionFoundOfficialSymbol PositionFoundOfficialSymbol
        {
            get { return _positionFoundOfficialSymbol; }
            set { _positionFoundOfficialSymbol = value; }
        }
        //--------------------------------------------------------------------
        [Browsable(false), DefaultValue(typeof(PositionFoundOfficialSymbol), "End")]
        public PositionFoundOfficialSymbol PositionFoundOfficialSymbolByClipboardPasteMessage
        {
            get { return _positionFoundOfficialSymbolByClipboardPasteMessage; }
            set { _positionFoundOfficialSymbolByClipboardPasteMessage = value; }
        }
        //--------------------------------------------------------------------
        public void FindTextStart()
        {
            _findTextStart();
        }
        public void FindTextStop()
        {
            _findTextStop();
        }
        public void FindTextRestart()
        {
            _findTextRestart();
        }
        //--------------------------------------------------------------------
        public void PasteText(string text)
        {
            var control = _getControl(_textBox);

            control?.SelectAll();

            _setPositionFoundOfficialSymbolAndCursor(control, text, _positionFoundOfficialSymbolByClipboardPasteMessage);
        }
        //--------------------------------------------------------------------
        public event EventHandler Finding;
        public event EventHandler<FindDataResultEventArgs> Finded;
        public event EventHandler FindedCompleted;
        //--------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public sealed class FindDataResultEventArgs : EventArgs
    {
        #region private
        private readonly string _findText;
        private readonly object _result;
        #endregion
        public FindDataResultEventArgs(string findText, object result)
        {
            _findText = findText;
            _result = result;
        }

        //-------------------------------------------------------------------------
        public string FindText
        {
            get { return _findText; }
        }
        //-------------------------------------------------------------------------
        public object Result
        {
            get { return _result; }
        }
        //-------------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}
