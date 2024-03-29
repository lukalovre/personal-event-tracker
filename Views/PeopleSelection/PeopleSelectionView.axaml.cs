using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class PeopleSelectionView : UserControl
{
    public PeopleSelectionView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
