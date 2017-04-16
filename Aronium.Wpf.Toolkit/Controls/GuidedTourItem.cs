using System.Windows;
using System.Windows.Controls;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class GuidedTourItem : Control
    {
        public enum ItemPlacement
        {
            Left, Top, Right, Bottom
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(GuidedTourItem));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(GuidedTourItem));
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(ItemPlacement), typeof(GuidedTourItem));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Point), typeof(GuidedTourItem));

        static GuidedTourItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GuidedTourItem), new FrameworkPropertyMetadata(typeof(GuidedTourItem)));
        }

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public object Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public FrameworkElement Target { get; set; }

        public ItemPlacement Placement
        {
            get { return (ItemPlacement)GetValue(PlacementProperty); }
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
