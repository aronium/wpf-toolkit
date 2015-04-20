using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Aronium.Wpf.Toolkit.Converters
{
    /// <summary>
    /// Conntains helper methods for converting boolean values to oposite.
    /// </summary>
    public class InvertBooleanConverter : IValueConverter
    {
        #region - IValueConverter Members -

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return !((bool)value);
            }

            return false;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return !((bool)value);
            }

            return false;
        }

        #endregion
    } 
}
