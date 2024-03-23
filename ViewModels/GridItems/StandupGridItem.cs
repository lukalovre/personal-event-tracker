using AvaloniaApplication1.Models;

public record StandupGridItem(int ID, string Title, string Performer, int Year, int Rating) : IGridItem;
