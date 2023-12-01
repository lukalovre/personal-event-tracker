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
        where T : IItem
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

    List<Event> IDatasource.GetEventList<T>()
    {
        var listPath = Path.Combine($"../../Data/{typeof(T)}Events.tsv");
        var text = File.ReadAllText(listPath);

        var reader = new StringReader(text);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "\t" };

        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<Event>().ToList();
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

    private Event ConvertMusic(MusicEvent e, List<Music> itemList)
    {
        var item = itemList.First(o => o.ItemID == e.ItemID);

        var amount = item.Runtime;
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
            Completed = true,
            DateEnd = dateEnd,
            DateStart = dateStart,
            Rating = rating,
            Platform = null,
            ExternalID = ShemaZenNull(item.SpotifyID),
            ItemID = item.ID,
            People = ShemaZenNull(string.Join(",", pepleList)),
            Bookmakred = e.In,
            Chapter = null,
            LocationID = null
        };
    }

    private Event ConvertTVShow(TVShowEvent e, List<TVShow> itemList)
    {
        var item = itemList.First(o => o.Imdb == e.Imdb);

        var amount = e.Runtime;
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
            Completed = false,
            DateEnd = dateEnd,
            DateStart = dateStart,
            Rating = rating,
            Platform = null,
            ExternalID = ShemaZenNull(item.Imdb),
            ItemID = item.ID,
            People = ShemaZenNull(string.Join(",", pepleList)),
            Bookmakred = false,
            Chapter = e.Season,
            LocationID = null
        };
    }

    private Event ConvertComic(ComicEvent e, List<Comic> itemList)
    {
        var item = itemList.First(o => o.GoodreadsID.ToString() == e.GoodreadsID);

        var amount = e.Pages;
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
                dateStart = dateEnd.Value.AddMinutes(-(int)(amount / 0.5f));
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
            AmountType = eAmountType.Pages,
            Comment = ShemaZenNull(e.Comment),
            Completed = e.Read,
            DateEnd = dateEnd,
            DateStart = dateStart,
            Rating = rating,
            Platform = null,
            ExternalID = ShemaZenNull(item.GoodreadsID.ToString()),
            ItemID = item.ID,
            People = ShemaZenNull(string.Join(",", pepleList)),
            Bookmakred = false,
            Chapter = e.Chapter,
            LocationID = null
        };
    }

    private Event ConvertSong(SongEvent e, List<Song> itemList)
    {
        var item = itemList.First(o => o.ID == e.ItemID);

        var amount = item.Runtime;
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
            Completed = true,
            DateEnd = dateEnd,
            DateStart = dateStart,
            Rating = rating,
            Platform = null,
            ExternalID = ShemaZenNull(item.Link),
            ItemID = item.ID,
            People = ShemaZenNull(string.Join(",", pepleList)),
            Bookmakred = e.In,
            Chapter = null,
            LocationID = null
        };
    }

    private Event ConvertZoo(ZooEvent e, List<Zoo> itemList)
    {
        var item = itemList.First(o => o.ID == e.ItemID);

        var amount = 1;
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
                dateStart = dateEnd;
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
            AmountType = eAmountType.Visit,
            Comment = ShemaZenNull(e.Comment),
            Completed = true,
            DateEnd = dateEnd,
            DateStart = dateStart,
            Rating = rating,
            Platform = null,
            ExternalID = null,
            ItemID = item.ID,
            People = ShemaZenNull(string.Join(",", pepleList)),
            Bookmakred = false,
            Chapter = null,
            LocationID = null
        };
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

        var oldEventList = csv.GetRecords<GameEvent>().ToList();
        var item = GetList<Game>();

        var convertedEventsList = oldEventList.Select(o => ConvertGame(o, item)).ToList();

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
