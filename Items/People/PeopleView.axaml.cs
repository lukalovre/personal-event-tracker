using Avalonia.Controls;

namespace EventTracker.Views;

public partial class PeopleView : UserControl
{
    public PeopleView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
