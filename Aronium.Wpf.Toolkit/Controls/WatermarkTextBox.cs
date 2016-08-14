using System.Windows;
using System.Windows.Controls;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_Watermark", Type = typeof(TextBlock))]
    public class WatermarkTextBox : TextBox
    {
        #region - Fields -
        private TextBlock watermarkTextBlock;
        #endregion

        #region - Dependecy properties -

        /// <summary>
        /// Identifies Watermark dependency property.
        /// </summary>
        public static DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkTextBox)); 

        #endregion

        #region - Constructor -

        /// <summary>
        /// Initializes new instance of WatermarkTextBox class.
        /// </summary>
        public WatermarkTextBox()
        {
            Loaded += OnLoaded;
        }

        #endregion

        #region - Private methods -

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            // In case Text property is bound, make sure watermark text is not visible on load.
            // TextChanged event will toggle watermark text visibility, ensure it is set on load, too.
            if (!string.IsNullOrEmpty(Text))
                ToggleWatermark();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            ToggleWatermark();
        }

        private void ToggleWatermark()
        {
            if (watermarkTextBlock != null)
            {
                watermarkTextBlock.Visibility = string.IsNullOrEmpty(this.Text) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #region - Public methods -
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            watermarkTextBlock = this.Template.FindName("PART_Watermark", this) as TextBlock;

            // Make sure watermark is shown or hidden on start.
            // For example, if watermark text box is hidden, make sure watermark is toggled once control becomes visible.
            ToggleWatermark();
        }
        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or sets watermark.
        /// </summary>
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        } 

        #endregion
    }
}
