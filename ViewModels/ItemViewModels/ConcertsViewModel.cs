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
            i.Artist,
            i.Festival,
            i.Venue,
            i.City,
            i.Country,
            i.Price,
            eventList.LastEventDate().Year,
            eventList.LastEventDate());
    }
}