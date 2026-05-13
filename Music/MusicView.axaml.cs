using Avalonia.Controls;

namespace EventTracker.Views;

public partial class MusicView : UserControl
{
    public MusicView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
