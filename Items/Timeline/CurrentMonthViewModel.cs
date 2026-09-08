using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    private string _monthName = string.Empty;
    private string _totalHoursText = string.Empty;
    private string _selectedMonth = string.Empty;
    private int _selectedYear;
    private List<ISeries> _amountByType = [];

    public CurrentMonthViewModel(IDatasource datasource)
    {
        _datasource = datasource;
        MonthNames = new ObservableCollection<string>(Enumerable.Range(1, 12).Select(month => new DateTime(2000, month, 1).ToString("MMMM")));
        Years = new ObservableCollection<int>(Enumerable.Range(2010, DateTime.Today.Year - 2009));
        _selectedMonth = MonthNames[DateTime.Today.Month - 1];
        _selectedYear = DateTime.Today.Year;
        RefreshReport();
    }

    public ObservableCollection<PersonEventGridItem> Events { get; } = [];
    public ObservableCollection<string> MonthNames { get; }
    public ObservableCollection<int> Years { get; }
    public List<ISeries> AmountByType
    {
        get => _amountByType;
        private set => this.RaiseAndSetIfChanged(ref _amountByType, value);
    }
    public string MonthName
    {
        get => _monthName;
        private set => this.RaiseAndSetIfChanged(ref _monthName, value);
    }

    public string SelectedMonth
    {
        get => _selectedMonth;
        set
        {
            if (_selectedMonth == value)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _selectedMonth, value);
            RefreshReport();
        }
    }

    public int SelectedYear
    {
        get => _selectedYear;
        set
        {
            if (_selectedYear == value)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _selectedYear, value);
            RefreshReport();
        }
    }

    public int TotalMinutes { get; private set; }
    public string TotalHoursText
    {
        get => _totalHoursText;
        private set => this.RaiseAndSetIfChanged(ref _totalHoursText, value);
    }

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

    private void RefreshReport()
    {
        Events.Clear();
        AmountByType = [];
        TotalMinutes = 0;
        SelectedGridItem = null!;

        Events.AddRange(LoadEvents());
        AddPieSeries();

        MonthName = SelectedMonth;
        TotalHoursText = $"{TotalMinutes / 60d:F1} hours total";
    }

    private DateTime SelectedPeriod => new(
        SelectedYear,
        DateTime.ParseExact(SelectedMonth, "MMMM", CultureInfo.CurrentCulture).Month,
        1);

    private bool IsInSelectedPeriod(DateTime? date)
    {
        return date.HasValue
            && date.Value.Year == SelectedPeriod.Year
            && date.Value.Month == SelectedPeriod.Month;
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
            .Where(item => IsInSelectedPeriod(item.DateEnd))
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
            .Where(item => IsInSelectedPeriod(item.DateEnd))
            .Sum(item => (int)Math.Round(item.Amount * modifier));

        groupedMinutes[type] = minutes;
    }
}