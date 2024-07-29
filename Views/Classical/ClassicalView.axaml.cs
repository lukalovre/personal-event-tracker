using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class ClassicalView : UserControl
{
    public ClassicalView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
