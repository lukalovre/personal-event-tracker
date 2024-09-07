using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class TimelineView : UserControl
{
    public TimelineView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}