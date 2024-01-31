using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class StandupView : UserControl
{
    public StandupView()
    {
        Resources.Add("TimeToStringConverter", new TimeToStringConverter());
        Resources.Add("DateTimeToStringConverter", new DateTimeToStringConverter());
        InitializeComponent();
    }
}
