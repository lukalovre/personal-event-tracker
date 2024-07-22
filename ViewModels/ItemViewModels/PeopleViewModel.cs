using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AvaloniaApplication1.Models;
using DynamicData;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class PeopleViewModel(IDatasource datasource) : ItemViewModel<Person, PersonGridItem>(datasource, null!)
{

    public PersonEventsViewModel PersonEventsViewModel { get; } = new PersonEventsViewModel(null, null);
    public ObservableCollection<PersonGridItem> Movies { get; set; } = [];
    public ObservableCollection<PersonGridItem> PeopleGrid { get; set; } = [];

    protected override void ReloadData()
    {
        base.ReloadData();

        PeopleGrid.Clear();
        PeopleGrid.AddRange(LoadPeople());

        Movies.Clear();
        Movies.AddRange(LoadMovies());
    }

    private List<PersonGridItem> LoadPeople()
    {
        var itemList = datasource.GetList<Person>();
        return itemList.Select(o => Convert(null!, o, null!)).ToList();
    }

    private List<PersonGridItem> LoadMovies()
    {
        var itemList = datasource.GetList<Person>();

        var peopleEventList = new List<Event>();

        var eventList = datasource.GetEventList<Movie>();

        foreach (var e in eventList)
        {
            if (string.IsNullOrWhiteSpace(e.People))
            {
                continue;
            }

            var peopleIDList = e.People.Split(',').Select(o => int.Parse(o));

            foreach (var perosnID in peopleIDList)
            {
                e.ItemID = perosnID;
                peopleEventList.Add(e);
            }
        }

        _eventList = peopleEventList;

        return peopleEventList
            .OrderByDescending(o => o.DateEnd)
            .DistinctBy(o => o.ItemID)
            .OrderBy(o => o.DateEnd)
            .Select(
                o =>
                    Convert(
                        o,
                        itemList.First(m => m.ID == o.ItemID),
                        eventList.Where(e => e.ItemID == o.ItemID)
                    )
            )
            .ToList();
    }

    protected override PersonGridItem Convert(Event e, Person i, IEnumerable<Event> eventList)
    {
        return new PersonGridItem(
            i.ID,
            i.FirstName,
            i.LastName,
            i.Nickname);
    }
}