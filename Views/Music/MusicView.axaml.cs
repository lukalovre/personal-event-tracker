using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class MusicView : UserControl
{
    public MusicView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
