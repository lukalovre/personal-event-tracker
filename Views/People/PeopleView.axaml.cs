using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class PeopleView : UserControl
{
    public PeopleView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
