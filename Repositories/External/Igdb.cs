namespace AvaloniaApplication1.Repositories.External;

public class Igdb : IExternal<Game>
{
    public static string UrlIdentifier => "igdb.com";

    public Game GetItem(string url)
    {
        throw new System.NotImplementedException();
    }

    // public static async Task<Model.dbo.Game> GetDataFromAPIAsync(string igdbUrl, bool downloadPoster = true)
    // {
    // 	m_api = GetApiClient();

    // 	var games = await m_api.QueryAsync<Game>(IGDBClient.Endpoints.Games, $"fields name, url, summary, first_release_date, id, involved_companies, cover.image_id; where url = \"{igdbUrl.Trim()}\";");
    // 	var game = games.FirstOrDefault();

    // 	if (downloadPoster)
    // 	{
    // 		var imageId = game.Cover.Value.ImageId;
    // 		var coverUrl = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{imageId}.jpg";
    // 		var destinationFile = Path.Combine(Paths.GameCovers, $"{game.Id.Value}");

    // 		Web.DownloadPNG(coverUrl, destinationFile);
    // 	}

    // 	return new Model.dbo.Game
    // 	{
    // 		Igdb = (int)game.Id.Value,
    // 		Title = game.Name,
    // 		Year = game.FirstReleaseDate.Value.Year
    // 	};
    // }


    // public static async Task<string> GetUrlFromAPIAsync(int igdbID)
    // {
    //     m_api = GetApiClient();

    //     var games = await m_api.QueryAsync<Game>(
    //         IGDBClient.Endpoints.Games,
    //         $"fields name, url, summary, first_release_date, id, involved_companies, cover.image_id; where id = {igdbID};"
    //     );
    //     var game = games.FirstOrDefault();

    //     return game.Url;
    // }


    // 	public static async void OpenLink(Game1001 listItem)
    // {
    // 	var url = await GetUrlFromAPIAsync(listItem.Igdb);
    // 	Web.OpenLink(url);
    // }


    // 		private static IGDBClient GetApiClient()
    // {
    // 	if (m_api != null)
    // 	{
    // 		return m_api;
    // 	}

    // 	var lines = File.ReadAllLines(@"..\..\..\Keys\igdb_keys.txt");

    // 	var clientId = lines[0];
    // 	var clientSecret = lines[1];

    // 	return new IGDBClient(clientId, clientSecret);
    // }
}
