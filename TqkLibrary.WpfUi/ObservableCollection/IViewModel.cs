namespace TqkLibrary.WpfUi.ObservableCollection
{
  public delegate void ChangeCallBack<T>(object obj, T data);
  public interface IViewModel<T> where T : class
  {
    T Data { get; }

    event ChangeCallBack<T> Change;
  }
}