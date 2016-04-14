using System.Collections.ObjectModel;

namespace Tools.CSharp.AnalyzerLines
{
    internal interface IUnitForIgnoreSymbols
    {
        //---------------------------------------------------------------------
        void AddIgnoreSymbol(char symbol);
        void AddRangeIgnoreSymbols(ReadOnlyCollection<char> symbols);
        //---------------------------------------------------------------------
        void RemoveIgnoreSymbol(char symbol);
        void RemoveRangeIgnoreSymbols(ReadOnlyCollection<char> symbols);
        //---------------------------------------------------------------------
        void ClearIgnoreSymbols(bool forever);
        //---------------------------------------------------------------------
    }
}