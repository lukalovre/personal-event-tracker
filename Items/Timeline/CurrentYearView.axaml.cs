using Avalonia.Controls;
using EventTracker.ViewModels;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;

namespace EventTracker.Views;

public partial class CurrentYearView : UserControl
{
    public CurrentYearView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }

    private void PieChartOnDataPointerDown(IChartView chart, IEnumerable<ChartPoint> points)
    {
        var category = points.FirstOrDefault()?.Context?.Series?.Name;
        (DataContext as CurrentYearViewModel)?.ToggleCategory(category);
    }
}
