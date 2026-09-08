using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Controls.Selection;
using EventTracker.ViewModels;

namespace EventTracker.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AddHandler(Button.ClickEvent, OnButtonClick, RoutingStrategies.Bubble);
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
    }
}
