using System;
using System.Collections.Generic;
using System.Linq;

namespace Tools.CSharp.FindDatas
{
    public abstract class FindDatasSettings<TValue> : IFindDatasSettings<TValue>
    {
        #region private
        private readonly List<TValue> _findDatasSettingsAvailable = new List<TValue>();
        //---------------------------------------------------------------------
        private bool _disposed;
        #endregion
        #region protected
        protected abstract TValue GetDefaultFindDatasSettings();
        protected virtual void UpdateFindDatasSetting(TValue value)
        {
            Value = value;
        }
        protected virtual void InternalUpdateFindDatasSettings(TValue value, IEnumerable<TValue> collection)
        {
            if (collection == null)
            { throw new ArgumentNullException(nameof(collection)); }

            var bufValue = collection.Any(item => item.Equals(value)) ? value : GetDefaultFindDatasSettings();
            UpdateFindDatasSetting(bufValue);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }
        #endregion
        //---------------------------------------------------------------------
        public abstract TValue Value { get; set; }
        //---------------------------------------------------------------------
        public virtual void Load()
        {
            throw new NotImplementedException();
        }
        public virtual void Save()
        {
            throw new NotImplementedException();
        }
        //---------------------------------------------------------------------
        public bool IsDisposed
        {
            get { return _disposed; }
        }
        //---------------------------------------------------------------------
        public IEnumerable<TValue> GetFindDatasSettingAvailable
        {
            get { return _findDatasSettingsAvailable; }
        }
        //---------------------------------------------------------------------
        public void AddFindDatasSettingAvailable(object value)
        {
            if (value is TValue)
            { AddFindDatasSettingAvailable((TValue)value); }
        }
        public void AddFindDatasSettingAvailable(TValue value)
        {
            if (!_findDatasSettingsAvailable.Contains(value))
            { _findDatasSettingsAvailable.Add(value); }
        }
        public void RemoveFindDatasSettingAvailable(object value)
        {
            if (value is TValue)
            { RemoveFindDatasSettingAvailable((TValue)value); }
        }
        public void RemoveFindDatasSettingAvailable(TValue value)
        {
            _findDatasSettingsAvailable.Remove(value);
        }
        //---------------------------------------------------------------------
        public void UpdateFindDatasSettings()
        {
            UpdateFindDatasSettings(Value);
        }
        public void UpdateFindDatasSettings(TValue value)
        {
            UpdateFindDatasSettings(value, _findDatasSettingsAvailable);
        }
        public void UpdateFindDatasSettings(TValue value, IEnumerable<TValue> collection)
        {
            if (collection == null)
            { throw new ArgumentNullException(nameof(collection)); }

            InternalUpdateFindDatasSettings(value, collection);
        }
        //---------------------------------------------------------------------
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //---------------------------------------------------------------------
    }
}