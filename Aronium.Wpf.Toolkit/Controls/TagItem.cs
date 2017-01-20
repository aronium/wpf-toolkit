using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class TagItem : Control
    {
        public TagItem()
        {
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            Keyboard.Focus(this);
            Focus();

            // In case control is wrapped in ScrolViewer, prevent ScrollViewer from stealing focus
            e.Handled = true;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            var parent = this.FindVisualParent<TagControl>();

            if (parent != null)
            {
                parent.SelectedItem = this.DataContext;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var closeButtn = Template.FindName("PART_CloseButton", this) as Button;

            if (closeButtn != null)
            {
                closeButtn.Click += (sender, e) =>
                {
                    var parent = this.FindVisualParent<TagControl>();

                    if (parent != null)
                    {
                        parent.Remove(this.DataContext);
                    }
                };
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            var parent = GetParent();

            switch (e.Key)
            {
                case Key.Delete:
                case Key.Back:
                    if (parent != null)
                        parent.Remove(this.DataContext);
                    break;
                case Key.Left:
                case Key.Up:
                    // In case control is wrapped in ScrollViewer, arrows will not work.
                    // Manually force navigateion using arrows
                    if (parent != null)
                        parent.SelectPrevious();
                    e.Handled = true;
                    break;
                case Key.Right:
                case Key.Down:
                    if (parent != null)
                        parent.SelectNext();
                    e.Handled = true;
                    break;
            }
        }

        private TagControl GetParent()
        {
            return this.FindVisualParent<TagControl>();
        }
    }
}
