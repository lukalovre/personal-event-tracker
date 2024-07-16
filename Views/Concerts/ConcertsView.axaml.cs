using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class ConcertsView : UserControl
{
    public ConcertsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
