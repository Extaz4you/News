using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace News.Backend.Articles.Models;

public class Article
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "AuthorId is required")]
    public int AuthorId { get; set; }

    [Required(ErrorMessage = "Theme is required")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Theme must be min 5 max 50 characters")]
    public string Theme { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Content must be min 5 max 100 characters")]
    public string Content { get; set; } = string.Empty;

    public DateTime DatePublication { get; set; } = DateTime.UtcNow;

}
