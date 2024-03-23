using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class ComicsView : UserControl
{
    public ComicsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
