using Avalonia.Controls;

namespace EventTracker.Views;

public partial class WorkView : UserControl
{
    public WorkView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
