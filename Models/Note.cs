using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace NotesApp2.Models
{
    public class Note
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Заглавието е задължително")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Съдържанието е задължително")]
        public string Content { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        
        public IdentityUser? User { get; set; }
        
        [StringLength(7)]
        public string Color { get; set; } = "#ffcccb";
    }
}