using Avalonia.Controls;

namespace EventTracker.Views;

public partial class ZooView : UserControl
{
    public ZooView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
