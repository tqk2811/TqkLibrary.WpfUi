using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TqkLibrary.WpfUi.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class NullToHiddenConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public Visibility VisibilityType { get; set; } = Visibility.Hidden;

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
            bool val = value != null;//
            if (this.IsReversed) val = !val;
            if (val) return Visibility.Visible;
            else return this.VisibilityType;
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
