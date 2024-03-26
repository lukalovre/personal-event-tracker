using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class LocationsView : UserControl
{
    public LocationsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
