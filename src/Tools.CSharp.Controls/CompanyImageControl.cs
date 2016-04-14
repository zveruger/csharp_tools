using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Tools.CSharp.Controls.Extensions;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Controls
{
    //-------------------------------------------------------------------------
    [Description("Логотип компании ЗАО ТЕЛРОС"), Designer(typeof(CompanyImageDesigner))]
    public sealed partial class CompanyImageControl : UserControl
    {
        #region private
        private const double _CornerValuePiDivide180 = 0.017453292519943295769236907684886;
        private const float _RadiusConst = 109.0f;
        private const float _RadiusCircleConst = 42.0f;
        private const float _DimRadiusConst = 31.0f;
        private const float _DimConst = 10.0f;
        private const float _EditDivide = 0.005f;
        private const double _MainAngle = 45;
        private const float _MounthWidth = 8.0f;
        private readonly Size _minSize = new Size(30, 30);
        //---------------------------------------------------------------------
        private Color _startGradientColor = Color.Green;
        private int _startGradientAlpha = 255;
        private Color _endGradientColor = Color.DarkGreen;
        private int _endGradientAlpha = 255;
        private float _angleGradient = 135.0f;
        //---------------------------------------------------------------------
        private PointF _centreCoordinate = new PointF(0, 0);
        private float _radiusMainCircle;
        private float _radiusCircle;
        private float _dim;
        private float _dimRadius;
        //---------------------------------------------------------------------
        private static PointF _calculationCoordinate(float radius, double deg)
        {
            var buf = _CornerValuePiDivide180 * (deg + 90);
            var x = (float)(radius * Math.Cos(buf));
            var y = (float)(radius * Math.Sin(buf));
            return new PointF(-x, -y);
        }
        private static RectangleF _calculateRectangle(float radius, double angle)
        {
            var rightTopPoint = _calculationCoordinate(radius, angle);
            var rightBottomPoint = _calculationCoordinate(radius, 180 - angle);
            //var leftBottomPoint  = _calculationCoordinate(radius, 180 + angle);
            var leftTopPoint = _calculationCoordinate(radius, 360 - angle);

            var width = Math.Abs(rightTopPoint.X) + Math.Abs(leftTopPoint.X);
            var height = Math.Abs(rightTopPoint.Y) + Math.Abs(rightBottomPoint.Y);

            return new RectangleF(leftTopPoint, new SizeF(width, height));
        }
        //---------------------------------------------------------------------
        private void _createMainCirlce(Graphics graph, PointF centre, float radius, int leftDelta)
        {
            var leftPointX = centre.X - radius + leftDelta;
            var leftPointY = leftPointX;
            var diametr = 2 * (radius - leftDelta);

            var leftPoint = new PointF(leftPointX, leftPointY);
            var size = new SizeF(diametr, diametr);
            var mainRect = new RectangleF(leftPoint, size);

            _circleShow(graph, mainRect);
        }
        private void _createMainRectangle(Graphics graph, PointF centre, float radius)
        {
            var mainRect = _calculateRectangle(radius, _MainAngle);

            var topPoint = _calculationCoordinate(radius, 0);
            //var rightPoint = _calculationCoordinate(radius, 90);

            float relative = 2;
            var pointMidY = Math.Abs(topPoint.Y) - Math.Abs(mainRect.Y + relative);
            graph.FillRectangle(new SolidBrush(BackColor), mainRect.X, -radius - relative, mainRect.Width + pointMidY, mainRect.Height + pointMidY);
        }
        private void _createCircleCollection(Graphics graph, RectangleF mainRect, float radiusCirlce)
        {
            var pointRectangleX = mainRect.Width / 2.0f;
            var pointRectangleY = mainRect.Height / 2.0f;

            _createCircle(graph, new PointF(mainRect.X, mainRect.Y), radiusCirlce);
            _createCircle(graph, new PointF(mainRect.X + pointRectangleX, mainRect.Y), radiusCirlce);
            _createCircle(graph, new PointF(mainRect.X + mainRect.Width, mainRect.Y), radiusCirlce);

            _createCircle(graph, new PointF(mainRect.X, mainRect.Y + pointRectangleY), radiusCirlce);
            _createCircle(graph, new PointF(mainRect.X + pointRectangleX, mainRect.Y + pointRectangleY), radiusCirlce);
            _createCircle(graph, new PointF(mainRect.X + mainRect.Width, mainRect.Y + pointRectangleY), radiusCirlce);

            _createCircle(graph, new PointF(mainRect.X, mainRect.Y + mainRect.Height), radiusCirlce);
            _createCircle(graph, new PointF(mainRect.X + pointRectangleX, mainRect.Y + mainRect.Height), radiusCirlce);
            _createCircle(graph, new PointF(mainRect.X + mainRect.Width, mainRect.Y + mainRect.Height), radiusCirlce);

            // graph.DrawRectangle(new Pen(Color.Black, 1), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
        private void _createCircle(Graphics graph, PointF location, float radiusCircle)
        {
            var centreX = Convert.ToSingle(radiusCircle / 2.0);
            var centreY = centreX;
            var centre = new PointF(centreX, centreY);
            var mainRect = new RectangleF(location.X - centre.X, location.Y - centre.Y, radiusCircle, radiusCircle);

            _circleShow(graph, mainRect);
        }
        private void _circleShow(Graphics graph, RectangleF rectCircle)
        {
            using (var brush = new LinearGradientBrush(
                rectCircle,
                Color.FromArgb(_startGradientAlpha, _startGradientColor),
                Color.FromArgb(_endGradientAlpha, _endGradientColor),
                _angleGradient))
            {
                graph.FillEllipse(brush, rectCircle);
            }
        }
        //---------------------------------------------------------------------
        private void _draw(Graphics e)
        {
            e.SmoothingMode = SmoothingMode.AntiAlias;
            e.TextRenderingHint = TextRenderingHint.AntiAlias;
            e.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //----------------------
            _createMainCirlce(e, _centreCoordinate, _radiusMainCircle, -1);

            e.TranslateTransform(_centreCoordinate.X, _centreCoordinate.Y);
            _createMainRectangle(e, _centreCoordinate, _radiusMainCircle);

            e.TranslateTransform(_dim, -_dim);
            var rectangleCircles = _calculateRectangle(_radiusMainCircle - _dimRadius, _MainAngle);//31
            _createCircleCollection(e, rectangleCircles, _radiusCircle);
        }
        //---------------------------------------------------------------------
        private void _initializeParams()
        {
            var centreX = Width / 2.0f;
            var centreY = centreX;

            _centreCoordinate = new PointF(centreX, centreY);
            _radiusMainCircle = _centreCoordinate.X - _MounthWidth;

            var bufMash = _radiusMainCircle / _RadiusConst;
            _radiusCircle = _RadiusCircleConst * (bufMash + _EditDivide);
            _dimRadius = _DimRadiusConst * (bufMash - _EditDivide);
            _dim = _DimConst * bufMash;
        }
        #endregion
        #region protected
        protected override void OnPaint(PaintEventArgs e)
        {
            _draw(e.Graphics);
            base.OnPaint(e);
        }
        protected override void OnResize(EventArgs e)
        {
            Size = new Size(Width, Width);
            _initializeParams();
            Invalidate();
        }
        #endregion
        public CompanyImageControl()
        {
            InitializeComponent();

            MinimumSize = _minSize;
            _initializeParams();
        }

        //---------------------------------------------------------------------
        [Category("Gradient"), Description("Начальный цвет градиента"), DefaultValue(typeof(Color), "Green")]
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
        [Category("Gradient"), Description("Конечный цвет градиента"), DefaultValue(typeof(Color), "DarkGreen")]
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
        [Category("Gradient"), Description("Угол заливки градиента"), DefaultValue(135.0f)]
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
    }
    //-------------------------------------------------------------------------
    internal sealed class CompanyImageDesigner : ControlDesigner
    {
        #region private
        private DesignerActionListCollection _actionList;
        private DesignerVerbCollection _verbs;
        #endregion
        #region protected
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            properties.Remove("BackColor");
        }
        #endregion
        //---------------------------------------------------------------------
        public override DesignerActionListCollection ActionLists
        {
            get { return LazyInitializer.EnsureInitialized(ref _actionList, () => new DesignerActionListCollection { new DesignerCompanyImageActionList(Control) }); }
        }
        public override DesignerVerbCollection Verbs
        {
            get { return LazyInitializer.EnsureInitialized(ref _verbs, () => new DesignerCompanyImageVerbCollection(Control)); }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class DesignerCompanyImageActionList : DesignerActionList
    {
        #region private
        private readonly CompanyImageControl _control;
        private readonly DesignerActionUIService _uiService;
        #endregion
        public DesignerCompanyImageActionList(IComponent component)
            : base(component)
        {
            if (!(component is CompanyImageControl))
            { throw new ArgumentNullException(nameof(component)); }

            _control = (CompanyImageControl)component;
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
    internal sealed class DesignerCompanyImageVerbCollection : DesignerVerbCollection
    {
        #region private
        private readonly CompanyImageControl _control;
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
        public DesignerCompanyImageVerbCollection(IComponent component)
        {
            if (!(component is CompanyImageControl))
            { throw new ArgumentNullException(nameof(component)); }

            _control = (CompanyImageControl)component;

            Add(new DesignerVerb("Инвертировать цвета градиента", (o, e) => _invertGradientColor()));
        }
    }
    //-------------------------------------------------------------------------
}
