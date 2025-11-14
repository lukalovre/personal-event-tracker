using System.Threading.Tasks;
using EventTracker.Models.Interfaces;

namespace EventTracker.Repositories;

public interface IExternal<T>
    where T : IItem
{
    public static string UrlIdentifier { get; } = string.Empty;

    public Task<T> GetItem(string url);
}
