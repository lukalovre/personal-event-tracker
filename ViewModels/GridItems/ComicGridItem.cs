using System;
using AvaloniaApplication1.Models;

public record ComicGridItem(int ID, string Title, string Writer, int? Chapter, int Pages, int? Rating, DateTime LastDate) : IGridItem;
