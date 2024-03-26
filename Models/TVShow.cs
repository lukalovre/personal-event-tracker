using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaloniaApplication1.Models;

[Table("TVShows")]
public record TVShow : IItem
{
    [Key]
    public int ID { get; set; }
    public string ExternalID { get; set; }
    public string Title { get; set; }
    public string OriginalTitle { get; set; }
    public int Year { get; set; }
    public int Runtime { get; set; }
    public string Actors { get; set; }
    public string Country { get; set; }
    public string Genre { get; set; }

    public string Language { get; set; }

    public string Plot { get; set; }

    public string Type { get; set; }

    public string Director { get; set; }

    public string Writer { get; set; }
}