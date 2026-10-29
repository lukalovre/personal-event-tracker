using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using EventTracker.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SkiaSharp;

namespace EventTracker.ViewModels;

public partial class EventViewModel : ViewModelBase
{
    private Event _selectedEvent = null!;
    private bool _isEditDate;
    private bool _isDateTimePickerOpen;
    private DateTimeOffset? _pendingEventDate;
    private TimeSpan? _pendingEventTime;

    public ObservableCollection<Event> Events { get; set; }

    public string SelectedEventDateTimeText => SelectedEvent?.DateEnd?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;

    public bool IsDateTimePickerOpen
    {
        get => _isDateTimePickerOpen;
        set
        {
            this.RaiseAndSetIfChanged(ref _isDateTimePickerOpen, value);
            if (value)
            {
                var dateEnd = SelectedEvent?.DateEnd ?? DateTime.Now;
                PendingEventDate = new DateTimeOffset(dateEnd);
                PendingEventTime = dateEnd.TimeOfDay;
            }
        }
    }

    public DateTimeOffset? PendingEventDate
    {
        get => _pendingEventDate;
        set => this.RaiseAndSetIfChanged(ref _pendingEventDate, value);
    }

    public TimeSpan? PendingEventTime
    {
        get => _pendingEventTime;
        set => this.RaiseAndSetIfChanged(ref _pendingEventTime, value);
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
            this.RaisePropertyChanged(nameof(SelectedEventDateTimeText));
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

    public ReactiveCommand<Unit, Unit> ToggleDateTimePicker { get; }
    public ReactiveCommand<Unit, Unit> ConfirmDateTime { get; }

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

    private List<Axis> _statsXAxes = [];
    private List<ISeries> _stats = [];

    public List<Axis> StatsXAxes
    {
        get => _statsXAxes;
        private set => this.RaiseAndSetIfChanged(ref _statsXAxes, value);
    }

    public List<ISeries> Stats
    {
        get => _stats;
        private set => this.RaiseAndSetIfChanged(ref _stats, value);
    }

    public EventViewModel(ObservableCollection<Event> events, ObservableCollection<string> platformTypes)
    {
        Events = events;
        Events.CollectionChanged += CollectionChanged;
        PlatformTypes = platformTypes;
        People = new PeopleSelectionViewModel();
        ToggleDateTimePicker = ReactiveCommand.Create(ToggleDateTimePickerAction);
        ConfirmDateTime = ReactiveCommand.Create(ConfirmDateTimeAction);
    }

    internal void FillStats<TItem>(ObservableCollection<Event> events)
    {
        var amountModifier = Settings.Instance.GetItemSettigns<TItem>().AmountToMinutesModifier;
        var dateEvents = events.Where(o => o.DateEnd.HasValue).ToList();

        if (dateEvents.Count == 0)
        {
            StatsXAxes = [];
            Stats = [];
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

        var statsXAxes = new List<Axis>
        {
            new Axis
            {
                Labels = yearLabels.Select(o => o.ToString()).ToList(),
                LabelsRotation = 0,
                SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                SeparatorsAtCenter = false,
                TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                TicksAtCenter = true
            }
        };

        var stats = new List<ISeries>
        {
            new ColumnSeries<int>
            {
                Values = values,
                // Stroke = new SolidColorPaint(new SKColor(color.R, color.G, color.B)),
                Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B))
            }
        };

        StatsXAxes = statsXAxes;
        Stats = stats;
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

        SelectedPlatformType = SelectedEvent.Platform;
        NewEventChapter = SelectedEvent.Chapter ?? 1;
    }

    private void ConfirmDateTimeAction()
    {
        if (SelectedEvent is null || PendingEventDate is null || PendingEventTime is null)
        {
            return;
        }

        SelectedEvent.DateEnd = PendingEventDate.Value.Date.Add(PendingEventTime.Value);
        this.RaisePropertyChanged(nameof(SelectedEventDateTimeText));
        IsDateTimePickerOpen = false;
    }

    private void ToggleDateTimePickerAction()
    {
        IsDateTimePickerOpen = !IsDateTimePickerOpen;
    }
}
