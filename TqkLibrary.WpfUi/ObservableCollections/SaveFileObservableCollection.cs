using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TqkLibrary.Data.Json;
using TqkLibrary.WpfUi.Interfaces;

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
        public ISaveJsonDataControl SaveJsonData => this._saveJsonData;

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
        public SaveFileObservableCollection(string savePath, Func<TData, TViewModel> func, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            if (string.IsNullOrWhiteSpace(savePath)) throw new ArgumentNullException(nameof(savePath));
            if (func is null) throw new ArgumentNullException(nameof(func));

            this._saveJsonData = new SaveJsonData<List<TData>>(savePath, jsonSerializerSettings);
            base.Load(this._saveJsonData.Data, func);
            base.OnSave += SaveFileObservableCollection_OnSave;
        }


        /// <summary>
        /// 
        /// </summary>
        ~SaveFileObservableCollection()
        {
            this._saveJsonData.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this._saveJsonData.Dispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void SaveFileObservableCollection_OnSave(UpdateData updateData)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                if (updateData.NewDatas?.Any() == true || updateData.OldDatas?.Any() == true)
                {
                    this._saveJsonData.Data.Clear();
                    this._saveJsonData.Data.AddRange(updateData.CurrentDatas);
                }
                if (this.IsAutoSave)
                {
                    this._saveJsonData.TriggerSave();
                }
            });
        }
    }
}