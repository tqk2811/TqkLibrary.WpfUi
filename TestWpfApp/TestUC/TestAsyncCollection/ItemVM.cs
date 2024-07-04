using TqkLibrary.WpfUi.ObservableCollections;

namespace TestWpfApp.TestUC.TestAsyncCollection
{
    class ItemVM : IViewModel<ItemData>
    {
        public ItemVM(ItemData itemData)
        {
            this.Data = itemData;
        }
        public ItemData Data { get; }

        public event ChangeCallBack<ItemData>? Change;

        public int Id { get { return Data.Id; } }
    }
}
