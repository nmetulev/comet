using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Comet.Converters
{
    /// <summary>
    /// Converts string conteining hex color of the form (#FFFFFFFF) to SolidColorBrush
    /// </summary>
    class HexToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var hexCode = value as string;
                var color = new Color();

                if (hexCode.Length == 9)
                {
                    color.A = byte.Parse(hexCode.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                    color.R = byte.Parse(hexCode.Substring(3, 2), NumberStyles.AllowHexSpecifier);
                    color.G = byte.Parse(hexCode.Substring(5, 2), NumberStyles.AllowHexSpecifier);
                    color.B = byte.Parse(hexCode.Substring(7, 2), NumberStyles.AllowHexSpecifier);
                }
                else if (hexCode.Length == 7)
                {
                    color.A = 0xFF;
                    color.R = byte.Parse(hexCode.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                    color.G = byte.Parse(hexCode.Substring(3, 2), NumberStyles.AllowHexSpecifier);
                    color.B = byte.Parse(hexCode.Substring(5, 2), NumberStyles.AllowHexSpecifier);
                }
                else
                {
                    color.A = 0xFF;
                    color.R = 0xFF;
                    color.G = 0xFF;
                    color.B = 0xFF;
                }

                return new SolidColorBrush(color);
            }
            catch { return value; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
