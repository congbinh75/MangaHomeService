using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class ChangePassword
    {
        [Required]
        public required string OldPassword { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(16)]
        public required string NewPassword { get; set; }
    }
}
