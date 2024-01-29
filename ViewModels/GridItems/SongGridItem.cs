using AvaloniaApplication1.Models;

public record SongGridItem(int ID, int Index, string Artist, string Title, int Year, int Times, bool Bookmarked) : IGridItem;
