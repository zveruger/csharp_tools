using System.Collections.Generic;
using System.Linq;

namespace Tools.CSharp.Uniques
{
    public class UniqueObjectsId
    {
        #region private
        private readonly HashSet<int> _uniqueIdAddedCollection = new HashSet<int>();
        private readonly HashSet<int> _uniqueIdRemovedCollection = new HashSet<int>(); 
        //---------------------------------------------------------------------
        private int _uniqueCounter;
        #endregion

        //---------------------------------------------------------------------
        public int CreateUniqueId()
        {
            int uniqueId;
            if (_uniqueIdRemovedCollection.Count == 0)
            {
                do
                { _uniqueCounter++; }
                while (_uniqueIdAddedCollection.Contains(_uniqueCounter));

                uniqueId = _uniqueCounter;
            }
            else
            { uniqueId = _uniqueIdRemovedCollection.Min(); }

            return uniqueId;
        }
        //---------------------------------------------------------------------
        public void Add(int id)
        {
            _uniqueIdAddedCollection.Add(id);
            _uniqueIdRemovedCollection.Remove(id);
        }
        public void Remove(int id)
        {
            _uniqueIdAddedCollection.Remove(id);
            if (_uniqueIdAddedCollection.Count == 0)
            {
                _uniqueCounter = 0;
                _uniqueIdRemovedCollection.Clear();
            }
            else
            { _uniqueIdRemovedCollection.Add(id); }
        }
        public void Clear()
        {
            _uniqueCounter = 0;
            _uniqueIdAddedCollection.Clear();
            _uniqueIdRemovedCollection.Clear();
        }
        //---------------------------------------------------------------------
    }
}
