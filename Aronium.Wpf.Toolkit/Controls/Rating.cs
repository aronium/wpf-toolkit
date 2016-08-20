using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_Stars", Type = typeof(Panel))]
    public class Rating : Control
    {
        private Panel starsPanel;
        public static readonly DependencyProperty StarsProperty = DependencyProperty.Register("Stars", typeof(int), typeof(Rating), new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnStarsChanged)));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(Rating), new PropertyMetadata(3, new PropertyChangedCallback(OnValueChanged)));

        public static readonly DependencyProperty StarSizeProperty = DependencyProperty.Register("StarSize", typeof(double), typeof(Rating));
        public static readonly DependencyProperty StarBrushProperty = DependencyProperty.Register("StarBrush", typeof(Brush), typeof(Rating), new PropertyMetadata(Brushes.Gold));

        private static void OnStarsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = (Rating)d;

            @this.GenerateStars();
        }

        public Rating()
        {
            MouseLeave += OnMouseLeave;
        }

        private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HighlightResult();
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Rating)d).HighlightResult();
        }

        private void HighlightResult()
        {
            HighlightResult(Value);
        }

        private void HighlightResult(int value)
        {
            if (starsPanel != null)
            {
                for (int i = 0; i < starsPanel.Children.Count; i++)
                {
                    ((RatingItem)starsPanel.Children[i]).IsSelected = i < value;
                }
            }
        }

        static Rating()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Rating), new FrameworkPropertyMetadata(typeof(Rating)));
        }

        public int Stars
        {
            get { return (int)GetValue(StarsProperty); }
            set { SetValue(StarsProperty, value); }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double StarSize
        {
            get { return (double)GetValue(StarSizeProperty); }
            set { SetValue(StarSizeProperty, value); }
        }

        public Brush StarBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(StarBrushProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            starsPanel = this.Template.FindName("PART_Stars", this) as Panel;

            GenerateStars();
        }

        private void GenerateStars()
        {
            if (starsPanel != null)
            {
                foreach (var item in starsPanel.Children.Cast<RatingItem>())
                {
                    item.MouseDown -= OnItemMouseDown;
                    item.MouseEnter -= OnItemMouseEnter;
                }

                starsPanel.Children.Clear();

                for (int i = 1; i <= Stars; i++)
                {
                    var item = new RatingItem()
                    {
                        Value = i,
                        ToolTip = $"{i} / {Stars}"
                    };

                    item.MouseDown += OnItemMouseDown;
                    item.MouseEnter += OnItemMouseEnter;

                    starsPanel.Children.Add(item);
                }
            }
        }

        private void OnItemMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HighlightResult(((RatingItem)sender).Value);
        }

        private void OnItemMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Value = ((RatingItem)sender).Value;
        }
    }

    internal class RatingItem : Control
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RatingItem));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(RatingItem));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}

