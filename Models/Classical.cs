using System.ComponentModel.DataAnnotations.Schema;
using EventTracker.Models.Interfaces;

namespace EventTracker.Models;

[Table("Classical")]
public record Classical : IItem
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Composser { get; set; } = string.Empty;
    public int? Year { get; set; } = null!;
}