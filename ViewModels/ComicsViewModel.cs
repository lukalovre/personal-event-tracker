using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class ComicsViewModel(IDatasource datasource, IExternal<Comic> external)
: ItemViewModel<Comic, ComicGridItem>(datasource, external)
{
    public override float AmountToMinutesModifier => 0.3f;

    public override ComicGridItem Convert(int index, Event e, Comic i, IEnumerable<Event> eventList)
    {
        return new ComicGridItem(
            i.ID,
            index + 1,
            i.Title,
            i.Writer,
            e.Chapter,
            GetItemAmount(eventList),
            e.Rating
        );
    }

}
