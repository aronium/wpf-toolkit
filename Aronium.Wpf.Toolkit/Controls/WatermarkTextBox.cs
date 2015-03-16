using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_Watermark", Type = typeof(TextBlock))]
    public class WatermarkTextBox : TextBox
    {
        private TextBlock watermarkTextBlock;

        public static DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkTextBox));

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            if (watermarkTextBlock != null)
            {
                watermarkTextBlock.Visibility = string.IsNullOrEmpty(this.Text) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
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
