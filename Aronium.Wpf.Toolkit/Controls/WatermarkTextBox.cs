using System.Windows;
using System.Windows.Controls;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_Watermark", Type = typeof(TextBlock))]
    public class WatermarkTextBox : TextBox
    {
        private TextBlock watermarkTextBlock;

        public static DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkTextBox));

        public WatermarkTextBox()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            // In case Text property is bound, make sure watermark text is not visible on load.
            // TextChanged event will toggle watermark text visibility, ensure it is set on load, too.
            if(!string.IsNullOrEmpty(Text))
                ToggleWatermarkTextBlock();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            ToggleWatermarkTextBlock();
        }

        private void ToggleWatermarkTextBlock()
        {
            if (watermarkTextBlock != null)
            {
                watermarkTextBlock.Visibility = string.IsNullOrEmpty(this.Text) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            watermarkTextBlock = this.Template.FindName("PART_Watermark", this) as TextBlock;
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }
    }
}
