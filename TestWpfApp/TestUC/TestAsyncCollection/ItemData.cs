namespace TestWpfApp.TestUC.TestAsyncCollection
{
    class ItemData
    {
        public int Id { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (obj is ItemData itemData)
            {
                return this.Id == itemData.Id;
            }
            return base.Equals(obj);
        }
    }
}
