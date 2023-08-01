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
using System.Timers;

namespace TqkLibrary.WpfUi.ObservableCollection
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
        public event Action<IEnumerable<TData>> OnSave;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="func"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Load(IEnumerable<TData> datas, Func<TData, TViewModel> func)
        {
            if (datas is null) throw new ArgumentNullException(nameof(datas));
            if (func is null) throw new ArgumentNullException(nameof(func));

            IsLoaded = false;
            try
            {
                this.Clear();
                foreach(var item in datas)
                {
                    this.Add(func(item));
                }
            }
            finally
            {
                IsLoaded = true;
            }
        }

        private bool IsLoaded = false;

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            this.OnSave?.Invoke(this.Select(x => x.Data));
        }

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

        private void ItemData_Change(object obj, TData data)
        {
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
    }
}