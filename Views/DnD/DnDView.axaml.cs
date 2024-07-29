using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class DnDView : UserControl
{
    public DnDView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
