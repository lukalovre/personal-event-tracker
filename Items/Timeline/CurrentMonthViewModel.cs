using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media.Imaging;
using EventTracker.Models;
using EventTracker.Models.Interfaces;
using EventTracker.Repositories;
using DynamicData;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using Repositories;
using SkiaSharp;

namespace EventTracker.ViewModels;

public class CurrentMonthViewModel : ViewModelBase
{
    private readonly IDatasource _datasource;
    private PersonEventGridItem _selectedGridItem;
    private Bitmap? _itemImage;
    private string _comment;

    public CurrentMonthViewModel(IDatasource datasource)
    {
        _datasource = datasource;
        Events.AddRange(LoadEvents());
        TotalHoursText = $"{TotalMinutes / 60d:F1} hours total";
        AddPieSeries();
    }

    public ObservableCollection<PersonEventGridItem> Events { get; } = [];
    public List<ISeries> AmountByType { get; } = [];
    public string MonthName { get; } = DateTime.Today.ToString("MMMM");
    public int TotalMinutes { get; private set; }
    public string TotalHoursText { get; }

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

        if (SelectedGridItem is null)
        {
            return;
        }

        Image = FileRepository.GetImage(SelectedGridItem.Type, SelectedGridItem.ID);
        Comment = SelectedGridItem.Comment;
    }

    private List<PersonEventGridItem> LoadEvents()
    {
        var events = new List<PersonEventGridItem>();
        events.AddRange(GetEvents<Boardgame>());
        events.AddRange(GetEvents<Book>());
        events.AddRange(GetEvents<Clip>());
        events.AddRange(GetEvents<Comic>());
        events.AddRange(GetEvents<Concert>());
        events.AddRange(GetEvents<DnD>());
        events.AddRange(GetEvents<Game>());
        events.AddRange(GetEvents<Location>());
        events.AddRange(GetEvents<Magazine>());
        events.AddRange(GetEvents<Movie>());
        events.AddRange(GetEvents<Music>());
        events.AddRange(GetEvents<Painting>());
        events.AddRange(GetEvents<Pinball>());
        events.AddRange(GetEvents<Song>());
        events.AddRange(GetEvents<Standup>());
        events.AddRange(GetEvents<Theatre>());
        events.AddRange(GetEvents<TVShow>());
        events.AddRange(GetEvents<Work>());
        events.AddRange(GetEvents<Zoo>());
        events.AddRange(GetEvents<Adventure>());

        return events.OrderByDescending(item => item.Date).ToList();
    }

    private List<PersonEventGridItem> GetEvents<T>() where T : IItem
    {
        var type = Helpers.GetClassName<T>();
        var itemList = _datasource.GetList<T>(type);
        var eventList = _datasource.GetEventList(type)
            .Where(item => item.DateEnd.HasValue && item.DateEnd.Value.Year == DateTime.Today.Year && item.DateEnd.Value.Month == DateTime.Today.Month)
            .ToList();

        foreach (var eventItem in eventList)
        {
            var item = itemList.First(item => item.ID == eventItem.ItemID);
            var minutes = (int)Math.Round(eventItem.Amount * Settings.Instance.GetItemSettigns<T>().AmountToMinutesModifier);
            TotalMinutes += minutes;
        }

        return eventList
            .Select(eventItem =>
            {
                var item = itemList.First(item => item.ID == eventItem.ItemID);
                return new PersonEventGridItem(item.ID, type, item.Title, eventItem.DateEnd, eventItem.Comment);
            })
            .ToList();
    }

    private void AddPieSeries()
    {
        var groupedMinutes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        AddMinutes<Boardgame>(groupedMinutes);
        AddMinutes<Book>(groupedMinutes);
        AddMinutes<Clip>(groupedMinutes);
        AddMinutes<Comic>(groupedMinutes);
        AddMinutes<Concert>(groupedMinutes);
        AddMinutes<DnD>(groupedMinutes);
        AddMinutes<Game>(groupedMinutes);
        AddMinutes<Location>(groupedMinutes);
        AddMinutes<Magazine>(groupedMinutes);
        AddMinutes<Movie>(groupedMinutes);
        AddMinutes<Music>(groupedMinutes);
        AddMinutes<Painting>(groupedMinutes);
        AddMinutes<Pinball>(groupedMinutes);
        AddMinutes<Song>(groupedMinutes);
        AddMinutes<Standup>(groupedMinutes);
        AddMinutes<Theatre>(groupedMinutes);
        AddMinutes<TVShow>(groupedMinutes);
        AddMinutes<Work>(groupedMinutes);
        AddMinutes<Zoo>(groupedMinutes);
        AddMinutes<Adventure>(groupedMinutes);

        foreach (var entry in groupedMinutes.Where(entry => entry.Value > 0).OrderByDescending(entry => entry.Value))
        {
            var color = ChartColors.GetColor(entry.Key);
            AmountByType.Add(new PieSeries<int>
            {
                Values = [entry.Value],
                Name = entry.Key,
                Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B))
            });
        }
    }

    private void AddMinutes<T>(Dictionary<string, int> groupedMinutes) where T : IItem
    {
        var type = Helpers.GetClassName<T>();
        var modifier = Settings.Instance.GetItemSettigns<T>().AmountToMinutesModifier;
        var minutes = _datasource.GetEventList(type)
            .Where(item => item.DateEnd.HasValue && item.DateEnd.Value.Year == DateTime.Today.Year && item.DateEnd.Value.Month == DateTime.Today.Month)
            .Sum(item => (int)Math.Round(item.Amount * modifier));

        groupedMinutes[type] = minutes;
    }
}