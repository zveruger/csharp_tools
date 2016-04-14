namespace Tools.CSharp.Collections
{
    public class DataHelpItem<TData> : DataItem<TData>
    {
        #region private
        private readonly string _help;
        #endregion
        public DataHelpItem(TData data, string name, string help)
            : base(data, name)
        {
            _help = help;
        }

        //---------------------------------------------------------------------
        public string Help
        {
            get { return _help; }
        }
        //---------------------------------------------------------------------
    }
}