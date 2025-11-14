using Avalonia.Controls;

namespace EventTracker.Views;

public partial class ClassicalView : UserControl
{
    public ClassicalView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
