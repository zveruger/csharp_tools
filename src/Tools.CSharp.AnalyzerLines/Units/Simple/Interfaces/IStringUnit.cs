namespace Tools.CSharp.AnalyzerLines
{
    public interface IStringUnit : IValueUnit
    {
        //---------------------------------------------------------------------
        string OriginalValue { get; }
        //---------------------------------------------------------------------
    }
}