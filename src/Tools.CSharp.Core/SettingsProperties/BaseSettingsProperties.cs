using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Tools.CSharp.SettingsProperties
{
    public abstract class BaseSettingsProperties<TSettings> : IInternalSettingsProperties
        where TSettings : class
    {
        #region private
        private readonly Dictionary<string, string> _cacheSettingsPropertyNames = new Dictionary<string, string>();
        //---------------------------------------------------------------------
        private bool _propertiesChanged;
        private ActionState _actionState = ActionState.Init;
        private IComplexSettingsProperties _parent;
        //---------------------------------------------------------------------
        private enum ActionState
        {
            Init,
            Loading,
            Saving
        }
        #endregion
        #region protected
        protected delegate string ExtractSettingsPropertyName(TSettings settings);
        //---------------------------------------------------------------------
        protected string GetSettingsPropertyName(string propertyName, ExtractSettingsPropertyName extractSettingsPropertyName, TSettings settings)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            { throw new ArgumentNullException(nameof(propertyName)); }

            string cacheSettingsPropertyName;
            if (!_cacheSettingsPropertyNames.TryGetValue(propertyName, out cacheSettingsPropertyName))
            {
                if (extractSettingsPropertyName == null)
                { throw new ArgumentNullException(nameof(extractSettingsPropertyName)); }
                if (settings == null)
                { throw new ArgumentNullException(nameof(settings)); }

                cacheSettingsPropertyName = extractSettingsPropertyName(settings);
                _cacheSettingsPropertyNames.Add(propertyName, cacheSettingsPropertyName);
            }
            return cacheSettingsPropertyName;

        }
        //---------------------------------------------------------------------
        protected abstract void LoadSettingsProperties(TSettings settings);
        protected abstract void SaveSettingsProperties(TSettings settings);
        //---------------------------------------------------------------------
        protected void OnPropertyChanged(string propertyName)
        {
            IsPropertiesChanged = true;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        //---------------------------------------------------------------------
        public IComplexSettingsProperties Parent
        {
            get { return _parent; }
        }
        IComplexSettingsProperties IInternalSettingsProperties.Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        //---------------------------------------------------------------------
        public bool IsPropertiesChanged
        {
            get { return _propertiesChanged; }
            private set
            {
                if (_actionState != ActionState.Loading)
                { _propertiesChanged = value; }
            }
        }
        //---------------------------------------------------------------------
        public void Load(TSettings settings)
        {
            if (settings == null)
            { throw new ArgumentNullException(nameof(settings)); }

            if (_actionState == ActionState.Init)
            {
                _actionState = ActionState.Loading;

                try
                {
                    LoadSettingsProperties(settings);
                }
                finally
                { _actionState = ActionState.Init; }
            }
        }
        public void Save(TSettings settings)
        {
            if (settings == null)
            { throw new ArgumentNullException(nameof(settings)); }

            if (_actionState == ActionState.Init)
            {
                _actionState = ActionState.Saving;

                try
                {
                    SaveSettingsProperties(settings);

                    IsPropertiesChanged = false;
                }
                finally
                { _actionState = ActionState.Init; }
            }
        }
        //---------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        //---------------------------------------------------------------------
    }
}