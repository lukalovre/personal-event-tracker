using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class GamesView : UserControl
{
    public GamesView()
    {
        Resources.Add("TimeToStringConverter", new TimeToStringConverter());
        Resources.Add("DateTimeToStringConverter", new DateTimeToStringConverter());
        InitializeComponent();
    }
}
