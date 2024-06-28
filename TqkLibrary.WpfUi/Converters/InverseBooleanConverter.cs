using System;
using System.Windows.Data;

namespace TqkLibrary.WpfUi.Converters
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool?))]
    public class InverseBooleanConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public bool? DefaultOnNull { get; set; } = null;
        /// <summary>
        /// 
        /// </summary>
        public bool? DefaultOnNonBool { get; set; } = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DefaultOnNull;
            if (value is bool b)
            {
                return !b;
            }
            else if (value is bool?)
            {
                bool? nb = (bool?)value;
                return nb.HasValue ? !nb.Value : DefaultOnNull;
            }
            else return DefaultOnNonBool;
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
        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DefaultOnNull;
            if (value is bool b)
            {
                return !b;
            }
            else if (value is bool?)
            {
                bool? nb = (bool?)value;
                return nb.HasValue ? !nb.Value : DefaultOnNull;
            }
            else return DefaultOnNonBool;
        }
    }
}