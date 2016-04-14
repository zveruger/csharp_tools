namespace Tools.CSharp.AnalyzerLines
{
    public interface IContainerUnits : IUnit
    {
        //---------------------------------------------------------------------
        Unit FirstAddedUnit { get; }
        Unit LastAddedUnit { get; }
        //---------------------------------------------------------------------
        Unit FindFirstUnit();
        Unit FindLastUnit();
        //---------------------------------------------------------------------
        void AddUnit(Unit unit);
        //---------------------------------------------------------------------
    }
}