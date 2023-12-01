using System.ComponentModel.DataAnnotations;

public class GameEvent
{
    [Key]
    public int ID { get; set; }
    public string Rating { get; set; }
    public string Comment { get; set; }
    public bool Completed { get; set; }
    public int Time { get; set; }
    public string Date { get; set; }

    public int Igdb { get; set; }

    public string People { get; set; }
}
