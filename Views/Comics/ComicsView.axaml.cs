using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class ComicsView : UserControl
{
    public ComicsView()
    {
        Resources.Add("TimeToStringConverter", new TimeToStringConverter());
        Resources.Add("DateTimeToStringConverter", new DateTimeToStringConverter());
        InitializeComponent();
    }
}
