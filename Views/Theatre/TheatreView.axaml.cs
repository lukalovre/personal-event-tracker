using Avalonia.Controls;

namespace EventTracker.Views;

public partial class TheatreView : UserControl
{
    public TheatreView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
