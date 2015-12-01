using System;
using Windows.UI.Xaml.Data;

namespace Comet.Converters
{
    /// <summary>
    /// Converter for returning a specific value when a value equals to the spcified Value, or returning otherwise
    /// </summary>
    public class ValueWhenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value.Equals(When))
                    return this.Value;
                return this.Otherwise;
            }
            catch
            {
                return this.Otherwise;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value == Value ? When : null;
        }

        public object Value { get; set; }
        public object Otherwise { get; set; }
        public object When { get; set; }
    }
}
