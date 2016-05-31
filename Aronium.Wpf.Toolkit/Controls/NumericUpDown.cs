using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class NumericUpDown : TextBox
    {
        #region - Fields -

        private string stringFormat = "N0";
        private static Regex regex = new Regex(@"^[0-9.,-]+$");
        private decimal decimalValueParsed;
        private bool isUpdatingText;

        #endregion

        #region - Dependency properties -

        /// <summary>
        /// Identifies decimal places property.
        /// </summary>
        public static readonly DependencyProperty DecimalPlacesProperty = DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(NumericUpDown), new PropertyMetadata(0, new PropertyChangedCallback(OnDecimalPlacesChanged)));

        /// <summary>
        /// Identifies minimum property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(decimal), typeof(NumericUpDown), new PropertyMetadata(0M));

        /// <summary>
        /// Identifies maximum property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(decimal), typeof(NumericUpDown), new PropertyMetadata(Decimal.MaxValue));

        /// <summary>
        /// Identifies increment property.
        /// </summary>
        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(decimal), typeof(NumericUpDown), new PropertyMetadata(1M));

        /// <summary>
        /// Identifies value property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(decimal), typeof(NumericUpDown));

        /// <summary>
        /// Identifies AcceptEmptyValueProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty AcceptEmptyValueProperty = DependencyProperty.Register("AcceptEmptyValue", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(true));


        /// <summary>
        /// Identifies ShowUpDownArrowsProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowPlusMinusButtonsProperty = DependencyProperty.Register("ShowPlusMinusButtons", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(true));

        #endregion

        #region - Constructor -
        
        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        /// <summary>
        /// Initializes new isntance of NumericUpDown class.
        /// </summary>
        public NumericUpDown()
        {
            // Use global culture for e.g. decimal separators
            this.Language = XmlLanguage.GetLanguage(System.Globalization.CultureInfo.InvariantCulture.IetfLanguageTag);

            this.PreviewTextInput += new TextCompositionEventHandler(OnPreviewTextInput);
            this.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);

            LostFocus += OnLostFocus;
        }

        #endregion

        #region - Private methods -

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            UpdateText();
        }

        private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = ((NumericUpDown)d);

            @this.stringFormat = "N" + e.NewValue;

            @this.UpdateText();
        }

        protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta > 0)
            {
                Up();
            }
            else
            {
                Down();
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (!isUpdatingText)
            {
                if (string.IsNullOrEmpty(this.Text) || string.IsNullOrWhiteSpace(this.Text))
                {
                    if (!AcceptEmptyValue)
                        this.UpdateText(this.Minimum);

                    return;
                }

                var parsed = regex.IsMatch(this.Text);

                if (!parsed)
                {
                    this.Text = Regex.Replace(this.Text, "[^0-9.,]+", string.Empty);

                    UpdateText();
                }
            }

            base.OnTextChanged(e);
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down: Down(); break;
                case Key.Up: Up(); break;
            }
        }

        private void UpdateText()
        {
            if (!IsFocused)
            {
                if (string.IsNullOrEmpty(this.Text.Trim()))
                {
                    if (!AcceptEmptyValue)
                        this.UpdateText(this.Minimum);
                }
                else if (decimal.TryParse(this.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimalValueParsed))
                {
                    this.UpdateText(decimalValueParsed);
                }
            }
        }

        private void UpdateText(decimal value)
        {
            this.isUpdatingText = true;

            Value = value;

            this.SetValue(TextProperty, value.ToString(stringFormat, CultureInfo.InvariantCulture));

            this.Select(Text.Length, 0);

            this.isUpdatingText = false;
        }

        private void Up()
        {
            if (!this.IsEnabled || this.IsReadOnly) return;

            if (!IsFocused)
            {
                this.Focus();
            }

            if (decimal.TryParse(this.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimalValueParsed))
            {
                if (decimalValueParsed + Increment <= Maximum)
                {
                    UpdateText(decimalValueParsed + Increment);
                }
            }
        }

        private void Down()
        {
            if (!this.IsEnabled || this.IsReadOnly) return;

            if (!IsFocused)
            {
                this.Focus();
            }

            if (decimal.TryParse(this.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimalValueParsed))
            {
                if (decimalValueParsed - Increment >= Minimum)
                {
                    UpdateText(decimalValueParsed - Increment);
                }
            }
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or sets number of decimal places to display.
        /// </summary>
        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        /// <summary>
        /// Gets or sets icrement value.
        /// </summary>
        public decimal Increment
        {
            get { return (decimal)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        /// <summary>
        /// Gets or sets maximum value.
        /// </summary>
        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Gets or sets minimum value.
        /// </summary>
        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        [Bindable(true)]
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether empty text should be automatically converted to Minimum value.
        /// </summary>
        public bool AcceptEmptyValue
        {
            get { return (bool)GetValue(AcceptEmptyValueProperty); }
            set { SetValue(AcceptEmptyValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether plus and minus buttons are displayed.
        /// </summary>
        public bool ShowPlusMinusButtons
        {
            get { return (bool)GetValue(ShowPlusMinusButtonsProperty); }
            set { SetValue(ShowPlusMinusButtonsProperty, value); }
        }

        #endregion

        #region - Public methods -

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var upButton = this.Template.FindName("PART_Up", this) as RepeatButton;
            var downButton = this.Template.FindName("PART_Down", this) as RepeatButton;

            if (upButton != null)
            {
                upButton.Click += (sender, e) => { Up(); };
            }

            if (downButton != null)
            {
                downButton.Click += (sender, e) => { Down(); };
            }
        }

        #endregion
    }
}
