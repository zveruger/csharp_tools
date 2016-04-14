using System;
using System.Collections.Generic;

namespace Tools.CSharp.Network.Tcp.DataTypes
{
    public sealed class TcpSimpleTypeComposite : BaseTcpTypeComposite<TcpSimpleTypeComposite>, ITcpCloneable<TcpSimpleTypeComposite>
    {
        #region protected
        protected override TcpSimpleTypeComposite CreateObjectClone()
        {
            return new TcpSimpleTypeComposite();
        }
        #endregion
        //---------------------------------------------------------------------
        public void Add(ITcpType value)
        {
            if (value == null)
            { throw new ArgumentNullException(nameof(value)); }

            AddChild(value);
        }
        //---------------------------------------------------------------------
        public TcpSimpleTypeComposite Clone()
        {
            return CreateClone();
        }
        //---------------------------------------------------------------------
    }
}