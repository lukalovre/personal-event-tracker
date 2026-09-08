using Avalonia.Controls;

namespace EventTracker.Views;

public partial class CurrentMonthView : UserControl
{
    public CurrentMonthView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}