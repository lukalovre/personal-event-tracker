using Avalonia.Controls;

namespace EventTracker.Views;

public partial class PaintingsView : UserControl
{
    public PaintingsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
