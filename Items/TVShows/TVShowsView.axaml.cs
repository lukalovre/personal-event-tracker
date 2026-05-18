using Avalonia.Controls;

namespace EventTracker.Views;

public partial class TVShowsView : UserControl
{
    public TVShowsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
