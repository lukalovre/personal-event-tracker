using System.ComponentModel.DataAnnotations;

public class MusicEvent
{
    [Key]
    public int ID { get; set; }
    public int ItemID { get; set; }
    public bool In { get; set; }

    public string Date { get; set; }
    public string Comment { get; set; }
    public string Rating { get; set; }
    public string People { get; set; }
}
