using System;

namespace Tools.CSharp.AnalyzerLines
{
    public sealed class SymbolUnit : BaseValueUnit<SymbolUnit, char>, ISymbolUnit
    {
        #region private
        private const char _DefaultValue = Char.MinValue;
        //---------------------------------------------------------------------
        private readonly char _originalValue;
        //---------------------------------------------------------------------
        private char _value = _DefaultValue;
        #endregion
        #region protected
        protected override char GetValue()
        {
            return _value;
        }
        protected override void UpdateValue(char[] valueSymbols)
        {
            _value = valueSymbols.Length == 0 ? _DefaultValue : valueSymbols[0];
        }
        protected override char GetSymbol(int index)
        {
            return _value;
        }
        protected override int GetCount()
        {
            return 1;
        }
        #endregion
        public SymbolUnit(char originalValue, bool decodeLineIgnoreCaseSymbols = false)
            : base(decodeLineIgnoreCaseSymbols, originalValue)
        {
            _originalValue = originalValue;
        }

        //---------------------------------------------------------------------
        public char OriginalValue
        {
            get { return _originalValue; }
        }
        //---------------------------------------------------------------------
    }
}