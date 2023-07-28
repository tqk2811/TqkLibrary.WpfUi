using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TqkLibrary.WpfUi.ObservableCollection
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
        public override event NotifyCollectionChangedEventHandler CollectionChanged;
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
        public DispatcherObservableCollection() : this(Application.Current.Dispatcher, SynchronizationContext.Current)
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
            base.CollectionChanged += this.CollectionChanged;
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void ClearItems()
        {
            Dispatcher.InvokeAsync(() =>
            {
                var tmp = this.ToList();
                base.ClearItems();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null, tmp));
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, T item)
        {
            Dispatcher.InvokeAsync(() => base.InsertItem(ReCalcIndexInsert(index), item));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            Dispatcher.InvokeAsync(() => base.MoveItem(ReCalcIndexRemove(oldIndex), ReCalcIndexInsert(newIndex)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            Dispatcher.InvokeAsync(() => base.RemoveItem(ReCalcIndexRemove(index)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, T item)
        {
            Dispatcher.InvokeAsync(() => base.SetItem(ReCalcIndexRemove(index), item));
        }

        int ReCalcIndexInsert(int index) => Math.Max(0, Math.Min(index, this.Count));
        int ReCalcIndexRemove(int index) => Math.Max(0, Math.Min(index, this.Count - 1));
    }
}
