using System.ComponentModel.DataAnnotations;

namespace Cars.Core.Models;

public class Bid
{
    [Key]
    public Guid Id { get; set; }

    public string? Car { get; set; }
    public string? User { get; set; }
    public double? Value { get; set; }
}