using System.ComponentModel.DataAnnotations;

namespace Ascendion.Products.Dashboard.Models;

public class Product
{
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public required string Name { get; set; }
    [Required]
    [StringLength(50)]
    [MinLength(10), MaxLength(50)]
    public required string Status { get; set; }

}
