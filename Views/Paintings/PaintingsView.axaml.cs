using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class PaintingsView : UserControl
{
    public PaintingsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
