using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Models.Interfaces;
using AvaloniaApplication1.Repositories;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Repositories;
using SkiaSharp;

namespace AvaloniaApplication1.ViewModels;

public partial class StatsViewModel : ViewModelBase
{
    private readonly IDatasource _datasource;
    private readonly List<int> _yearLabels;
    private Dictionary<int, int> _all = [];

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
                Values = _all.Select(o => o.Value),
                // Stroke = new SolidColorPaint(new SKColor(color.R, color.G, color.B)),
                Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B))
            });
    }

    private void FillData<T>(List<ISeries> series, List<Axis> xAxes) where T : IItem
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
                Values = GetInfo<T>(),
                // Stroke = new SolidColorPaint(new SKColor(color.R, color.G, color.B)),
                Fill = new SolidColorPaint(new SKColor(color.R, color.G, color.B))
            });
    }

    private List<int> GetInfo<T>() where T : IItem
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
                .Sum(o => o.Amount)
                * amountModifier
                / 60f;

            var totalAmountInt = (int)totalAmount;

            result.Add(totalAmountInt);
            _all[year] += totalAmountInt;
        }

        return result;
    }
}