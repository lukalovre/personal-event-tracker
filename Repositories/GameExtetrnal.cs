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

    public Game GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Igdb.UrlIdentifier))
        {
            return _igdb.GetItem(url);
        }

        return new Game();
    }
}
