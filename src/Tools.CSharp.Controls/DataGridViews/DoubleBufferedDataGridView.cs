using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Tools.CSharp.Controls
{
    public class DoubleBufferedDataGridView : DataGridView
    {
        #region private
        private bool _doubleBuffered;
        //---------------------------------------------------------------------
        private void _setDoubleBuffered(bool value)
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, value);
            UpdateStyles();
        }
        //---------------------------------------------------------------------
        private void _subscribeVerticalScrollBarAllEvents(bool addEvents)
        {
            if (addEvents)
            {
                VerticalScrollBar.VisibleChanged += _verticalScrollBarOnVisibleChanged;
                HorizontalScrollBar.VisibleChanged += _horizontalScrollBarOnVisibleChanged;
            }
            else
            {
                VerticalScrollBar.VisibleChanged -= _verticalScrollBarOnVisibleChanged;
                HorizontalScrollBar.VisibleChanged -= _horizontalScrollBarOnVisibleChanged;
            }
        }

        private void _verticalScrollBarOnVisibleChanged(object sender, EventArgs e)
        {
            OnVerticalScrollBarVisibleChanged();
        }
        private void _horizontalScrollBarOnVisibleChanged(object sender, EventArgs eventArgs)
        {
            OnHorizontalScrollBarVisibleChanged();
        }
        #endregion
        #region protected
        protected virtual void OnVerticalScrollBarVisibleChanged()
        {
            VerticalScrollBarVisibleChanged?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnHorizontalScrollBarVisibleChanged()
        {
            HorizontalScrollBarVisibleChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnCursorChanged(EventArgs e)
        {
            base.OnCursorChanged(e);

            if (_doubleBuffered)
            { _setDoubleBuffered(Cursor == Cursors.Default); }
        }
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                { _subscribeVerticalScrollBarAllEvents(false); }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        public DoubleBufferedDataGridView()
        {
            _subscribeVerticalScrollBarAllEvents(true);
        }

        //---------------------------------------------------------------------
        [Browsable(true), DefaultValue(false)]
        public bool SupperDoubleBuffered
        {
            get { return _doubleBuffered; }
            set
            {
                _doubleBuffered = value;
                _setDoubleBuffered(_doubleBuffered);
            }
        }
        //---------------------------------------------------------------------
        [Browsable(true)]
        public bool IsVerticalScrollBarVisible
        {
            get { return VerticalScrollBar.Visible; }
        }

        [Browsable(true)]
        public bool IsHorizontalScrollBarVisible
        {
            get { return HorizontalScrollBar.Visible; }
        }
        //---------------------------------------------------------------------
        public event EventHandler VerticalScrollBarVisibleChanged;
        public event EventHandler HorizontalScrollBarVisibleChanged;
        //---------------------------------------------------------------------
    }
}
