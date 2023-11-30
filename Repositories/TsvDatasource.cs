using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
            HasHeaderRecord = false,
            Delimiter = "\t"
        };

        using var csv = new CsvReader(reader, config);

        var oldEventList = csv.GetRecords<BookEvent>().ToList();

        var movies = GetList<Book>();

        var convertedEventsList = oldEventList.Select(o => ConvertBook(o, movies)).ToList();

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

        if (missingItems.Any()) { }

        return newList;
    }

    private Event ConvertMovie(MovieEvent me, List<Movie> movies)
    {
        var movie = movies.First(o => o.Imdb == me.Imdb);

        var runtime = movie.Runtime;
        DateTime? dateEnd = null;

        if (
            DateTime.TryParse(
                me.Date,
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
            if (me.Date.Contains("00:00:00"))
            {
                dateStart = dateEnd;
            }
            else
            {
                dateStart = dateEnd.Value.AddMinutes(-runtime);
            }
        }

        int? rating = null;

        if (int.TryParse(me.Rating, out var retInt))
        {
            rating = retInt;
        }

        var peopleSplit = me.People.Split(',');

        var pepleList = new List<int>();

        if (peopleSplit.Any())
        {
            foreach (var item in peopleSplit)
            {
                if (int.TryParse(item, out var resInt))
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
            ID = me.ID,
            Amount = runtime,
            AmountType = eAmountType.Minutes,
            Comment = ShemaZenNull(me.Comment),
            Completed = true,
            DateEnd = dateEnd,
            DateStart = dateStart,
            Rating = rating,
            Platform = ShemaZenNull(me.Platform),
            ExternalID = ShemaZenNull(me.Imdb),
            ItemID = movie.ID,
            People = ShemaZenNull(string.Join(",", pepleList)),
            Bookmakred = false,
            Chapter = null,
            LocationID = null
        };
    }

    private Event ConvertBook(BookEvent e, List<Book> bookList)
    {
        var book = bookList.First(o => o.GoodreadsID.ToString() == e.GoodreadsID);

        var pages = e.Pages;
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
                dateStart = dateEnd.Value.AddMinutes(-pages * 2);
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
            foreach (var item in peopleSplit)
            {
                if (int.TryParse(item, out var resInt))
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
            Amount = pages,
            AmountType = eAmountType.Pages,
            Comment = ShemaZenNull(e.Comment),
            Completed = e.Read,
            DateEnd = dateEnd,
            DateStart = dateStart,
            Rating = rating,
            Platform = null,
            ExternalID = ShemaZenNull(e.GoodreadsID),
            ItemID = book.ID,
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
        where T : class
    {
        throw new System.NotImplementedException();
    }
}
