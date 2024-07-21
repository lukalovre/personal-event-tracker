using System.Collections.Generic;
using AvaloniaApplication1.Models;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class PeopleViewModel(IDatasource datasource) : ItemViewModel<Person, PersonGridItem>(datasource, null!)
{
    protected override PersonGridItem Convert(Event e, Person i, IEnumerable<Event> eventList)
    {
        return new PersonGridItem(
            i.ID,
            i.FirstName,
            i.LastName,
            i.Nickname);
    }
}