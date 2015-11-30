using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace R54IN0
{
    public class CommandHandler : ICommand
    {
        Action<object> _execute;
        Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public CommandHandler(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter);
            else
                return false;
        }

        public void Execute(object parameter)
        {
            if (_canExecute != null)
                _execute(parameter);
        }

        public void UpdateCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}