using System;
using System.Windows.Input;

namespace OpcUaCore.Command
{
    class CommandBase :
        ICommand
    {
        private Action _execute;
        private Predicate<object> _canExecute;

        public CommandBase(Action execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;
            return _canExecute(parameter);
        }

        public void Execute(object parameter) => _execute();
        #endregion
    }
}
