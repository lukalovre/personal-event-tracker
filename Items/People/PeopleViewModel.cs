using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using EventTracker.Models;
using EventTracker.Repositories;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace EventTracker.ViewModels;

public partial class PeopleViewModel : ItemViewModel<Person, PersonGridItem>, IDataGrid
{
    private PersonGridItem _selectedPersonGridItem;
    private BirthdayGridItem _selectedBirthdayGridItem = null!;
    private string _selectedExistingTag = string.Empty;

    public PeopleViewModel(IDatasource datasource) : base(datasource, null!)
    {
        PersonEventsViewModel = new PersonEventsViewModel(datasource, null);
        AddTag = ReactiveCommand.Create<string, Unit>(AddTagAction);
    }

    public PersonEventsViewModel PersonEventsViewModel { get; }
    public ObservableCollection<PersonGridItem> PeopleGrid { get; set; } = [];
    public ObservableCollection<BirthdayGridItem> BirthdaysGrid { get; set; } = [];
    public ObservableCollection<PersonGridItem> MissingBirthdaysGrid { get; set; } = [];
    public ObservableCollection<PersonGridItem> ActivePeopleGrid { get; set; } = [];
    public ObservableCollection<string> ExistingTags { get; set; } = [];
    public ReactiveCommand<string, Unit> AddTag { get; }

    public string SelectedExistingTag
    {
        get => _selectedExistingTag;
        set => this.RaiseAndSetIfChanged(ref _selectedExistingTag, value);
    }

    protected override void ReloadData()
    {
        base.ReloadData();

        PeopleGrid.Clear();
        PeopleGrid.AddRange(LoadPeople());
        ExistingTags.Clear();
        ExistingTags.AddRange(LoadExistingTags());
        BirthdaysGrid.Clear();
        BirthdaysGrid.AddRange(LoadBirthdays());
        MissingBirthdaysGrid.Clear();
        MissingBirthdaysGrid.AddRange(LoadPeople().Where(person => string.IsNullOrWhiteSpace(person.Birthday)));
        ActivePeopleGrid.Clear();
        ActivePeopleGrid.AddRange(LoadPeople().Where(person => HasTag(person.Tags, "Active")));
    }

    private List<PersonGridItem> LoadPeople()
    {
        var itemList = _datasource.GetList<Person>(Helpers.GetClassName<Person>());
        var searchText = GridFilterViewModel.SearchText?.Trim() ?? string.Empty;

        return itemList
            .Where(person => string.IsNullOrWhiteSpace(searchText)
                || MatchesSearch(person.FirstName, searchText)
                || MatchesSearch(person.LastName, searchText)
                || MatchesSearch(person.Nickname, searchText))
            .Select(o => Convert(null!, o, null!))
            .ToList();
    }

    private List<string> LoadExistingTags()
    {
        return _datasource.GetList<Person>(Helpers.GetClassName<Person>())
            .SelectMany(person => person.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private Unit AddTagAction(string target)
    {
        if (string.IsNullOrWhiteSpace(SelectedExistingTag))
        {
            return Unit.Default;
        }

        var person = target == "New" ? NewItem : SelectedItem;
        if (person is not Person selectedPerson)
        {
            return Unit.Default;
        }

        var tags = selectedPerson.Tags
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        if (tags.Any(tag => string.Equals(tag, SelectedExistingTag, StringComparison.OrdinalIgnoreCase)))
        {
            return Unit.Default;
        }

        var updatedPerson = selectedPerson with { Tags = string.Join(", ", tags.Append(SelectedExistingTag.Trim())) };
        if (target == "New")
        {
            NewItem = (Person)(object)updatedPerson;
        }
        else
        {
            SelectedItem = (Person)(object)updatedPerson;
        }

        return Unit.Default;
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

    private static bool HasTag(string tags, string tag)
    {
        return tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(value => string.Equals(value, tag, StringComparison.OrdinalIgnoreCase));
    }

    protected override PersonGridItem Convert(Event e, Person i, IEnumerable<Event> eventList)
    {
        return new PersonGridItem(
            i.ID,
            i.FirstName,
            i.LastName,
            i.Nickname,
            i.Birthday,
            i.Tags);
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
        ExistingTags.Clear();
        ExistingTags.AddRange(LoadExistingTags());
        BirthdaysGrid.Clear();
        BirthdaysGrid.AddRange(LoadBirthdays());
        MissingBirthdaysGrid.Clear();
        MissingBirthdaysGrid.AddRange(LoadPeople().Where(person => string.IsNullOrWhiteSpace(person.Birthday)));
        ActivePeopleGrid.Clear();
        ActivePeopleGrid.AddRange(LoadPeople().Where(person => HasTag(person.Tags, "Active")));
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