using System.Collections.Generic;
using AvaloniaApplication1.Models;

namespace Repositories;

public interface IDatasource
{
    void Add<T>(T item, Event e)
        where T : IItem;

    List<T> GetList<T>()
        where T : IItem;

    List<Event> GetEventList<T>()
        where T : IItem;

    List<Event> GetEventListConvert<T>()
        where T : IItem;

    void MakeBackup(string path);

    void Update<T>(T item)
        where T : IItem;
}
