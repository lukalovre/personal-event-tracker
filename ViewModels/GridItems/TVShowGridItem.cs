using System;
using AvaloniaApplication1.Models;

public record TVShowGridItem(
    int ID,
    int Index,
    string Title,
    int Season,
    int Episode,
    DateTime? LastDate,
    string DaysAgo
) : IGridItem;
