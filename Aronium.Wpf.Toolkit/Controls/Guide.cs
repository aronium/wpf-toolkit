using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Aronium.Wpf.Toolkit.Controls
{
    public class Guide : Canvas
    {
        private const double MARGIN = 12;

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable<GuideItem>), typeof(Guide), new PropertyMetadata(new PropertyChangedCallback(OnItemsChanged)));

        private int currentIndex = 0;
        private GuideItem currentItem;
        Storyboard animateGuideStoryboard = new Storyboard();

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Guide @this = (Guide)d;
        }

        static Guide()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Guide), new FrameworkPropertyMetadata(typeof(Guide)));
        }

        public Guide()
        {
            this.Loaded += OnLoaded;
            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (currentItem != null)
            {
                SetElementGuidePosition(currentItem);

                Animate(currentItem);
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

                    currentItem = item;

                    this.Children.Add(item);

                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        SetElementGuidePosition(item);

                        item.Show();

                        item.Target.PreviewMouseDown += OnElementMouseDown;

                        Animate(item);

                    }), DispatcherPriority.ContextIdle);
                }
            }
            else
            {
                currentItem = null;
            }
        }

        private void Animate(GuideItem item)
        {
            if (animateGuideStoryboard != null)
            {
                animateGuideStoryboard.Stop();
                animateGuideStoryboard.Remove();
                animateGuideStoryboard.Children.Clear();
            }
            else
                animateGuideStoryboard = new Storyboard();

            double from = 0, to = 0;
            DoubleAnimation doubleAnimation;

            switch (item.Placement)
            {
                case PlacementMode.Left:
                case PlacementMode.Right:
                    from = item.Placement == PlacementMode.Right ? item.Position.X + 10 : item.Position.X - 10;
                    to = item.Position.X;

                    doubleAnimation = new DoubleAnimation(from, to, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                    doubleAnimation.AutoReverse = true;
                    Storyboard.SetTarget(doubleAnimation, item);
                    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)"));
                    animateGuideStoryboard.Children.Add(doubleAnimation);
                    doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                    animateGuideStoryboard.Begin();
                    break;
                case PlacementMode.Top:
                case PlacementMode.Bottom:
                    from = item.Placement == PlacementMode.Top ? item.Position.Y - 10 : item.Position.Y + 10;
                    to = item.Position.Y;

                    doubleAnimation = new DoubleAnimation(from, to, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                    doubleAnimation.AutoReverse = true;
                    Storyboard.SetTarget(doubleAnimation, item);
                    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)"));
                    animateGuideStoryboard.Children.Add(doubleAnimation);
                    doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                    animateGuideStoryboard.Begin();
                    break;
            }
        }

        private void OnElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            var currentGuideItem = GetGuideItem(sender as FrameworkElement);

            currentIndex++;

            ((FrameworkElement)sender).PreviewMouseDown -= OnElementMouseDown;

            this.Children.Remove(currentGuideItem);

            ShowNextGuide(currentIndex);
        }

        private GuideItem GetGuideItem(FrameworkElement target)
        {
            return Items.First(x => x.Target == target);
        }

        private void SetElementGuidePosition(GuideItem item)
        {
            var targetPoint = item.Target.PointToScreen(new Point(0, 0));
            var thisPoint = this.PointToScreen(new Point(0, 0));

            switch (item.Placement)
            {
                case PlacementMode.Left:
                    item.Position = new Point((targetPoint.X - thisPoint.X) - item.ActualWidth - MARGIN, targetPoint.Y - thisPoint.Y + ((item.Target.ActualHeight / 2) - (item.ActualHeight / 2)));
                    break;
                case PlacementMode.Right:
                    item.Position = new Point((targetPoint.X - thisPoint.X) + item.Target.ActualWidth + MARGIN, targetPoint.Y - thisPoint.Y + ((item.Target.ActualHeight / 2) - (item.ActualHeight / 2)));
                    break;
                case PlacementMode.Bottom:
                    item.Position = new Point((targetPoint.X - thisPoint.X) + ((item.Target.ActualWidth / 2) - (item.ActualWidth / 2)),
                       targetPoint.Y - thisPoint.Y + (item.Target.ActualHeight + MARGIN));
                    break;
                case PlacementMode.Top:
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

        public IEnumerable<GuideItem> Items
        {
            get { return (IEnumerable<GuideItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
    }
}
