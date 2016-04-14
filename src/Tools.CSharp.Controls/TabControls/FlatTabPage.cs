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
    //[Designer(typeof(FlatTabPageDesigner))]
    public class FlatTabPage : TabPage
    {
        #region private
        private Color _startGradientColor = Color.White;
        private int _startGradientAlpha = 255;
        private Color _endGradientColor = Color.LightGray;
        private int _endGradientAlpha = 255;
        private float _angleGradient = 90.0f;
        //---------------------------------------------------------------------
        private Color _colorText = Color.Black;
        //---------------------------------------------------------------------
        private FlatTabControl _owner;
        private int _originalIndex = -1;
        private int _index = -1;
        //---------------------------------------------------------------------
        private void _invalidate()
        {
            _owner?.Invalidate();
        }
        #endregion
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
                    _invalidate();
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
                    _invalidate();
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
                    _invalidate();
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
                    _invalidate();
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
                    _invalidate();
                }
            }
        }
        //---------------------------------------------------------------------
        [Category("Text"), Description("Цвет текста"), DefaultValue(typeof(Color), "Black")]
        public Color ColorText
        {
            get { return _colorText; }
            set
            {
                if (_colorText != value)
                {
                    _colorText = value;
                    _invalidate();
                }
            }
        }
        //---------------------------------------------------------------------
        [Browsable(false), DefaultValue(null)]
        public FlatTabControl Owner
        {
            get { return _owner; }
            internal set { _owner = value; }
        
        }
        //---------------------------------------------------------------------
        [Browsable(false), DefaultValue(-1)]
        public int OriginalIndex
        {
            get { return _originalIndex; }
            internal set { _originalIndex = value; }
        }

        [Browsable(false), DefaultValue(-1)]
        public int Index
        {
            get { return _index; }
            internal set { _index = value; }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class FlatTabPageDesigner : ControlDesigner
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
            get { return LazyInitializer.EnsureInitialized(ref _actionList, () => new DesignerActionListCollection { new DesignerFlatTabPageActionList(Control) }); }
        }
        public override DesignerVerbCollection Verbs
        {
            get { return LazyInitializer.EnsureInitialized(ref _verbs, () => new DesignerFlatTabPageVerbCollection(Control)); }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class DesignerFlatTabPageActionList : DesignerActionList
    {
        #region private
        private readonly FlatTabPage _control;
        private readonly DesignerActionUIService _uiService;
        #endregion
        public DesignerFlatTabPageActionList(IComponent component)
            : base(component)
        {
            if (!(component is FlatTabPage))
            { throw new ArgumentNullException(nameof(component)); }

            _control = (FlatTabPage)component;
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
    internal sealed class DesignerFlatTabPageVerbCollection : DesignerVerbCollection
    {
        #region private
        private readonly FlatTabPage _control;
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
        public DesignerFlatTabPageVerbCollection(IComponent component)
        {
            if (!(component is FlatTabPage))
            { throw new ArgumentNullException(nameof(component)); }

            _control = (FlatTabPage)component;

            Add(new DesignerVerb("Инвертировать цвета градиента", (o, e) => _invertGradientColor()));
        }
    }
    //-------------------------------------------------------------------------
}