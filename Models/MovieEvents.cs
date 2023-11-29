using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("MovieEvents")]
public class MovieEvent
{
    [Key]
    public int ID { get; set; }
    public string Imdb { get; set; }
    public DateTime? Date { get; set; }
    public int? Rating { get; set; }
    public string Comment { get; set; }
    public string People { get; set; }
    public string Platform { get; set; }
}
