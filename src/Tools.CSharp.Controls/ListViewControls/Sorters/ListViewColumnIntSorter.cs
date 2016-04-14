namespace Tools.CSharp.Controls
{
    public class ListViewColumnIntSorter : ListViewColumnTextSorter
    {
        #region protected
        protected override int Compar(string x, string y)
        {
            int intX;
            int intY;
            if (int.TryParse(x, out intX) && int.TryParse(y, out intY))
            {
                var compareResult = intX.CompareTo(intY);
                return CreateCompareResult(compareResult);
            }

            return base.Compar(x, y);
        }
        #endregion
    }
}
