using System.Collections.Generic;

namespace Repositories;

internal class JsonnDatasource : IDatasource
{
    public void Add<T>(T item)
        where T : class
    {
        throw new System.NotImplementedException();
    }

    public List<T> GetList<T>()
        where T : class
    {
        var result = new List<Movie>();

        for (int i = 0; i < 500; i++)
        {
            result.Add(new() { Director = $"Director {i}", });
        }

        return result as List<T>;
    }

    public void MakeBackup(string path)
    {
        throw new System.NotImplementedException();
    }

    public void Update<T>(T item)
        where T : class
    {
        throw new System.NotImplementedException();
    }
}
