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

public partial class TimelineViewModel : ViewModelBase
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

    public TimelineViewModel(IDatasource datasource)
    {
        _datasource = datasource;

        Events.Clear();
        Events.AddRange(LoadEvents());
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
        Comment = string.Empty;

        if (SelectedGridItem == null)
        {
            return;
        }

        Image = FileRepsitory.GetImage(SelectedGridItem.Type, SelectedGridItem.ID);
        Comment = SelectedGridItem.Comment;

    }

    private List<PersonEventGridItem> LoadEvents()
    {
        var peopleEventGridList = new List<PersonEventGridItem>();
        peopleEventGridList.AddRange(GetEvents<Boardgame>());
        peopleEventGridList.AddRange(GetEvents<Book>());
        peopleEventGridList.AddRange(GetEvents<Clip>());
        peopleEventGridList.AddRange(GetEvents<Comic>());
        peopleEventGridList.AddRange(GetEvents<Concert>());
        peopleEventGridList.AddRange(GetEvents<DnD>());
        peopleEventGridList.AddRange(GetEvents<Game>());
        peopleEventGridList.AddRange(GetEvents<Location>());
        peopleEventGridList.AddRange(GetEvents<Magazine>());
        peopleEventGridList.AddRange(GetEvents<Movie>());
        peopleEventGridList.AddRange(GetEvents<Music>());
        peopleEventGridList.AddRange(GetEvents<Painting>());
        peopleEventGridList.AddRange(GetEvents<Pinball>());
        peopleEventGridList.AddRange(GetEvents<Song>());
        peopleEventGridList.AddRange(GetEvents<Standup>());
        peopleEventGridList.AddRange(GetEvents<Theatre>());
        peopleEventGridList.AddRange(GetEvents<TVShow>());
        peopleEventGridList.AddRange(GetEvents<Work>());
        peopleEventGridList.AddRange(GetEvents<Zoo>());

        return peopleEventGridList.OrderByDescending(o => o.Date).ToList();
    }

    private List<PersonEventGridItem> GetEvents<T>() where T : IItem
    {
        var peopleEventGridList = new List<PersonEventGridItem>();

        var type = Helpers.GetClassName<T>();

        var eventList = _datasource.GetEventList(type);
        var itemList = _datasource.GetList<T>(type);

        foreach (var e in eventList)
        {
            var item = itemList.First(o => o.ID == e.ItemID);
            var gridItem = new PersonEventGridItem(item.ID, type, item.Title, e.DateEnd, e.Comment);
            peopleEventGridList.Add(gridItem);
        }

        return peopleEventGridList;
    }

}