namespace AvaloniaApplication1.Repositories;

public interface IExternal<T>
    where T : IItem
{
    public static string UrlIdentifier { get; } = string.Empty;

    public T GetItem(string url);
}
