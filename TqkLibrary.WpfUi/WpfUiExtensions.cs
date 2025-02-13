using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TqkLibrary.WpfUi.Interfaces;

namespace TqkLibrary.WpfUi
{
    /// <summary>
    /// 
    /// </summary>
    public static class WpfUiExtensions
    {
        #region ImageSource

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject([In] IntPtr hObject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static System.Windows.Media.ImageSource ToImageSource(this Bitmap src)
        {
            if (null == src) throw new ArgumentNullException(nameof(src));
            var handle = src.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using MemoryStream memory = new();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage bitmapimage = new();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();
            return bitmapimage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <param name="bitmap"></param>
        public static void FillBitmapImage(this BitmapImage bitmapImage, Bitmap bitmap)
        {
            using MemoryStream memory = new();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
        }

        #endregion



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="count"></param>
        /// <param name="randomFirst"></param>
        /// <returns></returns>
        public static IEnumerable<T> RandomLoop<T>(this IEnumerable<T> input, int count, bool randomFirst = false)
        {
            if (input == null || input.Count() <= 1) return input!;
            if (count <= 0) count = 1;

            Random random = new(DateTime.Now.Millisecond);
            List<T> result = new();

            if (randomFirst) result.AddRange(input.OrderBy(x => Guid.NewGuid()));
            else result.AddRange(input);

            for (int i = 1; i < count; i++)
            {
                T last_index = result.Last();
                List<T> list_work = input.ToList();
                list_work.Remove(last_index);

                T next = list_work[random.Next(list_work.Count)];
                list_work.Remove(next);
                list_work.Add(last_index);

                list_work = list_work.OrderBy(x => Guid.NewGuid()).ToList();

                result.Add(next);
                result.AddRange(list_work);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TAttribute? GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var enumType = value.GetType();
            string? name = Enum.GetName(enumType, value);
            if (string.IsNullOrWhiteSpace(name))
                return default;
            return enumType.GetField(name)?.GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }



        #region TrueThreadInvokeAsync

        public static Task TrueThreadInvokeAsync(
            this IMainThread mainThread,
            Action action,
            DispatcherPriority priority = DispatcherPriority.Normal,
            CancellationToken cancellationToken = default
            )
            => mainThread.Dispatcher.TrueThreadInvokeAsync(action, priority, cancellationToken);
        public static async Task TrueThreadInvokeAsync(
            this Dispatcher dispatcher,
            Action action,
            DispatcherPriority priority = DispatcherPriority.Normal,
            CancellationToken cancellationToken = default
            )
        {
            if (dispatcher is null) throw new ArgumentNullException(nameof(dispatcher));
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                await dispatcher.InvokeAsync(action, priority, cancellationToken);
            }
        }

        public static Task<T> TrueThreadInvokeAsync<T>(
            this IMainThread mainThread,
            Func<T> func,
            DispatcherPriority priority = DispatcherPriority.Normal,
            CancellationToken cancellationToken = default
            )
            => mainThread.Dispatcher.TrueThreadInvokeAsync(func, priority, cancellationToken);
        public static async Task<T> TrueThreadInvokeAsync<T>(
            this Dispatcher dispatcher,
            Func<T> func,
            DispatcherPriority priority = DispatcherPriority.Normal,
            CancellationToken cancellationToken = default
            )
        {
            if (dispatcher is null) throw new ArgumentNullException(nameof(dispatcher));
            if (func is null) throw new ArgumentNullException(nameof(func));
            if (dispatcher.CheckAccess())
            {
                return func.Invoke();
            }
            else
            {
                return await dispatcher.InvokeAsync<T>(func, priority, cancellationToken);
            }
        }

        #endregion



        #region  ICollection<T>

        public static Task AddAsync<T, TCollection>(this TCollection collection, T item, CancellationToken cancellationToken = default) where TCollection : ICollection<T>, IMainThread
            => collection.Dispatcher.TrueThreadInvokeAsync(() => collection.Add(item), cancellationToken: cancellationToken);
        public static Task ClearAsync<T, TCollection>(this TCollection collection, CancellationToken cancellationToken = default) where TCollection : ICollection<T>, IMainThread
            => collection.Dispatcher.TrueThreadInvokeAsync(() => collection.Clear(), cancellationToken: cancellationToken);
        public static Task<bool> RemoveAsync<T, TCollection>(this TCollection collection, T item, CancellationToken cancellationToken = default) where TCollection : ICollection<T>, IMainThread
            => collection.Dispatcher.TrueThreadInvokeAsync(() => collection.Remove(item), cancellationToken: cancellationToken);
        public static Task InsertAsync<T, TList>(this TList list, int index, T item, CancellationToken cancellationToken = default) where TList : IList<T>, IMainThread
            => list.Dispatcher.TrueThreadInvokeAsync(() => list.Insert(index, item), cancellationToken: cancellationToken);
        public static Task RemoveAtAsync<T, TList>(this TList list, int index, CancellationToken cancellationToken = default) where TList : IList<T>, IMainThread
            => list.Dispatcher.TrueThreadInvokeAsync(() => list.RemoveAt(index), cancellationToken: cancellationToken);



        /// <summary>
        /// Sync this collection with datas
        /// </summary>
        /// <param name="datas"></param>
        public static void Sync<T, TCollection>(this TCollection collection, IEnumerable<T> datas) where TCollection : ICollection<T>
        {
            var news = datas.Except(collection).ToList();
            var deleteds = collection.Except(datas).ToList();

            news.ForEach(x => collection.Add(x));
            deleteds.ForEach(x => collection.Remove(x));
        }
        /// <summary>
        /// Sync this collection with datas
        /// </summary>
        /// <param name="datas"></param>
        public static Task SyncAsync<T, TCollection>(this TCollection collection, IEnumerable<T> datas, CancellationToken cancellationToken = default) where TCollection : ICollection<T>, IMainThread
            => collection.TrueThreadInvokeAsync(() => collection.Sync(datas), cancellationToken: cancellationToken);


        #endregion
    }
}