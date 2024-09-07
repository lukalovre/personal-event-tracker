using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class StatsView : UserControl
{
    public StatsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}