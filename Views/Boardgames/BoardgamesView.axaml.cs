using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class BoardgamesView : UserControl
{
    public BoardgamesView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
