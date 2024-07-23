using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class ZooViewModel(IDatasource datasource) : ItemViewModel<Zoo, ZooGridItem>(datasource, null!)
{
    protected override ZooGridItem Convert(Event e, Zoo i, IEnumerable<Event> eventList)
    {
        return new ZooGridItem(
            i.ID,
            i.Title,
            i.City,
            i.Country,
            eventList.LastEventDate().Date.Year);
    }
}
