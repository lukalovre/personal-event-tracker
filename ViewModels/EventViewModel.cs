using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using AvaloniaApplication1.Models;
using ReactiveUI;

namespace AvaloniaApplication1.ViewModels;

public partial class EventViewModel : ViewModelBase
{
    private Event selectedEvent;
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
        get => selectedEvent;
        set => this.RaiseAndSetIfChanged(ref selectedEvent, value);
    }

    public ObservableCollection<PersonComboBoxItem> PeopleList =>
        new(PeopleManager.Instance.GetComboboxList());

    private PersonComboBoxItem _selectedPerson;
    public PersonComboBoxItem SelectedPerson
    {
        get => _selectedPerson;
        set
        {
            _selectedPerson = value;
            SelectedPersonChanged();
        }
    }

    public ObservableCollection<string> PlatformTypes { get; set; }
    public string SelectedPlatformType { get; set; }

    public EventViewModel(
        ObservableCollection<Event> events,
        ObservableCollection<string> platformTypes
    )
    {
        Events = events;
        Events.CollectionChanged += CollectionChanged;
        PlatformTypes = platformTypes;
    }

    private void SelectedPersonChanged()
    {
        SelectedEvent.People = SelectedPerson.ID.ToString();
    }

    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var events = sender as ObservableCollection<Event>;
        SelectedEvent = events.MaxBy(o => o.DateEnd);

        if (SelectedEvent == null)
        {
            return;
        }

        _date = SelectedEvent.DateEnd.Value;
        _time = SelectedEvent.DateEnd.Value.TimeOfDay;
    }

    private void DateTimeChanged()
    {
        SelectedEvent.DateEnd = Date + Time;
    }
}
