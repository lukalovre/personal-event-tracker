using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using AvaloniaApplication1.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SkiaSharp;

namespace AvaloniaApplication1.ViewModels;

public partial class EventViewModel : ViewModelBase
{
    private Event _selectedEvent = null!;
    private bool _isEditDate;
    private TimeSpan _time;
    private DateTime _date;

    public ObservableCollection<Event> Events { get; set; }

    public DateTime Date
    {
        get => _date;
        set
        {
            this.RaiseAndSetIfChanged(ref _date, value);
            DateTimeChanged();
        }
    }

    public TimeSpan Time
    {
        get => _time;
        set
        {
            this.RaiseAndSetIfChanged(ref _time, value);
            DateTimeChanged();
        }
    }

    public bool IsEditDate
    {
        get => _isEditDate;
        set => this.RaiseAndSetIfChanged(ref _isEditDate, value);
    }

    public Event SelectedEvent
    {
        get => _selectedEvent;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedEvent, value);
            SelectedEventChanged();
        }
    }

    private string _selectedPlatformType = string.Empty;
    private string _selectedPersonString = string.Empty;
    private int _newEventChapter = 1;

    public string SelectedPersonString
    {
        get => _selectedPersonString;
        set => this.RaiseAndSetIfChanged(ref _selectedPersonString, value);
    }

    public ObservableCollection<string> PlatformTypes { get; set; }

    public PeopleSelectionViewModel People { get; }

    public string SelectedPlatformType
    {
        get => _selectedPlatformType;
        set => this.RaiseAndSetIfChanged(ref _selectedPlatformType, value);
    }

    public int NewEventChapter
    {
        get => _newEventChapter;
        set => this.RaiseAndSetIfChanged(ref _newEventChapter, value);
    }

    public List<Axis> StatsXAxes { get; set; } = [];
    public List<ISeries> Stats { get; } = [];

    public EventViewModel(ObservableCollection<Event> events, ObservableCollection<string> platformTypes)
    {
        Events = events;
        Events.CollectionChanged += CollectionChanged;
        PlatformTypes = platformTypes;
        People = new PeopleSelectionViewModel();
    }

    internal void FillStats<TItem>(ObservableCollection<Event> events)
    {
        var amountModifier = Settings.Instance.GetItemSettigns<TItem>().AmountToMinutesModifier;
        var dateEvents = events.Where(o => o.DateEnd.HasValue).ToList();

        if (dateEvents is null || dateEvents.Count == 0)
        {
            return;
        }

        var minYear = dateEvents.Min(o => o.DateEnd.HasValue ? o.DateEnd.Value.Year : 0);
        var maxYear = dateEvents.Max(o => o.DateEnd.HasValue ? o.DateEnd.Value.Year : 0);

        var yearLabels = new List<int>();

        for (int i = minYear; i <= maxYear; i++)
        {
            yearLabels.Add(i);
        }

        var values = new List<int>();

        foreach (var year in yearLabels)
        {
            var totalAmount = dateEvents
                        .Where(o => o.DateEnd.HasValue && o.DateEnd.Value.Year == year)
                        .Sum(o => o.Amount);

            var amountModified = totalAmount * amountModifier / 60f;
            values.Add((int)amountModified);
        }

        var color = ChartColors.GetColor("All");

        StatsXAxes.Clear();
        Stats.Clear();

        StatsXAxes.Add(
            new Axis
            {
                Labels = yearLabels.Select(o => o.ToString()).ToList(),
                LabelsRotation = 0,
                SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                SeparatorsAtCenter = false,
                TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                TicksAtCenter = true
            });

        Stats.Add(
            new ColumnSeries<int>
            {
                Values = values,
                // Stroke = new SolidColorPaint(new SKColor(color.R, color.G, color.B)),
                Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B))
            });
    }

    private void SelectedEventChanged()
    {
        if (SelectedEvent == null)
        {
            return;
        }

        SelectedPersonString = PeopleManager.Instance.GetDisplayNames(SelectedEvent.People);
        People.SetPeople(SelectedEvent.People);
    }

    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var events = sender as ObservableCollection<Event>;
        SelectedEvent = events?.MaxBy(o => o.DateEnd)!;

        if (SelectedEvent == null)
        {
            return;
        }

        _date = SelectedEvent.DateEnd ?? DateTime.MinValue;
        _time = _date.TimeOfDay;

        SelectedPlatformType = SelectedEvent.Platform;
        NewEventChapter = SelectedEvent.Chapter ?? 1;
    }

    private void DateTimeChanged()
    {
        SelectedEvent.DateEnd = Date + Time;
    }
}
