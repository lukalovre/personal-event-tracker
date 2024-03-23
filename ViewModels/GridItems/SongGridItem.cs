using AvaloniaApplication1.Models;

public record SongGridItem(int ID, string Artist, string Title, int Year, int Times, bool Bookmarked) : IGridItem;
