using System;
using System.ComponentModel.DataAnnotations;

namespace AvaloniaApplication1.Models;

public class Event
{
    [Key]
    public int ID { get; set; }
    public int ItemID { get; set; }
    public string ExternalID { get; set; }
    public DateTime? DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    public int? Rating { get; set; }
    public bool Bookmakred { get; set; }
    public int? Chapter { get; set; }
    public int Amount { get; set; }
    public eAmountType AmountType { get; set; }
    public bool Completed { get; set; }
    public string Comment { get; set; }
    public string People { get; set; }
    public string Platform { get; set; }
    public int? LocationID { get; set; }
}
