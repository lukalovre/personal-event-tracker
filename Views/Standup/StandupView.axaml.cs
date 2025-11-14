using Avalonia.Controls;

namespace EventTracker.Views;

public partial class StandupView : UserControl
{
    public StandupView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
