using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_SearchTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_ListBox", Type = typeof(ListBox))]
    public class SearchableComboBox : ComboBox
    {
        #region - Events -

        public event EventHandler<SearchTextChangedEventArgs> SearchTextChanged;

        #endregion

        #region - Dependency properties -

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(nameof(Watermark),
            typeof(string),
            typeof(SearchableComboBox),
            new PropertyMetadata("Search..."));

        public static readonly DependencyProperty NoItemsTextProperty = DependencyProperty.Register(nameof(NoItemsText),
            typeof(string),
            typeof(SearchableComboBox),
            new PropertyMetadata("No items"));

        public static readonly DependencyProperty AllowNullSelectionProperty = DependencyProperty.Register(nameof(AllowNullSelection),
            typeof(bool),
            typeof(SearchableComboBox),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ClearSearchOnCloseProperty = DependencyProperty.Register(nameof(ClearSearchOnClose),
            typeof(bool),
            typeof(SearchableComboBox),
            new PropertyMetadata(true));

        #endregion

        #region - Fields -

        private object cachedSelectedItem;

        protected TextBox SearchTextBox => GetTemplateChild("PART_SearchTextBox") as TextBox;
        protected ListBox ItemsListBox => GetTemplateChild("PART_ListBox") as ListBox;

        #endregion

        #region - Constructors -

        static SearchableComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchableComboBox), new FrameworkPropertyMetadata(typeof(SearchableComboBox)));
        }

        public SearchableComboBox()
        {
            Loaded += OnLoaded;
        }

        #endregion

        #region - Private methods -

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            SearchTextBox.PreviewKeyDown += OnSearchTextBoxPreviewKeyDown;
            SearchTextBox.TextChanged += OnSearchTextBoxTextChanged;

            ItemsListBox.PreviewKeyDown += OnItemsListBoxPreviewKeyDown;
            ItemsListBox.PreviewMouseUp += OnItemsListBoxPreviewMouseUp;
            ItemsListBox.SelectionChanged += OnItemsListBoxSelectionChanged;
        }

        private void OnSearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            // If no custom search is used, filter current view
            if(SearchTextChanged == null)
            {
                CollectionViewSource.GetDefaultView(ItemsSource).Refresh();
            }
        }

        private void OnItemsListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            // If null is not allowed, and selected item is not in the list anymore,
            // select the first item in the list
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (ItemsListBox?.SelectedIndex == -1)
                {
                    SelectItemAt(0);
                }
            }), DispatcherPriority.Normal);
        }

        private void OnItemsListBoxPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is Thumb))
            {
                IsDropDownOpen = e.ChangedButton != MouseButton.Left;

                SelectedItem = ItemsListBox.SelectedItem;
            }
        }

        private void OnItemsListBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    // In case list box is focused by mouse or TAB and user keeps scrolling, when list box reaches the end,
                    // ComboBox items start scrolling, behaving likethere is another, detached list to scroll.
                    // To prevent this behavior, in case list box reached the end, we will stop even execution.
                    e.Handled = ItemsListBox.SelectedIndex == ItemsListBox.Items.Count - 1;

                    // Forcibly select item so we have a constant behaviour when scrolling manually while SearchTextBox is focused 
                    if (!AllowNullSelection && SelectedItem != ItemsListBox.SelectedItem)
                    {
                        SelectedItem = ItemsListBox.SelectedItem;
                    }
                    break;
                case Key.Up:
                    // Same as above for Key.Down
                    e.Handled = ItemsListBox.SelectedIndex == 0;
                    
                    // Forcibly select item so we have a constant behaviour when scrolling manually while SearchTextBox is focused 
                    if (!AllowNullSelection && SelectedItem != ItemsListBox.SelectedItem)
                    {
                        SelectedItem = ItemsListBox.SelectedItem;
                    }
                    break;
                case Key.Enter:
                    HandleEnterKey(e);
                    break;
                case Key.Escape:
                    HandleEscKey();
                    break;
            }
        }

        private void OnSearchTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    if (ItemsListBox.HasItems)
                    {
                        if (ItemsListBox.SelectedIndex < ItemsListBox.Items.Count - 1)
                        {
                            SelectItemAt(ItemsListBox.SelectedIndex + 1);
                        }

                        e.Handled = true;
                    }
                    break;
                case Key.Up:
                    if (ItemsListBox.HasItems)
                    {
                        if (ItemsListBox.SelectedIndex > 0)
                        {
                            SelectItemAt(ItemsListBox.SelectedIndex - 1);
                        }

                        e.Handled = true;
                    }
                    break;
                case Key.Enter:
                    HandleEnterKey(e);
                    break;
                case Key.Escape:
                    HandleEscKey();
                    break;
                default:
                    DispatchSearchEvent();
                    break;
            }
        }

        private void SelectItemAt(int index)
        {
            ItemsListBox.SelectedIndex = index;

            BringSelectedItemIntoView();

            if (!AllowNullSelection)
            {
                // Forcibly set selected item on change. This way, ComboBox.
                // Text will not be cleared in case selected item is not in the list, and no validators will be triggered, if any.
                // The idea is to provide a UX feedback on selected item, and there is still ESC key to returned to the last selection.
                SelectedItem = ItemsListBox.SelectedItem;
            }
        }

        private void HandleEnterKey(KeyEventArgs e)
        {
            if (IsDropDownOpen && ItemsListBox.SelectedItem != null)
            {
                IsDropDownOpen = false;

                SelectedItem = ItemsListBox.SelectedItem;

                e.Handled = true;
            }
        }

        private void HandleEscKey()
        {
            IsDropDownOpen = false;
            SelectedItem = cachedSelectedItem;
        }

        private void BringSelectedItemIntoView()
        {
            if (ItemsListBox.SelectedIndex >= 0)
            {
                ItemsListBox.ScrollIntoView(ItemsListBox.SelectedItem);
            }
        }

        private void DispatchSearchEvent()
        {
            if (SearchTextChanged != null)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    SearchTextChanged.Invoke(this, new SearchTextChangedEventArgs(SearchTextBox.Text));
                }), DispatcherPriority.Input);
            }
        }

        private bool IsMatch(object value)
        {
            if (value == null) return false;
            if (string.IsNullOrEmpty(SearchTextBox?.Text)) return true;

            return value.ToString().ToUpper().Contains(SearchTextBox.Text.ToUpper());
        }

        #endregion

        #region - Overrides -

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // Attach filter event only if custom search is not used
            if (newValue != null && SearchTextChanged == null)
            {
                var view = CollectionViewSource.GetDefaultView(newValue);
                view.Filter += IsMatch;
            }

            if (oldValue != null)
            {
                var view = CollectionViewSource.GetDefaultView(oldValue);
                if (view != null) view.Filter -= IsMatch;
            }

            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            cachedSelectedItem = SelectedItem;

            Dispatcher.BeginInvoke((Action)(() =>
            {
                SearchTextBox.Focus();
                Keyboard.Focus(SearchTextBox);

                // if something was focused and ESC pressed,
                // make sure selected item is set correctly
                ItemsListBox.SelectedItem = SelectedItem;

                // Make sure selected item is visible in the list
                BringSelectedItemIntoView();
            }), DispatcherPriority.Input);

            base.OnDropDownOpened(e);
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);

            if (ClearSearchOnClose)
            {
                SearchTextBox.Text = string.Empty;

                DispatchSearchEvent();

                CollectionViewSource.GetDefaultView(ItemsSource).Refresh();
            }

            if (SelectedItem == null && !AllowNullSelection)
            {
                SelectedItem = cachedSelectedItem;
            }
        }

        #endregion

        #region - Properties -

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public string NoItemsText
        {
            get { return (string)GetValue(NoItemsTextProperty); }
            set { SetValue(NoItemsTextProperty, value); }
        }

        public bool AllowNullSelection
        {
            get { return (bool)GetValue(AllowNullSelectionProperty); }
            set { SetValue(AllowNullSelectionProperty, value); }
        }

        public bool ClearSearchOnClose
        {
            get { return (bool)GetValue(ClearSearchOnCloseProperty); }
            set { SetValue(ClearSearchOnCloseProperty, value); }
        }

        #endregion
    }

    public class SearchTextChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes new instance of SearchTextChangedEventArgs with sepcified search text.
        /// </summary>
        /// <param name="value">Search text.</param>
        public SearchTextChangedEventArgs(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets search text.
        /// </summary>
        public string Value { get; private set; }
    }
}
