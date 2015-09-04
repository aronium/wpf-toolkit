using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_Backdrop", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ContentSite", Type = typeof(Border))]
    [TemplatePart(Name = "PART_CollapseButton", Type = typeof(Button))]
    [ContentProperty("Content")]
    public class Flyout : Control
    {
        #region - Dependency properties -

        /// <summary>
        /// Identifies Content property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(Flyout), new UIPropertyMetadata());

        public static readonly DependencyProperty FlyoutwidthProperty = DependencyProperty.Register("Flyoutwidth", typeof(double), typeof(Flyout), new UIPropertyMetadata(300.0));

        public static readonly DependencyProperty BackdropProperty = DependencyProperty.Register("Backdrop", typeof(bool), typeof(Flyout), new UIPropertyMetadata(true));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Flyout));

        #endregion

        #region - Routed events -

        /// <summary>
        /// Identifies ExpandedEvent routed event.
        /// </summary>
        public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent("Expanded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Flyout));

        /// <summary>
        /// Identifies CollapsedEvent routed event.
        /// </summary>
        public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent("Collapsed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Flyout));

        /// <summary>
        /// Occurs when this instance is expanded.
        /// </summary>
        public event RoutedEventHandler Expanded
        {
            add { AddHandler(ExpandedEvent, value); }
            remove { RemoveHandler(ExpandedEvent, value); }
        }

        /// <summary>
        /// Occurs when this instance is collapsed.
        /// </summary>
        public event RoutedEventHandler Collapsed
        {
            add { AddHandler(CollapsedEvent, value); }
            remove { RemoveHandler(CollapsedEvent, value); }
        }

        #endregion

        #region - Fields -

        private ThicknessAnimation outAnimation;
        private ThicknessAnimation inAnimation;
        private Border contentSite; 

        #endregion

        #region - Constructors -
        static Flyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
        }

        /// <summary>
        /// Initialies new instance of Flyout class.
        /// </summary>
        public Flyout()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.KeyDown += OnKeyDown;

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    CreateAnimationObjects();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        } 
        #endregion

        #region - Private methods -
        
        private void CreateAnimationObjects()
        {
            inAnimation = new ThicknessAnimation()
            {
                From = new Thickness(0, 0, -Flyoutwidth, 0),
                To = new Thickness(0),
                Duration = TimeSpan.FromMilliseconds(200)
            };

            inAnimation.Completed += OnSlideIn;

            inAnimation.Freeze();

            outAnimation = new ThicknessAnimation()
            {
                From = new Thickness(0),
                To = new Thickness(0, 0, -Flyoutwidth, 0),
                Duration = TimeSpan.FromMilliseconds(200)
            };

            outAnimation.Completed += OnCollapsed;

            outAnimation.Freeze();
        }

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape && this.IsVisible)
            {
                Collapse();
            }
        }

        private void OnHideFlyout(object sender, RoutedEventArgs e)
        {
            Collapse();
        }

        private void OnCollapsed(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;

            RaiseEvent(new RoutedEventArgs(CollapsedEvent));
        }

        private void OnSlideIn(object sender, EventArgs e)
        {
            this.Focus();

            RaiseEvent(new RoutedEventArgs(ExpandedEvent));
        } 

        #endregion

        #region - Public methods -

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var backdrop = this.Template.FindName("PART_Backdrop", this) as Border;
            backdrop.MouseUp += OnHideFlyout;

            var collapseButton = this.Template.FindName("PART_CollapseButton", this) as Button;
            collapseButton.Click += OnHideFlyout;

            contentSite = this.Template.FindName("PART_ContentSite", this) as Border;

            // Template is applied once controls becomes visible.
            // At this point, it is collapsed, if visible for the first time, re-run slide-in animation
            if (this.IsVisible)
                Show();
        }

        public void Show()
        {
            this.Visibility = System.Windows.Visibility.Visible;

            if (contentSite != null)
            {
                contentSite.BeginAnimation(MarginProperty, inAnimation);
            }
        }

        public void Collapse()
        {
            contentSite.BeginAnimation(MarginProperty, outAnimation);
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or sets flyout content.
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets flyout width.
        /// </summary>
        public double Flyoutwidth
        {
            get { return (double)GetValue(FlyoutwidthProperty); }
            set { SetValue(FlyoutwidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether backdrop is visible.
        /// </summary>
        public bool Backdrop
        {
            get { return (bool)GetValue(BackdropProperty); }
            set { SetValue(BackdropProperty, value); }
        }

        /// <summary>
        /// Gets or sets title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion
    }
}
