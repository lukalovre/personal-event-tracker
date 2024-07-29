using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class DnDViewModel(IDatasource datasource) : ItemViewModel<DnD, DnDGridItem>(datasource, null!)
{
    protected override DnDGridItem Convert(Event e, DnD i, IEnumerable<Event> eventList)
    {
        return new DnDGridItem(
            i.ID,
            i.Title,
            eventList.LastEventDate());
    }
}