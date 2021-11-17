using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TqkLibrary.WpfUi.Converters
{
    /// <summary>
    /// false -> VisibilityType (default Collapsed)<br></br>
    /// true -> Visible
    /// </summary>
    public class BooleanToVisibleConverter : IValueConverter
    {
        public Visibility VisibilityType { get; set; } = Visibility.Collapsed;
        public bool IsReversed { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;//
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