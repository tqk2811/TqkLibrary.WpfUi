namespace TqkLibrary.WpfUi.ObservableCollections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IItemData<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        TId GroupId { get; }
    }
}