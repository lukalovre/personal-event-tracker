using System.Collections.Generic;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	public string Greeting => "Welcome to Avalonia!";

	public List<string> Movies => new() { "1", "2", "3" };
}