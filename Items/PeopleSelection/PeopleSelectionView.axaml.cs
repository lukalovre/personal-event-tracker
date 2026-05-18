using Avalonia.Controls;

namespace EventTracker.Views;

public partial class PeopleSelectionView : UserControl
{
    public PeopleSelectionView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
