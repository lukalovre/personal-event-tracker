using System;
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
    private BirthdayGridItem _selectedBirthdayGridItem = null!;

    public PersonEventsViewModel PersonEventsViewModel { get; } = new PersonEventsViewModel(datasource, null);
    public ObservableCollection<PersonGridItem> PeopleGrid { get; set; } = [];
    public ObservableCollection<BirthdayGridItem> BirthdaysGrid { get; set; } = [];
    public ObservableCollection<PersonGridItem> MissingBirthdaysGrid { get; set; } = [];

    protected override void ReloadData()
    {
        base.ReloadData();

        PeopleGrid.Clear();
        PeopleGrid.AddRange(LoadPeople());
        BirthdaysGrid.Clear();
        BirthdaysGrid.AddRange(LoadBirthdays());
        MissingBirthdaysGrid.Clear();
        MissingBirthdaysGrid.AddRange(LoadPeople().Where(person => string.IsNullOrWhiteSpace(person.Birthday)));
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

    private List<BirthdayGridItem> LoadBirthdays()
    {
        var today = DateTime.Today;

        return LoadPeople()
            .Select(person => new
            {
                Person = person,
                Birthday = ParseBirthday(person.Birthday)
            })
            .Where(item => item.Birthday.HasValue
                && (item.Birthday.Value.Month > today.Month
                    || item.Birthday.Value.Month == today.Month && item.Birthday.Value.Day >= today.Day))
            .OrderBy(item => item.Birthday!.Value.Month)
            .ThenBy(item => item.Birthday!.Value.Day)
            .Select(item => new BirthdayGridItem(
                item.Person.ID,
                item.Person.FirstName,
                item.Person.LastName,
                item.Birthday!.Value.ToString("MMMM, d", CultureInfo.InvariantCulture),
                today.Year - item.Birthday.Value.Year))
            .ToList();
    }

    private static DateTime? ParseBirthday(string birthday)
    {
        if (DateTime.TryParse(birthday, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var result)
            || DateTime.TryParse(birthday, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out result))
        {
            return result;
        }

        return null;
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
            i.Nickname,
            i.Birthday);
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

    public BirthdayGridItem SelectedBirthdayGridItem
    {
        get => _selectedBirthdayGridItem;
        set
        {
            _selectedBirthdayGridItem = value;
            var person = PeopleGrid.FirstOrDefault(item => item.ID == value.ID);
            if (person is not null)
            {
                SelectedPersonGridItem = person;
            }
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
        BirthdaysGrid.Clear();
        BirthdaysGrid.AddRange(LoadBirthdays());
        MissingBirthdaysGrid.Clear();
        MissingBirthdaysGrid.AddRange(LoadPeople().Where(person => string.IsNullOrWhiteSpace(person.Birthday)));
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