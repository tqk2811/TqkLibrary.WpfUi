using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Threading;

namespace TqkLibrary.WpfUi
{
    /// <summary>
    /// 
    /// </summary>
    public static class WpfUiExtensions
    {
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
            using MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage bitmapimage = new BitmapImage();
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
            using MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
        }



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
            if (input == null || input.Count() <= 1) return input;
            if (count <= 0) count = 1;

            Random random = new Random(DateTime.Now.Millisecond);
            List<T> result = new List<T>();

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
        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name)?.GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task TrueThreadInvokeAsync(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                await dispatcher.InvokeAsync(action);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static async Task TrueThreadInvokeAsync(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
        {
            if (dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                await dispatcher.InvokeAsync(action, priority);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task TrueThreadInvokeAsync(this Dispatcher dispatcher, Action action, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            if (dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                await dispatcher.InvokeAsync(action, priority, cancellationToken);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dispatcher"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task<T> TrueThreadInvokeAsync<T>(this Dispatcher dispatcher, Func<T> func)
        {
            if (dispatcher.CheckAccess())
            {
                return func.Invoke();
            }
            else
            {
                return await dispatcher.InvokeAsync<T>(func);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dispatcher"></param>
        /// <param name="func"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static async Task<T> TrueThreadInvokeAsync<T>(this Dispatcher dispatcher, Func<T> func, DispatcherPriority priority)
        {
            if (dispatcher.CheckAccess())
            {
                return func.Invoke();
            }
            else
            {
                return await dispatcher.InvokeAsync<T>(func, priority);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dispatcher"></param>
        /// <param name="func"></param>
        /// <param name="priority"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T> TrueThreadInvokeAsync<T>(this Dispatcher dispatcher, Func<T> func, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            if (dispatcher.CheckAccess())
            {
                return func.Invoke();
            }
            else
            {
                return await dispatcher.InvokeAsync<T>(func, priority, cancellationToken);
            }
        }
    }
}