using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace TqkLibrary.WpfUi
{
  public static class WpfUiExtensions
  {
    [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteObject([In] IntPtr hObject);

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



    public static readonly string[] unit_size = { "Byte", "Kib", "Mib", "Gib", "Tib" };
    public static readonly string[] unit_speed = { "Byte/s", "Kib/s", "Mib/s", "Gib/s", "Tib/s" };
    public static string ConvertSize(double num, int round, string[] units, int div = 1024)
    {
      if (num == 0) return "0 " + units[0];
      for (double i = 0; i < units.Length; i++)
      {
        double sizeitem = num / Math.Pow(div, i);
        if (sizeitem < 1 && sizeitem > -1)
        {
          if (i == 0) return "0 " + units[0];
          else return Math.Round((num / Math.Pow(div, i - 1)), round).ToString() + " " + units[(int)i - 1];
        }
      }
      return Math.Round(num / Math.Pow(div, units.Length - 1), round).ToString() + " " + units[units.Length - 1];
    }

    public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
    {
      var enumType = value.GetType();
      var name = Enum.GetName(enumType, value);
      return enumType.GetField(name)?.GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
    }
  }
}