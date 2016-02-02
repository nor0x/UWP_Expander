using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Project.Views.Controls.ExpaControl
{
    public partial class HierarchicalDataTemplate : DataTemplate
    {
        public Binding ItemsSource { get; set; }

        private DataTemplate _itemTemplate;

        internal bool IsItemTemplateSet { get; private set; }

        public DataTemplate ItemTemplate
        {
            get { return _itemTemplate; }
            set
            {
                IsItemTemplateSet = true;
                _itemTemplate = value;
            }
        }

        private Style _itemContainerStyle;

        internal bool IsItemContainerStyleSet { get; private set; }

        public Style ItemContainerStyle
        {
            get { return _itemContainerStyle; }
            set
            {
                IsItemContainerStyleSet = true;
                _itemContainerStyle = value;
            }
        }

        public HierarchicalDataTemplate()
        {
        }
    }
}
