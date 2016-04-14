using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tools.CSharp.AnalyzerLines
{
    //-------------------------------------------------------------------------
    public abstract class Unit : IUnitForIgnoreSymbols, IUnit
    {
        #region private
        private readonly IgnoreSymbols _ignoreSymbols;
        private readonly List<Delegate> _actions = new List<Delegate>(); 
        //---------------------------------------------------------------------
        private Action<UnitError> _raiseError;
        private bool _decodeLineIgnoreCaseSymbols;
        //---------------------------------------------------------------------
        #endregion
        #region protected
        protected UnitError CreateError(string line, int positionInLine, Unit unit, int positionInUnit)
        {
            var newPosition = NormalizePositionInLine(positionInLine, line);
            return new UnitError(line, newPosition, positionInUnit, unit);
        }
        protected int NormalizePositionInLine(int position, string line)
        {
            if (string.IsNullOrEmpty(line))
            { return -1; }

            return NormalizePositionInLine(position, line.Length);
        }
        protected int NormalizePositionInLine(int position, int lineLength)
        {
            var normalizePosition = position;

            if (normalizePosition >= lineLength)
            { normalizePosition = lineLength - 1; }

            if (normalizePosition < 0)
            { normalizePosition = 0; }

            return normalizePosition;
        }

        protected abstract UnitError DecodeLine(string line, ref int position, Func<char, bool> isIgnoreSymbol);
        protected abstract bool IsFirstSymbolUnit(char symbol);
        protected abstract int GetLengthUnit(); 

        protected virtual void AddIgnoreSymbol(char symbol)
        { }
        protected virtual void AddRangeIgnoreSymbols(ReadOnlyCollection<char> symbols)
        { }
        protected virtual void RemoveIgnoreSymbol(char symbol)
        { }
        protected virtual void RemoveRangeIgnoreSymbols(ReadOnlyCollection<char> symbols)
        {
            
        }
        protected virtual void ClearIgnoreSymbols(bool forever)
        { }

        protected virtual void SetDecodeLineIgnoreCaseSymbols(bool ignoreCaseSymbols)
        {
            _decodeLineIgnoreCaseSymbols = ignoreCaseSymbols;
        }

        protected virtual void IntializeDecode()
        { }
        protected virtual void RaiseAllActions()
        {
            foreach (var action in _actions)
            {
                RaiseAction(action);
            }
        }
        protected virtual void RaiseAction(Delegate action)
        {
            var tmpAction = action as Action;
            tmpAction?.Invoke();
        }
        #endregion
        protected Unit(bool decodeLineIgnoreCaseSymbols)
        {
            _ignoreSymbols = new IgnoreSymbols(this);
            _decodeLineIgnoreCaseSymbols = decodeLineIgnoreCaseSymbols;
        }

        //---------------------------------------------------------------------
        public IgnoreSymbols IgnoreSymbols
        {
            get { return _ignoreSymbols; }
        }
        public bool DecodeLineIgnoreCaseSymbols
        {
            get { return _decodeLineIgnoreCaseSymbols; }
            set
            {
                if (_decodeLineIgnoreCaseSymbols != value)
                { SetDecodeLineIgnoreCaseSymbols(value); }
            }
        }
        //---------------------------------------------------------------------
        public void InitializeDecodeLine()
        {
            IntializeDecode();
        }
        //---------------------------------------------------------------------
        public bool DecodeLine(string line)
        {
            var position = 0;
            UnitError error;

            InitializeDecodeLine();

            var decodeResult = DecodeLine(line, ref position, out error);

            if (!decodeResult)
            {
                if ((error.PositionInLine != -1) && (error.Unit != null))
                {
                    if ((error.PositionInLine + 1) == line.Length)
                    {
                        if (error.Unit.IgnoreSymbols.IsIgnoreSymbol(line[error.PositionInLine]))
                        { decodeResult = true; }
                    }
                }
            }

            if (decodeResult)
            { decodeResult = DecodeEndLine(line, position, out error); }

            if (decodeResult)
            { ActivateAllActions(); }
            else
            { _raiseError?.Invoke(error); }

            return decodeResult;
        }
        public bool DecodeLine(string line, ref int position, out UnitError error)
        {
            error = line == null ? CreateError(null, position, this, 0) : DecodeLine(line, ref position, _ignoreSymbols.IsIgnoreSymbol);
            return error == null;
        }
        public bool DecodeEndLine(string line, int position, out UnitError error)
        {
            error = null;

            if (!string.IsNullOrEmpty(line))
            {
                for (var i = position; i < line.Length; i++)
                {
                    var symbol = line[i];

                    if (!_ignoreSymbols.IsIgnoreSymbol(symbol))
                    {
                        error = CreateError(line, i, null, -1);
                        break;
                    }
                }
            }
            
            return error == null;
        }
        //---------------------------------------------------------------------
        public bool IsFirstSymbol(char symbol)
        {
            return IsFirstSymbolUnit(symbol);
        }

        //---------------------------------------------------------------------
        internal void AddAction(Delegate action)
        {
            if (action != null)
            { _actions.Add(action); }
        }
        internal void ActivateAllActions()
        {
            RaiseAllActions();
        }
        //---------------------------------------------------------------------
        internal int GetLength()
        {
            return GetLengthUnit();
        }
        //---------------------------------------------------------------------
        internal void SetRaiseError(Action<UnitError> raiseError)
        {
            _raiseError = raiseError;
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
    //-------------------------------------------------------------------------
    public abstract class Unit<TUnit> : Unit
        where TUnit : Unit<TUnit>
    {
        #region protected
        protected override void RaiseAction(Delegate action)
        {
            var actionUnit = action as Action<TUnit>;
            if (actionUnit == null)
            { base.RaiseAction(action); }
            else
            { actionUnit((TUnit)this); }
        }
        #endregion
        protected Unit(bool decodeLineIgnoreCaseSymbols)
            : base(decodeLineIgnoreCaseSymbols)
        { }
    }
    //-------------------------------------------------------------------------
}