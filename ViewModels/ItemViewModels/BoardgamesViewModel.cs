using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class BoardgamesViewModel(IDatasource datasource) : ItemViewModel<Boardgame, BoardgamesGridItem>(datasource, null!)
{
    protected override BoardgamesGridItem Convert(Event e, Boardgame i, IEnumerable<Event> eventList)
    {
        return new BoardgamesGridItem(
            i.ID,
            i.Title,
            eventList.Last().Chapter ?? 0,
            eventList.LastEventDate());
    }
}