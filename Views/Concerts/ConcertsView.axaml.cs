using Avalonia.Controls;

namespace EventTracker.Views;

public partial class ConcertsView : UserControl
{
    public ConcertsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
