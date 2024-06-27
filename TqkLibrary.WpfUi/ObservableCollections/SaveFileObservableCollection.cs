using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using TqkLibrary.Data.Json;

namespace TqkLibrary.WpfUi.ObservableCollections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public class SaveFileObservableCollection<TData, TViewModel> : SaveObservableCollection<TData, TViewModel>, IDisposable
      where TData : class
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
        public SaveFileObservableCollection(string savePath, Func<TData, TViewModel> func, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (string.IsNullOrWhiteSpace(savePath)) throw new ArgumentNullException(nameof(savePath));
            if (func is null) throw new ArgumentNullException(nameof(func));

            _saveJsonData = new SaveJsonData<List<TData>>(savePath, jsonSerializerSettings);
            base.Load(_saveJsonData.Data, func);
            base.OnSave += SaveFileObservableCollection_OnSave;
        }
        /// <summary>
        /// 
        /// </summary>
        ~SaveFileObservableCollection()
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

        private void SaveFileObservableCollection_OnSave(IEnumerable<TData> datas)
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