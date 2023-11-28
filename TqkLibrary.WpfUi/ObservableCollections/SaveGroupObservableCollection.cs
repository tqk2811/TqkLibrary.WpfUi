using System;
using System.Collections.Generic;
using System.Linq;
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
        readonly List<TViewModel> _datas = new List<TViewModel>();

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TViewModel> ViewModels => _datas;

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

            _datas.Clear();
            _datas.AddRange(datas.Select(func));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public override Task LoadAsync(IEnumerable<TData> datas, Func<TData, TViewModel> func)
        {
            if (datas is null) throw new ArgumentNullException(nameof(datas));
            if (func is null) throw new ArgumentNullException(nameof(func));

            _datas.Clear();
            _datas.AddRange(datas.Select(func));

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        public virtual void Show(Func<TViewModel, bool> func)
        {
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
        public virtual async Task ShowAsync(Func<TViewModel, bool> func)
        {
            using var l = GetLockLoader();
            await this.ClearAsync();
            foreach (var data in _datas)
            {
                if (func(data))
                    await this.AddAsync(data);
            }
        }

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
    }
}