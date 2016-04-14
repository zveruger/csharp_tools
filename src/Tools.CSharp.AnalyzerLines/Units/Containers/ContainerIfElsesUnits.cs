using System;
using System.Linq;

namespace Tools.CSharp.AnalyzerLines
{
    public sealed class ContainerIfElsesUnits : ContainerListUnits
    {
        #region private
        private int _selectedIndex = -1;
        #endregion
        #region protected
        protected override void IntializeDecode()
        {
            base.IntializeDecode();

            _selectedIndex = -1;
        }
        protected override UnitError DecodeLine(string line, ref int position, Func<char, bool> isIgnoreSymbol)
        {
            UnitError oldError = null;
            var savePosition = position;
            _selectedIndex = -1;
            var units = Units;

            for (var i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                unit.InitializeDecodeLine();

                UnitError error;
                if (unit.DecodeLine(line, ref position, out error))
                {
                    _selectedIndex = i;
                    break;
                }

                position = savePosition;

                if (oldError == null)
                { oldError = error; }
                else
                {
                    if (oldError.PositionInLine == error.PositionInLine)
                    { oldError = CreateError(oldError.Line, oldError.PositionInLine, null, -1); }
                    else if (oldError.PositionInLine < error.PositionInLine)
                    { oldError = error; }
                }
            }

            return _selectedIndex != -1 ? null : oldError;
        }

        protected override bool AddUnitInContainer(Unit unit)
        {
            Units.Add(unit);
            return true;
        }
        protected override int GetLengthUnit()
        {
            var selectedUnit = _selectedIndex == -1 ? null : this[_selectedIndex];
            return GetLength(selectedUnit);
        }
        protected override bool IsFirstSymbolUnit(char symbol)
        {
            return Units.Any(unit => unit.IsFirstSymbol(symbol)); 
        }
        protected override void RaiseAllActions()
        {
            if (_selectedIndex != -1)
            {
                var selectedUnit = this[_selectedIndex];
                ActivateAllActions(selectedUnit);
            }            
        }
        #endregion
        public ContainerIfElsesUnits(bool decodeLineIgnoreCaseSymbols = false)
            : base(decodeLineIgnoreCaseSymbols)
        { }
    }
}