using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using Tools.CSharp.Controls.Extensions;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Controls
{
    //-------------------------------------------------------------------------
    [Designer(typeof(TextBoxExtendedDesigner))]
    public sealed class TextBoxExtended : TextBox
    {
        #region private
        private const int _EmSetcuebanner = 0x1501;
        private const int _WmPaste = 0x0302;
        private const int _WmCopy = 0x0301;
        private static readonly Color _DefaultWaterMarkActiveColor = Color.Gray;
        private static readonly Color _DefaultWaterMarkColor = Color.Gray;
        private static readonly Color _DefaultWaterMarkBackColor = Color.White;
        private static readonly char _EmptyPasswordChar = Char.MinValue;
        //---------------------------------------------------------------------
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        //---------------------------------------------------------------------
        private TextBoxTypeInput _typeInput = TextBoxTypeInput.Normal;
        private static readonly Dictionary<TextBoxTypeInput, string> _TypeInputTemplates = new Dictionary<TextBoxTypeInput, string>
        {
            { TextBoxTypeInput.PhoneNumber, @"^[0-9\*\#]+$" },
            { TextBoxTypeInput.Digits, @"^[0-9]+$" },
            { TextBoxTypeInput.Hex, @"^[0-9a-fA-F]+$"}, 
            { TextBoxTypeInput.EnglishLanguage, @"^[А-zA-Z_.]+$"},
            { TextBoxTypeInput.DigitsAndEnglishLanguage, @"^[0-9a-zA-Z_.]+$"},
            { TextBoxTypeInput.DigitsAndEnglishAndRussianLanguage, @"^[0-9a-zA-Z_.а-яА-Я]+$"}
        };
        private static readonly Dictionary<TextBoxTypeInput, string> _TypeInputSequence = new Dictionary<TextBoxTypeInput, string>
        {
            { TextBoxTypeInput.PhoneNumber, "0123456789*#" },
            { TextBoxTypeInput.Digits, "0123456789" },
            { TextBoxTypeInput.Hex, "0123456789abcdefABCDEF" },
            { TextBoxTypeInput.EnglishLanguage, "abcdefghijklmnopqrstuvwxyzABCDEDFGHIJKLMNOPQRSTUVWXYZ_." },
            { TextBoxTypeInput.DigitsAndEnglishLanguage, "0123456789abcdefghijklmnopqrstuvwxyzABCDEDFGHIJKLMNOPQRSTUVWXYZ_." },
            { TextBoxTypeInput.DigitsAndEnglishAndRussianLanguage, "0123456789abcdefghijklmnopqrstuvwxyzABCDEDFGHIJKLMNOPQRSTUVWXYZ_.абвгдежзийклмнопрстуфхцчшщъыьэюяАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ" }                 
        };
        //---------------------------------------------------------------------
        private static readonly bool _IsCustomeWaterMark;

        private Size _fixed3DOffset = new Size(3, 3);
        private Size _fixedSingleOffset = new Size(2, 2);
        private Color _mainColor = Color.White;
        private Color _readOnlyUserColor = Color.WhiteSmoke;
        private bool _readOnlyUser;
        private bool _nonInputEntered;
        private bool _isCustomeWaterMark = _IsCustomeWaterMark;
        private bool _isNoEmptyWaterMarkText;
        private bool _isOldCustomeWaterMark;

        private string _waterMarkText;
        private Color _waterMarkColor = _DefaultWaterMarkColor;
        private Color _waterMarkActiveColor = _DefaultWaterMarkActiveColor;
        private Panel _waterMarkContainer;

        private char _passwordChar;
        private bool _copyAvailable = true;
        private bool _pasteAvailable = true;
        //---------------------------------------------------------------------
        private void _readOnlyUserUpdate(bool value)
        {
            SuspendLayout();
            ReadOnly = value;
            BackColor = (value) ? _readOnlyUserColor : _mainColor;
            ResumeLayout();
        }
        //---------------------------------------------------------------------
        private void _onClipboardPasteMessage(string message, out bool cancel)
        {
            cancel = false;

            var handler = Interlocked.CompareExchange(ref ClipboardPasteMessage, null, null);
            if (handler != null)
            {
                var eventArgs = new TextBoxClipboardMessageEventArgs(message);
                handler(this, eventArgs);
                cancel = eventArgs.Cancel;
            }
        }
        //---------------------------------------------------------------------
        private void _clear()
        {
            _nonInputEntered = false;
            Clear();
        }
        //---------------------------------------------------------------------
        private void _typeInputSymbolKeyDown(PreviewKeyDownEventArgs e, bool copyAvailable, bool pasteAvailable)
        {
            _nonInputEntered = false;
            var isKeyBackCheck = !_nonInputEntered;

            if (e.Control)
            {
                _nonInputEntered = true;
                switch (e.KeyCode)
                {
                    case Keys.C:
                    case Keys.X:
                    case Keys.Insert:
                        {
                            if (copyAvailable)
                            { _nonInputEntered = false; }
                        }
                        break;
                    case Keys.V:
                        {
                            if (pasteAvailable)
                            { _nonInputEntered = false; }
                        }
                        break;
                    default:
                        { _nonInputEntered = false; }
                        break;
                }
                isKeyBackCheck = _nonInputEntered;
            }
            else if (e.Shift)
            {
                _nonInputEntered = true;
                if (e.KeyCode == Keys.Insert)
                {
                    if (pasteAvailable)
                    { _nonInputEntered = false; }
                }
                isKeyBackCheck = _nonInputEntered;
            }

            if (isKeyBackCheck)
            {
                switch (e.KeyCode)
                {
                    case Keys.Back:
                    case Keys.Delete:
                    case Keys.Left:
                    case Keys.Right:
                        { _nonInputEntered = false; }
                        break;
                    default:
                        { _nonInputEntered = true; }
                        break;
                }
            }
        }
        //---------------------------------------------------------------------
        private static void _noEnglishAndSetEnglishInputLanguage()
        {
            if (!_isEnglishInputLanguage())
            { _setEnglishInputLanguage(); }
        }
        private static bool _isEnglishInputLanguage()
        {
            return CultureInfo.CurrentCulture.KeyboardLayoutId == 1033;
        }
        private static void _setEnglishInputLanguage()
        {
            try
            { InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(CultureInfo.GetCultureInfo("en-US")); }
            catch (Exception)
            { }
        }
        //---------------------------------------------------------------------
        private void _isNoEmptyWaterMarkTextUpdate()
        {
            _isNoEmptyWaterMarkText = !string.IsNullOrWhiteSpace(_waterMarkText);
        }
        //---------------------------------------------------------------------
        private void _updateWaterMark()
        {
            _removeWaterMark();

            if (_isCustomeWaterMark)
            {
                _drawWaterMark();
                Invalidate();
            }
            else
            { _standartWaterMark(); }
        }

        private void _thisHasFocus()
        {
            if (TextLength == 0)
            {
                _removeWaterMark();
                _drawWaterMark();
            }
        }
        private void _thisWasLeaved()
        {
            if (TextLength > 0)
            { _removeWaterMark(); }
            else
            { Invalidate(); }
        }
        private void _thisTextChanged()
        {
            if (TextLength > 0)
            { _removeWaterMark(); }
            else
            { _drawWaterMark(); }
        }

        private void _drawWaterMark()
        {
            if (_waterMarkContainer == null && TextLength == 0)
            {
                _waterMarkContainer = new Panel { BackColor = _DefaultWaterMarkBackColor };
                BackColor = _waterMarkContainer.BackColor;

                _subscribeWatermarkControlAllEvents(true);
                _waterMarkContainer.Invalidate();

                Controls.Add(_waterMarkContainer);
            }
        }
        private void _removeWaterMark()
        {
            if (_waterMarkContainer != null)
            {
                _subscribeWatermarkControlAllEvents(false);

                Controls.Remove(_waterMarkContainer);

                _waterMarkContainer.Dispose();
                _waterMarkContainer = null;
            }
        }

        private void _subscribeWatermarkControlAllEvents(bool addEvents)
        {
            if (_waterMarkContainer != null)
            {
                if (addEvents)
                {
                    _waterMarkContainer.Paint += _waterMarkContainerPaint;
                    _waterMarkContainer.Click += _waterMarkContainerClick;
                }
                else
                {
                    _waterMarkContainer.Paint -= _waterMarkContainerPaint;
                    _waterMarkContainer.Click -= _waterMarkContainerClick;
                }
            }
        }
        private void _waterMarkContainerPaint(object sender, PaintEventArgs e)
        {
            if (_isNoEmptyWaterMarkText)
            {
                _waterMarkContainer.SuspendLayout();
                _waterMarkContainer.Location = new Point(2, 0);
                _waterMarkContainer.Height = Height;
                _waterMarkContainer.Width = Width;
                _waterMarkContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                _waterMarkContainer.ResumeLayout();

                using (var waterMarkBrush = ContainsFocus ? new SolidBrush(_waterMarkActiveColor) : new SolidBrush(_waterMarkColor))
                { e.Graphics.DrawString(_waterMarkText, Font, waterMarkBrush, new PointF(-3f, 1f)); }
            }
            else
            { _waterMarkContainer.Height = _waterMarkContainer.Width = 0; }
        }
        private void _waterMarkContainerClick(object sender, EventArgs e)
        {
            Focus();
        }
        //---------------------------------------------------------------------
        private void _standartWaterMark()
        {
            SendMessage(Handle, _EmSetcuebanner, 1, _waterMarkText);
        }
        //---------------------------------------------------------------------
        private bool _isPaste()
        {
            var result = false;

            if (_pasteAvailable)
            {
                string clipboardMessage;
                if (ClipboardExtension.GetTextByUnicode(out clipboardMessage))
                {
                    bool cancel;
                    _onClipboardPasteMessage(clipboardMessage, out cancel);
                    if (!cancel)
                    { result = _isPaste(clipboardMessage); }
                }
            }

            return result;
        }
        private bool _isPaste(string message)
        {
            return _IsPaste(message, _typeInput);
        }
        private static bool _IsPaste(string message, TextBoxTypeInput typeInput)
        {
            var result = false;

            if ((typeInput == TextBoxTypeInput.Normal) || (string.IsNullOrEmpty(message)))
            { result = true; }
            else
            {
                string typeInputTemplate;
                if (_TypeInputTemplates.TryGetValue(typeInput, out typeInputTemplate))
                { result = Regex.IsMatch(message, typeInputTemplate, RegexOptions.IgnoreCase); }
            }

            return result;
        }

        private bool _copy()
        {
            return _copyAvailable;
        }
        //---------------------------------------------------------------------
        private struct ParametersWord
        {
            #region private
            private readonly int _startPosition;
            private readonly int _position;
            private readonly int _length;
            private readonly bool _isAvailable;
            //-----------------------------------------------------------------
            private void _checkAvailable()
            {
                if (!_isAvailable)
                { throw new InvalidOperationException(); }
            }
            #endregion
            public ParametersWord(int position, int length, int startPosition)
            {
                _position = position;
                _length = length;
                _startPosition = startPosition;
                _isAvailable = _length > 0;
            }

            //-----------------------------------------------------------------
            public int Position
            {
                get
                {
                    _checkAvailable();
                    return _position;
                }
            }
            public int Length
            {
                get
                {
                    _checkAvailable();
                    return _length;
                }
            }
            //-----------------------------------------------------------------
            public int StartPosition
            {
                get
                {
                    _checkAvailable();
                    return _startPosition;
                }
            }
            //-----------------------------------------------------------------
            public bool IsAvailable
            {
                get { return _isAvailable; }
            }
            //-----------------------------------------------------------------

        }
        private ParametersWord _getParametersWord(int startPosition)
        {
            var position = 0;
            var length = 0;
            var strPosition = 0;
            var textLength = TextLength;

            if ((startPosition > 0) && (startPosition <= textLength))
            {
                var newStartPosition = (startPosition < textLength ? startPosition : textLength) - 1;
                for (var i = newStartPosition; i >= 0; i--)
                {
                    if ((Char.IsLetterOrDigit(Text, i)) || (i == 0))
                    { newStartPosition = i; break; }
                }

                for (var i = newStartPosition; i >= 0; i--)
                {
                    if (!Char.IsLetterOrDigit(Text, i))
                    { position = i == 0 ? 0 : i + 1; break; }
                }

                length = startPosition - position;
                strPosition = startPosition - length;
            }

            return new ParametersWord(position, length, strPosition);
        }
        #endregion
        #region protected
        protected override void OnEnabledChanged(EventArgs e)
        {
            if (Enabled)
            { IsCustomeWaterMark = _isOldCustomeWaterMark; }
            else
            {
                _isOldCustomeWaterMark = IsCustomeWaterMark;
                IsCustomeWaterMark = false;
            }

            base.OnEnabledChanged(e);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == _WmPaste)
            {
                if (!_isPaste())
                { return; }
            }
            else if (m.Msg == _WmCopy)
            {
                if (!_copy())
                { return; }
            }
            base.WndProc(ref m);
        }
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (_typeInput != TextBoxTypeInput.Normal)
            {
                _typeInputSymbolKeyDown(e, _copyAvailable, _pasteAvailable);
                if (_typeInput == TextBoxTypeInput.Hex || _typeInput == TextBoxTypeInput.EnglishLanguage || _typeInput == TextBoxTypeInput.DigitsAndEnglishLanguage)
                { _noEnglishAndSetEnglishInputLanguage(); }
            }

            base.OnPreviewKeyDown(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Back)
            {
                _nonInputEntered = true;

                var parametersWord = _getParametersWord(SelectionStart);
                if (parametersWord.IsAvailable)
                {
                    Text = Text.Remove(parametersWord.Position, parametersWord.Length);
                    this.SetPositionCursor(parametersWord.StartPosition);
                }
            }
            else
            {
                if (e.KeyCode != Keys.Home && e.KeyCode != Keys.End)
                { e.Handled = (_nonInputEntered && _typeInput != TextBoxTypeInput.Normal); }

                if (!e.Handled)
                { base.OnKeyDown(e); }
            }
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (_nonInputEntered)
            {
                _nonInputEntered = false;
                var handled = true;

                string sequence;
                if (_TypeInputSequence.TryGetValue(_typeInput, out sequence))
                { handled = sequence.IndexOf(e.KeyChar) == -1; }

                e.Handled = handled;
            }

            if (!e.Handled)
            { base.OnKeyPress(e); }
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_isCustomeWaterMark)
            { _drawWaterMark(); }
        }
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);

            if (_waterMarkContainer != null)
            { _waterMarkContainer.Invalidate(); }
        }
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            if (_isCustomeWaterMark)
            { _thisHasFocus(); }
        }
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            if (_isCustomeWaterMark)
            { _thisWasLeaved(); }
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (_isCustomeWaterMark)
            { _thisTextChanged(); }
        }
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    if (_waterMarkContainer != null)
                    {
                        _subscribeWatermarkControlAllEvents(false);
                        _waterMarkContainer.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion
        static TextBoxExtended()
        {
            _IsCustomeWaterMark = Environment.OSVersion.Version.Major < 6;
        }
        public TextBoxExtended()
        {
            Font = new Font("Arial", 10F);

            _passwordChar = PasswordChar;

            _isNoEmptyWaterMarkTextUpdate();
            _drawWaterMark();
        }

        //---------------------------------------------------------------------
        public static bool IsAvailableMessage(TextBoxTypeInput typeInput, string message)
        {
            var isAvailable = _IsPaste(message, typeInput);
            return isAvailable;
        }
        //---------------------------------------------------------------------
        public new char PasswordChar
        {
            get { return base.PasswordChar; }
            set
            {
                base.PasswordChar = value;
                _passwordChar = base.PasswordChar;
            }
        }
        public new bool UseSystemPasswordChar
        {
            get { return base.UseSystemPasswordChar; }
            set
            {
                base.UseSystemPasswordChar = value;
                PasswordChar = base.UseSystemPasswordChar ? _passwordChar : _EmptyPasswordChar;
            }
        }
        //---------------------------------------------------------------------
        [DefaultValue(typeof(Font), "Arial, 10pt")]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }
        //---------------------------------------------------------------------
        [Category("Behavior"), Description("Пользовательский режим ReadOnly"), DefaultValue(false)]
        public bool ReadOnlyUser
        {
            get { return _readOnlyUser; }
            set
            {
                _readOnlyUser = value;
                _readOnlyUserUpdate(_readOnlyUser);
                Invalidate();
            }
        }
        //---------------------------------------------------------------------
        [Category("Appearance"), Description("Основной цвет поля ввода"), DefaultValue(typeof(Color), "White")]
        public Color MainColor
        {
            get { return _mainColor; }
            set
            {
                BackColor = _mainColor = value;
                _readOnlyUserUpdate(_readOnlyUser);
                Invalidate();
            }
        }
        [Category("Appearance"), Description("Цвет при пользовательском режиме ReadOnly"), DefaultValue(typeof(Color), "WhiteSmoke")]
        public Color ReadOnlyUserColor
        {
            get { return _readOnlyUserColor; }
            set
            {
                _readOnlyUserColor = value;
                _readOnlyUserUpdate(_readOnlyUser);
                Invalidate();
            }
        }
        [Category("Appearance"), Description("Цвет текста"), DefaultValue(typeof(Color), "Black")]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }
        //---------------------------------------------------------------------
        [Browsable(false)]
        public int Baseline
        {
            get
            {
                NativeMethods.Textmetric textMetric = NativeMethods.GetTextMetrics(Handle, Font);

                int offset = textMetric.tmAscent + 1;

                switch (BorderStyle)
                {
                    case BorderStyle.Fixed3D:
                        offset += _fixed3DOffset.Height;
                        break;
                    case BorderStyle.FixedSingle:
                        offset += _fixedSingleOffset.Height;
                        break;
                }

                return offset;
            }
        }
        //---------------------------------------------------------------------
        [Category("Watermark attribtues")]
        [Description("Sets the text of the watermark")]
        [Browsable(true), DefaultValue(null)]
        public string WaterMark
        {
            get { return _waterMarkText; }
            set
            {
                if (!string.Equals(_waterMarkText, value, StringComparison.OrdinalIgnoreCase))
                {
                    _waterMarkText = value;

                    _isNoEmptyWaterMarkTextUpdate();
                    _updateWaterMark();
                }
            }
        }

        [Category("Watermark attribtues")]
        [Browsable(true), DefaultValue(typeof(bool), "False")]
        public bool IsCustomeWaterMark
        {
            get { return _isCustomeWaterMark; }
            set
            {
                if (_isCustomeWaterMark != value)
                {
                    _isCustomeWaterMark = value;
                    _updateWaterMark();
                }
            }
        }
        //---------------------------------------------------------------------
        [Category("Watermark attribtues")]
        [Description("When the control gaines focus, this color will be used as the watermark's forecolor")]
        [Browsable(true), DefaultValue(typeof(Color), "Gray")]
        private Color WaterMarkActiveForeColor
        {
            get { return _waterMarkActiveColor; }
            set
            {
                if (_waterMarkActiveColor != value)
                {
                    _waterMarkActiveColor = value;
                    Invalidate();
                }
            }
        }

        [Category("Watermark attribtues")]
        [Description("When the control looses focus, this color will be used as the watermark's forecolor")]
        [Browsable(true), DefaultValue(typeof(Color), "LightGray")]
        private Color WaterMarkForeColor
        {
            get { return _waterMarkColor; }
            set
            {
                if (_waterMarkColor != value)
                {
                    _waterMarkColor = value;
                    Invalidate();
                }
            }
        }
        //---------------------------------------------------------------------
        [Browsable(true), Category("Behavior"), Description("Тип ввода"), DefaultValue(typeof(TextBoxTypeInput), "Normal")]
        public TextBoxTypeInput TypeInput
        {
            get { return _typeInput; }
            set
            {
                if (value < TextBoxTypeInput.Normal || value > TextBoxTypeInput.DigitsAndEnglishAndRussianLanguage)
                { throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TextBoxTypeInput)); }

                _typeInput = value;

                _clear();
                Invalidate();
            }
        }
        //---------------------------------------------------------------------
        [Browsable(true), Category("Clipboard"), DefaultValue(true)]
        public bool CopyAvailable
        {
            get { return _copyAvailable; }
            set { _copyAvailable = value; }
        }
        [Browsable(true), Category("Clipboard"), DefaultValue(true)]
        public bool PasteAvailable
        {
            get { return _pasteAvailable; }
            set { _pasteAvailable = value; }
        }
        //---------------------------------------------------------------------
        [Browsable(true), DefaultValue("")]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (_isPaste(value))
                { base.Text = value; }
            }
        }
        //---------------------------------------------------------------------
        public void PhoneNumberClear()
        {
            if (_typeInput != TextBoxTypeInput.PhoneNumber)
            { throw new InvalidOperationException(); }

            _clear();
        }
        public void PhoneNumberBackSpaceKey()
        {
            if (_typeInput != TextBoxTypeInput.PhoneNumber)
            { throw new InvalidOperationException(); }

            if (Text.Length != 0)
            {
                var count = 1;
                int startIndex;
                if (SelectedText != string.Empty)
                {
                    count = SelectionLength;
                    startIndex = SelectionStart;
                    Text = Text.Remove(startIndex, count);
                }
                else
                {
                    startIndex = SelectionStart - count;
                    if (startIndex <= 0)
                        startIndex = Text.Length - 1;
                    Text = Text.Remove(startIndex, count);
                }
            }
        }
        public void PhoneNumberAdd(char value)
        {
            if (_typeInput != TextBoxTypeInput.PhoneNumber)
            { throw new InvalidOperationException(); }

            PhoneNumberAdd(value.ToString());
        }
        public void PhoneNumberAdd(string value)
        {
            if (_typeInput != TextBoxTypeInput.PhoneNumber)
            { throw new InvalidOperationException(); }

            Paste(value);
        }
        //---------------------------------------------------------------------
        public bool IsPaste(string message)
        {
            return _isPaste(message);
        }
        public new void Paste(string message)
        {
            if (_isPaste(message))
            { base.Paste(message); }
        }
        //---------------------------------------------------------------------
        public event EventHandler<TextBoxClipboardMessageEventArgs> ClipboardPasteMessage;
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public enum TextBoxTypeInput
    {
        Normal,
        PhoneNumber,
        Digits,
        Hex,
        EnglishLanguage,
        DigitsAndEnglishLanguage,
        DigitsAndEnglishAndRussianLanguage
    }
    //-------------------------------------------------------------------------
    public sealed class TextBoxClipboardMessageEventArgs : EventArgs
    {
        #region private
        private readonly string _message;
        #endregion
        public TextBoxClipboardMessageEventArgs(string message)
        {
            _message = message;
        }

        //---------------------------------------------------------------------
        public bool Cancel { get; set; }
        //---------------------------------------------------------------------
        public string Message
        {
            get { return _message; }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal static class TextBoxBaseExtension
    {
        //---------------------------------------------------------------------
        public static void SetPositionCursor(this TextBoxBase textBox, int position)
        {
            if ((textBox != null) && (position > -1))
            {
                textBox.SelectionStart = position;
                textBox.SelectionLength = 0;
            }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal static class NativeMethods
    {
        #region private
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetTextMetrics(IntPtr hdc, out Textmetric lptm);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr hdc);
        #endregion
        //---------------------------------------------------------------------
        [Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Textmetric
        {
            public int tmHeight;
            public int tmAscent;
            public int tmDescent;
            public int tmInternalLeading;
            public int tmExternalLeading;
            public int tmAveCharWidth;
            public int tmMaxCharWidth;
            public int tmWeight;
            public int tmOverhang;
            public int tmDigitizedAspectX;
            public int tmDigitizedAspectY;
            public char tmFirstChar;
            public char tmLastChar;
            public char tmDefaultChar;
            public char tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public byte tmPitchAndFamily;
            public byte tmCharSet;
        }
        //---------------------------------------------------------------------
        [SuppressMessage("Microsoft.Usage", "CA1806", Justification = "What should be done if ReleaseDC() doesn't work?")]
        public static Textmetric GetTextMetrics(IntPtr hwnd, Font font)
        {
            IntPtr hdc = GetWindowDC(hwnd);

            Textmetric textMetric;
            IntPtr hFont = font.ToHfont();

            try
            {
                IntPtr hFontPrevious = SelectObject(hdc, hFont);
                GetTextMetrics(hdc, out textMetric);
                SelectObject(hdc, hFontPrevious);
            }
            finally
            {
                ReleaseDC(hwnd, hdc);
                DeleteObject(hFont);
            }

            return textMetric;
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class TextBoxExtendedDesigner : ControlDesigner
    {
        #region private
        private DesignerActionListCollection _actionList;
        private TextBoxExtended _control;
        #endregion
        #region protected
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            properties.Remove("BackColor");
            properties.Remove("ReadOnly");
        }
        #endregion
        //---------------------------------------------------------------------
        public override DesignerActionListCollection ActionLists
        {
            get { return LazyInitializer.EnsureInitialized(ref _actionList, () => new DesignerActionListCollection { new DesignerTextBoxExtendedActionList(Component) }); }
        }
        public override IList SnapLines
        {
            get
            {
                if (_control == null)
                    _control = (TextBoxExtended)Control;

                var snapLines = base.SnapLines;
                snapLines?.Add(new SnapLine(SnapLineType.Baseline, _control.Baseline));
                return snapLines;
            }
        }
        //---------------------------------------------------------------------
    }
    internal sealed class DesignerTextBoxExtendedActionList : DesignerActionList
    {
        #region private
        private readonly TextBoxExtended _control;
        private readonly DesignerActionUIService _uiService;
        #endregion
        public DesignerTextBoxExtendedActionList(IComponent component)
            : base(component)
        {
            if (!(component is TextBoxExtended))
            { throw new ArgumentException(string.Empty, nameof(component)); }

            _control = (TextBoxExtended)component;
            _uiService = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
        }

        //---------------------------------------------------------------------
        public string WaterMark
        {
            get { return _control.WaterMark; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.Text)).SetValue(_control, string.Empty);
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.WaterMark)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        //---------------------------------------------------------------------
        public TextBoxTypeInput TypeInput
        {
            get { return _control.TypeInput; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.TypeInput)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        //---------------------------------------------------------------------
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var actionPropertiesHeader = new DesignerActionHeaderItem("Свойства", "Properties");
            var items = new DesignerActionItemCollection { actionPropertiesHeader };
            //-----------------------------------------------------------------
            items.AddActionPropertyItem(
               nameof(WaterMark),
               _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.WaterMark)),
               actionPropertiesHeader.Category
            );
            //-----------------------------------------------------------------
            items.AddActionPropertyItem(
              nameof(TypeInput),
              _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.TypeInput)),
              actionPropertiesHeader.Category
            );
            //-----------------------------------------------------------------
            return items;
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}
