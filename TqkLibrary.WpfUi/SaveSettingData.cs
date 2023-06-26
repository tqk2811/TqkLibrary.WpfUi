using Newtonsoft.Json;
using System;
using System.IO;
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
        private readonly string SavePath;
        private readonly Timer timer;
        /// <summary>
        /// 
        /// </summary>
        public event Action<T> OnSaved;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SavePath"></param>
        /// <param name="delay"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SaveSettingData(string SavePath, int delay = 500)
        {
            if (string.IsNullOrEmpty(SavePath)) throw new ArgumentNullException(nameof(SavePath));
            this.SavePath = SavePath;
            if (File.Exists(SavePath)) Setting = JsonConvert.DeserializeObject<T>(File.ReadAllText(SavePath));
            else Setting = new T();
            timer = new Timer(delay);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                File.WriteAllText(SavePath, JsonConvert.SerializeObject(Setting, Formatting.Indented));
            }
            catch
            {

            }
            OnSaved?.Invoke(Setting);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            timer?.Stop();
            timer?.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();
            ElapsedEventHandler action = (object sender, ElapsedEventArgs e) => result.TrySetResult(true);
            Save();
            try
            {
                timer.Elapsed += action;
                await result.Task.ConfigureAwait(false);
            }
            finally
            {
                timer.Elapsed -= action;
            }
        }
    }
}