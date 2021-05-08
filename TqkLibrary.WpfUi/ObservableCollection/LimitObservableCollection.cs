using System;
using System.Collections.ObjectModel;
using System.IO;

namespace TqkLibrary.WpfUi.ObservableCollection
{
  public class LimitObservableCollection<T> : DispatcherObservableCollection<T>
  {
    public int Limit { get; set; } = 100;
    public bool IsInsertTop { get; set; } = false;
    public string LogPath { get; set; } = $"{DateTime.Now:yyyy-MM-dd}.log";

    protected override void InsertItem(int index, T item)
    {
      if (this.Count == Limit) base.RemoveAt(IsInsertTop ? this.Count - 1 : 0);
      if (!string.IsNullOrEmpty(LogPath)) using (StreamWriter sw = new StreamWriter(LogPath, true)) sw.WriteLine(item.ToString());
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