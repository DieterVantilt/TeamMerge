using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TeamMerge.Commands
{
    public interface IRelayCommand
        : ICommand
    {
        void RaiseCanExecuteChanged();
    }
    
    public class RelayCommand<T>
        : RelayCommandBase<T>
    {
        private readonly Action<T> _execute;

        public RelayCommand(Action<T> execute)
            : this(execute, x => true)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
            : base(canExecute)
        {
            _execute = execute;
        }

        public override void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }

    public abstract class RelayCommandBase<T>
        : IRelayCommand
    {
        private readonly Func<T, bool> _canExecute;

        public RelayCommandBase(Func<T, bool> canExecute)
        {
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute((T)parameter); ;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;
    }

    public class RelayCommand
        : RelayCommandBase
    {
        private readonly Action _execute;

        public RelayCommand(Action execute)
            : this(execute, () => true)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
            : base(canExecute)
        {
            _execute = execute;
        }

        public override void Execute(object parameter)
        {
            _execute();
        }
    }

    public abstract class RelayCommandBase
        : IRelayCommand
    {
        private readonly Func<bool> _canExecute;

        public RelayCommandBase(Func<bool> canExecute)
        {
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;
    }
}
