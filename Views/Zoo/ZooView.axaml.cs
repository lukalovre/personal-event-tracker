using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class ZooView : UserControl
{
    public ZooView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
