using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventTracker.Models.Interfaces;

namespace EventTracker.Models;

[Table("Comics")]
[CsvHelper.CsvRecord]
public partial record Comic : IItem
{
    [Key]
    public int ID { get; set; }
    public string ExternalID { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Writer { get; set; } = string.Empty;
    public string Illustrator { get; set; } = string.Empty;
    public int Year { get; set; }
}