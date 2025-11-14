using System.Threading.Tasks;
using EventTracker.Models;
using EventTracker.Repositories.External;
using Repositories;

namespace EventTracker.Repositories;

public class MusicExternal : IExternal<Music>
{
    public async Task<Music> GetItem(string url)
    {
        if (url.Contains(YouTube.UrlIdentifier))
        {
            var item = await YouTube.GetYoutubeItem<Music>(url);

            return new Music
            {
                Title = item.MusicTitle,
                Artist = item.Artist,
                Year = item.Year,
                Runtime = item.Runtime,
                ExternalID = item.Link
            };
        }

        url = HtmlHelper.CleanUrl(url);

        if (url.Contains(Bandcamp.UrlIdentifier))
        {
            var item = await Bandcamp.GetBandcampItem<Music>(url);

            return new Music
            {
                Artist = item.Artist,
                Title = item.Title,
                Year = item.Year,
                _1001 = false,
                Runtime = item.Runtime,
                ExternalID = item.Link
            };
        }

        if (url.Contains(Spotify.UrlIdentifier))
        {
            return await Spotify.GetItem(url);
        }

        if (url.Contains(Soundcloud.UrlIdentifier))
        {
            var item = await Soundcloud.GetSoundcloudItem<Music>(url);

            return new Music
            {
                Title = item.Title,
                Artist = item.Artist,
                Year = item.Year,
                Runtime = item.Runtime,
                _1001 = false,
                ExternalID = item.ExternalID
            };
        }

        return new Music();
    }
}
