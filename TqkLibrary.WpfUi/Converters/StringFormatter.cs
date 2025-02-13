using System;
using System.Globalization;
using System.Windows.Data;

namespace TqkLibrary.WpfUi.Converters
{
    /// <summary>
    /// 
    /// </summary>
    internal class StringFormatter : IMultiValueConverter
    {
        string? _stringFormat = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object? Convert(object?[]? values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1 && values[0] != null && values[1] is string format)
            {
                if (string.IsNullOrEmpty(format)) return values[0]?.ToString();
                else
                {
                    this._stringFormat = format;
                    if (values[0] is double _double) return _double.ToString(format);
                    return string.Format(format, values[0]);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object?[] ConvertBack(object? value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object?[] { value, this._stringFormat };
        }
    }
}
