using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace EventTracker.ViewModels;

public sealed class YearlyTrendsCategorySetting
{
    public string Name { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
}

public sealed class YearlyTrendsSettings
{
    private ObservableCollection<YearlyTrendsCategorySetting> _categories = CreateDefaults();

    public static IReadOnlyList<string> SupportedCategories { get; } =
    [
        "Boardgame",
        "Book",
        "Clip",
        "Comic",
        "Concert",
        "DnD",
        "Game",
        "Location",
        "Magazine",
        "Movie",
        "Music",
        "Painting",
        "Pinball",
        "Song",
        "Standup",
        "Theatre",
        "TVShow",
        "Work",
        "Zoo",
        "Adventure"
    ];

    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public ObservableCollection<YearlyTrendsCategorySetting> Categories
    {
        get => _categories;
        set => _categories = value ?? CreateDefaults();
    }

    public void Normalize()
    {
        var savedCategories = Categories ?? [];
        var knownCategories = new HashSet<string>(SupportedCategories, System.StringComparer.OrdinalIgnoreCase);
        var normalized = new ObservableCollection<YearlyTrendsCategorySetting>();
        var addedCategories = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

        foreach (var category in savedCategories)
        {
            if (category is null || string.IsNullOrWhiteSpace(category.Name))
            {
                continue;
            }

            var name = SupportedCategories.FirstOrDefault(item =>
                string.Equals(item, category.Name, System.StringComparison.OrdinalIgnoreCase));
            if (name is null || !knownCategories.Contains(name) || !addedCategories.Add(name))
            {
                continue;
            }

            normalized.Add(new YearlyTrendsCategorySetting
            {
                Name = name,
                IsVisible = category.IsVisible
            });
        }

        foreach (var name in SupportedCategories)
        {
            if (addedCategories.Add(name))
            {
                normalized.Add(new YearlyTrendsCategorySetting
                {
                    Name = name,
                    IsVisible = true
                });
            }
        }

        _categories.Clear();
        foreach (var category in normalized.OrderByDescending(category => category.IsVisible))
        {
            _categories.Add(category);
        }
    }

    private static ObservableCollection<YearlyTrendsCategorySetting> CreateDefaults()
    {
        return new ObservableCollection<YearlyTrendsCategorySetting>(SupportedCategories.Select(name => new YearlyTrendsCategorySetting
        {
            Name = name,
            IsVisible = true
        }));
    }
}
