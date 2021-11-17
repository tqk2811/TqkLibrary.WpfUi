using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TqkLibrary.WpfUi.Converters
{
    public class NullToHiddenConverter : IValueConverter
    {
        public Visibility VisibilityType { get; set; } = Visibility.Hidden;
        public bool IsReversed { get; set; } = false;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = value != null;//
            if (this.IsReversed) val = !val;
            if (val) return Visibility.Visible;
            else return VisibilityType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
