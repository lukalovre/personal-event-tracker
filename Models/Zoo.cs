using System.ComponentModel.DataAnnotations.Schema;

[Table("Zoo")]
public class Zoo : IItem
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}
