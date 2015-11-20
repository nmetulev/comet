using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Comet.Converters
{
    /// <summary>
    /// Converter to convert DateTime to string with the specified format (parameter)
    /// </summary>
    public class DateTimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter != null)
                {
                    return System.Convert.ToDateTime(value).ToString(parameter.ToString());
                }
                else
                {
                    return System.Convert.ToDateTime(value).ToString();
                }
            }
            // not a DateTime
            catch { return "NaD"; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is string ? DateTime.Parse((String)value) : DateTime.MinValue;
        }
    }
}
