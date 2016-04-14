using System;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.AnalyzerLines
{
    public sealed class CommentUnit : BaseValueUnit<CommentUnit>
    {
        #region private
        private readonly string _commentStr;
        #endregion
        #region protected
        protected override UnitError DecodeLine(string line, ref int position, Func<char, bool> isIgnoreSymbol)
        {
            var value = DefaultValueSymbols;
            var startPosition = -1;

            var savePosition = position;
            var endPosition = -1;
            var valueSymbolPosition = 0;
            var symbolLength = GetLength();

            for (; position < line.Length; position++)
            {
                var symbolLine = line[position];

                if (valueSymbolPosition == symbolLength)
                {
                    if (isIgnoreSymbol(symbolLine))
                    { continue; }

                    if (startPosition == -1)
                    { startPosition = position; }

                    endPosition = position;
                }
                else
                {
                    if (symbolLine.EqualsIgnoreCase(GetOriginalSymbol(valueSymbolPosition), DecodeLineIgnoreCaseSymbols))
                    { ++valueSymbolPosition; }
                    else
                    {
                        var isError = !(valueSymbolPosition == 0 && isIgnoreSymbol(symbolLine));
                        if (isError)
                        { break; }
                    }
                }
            }

            var available = valueSymbolPosition == symbolLength;
            if (available)
            {
                
                if (endPosition == -1)
                { startPosition = NormalizePositionInLine(position, line.Length); }
                else
                { value = GetLineSymbols(line, startPosition, endPosition); }
            }
            else
            { position = savePosition; }

            Update(available, startPosition, value);
            return null;
        }
        #endregion
        public CommentUnit(char commentSymbol)
            : base(false, commentSymbol)
        {
            _commentStr = new string(commentSymbol, 1);
        }
        public CommentUnit(string commentStr)
            : base(false, commentStr)
        {
            _commentStr = commentStr;
        }

        //---------------------------------------------------------------------
        public string CommentStr
        {
            get { return _commentStr; }
        }
        //---------------------------------------------------------------------
    }
}