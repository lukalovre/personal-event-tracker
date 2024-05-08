using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class WorkViewModel(IDatasource datasource) : ItemViewModel<Work, WorkGridItem>(datasource, null!)
{
    protected override WorkGridItem Convert(Event e, Work i, IEnumerable<Event> eventList)
    {
        return new WorkGridItem(
            i.ID,
            i.Title,
            i.Type,
            eventList.Sum(o => o.Amount),
            eventList.LastEventDate());
    }
}
