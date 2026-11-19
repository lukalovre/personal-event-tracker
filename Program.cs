using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace EventTracker;

internal sealed class Program
{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args) => BuildAvaloniaApp()
		.StartWithClassicDesktopLifetime(args);

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.UseReactiveUI();
}

// Make the Info/New/Edit item tabs be top level till the top of the screen
// Have a separating line between item info and the grid, the color of the app 
// People list also to load only once
