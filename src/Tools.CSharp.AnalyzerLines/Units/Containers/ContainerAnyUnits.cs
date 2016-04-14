using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tools.CSharp.AnalyzerLines
{
    public sealed class ContainerAnyUnits : BaseContainerUnits, IEnumerable<Unit>
    {
        #region private
        private readonly HashSet<InternalAliveUnit> _units = new HashSet<InternalAliveUnit>(); 
        //---------------------------------------------------------------------
        private bool _isNoNextUnit = true;
        //---------------------------------------------------------------------
        private sealed class InternalAliveUnit : Unit
        {
            #region private
            private readonly ContainerAnyUnits _owner;
            private readonly Unit _unit;
            //-----------------------------------------------------------------
            private int _aliveCount;
            private int _realAliveCount;
            private int _agedCount;
            #endregion
            #region protected
            protected override UnitError DecodeLine(string line, ref int position, Func<char, bool> isIgnoreSymbol)
            {
                UnitError error;
                _unit.DecodeLine(line, ref position, out error);
                return error;
            }
            protected override int GetLengthUnit()
            {
                return _owner.GetLength(_unit);
            }
            protected override bool IsFirstSymbolUnit(char symbol)
            {
                return _unit.IsFirstSymbol(symbol);
            }
            protected override void AddIgnoreSymbol(char symbol)
            {
                _unit.IgnoreSymbols.Add(symbol);
            }
            protected override void AddRangeIgnoreSymbols(ReadOnlyCollection<char> symbols)
            {
                _unit.IgnoreSymbols.AddRange(symbols);
            }
            protected override void RemoveIgnoreSymbol(char symbol)
            {
                _unit.IgnoreSymbols.Remove(symbol);
            }
            protected override void ClearIgnoreSymbols(bool forever)
            {
                _unit.IgnoreSymbols.Clear(forever);
            }
            protected override void RaiseAllActions()
            {
                _owner.ActivateAllActions(_unit);
            }
            protected override void SetDecodeLineIgnoreCaseSymbols(bool ignoreCaseSymbols)
            {
                base.SetDecodeLineIgnoreCaseSymbols(ignoreCaseSymbols);

                _unit.DecodeLineIgnoreCaseSymbols = ignoreCaseSymbols;
            }
            #endregion
            public InternalAliveUnit(ContainerAnyUnits owner, Unit unit, int aliveCount)
                : base(unit.DecodeLineIgnoreCaseSymbols)
            {
                _owner = owner;
                _unit = unit;
                _aliveCount = aliveCount;
                _realAliveCount = _aliveCount;
            }

            //-----------------------------------------------------------------
            public const int ImmortalAliveCount = -1;
            //-----------------------------------------------------------------
            public int RealAliveCount
            {
                get { return _realAliveCount; }
            }
            public int AliveCount
            {
                get { return _aliveCount; }
                set
                {
                    if (value < ImmortalAliveCount || value == 0)
                    { throw new ArgumentOutOfRangeException(nameof(value)); }

                    if (_aliveCount != ImmortalAliveCount)
                    {
                        _aliveCount = value;

                        if (_realAliveCount == 0 || _aliveCount == ImmortalAliveCount)
                        { _realAliveCount = _aliveCount; }
                        else
                        { _realAliveCount += _aliveCount; }
                    }
                }
            }
            public int AgedCount 
            {
                get { return _agedCount; }
            }
            //-----------------------------------------------------------------
            public Unit Unit
            {
                get { return _unit; }
            }
            //-----------------------------------------------------------------
            public bool IsImmortal
            {
                get { return _aliveCount == ImmortalAliveCount; }
            }
            public bool IsAlive
            {
                get { return IsImmortal || _realAliveCount != 0; }
            }
            public bool IsDead
            {
                get { return _realAliveCount == 0; }
            }
            //-----------------------------------------------------------------
            public void Ageing()
            {
                if (IsImmortal)
                { ++_agedCount; }
                else if (_realAliveCount > 0)
                {
                    --_realAliveCount;
                    ++_agedCount;
                }
            }
            public void Resurrection()
            {
                _agedCount = 0;
                _realAliveCount = _aliveCount;

                _unit.InitializeDecodeLine();
            }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        private sealed class InternalEnumerator : IEnumerator<Unit>
        {
            #region private
            private readonly IEnumerator<InternalAliveUnit> _unitsEnumerator;
            #endregion
            public InternalEnumerator(ContainerAnyUnits owner)
            {
                _unitsEnumerator = owner._units.GetEnumerator();
            }
            //-----------------------------------------------------------------
            public Unit Current
            {
                get { return _unitsEnumerator.Current.Unit; }
            }
            object IEnumerator.Current
            {
                get { return Current; }
            }
            //-----------------------------------------------------------------
            public bool MoveNext()
            {
                return _unitsEnumerator.MoveNext();
            }
            public void Reset()
            {
                _unitsEnumerator.Reset();
            }
            public void Dispose()
            {
                _unitsEnumerator.Dispose();
            }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        private void _actionAllUnits(Action<InternalAliveUnit> action)
        {
            if (action == null)
            { throw new ArgumentNullException(nameof(action)); }

            foreach (var unit in _units)
            { action(unit); }
        }
        #endregion
        #region protected
        protected override void IntializeDecode()
        {
            base.IntializeDecode();

            _actionAllUnits(unit => unit.Resurrection());
        }
        protected override UnitError DecodeLine(string line, ref int position, Func<char, bool> isIgnoreSymbol)
        {
            UnitError error;
            UnitError oldError = null;

            while (true)
            {
                var isExit = true;

                foreach (var unit in _units)
                {
                    if ((position == line.Length) || ((position + 1) == line.Length))
                    { break; }

                    if (unit.DecodeLine(line, ref position, out error))
                    {
                        if (unit.IsDead)
                        { oldError = CreateError(line, position - GetLength(unit), unit.Unit, 0); }
                        else
                        {
                            if (_isNoNextUnit)
                            { oldError = null; }

                            unit.Ageing();

                            if ((position + 1) != line.Length)
                            { isExit = false; }
                        }
                        break;
                    }

                    if (_isNoNextUnit && unit.IsAlive)
                    {
                        if (error.Unit == null || !error.Unit.IgnoreSymbols.IsIgnoreSymbol(error.Line[error.PositionInLine]))
                        {
                            if (oldError == null || oldError.PositionInLine < error.PositionInLine)
                            { oldError = error; }
                            else
                            {
                                if (oldError.Unit != null && oldError.PositionInLine == error.PositionInLine)
                                { oldError = new UnitError(error.Line, error.PositionInLine); }
                            }
                        }
                    }
                }

                if (isExit)
                {
                    error = null;
                    break;
                }
            }

            if (oldError != null)
            { error = oldError; }

            return error;
        }

        protected override bool AddUnitInContainer(Unit unit)
        {
            var anyUnit = unit as InternalAliveUnit ?? new InternalAliveUnit(this, unit, 1);

            if (_units.Add(anyUnit))
            {
                if (ReferenceEquals(anyUnit.Unit, this))
                { anyUnit.AliveCount += anyUnit.AliveCount; }

                return true;
            }

            return false;
        }
        protected override int GetLengthUnit()
        {
            return (from unit in _units let agedCount = unit.AgedCount where agedCount != 0 select (GetLength(unit) * agedCount)).Sum();
        }
        protected override bool IsFirstSymbolUnit(char symbol)
        {
            return _units.Any(unit => unit.IsFirstSymbol(symbol));
        }
        protected override void AddIgnoreSymbol(char symbol)
        {
            _actionAllUnits(unit => unit.IgnoreSymbols.Add(symbol));
        }
        protected override void AddRangeIgnoreSymbols(ReadOnlyCollection<char> symbols)
        {
            _actionAllUnits(unit => unit.IgnoreSymbols.AddRange(symbols));
        }
        protected override void RemoveIgnoreSymbol(char symbol)
        {
            _actionAllUnits(unit => unit.IgnoreSymbols.Remove(symbol));
        }
        protected override void RemoveRangeIgnoreSymbols(ReadOnlyCollection<char> symbols)
        {
            _actionAllUnits(unit => unit.IgnoreSymbols.RemoveRange(symbols));
            foreach (var unit in _units)
            { unit.IgnoreSymbols.RemoveRange(symbols); }
        }
        protected override void ClearIgnoreSymbols(bool forever)
        {
            _actionAllUnits(unit => unit.IgnoreSymbols.Clear(forever));
        }
        protected override void RaiseAllActions()
        {
            var isActivateAllActionAvailable = false;

            _actionAllUnits(unit =>
            {
                var agedCount = unit.AgedCount;
                while (agedCount != 0)
                {
                    --agedCount;
                    isActivateAllActionAvailable = true;

                    ActivateAllActions(unit);
                }
            });

            if (isActivateAllActionAvailable)
            { base.RaiseAllActions(); }
        }
        protected override void SetDecodeLineIgnoreCaseSymbolsUnits(bool ignoreCaseSymbols)
        {
            _actionAllUnits(unit => unit.DecodeLineIgnoreCaseSymbols = ignoreCaseSymbols);
        }
        #endregion
        public ContainerAnyUnits(bool decodeLineIgnoreCaseSymbols = false)
            : base(decodeLineIgnoreCaseSymbols)
        { }

        //---------------------------------------------------------------------
        public const int ImmortalUnitAliveCount = InternalAliveUnit.ImmortalAliveCount;
        //---------------------------------------------------------------------
        public bool IsNextUnit
        {
            get { return !_isNoNextUnit; }
            set { _isNoNextUnit = !value; }
        }
        //---------------------------------------------------------------------
        public void AddUnit(Unit unit, int aliveCount)
        {
            if (unit == null)
            { throw new ArgumentNullException(nameof(unit)); }
            if (aliveCount < InternalAliveUnit.ImmortalAliveCount || aliveCount == 0)
            { throw new ArgumentOutOfRangeException(nameof(aliveCount)); }

            var anyUnit = new InternalAliveUnit(this, unit, aliveCount);
            AddUnit(anyUnit);
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