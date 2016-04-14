using System;
using System.Collections.Generic;
using System.Linq;

namespace Tools.CSharp.AnalyzerLines
{
    public sealed class IgnoreSymbols
    {
        #region private
        private readonly IUnitForIgnoreSymbols _owner;
        private readonly HashSet<char> _ignoreSymbols = new HashSet<char>(); 
        //---------------------------------------------------------------------
        private bool _noClearedForever = true;
        #endregion
        internal IgnoreSymbols(IUnitForIgnoreSymbols owner)
        {
            _owner = owner;
        }

        //---------------------------------------------------------------------
        public bool ClearedForever
        {
            get { return !_noClearedForever; }
        }
        //---------------------------------------------------------------------
        public void Add(char symbol)
        {
            if (_noClearedForever && _ignoreSymbols.Add(symbol))
            { _owner.AddIgnoreSymbol(symbol);  }
        }
        public void Add(params char[] symbols)
        {
            AddRange(symbols);
        }
        public void AddRange(ICollection<char> symbols)
        {
            if (symbols == null)
            { throw new ArgumentNullException(nameof(symbols)); }

            if (_noClearedForever)
            {
                var ignoreSymbols = new List<char>(symbols.Count);
                ignoreSymbols.AddRange(symbols.Where(symbol => _ignoreSymbols.Add(symbol)));

                if (ignoreSymbols.Count != 0)
                {
                    if (ignoreSymbols.Count == 1)
                    { _owner.AddIgnoreSymbol(ignoreSymbols[0]); }
                    else
                    { _owner.AddRangeIgnoreSymbols(ignoreSymbols.AsReadOnly()); }
                }
            }
        }
        internal void Add(IgnoreSymbols ignoreSymbols)
        {
            if (ignoreSymbols == null)
            { throw new ArgumentNullException(nameof(ignoreSymbols)); }

            if (_noClearedForever)
            { AddRange(ignoreSymbols._ignoreSymbols.ToList()); }
        }
        //---------------------------------------------------------------------
        public bool IsIgnoreSymbol(char symbol)
        {
            return _ignoreSymbols.Contains(symbol);
        }
        //---------------------------------------------------------------------
        public bool Remove(char symbol)
        {
            if (_noClearedForever)
            {
                var result = _ignoreSymbols.Remove(symbol);

                if (result)
                { _owner.RemoveIgnoreSymbol(symbol); }

                return result;
            }
            return false;
        }
        public void RemoveRange(ICollection<char> symbols)
        {
            if (symbols == null)
            { throw new ArgumentNullException(nameof(symbols)); }

            if (_noClearedForever)
            {
                var ignoreSymbols = new List<char>(symbols.Count);
                ignoreSymbols.AddRange(symbols.Where(symbol => _ignoreSymbols.Remove(symbol)));

                if (ignoreSymbols.Count != 0)
                {
                    if (ignoreSymbols.Count == 1)
                    { _owner.RemoveIgnoreSymbol(ignoreSymbols[0]); }
                    else
                    { _owner.RemoveRangeIgnoreSymbols(ignoreSymbols.AsReadOnly()); }
                }
            }
        }
        //---------------------------------------------------------------------
        public void Clear(bool forever)
        {
            _noClearedForever = !forever;

            if (_ignoreSymbols.Count != 0)
            {
                _ignoreSymbols.Clear();
                _owner.ClearIgnoreSymbols(forever);
            }
        }
        //---------------------------------------------------------------------
    }
}