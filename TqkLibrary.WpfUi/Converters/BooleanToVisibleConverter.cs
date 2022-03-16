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
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibleConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public Visibility VisibilityType { get; set; } = Visibility.Collapsed;
        /// <summary>
        /// 
        /// </summary>
        public bool IsReversed { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;//
            if (this.IsReversed) val = !val;
            if (val) return Visibility.Visible;
            else return VisibilityType;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}