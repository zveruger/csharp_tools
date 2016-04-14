using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Tools.CSharp.Controls.Extensions;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Controls
{
    //-------------------------------------------------------------------------
    //[Designer(typeof(GradientToolStripDesigner))]
    public sealed class GradientToolStrip : ToolStrip
    {
        #region private
        private readonly GradientToolStripProfessionalRenderer _renderer = new GradientToolStripProfessionalRenderer();
        //---------------------------------------------------------------------
        private Color _startGradientColor = Color.White;
        private int _startGradientAlpha = 255;
        private Color _endGradientColor = Color.LightGray;
        private int _endGradientAlpha = 255;
        private float _angleGradient = 90.0f;
        #endregion
        #region protected
        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            var mainRect = ClientRectangle;
            if (mainRect.Height != 0 && mainRect.Width != 0)
            {
                using (var brush = new LinearGradientBrush(mainRect, Color.FromArgb(_startGradientAlpha, _startGradientColor), Color.FromArgb(_endGradientAlpha, _endGradientColor), _angleGradient))
                {
                    brush.WrapMode = WrapMode.TileFlipXY;
                    graphics.FillRectangle(brush, mainRect);
                }
            }

            base.OnPaint(e);
        }
        #endregion
        public GradientToolStrip()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);
            UpdateStyles();

            Renderer = _renderer;
            Font = new Font("Arial", 9F);
        }

        //---------------------------------------------------------------------
        [Category("Gradient"), Description("Начальный цвет градиента"), DefaultValue(typeof(Color), "White")]
        public Color StartGradientColor
        {
            get { return _startGradientColor; }
            set
            {
                if (_startGradientColor != value)
                {
                    _startGradientColor = value;
                    Invalidate();
                }
            }
        }

        [Category("Gradient"), Description("Прозрачность начального цвета градиента. Допустимые значения — от 0 до 255"), DefaultValue(255)]
        public int StartGradientAlpha
        {
            get { return _startGradientAlpha; }
            set
            {
                if (_startGradientAlpha != value)
                {
                    _startGradientAlpha = value;
                    Invalidate();
                }
            }
        }
        //---------------------------------------------------------------------
        [Category("Gradient"), Description("Конечный цвет градиента"), DefaultValue(typeof(Color), "LightGray")]
        public Color EndGradientColor
        {
            get { return _endGradientColor; }
            set
            {
                if (_endGradientColor != value)
                {
                    _endGradientColor = value;
                    Invalidate();
                }
            }
        }

        [Category("Gradient"), Description("Прозрачность конечного цвета градиента. Допустимые значения — от 0 до 255"), DefaultValue(255)]
        public int EndGradientAlpha
        {
            get { return _endGradientAlpha; }
            set
            {
                if (_endGradientAlpha != value)
                {
                    _endGradientAlpha = value;
                    Invalidate();
                }
            }
        }
        //---------------------------------------------------------------------
        [Category("Gradient"), Description("Угол заливки градиента"), DefaultValue(90.0f)]
        public float AngleGradient
        {
            get { return _angleGradient; }
            set
            {
                if (!_angleGradient.Equals(value))
                {
                    _angleGradient = value;
                    Invalidate();
                }
            }
        }
        //---------------------------------------------------------------------
        [Browsable(true), DefaultValue(true)]
        public bool RenderBorder
        {
            get { return _renderer.RenderBorder; }
            set
            {
                if (_renderer.RenderBorder != value)
                {
                    _renderer.RenderBorder = value;
                    Invalidate();
                }
            }
        }
        //---------------------------------------------------------------------
        [DefaultValue(typeof(Font), "Arial, 9pt")]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class GradientToolStripProfessionalRenderer : ToolStripProfessionalRenderer
    {
        #region private
        private bool _renderBorder = true;
        #endregion
        #region protected
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (_renderBorder)
            { base.OnRenderToolStripBorder(e); }
            else
            {
                var toolStrip = e.ToolStrip;
                if (toolStrip is ToolStripDropDown)
                { base.OnRenderToolStripBorder(e); }
                else if (toolStrip is MenuStrip)
                { }
                else if (toolStrip is StatusStrip)
                { base.OnRenderToolStripBorder(e); }
                else
                {
                    var g = e.Graphics;
                    var bounds = new Rectangle(Point.Empty, toolStrip.Size);
                    using (var p = new Pen(ColorTable.ToolStripBorder))
                    {
                        if (toolStrip.Orientation == Orientation.Horizontal)
                        { g.DrawLine(p, bounds.Left, bounds.Height - 1, bounds.Right, bounds.Height - 1); }
                        else
                        { g.DrawLine(p, bounds.Width - 1, 0, bounds.Width - 1, bounds.Height - 1); }
                    }
                }
            }
        }
        #endregion
        public GradientToolStripProfessionalRenderer()
        {
            RoundedEdges = false;
        }

        //---------------------------------------------------------------------
        public bool RenderBorder
        {
            get { return _renderBorder; }
            set { _renderBorder = value; }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class GradientToolStripDesigner : ControlDesigner
    {
        #region private
        private DesignerActionListCollection _actionList;
        private DesignerVerbCollection _verbs;
        #endregion
        #region protected
        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            base.PreFilterProperties(properties);

            properties.Remove("BackColor");
            properties.Remove("Text");
        }
        #endregion
        //---------------------------------------------------------------------
        public override DesignerActionListCollection ActionLists
        {
            get { return LazyInitializer.EnsureInitialized(ref _actionList, () => new DesignerActionListCollection { new DesignerGradientToolStripActionList(Control) }); }
        }
        public override DesignerVerbCollection Verbs
        {
            get { return LazyInitializer.EnsureInitialized(ref _verbs, () => new DesignerGradientToolStripVerbCollection(Control)); }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class DesignerGradientToolStripActionList : DesignerActionList
    {
        #region private
        private readonly GradientToolStrip _control;
        private readonly DesignerActionUIService _uiService;
        #endregion
        public DesignerGradientToolStripActionList(IComponent component)
            : base(component)
        {
            if (!(component is GradientToolStrip))
            { throw new ArgumentNullException(nameof(component)); }

            _control = (GradientToolStrip)component;
            _uiService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
        }

        //---------------------------------------------------------------------
        public Color StartGradientColor
        {
            get { return _control.StartGradientColor; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.StartGradientColor)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        public int StartGradientAlpha
        {
            get { return _control.StartGradientAlpha; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.StartGradientAlpha)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        //---------------------------------------------------------------------
        public Color EndGradientColor
        {
            get { return _control.EndGradientColor; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.EndGradientColor)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        public int EndGradientAlpha
        {
            get { return _control.EndGradientAlpha; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.EndGradientAlpha)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        //---------------------------------------------------------------------
        public float AngleGradient
        {
            get { return _control.AngleGradient; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.AngleGradient)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        //---------------------------------------------------------------------
        public void InvertGradientColor()
        {
            var tmpStartGradColor = StartGradientColor;
            StartGradientColor = EndGradientColor;
            EndGradientColor = tmpStartGradColor;
        }
        //---------------------------------------------------------------------
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var actionPropertiesHeader = new DesignerActionHeaderItem("Свойства", "Properties");
            var items = new DesignerActionItemCollection { actionPropertiesHeader };
            //-----------------------------------------------------------------
            items.AddActionPropertyItem(
                nameof(StartGradientColor),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.StartGradientColor)),
                actionPropertiesHeader.Category
            );
            items.AddActionPropertyItem(
                nameof(StartGradientAlpha),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.StartGradientAlpha)),
                actionPropertiesHeader.Category
            );
            //-----------------------------------------------------------------
            items.AddActionPropertyItem(
                nameof(EndGradientColor),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.EndGradientColor)),
                actionPropertiesHeader.Category
            );
            items.AddActionPropertyItem(
                nameof(EndGradientAlpha),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.EndGradientAlpha)),
                actionPropertiesHeader.Category
            );
            //-----------------------------------------------------------------
            items.AddActionPropertyItem(
               nameof(AngleGradient),
               _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.AngleGradient)),
               actionPropertiesHeader.Category
            );
            //-----------------------------------------------------------------
            if (StartGradientColor != EndGradientColor)
            {
                var actionMethodsHeader = new DesignerActionHeaderItem("Методы", "Methods");
                items.Add(actionMethodsHeader);
                //-------------------------------------------------------------
                items.Add(new DesignerActionMethodItem(
                    this,
                    nameof(InvertGradientColor),
                    "Инвертировать цвета градиента",
                    actionMethodsHeader.Category,
                    "Поменять местами начальный и конечный цвет градиента"
                ));
                //-------------------------------------------------------------
            }
            //-----------------------------------------------------------------
            return items;
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class DesignerGradientToolStripVerbCollection : DesignerVerbCollection
    {
        #region private
        private readonly GradientToolStrip _control;
        //---------------------------------------
        private Color _StartGradientColor
        {
            get { return _control.StartGradientColor; }
            set { _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.StartGradientColor)).SetValue(_control, value); }
        }
        private Color _EndGradientColor
        {
            get { return _control.EndGradientColor; }
            set { _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.EndGradientColor)).SetValue(_control, value); }
        }
        private void _invertGradientColor()
        {
            var tmpStartGradColor = _StartGradientColor;
            _StartGradientColor = _EndGradientColor;
            _EndGradientColor = tmpStartGradColor;
        }
        #endregion
        public DesignerGradientToolStripVerbCollection(IComponent component)
        {
            if (!(component is GradientToolStrip))
            { throw new ArgumentNullException(nameof(component)); }

            _control = (GradientToolStrip)component;

            Add(new DesignerVerb("Инвертировать цвета градиента", (o, e) => _invertGradientColor()));
        }
    }
    //-------------------------------------------------------------------------
}
