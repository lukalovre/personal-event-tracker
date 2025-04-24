using System.ComponentModel.DataAnnotations.Schema;
using AvaloniaApplication1.Models.Interfaces;

namespace AvaloniaApplication1.Models;

[Table("Adventures")]
public record Adventure : IItem
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}