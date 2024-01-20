using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class CreateGroup
    {
        [Required]
        public required string Name { get; set; } = string.Empty;

        [MaxLength(512)]
        public string? Description { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        public ICollection<string>? MembersIds { get; set; }
    }
}
