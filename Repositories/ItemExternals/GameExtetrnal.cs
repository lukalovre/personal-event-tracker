using System.Threading.Tasks;
using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class GameExtetrnal : IExternal<Game>
{
    private readonly Igdb _igdb;

    public GameExtetrnal()
    {
        _igdb = new Igdb();
    }

    public async Task<Game> GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Igdb.UrlIdentifier))
        {
            return await _igdb.GetItem(url);
        }

        return new Game();
    }
}
