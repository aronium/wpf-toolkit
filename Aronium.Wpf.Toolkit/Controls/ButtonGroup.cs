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
        #region - Dependency properties -
        /// <summary>
        /// Identifies element offset property.
        /// </summary>
        public static DependencyProperty ElementOffsetProperty = DependencyProperty.Register("ElementOffset", typeof(Thickness), typeof(ButtonGroup), new PropertyMetadata(new Thickness(-1, 0, 0, 0))); 
        #endregion

        #region - Private methods -

        /// <summary>
        /// Handle visual children being added or removed.
        /// </summary>
        /// <param name="visualAdded">Visual child added.</param>
        /// <param name="visualRemoved">Visual child removed.</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            // Track when objects are added and removed
            if (visualAdded != null && this.Children.Count > 1)
            {
                SetOffset(visualAdded);
            }

            if (visualRemoved != null)
            {
                if (this.Children.Count > 0)
                {
                    if (this.Children[0] is Button)
                    {
                        ((Button)this.Children[0]).Margin = new Thickness(0);
                    }

                    for (int i = 1; i < this.Children.Count; i++)
                    {
                        SetOffset(visualAdded);
                    }
                }
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

        #endregion

        #region - Properties -
        /// <summary>
        /// Gets or sets element offset property.
        /// </summary>
        public Thickness ElementOffset
        {
            get { return (Thickness)GetValue(ElementOffsetProperty); }
            set { SetValue(ElementOffsetProperty, value); }
        } 
        #endregion

        #region - Private properties -

        private bool IsEmptyMargin(Button button)
        {
            return button.Margin.Left == 0 && button.Margin.Right == 0 && button.Margin.Top == 0 && button.Margin.Bottom == 0;
        } 

        #endregion
    }
}
