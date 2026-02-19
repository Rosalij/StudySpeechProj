using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudySpeech.Models
{
    public class NoteModel
    {
        public int Id { get; set; }

[Required]
        public string? Title { get; set; }

[Required]
        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; } = string.Empty;

        public IdentityUser? User { get; set; }
    }
}