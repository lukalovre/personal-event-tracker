using System.ComponentModel.DataAnnotations;

public class BookEvent
{
    [Key]
    public int ID { get; set; }
    public string GoodreadsID { get; set; }
    public string Rating { get; set; }
    public string Date { get; set; }
    public int Pages { get; set; }
    public bool Read { get; set; }
    public string Comment { get; set; }
    public string People { get; set; }
}
