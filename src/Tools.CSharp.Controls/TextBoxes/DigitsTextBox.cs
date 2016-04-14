using System;
using System.Windows;
using System.Windows.Controls;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Controls
{
    public class DigitsTextBox : TextBox
    {
        #region private
        private static bool _AreAllValidDigitChars(string str)
        {
            /*bool result = true;
            if (str == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencySymbol |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.NegativeSign |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.NegativeInfinitySymbol |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.NumberGroupSeparator |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.PercentDecimalSeparator |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.PercentGroupSeparator |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.PercentSymbol |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.PerMilleSymbol |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.PositiveInfinitySymbol |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.PositiveSign)
                return result;

            result &&= str.IsDigit();
            return result; 
            */
            return str.IsDigit();
        }
        #endregion
        #region protected
        protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !_AreAllValidDigitChars(e.Text);
            base.OnPreviewTextInput(e);
        }
        #endregion
        public DigitsTextBox()
        {
            DataObject.AddPastingHandler(this, (o, e) =>
            {
                var isText = e.SourceDataObject.GetDataPresent(System.Windows.DataFormats.Text, true);
                if (isText)
                {
                    var text = e.SourceDataObject.GetData(DataFormats.Text) as string;
                    if (_AreAllValidDigitChars(text))
                    { return; }
                }
                e.CancelCommand();
            });
        }
    }
}
