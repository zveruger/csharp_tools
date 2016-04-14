using System;

namespace Tools.CSharp.SettingsProperties
{
    public interface IComplexSettingsProperties : ISettingsProperties, IDisposable
    {
        //---------------------------------------------------------------------
        IComplexSettingsProperties Parent { get; }
        //---------------------------------------------------------------------
        void Load();
        void Save();
        //---------------------------------------------------------------------
        bool TrySave(object settingsProperties);
        //---------------------------------------------------------------------
        event EventHandler Loading;
        event EventHandler Loaded;
        //---------------------------------------------------------------------
        event EventHandler Saving;
        event EventHandler Saved;
        //---------------------------------------------------------------------
    }
}