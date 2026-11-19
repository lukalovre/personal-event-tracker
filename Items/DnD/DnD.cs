using System.ComponentModel.DataAnnotations.Schema;
using EventTracker.Models.Interfaces;

namespace EventTracker.Models;

[Table("DnD")]
[CsvHelper.CsvRecord]
public partial record DnD : IItem
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}