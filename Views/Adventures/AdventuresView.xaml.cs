using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class AdventuresView : UserControl
{
    public AdventuresView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
