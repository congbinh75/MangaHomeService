using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class UpdateGroup
    {
        [Required]
        public required string Id;

        [MaxLength(128)]
        public string? Name;

        [MaxLength(512)]
        public string? Description;

        public IFormFile? ProfilePicture;

        public ICollection<string>? MembersIds;
    }
}
