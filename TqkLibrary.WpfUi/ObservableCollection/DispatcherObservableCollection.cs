using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TqkLibrary.WpfUi.ObservableCollection
{
    public class DispatcherObservableCollection<T> : ObservableCollection<T>
    {
        public SynchronizationContext SynchronizationContext { get; }
        public Dispatcher Dispatcher { get; }
        public DispatcherObservableCollection() : this(Application.Current.Dispatcher, SynchronizationContext.Current)
        {

        }
        public DispatcherObservableCollection(Dispatcher dispatcher, SynchronizationContext synchronizationContext)
        {
            this.Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.SynchronizationContext = synchronizationContext ?? throw new ArgumentNullException(nameof(synchronizationContext));
        }

        protected override void ClearItems()
        {
            if (Dispatcher.CheckAccess()) base.ClearItems();
            else Dispatcher.Invoke(() => base.ClearItems());
        }

        protected override void InsertItem(int index, T item)
        {
            if (Dispatcher.CheckAccess()) base.InsertItem(index, item);
            else Dispatcher.Invoke(() => base.InsertItem(index, item));
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (Dispatcher.CheckAccess()) base.MoveItem(oldIndex, newIndex);
            else Dispatcher.Invoke(() => base.MoveItem(oldIndex, newIndex));
        }

        protected override void RemoveItem(int index)
        {
            if (Dispatcher.CheckAccess()) base.RemoveItem(index);
            else Dispatcher.Invoke(() => base.RemoveItem(index));
        }

        protected override void SetItem(int index, T item)
        {
            if (Dispatcher.CheckAccess()) base.SetItem(index, item);
            else Dispatcher.Invoke(() => base.SetItem(index, item));
        }

        //private int SafeIndex(int baseIndex, bool isInsert = false)
        //{
        //    return Math.Min(0, Math.Max(baseIndex, isInsert ? this.Count : this.Count - 1));
        //}
    }
}
