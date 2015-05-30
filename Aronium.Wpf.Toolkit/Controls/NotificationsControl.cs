using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class NotificationsControl : ItemsControl
    {
        #region - Routed events and dependency properties -

        public static DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(int), typeof(NotificationsControl), new PropertyMetadata(10));
        public static DependencyProperty ClickToCloseProperty = DependencyProperty.Register("ClickToClose", typeof(bool), typeof(NotificationsControl));

        public static readonly RoutedEvent ItemClosedEvent = EventManager.RegisterRoutedEvent("ItemClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotificationsControl));

        #endregion

        #region - Events -

        /// <summary>
        /// Occurs when item is closed.
        /// </summary>
        public event RoutedEventHandler ItemClosed
        {
            add { AddHandler(ItemClosedEvent, value); }
            remove { RemoveHandler(ItemClosedEvent, value); }
        } 

        #endregion

        #region - Constructor -

        /// <summary>
        /// Initializes new instance of NotificationsControl class.
        /// </summary>
        public NotificationsControl()
        {
            this.Focusable = false;
        } 

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or sets duration indicating number of seconds before notification is dismissed.
        /// </summary>
        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        /// <summary>
        /// Indicates whether notification items should be closed on click.
        /// </summary>
        public bool ClickToClose
        {
            get { return (bool)GetValue(ClickToCloseProperty); }
            set { SetValue(ClickToCloseProperty, value); }
        } 

        #endregion

        #region - Overrides -

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            NotificationItem notificationItem = element as NotificationItem;

            if (!Object.ReferenceEquals(element, item) && notificationItem != null)
            {
                notificationItem.Content = item;
                if (Duration > 0)
                    notificationItem.RunAutoHideTimer(Duration);

                notificationItem.IsVisibleChanged += OnNotificationItemIsVisibleChanged;
            }
        }

        /// <summary>
        /// Overriden GetContainerForItemOverride method.
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new NotificationItem();
        }

        /// <summary>
        /// Overriden IsItemItsOwnContainerOverride method.
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is NotificationItem;
        } 

        #endregion

        #region - Private methods -

        private void OnNotificationItemIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!((bool)e.NewValue))
            {
                this.RaiseEvent(new RoutedEventArgs(ItemClosedEvent, sender));
            }
        } 

        #endregion
    }
}
