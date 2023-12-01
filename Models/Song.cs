using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Songs")]
public class Song : IItem
{
    [Key]
    public int ID { get; set; }
    public string Artist { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public int Runtime { get; set; }
    public string Link { get; set; }
}
