using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public abstract class BaseTcpTypeComposite<TTcpTypeComposite> : ITcpTypeComposite
        where TTcpTypeComposite : BaseTcpTypeComposite<TTcpTypeComposite>
    {
        #region private
        private readonly List<ITcpType> _childs = new List<ITcpType>();
        #endregion
        #region protected
        protected int GetCount
        {
            get { return _childs.Count; }
        }
        protected ITcpType GetItem(int index)
        {
            return _childs[index];
        }
        //---------------------------------------------------------------------
        protected void AddChild(ITcpType child)
        {
            if (child == null)
            { throw new ArgumentNullException(nameof(child)); }

            _childs.Add(child);
        }
        protected void ClearChilds()
        {
            _childs.Clear();
        }
        //---------------------------------------------------------------------
        protected abstract TTcpTypeComposite CreateObjectClone();
        //---------------------------------------------------------------------
        protected TTcpTypeComposite CreateClone()
        {
            var clone = CreateObjectClone();
            if (clone == null)
            { throw new InvalidOperationException(); }

            foreach (var child in _childs)
            { clone._childs.Add((ITcpType)child.Clone()); }

            return clone;
        }
        #endregion
        //---------------------------------------------------------------------
        public virtual int DataLength
        {
            get { return _childs.Sum(child => child.DataLength); }
        }
        //---------------------------------------------------------------------
        public virtual void Read(byte[] dataTarget, ref int startPositionInDataTarget)
        {
            if (dataTarget == null)
            { throw new ArgumentNullException(nameof(dataTarget)); }
            if (startPositionInDataTarget < 0 || startPositionInDataTarget > dataTarget.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataTarget)); }
            if (dataTarget.Length < (startPositionInDataTarget + DataLength))
            { throw new ArgumentException(string.Empty, nameof(startPositionInDataTarget)); }

            //-----------------------------------------------------------------
            foreach (var child in _childs)
            { child.Read(dataTarget, ref startPositionInDataTarget); }
            //-----------------------------------------------------------------
        }
        public virtual void Read(ITcpTypeData dataTarget, ref int startPositionInDataTarget)
        {
            if (dataTarget == null)
            { throw new ArgumentNullException(nameof(dataTarget)); }
            if (startPositionInDataTarget < 0 || startPositionInDataTarget > dataTarget.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataTarget)); }
            if (dataTarget.Length < (startPositionInDataTarget + DataLength))
            { throw new ArgumentException(string.Empty, nameof(startPositionInDataTarget)); }

            //-----------------------------------------------------------------
            foreach (var child in _childs)
            { child.Read(dataTarget, ref startPositionInDataTarget); }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        public virtual void Write(byte[] dataSource, ref int startPositionInDataSource)
        {
            if (dataSource == null)
            { throw new ArgumentNullException(nameof(dataSource)); }
            if (startPositionInDataSource < 0 || startPositionInDataSource > dataSource.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataSource)); }

            //-----------------------------------------------------------------
            foreach (var child in _childs)
            { child.Write(dataSource, ref startPositionInDataSource); }
            //-----------------------------------------------------------------
        }
        public virtual void Write(ITcpTypeData dataSource, ref int startPositionInDataSource)
        {
            if (dataSource == null)
            { throw new ArgumentNullException(nameof(dataSource)); }
            if (startPositionInDataSource < 0 || startPositionInDataSource > dataSource.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataSource)); }

            //-----------------------------------------------------------------
            foreach (var child in _childs)
            { child.Write(dataSource, ref startPositionInDataSource); }
            //-----------------------------------------------------------------
        }
        //---------------------------------------------------------------------
        public virtual void ContinueWrite(ITcpTypeData dataSource, ref int startPositionInDataSource)
        {
            if (dataSource == null)
            { throw new ArgumentNullException(nameof(dataSource)); }
            if (startPositionInDataSource < 0 || startPositionInDataSource > dataSource.Length)
            { throw new ArgumentOutOfRangeException(nameof(startPositionInDataSource)); }

            var isContinueWrite = false;
            var writedChildsDataLength = 0;

            for (var i = 0; i < _childs.Count; i++)
            {
                var child = _childs[i];

                if (isContinueWrite)
                { child.Write(dataSource, ref startPositionInDataSource); }
                else
                {
                    writedChildsDataLength += child.DataLength;
                    if (writedChildsDataLength < startPositionInDataSource)
                    { continue; }

                    isContinueWrite = true;

                    //Если startPositionInDataSource == 0 или startPositionInDataSource не синхронизирован с началом данных в composite.  
                    if (writedChildsDataLength > startPositionInDataSource)
                    { --i; }
                }
            }
        }
        //---------------------------------------------------------------------
        object ICloneable.Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
        public IEnumerator<ITcpType> GetEnumerator()
        {
            return _childs.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        //---------------------------------------------------------------------
        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            var lastIndex = _childs.Count - 1;

            for (var i = 0; i < _childs.Count; i++)
            {
                var child = _childs[i];

                strBuilder.Append(child);

                if (i != lastIndex)
                { strBuilder.Append(", "); }
            }

            return strBuilder.ToString();
        }
        //---------------------------------------------------------------------
    }
}