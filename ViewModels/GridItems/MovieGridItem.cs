using AvaloniaApplication1.Models;

public record MovieGridItem(int ID, string Title, string Director, int Year, int Rating) : IGridItem;
