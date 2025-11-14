using Avalonia.Controls;

namespace EventTracker.Views;

public partial class SongsView : UserControl
{
    public SongsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
