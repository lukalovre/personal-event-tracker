using System;
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
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false };

        var tAttribute = (TableAttribute)
            typeof(T).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
        var tableName = tAttribute?.Name;
        var listPath = Path.Combine($"{tableName}.tsv");

        using var writer = new StreamWriter(listPath, true);
        using var csv = new CsvWriter(writer, config);
        csv.WriteRecord(item);
    }

    public List<T> GetList<T>()
        where T : class
    {
        var tAttribute = (TableAttribute)
            typeof(T).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
        var tableName = tAttribute?.Name;
        var listPath = Path.Combine($"../../Data/{tableName}.tsv");
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

        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<T>().ToList();
    }

    List<Event> IDatasource.GetEventList<T>()
    {
        var listPath = Path.Combine($"../../Data/{typeof(T)}Events.tsv");
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

        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<Event>().ToList();
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
