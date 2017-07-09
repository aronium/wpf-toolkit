using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class ButtonGroup : StackPanel
    {
        private static Thickness horizontalOffset = new Thickness(-1, 0, 0, 0);
        private static Thickness verticalOffset = new Thickness(0, -1, 0, 0); 

        #region - Private methods -

        /// <summary>
        /// Handle visual children being added or removed.
        /// </summary>
        /// <param name="visualAdded">Visual child added.</param>
        /// <param name="visualRemoved">Visual child removed.</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            // Track when objects are added and removed
            if (visualAdded != null)
            {
                if (visualAdded is ButtonBase)
                {
                    ((ButtonBase)visualAdded).GotFocus += OnChildGotFocus;
                    ((ButtonBase)visualAdded).LostFocus += OnChildLostFocus;
                    ((ButtonBase)visualAdded).IsEnabledChanged += OnChildIsEnabledChanged;
                }

                if (this.Children.Count > 1)
                    SetOffset(visualAdded);
            }

            if (visualRemoved != null)
            {
                if (visualRemoved is ButtonBase)
                {
                    ((ButtonBase)visualRemoved).GotFocus -= OnChildGotFocus;
                    ((ButtonBase)visualRemoved).LostFocus -= OnChildLostFocus;
                }

                if (this.Children.Count > 0)
                {
                    if (this.Children[0] is ButtonBase)
                    {
                        ((ButtonBase)this.Children[0]).Margin = new Thickness(0);
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

        private void OnChildIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ButtonBase)
            {
                if (e.NewValue is bool && !((bool)e.NewValue))
                    ((ButtonBase)sender).SetValue(Panel.ZIndexProperty, -1);
                else
                {
                    ((ButtonBase)sender).SetValue(Panel.ZIndexProperty, -1);
                }
            }
        }

        private void OnChildLostFocus(object sender, RoutedEventArgs e)
        {
            ((ButtonBase)sender).SetValue(Panel.ZIndexProperty, 0);
        }

        private void OnChildGotFocus(object sender, RoutedEventArgs e)
        {
            ((ButtonBase)sender).SetValue(Panel.ZIndexProperty, 1);
        }

        private void SetOffset(DependencyObject element)
        {
            if (element is ButtonBase && IsEmptyMargin((ButtonBase)element))
            {
                ((ButtonBase)element).Margin = this.Orientation == System.Windows.Controls.Orientation.Horizontal ? horizontalOffset : verticalOffset;
            }
        } 

        #endregion

        #region - Private properties -

        private bool IsEmptyMargin(ButtonBase button)
        {
            return button.Margin.Left == 0 && button.Margin.Right == 0 && button.Margin.Top == 0 && button.Margin.Bottom == 0;
        }

        #endregion

    }
}
