using System.Threading.Tasks;
using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class ComicExtetrnal : IExternal<Comic>
{
    private readonly IExternal<Comic> _goodreads;

    public ComicExtetrnal()
    {
        _goodreads = new Goodreads();
    }

    public async Task<Comic> GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Goodreads.UrlIdentifier))
        {
            return await _goodreads.GetItem(url);
        }

        return new Comic();
    }
}
