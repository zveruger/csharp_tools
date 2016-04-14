using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Tools.CSharp.Controls.Extensions;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Controls
{
    //-------------------------------------------------------------------------
    [Designer(typeof(WatermarkFrezableTextComboBoxDesigner))]
    public class WatermarkFrezableTextComboBox : FrezableTextComboBox
    {
        #region private
        //http://www.ageektrapped.com/blog/the-missing-net-1-cue-banners-in-windows-forms-em_setcuebanner-text-prompt/
        private const int EM_SETCUEBANNER = 0x1501;
        private const int EM_GETCUEBANNER = 0x1502;
        //---------------------------------------------------------------------
        private string _watermarkText;
        //---------------------------------------------------------------------
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hwnd, int msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll")]
        private static extern bool GetComboBoxInfo(IntPtr hwnd, ref COMBOBOXINFO pcbi);

        [StructLayout(LayoutKind.Sequential)]
        private struct COMBOBOXINFO
        {
            public int cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public IntPtr stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndItem;
            public IntPtr hwndList;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        //---------------------------------------------------------------------
        private static void SetCueText(Control control, string text)
        {
            if (control is ComboBox)
            {
                var info = GetComboBoxInfo(control);
                SendMessage(info.hwndItem, EM_SETCUEBANNER, 1, text);
            }
            else
            {
                SendMessage(control.Handle, EM_SETCUEBANNER, 1, text);
            }
        }
        private static string GetCueText(Control control)
        {
            var builder = new StringBuilder();
            if (control is ComboBox)
            {
                var info = new COMBOBOXINFO();
                info.cbSize = Marshal.SizeOf(info);
                GetComboBoxInfo(control.Handle, ref info);
                SendMessage(info.hwndItem, EM_GETCUEBANNER, 0, builder);
            }
            else
            {
                SendMessage(control.Handle, EM_GETCUEBANNER, 0, builder);
            }
            return builder.ToString();
        }
        private static COMBOBOXINFO GetComboBoxInfo(Control control)
        {
            var info = new COMBOBOXINFO();
            info.cbSize = Marshal.SizeOf(info);
            GetComboBoxInfo(control.Handle, ref info);
            return info;
        }
        #endregion
        //---------------------------------------------------------------------
        [Browsable(true), DefaultValue(null)]
        public string WatermarkText
        {
            get { return _watermarkText; }
            set
            {
                _watermarkText = value;
                SetCueText(this, _watermarkText);
            }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class WatermarkFrezableTextComboBoxDesigner : ControlDesigner
    {
        #region private
        private DesignerActionListCollection _actionList;
        #endregion
        //---------------------------------------------------------------------
        public override DesignerActionListCollection ActionLists
        {
            get { return LazyInitializer.EnsureInitialized(ref _actionList, () => new DesignerActionListCollection { new DesignerWatermarkFrezableTextComboBoxActionList(Control) }); }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    internal sealed class DesignerWatermarkFrezableTextComboBoxActionList : DesignerActionList
    {
        #region private
        private readonly WatermarkFrezableTextComboBox _control;
        private readonly DesignerActionUIService _uiService;
        #endregion
        public DesignerWatermarkFrezableTextComboBoxActionList(IComponent component)
            : base(component)
        {
            if (!(component is WatermarkFrezableTextComboBox))
            { throw new ArgumentNullException(nameof(component)); }

            _control = (WatermarkFrezableTextComboBox)component;
            _uiService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
        }

        //---------------------------------------------------------------------
        public string WatermarkText
        {
            get { return _control.WatermarkText; }
            set
            {
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.Text)).SetValue(_control, string.Empty);
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.WatermarkText)).SetValue(_control, value);
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
                nameof(WatermarkText),
                _control.GetPropertyName(ExpressionExtensions.LastPartNameOf(() => _control.WatermarkText)),
                actionPropertiesHeader.Category
            );
            //-----------------------------------------------------------------
            return items;
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}
