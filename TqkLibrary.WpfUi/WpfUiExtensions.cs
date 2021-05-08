﻿using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace TqkLibrary.WpfUi
{
  public static class WpfUiExtensions
  {
    [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteObject([In] IntPtr hObject);

    public static System.Windows.Media.ImageSource ToImageSource(this System.Drawing.Bitmap src)
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
      if (input == null || input.Count() == 0) return input;
      if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));

      Random random = new Random();
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
  }
}