using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class EventView : UserControl
{
    public EventView()
    {
        Resources.Add("DateTimeToStringConverter", new DateTimeToStringConverter());
        InitializeComponent();
    }
}
