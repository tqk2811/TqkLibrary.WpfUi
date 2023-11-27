using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace TqkLibrary.WpfUi
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SaveSettingData<T> where T : new()
    {
        /// <summary>
        /// 
        /// </summary>
        public T Setting { get; private set; }
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
        public SaveSettingData(string SavePath, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (string.IsNullOrEmpty(SavePath)) throw new ArgumentNullException(nameof(SavePath));
            this._jsonSerializerSettings = jsonSerializerSettings;
            this._savePath = SavePath;
            if (File.Exists(SavePath)) Setting = JsonConvert.DeserializeObject<T>(File.ReadAllText(SavePath), jsonSerializerSettings);
            else Setting = new T();
            _timer = new System.Timers.Timer(_defaultDelaySaving);
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                File.WriteAllText(_savePath, JsonConvert.SerializeObject(Setting, Formatting.Indented, _jsonSerializerSettings));
                OnSaved?.Invoke(Setting);
            }
            catch (Exception ex)
            {
                OnSaveError?.Invoke(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            _timer.Stop();
            _timer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<T> SaveAsync(CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<T> result = new TaskCompletionSource<T>();
            Action<T> onSaved = (o) => result.TrySetResult(o);
            Action<Exception> onError = (e) => result.TrySetException(e);
            using var register = cancellationToken.Register(() => result.TrySetCanceled());
            try
            {
                OnSaved += onSaved;
                OnSaveError += onError;
                Save();
                return await result.Task.ConfigureAwait(false);
            }
            finally
            {
                OnSaved -= onSaved;
                OnSaveError -= onError;
            }
        }
    }
}