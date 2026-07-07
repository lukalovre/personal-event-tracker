using System.Threading.Tasks;
using EventTracker.Models;
using EventTracker.Repositories.External;
using Repositories;

namespace EventTracker.Repositories;

public class ClassicalExternal : IExternal<Classical>
{
    public async Task<Classical> GetItem(string url)
    {
        if (url.Contains(YouTube.UrlIdentifier))
        {
            var item = await YouTube.GetYoutubeItem<Classical>(url);

            return new Classical
            {
                Title = item.MusicTitle,
                Composser = item.Artist,
                Year = item.Year,
                Runtime = item.Runtime,
                ExternalID = item.Link
            };
        }

        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Bandcamp.UrlIdentifier))
        {
            var item = await Bandcamp.GetBandcampItem<Classical>(url);

            return new Classical
            {
                Composser = item.Artist,
                Title = item.Title,
                Year = item.Year,
                Runtime = item.Runtime,
                ExternalID = item.Link
            };
        }

        if (url.Contains(Soundcloud.UrlIdentifier))
        {
            var item = await Soundcloud.GetSoundcloudItem<Classical>(url);

            return new Classical
            {
                Title = item.Title,
                Composser = item.Artist,
                Year = item.Year,
                Runtime = item.Runtime,
                ExternalID = item.ExternalID
            };
        }

        return new Classical();
    }
}
