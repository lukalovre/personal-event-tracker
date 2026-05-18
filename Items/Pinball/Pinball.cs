using System.ComponentModel.DataAnnotations.Schema;
using EventTracker.Models.Interfaces;

namespace EventTracker.Models;

[Table("Pinball")]
public record Pinball : IItem
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? Year { get; set; }
    public string ExternalID { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}