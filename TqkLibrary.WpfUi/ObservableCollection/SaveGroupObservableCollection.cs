using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;

namespace TqkLibrary.WpfUi.ObservableCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IItemData<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        public TId GroupId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public class SaveGroupObservableCollection<TId, TData, TViewModel> : DispatcherObservableCollection<TViewModel>
      where TData : class, IItemData<TId>//data
      where TViewModel : class, IViewModel<TData>//viewmodel
    {
        /// <summary>
        /// 
        /// </summary>
        public string SavePath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAutoSave { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public double Interval
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSearchMode { get; private set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public TId CurrentGroupId { get; private set; }


        readonly Func<TData, TViewModel> func;
        readonly List<TData> datas = new List<TData>();
        private readonly Timer timer;
        private bool IsLoaded = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="savePath"></param>
        /// <param name="interval"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SaveGroupObservableCollection(Func<TData, TViewModel> func, string savePath, int interval = 500)
        {
            if (string.IsNullOrEmpty(savePath)) throw new ArgumentNullException(nameof(savePath));
            this.func = func ?? throw new ArgumentNullException(nameof(func));
            this.SavePath = savePath;
            timer = new Timer(interval)
            {
                AutoReset = false
            };
            timer.Elapsed += Timer_Elapsed;
            Load();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (datas)
            {
                using StreamWriter sw = new StreamWriter(SavePath, false);
                sw.Write(JsonConvert.SerializeObject(datas, Formatting.Indented));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            if (IsLoaded)
            {
                timer.Stop();
                timer.Start();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void ForceSave()
        {
            timer.Stop();
            try
            {
                Timer_Elapsed(null, null);
            }
            catch (Exception) { }
        }

        void Load()
        {
            if (File.Exists(this.SavePath))
            {
                try
                {
                    using StreamReader sr = new StreamReader(SavePath);
                    datas.AddRange(JsonConvert.DeserializeObject<List<TData>>(sr.ReadToEnd()));
                }
                catch (Exception) { }
            }
            IsLoaded = true;
        }
        /// <summary>
        /// 
        /// </summary>
        public void HideAll()
        {
            IsLoaded = false;
            IsSearchMode = false;
            this.Clear();
            IsLoaded = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TId> GetGroups()
        {
            return datas.GroupBy(x => x.GroupId).Select(x => x.Key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void ShowGroup(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            IsLoaded = false;
            this.CurrentGroupId = id;
            IsSearchMode = false;
            this.Clear();
            foreach (var item in datas.Where(x => id.Equals(x.GroupId))) this.Add(func?.Invoke(item));
            IsLoaded = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        public void Search(Func<TData, bool> predicate)
        {
            IsLoaded = false;
            IsSearchMode = true;
            this.Clear();
            foreach (var item in datas.Where(predicate)) this.Add(func?.Invoke(item));
            IsLoaded = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="newId"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void ChangeGroup(TData data, TId newId)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (newId == null) throw new ArgumentNullException(nameof(newId));
            IsLoaded = false;
            data.GroupId = newId;
            if (!IsSearchMode)
            {
                TViewModel viewModel = this.FirstOrDefault(x => data.Equals(x.Data));
                if (!this.Remove(viewModel))
                {
                    if (newId.Equals(CurrentGroupId))
                    {
                        if (viewModel == null) viewModel = func.Invoke(data);
                        this.Add(viewModel);
                    }
                }
            }
            IsLoaded = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TData> GetDataSave() => this.datas.ToList();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="restore_datas"></param>
        public void RestoreData(IEnumerable<TData> restore_datas)
        {
            lock (datas)
            {
                HideAll();
                datas.Clear();
                datas.AddRange(restore_datas);
                Save();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool IsAny(Predicate<TData> predicate)
        {
            lock (datas) return datas.Any(x => predicate.Invoke(x));
        }

        private void ItemData_Change(object obj, TData data)
        {
            if (IsAutoSave) Save();
        }


        #region ObservableCollection
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (IsAutoSave) Save();
            base.OnCollectionChanged(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        /// <exception cref="NotSupportedException"></exception>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (IsLoaded && IsSearchMode) throw new NotSupportedException("Can't change in search mode");
            if (IsLoaded)
            {
                lock (datas)
                {
                    var data_move = this[oldIndex].Data;
                    int data_move_index = datas.IndexOf(data_move);
                    var data_target = this[newIndex].Data;
                    int data_target_index = datas.IndexOf(data_target);
                    if (data_move_index >= 0 && data_target_index >= 0)
                    {
                        datas.Remove(data_move);
                        datas.Insert(data_target_index, data_move);
                    }
                    else throw new NotSupportedException("Item not found in base datas");
                }
            }
            base.MoveItem(oldIndex, newIndex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <exception cref="NotSupportedException"></exception>
        protected override void InsertItem(int index, TViewModel item)
        {
            if (IsLoaded && IsSearchMode) throw new NotSupportedException("Can't change in search mode");
            if (IsLoaded)
            {
                if (index == this.Count)
                {
                    lock (datas) datas.Add(item.Data);
                }
                else throw new NotSupportedException("InsertItem last postion only");
            }
            item.Change += ItemData_Change;
            base.InsertItem(index, item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        protected override void ClearItems()
        {
            if (IsLoaded && IsSearchMode) throw new NotSupportedException("Can't change in search mode");
            if (IsLoaded)
            {

            }
            foreach (var item in this) item.Change -= ItemData_Change;
            base.ClearItems();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="NotSupportedException"></exception>
        protected override void RemoveItem(int index)
        {
            if (IsLoaded && IsSearchMode) throw new NotSupportedException("Can't change in search mode");
            if (IsLoaded)
            {
                var data_target = this[index].Data;
                lock (datas) datas.Remove(data_target);
            }
            this[index].Change -= ItemData_Change;
            base.RemoveItem(index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <exception cref="NotSupportedException"></exception>
        protected override void SetItem(int index, TViewModel item)
        {
            throw new NotSupportedException();
            //this[index].Change -= ItemData_Change;
            //item.Change += ItemData_Change;
            //base.SetItem(index, item);
        }
        #endregion ObservableCollection
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
