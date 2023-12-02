using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.Entities
{
    public class ReadingList : BaseEntity
    {
        [Required]
        public required User User { get; set; }

        [Required]
        public required string Name { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; } = string.Empty;

        [Required]
        public required bool IsPublic { get; set; } = false;

        [Required]
        public List<Title> Titles { get; set; } = [];
    }
}
