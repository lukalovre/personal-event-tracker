using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Avalonia.Controls.Selection;
using EventTracker.ViewModels;
using Repositories;

namespace EventTracker.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        HtmlHelper.LocalHtmlFilePicker = OpenLocalHtmlFileAsync;
        AddHandler(Button.ClickEvent, OnButtonClick, RoutingStrategies.Bubble);
        AddHandler(TabControl.SelectionChangedEvent, OnTabSelectionChanged, RoutingStrategies.Bubble);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        var mainTabControl = (Content as Control)?.GetVisualDescendants().OfType<TabControl>().FirstOrDefault();
        if (mainTabControl?.SelectedItem is not TabItem selectedTab)
        {
            return;
        }

        LoadSelectedTab(selectedTab);
        UpdateSelectedGridCount(selectedTab);
    }

    private async Task<string?> OpenLocalHtmlFileAsync()
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open downloaded webpage",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("HTML files")
                {
                    Patterns = ["*.html", "*.htm"]
                }
            ]
        });

        return files.FirstOrDefault()?.Path.LocalPath;
    }

    private async void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not Button { Content: Image image } button)
        {
            return;
        }

        if (image.Source is not null)
        {
            ShowFullScreenImage(image.Source);
            return;
        }

        if (button.DataContext is IImagePickerViewModel viewModel)
        {
            await viewModel.PickImageAsync(this, button.Tag is "New");
        }
    }

    private void ShowFullScreenImage(IImage source)
    {
        var previewWindow = new Window
        {
            Title = "Image preview",
            Width = 1200,
            Height = 800,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ShowInTaskbar = false,
            CanResize = false,
            WindowDecorations = global::Avalonia.Controls.WindowDecorations.None,
            Background = Brushes.Black,
            Content = new Border
            {
                Background = Brushes.Black,
                Child = new Image
                {
                    Source = source,
                    Stretch = Stretch.Uniform,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            }
        };

        previewWindow.WindowState = WindowState.FullScreen;

        previewWindow.KeyDown += (_, keyArgs) =>
        {
            if (keyArgs.Key == Key.Escape)
            {
                previewWindow.Close();
            }
        };

        var imageControl = (Image)((Border)previewWindow.Content!).Child!;
        imageControl.PointerPressed += (_, _) => previewWindow.Close();

        previewWindow.Show(this);
    }

    private void ItemTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.Source != sender
            || sender is not TabControl { SelectedItem: TabItem selectedTab })
        {
            return;
        }

        LoadSelectedTab(selectedTab);
        UpdateSelectedGridCount(selectedTab);
    }

    private void OnTabSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.Source is TabControl { SelectedItem: TabItem selectedTab })
        {
            LoadSelectedTab(selectedTab);
            UpdateSelectedGridCount(selectedTab);
        }
    }

    private void LoadSelectedTab(TabItem selectedTab)
    {
        if (selectedTab.Header?.ToString() == "My"
            && selectedTab.Content is TabControl myTabs
            && DataContext is MainWindowViewModel mainViewModel)
        {
            var selectedMyTab = myTabs.SelectedItem as TabItem
                ?? myTabs.Items.OfType<TabItem>().FirstOrDefault();
            var selectedMyTabHeader = selectedMyTab?.Header?.ToString();

            switch (selectedMyTabHeader)
            {
                case "D&D":
                    mainViewModel.DnDViewModel.LoadDefaultGridOnSelection();
                    break;
                case "People":
                    mainViewModel.PeopleViewModel.LoadGridOnSelection("Birthdays");
                    break;
                case "Adventures":
                    mainViewModel.AdventuresViewModel.LoadDefaultGridOnSelection();
                    break;
                default:
                    mainViewModel.WorkViewModel.LoadDefaultGridOnSelection();
                    break;
            }

            return;
        }

        if (selectedTab.Content is TabControl tabControl)
        {
            var childTab = tabControl.SelectedItem as TabItem
                ?? tabControl.Items.OfType<TabItem>().FirstOrDefault();
            if (childTab is not null)
            {
                LoadSelectedTab(childTab);
            }

            return;
        }

        if (selectedTab.Content is not Control content)
        {
            return;
        }

        if (selectedTab.Content is UserControl
            && content.DataContext is ILoadGridOnSelection defaultGridViewModel)
        {
            defaultGridViewModel.LoadDefaultGridOnSelection();
            return;
        }

        var nestedTabControl = content.GetVisualDescendants().OfType<TabControl>().FirstOrDefault();
        if (nestedTabControl?.SelectedItem is TabItem nestedTab)
        {
            LoadSelectedTab(nestedTab);
            return;
        }

        if (content.DataContext is ILoadGridOnSelection gridViewModel)
        {
            if (selectedTab.Content is UserControl)
            {
                gridViewModel.LoadDefaultGridOnSelection();
            }
            else
            {
                gridViewModel.LoadGridOnSelection(selectedTab.Header?.ToString());
            }
        }
        else if (content.DataContext is ILoadOnSelection viewModel)
        {
            viewModel.LoadOnSelection();
        }
    }

    private static void UpdateSelectedGridCount(TabItem? selectedTab)
    {
        if (selectedTab?.Content is not Control content)
        {
            return;
        }

        var gridFilter = content.GetVisualDescendants().OfType<GridFilterView>().FirstOrDefault();
        var dataGrid = content.GetVisualDescendants().OfType<DataGrid>().FirstOrDefault();

        if (gridFilter?.DataContext is GridFilterViewModel filterViewModel && dataGrid is not null)
        {
            filterViewModel.GridCountItems = dataGrid.ItemsSource switch
            {
                ICollection collection => collection.Count,
                IEnumerable items => items.Cast<object>().Count(),
                _ => 0
            };
        }
    }
}
