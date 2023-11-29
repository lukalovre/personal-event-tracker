using System;
using System.ComponentModel.DataAnnotations;

public class Event
{
    [Key]
    public int ID { get; set; }
    public string ItemID { get; set; }
    public DateTime? Date { get; set; }
    public int? Rating { get; set; }
    public string Comment { get; set; }
    public string People { get; set; }
    public string Platform { get; set; }
}
