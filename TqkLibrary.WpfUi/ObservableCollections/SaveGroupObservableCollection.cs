using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
        readonly Dictionary<TId, List<TViewModel>> _dict_datas = new Dictionary<TId, List<TViewModel>>();

        List<TViewModel> _EnsureCreateDictKey(TId key)
        {
            this.Dispatcher.VerifyAccess();
            if (!_dict_datas.ContainsKey(key))
                _dict_datas.Add(key, new List<TViewModel>());
            return _dict_datas[key];
        }
        void _CleanEmptyDict()
        {
            this.Dispatcher.VerifyAccess();
            foreach (var pair in _dict_datas.Where(x => x.Value.Count == 0).ToList())
            {
                _dict_datas.Remove(pair.Key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TViewModel> ViewModels => _dict_datas.SelectMany(x => x.Value);

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

            _dict_datas.Clear();
            foreach (var data in datas)
            {
                _EnsureCreateDictKey(data.GroupId).Add(func(data));
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
            foreach (var data in _dict_datas.SelectMany(x => x.Value))
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
            TriggerEventSave(_dict_datas.SelectMany(x => x.Value).Select(x => x.Data));
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
                var datas = _EnsureCreateDictKey(item.Data.GroupId);

                if (this.GroupBy(x => x.Data.GroupId).Count() == 1 &&
                    this.First().Data.GroupId?.Equals(item.Data.GroupId) == true)
                {
                    //all items same group
                    base.InsertItem(index, item);

                    datas.Clear();
                    datas.AddRange(this);
                }
                else
                {
                    //search mode, just add to last
                    datas.Add(item);
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
            if (this.IsLoaded)
            {
                if (this.GroupBy(x => x.Data.GroupId).Count() == 1)
                {
                    //all same group
                    base.MoveItem(oldIndex, newIndex);

                    var datas = _EnsureCreateDictKey(this.First().Data.GroupId);
                    datas.Clear();
                    datas.AddRange(this);
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
            _CleanEmptyDict();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            if (this.IsLoaded)
            {
                if (this.GroupBy(x => x.Data.GroupId).Count() == 1)
                {
                    var datas = _EnsureCreateDictKey(this.First().Data.GroupId);
                    //all same group
                    base.RemoveItem(index);//maybe last items

                    datas.Clear();
                    datas.AddRange(this);
                }
                else
                {
                    //search mode
                    var datas = _EnsureCreateDictKey(this[index].Data.GroupId);
                    datas.Remove(this[index]);
                    base.RemoveItem(index);
                }
            }
            else
            {
                //loading
                base.RemoveItem(index);
            }
            _CleanEmptyDict();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, TViewModel item)
        {
            if (this.IsLoaded)
            {
                var datas = _EnsureCreateDictKey(item.Data.GroupId);

                if (this.GroupBy(x => x.Data.GroupId).Count() == 1 &&
                    this.First().Data.GroupId?.Equals(item.Data.GroupId) == true)
                {
                    //all same group
                    base.SetItem(index, item);

                    datas.Clear();
                    datas.AddRange(this);
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
            _CleanEmptyDict();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void ClearItems()
        {
            if (this.IsLoaded)
            {
                if (this.GroupBy(x => x.Data.GroupId).Count() == 1)
                {
                    //all same group
                    var datas = _EnsureCreateDictKey(this.First().Data.GroupId);
                    datas.Clear();
                    base.ClearItems();
                }
                else
                {
                    //search mode
                    foreach (var item in this)
                    {
                        var datas = _EnsureCreateDictKey(item.Data.GroupId);
                        datas.Remove(item);
                    }
                    base.ClearItems();
                }
            }
            else
            {
                base.ClearItems();
            }
            _CleanEmptyDict();
        }
    }
}