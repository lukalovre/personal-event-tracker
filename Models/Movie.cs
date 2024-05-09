using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AvaloniaApplication1.Models.Interfaces;

namespace AvaloniaApplication1.Models;

[Table("Movies")]
public record Movie : IItem, IRuntime
{
    [Key]
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OriginalTitle { get; set; } = string.Empty;
    public int Runtime { get; set; }
    public int Year { get; set; }
    public string Director { get; set; } = string.Empty;
    public string Writer { get; set; } = string.Empty;
    public string ExternalID { get; set; } = string.Empty;
    public string Actors { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Ganre { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Plot { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}