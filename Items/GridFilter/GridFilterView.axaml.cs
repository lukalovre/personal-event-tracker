using Avalonia.Controls;
using Avalonia.Input;
using System;
using EventTracker.ViewModels;

namespace EventTracker.Views;

public partial class GridFilterView : UserControl
{
    public GridFilterView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }

    private void SearchTextBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || DataContext is not GridFilterViewModel viewModel)
        {
            return;
        }

        viewModel.Search.Execute().Subscribe();
        e.Handled = true;
    }
}
