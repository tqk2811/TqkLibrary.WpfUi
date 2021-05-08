using System;
using System.Globalization;
using System.Windows.Data;

namespace TqkLibrary.WpfUi.Converters
{
  public class EnumConverter : IValueConverter
  {
    public bool IsAttributeFlag { get; set; } = true;

    private Enum CurrentValue;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      this.CurrentValue = (Enum)value;
      if (IsAttributeFlag) return CurrentValue.HasFlag(parameter as Enum);// ((mask & target) != 0);
      else return CurrentValue.Equals(parameter as Enum);
    }

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

  public static class EnumExtensions
  {
    public static Enum Or(this Enum a, Enum b)
    {
      // consider adding argument validation here
      if (Enum.GetUnderlyingType(a.GetType()) != typeof(ulong))
        return (Enum)Enum.ToObject(a.GetType(), Convert.ToInt64(a) | Convert.ToInt64(b));
      else
        return (Enum)Enum.ToObject(a.GetType(), Convert.ToUInt64(a) | Convert.ToUInt64(b));
    }

    public static Enum And(this Enum a, Enum b)
    {
      // consider adding argument validation here
      if (Enum.GetUnderlyingType(a.GetType()) != typeof(ulong))
        return (Enum)Enum.ToObject(a.GetType(), Convert.ToInt64(a) & Convert.ToInt64(b));
      else
        return (Enum)Enum.ToObject(a.GetType(), Convert.ToUInt64(a) & Convert.ToUInt64(b));
    }

    public static Enum Not(this Enum a)
    {
      // consider adding argument validation here
      return (Enum)Enum.ToObject(a.GetType(), ~Convert.ToInt64(a));
    }
  }
}