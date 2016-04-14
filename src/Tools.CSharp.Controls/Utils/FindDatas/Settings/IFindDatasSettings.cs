using System;
using System.Collections.Generic;

namespace Tools.CSharp.FindDatas
{
    //-------------------------------------------------------------------------
    public interface IFindDatasSettings : IDisposable
    {
        //---------------------------------------------------------------------
        void Load();
        void Save();
        //---------------------------------------------------------------------
        void AddFindDatasSettingAvailable(object value);
        void RemoveFindDatasSettingAvailable(object value);
        void UpdateFindDatasSettings();
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
    public interface IFindDatasSettings<TValue> : IFindDatasSettings
    {
        //---------------------------------------------------------------------
        TValue Value { get; set; }
        //---------------------------------------------------------------------
        IEnumerable<TValue> GetFindDatasSettingAvailable { get; }
        //---------------------------------------------------------------------
        void AddFindDatasSettingAvailable(TValue value);
        void RemoveFindDatasSettingAvailable(TValue value);
        void UpdateFindDatasSettings(TValue value);
        void UpdateFindDatasSettings(TValue value, IEnumerable<TValue> collection);
        //---------------------------------------------------------------------
    }
    //-------------------------------------------------------------------------
}
