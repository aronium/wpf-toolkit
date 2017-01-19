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
    [TemplatePart(Name = "PART_InputCanvas", Type = typeof(Canvas))]
    public class TagsInput : ItemsControl
    {
        #region - Fields -

        private TextBox inputBox;
        private WrapPanel itemsPresenter;
        private Canvas canvas;

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
            canvas = this.Template.FindName("PART_InputCanvas", this) as Canvas;

            inputBox.PreviewKeyDown += OnInputBoxKeyDown;
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
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Tab:
                    if (!string.IsNullOrWhiteSpace(inputBox.Text))
                    {
                        AddTag();
                        e.Handled = true;
                    }
                    break;
                case Key.Back:
                    if (string.IsNullOrEmpty(inputBox.Text) && Items.Count > 0)
                    {
                        var tag = ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as TagItem;

                        if (tag != null)
                            tag.Focus();
                    }
                    break;
            }
        }

        private void SetInputBoxPosition()
        {
            if (Items.Count > 0)
            {
                // Find last item container
                var container = this.ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as TagItem;

                // Make sure container is found
                if (container != null)
                {
                    // Get absolute position of last item container within wrap panel
                    var point = container.TranslatePoint(new Point(0, 0), itemsPresenter);

                    // Calculate right point based on container actual with and X point
                    var left = point.X + container.ActualWidth - 1;

                    // Check if default input box width can fit current width
                    if (left + InputBoxWidth > this.ActualWidth)
                    {
                        // If input box do not fit current width, move it one row down and left to 0
                        left = -1;

                        Canvas.SetLeft(inputBox, left);
                        Canvas.SetTop(inputBox, point.Y + container.ActualHeight + 1);

                        // Set height based on
                        // 1. Last item's Y position
                        // 2. Last item's height (container)
                        // 3. Container margins
                        // 4. Bottom padding
                        // 5. Input box height
                        canvas.Height = point.Y + container.ActualHeight + container.Margin.Bottom + container.Margin.Top + this.Padding.Bottom + inputBox.ActualHeight + 2;
                    }
                    else
                    {
                        // Moving input box to a new row
                        Canvas.SetLeft(inputBox, left);
                        Canvas.SetTop(inputBox, point.Y);

                        canvas.Height = double.NaN;
                    }

                    if (ActualWidth > 0)
                    {
                        // Use dispatcher as it will not work until control is fully resized
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            var inputWidth = this.ActualWidth - (left + this.Padding.Left + this.Padding.Right);

                            if (ActualWidth >= inputWidth)
                                inputBox.Width = inputWidth;
                        }), DispatcherPriority.Input);
                    }
                }
            }
            else
            {
                Canvas.SetLeft(inputBox, -1);
                Canvas.SetTop(inputBox, 0);

                canvas.Height = double.NaN;
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
