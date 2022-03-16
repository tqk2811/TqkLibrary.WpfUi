using System;
using System.Windows.Data;

namespace TqkLibrary.WpfUi.Converters
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool) && targetType != typeof(bool?)) throw new InvalidOperationException("The target must be a bool or bool?");
            if (value == null) return true;
            return !(bool)value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool) && targetType != typeof(bool?)) throw new InvalidOperationException("The target must be a bool or bool?");
            if (value == null) return true;
            return !(bool)value;
        }
    }
}