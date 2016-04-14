using System;
using System.Windows;
using System.Windows.Input;

namespace Tools.CSharp.Windows.Commands
{
    public class CommandReference : Freezable, ICommand
    {
        #region private
        private static void _OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var commandReference = d as CommandReference;
            if (commandReference != null)
            {
                var oldCommand = e.OldValue as ICommand;
                if (oldCommand != null)
                { oldCommand.CanExecuteChanged -= commandReference.CanExecuteChanged; }

                var newCommand = e.NewValue as ICommand;
                if (newCommand != null)
                { newCommand.CanExecuteChanged += commandReference.CanExecuteChanged; }
            }
        }
        #endregion
        #region protected
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
        #endregion
        //---------------------------------------------------------------------
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandReference), new PropertyMetadata(_OnCommandChanged));
        //---------------------------------------------------------------------
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        //---------------------------------------------------------------------
        public bool CanExecute(object parameter)
        {
            var command = Command;
            if (command != null)
                return command.CanExecute(parameter);
            return false;
        }
        public void Execute(object parameter)
        {
            Command.Execute(parameter);
        }
        //---------------------------------------------------------------------
        public event EventHandler CanExecuteChanged;
        //---------------------------------------------------------------------
    }
}