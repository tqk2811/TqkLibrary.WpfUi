using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TqkLibrary.WpfUi.Interfaces;

namespace TqkLibrary.WpfUi.ObservableCollections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DispatcherObservableCollection<T> : ObservableCollection<T>, IMainThread
    {
        Cursor? _Cursor = null;
        public virtual Cursor? Cursor
        {
            get { return _Cursor; }
            set { _Cursor = value; OnPropertyChanged(nameof(Cursor)); }
        }

        public event Action<IEnumerable<T>>? OnItemsCleared;
        public event Action<T>? OnItemAdded;
        public event Action<T>? OnItemRemoved;

        public SynchronizationContext SynchronizationContext { get; }
        public Dispatcher Dispatcher { get; }


        public DispatcherObservableCollection()
            : this(
                  Application.Current.Dispatcher,
                  SynchronizationContext.Current ?? throw new InvalidOperationException("Must create in main thread")
                  )//will create on main thread
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="synchronizationContext"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DispatcherObservableCollection(Dispatcher dispatcher, SynchronizationContext synchronizationContext)
        {
            this.Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.SynchronizationContext = synchronizationContext ?? throw new ArgumentNullException(nameof(synchronizationContext));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is not null && OnItemAdded is not null)
                    {
                        foreach (var item in e.NewItems.OfType<T>())
                        {
                            OnItemAdded?.Invoke(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is not null && OnItemRemoved is not null)
                    {
                        foreach (var item in e.OldItems.OfType<T>())
                        {
                            OnItemRemoved?.Invoke(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems is not null && OnItemAdded is not null)
                    {
                        foreach (var item in e.NewItems.OfType<T>())
                        {
                            OnItemAdded?.Invoke(item);
                        }
                    }
                    if (e.OldItems is not null && OnItemRemoved is not null)
                    {
                        foreach (var item in e.OldItems.OfType<T>())
                        {
                            OnItemRemoved?.Invoke(item);
                        }
                    }
                    break;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string name = "") => OnPropertyChanged(new PropertyChangedEventArgs(name));

        protected override void ClearItems()
        {
            var tmp = this.ToList();
            base.ClearItems();
            if (OnItemRemoved is not null)
                tmp.ForEach(x => this.OnItemRemoved?.Invoke(x));
            OnItemsCleared?.Invoke(tmp);
        }

    }
}
