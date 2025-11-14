using Avalonia.Controls;

namespace EventTracker.Views;

public partial class MagazinesView : UserControl
{
    public MagazinesView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
