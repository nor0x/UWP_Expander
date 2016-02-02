using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;

namespace Project.Views.Controls.ExpaControl
{
    [TemplateVisualState(Name = CollapsedState, GroupName = ExpansionStates)]
    [TemplateVisualState(Name = ExpandedState, GroupName = ExpansionStates)]
    [TemplateVisualState(Name = ExpandableState, GroupName = ExpandabilityStates)]
    [TemplateVisualState(Name = NonExpandableState, GroupName = ExpandabilityStates)]
    [TemplatePart(Name = Presenter, Type = typeof(ItemsPresenter))]
    [TemplatePart(Name = ExpanderPanel, Type = typeof(Grid))]
    [TemplatePart(Name = ExpandedStateAnimation, Type = typeof(DoubleAnimation))]
    [TemplatePart(Name = CollapsedToExpandedKeyFrame, Type = typeof(EasingDoubleKeyFrame))]
    [TemplatePart(Name = ExpandedToCollapsedKeyFrame, Type = typeof(EasingDoubleKeyFrame))]

    public class ExpanderControl : HeaderedItemsControl
    {
        public const string ExpansionStates = "ExpansionStates";
        public const string ExpandabilityStates = "ExpandabilityStates";
        public const string CollapsedState = "Collapsed";
        public const string ExpandedState = "Expanded";
        public const string ExpandableState = "Expandable";
        public const string NonExpandableState = "NonExpandable";
        private const string Presenter = "Presenter";
        private const string ExpanderPanel = "ExpanderPanel";
        private const string ExpandedStateAnimation = "ExpandedStateAnimation";
        private const string CollapsedToExpandedKeyFrame = "CollapsedToExpandedKeyFrame";
        private const string ExpandedToCollapsedKeyFrame = "ExpandedToCollapsedKeyFrame";
        private ItemsPresenter _presenter;
        private Canvas _itemsCanvas;
        private Grid _expanderPanel;
        private DoubleAnimation _expandedStateAnimation;
        private EasingDoubleKeyFrame _collapsedToExpandedFrame;
        private EasingDoubleKeyFrame _expandedToCollapsedFrame;
        private const int KeyTimeStep = 20;
        private const int InitialKeyTime = 225;
        private const int FinalKeyTime = 250;

        public event RoutedEventHandler Expanded;
        public event RoutedEventHandler Collapsed;

        public static readonly DependencyProperty ExpanderProperty = DependencyProperty.Register(
            "Expander", typeof(object), typeof(ExpanderControl), new PropertyMetadata(default(object), OnExpanderPropertyChanged));

        public object Expander
        {
            get { return GetValue(ExpanderProperty); }
            set { SetValue(ExpanderProperty, value); }
        }

        private static void OnExpanderPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var source = (ExpanderControl)obj;
            source.OnExpanderChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnExpanderChanged(object oldExpander, object newExpander)
        {
        }

        public static readonly DependencyProperty ExpanderTemplateProperty = DependencyProperty.Register(
            "ExpanderTemplate", typeof(DataTemplate), typeof(ExpanderControl), new PropertyMetadata(default(DataTemplate), OnExpanderTemplatePropertyChanged));

        public DataTemplate ExpanderTemplate
        {
            get { return (DataTemplate)GetValue(ExpanderTemplateProperty); }
            set { SetValue(ExpanderTemplateProperty, value); }
        }

        private static void OnExpanderTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var source = (ExpanderControl)obj;
            source.OnExpanderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        protected virtual void OnExpanderTemplateChanged(DataTemplate oldTemplate, DataTemplate newTemplate)
        {
        }

        public static readonly DependencyProperty NonExpandableHeaderProperty = DependencyProperty.Register(
            "NonExpandableHeader", typeof(object), typeof(ExpanderControl), new PropertyMetadata(default(object), OnNonExpandableHeaderPropertyChanged));

        public object NonExpandableHeader
        {
            get { return GetValue(NonExpandableHeaderProperty); }
            set { SetValue(NonExpandableHeaderProperty, value); }
        }

        private static void OnNonExpandableHeaderPropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            var source = (ExpanderControl)obj;
            source.OnNonExpandableHeaderChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnNonExpandableHeaderChanged(object oldHeader, object newHeader)
        {
        }

        public static readonly DependencyProperty NonExpandableHeaderTemplateProperty = DependencyProperty.Register(
            "NonExpandableHeaderTemplate", typeof(DataTemplate), typeof(ExpanderControl), new PropertyMetadata(default(DataTemplate), OnNonExpandableHeaderTemplatePropertyChanged));

        public DataTemplate NonExpandableHeaderTemplate
        {
            get { return (DataTemplate)GetValue(NonExpandableHeaderTemplateProperty); }
            set { SetValue(NonExpandableHeaderTemplateProperty, value); }
        }

        private static void OnNonExpandableHeaderTemplatePropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            var source = (ExpanderControl)obj;
            source.OnNonExpandableHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        protected virtual void OnNonExpandableHeaderTemplateChanged(DataTemplate oldTemplate, DataTemplate newTemplate)
        {
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded", typeof(bool), typeof(ExpanderControl), new PropertyMetadata(default(bool), OnIsExpandedPropertyChanged));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set
            {
                if (!IsNonExpandable)
                {
                    SetValue(IsExpandedProperty, value);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        private static void OnIsExpandedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var source = (ExpanderControl)obj;
            var args = new RoutedEventArgs();
            if ((bool)e.NewValue)
            {
                source.OnExpanded(args);
            }
            else
            {
                source.OnCollapsed(args);
            }
            source.UpdateVisualState(true);
        }

        public static readonly DependencyProperty HasItemsProperty = DependencyProperty.Register(
            "HasItems", typeof(bool), typeof(ExpanderControl), new PropertyMetadata(default(bool)));

        public bool HasItems
        {
            get { return (bool)GetValue(HasItemsProperty); }
            set { SetValue(HasItemsProperty, value); }
        }

        public static readonly DependencyProperty IsNonExpandableProperty = DependencyProperty.Register(
            "IsNonExpandable", typeof(bool), typeof(ExpanderControl), new PropertyMetadata(default(bool), OnIsNonExpandablePropertyChanged));

        public bool IsNonExpandable
        {
            get { return (bool)GetValue(IsNonExpandableProperty); }
            set { SetValue(IsNonExpandableProperty, value); }
        }

        private static void OnIsNonExpandablePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var source = (ExpanderControl)obj;
            if ((bool)e.NewValue)
            {
                if (source.IsExpanded)
                {
                    source.IsExpanded = false;
                }
            }
            source.UpdateVisualState(true);
        }

        protected override void OnApplyTemplate()
        {
            if (_expanderPanel != null)
            {
                _expanderPanel.Tapped -= OnExpanderPanelTap;
            }
            base.OnApplyTemplate();

            _expanderPanel = GetTemplateChild(ExpanderPanel) as Grid;
            _expandedToCollapsedFrame = GetTemplateChild(ExpandedToCollapsedKeyFrame) as EasingDoubleKeyFrame;
            _collapsedToExpandedFrame = GetTemplateChild(CollapsedToExpandedKeyFrame) as EasingDoubleKeyFrame;
            _itemsCanvas = GetTemplateChild("ItemsCanvas") as Canvas;

            var expandedState = (GetTemplateChild(ExpandedState) as VisualState);
            if (expandedState != null)
            {
                _expandedStateAnimation = expandedState.Storyboard.Children[0] as DoubleAnimation;
            }
            _presenter = GetTemplateChild(Presenter) as ItemsPresenter;
            if (_presenter != null)
            {
                _presenter.SizeChanged += OnPresenterSizeChanged;
            }
            if (_expanderPanel != null)
            {
                _expanderPanel.Tapped += OnExpanderPanelTap;
            }
            UpdateVisualState(false);
        }

        public ExpanderControl()
        {
            DefaultStyleKey = typeof(ExpanderControl);
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_presenter == null) return;
            var parent = _presenter.GetParentByType<ExpanderControl>();
            var gt = parent.TransformToVisual(_presenter);
            var childToParentCoordinates = gt.TransformPoint(new Point(0, 0));
            _presenter.Width = parent.RenderSize.Width + childToParentCoordinates.X;
        }

        private void OnPresenterSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (null != _itemsCanvas && null != _presenter && IsExpanded)
            {
                _itemsCanvas.Height = _presenter.DesiredSize.Height;
            }
            else if (null != _itemsCanvas && null != _presenter && !IsExpanded)
            {
                _itemsCanvas.Height = 0;
            }
        }

        internal virtual void UpdateVisualState(bool useTransitions)
        {
            string isExapandedState, isNonExpandableState;
            if (_presenter != null)
            {
                if (_expandedStateAnimation != null)
                {
                    _expandedStateAnimation.To = _presenter.DesiredSize.Height;
                }
                if (_collapsedToExpandedFrame != null)
                {
                    _collapsedToExpandedFrame.Value = _presenter.DesiredSize.Height;
                }
                if (_expandedToCollapsedFrame != null)
                {
                    _expandedToCollapsedFrame.Value = _presenter.DesiredSize.Height;
                }
            }
            if (IsExpanded)
            {
                isExapandedState = ExpandedState;
                if (useTransitions)
                {
                    AnimateContainerDropDown();
                }
            }
            else
            {
                isExapandedState = CollapsedState;
            }
            VisualStateManager.GoToState(this, isExapandedState, useTransitions);
            if (IsNonExpandable)
            {
                isNonExpandableState = NonExpandableState;
            }
            else
            {
                isNonExpandableState = ExpandableState;
            }
            VisualStateManager.GoToState(this, isNonExpandableState, useTransitions);
        }

        private void RaiseEvent(RoutedEventHandler handler, RoutedEventArgs args)
        {
            if (handler != null)
            {
                handler(this, args);
            }
        }

        internal void AnimateContainerDropDown()
        {
            if (Items == null) return;
            for (var i = 0; i < Items.Count; i++)
            {
                var container = ContainerFromIndex(i) as FrameworkElement;
                if (container == null)
                {
                    break;
                }

                var itemDropDown = new Storyboard();
                var quadraticEase = new QuadraticEase { EasingMode = EasingMode.EaseOut };
                var initialKeyTime = InitialKeyTime + (KeyTimeStep * i);
                var finalKeyTime = FinalKeyTime + (KeyTimeStep * i);

                var translation = new TranslateTransform();
                container.RenderTransform = translation;

                var transAnimation = new DoubleAnimationUsingKeyFrames();

                var transKeyFrame1 = new EasingDoubleKeyFrame
                {
                    EasingFunction = quadraticEase,
                    KeyTime = TimeSpan.FromMilliseconds(0.0),
                    Value = -150.0
                };

                var transKeyFrame2 = new EasingDoubleKeyFrame
                {
                    EasingFunction = quadraticEase,
                    KeyTime = TimeSpan.FromMilliseconds(initialKeyTime),
                    Value = 0.0
                };

                var transKeyFrame3 = new EasingDoubleKeyFrame
                {
                    EasingFunction = quadraticEase,
                    KeyTime = TimeSpan.FromMilliseconds(finalKeyTime),
                    Value = 0.0
                };

                transAnimation.KeyFrames.Add(transKeyFrame1);
                transAnimation.KeyFrames.Add(transKeyFrame2);
                transAnimation.KeyFrames.Add(transKeyFrame3);

                Storyboard.SetTarget(transAnimation, container);
                Storyboard.SetTargetProperty(transAnimation, "(UIElement.RenderTransform).(TranslateTransform.Y)");
                itemDropDown.Children.Add(transAnimation);

                var opacityAnimation = new DoubleAnimationUsingKeyFrames();
                var opacityKeyFrame1 = new EasingDoubleKeyFrame
                {
                    EasingFunction = quadraticEase,
                    KeyTime = TimeSpan.FromMilliseconds(0.0),
                    Value = 0.0
                };

                var opacityKeyFrame2 = new EasingDoubleKeyFrame
                {
                    EasingFunction = quadraticEase,
                    KeyTime = TimeSpan.FromMilliseconds(initialKeyTime - 150),
                    Value = 0.0
                };

                var opacityKeyFrame3 = new EasingDoubleKeyFrame
                {
                    EasingFunction = quadraticEase,
                    KeyTime = TimeSpan.FromMilliseconds(finalKeyTime),
                    Value = 1.0
                };

                opacityAnimation.KeyFrames.Add(opacityKeyFrame1);
                opacityAnimation.KeyFrames.Add(opacityKeyFrame2);
                opacityAnimation.KeyFrames.Add(opacityKeyFrame3);

                Storyboard.SetTarget(opacityAnimation, container);
                Storyboard.SetTargetProperty(opacityAnimation, "(UIElement.Opacity)");
                itemDropDown.Children.Add(opacityAnimation);

                itemDropDown.Begin();
            }
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            if (Items != null) HasItems = Items.Count > 0;
        }

        private void OnExpanderPanelTap(object sender, TappedRoutedEventArgs e)
        {
            if (!IsNonExpandable)
            {
                IsExpanded = !IsExpanded;
            }
        }

        protected virtual void OnExpanded(RoutedEventArgs e)
        {
            RaiseEvent(Expanded, e);
        }

        protected virtual void OnCollapsed(RoutedEventArgs e)
        {
            RaiseEvent(Collapsed, e);
        }
    }
    internal enum ExpansionStates
    {
        Collapsed = 0,
        Expanded = 1,
    }

    internal enum ExpandabilityStates
    {
        Expandable = 0,
        NonExpandable = 1,
    }
}
