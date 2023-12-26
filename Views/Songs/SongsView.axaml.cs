using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class SongsView : UserControl
{
    public SongsView()
    {
        Resources.Add("TimeToStringConverter", new TimeToStringConverter());
        Resources.Add("DateTimeToStringConverter", new DateTimeToStringConverter());
        InitializeComponent();
    }
}
