using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace Repositories;

internal class TsvDatasource : IDatasource
{
    public void Add<T>(T item)
        where T : class
    {
        throw new System.NotImplementedException();
    }

    public List<T> GetList<T>()
        where T : class
    {
        var tAttribute = (TableAttribute)
            typeof(T).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
        var tableName = tAttribute?.Name;
        var listPath = Path.Combine($"{tableName}.tsv");
        var text = File.ReadAllText(listPath);

        var reader = new StringReader(text);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            HasHeaderRecord = false,
            MissingFieldFound = null,
            Delimiter = "\t",
            BadDataFound = null
        };

        using (var csv = new CsvReader(reader, config))
        {
            return csv.GetRecords<T>().ToList();
        }
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
