using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using AvaloniaApplication1.Models;
using DynamicData;
using ReactiveUI;
using Repositories;

namespace AvaloniaApplication1.ViewModels;

public partial class PersonEventsViewModel : ViewModelBase
{

    private Event _selectedEvent = null!;
    private PersonGridItem _selectedGridItem;

    public ObservableCollection<PersonEventGridItem> Events { get; set; } = [];

    public Event SelectedEvent
    {
        get => _selectedEvent;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedEvent, value);
            SelectedEventChanged();
        }
    }

    private void SelectedEventChanged()
    {
    }

    private readonly IDatasource _datasource;

    public PersonEventsViewModel(IDatasource datasource, ObservableCollection<string> platformTypes)
    {
        _datasource = datasource;
    }

    public PersonGridItem SelectedGridItem
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
    }

    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var events = sender as ObservableCollection<Event>;
        SelectedEvent = events?.MaxBy(o => o.DateEnd)!;

        if (SelectedEvent == null)
        {
            return;
        }
    }

    private List<PersonEventGridItem> LoadEvents(int id)
    {
        var peopleEventList = new List<Event>();
        var peopleEventGridList = new List<PersonEventGridItem>();

        var eventList = _datasource.GetEventList<Movie>();

        foreach (var e in eventList)
        {
            if (string.IsNullOrWhiteSpace(e.People))
            {
                continue;
            }

            var peopleIDList = e.People.Split(',').Select(o => int.Parse(o));

            foreach (var perosnID in peopleIDList)
            {
                if (perosnID != id)
                {
                    continue;
                }

                peopleEventGridList.Add(new PersonEventGridItem(perosnID, e.ExternalID, "Movie", e.DateEnd));
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
