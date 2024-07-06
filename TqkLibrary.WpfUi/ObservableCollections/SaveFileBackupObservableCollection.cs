using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using TqkLibrary.Data.Json;
using TqkLibrary.WpfUi.Interfaces;

namespace TqkLibrary.WpfUi.ObservableCollections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public class SaveFileBackupObservableCollection<TData, TViewModel> : SaveObservableCollection<TData, TViewModel>, IDisposable
      where TData : class
      where TViewModel : class, IViewModel<TData>
    {
        readonly SaveJsonDataAutoBackup<List<TData>> _saveJsonData;

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
        public TimeSpan BackupInterval
        {
            get { return _saveJsonData.BackupInterval; }
            set { _saveJsonData.BackupInterval = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="backupDir"></param>
        /// <param name="func"></param>
        /// <param name="jsonSerializerSettings"></param>
        public SaveFileBackupObservableCollection(string savePath, string backupDir, Func<TData, TViewModel> func, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            if (string.IsNullOrWhiteSpace(savePath)) throw new ArgumentNullException(nameof(savePath));
            if (func is null) throw new ArgumentNullException(nameof(func));

            _saveJsonData = new SaveJsonDataAutoBackup<List<TData>>(savePath, backupDir, jsonSerializerSettings);
            base.Load(_saveJsonData.Data, func);
            base.OnSave += SaveFileObservableCollection_OnSave;
        }
        /// <summary>
        /// 
        /// </summary>
        ~SaveFileBackupObservableCollection()
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