using AvaloniaApplication1.Models;

public record MovieGridItem(int ID, int Index, string Title, string Director, int Year, int Rating) : IGridItem;
