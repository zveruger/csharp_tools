using System.Windows.Forms;

namespace Tools.CSharp.Controls.Extensions
{
    public static class ComboBoxExtensions
    {
        //---------------------------------------------------------------------
        public static bool CheckIndex(this ComboBox comboBox, int index)
        {
            return index > -1 && index < comboBox.Items.Count;
        }
        //---------------------------------------------------------------------
    }
}
