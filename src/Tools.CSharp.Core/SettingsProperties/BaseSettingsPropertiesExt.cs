using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Tools.CSharp.SettingsProperties
{
    public abstract class BaseSettingsPropertiesExt<TSettings> : BaseSettingsProperties<TSettings>, IDisposable
        where TSettings : class
    {
        #region private
        private readonly Dictionary<ISettingsProperties, Dictionary<string, Action>> _actionsSettingsPropertyNames = new Dictionary<ISettingsProperties, Dictionary<string, Action>>();
        //---------------------------------------------------------------------
        private bool _isDisposed;
        //---------------------------------------------------------------------
        private void _subscribeSettingsPropertiesAllEvents(ISettingsProperties settingsProperties, bool addEvents)
        {
            if (settingsProperties != null)
            {
                if (addEvents)
                { settingsProperties.PropertyChanged += _settingsPropertiesOnPropertyChanged; }
                else
                { settingsProperties.PropertyChanged -= _settingsPropertiesOnPropertyChanged; }
            }
        }
        private void _settingsPropertiesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dictionary<string, Action> actionSettingsPropertyNames;
            if (_actionsSettingsPropertyNames.TryGetValue((ISettingsProperties) sender, out actionSettingsPropertyNames))
            {
                Action actionSettingPropertyName;
                if(actionSettingsPropertyNames.TryGetValue(e.PropertyName, out actionSettingPropertyName))
                { actionSettingPropertyName(); }
            }
        }
        //---------------------------------------------------------------------
        private Dictionary<string, Action> _getActionsSettingsPropertyNames(ISettingsProperties settingsProperties)
        {
            Dictionary<string, Action> actionSettingsPropertyNames;

            if (!_actionsSettingsPropertyNames.TryGetValue(settingsProperties, out actionSettingsPropertyNames))
            {
                actionSettingsPropertyNames = new Dictionary<string, Action>();
                _actionsSettingsPropertyNames.Add(settingsProperties, actionSettingsPropertyNames);

                _subscribeSettingsPropertiesAllEvents(settingsProperties, true);
            }

            return actionSettingsPropertyNames;
        }
        #endregion
        #region protected
        protected void AddActionSettingsProperties(ISettingsProperties settingsProperties, string propertyName, Action action)
        {
            if(settingsProperties == null)
            { throw new ArgumentNullException(nameof(settingsProperties)); }
            if(string.IsNullOrWhiteSpace(propertyName))
            { throw new ArgumentNullException(nameof(propertyName)); }
            if(action == null)
            { throw new ArgumentNullException(nameof(action)); }

            var actionSettingsPropertyNames = _getActionsSettingsPropertyNames(settingsProperties);
            actionSettingsPropertyNames[propertyName] = action;
        }
        protected void AddActionsSettingsProperties(ISettingsProperties settingsProperties, params Tuple<string, Action>[] actionsPropertyNames)
        {
            if(settingsProperties == null)
            { throw new ArgumentNullException(nameof(settingsProperties)); }
            if(actionsPropertyNames == null)
            { throw new ArgumentNullException(nameof(actionsPropertyNames)); }

            var actionSettingsPropertyNames = _getActionsSettingsPropertyNames(settingsProperties);
            foreach (var actionsPropertyName in actionsPropertyNames.Where(actionsPropertyName => !string.IsNullOrWhiteSpace(actionsPropertyName.Item1) && actionsPropertyName.Item2 != null))
            {
                actionSettingsPropertyNames[actionsPropertyName.Item1] = actionsPropertyName.Item2;
            }
        }
        //---------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (!_isDisposed && disposing)
                {
                    foreach (var actionsSettingsPropertyName in _actionsSettingsPropertyNames)
                    {
                        _subscribeSettingsPropertiesAllEvents(actionsSettingsPropertyName.Key, false);
                    }
                }
            }
            finally
            { _isDisposed = true; }
        }
        #endregion
        ~BaseSettingsPropertiesExt()
        {
            Dispose(false);
        }

        //---------------------------------------------------------------------
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
        //---------------------------------------------------------------------
    }
}