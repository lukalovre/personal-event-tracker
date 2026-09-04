using Avalonia.Controls;
using Avalonia.Controls.Selection;
using EventTracker.ViewModels;

namespace EventTracker.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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
