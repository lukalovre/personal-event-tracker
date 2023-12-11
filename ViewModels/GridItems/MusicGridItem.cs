namespace AvaloniaApplication1.ViewModels.GridItems;

public record MusicGridItem(
    int ID,
    int Index,
    string Artist,
    string Title,
    int Year,
    int Minutes,
    bool Bookmarked,
    int Played
);
