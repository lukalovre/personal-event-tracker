using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaloniaApplication1.Models;

[Table("Clips")]
public record Clip : IItem
{
    [Key]
    public int ID { get; set; }
    public string ExternalID { get; set; }
    public string Author { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public int Runtime { get; set; }
}