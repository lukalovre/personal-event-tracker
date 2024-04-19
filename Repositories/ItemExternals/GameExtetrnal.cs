using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class GameExtetrnal : IExternal<Game>
{
    public async Task<Game> GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Igdb.UrlIdentifier))
        {
            return await Igdb.GetItem(url);
        }

        return new Game();
    }
}
