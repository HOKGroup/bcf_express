using BCFExpress.Data.Utils;
using System;
using System.Globalization;
using System.Windows.Data;



namespace BCFExpress.Data.ValueConverters
{
  /// <summary>
  /// Converts a date to relative
  /// </summary>
  [ValueConversion(typeof(DateTime), typeof(String))]
  public class DateConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return "";
      var date = new DateTime();
      if (! DateTime.TryParse(value.ToString(), out date))
        return "";

      if (parameter!=null&&parameter.ToString() == "relative")
        return RelativeDate.ToRelative(date);
      else
        return date.ToShortDateString() + " at " + date.ToShortTimeString();

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {

      throw new NotImplementedException();
    }


  }
}
