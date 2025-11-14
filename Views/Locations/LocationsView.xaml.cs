using Avalonia.Controls;

namespace EventTracker.Views;

public partial class LocationsView : UserControl
{
    public LocationsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
