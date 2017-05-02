using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Aronium.Wpf.Toolkit
{
    public static class VisualHelper
    {
        /// <summary>
        /// Gets visual parent of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type or parent to find.</typeparam>
        /// <param name="child">Child element.</param>
        /// <returns>First visual parent of specified type, if any, otherwise null.</returns>
        public static T FindVisualParent<T>(this DependencyObject child) where T : DependencyObject
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

        /// <summary>
        /// Gets nearest container of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of visual container to search for.</typeparam>
        /// <param name="initial">Starting element in a visual tree.</param>
        /// <returns>Instance of specified type, if any, otherwise null.</returns>
        public static T GetNearestContainer<T>(this DependencyObject initial) where T : DependencyObject
        {
            DependencyObject visual = initial;

            if (visual is Visual || visual is System.Windows.Media.Media3D.Visual3D)
            {
                while (visual != null && visual.GetType() != typeof(T))
                {
                    visual = VisualTreeHelper.GetParent(visual);
                }
            }

            return visual as T;
        }

        /// <summary>
        /// Gets visual child of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to search for.</typeparam>
        /// <param name="parent">Parent element.</param>
        /// <returns>First instance of specified type, if any, otherwise null.</returns>
        public static T GetVisualChild<T>(this DependencyObject parent) where T : FrameworkElement
        {
            return GetVisualChild<T>(parent, null);
        }

        /// <summary>
        /// Gets child element of specified type with specified name.
        /// </summary>
        /// <typeparam name="T">Type to search for.</typeparam>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Element name to search for.</param>
        /// <returns></returns>
        public static T GetVisualChild<T>(this DependencyObject parent, string name) where T : FrameworkElement
        {
            T child = default(T);

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                FrameworkElement v = (FrameworkElement)VisualTreeHelper.GetChild(parent, i);

                if (name == null || v.Name == name)
                    child = v as T;

                if (child == null)
                {
                    child = GetVisualChild<T>(v, name);
                }

                if (child != null)
                {
                    // Break current and recursive lookup
                    break;
                }
            }

            return child;
        }

        /// <summary>
        /// Gets list of visual children of specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to search for.</typeparam>
        /// <param name="depObj">Parent element.</param>
        /// <returns>List of children of specified type.</returns>
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Gets element of specified type at specified location from within reference visual.
        /// </summary>
        /// <typeparam name="T">Type to search for.</typeparam>
        /// <param name="reference">Visual instance to search element within.</param>
        /// <param name="location">Hit point.</param>
        /// <returns>Instance of specified type, if any, othwerise null.</returns>
        public static T GetItemAtLocation<T>(this Visual reference, Point location)
        {
            T foundItem = default(T);
            HitTestResult hitTestResults = VisualTreeHelper.HitTest(reference, location);

            if (hitTestResults != null && hitTestResults.VisualHit is FrameworkElement)
            {
                object dataObject = (hitTestResults.VisualHit as FrameworkElement).DataContext;

                if (dataObject is T)
                {
                    foundItem = (T)dataObject;
                }
            }

            return foundItem;
        }
    }
}
