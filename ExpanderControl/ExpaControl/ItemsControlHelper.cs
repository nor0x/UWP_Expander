using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Project.Views.Controls.ExpaControl
{
    internal sealed partial class ItemsControlHelper
    {
        private ItemsControl ItemsControl { get; set; }

        private Panel _itemsHost;

        internal Panel ItemHost
        {
            get
            {
                if (_itemsHost == null && ItemsControl != null && ItemsControl.ItemContainerGenerator != null)
                {
                    var container = ItemsControl.ContainerFromIndex(0);
                    if (container != null)
                    {
                        _itemsHost = VisualTreeHelper.GetParent(container) as Panel;
                    }
                }
                return _itemsHost;
            }
        }

        private ScrollViewer _scrollHost;

        internal ScrollViewer ScrollHost
        {
            get
            {
                if (_scrollHost == null)
                {
                    var itemsHost = ItemHost;
                    if (itemsHost != null)
                    {
                        for (DependencyObject obj = itemsHost; obj != ItemsControl && obj != null; obj = VisualTreeHelper.GetParent(obj))
                        {
                            var viewer = obj as ScrollViewer;
                            if (viewer != null)
                            {
                                _scrollHost = viewer;
                                break;
                            }
                        }
                    }
                }
                return _scrollHost;
            }
        }

        internal ItemsControlHelper(ItemsControl control)
        {
            Debug.Assert(control != null, "control cannot be null!");
            ItemsControl = control;
        }

        internal void OnApplyTemplate()
        {
            _itemsHost = null;
            _scrollHost = null;
        }

        internal static void PrepareContainerForItemOverride(DependencyObject element, Style parentItemContainerStyle)
        {
            var control = element as Control;
            if (parentItemContainerStyle != null && control != null && control.Style == null)
            {
                control.SetValue(Control.StyleProperty, parentItemContainerStyle);
            }
        }

        internal void UpdateItemContainerStyle(Style itemContainerStyle)
        {
            if (itemContainerStyle == null)
            {
                return;
            }
            var itemsHost = ItemHost;
            if (itemsHost == null || itemsHost.Children == null)
            {
                return;
            }
            foreach (var element in itemsHost.Children)
            {
                var obj = element as FrameworkElement;
                if (obj != null && obj.Style == null)
                {
                    obj.Style = itemContainerStyle;
                }
            }
        }

        internal void ScrollIntoView(FrameworkElement element)
        {
            var scrollHost = ScrollHost;
            if (scrollHost == null)
            {
                return;
            }
            GeneralTransform transform;
            try
            {
                transform = element.TransformToVisual(scrollHost);
            }
            catch (ArgumentException)
            {
                return;
            }
            var itemRect = new Rect(transform.TransformPoint(new Point()),
                transform.TransformPoint(new Point(element.ActualWidth, element.ActualHeight)));
            var verticalOffset = scrollHost.VerticalOffset;
            double verticalDelta = 0;
            var hostBottom = scrollHost.ViewportHeight;
            var itemBottom = itemRect.Bottom;
            if (hostBottom < itemBottom)
            {
                verticalDelta = itemBottom - hostBottom;
                verticalOffset += verticalDelta;
            }
            var itemTop = itemRect.Top;
            if (itemTop - verticalDelta < 0)
            {
                verticalOffset -= verticalDelta - itemTop;
            }

            var horizontalOffset = scrollHost.HorizontalOffset;
            double horizontalDelta = 0;
            var hostRight = scrollHost.ViewportWidth;
            var itemRight = itemRect.Right;
            if (hostRight < itemRight)
            {
                horizontalDelta = itemRight - hostRight;
                horizontalOffset += horizontalDelta;
            }
            var itemLeft = itemRect.Left;
            if (itemLeft - horizontalDelta < 0)
            {
                horizontalOffset -= horizontalDelta - itemLeft;
            }
            scrollHost.ChangeView(horizontalOffset, verticalOffset, null);
        }
    }
}
