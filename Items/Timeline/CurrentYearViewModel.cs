using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media.Imaging;
using DynamicData;
using EventTracker.Models;
using EventTracker.Models.Interfaces;
using EventTracker.Repositories;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using Repositories;
using SkiaSharp;

namespace EventTracker.ViewModels;

public class CurrentYearViewModel : ViewModelBase
{
    private readonly IDatasource _datasource;
    private PersonEventGridItem _selectedGridItem = null!;
    private Bitmap? _itemImage;
    private string _comment = string.Empty;
    private string _yearName = string.Empty;
    private string _totalHoursText = string.Empty;
    private int _selectedYear;
    private string? _selectedCategory;
    private bool _excludeMusic = true;
    private List<ISeries> _amountByType = [];

    public CurrentYearViewModel(IDatasource datasource)
    {
        _datasource = datasource;
        _selectedYear = DateTime.Today.Year;
        RefreshReport();
    }

    public ObservableCollection<PersonEventGridItem> Events { get; } = [];

    public List<ISeries> AmountByType
    {
        get => _amountByType;
        private set => this.RaiseAndSetIfChanged(ref _amountByType, value);
    }

    public string YearName
    {
        get => _yearName;
        private set => this.RaiseAndSetIfChanged(ref _yearName, value);
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

    public int MinimumYear => 2010;
    public int MaximumYear => DateTime.Today.Year;

    public bool ExcludeMusic
    {
        get => _excludeMusic;
        set
        {
            if (_excludeMusic == value)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _excludeMusic, value);
            if (value && string.Equals(_selectedCategory, Helpers.GetClassName<Music>(), StringComparison.OrdinalIgnoreCase))
            {
                _selectedCategory = null;
            }

            RefreshReport();
        }
    }

    public int TotalMinutes { get; private set; }
    public string? SelectedCategory => _selectedCategory;

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

        YearName = SelectedYear.ToString();
        TotalHoursText = $"{TotalMinutes / 60d:F1} hours total";
    }

    public void ToggleCategory(string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return;
        }

        _selectedCategory = string.Equals(_selectedCategory, category, StringComparison.OrdinalIgnoreCase)
            ? null
            : category;
        RefreshReport();
    }

    private bool IsInSelectedYear(DateTime? date)
    {
        return date.HasValue && date.Value.Year == SelectedYear;
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
        if (ExcludeMusic && type == Helpers.GetClassName<Music>())
        {
            return [];
        }

        if (_selectedCategory is not null && !string.Equals(_selectedCategory, type, StringComparison.OrdinalIgnoreCase))
        {
            return [];
        }

        var itemList = _datasource.GetList<T>(type);
        var eventList = _datasource.GetEventList(type)
            .Where(item => IsInSelectedYear(item.DateEnd))
            .ToList();

        foreach (var eventItem in eventList)
        {
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
            AmountByType.Add(new PieSeries<double>
            {
                Values = [Math.Round(entry.Value / 60d, 1)],
                Name = entry.Key,
                Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B))
            });
        }
    }

    private void AddMinutes<T>(Dictionary<string, int> groupedMinutes) where T : IItem
    {
        var type = Helpers.GetClassName<T>();
        if (ExcludeMusic && type == Helpers.GetClassName<Music>())
        {
            return;
        }

        var modifier = Settings.Instance.GetItemSettigns<T>().AmountToMinutesModifier;
        var minutes = _datasource.GetEventList(type)
            .Where(item => IsInSelectedYear(item.DateEnd))
            .Sum(item => (int)Math.Round(item.Amount * modifier));

        groupedMinutes[type] = minutes;
    }
}
