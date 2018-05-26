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

    public class AsyncRelayCommand<T>
        : RelayCommandBase<T>
    {
        private readonly Func<T, Task> _canExecuteAsync;

        public AsyncRelayCommand(Func<T, Task> canExecuteAsync) 
            : this(canExecuteAsync, x => true)
        {
        }

        public AsyncRelayCommand(Func<T, Task> canExecuteAsync, Func<T, bool> canExecute)
            : base(canExecute)
        {
            _canExecuteAsync = canExecuteAsync;
        }

        public async override void Execute(object parameter)
        {
            await _canExecuteAsync((T) parameter);
        }
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
}
