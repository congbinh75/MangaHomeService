using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class LoginUser
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
