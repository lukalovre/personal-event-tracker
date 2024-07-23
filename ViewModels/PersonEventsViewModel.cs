using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media.Imaging;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Models.Interfaces;
using AvaloniaApplication1.Repositories;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class PersonEventsViewModel : ViewModelBase
{

    private Event _selectedEvent = null!;
    private PersonEventGridItem _selectedGridItem;
    private Bitmap? _itemImage;
    private string _comment;

    public ObservableCollection<PersonEventGridItem> Events { get; set; } = [];

    public Bitmap? Image
    {
        get => _itemImage;
        private set => this.RaiseAndSetIfChanged(ref _itemImage, value);
    }

    public string Comment
    {
        get => _comment;
        private set => this.RaiseAndSetIfChanged(ref _comment, value);
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

    private object getclass()
    {
        return typeof(Movie);
    }

    private void SelectedItemChanged()
    {
        Image = null;
        Comment = string.Empty;

        if (SelectedGridItem == null)
        {
            return;
        }

        Image = FileRepsitory.GetImage(SelectedGridItem.Type, SelectedGridItem.ID);
        Comment = SelectedGridItem.Comment;

    }

    private List<PersonEventGridItem> LoadEvents(int id)
    {
        var peopleEventGridList = new List<PersonEventGridItem>();
        peopleEventGridList.AddRange(GetEvents<Boardgame>(id));
        peopleEventGridList.AddRange(GetEvents<Book>(id));
        peopleEventGridList.AddRange(GetEvents<Clip>(id));
        peopleEventGridList.AddRange(GetEvents<Comic>(id));
        peopleEventGridList.AddRange(GetEvents<Concert>(id));
        peopleEventGridList.AddRange(GetEvents<Game>(id));
        peopleEventGridList.AddRange(GetEvents<Location>(id));
        peopleEventGridList.AddRange(GetEvents<Magazine>(id));
        peopleEventGridList.AddRange(GetEvents<Movie>(id));
        peopleEventGridList.AddRange(GetEvents<Music>(id));
        peopleEventGridList.AddRange(GetEvents<Painting>(id));
        peopleEventGridList.AddRange(GetEvents<Pinball>(id));
        peopleEventGridList.AddRange(GetEvents<Song>(id));
        peopleEventGridList.AddRange(GetEvents<Standup>(id));
        peopleEventGridList.AddRange(GetEvents<Theatre>(id));
        peopleEventGridList.AddRange(GetEvents<TVShow>(id));
        peopleEventGridList.AddRange(GetEvents<Work>(id));
        peopleEventGridList.AddRange(GetEvents<Zoo>(id));

        return peopleEventGridList.OrderByDescending(o => o.Date).ToList();
    }

    private List<PersonEventGridItem> GetEvents<T>(int id) where T : IItem
    {
        var peopleEventGridList = new List<PersonEventGridItem>();

        var type = Helpers.GetClassName<T>();

        var eventList = _datasource.GetEventList(type);
        var itemList = _datasource.GetList<T>(type);

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

                var gridItem = new PersonEventGridItem(item.ID, type, item.Title, e.DateEnd, e.Comment);
                peopleEventGridList.Add(gridItem);
            }
        }

        return peopleEventGridList;
    }

    internal void LoadData(int id)
    {
        Events.Clear();
        Events.AddRange(LoadEvents(id));
    }
}
