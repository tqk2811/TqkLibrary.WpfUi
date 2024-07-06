namespace TqkLibrary.WpfUi.Interfaces
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
}