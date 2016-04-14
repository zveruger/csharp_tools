using System;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.AnalyzerLines
{
    public sealed class StringUnit : BaseValueUnit<StringUnit>, IStringUnit
    {
        #region private
        private readonly string _originalValue;
        #endregion
        public StringUnit(string originalValue, bool decodeLineIgnoreCaseSymbols = false)
            : base(decodeLineIgnoreCaseSymbols, originalValue)
        {
            if (string.IsNullOrEmpty(originalValue))
            { throw new ArgumentNullException(nameof(originalValue)); }

            _originalValue = originalValue;
        }

        //---------------------------------------------------------------------
        public string OriginalValue
        {
            get { return _originalValue; }
        }
        //---------------------------------------------------------------------
    }
}