using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaloniaApplication1.Models;

[Table("Music")]
public class Music : IItem
{
    [Key]
    public int ID { get; set; }
    public int ItemID { get; set; }
    public string Artist { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public int Runtime { get; set; }
    public bool _1001 { get; set; }
    public string ExternalID { get; set; }
}