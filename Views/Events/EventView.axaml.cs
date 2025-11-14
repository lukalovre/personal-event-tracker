using Avalonia.Controls;

namespace EventTracker.Views;

public partial class EventView : UserControl
{
    public EventView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
