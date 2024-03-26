using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaloniaApplication1.Models;

[Table("Books")]
public class Book : IItem
{
    [Key]
    public int ID { get; set; }
    public int ExternalID { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
    public bool is1001 { get; set; }
}