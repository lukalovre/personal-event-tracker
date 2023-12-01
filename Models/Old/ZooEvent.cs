using System.ComponentModel.DataAnnotations;

public class ZooEvent
{
    [Key]
    public int ID { get; set; }
    public int ItemID { get; set; }
    public string Date { get; set; }
    public string People { get; set; }
    public string Comment { get; set; }
    public string Rating { get; set; }
}
