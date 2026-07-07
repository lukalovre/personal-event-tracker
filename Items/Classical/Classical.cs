using System.ComponentModel.DataAnnotations.Schema;
using EventTracker.Models.Interfaces;

namespace EventTracker.Models;

[Table("Classical")]
public record Classical : IItem, IExternal
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Composser { get; set; } = string.Empty;
    public int? Year { get; set; } = null!;
    public int? Runtime { get; set; } = null!;
    public string ExternalID { get; set; } = string.Empty;
}