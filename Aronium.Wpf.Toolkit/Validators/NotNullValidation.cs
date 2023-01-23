using System.Windows.Controls;

namespace Aronium.Wpf.Toolkit.Validators
{
    public class NotNullValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            return value == null
                ? new ValidationResult(false, "Field is required")
                : new ValidationResult(true, null);
        }
    }
}
