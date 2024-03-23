using System;
using AvaloniaApplication1.Models;

public record TVShowGridItem(int ID, string Title, int Season, int Episode, DateTime? LastDate) : IGridItem;
