using Avalonia.Controls;

namespace EventTracker.Views;

public partial class ClipsView : UserControl
{
    public ClipsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
