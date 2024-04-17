namespace AvaloniaApplication1.Repositories.External;

public record BandcampItem(string Title, string Artist, int Year, int Runtime, string ImageUrl, string Link);
public record GoodreadsItem(string Title, string Writer, string Illustrator, int Year, int GoodreadsID, int Pages, string ImageUrl);