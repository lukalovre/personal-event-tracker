using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class ClipsExternal : IExternal<Clip>
{
    private readonly IExternal<Clip> _youtube;

    public ClipsExternal()
    {
        _youtube = new YouTube();
    }

    public async Task<Clip> GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(YouTube.UrlIdentifier))
        {
            return await _youtube.GetItem(url);
        }

        return new Clip();
    }
}
