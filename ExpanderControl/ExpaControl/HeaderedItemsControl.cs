using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Project.Views.Controls.ExpaControl
{
    public class HeaderedItemsControl : ItemsControl
    {
        internal bool HeaderIsItem { get; set; }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof(object), typeof(HeaderedItemsControl), new PropertyMetadata(default(object), OnHeaderPropertyChanged));

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as HeaderedItemsControl;
            if (source != null) source.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        private void OnHeaderChanged(object oldHeader, object newHeader)
        {
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate", typeof(DataTemplate), typeof(HeaderedItemsControl), new PropertyMetadata(default(DataTemplate), OnHeaderTemplatePropertyChanged));

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        private static void OnHeaderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as HeaderedItemsControl;
            var oldHeaderTemplate = e.OldValue as DataTemplate;
            var newHeaderTemplate = e.NewValue as DataTemplate;
            if (source != null) source.OnHeaderTemplateChanged(oldHeaderTemplate, newHeaderTemplate);
        }

        private void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
        }

        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(
            "ItemContainerStyle", typeof(Style), typeof(HeaderedItemsControl),
            new PropertyMetadata(default(Style), OnItemContainerStylePropertyChanged));

        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        private static void OnItemContainerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as HeaderedItemsControl;
            var value = e.NewValue as Style;
            if (source != null) source.ItemsControlHelper.UpdateItemContainerStyle(value);
        }

        internal ItemsControlHelper ItemsControlHelper { get; private set; }

        public HeaderedItemsControl()
        {
            DefaultStyleKey = typeof(HeaderedItemsControl);
            ItemsControlHelper = new ItemsControlHelper(this);
        }

        protected override void OnApplyTemplate()
        {
            ItemsControlHelper.OnApplyTemplate();
            base.OnApplyTemplate();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            ItemsControlHelper.PrepareContainerForItemOverride(element, ItemContainerStyle);
            PreparePrepareHeaderedItemsControlContainerForItemOverride(element, item, this, ItemContainerStyle);

            base.PrepareContainerForItemOverride(element, item);
        }

        internal static void PreparePrepareHeaderedItemsControlContainerForItemOverride(DependencyObject element, object item, ItemsControl parent, Style parentItemContainerStyle)
        {
            var headeredItemsControl = element as HeaderedItemsControl;
            if (headeredItemsControl != null)
            {
                PreparePrepareHeaderedItemsControlContainer(headeredItemsControl, item, parent, parentItemContainerStyle);
            }
        }

        private static void PreparePrepareHeaderedItemsControlContainer(HeaderedItemsControl control, object item, ItemsControl parentItemsControl, Style parentItemContainerStyle)
        {
            if (control != item)
            {
                var parentItemTemplate = parentItemsControl.ItemTemplate;
                if (parentItemTemplate != null)
                {
                    control.SetValue(ItemTemplateProperty, parentItemTemplate);
                }
                if (parentItemContainerStyle != null && HasDefaultValue(control, ItemContainerStyleProperty))
                {
                    control.SetValue(ItemContainerStyleProperty, parentItemContainerStyle);
                }
                if (control.HeaderIsItem || HasDefaultValue(control, HeaderProperty))
                {
                    control.Header = item;
                    control.HeaderIsItem = true;
                }
                if (parentItemTemplate != null)
                {
                    control.SetValue(HeaderTemplateProperty, parentItemTemplate);
                }
                if (parentItemContainerStyle != null && control.Style == null)
                {
                    control.SetValue(StyleProperty, parentItemContainerStyle);
                }

                var headerTemplate = parentItemTemplate as HierarchicalDataTemplate;
                if (headerTemplate != null)
                {
                    if (headerTemplate.ItemsSource != null && HasDefaultValue(control, ItemsSourceProperty))
                    {
                        control.SetBinding(ItemsSourceProperty, new Binding
                        {
                            Converter = headerTemplate.ItemsSource.Converter,
                            ConverterLanguage = headerTemplate.ItemsSource.ConverterLanguage,
                            ConverterParameter = headerTemplate.ItemsSource.ConverterParameter,
                            Mode = headerTemplate.ItemsSource.Mode,
                            Path = headerTemplate.ItemsSource.Path,
                            Source = control.Header
                        });
                    }
                    if (headerTemplate.IsItemTemplateSet && control.ItemTemplate == parentItemTemplate)
                    {
                        control.ClearValue(ItemTemplateProperty);
                        if (headerTemplate.ItemTemplate != null)
                        {
                            control.ItemTemplate = headerTemplate.ItemTemplate;
                        }
                    }
                    if (headerTemplate.IsItemContainerStyleSet && control.ItemContainerStyle == parentItemContainerStyle)
                    {
                        control.ClearValue(ItemContainerStyleProperty);
                        if (headerTemplate.ItemContainerStyle != null)
                        {
                            control.ItemContainerStyle = headerTemplate.ItemContainerStyle;
                        }
                    }
                }
            }
        }

        private static bool HasDefaultValue(Control control, DependencyProperty property)
        {
            Debug.Assert(control != null, "control should not be null!");
            Debug.Assert(property != null, "property should not be null!");
            return control != null && control.ReadLocalValue(property) == DependencyProperty.UnsetValue;
        }
    }
}
