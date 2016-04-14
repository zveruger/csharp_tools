using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using Tools.CSharp.Controls.Extensions;

namespace Tools.CSharp.Controls
{
    public sealed class NumericUpDownExtended : NumericUpDown
    {
        #region private
        private TextBox _editTextBox;
        private NativeWindowListener _contextMenuListener;
        //---------------------------------------------------------------------
        private TextBox _getEditTextBox(NumericUpDownExtended control)
        {
            var fieldInfos = typeof(NumericUpDown).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.FieldType.Name.Equals("UpDownEdit", StringComparison.CurrentCultureIgnoreCase))
                { return fieldInfo.GetValue(control) as TextBox; }
            }
            return null;
        }
        //---------------------------------------------------------------------
        private class NativeWindowListener : NativeWindow
        {
            #region private
            private const int WM_PASTE = 0x0302;
            //-----------------------------------------------------------------
            private readonly Form _parentForm;
            private readonly NumericUpDownExtended _owner;
            //-----------------------------------------------------------------
            private void _parentFormClosed(object sender, EventArgs e)
            {
                _parentForm.FormClosed -= _parentFormClosed;
                ReleaseHandle();
            }
            #endregion
            #region protected
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_PASTE)
                {
                    if (!_owner._isValidClipboardMessage())
                    { return; }
                }

                base.WndProc(ref m);
            }
            #endregion
            public NativeWindowListener(Control editControl)
            {
                AssignHandle(editControl.Handle);

                _owner = (NumericUpDownExtended)editControl.Parent;
                _parentForm = _owner.FindForm();

                if (_parentForm != null)
                { _parentForm.FormClosed += _parentFormClosed; }
            }
        }
        //---------------------------------------------------------------------
        private bool _isValidClipboardMessage()
        {
            string clipboardMessage;
            if (ClipboardExtension.GetTextByUnicode(out clipboardMessage))
            {
                return _isValidMessage(clipboardMessage);
            }
            return false;
        }
        private bool _isValidMessage(string message)
        {
            if (_editTextBox == null || string.IsNullOrWhiteSpace(message))
            { return false; }

            var valueStr = Value.ToString(CultureInfo.CurrentCulture);

            var position = _editTextBox.SelectionStart;
            var length = _editTextBox.SelectionLength;
            var newMessage = length == 0 ? valueStr.Insert(position, message) : valueStr.Remove(position, length).Insert(position, message);

            var isValid = false;
            try
            {
                var nValue = Decimal.Parse(newMessage);
                isValid = nValue >= Minimum && nValue <= Maximum;
            }
            catch (ArgumentException) { }
            catch (FormatException) { }
            catch (OverflowException) { }

            return isValid;
        }
        #endregion
        #region protected
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (_editTextBox == null)
            {
                _editTextBox = _getEditTextBox(this);

                if (_editTextBox != null)
                { _contextMenuListener = new NativeWindowListener(_editTextBox); }
            }
        }
        protected override void OnTextBoxKeyPress(object source, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = !_isValidMessage(e.KeyChar.ToString(CultureInfo.CurrentCulture));
            }

            base.OnTextBoxKeyPress(source, e);
        }
        #endregion
        public NumericUpDownExtended()
        {
            Font = new Font("Arial", 10F);
        }

        //---------------------------------------------------------------------
        [DefaultValue(typeof(Font), "Arial, 10pt")]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }
        //---------------------------------------------------------------------
    }
}
