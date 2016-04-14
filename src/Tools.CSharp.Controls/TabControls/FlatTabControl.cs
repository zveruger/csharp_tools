using System;
using System.Collections.Generic;
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
    //[Designer(typeof(FlatTabControlDesigner))]
    public class FlatTabControl : TabControl
    {
        #region private
        private readonly StringFormat _stringFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap,
            Trimming = StringTrimming.EllipsisCharacter
        };
        private readonly Color _colorContoure = SystemColors.ControlDarkDark;
        private readonly Dictionary<int, FlatTabPage> _cacheOriginalTabPages = new Dictionary<int, FlatTabPage>();
        private readonly Dictionary<int, FlatTabPage> _cacheDrawOriginalTabPages = new Dictionary<int, FlatTabPage>();
        private readonly Rectangle _additionSelectedTabPageRectangle = new Rectangle(1, 2, 7, -1);
        private readonly Rectangle _additionTabPageRectangle = new Rectangle(8, 8, -4, -7);
        private readonly Rectangle _defaultRectangle = default(Rectangle);
        //-------------------------------------------------------------------------
        private bool _isShowOriginalTabPage;
        private bool _isSelectedIndexFreeze;
        private int _radiusCorner = 6;
        private Color _backColor = Color.FromArgb(224,224,224);
        //-------------------------------------------------------------------------
        private static Rectangle _rectangleNewSize(Rectangle rectangle, Rectangle additionRectangle)
        {
            return new Rectangle(rectangle.X + additionRectangle.X,
                rectangle.Y + additionRectangle.Y,
                rectangle.Width + additionRectangle.Width,
                rectangle.Height + additionRectangle.Height
                );
        }
        private static GraphicsPath _createGraphicsPathByRectangle(RectangleF rect, float radiusCorner)
        {
            var widthRect = rect.Width;
            var heightRect = rect.Height;
            var graphObject = new GraphicsPath();

            graphObject.StartFigure();
            graphObject.AddArc(0, 0, radiusCorner, radiusCorner, 180, 90);
            graphObject.AddArc(0 + widthRect - radiusCorner, 0, radiusCorner, radiusCorner, 270, 90);
            graphObject.AddLine(widthRect, heightRect, 0, heightRect);
            graphObject.CloseFigure();

            return graphObject;
        }
        //-------------------------------------------------------------------------
        private void _drawTab(Graphics graphics, FlatTabPage tabPage, Rectangle tabPageRectangle)
        {
            var newRadiusCorner = tabPageRectangle.Height/((_radiusCorner == 0) ? 1.0f : _radiusCorner);

            using (var graphicsPath = _createGraphicsPathByRectangle(tabPageRectangle, newRadiusCorner))
            {
                using (var brush = new LinearGradientBrush(
                    tabPageRectangle, 
                    Color.FromArgb(tabPage.StartGradientAlpha, tabPage.StartGradientColor), 
                    Color.FromArgb(tabPage.EndGradientAlpha, tabPage.EndGradientColor), 
                    tabPage.AngleGradient
                ))
                {
                    brush.WrapMode = WrapMode.TileFlipXY;
                    graphics.FillPathByTransform(brush, graphicsPath, tabPageRectangle.X, tabPageRectangle.Y);
                }

                using (var pen = new Pen(_colorContoure))
                { graphics.DrawPathByTransform(pen, graphicsPath, tabPageRectangle.X, tabPageRectangle.Y); }
            }

            using (var brush = new SolidBrush(tabPage.ColorText))
            { graphics.DrawString(tabPage.Text, Font, brush, new RectangleF(tabPageRectangle.X, tabPageRectangle.Y, tabPageRectangle.Width, tabPageRectangle.Height - (SelectedIndex == tabPage.Index ? 4 : 0)), _stringFormat); }
        }
        //-------------------------------------------------------------------------
        private int _selectedOriginalIndex
        {
            get
            {
                if (SelectedIndex == -1 || _isShowOriginalTabPage)
                { return SelectedIndex; }

                var flatTabPage = TabPages[SelectedIndex] as FlatTabPage;
                return flatTabPage?.OriginalIndex ?? SelectedIndex;
            }
            set { SelectedIndex = _isShowOriginalTabPage ? value : _getTabPageIndexByOriginalIndex(value); }
        }
        private int _getTabPageIndexByOriginalIndex(int originalIndex)
        {
            if (originalIndex != -1)
            {
                if (_isShowOriginalTabPage)
                { return originalIndex; }

                if (_cacheDrawOriginalTabPages.ContainsKey(originalIndex))
                { return _cacheDrawOriginalTabPages[originalIndex].Index; }
            }
            return -1;
        }
        private int _getOriginalIndexByTabPageIndex(int tabPageIndex)
        {
            if (tabPageIndex != -1)
            {
                if (_isShowOriginalTabPage)
                { return tabPageIndex; }

                if (tabPageIndex > -1 && tabPageIndex < TabCount)
                {
                    var flatTabPage = TabPages[tabPageIndex] as FlatTabPage;
                    if (flatTabPage != null)
                    { return flatTabPage.OriginalIndex; }
                }
            }
            return -1;
        }
        //-------------------------------------------------------------------------
        private void _updateCacheOriginalTabPages(bool updateCache)
        {
            if (updateCache)
            { _cacheOriginalTabPages.Clear(); }

            if (_cacheOriginalTabPages.Count == 0)
            {
                for (var i = 0; i < TabPages.Count; i++)
                {
                    var tabPage = (FlatTabPage)TabPages[i];
                    tabPage.OriginalIndex = i;
                    _cacheOriginalTabPages.Add(i, tabPage);
                }
            }
        }
        private void _showTabPagesByCacheOriginalTabPages(IEnumerable<int> tabPageIndexCollection, bool updateCache)
        {
            _updateCacheOriginalTabPages(updateCache);

            var selectedOriginalIndex = _selectedOriginalIndex;
            var isTabPageSelectedIndexContainsInShowTabPages = false;
            _isSelectedIndexFreeze = true;

            SuspendLayout();
            SelectedIndex = -1;
            _cacheDrawOriginalTabPages.Clear();

            if (!IsDisposed)
            { TabPages.Clear(); }

            foreach (var tabPageIndex in tabPageIndexCollection)
            {
                if (_cacheOriginalTabPages.ContainsKey(tabPageIndex))
                {
                    var tabPage = _cacheOriginalTabPages[tabPageIndex];
                    if (tabPage != null)
                    {
                        tabPage.Index = TabCount;
                        TabPages.Add(tabPage);
                        _cacheDrawOriginalTabPages.Add(tabPage.OriginalIndex, tabPage);

                        if (tabPage.OriginalIndex == selectedOriginalIndex)
                        {
                            isTabPageSelectedIndexContainsInShowTabPages = true;
                            selectedOriginalIndex = tabPage.Index;
                        }
                    }
                }
            }

            _isSelectedIndexFreeze = false;
            _isShowOriginalTabPage = _cacheOriginalTabPages.Count == _cacheDrawOriginalTabPages.Count;
            selectedOriginalIndex = isTabPageSelectedIndexContainsInShowTabPages ? selectedOriginalIndex : (TabCount == 0 ? -1 : 0);

            if (SelectedIndex == selectedOriginalIndex)
            { OnSelectedIndexChanged(EventArgs.Empty); }
            else
            { SelectedIndex = isTabPageSelectedIndexContainsInShowTabPages ? selectedOriginalIndex : (TabCount == 0 ? -1 : 0); }

            ResumeLayout();
        }
        //-------------------------------------------------------------------------
        private void _clearAllCache()
        {
            _cacheOriginalTabPages.Clear();
            _cacheDrawOriginalTabPages.Clear();
        }
        #endregion
        #region protected
        protected override void OnControlAdded(ControlEventArgs e)
        {
            var flatTabPage = e.Control as FlatTabPage;
            if (flatTabPage != null)
            {
                base.OnControlAdded(e);

                ((FlatTabPage)e.Control).Owner = this;
            }
            else
            { Controls.Remove(e.Control); }
        }
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (!_isSelectedIndexFreeze)
            {
                base.OnSelectedIndexChanged(e);
                Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_isSelectedIndexFreeze)
            { return; }

            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            using (var brush = new SolidBrush(BackColor))
            { graphics.FillRectangle(brush, e.ClipRectangle); }

            if (Visible && TabCount != 0)
            {
                var selectedTabPageIndex = SelectedIndex;
                var selectedTabPageRectangle = _defaultRectangle;

                for (var i = 0; i < TabPages.Count; i++)
                {
                    var flatTabPage = TabPages[i] as FlatTabPage;
                    if (flatTabPage != null)
                    {
                        var tabPageRectangle = GetTabRect(i);

                        if (selectedTabPageIndex == i)
                        { tabPageRectangle = selectedTabPageRectangle = _rectangleNewSize(tabPageRectangle, _additionSelectedTabPageRectangle); }
                        else
                        { tabPageRectangle = _rectangleNewSize(tabPageRectangle, _additionTabPageRectangle); }

                        _drawTab(graphics, flatTabPage, tabPageRectangle);
                    }
                }

                var displayRectangle = DisplayRectangle;
                using (var pen = new Pen(_colorContoure))
                {
                    displayRectangle.X--;
                    displayRectangle.Y--;
                    displayRectangle.Width++;
                    displayRectangle.Height++;
                    graphics.DrawRectangle(pen, displayRectangle);
                }
                if (selectedTabPageRectangle != _defaultRectangle)
                {
                    graphics.DrawLine(new Pen(BackColor, 1f), 
                        selectedTabPageRectangle.X + 1,
                        selectedTabPageRectangle.Y + selectedTabPageRectangle.Height,
                        selectedTabPageRectangle.X + selectedTabPageRectangle.Width,
                        selectedTabPageRectangle.Y + selectedTabPageRectangle.Height
                    );
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                try
                {
                    if (disposing)
                    {
                        _stringFormat?.Dispose();

                        _clearAllCache();
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }
        #endregion
        public FlatTabControl()
        {
            SetStyle(
                ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint,
                true
            );
            UpdateStyles();
            Padding = new Point(12, 8);
        }

        //---------------------------------------------------------------------
        [Browsable(true), Category("Main"), Description("Угловой радиус страниц"), DefaultValue(6)]
        public int RadiusCorner
        {
            get { return _radiusCorner; }
            set
            {
                if (_radiusCorner != value)
                {
                    _radiusCorner = value;
                    Invalidate();
                }
            }
        }
        //---------------------------------------------------------------------
        [Browsable(true), DefaultValue(typeof(Color), "224,224,224")]
        public new Color BackColor
        {
            get { return _backColor; }
            set
            {
                if (!value.Equals(Color.Empty) && !GetStyle(ControlStyles.SupportsTransparentBackColor) && value.A < 255)
                    throw new ArgumentException(nameof(value));

                if (!_backColor.Equals(BackColor))
                {
                    _backColor = value;
                    OnBackColorChanged(EventArgs.Empty);
                }


            }
        }
        //---------------------------------------------------------------------
        [Browsable(true), DefaultValue(typeof(Point), "12, 8")]
        public new Point Padding
        {
            get { return base.Padding; }
            set { base.Padding = value; }
        }
        //---------------------------------------------------------------------
        public int SelectedOriginalIndex
        {
            get { return _selectedOriginalIndex; }
            set { _selectedOriginalIndex = value; }
        }
        public int GetTabPageIndexByOriginalIndex(int originalIndex)
        {
            return _getTabPageIndexByOriginalIndex(originalIndex);
        }
        public int GetOriginalIndexByTabPageIndex(int tabPageIndex)
        {
            return _getOriginalIndexByTabPageIndex(tabPageIndex);
        }
        //---------------------------------------------------------------------
        public void TabPagesClear()
        {
            SuspendLayout();
            UpdateDrawTabPages(false);
            _clearAllCache();
            TabPages.Clear();
            ResumeLayout();
        }
        //---------------------------------------------------------------------
        public void UpdateDrawTabPages(bool drawAllTabPages)
        {
            UpdateDrawTabPages(drawAllTabPages, false);
        }
        public void UpdateDrawTabPages(bool drawAllTabPages, bool updateCache)
        {
            if (drawAllTabPages)
            { _showTabPagesByCacheOriginalTabPages(_cacheOriginalTabPages.Keys, updateCache); }
            else
            { _showTabPagesByCacheOriginalTabPages(new int[0], updateCache); }
        }
        public void UpdateDrawTabPages(ICollection<int> tabPageOriginalIndexCollection)
        {
            UpdateDrawTabPages(tabPageOriginalIndexCollection, false);
        }
        public void UpdateDrawTabPages(ICollection<int> tabPageOriginalIndexCollection, bool updateCache)
        {
            if (tabPageOriginalIndexCollection == null)
            { throw new ArgumentNullException(nameof(tabPageOriginalIndexCollection)); }

            _showTabPagesByCacheOriginalTabPages(tabPageOriginalIndexCollection, updateCache);
        }
        //---------------------------------------------------------------------
        [Browsable(true)]
        public new event EventHandler BackColorChanged
        {
            add
            {
                base.BackColorChanged += value;
            }
            remove
            {
                base.BackColorChanged -= value;
            }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class FlatTabControlDesigner : ParentControlDesigner
    {
        #region private
        private DesignerActionListCollection _actionList;



       

        #endregion
        //---------------------------------------------------------------------
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _actionList,
                    () => new DesignerActionListCollection {new DesignerFlatTabControlActionList(Control)});
            }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class DesignerFlatTabControlActionList : DesignerActionList, ITypeDescriptorContext
    {
        #region private
        private readonly FlatTabControl _control;
        private readonly DesignerActionUIService _uiService;
        private readonly IDesignerHost _designerHost;
        private readonly IReferenceService _referenceService;
        private readonly PropertyDescriptor _tabPagesPropertyDescriptor;
        private readonly IComponentChangeService _componentChangeService;
        //---------------------------------------------------------------------
        private string _generateNewTabPageName()
        {
            string tabPageName;
            var tabPageIndex = 0;
            bool isDublicateTabPageName;
            
            do
            {
                ++tabPageIndex;
                tabPageName = $"tabPage{tabPageIndex.ToString()}";
                isDublicateTabPageName = _referenceService.GetReference(tabPageName) != null;
            }
            while (isDublicateTabPageName);

            return tabPageName;
        }
        #endregion
        public DesignerFlatTabControlActionList(IComponent component)
            : base(component)
        {
            if (!(component is FlatTabControl))
            { throw new ArgumentNullException(nameof(component)); }

            _control = (FlatTabControl) component;
            _uiService = (DesignerActionUIService)GetService(typeof (DesignerActionUIService));
            _referenceService = (IReferenceService)GetService(typeof(IReferenceService));
            _tabPagesPropertyDescriptor = _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.TabPages));
            _designerHost = (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
            _componentChangeService = (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
        }

        //---------------------------------------------------------------------
        public int RadiusCorner
        {
            get { return _control.RadiusCorner; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.RadiusCorner)).SetValue(_control, value);
                _uiService.Refresh(_control);
            }
        }
        //---------------------------------------------------------------------
        public void AddNewTab()
        {
            var newNameTabPage = _generateNewTabPageName();

            var tabPage = (FlatTabPage)_designerHost.CreateComponent(typeof(FlatTabPage), newNameTabPage);
            tabPage.Text = newNameTabPage;
            _control.TabPages.Add(tabPage);
        }
        //---------------------------------------------------------------------
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var actionPropertiesHeader = new DesignerActionHeaderItem("Свойства", "Properties");
            var items = new DesignerActionItemCollection {actionPropertiesHeader};
            //-----------------------------------------------------------------
            items.AddActionPropertyItem(
                nameof(RadiusCorner),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.RadiusCorner)),
                actionPropertiesHeader.Category
            );
            //-----------------------------------------------------------------
            var actionMethodsHeader = new DesignerActionHeaderItem("Методы", "Methods");
            items.Add(actionMethodsHeader);
            //-------------------------------------------------------------
            items.Add(new DesignerActionMethodItem(
                this,
                nameof(AddNewTab),
                "Добавить новую страницу",
                actionMethodsHeader.Category
            ));
            return items;
        }
        //---------------------------------------------------------------------
        IContainer ITypeDescriptorContext.Container
        {
            get { return Component.Site.Container; }
        }
        object ITypeDescriptorContext.Instance
        {
            get { return Component; }
        }
        void ITypeDescriptorContext.OnComponentChanged()
        {
            object newValue = _control.TabPages;
            object oldValue = _control.TabPages;

            _componentChangeService.OnComponentChanged(Component, _tabPagesPropertyDescriptor, oldValue, newValue);
        }
        bool ITypeDescriptorContext.OnComponentChanging()
        {
            _componentChangeService.OnComponentChanging(Component, _tabPagesPropertyDescriptor);
            return true;
        }
        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
        {
            get { return _tabPagesPropertyDescriptor; }
        }
    }
    //-------------------------------------------------------------------------
}