using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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

    private List<InfoModel> GetSelectedEventInfo<T>()
    {
        var result = new List<InfoModel>();

        var properties = typeof(T).GetProperties();

        foreach (PropertyInfo property in properties)
        {
            var e = Events.ToList().Last();

            var value = property.GetValue(e);
            result.Add(new InfoModel(property.Name, value));
        }

        return result;
    }
}
