using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    public class NotificationItem : Control
    {
        public static DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(NotificationItem));
        public static DependencyProperty ClickToCloseProperty = DependencyProperty.Register("ClickToClose", typeof(bool), typeof(NotificationItem));
        public static DependencyProperty ShowCloseProperty = DependencyProperty.Register("ShowClose", typeof(bool), typeof(NotificationItem), new PropertyMetadata(true));
        public static DependencyProperty SlideInProperty = DependencyProperty.Register("SlideIn", typeof(bool), typeof(NotificationItem), new PropertyMetadata(true));

        private const double MARGIN = 5;

        DispatcherTimer timer;

        public NotificationItem()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            if (SlideIn)
                RunSlideInAnimation();
            else
                RunFadeInAnimation();
        }

        private void RunSlideInAnimation()
        {
            // If width is not explicitly set before animation begins, control will stretch to fit content height
            this.Width = this.ActualWidth;

            this.BeginAnimation(MarginProperty, new ThicknessAnimation()
            {
                From = new Thickness(MARGIN, MARGIN, -(ActualWidth + 10), MARGIN),
                To = new Thickness(MARGIN, MARGIN, 0, MARGIN),
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            });
        }

        private void RunFadeInAnimation()
        {
            this.BeginAnimation(OpacityProperty, new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(200)
            });
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (this.ClickToClose)
                this.AnimateOut();
        }

        public object Content
        {
            get
            {
                return GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public bool ClickToClose
        {
            get { return (bool)GetValue(ClickToCloseProperty); }
            set { SetValue(ClickToCloseProperty, value); }
        }

        public bool ShowClose
        {
            get { return (bool)GetValue(ShowCloseProperty); }
            set { SetValue(ShowCloseProperty, value); }
        }

        public bool SlideIn
        {
            get { return (bool)GetValue(SlideInProperty); }
            set { SetValue(SlideInProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var closeBtn = this.Template.FindName("PART_CloseButton", this) as Button;

            if (closeBtn != null)
            {
                closeBtn.Click += (sender, e) =>
                {
                    this.Close();
                };
            }
        }

        internal void Close()
        {
            if (timer != null)
            {
                timer.Tick -= OnTimerTick;
                timer.Stop();
                timer.IsEnabled = false;
            }

            Visibility = Visibility.Collapsed;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            AnimateOut();
        }

        private void AnimateOut()
        {
            EventHandler outAnimationCompletedHandler = null;
            EventHandler sizeAnimationCompletedHandler = null;

            var sizeAnimation = new DoubleAnimation()
            {
                From = this.ActualHeight,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200),
                BeginTime = TimeSpan.FromMilliseconds(200)
            };

            sizeAnimationCompletedHandler = (s, ea) =>
            {
                sizeAnimation.Completed -= sizeAnimationCompletedHandler;

                this.Close();
            };

            sizeAnimation.Completed += sizeAnimationCompletedHandler;

            if (SlideIn)
            {
                var slideOutAnimation = new ThicknessAnimation()
                {
                    To = new Thickness((ActualWidth + 10), MARGIN, 0, MARGIN),
                    Duration = TimeSpan.FromMilliseconds(300)
                };

                outAnimationCompletedHandler = (s, ea) =>
                {
                    slideOutAnimation.Completed -= outAnimationCompletedHandler;

                    this.BeginAnimation(HeightProperty, sizeAnimation);
                };

                slideOutAnimation.Completed += outAnimationCompletedHandler;

                this.BeginAnimation(MarginProperty, slideOutAnimation);
            }
            else
            {
                var fadeOutAnimation = new DoubleAnimation()
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(200)
                };

                outAnimationCompletedHandler = (s, ea) =>
                {
                    fadeOutAnimation.Completed -= outAnimationCompletedHandler;

                    this.BeginAnimation(HeightProperty, sizeAnimation);
                };

                fadeOutAnimation.Completed += outAnimationCompletedHandler;

                this.BeginAnimation(OpacityProperty, fadeOutAnimation);
            }
        }

        internal void RunAutoHideTimer(int duration)
        {
            timer = new DispatcherTimer();
            timer.Tick += OnTimerTick;
            timer.Interval = TimeSpan.FromSeconds(duration);
            timer.Start();
        }
    }
}
