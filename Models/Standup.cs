using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AvaloniaApplication1.Models.Interfaces;

namespace AvaloniaApplication1.Models;

[Table("Standup")]
public record Standup : IItem, IRuntime
{
    [Key]
    public int ID { get; set; }
    public string Performer { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Runtime { get; set; }
    public string Link { get; set; } = string.Empty;
    public string Writer { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Plot { get; set; } = string.Empty;
}