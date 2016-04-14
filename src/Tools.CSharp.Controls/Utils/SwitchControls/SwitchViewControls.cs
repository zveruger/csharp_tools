using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Tools.CSharp.SwitchControls
{
    //-------------------------------------------------------------------------
    public abstract class SwitchViewControls<TKey> : BaseDisposable
    {
        #region private
        private readonly Dictionary<TKey, Lazy<Control>> _cacheControls = new Dictionary<TKey, Lazy<Control>>();
        private readonly ISwitchViewControlsOwner _owner;
        //---------------------------------------------------------------------
        private Control _getControl(TKey key)
        {
            Lazy<Control> lazyControl;
            var control = _cacheControls.TryGetValue(key, out lazyControl) ? lazyControl.Value : null;
            return control;
        }
        //---------------------------------------------------------------------
        private void _showControl(Control control, Control.ControlCollection controls)
        {
            if (control != null)
            {
                UpdateShowControl(control);
                control.Dock = DockStyle.Fill;
                controls.Add(control);
                control.BringToFront();
            }
        }
        //---------------------------------------------------------------------
        private static void _RemoveControlsButShowedControl(Control.ControlCollection controls, Control showedControl)
        {
            for (var i = 0; i < controls.Count; i++)
            {
                var tmpControl = controls[i];
                if (!ReferenceEquals(tmpControl, showedControl))
                {
                    controls.RemoveAt(i);
                    i--;
                }
            }
        }
        #endregion
        #region protected
        protected void AddControl(TKey key, Func<Control> valueFactory)
        {
            _cacheControls.Add(key, new Lazy<Control>(valueFactory));
        }
        //---------------------------------------------------------------------
        protected abstract void UpdateShowControl(Control control);
        //---------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    foreach (var lazyControl in _cacheControls.Select(cacheControl => cacheControl.Value).Where(lazyControl => lazyControl.IsValueCreated && lazyControl.Value != null))
                    { lazyControl.Value.Dispose(); }
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        protected SwitchViewControls(ISwitchViewControlsOwner owner)
        {
            if (owner == null)
            { throw new ArgumentNullException(nameof(owner)); }

            _owner = owner;
        }

        //---------------------------------------------------------------------
        public void SwitchOrUpdate(TKey key)
        {
            var controls = _owner.GetControls();
            if (controls != null)
            {
                var control = _getControl(key);
                var showedControl = controls.Count == 0 ? null : controls[0];
                if (ReferenceEquals(control, showedControl))
                { UpdateShowControl(showedControl); }
                else
                {
                    _showControl(control, controls);
                    _RemoveControlsButShowedControl(controls, control);
                }
            }
        }
        //---------------------------------------------------------------------
        public void Switch(TKey key)
        {
            var controls = _owner.GetControls();
            if (controls != null)
            {
                var control = _getControl(key);
                var showedControl = controls.Count == 0 ? null : controls[0];
                if (!ReferenceEquals(control, showedControl))
                {
                    _showControl(control, controls);
                    _RemoveControlsButShowedControl(controls, control);
                }
            }
        }
        //---------------------------------------------------------------------
        public void UpdateShowControl()
        {
            var controls = _owner.GetControls();
            var showedControl = controls.Count == 0 ? null : controls[0];
            if (showedControl != null)
            { UpdateShowControl(showedControl); }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public abstract class SwitchViewControls<TKey, TOwner> : SwitchViewControls<TKey>
        where TOwner : ISwitchViewControlsOwner
    {
        #region private
        private readonly TOwner _owner;
        #endregion
        #region protected
        protected TOwner GetOwner
        {
            get { return _owner; }
        }
        //---------------------------------------------------------------------
        protected abstract void UpdateShowControl(Control control, TOwner owner);
        //---------------------------------------------------------------------
        protected override void UpdateShowControl(Control control)
        {
            UpdateShowControl(control, _owner);
        }
        #endregion
        protected SwitchViewControls(TOwner owner)
            : base(owner)
        {
            _owner = owner;
        }
    }
    //-------------------------------------------------------------------------
}