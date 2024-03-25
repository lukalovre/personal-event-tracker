using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class GridFilterView : UserControl
{
    public GridFilterView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
