using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class ConcertsViewModel(IDatasource datasource) : ItemViewModel<Concert, ConcertsGridItem>(datasource, null!)
{
    protected override ConcertsGridItem Convert(Event e, Concert i, IEnumerable<Event> eventList)
    {
        return new ConcertsGridItem(
            i.ID,
            i.Title,
            i.Year,
            eventList.LastEventDate());
    }
}