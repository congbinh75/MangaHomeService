using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class RegisterUser
    {
        [Required]
        [MinLength(6)]
        [MaxLength(16)]
        public required string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(16)]
        public required string Password { get; set; }
    }
}
