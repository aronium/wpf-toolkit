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
        public static DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(int), typeof(NotificationsControl), new PropertyMetadata(10));
        public static DependencyProperty ClickToCloseProperty = DependencyProperty.Register("ClickToClose", typeof(bool), typeof(NotificationsControl));

        public static readonly RoutedEvent ItemClosedEvent = EventManager.RegisterRoutedEvent("ItemClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotificationsControl));
        
        /// <summary>
        /// Occurs when item is closed.
        /// </summary>
        public event RoutedEventHandler ItemClosed
        {
            add { AddHandler(ItemClosedEvent, value); }
            remove { RemoveHandler(ItemClosedEvent, value); }
        }

        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public bool ClickToClose
        {
            get { return (bool)GetValue(ClickToCloseProperty); }
            set { SetValue(ClickToCloseProperty, value); }
        }

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

        void OnNotificationItemIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!((bool)e.NewValue))
            {
                this.RaiseEvent(new RoutedEventArgs(ItemClosedEvent, sender));
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
    }
}
