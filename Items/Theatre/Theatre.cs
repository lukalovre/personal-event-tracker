using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTracker.Models.Interfaces;

namespace EventTracker.Models;

[Table("Theatre")]
public record Theatre : IItem, IRuntime
{
    [Key]
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Writer { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public int Runtime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}