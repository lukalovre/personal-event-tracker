using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaloniaApplication1.Models;

[Table("Games")]
public record Game : IItem
{
    [Key]
    public int ID { get; set; }

    public string Title { get; set; }
    public int Year { get; set; }
    public string Platform { get; set; }
    public int ExternalID { get; set; }
}