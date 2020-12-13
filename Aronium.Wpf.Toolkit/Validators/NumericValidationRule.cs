using System.Globalization;
using System.Windows.Controls;

namespace Aronium.Wpf.Toolkit.Validators
{
    public class NumericValidationRule : ValidationRule
    {
        public decimal Min { get; set; } = 0;

        public decimal Max { get; set; } = decimal.MaxValue;

        public string ErrorMessage { get; set; }

        public bool AllowNull { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (AllowNull && string.IsNullOrEmpty(value?.ToString()))
            {
                return new ValidationResult(true, null);
            }

            bool isNumberInRange = true;

            if (decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsed))
            {
                if (parsed < this.Min || parsed > this.Max)
                {
                    isNumberInRange = false;
                    ErrorMessage = "Value is out of range.";
                }
            }
            else
            {
                isNumberInRange = false;
                ErrorMessage = "Invalid value.";
            }

            return new ValidationResult(isNumberInRange, ErrorMessage);
        }
    }
}
