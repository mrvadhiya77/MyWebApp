using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models;

public partial class Category
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public DateTime CreatedDateTime { get; set; }
}
