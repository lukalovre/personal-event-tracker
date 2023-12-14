using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class TVShowsView : UserControl
{
    public TVShowsView()
    {
        Resources.Add("TimeToStringConverter", new TimeToStringConverter());
        Resources.Add("DateTimeToStringConverter", new DateTimeToStringConverter());
        InitializeComponent();
    }
}
