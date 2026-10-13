using Avalonia.Controls;
using EventTracker.ViewModels;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using System.Collections.Generic;
using System.Linq;

namespace EventTracker.Views;

public partial class YearlyTrendsView : UserControl
{
    public YearlyTrendsView()
    {
        InitializeComponent();
    }

    private void ChartOnDataPointerDown(IChartView chart, IEnumerable<ChartPoint> points)
    {
        var category = points.FirstOrDefault()?.Context?.Series?.Name;
        (DataContext as YearlyTrendsViewModel)?.ToggleChartUnits(category);
    }
}
