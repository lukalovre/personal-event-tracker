using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using EventTracker.ViewModels;
using EventTracker.Views;
using Repositories;

namespace EventTracker;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(new TsvDatasource()),
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Position = new PixelPoint(0, 1000)
            };

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                desktop.MainWindow.WindowState = WindowState.Maximized;
            }, DispatcherPriority.Background);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
