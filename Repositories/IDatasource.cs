using System.Collections.Generic;

namespace Repositories;

public interface IDatasource
{
    void Add<T>(T item)
        where T : class;

    List<T> GetList<T>()
        where T : class;

    List<Event> GetEventList<T>()
        where T : class;

    void MakeBackup(string path);

    void Update<T>(T item)
        where T : class;
}
