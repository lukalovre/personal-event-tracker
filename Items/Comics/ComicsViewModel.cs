using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using EventTracker.Models;
using EventTracker.Repositories;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class ComicsViewModel(IDatasource datasource, IExternal<Comic> external) : ItemViewModel<Comic, ComicGridItem>(datasource, external)
{
    public ObservableCollection<ComicGridItem> ComicsList { get; set; } = [];

    protected override string GetOpenLink()
    {
        return SelectedItem?.ExternalID ?? string.Empty;
    }

    protected override List<string> GetAlternativeOpenLinkSearchParams()
    {
        var searchParams = SelectedItem.Title.Split(' ').ToList();
        searchParams.AddRange(SelectedItem.Writer.Split(' '));
        searchParams.Add(SelectedItem.Year.ToString());
        return searchParams;
    }

    protected override ComicGridItem Convert(Event e, Comic i, IEnumerable<Event> eventList)
    {
        return new ComicGridItem(
            i.ID,
            i.Title,
            i.Writer,
            e.Chapter,
            GetItemAmount(eventList),
            e.Rating,
            eventList.LastEventDate()
        );
    }

    protected override async void ReloadData()
    {
        base.ReloadData();

        ComicsList.Clear();
        ComicsList.AddRange(await LoadDataByComic());
    }

    protected override async Task LoadGridTabData(string? tabHeader)
    {
        if (tabHeader == "Comics")
        {
            LoadItemsAndEvents(out _, out _, realLoad: true);
            ComicsList.Clear();
            ComicsList.AddRange(await LoadDataByComic());
            return;
        }

        await base.LoadGridTabData(tabHeader);
    }

    private async Task<List<ComicGridItem>> LoadDataByComic()
    {
        var resultGrid = new List<ComicGridItem>();

        LoadItemsAndEvents(out List<Comic> itemList, out List<Event> eventList);

        foreach (var comic in itemList)
        {
            var pagesPerComic = eventList.Where(o => o.ItemID == comic.ID).Sum(o => o.Amount);
            var gridItem = new ComicGridItem(comic.ID, comic.Title, comic.Writer, null, pagesPerComic, null, DateTime.MinValue);
            resultGrid.Add(gridItem);
        }

        resultGrid = resultGrid.OrderByDescending(o => o.Pages).ToList();
        return resultGrid;
    }
}
