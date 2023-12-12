using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class WorkView : UserControl
{
    public WorkView()
    {
        Resources.Add("TimeToStringConverter", new TimeToStringConverter());
        Resources.Add("DateTimeToStringConverter", new DateTimeToStringConverter());
        InitializeComponent();
    }
}
