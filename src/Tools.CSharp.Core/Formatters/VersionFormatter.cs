using System;

namespace Tools.CSharp.Formatters
{
    public sealed class VersionFormatter : IFormatProvider, ICustomFormatter
    {
        #region private
        private readonly int _fieldCount;
        #endregion
        public VersionFormatter(int fieldCount)
        {
            if (fieldCount < 0)
            { throw new ArgumentOutOfRangeException(nameof(fieldCount)); }

            _fieldCount = fieldCount;
        }

        //---------------------------------------------------------------------
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }
        //---------------------------------------------------------------------
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var version = arg as Version;
            if (version != null)
            { return version.ToString(_fieldCount); }

            var formatter = arg as IFormattable;
            if (formatter != null)
            { return formatter.ToString(format, formatProvider); }

            return arg.ToString();
        }
        //---------------------------------------------------------------------
    }
}