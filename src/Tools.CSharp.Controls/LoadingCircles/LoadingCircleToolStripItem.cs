using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Tools.CSharp.Controls.Extensions;
using Tools.CSharp.Extensions;
using Timer = System.Windows.Forms.Timer;

namespace Tools.CSharp.Controls
{
    //[Designer(typeof(LoadingCircleToolStripItemDesigner))]
    public class LoadingCircleToolStripItem : ToolStripItem, ILoadingCircle
    {
        #region private
        private const double _NumberOfDegreesInCircle = 360;
        private const double _NumberOfDegreesInHalfCircle = _NumberOfDegreesInCircle / 2;
        private int _innerCircleRadius = 5;
        private int _outerCircleRadius = 10;
        private int _numberOfSpoke = 12;
        private int _spokeThickness = 2;
        private Color _spokeColor = Color.DarkGray;
        private bool _timerActive;
        //--------------------------------------------------
        private readonly Timer _progressTimer;
        private PointF _centerPoint;
        private int _progressValue;
        private Color[] _spokeColors;
        private double[] _angles;
        //--------------------------------------------------
        private void _activeTimer()
        {
            if (_timerActive)
            { _progressTimer.Start(); }
            else
            {
                _progressTimer.Stop();
                _progressValue = 0;
            }

            _updateColors(_spokeColor);
            Invalidate();
        }
        private void _subscribeProgressTimerAllEvents(bool addEvents)
        {
            if (_progressTimer != null)
            {
                if (addEvents)
                { _progressTimer.Tick += _progressTimerTick; }
                else
                { _progressTimer.Tick -= _progressTimerTick; }
            }
        }
        private void _progressTimerTick(object sender, EventArgs e)
        {
            _progressValue = ++_progressValue % _numberOfSpoke;
            Invalidate();
        }
        //--------------------------------------------------
        private void _updateColors(Color color)
        {
            _spokeColors = _GenerateColorsPallet(color, Active, _numberOfSpoke);
        }
        private void _updateAngles(int numberOfSpoke)
        {
            _angles = _GetSpokesAngles(numberOfSpoke);
        }
        private void _updateCenterPoint()
        {
            _centerPoint = _GetControlCenterPoint(this);
        }
        //--------------------------------------------------
        private static Color[] _GenerateColorsPallet(Color color, bool active, int numberOfSpoke)
        {
            var colors = new Color[numberOfSpoke];
            var increment = (byte)(byte.MaxValue / numberOfSpoke);
            var percentageOfDarken = 0;

            for (var counter = 0; counter < numberOfSpoke; counter++)
            {
                if (counter == 0 || !active)
                { colors[counter] = color; }
                else
                {
                    percentageOfDarken += increment;
                    colors[counter] = _Darken(color, percentageOfDarken);
                }
            }

            return colors;
        }
        private static double[] _GetSpokesAngles(int numberOfSpoke)
        {
            var angles = new double[numberOfSpoke];
            var angle = _NumberOfDegreesInCircle / numberOfSpoke;

            for (var counter = 0; counter < numberOfSpoke; counter++)
            { angles[counter] = (counter == 0 ? angle : angles[counter - 1] + angle); }

            return angles;
        }
        private static PointF _GetControlCenterPoint(ToolStripItem control)
        {
            return new PointF(control.Width / 2.0f, control.Height / 2.0f - 1);
        }
        private static Color _Darken(Color color, int percent)
        {
            return Color.FromArgb(percent, Math.Min(color.R, byte.MaxValue), Math.Min(color.G, byte.MaxValue), Math.Min(color.B, byte.MaxValue));
        }
        private static void _DrawLine(Graphics graphics, PointF pointFirst, PointF pointSecond, Color color, int spokeThickness)
        {
            using (var brush = new SolidBrush(color))
            {
                using (var pen = new Pen(brush, spokeThickness))
                {
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    graphics.DrawLine(pen, pointFirst, pointSecond);
                }
            }
        }
        private static PointF _GetCoordinate(PointF centerPoint, int innerCircleRadius, double angle)
        {
            var bufAngle = Math.PI * angle / _NumberOfDegreesInHalfCircle;
            return new PointF(centerPoint.X + innerCircleRadius * (float)Math.Cos(bufAngle), centerPoint.Y + innerCircleRadius * (float)Math.Sin(bufAngle));
        }
        #endregion
        #region protected
        protected override Size DefaultSize
        {
            get { return new Size(24, 32); }
        }
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                try
                {
                    if (disposing)
                    {
                        if (_progressTimer != null)
                        {
                            _progressTimer.Stop();
                            _subscribeProgressTimerAllEvents(false);
                            _progressTimer.Dispose();
                        }
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_numberOfSpoke > 0)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                int position = _progressValue;
                for (int counter = 0; counter < _numberOfSpoke; counter++)
                {
                    position = position % _numberOfSpoke;
                    _DrawLine(e.Graphics,
                        _GetCoordinate(_centerPoint, _innerCircleRadius, _angles[position]),
                        _GetCoordinate(_centerPoint, _outerCircleRadius, _angles[position]),
                        _spokeColors[counter],
                        _spokeThickness
                    );
                    position++;
                }
            }
            base.OnPaint(e);
        }
        #endregion
        public LoadingCircleToolStripItem()
        {
            _progressTimer = new Timer();

            _updateColors(_spokeColor);
            _updateAngles(_numberOfSpoke);
            _updateCenterPoint();

            _subscribeProgressTimerAllEvents(true);
            _activeTimer();

            Size = DefaultSize;
        }

        //---------------------------------------------------------------------
        [Category("Main"), Description("Внешний радиус"), DefaultValue(10)]
        public int OuterCircleRadius
        {
            get { return _outerCircleRadius; }
            set
            {
                if (_outerCircleRadius != value)
                {
                    _outerCircleRadius = value;
                    Invalidate();
                }
            }
        }

        [Category("Main"), Description("Внутренний радиус"), DefaultValue(5)]
        public int InnerCircleRadius
        {
            get { return _innerCircleRadius; }
            set
            {
                if (_innerCircleRadius != value)
                {
                    _innerCircleRadius = value;
                    Invalidate();
                }
            }
        }
        //---------------------------------------------------------------------
        [Category("Main"), Description("Количество спиц"), DefaultValue(12)]
        public int NumberSpoke
        {
            get { return _numberOfSpoke; }
            set
            {
                if (_numberOfSpoke != value && value > 0)
                {
                    _numberOfSpoke = value;
                    _updateColors(_spokeColor);
                    _updateAngles(_numberOfSpoke);
                    Invalidate();
                }
            }
        }

        [Category("Main"), Description("Толщина спицы"), DefaultValue(2)]
        public int SpokeThickness
        {
            get { return _spokeThickness; }
            set
            {
                if (_spokeThickness != value)
                {
                    _spokeThickness = value;
                    Invalidate();
                }
            }
        }

        [Category("Main"), Description("Цвет спиц"), DefaultValue(typeof(Color), "DarkGray")]
        public Color SpokeColor
        {
            get { return _spokeColor; }
            set
            {
                if (_spokeColor != value)
                {
                    _spokeColor = value;
                    _updateColors(_spokeColor);
                    Invalidate();
                }
            }
        }
        //---------------------------------------------------------------------
        [Category("Main"), Description("Скорость вращения"), DefaultValue(40)]
        public int RotationSpeed
        {
            get { return _progressTimer.Interval; }
            set
            {
                if (_progressTimer.Interval != value && value > 0)
                {
                    _progressTimer.Interval = value;
                }
            }
        }
        //---------------------------------------------------------------------
        [Category("Main"), Description("Включить вращение"), DefaultValue(false)]
        public bool Active
        {
            get { return _timerActive; }
            set
            {
                if (_timerActive != value)
                {
                    _timerActive = value;
                    _activeTimer();
                }
            }
        }
        //---------------------------------------------------------------------
        public new Size Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;
                _updateCenterPoint();
                Invalidate();
            }
        }
        //---------------------------------------------------------------------
        public override Size GetPreferredSize(Size proposedSize)
        {
            proposedSize.Width = (_outerCircleRadius + _spokeThickness) * 2;
            return proposedSize;
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class LoadingCircleToolStripItemDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        #region private
        private DesignerActionListCollection _actionList;
        #endregion
        //---------------------------------------------------------------------
        public override DesignerActionListCollection ActionLists
        {
            get { return LazyInitializer.EnsureInitialized(ref _actionList, () => new DesignerActionListCollection { new DesignerLoadingCircleToolStripItemActionList(Control) }); }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class DesignerLoadingCircleToolStripItemActionList : DesignerActionList
    {
        #region private
        private readonly LoadingCircleToolStripItem _control;
        private readonly DesignerActionUIService _uiService;
        #endregion
        public DesignerLoadingCircleToolStripItemActionList(IComponent component)
            : base(component)
        {
            if (!(component is LoadingCircleToolStripItem))
            { throw new ArgumentNullException(nameof(component)); }

            _control = (LoadingCircleToolStripItem)component;
            _uiService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
        }

        //---------------------------------------------------------------------
        public int OuterCircleRadius
        {
            get { return _control.OuterCircleRadius; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.OuterCircleRadius)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        public int InnerCircleRadius
        {
            get { return _control.InnerCircleRadius; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.InnerCircleRadius)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        //---------------------------------------------------------------------
        public int NumberSpoke
        {
            get { return _control.NumberSpoke; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.NumberSpoke)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        public int SpokeThickness
        {
            get { return _control.SpokeThickness; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.SpokeThickness)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        public Color SpokeColor
        {
            get { return _control.SpokeColor; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.SpokeColor)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        //---------------------------------------------------------------------
        public int RotationSpeed
        {
            get { return _control.RotationSpeed; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.RotationSpeed)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        //---------------------------------------------------------------------
        public bool Active
        {
            get { return _control.Active; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.Active)).SetValue(_control, value);
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
               nameof(Active),
               _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.Active)),
               actionPropertiesHeader.Category
            );
            //-----------------------------------------------------------------
            items.AddActionPropertyItem(
                nameof(OuterCircleRadius),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.OuterCircleRadius)),
                actionPropertiesHeader.Category
            );
            items.AddActionPropertyItem(
                nameof(InnerCircleRadius),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.InnerCircleRadius)),
                actionPropertiesHeader.Category
            );
            //-----------------------------------------------------------------
            items.AddActionPropertyItem(
                nameof(NumberSpoke),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.NumberSpoke)),
                actionPropertiesHeader.Category
            );
            items.AddActionPropertyItem(
                nameof(SpokeThickness),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.SpokeThickness)),
                actionPropertiesHeader.Category
            );
            items.AddActionPropertyItem(
                nameof(SpokeColor),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.SpokeColor)),
                actionPropertiesHeader.Category
            );
            //-----------------------------------------------------------------
            items.AddActionPropertyItem(
                nameof(RotationSpeed),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.RotationSpeed)),
                actionPropertiesHeader.Category
            );
            //-----------------------------------------------------------------
            return items;
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}
