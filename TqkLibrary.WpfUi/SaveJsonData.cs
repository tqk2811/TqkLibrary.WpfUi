using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace TqkLibrary.WpfUi
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SaveJsonData<T> : IDisposable, ISaveJsonDataControl
        where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double DelaySaving
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        const double _defaultDelaySaving = 500;
        /// <summary>
        /// 
        /// </summary>
        public event Action<T> OnSaved;
        /// <summary>
        /// 
        /// </summary>
        public event Action<Exception> OnSaveError;


        private readonly string _savePath;
        private readonly System.Timers.Timer _timer;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SavePath"></param>
        /// <param name="jsonSerializerSettings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SaveJsonData(string SavePath, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (string.IsNullOrEmpty(SavePath)) throw new ArgumentNullException(nameof(SavePath));
            this._jsonSerializerSettings = jsonSerializerSettings;
            this._savePath = SavePath;
            Load();
            _timer = new System.Timers.Timer(_defaultDelaySaving);
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SavePath"></param>
        /// <param name="defaultData"></param>
        /// <param name="jsonSerializerSettings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SaveJsonData(string SavePath, T defaultData, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (string.IsNullOrEmpty(SavePath)) throw new ArgumentNullException(nameof(SavePath));
            this._jsonSerializerSettings = jsonSerializerSettings;
            this._savePath = SavePath;
            Load(defaultData);
            _timer = new System.Timers.Timer(_defaultDelaySaving);
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = false;
        }
        /// <summary>
        /// 
        /// </summary>
        ~SaveJsonData()
        {
            _timer.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _timer.Dispose();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ForceSave();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void ForceSave()
        {
            try
            {
                File.WriteAllText(_savePath, JsonConvert.SerializeObject(Data, Formatting.Indented, _jsonSerializerSettings));
                OnSaved?.Invoke(Data);
            }
            catch (Exception ex)
            {
                OnSaveError?.Invoke(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void TriggerSave()
        {
            _timer.Stop();
            _timer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<T> result = new TaskCompletionSource<T>();
            Action<T> onSaved = (o) => result.TrySetResult(o);
            Action<Exception> onError = (e) => result.TrySetException(e);
            using var register = cancellationToken.Register(() => result.TrySetCanceled());
            try
            {
                OnSaved += onSaved;
                OnSaveError += onError;
                TriggerSave();
                await result.Task.ConfigureAwait(false);
            }
            finally
            {
                OnSaved -= onSaved;
                OnSaveError -= onError;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Load()
        {
            if (File.Exists(_savePath)) Data = JsonConvert.DeserializeObject<T>(File.ReadAllText(_savePath), _jsonSerializerSettings);
            else
            {
                T defaultData = (T)Activator.CreateInstance(typeof(T));//throw if not have Parameterless
                Load(defaultData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultData">default Data if file not exist</param>
        public virtual void Load(T defaultData)
        {
            if (File.Exists(_savePath)) Data = JsonConvert.DeserializeObject<T>(File.ReadAllText(_savePath), _jsonSerializerSettings);
            else
            {
                Data = defaultData ?? throw new ArgumentNullException(nameof(defaultData));
            }
        }
    }
}