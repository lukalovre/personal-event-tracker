using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class StandupExternal : IExternal<Standup>
{
    private readonly IExternal<Standup> _imdb;

    public StandupExternal()
    {
        _imdb = new Imdb();
    }

    public Standup GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Imdb.UrlIdentifier))
        {
            return _imdb.GetItem(url);
        }

        return new Standup();
    }

}
