using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_Backdrop", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ContentSite", Type = typeof(Border))]
    [TemplatePart(Name = "PART_CollapseButton", Type = typeof(Button))]
    [ContentProperty("Content")]
    public class Flyout : Control
    {
        #region - Fields -

        private bool isOpening;

        #endregion

        #region - Dependency properties -

        /// <summary>
        /// Identifies Content property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(Flyout), new UIPropertyMetadata());

        /// <summary>
        /// Identifies FlyoutWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty FlyoutWidthProperty = DependencyProperty.Register("FlyoutWidth", typeof(double), typeof(Flyout), new UIPropertyMetadata(300.0, new PropertyChangedCallback(OnWidthChanged)));

        /// <summary>
        /// Identifies SlideInAnimationWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty SlideInAnimationWidthProperty = DependencyProperty.Register("SlideInAnimationWidth", typeof(double?), typeof(Flyout), new UIPropertyMetadata(default(double?), new PropertyChangedCallback(OnWidthChanged)));

        /// <summary>
        /// Identifies SlideOutAnimationWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty SlideOutAnimationWidthProperty = DependencyProperty.Register("SlideOutAnimationWidth", typeof(double?), typeof(Flyout), new UIPropertyMetadata(default(double?), new PropertyChangedCallback(OnWidthChanged)));

        /// <summary>
        /// Identifies Backdrop dependency property.
        /// </summary>
        public static readonly DependencyProperty BackdropProperty = DependencyProperty.Register("Backdrop", typeof(bool), typeof(Flyout), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies ShowBackArrow dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowBackArrowProperty = DependencyProperty.Register("ShowBackArrow", typeof(bool), typeof(Flyout), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Flyout));

        /// <summary>
        /// Identifies Duration dependency property.
        /// </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(double), typeof(Flyout), new PropertyMetadata(200.0));

        /// <summary>
        /// Identifies IsOpen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(Flyout), 
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsOpenChanged)));

        /// <summary>
        /// Identifies FocusElementProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusElementProperty = DependencyProperty.Register("FocusElement", typeof(UIElement), typeof(Flyout));

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

        private ThicknessAnimation _outAnimation;
        private ThicknessAnimation _inAnimation;
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
                this.Visibility = Visibility.Collapsed;

                KeyDown += OnKeyDown;
            }
        }
        #endregion

        #region - Private methods -

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape && this.IsVisible && !isOpening)
            {
                this.IsOpen = false;
            }
        }

        private void OnHideFlyout(object sender, RoutedEventArgs e)
        {
            if (isOpening) return;

            this.IsOpen = false;
        }

        private void OnExpanded(object sender, EventArgs e)
        {
            if (FocusElement == null)
            {
                // Focus content, if content is framework element
                //if (this.Content != null && this.Content is FrameworkElement)
                //    (this.Content as FrameworkElement).Focus();
                //else
                //    Focus();
                FocusManager.SetFocusedElement(this, this);
                Keyboard.Focus(this);
            }
            else
            {
                FocusManager.SetFocusedElement(this, FocusElement);
                Keyboard.Focus(FocusElement);
            }

            RaiseEvent(new RoutedEventArgs(ExpandedEvent));

            isOpening = false;

            if (!IsOpen) IsOpen = true;
        }

        private void OnCollapsed(object sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;

            RaiseEvent(new RoutedEventArgs(CollapsedEvent));

            if (IsOpen) IsOpen = false;
        }

        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Clear animations so they can be created with current width
            ((Flyout)d).InAnimation = null;
            ((Flyout)d).OutAnimation = null;
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Flyout @this = (Flyout)d;
            bool newValue = (bool)e.NewValue;

            if (newValue)
                @this.Show();
            else if (!newValue)
                @this.Hide();
        }

        /// <summary>
        /// Shows flyout.
        /// </summary>
        private void Show()
        {
            if (isOpening) return;

            isOpening = true;

            this.Visibility = Visibility.Visible;

            if (contentSite != null)
            {
                contentSite.BeginAnimation(MarginProperty, InAnimation);
            }
            else
            {
                isOpening = false;
            }
        }

        /// <summary>
        /// Hides flyout.
        /// </summary>
        private void Hide()
        {
            if (isOpening) return;

            contentSite.BeginAnimation(MarginProperty, OutAnimation);
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

            if (this.IsVisible)
                Show();
        }

        #endregion

        #region - Private properties -

        private ThicknessAnimation InAnimation
        {
            get
            {
                if (_inAnimation == null)
                {
                    _inAnimation = new ThicknessAnimation()
                    {
                        From = new Thickness(0, 0, -(SlideInAnimationWidth ?? FlyoutWidth), 0),
                        To = new Thickness(0)
                    };

                    // In case open animation width is set to 0, duration is ignore
                    _inAnimation.Duration = TimeSpan.FromMilliseconds(((SlideInAnimationWidth ?? FlyoutWidth) > 0 ? Duration : 0));

                    _inAnimation.Completed += OnExpanded;

                    _inAnimation.Freeze();
                }

                return _inAnimation;
            }
            set { _inAnimation = value; }
        }

        private ThicknessAnimation OutAnimation
        {
            get
            {
                if (_outAnimation == null)
                {
                    _outAnimation = new ThicknessAnimation()
                    {
                        From = new Thickness(0),
                        To = new Thickness(0, 0, -(SlideOutAnimationWidth ?? FlyoutWidth), 0)
                    };

                    // In case close animation width is set to 0, duration is ignore
                    _outAnimation.Duration = TimeSpan.FromMilliseconds(((SlideOutAnimationWidth ?? FlyoutWidth) > 0 ? Duration : 0));

                    _outAnimation.Completed += OnCollapsed;

                    _outAnimation.Freeze();
                }

                return _outAnimation;
            }
            set { _outAnimation = value; }
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
        public double FlyoutWidth
        {
            get { return (double)GetValue(FlyoutWidthProperty); }
            set { SetValue(FlyoutWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets slide-in animation size. 
        /// <para>If not set, flyout width is used.</para>
        /// </summary>
        public double? SlideInAnimationWidth
        {
            get { return (double?)GetValue(SlideInAnimationWidthProperty); }
            set { SetValue(SlideInAnimationWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets slide-out animation size. 
        /// <para>If not set, flyout width is used.</para>
        /// </summary>
        public double? SlideOutAnimationWidth
        {
            get { return (double?)GetValue(SlideOutAnimationWidthProperty); }
            set { SetValue(SlideOutAnimationWidthProperty, value); }
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
        /// Indicates whether back arrow should be displayed in flyout.
        /// </summary>
        public bool ShowBackArrow
        {
            get { return (bool)GetValue(ShowBackArrowProperty); }
            set { SetValue(ShowBackArrowProperty, value); }
        }

        /// <summary>
        /// Gets or sets title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets or sets duration, in milliseconds, for animation used when showing or hiding flyout.
        /// </summary>
        public double Duration
        {
            get { return (double)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether flyout is open.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Gets or sets focused element on expand.
        /// </summary>
        public UIElement FocusElement
        {
            get { return (UIElement)GetValue(FocusElementProperty); }
            set { SetValue(FocusElementProperty, value); }
        }

        #endregion
    }
}
