using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class ClipsView : UserControl
{
    public ClipsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
