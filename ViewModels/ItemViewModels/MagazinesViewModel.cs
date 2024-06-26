using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class MagazinesViewModel(IDatasource datasource) : ItemViewModel<Magazine, MagazinesGridItem>(datasource, null!)
{
    protected override MagazinesGridItem Convert(Event e, Magazine i, IEnumerable<Event> eventList)
    {
        return new MagazinesGridItem(
            i.ID,
            i.Title,
            eventList.Last().Chapter ?? 0,
            eventList.LastEventDate());
    }
}