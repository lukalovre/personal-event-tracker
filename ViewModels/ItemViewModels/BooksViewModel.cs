using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class BooksViewModel(IDatasource datasource, IExternal<Book> external)
: ItemViewModel<Book, BookGridItem>(datasource, external)
{
    protected override float AmountToMinutesModifier => 2f;
    protected override bool IsFullAmountDefaultValue => false;
    protected override string AmountVerb => "pages";
    protected override int? DefaultNewItemChapter => null;

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
