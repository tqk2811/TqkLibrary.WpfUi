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
    /// <summary>
    /// enum is equal (value & parameter) -> Visible else VisibilityType
    /// </summary>
    public class EnumToVisibleConverter : IValueConverter
    {
        public Visibility VisibilityType { get; set; } = Visibility.Collapsed;
        public bool IsReversed { get; set; } = false;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum val = (Enum)value;//
            Enum param = (Enum)parameter;
            bool result = val.Equals(param);
            if (this.IsReversed) result = !result;

            if (result) return Visibility.Visible;
            else return VisibilityType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
