using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class ClassicalViewModel(IDatasource datasource) : ItemViewModel<Classical, ClassicalGridItem>(datasource, null!)
{
    protected override ClassicalGridItem Convert(Event e, Classical i, IEnumerable<Event> eventList)
    {
        return new ClassicalGridItem(
            i.ID,
            i.Title,
            eventList.LastEventDate());
    }
}