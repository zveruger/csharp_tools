using System.Windows.Forms;

namespace Tools.CSharp.Controls.Extensions
{
    public static class ClipboardExtension
    {
        //---------------------------------------------------------------------
        public static bool GetTextByUnicode(out string text)
        {
            var result = false;
            if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
            {
                text = Clipboard.GetText(TextDataFormat.UnicodeText);
                result = true;
            }
            else
            { text = string.Empty; }

            return result;
        }
        public static void SetTextByUnicode(string text)
        {
            Clipboard.Clear();
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }
        //---------------------------------------------------------------------
    }
}
