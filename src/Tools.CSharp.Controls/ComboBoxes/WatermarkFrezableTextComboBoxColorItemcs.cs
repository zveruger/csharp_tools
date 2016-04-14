using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Tools.CSharp.Controls
{
    public class WatermarkFrezableTextComboBoxColorItem : WatermarkFrezableTextComboBox
    {
        #region private
        private int _minItemCountDrawEvenRow = 3;
        private Color _selectedRowColor = Color.LightGray;
        private Color _oddRowColor = Color.White;
        private Color _evenRowColor = Color.Linen;
        private Color _disableColor = SystemColors.Control;
        private Color _disableForeColor = Color.Gray;
        #endregion
        #region protected
        protected virtual void DrawUserItem(DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                Color color = _oddRowColor;
                bool disableControl = false;
                e.DrawBackground();

                if ((e.State == DrawItemState.Disabled) || ((e.State & DrawItemState.Disabled) != 0))
                {
                    color = _disableColor;
                    disableControl = true;
                }
                else if ((e.State == DrawItemState.ComboBoxEdit) || ((e.State & DrawItemState.ComboBoxEdit) != 0))
                {
                    color = _oddRowColor;
                }
                else if ((e.State & DrawItemState.Selected) != 0)
                {
                    color = _selectedRowColor;
                }
                else if (_minItemCountDrawEvenRow < Items.Count)
                {
                    if ((e.Index % 2) != 0)
                        color = _evenRowColor;
                }
                e.Graphics.FillRectangle(new SolidBrush(color), e.Bounds);

                e.Graphics.DrawString(Items[e.Index].ToString(), Font, new SolidBrush(disableControl ? _disableForeColor : ForeColor), e.Bounds.X, e.Bounds.Y);
                e.DrawFocusRectangle();
            }
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            DrawUserItem(e);
        }
        #endregion
        public WatermarkFrezableTextComboBoxColorItem()
        {
            DrawMode = DrawMode.OwnerDrawVariable;
        }

        //---------------------------------------------------------------------
        [Category("RowItem"), Description("Цвет выделенной строки"), DefaultValue(typeof(Color), "LightGray")]
        public Color SelectedRowColor
        {
            get { return _selectedRowColor; }
            set { _selectedRowColor = value; }
        }
        //---------------------------------------------------------------------
        [Category("RowItem"), Description("Цвет нечетной строки"), DefaultValue(typeof(Color), "White")]
        public Color OddRowColor
        {
            get { return _oddRowColor; }
            set { _oddRowColor = value; }
        }

        [Category("RowItem"), Description("Цвет четной строки"), DefaultValue(typeof(Color), "Linen")]
        public Color EvenRowColor
        {
            get { return _evenRowColor; }
            set { _evenRowColor = value; }
        }
        //---------------------------------------------------------------------
        [Category("RowItem"), Description("Цвет строки когда установлено свойство Disable"), DefaultValue(typeof(Color), "LightGray")]
        public Color DisableColor
        {
            get { return _disableColor; }
            set { _disableColor = value; }
        }

        [Category("RowItem"), Description("Цвет текста когда установлено свойство Disable"), DefaultValue(typeof(Color), "Gray")]
        public Color DisableForeColor
        {
            get { return _disableForeColor; }
            set { _disableForeColor = value; }
        }
        //---------------------------------------------------------------------
        [Category("RowItem"), DefaultValue(typeof(int), "3"), Description("Минимальное количество строк после которых происходит рисование цветных четных строк")]
        public int MinItemCountDrawEvenRow
        {
            get { return _minItemCountDrawEvenRow; }
            protected set { _minItemCountDrawEvenRow = value; }
        }
        //---------------------------------------------------------------------
        [DefaultValue(typeof(DrawMode), "OwnerDrawVariable")]
        public new DrawMode DrawMode
        {
            get { return base.DrawMode; }
            set { base.DrawMode = value; }
        }
        //---------------------------------------------------------------------
    }
}
