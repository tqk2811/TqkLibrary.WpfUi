using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace TqkLibrary.WpfUi
{
  public abstract class BaseViewModel : INotifyPropertyChanged
  {
    #region INotifyPropertyChanged

    protected void NotifyPropertyChange([CallerMemberName] string name = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion INotifyPropertyChanged

    protected readonly Dispatcher dispatcher;

    public BaseViewModel() : this(Application.Current.Dispatcher)
    {
    }

    public BaseViewModel(Dispatcher dispatcher)
    {
      this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }
  }
}