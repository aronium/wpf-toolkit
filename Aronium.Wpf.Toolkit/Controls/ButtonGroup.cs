using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class ButtonGroup : DockPanel
    {
        /// <summary>
        /// Identifies element offset property.
        /// </summary>
        public static DependencyProperty ElementOffsetProperty = DependencyProperty.Register("ElementOffset", typeof(Thickness), typeof(ButtonGroup), new PropertyMetadata(new Thickness(-1, 0, 0, 0)));

        /// <summary>
        /// Handle visual children being added or removed
        /// </summary>
        /// <param name="visualAdded">Visual child added</param>
        /// <param name="visualRemoved">Visual child removed</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            // Track when objects are added and removed
            if (visualAdded != null && this.Children.Count > 1)
            {
                SetOffset(visualAdded);
            }

            if (visualRemoved != null)
            {
                // Do stuff with the removed object
            }

            // Call base function
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        private void SetOffset(DependencyObject element)
        {
            if (element is Button && IsEmptyMargin((Button)element))
            {
                ((Button)element).Margin = this.ElementOffset;
            }
        }


        /// <summary>
        /// Gets or sets element offset property.
        /// </summary>
        public Thickness ElementOffset
        {
            get { return (Thickness)GetValue(ElementOffsetProperty); }
            set { SetValue(ElementOffsetProperty, value); }
        }

        private bool IsEmptyMargin(Button button)
        {
            return button.Margin.Left == 0 && button.Margin.Right == 0 && button.Margin.Top == 0 && button.Margin.Bottom == 0;
        }
    }
}
