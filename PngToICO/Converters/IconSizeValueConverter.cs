using System;
using System.Globalization;
using System.Windows.Data;

namespace PngToICO
{
    public class IconSizeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IconSize size)
            {
                return Utility.GetString(size);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                return Enum.Parse(typeof(IconSize), s.Substring(0, s.IndexOf('p')));
            }

            return null;
        }


    }
}
