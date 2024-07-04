using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using TqkLibrary.WpfUi;
using TqkLibrary.WpfUi.ObservableCollections.AsyncCollections;

namespace TestWpfApp.TestUC.TestAsyncCollection
{
    internal class AsyncCollectionWVM : BaseViewModel
    {
        public AsyncCollectionWVM()
        {
            Items = new AsyncCollection<ItemData, ItemVM>(new AsyncData(), x => new ItemVM(x));
        }
        public AsyncCollection<ItemData, ItemVM> Items { get; }
    }
}
