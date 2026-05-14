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

public partial class BooksViewModel(IDatasource datasource, IExternal<Book> external)
: ItemViewModel<Book, BookGridItem>(datasource, external)
{
    public ObservableCollection<AuthorGridItem> BookAuthorList { get; set; } = [];

    protected override BookGridItem Convert(Event e, Book i, IEnumerable<Event> eventList)
    {
        return new BookGridItem(
            i.ID,
            i.Title,
            i.Author,
            i.Year,
            e.Rating,
            e.Completed,
            GetItemAmount(eventList),
            eventList.LastEventDate()
        );
    }

    protected override async void ReloadData()
    {
        base.ReloadData();

        BookAuthorList.Clear();
        BookAuthorList.AddRange(await LoadDataByAuthor());
    }

    private async Task<List<AuthorGridItem>> LoadDataByAuthor()
    {
        var resultGrid = new List<AuthorGridItem>();

        var type = Helpers.GetClassName<Book>();
        var itemList = _datasource.GetList<Book>(type);
        var eventList = _datasource.GetEventList(type);

        var authorList = itemList.DistinctBy(o => o.Author).Select(o => o.Author);

        foreach (var author in authorList)
        {
            var bookList = itemList.Where(o => o.Author == author);
            var pagesAuthor = 0;

            foreach (var book in bookList)
            {
                var pagesBook = eventList.Where(o => o.ItemID == book.ID).Sum(o => o.Amount);
                pagesAuthor += pagesBook;
            }

            var gridItem = new AuthorGridItem(1, author, pagesAuthor, bookList.Count());
            resultGrid.Add(gridItem);
        }

        resultGrid = resultGrid.OrderByDescending(o => o.Pages).ToList();
        return resultGrid;
    }
}
