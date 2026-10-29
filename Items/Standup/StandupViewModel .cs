using System;
using System.Collections.Generic;
using System.Linq;
using EventTracker.Models;
using EventTracker.Repositories;
using Repositories;

namespace EventTracker.ViewModels;

public partial class StandupViewModel(IDatasource datasource, IExternal<Standup> external) : ItemViewModel<Standup, StandupGridItem>(datasource, external)
{
    protected override string GetOpenLink()
    {
        var link = SelectedItem?.Link;
        if (string.IsNullOrWhiteSpace(link))
        {
            return string.Empty;
        }

        return Uri.TryCreate(link, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                ? link
                : $"https://www.imdb.com/title/{link}";
    }

    protected override List<string> GetAlternativeOpenLinkSearchParams()
    {
        var searchParams = SelectedItem.Performer.Split(' ').ToList();
        searchParams.AddRange(SelectedItem.Title.Split(' '));
        searchParams.Add(SelectedItem.Year.ToString());
        return searchParams;
    }

    protected override StandupGridItem Convert(Event e, Standup i, IEnumerable<Event> eventList)
    {
        return new StandupGridItem(
            i.ID,
            i.Title,
            i.Performer,
            i.Year,
            e?.Rating ?? 0
        );
    }

}
