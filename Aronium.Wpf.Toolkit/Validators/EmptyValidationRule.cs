using System.Windows.Controls;

namespace Aronium.Wpf.Toolkit.Validators
{
    public class EmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value as string))
                return new ValidationResult(false, "Value cannot be empty.");

            return ValidationResult.ValidResult;
        }
    }
}
