using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class AdventuresViewModel(IDatasource datasource) : ItemViewModel<Adventure, AdventureGridItem>(datasource, null!)
{
    protected override AdventureGridItem Convert(Event e, Adventure i, IEnumerable<Event> eventList)
    {
        return new AdventureGridItem(
            i.ID,
            i.Title,
            i.City,
            eventList.LastEventDate().Date.Year);
    }
}
