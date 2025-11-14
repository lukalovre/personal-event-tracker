using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTracker.Models.Interfaces;

namespace EventTracker.Models;

[Table("Comics")]
public record Comic : IItem
{
    [Key]
    public int ID { get; set; }
    public int ExternalID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Writer { get; set; } = string.Empty;
    public string Illustrator { get; set; } = string.Empty;
    public int Year { get; set; }
    public bool _1001 { get; set; }
}