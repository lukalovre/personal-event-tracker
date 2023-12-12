using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Work")]
public class Work : IItem
{
    [Key]
    public int ID { get; set; }

    public int ItemID { get; set; }

    public string Title { get; set; }

    public string Type { get; set; }
}
