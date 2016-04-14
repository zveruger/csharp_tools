using System;
using System.ComponentModel;
using System.Data;

namespace Tools.CSharp.DataTable.CompositeObjects
{
	public abstract class CoreContainedObjectCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection> : BaseDisposable, IContainedObjectCollection<TContainedObject>
		where TDataTableController : IDataTableController
		where TContainedObject : BaseContainedObject<TDataTableController, TContainedObjectCollection, TRow, TContainedObject>
		where TRow : DataRow
		where TContainedObjectCollection : CoreContainedObjectCollection<TDataTableController, TContainedObject, TRow, TContainedObjectCollection>
	{
		#region internal
		internal abstract TRow GetRowWithoutVerification(int index);
		internal abstract TContainedObject GetObjectByIndex(int index);
		//---------------------------------------------------------------------
		internal abstract int GetRowUniqueIdWithoutVerification(TRow row);
        internal abstract int FindIndexImp(int rowUniqueId);
        //---------------------------------------------------------------------
        internal virtual void DeleteRowFromCollectionImp(TRow row)
		{
			row?.Delete();
		}
		//---------------------------------------------------------------------
		internal abstract TContainedObject GetObjectWithoutVerification(int rowUniqueId);
		internal abstract TContainedObject GetObjectWithoutVerification(TRow row);
		#endregion
		#region protected
		protected TContainedObject GetObject(int rowUniqueId)
		{
			return GetObjectWithoutVerification(rowUniqueId);
		}
		protected TContainedObject GetObject(TRow row)
		{
			if (row == null)
			{ throw new ArgumentNullException(nameof(row)); }

			return GetObjectWithoutVerification(row);
		}
		//---------------------------------------------------------------------
		protected virtual void ObjectRemove(TContainedObject obj)
		{
			
		}
		protected virtual void CollectionChange(ListChangedEventArgs e)
		{
			Changed?.Invoke(this, e);
		}
		#endregion
		//---------------------------------------------------------------------
		internal TRow GetRow(int index)
		{
			if (index < 0 || index >= Count)
			{ throw new ArgumentOutOfRangeException(nameof(index)); }

			return GetRowWithoutVerification(index);
		}
		//---------------------------------------------------------------------
		internal int GetRowUniqueIdFromCollection(TRow row)
		{
			if (row == null)
			{ throw new ArgumentNullException(nameof(row)); }

			return GetRowUniqueIdWithoutVerification(row);
		}
		//---------------------------------------------------------------------
		internal void DeleteObjectFromCollection(TContainedObject obj)
		{
			if (obj == null)
			{ throw new ArgumentNullException(nameof(obj)); }

		    ObjectRemove(obj);
			DeleteRowFromCollectionImp(obj.InternalRow);
		}
		//---------------------------------------------------------------------
		public abstract TDataTableController Controller { get; }
		public abstract int Count { get; }
		//---------------------------------------------------------------------
		public TContainedObject this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{ throw new ArgumentOutOfRangeException(nameof(index)); }

				return GetObjectByIndex(index);
			}
		}
		//---------------------------------------------------------------------
		public int FindIndex(int uniqueId)
		{
			return FindIndexImp(uniqueId);
		}
		//---------------------------------------------------------------------
		public event ListChangedEventHandler Changed;
		//---------------------------------------------------------------------
	}
}