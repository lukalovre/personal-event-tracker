using Avalonia.Controls;

namespace EventTracker.Views;

public partial class AdventuresView : UserControl
{
    public AdventuresView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
