using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TqkLibrary.Data.Json;

namespace TqkLibrary.WpfUi.ObservableCollections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public class SaveFileGroupObservableCollection<TId, TData, TViewModel> : SaveGroupObservableCollection<TId, TData, TViewModel>, IDisposable
        where TData : class, IItemData<TId>
        where TViewModel : class, IViewModel<TData>
    {
        readonly SaveJsonData<List<TData>> _saveJsonData;

        /// <summary>
        /// 
        /// </summary>
        public ISaveJsonDataControl SaveJsonData => _saveJsonData;

        /// <summary>
        /// 
        /// </summary>
        public bool IsAutoSave { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="func"></param>
        /// <param name="jsonSerializerSettings"></param>
        public SaveFileGroupObservableCollection(string savePath, Func<TData, TViewModel> func, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (string.IsNullOrWhiteSpace(savePath)) throw new ArgumentNullException(nameof(savePath));
            if (func is null) throw new ArgumentNullException(nameof(func));

            _saveJsonData = new SaveJsonData<List<TData>>(savePath, jsonSerializerSettings);
            this.Load(_saveJsonData.Data, func);
            this.OnSave += SaveFileGroupObservableCollection_OnSave;
        }
        /// <summary>
        /// 
        /// </summary>
        ~SaveFileGroupObservableCollection()
        {
            _saveJsonData.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _saveJsonData.Dispose();
            GC.SuppressFinalize(this);
        }

        private void SaveFileGroupObservableCollection_OnSave(IEnumerable<TData> datas)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                _saveJsonData.Data.Clear();
                _saveJsonData.Data.AddRange(datas);
                if (IsAutoSave)
                {
                    _saveJsonData.TriggerSave();
                }
            });
        }
    }
}