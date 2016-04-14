using System;

namespace Tools.CSharp.AnalyzerLines
{
    public sealed class UnitError
    {
        #region private
        private readonly string _line;
        private readonly int _positionInLine;
        private readonly Unit _unit;
        private readonly int _positionInUnit;
        #endregion
        internal UnitError(string line, int positionInLine, int positionInUnit = -1, Unit unit = null)
        {
            if (string.IsNullOrEmpty(line))
            {
                if (positionInLine != -1)
                { throw new ArgumentOutOfRangeException(nameof(positionInLine)); }
            }
            else
            {
                if (positionInLine < 0 || positionInLine >= line.Length)
                { throw new ArgumentOutOfRangeException(nameof(positionInLine)); }
            }
           
            if (positionInUnit < -1)
            { throw new ArgumentOutOfRangeException(nameof(positionInUnit)); }

            if (unit == null)
            {
                if (positionInUnit != -1)
                { throw new ArgumentNullException(nameof(positionInUnit)); }
            }
            else
            {
                if (positionInUnit == -1)
                { throw new ArgumentException(string.Empty, nameof(positionInUnit)); }
            }

            _line = line;
            _positionInLine = positionInLine;

            _unit = unit;
            _positionInUnit = positionInUnit;
        }

        //---------------------------------------------------------------------
        public string Line
        {
            get { return _line; }
        }
        public int PositionInLine
        {
            get { return _positionInLine; }
        }
        //---------------------------------------------------------------------
        public Unit Unit
        {
            get { return _unit; }
        }
        public int PositionInUnit
        {
            get { return _positionInUnit; }
        }
        //---------------------------------------------------------------------
    }
}