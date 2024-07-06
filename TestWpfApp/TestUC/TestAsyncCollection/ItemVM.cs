using TqkLibrary.WpfUi.Interfaces;

namespace TestWpfApp.TestUC.TestAsyncCollection
{
    class ItemVM : IViewModelUpdate<ItemData>
    {
        public ItemVM(ItemData itemData)
        {
            this.Data = itemData;
        }
        public ItemData Data { get; private set; }

        public event ChangeCallBack<ItemData>? Change;

        public int Id { get { return Data.Id; } }

        public void Update(ItemData data)
        {
            this.Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
