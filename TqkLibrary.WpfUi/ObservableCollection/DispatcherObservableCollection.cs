﻿using System;
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
        public event Action<IEnumerable<T>> OnItemsCleared;
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
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void ClearItems()
        {
            _ = Dispatcher.TrueThreadInvokeAsync(() =>
            {
                var tmp = this.ToList();
                base.ClearItems();
                OnItemsCleared?.Invoke(tmp);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, T item)
        {
            _ = Dispatcher.TrueThreadInvokeAsync(() => base.InsertItem(ReCalcIndexInsert(index), item));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            _ = Dispatcher.TrueThreadInvokeAsync(() => base.MoveItem(ReCalcIndexRemove(oldIndex), ReCalcIndexInsert(newIndex)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            _ = Dispatcher.TrueThreadInvokeAsync(() => base.RemoveItem(ReCalcIndexRemove(index)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, T item)
        {
            _ = Dispatcher.TrueThreadInvokeAsync(() => base.SetItem(ReCalcIndexRemove(index), item));
        }

        int ReCalcIndexInsert(int index) => Math.Max(0, Math.Min(index, this.Count));
        int ReCalcIndexRemove(int index) => Math.Max(0, Math.Min(index, this.Count - 1));

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
