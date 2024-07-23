using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media.Imaging;
using AvaloniaApplication1.Models;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class PersonEventsViewModel : ViewModelBase
{

    private Event _selectedEvent = null!;
    private PersonEventGridItem _selectedGridItem;
    private Bitmap? _itemImage;

    public ObservableCollection<PersonEventGridItem> Events { get; set; } = [];

    public Bitmap? Image
    {
        get => _itemImage;
        private set => this.RaiseAndSetIfChanged(ref _itemImage, value);
    }

    private readonly IDatasource _datasource;

    public PersonEventsViewModel(IDatasource datasource, ObservableCollection<string> platformTypes)
    {
        _datasource = datasource;
    }

    public PersonEventGridItem SelectedGridItem
    {
        get => _selectedGridItem;
        set
        {
            _selectedGridItem = value;
            SelectedItemChanged();
        }
    }

    private void SelectedItemChanged()
    {
        Image = null;

        if (SelectedGridItem == null)
        {
            return;
        }

        Image = FileRepsitory.GetImage<Movie>(SelectedGridItem.ID);
    }

    private List<PersonEventGridItem> LoadEvents(int id)
    {
        var peopleEventList = new List<Event>();
        var peopleEventGridList = new List<PersonEventGridItem>();

        var eventList = _datasource.GetEventList<Movie>();
        var itemList = _datasource.GetList<Movie>();

        foreach (var e in eventList)
        {
            if (string.IsNullOrWhiteSpace(e.People))
            {
                continue;
            }

            var item = itemList.First(o => o.ID == e.ItemID);
            var peopleIDList = e.People.Split(',').Select(o => int.Parse(o));

            foreach (var perosnID in peopleIDList)
            {
                if (perosnID != id)
                {
                    continue;
                }

                var gridItem = new PersonEventGridItem(item.ID, nameof(Movie), item.Title, e.DateEnd);
                peopleEventGridList.Add(gridItem);
            }
        }

        return peopleEventGridList.OrderByDescending(o => o.Date).ToList();
    }

    internal void LoadData(int id)
    {
        Events.Clear();
        Events.AddRange(LoadEvents(id));
    }
}
