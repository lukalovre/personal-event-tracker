using System.Collections.Generic;
using System.Linq;
using EventTracker.Models;
using EventTracker.Repositories;
using Repositories;

namespace EventTracker.ViewModels;

public partial class ClipsViewModel(IDatasource datasource, IExternal<Clip> external) : ItemViewModel<Clip, ClipGridItem>(datasource, external)
{
    protected override string GetOpenLink()
    {
        return SelectedItem?.ExternalID ?? string.Empty;
    }

    protected override List<string> GetAlternativeOpenLinkSearchParams()
    {
        var searchParams = SelectedItem.Title.Split(' ').ToList();
        searchParams.AddRange(SelectedItem.Author.Split(' '));
        searchParams.Add(SelectedItem.Year.ToString());
        return searchParams;
    }

    protected override ClipGridItem Convert(Event e, Clip i, IEnumerable<Event> eventList)
    {
        return new ClipGridItem(i.ID, i.Author, i.Title, e?.Rating ?? 1);
    }
}
