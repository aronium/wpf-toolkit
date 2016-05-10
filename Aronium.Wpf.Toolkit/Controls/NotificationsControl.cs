using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class NotificationsControl : ItemsControl
    {
        private bool isApplicationClosing;

        #region - Routed events and dependency properties -

        public static DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(int), typeof(NotificationsControl), new PropertyMetadata(10));
        public static DependencyProperty ClickToCloseProperty = DependencyProperty.Register("ClickToClose", typeof(bool), typeof(NotificationsControl));
        public static DependencyProperty ShowCloseProperty = DependencyProperty.Register("ShowClose", typeof(bool), typeof(NotificationsControl), new PropertyMetadata(true));

        public static readonly RoutedEvent ItemClosedEvent = EventManager.RegisterRoutedEvent("ItemClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotificationsControl));
        public static DependencyProperty SlideInProperty = DependencyProperty.Register("SlideIn", typeof(bool), typeof(NotificationsControl), new PropertyMetadata(true));

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
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Focusable = false;

                Application.Current.MainWindow.Closing += OnApplicationMainWindowClosing;
            }
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

        /// <summary>
        /// Gets or sets a value indicating whether close button is visible on all notification items.
        /// </summary>
        public bool ShowClose
        {
            get { return (bool)GetValue(ShowCloseProperty); }
            set { SetValue(ShowCloseProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether notification items should ease in instead of fade animation.
        /// </summary>
        public bool SlideIn
        {
            get { return (bool)GetValue(SlideInProperty); }
            set { SetValue(SlideInProperty, value); }
        }

        #endregion

        #region - Overrides -

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                NotificationItem notificationItem = element as NotificationItem;

                if (!Object.ReferenceEquals(element, item) && notificationItem != null)
                {
                    notificationItem.Content = item;
                    if (Duration > 0)
                        notificationItem.RunAutoHideTimer(Duration);

                    notificationItem.IsVisibleChanged += OnNotificationItemIsVisibleChanged;
                }
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
            if (!isApplicationClosing && !((bool)e.NewValue))
            {
                this.RaiseEvent(new RoutedEventArgs(ItemClosedEvent, sender));
            }
        }

        private void OnApplicationMainWindowClosing(object sender, CancelEventArgs e)
        {
            /***********************************************************************************************
             * 
             * IMPORTANT!
             * 
             * Issue occured if notifications are visible on application close.
             * If routed event was fired during application shutdown, exception is thrown.
             * 
             * On main window application close, close items and stop running timers, if any.
             * 
             ***********************************************************************************************/

            isApplicationClosing = !e.Cancel;

            if (isApplicationClosing)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(this); i++)
                {
                    var container = ItemContainerGenerator.ContainerFromIndex(i) as NotificationItem;

                    if (container != null)
                    {
                        container.Close();
                    }
                }
            }
        }

        #endregion
    }
}
