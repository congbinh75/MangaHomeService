using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class Login
    {
        [Required]
        [MinLength(6)]
        [MaxLength(32)]
        public required string Username { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(16)]
        public required string Password { get; set; }
    }
}
