using System;
using System.Windows.Input;


namespace Test
{
  public class RelayCommand<T> : ICommand
  {
    #region Constructors and fields
    /// <summary>
    ///   Initializes a new instance of a <see cref="RelayCommand{T}"/>.
    /// </summary>
    /// <param name="execute">
    ///   Specifies the delegate to execute when the <see cref="Execute"/> method is called on the command.
    /// </param>
    public RelayCommand(Action<T> execute) : this(execute, null) { }

    /// <summary>
    ///   Initializes a new instance of a <see cref="RelayCommand{T}"/>.
    /// </summary>
    /// <param name="execute">
    ///   Specifies the delegate to execute when the <see cref="Execute"/> method is called on the command.
    /// </param>
    /// <param name="canExecute">
    ///   Specifies the delegate, which indicates if the command can be executed via the <see cref="Execute"/> method.
    /// </param>
    public RelayCommand(Action<T> execute, Predicate<T> canExecute)
    {
      _execute = execute ?? throw new ArgumentNullException(nameof(execute));
      _canExecute = canExecute;
    }

    /// <summary>
    ///   Holds the delegate, which is invoked when the <see cref="Execute"/> method is invoked on the command.
    /// </summary>
    private readonly Action<T> _execute;

    /// <summary>
    ///   Holds the delegate, which is invoked to check if the <see cref="Execute"/> method can be called.
    /// </summary>
    private readonly Predicate<T> _canExecute;
    #endregion


    #region ICommand implementation
    /// <inheritdoc />
    public bool CanExecute(object parameter = null) => _canExecute?.Invoke((T)parameter) ?? true;

    /// <inheritdoc />
    public event EventHandler CanExecuteChanged
    {
      add => CommandManager.RequerySuggested += value;
      remove => CommandManager.RequerySuggested -= value;
    }

    /// <inheritdoc />
    public void Execute(object parameter = null) => _execute((T)parameter);
    #endregion
  }

  public class RelayCommand : RelayCommand<object>
  {
    /// <summary>
    ///   Initializes a new instance of a <see cref="RelayCommand"/>.
    /// </summary>
    /// <param name="execute">
    ///   Specifies the delegate to execute when the <see cref="RelayCommand.Execute"/> method is called on the command.
    /// </param>
    public RelayCommand(Action<object> execute) : base(execute, null) { }


    /// <summary>
    ///   Initializes a new instance of a <see cref="RelayCommand"/>.
    /// </summary>
    /// <param name="execute">
    ///   Specifies the delegate to execute when the <see cref="RelayCommand.Execute"/> method is called on the command.
    /// </param>
    /// <param name="canExecute">
    ///   Specifies the delegate, which indicates if the command can be executed via the <see cref="RelayCommand.Execute"/> method.
    /// </param>
    public RelayCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute) { }
  }
}
