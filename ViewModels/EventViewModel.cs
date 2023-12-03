using System.Collections.ObjectModel;

namespace AvaloniaApplication1.ViewModels;

public partial class EventViewModel : ViewModelBase
{
    public ObservableCollection<InfoModel> Info { get; set; }

    public EventViewModel(ObservableCollection<InfoModel> info)
    {
        Info = info;
    }
}
