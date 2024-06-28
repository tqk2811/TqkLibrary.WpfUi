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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected void NotifyPropertyChange([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion INotifyPropertyChanged

        /// <summary>
        /// 
        /// </summary>
        protected readonly Dispatcher Dispatcher;
        /// <summary>
        /// 
        /// </summary>
        public BaseViewModel() : this(Application.Current.Dispatcher)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BaseViewModel(Dispatcher dispatcher)
        {
            this.Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }
    }
}