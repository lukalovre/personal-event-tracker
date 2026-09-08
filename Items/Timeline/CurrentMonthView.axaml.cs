using Avalonia.Controls;
using EventTracker.ViewModels;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;

namespace EventTracker.Views;

public partial class CurrentMonthView : UserControl
{
    public CurrentMonthView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }

    private void PieChartOnDataPointerDown(IChartView chart, IEnumerable<ChartPoint> points)
    {
        var category = points.FirstOrDefault()?.Context?.Series?.Name;
        (DataContext as CurrentMonthViewModel)?.ToggleCategory(category);
    }
}