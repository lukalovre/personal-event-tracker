using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class PaintingsViewModel(IDatasource datasource) : ItemViewModel<Painting, PaintingsGridItem>(datasource, null!)
{
    protected override PaintingsGridItem Convert(Event e, Painting i, IEnumerable<Event> eventList)
    {
        return new PaintingsGridItem(
            i.ID,
            i.Title,
            i.Author,
            i.Year,
            eventList.LastEventDate());
    }
}