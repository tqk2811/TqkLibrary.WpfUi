using System;
using System.Globalization;
using System.Windows.Data;

namespace TqkLibrary.WpfUi.Converters
{
  public class ListViewAlternationConverter : IValueConverter
  {
    //public Brush Odd { get; set; }
    //public Brush Even { get; set; }
    public ListViewAlternationConverter()
    {
      //BrushConverter brushConverter = new BrushConverter();
      //Odd = (Brush)brushConverter.ConvertFrom("#ffffff");
      //Even = (Brush)brushConverter.ConvertFrom("#ebebeb");
    }


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is int @int) return !(@int % 2 == 1);
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
