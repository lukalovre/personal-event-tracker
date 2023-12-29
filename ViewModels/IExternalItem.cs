using System.Reactive;
using ReactiveUI;

namespace AvaloniaApplication1.ViewModels;

public interface IExternalItem
{
    string InputUrl { get; set; }
    void InputUrlChanged();
    ReactiveCommand<Unit, Unit> OpenLink { get; }
    void OpenLinkAction();
}