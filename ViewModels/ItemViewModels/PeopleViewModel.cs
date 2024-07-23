using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AvaloniaApplication1.Models;
using DynamicData;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class PeopleViewModel(IDatasource datasource) : ItemViewModel<Person, PersonGridItem>(datasource, null!)
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
        var itemList = datasource.GetList<Person>();
        return itemList.Select(o => Convert(null!, o, null!)).ToList();
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
        }
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