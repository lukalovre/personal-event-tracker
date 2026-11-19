using System.ComponentModel.DataAnnotations.Schema;
using EventTracker.Models.Interfaces;

namespace EventTracker.Models;

[Table("Magazines")]
[CsvHelper.CsvRecord]
public partial record Magazine : IItem
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
}