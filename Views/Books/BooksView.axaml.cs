using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class BooksView : UserControl
{
    public BooksView()
    {
        Resources.Add("TimeToStringConverter", new TimeToStringConverter());
        Resources.Add("DateTimeToStringConverter", new DateTimeToStringConverter());
        InitializeComponent();
    }
}
