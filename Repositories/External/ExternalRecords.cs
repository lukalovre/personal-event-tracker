namespace AvaloniaApplication1.Repositories.External;

public record BandcampItem(
    string Title,
    string Artist,
    int Year,
    int Runtime,
    string ImageUrl,
    string Link);

public record GoodreadsItem(
    string Title,
    string Writer,
    string Illustrator,
    int Year,
    int GoodreadsID,
    int Pages,
    string ImageUrl);

public record ImdbItem(
    string Title,
    int Runtime,
    int Year,
    string ExternalID,
    string Actors,
    string Country,
    string Director,
    string Genre,
    string Language,
    string Plot,
    string Type,
    string Writer,
    string StandupPerformer,
    string StandupTitle);