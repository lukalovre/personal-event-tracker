using System.ComponentModel.DataAnnotations.Schema;
using EventTracker.Models.Interfaces;

namespace EventTracker.Models;

[Table("Adventures")]
public record Adventure : IItem
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}