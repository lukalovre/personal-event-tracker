using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class SongsView : UserControl
{
    public SongsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
