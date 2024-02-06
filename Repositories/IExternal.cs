using System.Threading.Tasks;

namespace AvaloniaApplication1.Repositories;

public interface IExternal<T>
    where T : IItem
{
    public static string UrlIdentifier { get; } = string.Empty;

    public Task<T> GetItem(string url);
}
