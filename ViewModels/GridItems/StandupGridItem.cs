using AvaloniaApplication1.Models;

public record StandupGridItem(int ID, int Index, string Title, string Director, int Year, int Rating) : IGridItem;
