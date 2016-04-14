using System;

namespace Tools.CSharp.AnalyzerLines
{
    public sealed class ValueUnit : BaseValueUnit<ValueUnit>
    {
        #region private
        private Unit _nextUnit;
        #endregion
        #region protected
        protected override UnitError DecodeLine(string line, ref int position, Func<char, bool> isIgnoreSymbol)
        {
            var startPosition = position;

            var endPosition = -1;
            var isNoStartPosition = true;

            for (; position < line.Length; position++)
            {
                var symbol = line[position];

                if (_nextUnit == null)
                {
                    if (isIgnoreSymbol(symbol))
                    { break; }
                }
                else
                {
                    if (_nextUnit.IsFirstSymbol(symbol))
                    { break; }

                    if (isIgnoreSymbol(symbol))
                    { continue; }
                }

                if (isNoStartPosition)
                {
                    isNoStartPosition = false;
                    startPosition = position;
                }

                endPosition = position;
            }

            var value = GetLineSymbols(line, startPosition, endPosition);
            Update(true, startPosition, value);
            return null;
        }
        protected override int GetLengthUnit()
        {
            return GetCount();
        }
        protected override bool IsFirstSymbolUnit(char symbol)
        {
            return false;
        }
        protected override void SetDecodeLineIgnoreCaseSymbols(bool ignoreCaseSymbols)
        { }
        #endregion
        public ValueUnit()
            : base(false)
        { }

        //---------------------------------------------------------------------
        public Unit NextUnit
        {
            get { return _nextUnit; }
            set { _nextUnit = value; }
        }
        //---------------------------------------------------------------------
    }
}