using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TqkLibrary.WpfUi.Interfaces;

namespace TqkLibrary.WpfUi.ObservableCollections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public class SaveGroupObservableCollection<TId, TData, TViewModel> : SaveObservableCollection<TData, TViewModel>
      where TData : class, IItemData<TId>
      where TViewModel : class, IViewModel<TData>
    {
        readonly List<TViewModel> _datas = new();

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<TViewModel> ViewModels => this._datas;

        /// <summary>
        /// 
        /// </summary>
        public virtual void ClearAll()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="func">Un used</param>
        /// <exception cref="ArgumentNullException"></exception>
        public override void Load(IEnumerable<TData> datas, Func<TData, TViewModel> func)
        {
            if (datas is null) throw new ArgumentNullException(nameof(datas));
            if (func is null) throw new ArgumentNullException(nameof(func));
            this.Dispatcher.VerifyAccess();

            this._datas.Clear();
            foreach (var data in datas)
            {
                this._datas.Add(func(data));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public override Task LoadAsync(IEnumerable<TData> datas, Func<TData, TViewModel> func)
            => this.Dispatcher.TrueThreadInvokeAsync(() => this.Load(datas, func));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Show(Func<TViewModel, bool> func)
        {
            if (func is null) throw new ArgumentNullException(nameof(func));
            this.Dispatcher.VerifyAccess();

            using var l = this.GetLockLoader();
            this.Clear();
            foreach (var data in this._datas)
            {
                if (func(data))
                    this.Add(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public virtual Task ShowAsync(Func<TViewModel, bool> func)
            => this.Dispatcher.TrueThreadInvokeAsync(() => this.Show(func));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public virtual void ShowGroup(TId id)
            => this.Show(x => x.Data?.GroupId?.Equals(id) == true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task ShowGroupAsync(TId id)
            => this.ShowAsync(x => x.Data?.GroupId?.Equals(id) == true);

        /// <summary>
        /// 
        /// </summary>
        public override void Save()
        {
            this.TriggerEventSave(this._datas.Select(x => x.Data));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, TViewModel item)
        {
            this.Dispatcher.VerifyAccess();
            if (item is null) throw new ArgumentNullException(nameof(item));

            if (this.IsLoaded)
            {
                var currentGroup = this.GroupBy(x => x.Data.GroupId);
                if (currentGroup.Count() == 1 &&
                    this.First().Data.GroupId?.Equals(item.Data.GroupId) == true)
                {
                    //all items same group
                    var targetInsert = currentGroup.First().Skip(0).FirstOrDefault();
                    if (targetInsert is null)
                    {
                        //add to last 
                        this._datas.Add(item);
                    }
                    else
                    {
                        int data_insert_index = this._datas.IndexOf(targetInsert);
                        this._datas.Insert(data_insert_index, item);
                    }
                    base.InsertItem(index, item);
                }
                else
                {
                    //search mode, just add to last
                    this._datas.Add(item);
                }
            }
            else
            {
                //loading
                base.InsertItem(index, item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex) return;
            this.Dispatcher.VerifyAccess();
            if (this.IsLoaded)
            {
                if (this.GroupBy(x => x.Data.GroupId).Count() == 1)
                {
                    //all same group
                    var old = this[oldIndex];
                    var @new = this[newIndex];
                    var new_data_index = this._datas.IndexOf(@new);
                    this._datas.Remove(old);
                    this._datas.Insert(new_data_index, old);

                    base.MoveItem(oldIndex, newIndex);
                }
                else
                {
                    //search mode => ignore
                }
            }
            else
            {
                //loading
                base.MoveItem(oldIndex, newIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            this.Dispatcher.VerifyAccess();
            if (this.IsLoaded)
            {
                this._datas.Remove(this[index]);
                base.RemoveItem(index);
            }
            else
            {
                //loading
                base.RemoveItem(index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, TViewModel item)
        {
            this.Dispatcher.VerifyAccess();
            if (this.IsLoaded)
            {
                var oldItem = this[index];
                int index_data_oldItem = this._datas.IndexOf(oldItem);
                this._datas.Remove(oldItem);
                this._datas.Insert(index_data_oldItem, item);

                if (this.GroupBy(x => x.Data.GroupId).Count() == 1 &&
                    this.First().Data.GroupId?.Equals(item.Data.GroupId) == true)
                {
                    //all same group
                    base.SetItem(index, item);
                }
                else
                {
                    //search mode, ignore
                }
            }
            else
            {
                //loading
                base.SetItem(index, item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void ClearItems()
        {
            this.Dispatcher.VerifyAccess();
            if (this.IsLoaded)
            {
                foreach (var item in this)
                {
                    this._datas.Remove(item);
                }
                base.ClearItems();
            }
            else
            {
                base.ClearItems();
            }
        }
    }
}