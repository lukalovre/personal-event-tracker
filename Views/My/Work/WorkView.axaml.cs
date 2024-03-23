using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class WorkView : UserControl
{
    public WorkView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
