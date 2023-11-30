using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Markup;

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
        readonly List<TViewModel> _datas = new List<TViewModel>();

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<TViewModel> ViewModels => _datas;

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

            _datas.Clear();
            foreach (var data in datas)
            {
                _datas.Add(func(data));
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
            => this.Dispatcher.TrueThreadInvokeAsync(() => Load(datas, func));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Show(Func<TViewModel, bool> func)
        {
            if (func is null) throw new ArgumentNullException(nameof(func));
            this.Dispatcher.VerifyAccess();

            using var l = GetLockLoader();
            this.Clear();
            foreach (var data in _datas)
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
            => this.Dispatcher.TrueThreadInvokeAsync(() => Show(func));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public virtual void ShowGroup(TId id)
            => Show(x => x.Data?.GroupId?.Equals(id) == true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task ShowGroupAsync(TId id)
            => ShowAsync(x => x.Data?.GroupId?.Equals(id) == true);

        /// <summary>
        /// 
        /// </summary>
        public override void Save()
        {
            TriggerEventSave(_datas.Select(x => x.Data));
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
                        _datas.Add(item);
                    }
                    else
                    {
                        int data_insert_index = _datas.IndexOf(targetInsert);
                        _datas.Insert(data_insert_index, item);
                    }
                    base.InsertItem(index, item);
                }
                else
                {
                    //search mode, just add to last
                    _datas.Add(item);
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
                    var new_data_index = _datas.IndexOf(@new);
                    _datas.Remove(old);
                    _datas.Insert(new_data_index, old);

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
                _datas.Remove(this[index]);
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
                int index_data_oldItem = _datas.IndexOf(oldItem);
                _datas.Remove(oldItem);
                _datas.Insert(index_data_oldItem, item);

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
                    _datas.Remove(item);
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