using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Tools.CSharp.Controls
{
    public class TransparentLabel : Label
    {
        public TransparentLabel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.DoubleBuffer, true);
            UpdateStyles();

            base.BackColor = Color.Transparent;
            Font = new Font("Arial", 9F);
        }

        //---------------------------------------------------------------------
        [Browsable(false), DefaultValue(typeof(Color), "Transparent")]
        public new Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
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
}
