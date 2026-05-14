using Avalonia.Controls;

namespace EventTracker.Views;

public partial class GamesView : UserControl
{
    public GamesView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
