using AvaloniaApplication1.Models;

public record BookGridItem(
    int ID,
    int Index,
    string Title,
    string Author,
    int Year,
    int? Rating,
    int Pages,
    int DaysAgo
) : IGridItem;
