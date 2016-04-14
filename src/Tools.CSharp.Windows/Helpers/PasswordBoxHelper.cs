using System;
using System.Windows;
using System.Windows.Controls;

namespace Tools.CSharp.Windows.Helpers
{
    public sealed class PasswordBoxHelper : Control, IPasswordBoxHelper
    {
        #region private
        private static readonly DependencyProperty _IsUpdatingPassword      = DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxHelper));

        private static bool _GetIsUpdatingPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(_IsUpdatingPassword);
        }
        private static void _SetIsUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(_IsUpdatingPassword, value);
        }

        private static void _HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;

            _SetIsUpdatingPassword(passwordBox, true);

            SetPassword(passwordBox, passwordBox.Password);

            var passwordBoxHelper = passwordBox.Tag as IPasswordBoxHelper;
            if (passwordBoxHelper != null)
            { passwordBoxHelper.Password = passwordBox.Password; }

            _SetIsUpdatingPassword(passwordBox, false);
        }
        private static void _OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = d as PasswordBox;
            if (passwordBox != null)
            {
                if (e.Property == IsBindingPasswordProperty)
                {
                    var oldValue = (bool)e.OldValue;
                    var newValue = (bool)e.NewValue;

                    if (oldValue)
                    { passwordBox.PasswordChanged -= _HandlePasswordChanged; }
                    if (newValue)
                    { passwordBox.PasswordChanged += _HandlePasswordChanged; }
                }
                else if (e.Property == PasswordProperty)
                {
                    if (!GetIsBindingPassword(passwordBox))
                        return;

                    passwordBox.PasswordChanged -= _HandlePasswordChanged;

                    if (!_GetIsUpdatingPassword(passwordBox))
                    { passwordBox.Password = (string)e.NewValue; }

                    passwordBox.PasswordChanged += _HandlePasswordChanged;
                }
            }
            else
            {
                var passwordBoxHelperExtended = d as PasswordBoxHelper;
                if (passwordBoxHelperExtended != null)
                {
                    if (e.Property == PasswordProperty)
                    { passwordBoxHelperExtended.Password = (string)e.NewValue; }
                }
            }
        }
        #endregion
        //---------------------------------------------------------------------
        public static readonly DependencyProperty PasswordProperty          = DependencyProperty.Register("Password", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(string.Empty, _OnPropertyChanged));
        public static readonly DependencyProperty IsBindingPasswordProperty = DependencyProperty.RegisterAttached("IsBindingPassword", typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, _OnPropertyChanged));
        //---------------------------------------------------------------------
        public static bool GetIsBindingPassword(DependencyObject dp)
        {
            if (dp == null)
            { throw new ArgumentNullException(nameof(dp)); }

            return (bool)dp.GetValue(IsBindingPasswordProperty);
        }
        public static void SetIsBindingPassword(DependencyObject dp, bool value)
        {
            if (dp == null)
            { throw new ArgumentNullException(nameof(dp)); }

            dp.SetValue(IsBindingPasswordProperty, value);
        }
        //---------------------------------------------------------------------
        public static string GetPassword(DependencyObject dp)
        {
            if (dp == null)
            { throw new ArgumentNullException(nameof(dp)); }

            return (string)dp.GetValue(PasswordProperty);
        }
        public static void SetPassword(DependencyObject dp, string value)
        {
            if (dp == null)
            { throw new ArgumentNullException(nameof(dp)); }

            dp.SetValue(PasswordProperty, value);
        }
        //---------------------------------------------------------------------
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        //---------------------------------------------------------------------
    }
}