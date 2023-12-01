using System.ComponentModel.DataAnnotations;

public class TVShowEvent
{
    [Key]
    public int ID { get; set; }
    public string Imdb { get; set; }
    public int Season { get; set; }
    public string Rating { get; set; }
    public string Date { get; set; }

    public string Comment { get; set; }

    public string People { get; set; }

    public int Runtime { get; set; }
}
