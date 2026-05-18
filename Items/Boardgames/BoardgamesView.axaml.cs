using Avalonia.Controls;

namespace EventTracker.Views;

public partial class BoardgamesView : UserControl
{
    public BoardgamesView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
