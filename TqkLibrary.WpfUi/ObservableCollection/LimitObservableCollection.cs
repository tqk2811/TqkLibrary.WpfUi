using System;
using System.Collections.ObjectModel;
using System.IO;

namespace TqkLibrary.WpfUi.ObservableCollection
{
  public class LimitObservableCollection<T> : DispatcherObservableCollection<T>
  {
    public LimitObservableCollection()
    {
      this.LogPath = new Func<string>(() => $"{Directory.GetCurrentDirectory()}\\{DateTime.Now:yyyy-MM-dd}.log");
    }
    public LimitObservableCollection(Func<string> delegatePath, int Limit = 500, bool IsInsertTop = true)
    {
      this.LogPath = delegatePath;
      this.Limit = Limit;
      this.IsInsertTop = IsInsertTop;
    }
    public int Limit { get; set; } = 100;
    public bool IsInsertTop { get; set; } = false;
    public Func<string> LogPath { get; set; }

    protected override void InsertItem(int index, T item)
    {
      if (this.Count == Limit) base.RemoveAt(IsInsertTop ? this.Count - 1 : 0);
      if (LogPath != null)
      {
        string path = LogPath.Invoke();
        if(!string.IsNullOrEmpty(path))
        {
          Dispatcher.Invoke(() =>
          {
            using StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine(item.ToString());
          });
        }
      }
      base.InsertItem(IsInsertTop ? 0 : this.Count, item);
    }

    protected override void MoveItem(int oldIndex, int newIndex)
    {
      throw new NotSupportedException();// base.MoveItem(oldIndex, newIndex);
    }

    protected override void SetItem(int index, T item)
    {
      throw new NotSupportedException();// base.SetItem(IsInsertTop ? 0 : this.Count, item);
    }
  }
}