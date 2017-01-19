using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class TagItem : Control
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(TagItem));
        public static readonly DependencyProperty BackgroundSelectedProperty = DependencyProperty.Register("BackgroundSelected", typeof(Brush), typeof(TagItem));

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

        public Brush BackgroundSelected
        {
            get
            {
                return (Brush)GetValue(BackgroundSelectedProperty);
            }
            set
            {
                SetValue(BackgroundSelectedProperty, value);
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
