using System;
using AvaloniaApplication1.Models;

namespace AvaloniaApplication1.ViewModels;

public record BookGridItem(int ID, string Title, string Author, int Year, int? Rating, bool Completed, int Pages, DateTime LastDate) : IGridItem;
public record ComicGridItem(int ID, string Title, string Writer, int? Chapter, int Pages, int? Rating, DateTime LastDate) : IGridItem;
public record GameGridItem(int ID, string Title, int Year, string Platform, int Time, bool Completed, int Rating, DateTime? LastDate) : IGridItem;
public record MovieGridItem(int ID, string Title, string Director, int Year, int Rating) : IGridItem;
public record MusicGridItem(int ID, string Artist, string Title, int Year, int Minutes, bool Bookmarked, int Played) : IGridItem;
public record SongGridItem(int ID, string Artist, string Title, int Year, int Times, bool Bookmarked) : IGridItem;
public record StandupGridItem(int ID, string Title, string Performer, int Year, int Rating) : IGridItem;
public record TheatreGridItem(int ID, string Title, string Writer, string Director, int Rating, DateTime? LastDate) : IGridItem;
public record TVShowGridItem(int ID, string Title, int Season, int Episode, int Rating, DateTime? LastDate) : IGridItem;
public record ClipGridItem(int ID, string Author, string Title, int Rating) : IGridItem;
public record WorkGridItem(int ID, string Title, string Type, int Minutes, DateTime LastDate) : IGridItem;
public record ZooGridItem(int ID, string Name, string City, string Country, int Year) : IGridItem;
public record LocationGridItem(int ID, string Name, string City, string Country, int Year) : IGridItem;