using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class TVShowsView : UserControl
{
    public TVShowsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
