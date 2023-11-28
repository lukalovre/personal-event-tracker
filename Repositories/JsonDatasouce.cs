using System.Collections.Generic;
using Newtonsoft.Json;

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

        var json = JsonConvert.SerializeObject(result);

        return JsonConvert.DeserializeObject<List<T>>(json);
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
