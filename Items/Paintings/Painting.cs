using System.ComponentModel.DataAnnotations.Schema;
using EventTracker.Models.Interfaces;

namespace EventTracker.Models;

[Table("Paintings")]
[CsvHelper.CsvRecord]
public partial record Painting : IItem
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
}