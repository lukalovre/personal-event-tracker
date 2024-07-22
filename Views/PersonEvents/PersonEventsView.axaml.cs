using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class PersonEventsView : UserControl
{
    public PersonEventsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
