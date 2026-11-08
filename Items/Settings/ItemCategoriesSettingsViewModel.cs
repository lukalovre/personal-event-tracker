using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using ReactiveUI;

namespace EventTracker.ViewModels;

public class ItemCategorySettingsEditor(string name, ItemSettingsJson settings)
{
    public string Name { get; } = name;
    public ItemSettingsJson Settings { get; } = settings;

    public string DateTimeFilter
    {
        get => Settings.DateTimeFilter switch
        {
            "min" => "Earliest date",
            "-24" => "Last 24 hours",
            _ => "No filter"
        };
        set => Settings.DateTimeFilter = value switch
        {
            "Earliest date" => "min",
            "Last 24 hours" => "-24",
            _ => string.Empty
        };
    }
}

public class ItemCategoriesSettingsViewModel : ViewModelBase
{
    private static readonly string[] _categoryNames =
    [
        "Adventure", "Boardgame", "Book", "Classical", "Clip", "Comic", "Concert", "DnD", "Game",
        "Location", "Magazine", "Movie", "Music", "Painting", "Person", "Pinball", "Song", "Standup",
        "Theatre", "TVShow", "Work", "Zoo"
    ];

    private ItemCategorySettingsEditor _selectedCategory = null!;

    public ItemCategoriesSettingsViewModel()
    {
        Categories = new ObservableCollection<ItemCategorySettingsEditor>(
            _categoryNames.Select(name => new ItemCategorySettingsEditor(
                name,
                Settings.Instance.ItemSettings.TryGetValue(name, out var settings) ? settings : new ItemSettingsJson())));
        AmountTypes = Enum.GetValues<eAmountType>();
        DateTimeFilterOptions = ["No filter", "Earliest date", "Last 24 hours"];
        SelectedCategory = Categories[0];
        Save = ReactiveCommand.Create(SaveAction);
    }

    public ObservableCollection<ItemCategorySettingsEditor> Categories { get; }
    public eAmountType[] AmountTypes { get; }
    public string[] DateTimeFilterOptions { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }

    public ItemCategorySettingsEditor SelectedCategory
    {
        get => _selectedCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
    }

    private void SaveAction()
    {
        foreach (var category in Categories)
        {
            Settings.Instance.ItemSettings[category.Name] = category.Settings;
        }

        Settings.Save();
    }
}