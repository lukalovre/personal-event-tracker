using System.Collections.Generic;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class BooksViewModel(IDatasource datasource, IExternal<Book> external)
: ItemViewModel<Book, BookGridItem>(datasource, external)
{
    protected override float AmountToMinutesModifier => 2f;
    protected override bool IsFullAmountDefaultValue => false;
    protected override string AmountVerb => "pages";

    protected override BookGridItem Convert(int index, Event e, Book i, IEnumerable<Event> eventList)
    {
        return new BookGridItem(
            i.ID,
            index + 1,
            i.Title,
            i.Author,
            i.Year,
            e.Rating,
            GetItemAmount(eventList),
            e.DateEnd
        );
    }
}
