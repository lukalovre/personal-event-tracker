using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class PinballView : UserControl
{
    public PinballView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
