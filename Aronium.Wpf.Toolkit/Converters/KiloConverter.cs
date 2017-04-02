using System;
using System.Windows.Data;

namespace Aronium.Wpf.Toolkit.Converters
{
    public class KiloConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var val = System.Convert.ToDouble(value);

                if (val > 1000)
                {
                    return (val / 1000).ToString("0.##K");
                }

                return val.ToString("0.##");
            }
            catch
            {
                return Double.NaN;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
