using System.Collections.ObjectModel;
using ReactiveUI;

namespace AvaloniaApplication1.ViewModels;

public partial class EventViewModel : ViewModelBase
{
    private Event selectedEvent;

    public ObservableCollection<Event> Events { get; set; }

    public Event SelectedEvent
    {
        get => selectedEvent;
        set => this.RaiseAndSetIfChanged(ref selectedEvent, value);
    }

    public EventViewModel(ObservableCollection<Event> events)
    {
        Events = events;
    }
}
