using System;
using System.Windows.Input;

namespace TaxChargingWPF.Services
{
    public class Command : ICommand
    {
        private readonly Action<object> methodCommand;
        private readonly Predicate<object> canExecute;

        public Command(Action<object> methodCommand, Predicate<object> canExecute = null)
        {
            this.methodCommand = methodCommand;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            methodCommand?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
