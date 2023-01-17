using System;
using System.Windows.Input;

namespace BasicSample;

public class ActionCommand : ICommand
{
    private readonly Action _action;
    private readonly Func<bool> _canExecute;

    public ActionCommand(Action action, Func<bool> canExecute)
    {
        _action = action;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute.Invoke();
    }

    public void Execute(object? parameter)
    {
        _action();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}