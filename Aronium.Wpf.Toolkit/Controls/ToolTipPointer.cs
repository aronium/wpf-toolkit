using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class ToolTipPointer : ToolTip
    {
        private Path _arrow;

        protected override void AddChild(object value)
        {
            base.AddChild(value);
        }

        public ToolTipPointer()
        {
            this.SizeChanged += new System.Windows.SizeChangedEventHandler(OnSizeChanged);
        }

        void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {

            switch (this.Placement)
            {
                case PlacementMode.Left:
                case PlacementMode.Right:

                    double targetHeight = 0;

                    if (this.PlacementTarget is FrameworkElement)
                    {
                        targetHeight = (this.PlacementTarget as System.Windows.FrameworkElement).ActualHeight;
                    }

                    // Arrow will be drawn on center of the tooltip content, move it up by half size
                    this.VerticalOffset = -((this.ActualHeight / 2.0) - (targetHeight / 2.0) - (_arrow.ActualHeight / 2.0) + 1.0);

                    break;
                case PlacementMode.Bottom:
                case PlacementMode.Top:
                    double targetWidth = 0;

                    if (this.PlacementTarget is FrameworkElement)
                    {
                        targetWidth = (this.PlacementTarget as System.Windows.FrameworkElement).ActualWidth;
                    }

                    // Arrow will be drawn on center of the tooltip content, move it left by half size
                    this.HorizontalOffset = -((this.ActualWidth / 2.0) - (targetWidth / 2.0) - (_arrow.ActualWidth / 2.0) + 1.0);

                    break;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _arrow = this.Template.FindName("PART_Arrow", this) as Path;
        }
    }
}
