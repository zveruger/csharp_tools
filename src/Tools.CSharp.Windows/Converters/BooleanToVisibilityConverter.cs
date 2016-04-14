using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tools.CSharp.Windows.Converters
{
    [Localizability(LocalizationCategory.NeverLocalize)]
    [ValueConversion(typeof(Boolean), typeof(Visibility))]
    public sealed class BooleanToVisibilityConverter : ConverterMarkupExtension<BooleanToVisibilityConverter>
    {
        //---------------------------------------------------------------------
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is bool?)
            {
                var tmp = (bool?)value;
                bValue = tmp.HasValue ? tmp.Value : false;
            }

            bool isContrary;
            if (parameter != null && Boolean.TryParse(parameter.ToString(), out isContrary))
            {
                if (isContrary)
                { bValue = !bValue; }
            }

            return bValue ? Visibility.Visible : Visibility.Collapsed;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            { return (Visibility)value == Visibility.Visible; }
            return false;
        }
        //---------------------------------------------------------------------
    }
}