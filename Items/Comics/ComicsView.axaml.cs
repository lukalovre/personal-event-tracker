using Avalonia.Controls;

namespace EventTracker.Views;

public partial class ComicsView : UserControl
{
    public ComicsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
