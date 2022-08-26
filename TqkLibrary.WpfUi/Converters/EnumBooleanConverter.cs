using System;
using System.Globalization;
using System.Windows.Data;

namespace TqkLibrary.WpfUi.Converters
{
    /// <summary>
    /// enum is equal (value &amp; parameter) -> true else false
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(bool))]
    public class EnumBooleanConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsAttributeFlag { get; set; } = true;

        private Enum CurrentValue;

        /// <summary>
        /// enum -> bool
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>bool</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Enum e)
            {
                this.CurrentValue = e;
                if (IsAttributeFlag) return CurrentValue.HasFlag(parameter as Enum);// ((mask & target) != 0);
                else return CurrentValue.Equals(parameter as Enum);
            }
            return false;            
        }

        /// <summary>
        /// bool -> enum
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (IsAttributeFlag)
            {
                var par = parameter as Enum;
                if ((bool)value) CurrentValue = CurrentValue.Or(par);
                else CurrentValue = CurrentValue.And(par.Not());
                return CurrentValue;
            }
            else
            {
                if ((bool)value) return parameter;
                else return CurrentValue;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Enum Or(this Enum a, Enum b)
        {
            // consider adding argument validation here
            if (Enum.GetUnderlyingType(a.GetType()) != typeof(ulong))
                return (Enum)Enum.ToObject(a.GetType(), Convert.ToInt64(a) | Convert.ToInt64(b));
            else
                return (Enum)Enum.ToObject(a.GetType(), Convert.ToUInt64(a) | Convert.ToUInt64(b));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Enum And(this Enum a, Enum b)
        {
            // consider adding argument validation here
            if (Enum.GetUnderlyingType(a.GetType()) != typeof(ulong))
                return (Enum)Enum.ToObject(a.GetType(), Convert.ToInt64(a) & Convert.ToInt64(b));
            else
                return (Enum)Enum.ToObject(a.GetType(), Convert.ToUInt64(a) & Convert.ToUInt64(b));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Enum Not(this Enum a)
        {
            // consider adding argument validation here
            return (Enum)Enum.ToObject(a.GetType(), ~Convert.ToInt64(a));
        }
    }
}