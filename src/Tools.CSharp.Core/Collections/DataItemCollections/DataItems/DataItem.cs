namespace Tools.CSharp.Collections
{
    public class DataItem<TData>
    {
        #region private
        private string _name;
        //---------------------------------------------------------------------
        private readonly TData _data;
        #endregion
        public DataItem(TData data, string name)
        {
            _data = data;
            _name = name;
        }

        //---------------------------------------------------------------------
        public TData Data
        {
            get { return _data; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        //---------------------------------------------------------------------
        public override string ToString()
        {
            return _name;
        }
        //---------------------------------------------------------------------
    }
}