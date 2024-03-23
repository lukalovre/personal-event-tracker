using AvaloniaApplication1.Models;

public record StandupGridItem(int ID, int Index, string Title, string Performer, int Year, int Rating) : IGridItem;
