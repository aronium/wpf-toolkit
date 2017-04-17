using System.Windows;
using System.Windows.Controls;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class GuidedTourItem : Control
    {
        public enum ItemPlacement
        {
            /// <summary>
            /// Indicates left placement.
            /// </summary>
            Left,
            /// <summary>
            /// Indicates top placement.
            /// </summary>
            Top,
            /// <summary>
            /// Indicates right placement.
            /// </summary>
            Right,
            /// <summary>
            /// Indicates bottom placement.
            /// </summary>
            Bottom
        }

        /// <summary>
        /// Identifies ContentProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(GuidedTourItem));

        /// <summary>
        /// Identifies TitleProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(GuidedTourItem));

        /// <summary>
        /// Identifies PlacementProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(ItemPlacement), typeof(GuidedTourItem));

        /// <summary>
        /// Identifies PositionProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Point), typeof(GuidedTourItem));

        /// <summary>
        /// Static constructor for GuidedTourItem class.
        /// </summary>
        static GuidedTourItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GuidedTourItem), new FrameworkPropertyMetadata(typeof(GuidedTourItem)));
        }

        /// <summary>
        /// Gets or sets content.
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets title.
        /// </summary>
        public object Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets or sets target element.
        /// </summary>
        public FrameworkElement Target { get; set; }

        /// <summary>
        /// Gets or sets alternate target elements.
        /// </summary>
        public FrameworkElement[] AlternateTargets { get; set; }

        /// <summary>
        /// Gets or sets placement.
        /// </summary>
        public ItemPlacement Placement
        {
            get { return (ItemPlacement)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        /// <summary>
        /// Gets or sets item tip position.
        /// </summary>
        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Hides this item.
        /// </summary>
        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Shows this item.
        /// </summary>
        public void Show()
        {
            Visibility = Visibility.Visible;
        }
    }
}
