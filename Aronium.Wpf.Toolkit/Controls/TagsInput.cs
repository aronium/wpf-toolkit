using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_TextInput", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_ItemsHost", Type = typeof(WrapPanel))]
    public class TagsInput : ItemsControl
    {
        #region - Fields -

        private TextBox inputBox;
        private WrapPanel itemsPresenter;

        #endregion

        #region - Dependency Properties -

        public static readonly DependencyProperty InputBoxWidthProperty = DependencyProperty.Register("InputBoxWidth", typeof(double), typeof(TagsInput), new PropertyMetadata(50.0));
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem",
            typeof(object),
            typeof(TagsInput),
            new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        #endregion

        #region - Constructors -

        static TagsInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TagsInput), new FrameworkPropertyMetadata(typeof(TagsInput)));
        }

        /// <summary>
        /// Initializes new instance of TagsInput class.
        /// </summary>
        public TagsInput()
        {
            Focusable = false;

            this.LostFocus += OnLostFocus;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            AddTag();
        }

        #endregion

        #region - Overrides -

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

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            Dispatcher.BeginInvoke((Action)(() =>
            {
                SetInputBoxPosition();
            }), DispatcherPriority.Input);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.OriginalSource is UIElement)
            {
                var tagItem = (e.OriginalSource as DependencyObject).FindVisualParent<TagItem>();

                if (tagItem == null)
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

        #endregion

        #region - Private methods -

        internal void Remove(object item)
        {
            if (ItemsSource == null)
                this.Items.Remove(item);
            else if (ItemsSource is IList)
                (ItemsSource as IList).Remove(item);

            inputBox.Focus(); inputBox.SelectAll();
        }

        private void OnInputBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (!string.IsNullOrWhiteSpace(inputBox.Text))
                {
                    AddTag();

                    e.Handled = true;
                }
            }
        }

        private void SetInputBoxPosition()
        {
            if (Items.Count > 0)
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
            else
            {
                Canvas.SetLeft(inputBox, -1);
                Canvas.SetTop(inputBox, 0);

                this.Height = double.NaN;
            }
        }

        private void AddTag()
        {
            if (string.IsNullOrEmpty(inputBox.Text)) return;

            if (ItemsSource == null)
                Items.Add(inputBox.Text);
            else if (ItemsSource is IList)
                (ItemsSource as IList).Add(inputBox.Text);

            inputBox.Clear();
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or sets input box minimum width.
        /// </summary>
        public double InputBoxWidth
        {
            get { return (double)GetValue(InputBoxWidthProperty); }
            set { SetValue(InputBoxWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        #endregion
    }

}
