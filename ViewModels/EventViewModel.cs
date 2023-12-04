using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
        Events.CollectionChanged += CollectionChanged;
    }

    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var events = sender as ObservableCollection<Event>;
        SelectedEvent = events.MaxBy(o => o.DateEnd);
    }
}
