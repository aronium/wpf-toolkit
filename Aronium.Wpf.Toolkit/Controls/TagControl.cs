using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Linq;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_TextInput", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_ItemsHost", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_InputCanvas", Type = typeof(Canvas))]
    public class TagControl : ItemsControl
    {
        #region - Fields -

        private TextBox inputBox;
        private Panel itemsHostPanel;
        private Canvas canvas;

        #endregion

        #region - Events -

        public static readonly RoutedEvent ItemAddedEvent = EventManager.RegisterRoutedEvent("ItemAdded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TagControl));
        public static readonly RoutedEvent ItemRemovedEvent = EventManager.RegisterRoutedEvent("ItemRemoved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TagControl));

        public event RoutedEventHandler ItemAdded
        {
            add { AddHandler(ItemAddedEvent, value); }
            remove { RemoveHandler(ItemAddedEvent, value); }
        }

        public event RoutedEventHandler ItemRemoved
        {
            add { AddHandler(ItemRemovedEvent, value); }
            remove { RemoveHandler(ItemRemovedEvent, value); }
        }

        #endregion

        #region - Dependency Properties -

        /// <summary>
        /// Identifies InputBoxWidthProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty InputBoxWidthProperty = DependencyProperty.Register("InputBoxWidth", typeof(double), typeof(TagControl), new PropertyMetadata(50.0));

        /// <summary>
        /// Idenftifies AllowDuplicatesProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowDuplicatesProperty = DependencyProperty.Register("AllowDuplicates", typeof(bool), typeof(TagControl), new PropertyMetadata(false));

        /// <summary>
        /// Idenftifies CanAddTags dependency property.
        /// </summary>
        public static readonly DependencyProperty UserCanAddTagsProperty = DependencyProperty.Register("UserCanAddTags", typeof(bool), typeof(TagControl), new PropertyMetadata(true));

        /// <summary>
        /// Identifies SelectedItemProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem",
            typeof(object),
            typeof(TagControl),
            new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        #endregion

        #region - Constructors -

        static TagControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TagControl), new FrameworkPropertyMetadata(typeof(TagControl)));
        }

        /// <summary>
        /// Initializes new instance of TagsInput class.
        /// </summary>
        public TagControl()
        {
            Focusable = false;

            LostFocus += OnLostFocus;
        }

        #endregion

        #region - Private methods -

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            AddTag();
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
                case Key.Left:
                case Key.Up:
                    if (string.IsNullOrEmpty(inputBox.Text) && Items.Count > 0)
                    {
                        SelectItemAt(Items.Count - 1);

                        e.Handled = true;
                    }
                    break;
            }
        }

        private void SelectItemAt(int index)
        {
            if (index < 0 || index > Items.Count - 1) return;

            var tag = ItemContainerGenerator.ContainerFromIndex(index) as TagItem;

            if (tag != null)
            {
                tag.Focus();
                Keyboard.Focus(tag);

                this.SelectedItem = tag.DataContext;
            }
        }

        private void CalculateSizeAndInputBoxPosition()
        {
            if (Items.Count > 0)
            {
                // Find last item container
                var container = this.ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as TagItem;

                // Make sure container is found
                if (container != null)
                {
                    // Get absolute position of last item container within wrap panel
                    var point = container.TranslatePoint(new Point(0, 0), itemsHostPanel);

                    // Calculate right point based on container actual with and X point
                    var left = point.X + container.ActualWidth - 1;

                    // Check if default input box width can fit current width
                    if (left + InputBoxWidth > this.ActualWidth)
                    {
                        // If input box do not fit current width, move it one row down and left to 0
                        left = -1;

                        Canvas.SetLeft(inputBox, left);
                        Canvas.SetTop(inputBox, point.Y + container.ActualHeight + 1);

                        // Canvas height will set actual control height
                        // Set canvas height based on the following:
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
            else if (inputBox != null)
            {
                Canvas.SetLeft(inputBox, -1);
                Canvas.SetTop(inputBox, 0);

                inputBox.Width = this.ActualWidth;

                canvas.Height = double.NaN;
            }
        }

        private void AddTag()
        {
            if (string.IsNullOrEmpty(inputBox.Text)) return;

            if (!AllowDuplicates)
            {
                if (Items.IndexOf(inputBox.Text) >= 0)
                {
                    inputBox.Clear();

                    return;
                }
            }

            if (ItemsSource == null)
                Items.Add(inputBox.Text);
            else if (ItemsSource is IList)
                (ItemsSource as IList).Add(inputBox.Text);

            inputBox.Clear();

            RaiseEvent(new RoutedEventArgs(ItemAddedEvent));
        }

        internal void Remove(object item)
        {
            if (ItemsSource == null)
                this.Items.Remove(item);
            else if (ItemsSource is IList)
                (ItemsSource as IList).Remove(item);

            FocusInputBox();

            RaiseEvent(new RoutedEventArgs(ItemRemovedEvent));
        }

        private void FocusInputBox()
        {
            inputBox.Focus();
            Keyboard.Focus(inputBox);

            inputBox.SelectAll();
        }

        internal void SelectNext()
        {
            if (Items.Count == 0) return;

            var index = this.Items.IndexOf(this.SelectedItem);

            if (index < Items.Count - 1)
            {
                SelectItemAt(index + 1);
            }
            else
            {
                FocusInputBox();
            }
        }

        internal void SelectPrevious()
        {
            if (Items.Count == 0) return;

            var index = this.Items.IndexOf(this.SelectedItem);

            if (index > 0)
            {
                SelectItemAt(index - 1);
            }
        }

        #endregion

        #region - Overrides -

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            inputBox = Template.FindName("PART_TextInput", this) as TextBox;
            itemsHostPanel = Template.FindName("PART_ItemsHost", this) as Panel;
            canvas = Template.FindName("PART_InputCanvas", this) as Canvas;

            inputBox.PreviewKeyDown += OnInputBoxKeyDown;

            if (this.IsFocused)
            {
                FocusInputBox();
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            Dispatcher.BeginInvoke((Action)(() =>
            {
                CalculateSizeAndInputBoxPosition();
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
                    FocusInputBox();

                    e.Handled = true;
                }
                else
                {
                    tagItem.Focus();
                    Keyboard.Focus(tagItem);
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

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            CalculateSizeAndInputBoxPosition();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            switch (e.Key)
            {
                case Key.Home:
                    SelectItemAt(0);
                    e.Handled = true;
                    break;
                case Key.End:
                    SelectItemAt(Items.Count - 1);
                    e.Handled = true;
                    break;
                case Key.C:
                    if (SelectedItem != null && Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        Clipboard.SetDataObject(SelectedItem.ToString(), true);
                        e.Handled = true;
                    }
                    break;
            }
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

        /// <summary>
        /// Gets or sets a value indicating whether list should accept duplicates.
        /// </summary>
        public bool AllowDuplicates
        {
            get { return (bool)GetValue(AllowDuplicatesProperty); }
            set { SetValue(AllowDuplicatesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether user can add new tags.
        /// </summary>
        public bool UserCanAddTags
        {
            get { return (bool)GetValue(UserCanAddTagsProperty); }
            set { SetValue(UserCanAddTagsProperty, value); }
        }

        #endregion
    }
}
