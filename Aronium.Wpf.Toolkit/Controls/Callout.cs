using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Aronium.Wpf.Toolkit.Controls
{
    [ContentProperty("Content")]
    public class Callout : Control
    {
        static Callout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Callout), new FrameworkPropertyMetadata(typeof(Callout)));
        }

        public static DependencyProperty CalloutTypeProperty = DependencyProperty.Register("CalloutType", typeof(CalloutType), typeof(Callout), new PropertyMetadata(CalloutType.Info));
        public static DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Callout));
        public static DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(Callout));

        /// <summary>
        /// Gets or sets callout type.
        /// </summary>
        public CalloutType CalloutType
        {
            get { return (CalloutType)GetValue(CalloutTypeProperty); }
            set { SetValue(CalloutTypeProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
    }

    public enum CalloutType
    {
        Info,
        Warning,
        Error
    }
}
