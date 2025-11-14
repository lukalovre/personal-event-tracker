using Avalonia.Controls;

namespace EventTracker.Views;

public partial class TimelineView : UserControl
{
    public TimelineView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}