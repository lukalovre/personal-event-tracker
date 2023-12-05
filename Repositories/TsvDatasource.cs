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
    private readonly CsvConfiguration _config =
        new(CultureInfo.InvariantCulture) { Delimiter = "\t" };

    public void Add<T>(T item, Event e)
        where T : IItem
    {
        var items = GetList<T>();
        var maxID = items.MaxBy(o => o.ID).ID;
        item.ID = maxID + 1;
        e.ItemID = item.ID;

        var events = GetEventList<T>();
        maxID = events.MaxBy(o => o.ID).ID;
        e.ID = maxID + 1;

        string itemFilePath = GetFilePath<T>();
        using (var writer = new StreamWriter(itemFilePath, true))
        {
            using var csv = new CsvWriter(writer, _config);
            csv.WriteRecord(item);
        }

        string eventFilePath = GetEventFilePath<T>();
        using (var writer = new StreamWriter(eventFilePath, true))
        {
            using var csv = new CsvWriter(writer, _config);
            csv.WriteRecord(e);
        }
    }

    private static string GetFilePath<T>()
    {
        var tAttribute = (TableAttribute)
            typeof(T).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
        var tableName = tAttribute?.Name;
        return Path.Combine(Paths.Data, $"{tableName}.tsv");
    }

    private static string GetEventFilePath<T>()
    {
        var tAttribute = (TableAttribute)
            typeof(T).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
        var tableName = tAttribute?.Name;
        var listPath = Path.Combine(Paths.Data, $"{tableName}Events.tsv");
        return listPath;
    }

    public List<T> GetList<T>()
        where T : IItem
    {
        var tAttribute = (TableAttribute)
            typeof(T).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
        var tableName = tAttribute?.Name;
        var listPath = Path.Combine($"../../Data/{tableName}.tsv");
        var text = File.ReadAllText(listPath);

        var reader = new StringReader(text);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "\t",
            HasHeaderRecord = false,
            MissingFieldFound = null,
            BadDataFound = null
        };

        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<T>().ToList();
    }

    public List<Event> GetEventList<T>()
        where T : IItem
    {
        var listPath = Path.Combine($"../../Data/{typeof(T)}Events.tsv");
        var text = File.ReadAllText(listPath);

        var reader = new StringReader(text);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "\t" };

        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<Event>().ToList();
    }

    private Event ConvertGame(GameEvent e, List<Game> itemList)
    {
        var item = itemList.First(o => o.Igdb == e.Igdb);

        var amount = e.Time;
        DateTime? dateEnd = null;

        if (
            DateTime.TryParse(
                e.Date,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var fromDtEnd
            )
        )
        {
            dateEnd = fromDtEnd;
        }

        DateTime? dateStart = null;

        if (dateEnd.HasValue)
        {
            if (e.Date.Contains("00:00:00"))
            {
                dateStart = dateEnd;
            }
            else
            {
                dateStart = dateEnd.Value.AddMinutes(-amount);
            }
        }

        int? rating = null;

        if (int.TryParse(e.Rating, out var retInt))
        {
            rating = retInt;
        }

        var peopleSplit = e.People.Split(',');

        var pepleList = new List<int>();

        if (peopleSplit.Any())
        {
            foreach (var splitItem in peopleSplit)
            {
                if (int.TryParse(splitItem, out var resInt))
                {
                    pepleList.Add(resInt);
                }
            }
        }

        string ShemaZenNull(string s)
        {
            if (s == "--SchemaZenNull--" || string.IsNullOrWhiteSpace(s))
            {
                return null;
            }
            return s;
        }

        return new Event
        {
            ID = e.ID,
            Amount = amount,
            AmountType = eAmountType.Minutes,
            Comment = ShemaZenNull(e.Comment),
            Completed = e.Completed,
            DateEnd = dateEnd,
            DateStart = dateStart,
            Rating = rating,
            Platform = item.Platform,
            ExternalID = item.Igdb.ToString(),
            ItemID = item.ID,
            People = ShemaZenNull(string.Join(",", pepleList)),
            Bookmakred = false,
            Chapter = null,
            LocationID = null
        };
    }

    public void MakeBackup(string path)
    {
        throw new System.NotImplementedException();
    }

    public void Update<T>(T item)
        where T : IItem
    {
        throw new System.NotImplementedException();
    }

    public List<Event> GetEventListConvert<T>()
        where T : IItem
    {
        var listPath = Path.Combine($"../../Data/TODO/{typeof(T)}Events.tsv");
        var text = File.ReadAllText(listPath);

        var reader = new StringReader(text);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Delimiter = "\t"
        };

        using var csv = new CsvReader(reader, config);

        var oldEventList = csv.GetRecords<ComicEvent>().ToList();
        var item = GetList<Comic>();

        var convertedEventsList = oldEventList.Select(o => ConvertComic(o, item)).ToList();

        var newPath = $"../../Data/{typeof(T)}Events_converted.tsv";
        using var writer = new StreamWriter(newPath, false, System.Text.Encoding.UTF8);

        var configWrite = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = "\t"
        };

        var csvText = new CsvWriter(writer, configWrite);
        csvText.WriteRecords(convertedEventsList);
        writer.Flush();

        text = File.ReadAllText(newPath);
        reader = new StringReader(text);
        using var csv2 = new CsvReader(reader, configWrite);

        var newList = csv2.GetRecords<Event>().ToList();

        var newListIds = newList.Select(o => o.ID);
        var missingItems = oldEventList
            .Select(o => o.ID)
            .Where(o => !newListIds.Contains(o))
            .ToList();

        if (missingItems.Any())
        {
            throw new Exception("Yo!");
        }

        return newList;
    }
}
