using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Comics")]
public class Comic : IItem
{
    [Key]
    public int ID { get; set; }
    public int GoodreadsID { get; set; }
    public string Title { get; set; }
    public string Writer { get; set; }
    public string Illustrator { get; set; }
    public int Year { get; set; }
    public bool _1001 { get; set; }
}
