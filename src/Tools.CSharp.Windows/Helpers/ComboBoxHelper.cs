using System.Windows;
using System.Windows.Controls;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Windows.Helpers
{
    public sealed class ComboBoxHelper
    {
        #region private
        private static void _OnMaxLengthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var comboBox = obj as ComboBox;
            if (comboBox != null)
            {
                _SetComboBoxMaxLength(comboBox, args.NewValue);

                comboBox.Loaded += (o, e) => { _SetComboBoxMaxLength(comboBox, args.NewValue); };
            }
        }
        private static void _SetComboBoxMaxLength(ComboBox comboBox, object maxLength)
        {
            var textBox = comboBox?.GetVisualChild<TextBox>();
            textBox?.SetValue(TextBox.MaxLengthProperty, maxLength);
        }
        #endregion
        //---------------------------------------------------------------------
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached("MaxLength", typeof(int), typeof(ComboBoxHelper), new UIPropertyMetadata(_OnMaxLengthChanged));
        //---------------------------------------------------------------------
        public static int GetMaxLength(DependencyObject obj)
        {
            return (int)obj.GetValue(MaxLengthProperty);
        }
        public static void SetMaxLength(DependencyObject obj, int value)
        {
            obj.SetValue(MaxLengthProperty, value);
        }
        //---------------------------------------------------------------------
    }
}
