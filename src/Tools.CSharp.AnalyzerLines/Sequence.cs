namespace Tools.CSharp.AnalyzerLines
{
    public sealed class Sequence : ContainerListUnits
    {
        //---------------------------------------------------------------------
        public static readonly CommentUnit CommentAsm = new CommentUnit(';');
        public static readonly CommentUnit CommentC = new CommentUnit("//");
        //---------------------------------------------------------------------
        public static readonly SymbolUnit Space = new SymbolUnit(' ');
        //---------------------------------------------------------------------
        public static readonly SymbolUnit StartCircleBracket = new SymbolUnit('(');
        public static readonly SymbolUnit EndCircleBracket = new SymbolUnit(')');
        public static readonly SymbolUnit StartCurclyBracket = new SymbolUnit('{');
        public static readonly SymbolUnit EndCurclyBracket = new SymbolUnit('}');
        public static readonly SymbolUnit StartSquareBracket = new SymbolUnit('[');
        public static readonly SymbolUnit EndSquareBracket = new SymbolUnit(']');
        //---------------------------------------------------------------------
        public static readonly SymbolUnit Separator = new SymbolUnit(',');
        //---------------------------------------------------------------------
        public static readonly SymbolUnit Equal = new SymbolUnit('=');
        //---------------------------------------------------------------------
        public static readonly SymbolUnit Minus = new SymbolUnit('-');
        public static readonly SymbolUnit Plus = new SymbolUnit('+');
        public static readonly SymbolUnit Multiply = new SymbolUnit('*');
        public static readonly SymbolUnit Divide = new SymbolUnit('/');
        //---------------------------------------------------------------------
    }
}