using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Tools.CSharp.Windows.Converters
{
    public abstract class ConverterMarkupExtension<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {
        #region private
        private static T _Converter;
        #endregion
        //---------------------------------------------------------------------
        public static T Converter
        {
            get { return _Converter ?? (_Converter = new T()); }
        }
        //---------------------------------------------------------------------
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Converter;
        }
        //---------------------------------------------------------------------
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        //---------------------------------------------------------------------
    }
}