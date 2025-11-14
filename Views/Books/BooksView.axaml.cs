using Avalonia.Controls;

namespace EventTracker.Views;

public partial class BooksView : UserControl
{
    public BooksView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
