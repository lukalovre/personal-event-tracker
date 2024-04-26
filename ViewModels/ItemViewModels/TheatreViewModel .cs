using System;
using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class TheatreViewModel(IDatasource datasource) : ItemViewModel<Theatre, TheatreGridItem>(datasource, null!)
{
    protected override int? NewItemAmountOverride => NewItem.Runtime;
    protected override int? DefaultNewItemChapter => null;
    protected override bool DefaultNewItemCompleted => true;
    protected override DateTime? DateTimeFilter => DateTime.MinValue;

    protected override TheatreGridItem Convert(Event e, Theatre i, IEnumerable<Event> eventList)
    {
        return new TheatreGridItem(
            i.ID,
            i.Title,
            i.Writer,
            i.Director,
            e?.Rating ?? 0,
            eventList.LastEventDate()
        );
    }
}
