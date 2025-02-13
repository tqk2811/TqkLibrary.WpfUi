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
        /// Visible
        /// </summary>
        public Visibility DefaultOnNull { get; set; } = Visibility.Visible;

        /// <summary>
        /// Visible
        /// </summary>
        public Visibility DefaultOnNonBool { get; set; } = Visibility.Visible;

        /// <summary>
        /// Collapsed
        /// </summary>
        public Visibility VisibilityTypeOnFalse { get; set; } = Visibility.Collapsed;

        /// <summary>
        /// Visible
        /// </summary>
        public Visibility VisibilityTypeOnTrue { get; set; } = Visibility.Visible;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReversedBool { get; set; } = false;

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
            if (value == null) return this.DefaultOnNull;
            if (value is bool b)
            {
                if (this.IsReversedBool) b = !b;
                return b ? this.VisibilityTypeOnTrue : this.VisibilityTypeOnFalse;
            }
            else if (value is bool?)
            {
                bool? nb = value as bool?;
                if (!nb.HasValue) return this.DefaultOnNull;
                if (this.IsReversedBool) nb = !nb.Value;
                return nb.Value ? this.VisibilityTypeOnTrue : this.VisibilityTypeOnFalse;
            }
            return this.DefaultOnNonBool;
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
            if (value != null)
            {
                if (value is Visibility visibility)
                {
                    if (visibility == this.VisibilityTypeOnTrue) return this.IsReversedBool ? false : true;
                    if (visibility == this.VisibilityTypeOnFalse) return this.IsReversedBool ? true : false;
                }
            }
            return false;
        }
    }
}