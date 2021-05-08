using Newtonsoft.Json;
using System;
using System.IO;
using System.Timers;

namespace TqkLibrary.WpfUi
{
  public class SaveSettingData<T> where T : new()
  {
    public T Setting { get; private set; }
    private readonly string SavePath;
    private readonly Timer timer;

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

    private void Timer_Elapsed(object sender, ElapsedEventArgs e) => File.WriteAllText(SavePath, JsonConvert.SerializeObject(Setting));

    public void Save()
    {
      timer?.Stop();
      timer?.Start();
    }
  }
}