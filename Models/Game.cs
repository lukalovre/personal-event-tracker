using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Games")]
public class Game : IItem
{
    [Key]
    public int ID { get; set; }

    public string Title { get; set; }
    public int Year { get; set; }
    public string Platform { get; set; }
    public int Igdb { get; set; }
}
