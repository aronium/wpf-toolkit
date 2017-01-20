using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class TagItem : Control
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(TagItem));

        public TagItem()
        {
        }

        private void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Keyboard.Focus(this);
            Focus();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            var parent = this.FindVisualParent<TagControl>();

            if(parent != null)
            {
                parent.SelectedItem = this.DataContext;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var closeButtn = Template.FindName("PART_CloseButton", this) as Button;

            if(closeButtn != null)
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

            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                var parent = this.FindVisualParent<TagControl>();

                if (parent != null)
                {
                    parent.Remove(this.DataContext);
                }
            }
        }

        public string Value
        {
            get
            {
                return (string)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
