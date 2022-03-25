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
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public class SaveObservableCollection<TData, TViewModel> : ObservableCollection<TViewModel>
      where TData : class
      where TViewModel : class, IViewModel<TData>
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

        private readonly Timer timer;
        private bool IsLoaded = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="func"></param>
        /// <param name="interval"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SaveObservableCollection(string savePath, Func<TData, TViewModel> func, int interval = 500)
        {
            if (string.IsNullOrEmpty(savePath)) throw new ArgumentNullException(nameof(savePath));
            this.SavePath = savePath;
            timer = new Timer(interval)
            {
                AutoReset = false
            };
            timer.Elapsed += Timer_Elapsed;
            Load(func);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        public SaveObservableCollection(int interval = 500)
        {
            timer = new Timer(interval)
            {
                AutoReset = false
            };
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (timer)
            {
                using StreamWriter sw = new StreamWriter(SavePath, false);
                sw.Write(JsonConvert.SerializeObject(this.Select(x => x.Data).ToList(), Formatting.Indented));
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Load(Func<TData, TViewModel> func)//Func - Action - Predicate - ....
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            IsLoaded = false;
            this.Clear();
            List<TData> t2s = new List<TData>();
            if (File.Exists(SavePath))
            {
                try
                {
                    using StreamReader sr = new StreamReader(SavePath);
                    List<TData> list = JsonConvert.DeserializeObject<List<TData>>(sr.ReadToEnd());
                    t2s.AddRange(list);
                }
                catch (Exception) { }
            }
            t2s.ForEach(x => this.Add(func.Invoke(x)));
            IsLoaded = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SavePath"></param>
        /// <param name="func"></param>
        public void Load(string SavePath, Func<TData, TViewModel> func)
        {
            this.SavePath = SavePath;
            Load(func);
        }

        #region ObservableCollection

        protected override void InsertItem(int index, TViewModel item)
        {
            item.Change += ItemData_Change;
            base.InsertItem(index, item);
        }

        protected override void ClearItems()
        {
            foreach (var item in this) item.Change -= ItemData_Change;
            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            this[index].Change -= ItemData_Change;
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, TViewModel item)
        {
            this[index].Change -= ItemData_Change;
            item.Change += ItemData_Change;
            base.SetItem(index, item);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (IsAutoSave) Save();
            base.OnCollectionChanged(e);
        }

        #endregion ObservableCollection

        private void ItemData_Change(object obj, TData data)
        {
            if (IsAutoSave) Save();
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