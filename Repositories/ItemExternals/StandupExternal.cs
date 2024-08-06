using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories.External;
using Repositories;

namespace AvaloniaApplication1.Repositories;

public class StandupExternal : IExternal<Standup>
{
    public async Task<Standup> GetItem(string url)
    {
        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Imdb.UrlIdentifier))
        {
            var item = await Imdb.GetImdbItem<Standup>(url);

            if (item is null)
            {
                return new Standup();
            }

            return new Standup
            {
                Performer = item.StandupPerformer,
                Title = item.StandupTitle,
                Link = item.ExternalID,
                Country = item.Country,
                Director = item.Director,
                Writer = item.Writer,
                Plot = item.Plot,
                Runtime = item.Runtime,
                Year = item.Year
            };
        }

        return new Standup();
    }

}
