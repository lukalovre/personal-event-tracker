using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class TheatreView : UserControl
{
    public TheatreView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
