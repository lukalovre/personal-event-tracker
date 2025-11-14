using Avalonia.Controls;

namespace EventTracker.Views;

public partial class DnDView : UserControl
{
    public DnDView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
