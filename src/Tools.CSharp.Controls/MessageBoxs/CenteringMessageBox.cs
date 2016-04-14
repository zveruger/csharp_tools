using System.Windows.Forms;

namespace Tools.CSharp.Controls
{
    public sealed class CenteringMessageBox
    {
        private CenteringMessageBox()
        { }

        //---------------------------------------------------------------------
        public static DialogResult Show(string text)
        {
            return MessageBox.Show(text);
        }

        public static DialogResult Show(IWin32Window owner, string text)
        {
            using (new DialogCenteringService(owner))
            {
                return MessageBox.Show(owner, text);
            }
        }
        //---------------------------------------------------------------------
        public static DialogResult Show(string text, string caption)
        {
            return MessageBox.Show(text, caption);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            using (new DialogCenteringService(owner))
            {
                return MessageBox.Show(owner, text, caption);
            }
        }
        //---------------------------------------------------------------------
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return MessageBox.Show(text, caption, buttons);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            using (new DialogCenteringService(owner))
            {
                return MessageBox.Show(owner, text, caption, buttons);
            }
        }
        //---------------------------------------------------------------------
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(text, caption, buttons, icon);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon)
        {
            using (new DialogCenteringService(owner))
            {
                return MessageBox.Show(owner, text, caption, buttons, icon);
            }
        }
        //---------------------------------------------------------------------
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            using (new DialogCenteringService(owner))
            {
                return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton);
            }
        }
        //---------------------------------------------------------------------
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        {
            using (new DialogCenteringService(owner))
            {
                return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, options);
            }
        }
        //---------------------------------------------------------------------
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options, helpFilePath);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
        {
            using (new DialogCenteringService(owner))
            {
                return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath);
            }
        }
        //---------------------------------------------------------------------
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options, displayHelpButton);
        }
        //---------------------------------------------------------------------
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options, helpFilePath, navigator);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath,
            HelpNavigator navigator)
        {
            using (new DialogCenteringService(owner))
            {
                return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, navigator);
            }
        }
        //---------------------------------------------------------------------
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options, helpFilePath, keyword);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath,
            string keyword)
        {
            using (new DialogCenteringService(owner))
            {
                return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, keyword);
            }
        }
        //---------------------------------------------------------------------
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath,
            HelpNavigator navigator, object param)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options, helpFilePath, navigator, param);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath,
            HelpNavigator navigator, object param)
        {
            using (new DialogCenteringService(owner))
            {
                return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, navigator, param);
            }
        }
        //---------------------------------------------------------------------
    }
}