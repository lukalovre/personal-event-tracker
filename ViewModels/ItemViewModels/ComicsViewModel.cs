using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class ComicsViewModel(IDatasource datasource, IExternal<Comic> external) : ItemViewModel<Comic, ComicGridItem>(datasource, external)
{
    protected override ComicGridItem Convert(Event e, Comic i, IEnumerable<Event> eventList)
    {
        return new ComicGridItem(
            i.ID,
            i.Title,
            i.Writer,
            e.Chapter,
            GetItemAmount(eventList),
            e.Rating,
            eventList.LastEventDate()
        );
    }

}
