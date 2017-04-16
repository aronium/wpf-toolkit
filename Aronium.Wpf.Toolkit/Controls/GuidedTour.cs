using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class GuidedTour : Canvas
    {
        #region - Fields -

        private const double MARGIN = 0;

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

        #endregion

        #region - Events -

        /// <summary>
        /// Identifies ClosedEvent routed event.
        /// </summary>
        public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GuidedTour));

        /// <summary>
        /// Identifies FinishedEvent routed event.
        /// </summary>
        public static readonly RoutedEvent FinishedEvent = EventManager.RegisterRoutedEvent("Finished", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GuidedTour));

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

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CurrentItem != null)
            {
                SetElementGuidePosition(CurrentItem);

                if (Animate)
                    CreateAnimation(CurrentItem);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnLoaded;

            ShowNextGuide();
        }

        private void ShowNextGuide(int index = 0)
        {
            if (Items != null && index < Items.Count())
            {
                currentIndex = index;

                if (Items.Any())
                {
                    var item = Items.ElementAt(index);

                    CurrentItem = item;

                    Children.Add(item);

                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        SetElementGuidePosition(item);

                        item.Show();

                        if (item.Target is Button)
                            ((Button)item.Target).Click += OnTargetButtonClick;
                        else
                            item.Target.PreviewMouseDown += OnElementMouseDown;

                        item.Target.SizeChanged += OnTargetSizeChanged;

                        var closeButton = item.Template.FindName("PART_ButtonClose", item) as Button;
                        if (closeButton != null)
                            closeButton.Click += OnCloseItemClick;

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
            IEasingFunction easing = null;// new BackEase() { EasingMode = EasingMode.EaseIn };
            int millis = 400;

            switch (item.Placement)
            {
                case GuidedTourItem.ItemPlacement.Left:
                case GuidedTourItem.ItemPlacement.Right:
                    to = item.Placement == GuidedTourItem.ItemPlacement.Right ? item.Position.X + 10 : item.Position.X - 10;
                    from = item.Position.X;
                    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)"));
                    break;
                case GuidedTourItem.ItemPlacement.Top:
                case GuidedTourItem.ItemPlacement.Bottom:
                    to = item.Placement == GuidedTourItem.ItemPlacement.Top ? item.Position.Y - 10 : item.Position.Y + 10;
                    from = item.Position.Y;
                    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)"));
                    break;
            }

            doubleAnimation.From = from;
            doubleAnimation.To = to;
            doubleAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, millis));
            doubleAnimation.BeginTime = TimeSpan.FromSeconds(0.3);
            doubleAnimation.AutoReverse = true;
            Storyboard.SetTarget(doubleAnimation, item);
            animateGuideStoryboard.Children.Add(doubleAnimation);
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnimation.EasingFunction = easing;
            animateGuideStoryboard.Begin();
        }

        private void RemoveAnimation()
        {
            if (animateGuideStoryboard == null) return;

            animateGuideStoryboard.Stop();
            animateGuideStoryboard.Remove();
            animateGuideStoryboard.Children.Clear();
            animateGuideStoryboard.Remove();
        }

        private void OnCloseItemClick(object sender, RoutedEventArgs e)
        {
            IsCanceled = true;

            animateGuideStoryboard.Stop();
            animateGuideStoryboard = null;

            RemoveGuideItem(CurrentItem.Target);

            RaiseEvent(new RoutedEventArgs(ClosedEvent, CurrentItem));
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

        private void OnTargetButtonClick(object sender, RoutedEventArgs e)
        {
            OnGuideItemTargetSelected(sender);
        }

        private void OnTargetSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CurrentItem != null)
                SetElementGuidePosition(CurrentItem);
        }

        private void OnElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnGuideItemTargetSelected(sender);
        }

        private void OnGuideItemTargetSelected(object sender)
        {
            RemoveGuideItem(sender);

            currentIndex++;

            ShowNextGuide(currentIndex);
        }

        private void RemoveGuideItem(object targetElement)
        {
            var guideItem = GetGuideItem(targetElement as FrameworkElement);

            guideItem.Visibility = Visibility.Hidden;

            var target = ((FrameworkElement)targetElement);

            if (target is Button)
                ((Button)target).Click -= OnTargetButtonClick;
            else
                target.PreviewMouseDown -= OnElementMouseDown;

            guideItem.MouseEnter -= OnItemMouseEnter;
            guideItem.MouseLeave -= OnItemMouseLeave;
            target.SizeChanged -= OnTargetSizeChanged;

            Children.Remove(guideItem);
        }

        private GuidedTourItem GetGuideItem(FrameworkElement target)
        {
            return Items.First(x => x.Target == target);
        }

        private void SetElementGuidePosition(GuidedTourItem item)
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

        #endregion

        #region - Properties -

        /// <summary>
        /// Occurs when active guide item is closed.
        /// </summary>
        public event RoutedEventHandler Closed
        {
            add { AddHandler(ClosedEvent, value); }
            remove { RemoveHandler(ClosedEvent, value); }
        }

        /// <summary>
        /// Occurs when guided tour is finished.
        /// </summary>
        public event RoutedEventHandler Finished
        {
            add { AddHandler(FinishedEvent, value); }
            remove { RemoveHandler(FinishedEvent, value); }
        }

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

            ShowNextGuide();
        }

        #endregion
    }
}
