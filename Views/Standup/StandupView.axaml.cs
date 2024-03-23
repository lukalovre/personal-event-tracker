using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class StandupView : UserControl
{
    public StandupView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
