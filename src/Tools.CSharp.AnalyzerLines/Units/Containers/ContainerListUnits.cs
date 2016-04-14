using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tools.CSharp.AnalyzerLines
{
    public class ContainerListUnits : BaseContainerUnits, IEnumerable<Unit>
    {
        #region private
        private readonly List<Unit> _units = new List<Unit>();
        //---------------------------------------------------------------------
        private sealed class InternalEnumerator : IEnumerator<Unit>
        {
            #region private
            private readonly int _count;
            private readonly List<Unit> _units; 
            //-----------------------------------------------------------------
            private int _index = -1;
            #endregion
            public InternalEnumerator(ContainerListUnits owner)
            {
                _units = owner._units;
                _count = _units.Count;
            }

            //-----------------------------------------------------------------
            public Unit Current
            {
                get { return _units[_index]; }
            }
            object IEnumerator.Current
            {
                get { return Current; }
            }
            //-----------------------------------------------------------------
            public bool MoveNext()
            {
                if (_index < _count)
                {
                    ++_index;
                    return (_index < _count);
                }
                return false;
            }
            public void Reset()
            {
                _index = -1;
            }
            public void Dispose()
            {
                
            }
            //-----------------------------------------------------------------
        }
        #endregion
        #region protected
        protected List<Unit> Units
        {
            get { return _units; }
        }
        protected void ActionAllUnits(Action<Unit> action)
        {
            if (action == null)
            { throw new ArgumentNullException(nameof(action)); }

            foreach (var unit in _units)
            { action(unit); }
        }

        protected override void IntializeDecode()
        {
            base.IntializeDecode();

            foreach (var unit in _units)
            { unit.InitializeDecodeLine(); }
        }
        protected override UnitError DecodeLine(string line, ref int position, Func<char, bool> isIgnoreSymbol)
        {
            UnitError error = null;
            var savePosition = position;
            var unitCounter = 0;

            for (; unitCounter < _units.Count; unitCounter++)
            {
                var unit = _units[unitCounter];

                if (!unit.DecodeLine(line, ref position, out error))
                {
                    position = savePosition;
                    break;
                }
            }

            return error;
        }

        protected override bool AddUnitInContainer(Unit unit)
        {
            var lastAddContainerAnyUnit = LastAddedUnit as ContainerAnyUnits;

            if (lastAddContainerAnyUnit != null)
            { lastAddContainerAnyUnit.IsNextUnit = true; }

            var valueUnit = FindLastUnit() as ValueUnit;

            if (valueUnit != null && GetLength(unit) == 1)
            { valueUnit.NextUnit = unit; }

            _units.Add(unit);

            return true;
        }
        protected override int GetLengthUnit()
        {
            return _units.Sum(unit => GetLength(unit));
        }
        protected override void AddIgnoreSymbol(char symbol)
        {
            ActionAllUnits(unit => unit.IgnoreSymbols.Add(symbol));
        }
        protected override void AddRangeIgnoreSymbols(ReadOnlyCollection<char> symbols)
        {
            ActionAllUnits(unit => unit.IgnoreSymbols.AddRange(symbols));
        }
        protected override void RemoveIgnoreSymbol(char symbol)
        {
            ActionAllUnits(unit => unit.IgnoreSymbols.Remove(symbol));
        }
        protected override void RemoveRangeIgnoreSymbols(ReadOnlyCollection<char> symbols)
        {
            ActionAllUnits(unit => unit.IgnoreSymbols.RemoveRange(symbols));
        }
        protected override void ClearIgnoreSymbols(bool forever)
        {
            ActionAllUnits(unit => unit.IgnoreSymbols.Clear(forever));
        }
        protected override void RaiseAllActions()
        {
            base.RaiseAllActions();

            ActionAllUnits(ActivateAllActions);
        }
        protected override void SetDecodeLineIgnoreCaseSymbolsUnits(bool ignoreCaseSymbols)
        {
            ActionAllUnits(unit => unit.DecodeLineIgnoreCaseSymbols = ignoreCaseSymbols);
        }
        #endregion
        public ContainerListUnits(bool decodeLineIgnoreCaseSymbols = false)
            : base(decodeLineIgnoreCaseSymbols)
        { }

        //---------------------------------------------------------------------
        public Unit this[int index]
        {
            get { return _units[index]; }
        }
        public int Count
        {
            get { return _units.Count; }
        }
        //---------------------------------------------------------------------
        public IEnumerator<Unit> GetEnumerator()
        {
            return new InternalEnumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        //---------------------------------------------------------------------
    }
}