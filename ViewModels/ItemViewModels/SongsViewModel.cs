using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class SongsViewModel(IDatasource datasource, IExternal<Song> external)
: ItemViewModel<Song, SongGridItem>(datasource, external)
{
    public override ObservableCollection<string> PlatformTypes =>
    new(
        Enum.GetValues(typeof(eMusicPlatformType))
            .Cast<eMusicPlatformType>()
            .Select(v => v.ToString())
    );

    protected override string DefaultNewItemPlatform => eMusicPlatformType.Streaming.ToString();
    protected override int DefaultNewItemRating => 3;
    protected override bool DefaultNewItemCompleted => true;
    protected override bool DefaultNewItemBookmakred => true;
    protected override int? DefaultNewItemChapter => null;
    protected override int? NewItemAmountOverride => NewItem.Runtime;
    protected override string OpenLinkUrl => SelectedItem.Link;
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
