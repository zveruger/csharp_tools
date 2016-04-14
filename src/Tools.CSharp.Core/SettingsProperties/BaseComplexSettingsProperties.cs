using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Tools.CSharp.SettingsProperties
{
    public abstract class BaseComplexSettingsProperties<TSettings> : BaseDisposable, IComplexSettingsProperties, IInternalSettingsProperties
        where TSettings : class
    {
        #region private 
        private readonly Dictionary<Type, Lazy<ISettingsProperties>> _cacheLazySettingsProperties = new Dictionary<Type, Lazy<ISettingsProperties>>();
        private readonly TSettings _settings;
        //---------------------------------------------------------------------
        private IComplexSettingsProperties _parent;
        //---------------------------------------------------------------------
        private TSettingsProperties _createSettingsProperties<TSettingsProperties>()
            where TSettingsProperties : ISettingsProperties, new()
        {
            var settingsProperties = new TSettingsProperties();
            return _initializeSettingsProperties(settingsProperties);

        }
        private TSettingsProperties _createSettingsProperties<TSettingsProperties>(Func<TSettingsProperties> functor)
            where TSettingsProperties : ISettingsProperties
        {
            var settingsProperties = functor();
            return _initializeSettingsProperties(settingsProperties);
        }
        //---------------------------------------------------------------------
        private TSettingsProperties _initializeSettingsProperties<TSettingsProperties>(TSettingsProperties settingsProperties)
            where TSettingsProperties : ISettingsProperties
        {
            var internalSettingsProperties = settingsProperties as IInternalSettingsProperties;
            if (internalSettingsProperties != null)
            { internalSettingsProperties.Parent = this; }
            //-----------------------------------------------------------------
            _loadSettingsProperties(settingsProperties);
            //-----------------------------------------------------------------
            _subscribeSettingsPropertiesAllEvents(settingsProperties, true);
            //-----------------------------------------------------------------
            return settingsProperties;
        }
        //---------------------------------------------------------------------
        private void _loadSettingsProperties<TSettingsProperties>(TSettingsProperties settingsProperties)
            where TSettingsProperties : ISettingsProperties
        {
            var complexSettingsProperties = settingsProperties as IComplexSettingsProperties;
            if (complexSettingsProperties != null)
            { complexSettingsProperties.Load(); }
            else
            {
                var simpleSettingsProperties = settingsProperties as BaseSettingsProperties<TSettings>;
                simpleSettingsProperties?.Load(_settings);
            }
        }
        private void _saveSettingsProperties<TSettingsProperties>(TSettingsProperties settingsProperties)
            where TSettingsProperties : ISettingsProperties
        {
            var complexSettingsProperties = settingsProperties as IComplexSettingsProperties;
            if (complexSettingsProperties != null)
            { complexSettingsProperties.Save(); }
            else
            {
                var simpleSettingsProperties = settingsProperties as BaseSettingsProperties<TSettings>;
                simpleSettingsProperties?.Save(_settings);
            }
        }
        private void _disposeSettingsProperties<TSettingsProperties>(TSettingsProperties settingsProperties)
            where TSettingsProperties : ISettingsProperties
        {
            if (settingsProperties != null)
            {
                _subscribeSettingsPropertiesAllEvents(settingsProperties, false);

                var disposableSettingsProperties = settingsProperties as IDisposable;
                disposableSettingsProperties?.Dispose();
            }
        }
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
            PropertyChanged?.Invoke(sender, e);
        }
        #endregion
        #region protected
        protected void AddSettingsProperties<TSettingsProperties>()
            where TSettingsProperties : ISettingsProperties, new()
        {
            var key = typeof(TSettingsProperties);
            Lazy<ISettingsProperties> lazySettingsProperties;
            
            if (!_cacheLazySettingsProperties.TryGetValue(key, out lazySettingsProperties))
            {
                lazySettingsProperties = new Lazy<ISettingsProperties>(() => _createSettingsProperties<TSettingsProperties>());
                _cacheLazySettingsProperties.Add(key, lazySettingsProperties);
            }
        }
        protected void AddSettingsProperties<TSettingsProperties>(Func<TSettingsProperties> functor)
            where TSettingsProperties : ISettingsProperties
        {
            if (functor == null)
            { throw new ArgumentNullException(nameof(functor)); }

            var key = typeof(TSettingsProperties);
            Lazy<ISettingsProperties> lazySettingsProperties;
            
            if (!_cacheLazySettingsProperties.TryGetValue(key, out lazySettingsProperties))
            {
                lazySettingsProperties = new Lazy<ISettingsProperties>(() => _createSettingsProperties(functor));
                _cacheLazySettingsProperties.Add(key, lazySettingsProperties);
            }
            else
            {
                if (lazySettingsProperties.IsValueCreated && lazySettingsProperties.Value == null)
                { _cacheLazySettingsProperties[key] = new Lazy<ISettingsProperties>(() => _createSettingsProperties(functor)); }
            }
        }
        //---------------------------------------------------------------------
        protected TSettingsProperties GetSettingsProperties<TSettingsProperties>()
            where TSettingsProperties : ISettingsProperties
        {
            var key = typeof(TSettingsProperties);
            Lazy<ISettingsProperties> lazySettingsProperties;

            if ((_cacheLazySettingsProperties.TryGetValue(key, out lazySettingsProperties)) && (lazySettingsProperties.Value is TSettingsProperties))
            { return (TSettingsProperties)lazySettingsProperties.Value; }

            return default(TSettingsProperties);
        }
        //---------------------------------------------------------------------
        protected virtual void LoadingSettingsProperties()
        { }
        protected virtual void LoadedSettingsProperties()
        { }
        protected virtual void SavingSettingsProperties()
        { }
        protected virtual void SavedSettingsProperties()
        { }
        //---------------------------------------------------------------------
        protected virtual void OnLoading()
        {
            Loading?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnLoaded()
        {
            Loaded?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnSaving()
        {
            Saving?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnSaved()
        {
            Saved?.Invoke(this, EventArgs.Empty);
        }
        //---------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    foreach (var keyValuePair in _cacheLazySettingsProperties)
                    {
                        var lazySettingsProperties = keyValuePair.Value;
                        if (lazySettingsProperties.IsValueCreated)
                        {
                            var settingsProperties = lazySettingsProperties.Value;
                            _disposeSettingsProperties(settingsProperties);
                        }
                    }
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        protected BaseComplexSettingsProperties(TSettings settings)
        {
            if (settings == null)
            { throw new ArgumentNullException(nameof(settings)); }

            _settings = settings;
        }

        //---------------------------------------------------------------------
        public TSettings Settings
        {
            get { return _settings; }
        }
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
            get
            {
                return _cacheLazySettingsProperties.Any(keyValuePair => {
                    var lazySettingsValue = keyValuePair.Value;
                    if (lazySettingsValue.IsValueCreated)
                    {
                        var settingsProperties = lazySettingsValue.Value;
                        return settingsProperties != null && settingsProperties.IsPropertiesChanged;
                    }
                    return false;
                });
            }
        }
        //---------------------------------------------------------------------
        public void Load()
        {
            OnLoading();
            LoadingSettingsProperties();

            foreach (var keyValuePair in _cacheLazySettingsProperties)
            {
                var lazySettingsProperties = keyValuePair.Value;
                if (lazySettingsProperties.IsValueCreated)
                {
                    var settingsProperties = lazySettingsProperties.Value;
                    _loadSettingsProperties(settingsProperties);
                }
            }

            LoadedSettingsProperties();
            OnLoaded();
        }
        public void Save()
        {
            OnSaving();
            SavingSettingsProperties();

            foreach (var keyValuePair in _cacheLazySettingsProperties)
            {
                var lazySettingsProperties = keyValuePair.Value;
                if (lazySettingsProperties.IsValueCreated)
                {
                    var settingsProperties = lazySettingsProperties.Value;
                    _saveSettingsProperties(settingsProperties);
                }
            }

            SavedSettingsProperties();
            OnSaved();
        }
        //---------------------------------------------------------------------
        public bool TrySave(object settingsProperties)
        {
            var complexSettingsProperties = settingsProperties as IComplexSettingsProperties;
            if (complexSettingsProperties != null)
            {
                complexSettingsProperties.Save();
                return true;
            }

            var internalSettingsProperties = settingsProperties as IInternalSettingsProperties;
            if (internalSettingsProperties != null)
            {
                var parent = internalSettingsProperties.Parent;
                if (parent != null)
                {
                    parent.Save();
                    return true;
                }

                var simpleSettingsProperties = settingsProperties as BaseSettingsProperties<TSettings>;
                simpleSettingsProperties?.Save(_settings);
                return true;
            }

            return false;
        }
        //---------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        //---------------------------------------------------------------------
        public event EventHandler Loading;
        public event EventHandler Loaded;
        public event EventHandler Saving;
        public event EventHandler Saved;
        //---------------------------------------------------------------------
    }
}