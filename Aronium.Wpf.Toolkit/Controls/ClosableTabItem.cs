using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    public class ClosableTabItem : TabItem
    {
        #region - Fields -

        private ContextMenu contextMenu = new ContextMenu();
        private static RoutedCommand _closeAllCommand = new RoutedCommand("CloseAllCommand", typeof(ClosableTabItem));
        private static RoutedCommand _closeAllButActiveCommand = new RoutedCommand("CloseAllButActiveCommand", typeof(ClosableTabItem));
        private static RoutedCommand _closeTabCommand = new RoutedCommand("CloseTabCommand", typeof(ClosableTabItem));
        private static RoutedCommand _closeOthersCommand = new RoutedCommand("CloseOthersCommand", typeof(ClosableTabItem));
        private static RoutedCommand _scrollRightCommand = new RoutedCommand("ScrollRightCommand", typeof(ClosableTabItem));
        private static RoutedCommand _scrollLeftCommand = new RoutedCommand("ScrollLeftCommand", typeof(ClosableTabItem));
        private MenuItem close = new MenuItem();

        private bool _isMouseDown;
        private bool _isMouseOver;
        private bool _isDragging;
        private Point _mouseDownPosition;

        #endregion

        #region - Dependency Properties -

        public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register("CanClose", typeof(bool), typeof(ClosableTabItem), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty CloseTextProperty = DependencyProperty.Register("CloseText", typeof(string), typeof(ClosableTabItem), new FrameworkPropertyMetadata("Close"));
        public static readonly DependencyProperty CloseAllButThisTextProperty = DependencyProperty.Register("CloseAllButThisText", typeof(string), typeof(ClosableTabItem), new FrameworkPropertyMetadata("Close All But This"));
        public static readonly DependencyProperty CloseAllButActiveTextProperty = DependencyProperty.Register("CloseAllButActiveText", typeof(string), typeof(ClosableTabItem), new FrameworkPropertyMetadata("Close All But Active"));
        public static readonly DependencyProperty CloseAllTextProperty = DependencyProperty.Register("CloseAllText", typeof(string), typeof(ClosableTabItem), new FrameworkPropertyMetadata("Close All"));

        #endregion

        #region - Events -
        /// <summary>
        /// Occurs when child item is closed.
        /// </summary>
        public event EventHandler Closed;
        #endregion

        #region - Commands -

        public static RoutedCommand CloseTabCommand
        {
            get
            {
                return _closeTabCommand;
            }
        }
        public static RoutedCommand CloseOthersCommand
        {
            get
            {
                return _closeOthersCommand;
            }
        }
        public static RoutedCommand CloseAllCommand
        {
            get
            {
                return _closeAllCommand;
            }
        }
        public static RoutedCommand CloseAllButActiveCommand
        {
            get
            {
                return _closeAllButActiveCommand;
            }
        }

        public static RoutedCommand ScrollLeftCommand
        {
            get
            {
                return _scrollLeftCommand;
            }
        }
        public static RoutedCommand ScrollRightCommand
        {
            get
            {
                return _scrollRightCommand;
            }
        }

        #endregion

        #region - Constructor -

        /// <summary>
        /// Initializes new instance of ClosableTabItem class.
        /// </summary>
        public ClosableTabItem()
        {
            CommandManager.RegisterClassCommandBinding(typeof(ClosableTabItem), new CommandBinding(_closeTabCommand, CloseClicked, (sender, e) => { e.CanExecute = CanClose; }));
            CommandManager.RegisterClassCommandBinding(typeof(ClosableTabItem), new CommandBinding(_closeOthersCommand, CloseOthersClicked, (sender, e) => { e.CanExecute = CanClose; }));

            CommandManager.RegisterClassCommandBinding(typeof(ClosableTabControl), new CommandBinding(_closeAllCommand, CloseAll, (sender, e) => { e.CanExecute = CanClose; }));
            CommandManager.RegisterClassCommandBinding(typeof(ClosableTabControl), new CommandBinding(_closeAllButActiveCommand, CloseAllButActive, (sender, e) => { e.CanExecute = CanClose; }));
        }

        #endregion

        #region - Overrides -

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);
            close.Header = string.Format("Close {0}", this.Header);
            close.IsEnabled = this.CanClose;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var headerBorder = this.Template.FindName("PART_ItemBorder", this) as Border;
            if (headerBorder != null)
            {
                headerBorder.MouseDown += new MouseButtonEventHandler(OnTabHeaderMouseDown);
                headerBorder.MouseMove += new MouseEventHandler(OnTabHeaderMouseMove);
            }
        }

        private void OnTabHeaderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Parent == null || !(this.Parent is ClosableTabControl) || !(sender is Border)) return;

            if (((ClosableTabControl)this.Parent).AllowItemsReorder)
            {
                _isMouseDown = true;

                _mouseDownPosition = e.GetPosition((Border)sender);
            }
        }

        private void OnTabHeaderMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMouseDown) return;

            if (!_isDragging)
            {
                _isDragging = true;
                Mouse.Capture((Border)sender);
            }

            var position = e.GetPosition((Border)sender);

            if (position.X < 0 || position.X > ((Border)sender).ActualWidth)
            {
                // If mouse WAS over header border
                if (_isMouseOver)
                {
                    var parent = this.Parent as ClosableTabControl;

                    var index = parent.SelectedIndex;

                    index = position.X < 0 ? index - 1 : index + 1;

                    if (index >= 0 && index <= parent.Items.Count - 1)
                    {
                        parent.Items.Remove(this);
                        parent.Items.Insert(index, this);

                        parent.SelectedItem = this;
                    }
                }

                _isMouseOver = false;
            }
            else
            {
                _isMouseOver = true;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (!_isDragging && e.ChangedButton == MouseButton.Middle && this.CanClose)
            {
                if ((this.Parent as ClosableTabControl) != null)
                {
                    ((ClosableTabControl)this.Parent).RemoveItem(this);
                }
            }

            if (_isDragging)
            {
                Mouse.Capture(null);
            }

            this._isDragging = false;
            this._isMouseDown = false;
            this._isMouseOver = false;

            base.OnMouseUp(e);
        }

        #endregion

        #region - Private methods -

        private void CloseOthersClicked(object sender, ExecutedRoutedEventArgs e)
        {
            // ContextMenu.PlacementTarget is passed as command parameter
            // Border is placement target for context menu. 
            var item = FindVisualParent<ClosableTabItem>(e.Parameter as Border); 

            ((ClosableTabControl)item.Parent).CloseAllButThis(item);

            ((ClosableTabControl)item.Parent).SelectedItem = item;
        }

        private void CloseClicked(object sender, ExecutedRoutedEventArgs e)
        {
            var item = sender as ClosableTabItem;

            // Close command is executed using keyboard, close button and context menu
            // In case context menu is clicked, e.Parameter will be set, search visual parent from ContextMenu.PlacemenetTarget
            if (e.Parameter != null)
            {
                // ContextMenu.PlacementTarget is passed as command parameter
                // Border is placement target for context menu. 
                item = FindVisualParent<ClosableTabItem>(e.Parameter as Border);
            }

            if (item != null && ((ClosableTabItem)item).Parent != null)
            {
                ((ClosableTabControl)item.Parent).RemoveItem(item);
            }
        }

        private void CloseAll(object sender, ExecutedRoutedEventArgs e)
        {
            ((ClosableTabControl)sender).CloseAll();
            e.Handled = true;
        }

        private void CloseAllButActive(object sender, ExecutedRoutedEventArgs e)
        {
            ((ClosableTabControl)sender).CloseAllButActive();
            e.Handled = true;
        }

        #region - Helper methods -

        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }

        #endregion

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or sets a value indicating if tab item can close
        /// </summary>
        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set
            {
                SetValue(CanCloseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value for Close text
        /// </summary>
        public string CloseText
        {
            get { return (string)GetValue(CloseTextProperty); }
            set
            {
                SetValue(CloseTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value for Close All But This text
        /// </summary>
        public string CloseAllButThisText
        {
            get { return (string)GetValue(CloseAllButThisTextProperty); }
            set
            {
                SetValue(CloseAllButThisTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value for Close All But Active text
        /// </summary>
        public string CloseAllButActiveText
        {
            get { return (string)GetValue(CloseAllButActiveTextProperty); }
            set
            {
                SetValue(CloseAllButActiveTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value for Close All text
        /// </summary>
        public string CloseAllText
        {
            get { return (string)GetValue(CloseAllTextProperty); }
            set
            {
                SetValue(CloseAllTextProperty, value);
            }
        }

        #endregion

        #region - Public methods -

        /// <summary>
        /// Dispatches Closed event.
        /// </summary>
        internal void NotifyClosed()
        {
            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
