using System.Threading.Tasks;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Repositories.External;

namespace AvaloniaApplication1.Repositories;

public class SongExternal : IExternal<Song>
{
    public async Task<Song> GetItem(string url)
    {
        if (url.Contains(YouTube.UrlIdentifier))
        {
            var item = await YouTube.GetYoutubeItem<Song>(url);

            return new Song
            {
                Artist = item.Artist,
                Title = item.MusicTitle,
                Link = item.Link,
                Year = item.Year,
                Runtime = item.Runtime
            };
        }

        if (url.Contains(Bandcamp.UrlIdentifier))
        {
            var item = await Bandcamp.GetBandcampItem<Song>(url);

            return new Song
            {
                Title = item.Title,
                Artist = item.Artist,
                Year = item.Year,
                Runtime = item.Runtime,
                Link = item.Link
            };
        }

        if (url.Contains(Soundcloud.UrlIdentifier))
        {
            var item = await Soundcloud.GetSoundcloudItem<Song>(url);

            return new Song
            {
                Title = item.Title,
                Artist = item.Artist,
                Year = item.Year,
                Runtime = item.Runtime,
                Link = item.ExternalID
            };
        }

        return new Song();
    }
}
