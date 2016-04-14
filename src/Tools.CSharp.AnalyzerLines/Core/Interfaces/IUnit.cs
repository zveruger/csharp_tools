namespace Tools.CSharp.AnalyzerLines
{
    public interface IUnit
    {
        //---------------------------------------------------------------------
        IgnoreSymbols IgnoreSymbols { get; }
        bool DecodeLineIgnoreCaseSymbols { get; set; }
        //---------------------------------------------------------------------
        bool IsFirstSymbol(char symbol);
        //---------------------------------------------------------------------
        void InitializeDecodeLine();
        //---------------------------------------------------------------------
        bool DecodeLine(string line);
        bool DecodeLine(string line, ref int position, out UnitError error);
        bool DecodeEndLine(string line, int position, out UnitError error);
        //---------------------------------------------------------------------
    }
}