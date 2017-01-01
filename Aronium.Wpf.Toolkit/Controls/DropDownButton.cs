using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class DropDownButton : ToggleButton
    {
        #region -  Dependency Properties -

        /// <summary>
        /// Idenfifies DropDownProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty DropDownProperty = DependencyProperty.Register("DropDown", typeof(ContextMenu), typeof(DropDownButton));

        /// <summary>
        /// Identifies OrientationProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(DropDownButton));

        /// <summary>
        /// Idenfifies ItemsSourceProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DropDownButton), new PropertyMetadata(null, OnItemsSourceChanged));

        #endregion

        #region - Constructors -

        /// <summary>
        /// Initializes new instance of DropDownButton class.
        /// </summary>
        public DropDownButton()
        {
            // Bind the ToogleButton.IsChecked property to the drop-down's IsOpen property 
            Binding binding = new Binding("DropDown.IsOpen");
            binding.Source = this;
            this.SetBinding(IsCheckedProperty, binding);
        }

        #endregion

        #region - Private methods -

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DropDownButton).DropDown = new ContextMenu() { ItemsSource = e.NewValue as IEnumerable };
            foreach (MenuItem item in (d as DropDownButton).DropDown.Items)
            {
                item.CommandTarget = d as IInputElement;
            }
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or sets button drop down.
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
        /// Gets or sets items source used for drop down items.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return GetValue(ItemsSourceProperty) as IEnumerable;
            }
            set
            {
                SetValue(ItemsSourceProperty, value);

                if (this.DropDown == null) 
                    this.DropDown = new ContextMenu();

                this.DropDown.ItemsSource = value;
            }
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

        #region - Overridden Methods -

        protected override void OnClick()
        {
            if (DropDown != null && DropDown.Items.Count > 0)
            {
                DropDown.PlacementTarget = this;
                DropDown.Placement = PlacementMode.Bottom;

                DropDown.IsOpen = true;
            }
        }

        #endregion
    }
}
