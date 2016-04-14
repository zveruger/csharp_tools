using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Tools.CSharp.Controls.Extensions;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Controls
{
    //-------------------------------------------------------------------------
    [Designer(typeof(SeparatorLineDesigner))]
    public sealed class SeparatorLine : UserControl
    {
        #region private
        private Orientation _orientation = Orientation.Horizontal;
        //---------------------------------------------------------------------
        private void _orientationUpdate(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    {
                        Width = Height;
                        Height = SystemInformation.Border3DSize.Height;
                    }
                    break;
                case Orientation.Vertical:
                    {
                        Height = Width;
                        Width = SystemInformation.Border3DSize.Width;
                    }
                    break;
            }
        }
        #endregion
        #region protected
        protected override Size DefaultSize
        {
            get { return new Size(base.DefaultSize.Width, 3); }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.SunkenOuter, Border3DSide.All);
        }
        #endregion
        public SeparatorLine()
        {
            _orientationUpdate(_orientation);

            ResizeRedraw = true;
            TabStop = false;
            Orientation = Orientation.Horizontal;

            SetStyle(ControlStyles.UserPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);
            UpdateStyles();

            Size = DefaultSize;
        }

        //---------------------------------------------------------------------
        [Browsable(false), DefaultValue(true)]
        public new bool ResizeRedraw
        {
            get { return base.ResizeRedraw; }
            set { base.ResizeRedraw = value; }
        }
        //---------------------------------------------------------------------
        [Browsable(false), DefaultValue(false)]
        public new bool TabStop
        {
            get { return base.TabStop; }
            set { base.TabStop = value; }
        }
        //---------------------------------------------------------------------
        [Category("Appearance"), Browsable(true), DefaultValue(typeof(Orientation), "Horizontal")]
        public Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                if (value < Orientation.Horizontal || value > Orientation.Vertical)
                { throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Orientation)); }

                if (_orientation != value)
                {
                    _orientation = value;
                    _orientationUpdate(_orientation);
                }
            }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal class SeparatorLineDesigner : ControlDesigner
    {
        #region private
        private DesignerActionListCollection _actionList;
        #endregion
        #region protected
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            properties.Remove("ResizeRedraw");
            properties.Remove("TabStop");
        }
        #endregion
        //---------------------------------------------------------------------
        public override SelectionRules SelectionRules
        {
            get
            {
                var rules = base.SelectionRules;
                var control = (SeparatorLine)Control;
                var orientation = control.Orientation;

                switch (orientation)
                {
                    case Orientation.Horizontal:
                        rules &= ~(SelectionRules.BottomSizeable | SelectionRules.TopSizeable);
                        break;
                    case Orientation.Vertical:
                        rules &= ~(SelectionRules.LeftSizeable | SelectionRules.RightSizeable);
                        break;
                }

                return rules;
            }
        }
        public override DesignerActionListCollection ActionLists
        {
            get { return LazyInitializer.EnsureInitialized(ref _actionList, () => new DesignerActionListCollection { new DesignerSeparatorLineActionList(Control)}); }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class DesignerSeparatorLineActionList : DesignerActionList
    {
        #region private
        private readonly SeparatorLine _control;
        private readonly DesignerActionUIService _uiService;
        #endregion
        public DesignerSeparatorLineActionList(IComponent component)
            : base(component)
        {
            if (!(component is SeparatorLine))
            { throw new ArgumentNullException(nameof(component)); }

            _control = (SeparatorLine)component;
            _uiService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
        }

        //---------------------------------------------------------------------
        public Orientation Orientation
        {
            get { return _control.Orientation; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.Orientation)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        //---------------------------------------------------------------------
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var actionPropertiesHeder = new DesignerActionHeaderItem("Свойства", "Properties");
            var items = new DesignerActionItemCollection { actionPropertiesHeder };
            //-----------------------------------------------------------------
            items.AddActionPropertyItem(
                nameof(Orientation),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.Orientation)),
                actionPropertiesHeder.Category
            );
            //-----------------------------------------------------------------
            return items;
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}
