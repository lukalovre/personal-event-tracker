using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class MoviesView : UserControl
{
    public MoviesView()
    {
        Resources.Add("TimeToStringConverter", new TimeToStringConverter());
        Resources.Add("DateTimeToStringConverter", new DateTimeToStringConverter());
        InitializeComponent();
    }
}
