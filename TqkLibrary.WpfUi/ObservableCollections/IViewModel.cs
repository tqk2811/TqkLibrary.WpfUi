namespace TqkLibrary.WpfUi.ObservableCollections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="data"></param>
    public delegate void ChangeCallBack<T>(object obj, T data);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IViewModel<T>
    {
        /// <summary>
        /// 
        /// </summary>
        T Data { get; }
        /// <summary>
        /// 
        /// </summary>
        event ChangeCallBack<T>? Change;
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <typeparam name="TInterface"></typeparam>
    ///// <typeparam name="TClass"></typeparam>
    //public interface IViewModel<TInterface, TClass>
    //    where TClass : class, TInterface
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    TInterface Data { get; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    event ChangeCallBack<TInterface> Change;
    //}
}