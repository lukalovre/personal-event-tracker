using System.Collections.Generic;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	public string Title => "Data";

	public List<string> Movies { get; set; }
}