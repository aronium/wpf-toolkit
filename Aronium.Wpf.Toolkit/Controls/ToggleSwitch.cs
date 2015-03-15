using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_Thumb", Type = typeof(Ellipse))]
    public class ToggleSwitch : ToggleButton
    {
        #region - Fields -
        private Ellipse slider;
        private DoubleAnimation checkAnimation;
        private DoubleAnimation uncheckAnimation;
        #endregion

        #region - Dependency properties -

        public static DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", 
            typeof(CornerRadius), 
            typeof(ToggleSwitch),
            new FrameworkPropertyMetadata(new CornerRadius(9), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        
        public static DependencyProperty BackgroundCheckedProperty = DependencyProperty.Register("BackgroundChecked", typeof(Brush), typeof(ToggleSwitch));
                
        public static DependencyProperty BorderBrushCheckedProperty = DependencyProperty.Register("BorderBrushChecked", typeof(Brush), typeof(ToggleSwitch));
        
        public static DependencyProperty SliderWidthProperty = DependencyProperty.Register("SliderWidth", typeof(double), typeof(ToggleSwitch),
            new FrameworkPropertyMetadata(34.0,
                FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnSliderWidthChanged)));

        public static DependencyProperty SliderHeightProperty = DependencyProperty.Register("SliderHeight", typeof(double), typeof(ToggleSwitch),
            new FrameworkPropertyMetadata(20.0,
                FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static DependencyProperty ThumbSizeProperty = DependencyProperty.Register("ThumbSize", typeof(double), typeof(ToggleSwitch),
            new FrameworkPropertyMetadata(18.0,
                FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        
        public static DependencyProperty ThumbBackgroundProperty = DependencyProperty.Register("ThumbBackground", typeof(Brush), typeof(ToggleSwitch));
       
        public static DependencyProperty ThumbBackgroundCheckedProperty = DependencyProperty.Register("ThumbBackgroundChecked", typeof(Brush), typeof(ToggleSwitch));
        
        public static DependencyProperty SliderPaddingProperty = DependencyProperty.Register("SliderPadding", typeof(Thickness), typeof(ToggleSwitch));

        #endregion

        #region - Constructor -

        /// <summary>
        /// Initializes new instance of OnOffButton class.
        /// </summary>
        public ToggleSwitch()
        {
            this.Checked += new RoutedEventHandler(OnChecked);
            this.Unchecked += new RoutedEventHandler(OnUnchecked);
        }

        #endregion

        #region - Private methods -

        private void CreateAnimations()
        {
            if (slider != null)
            {
                checkAnimation = new DoubleAnimation()
                {
                    From = 1,
                    To = SliderOffset,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new CircleEase()
                };

                uncheckAnimation = new DoubleAnimation()
                {
                    To = 1,
                    From = SliderOffset,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new CircleEase()
                };

                checkAnimation.Freeze();
                uncheckAnimation.Freeze();
            }
            else
            {
                checkAnimation = null;
                uncheckAnimation = null;
            }
        }

        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (slider != null && uncheckAnimation != null)
            {
                slider.BeginAnimation(Canvas.LeftProperty, uncheckAnimation);
            }
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            RunCheckedAnimationIfAvailable();
        }

        private void RunCheckedAnimationIfAvailable()
        {
            if (slider != null && checkAnimation != null)
            {
                slider.BeginAnimation(Canvas.LeftProperty, checkAnimation);
            }
        }

        private static void OnSliderWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToggleSwitch)d).UpdateSliderWidth();
        }

        private void UpdateSliderWidth()
        {
            CreateAnimations();
        } 

        #endregion

        #region - Public methods -

        /// <summary>
        /// Occurs when template is applied to control
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            slider = this.Template.FindName("PART_Thumb", this) as Ellipse;

            CreateAnimations();

            if (this.IsChecked == true)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    RunCheckedAnimationIfAvailable();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        #endregion

        #region - Private properties -

        /// <summary>
        /// Gets slider offset.
        /// </summary>
        private double SliderOffset
        {
            get { return this.SliderWidth - this.ThumbSize - 0.5; }
        } 

        #endregion
        
        #region - Properties -

        /// <summary>
        /// Gets or sets corner radius property.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Brush BackgroundChecked
        {
            get { return (Brush)GetValue(BackgroundCheckedProperty); }
            set { SetValue(BackgroundCheckedProperty, value); }
        }

        public Brush BorderBrushChecked
        {
            get { return (Brush)GetValue(BorderBrushCheckedProperty); }
            set { SetValue(BorderBrushCheckedProperty, value); }
        }

        public double SliderWidth
        {
            get { return (double)GetValue(SliderWidthProperty); }
            set { SetValue(SliderWidthProperty, value); }
        }

        public double SliderHeight
        {
            get { return (double)GetValue(SliderHeightProperty); }
            set { SetValue(SliderHeightProperty, value); }
        }

        public double ThumbSize
        {
            get { return (double)GetValue(ThumbSizeProperty); }
            set { SetValue(ThumbSizeProperty, value); }
        }

        public Brush ThumbBackground
        {
            get { return (Brush)GetValue(ThumbBackgroundProperty); }
            set { SetValue(ThumbBackgroundProperty, value); }
        }

        public Brush ThumbBackgroundChecked
        {
            get { return (Brush)GetValue(ThumbBackgroundCheckedProperty); }
            set { SetValue(ThumbBackgroundCheckedProperty, value); }
        }

        public Thickness SliderPadding
        {
            get { return (Thickness)GetValue(SliderPaddingProperty); }
            set { SetValue(SliderPaddingProperty, value); }
        }
        
        #endregion
    }
}
