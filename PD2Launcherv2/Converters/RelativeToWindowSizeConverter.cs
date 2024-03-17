using System.Globalization;

namespace ProjectDiablo2Launcherv2.Converters
{
    internal class RelativeToWindowSizeConverter
    {
        public double Factor { get; set; } = 1;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double windowSize)
            {
                return windowSize * Factor;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}