using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTracker.Models.Interfaces;

namespace EventTracker.Models;

[Table("Work")]
public record Work : IItem
{
    [Key]
    public int ID { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
}