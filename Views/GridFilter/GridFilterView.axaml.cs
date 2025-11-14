using Avalonia.Controls;

namespace EventTracker.Views;

public partial class GridFilterView : UserControl
{
    public GridFilterView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
