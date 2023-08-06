using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models;

public partial class Category
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;

    [DisplayName("Display Order")]
    public int DisplayOrder { get; set; }

    public DateTime CreatedDateTime { get; set; } = DateTime.Now;
}
