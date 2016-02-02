# UWP Expander (Accordion)

This control is an expandable ListView for the universal windows platform. It's a ported version of the expander for WPF which is not working properly on UWP. The repository is referenced here on [StackOverflow].

### Usage
  - Copy the content from `ExpanderControl` to your project
  - Reference the XAML Namespace
```xml
<Page xmlns:expander="using:Project.Views.Controls.ExpaControl">
```
  - Setup `ExpanderControl`
  
 - can be used as a `ItemTemplate` in a `ListView` see sample for more information



```xml
<expander:ExpanderControl
                        x:Name="expanderControl"
                        IsExpanded="False"
                        ItemsSource="{x:Bind ItemSource}"
                        IsNonExpandable="{x:Bind HasNoSubcategories}"
                        HeaderTemplate="{StaticResource HeaderTemplate}" 
                        ItemTemplate="{StaticResource ItemTemplate}"/>
```

   [StackOverflow]: <http://stackoverflow.com/questions/32607039/expanderview-is-expanded-after-page-navigation>


