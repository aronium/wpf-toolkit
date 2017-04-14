using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class GuideItem : Control
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(GuideItem));
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(GuideItem));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Point), typeof(GuideItem));

        static GuideItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GuideItem), new FrameworkPropertyMetadata(typeof(GuideItem)));
        }

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public FrameworkElement Target { get; set; }

        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }
    }
}
