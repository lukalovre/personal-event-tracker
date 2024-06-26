using System.ComponentModel.DataAnnotations.Schema;
using AvaloniaApplication1.Models.Interfaces;

namespace AvaloniaApplication1.Models;

[Table("Pinball")]
public record Pinball : IItem
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
}