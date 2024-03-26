using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class ClipsViewModel(IDatasource datasource, IExternal<Clip> external) : ItemViewModel<Clip, ClipGridItem>(datasource, external)
{
    protected override ClipGridItem Convert(Event e, Clip i, IEnumerable<Event> eventList)
    {
        return new ClipGridItem(i.ID, i.Author, i.Title);
    }
}
