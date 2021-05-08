using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TqkLibrary.WpfUi.ObservableCollection
{
  public class DispatcherObservableCollection<T> : ObservableCollection<T>
  {
    readonly Dispatcher dispatcher;
    public DispatcherObservableCollection(): this(Application.Current.Dispatcher)
    {

    }
    public DispatcherObservableCollection(Dispatcher dispatcher)
    {
      this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    protected override void ClearItems()
    {
      if (dispatcher.CheckAccess()) base.ClearItems();
      else dispatcher.Invoke(() => base.ClearItems());
    }

    protected override void InsertItem(int index, T item)
    {
      if (dispatcher.CheckAccess()) base.InsertItem(index, item);
      else dispatcher.Invoke(() => base.InsertItem(index, item));
    }

    protected override void MoveItem(int oldIndex, int newIndex)
    {
      if (dispatcher.CheckAccess()) base.MoveItem(oldIndex, newIndex);
      else dispatcher.Invoke(() => base.MoveItem(oldIndex, newIndex));
    }

    protected override void RemoveItem(int index)
    {
      if (dispatcher.CheckAccess()) base.RemoveItem(index);
      else dispatcher.Invoke(() => base.RemoveItem(index));
    }

    protected override void SetItem(int index, T item)
    {
      if (dispatcher.CheckAccess()) base.SetItem(index, item);
      else dispatcher.Invoke(() => base.SetItem(index, item));
    }
  }
}
