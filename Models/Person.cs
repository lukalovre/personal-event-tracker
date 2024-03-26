using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaloniaApplication1.Models;

[Table("People")]
public record Person : IItem
{
    [Key]
    public int ID { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Postcode { get; set; }
    public string Birthday { get; set; }
    public string Nickname { get; set; }
}