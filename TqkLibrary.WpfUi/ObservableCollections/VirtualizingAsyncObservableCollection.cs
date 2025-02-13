using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TqkLibrary.WpfUi.ObservableCollections
{
    public class VirtualizingAsyncObservableCollection<T> : DispatcherObservableCollection<T>, IList
    {
        int _pageSize = 100;
        public int PageSize
        {
            get { return this._pageSize; }
            set { this._pageSize = value; this.NotifyPropertyChange(); }
        }

        bool _isLoading = false;
        public bool IsLoading
        {
            get { return this._isLoading; }
            private set { this._isLoading = value; this.NotifyPropertyChange(); }
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
            NotifyCollectionChangedEventArgs e = new(NotifyCollectionChangedAction.Reset);
            this.OnCollectionChanged(e);
        }
    }
}
