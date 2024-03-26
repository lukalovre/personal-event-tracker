using System.ComponentModel.DataAnnotations.Schema;

namespace AvaloniaApplication1.Models;

[Table("Locations")]
public record Location : IItem
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}