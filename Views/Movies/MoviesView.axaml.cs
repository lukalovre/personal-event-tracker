using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class MoviesView : UserControl
{
    public MoviesView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
