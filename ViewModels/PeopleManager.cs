using System.Collections.Generic;
using System.Linq;
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

    public List<PersonComboboxItem> GetComboboxList()
    {
        return _peopleList
            .Select(o => new PersonComboboxItem(o.ID, o.FirstName, o.LastName, o.Nickname))
            .ToList();
    }
}
