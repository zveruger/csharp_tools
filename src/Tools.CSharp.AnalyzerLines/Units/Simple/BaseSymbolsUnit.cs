using System;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.AnalyzerLines
{
    public abstract class BaseSymbolsUnit<TSymbolsUnit> : Unit<TSymbolsUnit>, ISymbolsUnit 
        where TSymbolsUnit : BaseSymbolsUnit<TSymbolsUnit>
    {
        #region private
        private readonly char[] _symbols;
        private readonly int _count;
        #endregion
        #region protected
        protected virtual char GetSymbol(int index)
        {
            return _symbols[index];
        }
        protected virtual int GetCount()
        {
            return _count;
        }

        protected override UnitError DecodeLine(string line, ref int position, Func<char, bool> isIgnoreSymbol)
        {
            UnitError error = null;
            var savePosition = position;
            var valueSymbolPosition = 0;
            var ignoreCase = DecodeLineIgnoreCaseSymbols;

            if (_count != 0)
            {
                for (; position < line.Length; position++)
                {
                    var symbolLine = line[position];

                    if (symbolLine.EqualsIgnoreCase(_symbols[valueSymbolPosition], ignoreCase))
                    { ++valueSymbolPosition; }
                    else
                    {
                        var isError = !(valueSymbolPosition == 0 && isIgnoreSymbol(symbolLine));
                        if (isError)
                        { break; }
                    }

                    if (valueSymbolPosition == _count)
                    {
                        position++;
                        break;
                    }
                }
            }
            
            if (valueSymbolPosition != _count)
            {
                error = CreateError(line, position, this, valueSymbolPosition);
                position = savePosition;
            }

            return error;
        }
        protected override int GetLengthUnit()
        {
            return _count;
        }
        protected override bool IsFirstSymbolUnit(char symbol)
        {
            return _symbols[0].EqualsIgnoreCase(symbol, DecodeLineIgnoreCaseSymbols);
        }
        #endregion
        protected BaseSymbolsUnit(bool decodeLineIgnoreCaseSymbols)
            : this(decodeLineIgnoreCaseSymbols, new char[0])
        { }
        protected BaseSymbolsUnit(bool decodeLineIgnoreCaseSymbols, string str)
            : this(decodeLineIgnoreCaseSymbols, str?.ToCharArray() ?? new char[0])
        {
            if (string.IsNullOrEmpty(str))
            { throw new ArgumentNullException(nameof(str)); }
        }
        protected BaseSymbolsUnit(bool decodeLineIgnoreCaseSymbols, params char[] symbols)
            : base(decodeLineIgnoreCaseSymbols)
        {
            if (symbols == null)
            { throw new ArgumentNullException(nameof(symbols)); }

            _symbols = symbols;
            _count = symbols.Length;
        }

        //---------------------------------------------------------------------
        public char this[int index]
        {
            get { return GetSymbol(index); }
        }
        public int Count
        {
            get { return GetCount(); }
        }
        //---------------------------------------------------------------------
    }
}