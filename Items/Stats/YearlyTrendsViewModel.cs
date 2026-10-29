using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EventTracker.Models;
using EventTracker.Models.Interfaces;
using EventTracker.Repositories;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Repositories;
using SkiaSharp;

namespace EventTracker.ViewModels;

public sealed class YearlyChartReport
{
    public YearlyChartReport(string name, string unit, List<ISeries> series, List<Axis> xAxes)
    {
        Name = name;
        Unit = unit;
        Series = series;
        XAxes = xAxes;
    }

    public string Name { get; }
    public string Unit { get; }
    public List<ISeries> Series { get; }
    public List<Axis> XAxes { get; }
}

public class YearlyTrendsViewModel : ViewModelBase, ILoadOnSelection
{
    private const int StartYear = 2010;
    private readonly IDatasource _datasource;
    private readonly List<int> _years;
    private readonly HashSet<string> _showingActualValues = new(StringComparer.OrdinalIgnoreCase);
    private bool _hasLoaded;

    public YearlyTrendsViewModel(IDatasource datasource)
    {
        _datasource = datasource;
        _years = Enumerable.Range(StartYear, DateTime.Today.Year - StartYear + 1).ToList();
        Reports = [];
        Settings.Saved += SettingsSaved;
    }

    public void LoadOnSelection()
    {
        if (_hasLoaded)
        {
            return;
        }

        _hasLoaded = true;
        RefreshReports();
    }

    public ObservableCollection<YearlyChartReport> Reports { get; }

    public void RefreshReports()
    {
        var categoryReports = new Dictionary<string, YearlyChartReport>(StringComparer.OrdinalIgnoreCase);
        var combinedHours = Enumerable.Repeat(0d, _years.Count).ToList();

        AddReport<Boardgame>(categoryReports, combinedHours);
        AddReport<Book>(categoryReports, combinedHours);
        AddReport<Clip>(categoryReports, combinedHours);
        AddReport<Comic>(categoryReports, combinedHours);
        AddReport<Concert>(categoryReports, combinedHours);
        AddReport<DnD>(categoryReports, combinedHours);
        AddReport<Game>(categoryReports, combinedHours);
        AddReport<Location>(categoryReports, combinedHours);
        AddReport<Magazine>(categoryReports, combinedHours);
        AddReport<Movie>(categoryReports, combinedHours);
        AddReport<Music>(categoryReports, combinedHours);
        AddReport<Painting>(categoryReports, combinedHours);
        AddReport<Pinball>(categoryReports, combinedHours);
        AddReport<Song>(categoryReports, combinedHours);
        AddReport<Standup>(categoryReports, combinedHours);
        AddReport<Theatre>(categoryReports, combinedHours);
        AddReport<TVShow>(categoryReports, combinedHours);
        AddReport<Work>(categoryReports, combinedHours);
        AddReport<Zoo>(categoryReports, combinedHours);
        AddReport<Adventure>(categoryReports, combinedHours);

        Reports.Clear();
        Reports.Add(CreateReport("All Items", combinedHours, "All"));

        foreach (var category in Settings.Instance.YearlyTrends.Categories.Where(category => category.IsVisible))
        {
            if (categoryReports.TryGetValue(category.Name, out var report))
            {
                Reports.Add(report);
            }
        }
    }

    private void SettingsSaved()
    {
        if (_hasLoaded)
        {
            RefreshReports();
        }
    }

    public void ToggleChartUnits(string? category)
    {
        if (string.IsNullOrWhiteSpace(category) || category == "All Items")
        {
            return;
        }

        if (!_showingActualValues.Add(category))
        {
            _showingActualValues.Remove(category);
        }

        var reportIndex = -1;
        for (var index = 0; index < Reports.Count; index++)
        {
            if (string.Equals(Reports[index].Name, category, StringComparison.OrdinalIgnoreCase))
            {
                reportIndex = index;
                break;
            }
        }

        if (reportIndex < 0)
        {
            return;
        }

        Reports[reportIndex] = CreateCategoryReport(category);
    }

    private void AddReport<T>(Dictionary<string, YearlyChartReport> categoryReports, List<double> combinedHours) where T : IItem
    {
        var category = Helpers.GetClassName<T>();
        var yearlyHours = GetYearlyHours<T>();

        if (typeof(T) != typeof(Music) && typeof(T) != typeof(Song))
        {
            for (var index = 0; index < yearlyHours.Count; index++)
            {
                combinedHours[index] += yearlyHours[index];
            }
        }

        categoryReports[category] = CreateCategoryReport<T>(yearlyHours);
    }

    private YearlyChartReport CreateCategoryReport(string category)
    {
        return category switch
        {
            "Boardgame" => CreateCategoryReport<Boardgame>(),
            "Book" => CreateCategoryReport<Book>(),
            "Clip" => CreateCategoryReport<Clip>(),
            "Comic" => CreateCategoryReport<Comic>(),
            "Concert" => CreateCategoryReport<Concert>(),
            "DnD" => CreateCategoryReport<DnD>(),
            "Game" => CreateCategoryReport<Game>(),
            "Location" => CreateCategoryReport<Location>(),
            "Magazine" => CreateCategoryReport<Magazine>(),
            "Movie" => CreateCategoryReport<Movie>(),
            "Music" => CreateCategoryReport<Music>(),
            "Painting" => CreateCategoryReport<Painting>(),
            "Pinball" => CreateCategoryReport<Pinball>(),
            "Song" => CreateCategoryReport<Song>(),
            "Standup" => CreateCategoryReport<Standup>(),
            "Theatre" => CreateCategoryReport<Theatre>(),
            "TVShow" => CreateCategoryReport<TVShow>(),
            "Work" => CreateCategoryReport<Work>(),
            "Zoo" => CreateCategoryReport<Zoo>(),
            "Adventure" => CreateCategoryReport<Adventure>(),
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, "Unknown yearly trends category.")
        };
    }

    private YearlyChartReport CreateCategoryReport<T>(List<double>? yearlyHours = null) where T : IItem
    {
        var category = Helpers.GetClassName<T>();
        var showActualValues = _showingActualValues.Contains(category);
        var values = showActualValues ? GetYearlyAmounts<T>() : yearlyHours ?? GetYearlyHours<T>();
        var unit = showActualValues ? Settings.Instance.GetItemSettigns<T>().AmountVerb : "hours";
        return CreateReport(category, values, category, unit);
    }

    private List<double> GetYearlyHours<T>() where T : IItem
    {
        var modifier = Settings.Instance.GetItemSettigns<T>().AmountToMinutesModifier;
        var events = _datasource.GetEventList(Helpers.GetClassName<T>());

        return _years
            .Select(year => events
                .Where(item => item.DateEnd.HasValue && item.DateEnd.Value.Year == year)
                .Sum(item => item.Amount * modifier / 60d))
            .ToList();
    }

    private List<double> GetYearlyAmounts<T>() where T : IItem
    {
        var events = _datasource.GetEventList(Helpers.GetClassName<T>());

        return _years
            .Select(year => events
                .Where(item => item.DateEnd.HasValue && item.DateEnd.Value.Year == year)
                .Sum(item => (double)item.Amount))
            .ToList();
    }

    private YearlyChartReport CreateReport(string name, List<double> values, string colorName, string unit = "hours")
    {
        var color = ChartColors.GetColor(colorName);
        var axes = new List<Axis>
        {
            new()
            {
                Labels = _years.Select(year => year.ToString()).ToList(),
                LabelsRotation = 45,
                SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                SeparatorsAtCenter = false,
                TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                TicksAtCenter = true
            }
        };

        var series = new List<ISeries>
        {
            new ColumnSeries<double>
            {
                Values = values,
                Name = name,
                Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B)),
                DataLabelsPosition = DataLabelsPosition.Top,
                DataLabelsFormatter = point => FormatValue(point.Coordinate.PrimaryValue, unit),
                YToolTipLabelFormatter = point => FormatValue(point.Coordinate.PrimaryValue, unit)
            }
        };

        return new YearlyChartReport(name, unit, series, axes);
    }

    private static string FormatValue(double value, string unit)
    {
        return string.Equals(unit, "hours", StringComparison.OrdinalIgnoreCase)
            ? value.ToString("F1")
            : value.ToString("F0");
    }
}
