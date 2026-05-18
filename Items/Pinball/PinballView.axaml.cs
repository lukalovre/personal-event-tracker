using Avalonia.Controls;

namespace EventTracker.Views;

public partial class PinballView : UserControl
{
    public PinballView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
