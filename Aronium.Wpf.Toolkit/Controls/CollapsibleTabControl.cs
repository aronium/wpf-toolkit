using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_ButtonCollapse", Type = typeof(Button))]
    public class CollapsibleTabControl : TabControl
    {
        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(CollapsibleTabControl), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty VerticalSeparatorBrushProperty = DependencyProperty.Register("VerticalSeparatorBrush", typeof(Brush), typeof(CollapsibleTabControl));
        public static readonly DependencyProperty ItemsBackgroundBrushProperty = DependencyProperty.Register("ItemsBackgroundBrush", typeof(Brush), typeof(CollapsibleTabControl));

        static CollapsibleTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapsibleTabControl), new FrameworkPropertyMetadata(typeof(CollapsibleTabControl)));
        }

        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        public Brush VerticalSeparatorBrush
        {
            get { return (Brush)GetValue(VerticalSeparatorBrushProperty); }
            set { SetValue(VerticalSeparatorBrushProperty, value); }
        }

        public Brush ItemsBackgroundBrush
        {
            get { return (Brush)GetValue(ItemsBackgroundBrushProperty); }
            set { SetValue(ItemsBackgroundBrushProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var collapseButton = this.Template.FindName("PART_ButtonCollapse", this) as Button;

            if (collapseButton != null)
            {
                collapseButton.Click += (sender, e) =>
                {
                    IsCollapsed = !IsCollapsed;
                };
            }

        }
    }
}
