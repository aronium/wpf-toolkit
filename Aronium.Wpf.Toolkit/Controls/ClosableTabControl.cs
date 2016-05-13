using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_DropDown", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_ScrollLeft", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_ScrollRight", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    public class ClosableTabControl : TabControl
    {
        #region - Enums -
        private enum ScrollDirection
        {
            Left, Right
        }
        #endregion

        #region - Events -

        /// <summary>
        /// Occurs when child item is about to close.
        /// </summary>
        public event EventHandler<ClosableItemEventArgs> ItemClosing;

        #endregion

        #region - Fields -

        private ScrollDirection CurrentScrollDirection;
        private ToggleButton _toggleButton;
        private RepeatButton leftArrowButton, rightArrowButton;

        private IEnumerable<TabItem> InternalChildren
        {
            get
            {
                IEnumerator enumerator = ((IEnumerable)Items).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is TabItem)
                        yield return enumerator.Current as TabItem;
                    else
                        yield return this.ItemContainerGenerator.ContainerFromItem(enumerator.Current) as TabItem;
                }
            }
        }

        //private DispatcherTimer timer;
        //private Stopwatch stopwatch;

        private ScrollViewer scrollViewer;

        #endregion

        #region - Dependency Properties -

        /// <summary>
        /// Identifies IsVisibleWhenEmpty property.
        /// </summary>
        public static readonly DependencyProperty IsVisibleWhenEmptyProperty = DependencyProperty.Register("IsVisibleWhenEmpty", typeof(bool), typeof(ClosableTabControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies IsClosableProperty property.
        /// </summary>
        public static readonly DependencyProperty IsClosableProperty = DependencyProperty.Register("IsClosable", typeof(bool), typeof(ClosableTabControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies AllowItemsReorder property.
        /// </summary>
        public static readonly DependencyProperty AllowItemsReorderProperty = DependencyProperty.Register("AllowItemsReorder", typeof(bool), typeof(ClosableTabControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies Header property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ClosableTabControl));

        #endregion

        #region - Constructors -

        static ClosableTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClosableTabControl), new FrameworkPropertyMetadata(typeof(ClosableTabControl)));
        }

        /// <summary>
        /// Initializes new instance of ClosableTabControl class.
        /// </summary>
        public ClosableTabControl()
        {
            // Listen to Ctrl+W for selected item close
            this.KeyUp += (sender, e) =>
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (this.SelectedItem != null && e.Key == Key.W && this.SelectedItem is ClosableTabItem && ((ClosableTabItem)this.SelectedItem).CanClose)
                    {
                        this.Items.Remove(this.SelectedItem);
                        return;
                    }
                }
            };

            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        #endregion

        #region - Overrides -

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.scrollViewer = this.Template.FindName("PART_ScrollViewer", this) as ScrollViewer;
                this.rightArrowButton = this.Template.FindName("PART_ScrollRight", this) as RepeatButton;
                this.leftArrowButton = this.Template.FindName("PART_ScrollLeft", this) as RepeatButton;
                Button dropDownButton = this.Template.FindName("PART_DropDown", this) as Button;

                // set up the event handler for the template parts
                _toggleButton = this.Template.FindName("PART_DropDown", this) as ToggleButton;
                if (_toggleButton != null)
                {
                    // create a context menu for the togglebutton
                    ContextMenu cm = new ContextMenu { PlacementTarget = _toggleButton, Placement = PlacementMode.Bottom };

                    // create a binding between the togglebutton's IsChecked Property
                    // and the Context Menu's IsOpen Property
                    Binding b = new Binding
                    {
                        Source = _toggleButton,
                        Mode = BindingMode.TwoWay,
                        Path = new PropertyPath(ToggleButton.IsCheckedProperty)
                    };

                    cm.SetBinding(ContextMenu.IsOpenProperty, b);

                    _toggleButton.ContextMenu = cm;
                    _toggleButton.Checked += DropdownButton_Checked;
                }

                if (scrollViewer != null && rightArrowButton != null)
                {
                    EnableRightButton(rightArrowButton, scrollViewer);

                    rightArrowButton.Click += delegate
                    {
                        scrollViewer.LineRight();
                    };

                }

                if (scrollViewer != null && leftArrowButton != null)
                {
                    leftArrowButton.IsEnabled = false;
                    leftArrowButton.Click += delegate
                    {
                        scrollViewer.LineLeft();
                    };
                }

                if (scrollViewer != null)
                {
                    scrollViewer.ScrollChanged += delegate
                    {
                        if (leftArrowButton != null)
                        {
                            leftArrowButton.IsEnabled = scrollViewer.ContentHorizontalOffset > 0;
                        }
                        if (rightArrowButton != null)
                        {
                            EnableRightButton(rightArrowButton, scrollViewer);
                        }
                    };
                }
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (!IsVisibleWhenEmpty)
            {
                if (this.Items.Count == 0)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                }
            }
            return base.ArrangeOverride(arrangeBounds);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ClosableTabItem() { CanClose = this.IsClosable };
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ClosableTabItem;
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && !this.IsClosable)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is ClosableTabItem)
                    {
                        ((ClosableTabItem)item).CanClose = false;
                    }
                }
            }
        }

        #endregion

        #region - Private methods -

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnLoaded;

            if (!IsClosable)
            {
                foreach (var item in this.Items)
                {
                    if (item is ClosableTabItem)
                    {
                        ((ClosableTabItem)item).CanClose = false;
                    }
                }
            }
        }

        /// <summary>
        /// Handle the ToggleButton Checked event that displays a context menu of Header Headers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DropdownButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_toggleButton == null) return;

            _toggleButton.ContextMenu.Items.Clear();
            _toggleButton.ContextMenu.Placement = TabStripPlacement == Dock.Bottom ? PlacementMode.Top : PlacementMode.Bottom;

            int index = 0;
            foreach (TabItem tabItem in this.InternalChildren)
            {
                if (tabItem != null)
                {
                    if (!tabItem.IsVisible)
                    {
                        index++;
                        continue;
                    }

                    var header = tabItem.Header;

                    var mi = new MenuItem { Header = header, Tag = index++.ToString() };
                    mi.Click += ContextMenuItem_Click;
                    if (index == this.SelectedIndex + 1)
                    {
                        mi.IsCheckable = true;
                        mi.IsChecked = true;
                    }

                    _toggleButton.ContextMenu.Items.Add(mi);
                }
            }
        }

        /// <summary>
        /// Handle the MenuItem's Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi == null) return;

            int index;
            // get the index of the Header from the manuitems Tag property
            bool b = int.TryParse(mi.Tag.ToString(), out index);

            this.SelectedIndex = index;

            // Check if item is tabitem, itemsource could be set to object collection
            if (this.SelectedItem is TabItem)
            {
                (this.SelectedItem as TabItem).BringIntoView();
            }
            else
            {
                (this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as TabItem).BringIntoView();
            }
        }

        void EnableRightButton(RepeatButton rightButton, ScrollViewer scrollViewer)
        {
            var horizontalOffset = scrollViewer.HorizontalOffset;
            var maxhorizontalOffset = scrollViewer.ExtentWidth - scrollViewer.ViewportWidth;
            rightButton.IsEnabled = maxhorizontalOffset > 0 && horizontalOffset < maxhorizontalOffset;
        }

        /// <summary>
        /// Removes tab item.
        /// </summary>
        /// <param name="item">Tab item to remove.</param>
        internal void RemoveItem(ClosableTabItem item)
        {
            var args = new ClosableItemEventArgs(item);

            if (ItemClosing != null)
            {
                ItemClosing(this, args);

                if (args.Cancel)
                    return;
            }

            item.NotifyClosed();

            this.Items.Remove(item);
        }

        #endregion

        #region - Properties - 

        /// <summary>
        /// Gets or sets a value indicating whether control is visible when no items are present.
        /// </summary>
        public bool IsVisibleWhenEmpty
        {
            get { return (bool)GetValue(IsVisibleWhenEmptyProperty); }
            set { SetValue(IsVisibleWhenEmptyProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tab items can be closed.
        /// </summary>
        public bool IsClosable
        {
            get { return (bool)GetValue(IsClosableProperty); }
            set { SetValue(IsClosableProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether items can be reordered.
        /// </summary>
        public bool AllowItemsReorder
        {
            get { return (bool)GetValue(AllowItemsReorderProperty); }
            set { SetValue(AllowItemsReorderProperty, value); }
        }

        /// <summary>
        /// Gets or sets header.
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        #endregion

        #region - Public Methods -

        /// <summary>
        /// Removes all tabs
        /// </summary>
        public void CloseAll()
        {
            List<TabItem> items = new List<TabItem>(InternalChildren);

            foreach (ClosableTabItem item in items)
            {
                if (item.CanClose)
                {
                    RemoveItem(item);
                }
            }
        }

        /// <summary>
        /// Removes all tabs except active one
        /// </summary>
        public void CloseAllButActive()
        {
            List<TabItem> items = new List<TabItem>(InternalChildren);

            foreach (ClosableTabItem item in items)
            {
                if (item != this.SelectedItem && item.CanClose)
                {
                    RemoveItem(item);
                }
            }
        }

        /// <summary>
        /// Removes all items except one passed in as parameter
        /// </summary>
        /// <param name="closableTabItem">Close all closable items except this one</param>
        internal void CloseAllButThis(ClosableTabItem closableTabItem)
        {
            List<TabItem> items = new List<TabItem>(InternalChildren);

            foreach (ClosableTabItem item in items)
            {
                if (item != closableTabItem && item.CanClose)
                {
                    RemoveItem(item);
                }
            }
        }

        /// <summary>
        /// Closes currently selected tab, if any
        /// </summary>
        public void CloseActiveTab()
        {
            if (this.SelectedItem != null && (this.SelectedItem is ClosableTabItem && ((ClosableTabItem)this.SelectedItem).CanClose))
            {
                RemoveItem(this.SelectedItem as ClosableTabItem);
            }
        }

        /// <summary>
        /// Gets first tab item with same header
        /// </summary>
        /// <param name="header">Header / Tab title to lookup</param>
        /// <returns><see cref="ClosableTabItem"/> if exists, otherwise null</returns>
        /// <remarks>Header comparatino is case-sensitive</remarks>
        public TabItem GetItem(string header)
        {
            foreach (TabItem item in InternalChildren)
            {
                if (item.Header.ToString() == header)
                {
                    return item;
                }
            }

            return null;
        }

        #endregion
    }

    public class ClosableItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes new instance of ClosableItemEventArgs class.
        /// </summary>
        public ClosableItemEventArgs() { }

        /// <summary>
        /// Initializes new instance of ClosableItemEventArgs class with specified source item.
        /// </summary>
        /// <param name="item">Event source.</param>
        public ClosableItemEventArgs(TabItem item)
        {
            this.Item = item;
        }

        /// <summary>
        /// Gets or sets source item.
        /// </summary>
        public TabItem Item { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether event should be canceled.
        /// </summary>
        public bool Cancel { get; set; }
    }
}
