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
        public UInt64 UncheckNonAttributeFlag { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public bool IsAttributeFlag { get; set; } = true;

        private Enum? CurrentValue;
        ulong CurrentUlongValue = 0;
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
            if (value is Enum e && parameter is Enum p)
            {
                this.CurrentValue = e;
                if (this.IsAttributeFlag) return this.CurrentValue.HasFlag(p);// ((mask & target) != 0);
                else return this.CurrentValue.Equals(p);
            }
            else if (value.GetType().IsValueType && parameter.GetType().IsValueType)
            {
                this.CurrentUlongValue = System.Convert.ToUInt64(value);
                if (this.IsAttributeFlag) return (this.CurrentUlongValue & System.Convert.ToUInt64(parameter)) != 0;
                else return this.CurrentUlongValue == System.Convert.ToUInt64(parameter);
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
            if (this.CurrentValue is null)
                throw new InvalidOperationException($"Need call {nameof(Convert)} first");

            if (parameter is Enum par)
            {
                if (this.IsAttributeFlag)
                {
                    if ((bool)value) this.CurrentValue = this.CurrentValue.Or(par);
                    else this.CurrentValue = this.CurrentValue.And(par.Not());
                    return this.CurrentValue;
                }
                else
                {
                    if ((bool)value) return parameter;
                    else return (Enum)Enum.ToObject(targetType, this.UncheckNonAttributeFlag);
                }
            }
            else
            {
                if (this.IsAttributeFlag)
                {
                    if ((bool)value) this.CurrentUlongValue |= System.Convert.ToUInt64(parameter);
                    else this.CurrentUlongValue &= ~System.Convert.ToUInt64(parameter);
                    return this.CurrentUlongValue;
                }
                else
                {
                    if ((bool)value) return parameter;
                    else return this.UncheckNonAttributeFlag;
                }
            }

        }
    }
}