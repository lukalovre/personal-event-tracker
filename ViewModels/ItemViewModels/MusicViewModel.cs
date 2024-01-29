using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using AvaloniaApplication1.ViewModels.GridItems;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class MusicViewModel(IDatasource datasource, IExternal<Music> external)
: ItemViewModel<Music, MusicGridItem>(datasource, external)
{
    private int _gridCountMusicTodo1;
    private int _gridCountMusicTodo2;

    public override ObservableCollection<string> PlatformTypes =>
        new(
            Enum.GetValues(typeof(eMusicPlatformType))
                .Cast<eMusicPlatformType>()
                .Select(v => v.ToString())
        );
    public ObservableCollection<MusicGridItem> MusicTodo2 { get; set; }
    public ObservableCollection<MusicGridItem> MusicTodo1 { get; set; }
    public ObservableCollection<MusicGridItem> ArtistMusic { get; set; }

    protected override string OpenLinkUrl => SelectedItem.SpotifyID;
    protected override List<string> GetAlternativeOpenLinkSearchParams()
    {
        var openLinkParams = SelectedItem.Artist.Split(' ').ToList();
        openLinkParams.AddRange(SelectedItem.Title.Split(' '));
        openLinkParams.AddRange([SelectedItem.Year.ToString()]);

        return openLinkParams;
    }

    public int GridCountMusicTodo1
    {
        get => _gridCountMusicTodo1;
        private set => this.RaiseAndSetIfChanged(ref _gridCountMusicTodo1, value);
    }

    public int GridCountMusicTodo2
    {
        get => _gridCountMusicTodo2;
        private set => this.RaiseAndSetIfChanged(ref _gridCountMusicTodo2, value);
    }

    protected override DateTime? DateTimeFilter => DateTime.Now.AddHours(-24);

    protected override void ReloadData()
    {
        base.ReloadData();

        MusicTodo1 ??= [];
        MusicTodo1.Clear();
        MusicTodo1.AddRange(LoadDataBookmarked(1));
        GridCountMusicTodo1 = MusicTodo1.Count;

        MusicTodo2 ??= [];
        MusicTodo2.Clear();
        MusicTodo2.AddRange(LoadDataBookmarked(2));
        GridCountMusicTodo2 = MusicTodo2.Count;
    }

    protected override MusicGridItem Convert(int index, Event e, Music i, IEnumerable<Event> eventList)
    {
        return new MusicGridItem(
            i.ID,
            index + 1,
            i.Artist,
            i.Title,
            i.Year,
            i.Runtime,
            e.Bookmakred,
            eventList.Count()
        );
    }
}