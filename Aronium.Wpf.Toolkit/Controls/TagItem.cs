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

        public TagItem(string value)
        {
            this.Value = value;

            //Focusable = true;
            //IsTabStop = true;

            this.MouseDown += OnMouseDown;
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

            var parent = this.FindVisualParent<TagsInput>();

            if(parent != null)
            {
                parent.SelectedItem = this.DataContext;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var closeButtn = this.Template.FindName("PART_CloseButton", this) as Button;
            if(closeButtn != null)
            {
                closeButtn.Click += (sender, e) =>
                {
                    var parent = this.FindVisualParent<TagsInput>();

                    if (parent != null)
                    {
                        parent.Remove(this.DataContext);
                    }
                };
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Delete)
            {
                var parent = this.FindVisualParent<TagsInput>();

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
