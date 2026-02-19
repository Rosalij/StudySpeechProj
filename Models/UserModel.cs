using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudySpeech.Models
{
    public class UserModel
    {
        public string Id { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}