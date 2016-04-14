using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Tools.CSharp.Controls.Extensions
{
    public static class ControlExtensions
    {
        #region private
        private const int _WmSetredraw = 0xB;
        //---------------------------------------------------------------------
        [DllImport("user32.dll", EntryPoint = "SendMessageA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        //---------------------------------------------------------------------
        private static readonly Point _DefaultInitLocationMessage = new Point(0, 0);
        //---------------------------------------------------------------------
        private static void _SelectedTextTextBox(TextBox control)
        {
            control.SelectionStart = 0;
            control.SelectionLength = control.Text.Length;
        }
        private static void _SelectedTextComboBox(ComboBox control)
        {
            control.SelectionStart = 0;
            control.SelectionLength = control.Text.Length;
        }
        #endregion
        //---------------------------------------------------------------------
        public static void ShowTextByControlCenter(this Control control, int rectangleWidth, string message)
        {
            if (control == null)
            { throw new ArgumentNullException(nameof(control)); }

            control.Text = message;

            if (!string.IsNullOrWhiteSpace(message))
            {
                var messageSize = message.GetPossibleSize(control.Font);
                control.Left = (int)(rectangleWidth - messageSize.Width) / 2;
            }
        }
        //---------------------------------------------------------------------
        public static void SuspendDrawing(this Control control)
        {
            if (control == null)
            { throw new ArgumentNullException(nameof(control)); }

            SendMessage(control.Handle, _WmSetredraw, 0, 0);
        }
        public static void ResumeDrawing(this Control control)
        {
            ResumeDrawing(control, true);
        }
        public static void ResumeDrawing(this Control control, bool redraw)
        {
            if (control == null)
            { throw new ArgumentNullException(nameof(control)); }

            SendMessage(control.Handle, _WmSetredraw, 1, 0);

            if (redraw)
            { control.Refresh(); }
        }
        //---------------------------------------------------------------------
        public static void InvokeAsync(this Control control, Action action)
        {
            if (control == null)
            { throw new ArgumentNullException(nameof(control)); }
            if (action == null)
            { throw new ArgumentNullException(nameof(action)); }

            try
            {
                var thread = new Thread(() =>
                {
                    if (!control.IsDisposed)
                    {
                        try { control.Invoke(action); }
                        catch (InvalidOperationException) { action(); }
                    }
                });
                thread.Start();
            }
            catch (ObjectDisposedException)
            { }
        }
        public static void Invoke(this Control control, MethodInvoker method)
        {
            if (control == null)
            { throw new ArgumentNullException(nameof(control)); }
            if (method == null)
            { throw new ArgumentNullException(nameof(method)); }

            control.Invoke(method);
        }
        //---------------------------------------------------------------------
        public static bool SetFocus(this Control control)
        {
            if (control == null)
            { throw new ArgumentNullException(nameof(control)); }

            return control.CanFocus && control.Focus();
        }
        public static bool SetFocusAndSelectedTextControl(this Control control)
        {
            if (control == null)
            { throw new ArgumentNullException(nameof(control)); }

            if (control.SetFocus())
            {
                var textBox = control as TextBox;
                if (textBox != null)
                {
                    _SelectedTextTextBox(textBox);
                    return true;
                }

                var comboBox = control as ComboBox;
                if (comboBox != null)
                {
                    _SelectedTextComboBox(comboBox);
                    return true;
                }
            }
            return false;
        }
        //---------------------------------------------------------------------
        public static Point GetCentreLocationMessage(this Control control, string message, Font font)
        {
            return GetCentreLocationMessage(control, message, font, _DefaultInitLocationMessage, true, true);
        }
        public static Point GetCentreLocationMessage(this Control control, string message, Font font, Point initLocation, bool centreHorizontally, bool centerVertically)
        {
            if (control == null)
            { throw new ArgumentNullException(nameof(control)); }
            if (font == null)
            { throw new ArgumentNullException(nameof(font)); }

            var messageSize = TextRenderer.MeasureText(message, font);

            var locationX = centreHorizontally ? Convert.ToInt32((control.Width - messageSize.Width) / 2.0f) : initLocation.X;
            var locationY = centerVertically ? Convert.ToInt32((control.Height - messageSize.Height) / 2.0f) : initLocation.Y;
            return new Point(locationX, locationY);
        }
        //---------------------------------------------------------------------
    }
}
