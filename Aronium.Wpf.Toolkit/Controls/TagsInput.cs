using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_TextInput", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_ItemsHost", Type = typeof(WrapPanel))]
    public class TagsInput : ItemsControl
    {
        public static readonly DependencyProperty InputBoxWidthProperty = DependencyProperty.Register("InputBoxWidth", typeof(double), typeof(TagsInput), new PropertyMetadata(50.0));

        TextBox inputBox;
        WrapPanel itemsPresenter;

        static TagsInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TagsInput), new FrameworkPropertyMetadata(typeof(TagsInput)));
        }

        public TagsInput()
        {
            Focusable = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            inputBox = this.Template.FindName("PART_TextInput", this) as TextBox;
            itemsPresenter = this.Template.FindName("PART_ItemsHost", this) as WrapPanel;

            inputBox.KeyDown += OnInputBoxKeyDown;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);

            SetInputBoxPosition();

            return size;
        }

        private void OnInputBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Tab)
            {
                if (!string.IsNullOrWhiteSpace(inputBox.Text))
                {
                    AddTag();

                    e.Handled = true;
                }
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (Items.Count > 0)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    SetInputBoxPosition();
                }), DispatcherPriority.Input);
            }
        }

        private void SetInputBoxPosition()
        {
            var container = this.ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as TagItem;

            if (container != null)
            {
                var point = container.TranslatePoint(new Point(0, 0), itemsPresenter);

                var left = point.X + container.ActualWidth - 1;
                if (left + InputBoxWidth > this.ActualWidth)
                {
                    left = 0;

                    Canvas.SetLeft(inputBox, -1);
                    Canvas.SetTop(inputBox, point.Y + container.ActualHeight + 1);

                    this.Height = point.Y + container.ActualHeight + container.Margin.Bottom + container.Margin.Top + this.Padding.Bottom + inputBox.ActualHeight;
                }
                else
                {
                    Canvas.SetLeft(inputBox, left);
                    Canvas.SetTop(inputBox, point.Y);

                    this.Height = double.NaN;
                }

                if (ActualWidth > 0)
                {
                    var inputWidth = this.ActualWidth - (left + this.Padding.Left + this.Padding.Right);

                    if (ActualWidth >= inputWidth)
                        inputBox.Width = inputWidth;
                }
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if(e.OriginalSource is UIElement)
            {
                var tagItem = (e.OriginalSource as DependencyObject).FindVisualParent<TagItem>();

                if(tagItem == null)
                {
                    inputBox.Focus();
                    inputBox.SelectAll();
                }
                else
                {
                    tagItem.Focus();
                }
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TagItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TagItem;
        }

        public double InputBoxWidth
        {
            get { return (double)GetValue(InputBoxWidthProperty); }
            set { SetValue(InputBoxWidthProperty, value); }
        }

        private void AddTag()
        {
            if (ItemsSource == null)
                Items.Add(inputBox.Text);
            else if (ItemsSource is IList)
                (ItemsSource as IList).Add(inputBox.Text);

            inputBox.Clear();
        }
    }

}
