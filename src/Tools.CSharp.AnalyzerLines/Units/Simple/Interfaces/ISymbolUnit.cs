namespace Tools.CSharp.AnalyzerLines
{
    public interface ISymbolUnit : IValueUnit<char>
    {
        //---------------------------------------------------------------------
        char OriginalValue { get; }
        //---------------------------------------------------------------------
    }
}