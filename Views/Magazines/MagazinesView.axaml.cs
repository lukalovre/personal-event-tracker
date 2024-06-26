using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class MagazinesView : UserControl
{
    public MagazinesView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
