using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        DispatcherTimer timer;
        
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (this.ClickToClose)
                this.Close();
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

        private void Close()
        {
            if (timer != null)
            {
                timer.Tick -= OnTimerTick;
                timer.Stop();
                timer.IsEnabled = false;
            }

            this.Visibility = Visibility.Collapsed;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            EventHandler fadeOutAnimationCompletedHandler = null;
            EventHandler sizeAnimationCompletedHandler = null;

            var fadeOutAnimation = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            var sizeAnimation = new DoubleAnimation()
            {
                From = this.ActualHeight,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200),
                BeginTime = TimeSpan.FromMilliseconds(100)
            };

            fadeOutAnimationCompletedHandler = (s, ea) =>
            {
                fadeOutAnimation.Completed -= fadeOutAnimationCompletedHandler;
                this.BeginAnimation(HeightProperty, sizeAnimation);
            };

            sizeAnimationCompletedHandler = (s, ea) =>
            {
                sizeAnimation.Completed -= sizeAnimationCompletedHandler;

                this.Close();
            };

            fadeOutAnimation.Completed += fadeOutAnimationCompletedHandler;
            sizeAnimation.Completed += sizeAnimationCompletedHandler;

            this.BeginAnimation(OpacityProperty, fadeOutAnimation);
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
