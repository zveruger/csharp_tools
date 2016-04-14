namespace Tools.CSharp.AnalyzerLines
{
    public interface ISymbolsUnit : IUnit
    {
        //---------------------------------------------------------------------
        char this[int index] { get; }
        int Count { get; }
        //---------------------------------------------------------------------
    }
}