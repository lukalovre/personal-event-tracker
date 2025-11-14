using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using EventTracker.Models;
using EventTracker.Models.Interfaces;
using EventTracker.Repositories;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Repositories;

internal class TsvDatasource : IDatasource
{
    private readonly CsvConfiguration _config = new(CultureInfo.InvariantCulture)
    {
        Delimiter = "\t",
        MissingFieldFound = null,
        BadDataFound = null
    };

    public void Add<T>(T item, Event e) where T : IItem
    {
        var type = Helpers.GetClassName<T>();
        var items = GetList<T>(type);
        var options = new TypeConverterOptions { Formats = ["yyyy-MM-dd HH:mm:ss"] };

        if (!items.Any(o => o.ID == item.ID))
        {
            var maxItemID = items.MaxBy(o => o.ID)?.ID ?? 0;
            item.ID = maxItemID + 1;

            var itemFilePath = GetFilePath<T>();

            using var writerItem = new StreamWriter(itemFilePath, true);
            using var csvItem = new CsvWriter(writerItem, _config);
            csvItem.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
            csvItem.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);

            if (item.ID == 1)
            {
                // Start of file, write header first
                csvItem.WriteHeader<T>();
            }

            csvItem.NextRecord();
            csvItem.WriteRecord(item);
            FileRepsitory.MoveTempImage<T>(item.ID);
        }

        var events = GetEventList<T>();
        var maxEventID = events.MaxBy(o => o.ID)?.ID ?? 0;
        e.ID = maxEventID + 1;
        e.ItemID = item.ID;

        var eventFilePath = GetEventFilePath<T>();

        using var writerEvent = new StreamWriter(eventFilePath, true);
        using var csvEvent = new CsvWriter(writerEvent, _config);
        csvEvent.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
        csvEvent.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);

        if (e.ID == 1)
        {
            // Start of file, write header first
            csvEvent.WriteHeader<Event>();
        }

        csvEvent.NextRecord();
        csvEvent.WriteRecord(e);

        if (!FileRepsitory.ImageExists<T>(item.ID))
        {
            FileRepsitory.MoveTempImage<T>(item.ID);
        }
    }

    private static string GetFilePath<T>()
    {
        var tableName = GetDataName<T>();
        return Path.Combine(Paths.Data, $"{tableName}.tsv");
    }

    private static string GetEventFilePath<T>()
    {
        return GetEventFilePath(Helpers.GetClassName<T>());
    }

    private static string GetEventFilePath(string type)
    {
        return Path.Combine(Paths.Data, $"{type}Events.tsv");
    }

    private static string? GetDataName<T>()
    {
        var tAttribute = (TableAttribute?)typeof(T)?.GetCustomAttributes(typeof(TableAttribute), true)?.FirstOrDefault();
        var tableName = tAttribute?.Name;
        return tableName;
    }

    public List<T> GetList<T>(string type) where T : IItem
    {
        var itemFilePath = GetFilePath<T>();

        if (!File.Exists(itemFilePath))
        {
            return [];
        }

        var text = File.ReadAllText(itemFilePath);
        var reader = new StringReader(text);
        using var csv = new CsvReader(reader, _config);
        return csv.GetRecords<T>().ToList();
    }

    public List<Event> GetEventList<T>() where T : IItem
    {
        var eventFilePath = GetEventFilePath<T>();

        if (!File.Exists(eventFilePath))
        {
            return [];
        }

        var text = File.ReadAllText(eventFilePath);
        var reader = new StringReader(text);
        using var csv = new CsvReader(reader, _config);

        return csv.GetRecords<Event>().ToList();
    }

    public List<Event> GetEventList(string type)
    {
        var eventFilePath = GetEventFilePath(type);

        if (!File.Exists(eventFilePath))
        {
            return [];
        }

        var text = File.ReadAllText(eventFilePath);
        var reader = new StringReader(text);
        using var csv = new CsvReader(reader, _config);

        return csv.GetRecords<Event>().ToList();
    }

    public void MakeBackup(string path)
    {
        throw new NotImplementedException();
    }

    public void Update<T>(T item) where T : IItem
    {
        var events = GetList<T>(nameof(T));

        var updateItem = events.First(o => o.ID == item.ID);
        events[events.IndexOf(updateItem)] = item;

        var itemFilePath = GetFilePath<T>();
        using var writer = new StreamWriter(itemFilePath, false, System.Text.Encoding.UTF8);
        using var csvText = new CsvWriter(writer, _config);
        var options = new TypeConverterOptions { Formats = ["yyyy-MM-dd HH:mm:ss"] };
        csvText.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
        csvText.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
        csvText.WriteRecords(events);
        writer.Flush();
    }

    public List<Event> GetEventListConvert<T>() where T : IItem
    {
        throw new NotImplementedException();
    }

    #region Remove after converted all data
    // private Event Convert(MyWorkEvent e, List<Work> itemList)
    // {
    //     var item = itemList.First(o => o.ItemID == e.ItemID);

    //     var amount = e.Time;
    //     DateTime? dateEnd = null;

    //     if (
    //         DateTime.TryParse(
    //             e.Date,
    //             CultureInfo.InvariantCulture,
    //             DateTimeStyles.None,
    //             out var fromDtEnd
    //         )
    //     )
    //     {
    //         dateEnd = fromDtEnd;
    //     }

    //     DateTime? dateStart = null;

    //     if (dateEnd.HasValue)
    //     {
    //         if (e.Date.Contains("00:00:00"))
    //         {
    //             dateStart = dateEnd;
    //         }
    //         else
    //         {
    //             dateStart = dateEnd.Value.AddMinutes(-amount);
    //         }
    //     }

    //     int? rating = null;

    //     if (int.TryParse(e.Rating, out var retInt))
    //     {
    //         rating = retInt;
    //     }

    //     var peopleSplit = e.People.Split(',');

    //     var pepleList = new List<int>();

    //     if (peopleSplit.Any())
    //     {
    //         foreach (var splitItem in peopleSplit)
    //         {
    //             if (int.TryParse(splitItem, out var resInt))
    //             {
    //                 pepleList.Add(resInt);
    //             }
    //         }
    //     }

    //     string ShemaZenNull(string s)
    //     {
    //         if (s == "--SchemaZenNull--" || string.IsNullOrWhiteSpace(s))
    //         {
    //             return null;
    //         }
    //         return s;
    //     }

    //     return new Event
    //     {
    //         ID = e.ID,
    //         Amount = amount,
    //         AmountType = eAmountType.Minutes,
    //         Comment = ShemaZenNull(e.Comment),
    //         Completed = false,
    //         DateEnd = dateEnd,
    //         DateStart = dateStart,
    //         Rating = rating,
    //         Platform = null,
    //         ExternalID = null,
    //         ItemID = item.ID,
    //         People = ShemaZenNull(string.Join(",", pepleList)),
    //         Bookmakred = false,
    //         Chapter = null,
    //         LocationID = null
    //     };
    // }

    // public List<Event> GetEventListConvert<T>()
    //     where T : IItem
    // {
    //     var listPath = Path.Combine($"../../Data/TODO/{typeof(T)}Events.tsv");
    //     var text = File.ReadAllText(listPath);

    //     var reader = new StringReader(text);

    //     var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    //     {
    //         HasHeaderRecord = false,
    //         Delimiter = "\t"
    //     };

    //     using var csv = new CsvReader(reader, config);

    //     var oldEventList = csv.GetRecords<MyWorkEvent>().ToList();
    //     var item = GetList<Work>();

    //     var convertedEventsList = oldEventList.Select(o => Convert(o, item)).ToList();

    //     var newPath = $"../../Data/{typeof(T)}Events_converted.tsv";
    //     using var writer = new StreamWriter(newPath, false, System.Text.Encoding.UTF8);

    //     var configWrite = new CsvConfiguration(CultureInfo.InvariantCulture)
    //     {
    //         HasHeaderRecord = true,
    //         Delimiter = "\t"
    //     };

    //     var csvText = new CsvWriter(writer, configWrite);
    //     csvText.WriteRecords(convertedEventsList);
    //     var options = new TypeConverterOptions { Formats = ["yyyy-MM-dd HH:mm:ss"] };
    //     csvText.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
    //     csvText.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
    //     writer.Flush();

    //     text = File.ReadAllText(newPath);
    //     reader = new StringReader(text);
    //     using var csv2 = new CsvReader(reader, configWrite);

    //     var newList = csv2.GetRecords<Event>().ToList();

    //     var newListIds = newList.Select(o => o.ID);
    //     var missingItems = oldEventList
    //         .Select(o => o.ID)
    //         .Where(o => !newListIds.Contains(o))
    //         .ToList();

    //     if (missingItems.Any())
    //     {
    //         throw new Exception("Yo!");
    //     }

    //     return newList;
    // }
    #endregion
}
