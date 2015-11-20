using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace CometMailSample
{
    /// <summary>
    /// Converts string conteining hex color of the form (#FFFFFFFF) to SolidColorBrush
    /// </summary>
    public class BoolToAccentColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool && !((bool)value))
            {
                return new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);
            }
            else
            {
                return new SolidColorBrush(Colors.White);
            }
           
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}
