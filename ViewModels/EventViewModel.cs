using System.Collections.ObjectModel;

namespace AvaloniaApplication1.ViewModels;

public partial class EventViewModel : ViewModelBase
{
    public ObservableCollection<InfoModel> Info { get; set; }

    public ObservableCollection<Event> Events { get; set; }

    public EventViewModel(ObservableCollection<InfoModel> info, ObservableCollection<Event> events)
    {
        Info = info;
        Events = events;
    }
}
