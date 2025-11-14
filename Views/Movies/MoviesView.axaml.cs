using Avalonia.Controls;

namespace EventTracker.Views;

public partial class MoviesView : UserControl
{
    public MoviesView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
