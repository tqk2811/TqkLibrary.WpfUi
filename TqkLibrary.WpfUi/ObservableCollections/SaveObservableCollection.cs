using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace TqkLibrary.WpfUi.ObservableCollections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public class SaveObservableCollection<TData, TViewModel> : DispatcherObservableCollection<TViewModel>
      where TData : class
      where TViewModel : class, IViewModel<TData>
    {
        /// <summary>
        /// 
        /// </summary>
        public event Action<IEnumerable<TData>>? OnSave;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="func"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Load(IEnumerable<TData> datas, Func<TData, TViewModel> func)
        {
            if (datas is null) throw new ArgumentNullException(nameof(datas));
            if (func is null) throw new ArgumentNullException(nameof(func));

            this.Dispatcher.VerifyAccess();

            using var l = GetLockLoader();
            this.Clear();
            foreach (var item in datas)
            {
                this.Add(func(item));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual Task LoadAsync(IEnumerable<TData> datas, Func<TData, TViewModel> func)
            => this.Dispatcher.TrueThreadInvokeAsync(() => Load(datas, func));

        /// <summary>
        /// 
        /// </summary>
        protected bool IsLoaded => _lockCount == 0;

        private int _lockCount = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IDisposable GetLockLoader()
        {
            this.Dispatcher.VerifyAccess();
            return new LockHelper(this);
        }



        /// <summary>
        /// 
        /// </summary>
        public virtual void Save()
            => TriggerEventSave(this.Select(x => x.Data));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas"></param>
        protected virtual void TriggerEventSave(IEnumerable<TData> datas)
            => this.OnSave?.Invoke(datas);


        #region ObservableCollection
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, TViewModel item)
        {
            item.Change += ItemData_Change;
            base.InsertItem(index, item);
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void ClearItems()
        {
            foreach (var item in this) item.Change -= ItemData_Change;
            base.ClearItems();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            this[index].Change -= ItemData_Change;
            base.RemoveItem(index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, TViewModel item)
        {
            this[index].Change -= ItemData_Change;
            item.Change += ItemData_Change;
            base.SetItem(index, item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (IsLoaded) Save();
            base.OnCollectionChanged(e);
        }

        #endregion ObservableCollection

        protected virtual void OnItemChanged(TData data)
        {

        }
        private void ItemData_Change(object obj, TData data)
        {
            OnItemChanged(data);
            Save();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected void NotifyPropertyChange([CallerMemberName] string name = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(name));
        }



        private class LockHelper : IDisposable
        {
            readonly SaveObservableCollection<TData, TViewModel> _collection;
            public LockHelper(SaveObservableCollection<TData, TViewModel> collection)
            {
                this._collection = collection ?? throw new ArgumentNullException(nameof(collection));
                Interlocked.Increment(ref _collection._lockCount);
            }
            ~LockHelper()
            {
                Interlocked.Decrement(ref _collection._lockCount);
            }
            public void Dispose()
            {
                Interlocked.Decrement(ref _collection._lockCount);
                GC.SuppressFinalize(this);
            }
        }
    }
}