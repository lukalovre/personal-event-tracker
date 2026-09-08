using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using EventTracker.Models;
using EventTracker.Repositories;
using DynamicData;
using Repositories;

namespace EventTracker.ViewModels;

public partial class PeopleViewModel(IDatasource datasource) : ItemViewModel<Person, PersonGridItem>(datasource, null!), IDataGrid
{
    private PersonGridItem _selectedPersonGridItem;

    public PersonEventsViewModel PersonEventsViewModel { get; } = new PersonEventsViewModel(datasource, null);
    public ObservableCollection<PersonGridItem> PeopleGrid { get; set; } = [];

    protected override void ReloadData()
    {
        base.ReloadData();

        PeopleGrid.Clear();
        PeopleGrid.AddRange(LoadPeople());
    }

    private List<PersonGridItem> LoadPeople()
    {
        var itemList = datasource.GetList<Person>(Helpers.GetClassName<Person>());
        var searchText = GridFilterViewModel.SearchText?.Trim() ?? string.Empty;

        return itemList
            .Where(person => string.IsNullOrWhiteSpace(searchText)
                || MatchesSearch(person.FirstName, searchText)
                || MatchesSearch(person.LastName, searchText)
                || MatchesSearch(person.Nickname, searchText))
            .Select(o => Convert(null!, o, null!))
            .ToList();
    }

    private static bool MatchesSearch(string? value, string searchText)
    {
        return value is not null
            && CultureInfo.InvariantCulture.CompareInfo.IndexOf(
                value,
                searchText,
                CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;
    }

    protected override PersonGridItem Convert(Event e, Person i, IEnumerable<Event> eventList)
    {
        return new PersonGridItem(
            i.ID,
            i.FirstName,
            i.LastName,
            i.Nickname);
    }

    public PersonGridItem SelectedPersonGridItem
    {
        get => _selectedPersonGridItem;
        set
        {
            _selectedPersonGridItem = value;
            SelectedPersonChanged();
            SelectedGridItem = value;
        }
    }

    public override void SelectFirstItem()
    {
        if (PeopleGrid.Count > 0)
        {
            SelectedPersonGridItem = PeopleGrid[0];
        }
    }

    int IDataGrid.ReloadData()
    {
        PeopleGrid.Clear();
        PeopleGrid.AddRange(LoadPeople());
        return PeopleGrid.Count;
    }

    public void SelectedPersonChanged()
    {
        if (SelectedPersonGridItem == null)
        {
            return;
        }

        PersonEventsViewModel.LoadData(SelectedPersonGridItem.ID);
    }
}