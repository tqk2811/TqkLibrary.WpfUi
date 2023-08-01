using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;

namespace TqkLibrary.WpfUi.ObservableCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public class SaveFileObservableCollection<TData, TViewModel> : SaveObservableCollection<TData, TViewModel>
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
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        private readonly System.Timers.Timer _timer;


        private readonly object _locker = new object();
        List<TData> _datas = new List<TData>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="func"></param>
        /// <param name="interval"></param>
        public SaveFileObservableCollection(string savePath, Func<TData, TViewModel> func, int interval = 500)
        {
            if (string.IsNullOrWhiteSpace(savePath)) throw new ArgumentNullException(nameof(savePath));
            if (func is null) throw new ArgumentNullException(nameof(func));

            this.SavePath = savePath;
            _timer = new(interval)
            {
                AutoReset = false
            };
            _timer.Elapsed += Timer_Elapsed;

            _LoadFromFile();
            Load(this._datas, func);

            base.OnSave += SaveFileObservableCollection_OnSave;
        }


        private void SaveFileObservableCollection_OnSave(IEnumerable<TData> datas)
        {
            if (datas is null) return;
            lock (_locker)
            {
                this._datas = datas.ToList();
            }
            _timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _WriteToFile();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ForceSave()
        {
            Save();
            _timer.Stop();
            _WriteToFile();
        }



        void _WriteToFile()
        {
            lock (_locker)
            {
                if (_datas is not null && !string.IsNullOrWhiteSpace(SavePath))
                {
                    try
                    {
                        using StreamWriter sw = new StreamWriter(SavePath, false);
                        sw.Write(JsonConvert.SerializeObject(_datas.ToList(), Formatting.Indented));
                    }
                    catch
                    {

                    }
                }
            }
        }

        void _LoadFromFile()
        {
            lock (_locker)
            {
                if (File.Exists(SavePath))
                {
                    try
                    {
                        string text = File.ReadAllText(SavePath);
                        this._datas = JsonConvert.DeserializeObject<List<TData>>(text);
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (this._datas is null) this._datas = new List<TData>();
                    }
                }
            }
        }
    }
}