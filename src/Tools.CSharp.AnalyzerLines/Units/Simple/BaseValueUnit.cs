using System;
using System.Diagnostics.CodeAnalysis;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.AnalyzerLines
{
    //-------------------------------------------------------------------------
    public abstract class BaseValueUnit<TValueUnit> : BaseValueUnit<TValueUnit, string>, IValueUnit 
        where TValueUnit : BaseValueUnit<TValueUnit>
    {
        #region private
        private string _value = DefaultValue;
        #endregion
        #region protected
        protected const string DefaultValue = "";
        //---------------------------------------------------------------------
        protected override string GetValue()
        {
            return _value;
        }
        protected override void UpdateValue(char[] valueSymbols)
        {
            _value = new string(valueSymbols);
        }
        protected override char GetSymbol(int index)
        {
            return _value[index];
        }
        protected override int GetCount()
        {
            return _value.Length;
        }
        #endregion
        protected BaseValueUnit(bool decodeLineIgnoreCaseSymbols)
            : base(decodeLineIgnoreCaseSymbols)
        { }
        protected BaseValueUnit(bool decodeLineIgnoreCaseSymbols, string str)
            : base(decodeLineIgnoreCaseSymbols, str)
        { }
        protected BaseValueUnit(bool decodeLineIgnoreCaseSymbols, params char[] symbols)
            : base(decodeLineIgnoreCaseSymbols, symbols)
        { }
    }
    //-------------------------------------------------------------------------
    public abstract class BaseValueUnit<TValueUnit, TValue> : BaseSymbolsUnit<TValueUnit>, IValueUnit<TValue>
        where TValueUnit : BaseValueUnit<TValueUnit, TValue>
    {
        #region private
        private int _startPosition = -1;
        private bool _available;
        #endregion
        #region protected
        // ReSharper disable once StaticMemberInGenericType
        protected static readonly char[] DefaultValueSymbols = new char[0];
        //---------------------------------------------------------------------
        protected void Reset()
        {
            Update(false, -1, DefaultValueSymbols);
        }
        protected void Update(bool available, int startPosition, string value)
        {
            Update(available, startPosition, value?.ToCharArray() ?? DefaultValueSymbols);
        }
        protected void Update(bool available, int startPosition, char[] valueSymbols)
        {
            if (startPosition < -1)
            { throw new ArgumentOutOfRangeException(nameof(startPosition)); }

            _available = available;
            _startPosition = startPosition;

            UpdateValue(valueSymbols);
        }
        //---------------------------------------------------------------------
        protected static char[] GetLineSymbols(string line, int startIndex, int endIndex)
        {
            if (startIndex < 0)
            { throw new ArgumentNullException(nameof(startIndex)); }
            if (endIndex < -1)
            { throw new ArgumentNullException(nameof(endIndex)); }

            if (string.IsNullOrEmpty(line) || endIndex == -1)
            { return DefaultValueSymbols; }

            var newEndPosition = endIndex < line.Length ? endIndex + 1 : endIndex;
            var length = newEndPosition - startIndex;

            return line.ToCharArray(startIndex, length);
        }
        //---------------------------------------------------------------------
        [SuppressMessage("ReSharper", "RedundantBaseQualifier")]
        protected char GetOriginalSymbol(int index)
        {
            return base.GetSymbol(index);
        }
        //---------------------------------------------------------------------
        protected abstract TValue GetValue();
        protected abstract void UpdateValue(char[] valueSymbols);
        //---------------------------------------------------------------------
        protected override UnitError DecodeLine(string line, ref int position, Func<char, bool> isIgnoreSymbol)
        {
            var startPosition = -1;

            UnitError error = null;
            var savePosition = position;
            var endPosition = -1;
            var valueSymbolPosition = 0;
            // ReSharper disable once RedundantBaseQualifier
            var count = base.GetCount();
            var ignoreCase = DecodeLineIgnoreCaseSymbols;

            if (count != 0)
            {
                for (; position < line.Length; position++)
                {
                    var symbolLine = line[position];

                    // ReSharper disable once RedundantBaseQualifier
                    if (symbolLine.EqualsIgnoreCase(GetOriginalSymbol(valueSymbolPosition), ignoreCase))
                    {
                        if (startPosition == -1)
                        { startPosition = position; }

                        ++valueSymbolPosition;
                    }
                    else
                    {
                        var isError = !(valueSymbolPosition == 0 && isIgnoreSymbol(symbolLine));
                        if (isError)
                        { break; }
                    }

                    if (valueSymbolPosition == count)
                    {
                        endPosition = position;
                        position++;
                        break;
                    }
                }
            }

            if (valueSymbolPosition == count)
            {
                var value = GetLineSymbols(line, startPosition, endPosition);
                Update(true, startPosition, value);
            }
            else
            {
                error = CreateError(line, position, this, valueSymbolPosition); 
                position = savePosition;
            }

            return error;
        }
        protected override void IntializeDecode()
        {
            base.IntializeDecode();
            Reset();
        }
        protected override void RaiseAction(Delegate action)
        {
            var actionValue = action as Action<TValue>;
            if (actionValue == null)
            { base.RaiseAction(action); }
            else
            { actionValue(GetValue()); }
        }
        #endregion
        protected BaseValueUnit(bool decodeLineIgnoreCaseSymbols)
            : base(decodeLineIgnoreCaseSymbols)
        { }
        protected BaseValueUnit(bool decodeLineIgnoreCaseSymbols, string str)
            : base(decodeLineIgnoreCaseSymbols, str)
        { }
        protected BaseValueUnit(bool decodeLineIgnoreCaseSymbols, params char[] symbols)
            : base(decodeLineIgnoreCaseSymbols, symbols)
        { }

        //---------------------------------------------------------------------
        public bool Available
        {
            get { return _available; }
        }
        //---------------------------------------------------------------------
        public int StartPosition
        {
            get
            {
                if (_available)
                { return _startPosition; }

                throw new InvalidOperationException();
            }
        }
        //---------------------------------------------------------------------
        public TValue Value
        {
            get
            {
                if (_available)
                { return GetValue(); }

                throw new InvalidOperationException();
            }
        }
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}