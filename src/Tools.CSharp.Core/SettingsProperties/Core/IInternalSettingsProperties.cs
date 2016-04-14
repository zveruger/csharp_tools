namespace Tools.CSharp.SettingsProperties
{
    internal interface IInternalSettingsProperties : ISettingsProperties
    {
        //---------------------------------------------------------------------
        IComplexSettingsProperties Parent { get; set; }
        //---------------------------------------------------------------------
    }
}