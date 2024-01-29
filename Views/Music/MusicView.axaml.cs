using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class MusicView : UserControl
{
    public MusicView()
    {
        Resources.Add("TimeToStringConverter", new TimeToStringConverter());
        Resources.Add("DateTimeToStringConverter", new DateTimeToStringConverter());
        InitializeComponent();
    }
}
