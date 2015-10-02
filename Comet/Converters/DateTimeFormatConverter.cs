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
    class DateTimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return System.Convert.ToDateTime(value).ToString(parameter.ToString());
            }
            catch { return value; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
