using System.Collections.Generic;
using EventTracker.Models;
using EventTracker.Repositories;
using EventTracker.ViewModels.Extensions;
using Repositories;

namespace EventTracker.ViewModels;

public partial class BooksViewModel(IDatasource datasource, IExternal<Book> external) : ItemViewModel<Book, BookGridItem>(datasource, external)
{
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
}
