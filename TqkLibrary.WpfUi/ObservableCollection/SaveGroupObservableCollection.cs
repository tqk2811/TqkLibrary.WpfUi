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
  public interface IItemData<TId>
  {
    public TId GroupId { get; set; }
  }
  public class SaveGroupObservableCollection<TId, TData, TViewModel> : DispatcherObservableCollection<TViewModel>
    where TData : class, IItemData<TId>//data
    where TViewModel : class, IViewModel<TData>//viewmodel
  {
    public string SavePath { get; set; }
    public bool IsAutoSave { get; set; } = true;
    public double Interval
    {
      get { return timer.Interval; }
      set { timer.Interval = value; }
    }
    public bool IsSearchMode { get; private set; } = false;
    public TId CurrentGroupId { get; private set; }


    readonly Func<TData, TViewModel> func;
    readonly List<TData> datas = new List<TData>();
    private readonly Timer timer;
    private bool IsLoaded = false;
    
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
        sw.Write(JsonConvert.SerializeObject(datas));
      }
    }

    public void Save()
    {
      if (IsLoaded)
      {
        timer.Stop();
        timer.Start();
      }
    }

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

    public void HideAll()
    {
      IsLoaded = false;
      IsSearchMode = false;
      this.Clear();
      IsLoaded = true;
    }

    public IEnumerable<TId> GetGroups()
    {
      return datas.GroupBy(x => x.GroupId).Select(x => x.Key);
    }

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

    public void Search(Func<TData, bool> predicate)
    {
      IsLoaded = false;
      IsSearchMode = true;
      this.Clear();
      foreach (var item in datas.Where(predicate)) this.Add(func?.Invoke(item));
      IsLoaded = true;
    }

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

    public List<TData> GetDataSave() => this.datas.ToList();

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

    public bool IsAny(Predicate<TData> predicate)
    {
      lock (datas) return datas.Any(x => predicate.Invoke(x));
    }

    private void ItemData_Change(object obj, TData data)
    {
      if (IsAutoSave) Save();
    }


    #region ObservableCollection
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (IsAutoSave) Save();
      base.OnCollectionChanged(e);
    }

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

    protected override void ClearItems()
    {
      if (IsLoaded && IsSearchMode) throw new NotSupportedException("Can't change in search mode");
      if (IsLoaded)
      {

      }
      foreach (var item in this) item.Change -= ItemData_Change;
      base.ClearItems();
    }

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

    protected override void SetItem(int index, TViewModel item)
    {
      throw new NotSupportedException();
      //this[index].Change -= ItemData_Change;
      //item.Change += ItemData_Change;
      //base.SetItem(index, item);
    }
    #endregion ObservableCollection
    protected void NotifyPropertyChange([CallerMemberName] string name = "")
    {
      OnPropertyChanged(new PropertyChangedEventArgs(name));
    }
  }
}
