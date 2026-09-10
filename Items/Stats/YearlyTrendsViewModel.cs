using System;
using System.Collections.Generic;
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
    public YearlyChartReport(string name, List<ISeries> series, List<Axis> xAxes)
    {
        Name = name;
        Series = series;
        XAxes = xAxes;
    }

    public string Name { get; }
    public List<ISeries> Series { get; }
    public List<Axis> XAxes { get; }
}

public class YearlyTrendsViewModel : ViewModelBase
{
    private const int StartYear = 2010;
    private readonly IDatasource _datasource;
    private readonly List<int> _years;
    private readonly List<double> _combinedHours;

    public YearlyTrendsViewModel(IDatasource datasource)
    {
        _datasource = datasource;
        _years = Enumerable.Range(StartYear, DateTime.Today.Year - StartYear + 1).ToList();
        _combinedHours = Enumerable.Repeat(0d, _years.Count).ToList();
        Reports = [];

        AddReport<Boardgame>();
        AddReport<Book>();
        AddReport<Clip>();
        AddReport<Comic>();
        AddReport<Concert>();
        AddReport<DnD>();
        AddReport<Game>();
        AddReport<Location>();
        AddReport<Magazine>();
        AddReport<Movie>();
        AddReport<Music>();
        AddReport<Painting>();
        AddReport<Pinball>();
        AddReport<Song>();
        AddReport<Standup>();
        AddReport<Theatre>();
        AddReport<TVShow>();
        AddReport<Work>();
        AddReport<Zoo>();
        AddReport<Adventure>();

        Reports.Insert(0, CreateReport("All Items", _combinedHours, "All"));
    }

    public List<YearlyChartReport> Reports { get; }

    private void AddReport<T>() where T : IItem
    {
        var category = Helpers.GetClassName<T>();
        var yearlyHours = GetYearlyHours<T>();

        for (var index = 0; index < yearlyHours.Count; index++)
        {
            _combinedHours[index] += yearlyHours[index];
        }

        Reports.Add(CreateReport(category, yearlyHours, category));
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

    private YearlyChartReport CreateReport(string name, List<double> values, string colorName)
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
                DataLabelsPosition = DataLabelsPosition.Top
            }
        };

        return new YearlyChartReport(name, series, axes);
    }
}
