using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using EventTracker.Models;
using EventTracker.Repositories;
using Repositories;

namespace EventTracker.ViewModels;

public partial class SongsViewModel(IDatasource datasource, IExternal<Song> external) : ItemViewModel<Song, SongGridItem>(datasource, external)
{
    protected override List<string> GetAlternativeOpenLinkSearchParams()
    {
        var openLinkParams = SelectedItem.Artist.Split(' ').ToList();
        openLinkParams.AddRange(SelectedItem.Title.Split(' '));
        openLinkParams.AddRange([SelectedItem.Year.ToString()]);

        return openLinkParams;
    }

    protected override SongGridItem Convert(Event e, Song i, IEnumerable<Event> eventList)
    {
        return new SongGridItem(
            i.ID,
            i.Artist,
            i.Title,
            i.Year,
            eventList.Count(),
            e.Bookmakred);
    }
}
