using Avalonia.Controls;

namespace EventTracker.Views;

public partial class NewEventFieldsView : UserControl
{
    public NewEventFieldsView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
