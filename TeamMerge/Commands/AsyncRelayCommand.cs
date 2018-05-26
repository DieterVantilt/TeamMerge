using System;
using System.Threading.Tasks;

namespace TeamMerge.Commands
{
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
            await _canExecuteAsync((T)parameter);
        }
    }

    public class AsyncRelayCommand
        : RelayCommandBase
    {
        private readonly Func<Task> _canExecuteAsync;

        public AsyncRelayCommand(Func<Task> canExecuteAsync)
            : this(canExecuteAsync, () => true)
        {
        }

        public AsyncRelayCommand(Func<Task> canExecuteAsync, Func<bool> canExecute)
            : base(canExecute)
        {
            _canExecuteAsync = canExecuteAsync;
        }

        public async override void Execute(object parameter)
        {
            await _canExecuteAsync();
        }
    }
}