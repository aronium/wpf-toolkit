using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_DropDownButton", Type = typeof(Button))]
    public class SplitButton : ContentControl
    {
        #region -  Dependency Properties -

        /// <summary>
        /// Identifies thie DropDown dependency property.
        /// </summary>
        public static readonly DependencyProperty DropDownProperty = DependencyProperty.Register("DropDown", typeof(ContextMenu), typeof(SplitButton));
        
        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SplitButton), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// Identifies the Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitButton));

        /// <summary>
        /// Identifies the CommandParameter property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(SplitButton));

        /// <summary>
        /// Identifies OrientationProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SplitButton));

        #endregion

        #region - Routed Events -

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SplitButton)); 

        #endregion

        private Button contentButton;
        private ToggleButton dropDownButton;

        #region - Constructors -

        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        /// <summary>
        /// Initializes new instance of SplitButton class.
        /// </summary>
        public SplitButton()
        {
            this.GotFocus += OnGotFocus;
        }

        #endregion

        #region - Events -

        /// <summary>
        /// Occurs when a SplitButton is clicked.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        #endregion

        #region - Private methods -

        /// <summary>
        /// Occurs when items source property has changed.
        /// </summary>
        /// <param name="d">Dependency object.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DropDownButton).DropDown = new ContextMenu() { ItemsSource = e.NewValue as IEnumerable };
            foreach (MenuItem item in (d as DropDownButton).DropDown.Items)
            {
                item.CommandTarget = d as IInputElement;
            }
        }

        /// <summary>
        /// Occurs when control is focused.
        /// </summary>
        /// <param name="sender">Focused control.</param>
        /// <param name="e">Event arguments.</param>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // If main button is not focused, meaning this was the initial control focus (e.g. keyboard navigation), focus main button automatically.
            if (contentButton != null && !contentButton.IsFocused && !dropDownButton.IsFocused)
            {
                contentButton.Focus();
            }
        }

        /// <summary>
        /// Occurs when button is clicked.
        /// </summary>
        /// <param name="sender">Main content button.</param>
        /// <param name="e">Event arguments.</param>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(ClickEvent, this));

            if (Command != null)
            {
                Command.Execute(this.CommandParameter);
            }
        }

        /// <summary>
        /// Occurs when drop down button is clicked.
        /// </summary>
        /// <param name="sender">Drop down button part.</param>
        /// <param name="e">Event arguments.</param>
        private void OnDropDownButtonClick(object sender, RoutedEventArgs e)
        {
            if (DropDown != null && DropDown.Items.Count > 0)
            {
                DropDown.PlacementTarget = this;
                DropDown.Placement = PlacementMode.Bottom;

                DropDown.IsOpen = true;
            }
        }

        #endregion

        #region - Overrides -
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            dropDownButton = this.Template.FindName("PART_DropDownButton", this) as ToggleButton;
            contentButton = this.Template.FindName("PART_Button", this) as Button;

            if (dropDownButton != null)
            {
                Binding binding = new Binding("DropDown.IsOpen");
                binding.Source = this;
                dropDownButton.SetBinding(ToggleButton.IsCheckedProperty, binding);

                dropDownButton.Click += OnDropDownButtonClick;
            }

            if (contentButton != null)
            {
                contentButton.Click += OnButtonClick;
            }
        }
        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or sets drop down menu.
        /// </summary>
        public ContextMenu DropDown
        {
            get
            {
                return (ContextMenu)GetValue(DropDownProperty);
            }
            set
            {
                SetValue(DropDownProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets command.
        /// </summary>
        [Bindable(true)]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets command property.
        /// </summary>
        [Bindable(true)]
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets drop down arrow orientation.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        #endregion
    }

}
