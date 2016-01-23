using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Aronium.Wpf.Toolkit.Validators
{
    public class NumericValidationRule : ValidationRule
    {
        private decimal _min = 0;
        private decimal _max = Decimal.MaxValue;
        private string _errorMessage;

        public decimal Min
        {
            get { return _min; }
            set { _min = value; }
        }

        public decimal Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);

            decimal parsed = 0M;
            bool isNumberInRange = true;
            if (decimal.TryParse(value.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out parsed))
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
                ErrorMessage = "Value must be number.";
            }

            if (!isNumberInRange)
            {
                result = new ValidationResult(false, this.ErrorMessage);
            }

            return result;
        }
    }
}
