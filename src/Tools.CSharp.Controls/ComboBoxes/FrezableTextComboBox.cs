using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Tools.CSharp.Controls
{
    public class FrezableTextComboBox : ComboBox
    {
        #region private
        private const int _WmPaste = 0x0302;
        //---------------------------------------------------------------------
        private bool _isFrizableText;
        #endregion
        #region protected
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == _WmPaste)
            {
                if (_isFrizableText)
                { return; }
            }
            base.WndProc(ref m);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = _isFrizableText;
            base.OnKeyDown(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            e.Handled = _isFrizableText;
            base.OnKeyPress(e);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            if (!_isFrizableText)
            { base.OnTextChanged(e); }
        }
        #endregion
        public FrezableTextComboBox()
        {
            Font = new Font("Arial", 9.5F);
        }

        //---------------------------------------------------------------------
        [Category("Appearance"), DefaultValue(false)]
        public bool IsFrizableText
        {
            get { return _isFrizableText; }
            set { _isFrizableText = value; }
        }
        //---------------------------------------------------------------------
        [DefaultValue(typeof(Font), "Arial, 9.5pt")]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }
        //---------------------------------------------------------------------
    }
}
