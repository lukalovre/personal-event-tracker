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
    string Actors,
    string Awards,
    string BoxOffice,
    string Country,
    string Director,
    string DVD,
    string Genre,
    string imdbID,
    string imdbRating,
    string imdbVotes,
    string Language,
    string Metascore,
    string Plot,
    string Poster,
    string Production,
    string Rated,
    string Released,
    string Response,
    string Runtime,
    string Title,
    string Type,
    string Website,
    string Writer,
    string Year);