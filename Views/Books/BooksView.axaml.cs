using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class BooksView : UserControl
{
    public BooksView()
    {
        ViewHelper.AddConverters(Resources);
        InitializeComponent();
    }
}
