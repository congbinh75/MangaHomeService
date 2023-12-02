using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class UpdateRoleOfUser
    {
        [Required]
        public required string UserId { get; set; }

        [Required]
        [Range(0, 2)]
        public required string Role { get; set; }
    }
}
