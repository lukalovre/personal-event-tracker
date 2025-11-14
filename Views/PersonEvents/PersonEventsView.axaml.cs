using Avalonia.Controls;

namespace EventTracker.Views;

public partial class PersonEventsView : UserControl
{
    public PersonEventsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
