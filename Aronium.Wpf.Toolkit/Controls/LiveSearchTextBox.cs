using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_ListBox", Type = typeof(ListBox))]
    public class LiveSearchTextBox : Control
    {
        #region - Fields -

        private Popup popup;
        private TextBox textBox;
        private ListBox listBox;

        #endregion

        #region - Events -

        public event EventHandler<LiveSearchItemSelectedEventArgs> ItemSelected;

        #endregion

        #region - Dependecy properties -

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(LiveSearchTextBox));
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(LiveSearchTextBox));
        public static readonly DependencyProperty MaxPopupHeightProperty = DependencyProperty.Register("MaxPopupHeight", typeof(double), typeof(LiveSearchTextBox), new PropertyMetadata(Double.NaN));
        public static readonly DependencyProperty PopupWidthProperty = DependencyProperty.Register("PopupWidth", typeof(double), typeof(LiveSearchTextBox), new PropertyMetadata(Double.NaN));
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(LiveSearchTextBox));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
            typeof(string),
            typeof(LiveSearchTextBox),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ClearSearchOnSelectProperty = DependencyProperty.Register("ClearSearchOnSelect", typeof(bool), typeof(LiveSearchTextBox), new PropertyMetadata(true));
        public static readonly DependencyProperty IsLiveSearchEnabledProperty = DependencyProperty.Register("IsLiveSearchEnabled", typeof(bool), typeof(LiveSearchTextBox), new PropertyMetadata(true));

        #endregion

        #region - Events -

        /// <summary>
        /// Occurs when search text is changed.
        /// </summary>
        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent("TextChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(LiveSearchTextBox));

        /// <summary>
        /// Occurs when this serach text is changed.
        /// </summary>
        public event RoutedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        #endregion

        #region - Constructors -

        static LiveSearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LiveSearchTextBox), new FrameworkPropertyMetadata(typeof(LiveSearchTextBox)));
        }

        #endregion

        #region - Public methods -

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            textBox = Template.FindName("PART_TextBox", this) as TextBox;
            textBox.TextChanged += OnTextChanged;
            textBox.PreviewKeyDown += OnTextBoxPreviewKeyDown;

            popup = Template.FindName("PART_Popup", this) as Popup;
            popup.PreviewKeyDown += OnPopupPreviewKeyDown;

            listBox = Template.FindName("PART_ListBox", this) as ListBox;
            listBox.PreviewMouseUp += OnListBoxPreviewMouseUp;
        }

        #endregion

        #region - Private methods -

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (textBox != null && !textBox.IsFocused)
                FocusTextBox();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!textBox.IsFocused) return;

            if (textBox.Text != string.Empty && IsLiveSearchEnabled)
                ShowPopup();
            else if (popup.IsOpen)
                popup.IsOpen = false;
        }

        private void ShowPopup()
        {
            popup.IsOpen = listBox.HasItems;

            if (listBox.HasItems)
                listBox.SelectedIndex = 0;
        }

        private void OnPopupPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    HandleEnterKey(e);
                    break;
                case Key.Escape:
                    HandleEscKey(e);
                    break;
            }
        }

        private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    HandleEnterKey(e);
                    break;
                case Key.Escape:
                    HandleEscKey(e);
                    break;
                case Key.Tab:
                    HidePopup();
                    break;
                case Key.Down:
                    if (listBox.HasItems)
                    {
                        if (listBox.SelectedIndex + 1 <= listBox.Items.Count)
                        {
                            listBox.SelectedIndex++;
                            listBox.ScrollIntoView(listBox.SelectedItem);
                        }

                        e.Handled = true;
                    }
                    break;
                case Key.Up:
                    if (listBox.HasItems)
                    {
                        if (listBox.SelectedIndex > 0)
                        {
                            listBox.SelectedIndex--;
                            listBox.ScrollIntoView(listBox.SelectedItem);
                        }

                        e.Handled = true;
                    }
                    break;
            }
        }

        private void FocusSelectedListBoxItem()
        {
            var element = GetListBoxElementFromSelectedItem(listBox.SelectedItem);

            if (element != null)
            {
                element.Focus();
                Keyboard.Focus(element);
            }
        }

        private ListBoxItem GetListBoxElementFromSelectedItem(object item)
        {
            return listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
        }

        private void OnListBoxPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(listBox, e.OriginalSource as DependencyObject) as ListBoxItem;

            if (item != null)
            {
                SelectItem(item.DataContext);
            }
        }

        private void HandleEnterKey(KeyEventArgs e)
        {
            if (popup.IsOpen && listBox.SelectedItem != null)
            {
                SelectItem(listBox.SelectedItem);

                e.Handled = true;
            }
        }

        private void SelectItem(object item)
        {
            ItemSelected?.Invoke(this, new LiveSearchItemSelectedEventArgs(item));

            if (ClearSearchOnSelect)
                textBox.Clear();

            HidePopup();

            FocusTextBox();
        }

        private void HidePopup()
        {
            popup.IsOpen = false;
            listBox.SelectedItem = null;
        }

        private void HandleEscKey(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && popup.IsOpen)
            {
                HidePopup();
                FocusTextBox();

                e.Handled = true;
            }
        }

        private void FocusTextBox()
        {
            textBox.Focus();
            textBox.SelectAll();
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or sets search text.
        /// </summary>
        [Bindable(true, BindingDirection.TwoWay)]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets search text.
        /// </summary>
        [Bindable(true, BindingDirection.TwoWay)]
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// Gets or sets collection user for popup items.
        /// </summary>
        [Bindable(true)]
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets items template.
        /// </summary>
        [Bindable(true)]
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets max height for result list.
        /// </summary>
        [Bindable(true)]
        public double MaxPopupHeight
        {
            get { return (double)GetValue(MaxPopupHeightProperty); }
            set { SetValue(MaxPopupHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets popup width.
        /// </summary>
        [Bindable(true)]
        public double PopupWidth
        {
            get { return (double)GetValue(PopupWidthProperty); }
            set { SetValue(PopupWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether search text is cleared in item select.
        /// </summary>
        [Bindable(true, BindingDirection.TwoWay)]
        public bool ClearSearchOnSelect
        {
            get { return (bool)GetValue(ClearSearchOnSelectProperty); }
            set { SetValue(ClearSearchOnSelectProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether live search is enabled.
        /// </summary>
        [Bindable(true, BindingDirection.TwoWay)]
        public bool IsLiveSearchEnabled
        {
            get { return (bool)GetValue(IsLiveSearchEnabledProperty); }
            set { SetValue(IsLiveSearchEnabledProperty, value); }
        }

        #endregion
    }

    public class LiveSearchItemSelectedEventArgs : EventArgs
    {
        public LiveSearchItemSelectedEventArgs(object item)
        {
            Item = item;
        }

        /// <summary>
        /// Gets selected item.
        /// </summary>
        public object Item { get; private set; }
    }
}