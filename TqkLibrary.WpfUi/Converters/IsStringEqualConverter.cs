using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TqkLibrary.WpfUi.Converters
{
  public class IsStringEqualConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if(value is string str_in && parameter is string str_check && !string.IsNullOrEmpty(str_in))
      {
        return str_in.Equals(str_check);
      }
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
