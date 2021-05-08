using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TqkLibrary.WpfUi.Converters
{
  internal class StringFormatter : IMultiValueConverter
  {
    string StringFormat = string.Empty;
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if(values != null && values.Length > 1 && values[0] != null && values[1] is string format)
      {
        if (string.IsNullOrEmpty(format)) return values[0].ToString();
        else
        {
          StringFormat = format;
          if (values[0] is double _double) return _double.ToString(format);
          return string.Format(format, values[0]);
        }
      }
      return string.Empty;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return new object[] { value , StringFormat };
    }
  }
}
