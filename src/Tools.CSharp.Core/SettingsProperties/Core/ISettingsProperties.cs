using System.ComponentModel;

namespace Tools.CSharp.SettingsProperties
{
    public interface ISettingsProperties : INotifyPropertyChanged
    {
        //---------------------------------------------------------------------
        bool IsPropertiesChanged { get; }
        //---------------------------------------------------------------------
    }
}