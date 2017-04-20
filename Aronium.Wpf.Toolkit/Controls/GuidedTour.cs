using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class GuidedTour : Canvas, IDisposable
    {
        #region - Fields -

        private const double MARGIN = 0;
        private const double ANIMATION_MOVEMENT = 10;
        private const int ANIMATION_DURATION = 400;

        private int currentIndex = 0;
        private GuidedTourItem _currentItem;
        private Storyboard animateGuideStoryboard = new Storyboard();

        #endregion

        #region - Dependency properties -

        /// <summary>
        /// Identifies ItemsProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IList<GuidedTourItem>), typeof(GuidedTour));

        /// <summary>
        /// Identifies AnimateProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimateProperty = DependencyProperty.Register("Animate", typeof(bool), typeof(GuidedTour), new PropertyMetadata(true));

        /// <summary>
        /// Identifies ContextProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty ContextProperty = DependencyProperty.Register("Context", typeof(string), typeof(GuidedTour));

        /// <summary>
        /// Identifies AllowDismissProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowDismissProperty = DependencyProperty.Register("AllowDismiss", typeof(bool), typeof(GuidedTour));

        /// <summary>
        /// Identifies DismissButtonTextProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty DismissButtonTextProperty = DependencyProperty.Register("DismissButtonText", typeof(string), typeof(GuidedTour), new PropertyMetadata("(don't show this again)"));

        #endregion

        #region - Events -

        /// <summary>
        /// Identifies BeginEvent routed event.
        /// </summary>
        public static readonly RoutedEvent BeginEvent = EventManager.RegisterRoutedEvent("Begin", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GuidedTour));

        /// <summary>
        /// Identifies ClosingEvent routed event.
        /// </summary>
        public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent("Closing", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GuidedTour));

        /// <summary>
        /// Identifies ClosedEvent routed event.
        /// </summary>
        public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GuidedTour));

        /// <summary>
        /// Identifies FinishedEvent routed event.
        /// </summary>
        public static readonly RoutedEvent FinishedEvent = EventManager.RegisterRoutedEvent("Finished", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GuidedTour));

        /// <summary>
        /// Identifies DismissedEvent routed event.
        /// </summary>
        public static readonly RoutedEvent DismissedEvent = EventManager.RegisterRoutedEvent("Dismissed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GuidedTour));

        #endregion

        #region - Constructors -

        /// <summary>
        /// Guide class static constructor.
        /// </summary>
        static GuidedTour()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GuidedTour), new FrameworkPropertyMetadata(typeof(GuidedTour)));
        }

        /// <summary>
        /// Initializes new instance of Guide class.
        /// </summary>
        public GuidedTour()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Loaded += OnLoaded;
                SizeChanged += OnSizeChanged;
            }
        }

        #endregion

        #region - Private methods -

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            ShowGuideItem();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CurrentItem != null)
            {
                SetGuideItemPosition(CurrentItem);

                if (Animate)
                    CreateAnimation(CurrentItem);
            }
        }

        private void OnTargetSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CurrentItem != null)
                SetGuideItemPosition(CurrentItem);
        }

        private void ShowGuideItem(int index = 0)
        {
            if (Items != null && index < Items.Count())
            {
                currentIndex = index;

                if (Items.Any())
                {
                    // On first item, raise begin event notifying that guided tour has started
                    if (index == 0)
                    {
                        RaiseEvent(new RoutedEventArgs(BeginEvent));
                    }

                    var item = Items.ElementAt(index);

                    CurrentItem = item;

                    Children.Add(item);

                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        SetGuideItemPosition(item);

                        item.Show();

                        AttachActionEvents(item.Target);
                        if (item.AlternateTargets != null)
                        {
                            foreach (var el in item.AlternateTargets)
                                AttachActionEvents(el);
                        }

                        item.Target.SizeChanged += OnTargetSizeChanged;
                        item.Target.IsVisibleChanged += OnTargetIsVisibleChanged;

                        var closeButton = item.Template.FindName("PART_ButtonClose", item) as Button;
                        if (closeButton != null)
                        {
                            // Remove any previously set listeners
                            closeButton.Click -= OnCloseButtonClick;

                            // Add close button click listener
                            closeButton.Click += OnCloseButtonClick;
                        }

                        if (AllowDismiss)
                        {
                            var tbDismiss = item.Template.FindName("PART_DismissButton", item) as TextBlock;

                            if (tbDismiss != null)
                            {
                                var link = tbDismiss.Inlines.FirstInline as Hyperlink;

                                if (link != null)
                                    link.Click += OnDismissClick;
                            }
                        }

                        if (Animate)
                        {
                            CreateAnimation(item);

                            item.MouseEnter += OnItemMouseEnter;
                            item.MouseLeave += OnItemMouseLeave;
                        }

                    }), DispatcherPriority.ContextIdle);
                }
            }
            else
            {
                CurrentItem = null;

                RemoveAnimation();

                RaiseEvent(new RoutedEventArgs(FinishedEvent));
            }
        }

        private void OnDismissClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(DismissedEvent));

            CloseGuidedTour();
        }

        private void AttachActionEvents(FrameworkElement target)
        {
            // Assign item completition eveents
            if (target is Button)
                ((Button)target).Click += OnGuideStepComplete;
            else if (target is TextBoxBase)
            {
                target.KeyDown += OnGuideStepComplete;
                ((TextBoxBase)target).Focus();
            }
            else
                target.PreviewMouseDown += OnElementMouseDown;
        }

        private void OnTargetIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Make sure that target guide item is hidden if target is not visible on the screen
            // Once target becomes visible again, show guide item again
            if ((bool)e.NewValue == false)
                GetGuideItem(sender as FrameworkElement).Hide();
            else
                GetGuideItem(sender as FrameworkElement).Show();
        }

        private void SetGuideItemPosition(GuidedTourItem item)
        {
            if (!IsLoaded) return;

            var targetPoint = item.Target.PointToScreen(new Point(0, 0));
            var thisPoint = this.PointToScreen(new Point(0, 0));

            switch (item.Placement)
            {
                case GuidedTourItem.ItemPlacement.Left:
                    item.Position = new Point((targetPoint.X - thisPoint.X) - item.ActualWidth - MARGIN, targetPoint.Y - thisPoint.Y + ((item.Target.ActualHeight / 2) - (item.ActualHeight / 2)));
                    break;
                case GuidedTourItem.ItemPlacement.Right:
                    item.Position = new Point((targetPoint.X - thisPoint.X) + item.Target.ActualWidth + MARGIN, targetPoint.Y - thisPoint.Y + ((item.Target.ActualHeight / 2) - (item.ActualHeight / 2)));
                    break;
                case GuidedTourItem.ItemPlacement.Bottom:
                    item.Position = new Point((targetPoint.X - thisPoint.X) + ((item.Target.ActualWidth / 2) - (item.ActualWidth / 2)),
                       targetPoint.Y - thisPoint.Y + (item.Target.ActualHeight + MARGIN));
                    break;
                case GuidedTourItem.ItemPlacement.Top:
                    item.Position = new Point((targetPoint.X - thisPoint.X) + ((item.Target.ActualWidth / 2) - (item.ActualWidth / 2)),
                       targetPoint.Y - thisPoint.Y - (item.ActualHeight + MARGIN));
                    break;
                default:
                    item.Position = new Point((targetPoint.X - thisPoint.X) + ((item.Target.ActualWidth / 2) - (item.ActualWidth / 2)), targetPoint.Y - thisPoint.Y + ((item.Target.ActualHeight / 2) - (item.ActualHeight / 2)));
                    break;
            }

            SetLeft(item, item.Position.X);
            SetTop(item, item.Position.Y);
        }

        private void CreateAnimation(GuidedTourItem item)
        {
            if (animateGuideStoryboard != null)
            {
                RemoveAnimation();
            }
            else
                animateGuideStoryboard = new Storyboard();

            double from = 0, to = 0;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            IEasingFunction easing = null; // new CircleEase() { EasingMode = EasingMode.EaseOut };

            switch (item.Placement)
            {
                case GuidedTourItem.ItemPlacement.Left:
                case GuidedTourItem.ItemPlacement.Right:
                    to = item.Placement == GuidedTourItem.ItemPlacement.Right ? item.Position.X + ANIMATION_MOVEMENT : item.Position.X - ANIMATION_MOVEMENT;
                    from = item.Position.X;
                    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)"));
                    break;
                case GuidedTourItem.ItemPlacement.Top:
                case GuidedTourItem.ItemPlacement.Bottom:
                    to = item.Placement == GuidedTourItem.ItemPlacement.Top ? item.Position.Y - ANIMATION_MOVEMENT : item.Position.Y + ANIMATION_MOVEMENT;
                    from = item.Position.Y;
                    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)"));
                    break;
            }

            doubleAnimation.From = from;
            doubleAnimation.To = to;
            doubleAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, ANIMATION_DURATION));
            doubleAnimation.BeginTime = TimeSpan.FromSeconds(0.3);
            doubleAnimation.AutoReverse = true;
            Storyboard.SetTarget(doubleAnimation, item);
            animateGuideStoryboard.Children.Add(doubleAnimation);
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnimation.EasingFunction = easing;
            animateGuideStoryboard.Begin();
        }

        private void RemoveGuideItem(object targetElement)
        {
            var item = GetGuideItem(targetElement as FrameworkElement);

            item.Visibility = Visibility.Hidden;

            var target = ((FrameworkElement)targetElement);

            DetachActionEvents(target);

            if (item.AlternateTargets != null)
            {
                foreach (var el in item.AlternateTargets)
                    DetachActionEvents(el);
            }

            target.SizeChanged -= OnTargetSizeChanged;

            item.MouseEnter -= OnItemMouseEnter;
            item.MouseLeave -= OnItemMouseLeave;

            #region " Remove buttons listeners "

            var closeButton = item.Template.FindName("PART_ButtonClose", item) as Button;
            if (closeButton != null)
            {
                closeButton.Click -= OnCloseButtonClick;
            }

            if (AllowDismiss)
            {
                var tbDismiss = item.Template.FindName("PART_DismissButton", item) as TextBlock;

                if (tbDismiss != null)
                {
                    var link = tbDismiss.Inlines.FirstInline as Hyperlink;

                    if (link != null)
                        link.Click -= OnDismissClick;
                }
            }

            #endregion

            Children.Remove(item);
        }

        private void DetachActionEvents(FrameworkElement target)
        {
            if (target is Button)
                ((Button)target).Click -= OnGuideStepComplete;
            else if (target is TextBoxBase)
                target.KeyDown -= OnGuideStepComplete;
            else
                target.PreviewMouseDown -= OnElementMouseDown;
        }

        private void RemoveAnimation()
        {
            if (animateGuideStoryboard == null) return;

            animateGuideStoryboard.Stop();
            animateGuideStoryboard.Remove();
            animateGuideStoryboard.Children.Clear();
            animateGuideStoryboard.Remove();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            // Create new closing event args instance
            var ea = new ClosingGuidedTourEventArgs(ClosingEvent);

            // Raise closing event
            RaiseEvent(ea);

            // If closing was canceled, return
            if (ea.Cancel)
                return;

            CloseGuidedTour();

            RaiseEvent(new RoutedEventArgs(ClosedEvent, CurrentItem));
        }

        private void CloseGuidedTour()
        {
            IsCanceled = true;

            animateGuideStoryboard.Stop();
            animateGuideStoryboard = null;

            RemoveGuideItem(CurrentItem.Target);
        }

        private void OnGuideStepComplete(object sender, RoutedEventArgs e)
        {
            OnGuideItemTargetSelected(sender);
        }

        private void OnElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnGuideItemTargetSelected(sender);
        }

        private void OnGuideItemTargetSelected(object sender)
        {
            RemoveGuideItem(sender);

            currentIndex++;

            ShowGuideItem(currentIndex);
        }

        private void OnItemMouseLeave(object sender, MouseEventArgs e)
        {
            if (animateGuideStoryboard != null)
                animateGuideStoryboard.Resume();
        }

        private void OnItemMouseEnter(object sender, MouseEventArgs e)
        {
            animateGuideStoryboard.Pause();
        }

        private GuidedTourItem GetGuideItem(FrameworkElement target)
        {
            var item = Items.FirstOrDefault(x => x.Target == target);

            if (item == null)
                return CurrentItem;
            else
                return item;
        }

        #endregion

        #region " Events "

        /// <summary>
        /// Occurs when guided tour has started.
        /// </summary>
        public event RoutedEventHandler Begin
        {
            add { AddHandler(BeginEvent, value); }
            remove { RemoveHandler(BeginEvent, value); }
        }

        /// <summary>
        /// Occurs before guided tour is closed.
        /// </summary>
        public event RoutedEventHandler Closing
        {
            add { AddHandler(ClosingEvent, value); }
            remove { RemoveHandler(ClosingEvent, value); }
        }

        /// <summary>
        /// Occurs when guided tour is closed.
        /// </summary>
        public event RoutedEventHandler Closed
        {
            add { AddHandler(ClosedEvent, value); }
            remove { RemoveHandler(ClosedEvent, value); }
        }

        /// <summary>
        /// Occurs when guided tour is dismissed.
        /// </summary>
        public event RoutedEventHandler Dismissed
        {
            add { AddHandler(DismissedEvent, value); }
            remove { RemoveHandler(DismissedEvent, value); }
        }

        /// <summary>
        /// Occurs when guided tour is finished.
        /// </summary>
        public event RoutedEventHandler Finished
        {
            add { AddHandler(FinishedEvent, value); }
            remove { RemoveHandler(FinishedEvent, value); }
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or sets a value indicating whether guided tour was canceled.
        /// </summary>
        public bool IsCanceled { get; set; }

        /// <summary>
        /// Gets or sets guide items.
        /// </summary>
        public IList<GuidedTourItem> Items
        {
            get { return (IList<GuidedTourItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether guide items should be animated.
        /// </summary>
        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(AnimateProperty, value); }
        }

        /// <summary>
        /// Gets or sets distinctive context used for this guided tour.
        /// </summary>
        public string Context
        {
            get { return (string)GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether dismiss is allowed and dismiss button displayed.
        /// </summary>
        public bool AllowDismiss
        {
            get { return (bool)GetValue(AllowDismissProperty); }
            set { SetValue(AllowDismissProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether dismiss is allowed and dismiss button displayed.
        /// </summary>
        public string DismissButtonText
        {
            get { return (string)GetValue(DismissButtonTextProperty); }
            set { SetValue(DismissButtonTextProperty, value); }
        }

        /// <summary>
        /// Gets current guide item.
        /// </summary>
        public GuidedTourItem CurrentItem
        {
            get
            {
                return _currentItem;
            }
            private set
            {
                _currentItem = value;
            }
        }

        #endregion

        #region - Public methods -

        /// <summary>
        /// Restarts guided tour.
        /// </summary>
        public void Reset()
        {
            IsCanceled = false;

            if (CurrentItem != null)
                RemoveGuideItem(CurrentItem.Target);

            ShowGuideItem();
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            CurrentItem = null;
            RemoveAnimation();
        }

        #endregion
    }

    public class ClosingGuidedTourEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes new instance of ClosingGuidedTourEventArgs with specified routed event.
        /// </summary>
        /// <param name="routedEvent">Routed event.</param>
        public ClosingGuidedTourEventArgs(RoutedEvent routedEvent) : base(routedEvent) { }

        /// <summary>
        /// Gets or sets a value indicating whether closing action was canceled.
        /// </summary>
        public bool Cancel { get; set; }
    }
}
