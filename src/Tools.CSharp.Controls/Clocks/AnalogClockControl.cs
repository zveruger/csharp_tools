using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Tools.CSharp.Controls.Clocks
{
    public class AnalogClockControl : UserControl
    {
        #region private
        private const float _WidthMouth = 8;
        //---------------------------------------------------------------------
        private static readonly Color _ShadeColor = Color.Black;
        private const int _ShadeAlphaColor = 60;
        //---------------------------------------------------------------------
        private readonly Pen _secondPen = new Pen(Color.Red, 1);
        private const float _SecondHandIndentBorder = 5.0f;
        //---------------------------------------------------------------------
        private readonly Pen _minutePen = new Pen(Color.Black, 3);
        private const float _MinuteHandIndentBorder = 12.0f;
        //---------------------------------------------------------------------
        private readonly Pen _hourPen = new Pen(Color.Black, 5);
        private const float _HourHandIndentBorder = 24.0f;
        //---------------------------------------------------------------------
        private const double _CornerValuePiDivide180 = 0.017453292519943295769236907684886;
        //---------------------------------------------------------------------
        private TimeSpan _timeValue;
        //---------------------------------------------------------------------
        private void _drawClock(Graphics e)
        {
            //-----------------------------------------------------------------
            e.SmoothingMode = SmoothingMode.AntiAlias;
            e.TextRenderingHint = TextRenderingHint.AntiAlias;
            e.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //-----------------------------------------------------------------
            var midX = (float)Width / 2;
            var midY = midX;
            var radius = 3 * midX / 4;
            //-----------------------------------------------------------------
            //Вспомогательные линии
            //e.DrawLine(new Pen(Color.Black, 1), 0, midX, this.Width, midY);
            //e.DrawLine(new Pen(Color.Black, 1), midX, 0, midY, this.Height);
            //-----------------------------------------------------------------
            var speaceMainRect = midX - radius - 1;
            var mainRect = new RectangleF(speaceMainRect, speaceMainRect, 2 * radius + 2, 2 * radius + 2);
            using (var lineBrush = new LinearGradientBrush(mainRect, Color.White, Color.FromArgb(8, Color.Yellow), LinearGradientMode.Vertical))
            {
                using (var brush = new LinearGradientBrush(mainRect, Color.White, Color.FromArgb(180, Color.Gray), LinearGradientMode.Vertical))
                {
                    e.FillEllipse(brush, mainRect);
                    e.FillEllipse(lineBrush, mainRect);
                }
            }
            //-----------------------------------------------------------------
            var speaceMountRect = speaceMainRect - _WidthMouth / 2;
            var mountRect1 = new RectangleF(speaceMountRect + 2, speaceMountRect + 1.5f, Width - 2 * speaceMountRect, Width - 2 * speaceMountRect);
            e.DrawEllipse(new Pen(Color.FromArgb(60, Color.Black), 7), mountRect1);

            var mountRect = new RectangleF(speaceMountRect, speaceMountRect, Width - 2 * speaceMountRect, Width - 2 * speaceMountRect);
            e.DrawEllipse(new Pen(Color.FromArgb(190, Color.Black), _WidthMouth), mountRect);
            //-----------------------------------------------------------------
            e.TranslateTransform(midX - 1, midY - 1);

            //Рисование циферблата
            using (var format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                for (int pointCounter = 0; pointCounter < 60; pointCounter++)
                {
                    var startPoint = _CreatePointByDeg(radius - 2, pointCounter * 6 + 90);
                    if (pointCounter % 5 != 0)
                    {
                        e.FillEllipse(new SolidBrush(Color.Black), startPoint.X, startPoint.Y, 2, 2);
                    }
                    else
                    {
                        var number = pointCounter / 5;

                        if (number == 0)
                        { number = 12; }

                        var radBuf = (number < 8) ? radius - 3 : radius - 1.2f;
                        startPoint = _CreatePointByDeg(radBuf, pointCounter * 6 + 90);
                        var endPoint = _CreatePointByDeg(radius + 2, pointCounter * 6 + 90);
                        startPoint.X += 1.5f;
                        startPoint.Y += 1.5f;
                        endPoint.X += 1.5f;
                        endPoint.Y += 1.5f;
                        using (var ta = new Pen(Color.Black, 3.0f))
                        {
                            ta.StartCap = LineCap.Triangle;
                            ta.EndCap = LineCap.Round;
                            e.DrawLine(ta, endPoint, startPoint);
                        }
                        
                        var strPoint = _CreatePointByDeg(radius - 2 - Font.Size, pointCounter * 6 + 90);
                        e.DrawString(number.ToString(), Font, new SolidBrush(Color.FromArgb(_ShadeAlphaColor, _ShadeColor)), strPoint.X + 3, strPoint.Y + 3, format);
                        e.DrawString(number.ToString(), Font, new SolidBrush(Color.Black), strPoint.X + 2, strPoint.Y + 2, format);
                    }
                }
            }

            //-----------------------------------------------------------------
            var currentMinute = _timeValue.Minutes;
            var currentHour = _timeValue.Hours;
            var currentSecond = _timeValue.Seconds;

            //Рисуем часовую стрелку
            var hourAngle = 2.0 * Math.PI * (currentHour + currentMinute / 60.0) / 12.0;
            _ShowHand(e, _hourPen, radius - _HourHandIndentBorder, hourAngle);

            //Рисуем минутную стрелку
            var minuteAngle = 2.0 * Math.PI * (currentMinute + currentSecond / 60.0) / 60.0;
            _ShowHand(e, _minutePen, radius - _MinuteHandIndentBorder, minuteAngle);
            
            //Рисуем секундную стрелку
            var secondAngle = 2.0 * Math.PI * currentSecond / 60.0;
            _ShowHand(e, _secondPen, radius - _SecondHandIndentBorder, secondAngle);
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        private static void _ShowHand(Graphics graph, Pen pen, double lengthHand, double angle)
        {
            pen.StartCap = LineCap.RoundAnchor;
            pen.EndCap = LineCap.Triangle;
            //-----------------------------------------------------------------
            var endPointHour = _CreatePointByAngle(lengthHand, angle);
            //-----------------------------------------------------------------
            graph.DrawLine(new Pen(Color.FromArgb(_ShadeAlphaColor, _ShadeColor)), 2, 2, endPointHour.X + 1, endPointHour.Y + 1);
            graph.DrawLine(pen, new PointF(0, 0), endPointHour);
        }
        //---------------------------------------------------------------------
        private static PointF _CreatePointByAngle(double radius, double angle)
        {
            var x = radius * Math.Sin(angle);
            var y = -radius * Math.Cos(angle);

            return new PointF((float)x, (float)y);
        }
        private static PointF _CreatePointByDeg(float radius, double deg)
        {
            var angle = _CornerValuePiDivide180 * deg;

            var x = (float)(radius * Math.Cos(angle));
            var y = (float)(radius * Math.Sin(angle));

            return new PointF(-x, -y);
        }
        #endregion
        #region protected
        protected override void OnPaint(PaintEventArgs e)
        {
            _drawClock(e.Graphics);
        }
        protected override void OnResize(EventArgs e)
        {
            Size = new Size(Height, Height);
            Invalidate();
        }
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    _secondPen?.Dispose();
                    _minutePen?.Dispose();
                    _hourPen?.Dispose();
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        public AnalogClockControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            UpdateStyles();

            Font = new Font("Arial", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Name = "AnalogClockControl";
            Size = new Size(232, 232);
        }

        //---------------------------------------------------------------------
        [DefaultValue(typeof(TimeSpan), "00:00:00")]
        public TimeSpan Time
        {
            get { return _timeValue; }
            set
            {
                _timeValue = value; 
                Invalidate();
            }
        }
        //---------------------------------------------------------------------
        [DefaultValue(typeof(Font), "Arial, 14.25pt, style=Bold, unit=Point, gdiCharSet=0")]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }
        //---------------------------------------------------------------------
    }
}
