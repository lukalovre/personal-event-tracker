using Avalonia.Controls;

namespace EventTracker.Views;

public partial class StatsView : UserControl
{
    public StatsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}