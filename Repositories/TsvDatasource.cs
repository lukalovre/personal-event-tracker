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

namespace Repositories;

internal class TsvDatasource : IDatasource
{
    private readonly Dictionary<Type, object> _itemLists = [];
    private readonly Dictionary<string, object> _eventLists = new(StringComparer.Ordinal);

    private readonly CsvReaderOptions _readerOptions = new()
    {
        Separator = '\t',
        FormatProvider = CultureInfo.InvariantCulture,
        MissingField = CsvMissingFieldHandling.ReturnEmpty,
        MalformedData = CsvMalformedDataHandling.Recover,
        FieldCountMismatch = CsvFieldCountHandling.Allow
    };

    private static readonly CsvWriterOptions _writerOptions = new()
    {
        Separator = '\t',
        FormatProvider = CultureInfo.InvariantCulture,
        NewLine = Environment.NewLine
    };

    public void Add<T>(T item, Event e) where T : IItem
    {
        var type = Helpers.GetClassName<T>();
        var items = GetList<T>(type);
        var events = GetEventList<T>();
        var isNewItem = !items.Any(o => o.ID == item.ID);

        if (isNewItem)
        {
            var maxItemID = items.MaxBy(o => o.ID)?.ID ?? 0;
            item.ID = maxItemID + 1;
        }

        var maxEventID = events.MaxBy(o => o.ID)?.ID ?? 0;
        e.ID = maxEventID + 1;
        e.ItemID = item.ID;

        if (isNewItem)
        {
            InvalidateItems<T>();
        }

        InvalidateEvents<T>();

        if (isNewItem)
        {
            var itemFilePath = GetFilePath<T>();
            EnsureTrailingNewline(itemFilePath);

            using var writerItem = new StreamWriter(itemFilePath, true);
            using var csvItem = CsvWriter.Create(writerItem, _writerOptions);

            if (item.ID == 1)
            {
                // Start of file, write header first
                csvItem.WriteHeader<T>();
            }

            csvItem.WriteRecord(item);
            FileRepository.MoveTempImage<T>(item.ID);
        }

        var eventFilePath = GetEventFilePath<T>();
        EnsureTrailingNewline(eventFilePath);

        using var writerEvent = new StreamWriter(eventFilePath, true);
        using var csvEvent = CsvWriter.Create(writerEvent, _writerOptions);

        if (e.ID == 1)
        {
            // Start of file, write header first
            csvEvent.WriteHeader<Event>();
        }

        csvEvent.WriteRecord(e);

        if (!FileRepository.ImageExists<T>(item.ID))
        {
            FileRepository.MoveTempImage<T>(item.ID);
        }
    }

    private static void EnsureTrailingNewline(string filePath)
    {
        if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
        {
            return;
        }

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        stream.Seek(-1, SeekOrigin.End);
        var lastByte = stream.ReadByte();

        if (lastByte is not ('\n' or '\r'))
        {
            stream.Seek(0, SeekOrigin.End);
            using var writer = new StreamWriter(stream, leaveOpen: true);
            writer.Write(_writerOptions.NewLine);
            writer.Flush();
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
        if (!_itemLists.TryGetValue(typeof(T), out var cached))
        {
            cached = new CachedList<T>();
            _itemLists[typeof(T)] = cached;
        }

        var itemCache = (CachedList<T>)cached;
        if (itemCache.NeedsUpdate)
        {
            itemCache.Items = LoadList<T>();
            itemCache.NeedsUpdate = false;
        }

        return itemCache.Items;
    }

    private List<T> LoadList<T>() where T : IItem
    {
        var itemFilePath = GetFilePath<T>();

        if (!File.Exists(itemFilePath))
        {
            return [];
        }

        var text = File.ReadAllText(itemFilePath);
        var reader = new StringReader(text);
        using var csv = CsvReader.Create(reader, _readerOptions);
        return csv.GetRecords<T>().ToList();
    }

    public List<Event> GetEventList<T>() where T : IItem
    {
        return GetEventList(Helpers.GetClassName<T>());
    }

    public List<Event> GetEventList(string type)
    {
        if (!_eventLists.TryGetValue(type, out var cached))
        {
            cached = new CachedList<Event>();
            _eventLists[type] = cached;
        }

        var eventCache = (CachedList<Event>)cached;
        if (eventCache.NeedsUpdate)
        {
            eventCache.Items = LoadEventList(type);
            eventCache.NeedsUpdate = false;
        }

        return eventCache.Items;
    }

    private List<Event> LoadEventList(string type)
    {
        var eventFilePath = GetEventFilePath(type);

        if (!File.Exists(eventFilePath))
        {
            return [];
        }

        var text = File.ReadAllText(eventFilePath);
        var reader = new StringReader(text);
        using var csv = CsvReader.Create(reader, _readerOptions);

        return csv.GetRecords<Event>().ToList();
    }

    public void MakeBackup(string path)
    {
        throw new NotImplementedException();
    }

    public void Update<T>(T item) where T : IItem
    {
        var items = GetList<T>(nameof(T));

        var updateItem = items.First(o => o.ID == item.ID);
        items[items.IndexOf(updateItem)] = item;
        InvalidateItems<T>();

        var itemFilePath = GetFilePath<T>();
        using var writer = new StreamWriter(itemFilePath, false, System.Text.Encoding.UTF8);
        using var csvText = CsvWriter.Create(writer, _writerOptions);
        csvText.WriteRecords(items);
        writer.Flush();
    }

    private void InvalidateItems<T>() where T : IItem
    {
        if (_itemLists.TryGetValue(typeof(T), out var itemCache))
        {
            ((CachedList<T>)itemCache).NeedsUpdate = true;
        }
    }

    private void InvalidateEvents<T>() where T : IItem
    {
        var type = Helpers.GetClassName<T>();
        if (_eventLists.TryGetValue(type, out var eventCache))
        {
            ((CachedList<Event>)eventCache).NeedsUpdate = true;
        }
    }

    private sealed class CachedList<TItem>
    {
        public List<TItem> Items { get; set; } = [];
        public bool NeedsUpdate { get; set; } = true;
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
    //         Bookmarked = false,
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
