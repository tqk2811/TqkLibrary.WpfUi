using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TqkLibrary.WpfUi.Converters
{
    /// <summary>
    /// enum is equal (value &amp; parameter) -> Visible else VisibilityType
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(Visibility))]
    public class EnumToVisibleConverter : IValueConverter
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
            Enum val = (Enum)value;//
            Enum param = (Enum)parameter;
            bool result = val.Equals(param);
            if (this.IsReversed) result = !result;

            if (result) return Visibility.Visible;
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
