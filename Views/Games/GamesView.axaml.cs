using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class GamesView : UserControl
{
    public GamesView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
