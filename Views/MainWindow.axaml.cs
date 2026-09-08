using System.Collections;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using Avalonia.Controls.Selection;
using EventTracker.ViewModels;

namespace EventTracker.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AddHandler(Button.ClickEvent, OnButtonClick, RoutingStrategies.Bubble);
        AddHandler(TabControl.SelectionChangedEvent, OnTabSelectionChanged, RoutingStrategies.Bubble);
    }

    private async void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not Button { Content: Image } button
            || button.Command is not null
            || button.DataContext is not IImagePickerViewModel viewModel)
        {
            return;
        }

        await viewModel.PickImageAsync(this, button.Tag is "New");
    }

    private void ItemTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.Source != sender
            || sender is not TabControl { SelectedItem: TabItem { Content: Control { DataContext: IDataGrid viewModel } } })
        {
            return;
        }

        viewModel.SelectFirstItem();
        UpdateSelectedGridCount((sender as TabControl)?.SelectedItem as TabItem);
    }

    private void OnTabSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.Source is TabControl { SelectedItem: TabItem selectedTab })
        {
            UpdateSelectedGridCount(selectedTab);
        }
    }

    private static void UpdateSelectedGridCount(TabItem? selectedTab)
    {
        if (selectedTab?.Content is not Control content)
        {
            return;
        }

        var gridFilter = content.GetVisualDescendants().OfType<GridFilterView>().FirstOrDefault();
        var dataGrid = content.GetVisualDescendants().OfType<DataGrid>().FirstOrDefault();

        if (gridFilter?.DataContext is GridFilterViewModel filterViewModel && dataGrid is not null)
        {
            filterViewModel.GridCountItems = dataGrid.ItemsSource switch
            {
                ICollection collection => collection.Count,
                IEnumerable items => items.Cast<object>().Count(),
                _ => 0
            };
        }
    }
}
