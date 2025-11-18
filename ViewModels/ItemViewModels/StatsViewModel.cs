using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EventTracker.Models;
using EventTracker.Models.Interfaces;
using EventTracker.Repositories;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Repositories;
using SkiaSharp;

namespace EventTracker.ViewModels;

public partial class StatsViewModel : ViewModelBase
{
    private readonly IDatasource _datasource;
    private readonly List<int> _yearLabels;
    private readonly Dictionary<int, int> _all = [];
    private readonly List<Info> _allInfo = [];

    public class Info
    {
        public string Category { get; set; } = null!;
        public int Year { get; set; }
        public int Amount { get; set; }
    }

    public List<Axis> BooksXAxes { get; set; } = [];
    public List<Axis> GamesXAxes { get; set; } = [];
    public List<Axis> MusicXAxes { get; set; } = [];
    public List<Axis> SongsXAxes { get; set; } = [];
    public List<Axis> TVShowsXAxes { get; set; } = [];
    public List<Axis> MoviesXAxes { get; set; } = [];
    public List<Axis> ComicsXAxes { get; set; } = [];
    public List<Axis> StandupXAxes { get; set; } = [];
    public List<Axis> MagazineXAxes { get; set; } = [];
    public List<Axis> WorksXAxes { get; set; } = [];
    public List<Axis> ClipsXAxes { get; set; } = [];
    public List<Axis> ConcertsXAxes { get; set; } = [];
    public List<Axis> TheatreXAxes { get; set; } = [];
    public List<Axis> BoardgamesXAxes { get; set; } = [];
    public List<Axis> DnDXAxes { get; set; } = [];

    public List<ISeries> Books { get; set; } = [];
    public List<ISeries> Games { get; } = [];
    public List<ISeries> Music { get; } = [];
    public List<ISeries> Songs { get; } = [];
    public List<ISeries> TVShows { get; } = [];
    public List<ISeries> Movies { get; } = [];
    public List<ISeries> Comics { get; } = [];
    public List<ISeries> Standup { get; } = [];
    public List<ISeries> Magazine { get; } = [];
    public List<ISeries> Works { get; } = [];
    public List<ISeries> Clips { get; } = [];
    public List<ISeries> Concerts { get; } = [];
    public List<ISeries> Theatre { get; } = [];
    public List<ISeries> Boardgames { get; } = [];
    public List<ISeries> DnD { get; } = [];

    public List<Axis> AllXAxes { get; set; } = [];
    public List<ISeries> All { get; } = [];

    public List<ISeries> YearPie { get; } = [];
    public List<ISeries> YearPie_1 { get; } = [];
    public List<ISeries> YearPie_2 { get; } = [];
    public List<ISeries> YearPie_3 { get; } = [];
    public List<ISeries> YearPie_4 { get; } = [];
    public List<ISeries> YearPie_5 { get; } = [];

    public StatsViewModel(IDatasource datasource)
    {
        _datasource = datasource;

        var startYear = 2010;
        var endYear = DateTime.Now.Year;
        _yearLabels = Enumerable.Range(startYear, endYear - startYear + 1).ToList();

        foreach (var item in _yearLabels)
        {
            _all.Add(item, 0);
        }

        FillData<Book>(Books, BooksXAxes);
        FillData<Game>(Games, GamesXAxes);
        FillData<Music>(Music, MusicXAxes);

        FillData<Song>(Songs, SongsXAxes);
        FillData<TVShow>(TVShows, TVShowsXAxes);
        FillData<Movie>(Movies, MoviesXAxes);

        FillData<Comic>(Comics, ComicsXAxes);
        FillData<Standup>(Standup, StandupXAxes);
        FillData<Magazine>(Magazine, MagazineXAxes);

        FillData<Work>(Works, WorksXAxes);
        FillData<Clip>(Clips, ClipsXAxes);
        FillData<Concert>(Concerts, ConcertsXAxes);

        FillData<Theatre>(Theatre, TheatreXAxes);
        FillData<Boardgame>(Boardgames, BoardgamesXAxes);
        FillData<DnD>(DnD, DnDXAxes);

        var color = ChartColors.GetColor("All");

        AllXAxes.Add(
            new Axis
            {
                Labels = _yearLabels.Select(o => o.ToString()).ToList(),
                LabelsRotation = 0,
                SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                SeparatorsAtCenter = false,
                TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                TicksAtCenter = true
            });

        All.Add(
            new ColumnSeries<int>
            {
                Values = new ReadOnlyCollection<int>(_all.Select(o => o.Value).ToList()),
                Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B))
            });

        AddPie(YearPie, DateTime.Now.Year);
        AddPie(YearPie_1, DateTime.Now.Year - 1);
        AddPie(YearPie_2, DateTime.Now.Year - 2);
        AddPie(YearPie_3, DateTime.Now.Year - 3);
        AddPie(YearPie_4, DateTime.Now.Year - 4);
        AddPie(YearPie_5, DateTime.Now.Year - 5);

    }

    private void AddPie(List<ISeries> series, int year)
    {
        AddPieSeries<Book>(series, year);
        AddPieSeries<Game>(series, year);
        // AddPieSeries<Music>(series, year);
        // AddPieSeries<Song>(series, year);
        AddPieSeries<TVShow>(series, year);
        AddPieSeries<Movie>(series, year);
        AddPieSeries<Comic>(series, year);
        AddPieSeries<Standup>(series, year);
        AddPieSeries<Magazine>(series, year);
        AddPieSeries<Work>(series, year);
        AddPieSeries<Clip>(series, year);
        AddPieSeries<Concert>(series, year);
        AddPieSeries<Theatre>(series, year);
        AddPieSeries<Boardgame>(series, year);
        AddPieSeries<DnD>(series, year);
    }

    private void AddPieSeries<T>(List<ISeries> series, int year)
    {
        var category = Helpers.GetClassName<T>();
        var color = ChartColors.GetColor(category);
        var value = _allInfo.Where(o => o.Category == category && o.Year == year).Sum(o => o.Amount);

        if (value == 0)
        {
            return;
        }

        series.Add(
            new PieSeries<int>
            {
                Values = [value],
                Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B)),
                Name = category
            });
    }

    private void FillData<T>(List<ISeries> series, List<Axis> xAxes, bool pureAmount = false) where T : IItem
    {
        var color = ChartColors.GetColor(Helpers.GetClassName<T>());

        xAxes.Add(
            new Axis
            {
                Labels = _yearLabels.Select(o => o.ToString()).ToList(),
                LabelsRotation = 0,
                SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                SeparatorsAtCenter = false,
                TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                TicksAtCenter = true
            });

        series.Add(
            new ColumnSeries<int>
            {
                Values = GetInfo<T>(pureAmount),
                // Stroke = new SolidColorPaint(new SKColor(color.R, color.G, color.B)),
                Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B))
            });
    }

    private List<int> GetInfo<T>(bool pureAmount = false) where T : IItem
    {
        var events = _datasource.GetEventList(Helpers.GetClassName<T>());

        var startYear = 2010;
        var endYear = DateTime.Now.Year;

        var amountModifier = Settings.Instance.GetItemSettigns<T>().AmountToMinutesModifier;

        var result = new List<int>();

        for (int i = startYear; i <= endYear; i++)
        {
            var year = i;

            var totalAmount = events.
                Where(o => o.DateEnd.HasValue && o.DateEnd.Value.Year == year)
                .Sum(o => o.Amount);

            var modifiedTotalAmount = pureAmount
            ? totalAmount
            : totalAmount * amountModifier / 60f;

            var totalAmountInt = (int)modifiedTotalAmount;

            result.Add(totalAmountInt);
            _all[year] += totalAmountInt;
            _allInfo.Add(
                new Info
                {
                    Category = Helpers.GetClassName<T>(),
                    Year = year,
                    Amount = totalAmountInt
                }
            );
        }

        return result;
    }
}