using System;
using System.Windows.Data;

namespace Aronium.Wpf.Toolkit.Converters
{
    /// <summary>
    /// Class represents the converter that converts Boolean values to and from <see cref="System.Windows.Visibility"/> enumeration values.
    /// <para>Converter uses value negation to return Visible property.</para>
    /// </summary>
    public class InvertBooleanToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts <see cref="System.Boolean"/> to <see cref="System.Windows.Visibility"/> enumeration value.
        /// </summary>
        /// <param name="value">Boolean value used for conversion.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns><see cref="System.Windows.Visibility.Visible"/> if specified parameter value is <see langword="false"/>, otherwise <see cref="System.Windows.Visibility.Collapsed"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return !((bool)value) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            return null;
        }

        /// <summary>
        /// Converts <see cref="System.Windows.Visibility"/> enumeration value to <see langword="bool"/>.
        /// </summary>
        /// <param name="value">Visibility value used for conversion.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>If value is Visible returns <see langword="false"/> otherwise <see langword="true"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (System.Windows.Visibility)value == System.Windows.Visibility.Visible;
        }
        #endregion
    } 
}
