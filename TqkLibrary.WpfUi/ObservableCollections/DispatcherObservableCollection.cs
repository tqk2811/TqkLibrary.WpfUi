using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TqkLibrary.WpfUi.ObservableCollections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DispatcherObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public event Action<IEnumerable<T>>? OnItemsCleared;
        /// <summary>
        /// 
        /// </summary>
        public event Action<T>? OnItemAdded;
        /// <summary>
        /// 
        /// </summary>
        public event Action<T>? OnItemRemoved;


        /// <summary>
        /// 
        /// </summary>
        public SynchronizationContext SynchronizationContext { get; }
        /// <summary>
        /// 
        /// </summary>
        public Dispatcher Dispatcher { get; }
        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        protected override void ClearItems()
        {
            var tmp = this.ToList();
            base.ClearItems();
            if (OnItemRemoved is not null)
                tmp.ForEach(x => this.OnItemRemoved?.Invoke(x));
            OnItemsCleared?.Invoke(tmp);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual Task AddAsync(T item) => this.Dispatcher.TrueThreadInvokeAsync(() => this.Add(item));
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Task ClearAsync() => this.Dispatcher.TrueThreadInvokeAsync(() => this.Clear());
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual Task InsertAsync(int index, T item) => this.Dispatcher.TrueThreadInvokeAsync(() => this.Insert(index, item));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual Task<bool> RemoveAsync(T item) => this.Dispatcher.TrueThreadInvokeAsync(() => this.Remove(item));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual Task RemoveAtAsync(int index) => this.Dispatcher.TrueThreadInvokeAsync(() => this.RemoveAt(index));




        /// <summary>
        /// Sync this collection with datas
        /// </summary>
        /// <param name="datas"></param>
        public virtual void Sync(IEnumerable<T> datas)
        {
            var news = datas.Except(this).ToList();
            var deleteds = this.Except(datas).ToList();

            news.ForEach(x => this.Add(x));
            deleteds.ForEach(x => this.Remove(x));
        }
        /// <summary>
        /// Sync this collection with datas
        /// </summary>
        /// <param name="datas"></param>
        public virtual Task SyncAsync(IEnumerable<T> datas) => Dispatcher.TrueThreadInvokeAsync(() => Sync(datas));

    }
}
