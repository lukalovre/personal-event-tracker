using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.ViewModels.Extensions;
using Repositories;

public class PeopleManager
{
    private static PeopleManager _instance;
    private readonly IDatasource _datasource;
    private readonly List<Person> _peopleList;

    private PeopleManager() { }

    private PeopleManager(IDatasource datasource)
    {
        _datasource = datasource;
        _peopleList = _datasource.GetList<Person>();
    }

    public static PeopleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PeopleManager(new TsvDatasource());
            }

            return _instance;
        }
    }

    public List<PersonComboBoxItem> GetComboboxList()
    {
        return _peopleList
            .Select(o => new PersonComboBoxItem(o.ID, o.FirstName, o.LastName, o.Nickname))
            .ToList();
    }

    public string GetDisplayNames(string idStringList)
    {
        if (string.IsNullOrWhiteSpace(idStringList))
        {
            return string.Empty;
        }

        var idList = idStringList.Split(',').Select(o => int.Parse(o));

        var result = string.Empty;

        foreach (var id in idList)
        {
            result = result + ", " + GetDisplayName(id);
        }

        return result.TrimStart(", ");
    }

    public string GetDisplayName(int id)
    {
        var person = _peopleList.First(o => o.ID == id);

        if (!string.IsNullOrWhiteSpace(person.Nickname))
        {
            return person.Nickname;
        }

        return $"{person.FirstName} {person.LastName}";
    }
}
