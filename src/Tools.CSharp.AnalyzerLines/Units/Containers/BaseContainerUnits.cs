using System;
using System.Collections.ObjectModel;

namespace Tools.CSharp.AnalyzerLines
{
    public abstract class BaseContainerUnits : Unit, IUnitForIgnoreSymbols, IContainerUnits
    {
        #region private
        private Unit _firstAddedUnit;
        private Unit _lastAddedUnit;
        #endregion
        #region protected
        protected void ActivateAllActions(Unit unit)
        {
            unit?.ActivateAllActions();
        }
        protected int GetLength(Unit unit)
        {
            return unit?.GetLength() ?? 0;
        }

        protected abstract void SetDecodeLineIgnoreCaseSymbolsUnits(bool ignoreCaseSymbols);

        protected abstract bool AddUnitInContainer(Unit unit);
        protected new abstract void AddIgnoreSymbol(char symbol);
        protected new abstract void AddRangeIgnoreSymbols(ReadOnlyCollection<char> symbols);
        protected new abstract void RemoveIgnoreSymbol(char symbol);
        protected new abstract void RemoveRangeIgnoreSymbols(ReadOnlyCollection<char> symbols);
        protected new abstract void ClearIgnoreSymbols(bool forever);

        protected override void SetDecodeLineIgnoreCaseSymbols(bool ignoreCaseSymbols)
        {
            base.SetDecodeLineIgnoreCaseSymbols(ignoreCaseSymbols);

            SetDecodeLineIgnoreCaseSymbolsUnits(ignoreCaseSymbols);
        }
        protected override bool IsFirstSymbolUnit(char symbol)
        {
            return _firstAddedUnit != null && _firstAddedUnit.IsFirstSymbol(symbol);
        }
        #endregion
        protected BaseContainerUnits(bool decodeLineIgnoreCaseSymbols)
            : base(decodeLineIgnoreCaseSymbols)
        { }

        //---------------------------------------------------------------------
        public void AddUnit(Unit unit)
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }

            if (AddUnitInContainer(unit))
            {
                unit.IgnoreSymbols.Add(IgnoreSymbols);

                if (DecodeLineIgnoreCaseSymbols)
                { unit.DecodeLineIgnoreCaseSymbols = DecodeLineIgnoreCaseSymbols; }

                if (_firstAddedUnit == null)
                { _firstAddedUnit = unit; }

                _lastAddedUnit = unit;
            }
        }
        //---------------------------------------------------------------------
        public Unit FirstAddedUnit
        {
            get { return _firstAddedUnit; }
        }
        public Unit LastAddedUnit
        {
            get { return _lastAddedUnit; }
        }
        //---------------------------------------------------------------------
        public Unit FindFirstUnit()
        {
            var nextContainer = _firstAddedUnit as BaseContainerUnits;
            var firstUnit = _firstAddedUnit;

            while (nextContainer != null)
            {
                firstUnit = nextContainer.FindFirstUnit();

                if (firstUnit == null)
                {
                    firstUnit = nextContainer;
                    break;
                }

                nextContainer = firstUnit as BaseContainerUnits;
            }

            return firstUnit;
        }
        public Unit FindLastUnit()
        {
            var nextContainer = _lastAddedUnit as BaseContainerUnits;
            var lastUnit = _lastAddedUnit;

            while (nextContainer != null)
            {
                lastUnit = nextContainer.FindLastUnit();

                if (lastUnit == null)
                {
                    lastUnit = nextContainer;
                    break;
                }

                nextContainer = lastUnit as BaseContainerUnits;
            }

            return lastUnit;
        }
        //---------------------------------------------------------------------
        void IUnitForIgnoreSymbols.AddIgnoreSymbol(char symbol)
        {
            AddIgnoreSymbol(symbol);
        }
        void IUnitForIgnoreSymbols.AddRangeIgnoreSymbols(ReadOnlyCollection<char> symbols)
        {
            AddRangeIgnoreSymbols(symbols);
        }
        void IUnitForIgnoreSymbols.RemoveIgnoreSymbol(char symbol)
        {
            RemoveIgnoreSymbol(symbol);
        }
        void IUnitForIgnoreSymbols.RemoveRangeIgnoreSymbols(ReadOnlyCollection<char> symbols)
        {
            RemoveRangeIgnoreSymbols(symbols);
        }
        void IUnitForIgnoreSymbols.ClearIgnoreSymbols(bool forever)
        {
            ClearIgnoreSymbols(forever);
        }
        //---------------------------------------------------------------------
    }
}