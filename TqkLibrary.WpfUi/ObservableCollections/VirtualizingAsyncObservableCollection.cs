using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.WpfUi.ObservableCollections
{
    public class VirtualizingAsyncObservableCollection<T> : DispatcherObservableCollection<T>, IList
    {
        int _pageSize = 100;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; NotifyPropertyChange(); }
        }

        bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set { _isLoading = value; NotifyPropertyChange(); }
        }

        object? IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }


        //public new T this[int index]
        //{
        //    get
        //    {
        //        int pageIndex = this.Count / PageSize;
        //        int pageOffset = this.Count % PageSize;


        //    }
        //}






        protected virtual void NotifyPropertyChange([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        private void FireCollectionReset()
        {
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(e);
        }
    }
}
